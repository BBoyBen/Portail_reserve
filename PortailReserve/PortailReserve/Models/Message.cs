using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public Evenement Evenement { get; set; }
        public string Texte { get; set; }
        public DateTime Date { get; set; }
        public string Nom { get; set; }
        public String Prenom { get; set; }
    }
}