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

namespace DBI_Scripting.Forms.Scripting
{
    /// <summary>
    /// Interaction logic for FrmCreateLoopSyntax.xaml
    /// </summary>
    public partial class FrmCreateLoopSyntax : Window
    {
        Dictionary<string, string> dicCodeVsBrandName;
        public FrmCreateLoopSyntax()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnBuild_Click(object sender, RoutedEventArgs e)
        {
            dicCodeVsBrandName = new Dictionary<string, string>();
            if(txtCodeVsBrand.Text!="")
            {
                string[] mySyntax = txtCodeVsBrand.Text.Split('\n');

                for (int i = 0; i < mySyntax.Length; i++)
                {
                    dicCodeVsBrandName.Add(mySyntax[i].Split(':')[0], mySyntax[i].Split(':')[1].Split('\r')[0]);
                }
            }
            if(txtLoopArray.Text!="")
            { 
                if (txtScriptSyntax.Text != "")
                {
                    txtPreparedSyntax.Text = "";

                    string[] myloop = txtLoopArray.Text.Split(',');

                    string[] mySyntax = txtScriptSyntax.Text.Split('\n');

                    for (int j = 0; j < myloop.Length; j++)
                    {
                        for (int i = 0; i < mySyntax.Length; i++)
                        {
                            string myStr = mySyntax[i].ToString().Replace("XXX", myloop[j]);

                            if (myStr.Contains("YYY") && dicCodeVsBrandName.Count == myloop.Length)
                            {
                                myStr = myStr.Replace("YYY", dicCodeVsBrandName[myloop[j]]);
                            }

                            txtPreparedSyntax.AppendText(myStr);
                        }

                        txtPreparedSyntax.AppendText("\n");
                        txtPreparedSyntax.AppendText("\n");

                    }
                        //MessageBox.Show("");
                }
            }
        }
    }
}
