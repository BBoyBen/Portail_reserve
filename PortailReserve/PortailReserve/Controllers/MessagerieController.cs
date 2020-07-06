using PortailReserve.DAL;
using PortailReserve.DAL.Impl;
using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using PortailReserve.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PortailReserve.Controllers
{
    public class MessagerieController : Controller
    {
        private IMessageDal mDal;
        private IReponseDal rDal;
        private ILectureDal lDal;
        private IUtilisateurDal uDal;

        public MessagerieController()
        {
            mDal = new MessageDal();
            rDal = new ReponseDal();
            lDal = new LectureDal();
            uDal = new UtilisateurDal();
        }

        public ActionResult AfficherMessagerie(Guid id)
        {
            Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);

            List<Message> messages = mDal.GetMessagesByEvent(id);

            List<MessageUtil> messageUtil = new List<MessageUtil>();
            foreach(Message m in messages)
            {
                Utilisateur auteur = uDal.GetUtilisateurById(m.Envoyeur);
                Lecture lecture = lDal.GetLectureByMessageAndByUtil(m.Id, u.Id);

                messageUtil.Add(new MessageUtil
                {
                    Message = m,
                    Auteur = auteur.Grade + " " + auteur.Nom.ToUpper(),
                    Lu = lecture != null
                });
            }

            MessagerieViewModel vm = new MessagerieViewModel
            {
                Util = u,
                Messages = messageUtil,
                Event = id
            };

            return PartialView("AfficherMessagerie", vm);
        }

        [Authorize]
        public ActionResult AfficherMessage(Guid id)
        {
            Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
            if (u == null || u.Equals(typeof(UtilisateurNull)))
                u = new Utilisateur
                {
                    Grade = "Soldat",
                    Nom = "Empty",
                    Id = Guid.Empty
                };

            Message message = mDal.GetMessageById(id);
            if (message == null)
                message = new Message
                {
                    Date = DateTime.Now,
                    Envoyeur = Guid.Empty,
                    Texte = ""
                };

            Utilisateur auteur = uDal.GetUtilisateurById(message.Envoyeur);
            if (auteur == null || auteur.Equals(typeof(UtilisateurNull)))
                auteur = new Utilisateur
                {
                    Grade = "Soldat",
                    Nom = "Empty",
                    Id = Guid.Empty
                };

            Lecture lecture = lDal.GetLectureByMessageAndByUtil(id, u.Id);

            MessageUtil messUtil = new MessageUtil
            {
                Auteur = auteur.Grade + " " + auteur.Nom.ToUpper(),
                Message = message,
                Lu = lecture != null
            };

            MessageViewModel vm = new MessageViewModel
            {
                Util = u,
                Message = messUtil
            };

            return PartialView("AfficherMessage", vm);
        }

        [Authorize]
        public ActionResult AfficherNotifNonLu(bool lu)
        {
            return PartialView("AfficherNotifNonLu", lu);
        }

        [Authorize]
        public ActionResult AfficherReponses(Guid id)
        {
            if (id.Equals(Guid.Empty))
                return new HttpStatusCodeResult(400);

            Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);

            List<Reponse> reponses = rDal.GetReponsesByMessage(id);

            List<ReponseUtil> repUtil = new List<ReponseUtil>();
            foreach (Reponse r in reponses)
            {
                Utilisateur auteurRep = uDal.GetUtilisateurById(r.Envoyeur);
                Lecture lectureRep = lDal.GetLectureByMessageAndByUtil(r.Id, u.Id);

                repUtil.Add(new ReponseUtil
                {
                    Reponse = r,
                    Auteur = auteurRep.Grade + " " + auteurRep.Nom.ToUpper(),
                    Lu = lectureRep != null
                });
            }

            ReponseViewModel vm = new ReponseViewModel
            {
                Util = u,
                Reponse = repUtil,
                Message = id
            };

            return PartialView("AfficherReponses", vm);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Ecrire()
        {
            try
            {
                Guid idEvent = Guid.Parse(Request.Form["idEvent"]);
                if (idEvent.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400);

                string message = Request.Form["message"];

                Guid idUtil = Guid.Parse(Request.Form["util"]);
                if (idUtil.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400);

                Guid idMessage = mDal.AjouterMessage(new Message
                {
                    Evenement = idEvent,
                    Envoyeur = idUtil,
                    Texte = message
                });

                if (idMessage.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400);

                lDal.AjouterLecture(idUtil, idMessage);

                return RedirectToAction("AfficherMessagerie", new { id = idEvent });
            }
            catch(Exception e)
            {
                return new HttpStatusCodeResult(500);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Repondre ()
        {
            try
            {
                Guid idUtil = Guid.Parse(Request.Form["idUtil"]);
                if (idUtil.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400);

                Guid idMessage = Guid.Parse(Request.Form["idMessage"]);
                if (idMessage.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400);

                string reponse = Request.Form["rep"];

                Guid idReponse = rDal.AjouterReponse(new Reponse
                {
                    Envoyeur = idUtil,
                    Message = idMessage,
                    Texte = reponse
                });

                if (idReponse.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(500);

                lDal.AjouterLecture(idUtil, idReponse);

                return RedirectToAction("AfficherReponses", new { id = idMessage });
            }
            catch(Exception e)
            {
                return new HttpStatusCodeResult(500);
            }
        }

        [Authorize]
        public ActionResult Lire (Guid idMessage, Guid idUtil )
        {
            if (idMessage.Equals(Guid.Empty) || idUtil.Equals(Guid.Empty))
                return new HttpStatusCodeResult(400);

            Lecture lecture = lDal.GetLectureByMessageAndByUtil(idMessage, idUtil);
            if (lecture != null)
                return new HttpStatusCodeResult(200);

            Guid idLescture = lDal.AjouterLecture(idUtil, idMessage);
            if (idLescture.Equals(Guid.Empty))
                return new HttpStatusCodeResult(400);

            return RedirectToAction("AfficherNotifNonLu", new { lu = true });
        }
    }
}