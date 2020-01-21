using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Message
    {
        public long Id { get; set; }
        public long Id_Utilisateur { get; set; }
        public long Id_Evenement { get; set; }
        public String Texte { get; set; }
        public DateTime Date { get; set; }
    }
}