using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortailReserve.ViewModel
{
    public class AjoutModifChantViewModel
    {
        public Chant Chant { get; set; }
        public List<SelectListItem> Types { get; set; }
    }
}