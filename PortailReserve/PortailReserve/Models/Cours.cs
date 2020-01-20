using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Cours
    {
        public long Id { get; set; }
        public String Nom { get; set; }
        public String Description { get; set; }
        public String Fichier { get; set; }
        public String Theme { get; set; }
        public DateTime Publication { get; set; }
        public DateTime Modification { get; set; }
    }
}