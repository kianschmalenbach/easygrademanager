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

namespace EasyGradeManager.Controllers
{
    public class EvaluationCriterionsController : ApiController
    {
        private EasyGradeManagerContext db = new EasyGradeManagerContext();

        // GET: api/EvaluationCriterions
        public IQueryable<EvaluationCriterion> GetEvaluationCriterions()
        {
            return db.EvaluationCriterions;
        }

        // GET: api/EvaluationCriterions/5
        [ResponseType(typeof(EvaluationCriterion))]
        public IHttpActionResult GetEvaluationCriterion(int id)
        {
            EvaluationCriterion evaluationCriterion = db.EvaluationCriterions.Find(id);
            if (evaluationCriterion == null)
            {
                return NotFound();
            }

            return Ok(evaluationCriterion);
        }

        // PUT: api/EvaluationCriterions/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutEvaluationCriterion(int id, EvaluationCriterion evaluationCriterion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != evaluationCriterion.Id)
            {
                return BadRequest();
            }

            db.Entry(evaluationCriterion).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EvaluationCriterionExists(id))
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

        // POST: api/EvaluationCriterions
        [ResponseType(typeof(EvaluationCriterion))]
        public IHttpActionResult PostEvaluationCriterion(EvaluationCriterion evaluationCriterion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.EvaluationCriterions.Add(evaluationCriterion);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = evaluationCriterion.Id }, evaluationCriterion);
        }

        // DELETE: api/EvaluationCriterions/5
        [ResponseType(typeof(EvaluationCriterion))]
        public IHttpActionResult DeleteEvaluationCriterion(int id)
        {
            EvaluationCriterion evaluationCriterion = db.EvaluationCriterions.Find(id);
            if (evaluationCriterion == null)
            {
                return NotFound();
            }

            db.EvaluationCriterions.Remove(evaluationCriterion);
            db.SaveChanges();

            return Ok(evaluationCriterion);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool EvaluationCriterionExists(int id)
        {
            return db.EvaluationCriterions.Count(e => e.Id == id) > 0;
        }
    }
}