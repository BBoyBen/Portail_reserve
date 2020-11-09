using Microsoft.Extensions.Logging.Abstractions;
using PortailReserve.DAL;
using PortailReserve.DAL.Impl;
using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using PortailReserve.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using System.Web.Security;

namespace PortailReserve.Controllers
{
    public class CoursController : Controller
    {
        private IUtilisateurDal uDal;
        private ICoursDal cDal;
        private IChantDal chDal;

        public CoursController()
        {
            uDal = new UtilisateurDal();
            cDal = new CoursDal();
            chDal = new ChantDal();
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
        public ActionResult AfficherListeThemes()
        {
            return PartialView("AfficherListeThemes");
        }

        [Authorize]
        public ActionResult AfficherCours(string theme)
        {
            try
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

                List<Cours> coursParTheme = cDal.GetCoursByTheme(theme);

                return PartialView("AfficherCours", coursParTheme);
            }
            catch(Exception e)
            {
                return new HttpStatusCodeResult(500);
            }
        }

        [Authorize]
        public ActionResult AfficherPopUpNouveauCours()
        {
            Cours cours = new Cours
            {
                Nom = "",
                Description = "",
                Fichier = ""
            };

            List<SelectListItem> themes = new List<SelectListItem>();
            themes.Add(new SelectListItem { Text = "Connaissances Militaire Générale", Value = "CMG", Selected = true });
            themes.Add(new SelectListItem { Text = "Combat", Value = "Combat" });
            themes.Add(new SelectListItem { Text = "Génie", Value = "Genie" });
            themes.Add(new SelectListItem { Text = "ISTC", Value = "ISTC" });
            themes.Add(new SelectListItem { Text = "NRBC", Value = "NRBC" });
            themes.Add(new SelectListItem { Text = "Renseignement", Value = "Renseignement" });
            themes.Add(new SelectListItem { Text = "Secourisme", Value = "Secourisme" });
            themes.Add(new SelectListItem { Text = "Transmission", Value = "Transmission" });
            themes.Add(new SelectListItem { Text = "Topographie", Value = "Topographie" });
            themes.Add(new SelectListItem { Text = "Autres", Value = "Autres" });

            AjoutCoursViewModel vm = new AjoutCoursViewModel { 
                Cours = cours,
                Themes = themes
            };

            return PartialView("AfficherPopUpNouveauCours", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult AjouterCours (AjoutCoursViewModel vm)
        {
            try
            {
                Cours toSave = vm.Cours;

                var fichierCours = Request.Files["fichierCours"];
                int taille = fichierCours.ContentLength;
                if (taille/1024 >= 2097152)
                {
                    return new HttpStatusCodeResult(400);
                }

                string path = HttpContext.Server.MapPath("~/Content/Cours/") + toSave.Theme + "/" + fichierCours.FileName;
                fichierCours.SaveAs(path);
                string url = "/Content/Cours/" + toSave.Theme + "/" + fichierCours.FileName;

                toSave.Fichier = url;

                toSave.Extension = Path.GetExtension(url);

                Guid idCours = cDal.AjouterCours(toSave);

                if(idCours.Equals(Guid.Empty))
                {
                    return new HttpStatusCodeResult(400);
                }

                return RedirectToAction("AfficherListeThemes");
            }
            catch(Exception e)
            {
                return new HttpStatusCodeResult(500);
            }
        }

        [Authorize]
        public ActionResult AfficherPopUpSuppressionCours(Guid id)
        {
            try
            {
                Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
                if (u == null)
                {
                    FormsAuthentication.SignOut();
                    return new HttpUnauthorizedResult();
                }
                if (u.Equals(typeof(UtilisateurNull)))
                {
                    FormsAuthentication.SignOut();
                    ViewBag.Erreur = ((UtilisateurNull)u).Error;
                    return new HttpUnauthorizedResult();
                }
                if (u.PremiereCo)
                    return new HttpUnauthorizedResult();

                if(u.Role > 3)
                {
                    return new HttpUnauthorizedResult();
                }

                Cours cours = cDal.GetCoursById(id);
                if(cours == null || cours.Equals(typeof(CoursNull)))
                {
                    cours = new Cours
                    {
                        Nom = "Empty",
                        Description = "Empty",
                        Id = Guid.Empty
                    };
                }

                return PartialView("AfficherPopUpSuppressionCours", cours);
            }
            catch(Exception e)
            {
                return new HttpStatusCodeResult(400);
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult SupprimerCours()
        {
            try
            {
                Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
                if (u == null)
                {
                    FormsAuthentication.SignOut();
                    return new HttpUnauthorizedResult();
                }
                if (u.Equals(typeof(UtilisateurNull)))
                {
                    FormsAuthentication.SignOut();
                    ViewBag.Erreur = ((UtilisateurNull)u).Error;
                    return new HttpUnauthorizedResult();
                }
                if (u.PremiereCo)
                    return new HttpUnauthorizedResult();

                if (u.Role > 3)
                {
                    return new HttpUnauthorizedResult();
                }

                Guid idCours = Guid.Parse(Request.Form["idCours"]);
                if(idCours.Equals(Guid.Empty))
                {
                    return new HttpStatusCodeResult(400);
                }

                Cours toDelete = cDal.GetCoursById(idCours);
                if(toDelete == null || toDelete.Equals(typeof(CoursNull)))
                {
                    return new HttpStatusCodeResult(400);
                }

                int retour = cDal.SupprimerCours(idCours);
                if(retour != 1)
                {
                    return new HttpStatusCodeResult(400);
                }

                if (System.IO.File.Exists(toDelete.Fichier))
                    System.IO.File.Delete(toDelete.Fichier);

                return RedirectToAction("AfficherListeThemes");
            }
            catch(Exception e)
            {
                return new HttpStatusCodeResult(500);
            }
        }

        [Authorize]
        public ActionResult AfficherPopUpModifierCours(Guid id)
        {
            try
            {
                Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
                if (u == null)
                {
                    FormsAuthentication.SignOut();
                    return new HttpUnauthorizedResult();
                }
                if (u.Equals(typeof(UtilisateurNull)))
                {
                    FormsAuthentication.SignOut();
                    ViewBag.Erreur = ((UtilisateurNull)u).Error;
                    return new HttpUnauthorizedResult();
                }
                if (u.PremiereCo)
                    return new HttpUnauthorizedResult();

                if (u.Role > 3)
                {
                    return new HttpUnauthorizedResult();
                }

                Cours toModif = cDal.GetCoursById(id);
                if(toModif == null || toModif.Equals(typeof(CoursNull)))
                {
                    toModif = new Cours { 
                        Nom = "",
                        Id = Guid.Empty,
                        Description = "",
                        Theme = "CMG"
                    };
                }

                List<SelectListItem> themes = new List<SelectListItem>();
                themes.Add(new SelectListItem { Text = "Connaissances Militaire Générale", Value = "CMG", Selected = true });
                themes.Add(new SelectListItem { Text = "Combat", Value = "Combat" });
                themes.Add(new SelectListItem { Text = "Génie", Value = "Genie" });
                themes.Add(new SelectListItem { Text = "ISTC", Value = "ISTC" });
                themes.Add(new SelectListItem { Text = "NRBC", Value = "NRBC" });
                themes.Add(new SelectListItem { Text = "Renseignement", Value = "Renseignement" });
                themes.Add(new SelectListItem { Text = "Secourisme", Value = "Secourisme" });
                themes.Add(new SelectListItem { Text = "Transmission", Value = "Transmission" });
                themes.Add(new SelectListItem { Text = "Topographie", Value = "Topographie" });
                themes.Add(new SelectListItem { Text = "Autres", Value = "Autres" });

                AjoutCoursViewModel vm = new AjoutCoursViewModel
                {
                    Cours = toModif,
                    Themes = themes
                };

                return PartialView("AfficherPopUpModifierCours", vm);
            }
            catch(Exception e)
            {
                return new HttpStatusCodeResult(500);
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult ModifierCours(AjoutCoursViewModel vm)
        {
            try
            {
                Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
                if (u == null)
                {
                    FormsAuthentication.SignOut();
                    return new HttpUnauthorizedResult();
                }
                if (u.Equals(typeof(UtilisateurNull)))
                {
                    FormsAuthentication.SignOut();
                    ViewBag.Erreur = ((UtilisateurNull)u).Error;
                    return new HttpUnauthorizedResult();
                }
                if (u.PremiereCo)
                    return new HttpUnauthorizedResult();

                if (u.Role > 3)
                {
                    return new HttpUnauthorizedResult();
                }

                string oldFichier = vm.Cours.Fichier;

                var nveauFichier = Request.Files["nveauFichier"];
                if (nveauFichier != null)
                {
                    int taille = nveauFichier.ContentLength;
                    if (taille/1024 >= 2097152)
                    {
                        return new HttpStatusCodeResult(400);
                    }

                    string path = HttpContext.Server.MapPath("~/Content/Cours/") + vm.Cours.Theme + "/" + nveauFichier.FileName;
                    string newUrl = "/Content/Cours/" + vm.Cours.Theme + "/" + nveauFichier.FileName;

                    nveauFichier.SaveAs(path);
                    if (System.IO.File.Exists(oldFichier))
                        System.IO.File.Delete(oldFichier);

                    vm.Cours.Fichier = newUrl;
                    vm.Cours.Extension = Path.GetExtension(newUrl);
                }

                int retour = cDal.ModifierCours(vm.Cours.Id, vm.Cours);
                if(retour != 1)
                {
                    return new HttpStatusCodeResult(400);
                }

                return RedirectToAction("AfficherListeThemes");
            }
            catch(Exception e)
            {
                return new HttpStatusCodeResult(500);
            }
        }

        [Authorize]
        public ActionResult ChantsEtTraditions(string success = "", string erreur = "")
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

            ViewBag.Erreur = erreur;
            ViewBag.succes = success;

            return View();
        }

        [Authorize]
        public ActionResult AfficherListeTypeChants()
        {
            return PartialView("AfficherListeTypeChants");
        }

        [Authorize]
        public ActionResult AfficherChants(string type)
        {
            try
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

                List<Chant> chants = chDal.GetChantsByType(type);

                return PartialView("AfficherChants", chants);
            }
            catch(Exception e)
            {
                return new HttpStatusCodeResult(500);
            }
        }

        [Authorize]
        public ActionResult Chant (string titre, string success = "")
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

            ViewBag.Succes = "";

            Chant chant = chDal.GetChantByTitre(titre);
            if(chant == null || chant.Equals(typeof(ChantNull)))
            {
                chant = new Chant
                {
                    Titre = "Chant introuvable",
                    Id = Guid.Empty
                };
            }

            ViewBag.Succes = success;

            return View(chant);
        }

        [Authorize]
        public ActionResult AjouterChant()
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

            if(u.Role > 3)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Grade = u.Grade;
            ViewBag.Nom = u.Nom.ToUpperInvariant();
            ViewBag.Role = u.Role;

            Chant chant = new Chant
            {
                Titre = "",
                Texte = ""
            };

            List<SelectListItem> types = new List<SelectListItem>();
            types.Add(new SelectListItem { Text = "Chant", Value = "trad", Selected = true });
            types.Add(new SelectListItem { Text = "Chant Popote", Value = "popote" });

            AjoutModifChantViewModel vm = new AjoutModifChantViewModel
            {
                Chant = chant,
                Types = types
            };

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult AjouterChant(AjoutModifChantViewModel vm)
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

            if (u.Role > 3)
            {
                return RedirectToAction("ChantsEtTraditions");
            }

            ViewBag.Grade = u.Grade;
            ViewBag.Nom = u.Nom.ToUpperInvariant();
            ViewBag.Role = u.Role;

            ViewBag.Erreur = "";

            List<SelectListItem> types = new List<SelectListItem>();
            types.Add(new SelectListItem { Text = "Chant", Value = "trad", Selected = true });
            types.Add(new SelectListItem { Text = "Chant Popote", Value = "popote" });
            vm.Types = types;

            if (chDal.ValiderTitreChant(vm.Chant.Titre))
            {
                ModelState.AddModelError("Chant.Titre", "Un chant avec ce titre existe déjà.");
                return View(vm);
            }

            var type = Request.Form["typeChant"];
            vm.Chant.Type = type;

            Guid idChant = chDal.AjouterChant(vm.Chant);
            if(idChant.Equals(Guid.Empty))
            {
                ViewBag.Erreur = "Une erreur s'est produite lors de la création du nouveau chant. Veuillez réessayer plus tard.";
                return View(vm);
            }

            return RedirectToAction("Chant", new { titre = vm.Chant.Titre });
        }

        [Authorize]
        public ActionResult AfficherPopUpSupprimerChant(Guid id)
        {
            try
            {
                Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
                if (u == null)
                {
                    FormsAuthentication.SignOut();
                    return new HttpUnauthorizedResult();
                }
                if (u.Equals(typeof(UtilisateurNull)))
                {
                    FormsAuthentication.SignOut();
                    ViewBag.Erreur = ((UtilisateurNull)u).Error;
                    return new HttpUnauthorizedResult();
                }
                if (u.PremiereCo)
                    return new HttpUnauthorizedResult();

                if (u.Role > 3)
                {
                    return new HttpUnauthorizedResult();
                }

                Chant chant = chDal.GetChantById(id);
                if(chant == null || chant.Equals(typeof(ChantNull)))
                {
                    chant = new Chant
                    {
                        Titre = "Empty",
                        Id = Guid.Empty
                    };
                }

                return PartialView("AfficherPopUpSupprimerChant", chant);
            }
            catch(Exception e) {
                return new HttpStatusCodeResult(500);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult SuuprimerChant()
        {
            try
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

                if (u.Role > 3)
                {
                    return RedirectToAction("Index", "Home");
                }

                ViewBag.Grade = u.Grade;
                ViewBag.Nom = u.Nom.ToUpperInvariant();
                ViewBag.Role = u.Role;

                string err = "";
                string suc = "";

                Guid idChant = Guid.Parse(Request.Form["idChant"]);
                if (idChant.Equals(Guid.Empty))
                {
                    err = "Une erreur s'est produite lors de la suppression. Veuillez réessayer plus tard.";
                    return RedirectToAction("ChantsEtTraditions", new { erreur = err });
                }

                int result = chDal.SupprimerChant(idChant);
                if(result != 1)
                {
                    err = "Une erreur s'est produite lors de la suppression. Veuillez réessayer plus tard.";
                    return RedirectToAction("ChantsEtTraditions", new { erreur = err });
                }

                suc = "Chant supprimé avec succés.";
                return RedirectToAction("ChantsEtTraditions", new { success = suc });
            }
            catch(Exception e)
            {
                string err = "Une erreur s'est produite lors de la suppression. Veuillez réessayer plus tard.";
                return RedirectToAction("ChantsEtTraditions", new { erreur = err });
            }
        }

        [Authorize]
        public ActionResult ModifierChant(string titre)
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

            if (u.Role > 3)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Grade = u.Grade;
            ViewBag.Nom = u.Nom.ToUpperInvariant();
            ViewBag.Role = u.Role;

            Chant chant = chDal.GetChantByTitre(titre);
            if(chant == null || chant.Equals(typeof(ChantNull)))
            {
                chant = new Chant
                {
                    Titre = "",
                    Type = "",
                    Id = Guid.Empty
                };
            }

            List<SelectListItem> types = new List<SelectListItem>();
            if(chant.Type.Equals("popote"))
            {
                types.Add(new SelectListItem { Text = "Chant", Value = "trad" });
                types.Add(new SelectListItem { Text = "Chant Popote", Value = "popote", Selected = true });
            }
            else
            {
                types.Add(new SelectListItem { Text = "Chant", Value = "trad", Selected = true });
                types.Add(new SelectListItem { Text = "Chant Popote", Value = "popote" });
            }

            AjoutModifChantViewModel vm = new AjoutModifChantViewModel
            {
                Chant = chant,
                Types = types
            };

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ModifierChant (AjoutModifChantViewModel vm)
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

            if (u.Role > 3)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Grade = u.Grade;
            ViewBag.Nom = u.Nom.ToUpperInvariant();
            ViewBag.Role = u.Role;

            ViewBag.Erreur = "";

            var type = Request.Form["typeChant"];
            vm.Chant.Type = type;

            List<SelectListItem> types = new List<SelectListItem>();
            if (vm.Chant.Type.Equals("popote"))
            {
                types.Add(new SelectListItem { Text = "Chant", Value = "trad" });
                types.Add(new SelectListItem { Text = "Chant Popote", Value = "popote", Selected = true });
            }
            else
            {
                types.Add(new SelectListItem { Text = "Chant", Value = "trad", Selected = true });
                types.Add(new SelectListItem { Text = "Chant Popote", Value = "popote" });
            }
            vm.Types = types;

            if (chDal.ValiderTitreChant(vm.Chant.Titre))
            {
                ModelState.AddModelError("Chant.Titre", "Un chant avec ce titre existe déjà.");
                return View(vm);
            }

            int retour = chDal.ModifierChant(vm.Chant.Id, vm.Chant);
            if (retour != 1)
            {
                ViewBag.Erreur = "Une erreur s'est produite lors de la création du nouveau chant. Veuillez réessayer plus tard.";
                return View(vm);
            }

            string suc = "Modification apportée avec succés.";

            return RedirectToAction("Chant", new { titre = vm.Chant.Titre, success = suc });
        }
    }
}