using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Disponibilite
    {
        public Guid Id { get; set; }
        public Evenement Evenement { get; set; }
        public Utilisateur Utilisateur { get; set; }
        public bool TouteLaPeriode { get; set; }
        public DateTime Debut { get; set; }
        public DateTime Fin { get; set; }
        public bool Disponible { get; set; }
        public bool Valide { get; set; }
        public DateTime Reponse { get; set; }
    }
}