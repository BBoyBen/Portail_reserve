using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class EventEffectif
    {
        public Evenement Event { get; set; }
        public Effectif Effectif { get; set; }
    }
}