using Microsoft.Ajax.Utilities;
using PortailReserve.DAL;
using PortailReserve.DAL.Impl;
using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using PortailReserve.Utils;
using PortailReserve.ViewModel;
using System;
using System.Collections.Generic;
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

            List<Disponibilite> allDispo = dDal.GetDispoByIdUtilAndByIdEvent(u.Id, e.Id);
            bool aUneDispo = false;
            foreach(Disponibilite d in allDispo)
            {
                if (d.Disponible)
                    aUneDispo = true;
            }

            Disponibilite dispo = new Disponibilite();
            if (allDispo.Count > 0)
                dispo = (Disponibilite)allDispo.ToArray().GetValue(0);

            EventViewModel vm = new EventViewModel()
            {
                Event = e,
                Util = u,
                Effectif = eff,
                Dispo = dispo,
                AllDispo = allDispo,
                ADispo = aUneDispo,
                Participation = pDal.GetParticipationByUtilAndEvent(u.Id, e.Id),
                UtilParticipation = uDal.GetUtilisateursByParticipationOK(e.Id, u.Section, u.Compagnie),
                UtilDispo = uDal.GetUtilisateursByDispoOK(e.Id, u.Section, u.Compagnie),
                UtilNonParticipation = uDal.GetUtilisateursByParticipationKO(e.Id, u.Section, u.Compagnie),
                UtilNonDispo = uDal.GetUtilisateursByDispoKO(e.Id, u.Section, u.Compagnie),
                NoReponseD = uDal.GetUtilisateursSansReponseDispo(e.Id, u.Section, u.Compagnie),
                NoReponseP = uDal.GetUtilisateursSansReponseParticipation(e.Id, u.Section, u.Compagnie)
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
            types.Add(new SelectListItem { Text = "Instruction", Value = "Instruction", Selected = true });
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

            var type = Request.Form["Event.Type"];
            if (type.IsNullOrWhiteSpace())
                type = "Instruction";

            if(type.Equals("Mission") || type.Equals("Stage"))
            {
                if(vm.Event.LimiteReponse < DateTime.Now)
                {
                    ModelState.AddModelError("Event.LimiteReponse", "La date limite de réponse est déjà passée.");
                    isAllValid = false;
                }
            }

            var patracdrFile = Request.Files["patracdrFile"];
            int taille = patracdrFile.ContentLength;
            if(taille >= 4096000)
            {
                ModelState.AddModelError("Event.Patracdr", "La taille du PATRACDR ne doit pas dépasser 4096ko.");
                isAllValid = false;
            }

            if (!isAllValid)
                return View(vm);

            string path = HttpContext.Server.MapPath("~/Content/PATRACDR/") + patracdrFile.FileName;
            patracdrFile.SaveAs(path);
            string url = "/Content/PATRACDR/" + patracdrFile.FileName;

            Guid idEffectif = Guid.Empty;
            if(type.Equals("Mission") || type.Equals("Stage"))
                idEffectif = effDal.AjouterEffectif(vm.Effectif);

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

        [Authorize]
        public ActionResult Modifier (Guid id)
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
                return RedirectToAction("Evenement", "Planning", new { id });

            Evenement ev = eDal.GetEvenementById(id);
            if (ev == null || ev.Equals(typeof(EvenementNull)))
                ev = new Evenement();

            string[] urlSplit = new string[] { };
            if(!ev.Patracdr.IsNullOrWhiteSpace())
                urlSplit = ev.Patracdr.Split('/');

            ViewBag.FileName = "";
            if (urlSplit.Length > 0)
                ViewBag.FileName = urlSplit[urlSplit.Length - 1];

            Effectif eff = new Effectif();
            ViewBag.Display = "none;";
            if (ev.Effectif != null && !ev.Effectif.Equals(Guid.Empty))
            {
                eff = effDal.GetEffectifById(ev.Effectif);
                ViewBag.Display = "block;";
            }

            List<SelectListItem> types = new List<SelectListItem>();
            types.Add(new SelectListItem { Text = "Instruction", Value = "Instruction" });
            types.Add(new SelectListItem { Text = "Exercice", Value = "Exercice" });
            types.Add(new SelectListItem { Text = "Stage", Value = "Stage" });
            types.Add(new SelectListItem { Text = "Mission", Value = "Mission" });

            AjouterEventViewModel vm = new AjouterEventViewModel()
            {
                Event = ev,
                Effectif = eff,
                Types = types
            };

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Modifier(AjouterEventViewModel vm)
        {
            bool isAllValid = true;
            if (vm.Event.Nom.IsNullOrWhiteSpace())
            {
                ModelState.AddModelError("Event.Nom", "Le titre est obligatoire.");
                isAllValid = false;
            }

            if (vm.Event.Description.IsNullOrWhiteSpace())
            {
                ModelState.AddModelError("Event.Description", "Les informations complémentaires sont obligatoires.");
                isAllValid = false;
            }

            if (vm.Event.Lieu.IsNullOrWhiteSpace())
            {
                ModelState.AddModelError("Event.Lieu", "Le lieu est obligatoire.");
                isAllValid = false;
            }

            if (vm.Event.Debut.Year == 1 && vm.Event.Fin.Year == 1)
            {
                ModelState.AddModelError("Event.Debut", "Les dates de d&ébut et de fin sont obligatoire.");
                isAllValid = false;
            }
            else if (vm.Event.Debut.Year == 1)
            {
                ModelState.AddModelError("Event.Debut", "La date de début est obligatoire.");
                isAllValid = false;
            }
            else if (vm.Event.Fin.Year == 1)
            {
                ModelState.AddModelError("Event.Debut", "La date de fin est obligatoire.");
                isAllValid = false;
            }
            else if (vm.Event.Debut > vm.Event.Fin)
            {
                ModelState.AddModelError("Event.Debut", "La date de début doit être antérieure à la date de fin.");
                isAllValid = false;
            }

            string type = Request.Form["Event.Type"];
            if (type.IsNullOrWhiteSpace())
                type = "Instruction";

            if (type.Equals("Mission") || type.Equals("Stage"))
            {
                if (vm.Event.LimiteReponse < DateTime.Now)
                {
                    ModelState.AddModelError("Event.LimiteReponse", "La date limite de réponse est déjà passée.");
                    isAllValid = false;
                }
            }

            var patracdrFile = Request.Files["patracdrFile"];
            string url = vm.Event.Patracdr;
            if(!patracdrFile.FileName.Equals(""))
            {
                int taille = patracdrFile.ContentLength;
                if(taille >= 4096000)
                {
                    ModelState.AddModelError("Event.Patracdr", "La taille du PATRACDR ne doit pas dépasser 4096ko.");
                    isAllValid = false;
                }
                else
                {
                    string path = HttpContext.Server.MapPath("~/Content/PATRACDR/") + patracdrFile.FileName;
                    patracdrFile.SaveAs(path);
                    url = "/Content/PATRACDR/" + patracdrFile.FileName;
                }
            }

            if (!isAllValid)
                return View(vm);

            Guid idEffectif = vm.Effectif.Id;
            if (type.Equals("Mission") || type.Equals("Stage"))
                 effDal.ModifierEffectif(vm.Effectif.Id, vm.Effectif);

            Evenement toModif = vm.Event;
            toModif.Effectif = idEffectif;
            toModif.Type = type;
            toModif.Patracdr = url;

            int retour = eDal.ModifierEvenement(vm.Event.Id, toModif);

            if (retour != 1)
                ViewBag.Erreur = "Une erreure est survenue lors de la modification de lévénement.";

            return RedirectToAction("Evenement", "Planning", new { id = vm.Event.Id });
        }
    }
}