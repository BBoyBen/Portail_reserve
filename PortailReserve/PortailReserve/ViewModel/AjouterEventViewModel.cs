using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortailReserve.ViewModel
{
    public class AjouterEventViewModel
    {
        public Evenement Event { get; set; }
        public List<SelectListItem> Types { get; set; }
        public Effectif Effectif { get; set; }
    }
}