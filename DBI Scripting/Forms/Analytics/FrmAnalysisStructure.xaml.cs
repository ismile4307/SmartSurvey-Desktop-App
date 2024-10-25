using Microsoft.Win32;
using System;
using System.Collections.Generic;
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

namespace DBI_Scripting.Forms.Analytics
{
    /// <summary>
    /// Interaction logic for FrmAnalysisStructure.xaml
    /// </summary>
    public partial class FrmAnalysisStructure : Window
    {
        private string myPath, sSelectedSheet;
        public FrmAnalysisStructure()
        {
            InitializeComponent();
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
                openFileDialog1.Filter = "Excel File (*.xlsx)|*.xlsx|All Files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == true)
                {
                    txtAnalysisExcelPath.Text = openFileDialog1.FileName;
                    myPath = txtAnalysisExcelPath.Text.Substring(0, txtAnalysisExcelPath.Text.LastIndexOf('\\'));

                    this.loadWorksheet();
                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();
                }
                else
                    txtAnalysisExcelPath.Text = "";
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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

            //this.loadRespondentId();
        }
        private void btnRunAnalysisStructure_Click(object sender, RoutedEventArgs e)
        {
            if (sSelectedSheet != "" && sSelectedSheet != null && File.Exists(txtAnalysisExcelPath.Text))
            {
                //txtAnalysisExcelPath.Items.Clear();

                Excel.Application xlApp = new Excel.Application();
                Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtAnalysisExcelPath.Text, 0, false, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);

                foreach (Excel.Worksheet myWorksheet in xlApp.Worksheets)
                {
                    if (myWorksheet.Name == sSelectedSheet)
                    {
                        Excel.Range range;
                        //Read the excel file
                        range = myWorksheet.UsedRange;

                        //int ColNo = getOECodeColumnNumber(myWorksheet);
                        bool firstTime = true;
                        string priorQid = "";
                        string currentQid = "";
                        for (int i = 2; i <= range.Rows.Count; i++)
                        {
                            string temp1 = myWorksheet.Cells[i, 2].Value.ToString();


                            if (!temp1.Contains("_"))
                            {
                                myWorksheet.Cells[i, 1] = 5;
                                

                                if (firstTime == false)
                                {
                                    myWorksheet.Cells[i-1, 1] = 6;

                                    myWorksheet.Cells[i - 1, 5] = priorQid;
                                    myWorksheet.Cells[i - 1, 6] = "XXX";
                                    myWorksheet.Cells[i - 1, 7] = myWorksheet.Cells[i-1, 4];
                                }
                                firstTime = true;
                            }
                            else
                            {
                                string[] qId = temp1.Split('_');

                                if (qId.Length == 2)
                                    currentQid = qId[0];
                                else if (qId.Length == 3)
                                    currentQid = qId[0] + "_" + qId[1];

                                if (priorQid != currentQid && firstTime == false)
                                {
                                    myWorksheet.Cells[i, 1] = 6;

                                    myWorksheet.Cells[i - 1, 5] = priorQid;
                                    myWorksheet.Cells[i - 1, 6] = "XXX";
                                    myWorksheet.Cells[i - 1, 7] = myWorksheet.Cells[i-1, 4];

                                }
                                else if (priorQid != currentQid && firstTime == true)
                                {
                                    myWorksheet.Cells[i, 1] = 6;
                                }
                                else if (priorQid == currentQid)
                                {
                                    myWorksheet.Cells[i, 1] = 6;
                                }
                                firstTime = false;
                                priorQid = currentQid;
                            }


                            //if (myWorksheet.Cells[i, ColNo].Value2 != null)// && myWorksheet.Cells[i, ColNo].Value2 != null)
                            //{
                            //    chkListBoxRespondentId.Items.Add(temp1);

                            //}
                        }

                    }
                }

                xlWorkBook.Save();

                xlWorkBook.Close();
                xlApp.Quit();


                releaseObject(xlWorkBook);
                releaseObject(xlApp);

                MessageBox.Show("Write Complete");
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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

        private void btnKillProcess_Click(object sender, RoutedEventArgs e)
        {
            this.quitProcess();
            MessageBox.Show("All Excell process are killed");
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

        private void btnCreateStructureExcel_Click(object sender, RoutedEventArgs e)
        {
            String expression = txtSPSSPath.Text;
            bool xyz = convetToPostFixNotationAndExecute(expression);

        }

        public Boolean convetToPostFixNotationAndExecute(String expression)
        {
            //sProjectId = aProjectId;
            //sRespondentId = aRespondentId;
            //QuestionId = aQid;
            string _postfixExpression="";

            expression = expression.Replace(" ", "").Trim();
            //String seperators = "\\&|\\|";
            Char[] seperators={'&','|'};
            String[] operands = expression.Split(seperators);

            //if (operands.Length == 1)
            //    return ExecuteConditionalElement(expression);
            
            expression = "(" + expression + ")";
            char[] expInCharArray = expression.ToCharArray();
            Stack<Char> myStack = new Stack<Char>();

            try
            {
                int i;
                for (i = 0; i < expInCharArray.Length; i++)
                {
                    if (expInCharArray[i] == '(')
                        myStack.Push('(');
                    else if (expInCharArray[i] == '|' || expInCharArray[i] == '&')
                    {
                        _postfixExpression += ",";
                        if (myStack.Count() > 1 && myStack.Peek() != '(')
                        {
                            _postfixExpression += myStack.Pop() + ",";
                        }
                        myStack.Push(expInCharArray[i]);
                    }
                    else if (expInCharArray[i] == ')')
                    {
                        while (myStack.Peek() != '(')
                        {
                            _postfixExpression += "," + myStack.Pop();// + ",";
                        }
                        myStack.Pop();
                    }
                    else
                        _postfixExpression += expInCharArray[i];
                    // _postfixExpression = _postfixExpression.trim();

                }
            }
            catch (Exception ex)
            {
                //Log.e("wrong expression", QuestionId
                //        + " : Check the condition for bracet \"" + expression
                //        + "\"");
            }

            return executePostfixExpression(_postfixExpression);
        }

        private Boolean executePostfixExpression(String expression)
        {
            Stack<Boolean> myStack = new Stack<Boolean>();
            try
            {
                int i;

                //String seperator = ",";
                Char[] seperator = { ',' };
                String[] elements = expression.Split(seperator);
                for (i = 0; i < elements.Length; i++)
                {

                    if (elements[i].Equals("|"))
                        myStack.Push(myStack.Pop() | myStack.Pop());
                    else if (elements[i].Equals("&"))
                        myStack.Push(myStack.Pop() & myStack.Pop());
                    else
                        //myStack.Push(ExecuteConditionalElement(elements[i]));
                        MessageBox.Show(elements[i]);

                }
                return myStack.Pop();
            }
            catch (Exception ex)
            {
                //Log.e("wrong operator", QuestionId
                //        + " : Check the condition for operator");
            }

            return false;
        }
    }
}
