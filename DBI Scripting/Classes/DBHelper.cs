using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;

namespace DBI_Scripting.Classes
{
    class DBHelper
    {
        public DataTable getQntrTableData(String query, ConnectionDB connQntrDB)
        {
            DataTable dt = new DataTable();
            if (connQntrDB.connect(StaticClass.QDBPath) == true)
            {
                if (connQntrDB.sqlite_conn.State == ConnectionState.Closed)
                    connQntrDB.sqlite_conn.Open();

                SQLiteDataAdapter dadpt = new SQLiteDataAdapter(query, connQntrDB.sqlite_conn);
                DataSet ds = new DataSet();
                dadpt.Fill(ds, "Table1");
                dt = ds.Tables["Table1"];

                if (connQntrDB.sqlite_conn.State == ConnectionState.Open)
                    connQntrDB.sqlite_conn.Close();

                //connQntrDB = null;
            }
            return dt;
        }

        public DataTable getAnsTableData(String query, ConnectionDB connAnsDB)
        {
            DataTable dt = new DataTable();
            if (connAnsDB.connect(StaticClass.ADBPath) == true)
            {
                if (connAnsDB.sqlite_conn.State == ConnectionState.Closed)
                    connAnsDB.sqlite_conn.Open();

                SQLiteDataAdapter dadpt = new SQLiteDataAdapter(query, connAnsDB.sqlite_conn);
                DataSet ds = new DataSet();
                dadpt.Fill(ds, "Table1");
                dt = ds.Tables["Table1"];

                if (connAnsDB.sqlite_conn.State == ConnectionState.Open)
                    connAnsDB.sqlite_conn.Close();

                //connQntrDB = null;
            }
            return dt;
        }
    }
}
