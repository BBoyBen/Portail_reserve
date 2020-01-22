using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Utilisateur
    {
        public long Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public long Id_Adresse { get; set; }
        public string Matricule { get; set; }
        public string MotDePasse { get; set; }
        public long Id_Groupe { get; set; }
        public int Role { get; set; }
        public bool PremiereCo { get; set; }
    }
}