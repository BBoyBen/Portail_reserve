using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.ViewModel
{
    public class PremiereCoViewModel
    {
        public Utilisateur Util { get; set; }
        public Adresse Adr { get; set; }
        public string Mdp { get; set; }
        public string MdpBis { get; set; }
    }
}