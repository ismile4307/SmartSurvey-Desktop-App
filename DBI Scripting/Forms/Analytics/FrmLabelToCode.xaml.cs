using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using SpssLib.SpssDataset;
using SpssLib.DataReader;
using System.IO;

namespace DBI_Scripting.Forms.Analytics
{
    /// <summary>
    /// Interaction logic for FrmLabelToCode.xaml
    /// </summary>
    public partial class FrmLabelToCode : Window
    {
        private string myPath;

        private SpssReader spssDataset;
        private SpssReader spssDatasetShorted;


        Dictionary<String, List<Variable>> dicNameVsVariableSingleResponse;



        Dictionary<string, Dictionary<string, string>> dicVarNameVsValueLables;

        public FrmLabelToCode()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sTemp;

                sTemp = Properties.Settings.Default.StartupPath;

                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.InitialDirectory = sTemp;
                openFileDialog1.FileName = "";
                openFileDialog1.Filter = "SPSS Data (*.*sav)|*.sav|All Files (*.*)|*.*";
                if (openFileDialog1.ShowDialog() == true)
                {
                    txtSPSSDataPath.Text = openFileDialog1.FileName;
                    myPath = txtSPSSDataPath.Text.Substring(0, txtSPSSDataPath.Text.LastIndexOf('\\'));

                    Properties.Settings.Default.StartupPath = myPath;
                    Properties.Settings.Default.Save();

                    //this.getScriptVersion();

                }
                else
                    txtSPSSDataPath.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRun_Click(object sender, RoutedEventArgs e)
        {
            this.readSPSS();
            this.segmentVariableType();
            this.writeSyntax();

            MessageBox.Show("Write Complete");
        }

        private void readSPSS()
        {
            if (txtSPSSDataPath.Text != "")
            {
                if (File.Exists(txtSPSSDataPath.Text) == true)
                {
                    // Open file, can be read only and sequetial (for performance), or anything else
                    using (FileStream fileStream = new FileStream(txtSPSSDataPath.Text, FileMode.Open, FileAccess.Read, FileShare.Read, 2048 * 10, FileOptions.SequentialScan))
                    {
                        //this.createDatFileForError(txt_SAV_Location.Text.Substring(0, txt01SAVLocation.Text.LastIndexOf('\\')) + "\\MissingVarName.TXT");

                        //this.saveSAVLocation(txt_SAV_Location.Text, txt_File_Name.Text, txtWeekNo.Text);


                        // Create the reader, this will read the file header
                        spssDataset = new SpssReader(fileStream);
                    }
                }
            }
        }

        private void segmentVariableType()
        {
            
            dicVarNameVsValueLables = new Dictionary<string, Dictionary<string, string>>();
            foreach (var variable in spssDataset.Variables)
            {
                Dictionary<string, string> dicValueVsLabel = new Dictionary<string, string>();

                string sVType = variable.Type.ToString();
                String sVName = variable.Name;

                // Single Response Numeric Type
                if (sVType == "Numeric")
                {
                    foreach (KeyValuePair<double, string> myPair in variable.ValueLabels)
                    {
                        if (myPair.Value.Trim() == "")
                        {
                            //hasBlank = true;
                            break;
                        }
                        else
                        {
                            //hasValueLabel = true;
                            dicValueVsLabel.Add(myPair.Key.ToString(), myPair.Value);
                        }
                    }

                    dicVarNameVsValueLables.Add(sVName, dicValueVsLabel);

                }
                else if (sVType == "Text")
                {
                    // Single Response Text Type
                }


            }

            //MessageBox.Show("");
        }

        private void writeSyntax()
        {
            TextWriter txtWriter = new StreamWriter(myPath + "\\LabelToCode.sps");

            foreach (KeyValuePair<String, Dictionary<string, string>> pair in dicVarNameVsValueLables)
            {
                Dictionary<string,string> dicValueVsLabel=pair.Value;
                foreach(KeyValuePair<string ,string >mypair in dicValueVsLabel )
                {
                    txtWriter.WriteLine("RECODE " + pair.Key + "  (\"" + mypair.Value + "\"=\"" + mypair.Key + "\").");
                }
            }

            txtWriter.WriteLine("");

            txtWriter.Close();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
