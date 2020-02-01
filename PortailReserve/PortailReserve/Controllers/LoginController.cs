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
    public class LoginController : Controller
    {
        private IUtilisateurDal uDal;
        private IAdresseDal aDal;

        public LoginController()
        {
            uDal = new UtilisateurDal();
            aDal = new AdresseDal();
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
                if (!ValideMatricule(utilisateur.Matricule))
                {
                    ModelState.AddModelError("MotDePasse", "Matricule ou mot de passe incorrect.");
                    return View(utilisateur);
                }

                Utilisateur u = uDal.Authentifier(utilisateur.Matricule, utilisateur.MotDePasse);

                if (u != null && !u.Equals(typeof(UtilisateurNull)))
                {
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
                return RedirectToAction("Index");
            }
            if (u.Equals(typeof(UtilisateurNull)))
            {
                FormsAuthentication.SignOut();
                ViewBag.Erreur = ((UtilisateurNull)u).Error;
                return RedirectToAction("Index");
            }
            if (!u.PremiereCo)
                return RedirectToAction("Index", "Home");

            PremiereCoViewModel vm = new PremiereCoViewModel()
            {
                Util = u,
                Adr = aDal.GetAdresseById(u.Adresse),
                Mdp = "",
                MdpBis = ""
            };

            return View(vm);
        }

        [HttpPost]
        public ActionResult PremiereCo (PremiereCoViewModel vm)
        {
            if (ModelState.IsValid)
            {
                bool allValide = true;
                if (!ValideMotDePasse(vm.Mdp, vm.MdpBis))
                {
                    ModelState.AddModelError("Mdp", "Mot de passe invalide ou pas identique.");
                    allValide = false;
                }
                if(!ValideMail(vm.Util.Email))
                {
                    ModelState.AddModelError("Util.Email", "Adresse mail invalide.");
                    allValide = false;
                }
                if (!ValideTel(vm.Util.Telephone))
                {
                    ModelState.AddModelError("Utils.Telephone", "Numéro de téléphone invalide.");
                    allValide = false;
                }
                if (!ValideCodePostal(vm.Adr.CodePostal))
                {
                    ModelState.AddModelError("Adr.CodePostal", "Code postal invalide.");
                    allValide = false;
                }
                if (!allValide)
                    return View(vm);


                return RedirectToAction("Index", "Home");
            }
            return View(vm);
        }

        public ActionResult Deconnexion ()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");

        }
    }
}