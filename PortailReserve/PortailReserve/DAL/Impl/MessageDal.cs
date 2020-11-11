using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using PortailReserve.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortailReserve.DAL.Impl
{
    public class MessageDal : IMessageDal
    {
        private BddContext bdd;
        private readonly Logger LOGGER;

        public MessageDal()
        {
            bdd = new BddContext();
            LOGGER = new Logger(this.GetType());
        }

        public Guid AjouterMessage(Message message)
        {
            try
            {
                message.Date = DateTime.Now;

                bdd.Messages.Add(message);
                bdd.SaveChanges();

                return message.Id;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur ajout d'un message -> " + e);
                return Guid.Empty;
            }
        }

        public void Dispose()
        {
            bdd.Dispose();
        }

        public List<Message> GetAllMessage()
        {
            try
            {
                List<Message> all = bdd.Messages.ToList();
                return all;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur récupération de tous les messages -> " + e);
                return new List<Message>();
            }
        }

        public Message GetMessageById(Guid id)
        {
            try
            {
                Message message = bdd.Messages.FirstOrDefault(m => m.Id.Equals(id));
                return message;
            }catch(NullReferenceException nfe)
            {
                LOGGER.Log("ERROR", "Aucun message trouve avec l'id : " + id + " -> " + nfe);
                return new MessageNull() { Error = "Message introuvable." };
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur récupértion messag par id : " + id + " -> " + e);
                return null;
            }
        }

        public List<Message> GetMessagesByEvent(Guid idEvent)
        {
            try
            {
                List<Message> byEvent = bdd.Messages.Where(m => m.Evenement.Equals(idEvent)).ToList();
                return byEvent;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur récupération des messages de l'event id : " + idEvent + " -> " + e);
                return new List<Message>();
            }
        }

        public int SupprimerMessage(Guid id)
        {
            try
            {
                Message toDelete = GetMessageById(id);
                if (toDelete == null || toDelete.Equals(typeof(MessageNull)))
                {
                    LOGGER.Log("ERROR", "Aucun message à supprimer pour l'id : " + id.ToString());
                    return 0;
                }

                bdd.Messages.Remove(toDelete);
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur suppression message id : " + id + " -> " + e);
                return -1;
            }
        }
    }
}