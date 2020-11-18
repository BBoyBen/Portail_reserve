using PortailReserve.DAL.Impl;
using PortailReserve.DAL;
using PortailReserve.ViewModel;
using System;
using System.Web.Mvc;
using PortailReserve.Utils;

namespace PortailReserve.Controllers
{
    public class ParticipationController : Controller
    {
        private IParticipationDal pDal;
        private readonly Logger LOGGER;

        public ParticipationController()
        {
            pDal = new ParticipationDal();
            LOGGER = new Logger(this.GetType());
        }

        [Authorize]
        [HttpPost]
        public ActionResult Ajouter(EventViewModel vm)
        {
            try
            {
                bool participe = Request.Form["participation"].Equals("Oui") ? true : false;

                Guid created = pDal.AjouterParticipation(vm.Util.Id, vm.Event.Id, participe);
                string er = "";
                if (created.Equals(Guid.Empty))
                    er = "Une erreure c'est produite. Veuillez réessayer.";

                return RedirectToAction("AfficherBoutonEtListeDispo", "Planning", new { id = vm.Event.Id, erreur = er });
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur ajout de nouvelle participation -> " + e);
                return new HttpStatusCodeResult(500, "Exception ajout de participation -> " + e.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Modifier(EventViewModel vm)
        {
            try
            {
                bool modif = Request.Form["modifParticipation"].Equals("Oui") ? true : false;

                int retour = pDal.ModifierParticipation(vm.Event.Id, modif, vm.Util.Id);
                string er = "";
                if (retour != 1)
                    er = "Une erreure c'est produite. Veuillez réessayer.";

                return RedirectToAction("AfficherBoutonEtListeDispo", "Planning", new { id = vm.Event.Id, erreur = er });
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur modification d'une participation -> " + e);
                return new HttpStatusCodeResult(500, "Exception modification de participation -> " + e.Message);
            }
        }
    }
}