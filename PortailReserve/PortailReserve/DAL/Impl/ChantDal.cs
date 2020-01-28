using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.DAL.Impl
{
    public class ChantDal : IChantDal
    {
        private BddContext bdd;

        public ChantDal()
        {
            bdd = new BddContext();
        }

        public Guid AjouterChant(Chant chant)
        {
            try
            {
                chant.Modification = DateTime.Now;
                chant.Publication = DateTime.Now;

                bdd.Chants.Add(chant);
                bdd.SaveChanges();

                return bdd.Chants.ToList().Last().Id;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur ajout de nouveaux chant -> " + e);
                return Guid.Empty;
            }
        }

        public void Dispose()
        {
            bdd.Dispose();
        }

        public List<Chant> GetAllChants()
        {
            try
            {
                List<Chant> chants = bdd.Chants.ToList();

                return chants;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur récupération de tous les chants -> " + e);
                return new List<Chant>();
            }
        }

        public Chant GetChantById(Guid id)
        {
            try
            {
                Chant chant = bdd.Chants.FirstOrDefault(c => c.Id.Equals(id));
                return chant;
            }catch(NullReferenceException nfe)
            {
                Console.WriteLine("Aucun chant trouve pour l'id : " + id + " -> " + nfe);
                return new ChantNull() { Error = "Chant introuvable." };
            }catch(Exception e)
            {
                Console.WriteLine("Erreur récupération du chant id : " + id + " -> " + e);
                return null;
            }
        }

        public Chant GetChantByTitre(string titre)
        {
            try
            {
                Chant chant = bdd.Chants.FirstOrDefault(c => c.Titre.Equals(titre));
                return chant;
            }catch(NullReferenceException nfe)
            {
                Console.WriteLine("Aucun chant trouve pour le titre : " + titre + " -> " + nfe);
                return new ChantNull() { Error = "Chant introuvable pour le titre : " + titre };
            }catch(Exception e)
            {
                Console.WriteLine("Erreur récupration du chat au titre : " + titre + " -> "  e);
                return null;
            }
        }

        public List<Chant> GetChantsByType(int type)
        {
            try
            {
                List<Chant> chants = bdd.Chants.Where(c => c.Type == type).ToList();
                return chants;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur récupération des chants par type : " + type + " -> " + e);
                return new List<Chant>();
            }
        }

        public int ModifierChant(Guid id, Chant chant)
        {
            try
            {
                Chant toModifiy = GetChantById(id);
                if (toModifiy == null || toModifiy.Equals(typeof(ChantNull)))
                    return 0;

                toModifiy.Modification = DateTime.Now;
                toModifiy.Titre = chant.Titre;
                toModifiy.Texte = chant.Texte;
                toModifiy.Type = chant.Type;
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur modification du chant : " + id + " -> " + e);
                return -1;
            }
        }

        public int SupprimerChant(Guid id)
        {
            try
            {
                Chant toDelete = GetChantById(id);
                if (toDelete == null || toDelete.Equals(typeof(ChantNull)))
                    return 0;

                bdd.Chants.Remove(toDelete);
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur suppression du chant : " + id + " -> " + e);
                return -1;
            }
        }
    }
}