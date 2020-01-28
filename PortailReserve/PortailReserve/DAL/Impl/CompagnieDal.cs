using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.DAL.Impl
{
    public class CompagnieDal : ICompagnieDal
    {
        private BddContext bdd;

        public CompagnieDal ()
        {
            bdd = new BddContext();
        }

        public Guid AjouterCompagnie(Compagnie compagnie)
        {
            try
            {
                bdd.Compagnies.Add(compagnie);
                bdd.SaveChanges();

                return bdd.Compagnies.ToList().Last().Id;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur ajout nouvelle compagnie -> " + e);
                return Guid.Empty;
            }
        }

        public void Dispose()
        {
            bdd.Dispose();
        }

        public List<Compagnie> GetAllCompagnie()
        {
            try
            {
                List<Compagnie> compagnies = bdd.Compagnies.ToList();
                return compagnies;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur récupération de toutes les compagnies -> " + e);
                return new List<Compagnie>();
            }
        }

        public Compagnie GetCompagnieById(Guid id)
        {
            try
            {
                Compagnie compagnie = bdd.Compagnies.FirstOrDefault(c => c.Id.Equals(id));

                return compagnie;
            }catch(NullReferenceException nfe)
            {
                Console.WriteLine("Aucune compagnie pour l'id : " + id + " -> " + e);
                return new CompagnieNull() { Error = "Compagnie introuvable." };
            }catch(Exception e)
            {
                Console.WriteLine("Erreur récupération de la compagnie id : " + id + " -> " + e);
                return null;
            }
        }

        public int ModifierCompagnie(Guid id, Compagnie compagnie)
        {
            try
            {
                Compagnie toModify = GetCompagnieById(id);
                if (toModify == null || toModify.Equals(typeof(CompagnieNull)))
                    return 0;

                toModify.CDU = compagnie.CDU;
                toModify.ADU = compagnie.ADU;
                toModify.Numero = compagnie.Numero;
                toModify.Chant = compagnie.Chant;
                toModify.Devise = compagnie.Devise;
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur modification de la compagnie : " + id + " -> " + e);
                return -1;
            }
        }

        public int SupprimerCompagnie(Guid id)
        {
            try
            {
                Compagnie toDelete = GetCompagnieById(id);
                if (toDelete == null || toDelete.Equals(typeof(CompagnieNull)))
                    return 0;

                bdd.Compagnies.Remove(toDelete);
                bdd.SavesChange();

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur de suppression de la compagnie id : " + id + " -> " + e);
                return -1;
            }
        }
    }
}