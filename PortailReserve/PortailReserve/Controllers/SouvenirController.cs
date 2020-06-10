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
using System.IO;
using static PortailReserve.Utils.Logger;

namespace PortailReserve.Controllers
{
    public class SouvenirController : Controller
    {
        private IUtilisateurDal uDal;
        private IAlbumDal aDal;
        private IPhotoDal pDal;

        public SouvenirController()
        {
            uDal = new UtilisateurDal();
            aDal = new AlbumDal();
            pDal = new PhotoDal();
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

        [Authorize]
        public ActionResult Ajouter()
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

            if(u.Role > 3 || u.Compagnie == -1)
                return RedirectToAction("Index", "Login");

            ViewBag.Grade = u.Grade;
            ViewBag.Nom = u.Nom.ToUpperInvariant();
            ViewBag.Role = u.Role;

            Album album = new Album
            {
                Titre = "",
                Description = "",
                Cie = u.Compagnie
            };

            CreationAlbumViewModel vm = new CreationAlbumViewModel
            {
                Album = album,
            };

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Ajouter(CreationAlbumViewModel vm)
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

            if (u.Role > 3 || u.Compagnie == -1)
                return RedirectToAction("Index", "Login");

            ViewBag.Grade = u.Grade;
            ViewBag.Nom = u.Nom.ToUpperInvariant();
            ViewBag.Role = u.Role;

            int numCie = u.Compagnie;

            vm.Album.Cie = numCie;
            string nomAlbum = Utils.Utils.FormatTitreAlbum(vm.Album.Titre);
            vm.Album.Dossier = nomAlbum;

            if(aDal.ExisteParDossierEtCie(nomAlbum, numCie))
            {
                ModelState.AddModelError("Album.Titre", "Titre déjà existant !");
                return View(vm);
            }

            Guid idAlbum = aDal.AjouterAlbum(vm.Album);
            if (idAlbum.Equals(Guid.Empty)){
                ViewBag.Erreur = "Une erreur est survenue lors de l'ajout du nouvel album.";
                return RedirectToAction("Index");
            }
            ViewBag.Erreur = "Test d'erreur !";
            Directory.CreateDirectory(HttpContext.Server.MapPath("~/Content/Souvenirs/") + numCie);
            Directory.CreateDirectory(HttpContext.Server.MapPath("~/Content/Souvenirs/") + numCie + "/" + nomAlbum);
            int nbPhotoErreur = 0;

            int nbPhotos = Request.Files.Count;
            for(int i = 0; i < nbPhotos; i++)
            {
                var photo = Request.Files[i];

                string path = HttpContext.Server.MapPath("~/Content/Souvenirs/") + numCie + "/" + nomAlbum + "/" + photo.FileName;
                photo.SaveAs(path);
                string url = "/Content/Souvenirs/" + numCie + "/" + nomAlbum + "/" + photo.FileName;

                Photo toAdd = new Photo
                {
                    Fichier = url,
                    Album = idAlbum
                };

                Guid idPhoto = pDal.AjouterPhoto(toAdd);

                if (idPhoto.Equals(Guid.Empty))
                    nbPhotoErreur++;
            }

            if (nbPhotoErreur > 1)
                ViewBag.Erreur = nbPhotoErreur + " photos n'ont pas été importées sur " + nbPhotos;

            if(nbPhotoErreur == 1)
                ViewBag.Erreur = nbPhotoErreur + " photo n'a pas été importée sur " + nbPhotos;

            return RedirectToAction("Index");
        }
    }
}