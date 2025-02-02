﻿using EasyGradeManager.Models;
using EasyGradeManager.Static;
using System;
using System.Web.Mvc;
using static EasyGradeManager.Static.Initialize;
using static EasyGradeManager.Static.Webpage;

namespace EasyGradeManager.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Details(int? id)
        {
            User user = new Authorize().GetAuthorizedUser(Request.Cookies["user"]);
            string text = GetWebpage("Home", user, id, null);
            ContentResult result = Content(text, "text/html");
            return result;
        }

        public ActionResult Init()
        {
            InitializeDatabase();
            return new RedirectResult("/");
        }

        public ActionResult Logout()
        {
            if (Request.Cookies["user"] != null)
                Response.Cookies["user"].Expires = DateTime.Now.AddDays(-1);
            return new RedirectResult("/");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}