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
    public class ProfilController : Controller
    {
        private IUtilisateurDal uDal;
        private IAdresseDal aDal;
        private IGroupeDal gDal;
        private ISectionDal sDal;
        private ICompagnieDal cDal;

        public ProfilController()
        {
            uDal = new UtilisateurDal();
            aDal = new AdresseDal();
            gDal = new GroupeDal();
            sDal = new SectionDal();
            cDal = new CompagnieDal();
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

            Adresse a = aDal.GetAdresseById(u.Adresse);
            if (a == null || a.Equals(typeof(AdresseNull)))
                a = new Adresse { 
                    Pays = "",
                    CodePostal = "",
                    Ville = "",
                    Voie = ""
                };

            Groupe g = gDal.GetGroupeById(u.Groupe);
            if (g == null || g.Equals(typeof(GroupeNull)))
                g = new Groupe {
                    CDG = Guid.Empty,
                    Numero = -1,
                    Section = Guid.Empty
                };

            Section s = sDal.GetSectionById(g.Section);
            if (s == null || s.Equals(typeof(SectionNull)))
                s = new Section { 
                    Numero = -1,
                    CDS = Guid.Empty,
                    SOA = Guid.Empty,
                    Chant = "",
                    Compagnie = Guid.Empty,
                    Devise = "",
                    NumCie = -1
                };

            Compagnie c = cDal.GetCompagnieById(s.Compagnie);
            if (c == null || c.Equals(typeof(CompagnieNull)))
                c = new Compagnie { 
                    Numero = -1,
                    ADU = Guid.Empty,
                    Chant = "",
                    Devise = "",
                    CDU = Guid.Empty
                };

            Utilisateur cdg = uDal.GetUtilisateurById(g.CDG);
            if (cdg == null || cdg.Equals(typeof(UtilisateurNull)))
                cdg = new Utilisateur { 
                    Nom = "",
                    Prenom = "",
                    Grade = ""
                };

            Utilisateur cds = uDal.GetUtilisateurById(s.CDS);
            if (cds == null || cds.Equals(typeof(UtilisateurNull)))
                cds = new Utilisateur { 
                    Prenom = "",
                    Nom = "",
                    Grade = ""
                };

            Utilisateur soa = uDal.GetUtilisateurById(s.SOA);
            if (soa == null || soa.Equals(typeof(UtilisateurNull)))
                soa = new Utilisateur { 
                    Prenom = "",
                    Nom = "",
                    Grade = ""
                };

            Utilisateur cdu = uDal.GetUtilisateurById(c.CDU);
            if (cdu == null || cdu.Equals(typeof(UtilisateurNull)))
                cdu = new Utilisateur { 
                    Prenom = "",
                    Nom = "",
                    Grade = ""
                };

            ProfilViewModel vm = new ProfilViewModel()
            {
                Util = u,
                Adr = a,
                Grp = g,
                Section = s,
                Cie = c,
                Cdg = cdg,
                Cds = cds,
                Soa = soa,
                Cdu = cdu
            };

            return View(vm);
        }

        [Authorize]
        public ActionResult Modifier ()
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

            Adresse adr = aDal.GetAdresseById(u.Adresse);
            if (adr == null || adr.Equals(typeof(AdresseNull)))
                adr = new Adresse();

            ModifProfilViewModel vm = new ModifProfilViewModel()
            {
                Util = u,
                Adr = adr
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public ActionResult Modifier(ModifProfilViewModel vm)
        {
            if(ModelState.IsValid)
            {
                bool allValide = true;
                if (!ValideMail(vm.Util.Email))
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

                int erreur = 0;
                erreur = aDal.ModifierAdresse(vm.Util.Adresse, vm.Adr);
                if (erreur != 1)
                {
                    ViewBag.Erreur = "Une erreur s'est produite avec votre adresse.";
                    return RedirectToAction("Modifier", "Profil");
                }

                erreur = uDal.ModifierUtilisateur(vm.Util.Id, vm.Util);
                if (erreur != 1)
                {
                    ViewBag.Erreur = "Une erreur s'est produite avec vos informations.";
                    return RedirectToAction("Modifier", "Profil");
                }

                if (erreur != 1)
                {
                    ViewBag.Erreur = "Une erreur s'est produite. Veuillez réessayer.";
                    return RedirectToAction("Modifier", "Profil");
                }

                return RedirectToAction("Index", "Profil");

            }
            ViewBag.Erreur = "Une erreur s'est produite. Veuillez réessayer.";
            return RedirectToAction("Modifier", "Profil");
        }

        [Authorize]
        public ActionResult ModifMdp()
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

            ModifMdpViewModel vm = new ModifMdpViewModel()
            {
                Old = "",
                New = "",
                NewBis = "",
                IdUtil = u.Id
            };

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public ActionResult ModifMdp (ModifMdpViewModel vm)
        {
            if(ModelState.IsValid)
            {
                bool toutOk = true;

                if(!ValideMotDePasse(vm.New, vm.NewBis))
                {
                    ModelState.AddModelError("NewBis", "Les mots de passe doivent être identique.");
                    toutOk = false;
                }

                int retour = uDal.ChangerMotDePasse(vm.IdUtil, vm.Old, vm.New, vm.NewBis);
                switch(retour)
                {
                    case 0:
                        ModelState.AddModelError("NewBis", "Le nouveau mot de passe doit être différent de l'ancien.");
                        toutOk = false;
                        break;
                    case -1:
                        ViewBag.Erreur = "Une erreur est survenu lors du changement de mot de passe.";
                        toutOk = false;
                        break;
                    case -2:
                        ModelState.AddModelError("Old", "Mot de passe incorrect.");
                        toutOk = false;
                        break;
                    case -10:
                        ViewBag.Erreur = "Une erreur est survenue lors du changement de votre mot de passe.";
                        toutOk = false;
                        break;
                }

                if (toutOk)
                    return RedirectToAction("Index", "Home");
                else
                    return View(vm);
            }

            ViewBag.Erreur = "Vérifiez les informations renseignées.";
            return View(vm);
        }
    }
}