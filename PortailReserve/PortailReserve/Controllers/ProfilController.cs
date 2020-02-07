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

            Adresse a = aDal.GetAdresseById(u.Adresse);
            if (a == null || a.Equals(typeof(AdresseNull)))
                a = new Adresse();

            Groupe g = gDal.GetGroupeById(u.Groupe);
            if (g == null || g.Equals(typeof(GroupeNull)))
                g = new Groupe();

            Section s = sDal.GetSectionById(g.Section);
            if (s == null || s.Equals(typeof(SectionNull)))
                s = new Section();

            Compagnie c = cDal.GetCompagnieById(s.Compagnie);
            if (c == null || c.Equals(typeof(CompagnieNull)))
                c = new Compagnie();

            Utilisateur cdg = gDal.GetCdg(u.Groupe);
            if (cdg == null || cdg.Equals(typeof(UtilisateurNull)))
                cdg = new Utilisateur();

            Utilisateur cds = uDal.GetUtilisateurById(s.CDS);
            if (cds == null || cds.Equals(typeof(UtilisateurNull)))
                cds = new Utilisateur();

            Utilisateur soa = uDal.GetUtilisateurById(s.SOA);
            if (soa == null || soa.Equals(typeof(UtilisateurNull)))
                soa = new Utilisateur();

            Utilisateur cdu = uDal.GetUtilisateurById(c.CDU);
            if (cdu == null || cdu.Equals(typeof(UtilisateurNull)))
                cdu = new Utilisateur();

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
    }
}