using EasyGradeManager.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using static EasyGradeManager.Static.Authorize;
using static System.Data.Entity.EntityState;

namespace EasyGradeManager.Controllers.API
{
    public class CoursesController : ApiController
    {
        private readonly EasyGradeManagerContext db = new EasyGradeManagerContext();

        public IHttpActionResult GetCourses()
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            var result = new List<CourseListDTO>();
            foreach (Course course in db.Courses)
                result.Add(new CourseListDTO(course));
            return Ok(result);
        }

        public IHttpActionResult GetCourse(int id)
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            Course course = db.Courses.Find(id);
            if (course == null)
                return NotFound();
            return Ok(GetAccessRole(authorizedUser, course) != null ? new CourseDetailDTO(course) : new CourseListDTO(course));
        }

        public IHttpActionResult PutCourse(int id, CourseDetailDTO courseDTO)
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            Course course = db.Courses.Find(id);
            if (courseDTO == null || course == null || !ModelState.IsValid || id != courseDTO.Id)
                return BadRequest(ModelState);
            if (!"Teacher".Equals(GetAccessRole(authorizedUser, course)))
                return Unauthorized();
            if (!courseDTO.Validate(course))
                return BadRequest();
            courseDTO.Update(course);
            string error = db.Update(course, Modified);
            if (error != null)
                return BadRequest(error);
            return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Courses/" + authorizedUser.Id);
        }

        public IHttpActionResult PostCourse(CourseDetailDTO courseDTO)
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null || authorizedUser.GetTeacher() == null)
                return Unauthorized();
            if (!ModelState.IsValid || !courseDTO.Validate(null))
                return BadRequest();
            Course course = courseDTO.Create(authorizedUser.GetTeacher().Id);
            db.Courses.Add(course);
            string error = db.Update(course, Added);
            if (error != null)
                return BadRequest(error);
            return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Courses/" + course.Id);
        }

        public IHttpActionResult DeleteCourse(int id)
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            Course course = db.Courses.Find(id);
            if (course == null)
                return NotFound();
            if (!"Teacher".Equals(GetAccessRole(authorizedUser, course)))
                return Unauthorized();
            db.Courses.Remove(course);
            string error = db.Update(course, Deleted);
            if (error != null)
                return BadRequest(error);
            return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Users/" + authorizedUser.Id);
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
