using DBI_Scripting.Classes;
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

namespace DBI_Scripting.Forms
{
    /// <summary>
    /// Interaction logic for FrmServerAddress.xaml
    /// </summary>
    public partial class FrmServerAddress : Window
    {
        public FrmServerAddress()
        {
            InitializeComponent();
        }

        private void frmServerAddress_Loaded(object sender, RoutedEventArgs e)
        {
            comServerAddress.Items.Add("https://smartsurveybd.com");
            comServerAddress.Items.Add("https://surveyhive.dbibd.xyz");
            comServerAddress.Items.Add("https://surveyhive.dbibd.com");
            //txtServerAddress.Text = Properties.Settings.Default.ServerAddress;
            comServerAddress.Text = StaticClass.SERVER_URL;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (comServerAddress.Text != "")
            {
                Properties.Settings.Default.ServerAddress = comServerAddress.Text;
                Properties.Settings.Default.Save();
                MessageBox.Show("Server address has been Successfully..\nNeed to restart the appliction");
                System.Windows.Application.Current.Shutdown();
            }
            else
                MessageBox.Show("Server address should not be blank");
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
