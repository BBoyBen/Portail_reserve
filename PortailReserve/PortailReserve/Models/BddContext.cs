using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class BddContext : DbContext
    {
        DbSet<Utilisateur> Utilisateurs { get; set; }
        DbSet<Adresse> Adresses { get; set; }
        DbSet<Groupe> Groupes { get; set; }
        DbSet<Section> Sections { get; set; }
        DbSet<Message> Messages { get; set; }
        DbSet<Reponse> Reponses { get; set; }
        DbSet<Evenement> Evenements { get; set; }
        DbSet<Effectif> Effectifs { get; set; }
        DbSet<Participation> Participations { get; set; }
        DbSet<Disponibilite> Disponibilites { get; set; }
        DbSet<Cours> Cours { get; set; }
        DbSet<Album> Albums { get; set; }
        DbSet<Photo> Photos { get; set; }
    }
}