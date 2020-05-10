using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.ViewModel
{
    public class SectionCommandementViewModel
    {
        public Utilisateur Util { get; set; }
        public Utilisateur CDU { get; set; }
        public Utilisateur ADU { get; set; }
        public List<Utilisateur> UtilCmdt { get; set; }
        public Section Section { get; set; }
        public Compagnie Cie { get; set; }
    }
}