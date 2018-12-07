using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace DataBaseSchema
{
    public class DataSetActions
    {
        public static Dictionary<string, SqlDataAdapter> adapters = null;
        public static string ConnectionString { get; set; }
        static List<string> tablesName;
        public static void CreateDataSet(DataSet ds, string connectionString)
        {
            DataSetActions.ConnectionString = connectionString;
            tablesName = GetTablesName(DataSetActions.ConnectionString);
            FillDataSetTables(ds, tablesName);
            FillShemaDataSet(ds, DataSetActions.ConnectionString);
        }

        public static List<string> GetTablesName(string connString)
        {
            List<string> tables = new List<string>();
            DataTable dt;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                dt = conn.GetSchema("Tables");
            }
            if (dt == null) throw new Exception("Не удалось получить список таблиц базы данных");
            foreach (DataRow row in dt.Rows)
            {
                string tablename = (string)row[2];
                tables.Add(tablename);
            }
            tables.Sort((string x, string y)=> { return x.CompareTo(y); });
            return tables;
        }

        public static void FillDataSetTables(DataSet ds, List<string> tablesName)
        {
            foreach (string tn in tablesName)
            {
                DataTable table = new DataTable(tn);
                ds.Tables.Add(table);
            }
        }
        public static void GetSchemaTable(string connString, string tableName, DataSet ds)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(string.Format("select * from [{0}]", tableName), conn);
                conn.Open();
                adapter.FillSchema(ds, SchemaType.Source, tableName);
            }
        }
        public static void FillShemaDataSet(DataSet ds, string connString)
        {
            foreach (DataTable t in ds.Tables)
            {
                GetSchemaTable(connString, t.TableName, ds);
            }
        }
        public static void FillData(DataSet ds, string connectionString, int startRecord, int maxRecords)
        {
            if (DataSetActions.adapters == null)
            {
                DataSetActions.adapters = new Dictionary<string, SqlDataAdapter>();
                adapters = GetAdapters(ds, connectionString);
            }
            foreach(DataTable t in ds.Tables)
            {
                adapters[t.TableName].Fill(startRecord, maxRecords, t);
            }
        }
        public static void FillData(DataSet ds, string connectionString)
        {
            if (DataSetActions.adapters == null) DataSetActions.adapters = GetAdapters(ds, connectionString);
            foreach (DataTable t in ds.Tables)
            {
                adapters[t.TableName].Fill(t);
            }
        }
        public static void ClearDataSet(DataSet ds)
        {
            ds.Clear();
        }
        private static Dictionary<string, SqlDataAdapter> GetAdapters(DataSet ds, string connectionString)
        {
            Dictionary<string, SqlDataAdapter> adapters = new Dictionary<string, SqlDataAdapter>();
            foreach(DataTable t in ds.Tables)
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(string.Format("select * from [{0}]", t.TableName), new SqlConnection(connectionString));
                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(adapter);
                adapters.Add(t.TableName, adapter);
            }
            return adapters;
        }
    }
}

