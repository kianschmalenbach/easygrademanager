using EasyGradeManager.Models;
using EasyGradeManager.Static;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using static System.Data.Entity.EntityState;

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
            Authorize auth = new Authorize();
            User authorizedUser = auth.GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
                return NotFound();
            if (lesson.Assignment == null || lesson.Assignment.Course == null)
                return InternalServerError();
            string accessRole = auth.GetAccessRole(authorizedUser, lesson);
            if (accessRole == null || accessRole.Equals("Student"))
                return Unauthorized();
            return Ok(new LessonDetailDTO(lesson));
        }

        public IHttpActionResult PutLesson(int id, LessonDetailDTO lessonDTO)
        {
            Authorize auth = new Authorize();
            User authorizedUser = auth.GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null || authorizedUser.GetTeacher() == null)
                return Unauthorized();
            Lesson lesson = db.Lessons.Find(id);
            if (lessonDTO == null || lesson == null || lesson.Assignment == null || lesson.Assignment.Course == null ||
                !ModelState.IsValid)
                return BadRequest(ModelState);
            if (!"Teacher".Equals(auth.GetAccessRole(authorizedUser, lesson)))
                return Unauthorized();
            if (!lessonDTO.Validate(lesson, null, null, db.Tutors))
                return BadRequest();
            lessonDTO.Update(lesson, null);
            string error = db.Update(lesson, Modified);
            if (error != null)
                return BadRequest(error);
            return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Lessons/" + lesson.Id);
        }

        public IHttpActionResult PostLesson(LessonDetailDTO lessonDTO)
        {
            Authorize auth = new Authorize();
            User authorizedUser = auth.GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null || authorizedUser.GetTeacher() == null)
                return Unauthorized();
            Assignment assignment = db.Assignments.Find(lessonDTO.NewAssignmentId);
            if (!ModelState.IsValid || assignment == null || assignment.Course == null ||
                !lessonDTO.Validate(null, db.Lessons, assignment, db.Tutors))
                return BadRequest();
            if (!"Teacher".Equals(auth.GetAccessRole(authorizedUser, assignment)))
                return Unauthorized();
            Lesson lesson = lessonDTO.Create(db.Lessons);
            string error = db.Update(lesson, Added);
            if (error != null)
                return BadRequest(error);
            return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Lessons/" + lesson.Id);
        }

        public IHttpActionResult DeleteLesson(int id)
        {
            Authorize auth = new Authorize();
            User authorizedUser = auth.GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            Lesson lesson = db.Lessons.Find(id);
            if (lesson == null)
                return NotFound();
            if (lesson.Assignment == null || lesson.Assignment.Course == null)
                return BadRequest();
            if (!"Teacher".Equals(auth.GetAccessRole(authorizedUser, lesson)))
                return Unauthorized();
            int assignmentId = lesson.Assignment.Id;
            string error = db.Update(lesson, Deleted);
            if (error != null)
                return BadRequest(error);
            return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Assignments/" + assignmentId);
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
