using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Evenement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Nom { get; set; }
        public string Description { get; set; }
        public DateTime Debut { get; set; }
        public DateTime Fin { get; set; }
        public DateTime LimiteReponse { get; set; }
        public string Patracdr { get; set; }
        public TimeSpan Duree { get; set; }
        /***
         * Types possible :
         * Instruction
         * Stage
         * Mission
         * Exercice
         ***/
        public string Type { get; set; }
        public string Lieu { get; set; }
        public Guid Effectif { get; set; }
    }
}