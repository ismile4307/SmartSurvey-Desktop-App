using DBI_Scripting.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
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

namespace DBI_Scripting.Forms
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class FrmScripting : Window
    {
        private ConnectionDB AccessConDB;

        public FrmScripting()
        {
            InitializeComponent();
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Script File"; // Default file name
            dialog.DefaultExt = ".txt"; // Default file extension
            dialog.Filter = "Script File (.db)|*.db|All Files (*.*)|*.*"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                TxtFileName.Text = dialog.FileName;
                this.loadQuestionGrid();
            }
        }

        private void Frm_Scripting_Loaded(object sender, RoutedEventArgs e)
        {


            this.loadLanguageCombo();
        }

        private void loadLanguageCombo()
        {
            ComLanguage.Items.Clear();
            ComLanguage.Items.Add("Bengali");
            ComLanguage.Items.Add("English");
        }

        private void ComLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.loadQuestionGrid();
        }

        private void loadQuestionGrid()
        {
            if (ComLanguage.Text != "")
            {
                if (TxtFileName.Text != "" && File.Exists(TxtFileName.Text))
                {
                    //Create Connection Object
                    AccessConDB = new ConnectionDB(TxtFileName.Text);

                    //MessageBox.Show("Connection Ok");
                    if (AccessConDB.sqlite_conn.State == ConnectionState.Closed)
                        AccessConDB.sqlite_conn.Open();

                    SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT QId, QDesc1, QDesc3 FROM T_Question WHERE QId<>'' AND LanguageId='2'", AccessConDB.sqlite_conn);
                    DataSet ds = new DataSet();
                    dadpt.Fill(ds, "Table1");

                    if (ds.Tables["Table1"].Rows.Count > 0)
                    {
                        GridQuestions.ItemsSource = ds.Tables["Table1"].DefaultView;

                        GridQuestions.Columns[0].Width = 60;
                        GridQuestions.Columns[1].Width = 500;

                        //dataGrid1.Columns[1].Visibility = Visibility.Collapsed; 

                        //dataGrid1.Columns.RemoveAt(1);

                    }
                    else
                    {
                        GridQuestions.ItemsSource = null;
                        //GridQuestions.Visibility = Visibility.Hidden;
                    }

                    if (AccessConDB.sqlite_conn.State == ConnectionState.Open)
                        AccessConDB.sqlite_conn.Close();
                }
            }
        }

        private void GridQuestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show("Ismile");

            TextBlock x = GridQuestions.Columns[0].GetCellContent(GridQuestions.CurrentItem) as TextBlock;
            TextBlock y = GridQuestions.Columns[2].GetCellContent(GridQuestions.CurrentItem) as TextBlock;
            if (x != null)
            {
                //MessageBox.Show(x.Text);
                if (y != null && y.Text!="")
                    loadAttribute(y.Text);
                else
                    loadAttribute(x.Text);
                loadJumpLogic(x.Text);
                loadApperenceLogic(x.Text);
                loadMessageLogic(x.Text);
                loadGridAttribute(x.Text);
                loadFilterAttribute(x.Text);
            }

            TextBlock z = GridQuestions.Columns[1].GetCellContent(GridQuestions.CurrentItem) as TextBlock;
            if (z != null && z.Text != "")
                TxtQuestionText.Text=z.Text;
            else
                TxtQuestionText.Text = "";
        }

        private void loadAttribute(string QId)
        {
            if (AccessConDB.sqlite_conn.State == ConnectionState.Closed)
                AccessConDB.sqlite_conn.Open();

            SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT AttributeLabel, AttributeValue, AttributeOrder,TakeOpenended,IsExclusive,LinkId1,LinkId2,MinValue,MaxValue,ForceAndMsgOpt FROM T_OptAttribute WHERE QId='" + QId + "' AND LanguageId='2'", AccessConDB.sqlite_conn);
            DataSet ds = new DataSet();
            dadpt.Fill(ds, "Table1");

            if (ds.Tables["Table1"].Rows.Count > 0)
            {
                GridAttribute.ItemsSource = ds.Tables["Table1"].DefaultView;

                //GridAttribute.Columns[0].Width = 100;
                GridAttribute.Columns[1].Width = 40;
                GridAttribute.Columns[2].Width = 40;
                GridAttribute.Columns[3].Width = 40;
                GridAttribute.Columns[4].Width = 40;
                GridAttribute.Columns[5].Width = 40;
                GridAttribute.Columns[6].Width = 40;
                GridAttribute.Columns[7].Width = 40;
                GridAttribute.Columns[8].Width = 40;
                GridAttribute.Columns[9].Width = 40;

                //dataGrid1.Columns[1].Visibility = Visibility.Collapsed; 

                //dataGrid1.Columns.RemoveAt(1);

            }
            else
            {
                GridAttribute.ItemsSource = null;
                //GridQuestions.Visibility = Visibility.Hidden;
            }

            if (AccessConDB.sqlite_conn.State == ConnectionState.Open)
                AccessConDB.sqlite_conn.Close();
        }

        private void loadJumpLogic(string QId)
        {
            if (AccessConDB.sqlite_conn.State == ConnectionState.Closed)
                AccessConDB.sqlite_conn.Open();

            SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT QId, LogicTypeId, IfCondition, [Then], [Else] FROM T_LogicTable WHERE QId='" + QId + "' AND LogicTypeId='3'", AccessConDB.sqlite_conn);
            DataSet ds = new DataSet();
            dadpt.Fill(ds, "Table1");

            if (ds.Tables["Table1"].Rows.Count > 0)
            {
                GridJumpLogic.ItemsSource = ds.Tables["Table1"].DefaultView;

                //GridAttribute.Columns[0].Width = 100;
                //GridAttribute.Columns[1].Width = 40;
                //GridAttribute.Columns[2].Width = 40;
                //GridAttribute.Columns[3].Width = 40;
                //GridAttribute.Columns[4].Width = 40;
                //GridAttribute.Columns[5].Width = 40;
                //GridAttribute.Columns[6].Width = 40;
                //GridAttribute.Columns[7].Width = 40;
                //GridAttribute.Columns[8].Width = 40;
                //GridAttribute.Columns[9].Width = 40;

                //dataGrid1.Columns[1].Visibility = Visibility.Collapsed; 

                //dataGrid1.Columns.RemoveAt(1);

            }
            else
            {
                GridJumpLogic.ItemsSource = null;
                //GridQuestions.Visibility = Visibility.Hidden;
            }

            if (AccessConDB.sqlite_conn.State == ConnectionState.Open)
                AccessConDB.sqlite_conn.Close();
        }

        private void loadApperenceLogic(string QId)
        {
            if (AccessConDB.sqlite_conn.State == ConnectionState.Closed)
                AccessConDB.sqlite_conn.Open();

            SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT QId, LogicTypeId, IfCondition, [Then], [Else] FROM T_LogicTable WHERE QId='" + QId + "' AND LogicTypeId='4'", AccessConDB.sqlite_conn);
            DataSet ds = new DataSet();
            dadpt.Fill(ds, "Table1");

            if (ds.Tables["Table1"].Rows.Count > 0)
            {
                GridApperenceLogic.ItemsSource = ds.Tables["Table1"].DefaultView;

                //GridAttribute.Columns[0].Width = 100;
                //GridAttribute.Columns[1].Width = 40;
                //GridAttribute.Columns[2].Width = 40;
                //GridAttribute.Columns[3].Width = 40;
                //GridAttribute.Columns[4].Width = 40;
                //GridAttribute.Columns[5].Width = 40;
                //GridAttribute.Columns[6].Width = 40;
                //GridAttribute.Columns[7].Width = 40;
                //GridAttribute.Columns[8].Width = 40;
                //GridAttribute.Columns[9].Width = 40;

                //dataGrid1.Columns[1].Visibility = Visibility.Collapsed; 

                //dataGrid1.Columns.RemoveAt(1);

            }
            else
            {
                GridApperenceLogic.ItemsSource = null;
                //GridQuestions.Visibility = Visibility.Hidden;
            }

            if (AccessConDB.sqlite_conn.State == ConnectionState.Open)
                AccessConDB.sqlite_conn.Close();
        }

        private void loadMessageLogic(string QId)
        {
            if (AccessConDB.sqlite_conn.State == ConnectionState.Closed)
                AccessConDB.sqlite_conn.Open();

            SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT QId, LogicTypeId, IfCondition, [Then], [Else] FROM T_LogicTable WHERE QId='" + QId + "' AND LogicTypeId='2'", AccessConDB.sqlite_conn);
            DataSet ds = new DataSet();
            dadpt.Fill(ds, "Table1");

            if (ds.Tables["Table1"].Rows.Count > 0)
            {
                GridMessageLogic.ItemsSource = ds.Tables["Table1"].DefaultView;

                //GridAttribute.Columns[0].Width = 100;
                //GridAttribute.Columns[1].Width = 40;
                //GridAttribute.Columns[2].Width = 40;
                //GridAttribute.Columns[3].Width = 40;
                //GridAttribute.Columns[4].Width = 40;
                //GridAttribute.Columns[5].Width = 40;
                //GridAttribute.Columns[6].Width = 40;
                //GridAttribute.Columns[7].Width = 40;
                //GridAttribute.Columns[8].Width = 40;
                //GridAttribute.Columns[9].Width = 40;

                //dataGrid1.Columns[1].Visibility = Visibility.Collapsed; 

                //dataGrid1.Columns.RemoveAt(1);

            }
            else
            {
                GridMessageLogic.ItemsSource = null;
                //GridQuestions.Visibility = Visibility.Hidden;
            }

            if (AccessConDB.sqlite_conn.State == ConnectionState.Open)
                AccessConDB.sqlite_conn.Close();
        }

        private void loadGridAttribute(string QId)
        {
            if (AccessConDB.sqlite_conn.State == ConnectionState.Closed)
                AccessConDB.sqlite_conn.Open();

            SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT AttributeLabel, AttributeValue, AttributeOrder,TakeOpenended,IsExclusive,MinValue,MaxValue,ForceAndMsgOpt FROM T_GridInfo WHERE QId='" + QId + "' AND LanguageId='2'", AccessConDB.sqlite_conn);
            DataSet ds = new DataSet();
            dadpt.Fill(ds, "Table1");

            if (ds.Tables["Table1"].Rows.Count > 0)
            {
                GridAttributeGrid.ItemsSource = ds.Tables["Table1"].DefaultView;

                //GridAttribute.Columns[0].Width = 100;
                GridAttributeGrid.Columns[1].Width = 40;
                GridAttributeGrid.Columns[2].Width = 40;
                GridAttributeGrid.Columns[3].Width = 40;
                GridAttributeGrid.Columns[4].Width = 40;
                GridAttributeGrid.Columns[5].Width = 40;
                GridAttributeGrid.Columns[6].Width = 40;
                GridAttributeGrid.Columns[7].Width = 40;

                //dataGrid1.Columns[1].Visibility = Visibility.Collapsed; 

                //dataGrid1.Columns.RemoveAt(1);

            }
            else
            {
                GridAttributeGrid.ItemsSource = null;
                //GridQuestions.Visibility = Visibility.Hidden;
            }

            if (AccessConDB.sqlite_conn.State == ConnectionState.Open)
                AccessConDB.sqlite_conn.Close();
        }

        private void loadFilterAttribute(string QId)
        {
            if (AccessConDB.sqlite_conn.State == ConnectionState.Closed)
                AccessConDB.sqlite_conn.Open();

            SQLiteDataAdapter dadpt = new SQLiteDataAdapter("SELECT QId, InheritedQId, FilterType, ExceptionalValue, LabelTakenFrom FROM T_OptAttrbFilter WHERE QId='" + QId + "'", AccessConDB.sqlite_conn);
            DataSet ds = new DataSet();
            dadpt.Fill(ds, "Table1");

            if (ds.Tables["Table1"].Rows.Count > 0)
            {
                GridFilterAttribute.ItemsSource = ds.Tables["Table1"].DefaultView;

                //GridAttribute.Columns[0].Width = 100;
                //GridAttribute.Columns[1].Width = 40;
                //GridAttribute.Columns[2].Width = 40;
                //GridAttribute.Columns[3].Width = 40;
                //GridAttribute.Columns[4].Width = 40;
                //GridAttribute.Columns[5].Width = 40;
                //GridAttribute.Columns[6].Width = 40;
                //GridAttribute.Columns[7].Width = 40;
                //GridAttribute.Columns[8].Width = 40;
                //GridAttribute.Columns[9].Width = 40;

                //dataGrid1.Columns[1].Visibility = Visibility.Collapsed; 

                //dataGrid1.Columns.RemoveAt(1);

            }
            else
            {
                GridFilterAttribute.ItemsSource = null;
                //GridQuestions.Visibility = Visibility.Hidden;
            }

            if (AccessConDB.sqlite_conn.State == ConnectionState.Open)
                AccessConDB.sqlite_conn.Close();
        }
    }
}
