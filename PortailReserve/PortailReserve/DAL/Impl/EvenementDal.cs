using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PortailReserve.Utils.Logger;

namespace PortailReserve.DAL.Impl
{
    public class EvenementDal : IEvenementDal
    {
        private BddContext bdd;

        public EvenementDal ()
        {
            bdd = new BddContext();
        }

        public Guid CreerEvenement(Evenement evenement)
        {
            try
            {
                if (evenement.LimiteReponse.Year == 1)
                    evenement.LimiteReponse = evenement.Debut;

                bdd.Evenements.Add(evenement);
                bdd.SaveChanges();

                return evenement.Id;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur création d'evenement -> " + e);
                return Guid.Empty;
            }
        }

        public void Dispose()
        {
            bdd.Dispose();
        }

        public List<Evenement> GetAll()
        {
            try
            {
                List<Evenement> all = bdd.Evenements.ToList();

                return all;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur récupération de tous les événements -> " + e);
                return new List<Evenement>();
            }
        }

        public Evenement GetEvenementById(Guid id)
        {
            try
            {
                Evenement evenement = bdd.Evenements.FirstOrDefault(e => e.Id.Equals(id));
                return evenement;
            }catch(NullReferenceException nfe)
            {
                Log("ERROR", "Aucun evenement trouvable pour l'id : " + id + " -> " + nfe);
                return new EvenementNull() { Error = "Evenement introuvable." };
            }
            catch(Exception e)
            {
                Log("ERROR", "Erreur récupération de l'evenement id : " + id + " -> " + e);
                return null;
            }
        }

        public List<Evenement> GetEvenementsAVenir()
        {
            try
            {
                DateTime today = DateTime.Today;
                List<Evenement> aVenir = new List<Evenement>();

                foreach(Evenement e in bdd.Evenements)
                {
                    int ecart = DateTime.Compare(today, e.Debut);
                    if (ecart < 0)
                        aVenir.Add(e);
                }

                return aVenir;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur récupération des evenements à venir -> " + e);
                return new List<Evenement>();
            }
        }

        public List<Evenement> GetEvenementsByType(string type)
        {
            try
            {
                List<Evenement> byType = bdd.Evenements.Where(e => e.Type.Equals(type)).ToList();
                return byType;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur récupération des evenements du type : " + type + " -> " + e);
                return new List<Evenement>();
            }
        }

        public List<Evenement> GetEvenementsPasse()
        {
            try
            {
                DateTime today = DateTime.Today;
                List<Evenement> passe = new List<Evenement>();

                foreach(Evenement e in bdd.Evenements)
                {
                    int ecart = DateTime.Compare(today, e.Debut);
                    if (ecart >= 0)
                        passe.Add(e);
                }
                return passe;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur récupération des evenements passe -> " + e);
                return new List<Evenement>();
            }
        }

        public Evenement GetProchainEvenement()
        {
            try
            {
                DateTime today = DateTime.Today;
                List<Evenement> aVenir = GetEvenementsAVenir();
                if (aVenir.Count < 1)
                    return null;

                Evenement evenement = aVenir.ElementAt(0);
                if (aVenir.Count == 1)
                    return evenement;

                for(int i = 0; i<aVenir.Count; i++)
                {
                    if (DateTime.Compare(evenement.Debut, aVenir.ElementAt(i).Debut) > 0)
                        evenement = aVenir.ElementAt(i);
                }

                return evenement;

            }catch(Exception e)
            {
                Log("ERROR", "Erreur récupération du prochain evenement -> " + e);
                return null;
            }
        }

        public int ModifierEvenement(Guid id, Evenement evenement)
        {
            try
            {
                Evenement toModify = GetEvenementById(id);
                if (toModify == null || toModify.Equals(typeof(EvenementNull)))
                    return 0;

                toModify.Debut = evenement.Debut;
                toModify.Description = evenement.Description;
                toModify.Fin = evenement.Fin;
                toModify.Lieu = evenement.Lieu;
                toModify.LimiteReponse = evenement.LimiteReponse;
                toModify.Nom = evenement.Nom;
                toModify.Patracdr = evenement.Patracdr;
                toModify.Type = evenement.Type;

                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur modification evenement -> " + e);
                return -1;
            }
        }

        public int SupprimerEvenement(Guid id)
        {
            try
            {
                Evenement evenement = GetEvenementById(id);
                if (evenement == null || evenement.Equals(typeof(EvenementNull)))
                    return 0;

                Effectif effectif = bdd.Effectifs.FirstOrDefault(e => e.Id.Equals(evenement.Effectif));
                List<Disponibilite> dispos = bdd.Disponibilites.Where(d => d.Evenement.Equals(evenement.Id)).ToList();
                List<Participation> participations = bdd.Participations.Where(p => p.Evenement.Equals(evenement.Id)).ToList();

                bdd.Effectifs.Remove(effectif);
                bdd.SaveChanges();

                foreach(Disponibilite d in dispos)
                {
                    bdd.Disponibilites.Remove(d);
                    bdd.SaveChanges();
                }

                foreach(Participation p in participations)
                {
                    bdd.Participations.Remove(p);
                    bdd.SaveChanges();
                }

                bdd.Evenements.Remove(evenement);
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur suppression evenement id : " + id + " -> " + e);
                return -1;
            }
        }
    }
}