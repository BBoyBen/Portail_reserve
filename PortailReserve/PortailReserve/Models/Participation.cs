using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Participation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid Evenement { get; set; }
        public Guid Utilisateur { get; set; }
        public bool Participe { get; set; }
        public DateTime Reponse { get; set; }
        public DateTime Modification { get; set; }
    }
}