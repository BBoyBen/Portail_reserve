using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.ViewModel
{
    public class EventViewModel
    {
        public Evenement Event { get; set; }
        public Utilisateur Util { get; set; }
        public Effectif Effectif { get; set; }
        public Disponibilite Dispo { get; set; }
        public Participation Participation { get; set; }
        public List<Utilisateur> UtilParticipation { get; set; }
        public List<Utilisateur> UtilDispo { get; set; }
        public List<Utilisateur> NoReponseP { get; set; }
        public List<Utilisateur> NoReponseD { get; set; }
    }
}