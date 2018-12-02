using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SecretSantaWeb.Models;

namespace SecretSantaWeb.Controllers
{
    public class GroupsController : Controller
    {
        private SecretSantaDBContext db = new SecretSantaDBContext();

        // GET: Groups
        public ActionResult Index()
        {
            return View(db.Groups.ToList());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "GroupId,Title,Password")] Group group, string action)
        {
            var groupOriginal = db.Groups.Find(group.GroupId);
            if (groupOriginal != null && groupOriginal.Password == group.Password)
                return action != "Delete" ? RedirectToAction("Index", "Participants", new { id = group.GroupId, password = group.Password}) : DeleteAction(groupOriginal);
            return View(db.Groups.ToList());
        }
        public ActionResult DeleteAction(Group groupOriginal)
        {
            var participantsGroup = db.Participants.Where(x => x.GroupId == groupOriginal.GroupId);
            foreach (var participant in participantsGroup)
            {
                var secretSatan = db.Participants.FirstOrDefault(x => x.BestowedParticipantId == participant.ParticipantId);

                if (secretSatan != null)
                {
                    secretSatan.BestowedParticipant = null;
                    secretSatan.BestowedParticipantId = null;
                    db.Entry(secretSatan).State = EntityState.Modified;
                }
                db.Participants.Remove(participant);
               // db.SaveChanges();
            }
            db.Groups.Remove(groupOriginal);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Groups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // GET: Groups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Groups/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GroupId,Title,Password")] Group group)
        {
            if (ModelState.IsValid)
            {
                db.Groups.Add(group);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(group);
        }

        // GET: Groups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // POST: Groups/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "GroupId,Title,Password")] Group group)
        {
            if (ModelState.IsValid)
            {
                db.Entry(group).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(group);
        }

        // GET: Groups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Group group = db.Groups.Find(id);
            db.Groups.Remove(group);
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
