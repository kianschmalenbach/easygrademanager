using EasyGradeManager.Models;
using EasyGradeManager.Static;
using System.Web.Mvc;
using static EasyGradeManager.Static.Webpage;

namespace EasyGradeManager.Controllers.HTML
{
    public class UsersController : Controller
    {
        public ActionResult Details(int? id)
        {
            User user = new Authorize().GetAuthorizedUser(Request.Cookies["user"]);
            if (user == null || id == null || user.Id != id)
                return new RedirectResult("/");
            string text = GetWebpage("Users", user, (int)id, null);
            ContentResult result = Content(text, "text/html");
            return result;
        }
    }
}
