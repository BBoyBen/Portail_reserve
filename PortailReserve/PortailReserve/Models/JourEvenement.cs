using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class JourEvenement
    {
        public DateTime Jour { get; set; }
        public Evenement Evenement { get; set; }
        public bool CarreBlanc { get; set; }
        public List<Guid> AutresEvents { get; set; }
        public string Type { get; set; }
    }
}