using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.ViewModel
{
    public class ListeEventViewModel
    {
        public List<Evenement> AVenir { get; set; }
        public List<Evenement> Passe { get; set; }
    }
}