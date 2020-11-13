using PortailReserve.DAL;
using PortailReserve.DAL.Impl;
using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using PortailReserve.Utils;
using PortailReserve.ViewModel;
using System;
using System.Web.Mvc;
using System.Web.Security;
using static PortailReserve.Utils.Utils;

namespace PortailReserve.Controllers
{
    public class LoginController : Controller
    {
        private IUtilisateurDal uDal;
        private IAdresseDal aDal;
        private readonly Logger LOGGER;

        public LoginController()
        {
            uDal = new UtilisateurDal();
            aDal = new AdresseDal();
            LOGGER = new Logger(this.GetType());
        }
        
        public ActionResult Index()
        {
            try
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                    return RedirectToAction("Index", "Home", null);

                return View();
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur chargement de la page de connexion -> " + e);
                return new HttpStatusCodeResult(500, "Echec chargement de la page -> " + e.Message);
            }
        }

        [HttpPost]
        public ActionResult Index(Utilisateur utilisateur)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!ValideMatricule(utilisateur.Matricule))
                    {
                        ModelState.AddModelError("MotDePasse", "Matricule ou mot de passe incorrect.");
                        return View(utilisateur);
                    }

                    Utilisateur u = uDal.Authentifier(utilisateur.Matricule, utilisateur.MotDePasse);

                    if (u != null && !u.Equals(typeof(UtilisateurNull)))
                    {
                        FormsAuthentication.SetAuthCookie(u.Id.ToString(), false);

                        if (u.PremiereCo)
                            return RedirectToAction("PremiereCo");

                        return Redirect("/");
                    }
                    ModelState.AddModelError("MotDePasse", "Matricule ou mot de passe incorrect.");
                }
                ModelState.AddModelError("MotDePasse", "Matricule ou mot de passe incorrect.");

                return View(utilisateur);
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur lors de l'authentifaction -> " + e);
                return new HttpStatusCodeResult(500, "Exception lors de l'authentification -> " + e.Message);
            }
        }

        [Authorize]
        public ActionResult PremiereCo ()
        {
            try
            {
                Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
                if (u == null)
                {
                    FormsAuthentication.SignOut();
                    return RedirectToAction("Index");
                }
                if (u.Equals(typeof(UtilisateurNull)))
                {
                    FormsAuthentication.SignOut();
                    ViewBag.Erreur = ((UtilisateurNull)u).Error;
                    return RedirectToAction("Index");
                }
                if (!u.PremiereCo)
                    return RedirectToAction("Index", "Home");

                PremiereCoViewModel vm = new PremiereCoViewModel()
                {
                    Util = u,
                    Adr = aDal.GetAdresseById(u.Adresse),
                    Mdp = "",
                    MdpBis = ""
                };

                return View(vm);
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur du chargement de la page de premiere connexion -> " + e);
                return new HttpStatusCodeResult(500, "Echec chargement de la page -> " + e.Message);
            }
        }

        [HttpPost]
        public ActionResult PremiereCo (PremiereCoViewModel vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    bool allValide = true;
                    if (!ValideMotDePasse(vm.Mdp, vm.MdpBis))
                    {
                        ModelState.AddModelError("Mdp", "Mot de passe invalide ou pas identique.");
                        allValide = false;
                    }
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
                    {
                        LOGGER.Log("ERROR", "Echec validation des champs première co.");
                        return View(vm);
                    }

                    int erreur = 0;
                    erreur = aDal.ModifierAdresse(vm.Util.Adresse, vm.Adr);
                    if (erreur != 1)
                    {
                        LOGGER.Log("ERROR", "Erreur modification adresse : " + vm.Util.Adresse + " pour la premiere co.");
                        ViewBag.Erreur = "Une erreur s'est produite avec votre adresse.";
                        return RedirectToAction("PremiereCo", "Login");
                    }

                    erreur = uDal.ModifierUtilisateur(vm.Util.Id, vm.Util);
                    if (erreur != 1)
                    {
                        LOGGER.Log("ERROR", "Erreur modification utilisateur : " + vm.Util.Id + " pour la premiere co.");
                        ViewBag.Erreur = "Une erreur s'est produite avec vos informations.";
                        return RedirectToAction("PremiereCo", "Login");
                    }

                    erreur = uDal.PremierChangementMotDePasse(vm.Util.Id, vm.Mdp, vm.MdpBis);
                    if (erreur != 1)
                    {
                        LOGGER.Log("ERROR", "Erreur premier changement mot de passe pour l'utilisateur : " + vm.Util.Id);
                        ViewBag.Erreur = "Une erreur s'est produite pour le changement de mot de passe.";
                        return RedirectToAction("PremiereCo", "Login");
                    }

                    erreur = uDal.PremiereCoOk(vm.Util.Id);
                    if (erreur != 1)
                    {
                        LOGGER.Log("ERROR", "Erreur changement du paramètre de la première connexion pour l'utilisateur : " + vm.Util.Id);
                        ViewBag.Erreur = "Une erreur s'est produite.";
                        return RedirectToAction("PremiereCo", "Login");
                    }

                    return RedirectToAction("Index", "Home");
                }
                return View(vm);
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur lors de la première connexion -> " + e);
                return new HttpStatusCodeResult(500, "Erreur première connexion -> " + e);
            }
        }

        public ActionResult Deconnexion ()
        {
            try
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur lors de la deconnexion -> " + e);
                return new HttpStatusCodeResult(500, "Exception lors de la deconnexion -> " + e.Message);
            }

        }
    }
}