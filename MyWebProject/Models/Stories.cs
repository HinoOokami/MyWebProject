using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.ComponentModel.DataAnnotations;

namespace MyWebProject.Models
{
    public class Stories
    {
        public string Id { get; private set; }
        [Required(ErrorMessage = "Enter story title")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Enter story text")]
        public string Story { get; set; }
        [Required(ErrorMessage = "Enter author's Email")]
        [RegularExpression(@"^([a-z0-9_-]+\.)*[a-z0-9_-]+@[a-z0-9_-]+(\.[a-z0-9_-]+)*\.[a-z]{2,6}", ErrorMessage = "Enter correct Email")]
        public string Email { get; set; }
        public string Ename { get; private set; }
        public string Post_date { get; private set; }

        public Stories[] storiesList { get; private set; }

        public string Error { get; private set; }

        MSSQL sql;

        public Stories() : this(null)
        {
        }

        public Stories(MSSQL sql)
        {
            this.sql = sql;
            Error = "";
        }

        public void GenerateList(string sLimit)
        {
            int limit = int.TryParse(sLimit, out int result) ? result : 10;
            DataTable table = sql.Select(@"SELECT TOP " + limit + @" id, title, story, email, post_date
                                           FROM Stories
                                           WHERE status = 'show'
                                           ORDER BY post_date DESC");
            try
            {
                storiesList = new Stories[table.Rows.Count];
                for (int i = 0; i < storiesList.Length; i++)
                {
                    storiesList[i] = new Stories(sql);
                    storiesList[i].GetRow(table, i);
                }
            }
            catch (Exception e)
            {
                Error = "Generate list didn't return any rows: " + e.Message;
            }
        }

        public void Add()
        {
            if ((Email ?? "").IndexOf('@') == -1)
            {
                Error = "Incorrect Email";
                return;
            }
            long insertedId = sql.Insert(@"INSERT INTO Stories (title, story, email, post_date)
                                             VALUES (N'" + sql.AddSlashes(Title) +
                                                 "', N'" + sql.AddSlashes(Story) +
                                                  "', '" + sql.AddSlashes(Email) +
                                                  "', GETDATE()); SELECT SCOPE_IDENTITY()");
            if (insertedId == -1)
            {
                Error = "Could not insert record to database";
                return;
            }

            Id = insertedId.ToString();
        }

        public void Random()
        {
            DataTable table = sql.Select(@"SELECT TOP 1 id, title, story, email, post_date
                                             FROM Stories
                                             ORDER BY NEWID()");
            GetRow(table);
        }

        public void Number(string id)
        {
            DataTable table = sql.Select(@"SELECT TOP 1 id, title, story, email, post_date
                                             FROM Stories
                                             WHERE id = '" + sql.AddSlashes(id) + "'");
            GetRow(table);
        }

        void GetRow(DataTable table, int rowNum = 0)
        {
            try
            {
                Id = table.Rows[rowNum]["id"].ToString();
                Title = table.Rows[rowNum]["title"].ToString();
                Story = table.Rows[rowNum]["story"].ToString();
                Email = table.Rows[rowNum]["email"].ToString();
                int adPos = Email.IndexOf('@');
                Ename = adPos == -1 ? Email : Email.Substring(0, adPos);
                Post_date = ((DateTime)table.Rows[rowNum]["post_date"]).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            catch (Exception e)
            {
                Id = "";
                Title = "";
                Story = "";
                Email = "";
                Ename = "";
                Post_date = "";
                Error = "Record not found";
            }
        }

        public bool IsError()
        {
            return Error != "";
        }

        public bool SelectWaitStory()
        {
            DataTable table = sql.Select(@"SELECT TOP 1 id, title, story, email, post_date
                                             FROM Stories
                                             WHERE status = 'wait'
                                             ORDER BY post_date ASC");
            if (table == null || table.Rows.Count == 0) return false;
            GetRow(table);
            return true;
        }

        public void Approve(string id)
        {
            sql.Update(@"UPDATE Stories
                         SET status = 'show'
                         WHERE id = '" + sql.AddSlashes(id) + "'");
        }

        public void Decline(string id)
        {
            sql.Update(@"UPDATE Stories
                         SET status = 'hide'
                         WHERE id = '" + sql.AddSlashes(id) + "'");
        }
    }
}