﻿using EasyGradeManager.Models;
using EasyGradeManager.Static;
using System.Web.Mvc;
using static EasyGradeManager.Static.Webpage;

namespace EasyGradeManager.Controllers.HTML
{
    public class AssignmentsController : Controller
    {
        private readonly EasyGradeManagerContext db = new EasyGradeManagerContext();
        public ActionResult Details(int? id)
        {
            User user = new Authorize().GetAuthorizedUser(Request.Cookies["user"]);
            if (user == null || id == null)
                return new RedirectResult("/");
            string text = GetWebpage("Assignments", user, (int)id, db.Assignments.Find(id));
            ContentResult result = Content(text, "text/html");
            return result;
        }
    }
}
