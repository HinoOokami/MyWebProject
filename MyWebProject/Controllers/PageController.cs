using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyWebProject.Models;

namespace MyWebProject.Controllers
{
    public class PageController : Controller
    {
        MSSQL sql;
        // GET: page
        public ActionResult Index()
        {
            sql = new MSSQL();
            Stories storyPage = new Stories(sql);
            storyPage.GenerateList("100");
            return View(storyPage);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}