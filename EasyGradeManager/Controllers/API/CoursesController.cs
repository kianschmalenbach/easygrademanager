using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using EasyGradeManager.Models;
using static EasyGradeManager.Static.Authorize;

namespace EasyGradeManager.Controllers.API
{
    public class CoursesController : ApiController
    {
        private readonly EasyGradeManagerContext db = new EasyGradeManagerContext();

        // GET: api/Courses
        public IHttpActionResult GetCourses()
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            var result = new List<CourseListDTO>();
            foreach (Course course in db.Courses)
            {
                result.Add(new CourseListDTO()
                {
                    Id = course.Id,
                    Name = course.Name,
                    Term = course.Term,
                    Archived = course.Archived
                });
            }
            return Ok(result);
        }

        // GET: api/Courses/5
        public IHttpActionResult GetCourse(int id)
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            Course course = db.Courses.Find(id);
            if (course == null)
                return NotFound();
            bool authorized = authorizedUser.Equals(course.Teacher.User);
            if (!authorized && authorizedUser.GetTutor() != null) {
                Tutor authorizedTutor = authorizedUser.GetTutor();
                foreach(Lesson lesson in authorizedTutor.Lessons)
                {
                    if(course.Equals(lesson.Assignment.Course))
                    {
                        authorized = true;
                        break;
                    }
                }
            }
            if (!authorized && authorizedUser.GetStudent() != null)
            {
                Student authorizedStudent = authorizedUser.GetStudent();
                foreach (GroupMembership membership in authorizedStudent.GroupMemberships)
                {
                    if (course.Equals(membership.Group.Lesson.Assignment.Course))
                    {
                        authorized = true;
                        break;
                    }
                }
            }
            if(authorized)
            {
                CourseDetailDTO result = new CourseDetailDTO()
                {
                    Id = course.Id,
                    Name = course.Name,
                    Term = course.Term,
                    Archived = course.Archived,
                    MinRequiredAssignments = course.MinRequiredAssignments,
                    MinRequiredScore = course.MinRequiredScore,
                    Teacher = new UserListDTO()
                    {
                        Id = course.Teacher.User.Id,
                        Identifier = course.Teacher.User.Identifier,
                        Name = course.Teacher.User.Name
                    }
                };
                if (course.GradingScheme != null)
                {
                    GradingSchemeDTO gradingScheme = new GradingSchemeDTO()
                    {
                        Id = course.GradingScheme.Id,
                        Name = course.GradingScheme.Name
                    };
                    foreach(Grade grade in course.GradingScheme.Grades)
                    {
                        gradingScheme.Grades.Add(new GradeDTO()
                        { 
                            Id = grade.Id,
                            MinPercentage = grade.MinPercentage,
                            Name = grade.Name
                        });
                    }
                    result.GradingScheme = gradingScheme;
                }
                foreach (Assignment assignment in course.Assignments)
                {
                    result.Assignments.Add(new AssignmentListDTO()
                    {
                        Id = assignment.Id,
                        Deadline = assignment.Deadline,
                        IsFinal = assignment.IsFinal,
                        Mandatory = assignment.Mandatory,
                        MaxGroupSize = assignment.MaxGroupSize,
                        MinGroupSize = assignment.MinGroupSize,
                        MinRequiredScore = assignment.MinRequiredScore,
                        Name = assignment.Name,
                        Number = assignment.Number,
                        Weight = assignment.Weight
                    });
                }
                return Ok(result);
            }
            else
            {
                CourseListDTO result = new CourseListDTO()
                {
                    Id = course.Id,
                    Name = course.Name,
                    Term = course.Term,
                    Archived = course.Archived
                };
                return Ok(result);
            }
        }

        // PUT: api/Courses/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCourse(int id, Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != course.Id)
            {
                return BadRequest();
            }

            db.Entry(course).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(id))
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

        // POST: api/Courses
        [ResponseType(typeof(Course))]
        public IHttpActionResult PostCourse(Course course)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Courses.Add(course);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = course.Id }, course);
        }

        // DELETE: api/Courses/5
        [ResponseType(typeof(Course))]
        public IHttpActionResult DeleteCourse(int id)
        {
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return NotFound();
            }

            db.Courses.Remove(course);
            db.SaveChanges();

            return Ok(course);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CourseExists(int id)
        {
            return db.Courses.Count(e => e.Id == id) > 0;
        }
    }
}