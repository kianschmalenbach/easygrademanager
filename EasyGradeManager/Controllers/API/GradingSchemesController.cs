using EasyGradeManager.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static EasyGradeManager.Static.Authorize;
using static System.Data.Entity.EntityState;

namespace EasyGradeManager.Controllers.API
{
    public class GradingSchemesController : ApiController
    {
        private readonly EasyGradeManagerContext db = new EasyGradeManagerContext();

        public IHttpActionResult GetGradingSchemes()
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null || authorizedUser.GetTeacher() == null)
                return Unauthorized();
            var result = new List<GradingSchemeDTO>();
            foreach (GradingScheme scheme in db.GradingSchemes)
                result.Add(new GradingSchemeDTO(scheme));
            return Ok(result);
        }

        public IHttpActionResult GetGradingScheme(int id)
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null || authorizedUser.GetTeacher() == null)
                return Unauthorized();
            GradingScheme scheme = db.GradingSchemes.Find(id);
            if (scheme == null)
                return NotFound();
            return Ok(new GradingSchemeDTO(scheme));
        }

        public IHttpActionResult PutGradingScheme(int id, GradingSchemeDTO schemeDTO)
        {
            return BadRequest();
        }

        public IHttpActionResult PostGradingScheme(GradingSchemeDTO schemeDTO)
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null || authorizedUser.GetTeacher() == null)
                return Unauthorized();
            Teacher teacher = authorizedUser.GetTeacher();
            Course course = db.Courses.Find(schemeDTO.NewCourseId);
            if (!ModelState.IsValid || !schemeDTO.Validate(teacher) || course == null)
                return BadRequest();
            GradingScheme scheme = schemeDTO.Create(teacher);
            db.GradingSchemes.Add(scheme);
            string error = db.Update(scheme, Added);
            if (error != null)
                return BadRequest(error);
            return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Courses/" + course.Id);
        }

        public IHttpActionResult DeleteGradingScheme(int id)
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null || authorizedUser.GetTeacher() == null)
                return Unauthorized();
            GradingScheme scheme = db.GradingSchemes.Find(id);
            if (scheme == null)
                return NotFound();
            bool authorized = scheme.Courses.Count == 0;
            if (!authorized)
            {
                foreach (Course course in scheme.Courses)
                {
                    if ("Teacher".Equals(GetAccessRole(authorizedUser, course)))
                    {
                        authorized = true;
                        break;
                    }
                }
            }
            if (!authorized)
                return Unauthorized();
            ICollection<object> entities = new HashSet<object>();
            foreach (Grade grade in scheme.Grades)
                entities.Add(grade);
            entities.Add(scheme);
            string error = db.UpdateAll(entities, Deleted);
            if (error != null)
                return BadRequest(error);
            return StatusCode(HttpStatusCode.NoContent);
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
