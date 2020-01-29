using PortailReserve.DAL;
using PortailReserve.DAL.Impl;
using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PortailReserve.Controllers
{
    public class LoginController : Controller
    {
        private IUtilisateurDal uDal;

        public LoginController()
        {
            uDal = new UtilisateurDal();
        }
        // GET: Login
        public ActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home", null);

            return View();
        }

        [HttpPost]
        public ActionResult Index(Utilisateur utilisateur)
        {
            if (ModelState.IsValid)
            {
                if (!Utils.Utils.ValideMatricule(utilisateur.Matricule))
                {
                    ModelState.AddModelError("MotDePasse", "Matricule ou mot de passe incorrect.");
                    return View(utilisateur);
                }

                Utilisateur u = uDal.Authentifier(utilisateur.Matricule, utilisateur.MotDePasse);
                if (!u.Equals(typeof(UtilisateurNull)))
                {
                    if (u == null || u.Equals(typeof(UtilisateurNull)))
                        return View("Error");

                    FormsAuthentication.SetAuthCookie(u.Id.ToString(), false);

                    if (u.PremiereCo)
                        return RedirectToAction("PremiereCo");

                    return Redirect("/");
                }
                ModelState.AddModelError("MotDePasse", "Matricule ou mot de passe incorrect.");
            }
            ModelState.AddModelError("MotDePasse", "Matricule ou mot de passe incorrect.");
            return View(utilisateur);
        }

        [Authorize]
        public ActionResult PremiereCo ()
        {
            Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
            if(u == null)
            {
                FormsAuthentication.SignOut();
                return View("Error");
            }
            if (u.Equals(typeof(UtilisateurNull)))
            {
                FormsAuthentication.SignOut();
                ViewBag.Erreur = ((UtilisateurNull)u).Error;
                return RedirectToAction("Index");
            }
            if (!u.PremiereCo)
                return RedirectToAction("Index", "Home");

            return View(u);
        }
    }
}