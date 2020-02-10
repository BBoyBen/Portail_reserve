using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.ViewModel
{
    public class SectionViewModel
    {
        public Compagnie Cie { get; set; }
        public Section Section { get; set; }
        public Utilisateur CDS { get; set; }
        public Utilisateur SOA { get; set; }
        public List<Groupe> Groupes { get; set; }
        public List<Utilisateur> CDGs { get; set; }
        public List<Utilisateur> Soldats { get; set; }
    }
}