using PortailReserve.DAL;
using PortailReserve.DAL.Impl;
using PortailReserve.Models;
using PortailReserve.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortailReserve.Controllers
{
    public class DisponibiliteController : Controller
    {
        private IDisponibiliteDal dDal;

        public DisponibiliteController()
        {
            dDal = new DisponibiliteDal();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Ajouter(EventViewModel vm)
        {
            bool dispo = Request.Form["disponibilite"].Equals("Oui") ? true : false;

            Disponibilite toAdd = new Disponibilite()
            {
                Evenement = vm.Event.Id,
                Utilisateur = vm.Util.Id,
                Disponible = dispo 
            };

            if(dispo)
            {
                toAdd.TouteLaPeriode = vm.Dispo.TouteLaPeriode;
                if(vm.Dispo.TouteLaPeriode)
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
            if (retour.Equals(Guid.Empty))
                ViewBag.Erreur = "Une erreure est survenue. Réessayer plus tard.";

            return RedirectToAction("Evenement", "Planning", new { id = vm.Event.Id });
        }

        [Authorize]
        [HttpPost]
        public ActionResult Supprimer (EventViewModel vm)
        {
            Guid idDispo = new Guid(Request.Form["toSupp"]);

            dDal.SupprimerDispo(idDispo);

            return RedirectToAction("Evenement", "Planning", new { id = vm.Event.Id });
        }

        [Authorize]
        [HttpPost]
        public ActionResult Modifier (EventViewModel vm)
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

            dDal.ModifierDispo(vm.Dispo.Id, newDispo);

            return RedirectToAction("Evenement", "Planning", new { id = vm.Event.Id });
        }
    }
}