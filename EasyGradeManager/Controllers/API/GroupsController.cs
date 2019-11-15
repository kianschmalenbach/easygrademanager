using EasyGradeManager.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static EasyGradeManager.Static.Authorize;

namespace EasyGradeManager.Controllers.API
{
    public class GroupsController : ApiController
    {
        private readonly EasyGradeManagerContext db = new EasyGradeManagerContext();

        public IHttpActionResult GetGroups()
        {
            return BadRequest();
        }

        public IHttpActionResult GetGroup(int id)
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            Group group = db.Groups.Find(id);
            if (group == null)
                return NotFound();
            if (group.Lesson == null || group.Lesson.Assignment == null || group.Lesson.Assignment.Course == null)
                return InternalServerError();
            Course course = group.Lesson.Assignment.Course;
            string accessRole = GetAccessRole(authorizedUser, course);
            if (accessRole == null || accessRole.Equals("Student"))
                return Unauthorized();
            return Ok(new GroupDetailTeacherDTO(group));
        }

        //TODO implement
        public IHttpActionResult PutGroup(int id, Group group)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != group.Id)
            {
                return BadRequest();
            }

            db.Entry(group).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        //TODO implement
        public IHttpActionResult PostGroup(Group group)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Groups.Add(group);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = group.Id }, group);
        }

        //TODO implement
        public IHttpActionResult DeleteGroup(int id)
        {
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return NotFound();
            }

            db.Groups.Remove(group);
            db.SaveChanges();

            return Ok(group);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GroupExists(int id)
        {
            return db.Groups.Count(e => e.Id == id) > 0;
        }
    }
}