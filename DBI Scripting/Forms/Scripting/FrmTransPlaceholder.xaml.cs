using DBI_Scripting.Classes;
using DBI_Scripting.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Excel = Microsoft.Office.Interop.Excel;

namespace DBI_Scripting.Forms.Scripting
{
    /// <summary>
    /// Interaction logic for FrmTransPlaceholder.xaml
    /// </summary>
    public partial class FrmTransPlaceholder : Window
    {
        String databasePath;
        String myPath;
        List<String> listOfGridQid;
        String selectedSheetName;

        List<TranslatedQtext> listOfTranslatedQText;
        List<TranslatedAttribtext> listOfTranslatedAttribText;

        public FrmTransPlaceholder()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnGetPlaceholder_Click(object sender, RoutedEventArgs e)
        {

            databasePath = txtScriptPath.Text;
            if (File.Exists(databasePath) == false)
                return;
            ConnectionDB connDB = new ConnectionDB();
            if (connDB.connect(txtScriptPath.Text) == true)
            {
                if (connDB.sqlite_conn.State == ConnectionState.Closed)
                    connDB.sqlite_conn.Open();


                SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT * FROM T_Question Order by T_Question.OrderTag", connDB.sqlite_conn);
                DataSet ds = new DataSet();
                dadpt.Fill(ds, "Table1");

                int p = 0;

                //progressBarGetQuestion.Minimum = 0;
                //progressBarGetQuestion.Maximum = ds.Tables["Table1"].Rows.Count;


                if (ds.Tables["Table1"].Rows.Count > 0)
                {
                    Microsoft.Office.Interop.Excel.Application xlApp;
                    Microsoft.Office.Interop.Excel.Workbook xlWorkBook;
                    Microsoft.Office.Interop.Excel.Worksheet xlWorkSheet;
                    object misValue = System.Reflection.Missing.Value;

                    xlApp = new Microsoft.Office.Interop.Excel.Application();
                    xlWorkBook = xlApp.Workbooks.Add(misValue);

                    xlWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);
                    xlWorkSheet.Name = txtProjectName.Text;

                    string qid;
                    int rowno = 1;
                    foreach (DataRow dr in ds.Tables["Table1"].Rows)
                    {
                        //progressBarGetQuestion.Value = p++;

                        //if (dr["QDesc3"].ToString() == "")
                        qid = dr["QId"].ToString();
                        //else
                        //    qid = dr["QDesc3"].ToString();

                        //1,2,5,12,13,17,18,19
                        if (dr["QType"].ToString() == "1" ||
                            dr["QType"].ToString() == "2" ||
                            dr["QType"].ToString() == "5" ||
                            dr["QType"].ToString() == "12" ||
                            dr["QType"].ToString() == "13" ||
                            dr["QType"].ToString() == "17" ||
                            dr["QType"].ToString() == "18" ||
                            dr["QType"].ToString() == "19" ||
                            dr["QType"].ToString() == "60")
                        {
                            List<AttributeMain> listOfAttribute = getAttributeList(connDB.sqlite_conn, qid);

                            xlWorkSheet.Cells[rowno, 1] = "'" + dr["QId"].ToString();
                            xlWorkSheet.Cells[rowno, 3] = "'" + dr["QuestionEnglish"].ToString();

                            xlWorkSheet.Range[xlWorkSheet.Cells[rowno, 1], xlWorkSheet.Cells[rowno, 30]].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.PaleTurquoise);
                            rowno++;
                            rowno++;
                            int row = rowno;
                            int column = 3;
                            for (int i = 0; i < listOfAttribute.Count; i++)
                            {
                                xlWorkSheet.Cells[rowno, 1] = "'" + dr["QId"].ToString();
                                xlWorkSheet.Cells[rowno, 3] = "'" + listOfAttribute[i].AttributeEnglish;
                                xlWorkSheet.Cells[rowno, 2] = "'" + listOfAttribute[i].AttributeValue;
                                rowno++;
                            }

                            rowno++;

                            xlWorkSheet.Range[xlWorkSheet.Cells[row, 2], xlWorkSheet.Cells[rowno - 2, 3]].Cells.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;

                        }
                        else if (dr["QType"].ToString() == "3" ||
                        dr["QType"].ToString() == "4" ||
                        dr["QType"].ToString() == "6" ||
                        dr["QType"].ToString() == "9" ||
                        dr["QType"].ToString() == "10" ||
                        dr["QType"].ToString() == "14" ||
                        dr["QType"].ToString() == "15" ||
                        dr["QType"].ToString() == "16")
                        {
                            xlWorkSheet.Cells[rowno, 1] = "'" + dr["QId"].ToString();
                            xlWorkSheet.Cells[rowno, 3] = "'" + dr["QuestionEnglish"].ToString();

                            xlWorkSheet.Range[xlWorkSheet.Cells[rowno, 1], xlWorkSheet.Cells[rowno, 30]].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.PaleTurquoise);

                            rowno++;
                            rowno++;
                        }
                        else if (dr["QType"].ToString() == "49" ||
                            dr["QType"].ToString() == "50" ||
                            dr["QType"].ToString() == "51")
                        {
                            xlWorkSheet.Cells[rowno, 1] = "'" + dr["QId"].ToString();
                            xlWorkSheet.Cells[rowno, 3] = "'" + dr["QuestionEnglish"].ToString();

                            xlWorkSheet.Range[xlWorkSheet.Cells[rowno, 1], xlWorkSheet.Cells[rowno, 30]].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.PaleTurquoise);

                            rowno++;
                            rowno++;
                        }
                        else if (dr["QType"].ToString() == "7" ||
                            dr["QType"].ToString() == "8" ||
                            dr["QType"].ToString() == "22" ||
                            dr["QType"].ToString() == "23" ||
                            dr["QType"].ToString() == "24" ||
                            dr["QType"].ToString() == "40" ||
                            dr["QType"].ToString() == "48")
                        {
                            List<AttributeMain> listOfAttribute = getAttributeList(connDB.sqlite_conn, qid);
                            listOfGridQid = new List<string>();
                            //Change here for main QId
                            qid = dr["QId"].ToString();

                            xlWorkSheet.Cells[rowno, 1] = "'" + dr["QId"].ToString();
                            xlWorkSheet.Cells[rowno, 3] = "'" + dr["QuestionEnglish"].ToString();

                            xlWorkSheet.Range[xlWorkSheet.Cells[rowno, 1], xlWorkSheet.Cells[rowno, 30]].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.PaleTurquoise);
                            rowno++;
                            rowno++;
                            int row = rowno;
                            int column = 3;
                            for (int i = 0; i < listOfAttribute.Count; i++)
                            {
                                xlWorkSheet.Cells[rowno, 1] = "'" + listOfAttribute[i].QId;
                                xlWorkSheet.Cells[rowno, 3] = "'" + listOfAttribute[i].AttributeEnglish;
                                xlWorkSheet.Cells[rowno, 2] = "'" + listOfAttribute[i].AttributeValue;
                                //xlWorkSheet.Range[xlWorkSheet.Cells[rowno, 2], xlWorkSheet.Cells[rowno, 3]].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Aquamarine);
                                rowno++;

                                if (listOfAttribute[i].LinkId2 != "")
                                {
                                    qid = listOfAttribute[i].LinkId2;
                                    if (!listOfGridQid.Contains(qid))
                                    {
                                        listOfGridQid.Add(qid);
                                    }
                                }
                            }

                            xlWorkSheet.Range[xlWorkSheet.Cells[row, 2], xlWorkSheet.Cells[rowno - 1, 3]].Cells.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;

                            rowno++;

                            for (int x = 0; x < listOfGridQid.Count; x++)
                            {
                                List<GridInfo> listOfGridAttribute = getGridAttributeList(connDB.sqlite_conn, listOfGridQid[x]);

                                row = rowno;

                                for (int j = 0; j < listOfGridAttribute.Count; j++)
                                {
                                    xlWorkSheet.Cells[rowno, 1] = "'" + listOfGridAttribute[j].QId;
                                    xlWorkSheet.Cells[rowno, 3] = "'" + listOfGridAttribute[j].AttributeEnglish;
                                    xlWorkSheet.Cells[rowno, 2] = "'" + listOfGridAttribute[j].AttributeValue;
                                    xlWorkSheet.Range[xlWorkSheet.Cells[rowno, 2], xlWorkSheet.Cells[rowno, 3]].Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Aquamarine);
                                    rowno++;
                                }

                                xlWorkSheet.Range[xlWorkSheet.Cells[row, 2], xlWorkSheet.Cells[rowno - 1, 3]].Cells.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;

                                rowno++;

                            }

                        }
                    }

                    //xlWorkSheet.Columns.AutoFit();

                    xlWorkSheet.Columns[1].AutoFit();
                    xlWorkSheet.Columns[2].ColumnWidth = 5;
                    xlWorkSheet.Columns[3].ColumnWidth = 70;
                    xlWorkSheet.Columns[4].ColumnWidth = 70;

                    xlWorkSheet.Range["C:C"].Style.WrapText = true;
                    xlWorkSheet.Range["D:D"].Style.WrapText = true;

                    xlApp.ActiveWindow.DisplayGridlines = false;

                    //xlApp.Visible = true;




                    //xlWorkBook.SaveAs(txt_SQLiteDB_Location.Text.Substring(0, txt_SQLiteDB_Location.Text.LastIndexOf("\\")) + "\\" + comProject.Text + "_" + txtWeekNo.Text + ".xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault);
                    //xlWorkBook.SaveAs("D:\\Ismile Personal\\New folder (2)\\Analysis\\" + comProject.Text + "_" + txtWeekNo.Text + ".xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault);
                    xlWorkBook.SaveAs(myPath + "\\" + txtProjectName.Text + "_Placeholder.xlsx", Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault);
                    xlWorkBook.Close(true, misValue, misValue);
                    xlApp.Quit();


                    releaseObject(xlWorkSheet);
                    releaseObject(xlWorkBook);
                    releaseObject(xlApp);


                    dadpt.Dispose();

                    if (connDB.sqlite_conn.State == ConnectionState.Open)
                        connDB.sqlite_conn.Close();

                    connDB.sqlite_conn = null;
                    connDB = null;

                    //GC.Collect();
                    //GC.WaitForPendingFinalizers();

                    MessageBox.Show("Write Complete");
                }
            }
        }

        private List<AttributeMain> getAttributeList(SQLiteConnection sqlite_conn, string QId)
        {

            List<AttributeMain> listOfAttribute = new List<AttributeMain>();

            SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT * FROM T_OptAttribute WHERE QId='" + QId + "'", sqlite_conn);
            DataSet ds = new DataSet();
            dadpt.Fill(ds, "Table1");
            if (ds.Tables["Table1"].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables["Table1"].Rows)
                {
                    AttributeMain optAttribute = new AttributeMain();
                    optAttribute.ProjectId = dr["projectId"].ToString();
                    optAttribute.QId = dr["qId"].ToString();
                    optAttribute.AttributeEnglish = dr["attributeEnglish"].ToString();
                    optAttribute.AttributeBengali = dr["attributeBengali"].ToString();
                    optAttribute.AttributeValue = dr["attributeValue"].ToString();
                    optAttribute.AttributeOrder = dr["attributeOrder"].ToString();
                    optAttribute.TakeOpenended = dr["takeOpenended"].ToString();
                    optAttribute.IsExclusive = dr["isExclusive"].ToString();
                    optAttribute.LinkId1 = dr["linkId1"].ToString();
                    optAttribute.LinkId2 = dr["linkId2"].ToString();
                    optAttribute.MinValue = dr["minValue"].ToString();
                    optAttribute.MaxValue = dr["maxValue"].ToString();
                    optAttribute.ForceAndMsgOpt = dr["forceAndMsgOpt"].ToString();
                    optAttribute.GroupName = dr["groupName"].ToString();
                    optAttribute.FilterQid = dr["filterQid"].ToString();
                    optAttribute.FilterType = dr["filterType"].ToString();
                    optAttribute.ExcepValue = dr["excepValue"].ToString();
                    optAttribute.Comments = dr["comments"].ToString();
                    optAttribute.AttributeLang3 = dr["attributeLang3"].ToString();
                    optAttribute.AttributeLang4 = dr["attributeLang4"].ToString();
                    optAttribute.AttributeLang5 = dr["attributeLang5"].ToString();
                    optAttribute.AttributeLang6 = dr["attributeLang6"].ToString();
                    optAttribute.AttributeLang7 = dr["attributeLang7"].ToString();
                    optAttribute.AttributeLang8 = dr["attributeLang8"].ToString();
                    optAttribute.AttributeLang9 = dr["attributeLang9"].ToString();
                    optAttribute.AttributeLang10 = dr["attributeLang10"].ToString();


                    listOfAttribute.Add(optAttribute);
                }

            }
            return listOfAttribute;
        }
        private List<GridInfo> getGridAttributeList(SQLiteConnection sqlite_conn, string QId)
        {
            List<GridInfo> listOfGridAttribute = new List<GridInfo>();

            SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT * FROM T_GridInfo WHERE QId='" + QId + "'", sqlite_conn);
            DataSet ds = new DataSet();
            dadpt.Fill(ds, "Table1");
            if (ds.Tables["Table1"].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables["Table1"].Rows)
                {
                    GridInfo optAttribute = new GridInfo();

                    optAttribute.ProjectId = dr["ProjectId"].ToString();
                    optAttribute.QId = dr["QId"].ToString();
                    optAttribute.AttributeEnglish = dr["AttributeEnglish"].ToString();
                    optAttribute.AttributeBengali = dr["AttributeBengali"].ToString();
                    optAttribute.AttributeValue = dr["AttributeValue"].ToString();
                    optAttribute.AttributeOrder = dr["AttributeOrder"].ToString();
                    optAttribute.TakeOpenended = dr["TakeOpenended"].ToString();
                    optAttribute.IsExclusive = dr["IsExclusive"].ToString();
                    optAttribute.MinValue = dr["MinValue"].ToString();
                    optAttribute.MaxValue = dr["MaxValue"].ToString();
                    optAttribute.ForceAndMsgOpt = dr["ForceAndMsgOpt"].ToString();
                    optAttribute.Comments = dr["Comments"].ToString();
                    optAttribute.AttributeLang3 = dr["AttributeLang3"].ToString();
                    optAttribute.AttributeLang4 = dr["AttributeLang4"].ToString();
                    optAttribute.AttributeLang5 = dr["AttributeLang5"].ToString();
                    optAttribute.AttributeLang6 = dr["AttributeLang6"].ToString();
                    optAttribute.AttributeLang7 = dr["AttributeLang7"].ToString();
                    optAttribute.AttributeLang8 = dr["AttributeLang8"].ToString();
                    optAttribute.AttributeLang9 = dr["AttributeLang9"].ToString();
                    optAttribute.AttributeLang10 = dr["AttributeLang10"].ToString();

                    listOfGridAttribute.Add(optAttribute);
                }

            }
            return listOfGridAttribute;
        }

        public void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        private void btnBrowseScript_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sTemp;

                sTemp = Properties.Settings.Default.StartupPath;

                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = sTemp;
                openFileDialog1.FileName = "";
                openFileDialog1.Filter = "Script File (*.db)|*.db|All Files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == true)
                {
                    txtScriptPath.Text = openFileDialog1.FileName;
                    myPath = txtScriptPath.Text.Substring(0, txtScriptPath.Text.LastIndexOf('\\'));
                    //fileName = txtScriptPath.Text.Substring(txtScriptPath.Text.LastIndexOf('\\') + 1);


                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();

                    this.getScriptVersion();

                }
                else
                    txtScriptPath.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void getScriptVersion()
        {
            if (txtScriptPath.Text != "")
            {
                if (File.Exists(txtScriptPath.Text))
                {
                    ConnectionDB connDB = new ConnectionDB();
                    if (connDB.connect(txtScriptPath.Text) == true)
                    {
                        if (connDB.sqlite_conn.State == ConnectionState.Closed)
                            connDB.sqlite_conn.Open();

                        SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT * FROM T_ProjectInfo", connDB.sqlite_conn);
                        DataSet ds = new DataSet();
                        dadpt.Fill(ds, "Table1");
                        if (ds.Tables["Table1"].Rows.Count > 0)
                        {
                            foreach (DataRow dr in ds.Tables["Table1"].Rows)
                            {
                                txtScriptVersion.Text = dr["Version"].ToString();
                                //priorScriptVersion = dr["Version"].ToString();
                                txtProjectName.Text = dr["ProjectName"].ToString();
                                //projectId = dr["ProjectId"].ToString();
                            }
                        }

                        if (connDB.sqlite_conn.State == ConnectionState.Open)
                            connDB.sqlite_conn.Close();


                    }
                }
                else
                    MessageBox.Show("Invalid script file location");
            }
            else
                MessageBox.Show("Script location should not be blank");
        }

        private void btnKillProcess_Click(object sender, RoutedEventArgs e)
        {
            this.quitProcess();
        }

        private void quitProcess()
        {
            //try
            //{
            Process[] proc = Process.GetProcessesByName("EXCEL");
            foreach (Process myProcess in proc)
            {
                myProcess.Kill();
            }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void btnBrowseExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sTemp;

                sTemp = Properties.Settings.Default.StartupPath;

                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = sTemp;
                openFileDialog1.FileName = "";
                openFileDialog1.Filter = "Excel Data (*.xlsx)|*.xlsx|All Files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == true)
                {
                    txtPlaceholderExcel.Text = openFileDialog1.FileName;
                    myPath = txtPlaceholderExcel.Text.Substring(0, txtPlaceholderExcel.Text.LastIndexOf('\\'));
                    this.loadWorksheet();
                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();
                }
                else
                    txtPlaceholderExcel.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void loadWorksheet()
        {
            try
            {
                if (File.Exists(txtPlaceholderExcel.Text) == true)
                {
                    Excel.Application xlApp = new Excel.Application();
                    Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtPlaceholderExcel.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);


                    chkListBoxWorksheet.Items.Clear();
                    for (int i = 1; i <= xlWorkBook.Worksheets.Count; i++)
                    {
                        chkListBoxWorksheet.Items.Add(xlWorkBook.Worksheets[i].Name.ToString());
                    }

                    releaseObject(xlWorkBook);
                    releaseObject(xlApp);
                    //xlApp.Quit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void chkListBoxWorksheet_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
        {
            //int myCounter = 0;
            selectedSheetName = "";
            //foreach (var item in chkListBoxWorksheet.Items)
            //{
            //    for (int i = 0; i < chkListBoxWorksheet.SelectedItems.Count; i++)
            //    {
            //        if (chkListBoxWorksheet.SelectedItems[i].ToString() == item.ToString())
            //        {
            //            listOfSheetName.Add(item.ToString());
            //            myCounter++;
            //        }
            //    }

            //}

            if (chkListBoxWorksheet.SelectedItems.Count > 1)
            {
                string selecteditem = chkListBoxWorksheet.SelectedItems[0].ToString();
                //string item = e.Item as string;
                chkListBoxWorksheet.SelectedItems.Remove(selecteditem);
            }

            selectedSheetName = chkListBoxWorksheet.SelectedItems[0].ToString();

        }

        private void btnUpdateTranslation_Click(object sender, RoutedEventArgs e)
        {
            if (txtPlaceholderExcel.Text != "")
            {
                if (File.Exists(txtPlaceholderExcel.Text))
                {
                    if (txtScriptPath.Text != "")
                    {
                        if (selectedSheetName != "")
                        {
                            listOfTranslatedQText = new List<TranslatedQtext>();
                            listOfTranslatedAttribText = new List<TranslatedAttribtext>();

                            //TextWriter txtWriter = new StreamWriter(myPath + "\\05." + txtSaveFileName.Text + ".sps");

                            Excel.Application xlApp = new Excel.Application();
                            Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtPlaceholderExcel.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

                            //try
                            //{
                            //************************* This is for Single Variable ********************************************
                            foreach (Excel.Worksheet myWorksheet in xlApp.Worksheets)
                            {
                                if (selectedSheetName == myWorksheet.Name)
                                {
                                    Excel.Range range;
                                    //Read the excel file
                                    range = myWorksheet.UsedRange;

                                    int iStartRow = 1;

                                    for (int i = iStartRow; i <= range.Rows.Count; i++)
                                    {
                                        if (myWorksheet.Cells[i, 1].Value2 != null && myWorksheet.Cells[i, 3].Value2 != null && myWorksheet.Cells[i, 4].Value2 != null)
                                        {
                                            string qid = myWorksheet.Cells[i, 1].Value.ToString();
                                            string qText = myWorksheet.Cells[i, 4].Value.ToString();

                                            if (myWorksheet.Cells[i, 2].Value2 != null)
                                            {
                                                string attribValue = myWorksheet.Cells[i, 2].Value.ToString();

                                                TranslatedAttribtext translatedAttribText = new TranslatedAttribtext();
                                                translatedAttribText.Qid = qid;
                                                translatedAttribText.AttribText = qText;
                                                translatedAttribText.AttribValue = attribValue;
                                                listOfTranslatedAttribText.Add(translatedAttribText);

                                            }
                                            else
                                            {
                                                TranslatedQtext translatedQText = new TranslatedQtext();
                                                translatedQText.Qid = qid;
                                                translatedQText.QText = qText;
                                                listOfTranslatedQText.Add(translatedQText);
                                            }

                                            //txtWriter.WriteLine("IF RespondentId ='" + myWorksheet.Cells[i, 1].Value.ToString() + "' " + s_temp1 + "=" + myWorksheet.Cells[i, ColNo].Value.ToString().Trim() + ".");
                                        }
                                    }
                                }
                            }

                            xlApp.Quit();
                            releaseObject(xlWorkBook);
                            releaseObject(xlApp);

                            //***************************************************************

                            ConnectionDB connDB = new ConnectionDB();
                            if (connDB.connect(txtScriptPath.Text) == true)
                            {
                                if (connDB.sqlite_conn.State == ConnectionState.Closed)
                                    connDB.sqlite_conn.Open();


                                for (int x = 0; x < listOfTranslatedQText.Count; x++)
                                {
                                    SQLiteCommand command = new SQLiteCommand(connDB.sqlite_conn);
                                    command.CommandText = ("UPDATE T_Question SET QuestionBengali='" + listOfTranslatedQText[x].QText.Replace("'","''") + "' WHERE QId='" + listOfTranslatedQText[x].Qid + "'");
                                    command.ExecuteNonQuery();
                                }


                                for (int x = 0; x < listOfTranslatedAttribText.Count; x++)
                                {
                                    SQLiteCommand command = new SQLiteCommand(connDB.sqlite_conn);
                                    command.CommandText = ("UPDATE T_OptAttribute SET AttributeBengali='" + listOfTranslatedAttribText[x].AttribText.Replace("'", "''") + "' WHERE QId='" + listOfTranslatedAttribText[x].Qid + "' AND AttributeValue='" + listOfTranslatedAttribText[x].AttribValue + "'");
                                    command.ExecuteNonQuery();
                                }


                                for (int x = 0; x < listOfTranslatedAttribText.Count; x++)
                                {
                                    SQLiteCommand command = new SQLiteCommand(connDB.sqlite_conn);
                                    command.CommandText = ("UPDATE T_GridInfo SET AttributeBengali='" + listOfTranslatedAttribText[x].AttribText.Replace("'", "''") + "' WHERE QId='" + listOfTranslatedAttribText[x].Qid + "' AND AttributeValue='" + listOfTranslatedAttribText[x].AttribValue + "'");
                                    command.ExecuteNonQuery();
                                }


                                if (connDB.sqlite_conn.State == ConnectionState.Open)
                                    connDB.sqlite_conn.Close();

                            }


                            MessageBox.Show("Write Complete");

                            //if (listOfErrorMessage.Count > 0)
                            //{
                            //    this.Height = 575;
                            //    txtErrorMessage.Clear();
                            //    for (int i = 0; i < listOfErrorMessage.Count; i++)
                            //    {
                            //        txtErrorMessage.AppendText(listOfErrorMessage[i] + "\n");
                            //    }
                            //}


                        }
                    }
                    else
                        MessageBox.Show("Have to write OE syntax file name");
                }
                else
                    MessageBox.Show("Invalid File Location");
            }
            else
                MessageBox.Show("Have to select OE Excel");
            //MessageBox.Show("");
        }

    }
}
