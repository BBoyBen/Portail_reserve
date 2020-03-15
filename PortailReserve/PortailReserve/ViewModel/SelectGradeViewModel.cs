using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortailReserve.ViewModel
{
    public class SelectGradeViewModel
    {
        public Utilisateur Util { get; set; }
        public List<SelectListItem> Grades { get; set; }
    }
}