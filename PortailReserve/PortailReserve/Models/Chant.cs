using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Chant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Titre { get; set; }
        public string Texte { get; set; }
        public int Type { get; set; }
        public DateTime Publication { get; set; }
        public DateTime Modification { get; set; }
    }
}