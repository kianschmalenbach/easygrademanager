using EasyGradeManager.Models;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using static EasyGradeManager.Static.Authorize;
using static System.Data.Entity.EntityState;

namespace EasyGradeManager.Controllers.API
{
    public class TasksController : ApiController
    {
        private readonly EasyGradeManagerContext db = new EasyGradeManagerContext();

        public IHttpActionResult GetTasks()
        {
            return BadRequest();
        }

        public IHttpActionResult GetTask(int id)
        {
            return BadRequest();
        }

        public IHttpActionResult PutTask(int id, TaskDetailDTO taskDTO)
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null || authorizedUser.GetTeacher() == null)
                return Unauthorized();
            Task task = db.Tasks.Find(id);
            if (taskDTO == null || task == null || task.Assignment == null || task.Assignment.Course == null ||
                !ModelState.IsValid)
                return BadRequest(ModelState);
            Course course = task.Assignment.Course;
            if (!"Teacher".Equals(GetAccessRole(authorizedUser, course)))
                return Unauthorized();
            if (!taskDTO.Validate(task, task.Assignment))
                return BadRequest();
            taskDTO.Update(task);
            string error = db.Update(task, Modified);
            if (error != null)
                return BadRequest(error);
            return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Assignments/" + task.Assignment.Id);
        }

        public IHttpActionResult PostTask(TaskDetailDTO taskDTO)
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null || authorizedUser.GetTeacher() == null)
                return Unauthorized();
            Assignment assignment = db.Assignments.Find(taskDTO.NewAssignmentId);
            if (!ModelState.IsValid || assignment == null || assignment.Course == null ||
                !taskDTO.Validate(null, assignment))
                return BadRequest();
            if (!"Teacher".Equals(GetAccessRole(authorizedUser, assignment.Course)))
                return Unauthorized();
            Task task = taskDTO.Create();
            string error = db.Update(task, Added);
            if (error != null)
                return BadRequest(error);
            return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Assignments/" + assignment.Id);
        }

        public IHttpActionResult DeleteTask(int id)
        {
            User authorizedUser = GetAuthorizedUser(Request.Headers.GetCookies("user").FirstOrDefault());
            if (authorizedUser == null)
                return Unauthorized();
            Task task = db.Tasks.Find(id);
            if (task == null)
                return NotFound();
            if (task.Assignment == null || task.Assignment.Course == null)
                return BadRequest();
            if (!"Teacher".Equals(GetAccessRole(authorizedUser, task.Assignment.Course)))
                return Unauthorized();
            int assignmentId = task.Assignment.Id;
            string error = db.Update(task, Deleted);
            if (error != null)
                return BadRequest(error);
            return Redirect("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Assignments/" + assignmentId);
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