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
                    ModelState.AddModelError("Utilisateur.Matricule", "Format du matricule invalide");
                    return View();
                }

                Utilisateur u = uDal.Authentifier(utilisateur.Matricule, utilisateur.MotDePasse);
                if (!u.Equals(typeof(UtilisateurNull)))
                {
                    if (u == null)
                        return View("Error");

                    FormsAuthentication.SetAuthCookie(u.Id.ToString(), false);
                    return Redirect("/");
                }
                ModelState.AddModelError("Utilisateur.MotDePasse", "Matricule ou mot de passe incorrect.");
            }
            return View();
        }
    }
}