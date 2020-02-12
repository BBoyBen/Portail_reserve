using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using static PortailReserve.Utils.Logger;

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
                Log("ERROR", "Erreur ajout nouvelle compagnie -> " + e);
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
                Log("ERROR", "Erreur récupération de toutes les compagnies -> " + e);
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
                Log("ERROR", "Aucune compagnie pour l'id : " + id + " -> " + nfe);
                return new CompagnieNull() { Error = "Compagnie introuvable." };
            }catch(Exception e)
            {
                Log("ERROR", "Erreur récupération de la compagnie id : " + id + " -> " + e);
                return null;
            }
        }

        public Compagnie GetCompagnieByNumero(int numero)
        {
            try
            {
                Compagnie compagnie = bdd.Compagnies.FirstOrDefault(c => c.Numero.Equals(numero));

                return compagnie;
            }catch(NullReferenceException nfe)
            {
                Log("ERROR", "Aucune compagnie trouvée pour le numéro : " + numero + " -> " + nfe);
                return new CompagnieNull() { Error = "Compagnie introuvable." };
            }catch(Exception e)
            {
                Log("ERROR", "Erreur récupération de compagnie par numéro : " + numero + " -> " + e);
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

                toModify.Numero = compagnie.Numero;
                toModify.Chant = compagnie.Chant;
                toModify.Devise = compagnie.Devise;
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur modification de la compagnie : " + id + " -> " + e);
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
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur de suppression de la compagnie id : " + id + " -> " + e);
                return -1;
            }
        }
    }
}