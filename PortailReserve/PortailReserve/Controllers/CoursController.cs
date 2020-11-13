using PortailReserve.DAL;
using PortailReserve.DAL.Impl;
using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using PortailReserve.Utils;
using PortailReserve.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Web.Security;

namespace PortailReserve.Controllers
{
    public class CoursController : Controller
    {
        private IUtilisateurDal uDal;
        private ICoursDal cDal;
        private IChantDal chDal;
        private readonly Logger LOGGER;

        public CoursController()
        {
            uDal = new UtilisateurDal();
            cDal = new CoursDal();
            chDal = new ChantDal();
            LOGGER = new Logger(this.GetType());
        }

        [Authorize]
        public ActionResult Index()
        {
            try {
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
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur lors de l'affichage de la page index de cours -> " + e);
                return new HttpStatusCodeResult(500, "Echec affichage de la page -> " + e.Message);
            }
        }

        [Authorize]
        public ActionResult AfficherListeThemes()
        {
            try
            {
                return PartialView("AfficherListeThemes");
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur lors de l'affichage de la liste des thèmes -> " + e);
                return new HttpStatusCodeResult(500, "Echec affichage de la liste des themes -> " + e.Message);
            }
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
                LOGGER.Log("ERROR", "Erreur affichage liste des cours pour le theme : " + theme + " -> " + e);
                return new HttpStatusCodeResult(500);
            }
        }

        [Authorize]
        public ActionResult AfficherPopUpNouveauCours()
        {
            try
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

                AjoutCoursViewModel vm = new AjoutCoursViewModel
                {
                    Cours = cours,
                    Themes = themes
                };

                return PartialView("AfficherPopUpNouveauCours", vm);
            } 
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur affichage de la pop-up nouveau cours -> " + e);
                return new HttpStatusCodeResult(500, "Echec affichage de la pop-up nouveau cours -> " + e.Message);
            }
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
                    LOGGER.Log("ERROR", "Fichier trop volumineux pour ajout cours.");
                    return new HttpStatusCodeResult(400, "Fichier trop volumineux.");
                }

                string path = HttpContext.Server.MapPath("~/Content/Cours/") + toSave.Theme + "/" + fichierCours.FileName;
                fichierCours.SaveAs(path);
                string url = "/Content/Cours/" + toSave.Theme + "/" + fichierCours.FileName;

                toSave.Fichier = url;

                toSave.Extension = Path.GetExtension(url);

                Guid idCours = cDal.AjouterCours(toSave);

                if(idCours.Equals(Guid.Empty))
                {
                    LOGGER.Log("ERROR", "Echec ajout d'un nouveau cours");
                    return new HttpStatusCodeResult(400, "Erreur ajout nouveau cours.");
                }

                return RedirectToAction("AfficherListeThemes");
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur lors de l'ajout d'un nouveau cours -> " + e);
                return new HttpStatusCodeResult(500, "Erreur lors de l'ajout d'un nouveau cours -> " + e.Message);
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
                    return new HttpUnauthorizedResult("Veuillez vous authentifier.");
                }
                if (u.Equals(typeof(UtilisateurNull)))
                {
                    FormsAuthentication.SignOut();
                    ViewBag.Erreur = ((UtilisateurNull)u).Error;
                    return new HttpUnauthorizedResult("Veuillez vous authentifier.");
                }
                if (u.PremiereCo)
                    return new HttpUnauthorizedResult("Ceci est votre première connexion.");

                if(u.Role > 3)
                {
                    return new HttpUnauthorizedResult("Vous n'avez pas le niveau d'autorisation pour cette action.");
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
                LOGGER.Log("ERROR", "Erreur affichage pop-up suppression de cours -> " + e);
                return new HttpStatusCodeResult(500, "Erreur chargement de la pop-up -> " + e.Message);
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
                    return new HttpUnauthorizedResult("Veuillez vous authentifier.");
                }
                if (u.Equals(typeof(UtilisateurNull)))
                {
                    FormsAuthentication.SignOut();
                    ViewBag.Erreur = ((UtilisateurNull)u).Error;
                    return new HttpUnauthorizedResult("Veuillez vous authentifier.");
                }
                if (u.PremiereCo)
                    return new HttpUnauthorizedResult("Ceci est votre première connexion.");

                if (u.Role > 3)
                {
                    return new HttpUnauthorizedResult("Vous n'êtes pas autorisé à réaliser cette action.");
                }

                Guid idCours = Guid.Parse(Request.Form["idCours"]);
                if(idCours.Equals(Guid.Empty))
                {
                    return new HttpStatusCodeResult(400, "Echec récupération de l'id du cours à supprimer.");
                }

                Cours toDelete = cDal.GetCoursById(idCours);
                if(toDelete == null || toDelete.Equals(typeof(CoursNull)))
                {
                    return new HttpStatusCodeResult(400, "Echec récupération du cours à supprimer.");
                }

                int retour = cDal.SupprimerCours(idCours);
                if(retour != 1)
                {
                    return new HttpStatusCodeResult(400, "Echec suppression du cours.");
                }

                if (System.IO.File.Exists(toDelete.Fichier))
                    System.IO.File.Delete(toDelete.Fichier);

                return RedirectToAction("AfficherListeThemes");
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Echec suppression du cours -> " + e);
                return new HttpStatusCodeResult(500, "Erreur lors de la suppression du cours -> " +e.Message);
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
                    return new HttpUnauthorizedResult("Veuillez vous authentifier.");
                }
                if (u.Equals(typeof(UtilisateurNull)))
                {
                    FormsAuthentication.SignOut();
                    ViewBag.Erreur = ((UtilisateurNull)u).Error;
                    return new HttpUnauthorizedResult("Veuillez vous authentifier.");
                }
                if (u.PremiereCo)
                    return new HttpUnauthorizedResult("Ceci est votre première connexion.");

                if (u.Role > 3)
                {
                    return new HttpUnauthorizedResult("Vous n'avez pas l'autorisation pour cette action.");
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
                LOGGER.Log("ERROR", "Erreur affichage pop-up modification de cours -> " + e);
                return new HttpStatusCodeResult(500, "Echec affichage pop-up -> " + e.Message);
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
                    return new HttpUnauthorizedResult("Veuillez vous authentifier.");
                }
                if (u.Equals(typeof(UtilisateurNull)))
                {
                    FormsAuthentication.SignOut();
                    ViewBag.Erreur = ((UtilisateurNull)u).Error;
                    return new HttpUnauthorizedResult("Veuillez vous authentifier.");
                }
                if (u.PremiereCo)
                    return new HttpUnauthorizedResult("Ceci est vote première connexion.");

                if (u.Role > 3)
                {
                    return new HttpUnauthorizedResult("Vous n'avez pas l'autorisation pour cette action.");
                }

                string oldFichier = vm.Cours.Fichier;

                var nveauFichier = Request.Files["nveauFichier"];
                if (nveauFichier != null)
                {
                    int taille = nveauFichier.ContentLength;
                    if (taille/1024 >= 2097152)
                    {
                        return new HttpStatusCodeResult(400, "La fichier est trop volumineux.");
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
                    return new HttpStatusCodeResult(400, "Echec modification du cours.");
                }

                return RedirectToAction("AfficherListeThemes");
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur lors de la modification du cours -> " + e);
                return new HttpStatusCodeResult(500, "Erreur lors de la modification du cours -> " + e.Message);
            }
        }

        [Authorize]
        public ActionResult ChantsEtTraditions(string success = "", string erreur = "")
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

                ViewBag.Erreur = erreur;
                ViewBag.succes = success;

                return View();
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur chargement de la page d'affichage des chants et des traditions -> " + e);
                return new HttpStatusCodeResult(500, "Echec du chargement de la page -> " + e.Message);
            }
        }

        [Authorize]
        public ActionResult AfficherListeTypeChants()
        {
            try
            {
                return PartialView("AfficherListeTypeChants");
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur lors de l'affichage de la liste des types de chant -> " + e);
                return new HttpStatusCodeResult(500, "Echec du chargement de la liste des types de chants -> " + e.Message);
            }
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
                LOGGER.Log("ERROR", "Erreur affichage des chants par types : " + type + " -> " + e);
                return new HttpStatusCodeResult(500);
            }
        }

        [Authorize]
        public ActionResult Chant (string titre, string success = "")
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

                ViewBag.Succes = "";

                Chant chant = chDal.GetChantByTitre(titre);
                if (chant == null || chant.Equals(typeof(ChantNull)))
                {
                    chant = new Chant
                    {
                        Titre = "Chant introuvable",
                        Texte = "",
                        Id = Guid.Empty
                    };
                }

                chant.Texte = chant.Texte.Replace(Environment.NewLine, "<br/>");

                ViewBag.Succes = success;

                return View(chant);
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur de l'affichage du chant : " + titre + " -> " + e);
                return new HttpStatusCodeResult(500, "Echec de l'affichage du chant : " + titre + " -> " + e.Message);
            }
        }

        [Authorize]
        public ActionResult AjouterChant()
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
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur affichage de la page d'ajout de chant -> " + e);
                return new HttpStatusCodeResult(500, "Echec de l'affichage de la page d'ajout de chant -> " + e.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult AjouterChant(AjoutModifChantViewModel vm)
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
                if (idChant.Equals(Guid.Empty))
                {
                    LOGGER.Log("ERROR", "Erreur ajout d'un nouveau chant.");
                    ViewBag.Erreur = "Une erreur s'est produite lors de la création du nouveau chant. Veuillez réessayer plus tard.";
                    return View(vm);
                }

                return RedirectToAction("Chant", new { titre = vm.Chant.Titre });
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur lors de l'ajout d'un nouveau chant -> " + e);
                return new HttpStatusCodeResult(500, "Echec ajout de du nouveau chant -> " + e.Message);
            }
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
                    return new HttpUnauthorizedResult("Veuillez vous authentifier.");
                }
                if (u.Equals(typeof(UtilisateurNull)))
                {
                    FormsAuthentication.SignOut();
                    ViewBag.Erreur = ((UtilisateurNull)u).Error;
                    return new HttpUnauthorizedResult("Veuillez vous authentifier.");
                }
                if (u.PremiereCo)
                    return new HttpUnauthorizedResult("Ceci est votre première connexion.");

                if (u.Role > 3)
                {
                    return new HttpUnauthorizedResult("Vous n'avez pas l'autorisation pour cette action.");
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
            catch(Exception e) 
            {
                LOGGER.Log("ERROR", "Erreur pour l'affichage de la pop-up du suppression de chant -> " + e);
                return new HttpStatusCodeResult(500, "Erreur lors de l'affichage de la pop-up -> " + e.Message);
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
                    LOGGER.Log("ERROR", "Erreur récupération de l'id du chant.");
                    err = "Une erreur s'est produite lors de la suppression. Veuillez réessayer plus tard.";
                    return RedirectToAction("ChantsEtTraditions", new { erreur = err });
                }

                int result = chDal.SupprimerChant(idChant);
                if(result != 1)
                {
                    LOGGER.Log("ERROR", "Erreur suppression du chant : " + idChant);
                    err = "Une erreur s'est produite lors de la suppression. Veuillez réessayer plus tard.";
                    return RedirectToAction("ChantsEtTraditions", new { erreur = err });
                }

                suc = "Chant supprimé avec succés.";
                return RedirectToAction("ChantsEtTraditions", new { success = suc });
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur lors de la suppression d'un chant -> " + e);
                string err = "Une erreur s'est produite lors de la suppression. Veuillez réessayer plus tard.";
                return RedirectToAction("ChantsEtTraditions", new { erreur = err });
            }
        }

        [Authorize]
        public ActionResult ModifierChant(string titre)
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

                Chant chant = chDal.GetChantByTitre(titre);
                if (chant == null || chant.Equals(typeof(ChantNull)))
                {
                    chant = new Chant
                    {
                        Titre = "",
                        Type = "",
                        Texte = "",
                        Id = Guid.Empty
                    };
                }

                List<SelectListItem> types = new List<SelectListItem>();
                if (chant.Type.Equals("popote"))
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
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur lors de l'affichage de la page de modification de chant -> " + e);
                return new HttpStatusCodeResult(500, "Echec affichage de la page de moidifcation de chant -> " + e.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult ModifierChant (AjoutModifChantViewModel vm)
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
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur de la modification du chant -> " + e);
                return new HttpStatusCodeResult(500, "Echech modification du chant -> " + e.Message);
            }
        }
    }
}