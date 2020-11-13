using PortailReserve.DAL;
using PortailReserve.DAL.Impl;
using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using PortailReserve.Utils;
using PortailReserve.ViewModel;
using System;
using System.Web.Mvc;
using System.Web.Security;

namespace PortailReserve.Controllers
{
    public class DisponibiliteController : Controller
    {
        private IDisponibiliteDal dDal;
        private IUtilisateurDal uDal;
        private readonly Logger LOGGER;

        public DisponibiliteController()
        {
            dDal = new DisponibiliteDal();
            uDal = new UtilisateurDal();
            LOGGER = new Logger(this.GetType());
        }

        [Authorize]
        [HttpPost]
        public ActionResult Ajouter(EventViewModel vm)
        {
            try
            {
                bool dispo = Request.Form["disponibilite"].Equals("Oui") ? true : false;

                Disponibilite toAdd = new Disponibilite()
                {
                    Evenement = vm.Event.Id,
                    Utilisateur = vm.Util.Id,
                    Disponible = dispo
                };

                if (dispo)
                {
                    toAdd.TouteLaPeriode = vm.Dispo.TouteLaPeriode;
                    if (vm.Dispo.TouteLaPeriode)
                    {
                        toAdd.Debut = vm.Event.Debut;
                        toAdd.Fin = vm.Event.Fin;
                    }
                    else
                    {
                        toAdd.Debut = vm.Dispo.Debut;
                        toAdd.Fin = vm.Dispo.Fin;
                    }
                }
                else
                {
                    toAdd.TouteLaPeriode = false;
                    toAdd.Debut = vm.Event.Debut;
                    toAdd.Fin = vm.Event.Fin;
                }

                Guid retour = dDal.AjouterDispo(toAdd);
                string er = "";
                if (retour.Equals(Guid.Empty))
                {
                    LOGGER.Log("ERROR", "Erreur ajout de la dispo.");
                    er = "Une erreure est survenue. Réessayer plus tard.";
                }

                return RedirectToAction("AfficherBoutonEtListeDispo", "Planning", new { id = vm.Event.Id, erreur = er });
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur lors de l'ajout d'une dispo -> " + e);
                return new HttpStatusCodeResult(500, "Exception lors de l'ajout d'une dispo -> " + e.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Supprimer (EventViewModel vm)
        {
            try
            {
                Guid idDispo = new Guid(Request.Form["toSupp"]);
                if (idDispo.Equals(Guid.Empty))
                {
                    LOGGER.Log("ERROR", "Echec récupération de l'id de la dispo pour suppression.");
                    return new HttpStatusCodeResult(400, "Echec récupération de l'id de la dispo pour suppression.");
                }

                int retour = dDal.SupprimerDispo(idDispo);
                if(retour != 1)
                {
                    LOGGER.Log("ERROR", "Erreur lors de la suppression de la dispo : " + idDispo);
                    return new HttpStatusCodeResult(400, "Echec suppression de la dispo.");
                }

                return RedirectToAction("AfficherBoutonEtListeDispo", "Planning", new { id = vm.Event.Id });
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur lors de la suppression d'une dispo -> " + e);
                return new HttpStatusCodeResult(500, "Exception lors de la suppression de la dispo -> " + e.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Modifier (EventViewModel vm)
        {
            try
            {
                Disponibilite newDispo = new Disponibilite();
                newDispo.TouteLaPeriode = vm.Dispo.TouteLaPeriode;
                newDispo.Disponible = true;

                if (newDispo.TouteLaPeriode)
                {
                    newDispo.Debut = vm.Event.Debut;
                    newDispo.Fin = vm.Event.Fin;
                }
                else
                {
                    newDispo.Debut = vm.Dispo.Debut;
                    newDispo.Fin = vm.Dispo.Fin;
                }

                int retour = dDal.ModifierDispo(vm.Dispo.Id, newDispo);
                if (retour != 1)
                {
                    LOGGER.Log("ERROR", "Erreur modification de la dispo.");
                    ViewBag.Erreur = "Une erreur s'est produite lors de la mise à jour de la disponibilité.";
                }

                return RedirectToAction("AfficherBoutonEtListeDispo", "Planning", new { id = vm.Event.Id });
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur lors de la modification de la dispo -> " + e);
                return new HttpStatusCodeResult(500, "Exception lors de la modification de la dispo -> " + e.Message);
            }
        }

        [Authorize]
        public ActionResult Valider(Guid id, Guid ev)
        {
            try
            {
                Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
                if (u == null)
                {
                    FormsAuthentication.SignOut();
                    return new HttpStatusCodeResult(401, "Veuillez vous authentifier.");
                }
                if (u.Equals(typeof(UtilisateurNull)))
                {
                    FormsAuthentication.SignOut();
                    ViewBag.Erreur = ((UtilisateurNull)u).Error;
                    return new HttpStatusCodeResult(401, "Veuillez vous authentifier.");
                }
                if (u.PremiereCo)
                    return new HttpStatusCodeResult(401, "Ceci est votre première connexion.");

                ViewBag.Grade = u.Grade;
                ViewBag.Nom = u.Nom.ToUpperInvariant();
                ViewBag.Role = u.Role;

                if (u.Role > 3)
                    return RedirectToAction("AfficherBoutonEtListeDispo", "Planning", new { id = ev });

                int retour = dDal.ValiderDispo(id);
                string er = "";
                if (retour != 1)
                {
                    LOGGER.Log("ERROR", "Erreu de la validation de la dispo : " + id);
                    er = "Une erreur est survenue. Veuillez réessayer plus tard.";
                }

                return RedirectToAction("AfficherBoutonEtListeDispo", "Planning", new { id = ev, erreur = er });
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur lors de la validation d'une dispo -> " + e);
                return new HttpStatusCodeResult(500, "Exception validation dispo -> " + e.Message);
            }
        }

        [Authorize]
        public ActionResult Refuser(Guid id, Guid ev)
        {
            try
            {
                Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
                if (u == null)
                {
                    FormsAuthentication.SignOut();
                    return new HttpStatusCodeResult(401, "Veuillez vous authentifier.");
                }
                if (u.Equals(typeof(UtilisateurNull)))
                {
                    FormsAuthentication.SignOut();
                    ViewBag.Erreur = ((UtilisateurNull)u).Error;
                    return new HttpStatusCodeResult(401, "Veuillez vous authentifier.");
                }
                if (u.PremiereCo)
                    return new HttpStatusCodeResult(401, "Ceci est votre première connexion.");

                ViewBag.Grade = u.Grade;
                ViewBag.Nom = u.Nom.ToUpperInvariant();
                ViewBag.Role = u.Role;

                if (u.Role > 3)
                    return RedirectToAction("AfficherBoutonEtListeDispo", "Planning", new { id = ev });

                int retour = dDal.RefuserDispo(id);
                if (retour != 1)
                {
                    LOGGER.Log("ERROR", "Erreur lors du refeus de la dispo : " + id);
                    ViewBag.Erreur = "Une erreur est survenue. Veuillez réessayer plus tard.";
                }

                return RedirectToAction("AfficherBoutonEtListeDispo", "Planning", new { id = ev });
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur du refus de la dispo : " + id + " -> " + e);
                return new HttpStatusCodeResult(500, "Exception refus de la dispo : " + id + " -> " + e.Message);
            }
        }
    }
}