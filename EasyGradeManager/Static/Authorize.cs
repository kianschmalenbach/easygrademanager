using EasyGradeManager.Models;
using System.Collections.Generic;
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
            string[] credentials = value.Split('&');
            if (credentials.Length != 2)
                return null;
            string identifier = credentials[0];
            string password = credentials[1];
            User user = GetUserByIdentifier(identifier);
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

        public User GetUserByIdentifier(string identifier)
        {
            if (db != null)
                db.Dispose();
            db = new EasyGradeManagerContext();
            return (from u in db.Users
                    where u.Identifier.Equals(identifier)
                    select u).FirstOrDefault();
        }

        public string GetAccessRole(User user, object entity)
        {
            IList<string> accessRoles = GetAllAccessRoles(user, entity);
            if (accessRoles.Count > 0)
                return accessRoles[0];
            else
                return null;
        }

        public static IList<string> GetAllAccessRoles(User user, object entity)
        {
            IList<string> accessRoles = new List<string>();
            if (entity == null)
            {
                foreach (Role role in user.Roles)
                    accessRoles.Add(role.Name);
                return accessRoles;
            }
            switch (entity.GetType().BaseType.Name)
            {
                case "Course":
                    Course course = (Course)entity;
                    if (user.Equals(course.Teacher.User))
                        accessRoles.Add("Teacher");
                    if (user.GetTutor() != null)
                    {
                        Tutor authorizedTutor = user.GetTutor();
                        foreach (Lesson otherLesson in authorizedTutor.Lessons)
                        {
                            if (course.Equals(otherLesson.Assignment.Course))
                            {
                                accessRoles.Add("Tutor");
                                break;
                            }
                        }
                    }
                    if (user.GetStudent() != null)
                        accessRoles.Add("Student");
                    return accessRoles;

                case "Assignment":
                    Assignment assignment = (Assignment)entity;
                    if (user.Equals(assignment.Course.Teacher.User))
                        accessRoles.Add("Teacher");
                    if (user.GetTutor() != null)
                    {
                        Tutor authorizedTutor = user.GetTutor();
                        foreach (Lesson otherLesson in authorizedTutor.Lessons)
                        {
                            if (assignment.Equals(otherLesson.Assignment))
                            {
                                accessRoles.Add("Tutor");
                                break;
                            }
                        }
                    }
                    if (user.GetStudent() != null)
                    {
                        Student authorizedStudent = user.GetStudent();
                        foreach (GroupMembership membership in authorizedStudent.GroupMemberships)
                        {
                            if (membership.Group.Lesson.Assignment.Equals(assignment))
                            {
                                accessRoles.Add("Student");
                                break;
                            }
                        }
                    }
                    return accessRoles;

                case "Lesson":
                    Lesson lesson = (Lesson)entity;
                    if (user.Equals(lesson.Assignment.Course.Teacher.User))
                        accessRoles.Add("Teacher");
                    if (user.GetTutor() != null)
                    {
                        Tutor authorizedTutor = user.GetTutor();
                        if (authorizedTutor.Equals(lesson.Tutor))
                            accessRoles.Add("Tutor");
                    }
                    if (user.GetStudent() != null)
                    {
                        Student authorizedStudent = user.GetStudent();
                        foreach (GroupMembership membership in authorizedStudent.GroupMemberships)
                        {
                            if (lesson.Equals(membership.Group.Lesson))
                            {
                                accessRoles.Add("Student");
                                break;
                            }
                        }
                    }
                    return accessRoles;

                case "Group":
                    Models.Group group = (Models.Group)entity;
                    if (user.Equals(group.Lesson.Assignment.Course.Teacher.User))
                        accessRoles.Add("Teacher");
                    if (user.GetTutor() != null)
                    {
                        Tutor authorizedTutor = user.GetTutor();
                        if (authorizedTutor.Equals(group.Lesson.Tutor))
                            accessRoles.Add("Tutor");
                    }
                    if (user.GetStudent() != null)
                    {
                        Student authorizedStudent = user.GetStudent();
                        foreach (GroupMembership membership in authorizedStudent.GroupMemberships)
                        {
                            if (group.Equals(membership.Group))
                            {
                                accessRoles.Add("Student");
                                break;
                            }
                        }
                    }
                    return accessRoles;

                default:
                    return accessRoles;
            }
        }
    }
}
