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
            DataSet ds = new DataSet("Adventure Works DW2012 Test");
            string connString = "Data Source=.; Initial Catalog=Adventure Works DW2012; Integrated Security=true";
            string connStringOut = "Data Source=.; Initial Catalog=Adventure Works DW2012 Test; Integrated Security=true";
            DataSetActions.CreateDataSet(ds, connString);
            //PrintDataSetSchema(ds, "D:\\Adventure Works DW2012.txt");
            //DataBaseActions.CreateDataBase(ds, connString, DataBaseActions.SetTransformToSqlTypes());
            DataSetActions.FillData(ds, connString, 0, 1); 
            DataBaseActions.WriteData(ds, connStringOut, DataSetActions.adapters); //не работает, вероятно insert
            DataSetActions.ClearDataSet(ds);
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
                    schema.AppendLine(string.Format("Column NAME: {0}, TYPE: {1}, IS UNIQUE: {2}, IS AUTO INCREMENT: {3}({4},{5})\n", c.ColumnName, c.DataType, c.Unique, c.AutoIncrement, c.AutoIncrementSeed, c.AutoIncrementStep));
                }
                schema.AppendLine(string.Format(new string('\n', 3)));
            }
            StreamWriter sw = new StreamWriter(fileName);
            sw.Write(schema.ToString().ToArray<char>());
        }

    }
}
