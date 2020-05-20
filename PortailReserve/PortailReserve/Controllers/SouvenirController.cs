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
    public class SouvenirController : Controller
    {
        private IUtilisateurDal uDal;
        private IAlbumDal aDal;

        public SouvenirController()
        {
            uDal = new UtilisateurDal();
            aDal = new AlbumDal();
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

            int numCie = u.Compagnie;

            List<Album> albums = aDal.GetAlbumsByCie(numCie);

            ListeAlbumsViewModel vm = new ListeAlbumsViewModel
            {
                Albums = albums,
                Count = albums.Count,
                NumCie = numCie
            };

            return View(vm);
        }
    }
}