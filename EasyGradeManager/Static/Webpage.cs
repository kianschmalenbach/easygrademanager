using EasyGradeManager.Models;
using System;
using System.Text.RegularExpressions;

namespace EasyGradeManager.Static
{
    public class Webpage
    {
        public static string GetWebpage(string path, User user, int id)
        {
            path = AppDomain.CurrentDomain.BaseDirectory + "\\Views\\" + path + "\\Index.html";
            string text = System.IO.File.ReadAllText(path);
            var array = Regex.Split(text, "<head>");
            text = array[0] + "<head>\n\t<script type=\"text/javascript\">";
            text += "\n\t\tconst userId = " + user.Id + ";";
            text += "\n\t\tconst entityId = " + id + ";";
            text += "\n\t</script>" + array[1];
            return text;
        }
    }
}