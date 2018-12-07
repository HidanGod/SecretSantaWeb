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
            ViewData["Message"] = "Приветствую пользователь!";
            return View(db.Groups.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index([Bind(Include = "GroupId,Title,PasswordView,PasswordDelete")] Group group, string action)
        {
            var message = "Приветствую пользователь!";
            var groupOriginal = db.Groups.Find(group.GroupId);
            if (groupOriginal == null) return View(db.Groups.ToList());
            switch (action)
            {
                case "Delete":
                {
                    message = "Введите коректный пароль для удаления";
                    if (groupOriginal.PasswordDelete == @group.PasswordDelete)
                        return DeleteAction(groupOriginal);
                    break;
                }
                case "View":
                {
                    message = "Ведите коректный пароль для просмотра";
                    if (groupOriginal.PasswordView == @group.PasswordView)
                        return ViewAction(groupOriginal);
                    break;
                }
            }

            ViewData["Message"] = message;
            return View(db.Groups.ToList());
        }

        public ActionResult ViewAction(Group groupOriginal)
        {
           
            return RedirectToAction("Index", "Participants", new { id = groupOriginal.GroupId, password = groupOriginal.PasswordView });
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
            }
            db.Groups.Remove(groupOriginal);
            db.SaveChanges();
          
            return RedirectToAction("Index");
        }


        public ActionResult Create()
        {
            ViewData["Message"] = "Cоздай свою группу Secret Satan";
            return View();
        }

        // POST: Groups/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "GroupId,Title,PasswordView,PasswordDelete")] Group group)
        {
            if (ModelState.IsValid)
            {
                db.Groups.Add(group);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(group);
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
