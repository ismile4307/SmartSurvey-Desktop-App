using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DBI_Scripting.Classes
{
    class ConnectionDB
    {
        //public SQLiteConnection connForQues;
        //public SQLiteConnection connForAns;

        public SQLiteConnection sqlite_conn;

        public ConnectionDB()
        {
            //Its a simple constractor
        }

        public ConnectionDB(string DbPath)
        {
            try
            {
                if (DbPath != "" && File.Exists(DbPath))
                {
                    string connectionString1 = @"Data Source=" + DbPath;// @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Properties.Settings.Default.QDatabasePath + ";Jet OLEDB:Database Password=smile@sirius;";
                    sqlite_conn = new SQLiteConnection(connectionString1);

                    //string connectionString2 = @"Data Source=" + DbPath;// @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Properties.Settings.Default.ADatabasePath + ";Jet OLEDB:Database Password=smile@sirius;";
                    //connForAns = new SQLiteConnection(connectionString2);

                    //try
                    //{
                    sqlite_conn.Open();
                    //connForAns.Open();

                    //    if (connForQues.State != ConnectionState.Open || connForAns.State != ConnectionState.Open)
                    //        return false;
                    //    else
                    //    {
                    //        connForQues.Close();
                    //        connForAns.Close();
                    //        return true;
                    //    }
                }
                else
                {
                    MessageBox.Show("Database has not been connected successfully");
                    //SelectDB mySelectDB = new SelectDB();
                    //mySelectDB.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database has not been connected successfully");
                //SelectDB mySelectDB = new SelectDB();
                //mySelectDB.ShowDialog();
            }
        }

        //public bool connect(string provider, string serverName, string initialCatalog, string userId, string password, bool integratedSecurity)
        public bool connect(string DbPath)
        {
            //string connectionString = integratedSecurity ? string.Format("Provider={0};Data Source={1};Initial Catalog={2};Integrated Security=SSPI;", provider, serverName, initialCatalog) 
            //                                 : string.Format("Provider={0};Data Source={1};Initial Catalog={2};User ID={3};Password={4};", provider, serverName, initialCatalog, userId, password);
            if (DbPath != "" && File.Exists(DbPath))
            {
                string connectionString1 = @"Data Source=" + DbPath;//@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Properties.Settings.Default.QDatabasePath + ";Jet OLEDB:Database Password=smile@sirius;";
                sqlite_conn = new SQLiteConnection(connectionString1);

                //string connectionString2 = @"Data Source=" + DbPath;//@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Properties.Settings.Default.ADatabasePath + ";Jet OLEDB:Database Password=smile@sirius;";
                //connForAns = new SQLiteConnection(connectionString2);

                try
                {
                    sqlite_conn.Open();
                    //connForAns.Open();

                    if (sqlite_conn.State != ConnectionState.Open)// || connForAns.State != ConnectionState.Open)
                        return false;
                    else
                    {
                        sqlite_conn.Close();
                        //connForAns.Close();
                        return true;
                    }
                }

                catch (Exception ex)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }


        }

        public void closeConnection()
        {
            sqlite_conn.Close();
        }
    }
}
