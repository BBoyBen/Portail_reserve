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
        public ActionResult Index(string erreur = "")
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

            ViewBag.Erreur = erreur;

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
                return RedirectToAction("Index", "Souvenir");

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
                return RedirectToAction("Index", "Souvenir");

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

            return RedirectToAction("Album", new { id = idAlbum });
        }

        [Authorize]
        public ActionResult Album (Guid id)
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

            if (u.Compagnie == -1)
                return RedirectToAction("Index", "Home");

            ViewBag.Grade = u.Grade;
            ViewBag.Nom = u.Nom.ToUpperInvariant();
            ViewBag.Role = u.Role;

            int numCie = u.Compagnie;

            Album album = aDal.GetAlbumById(id);
            if (album == null && album.Equals(typeof(AlbumNull)))
                return RedirectToAction("Index", "Souvenir");

            if (album.Cie != numCie)
                return RedirectToAction("Index", "Home");

            AlbumViewModel vm = new AlbumViewModel
            {
                Album = album,
                Photos = new List<Photo>(),
                NbPhotos = 0
            };

            return View(vm);
        }

        [Authorize]
        public ActionResult AfficherListePhotos(Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                AlbumViewModel vmEmpty = new AlbumViewModel
                {
                    Album = new Album(),
                    Photos = new List<Photo>(),
                    NbPhotos = 0
                };

                return PartialView("AfficherListePhotos", vmEmpty);
            }

            List<Photo> photos = pDal.GetPhotosByAlbum(id);
            int nbPhotos = photos.Count;

            AlbumViewModel vm = new AlbumViewModel
            {
                Album = new Album(),
                Photos = photos,
                NbPhotos = nbPhotos
            };

            return PartialView("AfficherListePhotos", vm);
        }

        [Authorize]
        public ActionResult AfficherListePhotosPourSuppression(Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                AlbumViewModel vmEmpty = new AlbumViewModel
                {
                    Album = new Album(),
                    Photos = new List<Photo>(),
                    NbPhotos = 0
                };

                return PartialView("AfficherListePhotos", vmEmpty);
            }
            Album album = aDal.GetAlbumById(id);

            List<Photo> photos = pDal.GetPhotosByAlbum(id);
            int nbPhotos = photos.Count;

            AlbumViewModel vm = new AlbumViewModel
            {
                Album = album,
                Photos = photos,
                NbPhotos = nbPhotos
            };

            return PartialView("AfficherListePhotosPourSuppression", vm);
        }

        [HttpPost]
        [Authorize]
        public ActionResult SupprimerPhotos()
        {
            try
            {
                Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
                if (u == null)
                {
                    FormsAuthentication.SignOut();
                    return new HttpUnauthorizedResult();
                }
                if (u.Equals(typeof(UtilisateurNull)))
                {
                    FormsAuthentication.SignOut();
                    ViewBag.Erreur = ((UtilisateurNull)u).Error;
                    return new HttpUnauthorizedResult();
                }
                if (u.PremiereCo)
                    return new HttpUnauthorizedResult();

                Guid idAlbum = Guid.Parse(Request.Form["idAlbum"]);
                if (idAlbum.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400);

                List<Photo> photos = pDal.GetPhotosByAlbum(idAlbum);
                List<Photo> toDelete = new List<Photo>();
                foreach(Photo p in photos)
                {
                    var checkBoxPhoto = Request.Form[p.Id.ToString()];
                    if (checkBoxPhoto != null && checkBoxPhoto.Equals("on"))
                        toDelete.Add(p);
                }

                foreach(Photo trash in toDelete)
                {
                    if (System.IO.File.Exists(trash.Fichier))
                        System.IO.File.Delete(trash.Fichier);

                    pDal.SupprimerPhoto(trash.Id);
                }

                return RedirectToAction("AfficherListePhotosPourSuppression", new { id = idAlbum });
            }
            catch(Exception e)
            {
                return new HttpStatusCodeResult(400, "Erreur de la suppression des photos.");
            }
        }

        [Authorize]
        public ActionResult AfficherPopUpSupprimerAlbum(Guid id)
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

            if (u.Compagnie == -1)
                return RedirectToAction("Index", "Home");

            ViewBag.Grade = u.Grade;
            ViewBag.Nom = u.Nom.ToUpperInvariant();
            ViewBag.Role = u.Role;

            Album album = aDal.GetAlbumById(id);
            if (album == null || album.Equals(typeof(AlbumNull)))
                album = new Album
                {
                    Titre = "Empty",
                    Id = Guid.Empty
                };

            return PartialView("AfficherPopUpSupprimerAlbum", album);
        }

        [HttpPost]
        [Authorize]
        public ActionResult SupprimerAlbum()
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
                return RedirectToAction("Index", "Souvenir");

            ViewBag.Grade = u.Grade;
            ViewBag.Nom = u.Nom.ToUpperInvariant();
            ViewBag.Role = u.Role;

            int numCie = u.Compagnie;

            Guid idAlbum = Guid.Parse(Request.Form["idAlbum"]);
            if(idAlbum.Equals(Guid.Empty))
            {
                string e = "Erreur lors de la suppression de l'album.";
                return RedirectToAction("Index", "Souvenir", new { erreur = e });
            }

            Album album = aDal.GetAlbumById(idAlbum);
            if(album == null || album.Equals(typeof(AlbumNull)))
            {
                string e = "Erreur lors de la suppression de l'album.";
                return RedirectToAction("Index", "Souvenir", new { erreur = e });
            }

            int retour = aDal.SupprimerAlbum(idAlbum);

            if (retour == 1)
            {
                Directory.Delete(HttpContext.Server.MapPath("~/Content/Souvenirs/") + numCie + "/" + album.Dossier, true);
            }
            else
            {
                string e = "Erreur lors de la suppression de l'album.";
                return RedirectToAction("Index", "Souvenir", new { erreur = e });
            }

            return RedirectToAction("Index", "Souvenir");
        }

        [Authorize]
        public ActionResult AfficherPopUpAjouterPhoto(Guid id)
        {
            if (id.Equals(Guid.Empty))
            {
                Album albumIdEmpty = new Album
                {
                    Titre = "Empty",
                    Cie = 0,
                    Id = Guid.Empty
                };

                return PartialView("AfficherPopUpAjouterPhoto", albumIdEmpty);
            }

            Album album = aDal.GetAlbumById(id);
            if(album == null || album.Equals(typeof(AlbumNull)))
                album = new Album
                {
                    Titre = "Empty",
                    Cie = 0,
                    Id = Guid.Empty
                };

            return PartialView("AfficherPopUpAjouterPhoto", album);
        }

        [HttpPost]
        [Authorize]
        public ActionResult AjouterPhotos()
        {
            try
            {
                Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
                if (u == null)
                {
                    FormsAuthentication.SignOut();
                    return new HttpUnauthorizedResult();
                }
                if (u.Equals(typeof(UtilisateurNull)))
                {
                    FormsAuthentication.SignOut();
                    ViewBag.Erreur = ((UtilisateurNull)u).Error;
                    return new HttpUnauthorizedResult();
                }
                if (u.PremiereCo)
                    return new HttpUnauthorizedResult();

                Guid idAlbum = Guid.Parse(Request.Form["idAlbum"]);
                if (idAlbum.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400);

                int numCie = Int32.Parse(Request.Form["cie"]);
                if (numCie < 1)
                    return new HttpStatusCodeResult(400);

                string dossier = Request.Form["dossier"];

                string cheminDossier = HttpContext.Server.MapPath("~/Content/Souvenirs/") + numCie + "/" + dossier;

                if (!Directory.Exists(cheminDossier))
                    return new HttpStatusCodeResult(400);

                int nbPhotoErreur = 0;
                int nbPhotos = Request.Files.Count;
                for (int i = 0; i < nbPhotos; i++)
                {
                    var photo = Request.Files[i];

                    string path = cheminDossier + "/" + photo.FileName;
                    photo.SaveAs(path);
                    string url = "/Content/Souvenirs/" + numCie + "/" + dossier + "/" + photo.FileName;

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

                if (nbPhotoErreur == 1)
                    ViewBag.Erreur = nbPhotoErreur + " photo n'a pas été importée sur " + nbPhotos;

                return RedirectToAction("AfficherListePhotos", new { id = idAlbum });
            }
            catch(Exception e)
            {
                return new HttpStatusCodeResult(400, "Erreur de l'ajout des photos.");
            }
        }
    }
}