using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.ViewModel
{
    public class TableauPlanningViewModel
    {
        public List<JourEvenement> Lundi {get; set;}
        public List<JourEvenement> Mardi { get; set; }
        public List<JourEvenement> Mercredi { get; set; }
        public List<JourEvenement> Jeudi { get; set; }
        public List<JourEvenement> Vendredi { get; set; }
        public List<JourEvenement> Samedi { get; set; }
        public List<JourEvenement> Dimanche { get; set; }
        public List<string> Mois { get; set; }
        public Guid IdEventDuJour { get; set; }
        public List<Guid> IdEvents { get; set; }
    }
}