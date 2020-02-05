using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.ViewModel
{
    public class ProfilViewModel
    {
        public Utilisateur Util { get; set; }
        public Adresse Adr { get; set; }
        public Groupe Grp { get; set; }
        public Section Section { get; set; }
        public Compagnie Cie { get; set; }
        public Utilisateur Cdg { get; set; }
        public Utilisateur Cds { get; set; }
        public Utilisateur Soa { get; set; }
        public Utilisateur Cdu { get; set; }
    }
}