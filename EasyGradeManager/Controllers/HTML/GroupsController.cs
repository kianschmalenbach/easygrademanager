using EasyGradeManager.Models;
using System.Web.Mvc;
using static EasyGradeManager.Static.Authorize;
using static EasyGradeManager.Static.Webpage;

namespace EasyGradeManager.Controllers.HTML
{
    public class GroupsController : Controller
    {
        public ActionResult Details(int? id)
        {
            User user = GetAuthorizedUser(Request.Cookies["user"]);
            if (user == null || id == null)
                return new RedirectResult("/");
            string text = GetWebpage("Groups", user, (int)id);
            ContentResult result = Content(text, "text/html");
            return result;
        }
    }
}
