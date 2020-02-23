using PortailReserve.DAL.Impl;
using PortailReserve.DAL;
using PortailReserve.ViewModel;
using System;
using System.Web.Mvc;

namespace PortailReserve.Controllers
{
    public class ParticipationController : Controller
    {
        private IParticipationDal pDal;

        public ParticipationController()
        {
            pDal = new ParticipationDal();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Ajouter(EventViewModel vm)
        {
            bool participe = Request.Form["participation"].Equals("Oui") ? true : false;

            Guid created = pDal.AjouterParticipation(vm.Util.Id, vm.Event.Id, participe);
            if (created.Equals(Guid.Empty))
                ViewBag.Erreur = "Une erreure c'est produite. Veuillez réessayer.";

            return RedirectToAction("Evenement", "Planning", new { id = vm.Event.Id });
        }

        [Authorize]
        [HttpPost]
        public ActionResult Modifier(EventViewModel vm)
        {
            bool modif = Request.Form["modifParticipation"].Equals("Oui") ? true : false;

            int retour = pDal.ModifierParticipation(vm.Event.Id, modif, vm.Util.Id);
            if(retour != 1)
                ViewBag.Erreur = "Une erreure c'est produite. Veuillez réessayer.";

            return RedirectToAction("Evenement", "Planning", new { id = vm.Event.Id });
        }
    }
}