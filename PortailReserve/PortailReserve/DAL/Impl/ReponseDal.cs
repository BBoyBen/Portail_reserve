using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using PortailReserve.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortailReserve.DAL.Impl
{
    public class ReponseDal : IReponseDal
    {
        private BddContext bdd;
        private readonly Logger LOGGER;

        public ReponseDal()
        {
            bdd = new BddContext();
            LOGGER = new Logger(this.GetType());
        }

        public Guid AjouterReponse(Reponse reponse)
        {
            try
            {
                reponse.Date = DateTime.Now;

                bdd.Reponses.Add(reponse);
                bdd.SaveChanges();

                return reponse.Id;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur ajout de reponse -> " + e);
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
                LOGGER.Log("ERROR", "Erreur récupération de toutes les reponses -> " + e);
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
                LOGGER.Log("ERROR", "Aucune reponse trouve pour l'id : " + id + " -> " + nfe);
                return new ReponseNull() { Error = "Reponse introuvable." };
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur récupération de la reponse id : " + id + " -> " + e);
                return null;
            }
        }

        public List<Reponse> GetReponsesByMessage(Guid idMessage)
        {
            try
            {
                List<Reponse> byMessage = bdd.Reponses.Where(r => r.Message.Equals(idMessage)).ToList();
                return byMessage;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur récupération des reponse du message id : " + idMessage + " -> " + e);
                return new List<Reponse>();
            }
        }

        public int SupprimerReponse(Guid id)
        {
            try
            {
                Reponse toDelete = GetReponseById(id);
                if (toDelete == null || toDelete.Equals(typeof(ReponseNull)))
                {
                    LOGGER.Log("ERROR", "Aucune reponse à supprimer pour l'id : " + id.ToString());
                    return 0;
                }

                bdd.Reponses.Remove(toDelete);
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur suppression de reponse id : " + id + " -> " + e);
                return -1;
            }
        }
    }
}