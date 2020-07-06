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
        public List<Disponibilite> AllDispo { get; set; }
        public Participation Participation { get; set; }
        public List<UtilisateurParticipation> UtilParticipation { get; set; }
        public List<UtilisateurDispo> UtilDispo { get; set; }
        public List<UtilisateurParticipation> UtilNonParticipation { get; set; }
        public List<UtilisateurDispo> UtilNonDispo { get; set; }
        public List<Utilisateur> NoReponseP { get; set; }
        public List<Utilisateur> NoReponseD { get; set; }
        public bool ADispo { get; set; }
        public int NonLu { get; set; }
    }
}