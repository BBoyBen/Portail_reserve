using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class UtilisateurParticipation
    {
        public Utilisateur Util { get; set; }
        public Participation Participation { get; set; }
    }
}