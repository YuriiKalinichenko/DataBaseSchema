using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace DataBaseSchema
{
    public class DataBaseActions
    {
        public static Dictionary<Type, string> SetTransformToSqlTypes()
        {
            Dictionary<Type, string> typeDict = new Dictionary<Type, string>();
            typeDict.Add(typeof(string), "nvarchar");
            typeDict.Add(typeof(Int64), "bigint");
            typeDict.Add(typeof(Int32), "int");
            typeDict.Add(typeof(Byte), "tinyint");
            typeDict.Add(typeof(DateTime), "datetime");
            typeDict.Add(typeof(Boolean), "bit");
            typeDict.Add(typeof(Byte[]), "binary");
            typeDict.Add(typeof(Char[]), "char");
            typeDict.Add(typeof(Decimal), "decimal");
            typeDict.Add(typeof(Double), "float");
            typeDict.Add(typeof(Single), "real");
            typeDict.Add(typeof(Int16), "smallint");
            typeDict.Add(typeof(Object), "sql_variant");
            typeDict.Add(typeof(TimeSpan), "time");
            typeDict.Add(typeof(Guid), "uniqueidentifier");
            return typeDict;
        }

        public static void CreateDataBase(DataSet ds, string connectionString, Dictionary<Type, string> typeDict)
        {
            SqlCommand createCommand = new SqlCommand();
            StringBuilder commandText = new StringBuilder();
            commandText.AppendLine(string.Format("USE master; IF DB_ID(N'{0}') IS NOT NULL DROP DATABASE [{0}]; CREATE DATABASE [{0}];  ", ds.DataSetName));
            createCommand.CommandText = commandText.ToString();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                createCommand.Connection = conn;
                conn.Open();
                try
                {
                    createCommand.ExecuteNonQuery();
                }catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    MessageBox.Show(ex.Message);
                }
            }
            CreateTables(ds, connectionString, typeDict);
            MessageBox.Show(string.Format("Data base {0} has created", ds.DataSetName));
        }
        private static void CreateTables(DataSet ds, string connectionString, Dictionary<Type, string> typeDict)
        {
            SqlCommand createCommand = new SqlCommand();
            StringBuilder commandText = new StringBuilder();
            commandText.AppendLine(string.Format("USE [{0}]", ds.DataSetName));
            foreach (DataTable t in ds.Tables)
            {
                commandText.AppendLine(string.Format("IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME='{0}' AND xtype='U') drop table [{0}]", t.TableName));
                commandText.AppendLine(string.Format("create table {0} (", t.TableName));
                Console.WriteLine("Creating table: {0}", t.TableName);
                foreach (DataColumn c in t.Columns)
                {
                    commandText.Append(string.Format("[{0}] {1} ", c.ColumnName, typeDict[c.DataType]));
                    if (c.Unique) commandText.Append(string.Format(" UNIQUE "));
                    if (!c.AllowDBNull) commandText.Append(string.Format(" NOT NULL "));
                    if (c.AutoIncrement) commandText.Append(string.Format(" IDENTITY({0},{1}) ", c.AutoIncrementSeed, c.AutoIncrementStep));
                    commandText.Append(",\n");
                }
                commandText.AppendLine(")");
                createCommand.CommandText = commandText.ToString();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    createCommand.Connection = conn;
                    conn.Open();
                    try
                    { 
                        createCommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        public static void WriteData(DataSet ds, string connectionString, params Dictionary<string, SqlDataAdapter>[] adapters)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                foreach (Dictionary<string, SqlDataAdapter> dict in adapters)
                {
                    foreach(var item in dict)
                    {
                        item.Value.Update(ds.Tables[item.Key]);
                    }
                }
            }
        }



    }
}
