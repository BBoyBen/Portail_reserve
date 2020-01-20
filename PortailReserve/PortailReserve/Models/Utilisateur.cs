using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Utilisateur
    {
        public long Id { get; set; }
        public String Nom { get; set; }
        public String Prenom { get; set; }
        public String Telephone { get; set; }
        public String Email { get; set; }
        public long Id_Adresse { get; set; }
        public String Matricule { get; set; }
        public String MotDePasse { get; set; }
        public long Id_Groupe { get; set; }
        public int Role { get; set; }
    }
}