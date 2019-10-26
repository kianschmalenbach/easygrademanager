using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EasyGradeManager.Models;

namespace EasyGradeManager.Controllers.HTML
{
    public class GroupMembershipsController : Controller
    {
        private EasyGradeManagerContext db = new EasyGradeManagerContext();

        // GET: GroupMemberships
        public ActionResult Index()
        {
            var groupMemberships = db.GroupMemberships.Include(g => g.Group).Include(g => g.Student);
            return View(groupMemberships.ToList());
        }

        // GET: GroupMemberships/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupMembership groupMembership = db.GroupMemberships.Find(id);
            if (groupMembership == null)
            {
                return HttpNotFound();
            }
            return View(groupMembership);
        }

        // GET: GroupMemberships/Create
        public ActionResult Create()
        {
            ViewBag.GroupId = new SelectList(db.Groups, "Id", "Password");
            ViewBag.StudentId = new SelectList(db.Persons, "Id", "Id");
            return View();
        }

        // POST: GroupMemberships/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GroupId,StudentId")] GroupMembership groupMembership)
        {
            if (ModelState.IsValid)
            {
                db.GroupMemberships.Add(groupMembership);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.GroupId = new SelectList(db.Groups, "Id", "Password", groupMembership.GroupId);
            ViewBag.StudentId = new SelectList(db.Persons, "Id", "Id", groupMembership.StudentId);
            return View(groupMembership);
        }

        // GET: GroupMemberships/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupMembership groupMembership = db.GroupMemberships.Find(id);
            if (groupMembership == null)
            {
                return HttpNotFound();
            }
            ViewBag.GroupId = new SelectList(db.Groups, "Id", "Password", groupMembership.GroupId);
            ViewBag.StudentId = new SelectList(db.Persons, "Id", "Id", groupMembership.StudentId);
            return View(groupMembership);
        }

        // POST: GroupMemberships/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GroupId,StudentId")] GroupMembership groupMembership)
        {
            if (ModelState.IsValid)
            {
                db.Entry(groupMembership).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.GroupId = new SelectList(db.Groups, "Id", "Password", groupMembership.GroupId);
            ViewBag.StudentId = new SelectList(db.Persons, "Id", "Id", groupMembership.StudentId);
            return View(groupMembership);
        }

        // GET: GroupMemberships/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GroupMembership groupMembership = db.GroupMemberships.Find(id);
            if (groupMembership == null)
            {
                return HttpNotFound();
            }
            return View(groupMembership);
        }

        // POST: GroupMemberships/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GroupMembership groupMembership = db.GroupMemberships.Find(id);
            db.GroupMemberships.Remove(groupMembership);
            db.SaveChanges();
            return RedirectToAction("Index");
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
