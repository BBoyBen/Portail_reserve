using PortailReserve.DAL;
using PortailReserve.DAL.Impl;
using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using PortailReserve.Utils;
using PortailReserve.ViewModel;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace PortailReserve.Controllers
{
    public class MessagerieController : Controller
    {
        private IMessageDal mDal;
        private IReponseDal rDal;
        private ILectureDal lDal;
        private IUtilisateurDal uDal;
        private readonly Logger LOGGER;

        public MessagerieController()
        {
            mDal = new MessageDal();
            rDal = new ReponseDal();
            lDal = new LectureDal();
            uDal = new UtilisateurDal();
            LOGGER = new Logger(this.GetType());
        }

        public ActionResult AfficherMessagerie(Guid id)
        {
            try
            {
                Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);

                List<Message> messages = mDal.GetMessagesByEvent(id);

                List<MessageUtil> messageUtil = new List<MessageUtil>();
                foreach (Message m in messages)
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
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur lors de l'affichage de la messageire -> " + e);
                return new HttpStatusCodeResult(500, "Exception affichage messagerie -> " + e.Message);
            }
        }

        [Authorize]
        public ActionResult AfficherMessage(Guid id)
        {
            try
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
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur affichage du message : " + id + " -> " + e);
                return new HttpStatusCodeResult(500, "Exception affichage du message  -> " + e.Message);
            }
        }

        [Authorize]
        public ActionResult AfficherNotifNonLu(bool lu)
        {
            try
            {
                return PartialView("AfficherNotifNonLu", lu);
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur affichage de la page notif non lu -> " + e);
                return new HttpStatusCodeResult(500, "Exception affiche notif non lu -> " + e.Message);
            }
        }

        [Authorize]
        public ActionResult AfficherReponses(Guid id)
        {
            try
            {
                if (id.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400, "Id vide");

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
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur afficga edes reponses du messge : " + id + " -> " + e);
                return new HttpStatusCodeResult(500, "Exception affichage des réponses -> " + e.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult Ecrire()
        {
            try
            {
                Guid idEvent = Guid.Parse(Request.Form["idEvent"]);
                if (idEvent.Equals(Guid.Empty))
                {
                    LOGGER.Log("ERROR", "Erreur récupération de l'id de l'event pour écrire message");
                    return new HttpStatusCodeResult(400, "Echec récupération evenement");
                }

                string message = Request.Form["message"];

                Guid idUtil = Guid.Parse(Request.Form["util"]);
                if (idUtil.Equals(Guid.Empty))
                {
                    LOGGER.Log("ERROR", "Erreur récupération de l'id utilisateur pour écrire");
                    return new HttpStatusCodeResult(400, "Echec récupération utilisateur");
                }

                Guid idMessage = mDal.AjouterMessage(new Message
                {
                    Evenement = idEvent,
                    Envoyeur = idUtil,
                    Texte = message
                });

                if (idMessage.Equals(Guid.Empty))
                {
                    LOGGER.Log("ERROR", "Erreur ajout du nouveau message pour l'event : " + idEvent + " par l'util : " + idUtil );
                    return new HttpStatusCodeResult(400, "Echec de l'ajout du message");
                }

                lDal.AjouterLecture(idUtil, idMessage);

                return RedirectToAction("AfficherMessagerie", new { id = idEvent });
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur de l'écriture du message -> " + e);
                return new HttpStatusCodeResult(500, "Exception écriture du message -> " + e.Message);
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
                {
                    LOGGER.Log("ERROR", "Erreur récupération de l'id utilisateur pour répondre.");
                    return new HttpStatusCodeResult(400, "Echec récupération utilisateur");
                }

                Guid idMessage = Guid.Parse(Request.Form["idMessage"]);
                if (idMessage.Equals(Guid.Empty))
                {
                    LOGGER.Log("ERROR", "Erreur récupération de l'id message pour répondre.");
                    return new HttpStatusCodeResult(400, "Echec récupération du message.");
                }

                string reponse = Request.Form["rep"];

                Guid idReponse = rDal.AjouterReponse(new Reponse
                {
                    Envoyeur = idUtil,
                    Message = idMessage,
                    Texte = reponse
                });

                if (idReponse.Equals(Guid.Empty))
                {
                    LOGGER.Log("ERROR", "Erreur lors de l'ajout de la réponse pour le message : " + idMessage);
                    return new HttpStatusCodeResult(400, "Echec de l'ajout de la réponse.");
                }

                lDal.AjouterLecture(idUtil, idReponse);

                return RedirectToAction("AfficherReponses", new { id = idMessage });
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur lors de l'ajout de la réponse -> " + e);
                return new HttpStatusCodeResult(500, "Exception ajout de la réponse -> " + e.Message);
            }
        }

        [Authorize]
        public ActionResult Lire (Guid idMessage, Guid idUtil )
        {
            try
            {
                if (idMessage.Equals(Guid.Empty) || idUtil.Equals(Guid.Empty))
                {
                    LOGGER.Log("ERROR", "Erreur sur l'un des id pour la lecture.");
                    return new HttpStatusCodeResult(400, "Un ou deux des paramètres sont incorrects.");
                }

                Lecture lecture = lDal.GetLectureByMessageAndByUtil(idMessage, idUtil);
                if (lecture != null)
                    return new HttpStatusCodeResult(200);

                Guid idLescture = lDal.AjouterLecture(idUtil, idMessage);
                if (idLescture.Equals(Guid.Empty))
                {
                    LOGGER.Log("ERROR", "Erreur lors de l'ajout de la lecture du message : " + idMessage + " par l'util : " + idUtil);
                    return new HttpStatusCodeResult(400, "Erreur lecture message.");
                }

                return RedirectToAction("AfficherNotifNonLu", new { lu = true });
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur lecture du message : " + idMessage + " par l'utilisateur : " + idUtil + " -> " + e);
                return new HttpStatusCodeResult(500, "Exception lecture message -> " + e.Message);
            }
        }
    }
}