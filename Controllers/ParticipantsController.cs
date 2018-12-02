using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Windows.Forms;
using SecretSantaWeb.Models;

namespace SecretSantaWeb.Controllers
{
    public class ParticipantsController : Controller
    {
        private SecretSantaDBContext db = new SecretSantaDBContext();

        // GET: Participants
        public ActionResult Index(int? id, string password)
        {
            return GetListParticipantGroup(id, password);
        }

        // GET: Participants
        // POST: Participants/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "ParticipantId, Name, Surname, Password, GroupId, Group")] Participant participantPost, string action)
        {
            //MessageBox.Show($"{participantPost.GroupId.ToString()} ид {participantPost.Password} ид {participantPost.Surname}");
            // MessageBox.Show(participantPost.GroupId.ToString() + participantPost.Group.GroupId.ToString());
            if (action == "Create") return CreateAction(participantPost); 
            var id = participantPost.ParticipantId;
            var participantOriginal = db.Participants.Find(id);
            if (participantOriginal == null) return RedirectToAction("Index", "Groups");
            if (participantOriginal.Password == participantPost.Password)
            {
                return action == "Delete" ? DeleteAction(participantOriginal, id) : DiscoverAction(participantOriginal); 
            }
            participantPost.Group = db.Groups.Find(participantPost.GroupId);
            return GetListParticipantGroup(participantPost.GroupId, participantPost.Group.Password);
        }

        public ActionResult DeleteAction(Participant participantOriginal, int id)
        {
            var secretSanta = db.Participants.FirstOrDefault(x => x.BestowedParticipantId == id);
            if (secretSanta != null)
            {
                secretSanta.BestowedParticipant = null;
                secretSanta.BestowedParticipantId = null;
                db.Entry(secretSanta).State = EntityState.Modified;
            }
            db.Participants.Remove(participantOriginal);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult DiscoverAction(Participant participantPost)
        {
            return RedirectToAction("Victim", "Participants", new { id = participantPost.ParticipantId });
        }

        public ActionResult CreateAction(Participant participant)
        {
            participant.Group = db.Groups.Find(participant.GroupId);

            db.Participants.Add(participant);
                db.SaveChanges();
            

            return GetListParticipantGroup(participant.GroupId, participant.Group.Password);
        }

        public ActionResult GetListParticipantGroup(int? id, string password)
        {
            var participants = db.Participants.Include(p => p.BestowedParticipant).Include(p => p.Group);
            var gr = db.Groups.Where(x => x.GroupId == id).Include(p => p.Participants).FirstOrDefault(x => x.Password == password);
            var model = db.Groups.Where(x => x.GroupId == id).FirstOrDefault(x => x.Password == password);
                if (model == null) return View("Index");
            var listParticipant = db.Participants.Where(x => x.GroupId == id).ToList();
            model.Participants = listParticipant;
            return View("Index", gr);
        }

        // GET: Participants/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Participants/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ParticipantId,Name,Surname,Password")] Participant participant)
        {
            if (ModelState.IsValid)
            {
                db.Participants.Add(participant);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(participant);
        }



        // POST: Participants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Participant participant = db.Participants.Find(id);
            Participant secretSanta = db.Participants.FirstOrDefault(x => x.BestowedParticipantId == id);
            if(secretSanta != null)
            {
                secretSanta.BestowedParticipant = null;
                secretSanta.BestowedParticipantId = null;
                db.Entry(secretSanta).State = EntityState.Modified;
            }
            db.Participants.Remove(participant);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Participants/Edit/5
        public ActionResult Victim(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var bestowed = db.Participants.Find(id);
            var participanHaveNotBestowed = db.Participants.FirstOrDefault(x => x.BestowedParticipant == null);
            if (bestowed == null || participanHaveNotBestowed != null)
            {
                var participans = db.Participants.ToArray();
                var countParticipans = participans.Length;
                var randomNumsSant = SecretSanta.GeneratorRandomSant.GenerateRandomNumsSant(countParticipans);
                for (int i = 0; i < countParticipans; i++)
                {
                    participans[i].BestowedParticipant = participans[randomNumsSant[i]];
                    participans[i].BestowedParticipantId = participans[randomNumsSant[i]].ParticipantId;
                }

                foreach (var participant in participans)
                {
                    db.Entry(participant).State = EntityState.Modified;
                }

                db.SaveChanges();
            }
            bestowed = db.Participants.FirstOrDefault(x => x.BestowedParticipantId == id);

            return View(bestowed);
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
