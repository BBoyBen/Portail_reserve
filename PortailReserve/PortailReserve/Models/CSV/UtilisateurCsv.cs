using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models.CSV
{
    public class UtilisateurCsv
    {
        [Name("Nom")]
        public string Nom { get; set; }
        [Name("Prenom")]
        public string Prenom { get; set; }
        [Name("Telephone")]
        public string Telephone { get; set; }
        [Name("Email")]
        public string Email { get; set; }
        [Name("Matricule")]
        public string Matricule { get; set; }
        [Name("Motdepasse")]
        public string Motdepasse { get; set; }
        [Name("Groupe")]
        public string Groupe { get; set; }
        [Name("Role")]
        public string Role { get; set; }
        [Name("Grade")]
        public string Grade { get; set; }
        [Name("Naissance")]
        public string Naissance { get; set; }
        [Name("Voie")]
        public string Voie { get; set; }
        [Name("Ville")]
        public string Ville { get; set; }
        [Name("CodePostal")]
        public string CodePostal { get; set; }
        [Name("Pays")]
        public string Pays { get; set; }
        [Name("Section")]
        public string Section { get; set; }
        [Name("Compagnie")]
        public string Compagnie { get; set; }
    }
}