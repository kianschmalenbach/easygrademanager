using EasyGradeManager.Models;
using System;
using System.Text.RegularExpressions;
using static EasyGradeManager.Static.Authorize;

namespace EasyGradeManager.Static
{
    public static class Webpage
    {
        public static string GetWebpage(string path, User user, int? id, object entity)
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
                bool match = false;
                foreach (Role role in user.Roles)
                {
                    if (entity == null || role.Equals("Student") || GetAllAccessRoles(user, entity).Contains(role.Name))
                    {
                        text += " \"" + role.Name + "\",";
                        match = true;
                    }
                }
                if (match)
                    text = text.Remove(text.Length - 1);
                else
                    text += "\"none\"";
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
