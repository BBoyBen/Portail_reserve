using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Section
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public int Numero { get; set; }
        public Guid CDS { get; set; }
        public Guid SOA { get; set; }
        public string Chant { get; set; }
        public string Devise { get; set; }
        public Guid Compagnie { get; set; }
        public int NumCie { get; set; }
    }
}