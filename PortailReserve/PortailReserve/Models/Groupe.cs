using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Groupe
    {
        [Key]
        public Guid Id { get; set; }
        public Section Section { get; set; }
        public int Numero { get; set; }
        public Guid  CDG { get; set; }
    }
}