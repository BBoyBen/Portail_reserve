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

                return RedirectToAction("AfficherListeThemes");
            }
            catch(Exception e)
            {
                return new HttpStatusCodeResult(500);
            }
        }
    }
}