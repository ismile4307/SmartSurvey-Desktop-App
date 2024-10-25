using DBI_Scripting.Classes;
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

namespace DBI_Scripting.Forms.Admin
{
    /// <summary>
    /// Interaction logic for FrmLogin.xaml
    /// </summary>
    public partial class FrmLogin : Window
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (txtUserId.Text != "")
            {
                if (txtPasscode.Text != "")
                {
                    string sTemp;

                    sTemp = System.AppDomain.CurrentDomain.BaseDirectory;
                    TextWriter txtWriter = new StreamWriter(sTemp + "\\index.ini");
                    txtWriter.WriteLine(txtUserId.Text);
                    txtWriter.WriteLine(txtPasscode.Text);
                    txtWriter.Close();

                    if (StaticClass.success_check_user() == true)
                    {
                        this.Close();
                    }
                    else
                        MessageBox.Show("Invalid Credential");

                }
                else
                    MessageBox.Show("Passcode should not be blank");
            }
            else
                MessageBox.Show("User Id should not be blank");
            
        }
    }
}
