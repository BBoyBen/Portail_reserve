using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Reponse
    {
        public long Id { get; set; }
        public long Id_Message { get; set; }
        public String Texte { get; set; }
        public DateTime Date { get; set; }
    }
}