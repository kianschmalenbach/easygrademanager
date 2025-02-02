﻿using EasyGradeManager.Models;
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
            Tutor tutor = null;
            if (lessonDTO.NewTutorIdentifier != null)
            {
                User user = auth.GetUserByIdentifier(lessonDTO.NewTutorIdentifier);
                if (user == null && user.GetTutor() == null)
                    return BadRequest();
                tutor = user.GetTutor();
            }
            if (!lessonDTO.Validate(lesson, null, tutor))
                return BadRequest();
            lessonDTO.Update(lesson, tutor);
            string error = db.Update(lesson, Modified);
            if (error != null)
                return BadRequest(error);
            return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Assignments/" + lesson.Assignment.Id);
        }

        public IHttpActionResult PostLesson(LessonDetailDTO lessonDTO)
        {
            Authorize auth = new Authorize();
            User authorizedUser = auth.GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null || authorizedUser.GetTeacher() == null)
                return Unauthorized();
            Assignment assignment = db.Assignments.Find(lessonDTO.NewAssignmentId);
            if (lessonDTO.NewTutorIdentifier == null || assignment == null || assignment.Course == null)
                return BadRequest();
            if (!"Teacher".Equals(auth.GetAccessRole(authorizedUser, assignment)))
                return Unauthorized();
            User user = auth.GetUserByIdentifier(lessonDTO.NewTutorIdentifier);
            if (user == null && user.GetTutor() == null)
                return BadRequest();
            Tutor tutor = user.GetTutor();
            if (!ModelState.IsValid || !lessonDTO.Validate(null, assignment, tutor))
                return BadRequest();
            Lesson lesson = lessonDTO.Create(tutor);
            string error = db.Update(lesson, Added);
            if (error != null)
                return BadRequest(error);
            return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Assignments/" + lesson.Assignment.Id);
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
