using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace EasyGradeManager.Controllers.HTML
{
    public class CoursesController : Controller
    {
        public ActionResult Details(int? id)
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "\\Views\\Courses\\Index.html";
            string text = System.IO.File.ReadAllText(path);
            if (id != null)
            {
                var array = Regex.Split(text, "<head>");
                text = array[0] + "<head>\n\t<script type=\"text/javascript\">\n";
                text += "\t\tconst entityId = " + id + ";";
                text += "\n\t</script>" + array[1];
            }
            ContentResult result = Content(text, "text/html");
            Response.Headers.Add("entity-id", id.ToString());
            return result;
        }
    }
}
