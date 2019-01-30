using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace MyWebProject.Models
{
    public class MSSQL
    {
        SqlConnection sqlConnection;
        public string Error { get; private set; }
        public string Query { get; private set; }

        public MSSQL()
        {
            try
            {
                Error = "";
                Query = "Open connection to MSSQL";
                sqlConnection =
                    new SqlConnection(WebConfigurationManager.ConnectionStrings["myConnStr"].ConnectionString);
                sqlConnection.Open();
            }
            catch (Exception e)
            {
                Error = e.Message;
                sqlConnection = null;
            }
        }

        ~MSSQL()
        {
            try
            {
                sqlConnection.Close();
            }
            catch (Exception e)
            {
            }
        }

        public DataTable Select(string sqlQuery)
        {
            if (IsError()) return null;
            try
            {
                Query = sqlQuery;
                DataTable table = new DataTable();
                SqlCommand cmd = new SqlCommand(sqlQuery, sqlConnection);
                SqlDataReader reader = cmd.ExecuteReader();
                table.Load(reader);
                return table;
            }
            catch (Exception e)
            {
                Error = e.Message;
                return null;
            }
        }

        public string Scalar(string sqlQuery)
        {
            if (IsError()) return null;
            try
            {
                Query = sqlQuery;
                DataTable table = new DataTable();
                SqlCommand cmd = new SqlCommand(sqlQuery, sqlConnection);
                SqlDataReader reader = cmd.ExecuteReader();
                table.Load(reader);
                if (table.Rows.Count == 0) return "";
                return table.Rows[0][0].ToString();
            }
            catch (Exception e)
            {
                Error = e.Message;
                return null;
            }
        }

        public bool IsError()
        {
            return Error != "";
        }

        public string AddSlashes(string text)
        {
            return text.Replace("\'", "\\\'");
        }

        public long Insert(string sqlQuery)
        {
            if (IsError()) return -1;
            try
            {
                Query = sqlQuery;
                SqlCommand cmd = new SqlCommand(sqlQuery, sqlConnection);
                return Convert.ToInt64(cmd.ExecuteScalar());
            }
            catch (Exception e)
            {
                Error = e.Message;
                return -1;
            }
        }

        public long Update(string sqlQuery)
        {
            if (IsError()) return -1;
            try
            {
                Query = sqlQuery;
                SqlCommand cmd = new SqlCommand(sqlQuery, sqlConnection);
                return Convert.ToInt64(cmd.ExecuteScalar());
            }
            catch (Exception e)
            {
                Error = e.Message;
                return -1;
            }
        }
    }
}