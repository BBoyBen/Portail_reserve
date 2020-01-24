using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Reponse
    {
        public Guid Id { get; set; }
        public Message Message { get; set; }
        public string Texte { get; set; }
        public DateTime Date { get; set; }
        public string Prenom { get; set; }
        public string Nom { get; set; }
    }
}