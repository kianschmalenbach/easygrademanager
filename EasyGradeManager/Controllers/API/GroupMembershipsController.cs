using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using EasyGradeManager.Models;

namespace EasyGradeManager.Controllers.API
{
    public class GroupMembershipsController : ApiController
    {
        private EasyGradeManagerContext db = new EasyGradeManagerContext();

        // GET: api/GroupMemberships
        public IQueryable<GroupMembership> GetGroupMemberships()
        {
            return db.GroupMemberships;
        }

        // GET: api/GroupMemberships/5
        [ResponseType(typeof(GroupMembership))]
        public IHttpActionResult GetGroupMembership(int id)
        {
            GroupMembership groupMembership = db.GroupMemberships.Find(id);
            if (groupMembership == null)
            {
                return NotFound();
            }

            return Ok(groupMembership);
        }

        // PUT: api/GroupMemberships/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutGroupMembership(int id, GroupMembership groupMembership)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != groupMembership.GroupId)
            {
                return BadRequest();
            }

            db.Entry(groupMembership).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupMembershipExists(id))
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

        // POST: api/GroupMemberships
        [ResponseType(typeof(GroupMembership))]
        public IHttpActionResult PostGroupMembership(GroupMembership groupMembership)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.GroupMemberships.Add(groupMembership);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (GroupMembershipExists(groupMembership.GroupId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = groupMembership.GroupId }, groupMembership);
        }

        // DELETE: api/GroupMemberships/5
        [ResponseType(typeof(GroupMembership))]
        public IHttpActionResult DeleteGroupMembership(int id)
        {
            GroupMembership groupMembership = db.GroupMemberships.Find(id);
            if (groupMembership == null)
            {
                return NotFound();
            }

            db.GroupMemberships.Remove(groupMembership);
            db.SaveChanges();

            return Ok(groupMembership);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool GroupMembershipExists(int id)
        {
            return db.GroupMemberships.Count(e => e.GroupId == id) > 0;
        }
    }
}