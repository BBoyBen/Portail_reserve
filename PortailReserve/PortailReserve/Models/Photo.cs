using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Photo
    {
        public Guid Id { get; set; }
        public Album Album { get; set; }
        public string Fichier { get; set; }
        public DateTime Ajout { get; set; }
    }
}