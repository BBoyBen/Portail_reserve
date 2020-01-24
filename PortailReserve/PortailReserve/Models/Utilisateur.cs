using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Utilisateur
    {
        public Guid Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public Adresse Adresse { get; set; }
        [Display(Name = "Matricule : ")]
        public string Matricule { get; set; }
        [Display(Name = "Mot de passe : ")]
        public string MotDePasse { get; set; }
        public Groupe Groupe { get; set; }
        public int Role { get; set; }
        public bool PremiereCo { get; set; }
        public string Grade { get; set; }
        public DateTime Naissance { get; set; }
    }
}