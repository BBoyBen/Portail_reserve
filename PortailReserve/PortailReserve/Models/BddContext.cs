using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class BddContext : DbContext
    {
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Adresse> Adresses { get; set; }
        public DbSet<Groupe> Groupes { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Reponse> Reponses { get; set; }
        public DbSet<Evenement> Evenements { get; set; }
        public DbSet<Effectif> Effectifs { get; set; }
        public DbSet<Participation> Participations { get; set; }
        public DbSet<Disponibilite> Disponibilites { get; set; }
        public DbSet<Cours> Cours { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Compagnie> Compagnies { get; set; }
        public DbSet<Chant> Chants { get; set; }
        public DbSet<Lecture> Lectures { get; set; }

     
    }
}