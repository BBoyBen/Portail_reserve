using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.ViewModel
{
    public class EventPlanningViewModel
    {
        public Evenement Event { get; set; }
        public Effectif Effectif { get; set; }
        public DateTime Jour { get; set; }
        public List<EventEffectif> AutresEvents { get; set; }
    }
}