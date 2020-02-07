using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Disponibilite
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid Evenement { get; set; }
        public Guid Utilisateur { get; set; }
        public bool TouteLaPeriode { get; set; }
        public DateTime Debut { get; set; }
        public DateTime Fin { get; set; }
        public bool Disponible { get; set; }
        public int Valide { get; set; }
        public DateTime Reponse { get; set; }
    }
}