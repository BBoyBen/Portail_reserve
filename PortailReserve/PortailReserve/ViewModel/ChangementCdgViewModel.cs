using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortailReserve.ViewModel
{
    public class ChangementCdgViewModel
    {
        public Guid AncienCdg { get; set; }
        public Groupe Groupe { get; set; }
        public Section Section { get; set; }
        public List<SelectListItem> Grades { get; set; }
        public List<SelectListItem> Roles { get; set; }
        public List<SelectListItem> SansSection { get; set; }
        public string MotDePasse { get; set; }
    }
}