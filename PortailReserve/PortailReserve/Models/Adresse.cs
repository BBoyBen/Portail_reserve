using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Adresse
    {
        [Key]
        public Guid Id { get; set; }
        [Display(Name = "Numéros, nom de rue")]
        public string Voie { get; set; }
        [Display(Name = "Code postal")]
        public string CodePostal { get; set; }
        public string Ville { get; set; }
        public string Pays { get; set; }
    }
}