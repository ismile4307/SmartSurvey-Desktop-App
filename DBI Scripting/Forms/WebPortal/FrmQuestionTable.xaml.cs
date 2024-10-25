using DBI_Scripting.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace DBI_Scripting.Forms.WebPortal
{
    /// <summary>
    /// Interaction logic for FrmQuestionTable.xaml
    /// </summary>
    public partial class FrmQuestionTable : Window
    {
        private string myPath, sSelectedSheet;
        private List<TableStructure> listOfTableStructure;

        public FrmQuestionTable()
        {
            InitializeComponent();
        }

        private void btnBrowseStructureExcel_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            string sTemp;

            sTemp = Properties.Settings.Default.StartupPath;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = sTemp;
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "Excel File (*.xlsx)|*.xlsx|All Files (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == true)
            {
                txtAnalysisExcelPath.Text = openFileDialog1.FileName;
                this.loadCategoryList();
                myPath = txtAnalysisExcelPath.Text.Substring(0, txtAnalysisExcelPath.Text.LastIndexOf('\\'));

                Properties.Settings.Default.StartupPath = myPath;
                Properties.Settings.Default.Save();
            }
            else
                txtAnalysisExcelPath.Text = "";
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void loadCategoryList()
        {
            //try
            //{
            if (File.Exists(txtAnalysisExcelPath.Text) == true)
            {
                Excel.Application xlApp = new Excel.Application();
                Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtAnalysisExcelPath.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);


                chkListBoxWorksheet.Items.Clear();
                for (int i = 1; i <= xlWorkBook.Worksheets.Count; i++)
                {
                    chkListBoxWorksheet.Items.Add(xlWorkBook.Worksheets[i].Name.ToString());
                }

                releaseObject(xlWorkBook);
                releaseObject(xlApp);
                //xlApp.Quit();
            }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void releaseObject(object obj)
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

        private void chkListBoxWorksheet_ItemSelectionChanged(object sender, Xceed.Wpf.Toolkit.Primitives.ItemSelectionChangedEventArgs e)
        {
            if (chkListBoxWorksheet.SelectedItems.Count > 1)
            {
                string selecteditem = chkListBoxWorksheet.SelectedItems[0].ToString();
                //string item = e.Item as string;
                chkListBoxWorksheet.SelectedItems.Remove(selecteditem);
            }

            sSelectedSheet = chkListBoxWorksheet.SelectedItems[0].ToString();
        }

        private void btnQuestionTable_Click(object sender, RoutedEventArgs e)
        {
            this.prepareTableStructure();

            TextWriter txtWriter = new StreamWriter(myPath + "//Questions_" + txtProjectCode.Text + ".sql");

            txtWriter.WriteLine("DROP TABLE IF EXISTS `questions_" + txtProjectCode.Text + "`;");
            txtWriter.WriteLine("");
            txtWriter.WriteLine("");

            txtWriter.WriteLine("CREATE TABLE `questions_" + txtProjectCode.Text + "` (");
            txtWriter.WriteLine("`id` bigint(20) UNSIGNED NOT NULL,");
            //txtWriter.WriteLine("`project_code` int(11) DEFAULT NULL,");
            txtWriter.WriteLine("`qid` varchar(50) COLLATE utf8_unicode_ci NOT NULL,");
            txtWriter.WriteLine("`question_text` varchar(255) COLLATE utf8_unicode_ci NOT NULL,");
            txtWriter.WriteLine("`qtype` varchar(10) COLLATE utf8_unicode_ci NOT NULL,");
            txtWriter.WriteLine("`qorder` int(11) NOT NULL,");
            txtWriter.WriteLine("`show_in_search` int(11) NOT NULL,");
            txtWriter.WriteLine("`show_in_frequency` int(11) NOT NULL,");
            txtWriter.WriteLine("`show_in_cross` int(11) NOT NULL,");
            txtWriter.WriteLine("`show_in_filter` int(11) NOT NULL,");
            txtWriter.WriteLine("`status` int(11) NOT NULL");
            //txtWriter.WriteLine("`created_at` timestamp NULL DEFAULT NULL,");
            //txtWriter.WriteLine("`updated_at` timestamp NULL DEFAULT NULL");
            txtWriter.WriteLine(") ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;");



            txtWriter.WriteLine("");
            txtWriter.WriteLine("");
            txtWriter.WriteLine("ALTER TABLE `questions_" + txtProjectCode.Text + "`");
            txtWriter.WriteLine("ADD PRIMARY KEY (`id`);");

            txtWriter.WriteLine("");
            txtWriter.WriteLine("");
            txtWriter.WriteLine("ALTER TABLE `questions_" + txtProjectCode.Text + "`");
            txtWriter.WriteLine("  MODIFY `id` bigint(20) UNSIGNED NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=1;");
            //txtWriter.WriteLine("COMMIT;");

            txtWriter.WriteLine("");
            txtWriter.WriteLine("");


            //txtWriter.WriteLine("INSERT INTO `questions_" + txtProjectCode.Text + "` (`project_code`, `qid`, `question_text`, `qtype`, `qorder`, `show_in_search`, `show_in_frequency`, `show_in_cross`, `show_in_filter`, `status`, `created_at`, `updated_at`) VALUES");
            txtWriter.WriteLine("INSERT INTO `questions_" + txtProjectCode.Text + "` (`qid`, `question_text`, `qtype`, `qorder`, `show_in_search`, `show_in_frequency`, `show_in_cross`, `show_in_filter`, `status`) VALUES");
            string insert_str;// = "(" + txtProjectCode.Text + ", ";
            int noOfVar = listOfTableStructure.Count;
            for (int i = 0; i < noOfVar; i++)
            {
                //insert_str = "(" + txtProjectCode.Text + ", ";
                insert_str = "(";
                if (i < noOfVar - 1)
                {
                    insert_str = insert_str + "'" + listOfTableStructure[i].variableName + "', ";
                    insert_str = insert_str + "'" + listOfTableStructure[i].qText + "', ";
                    insert_str = insert_str + "'" + listOfTableStructure[i].qType + "', ";

                    insert_str = insert_str + listOfTableStructure[i].qOrder + ", ";
                    insert_str = insert_str + listOfTableStructure[i].showInSearch + ", ";
                    insert_str = insert_str + listOfTableStructure[i].showInFreq + ", ";
                    insert_str = insert_str + listOfTableStructure[i].showInCorssTable + ", ";
                    //insert_str = insert_str + listOfTableStructure[i].showInFilter + ", 1, NULL, NULL),";
                    insert_str = insert_str + listOfTableStructure[i].showInFilter + ", 1),";
                }
                else
                {
                    insert_str = insert_str + "'" + listOfTableStructure[i].variableName + "', ";
                    insert_str = insert_str + "'" + listOfTableStructure[i].qText + "', ";
                    insert_str = insert_str + "'" + listOfTableStructure[i].qType + "', ";

                    insert_str = insert_str + listOfTableStructure[i].qOrder + ", ";
                    insert_str = insert_str + listOfTableStructure[i].showInSearch + ", ";
                    insert_str = insert_str + listOfTableStructure[i].showInFreq + ", ";
                    insert_str = insert_str + listOfTableStructure[i].showInCorssTable + ", ";
                    //insert_str = insert_str + listOfTableStructure[i].showInFilter + ", 1, NULL, NULL);";
                    insert_str = insert_str + listOfTableStructure[i].showInFilter + ", 1);";
                }

                txtWriter.WriteLine(insert_str);
            }


            txtWriter.Close();

            MessageBox.Show("Write Complete");
        }

        private void prepareTableStructure()
        {

            if (sSelectedSheet != "" && sSelectedSheet != null && File.Exists(txtAnalysisExcelPath.Text))
            {
                listOfTableStructure = new List<TableStructure>();

                Excel.Application xlApp = new Excel.Application();
                Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtAnalysisExcelPath.Text, 0, false, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

                foreach (Excel.Worksheet myWorksheet in xlApp.Worksheets)
                {
                    if (myWorksheet.Name == sSelectedSheet)
                    {
                        Excel.Range range;
                        //Read the excel file
                        range = myWorksheet.UsedRange;

                        for (int i = 2; i <= range.Rows.Count; i++)
                        {
                            string varName = myWorksheet.Cells[i, 1].Value.ToString();

                            if (myWorksheet.Cells[i, 6].Value2 != null)
                            {
                                string varNameTemp = myWorksheet.Cells[i, 6].Value.ToString();
                                if (varNameTemp != "")
                                    varName = varNameTemp;
                            }
                            string varWidth = myWorksheet.Cells[i, 3].Value.ToString();
                            string qLabel = "";
                            if (myWorksheet.Cells[i, 8].Value2 != null)
                                qLabel = myWorksheet.Cells[i, 8].Value.ToString().Replace("'", "''");

                            string qType = "";
                            if (myWorksheet.Cells[i, 9].Value2 != null)
                                qType = myWorksheet.Cells[i, 9].Value.ToString();

                            string qOrder = (i-1).ToString();
                            if (myWorksheet.Cells[i, 10].Value2 != null)
                                qOrder = myWorksheet.Cells[i, 10].Value.ToString();

                            string showInSearch = "0";
                            if (myWorksheet.Cells[i, 11].Value2 != null)
                                showInSearch = myWorksheet.Cells[i, 11].Value.ToString();

                            string showInFreq = "0";
                            if (myWorksheet.Cells[i, 12].Value2 != null)
                                showInFreq = myWorksheet.Cells[i, 12].Value.ToString();

                            string showInCorss = "0";
                            if (myWorksheet.Cells[i, 13].Value2 != null)
                                showInCorss = myWorksheet.Cells[i, 13].Value.ToString();

                            string showInFilter = "0";
                            if (myWorksheet.Cells[i, 14].Value2 != null)
                                showInFilter = myWorksheet.Cells[i, 14].Value.ToString();

                            if (qType != "")
                                listOfTableStructure.Add(new TableStructure(varName, varWidth, qLabel, qType, qOrder, showInSearch, showInFreq, showInCorss, showInFilter, varName));

                        }

                    }
                }

                xlWorkBook.Close();
                xlApp.Quit();


                releaseObject(xlWorkBook);
                releaseObject(xlApp);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
