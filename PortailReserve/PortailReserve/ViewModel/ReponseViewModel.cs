using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.ViewModel
{
    public class ReponseViewModel
    {
        public Utilisateur Util { get; set; }
        public List<ReponseUtil> Reponse { get; set; }
        public Guid Message { get; set; }
    }
}