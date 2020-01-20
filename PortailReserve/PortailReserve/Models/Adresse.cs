using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Adresse
    {
        public long Id { get; set; }
        public String Voie { get; set; }
        public String CodePostal { get; set; }
        public String Ville { get; set; }
        public String Pays { get; set; }
    }
}