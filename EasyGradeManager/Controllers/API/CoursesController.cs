using EasyGradeManager.Models;
using EasyGradeManager.Static;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using static System.Data.Entity.EntityState;

namespace EasyGradeManager.Controllers.API
{
    public class CoursesController : ApiController
    {
        private readonly EasyGradeManagerContext db = new EasyGradeManagerContext();

        public IHttpActionResult GetCourses()
        {
            User authorizedUser = new Authorize().GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            var result = new List<CourseListDTO>();
            foreach (Course course in db.Courses)
                result.Add(new CourseListDTO(course));
            return Ok(result);
        }

        public IHttpActionResult GetCourse(int id)
        {
            Authorize auth = new Authorize();
            User authorizedUser = auth.GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            Course course = db.Courses.Find(id);
            if (course == null)
                return NotFound();
            string accessRole = auth.GetAccessRole(authorizedUser, course);
            if (accessRole == null)
                return Ok(new CourseListDTO(course));
            return Ok(new CourseDetailDTO(course, authorizedUser.GetStudent(), authorizedUser.GetTutor(), authorizedUser.GetTeacher()));

        }

        public IHttpActionResult PutCourse(int id, CourseDetailDTO courseDTO)
        {
            Authorize auth = new Authorize();
            User authorizedUser = auth.GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            Course course = db.Courses.Find(id);
            if (courseDTO == null || course == null || !ModelState.IsValid)
                return BadRequest(ModelState);
            if (!"Teacher".Equals(auth.GetAccessRole(authorizedUser, course)))
                return Unauthorized();
            if (!courseDTO.Validate(course))
                return BadRequest();
            courseDTO.Update(course);
            string error = db.Update(course, Modified);
            if (error != null)
                return BadRequest(error);
            return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Courses/" + course.Id);
        }

        public IHttpActionResult PostCourse(CourseDetailDTO courseDTO)
        {
            Authorize auth = new Authorize();
            User authorizedUser = auth.GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null || authorizedUser.GetTeacher() == null)
                return Unauthorized();
            if (!ModelState.IsValid || !courseDTO.Validate(null))
                return BadRequest();
            Course course = courseDTO.Create(authorizedUser.GetTeacher().Id);
            string error = db.Update(course, Added);
            if (error != null)
                return BadRequest(error);
            return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Courses/" + course.Id);
        }

        public IHttpActionResult DeleteCourse(int id)
        {
            Authorize auth = new Authorize();
            User authorizedUser = auth.GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            Course course = db.Courses.Find(id);
            if (course == null)
                return NotFound();
            if (!"Teacher".Equals(auth.GetAccessRole(authorizedUser, course)))
                return Unauthorized();
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
