using EasyGradeManager.Models;
using EasyGradeManager.Static;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using static System.Data.Entity.EntityState;

namespace EasyGradeManager.Controllers.API
{
    public class UsersController : ApiController
    {
        private readonly EasyGradeManagerContext db = new EasyGradeManagerContext();

        public IHttpActionResult GetUsers()
        {
            User authorizedUser = new Authorize().GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            var result = new List<UserListDTO>();
            foreach (User user in db.Users)
                result.Add(new UserListDTO(user));
            return Ok(result);
        }

        public IHttpActionResult GetUser(int id)
        {
            User authorizedUser = new Authorize().GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            User user = db.Users.Find(id);
            if (user == null)
                return NotFound();
            if (!user.Equals(authorizedUser))
                return Ok(new UserListDTO(user));
            ICollection<Course> ownCourses = new HashSet<Course>();
            if (user.GetTeacher() != null)
            {
                foreach (Course course in user.GetTeacher().Courses)
                    ownCourses.Add(course);
            }
            if (user.GetTutor() != null)
            {
                foreach (Lesson lesson in user.GetTutor().Lessons)
                    ownCourses.Add(lesson.Assignment.Course);
            }
            if (user.GetStudent() != null)
            {
                foreach (GroupMembership membership in user.GetStudent().GroupMemberships)
                    ownCourses.Add(membership.Group.Lesson.Assignment.Course);
            }
            ICollection<Course> courses = new HashSet<Course>();
            foreach (Course course in db.Courses)
            {
                if (!ownCourses.Contains(course) && !course.IsFinal() && !course.Archived)
                    courses.Add(course);
            }
            return Ok(new UserDetailDTO(user, courses));
        }

        public IHttpActionResult PutUser(int id, UserDetailDTO userDTO)
        {
            User authorizedUser = new Authorize().GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (userDTO == null)
                return BadRequest();
            if (authorizedUser == null)
                return Unauthorized();
            if (userDTO.NewUserIdentifier != null)
            {
                User otherUser = new Authorize().GetUserByIdentifier(userDTO.NewUserIdentifier);
                if (otherUser == null)
                    return NotFound();
                id = otherUser.Id;
            }
            User user = db.Users.Find(id);
            if (user == null || !ModelState.IsValid || !userDTO.Validate(false, authorizedUser.Id != id ? authorizedUser : null))
                return BadRequest(ModelState);
            bool logoutNecessary = false;
            if (authorizedUser.Id == id)
                userDTO.Update(user);
            if (userDTO.NewRole != null)
            {
                if (authorizedUser.GetTeacher() == null)
                    return Unauthorized();
                userDTO.UpdateRole(user);
            }
            string error = db.Update(user, Modified);
            if (error != null)
                return BadRequest(error);
            if (logoutNecessary)
                return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Logout");
            return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Users/" + authorizedUser.Id);
        }

        public IHttpActionResult PostUser(UserDetailDTO userDTO)
        {
            User authorizedUser = new Authorize().GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (!ModelState.IsValid || !userDTO.Validate(true, null))
                return BadRequest();
            if (authorizedUser == null && !userDTO.NewRole.Equals("Student"))
                return Unauthorized();
            User user = userDTO.Create();
            userDTO.UpdateRole(user);
            string error = db.Update(user, Added);
            if (error != null)
                return BadRequest(error);
            UserListDTO result = new UserListDTO(user);
            return CreatedAtRoute("DefaultApi", new { id = userDTO.Id }, result);
        }

        public IHttpActionResult DeleteUser(int id)
        {
            User authorizedUser = new Authorize().GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null || authorizedUser.Id != id)
                return Unauthorized();
            User user = db.Users.Find(id);
            if (user == null)
                return NotFound();
            if ((user.GetTeacher() != null && user.GetTeacher().Courses.Count > 0) ||
                (user.GetTutor() != null && user.GetTutor().Lessons.Count > 0) ||
                (user.GetStudent() != null && user.GetStudent().GroupMemberships.Count > 0))
                return BadRequest();
            ICollection<object> entities = new HashSet<object>();
            foreach (Role role in user.Roles)
                entities.Add(role);
            entities.Add(user);
            string error = db.UpdateAll(entities, Deleted);
            if (error != null)
                return BadRequest(error);
            return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Logout");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
