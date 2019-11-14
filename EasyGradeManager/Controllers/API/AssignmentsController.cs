using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EasyGradeManager.Models;
using static EasyGradeManager.Static.Authorize;

namespace EasyGradeManager.Controllers.API
{
    public class AssignmentsController : ApiController
    {
        private readonly EasyGradeManagerContext db = new EasyGradeManagerContext();

        public IHttpActionResult GetAssignments()
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            var result = new List<AssignmentListDTO>();
            foreach (Assignment assignment in db.Assignments)
                result.Add(new AssignmentListDTO(assignment));
            return Ok(result);
        }

        public IHttpActionResult GetAssignment(int id)
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
                return NotFound();
            Course course = assignment.Course;
            if (course == null)
                return InternalServerError();
            string accessRole = GetAccessRole(authorizedUser, course);
            if (accessRole == null)
                return Unauthorized();
            if (accessRole.Equals("Teacher") || accessRole.Equals("Tutor"))
                return Ok(new AssignmentDetailTeacherDTO(assignment));
            else
                return Ok(new AssignmentDetailStudentDTO(assignment, authorizedUser.GetStudent()));
        }

        //TODO implement
        public IHttpActionResult PutAssignment(int id, Assignment assignment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != assignment.Id)
            {
                return BadRequest();
            }

            db.Entry(assignment).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AssignmentExists(id))
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
        public IHttpActionResult PostAssignment(Assignment assignment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Assignments.Add(assignment);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = assignment.Id }, assignment);
        }

        //TODO implement
        public IHttpActionResult DeleteAssignment(int id)
        {
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
            {
                return NotFound();
            }

            db.Assignments.Remove(assignment);
            db.SaveChanges();

            return Ok(assignment);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AssignmentExists(int id)
        {
            return db.Assignments.Count(e => e.Id == id) > 0;
        }
    }
}
