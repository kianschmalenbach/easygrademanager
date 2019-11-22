using EasyGradeManager.Models;
using EasyGradeManager.Static;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using static System.Data.Entity.EntityState;

namespace EasyGradeManager.Controllers.API
{
    public class GroupsController : ApiController
    {
        private readonly EasyGradeManagerContext db = new EasyGradeManagerContext();

        public IHttpActionResult GetGroups()
        {
            return BadRequest();
        }

        public IHttpActionResult GetGroup(int id)
        {
            Authorize auth = new Authorize();
            User authorizedUser = auth.GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            Group group = db.Groups.Find(id);
            if (group == null)
                return NotFound();
            if (group.Lesson == null || group.Lesson.Assignment == null || group.Lesson.Assignment.Course == null)
                return InternalServerError();
            string accessRole = auth.GetAccessRole(authorizedUser, group);
            if (accessRole == null || accessRole.Equals("Student"))
                return Unauthorized();
            return Ok(new GroupDetailTeacherDTO(group));
        }

        public IHttpActionResult PutGroup(int id, GroupDetailTeacherDTO groupDTO)
        {
            Authorize auth = new Authorize();
            User authorizedUser = auth.GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null || (authorizedUser.GetTeacher() == null && authorizedUser.GetTutor() == null))
                return Unauthorized();
            Group group = db.Groups.Find(id);
            if (groupDTO == null || group == null || group.Lesson == null || group.Lesson.Assignment == null ||
                group.Lesson.Assignment.Course == null || !ModelState.IsValid)
                return BadRequest(ModelState);
            Course course = group.Lesson.Assignment.Course;
            bool isTeacher;
            if (!group.IsFinal)
            {
                if (authorizedUser.GetTutor() == null || !authorizedUser.GetTutor().Equals(group.Lesson.Tutor))
                    return Unauthorized();
                isTeacher = false;
            }
            else
            {
                if (authorizedUser.GetTeacher() == null || !"Teacher".Equals(auth.GetAccessRole(authorizedUser, course)))
                    return Unauthorized();
                isTeacher = true;
            }
            if (!groupDTO.Validate(group, isTeacher))
                return BadRequest();
            groupDTO.Update(group);
            string error = db.Update(group, Modified);
            if (error != null)
                return BadRequest(error);
            return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Groups/" + id);
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
