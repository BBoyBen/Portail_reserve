using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortailReserve.ViewModel
{
    public class AjoutSectionViewModel
    {
        public Guid Cie { get; set; }
        public int NumCie { get; set; }
        public int NumSection { get; set; }
        public List<SelectListItem> Grades { get; set; }
        public List<SelectListItem> SansSection { get; set; }
        public string MotDePasse { get; set; }
    }
}