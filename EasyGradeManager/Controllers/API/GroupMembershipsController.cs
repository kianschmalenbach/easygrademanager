using EasyGradeManager.Models;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using static EasyGradeManager.Static.Authorize;
using static System.Data.Entity.EntityState;

namespace EasyGradeManager.Controllers.API
{
    public class GroupMembershipsController : ApiController
    {
        private readonly EasyGradeManagerContext db = new EasyGradeManagerContext();

        public IHttpActionResult GetGroupMemberships()
        {
            return BadRequest();
        }

        public IHttpActionResult GetGroupMembership(int id)
        {
            return BadRequest();
        }

        public IHttpActionResult PutGroupMembership(int id, GroupMembershipDTO membershipDTO)
        {
            return BadRequest();
        }

        public IHttpActionResult PostGroupMembership(GroupMembershipDTO membershipDTO)
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null || authorizedUser.GetStudent() == null)
                return Unauthorized();
            Student student = authorizedUser.GetStudent();
            Lesson lesson = db.Lessons.Find(membershipDTO.NewLessonId);
            if (!ModelState.IsValid || lesson == null || lesson.Assignment == null ||
                lesson.Assignment.Course == null)
                return BadRequest();
            Group group = db.Groups.Find(membershipDTO.NewGroupId);
            if (!membershipDTO.Validate(student, group, lesson.Assignment))
                return BadRequest();
            GroupMembership membership = membershipDTO.Create(student, group, lesson);
            string error = db.Update(membership, Added);
            if (error != null)
                return BadRequest(error);
            return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Assignments/" + lesson.Assignment.Id);
        }

        public IHttpActionResult DeleteGroupMembership(int id)
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null || authorizedUser.GetStudent() == null)
                return Unauthorized();
            Student student = authorizedUser.GetStudent();
            GroupMembership membership = db.GroupMemberships.Find(id);
            if (membership == null)
                return NotFound();
            if (membership.StudentId != student.Id)
                return Unauthorized();
            if (membership.Group == null || membership.Group.Lesson == null || membership.Group.Lesson.Assignment == null ||
                membership.Group.Lesson.Assignment.MembershipsFinal)
                return BadRequest();
            int assignmentId = membership.Group.Lesson.Assignment.Id;
            string error = db.Update(membership, Deleted);
            if (error != null)
                return BadRequest(error);
            return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Assignments/" + assignmentId);
        }
    }
}
