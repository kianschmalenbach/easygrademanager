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
            string error = db.Update(user, Modified);
            if (error != null)
                return BadRequest(error);
            if (userDTO.NewRole != null)
            {
                user = db.Users.Find(id);
                userDTO.UpdateRole(user, db);
                error = db.Update(user, Modified);
                if (error != null)
                    return BadRequest(error);
            }
            if (logoutNecessary)
                return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Logout");
            else
                return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Users/" + authorizedUser.Id);
        }

        public IHttpActionResult PostUser(UserDetailDTO userDTO)
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            if (!ModelState.IsValid || !userDTO.Validate(true))
                return BadRequest();
            User user = userDTO.Create();
            db.Users.Add(user);
            string error = db.Update(user, Added);
            if (error != null)
                return BadRequest(error);
            userDTO.UpdateRole(user, db);
            error = db.Update(user, Modified);
            if (error != null)
            {
                db.Users.Remove(user);
                string error2 = db.Update(user, Deleted);
                if (error2 != null)
                    return BadRequest(error + '\n' + error2);
                return BadRequest(error);
            }
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
            if (!user.DeleteRoles(db))
                return BadRequest();
            string error = db.Update(user, Modified);
            if (error != null)
                return BadRequest(error);
            db.Users.Remove(user);
            error = db.Update(user, Deleted);
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
