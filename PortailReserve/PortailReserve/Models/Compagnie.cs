using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Compagnie
    {
        public Guid Id { get; set; }
        public int Numero { get; set; }
        public Utilisateur CDU { get; set; }
        public Utilisateur ADU { get; set; }
        public string Devise { get; set; }
        public string Chant { get; set; }
    }
}