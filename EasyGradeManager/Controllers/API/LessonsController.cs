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
    public class LessonsController : ApiController
    {
        private readonly EasyGradeManagerContext db = new EasyGradeManagerContext();

        public IHttpActionResult GetLessons()
        {
            return BadRequest();
        }

        public IHttpActionResult GetLesson(int id)
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
                return NotFound();
            if (lesson.Assignment == null || lesson.Assignment.Course == null)
                return InternalServerError();
            Course course = lesson.Assignment.Course;
            string accessRole = GetAccessRole(authorizedUser, course);
            if (accessRole == null || accessRole.Equals("Student"))
                return Unauthorized();
            return Ok(new LessonDetailDTO(lesson));
        }

        //TODO implement
        public IHttpActionResult PutLesson(int id, Lesson lesson)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != lesson.Id)
            {
                return BadRequest();
            }

            db.Entry(lesson).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LessonExists(id))
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
        public IHttpActionResult PostLesson(Lesson lesson)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Lessons.Add(lesson);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = lesson.Id }, lesson);
        }

        //TODO implement
        public IHttpActionResult DeleteLesson(int id)
        {
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
            {
                return NotFound();
            }

            db.Lessons.Remove(lesson);
            db.SaveChanges();

            return Ok(lesson);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LessonExists(int id)
        {
            return db.Lessons.Count(e => e.Id == id) > 0;
        }
    }
}
