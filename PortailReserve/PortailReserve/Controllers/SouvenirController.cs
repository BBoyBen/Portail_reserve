using PortailReserve.DAL;
using PortailReserve.DAL.Impl;
using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using PortailReserve.ViewModel;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;
using System.IO;
using System.IO.Compression;
using PortailReserve.Utils;

namespace PortailReserve.Controllers
{
    public class SouvenirController : Controller
    {
        private IUtilisateurDal uDal;
        private IAlbumDal aDal;
        private IPhotoDal pDal;
        private readonly Logger LOGGER;

        public SouvenirController()
        {
            uDal = new UtilisateurDal();
            aDal = new AlbumDal();
            pDal = new PhotoDal();
            LOGGER = new Logger(this.GetType());
        }

        [Authorize]
        public ActionResult Index(string erreur = "")
        {
            try
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
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur affichage de la page d'indec des souvenir -> " + e);
                return new HttpStatusCodeResult(500, "Erreur affichage de la page.");
            }
        }

        [Authorize]
        public ActionResult Ajouter()
        {
            try
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
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur affichage de la page d'ajout d'album -> " + e);
                return new HttpStatusCodeResult(500, "Erreur affichage de la page d'ajout d'album.");
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Ajouter(CreationAlbumViewModel vm)
        {
            try
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

                if (aDal.ExisteParDossierEtCie(nomAlbum, numCie))
                {
                    ModelState.AddModelError("Album.Titre", "Titre déjà existant !");
                    return View(vm);
                }

                Guid idAlbum = aDal.AjouterAlbum(vm.Album);
                if (idAlbum.Equals(Guid.Empty))
                {
                    ViewBag.Erreur = "Une erreur est survenue lors de l'ajout du nouvel album.";
                    return RedirectToAction("Index");
                }

                Directory.CreateDirectory(HttpContext.Server.MapPath("~/Content/Souvenirs/") + numCie);
                Directory.CreateDirectory(HttpContext.Server.MapPath("~/Content/Souvenirs/") + numCie + "/" + nomAlbum);

                int nbPhotoErreur = 0;

                int nbPhotos = Request.Files.Count;
                for (int i = 0; i < nbPhotos; i++)
                {
                    var photo = Request.Files[i];

                    string path = HttpContext.Server.MapPath("~/Content/Souvenirs/") + numCie + "/" + nomAlbum + "/" + photo.FileName;
                    photo.SaveAs(path);
                    string url = "/Content/Souvenirs/" + numCie + "/" + nomAlbum + "/" + photo.FileName;

                    /*byte[] image = new byte[photo.ContentLength];
                    photo.InputStream.Read(image, 0, image.Length);

                    string fichier = @"data:" + photo.ContentType + ";base64," + Convert.ToBase64String(image);*/

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

                return RedirectToAction("Album", new { id = idAlbum });
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur ajout d'un nouvel album -> " + e);
                return new HttpStatusCodeResult(500, "Erreur ajout d'un nouvel album");
            }
        }

        [Authorize]
        public ActionResult Album (Guid id)
        {
            try
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
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur affichage de la page de l'album : " + id + " -> " + e);
                return new HttpStatusCodeResult(500, "Erreur affichage de la page.");
            }
        }

        [Authorize]
        public ActionResult AfficherListePhotos(Guid id)
        {
            try
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
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur affichage de la liste des photos de l'album : " + id + " -> " + e);
                return new HttpStatusCodeResult(500, "Erreur affichage de la liste des photos.");
            }
        }

        [Authorize]
        public ActionResult AfficherListePhotosPourSuppression(Guid id)
        {
            try
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
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur affichage des la liste des photos pour suppression de l'album : " + id + " -> " + e);
                return new HttpStatusCodeResult(500, "Erreur affichage de la liste des photos pour suppression.");
            }
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
                    return new HttpUnauthorizedResult("Veuillez vous authentifier.");
                }
                if (u.Equals(typeof(UtilisateurNull)))
                {
                    FormsAuthentication.SignOut();
                    ViewBag.Erreur = ((UtilisateurNull)u).Error;
                    return new HttpUnauthorizedResult("Veuillez vous authentifier.");
                }
                if (u.PremiereCo)
                    return new HttpUnauthorizedResult("Ceci est votre première connexion.");

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
                LOGGER.Log("ERROR", "Erreur suppression des photos -> " + e);
                return new HttpStatusCodeResult(500, "Erreur de la suppression des photos.");
            }
        }

        [Authorize]
        public ActionResult AfficherPopUpSupprimerAlbum(Guid id)
        {
            try
            {
                Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
                if (u == null)
                {
                    FormsAuthentication.SignOut();
                    return new HttpUnauthorizedResult("Veuillez vous authentifier.");
                }
                if (u.Equals(typeof(UtilisateurNull)))
                {
                    FormsAuthentication.SignOut();
                    ViewBag.Erreur = ((UtilisateurNull)u).Error;
                    return new HttpUnauthorizedResult("Veuillez vous authentifier.");
                }
                if (u.PremiereCo)
                    return new HttpUnauthorizedResult("Ceci est votre première connexion.");

                if (u.Role > 3)
                    return new HttpUnauthorizedResult("Vous n'avez pas l'autorisation pour cette action.");

                if (u.Compagnie == -1)
                    return new HttpUnauthorizedResult("Vous n'êtes affecter à aucune compagnie.");

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
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur affichage de la pop-up de suppression d'album : " + id + " -> " + e);
                return new HttpStatusCodeResult(500, "Erreur affichage de la pop-up.");
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult SupprimerAlbum()
        {
            try
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
                if (idAlbum.Equals(Guid.Empty))
                {
                    string e = "Erreur lors de la suppression de l'album.";
                    return RedirectToAction("Index", "Souvenir", new { erreur = e });
                }

                Album album = aDal.GetAlbumById(idAlbum);
                if (album == null || album.Equals(typeof(AlbumNull)))
                {
                    string e = "Erreur lors de la suppression de l'album.";
                    return RedirectToAction("Index", "Souvenir", new { erreur = e });
                }

                int retour = aDal.SupprimerAlbum(idAlbum);

                string cheminAlbum = HttpContext.Server.MapPath("~/Content/Souvenirs/") + numCie + "/" + album.Dossier;
                if (retour == 1)
                {
                    Directory.Delete(cheminAlbum, true);
                    if (System.IO.File.Exists(cheminAlbum + ".zip"))
                        System.IO.File.Delete(cheminAlbum + ".zip");
                }
                else
                //if (retour != 1)
                {
                    string e = "Erreur lors de la suppression de l'album.";
                    return RedirectToAction("Index", "Souvenir", new { erreur = e });
                }

                return RedirectToAction("Index", "Souvenir");
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur suppression de l'album -> " + e);
                return new HttpStatusCodeResult(500, "Erreur suppression de l'album.");
            }
        }

        [Authorize]
        public ActionResult AfficherPopUpAjouterPhoto(Guid id)
        {
            try
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
                if (album == null || album.Equals(typeof(AlbumNull)))
                    album = new Album
                    {
                        Titre = "Empty",
                        Cie = 0,
                        Id = Guid.Empty
                    };

                return PartialView("AfficherPopUpAjouterPhoto", album);
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur affichage de la pop-up d'ajout de photos -> " + e);
                return new HttpStatusCodeResult(500, "Erreur affichage de la pop-up.");
            }
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
                    return new HttpUnauthorizedResult("Veuillez vous authentifier.");
                }
                if (u.Equals(typeof(UtilisateurNull)))
                {
                    FormsAuthentication.SignOut();
                    ViewBag.Erreur = ((UtilisateurNull)u).Error;
                    return new HttpUnauthorizedResult("Veuillez vous authentifier.");
                }
                if (u.PremiereCo)
                    return new HttpUnauthorizedResult("Ceci est votre première connexion.");

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

                    /*byte[] image = new byte[photo.ContentLength];
                    photo.InputStream.Read(image, 0, image.Length);

                    string fichier = @"data:" + photo.ContentType + ";base64," + Convert.ToBase64String(image);*/

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
                LOGGER.Log("ERROR", "Erreur ajout de photos -> " + e);
                return new HttpStatusCodeResult(500, "Erreur de l'ajout des photos.");
            }
        }

        [Authorize]
        public FileResult TelechargerAlbum(Guid id)
        {
            try
            {
                Album album = aDal.GetAlbumById(id);
                if (album == null || album.Equals(typeof(AlbumNull)))
                    album = new Album
                    {
                        Dossier = "",
                        Cie = 0
                    };

                string cheminDossier = HttpContext.Server.MapPath("~/Content/Souvenirs/") + album.Cie + "\\" + album.Dossier;
                string cheminArchive = HttpContext.Server.MapPath("~/Content/Souvenirs/") + album.Cie + "\\" + album.Dossier + ".zip";

                if (System.IO.File.Exists(cheminArchive))
                    System.IO.File.Delete(cheminArchive);

                ZipFile.CreateFromDirectory(cheminDossier, cheminArchive, CompressionLevel.Fastest, true);

                return File(cheminArchive, "application/zip", album.Dossier + ".zip");
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur création du zip pour le telechargement de l'album : " + id + " -> " + e);
                return null;
            }
        }
    }
}