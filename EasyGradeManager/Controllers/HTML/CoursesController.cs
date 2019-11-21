﻿using EasyGradeManager.Models;
using EasyGradeManager.Static;
using System.Web.Mvc;
using static EasyGradeManager.Static.Webpage;

namespace EasyGradeManager.Controllers.HTML
{
    public class CoursesController : Controller
    {
        public ActionResult Details(int? id)
        {
            User user = new Authorize().GetAuthorizedUser(Request.Cookies["user"]);
            if (user == null || id == null)
                return new RedirectResult("/");
            string text = GetWebpage("Courses", user, (int)id);
            ContentResult result = Content(text, "text/html");
            return result;
        }
    }
}
