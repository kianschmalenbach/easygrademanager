using EasyGradeManager.Models;
using System.Linq;
using System.Text.RegularExpressions;

namespace EasyGradeManager.Static
{
    public class Authorize
    {
        private EasyGradeManagerContext db;
        public User GetAuthorizedUser(System.Web.HttpCookie cookie)
        {
            if (cookie == null || cookie.Value == null)
                return null;
            string[] credentials = Regex.Split(cookie.Value, "%26");
            if (credentials.Length != 3)
                return null;
            return GetAuthorizedUser(credentials[1] + "&" + credentials[2]);
        }

        public User GetAuthorizedUser(System.Net.Http.Headers.CookieHeaderValue cookie)
        {
            if (cookie == null || cookie["user"] == null || cookie["user"].Value == null)
                return null;
            string[] credentials = Regex.Split(cookie["user"].Value, "&");
            if (credentials.Length != 3)
                return null;
            return GetAuthorizedUser(credentials[1] + "&" + credentials[2]);
        }

        public User GetAuthorizedUser(string value)
        {
            if (db != null)
                db.Dispose();
            db = new EasyGradeManagerContext();
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

        public string GetPassword(string value)
        {
            string[] credentials = value.Split('&');
            if (credentials.Length != 2)
                return null;
            return credentials[1];
        }

        public string GetAccessRole(User user, Course course)
        {
            if (user.Equals(course.Teacher.User))
                return "Teacher";
            if (user.GetTutor() != null)
            {
                Tutor authorizedTutor = user.GetTutor();
                foreach (Lesson lesson in authorizedTutor.Lessons)
                {
                    if (course.Equals(lesson.Assignment.Course))
                    {
                        return "Tutor";
                    }
                }
            }
            if (user.GetStudent() != null)
            {
                Student authorizedStudent = user.GetStudent();
                foreach (GroupMembership membership in authorizedStudent.GroupMemberships)
                {
                    if (course.Equals(membership.Group.Lesson.Assignment.Course))
                    {
                        return "Student";
                    }
                }
            }
            return null;
        }
    }
}