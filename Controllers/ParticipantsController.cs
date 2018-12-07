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
            ViewData["Message"] = "Ваш список Secret Satan";
            return id == null ? new HttpStatusCodeResult(HttpStatusCode.BadRequest) : GetListParticipantGroup(id, password);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "ParticipantId, Name, Surname, Password, GroupId, Group")] Participant participantPost, string action)
        {
            if (participantPost == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            participantPost.Group = db.Groups.Find(participantPost.GroupId);
            if (action == "Create") return CreateAction(participantPost); 
            var id = participantPost.ParticipantId;
            var participantOriginal = db.Participants.Find(id);
            if (participantOriginal == null) return RedirectToAction("Index", "Groups");
            if (participantOriginal.Password == participantPost.Password)
            {
                return action == "Delete" ? DeleteAction(participantOriginal, id) : DiscoverAction(participantOriginal); 
            }
            ViewData["Message"] = "Введите корректный пароль";
            return GetListParticipantGroup(participantPost.GroupId, participantPost.Group.PasswordView);
        }

        public ActionResult DeleteAction(Participant participantOriginal, int id)
        {
            var group = db.Groups.FirstOrDefault(x => x.GroupId == participantOriginal.GroupId);
            var secretSanta = db.Participants.FirstOrDefault(x => x.BestowedParticipantId == id);
            if (secretSanta != null)
            {
                secretSanta.BestowedParticipant = null;
                secretSanta.BestowedParticipantId = null;
                db.Entry(secretSanta).State = EntityState.Modified;
            }
            db.Participants.Remove(participantOriginal);
            db.SaveChanges();
            ViewData["Message"] = $"Вы удалились из в группы:\"{group.Title}\"";
            return GetListParticipantGroup(group.GroupId, group.PasswordView);
        }

        public ActionResult DiscoverAction(Participant participantPost)
        {
            return RedirectToAction("Victim", "Participants", new { id = participantPost.BestowedParticipantId});
        }

        public ActionResult CreateAction(Participant participant)
        {
           
            //if (ModelState.IsValid)
            {
                db.Participants.Add(participant);
                db.SaveChanges();
            }
            ViewData["Message"] = $"Вы вступили в группу:\"{participant.Group.Title}\"";
            return GetListParticipantGroup(participant.Group.GroupId, participant.Group.PasswordView);
        }

        public ActionResult GetListParticipantGroup(int? id, string password)
        {
            var model = db.Groups.Where(x => x.GroupId == id).Include(p => p.Participants).FirstOrDefault(x => x.PasswordView == password);
            if (model == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            return View("Index", model);
        }

        public ActionResult Victim(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var victim = db.Participants.Find(id);
            var participans = db.Participants.Where(x => x.GroupId == victim.GroupId).ToArray();
            var participanHaveNotVictim = participans.FirstOrDefault(x => x.BestowedParticipant == null);
            if (victim == null || participanHaveNotVictim != null)
            {
                var countParticipans = participans.Length;
                var randomNumsSant = GeneratorRandomSant.GenerateRandomNumsSant(countParticipans);
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
            victim = db.Participants.FirstOrDefault(x => x.BestowedParticipantId == id);
            ViewData["Message"] = $"Ваша Жертва это {victim.Name} {victim.Surname}";
            return View(victim);
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
