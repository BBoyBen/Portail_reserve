using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using PortailReserve.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortailReserve.DAL.Impl
{
    public class DisponibiliteDal : IDisponibiliteDal
    {
        private BddContext bdd;
        private readonly Logger LOGGER;

        public DisponibiliteDal ()
        {
            bdd = new BddContext();
            LOGGER = new Logger(this.GetType());
        }

        public Guid AjouterDispo(Disponibilite dispo)
        {
            try
            {
                dispo.Reponse = DateTime.Now;
                dispo.Valide = 0;
                bdd.Disponibilites.Add(dispo);
                bdd.SaveChanges();

                return dispo.Id;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur ajout dispo -> " + e);
                return Guid.Empty;
            }
        }

        public void Dispose()
        {
            bdd.Dispose();
        }

        public List<Disponibilite> GetAllDispoByEvent(Guid idEvent)
        {
            try
            {
                List<Disponibilite> byEvent = bdd.Disponibilites.Where(d => d.Evenement.Equals(idEvent)).ToList();

                return byEvent;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur récupration dispo par evenet : " + idEvent + " -> " + e);
                return new List<Disponibilite>();
            }
        }

        public List<Disponibilite> GetAllDispoByUser(Guid idUtil)
        {
            try
            {
                List<Disponibilite> byUtil = bdd.Disponibilites.Where(d => d.Utilisateur.Equals(idUtil)).ToList();

                return byUtil;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur récupération des dispo de l'util : " + idUtil + " -> " + e);
                return new List<Disponibilite>();
            }
        }

        public Disponibilite GetDispoById(Guid id)
        {
            try
            {
                Disponibilite dispo = bdd.Disponibilites.FirstOrDefault(d => d.Id.Equals(id));

                return dispo;
            }catch(NullReferenceException nfe)
            {
                LOGGER.Log("ERROR", "Aucune disponibilité trouvee pour l'id : " + id + " -> " + nfe);
                return new DisponibiliteNull() { Error = "Disponibilité introuvable." };
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur récupération de la dispo par id : " + id + " -> " + e);
                return null;
            }
        }

        public List<Disponibilite> GetDispoByIdUtilAndByIdEvent(Guid idUtil, Guid idEvent)
        {
            try
            {
                List<Disponibilite> disponibilite = bdd.Disponibilites.Where(d => d.Utilisateur.Equals(idUtil) && d.Evenement.Equals(idEvent)).ToList();
                return disponibilite;
            }
            catch (NullReferenceException nfe)
            {
                LOGGER.Log("ERROR", "Aucune disponibilite de l'utiliateur " + idUtil + " à l'event " + idEvent + " -> " + nfe);
                return new List<Disponibilite>();
            }
            catch (Exception e)
            {
                LOGGER.Log("ERROR", "Erreur récupération disponibilite de l'util : " + idUtil + " pour l'event : " + idEvent + " -> " + e);
                return new List<Disponibilite>();
            }
        }

        public int ModifierDispo(Guid id, Disponibilite dispo)
        {
            try
            {
                Disponibilite toModif = GetDispoById(id);
                if (toModif == null || toModif.Equals(typeof(DisponibiliteNull)))
                {
                    LOGGER.Log("ERROR", "Aucune dispo à modifier pour l'id : " + id.ToString());
                    return 0;
                }

                toModif.TouteLaPeriode = dispo.TouteLaPeriode;
                toModif.Debut = dispo.Debut;
                toModif.Fin = dispo.Fin;
                toModif.Disponible = dispo.Disponible;
                toModif.Valide = 0;
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur modificatio dispo id : " + id + " -> " + e);
                return -1;
            }
        }

        public int RefuserDispo(Guid id)
        {
            try
            {
                Disponibilite dispo = GetDispoById(id);
                if (dispo == null || dispo.Equals(typeof(DisponibiliteNull)))
                {
                    LOGGER.Log("ERROR", "Aucune dispo à refuser pour l'id : " + id.ToString());
                    return 0;
                }

                dispo.Valide = 2;
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur refuser dispo id : " + id + " -> " + e);
                return -1;
            }
        }

        public int SupprimerDispo(Guid id)
        {
            try
            {
                Disponibilite toDelete = GetDispoById(id);
                if (toDelete == null || toDelete.Equals(typeof(DisponibiliteNull)))
                {
                    LOGGER.Log("ERROR", "Aucune dispo à supprimer pour l'id : " + id.ToString());
                    return 0;
                }

                bdd.Disponibilites.Remove(toDelete);
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur suppression dispo id : " + id + " -> " + e);
                return -1;
            }
        }

        public int ValiderDispo(Guid id)
        {
            try
            {
                Disponibilite dispo = GetDispoById(id);
                if (dispo == null || dispo.Equals(typeof(DisponibiliteNull)))
                {
                    LOGGER.Log("ERROR", "Aucune dispo à valider pour l'id : " + id.ToString());
                    return 0;
                }

                dispo.Valide = 1;
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur validation dispo id : " + id + "-> " + e);
                return -1;
            }
        }
    }
}