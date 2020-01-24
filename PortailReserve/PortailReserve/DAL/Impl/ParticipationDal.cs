using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.DAL.Impl
{
    public class ParticipationDal : IParticipationDal
    {
        private BddContext bdd;

        public ParticipationDal ()
        {
            bdd = new BddContext();
        }

        public Guid AjouterParticipation(Utilisateur util, Evenement evenement, bool participe)
        {
            try
            {
                Participation participation = new Participation() { 
                    Utilisateur = util,
                    Evenement = evenement,
                    Participe = participe,
                    Reponse = DateTime.Now
                };

                bdd.Participations.Add(participation);
                bdd.SaveChanges();

                return bdd.Participations.ToList().Last().Id;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur ajout de participation -> " + e);
                return Guid.Empty;
            }

        }

        public void Dispose()
        {
            bdd.Dispose();
        }

        public Participation GetParticipationById(Guid id)
        {
            try
            {
                Participation participation = bdd.Participations.FirstOrDefault(p => p.Id.Equals(id));
                return participation;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur récupération de la participation id : " + id + " -> " + e);
                return null;
            }
        }

        public Participation GetParticipationByUtilAndEvent(Guid idUtil, Guid idEvent)
        {
            try
            {
                Participation participation = bdd.Participations.FirstOrDefault(p => p.Utilisateur.Id.Equals(idUtil) && p.Evenement.Id.Equals(idEvent));
                return participation;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur récupération participation de l'util : " + idUtil + " pour l'event : " + idEvent + " -> " + e);
                return null;
            }
        }

        public List<Participation> GetParticipationsByEvent(Guid idEvent)
        {
            try
            {
                List<Participation> participations = bdd.Participations.Where(p => p.Evenement.Id.Equals(idEvent)).ToList();
                return participations;
             }catch(Exception e)
            {
                Console.WriteLine("Erreur récuperation des participation pour l'event : " + idEvent + " -> " + e);
                return new List<Participation>();
            }
        }

        public int ModifierParticipation(Guid id, bool modif, Guid idUtil)
        {
            try
            {
                Participation toModif = GetParticipationById(id);
                if (toModif == null)
                    return 0;

                if (!toModif.Utilisateur.Id.Equals(idUtil))
                    return -10;

                toModif.Participe = modif;
                toModif.Reponse = DateTime.Now;

                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur modification participation : " + id + " -> " + e);
                return -1;
            }
        }
    }
}