using PortailReserve.DAL;
using PortailReserve.DAL.Impl;
using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PortailReserve.Controllers
{
    public class HomeController : Controller
    {
        private IUtilisateurDal uDal;
        private IAdresseDal aDal;

        public HomeController()
        {
            uDal = new UtilisateurDal();
            aDal = new AdresseDal();
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

        public ActionResult ResetBdd ()
        {
            IDatabaseInitializer<BddContext> init = new DropCreateDatabaseAlways<BddContext>();
            Database.SetInitializer(init);
            init.InitializeDatabase(new BddContext());

            Adresse a = new Adresse()
            {
                Ville = "Chateaugay",
                CodePostal = "63119",
                Voie = "15 rue des Rouchats",
                Pays = "France"
            };
            Guid adr = aDal.AjouterAdresse(a);

            Utilisateur u = new Utilisateur()
            {
                Matricule = "1763041044",
                Nom = "Maucotel",
                Prenom = "Benoit",
                Telephone = "0643849575",
                Email = "benoit.maucotel@gmail.com",
                Naissance = new DateTime(1997, 9, 4),
                Adresse = adr,
                Grade = "Caporal",
                MotDePasse = "changeme",
                Role = 1,
                PremiereCo = true
            };
            uDal.AjouterUtilisateur(u);

            return RedirectToAction("Index", "Login");
        }

    }
}