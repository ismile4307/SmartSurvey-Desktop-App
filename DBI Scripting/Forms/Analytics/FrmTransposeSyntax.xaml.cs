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

namespace DBI_Scripting.Forms.Analytics
{
    /// <summary>
    /// Interaction logic for FrmTransposeSyntax.xaml
    /// </summary>
    public partial class FrmTransposeSyntax : Window
    {
        String myPath;

        Dictionary<string, List<string>> dicNameVsList;
        public FrmTransposeSyntax()
        {
            InitializeComponent();
        }

        private void btnBrowseImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sTemp;

                sTemp = Properties.Settings.Default.StartupPath;

                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = sTemp;
                openFileDialog1.FileName = "";
                openFileDialog1.Filter = "Text File (*.txt)|*.txt|All Files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == true)
                {
                    txtImageVarFile.Text = openFileDialog1.FileName;
                    myPath = txtImageVarFile.Text.Substring(0, txtImageVarFile.Text.LastIndexOf('\\'));

                    //this.loadWorksheet();
                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();
                }
                else
                    txtImageVarFile.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnRunTranspose_Click(object sender, RoutedEventArgs e)
        {
            if (txtImageVarFile.Text != "")
            {
                if (File.Exists(txtImageVarFile.Text))
                {
                    String varName = "";
                    List<string> listOfImageVariable = new List<string>();
                    List<string> listOfValueLabel = new List<string>();
                    TextReader txtReader = new StreamReader(txtImageVarFile.Text);
                    String strline = txtReader.ReadLine();
                    int counter = 0;
                    string priorValue = "";
                    bool stop = false;
                    bool valueLable = false;

                    while (strline != null)
                    {
                        if (strline.Trim() != "")
                        {
                            if (strline.ToUpper().Contains("VARNAME"))
                            {
                                string[] word = strline.Split('=');
                                varName = word[1];
                            }
                            else if(strline.ToUpper().Contains("VALUELABEL"))
                            {
                                valueLable = true;
                            }
                            else if(valueLable==false)
                            {
                                string[] word = strline.Split('_');
                                if (priorValue == "" && stop == false)
                                {
                                    counter++;
                                }
                                else if (priorValue != "" && priorValue == word[0] && stop == false)
                                {
                                    counter++;
                                }
                                else if (priorValue != "" && priorValue != word[0] && stop == false)
                                {
                                    stop = true;
                                }
                                listOfImageVariable.Add(strline);
                                priorValue = word[0];
                            }
                            else if (valueLable == true)
                            {
                                listOfValueLabel.Add(strline);
                            }
                        }
                        strline = txtReader.ReadLine();
                    }

                    txtReader.Close();

                    string name;
                    dicNameVsList = new Dictionary<string, List<string>>();
                    for (int i = 0; i < counter; i++)
                    {
                        name = listOfImageVariable[i];
                        List<string> myList = new List<string>();
                        for (int j = i; j < listOfImageVariable.Count; j = j + counter)
                        {
                            myList.Add(listOfImageVariable[j]);
                        }
                        dicNameVsList.Add(name, myList);
                    }

                    //**************************************************************

                    TextWriter txtWriter = new StreamWriter(myPath + "\\ImageTransposeSyntax.sps");


                    List<string> listForValueLabel = new List<string>();

                    foreach (var item in dicNameVsList)
                    {
                        List<string> myList = item.Value;

                        for (int i = 0; i < myList.Count; i++)
                        {
                            string mydata = myList[i].Substring(varName.Length);
                            string[] word = mydata.Split('_');
                            txtWriter.WriteLine("NUMERIC New" + varName + word[1] + "_" + word[0] + " (F8.0).");
                        }

                        txtWriter.WriteLine("");

                        for (int i = 0; i < myList.Count; i++)
                        {
                            string mydata = myList[i].Substring(varName.Length);
                            string[] word = mydata.Split('_');
                            txtWriter.WriteLine("IF " + myList[i] + "=" + word[1] + " New" + varName + word[1] + "_" + word[0] + "=" + word[0] + ".");

                            listForValueLabel.Add("New" + varName + word[1] + "_" + word[0]);
                        }

                        txtWriter.WriteLine("");
                        txtWriter.WriteLine("");
                    }

                    txtWriter.WriteLine("VALUE LABELS");
                    
                    for (int i = 0; i < listForValueLabel.Count; i++)
                    {
                        txtWriter.WriteLine(listForValueLabel[i]);
                    }
                    for (int i = 0; i < listOfValueLabel.Count; i++)
                    {
                        txtWriter.WriteLine(listOfValueLabel[i]);
                    }
                    txtWriter.WriteLine(".");

                    txtWriter.Close();
                    MessageBox.Show("");

                }
            }
        }

        private void btnBrowseRank_Click(object sender, RoutedEventArgs e)
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
                txtRankExcelFile.Text = openFileDialog1.FileName;
                this.loadWorkSheetInListBox();
                myPath = txtRankExcelFile.Text.Substring(0, txtRankExcelFile.Text.LastIndexOf('\\'));

                Properties.Settings.Default.StartupPath = myPath;
                Properties.Settings.Default.Save();
            }
            else
                txtRankExcelFile.Text = "";
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //}
        }

        private void loadWorkSheetInListBox()
        {
            //try
            //{
            if (File.Exists(txtRankExcelFile.Text) == true)
            {
                Excel.Application xlApp = new Excel.Application();
                Excel.Workbook xlWorkBook = xlApp.Workbooks.Open(txtRankExcelFile.Text, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);


                chkListBoxWorksheetRank.Items.Clear();
                for (int i = 1; i <= xlWorkBook.Worksheets.Count; i++)
                {
                    chkListBoxWorksheetRank.Items.Add(xlWorkBook.Worksheets[i].Name.ToString());
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
    }
}
