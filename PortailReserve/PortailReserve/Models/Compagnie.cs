using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Compagnie
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public int Numero { get; set; }
        public Guid CDU { get; set; }
        public Guid ADU { get; set; }
        public string Devise { get; set; }
        public string Chant { get; set; }
    }
}