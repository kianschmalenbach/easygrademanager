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
    public class EvaluationsController : ApiController
    {
        private EasyGradeManagerContext db = new EasyGradeManagerContext();

        // GET: api/Evaluations
        public IQueryable<Evaluation> GetEvaluations()
        {
            return db.Evaluations;
        }

        // GET: api/Evaluations/5
        [ResponseType(typeof(Evaluation))]
        public IHttpActionResult GetEvaluation(int id)
        {
            Evaluation evaluation = db.Evaluations.Find(id);
            if (evaluation == null)
            {
                return NotFound();
            }

            return Ok(evaluation);
        }

        // PUT: api/Evaluations/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEvaluation(int id, Evaluation evaluation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != evaluation.GroupId)
            {
                return BadRequest();
            }

            db.Entry(evaluation).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EvaluationExists(id))
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

        // POST: api/Evaluations
        [ResponseType(typeof(Evaluation))]
        public IHttpActionResult PostEvaluation(Evaluation evaluation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Evaluations.Add(evaluation);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (EvaluationExists(evaluation.GroupId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = evaluation.GroupId }, evaluation);
        }

        // DELETE: api/Evaluations/5
        [ResponseType(typeof(Evaluation))]
        public IHttpActionResult DeleteEvaluation(int id)
        {
            Evaluation evaluation = db.Evaluations.Find(id);
            if (evaluation == null)
            {
                return NotFound();
            }

            db.Evaluations.Remove(evaluation);
            db.SaveChanges();

            return Ok(evaluation);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EvaluationExists(int id)
        {
            return db.Evaluations.Count(e => e.GroupId == id) > 0;
        }
    }
}