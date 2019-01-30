using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MyWebProject.Models;

namespace MyWebProject.Controllers
{
    public class StoryController : Controller
    {
        public ActionResult errorActionRes;
        Stories storyPage;
        MSSQL sql;

        public StoryController()
        {
            sql = new MSSQL();
            storyPage = new Stories(sql);
        }
        // GET: page
        public ActionResult Index()
        {
            if (IsError()) return errorActionRes;
            return View("Index", storyPage);
        }

        public ActionResult Random()
        {
            storyPage.Random();
            if (IsError()) return errorActionRes;
            return View(storyPage);
        }

        [HttpGet]
        public ActionResult Add()
        {
            storyPage.Title = "";
            storyPage.Story = "";
            storyPage.Email = "";
            if (IsError()) return errorActionRes;
            return View(storyPage);
        }

        [HttpPost]
        public ActionResult Add(Stories post)
        {
            if (!ModelState.IsValid) return View(post);
            storyPage.Title = post.Title;
            storyPage.Story = post.Story;
            storyPage.Email = post.Email;
            storyPage.Add();
            if (IsError()) return errorActionRes;
            return Redirect("/Story/Number/" + storyPage.Id);
        }

        public ActionResult Number()
        {
            string id = (RouteData.Values["id"] ?? "").ToString();
            if (id == "") Redirect("/Page");
            storyPage.Number(id);
            if (IsError()) return errorActionRes;
            return View(storyPage);
        }

        public bool IsError()
        {
            if (sql.IsError())
            {
                ViewBag.error = sql.Error;
                ViewBag.query = sql.Query;
                errorActionRes = View("~/Views/Error.cshtml");
                return true;
            }

            if (storyPage.IsError())
            {
                ViewBag.error = "Error: " + storyPage.Error;
                ViewBag.query = sql.Query;
                errorActionRes = View("~/Views/Error.cshtml");
                return true;
            }
            return false;
        }
    }
}