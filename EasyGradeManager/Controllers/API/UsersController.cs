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
    public class UsersController : ApiController
    {
        private readonly EasyGradeManagerContext db = new EasyGradeManagerContext();

        // GET: api/Users
        public IHttpActionResult GetUsers()
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            var result = new List<UserListDTO>();
            foreach (User user in db.Users)
            {
                result.Add(new UserListDTO()
                {
                    Id = user.Id,
                    Identifier = user.Identifier,
                    Name = user.Name
                });
            }
            return Ok(result);
        }

        // GET: api/Users/5
        public IHttpActionResult GetUser(int id)
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            User user = db.Users.Find(id);
            if (user == null)
                return NotFound();
            if (user.Equals(authorizedUser))
            {
                var result = new UserDetailDTO()
                {
                    Id = user.Id,
                    Identifier = user.Identifier,
                    Name = user.Name,
                    Email = user.Email
                };
                foreach (Role role in user.Roles)
                {
                    switch(role.Name)
                    {
                        case "Teacher":
                            result.Teacher = new TeacherDTO()
                            {
                                Id = role.Id
                            };
                            foreach (Course course in ((Teacher) role).Courses)
                                result.Teacher.Courses.Add(new CourseListDTO()
                                {
                                    Id = course.Id,
                                    Archived = course.Archived,
                                    Name = course.Name,
                                    Term = course.Term
                                });
                            break;
                        case "Tutor":
                            result.Tutor = new TutorDTO()
                            {
                                Id = role.Id
                            };
                            foreach (Lesson lesson in ((Tutor) role).Lessons)
                            {
                                result.Tutor.Courses.Add(new CourseListDTO()
                                {
                                    Id = lesson.Assignment.Course.Id,
                                    Archived = lesson.Assignment.Course.Archived,
                                    Name = lesson.Assignment.Course.Name,
                                    Term = lesson.Assignment.Course.Term
                                });
                                result.Tutor.Lessons.Add(new LessonListDTO()
                                {
                                    Id = lesson.Id,
                                    DerivedFromId = lesson.DerivedFromId,
                                    Number = lesson.Number,
                                    Date = lesson.Date
                                });
                            }
                            break;
                        case "Student":
                            result.Student = new StudentDTO()
                            {
                                Id = role.Id
                            };
                            foreach(GroupMembership membership in ((Student) role).GroupMemberships)
                            {
                                result.Student.Courses.Add(new CourseListDTO()
                                {
                                    Id = membership.Group.Lesson.Assignment.Course.Id,
                                    Archived = membership.Group.Lesson.Assignment.Course.Archived,
                                    Name = membership.Group.Lesson.Assignment.Course.Name,
                                    Term = membership.Group.Lesson.Assignment.Course.Term
                                });
                                result.Student.Groups.Add(new GroupListDTO()
                                {
                                    Id = membership.Group.Id,
                                    IsFinal = membership.Group.IsFinal,
                                    Number = membership.Group.Number
                                });
                            }
                            break;
                    }
                }
                return Ok(result);
            }
            else
            {
                var result = new UserListDTO()
                {
                    Id = user.Id,
                    Identifier = user.Identifier,
                    Name = user.Name
                };
                return Ok(result);
            }
        }

        // PUT: api/Users/5
        public IHttpActionResult PutUser(int id, UserDetailDTO userDTO)
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null || authorizedUser.Id != id)
                return Unauthorized();
            User user = db.Users.Find(id);
            if (userDTO == null || user == null || !ModelState.IsValid || id != userDTO.Id)
            {
                return BadRequest(ModelState);
            }
            bool logoutNecessary = userDTO.Update(user);
            db.Entry(user).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            if (userDTO.NewRole != null)
            {
                bool[] roles = { false, false, false };
                foreach(Role role in user.Roles)
                {
                    if (role.Name == "Teacher")
                        roles[0] = true;
                    else if (role.Name == "Tutor")
                        roles[1] = true;
                    else if (role.Name == "Student")
                        roles[2] = true;
                }
                switch (userDTO.NewRole)
                {
                    case "Teacher":
                        if(!roles[0])
                            db.Teachers.Add(new Teacher()
                            {
                                UserId = user.Id
                            });
                        break;
                    case "Tutor":
                        if(!roles[1])
                            db.Tutors.Add(new Tutor()
                            {
                                UserId = user.Id
                            });
                        break;
                    case "Student":
                        if(!roles[2])
                            db.Students.Add(new Student()
                            {
                                UserId = user.Id
                            });
                        break;
                }
                db.SaveChanges();
            }
            if (logoutNecessary)
                return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Logout");
            else
                return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Users
        public IHttpActionResult PostUser(UserDetailDTO userDTO)
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null || !ModelState.IsValid || userDTO.Identifier.Contains("&") || userDTO.NewPassword.Contains("&") || 
                userDTO.NewRole == null || (userDTO.NewRole != "Teacher" && userDTO.NewRole != "Tutor" && userDTO.NewRole != "Student"))
            {
                return BadRequest(ModelState);
            }
            User user = new User();
            userDTO.Update(user);
            db.Users.Add(user);
            db.SaveChanges();
            switch (userDTO.NewRole)
            {
                case "Teacher":
                    db.Teachers.Add(new Teacher()
                    {
                        UserId = user.Id
                    });
                    break;
                case "Tutor":
                    db.Tutors.Add(new Tutor()
                    {
                        UserId = user.Id
                    });
                    break;
                case "Student":
                    db.Students.Add(new Student()
                    {
                        UserId = user.Id
                    });
                    break;
            }
            db.SaveChanges();
            UserListDTO result = new UserListDTO()
            {
                Id = user.Id,
                Identifier = user.Identifier,
                Name = user.Name
            };
            return CreatedAtRoute("DefaultApi", new { id = userDTO.Id }, result);
        }

        // DELETE: api/Users/5
        public IHttpActionResult DeleteUser(int id)
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            //if (authorizedUser == null || authorizedUser.Id != id)
            //    return Unauthorized();
            User user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }

            db.Users.Remove(user);
            db.SaveChanges();

            return Ok(user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserExists(int id)
        {
            return db.Users.Count(e => e.Id == id) > 0;
        }
    }
}