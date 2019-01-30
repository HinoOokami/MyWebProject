using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyWebProject.Models
{
    public class User
    {
        [Required(ErrorMessage = "Enter login")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Enter password")]
        public string Passw { get; set; }
        public string Status { get; private set; }

        public string Error { get; private set; }

        MSSQL sql;

        public User() : this(null)
        {
        }

        public User(MSSQL sql)
        {
            this.sql = sql;
            Status = "";
        }

        public void CheckLogin()
        {
            Status = sql.Scalar(@"SELECT COUNT (*)
                                  FROM Users
                                  WHERE login = '" + sql.AddSlashes(Login) +
                                 "' AND passw = '" + sql.AddSlashes(Passw) +
                                 "' AND status > 0");
        }
    }
}