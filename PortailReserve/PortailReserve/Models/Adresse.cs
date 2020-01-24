using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Adresse
    {
        public Guid Id { get; set; }
        public string Voie { get; set; }
        public string CodePostal { get; set; }
        public string Ville { get; set; }
        public string Pays { get; set; }
    }
}