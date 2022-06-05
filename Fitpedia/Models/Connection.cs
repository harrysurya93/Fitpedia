using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace Fitpedia.Models
{
    public class Connection
    {
        private static readonly string server = "localhost";
        private static readonly string database = "fitpedia";
        private static readonly string uid = "root";
        private static readonly string password = "P@ssw0rdB1Admin";
        private static readonly string connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
        private static MySqlConnection connection = new MySqlConnection(connectionString);
        public static string ExecuteEditQuery(string query)
        {
            try
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            finally
            {
                connection.Close();
            }
        }
        public static List<dynamic> ExecuteSelectQuery(string query)
        {
            List<dynamic> dynamicDt = new List<dynamic>();
            try
            {
                connection.Open();
                DataTable dt = new DataTable();
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                da.Fill(dt);
                
                foreach (DataRow row in dt.Rows)
                {
                    dynamic dyn = new ExpandoObject();
                    dynamicDt.Add(dyn);
                    foreach (DataColumn column in dt.Columns)
                    {
                        var dic = (IDictionary<string, object>)dyn;
                        dic[column.ColumnName] = row[column];
                    }
                }
                return dynamicDt;
            }
            catch(Exception ex)
            {
                return dynamicDt;
            }
            finally
            {
                connection.Close();
            }
        }



    }
}