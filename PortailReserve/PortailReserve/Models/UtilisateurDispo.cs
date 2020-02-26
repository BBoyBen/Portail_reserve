using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class UtilisateurDispo
    {
        public Utilisateur Util { get; set; }
        public List<Disponibilite> Dispos { get; set; }
    }
}