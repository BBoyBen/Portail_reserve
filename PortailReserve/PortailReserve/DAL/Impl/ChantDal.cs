using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using PortailReserve.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortailReserve.DAL.Impl
{
    public class ChantDal : IChantDal
    {
        private BddContext bdd;
        private readonly Logger LOGGER;

        public ChantDal()
        {
            bdd = new BddContext();
            LOGGER = new Logger(this.GetType());
        }

        public Guid AjouterChant(Chant chant)
        {
            try
            {
                chant.Modification = DateTime.Now;
                chant.Publication = DateTime.Now;

                bdd.Chants.Add(chant);
                bdd.SaveChanges();

                return chant.Id;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur ajout de nouveaux chant -> " + e);
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
                LOGGER.Log("ERROR", "Erreur récupération de tous les chants -> " + e);
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
                LOGGER.Log("ERROR", "Aucun chant trouve pour l'id : " + id + " -> " + nfe);
                return new ChantNull() { Error = "Chant introuvable." };
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur récupération du chant id : " + id + " -> " + e);
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
                LOGGER.Log("ERROR", "Aucun chant trouve pour le titre : " + titre + " -> " + nfe);
                return new ChantNull() { Error = "Chant introuvable pour le titre : " + titre };
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur récupration du chat au titre : " + titre + " -> " + e);
                return null;
            }
        }

        public List<Chant> GetChantsByType(string type)
        {
            try
            {
                List<Chant> chants = bdd.Chants.Where(c => c.Type == type).ToList();
                return chants;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur récupération des chants par type : " + type + " -> " + e);
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
                LOGGER.Log("ERROR", "Erreur modification du chant : " + id + " -> " + e);
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
                LOGGER.Log("ERROR", "Erreur suppression du chant : " + id + " -> " + e);
                return -1;
            }
        }

        public bool ValiderTitreChant(string titre)
        {
            try
            {
                List<Chant> find = bdd.Chants.Where(c => c.Titre == titre).ToList();

                return find.Count > 0;
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur validation du titre d'un chant : " + titre + " -> " + e);
                return false;
            }
        }
    }
}