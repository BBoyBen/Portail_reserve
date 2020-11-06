using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Cours
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Display(Name = "Nom du cours")]
        public string Nom { get; set; }
        [Display(Name = "Description")]
        public string Description { get; set; }
        public string Fichier { get; set; }
        [Display(Name = "Thématique")]
        public string Theme { get; set; }
        public string Extension { get; set; }
        public DateTime Publication { get; set; }
        public DateTime Modification { get; set; }
    }
}