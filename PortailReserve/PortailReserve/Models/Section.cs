using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Section
    {
        [Key]
        public Guid Id { get; set; }
        public int Numero { get; set; }
        public Utilisateur CDS { get; set; }
        public Utilisateur SOA { get; set; }
        public string Chant { get; set; }
        public string Devise { get; set; }
        public Compagnie Compagnie { get; set; }
    }
}