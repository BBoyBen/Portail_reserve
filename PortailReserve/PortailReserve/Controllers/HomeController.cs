using PortailReserve.DAL;
using PortailReserve.DAL.Impl;
using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using PortailReserve.ViewModel;
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
        private IGroupeDal gDal;
        private ISectionDal sDal;
        private ICompagnieDal cDal;
        private IEvenementDal eDal;

        public HomeController()
        {
            uDal = new UtilisateurDal();
            aDal = new AdresseDal();
            gDal = new GroupeDal();
            sDal = new SectionDal();
            cDal = new CompagnieDal();
            eDal = new EvenementDal();
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

            Evenement prochain = eDal.GetProchainEvenement();
            AcceuilViewModel vm = new AcceuilViewModel()
            {
                ProchainEvent = prochain,
                HasProchainEvent = true
            };

            if (prochain == null)
                vm.HasProchainEvent = false;

            return View(vm);
        }

        public ActionResult ResetBdd ()
        {
            IDatabaseInitializer<BddContext> init = new DropCreateDatabaseAlways<BddContext>();
            Database.SetInitializer(init);
            init.InitializeDatabase(new BddContext());

            

            Evenement e = new Evenement()
            {
                Nom = "Week-end d'instruction Février",
                Debut = new DateTime(2020, 2, 15),
                Fin = new DateTime(2020, 2, 16),
                Type = "Instruction",
                Lieu = "Quartier - 92e RI",
                Description = "Week end d'instruction au quartier pour continuer la préparation sentinelle"
            };
            eDal.CreerEvenement(e);

            Utils.ImportCsv.InitBddByCsv();

            return RedirectToAction("Index", "Login");
        }

    }
}