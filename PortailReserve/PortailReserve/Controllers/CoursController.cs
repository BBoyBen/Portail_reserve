using Microsoft.Extensions.Logging.Abstractions;
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
using System.Web.Security;

namespace PortailReserve.Controllers
{
    public class CoursController : Controller
    {
        private IUtilisateurDal uDal;
        private ICoursDal cDal;

        public CoursController()
        {
            uDal = new UtilisateurDal();
            cDal = new CoursDal();
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
                if (taille >= 4096000)
                {
                    return new HttpStatusCodeResult(400);
                }

                string path = HttpContext.Server.MapPath("~/Content/Cours/") + toSave.Theme + "/" + fichierCours.FileName;
                fichierCours.SaveAs(path);
                string url = "/Content/Cours/" + toSave.Theme + "/" + fichierCours.FileName;

                toSave.Fichier = url;

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
    }
}