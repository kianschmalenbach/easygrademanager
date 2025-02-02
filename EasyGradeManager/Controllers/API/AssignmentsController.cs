﻿using EasyGradeManager.Models;
using EasyGradeManager.Static;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using static System.Data.Entity.EntityState;

namespace EasyGradeManager.Controllers.API
{
    public class AssignmentsController : ApiController
    {
        private readonly EasyGradeManagerContext db = new EasyGradeManagerContext();

        public IHttpActionResult GetAssignments()
        {
            return BadRequest();
        }

        public IHttpActionResult GetAssignment(int id)
        {
            Authorize auth = new Authorize();
            User authorizedUser = auth.GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
                return NotFound();
            if (assignment.Course == null)
                return InternalServerError();
            string accessRole = auth.GetAccessRole(authorizedUser, assignment);
            if (accessRole == null)
                accessRole = "Student";
            if (accessRole.Equals("Student"))
                return Ok(new AssignmentDetailStudentDTO(assignment, authorizedUser.GetStudent(), null));
            else
            {
                Tutor tutor = accessRole.Equals("Teacher") ? null : authorizedUser.GetTutor();
                return Ok(new AssignmentDetailTeacherDTO(assignment, authorizedUser.GetStudent(), tutor, authorizedUser.GetTeacher()));
            }
        }

        public IHttpActionResult PutAssignment(int id, AssignmentDetailTeacherDTO assignmentDTO)
        {
            Authorize auth = new Authorize();
            User authorizedUser = auth.GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null || authorizedUser.GetTeacher() == null)
                return Unauthorized();
            Assignment assignment = db.Assignments.Find(id);
            if (assignmentDTO == null || assignment == null || assignment.Course == null || !ModelState.IsValid)
                return BadRequest(ModelState);
            Course course = assignment.Course;
            if (!"Teacher".Equals(auth.GetAccessRole(authorizedUser, course)))
                return Unauthorized();
            if (!assignmentDTO.Validate(assignment, null))
                return BadRequest();
            assignmentDTO.Update(assignment, null);
            string error = db.Update(assignment, Modified);
            if (error != null)
                return BadRequest(error);
            return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Assignments/" + assignment.Id);
        }

        public IHttpActionResult PostAssignment(AssignmentDetailTeacherDTO assignmentDTO)
        {
            Authorize auth = new Authorize();
            User authorizedUser = auth.GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null || authorizedUser.GetTeacher() == null)
                return Unauthorized();
            Course course = db.Courses.Find(assignmentDTO.NewCourseId);
            if (!"Teacher".Equals(auth.GetAccessRole(authorizedUser, course)))
                return Unauthorized();
            Assignment derived = null;
            if (assignmentDTO.NewIsDerived)
            {
                if (assignmentDTO.NewDerivedFromName == null)
                    return BadRequest();
                foreach (Assignment otherAssignment in course.Assignments)
                {
                    if (assignmentDTO.NewDerivedFromName.Equals(otherAssignment.Name))
                    {
                        derived = otherAssignment;
                        break;
                    }
                }
                if (derived == null)
                    return BadRequest();
            }
            if (!ModelState.IsValid || course == null || !assignmentDTO.Validate(null, derived))
                return BadRequest();
            ICollection<object> objects = assignmentDTO.Create(derived);
            string error = db.UpdateAll(objects, Added);
            if (error != null)
                return BadRequest(error);
            return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Courses/" + course.Id);
        }

        public IHttpActionResult DeleteAssignment(int id)
        {
            Authorize auth = new Authorize();
            User authorizedUser = auth.GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
                return NotFound();
            Course course = assignment.Course;
            if (course == null)
                return BadRequest();
            if (!"Teacher".Equals(auth.GetAccessRole(authorizedUser, course)))
                return Unauthorized();
            string error = db.Update(assignment, Deleted);
            if (error != null)
                return BadRequest(error);
            return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Courses/" + course.Id);
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
