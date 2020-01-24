using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
                bdd.Evenements.Add(evenement);
                bdd.SaveChanges();

                return bdd.Evenements.ToList().Last().Id;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur création d'evenement -> " + e);
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
                Console.WriteLine("Erreur récupération de tous les événements -> " + e);
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
                Console.WriteLine("Aucun evenement trouvable pour l'id : " + id + " -> " + nfe);
                return new EvenementNull() { Error = "Evenement introuvable." };
            }
            catch(Exception e)
            {
                Console.WriteLine("Erreur récupération de l'evenement id : " + id + " -> " + e);
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
                Console.WriteLine("Erreur récupération des evenements à venir -> " + e);
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
                Console.WriteLine("Erreur récupération des evenements du type : " + type + " -> " + e);
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
                    int ecart = DateTime.Compare(today, e.Fin);
                    if (ecart >= 0)
                        passe.Add(e);
                }
                return passe;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur récupération des evenements passe -> " + e);
                return new List<Evenement>();
            }
        }

        public Evenement GetProchainEvenement()
        {
            try
            {
                DateTime today = DateTime.Today;
                List<Evenement> aVenir = GetEvenementsAVenir();
                Evenement evenement = aVenir.ElementAt(0);

                for(int i = 1; i<aVenir.Count; i++)
                {
                    if (DateTime.Compare(evenement.Debut, aVenir.ElementAt(i).Debut) > 0)
                        evenement = aVenir.ElementAt(i);
                }

                return evenement;

            }catch(Exception e)
            {
                Console.WriteLine("Erreur récupération du prochain evenement -> " + e);
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
                toModify.Duree = evenement.Duree;
                toModify.Effectif = evenement.Effectif;
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
                Console.WriteLine("Erreur modification evenement -> " + e);
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

                bdd.Evenements.Remove(evenement);
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur suppression evenement id : " + id + " -> " + e);
                return -1;
            }
        }
    }
}