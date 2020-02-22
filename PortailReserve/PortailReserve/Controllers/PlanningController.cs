using Microsoft.Ajax.Utilities;
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
        private IParticipationDal pDal;
        private IDisponibiliteDal dDal;

        public PlanningController()
        {
            uDal = new UtilisateurDal();
            eDal = new EvenementDal();
            effDal = new EffectifDal();
            pDal = new ParticipationDal();
            dDal = new DisponibiliteDal();
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
            ViewBag.Role = u.Role;

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
            ViewBag.Role = u.Role;

            Evenement e = eDal.GetEvenementById(id);
            ViewBag.Erreur = "";
            if (e == null || e.Equals(typeof(EvenementNull)))
                ViewBag.Erreur = "Une erreur s'est produite lors de la récupération de l'événement.";

            Effectif eff = effDal.GetEffectifById(e.Effectif);

            EventViewModel vm = new EventViewModel()
            {
                Event = e,
                Util = u,
                Effectif = eff,
                Dispo = dDal.GetDispoByIdUtilAndByIdEvent(u.Id, e.Id),
                Participation = pDal.GetParticipationByUtilAndEvent(u.Id, e.Id),
                UtilParticipation = uDal.GetUtilisateursByParticipation(e.Id),
                UtilDispo = uDal.GetUtilisateursByDispo(e.Id),
                NoReponseD = uDal.GetUtilisateursSansReponseDispo(e.Id),
                NoReponseP = uDal.GetUtilisateursSansReponseParticipation(e.Id)
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
            ViewBag.Role = u.Role;

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
            ViewBag.Role = u.Role;

            List<SelectListItem> types = new List<SelectListItem>();
            types.Add(new SelectListItem { Text = "Instruction", Value = "Intruction", Selected = true });
            types.Add(new SelectListItem { Text = "Exercice", Value = "Exercice" });
            types.Add(new SelectListItem { Text = "Stage", Value = "Stage" });
            types.Add(new SelectListItem { Text = "Mission", Value = "Mission" });

            Effectif eff = new Effectif
            {
                Officier = 0,
                SousOfficier = 0,
                Militaire = 0
            };

            AjouterEventViewModel vm = new AjouterEventViewModel()
            {
                Event = new Evenement(),
                Types = types,
                Effectif = eff
            };

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Ajouter (AjouterEventViewModel vm)
        {
            bool isAllValid = true;
            if(vm.Event.Nom.IsNullOrWhiteSpace())
            {
                ModelState.AddModelError("Event.Nom", "Le titre est obligatoire.");
                isAllValid = false;
            }

            if(vm.Event.Description.IsNullOrWhiteSpace())
            {
                ModelState.AddModelError("Event.Description", "Les informations complémentaires sont obligatoires.");
                isAllValid = false;
            }

            if (vm.Event.Lieu.IsNullOrWhiteSpace())
            {
                ModelState.AddModelError("Event.Lieu", "Le lieu est obligatoire.");
                isAllValid = false;
            }

            if(vm.Event.Debut.Year == 1 && vm.Event.Fin.Year == 1)
            {
                ModelState.AddModelError("Event.Debut", "Les dates de d&ébut et de fin sont obligatoire.");
                isAllValid = false;
            }
            else if(vm.Event.Debut.Year == 1)
            {
                ModelState.AddModelError("Event.Debut", "La date de début est obligatoire.");
                isAllValid = false;
            }
            else if(vm.Event.Fin.Year == 1)
            {
                ModelState.AddModelError("Event.Debut", "La date de fin est obligatoire.");
                isAllValid = false;
            }
            else if(vm.Event.Debut > vm.Event.Fin)
            {
                ModelState.AddModelError("Event.Debut", "La date de début doit être antérieure à la date de fin.");
                isAllValid = false;
            }

            if (!isAllValid)
                return View(vm);

            string type = Request.Form["Event.Type"];

            Guid idEffectif = Guid.Empty;
            if(type.Equals("Mission") || type.Equals("Stage"))
                idEffectif = effDal.AjouterEffectif(vm.Effectif);

            var patracdrFile = Request.Files["patracdrFile"];
            string path = HttpContext.Server.MapPath("~/Content/PATRACDR/") + patracdrFile.FileName;
            patracdrFile.SaveAs(path);
            string url = "/Content/PATRACDR/" + patracdrFile.FileName;

            Evenement toCreate = vm.Event;
            toCreate.Effectif = idEffectif;
            toCreate.Type = type;
            toCreate.Patracdr = url;

            Guid idEvent = eDal.CreerEvenement(toCreate);

            return RedirectToAction("Evenement", "Planning", new { id = idEvent });
        }

        [Authorize]
        public ActionResult Supprimer (Guid id)
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
            ViewBag.Role = u.Role;

            if (u.Role > 3)
                return RedirectToAction("Liste", "Planning");

            int retour = eDal.SupprimerEvenement(id);
            if (retour != 1)
                ViewBag.Erreur = "Un problème est surbenu lors de la suppression.";

            return RedirectToAction("Liste", "Planning");
        }
    }
}