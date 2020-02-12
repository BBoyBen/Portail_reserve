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

            Utilisateur cdu = new Utilisateur()
            {
                Matricule = "0000000000",
                Nom = "Rodier",
                Prenom = "Jean",
                Telephone = "0000000000",
                Email = "jean.rodier@gmail.com",
                Naissance = new DateTime(1970, 9, 4),
                Adresse = Guid.Empty,
                Grade = "Capitaine",
                MotDePasse = "changeme",
                Role = 2,
                PremiereCo = true
            };
            Guid idCdu = uDal.AjouterUtilisateur(cdu);

            Utilisateur cds = new Utilisateur()
            {
                Matricule = "0000000001",
                Nom = "Lys",
                Prenom = "Frederique",
                Telephone = "0000000000",
                Email = "fred.lys@gmail.com",
                Naissance = new DateTime(1970, 9, 4),
                Adresse = Guid.Empty,
                Grade = "Adjudant",
                MotDePasse = "changeme",
                Role = 3,
                PremiereCo = true
            };
            Guid idCds = uDal.AjouterUtilisateur(cds);

            Utilisateur soa = new Utilisateur()
            {
                Matricule = "0000000002",
                Nom = "Tanlet",
                Prenom = "Emilie",
                Telephone = "0000000000",
                Email = "emilie.tanlet@gmail.com",
                Naissance = new DateTime(1990, 9, 4),
                Adresse = Guid.Empty,
                Grade = "Sergent-chef",
                MotDePasse = "changeme",
                Role = 3,
                PremiereCo = true
            };
            Guid idSoa = uDal.AjouterUtilisateur(soa);

            Compagnie cie = new Compagnie()
            {
                Numero = 6,
                ADU = Guid.Empty,
                CDU = idCdu,
                Chant = "Les machins blancs",
                Devise = "Ne pas subir !"
            };
            Guid idCie = cDal.AjouterCompagnie(cie);

            Section section = new Section()
            {
                Numero = 1,
                CDS = idCds,
                Compagnie = idCie,
                SOA = idSoa,
                Chant = "Je sais pas",
                Devise = "Tous dans le trou, tous dans le même !"
            };
            Guid idSection = sDal.AjouterSection(section);

            Utilisateur cdg = new Utilisateur()
            {
                Matricule = "0000000003",
                Nom = "Aubernon",
                Prenom = "Matthieu",
                Telephone = "0000000000",
                Email = "matthieu.aubernon@gmail.com",
                Naissance = new DateTime(1985, 9, 4),
                Adresse = Guid.Empty,
                Grade = "Sergent",
                MotDePasse = "changeme",
                Role = 4,
                PremiereCo = true
            };
            Guid idCdg = uDal.AjouterUtilisateur(cdg);

            Groupe grp = new Groupe()
            {
                Numero = 1,
                Section = idSection,
                CDG = idCdg
            };
            Guid idGrp = gDal.AjouterGroupe(grp);

            

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
                Groupe = idGrp,
                PremiereCo = true
            };
            uDal.AjouterUtilisateur(u);

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

            return RedirectToAction("Index", "Login");
        }

    }
}