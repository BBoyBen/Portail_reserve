using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortailReserve.ViewModel
{
    public class AjoutCoursViewModel
    {
        public Cours Cours { get; set; }
        public List<SelectListItem> Themes { get; set; }
    }
}