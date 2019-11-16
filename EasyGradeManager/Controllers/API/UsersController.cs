using EasyGradeManager.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using static EasyGradeManager.Static.Authorize;
using static System.Data.Entity.EntityState;

namespace EasyGradeManager.Controllers.API
{
    public class UsersController : ApiController
    {
        private readonly EasyGradeManagerContext db = new EasyGradeManagerContext();

        public IHttpActionResult GetUsers()
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            var result = new List<UserListDTO>();
            foreach (User user in db.Users)
                result.Add(new UserListDTO(user));
            return Ok(result);
        }

        public IHttpActionResult GetUser(int id)
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            User user = db.Users.Find(id);
            if (user == null)
                return NotFound();
            return Ok(user.Equals(authorizedUser) ? new UserDetailDTO(user) : new UserListDTO(user));
        }

        public IHttpActionResult PutUser(int id, UserDetailDTO userDTO)
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null || authorizedUser.Id != id)
                return Unauthorized();
            User user = db.Users.Find(id);
            if (userDTO == null || user == null || !ModelState.IsValid || id != userDTO.Id || !userDTO.Validate(false))
                return BadRequest(ModelState);
            bool logoutNecessary = userDTO.Update(user);
            if (userDTO.NewRole != null)
            {
                if (authorizedUser.GetTeacher() == null)
                    return BadRequest();
                userDTO.UpdateRole(user);
            }
            string error = db.Update(user, Modified);
            if (error != null)
                return BadRequest(error);
            if (logoutNecessary)
                return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Logout");
            else
                return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Users/" + authorizedUser.Id);
        }

        public IHttpActionResult PostUser(UserDetailDTO userDTO)
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (!ModelState.IsValid || !userDTO.Validate(true))
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
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
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
