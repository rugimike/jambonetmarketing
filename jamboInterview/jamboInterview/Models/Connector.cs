using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Serialization;

namespace jamboInterview.Models
{
    public class Connector
    {
        string xx = "";
        SqlCommand myCommand;
        SqlConnection Conn  = new SqlConnection("Data Source = 127.0.0.1; Initial Catalog = jamboPayBdb;  Persist Security Info=True;User ID = sa;Password=njoroge54;Connection Timeout=30; Connection Lifetime=0;Min Pool Size=20;Max Pool Size=100;Pooling=true;");
        public DataTable NormalSelect(string SqlString)
        {
          
            SqlDataAdapter sda = new SqlDataAdapter(SqlString, Conn);
            DataTable dt = new DataTable();
            try
            {
                Conn.Open();
                sda.Fill(dt);
            }
            catch (SqlException se)
            {
                xx = se.Message;
            }
            finally
            {
                Conn.Close();
            }
            return dt;
        }

        public string GetJsonString(string SqlString)
        {
            SqlDataAdapter sda = new SqlDataAdapter(SqlString, Conn);
            DataTable dt = new DataTable();
            try
            {
                Conn.Open();
                sda.Fill(dt);
            }
            catch (SqlException se)
            {
                xx = se.Message;
            }
            finally
            {
                Conn.Close();
            }
            string jsonString = ConvertDataTableToJSON(dt);
            return jsonString;
        }

        public string ConvertDataTableToJSON(DataTable dt)
        {
            JavaScriptSerializer jSonString = new JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return jSonString.Serialize(rows);
        }

       
        public string PushDataToDb(string postquery)
        {
            string output = "Method PushDataToDb not invoked";
            try
            {
                Conn.Open();
                myCommand = new SqlCommand(postquery, Conn);
                myCommand.ExecuteNonQuery();
                Conn.Close();
                output = "Data Successfully Saved";
            }
            catch (Exception exe)
            {
                output = "Error " + exe.Message;
            }
            return output;
        }

      
       
    }
}