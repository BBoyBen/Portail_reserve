using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Effectif
    {
        public Guid Id { get; set; }
        public int Officier { get; set; }
        public int SousOfficier { get; set; }
        public int Militaire { get; set; }
    }
}