using EasyGradeManager.Models;
using System.Linq;
using System.Text.RegularExpressions;

namespace EasyGradeManager.Static
{
    public static class Authorize
    {
        private static readonly EasyGradeManagerContext db = new EasyGradeManagerContext();
        public static User GetAuthorizedUser(System.Web.HttpCookie cookie)
        {
            if (cookie == null || cookie.Value == null)
                return null;
            string[] credentials = Regex.Split(cookie.Value, "%26");
            if (credentials.Length != 3)
                return null;
            return GetAuthorizedUser(credentials[1] + "&" + credentials[2]);
        }

        public static User GetAuthorizedUser(string value)
        {
            string[] credentials = value.Split('&');
            if (credentials.Length != 2)
                return null;
            string identifier = credentials[0];
            string password = credentials[1];
            var user = (from u in db.Users
                        where u.Identifier.Equals(identifier)
                        select u).FirstOrDefault();
            if (user != null && SecurePasswordHasher.Verify(password, user.Password))
                return user;
            return null;
        }

        public static string GetPassword(string value)
        {
            string[] credentials = value.Split('&');
            if (credentials.Length != 2)
                return null;
            return credentials[1];
        }
    }
}