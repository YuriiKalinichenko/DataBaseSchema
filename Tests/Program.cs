using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBaseSchema;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            DataSet ds = new DataSet("DefaultDataSetName");
            string connString = "DefaultConnectionString";
            DataSetActions.CreateDataSet(ds, connString);
            //PrintDataSetSchema(ds, "D:\\MainGetonSchema.txt");
           //DataBaseActions.CreateDataBase(ds, connString, DataBaseActions.SetTransformToSqlTypes());
        }
        static void PrintDataSetSchema(DataSet ds, string fileName)
        {
            StringBuilder schema = new StringBuilder();
            schema.AppendLine(string.Format("Count tables: {0}", ds.Tables.Count));
            foreach (DataTable t in ds.Tables)
            {
                schema.AppendLine(string.Format("Table: {0}", t.TableName));
                schema.AppendLine(string.Format(new string('-', 32)));
                foreach (DataColumn c in t.Columns)
                {
                    schema.AppendLine(string.Format("Column NAME: {0}, TYPE: {1}, IS UNIQUE: {2}\n", c.ColumnName, c.DataType, c.Unique));
                }
                schema.AppendLine(string.Format(new string('\n', 3)));
            }
            StreamWriter sw = new StreamWriter(fileName);
            sw.Write(schema.ToString().ToArray<char>());
        }

    }
}
