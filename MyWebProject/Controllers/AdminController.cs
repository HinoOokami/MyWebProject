using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyWebProject.Models;

namespace MyWebProject.Controllers
{
    public class AdminController : Controller
    {
        MSSQL sql;
        User user;
        
        public AdminController()
        {
            sql = new MSSQL();
            user = new User(sql);
        }

        // GET: Admin
        [HttpGet]
        public ActionResult Index()
        {
            return View(user);
        }

        [HttpPost]
        public ActionResult Index(User postUser)
        {
            user.Login = postUser.Login;
            user.Passw = postUser.Passw;
            user.CheckLogin();
            if (user.Status == "1") Session["admin"] = "1";
            return View(user);
        }

        public ActionResult Logout()
        {
            Session["admin"] = "";
            return View("Index", user);
            //return Redirect("~/Admin/Index");
        }

        public ActionResult Checker()
        {
            if (Session["admin"] != "1") return Redirect("Index");

            Stories story = new Stories(sql);
            ViewBag.IsChecker = story.SelectWaitStory();
            return View(story);
        }

        public ActionResult Approve()
        {
            if (Session["admin"] != "1") return Redirect("Index");

            string id = (RouteData.Values["id"] ?? "").ToString();
            Stories story = new Stories(sql);
            story.Approve(id);
            return Redirect("~/Admin/Checker");
        }

        public ActionResult Decline()
        {
            string id = (RouteData.Values["id"] ?? "").ToString();

            if (Session["admin"] != "1") return Redirect("Index");
            Stories story = new Stories(sql);
            story.Decline(id);
            return Redirect("~/Admin/Checker");
        }
    }
}