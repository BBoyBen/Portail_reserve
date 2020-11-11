using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using PortailReserve.Utils;
using System;
using System.Linq;

namespace PortailReserve.DAL.Impl
{
    public class EffectifDal : IEffectifDal
    {
        private BddContext bdd;
        private readonly Logger LOGGER;

        public EffectifDal () 
        {
            bdd = new BddContext();
            LOGGER = new Logger(this.GetType());
        }

        public Guid AjouterEffectif(Effectif effectif)
        {
            try
            {
                bdd.Effectifs.Add(effectif);
                bdd.SaveChanges();

                return effectif.Id;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur ajout effectif -> " + e);
                return Guid.Empty;
            }
        }

        public void Dispose()
        {
            bdd.Dispose();
        }

        public Effectif GetEffectifById(Guid id)
        {
            try
            {
                Effectif effectif = bdd.Effectifs.FirstOrDefault(e => e.Id.Equals(id));

                return effectif;
            }catch(NullReferenceException nfe)
            {
                LOGGER.Log("ERROR", "Aucun effectif trouve pour l'id : " + id + " -> " + nfe);
                return null;
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur récupération effectif par id : " + id + " -> " + e);
                return null;
            }
        }

        public int ModifierEffectif(Guid id, Effectif effectif)
        {
            try
            {
                Effectif toModif = GetEffectifById(id);
                if (toModif == null || toModif.Equals(typeof(EffectifNull)))
                {
                    LOGGER.Log("ERROR", "Aucun effectif à modifier pour l'id : " + id.ToString());
                    return 0;
                }

                toModif.Officier = effectif.Officier;
                toModif.SousOfficier = effectif.SousOfficier;
                toModif.Militaire = effectif.Militaire;
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur modification effectif id : " + id + " -> " + e);
                return -1;
            }
        }

        public int SupprimerEffectif(Guid id)
        {
            try
            {
                Effectif toDelete = GetEffectifById(id);
                if (toDelete == null || toDelete.Equals(typeof(EffectifNull)))
                {
                    LOGGER.Log("ERROR", "Aucun effectif à supprimer pour l'id : " + id.ToString());
                    return 0;
                }

                bdd.Effectifs.Remove(toDelete);
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur suppression effectif id : " + id + " -> " + e);
                return -1;
            }
        }
    }
}