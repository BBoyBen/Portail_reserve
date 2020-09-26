using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.ViewModel
{
    public class ContactViewModel
    {
        public Utilisateur CDG { get; set; }
        public Utilisateur SOA { get; set; }
        public Utilisateur CDS { get; set; }
        public Utilisateur CDU { get; set; }
    }
}