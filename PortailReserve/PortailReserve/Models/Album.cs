using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Album
    {
        [Key]
        public Guid Id { get; set; }
        public string Titre { get; set; }
        public string Description { get; set; }
        public DateTime Creation { get; set; }
    }
}