using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Participation
    {
        public long Id { get; set; }
        public long Id_Evenement { get; set; }
        public long Id_Utilisateur { get; set; }
        public bool Participe { get; set; }
        public DateTime Reponse { get; set; }
    }
}