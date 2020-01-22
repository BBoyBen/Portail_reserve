using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Photo
    {
        public long Id { get; set; }
        public long Id_Album { get; set; }
        public string Fichier { get; set; }
        public DateTime Ajout { get; set; }
    }
}