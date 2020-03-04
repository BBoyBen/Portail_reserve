using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PortailReserve.Utils.Logger;

namespace PortailReserve.DAL.Impl
{
    public class ParticipationDal : IParticipationDal
    {
        private BddContext bdd;

        public ParticipationDal ()
        {
            bdd = new BddContext();
        }

        public Guid AjouterParticipation(Guid util, Guid evenement, bool participe)
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

                return participation.Id;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur ajout de participation -> " + e);
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
            }catch(NullReferenceException nfe)
            {
                Log("ERROR", "Aucune participation trouvee pour l'id : " + id + " -> " + nfe);
                return new ParticipationNull() { Error = "Partiticpation introuvable." };
            }
            catch(Exception e)
            {
                Log("ERROR", "Erreur récupération de la participation id : " + id + " -> " + e);
                return null;
            }
        }

        public Participation GetParticipationByUtilAndEvent(Guid idUtil, Guid idEvent)
        {
            try
            {
                Participation participation = bdd.Participations.FirstOrDefault(p => p.Utilisateur.Equals(idUtil) && p.Evenement.Equals(idEvent));
                return participation;
            }catch(NullReferenceException nfe)
            {
                Log("ERROR", "Aucune particiation de l'utiliateur " + idUtil + " à l'event " + idEvent + " -> " + nfe);
                return null;
            }
            catch(Exception e)
            {
                Log("ERROR", "Erreur récupération participation de l'util : " + idUtil + " pour l'event : " + idEvent + " -> " + e);
                return null;
            }
        }

        public List<Participation> GetParticipationsByEvent(Guid idEvent)
        {
            try
            {
                List<Participation> participations = bdd.Participations.Where(p => p.Evenement.Equals(idEvent)).ToList();
                return participations;
             }catch(Exception e)
            {
                Log("ERROR", "Erreur récuperation des participation pour l'event : " + idEvent + " -> " + e);
                return new List<Participation>();
            }
        }

        public int ModifierParticipation(Guid id, bool modif, Guid idUtil)
        {
            try
            {
                Participation toModif = GetParticipationByUtilAndEvent(idUtil, id);
                if (toModif == null || toModif.Equals(typeof(ParticipationNull)))
                    return 0;

                if (!toModif.Utilisateur.Equals(idUtil))
                    return -10;

                toModif.Participe = modif;
                toModif.Reponse = DateTime.Now;

                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur modification participation : " + id + " -> " + e);
                return -1;
            }
        }
    }
}