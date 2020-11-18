using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Utilisateur
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Nom { get; set; }
        [Display(Name = "Prénom")]
        public string Prenom { get; set; }
        [Display(Name = "Téléphone")]
        public string Telephone { get; set; }
        [Display(Name = "E-mail")]
        public string Email { get; set; }
        public Guid Adresse { get; set; }
        [Display(Name = "Matricule")]
        public string Matricule { get; set; }
        [Display(Name = "Mot de passe")]
        public string MotDePasse { get; set; }
        public Guid Groupe { get; set; }
        /***
         * 1 : super admin
         * 2 : section de commandement
         * 3 : gestion de section
         * 4 : personnel classique
         ***/
        public int Role { get; set; } 
        public bool PremiereCo { get; set; }
        public string Grade { get; set; }
        [Display(Name = "Date de naissance")]
        public DateTime Naissance { get; set; }
        public int Section { get; set; }
        public int Compagnie { get; set; }
    }
}