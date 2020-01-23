using PortailReserve.Models;
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

        public long CreerEvenement(Evenement evenement)
        {
            try
            {
                bdd.Evenements.Add(evenement);
                bdd.SaveChanges();

                return bdd.Evenements.ToList().Last().Id;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur création d'evenement -> " + e);
                return -1;
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

        public Evenement GetEvenementById(long id)
        {
            throw new NotImplementedException();
        }

        public List<Evenement> GetEvenementsAVenir()
        {
            throw new NotImplementedException();
        }

        public List<Evenement> GetEvenementsByType(string type)
        {
            throw new NotImplementedException();
        }

        public List<Evenement> GetEvenementsPasse()
        {
            throw new NotImplementedException();
        }

        public Evenement GetProchainEvenement()
        {
            throw new NotImplementedException();
        }

        public int ModifierEvenement(long id, Evenement evenement)
        {
            throw new NotImplementedException();
        }

        public int SupprimerEvenement(long id)
        {
            throw new NotImplementedException();
        }
    }
}