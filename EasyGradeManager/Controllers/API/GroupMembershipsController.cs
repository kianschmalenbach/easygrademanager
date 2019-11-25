using EasyGradeManager.Models;
using EasyGradeManager.Static;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
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
            User authorizedUser = new Authorize().GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null || authorizedUser.GetStudent() == null)
                return Unauthorized();
            Student student = authorizedUser.GetStudent();
            Assignment assignment = db.Assignments.Find(membershipDTO.NewAssignmentId);
            if (assignment == null || assignment.Lessons == null)
                return BadRequest();
            Lesson lesson = null;
            Group group = null;
            if (membershipDTO.NewLessonNumber > 0)
            {
                foreach (Lesson otherLesson in assignment.Lessons)
                {
                    if (membershipDTO.NewLessonNumber == otherLesson.Number)
                    {
                        lesson = otherLesson;
                        break;
                    }
                }
                if (lesson == null)
                    return BadRequest();
            }
            else if (membershipDTO.NewGroupNumber > 0)
            {
                lesson = null;
                foreach (Lesson otherLesson in assignment.Lessons)
                {
                    if (otherLesson.Groups != null)
                    {
                        foreach (Group otherGroup in otherLesson.Groups)
                        {
                            if (membershipDTO.NewGroupNumber == otherGroup.Number)
                            {
                                group = otherGroup;
                                lesson = otherLesson;
                                break;
                            }
                        }
                    }
                    if (group != null)
                        break;
                }
                if (group == null || lesson == null)
                    return BadRequest();
            }
            if (!ModelState.IsValid || lesson == null || lesson.Assignment == null ||
                lesson.Assignment.Course == null)
                return BadRequest();
            if (!membershipDTO.Validate(student, group, lesson.Assignment))
                return BadRequest();
            ICollection<object> memberships = membershipDTO.Create(student, group, lesson);
            string error = db.UpdateAll(memberships, Added);
            if (error != null)
                return BadRequest(error);
            return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Assignments/" + lesson.Assignment.Id);
        }

        public IHttpActionResult DeleteGroupMembership(int id)
        {
            User authorizedUser = new Authorize().GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null || authorizedUser.GetStudent() == null)
                return Unauthorized();
            Student student = authorizedUser.GetStudent();
            GroupMembership membership = db.GroupMemberships.Find(id);
            if (membership == null)
                return NotFound();
            if (membership.StudentId != student.Id)
                return Unauthorized();
            if (membership.Group == null || membership.Group.Lesson == null || membership.Group.Lesson.Assignment == null ||
                membership.Group.Lesson.Assignment.MembershipsFinal || membership.Student == null)
                return BadRequest();
            int assignmentId = membership.Group.Lesson.Assignment.Id;
            Group deleteGroup = null;
            if (membership.Group.GroupMemberships.Count == 0)
                deleteGroup = membership.Group;
            string error = db.Update(membership, Deleted);
            if (error != null)
                return BadRequest(error);
            if (deleteGroup != null)
                db.Update(deleteGroup, Deleted);
            return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Assignments/" + assignmentId);
        }
    }
}
