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

            Utilisateur cdg = uDal.GetUtilisateurById(g.CDG);
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
    }
}