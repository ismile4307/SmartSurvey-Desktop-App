using DBI_Scripting.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace DBI_Scripting.Forms.Scripting
{
    public partial class FrmDummyData : Window
    {
        // ── Project info ──────────────────────────────────────────────────────────
        private string sProjectId      = "";
        private string sProjectName    = "";
        private string sScriptVersion  = "";
        private string sOutputFolder   = "";

        // ── Live DB connections (open for the full generation run) ────────────────
        private SQLiteConnection qConn;   // script / questionnaire DB  (read)
        private SQLiteConnection aConn;   // answer DB                  (write)

        // ── Shared Random instance ────────────────────────────────────────────────
        private readonly Random rng = new Random();

        // ── Cached question list (loaded once per run) ────────────────────────────
        private List<DataRow> allQuestions;

        public FrmDummyData()
        {
            InitializeComponent();
        }

        // ═════════════════════════════════════════════════════════════════════════
        //  UI EVENT HANDLERS
        // ═════════════════════════════════════════════════════════════════════════

        private void btnExit_Click(object sender, RoutedEventArgs e) => Close();

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                InitialDirectory = Properties.Settings.Default.StartupPath,
                Filter = "Script Database (*.db)|*.db|All Files (*.*)|*.*"
            };
            if (dlg.ShowDialog() != true) return;

            txtScriptPath.Text  = dlg.FileName;
            string folder       = Path.GetDirectoryName(dlg.FileName);
            txtOutputFolder.Text = Path.Combine(folder, "DummyData");
            sOutputFolder        = txtOutputFolder.Text;

            StaticClass.QDBPath = dlg.FileName;
            Properties.Settings.Default.StartupPath = folder;
            Properties.Settings.Default.Save();

            LoadScriptInfo();
        }

        private void btnBrowseOutput_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog
            {
                Description  = "Select output folder for answer database",
                SelectedPath = sOutputFolder
            };
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                txtOutputFolder.Text = dlg.SelectedPath;
                sOutputFolder        = dlg.SelectedPath;
            }
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            // ── Validate ──────────────────────────────────────────────────────────
            if (string.IsNullOrWhiteSpace(txtScriptPath.Text))
                { MessageBox.Show("Please select a script database.", "Validation"); return; }
            if (!File.Exists(txtScriptPath.Text))
                { MessageBox.Show("Script file not found.", "Validation"); return; }
            if (!int.TryParse(txtNumberOfData.Text, out int noOfRecords) || noOfRecords <= 0)
                { MessageBox.Show("Enter a valid number of records.", "Validation"); return; }

            sOutputFolder = txtOutputFolder.Text.Trim();
            if (string.IsNullOrWhiteSpace(sOutputFolder))
                { MessageBox.Show("Please specify an output folder.", "Validation"); return; }

            try
            {
                btnGenerate.IsEnabled = false;
                txtLog.Clear();
                progressBar.Value   = 0;
                progressBar.Maximum = noOfRecords;
                lblStatus.Content   = "Initialising…";

                // ── Prepare answer DB ─────────────────────────────────────────────
                string ansDbPath = PrepareAnswerDB();
                StaticClass.ADBPath = ansDbPath;

                // ── Open connections ──────────────────────────────────────────────
                qConn = new SQLiteConnection("Data Source=" + txtScriptPath.Text);
                qConn.Open();

                aConn = new SQLiteConnection("Data Source=" + ansDbPath);
                aConn.Open();

                // ── Cache all questions once ──────────────────────────────────────
                allQuestions = LoadAllQuestions();

                Log($"Project  : {sProjectName}  v{sScriptVersion}");
                Log($"Questions: {allQuestions.Count}   Records to generate: {noOfRecords}");
                Log($"Output   : {ansDbPath}");
                Log(new string('─', 72));

                int totalAnswers = 0;

                for (int i = 0; i < noOfRecords; i++)
                {
                    string respondentId = BuildRespondentId(i);
                    string startTime    = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                    InsertInterviewInfo(respondentId, startTime);
                    InsertIntvSettings(respondentId);
                    int saved = WalkQuestionnaire(respondentId, startTime);
                    UpdateInterviewEndTime(respondentId);

                    totalAnswers += saved;

                    progressBar.Value   = i + 1;
                    txtProgress.Text    = $"{i + 1}/{noOfRecords}";
                    lblStatus.Content   = $"Respondent {i + 1}/{noOfRecords} — {saved} answers saved.";
                    Log($"[{i + 1:D4}] RespondentId={respondentId}  Answers={saved}");

                    // Allow UI to repaint
                    Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { }));
                }

                Log(new string('─', 72));
                Log($"Done.  {noOfRecords} respondents  |  {totalAnswers} answer rows.");

                // ── Export to Excel ───────────────────────────────────────────────
                lblStatus.Content = "Exporting to Excel…";
                Dispatcher.Invoke(DispatcherPriority.Background, new Action(() => { }));
                string xlsxPath = ExportToExcel(ansDbPath);

                qConn.Close();
                aConn.Close();

                Log($"Excel  : {xlsxPath}");
                Log(new string('─', 72));
                lblStatus.Content = $"Complete — {noOfRecords} records, {totalAnswers} answers.";

                MessageBox.Show(
                    $"Successfully generated {noOfRecords} dummy records.\n\n" +
                    $"Total answer rows : {totalAnswers}\n" +
                    $"Saved DB to       : {ansDbPath}\n" +
                    $"Saved Excel to    : {xlsxPath}",
                    "Done", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                Log($"ERROR: {ex.Message}");
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                if (qConn?.State == ConnectionState.Open) qConn.Close();
                if (aConn?.State == ConnectionState.Open) aConn.Close();
                btnGenerate.IsEnabled = true;
            }
        }

        // ═════════════════════════════════════════════════════════════════════════
        //  SCRIPT INFO
        // ═════════════════════════════════════════════════════════════════════════

        private void LoadScriptInfo()
        {
            if (!File.Exists(StaticClass.QDBPath)) return;
            try
            {
                using (var conn = new SQLiteConnection("Data Source=" + StaticClass.QDBPath))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(
                        "SELECT ProjectId, ProjectName, Version FROM T_ProjectInfo LIMIT 1", conn))
                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            sProjectId      = rdr["ProjectId"].ToString();
                            sProjectName    = rdr["ProjectName"].ToString();
                            sScriptVersion  = rdr["Version"].ToString();
                            txtProjectName.Text  = sProjectName;
                            txtScriptVersion.Text = sScriptVersion;
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        // ═════════════════════════════════════════════════════════════════════════
        //  ANSWER DB MANAGEMENT
        // ═════════════════════════════════════════════════════════════════════════

        private string PrepareAnswerDB()
        {
            if (!Directory.Exists(sOutputFolder))
                Directory.CreateDirectory(sOutputFolder);

            string template = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "ShellDB", "SYSACDB.db");
            if (!File.Exists(template))
                throw new FileNotFoundException($"Answer DB template not found:\n{template}");

            string stamp   = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string outPath = Path.Combine(sOutputFolder, $"DummyData_{stamp}.db");
            File.Copy(template, outPath, overwrite: true);
            return outPath;
        }

        // ═════════════════════════════════════════════════════════════════════════
        //  INTERVIEW INFO
        // ═════════════════════════════════════════════════════════════════════════

        private string BuildRespondentId(int index)
        {
            // Unique: timestamp + zero-padded index
            return DateTime.Now.ToString("yyyyMMddHHmmss") + index.ToString("D4");
        }

        private void InsertInterviewInfo(string respondentId, string startTime)
        {
            string[] fiPool = { "FI001", "FI002", "FI003", "FI004", "FI005" };
            string[] fsPool = { "FS001", "FS002", "FS003" };

            var cmd = new SQLiteCommand(aConn)
            {
                CommandText =
                    "INSERT INTO T_InterviewInfo " +
                    "(ProjectId,RespondentId,Latitude,Longitude,SurveyDateTime,SurveyEndTime," +
                    " LengthOfIntv,Intv_Type,FICode,FSCode,AccompaniedBy,BackCheckedBy," +
                    " Status,TabId,SyncStatus,ScriptVersion,LanguageId,FieldExtra1,FieldExtra2) " +
                    "VALUES " +
                    "(@PId,@RId,@Lat,@Lon,@Start,@End," +
                    " @LOI,@IType,@FI,@FS,@Acc,@BC," +
                    " @Stat,@Tab,@Sync,@Ver,@Lang,@FE1,@FE2)"
            };
            cmd.Parameters.AddWithValue("@PId",  sProjectId);
            cmd.Parameters.AddWithValue("@RId",  respondentId);
            cmd.Parameters.AddWithValue("@Lat",  RandLat().ToString("F6"));
            cmd.Parameters.AddWithValue("@Lon",  RandLon().ToString("F6"));
            cmd.Parameters.AddWithValue("@Start", startTime);
            cmd.Parameters.AddWithValue("@End",   startTime);
            cmd.Parameters.AddWithValue("@LOI",  rng.Next(15, 50).ToString());
            cmd.Parameters.AddWithValue("@IType", "1");
            cmd.Parameters.AddWithValue("@FI",   fiPool[rng.Next(fiPool.Length)]);
            cmd.Parameters.AddWithValue("@FS",   fsPool[rng.Next(fsPool.Length)]);
            cmd.Parameters.AddWithValue("@Acc",  "");
            cmd.Parameters.AddWithValue("@BC",   "");
            cmd.Parameters.AddWithValue("@Stat", "2");
            cmd.Parameters.AddWithValue("@Tab",  "DUMMY");
            cmd.Parameters.AddWithValue("@Sync", "0");
            cmd.Parameters.AddWithValue("@Ver",  sScriptVersion);
            cmd.Parameters.AddWithValue("@Lang", "1");
            cmd.Parameters.AddWithValue("@FE1",  "");
            cmd.Parameters.AddWithValue("@FE2",  "");
            cmd.ExecuteNonQuery();
        }

        private void UpdateInterviewEndTime(string respondentId)
        {
            var cmd = new SQLiteCommand(
                "UPDATE T_InterviewInfo SET SurveyEndTime=@T WHERE RespondentId=@R", aConn);
            cmd.Parameters.AddWithValue("@T", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            cmd.Parameters.AddWithValue("@R", respondentId);
            cmd.ExecuteNonQuery();
        }

        private void InsertIntvSettings(string respondentId)
        {
            string[] localities = { "Dhaka North", "Dhaka South", "Chittagong", "Sylhet", "Rajshahi",
                                    "Khulna", "Barisal", "Rangpur", "Mymensingh", "Comilla" };

            var cmd = new SQLiteCommand(aConn)
            {
                CommandText =
                    "INSERT INTO T_IntvSettings " +
                    "(ProjectId,RespondentId,QId,LocalityName,SegmentNo,MappersId," +
                    " Segment1,Segment2,Segment3,Segment4,Segment5," +
                    " Segment6,Segment7,Segment8,Segment9,Segment10," +
                    " RNumber,SelectedSeg) " +
                    "VALUES " +
                    "(@PId,@RId,@QId,@Loc,@Seg,@Map," +
                    " @S1,@S2,@S3,@S4,@S5,@S6,@S7,@S8,@S9,@S10," +
                    " @RNo,@SSeg)"
            };
            cmd.Parameters.AddWithValue("@PId",  sProjectId);
            cmd.Parameters.AddWithValue("@RId",  respondentId);
            cmd.Parameters.AddWithValue("@QId",  "");
            cmd.Parameters.AddWithValue("@Loc",  localities[rng.Next(localities.Length)]);
            cmd.Parameters.AddWithValue("@Seg",  rng.Next(1, 11).ToString());
            cmd.Parameters.AddWithValue("@Map",  $"MP{rng.Next(1, 10):D3}");
            for (int s = 1; s <= 10; s++)
                cmd.Parameters.AddWithValue($"@S{s}", rng.Next(1, 51).ToString());
            cmd.Parameters.AddWithValue("@RNo",  rng.Next(1, 100).ToString());
            cmd.Parameters.AddWithValue("@SSeg", rng.Next(1, 11).ToString());
            cmd.ExecuteNonQuery();
        }

        // ═════════════════════════════════════════════════════════════════════════
        //  QUESTION WALKER
        // ═════════════════════════════════════════════════════════════════════════

        private int WalkQuestionnaire(string respondentId, string dateTime)
        {
            if (allQuestions == null || allQuestions.Count == 0) return 0;

            bool followRouting = chkFollowRouting.IsChecked == true;

            int maxOrder     = allQuestions.Max(q => ToInt(q["OrderTag1"]));
            int currentIndex = 1;
            int answerCount  = 0;
            int safetyLimit  = maxOrder + 200;
            int iterations   = 0;

            // Without routing: single transaction per respondent (fast)
            // With routing   : autocommit so CheckCondition can read prior answers
            SQLiteTransaction tx = followRouting ? null : aConn.BeginTransaction();

            try
            {
                while (currentIndex <= maxOrder && iterations < safetyLimit)
                {
                    iterations++;

                    var q = allQuestions.FirstOrDefault(r => ToInt(r["OrderTag1"]) == currentIndex);
                    if (q == null) { currentIndex++; continue; }

                    string qType = q["QType"].ToString();
                    string qId   = q["QId"].ToString();

                    // ── STOP ─────────────────────────────────────────────────────
                    if (qType == "50" || qType == "51") break;

                    // ── SKIP (no answer collected) ────────────────────────────────
                    if (qType == "6" || qType == "9" || qType == "11" ||
                        qType == "16" || qType == "49")
                    { currentIndex++; continue; }

                    // ── Generate & save answer ────────────────────────────────────
                    var attrs = GetAttributes(qId);
                    int saved = GenerateAndSaveAnswer(q, currentIndex, respondentId, dateTime, attrs);
                    answerCount += saved;
                    if (saved > 0)
                        WriteRespAnsLog(respondentId, qId,
                            rng.Next(2, 25).ToString(), dateTime);

                    // ── Routing ───────────────────────────────────────────────────
                    if (followRouting)
                    {
                        int jump = EvaluateJumpLogic(qId, respondentId);
                        currentIndex = (jump > 0 && jump != currentIndex) ? jump : currentIndex + 1;
                    }
                    else
                    {
                        currentIndex++;
                    }
                }

                tx?.Commit();
            }
            catch
            {
                tx?.Rollback();
                throw;
            }

            return answerCount;
        }

        // ═════════════════════════════════════════════════════════════════════════
        //  ANSWER DISPATCHER
        // ═════════════════════════════════════════════════════════════════════════

        private int GenerateAndSaveAnswer(DataRow q, int orderIdx,
            string respondentId, string dateTime, List<DataRow> attrs)
        {
            string qId   = q["QId"].ToString();
            string qType = q["QType"].ToString();

            switch (qType)
            {
                case "1":  case "24": case "61":
                    return HandleSingleResponse(qId, orderIdx, respondentId, dateTime, attrs);

                case "2":
                    return HandleMultipleResponse(q, qId, orderIdx, respondentId, dateTime, attrs);

                case "3":  case "18":
                    return HandleOEString(qId, orderIdx, respondentId, dateTime, attrs);

                case "4":  case "19":
                    return HandleOENumber(q, qId, orderIdx, respondentId, dateTime, attrs);

                case "5":  case "26":
                    return HandleRank(q, qId, orderIdx, respondentId, dateTime, attrs);

                case "7":
                    return HandleGridOption(qId, orderIdx, respondentId, dateTime, attrs);

                case "8":
                    return HandleGridCheckBox(qId, orderIdx, respondentId, dateTime, attrs);

                case "10":
                    return HandleSoundRecorder(qId, orderIdx, respondentId, dateTime);

                case "12":
                    return HandleListOEString(qId, orderIdx, respondentId, dateTime, attrs);

                case "13":
                    return HandleListOENumber(qId, orderIdx, respondentId, dateTime, attrs);

                case "14":
                    return HandleDateControl(qId, orderIdx, respondentId, dateTime);

                case "15":
                    return HandleTimeControl(qId, orderIdx, respondentId, dateTime);

                case "17":
                    return HandleListOENumberWithTotal(qId, orderIdx, respondentId, dateTime, attrs);

                case "20": case "21": case "48": case "60":
                    return HandleFormQuestion(qId, orderIdx, respondentId, dateTime, attrs);

                case "22": case "23":
                    return HandleAutoSuggestion(qId, orderIdx, respondentId, dateTime);

                case "25":
                    return HandleSlider(qId, orderIdx, respondentId, dateTime, attrs);

                case "27":
                    return HandleGridNumber(qId, orderIdx, respondentId, dateTime, attrs);

                case "31":
                    return HandleScaleGrid(qId, orderIdx, respondentId, dateTime, attrs, 5);

                case "32":
                    return HandleScaleGrid(qId, orderIdx, respondentId, dateTime, attrs, 7);

                case "33":
                    return HandleScaleGrid(qId, orderIdx, respondentId, dateTime, attrs, 10);

                case "40":
                    return HandleMaxDiff(q, qId, orderIdx, respondentId, dateTime, attrs);

                case "41":
                    return HandleGPS(qId, orderIdx, respondentId, dateTime);

                default:
                    return 0;   // unknown / future type — skip silently
            }
        }

        // ═════════════════════════════════════════════════════════════════════════
        //  QTYPE HANDLERS
        // ═════════════════════════════════════════════════════════════════════════

        // ── QType 1 | 24 | 61  Single Response / Spinner / Scale10 SR ───────────
        private int HandleSingleResponse(string qId, int order,
            string rId, string dt, List<DataRow> attrs)
        {
            if (attrs.Count == 0) return 0;
            var pick = attrs[rng.Next(attrs.Count)];
            WriteAnswer(rId, qId, pick["AttributeValue"].ToString(), dt, order, 1);
            return 1;
        }

        // ── QType 2  Multiple Response ────────────────────────────────────────────
        private int HandleMultipleResponse(DataRow q, string qId, int order,
            string rId, string dt, List<DataRow> attrs)
        {
            if (attrs.Count == 0) return 0;

            int minR = ParseInt(q["NoOfResponseMin"], 1);
            int maxR = ParseInt(q["NoOfResponseMax"], attrs.Count);
            maxR = Math.Min(maxR, attrs.Count);
            if (maxR < minR) maxR = minR;
            int pickCount = rng.Next(minR, maxR + 1);

            var exclusive    = attrs.Where(a => a["IsExclusive"].ToString() == "1").ToList();
            var nonExclusive = attrs.Where(a => a["IsExclusive"].ToString() != "1").ToList();

            List<DataRow> selected;
            if (exclusive.Count > 0 && nonExclusive.Count > 0 && rng.Next(5) == 0)
                selected = new List<DataRow> { exclusive[rng.Next(exclusive.Count)] };
            else if (nonExclusive.Count > 0)
                selected = nonExclusive.OrderBy(_ => rng.Next()).Take(pickCount).ToList();
            else
                selected = attrs.OrderBy(_ => rng.Next()).Take(pickCount).ToList();

            for (int i = 0; i < selected.Count; i++)
                WriteAnswer(rId, qId, selected[i]["AttributeValue"].ToString(), dt, order, i + 1);

            return selected.Count;
        }

        // ── QType 3 | 18  Open-Ended String (with or without DK/CS) ─────────────
        private int HandleOEString(string qId, int order,
            string rId, string dt, List<DataRow> attrs)
        {
            var dkAttr = attrs.FirstOrDefault(a => a["IsExclusive"].ToString() == "1");
            var oeAttr = attrs.FirstOrDefault(a => a["IsExclusive"].ToString() != "1");

            // 30 % chance to choose DK when available
            if (dkAttr != null && rng.Next(10) < 3)
            {
                WriteAnswer(rId, qId, dkAttr["AttributeValue"].ToString(), dt, order, 1);
                return 1;
            }

            string attrVal = oeAttr != null ? oeAttr["AttributeValue"].ToString() : "1";
            WriteAnswer(rId, qId, attrVal, dt, order, 1);
            WriteOpenEnded(rId, qId, attrVal, $"Sample OE response for {qId}", "1");
            return 1;
        }

        // ── QType 4 | 19  Open-Ended Number (with or without DK/CS) ─────────────
        private int HandleOENumber(DataRow q, string qId, int order,
            string rId, string dt, List<DataRow> attrs)
        {
            var dkAttr  = attrs.FirstOrDefault(a => a["IsExclusive"].ToString() == "1");
            var numAttr = attrs.FirstOrDefault(a => a["IsExclusive"].ToString() != "1");

            // 20 % DK
            if (dkAttr != null && rng.Next(10) < 2)
            {
                WriteAnswer(rId, qId, dkAttr["AttributeValue"].ToString(), dt, order, 1);
                return 1;
            }

            int minVal = 1, maxVal = 99;
            if (numAttr != null)
            {
                minVal = ParseInt(numAttr["MinValue"], minVal);
                maxVal = ParseInt(numAttr["MaxValue"], maxVal);
            }
            else
            {
                minVal = ParseInt(q["NoOfResponseMin"], minVal);
                maxVal = ParseInt(q["NoOfResponseMax"], maxVal);
            }
            if (maxVal < minVal) maxVal = minVal + 10;

            WriteAnswer(rId, qId, rng.Next(minVal, maxVal + 1).ToString(), dt, order, 1);
            return 1;
        }

        // ── QType 5 | 26  Rank / Drag-Drop ───────────────────────────────────────
        private int HandleRank(DataRow q, string qId, int order,
            string rId, string dt, List<DataRow> attrs)
        {
            if (attrs.Count == 0) return 0;

            int maxRank = ParseInt(q["NoOfResponseMax"], attrs.Count);
            maxRank = Math.Min(Math.Max(maxRank, 1), attrs.Count);

            var shuffled = attrs.OrderBy(_ => rng.Next()).Take(maxRank).ToList();
            for (int i = 0; i < shuffled.Count; i++)
                WriteAnswer(rId, qId, shuffled[i]["AttributeValue"].ToString(), dt, order, i + 1);

            return shuffled.Count;
        }

        // ── QType 7  GridOption (single-select per row) ───────────────────────────
        private int HandleGridOption(string qId, int order,
            string rId, string dt, List<DataRow> attrs)
        {
            int count = 0;
            foreach (var row in attrs)
            {
                var cols = GridOptions(row["LinkId2"].ToString());
                if (cols.Count == 0) continue;
                string subQId = $"{qId}_{row["AttributeValue"]}";
                WriteAnswer(rId, subQId, cols[rng.Next(cols.Count)]["AttributeValue"].ToString(),
                            dt, order, 1);
                count++;
            }
            return count;
        }

        // ── QType 8  GridCheckBox (multi-select per row) ──────────────────────────
        private int HandleGridCheckBox(string qId, int order,
            string rId, string dt, List<DataRow> attrs)
        {
            int count = 0;
            foreach (var row in attrs)
            {
                var cols = GridOptions(row["LinkId2"].ToString());
                if (cols.Count == 0) continue;
                int pickCount = rng.Next(1, cols.Count + 1);
                var picked = cols.OrderBy(_ => rng.Next()).Take(pickCount).ToList();
                string subQId = $"{qId}_{row["AttributeValue"]}";
                for (int i = 0; i < picked.Count; i++)
                    WriteAnswer(rId, subQId, picked[i]["AttributeValue"].ToString(), dt, order, i + 1);
                count += picked.Count;
            }
            return count;
        }

        // ── QType 10  SoundRecorder ───────────────────────────────────────────────
        private int HandleSoundRecorder(string qId, int order, string rId, string dt)
        {
            WriteAnswer(rId, qId, "dummy_audio.m4a", dt, order, 1);
            return 1;
        }

        // ── QType 12  List OE String — 1 row per attribute in T_RespAnswer ────────
        private int HandleListOEString(string qId, int order,
            string rId, string dt, List<DataRow> attrs)
        {
            int count = 0;
            foreach (var attr in attrs)
            {
                int attrOrd = ParseInt(attr["AttributeOrder"], count + 1);
                string text = $"Sample text for {qId} option {attr["AttributeValue"]}";
                WriteAnswer(rId, qId, text, dt, order, attrOrd);
                count++;
            }
            return count;
        }

        // ── QType 13  List OE Number ──────────────────────────────────────────────
        private int HandleListOENumber(string qId, int order,
            string rId, string dt, List<DataRow> attrs)
        {
            int count = 0;
            foreach (var attr in attrs)
            {
                int minVal  = ParseInt(attr["MinValue"], 0);
                int maxVal  = ParseInt(attr["MaxValue"], 99);
                if (maxVal < minVal) maxVal = minVal + 10;
                int attrOrd = ParseInt(attr["AttributeOrder"], count + 1);
                WriteAnswer(rId, qId, rng.Next(minVal, maxVal + 1).ToString(), dt, order, attrOrd);
                count++;
            }
            return count;
        }

        // ── QType 14  DateControl ─────────────────────────────────────────────────
        private int HandleDateControl(string qId, int order, string rId, string dt)
        {
            var date = DateTime.Now.AddDays(-rng.Next(1, 730));
            WriteAnswer(rId, qId, date.ToString("yyyy-MM-dd"), dt, order, 1);
            return 1;
        }

        // ── QType 15  TimeControl ─────────────────────────────────────────────────
        private int HandleTimeControl(string qId, int order, string rId, string dt)
        {
            WriteAnswer(rId, qId, $"{rng.Next(8, 20):D2}:{rng.Next(0, 60):D2}", dt, order, 1);
            return 1;
        }

        // ── QType 17  List OE Number With Total ───────────────────────────────────
        private int HandleListOENumberWithTotal(string qId, int order,
            string rId, string dt, List<DataRow> attrs)
        {
            var regular  = attrs.Where(a => a["IsExclusive"].ToString() != "1").ToList();
            var totalRow = attrs.FirstOrDefault(a => a["IsExclusive"].ToString() == "1");

            int total = 0, count = 0;
            foreach (var attr in regular)
            {
                int minVal  = ParseInt(attr["MinValue"], 0);
                int maxVal  = ParseInt(attr["MaxValue"], 5);
                if (maxVal < minVal) maxVal = minVal + 5;
                int val     = rng.Next(minVal, maxVal + 1);
                int attrOrd = ParseInt(attr["AttributeOrder"], count + 1);
                WriteAnswer(rId, qId, val.ToString(), dt, order, attrOrd);
                total += val;
                count++;
            }
            if (totalRow != null)
            {
                int attrOrd = ParseInt(totalRow["AttributeOrder"], count + 1);
                WriteAnswer(rId, qId, total.ToString(), dt, order, attrOrd);
                count++;
            }
            return count;
        }

        // ── QType 20 | 21 | 48 | 60  Form / Member Info / Kids / FIFS Info ───────
        //  Each attribute = sub-field; LinkId1 = sub-QType (1/3/4); LinkId2 = GridInfo set
        private int HandleFormQuestion(string qId, int order,
            string rId, string dt, List<DataRow> attrs)
        {
            int count = 0;
            foreach (var attr in attrs)
            {
                string attrVal = attr["AttributeValue"].ToString();
                string linkId1 = attr["LinkId1"].ToString();
                string linkId2 = attr["LinkId2"].ToString();
                string subQId  = $"{qId}_{attrVal}";
                string response;

                switch (linkId1)
                {
                    case "1":   // Single — pick from T_GridInfo options
                        var opts = GridOptions(linkId2);
                        response = opts.Count > 0
                            ? opts[rng.Next(opts.Count)]["AttributeValue"].ToString()
                            : "1";
                        break;

                    case "3":   // OE String
                        response = $"Sample_{qId}_{attrVal}";
                        break;

                    case "4":   // OE Number
                        int minV = ParseInt(attr["MinValue"], 1);
                        int maxV = ParseInt(attr["MaxValue"], 99);
                        if (maxV < minV) maxV = minV + 10;
                        response = rng.Next(minV, maxV + 1).ToString();
                        break;

                    default:
                        response = $"Value_{attrVal}";
                        break;
                }

                WriteAnswer(rId, subQId, response, dt, order, 1);
                count++;
            }
            return count;
        }

        // ── QType 22 | 23  Auto-Suggestion / Auto-Suggestion from Response ────────
        //  Text response saved in T_RespAnswer
        private int HandleAutoSuggestion(string qId, int order, string rId, string dt)
        {
            WriteAnswer(rId, qId, $"AutoSug_{qId}_{rng.Next(100, 999)}", dt, order, 1);
            return 1;
        }

        // ── QType 25  Slider ──────────────────────────────────────────────────────
        private int HandleSlider(string qId, int order,
            string rId, string dt, List<DataRow> attrs)
        {
            int minVal = 0, maxVal = 100;
            if (attrs.Count > 0)
            {
                minVal = ParseInt(attrs[0]["MinValue"], minVal);
                maxVal = ParseInt(attrs[0]["MaxValue"], maxVal);
            }
            if (maxVal < minVal) maxVal = minVal + 100;
            WriteAnswer(rId, qId, rng.Next(minVal, maxVal + 1).ToString(), dt, order, 1);
            return 1;
        }

        // ── QType 27  GridNumber ──────────────────────────────────────────────────
        private int HandleGridNumber(string qId, int order,
            string rId, string dt, List<DataRow> attrs)
        {
            int count = 0;
            foreach (var attr in attrs)
            {
                int minVal = ParseInt(attr["MinValue"], 0);
                int maxVal = ParseInt(attr["MaxValue"], 100);
                if (maxVal < minVal) maxVal = minVal + 10;
                string subQId = $"{qId}_{attr["AttributeValue"]}";
                WriteAnswer(rId, subQId, rng.Next(minVal, maxVal + 1).ToString(), dt, order, 1);
                count++;
            }
            return count;
        }

        // ── QType 31 | 32 | 33  Scale Grid (5 / 7 / 10 points) ──────────────────
        private int HandleScaleGrid(string qId, int order,
            string rId, string dt, List<DataRow> attrs, int scaleMax)
        {
            int count = 0;
            foreach (var attr in attrs)
            {
                string subQId = $"{qId}_{attr["AttributeValue"]}";
                WriteAnswer(rId, subQId, rng.Next(1, scaleMax + 1).ToString(), dt, order, 1);
                count++;
            }
            return count;
        }

        // ── QType 40  MaxDiff (Best-Worst Scaling) ────────────────────────────────
        private int HandleMaxDiff(DataRow q, string qId, int order,
            string rId, string dt, List<DataRow> attrs)
        {
            if (attrs.Count < 2) return 0;

            int rounds = ParseInt(q["NoOfResponseMax"], Math.Max(1, attrs.Count / 4));
            if (rounds <= 0) rounds = 1;

            int count = 0;
            for (int r = 1; r <= rounds; r++)
            {
                // Take a random subset of ≥2 items per round
                int setSize = Math.Min(attrs.Count, Math.Max(2, rng.Next(3, 6)));
                var subset  = attrs.OrderBy(_ => rng.Next()).Take(setSize).ToList();

                // Best  → rOrderTag = 1
                WriteAnswer(rId, qId, subset[0]["AttributeValue"].ToString(), dt, r, 1);
                // Worst → rOrderTag = 2
                WriteAnswer(rId, qId, subset[1]["AttributeValue"].ToString(), dt, r, 2);
                count += 2;
            }
            return count;
        }

        // ── QType 41  GetGPS ──────────────────────────────────────────────────────
        private int HandleGPS(string qId, int order, string rId, string dt)
        {
            // Bangladesh bounding box: Lat 20.5–26.6 | Lon 88.0–92.7
            double lat = 20.5 + rng.NextDouble() * 6.1;
            double lon = 88.0 + rng.NextDouble() * 4.7;
            WriteAnswer(rId, qId, $"{lat:F6}", dt, order, 1);   // Latitude
            WriteAnswer(rId, qId, $"{lon:F6}", dt, order, 2);   // Longitude
            return 2;
        }

        // ═════════════════════════════════════════════════════════════════════════
        //  ROUTING LOGIC
        // ═════════════════════════════════════════════════════════════════════════

        private int EvaluateJumpLogic(string qId, string respondentId)
        {
            try
            {
                StaticClass.QDBPath = txtScriptPath.Text;
                // StaticClass.ADBPath already set to ansDbPath in btnGenerate_Click

                var connQ = new ConnectionDB();
                var connA = new ConnectionDB();

                DataTable dt = new DBHelper().getQntrTableData(
                    $"SELECT * FROM T_LogicTable WHERE QId='{qId}' AND LogicTypeId='3'", connQ);

                foreach (DataRow dr in dt.Rows)
                {
                    string condition = dr["IfCondition"].ToString();
                    string thenQId   = dr["Then"].ToString();
                    string elseQId   = dr["Else"].ToString();

                    bool matched = new CheckCondition(connA, connQ)
                        .convetToPostFixNotationAndExecute(sProjectId, respondentId, qId, condition);

                    string targetQId = matched ? thenQId : elseQId;
                    if (string.IsNullOrEmpty(targetQId)) continue;

                    DataTable dtQ = new DBHelper().getQntrTableData(
                        $"SELECT OrderTag1 FROM T_Question WHERE QId='{targetQId}'", connQ);
                    if (dtQ.Rows.Count > 0)
                        return ToInt(dtQ.Rows[0]["OrderTag1"]);
                }
            }
            catch { /* routing failure — proceed sequentially */ }

            return 0;
        }

        // ═════════════════════════════════════════════════════════════════════════
        //  DB QUERY HELPERS  (use open qConn / aConn directly)
        // ═════════════════════════════════════════════════════════════════════════

        private List<DataRow> LoadAllQuestions()
        {
            var cmd = new SQLiteCommand(
                "SELECT * FROM T_Question WHERE ProjectId=@P ORDER BY CAST(OrderTag1 AS INTEGER)",
                qConn);
            cmd.Parameters.AddWithValue("@P", sProjectId);
            var ds = new DataSet();
            new SQLiteDataAdapter(cmd).Fill(ds, "Q");
            return ds.Tables["Q"].Rows.Cast<DataRow>().ToList();
        }

        private List<DataRow> GetAttributes(string qId)
        {
            var cmd = new SQLiteCommand(
                "SELECT * FROM T_OptAttribute WHERE ProjectId=@P AND QId=@Q " +
                "ORDER BY CAST(AttributeOrder AS INTEGER)",
                qConn);
            cmd.Parameters.AddWithValue("@P", sProjectId);
            cmd.Parameters.AddWithValue("@Q", qId);
            var ds = new DataSet();
            new SQLiteDataAdapter(cmd).Fill(ds, "A");
            return ds.Tables["A"].Rows.Cast<DataRow>().ToList();
        }

        private List<DataRow> GridOptions(string gridSetId)
        {
            if (string.IsNullOrEmpty(gridSetId)) return new List<DataRow>();
            var cmd = new SQLiteCommand(
                "SELECT * FROM T_GridInfo WHERE ProjectId=@P AND QId=@Q " +
                "ORDER BY CAST(AttributeOrder AS INTEGER)",
                qConn);
            cmd.Parameters.AddWithValue("@P", sProjectId);
            cmd.Parameters.AddWithValue("@Q", gridSetId);
            var ds = new DataSet();
            new SQLiteDataAdapter(cmd).Fill(ds, "G");
            return ds.Tables["G"].Rows.Cast<DataRow>().ToList();
        }

        // ═════════════════════════════════════════════════════════════════════════
        //  DB WRITE HELPERS
        // ═════════════════════════════════════════════════════════════════════════

        private void WriteAnswer(string rId, string qId, string response,
            string dateTime, int qOrderTag, int rOrderTag)
        {
            var cmd = new SQLiteCommand(aConn)
            {
                CommandText =
                    "INSERT INTO T_RespAnswer " +
                    "(ProjectId,RespondentId,QId,Response,ResponseDateTime," +
                    " qElapsedTime,qOrderTag,rOrderTag) " +
                    "VALUES (@PId,@RId,@QId,@Resp,@DT,@Elapsed,@QOrd,@ROrd)"
            };
            cmd.Parameters.AddWithValue("@PId",     sProjectId);
            cmd.Parameters.AddWithValue("@RId",     rId);
            cmd.Parameters.AddWithValue("@QId",     qId);
            cmd.Parameters.AddWithValue("@Resp",    response);
            cmd.Parameters.AddWithValue("@DT",      dateTime);
            cmd.Parameters.AddWithValue("@Elapsed", rng.Next(2, 25).ToString());
            cmd.Parameters.AddWithValue("@QOrd",    qOrderTag);
            cmd.Parameters.AddWithValue("@ROrd",    rOrderTag);
            cmd.ExecuteNonQuery();
        }

        private void WriteOpenEnded(string rId, string qId,
            string attributeValue, string oeText, string oeType)
        {
            var cmd = new SQLiteCommand(aConn)
            {
                CommandText =
                    "INSERT INTO T_RespOpenended " +
                    "(ProjectId,RespondentId,QId,AttributeValue,OpenendedResp,OEResponseType) " +
                    "VALUES (@PId,@RId,@QId,@AV,@OE,@OET)"
            };
            cmd.Parameters.AddWithValue("@PId", sProjectId);
            cmd.Parameters.AddWithValue("@RId", rId);
            cmd.Parameters.AddWithValue("@QId", qId);
            cmd.Parameters.AddWithValue("@AV",  attributeValue);
            cmd.Parameters.AddWithValue("@OE",  oeText);
            cmd.Parameters.AddWithValue("@OET", oeType);
            cmd.ExecuteNonQuery();
        }

        private void WriteRespAnsLog(string rId, string qId, string elapsed, string dateTime)
        {
            var cmd = new SQLiteCommand(aConn)
            {
                CommandText =
                    "INSERT INTO T_RespAnsLog " +
                    "(ProjectId,RespondentId,QId,qElapsedTime,ResponseDateTime) " +
                    "VALUES (@PId,@RId,@QId,@Elapsed,@DT)"
            };
            cmd.Parameters.AddWithValue("@PId",     sProjectId);
            cmd.Parameters.AddWithValue("@RId",     rId);
            cmd.Parameters.AddWithValue("@QId",     qId);
            cmd.Parameters.AddWithValue("@Elapsed", elapsed);
            cmd.Parameters.AddWithValue("@DT",      dateTime);
            cmd.ExecuteNonQuery();
        }

        // ═════════════════════════════════════════════════════════════════════════
        //  UTILITY HELPERS
        // ═════════════════════════════════════════════════════════════════════════

        private static int ToInt(object val, int fallback = 0)
        {
            if (val == null || val == DBNull.Value) return fallback;
            return int.TryParse(val.ToString(), out int n) ? n : fallback;
        }

        private static int ParseInt(object val, int fallback)
        {
            if (val == null || val == DBNull.Value) return fallback;
            string s = val.ToString().Trim();
            if (string.IsNullOrEmpty(s)) return fallback;
            return int.TryParse(s, out int n) ? n : fallback;
        }

        private double RandLat() => 20.5 + rng.NextDouble() * 6.1;   // Bangladesh
        private double RandLon() => 88.0 + rng.NextDouble() * 4.7;

        private void Log(string msg)
        {
            txtLog.AppendText(msg + "\n");
            txtLog.ScrollToEnd();
        }

        // ═════════════════════════════════════════════════════════════════════════
        //  EXCEL EXPORT
        // ═════════════════════════════════════════════════════════════════════════

        private string ExportToExcel(string ansDbPath)
        {
            string xlsxPath = System.IO.Path.ChangeExtension(ansDbPath, ".xlsx");

            HashSet<string> multiQIds;
            var columns   = BuildColumnHeaders(out multiQIds);
            var tableData = BuildPivotData(columns, multiQIds);
            var oeData    = LoadOpenendedRows();

            WriteToExcel(xlsxPath, columns, tableData, oeData);
            return xlsxPath;
        }

        // ── Step 1: Discover columns from T_RespAnswer ────────────────────────
        //   multiQIds = QIds whose max rOrderTag > 1  (written as QId_1, QId_2…)
        private List<string> BuildColumnHeaders(out HashSet<string> multiQIds)
        {
            multiQIds = new HashSet<string>();

            // Ordered list of QId + MaxROrder pairs (string[] {QId, MaxR})
            var qList = new List<string[]>();

            using (var cmd = new SQLiteCommand(
                "SELECT QId, " +
                "       MAX(CAST(rOrderTag AS INTEGER)) AS MaxROrder " +
                "FROM T_RespAnswer " +
                "GROUP BY QId " +
                "ORDER BY CAST(MIN(CAST(qOrderTag AS INTEGER)) AS INTEGER), QId",
                aConn))
            using (var rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    string qid  = rdr["QId"].ToString();
                    int    maxR = (rdr["MaxROrder"] == DBNull.Value)
                                  ? 1 : Convert.ToInt32(rdr["MaxROrder"]);
                    qList.Add(new string[] { qid, maxR.ToString() });
                    if (maxR > 1) multiQIds.Add(qid);
                }
            }

            // OE QIds — collect those that have open-ended verbatims
            var oeQIds = new HashSet<string>();
            using (var cmd = new SQLiteCommand(
                "SELECT DISTINCT QId FROM T_RespOpenended", aConn))
            using (var rdr = cmd.ExecuteReader())
                while (rdr.Read())
                    oeQIds.Add(rdr["QId"].ToString());

            // ── Build ordered column list ─────────────────────────────────────
            var cols = new List<string>();
            cols.Add("RespondentId");
            cols.Add("SurveyDateTime");
            cols.Add("SurveyEndTime");
            cols.Add("LengthOfIntv");
            cols.Add("FICode");
            cols.Add("FSCode");
            cols.Add("Latitude");
            cols.Add("Longitude");
            cols.Add("Status");
            cols.Add("TabId");

            foreach (string[] item in qList)
            {
                string qid  = item[0];
                int    maxR = int.Parse(item[1]);

                if (maxR == 1)
                {
                    cols.Add(qid);
                }
                else
                {
                    for (int r = 1; r <= maxR; r++)
                        cols.Add(qid + "_" + r);
                }

                if (oeQIds.Contains(qid))
                    cols.Add(qid + "_OE");
            }

            return cols;
        }

        // ── Step 2: Pivot T_InterviewInfo + T_RespAnswer into rows ────────────
        private List<List<string>> BuildPivotData(
            List<string> columns, HashSet<string> multiQIds)
        {
            var interviews = new DataTable();
            new SQLiteDataAdapter(
                "SELECT RespondentId, SurveyDateTime, SurveyEndTime, LengthOfIntv, " +
                "       FICode, FSCode, Latitude, Longitude, Status, TabId " +
                "FROM T_InterviewInfo ORDER BY RespondentId", aConn).Fill(interviews);

            var answers = new DataTable();
            new SQLiteDataAdapter(
                "SELECT RespondentId, QId, Response, " +
                "       CAST(rOrderTag AS INTEGER) AS rOrd " +
                "FROM T_RespAnswer " +
                "ORDER BY RespondentId, " +
                "         CAST(qOrderTag AS INTEGER), " +
                "         CAST(rOrderTag AS INTEGER)", aConn).Fill(answers);

            var oeTable = new DataTable();
            new SQLiteDataAdapter(
                "SELECT RespondentId, QId, OpenendedResp " +
                "FROM T_RespOpenended ORDER BY RespondentId", aConn).Fill(oeTable);

            // Index by RespondentId
            var ansIndex = answers.AsEnumerable()
                .GroupBy(r => r["RespondentId"].ToString())
                .ToDictionary(g => g.Key, g => g.ToList());

            var oeIndex = oeTable.AsEnumerable()
                .GroupBy(r => r["RespondentId"].ToString())
                .ToDictionary(g => g.Key, g => g.ToList());

            var result = new List<List<string>>();

            foreach (DataRow intv in interviews.Rows)
            {
                string rId = intv["RespondentId"].ToString();

                var dic = new Dictionary<string, string>();
                dic["RespondentId"]   = rId;
                dic["SurveyDateTime"] = intv["SurveyDateTime"].ToString();
                dic["SurveyEndTime"]  = intv["SurveyEndTime"].ToString();
                dic["LengthOfIntv"]   = intv["LengthOfIntv"].ToString();
                dic["FICode"]         = intv["FICode"].ToString();
                dic["FSCode"]         = intv["FSCode"].ToString();
                dic["Latitude"]       = intv["Latitude"].ToString();
                dic["Longitude"]      = intv["Longitude"].ToString();
                dic["Status"]         = intv["Status"].ToString();
                dic["TabId"]          = intv["TabId"].ToString();

                List<DataRow> respRows;
                if (ansIndex.TryGetValue(rId, out respRows))
                {
                    foreach (DataRow row in respRows)
                    {
                        string qid  = row["QId"].ToString();
                        string resp = row["Response"].ToString();
                        int    rOrd = Convert.ToInt32(row["rOrd"]);
                        string key  = multiQIds.Contains(qid)
                                      ? qid + "_" + rOrd : qid;
                        if (!dic.ContainsKey(key))
                            dic[key] = resp;
                    }
                }

                List<DataRow> oeRows;
                if (oeIndex.TryGetValue(rId, out oeRows))
                {
                    foreach (DataRow row in oeRows)
                    {
                        string key = row["QId"].ToString() + "_OE";
                        if (!dic.ContainsKey(key))
                            dic[key] = row["OpenendedResp"].ToString();
                    }
                }

                var rowData = new List<string>();
                foreach (string col in columns)
                {
                    string v;
                    rowData.Add(dic.TryGetValue(col, out v) ? v : "");
                }
                result.Add(rowData);
            }

            return result;
        }

        // ── Step 3: Load OE rows for the Openended sheet ─────────────────────
        private DataTable LoadOpenendedRows()
        {
            var dt = new DataTable();
            new SQLiteDataAdapter(
                "SELECT RespondentId, QId, AttributeValue, OpenendedResp " +
                "FROM T_RespOpenended ORDER BY RespondentId, QId", aConn).Fill(dt);
            return dt;
        }

        // ── Step 4: Write workbook via COM Interop ────────────────────────────
        private void WriteToExcel(string xlsxPath,
            List<string> columns,
            List<List<string>> tableData,
            DataTable oeData)
        {
            object misValue = System.Reflection.Missing.Value;

            var xlApp      = new Microsoft.Office.Interop.Excel.Application();
            var xlWorkBook = xlApp.Workbooks.Add(misValue);

            try
            {
                // ── Sheet 1: Data ─────────────────────────────────────────────
                var xlData = (Microsoft.Office.Interop.Excel.Worksheet)
                    xlWorkBook.Worksheets.get_Item(1);
                xlData.Name = "Data";

                for (int c = 1; c <= columns.Count; c++)
                    xlData.Cells[1, c] = columns[c - 1];

                if (tableData.Count > 0)
                {
                    int totalRows = tableData.Count;
                    int totalCols = columns.Count;
                    int batchSize = 500;

                    for (int rowStart = 0; rowStart < totalRows; rowStart += batchSize)
                    {
                        int batchRows = Math.Min(batchSize, totalRows - rowStart);
                        var batch = new object[batchRows, totalCols];

                        for (int i = 0; i < batchRows; i++)
                            for (int j = 0; j < totalCols; j++)
                                batch[i, j] = "'" + ReplaceNewlines(
                                    tableData[rowStart + i][j], " ");

                        var startCell = (Microsoft.Office.Interop.Excel.Range)
                            xlData.Cells[rowStart + 2, 1];
                        var writeRange = startCell.get_Resize(batchRows, totalCols);
                        writeRange.Value2 = batch;

                        System.Runtime.InteropServices.Marshal.ReleaseComObject(writeRange);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(startCell);
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }
                }

                xlData.Columns.AutoFit();

                // ── Sheet 2: Openended ────────────────────────────────────────
                var xlOE = (Microsoft.Office.Interop.Excel.Worksheet)
                    xlWorkBook.Worksheets.Add(
                        System.Reflection.Missing.Value,
                        xlData,
                        System.Reflection.Missing.Value,
                        System.Reflection.Missing.Value);
                xlOE.Name = "Openended";

                xlOE.Cells[1, 1] = "RespondentId";
                xlOE.Cells[1, 2] = "QId";
                xlOE.Cells[1, 3] = "AttributeValue";
                xlOE.Cells[1, 4] = "OE Verbatim";

                int oeRow = 2;
                foreach (DataRow dr in oeData.Rows)
                {
                    xlOE.Cells[oeRow, 1] = "'" + dr["RespondentId"];
                    xlOE.Cells[oeRow, 2] = "'" + dr["QId"];
                    xlOE.Cells[oeRow, 3] = "'" + dr["AttributeValue"];
                    xlOE.Cells[oeRow, 4] = "'" + ReplaceNewlines(
                        dr["OpenendedResp"].ToString(), " ");
                    oeRow++;
                }

                xlOE.Columns.AutoFit();

                // ── Activate Data sheet then save ─────────────────────────────
                xlData.Activate();
                xlWorkBook.SaveAs(xlsxPath,
                    Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault,
                    misValue, misValue, misValue, misValue,
                    Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlNoChange,
                    misValue, misValue, misValue, misValue, misValue);
            }
            finally
            {
                xlWorkBook.Close(false, misValue, misValue);
                xlApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlWorkBook);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(xlApp);
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private string ReplaceNewlines(string text, string replaceWith)
        {
            return text.Replace("\r\n", replaceWith)
                       .Replace("\n",   replaceWith)
                       .Replace("\r",   replaceWith);
        }
    }
}
