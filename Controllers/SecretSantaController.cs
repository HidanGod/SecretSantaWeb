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
    public class SecretSantaController : Controller
    {
        private SecretSantaDBContext db = new SecretSantaDBContext();
        // GET: SecretSantaDictionary
        public ActionResult Index()
        {
            return View(db.Participants.ToList());
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
                var randomNumsSant =  SecretSanta.GeneratorRandomSant.GenerateRandomNumsSant(countParticipans);
                for (int i = 0; i < countParticipans; i++)
                {
                    participans[i].BestowedParticipant = participans[randomNumsSant[i]];
                    participans[i].BestowedParticipantId = participans[randomNumsSant[i]].ID;
                }

                foreach (var participant in participans)
                {
                    db.Entry(participant).State = EntityState.Modified;
                }
             
                db.SaveChanges();
            }
            bestowed = db.Participants.FirstOrDefault(x=>x.BestowedParticipantId == id);

            return View(bestowed);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "ID,Name,Surname,Password")] Participant participant)
        {
            var participantOriginal = db.Participants.Find(participant.ID);
            if(participantOriginal.Password == participant.Password)
                return RedirectToAction("Victim", "SecretSanta", new { id = participant.ID});
            return View(db.Participants.ToList());
        }
    }
}