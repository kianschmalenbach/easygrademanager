using EasyGradeManager.Models;
using System;
using System.Text.RegularExpressions;

namespace EasyGradeManager.Static
{
    public class Webpage
    {
        public static string GetWebpage(string path, User user, int? id)
        {
            path = AppDomain.CurrentDomain.BaseDirectory + "\\Views\\" + path + "\\Index.html";
            string text = System.IO.File.ReadAllText(path);
            var array = Regex.Split(text, "<head>");
            text = array[0] + "<head>\n\t<script type=\"text/javascript\">";
            if (user != null)
            {
                text += "\n\t\tconst authorizedUser = {";
                text += "\n\t\t\tId: " + user.Id + ",";
                text += "\n\t\t\tIdentifier: \"" + user.Identifier + "\",";
                text += "\n\t\t\tName: \"" + user.Name + "\",";
                text += "\n\t\t\tRoles: [ ";
                foreach (Role role in user.Roles)
                    text += "\"" + role.Name + "\", ";
                text = text.Remove(text.Length - 2);
                text += " ]";
                text += "\n\t\t};";
            }
            if (id != null)
                text += "\n\t\tconst entityId = " + id + ";";
            text += "\n\t</script>" + array[1];
            return text;
        }
    }
}