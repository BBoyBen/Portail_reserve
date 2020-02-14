using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models.CSV
{
    public class EvenementCsv
    {
        public string Nom { get; set; }
        public string Description { get; set; }
        public string Debut { get; set; }
        public string Fin { get; set; }
        public string LimiteReponse { get; set; }
        public string Patracdr { get; set; }
        public string Type { get; set; }
        public string Lieu { get; set; }
        public string Officier { get; set; }
        public string SousOfficier { get; set; }
        public string Militaire { get; set; }
    }
}