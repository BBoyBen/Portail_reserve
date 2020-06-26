using PortailReserve.DAL;
using PortailReserve.DAL.Impl;
using PortailReserve.Models;
using PortailReserve.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
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
                List<Reponse> reponses = rDal.GetReponsesByMessage(m.Id);

                List<ReponseUtil> repUtil = new List<ReponseUtil>();
                foreach(Reponse r in reponses)
                {
                    Utilisateur auteurRep = uDal.GetUtilisateurById(r.Id);
                    Lecture lectureRep = lDal.GetLectureByMessageAndByUtil(r.Id, u.Id);

                    repUtil.Add(new ReponseUtil
                    {
                        Reponse = r,
                        Auteur = auteurRep.Grade + " " + auteurRep.Nom.ToUpper(),
                        Lu = lectureRep != null
                    });
                }

                messageUtil.Add(new MessageUtil
                {
                    Message = m,
                    Auteur = auteur.Grade + " " + auteur.Nom.ToUpper(),
                    Lu = lecture != null,
                    Reponses = repUtil
                });
            }

            MessagerieViewModel vm = new MessagerieViewModel
            {
                Util = u,
                Messages = messageUtil
            };

            return PartialView("AfficherMessagerie", vm);
        }
    }
}