using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.DAL.Impl
{
    public class ReponseDal : IReponseDal
    {
        private BddContext bdd;

        public ReponseDal()
        {
            bdd = new BddContext();
        }

        public Guid AjouterReponse(Reponse reponse)
        {
            try
            {
                reponse.Date = DateTime.Now;

                bdd.Reponses.Add(reponse);
                bdd.SaveChanges();

                return bdd.Reponses.ToList().Last().Id;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur ajout de reponse -> " + e);
                return Guid.Empty;
            }
        }

        public void Dispose()
        {
            bdd.Dispose();
        }

        public List<Reponse> GetAllReponse()
        {
            try
            {
                List<Reponse> all = bdd.Reponses.ToList();
                return all;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur récupération de toutes les reponses -> " + e);
                return new List<Reponse>();
            }
        }

        public Reponse GetReponseById(Guid id)
        {
            try
            {
                Reponse reponse = bdd.Reponses.FirstOrDefault(r => r.Id.Equals(id));
                return reponse;
            }catch(NullReferenceException nfe)
            {
                Console.WriteLine("Aucune reponse trouve pour l'id : " + id + " -> " + nfe);
                return new ReponseNull() { Error = "Reponse introuvable." };
            }
            catch(Exception e)
            {
                Console.WriteLine("Erreur récupération de la reponse id : " + id + " -> " + e);
                return null;
            }
        }

        public List<Reponse> GetReponsesByMessage(Guid idMessage)
        {
            try
            {
                List<Reponse> byMessage = bdd.Reponses.Where(r => r.Message.Id.Equals(idMessage)).ToList();
                return byMessage;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur récupération des reponse du message id : " + idMessage + " -> " + e);
                return new List<Reponse>();
            }
        }

        public int SupprimerReponse(Guid id)
        {
            try
            {
                Reponse toDelete = GetReponseById(id);
                if (toDelete == null || toDelete.Equals(typeof(ReponseNull)))
                    return 0;

                bdd.Reponses.Remove(toDelete);
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur suppression de reponse id : " + id + " -> " + e);
                return -1;
            }
        }
    }
}