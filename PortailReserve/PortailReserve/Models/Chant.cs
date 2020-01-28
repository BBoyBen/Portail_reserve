using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Chant
    {
        public Guid Id { get; set; }
        public string Titre { get; set; }
        public string Texte { get; set; }
        public int Type { get; set; }
        public DateTime Publication { get; set; }
        public DateTime Modification { get; set; }
    }
}