using PortailReserve.DAL;
using PortailReserve.DAL.Impl;
using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using PortailReserve.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using static PortailReserve.Utils.Utils;

namespace PortailReserve.Controllers
{
    public class PlanningController : Controller
    {
        private IUtilisateurDal uDal;
        private IEvenementDal eDal;
        private IEffectifDal effDal;

        public PlanningController()
        {
            uDal = new UtilisateurDal();
            eDal = new EvenementDal();
            effDal = new EffectifDal();
        }

        [Authorize]
        public ActionResult Index()
        {
            Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
            if (u == null)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Index", "Login");
            }
            if (u.Equals(typeof(UtilisateurNull)))
            {
                FormsAuthentication.SignOut();
                ViewBag.Erreur = ((UtilisateurNull)u).Error;
                return RedirectToAction("Index", "Login");
            }
            if (u.PremiereCo)
                return RedirectToAction("PremiereCo", "Login");

            ViewBag.Grade = u.Grade;
            ViewBag.Nom = u.Nom.ToUpperInvariant();

            return View();
        }

        [Authorize]
        public ActionResult Evenement (Guid id)
        {
            Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
            if (u == null)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Index", "Login");
            }
            if (u.Equals(typeof(UtilisateurNull)))
            {
                FormsAuthentication.SignOut();
                ViewBag.Erreur = ((UtilisateurNull)u).Error;
                return RedirectToAction("Index", "Login");
            }
            if (u.PremiereCo)
                return RedirectToAction("PremiereCo", "Login");

            ViewBag.Grade = u.Grade;
            ViewBag.Nom = u.Nom.ToUpperInvariant();

            Evenement e = eDal.GetEvenementById(id);
            ViewBag.Erreur = "";
            if (e == null || e.Equals(typeof(EvenementNull)))
                ViewBag.Erreur = "Une erreur s'est produite lors de la récupération de l'événement.";

            Effectif eff = effDal.GetEffectifById(e.Effectif);

            EventViewModel vm = new EventViewModel()
            {
                Event = e,
                Util = u,
                Effectif = eff
            };  

            return View(vm);
        }

        [Authorize]
        public ActionResult Liste ()
        {
            Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
            if (u == null)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Index", "Login");
            }
            if (u.Equals(typeof(UtilisateurNull)))
            {
                FormsAuthentication.SignOut();
                ViewBag.Erreur = ((UtilisateurNull)u).Error;
                return RedirectToAction("Index", "Login");
            }
            if (u.PremiereCo)
                return RedirectToAction("PremiereCo", "Login");

            ViewBag.Grade = u.Grade;
            ViewBag.Nom = u.Nom.ToUpperInvariant();

            List<Evenement> aVenir = eDal.GetEvenementsAVenir();
            aVenir = TrieEventAVenir(aVenir);

            List<Evenement> passe = eDal.GetEvenementsPasse();
            passe = TrieEventPasse(passe);

            ListeEventViewModel vm = new ListeEventViewModel()
            {
                AVenir =aVenir,
                Passe = passe
            };

            return View(vm);
        }

        [Authorize]
        public ActionResult Ajouter()
        {
            Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
            if (u == null)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Index", "Login");
            }
            if (u.Equals(typeof(UtilisateurNull)))
            {
                FormsAuthentication.SignOut();
                ViewBag.Erreur = ((UtilisateurNull)u).Error;
                return RedirectToAction("Index", "Login");
            }
            if (u.PremiereCo)
                return RedirectToAction("PremiereCo", "Login");

            ViewBag.Grade = u.Grade;
            ViewBag.Nom = u.Nom.ToUpperInvariant();

            AjouterEventViewModel vm = new AjouterEventViewModel()
            {
                Event = new Evenement()
            };

            return View(vm);
        }
    }
}