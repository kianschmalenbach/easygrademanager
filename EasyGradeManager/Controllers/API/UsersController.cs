using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using EasyGradeManager.Models;
using static EasyGradeManager.Static.Authorize;

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
            if (userDTO == null || user == null || !ModelState.IsValid || id != userDTO.Id)
                return BadRequest(ModelState);
            bool logoutNecessary = userDTO.Update(user);
            string error = db.Update(user);
            if (error != null)
                return BadRequest(error);
            if (userDTO.NewRole != null)
            {
                user = db.Users.Find(id);
                userDTO.UpdateRole(user, db);
                error = db.Update(user);
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
            if (authorizedUser == null || !ModelState.IsValid || userDTO.Identifier.Contains("&") || userDTO.NewPassword.Contains("&") || 
                userDTO.NewRole == null || (userDTO.NewRole != "Teacher" && userDTO.NewRole != "Tutor" && userDTO.NewRole != "Student"))
                return BadRequest(ModelState);
            User user = new User();
            userDTO.Update(user);
            db.Users.Add(user);
            db.SaveChanges();
            userDTO.UpdateRole(user, db);
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            UserListDTO result = new UserListDTO(user);
            return CreatedAtRoute("DefaultApi", new { id = userDTO.Id }, result);
        }

        public IHttpActionResult DeleteUser(int id)
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            //if (authorizedUser == null || authorizedUser.Id != id)
            //    return Unauthorized();
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            db.SaveChanges();

            return Ok(user);
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
