using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Section
    {
        public Guid Id { get; set; }
        public int Numero { get; set; }
        public Utilisateur CDS { get; set; }
        public Utilisateur SOA { get; set; }
        public string Chant { get; set; }
        public string Devise { get; set; }
    }
}