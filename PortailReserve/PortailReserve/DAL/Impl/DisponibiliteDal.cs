using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PortailReserve.Utils.Logger;

namespace PortailReserve.DAL.Impl
{
    public class DisponibiliteDal : IDisponibiliteDal
    {
        private BddContext bdd;

        public DisponibiliteDal ()
        {
            bdd = new BddContext();
        }

        public Guid AjouterDispo(Disponibilite dispo)
        {
            try
            {
                dispo.Reponse = DateTime.Now;
                dispo.Valide = 0;
                bdd.Disponibilites.Add(dispo);
                bdd.SaveChanges();

                return bdd.Disponibilites.ToList().Last().Id;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur ajout dispo -> " + e);
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
                Log("ERROR", "Erreur récupration dispo par evenet : " + idEvent + " -> " + e);
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
                Log("ERROR", "Erreur récupération des dispo de l'util : " + idUtil + " -> " + e);
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
                Log("ERROR", "Aucune disponibilité trouvee pour l'id : " + id + " -> " + nfe);
                return new DisponibiliteNull() { Error = "Disponibilité introuvable." };
            }
            catch(Exception e)
            {
                Log("ERROR", "Erreur récupération de la dispo par id : " + id + " -> " + e);
                return null;
            }
        }

        public Disponibilite GetDispoByIdUtilAndByIdEvent(Guid idUtil, Guid idEvent)
        {
            try
            {
                Disponibilite disponibilite = bdd.Disponibilites.FirstOrDefault(d => d.Utilisateur.Equals(idUtil) && d.Evenement.Equals(idEvent));
                return disponibilite;
            }
            catch (NullReferenceException nfe)
            {
                Log("ERROR", "Aucune disponibilite de l'utiliateur " + idUtil + " à l'event " + idEvent + " -> " + nfe);
                return null;
            }
            catch (Exception e)
            {
                Log("ERROR", "Erreur récupération disponibilite de l'util : " + idUtil + " pour l'event : " + idEvent + " -> " + e);
                return null;
            }
        }

        public int ModifierDispo(Guid id, Disponibilite dispo)
        {
            try
            {
                Disponibilite toModif = GetDispoById(id);
                if (toModif == null || toModif.Equals(typeof(DisponibiliteNull)))
                    return 0;

                toModif.TouteLaPeriode = dispo.TouteLaPeriode;
                toModif.Debut = dispo.Debut;
                toModif.Fin = dispo.Fin;
                toModif.Disponible = dispo.Disponible;
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur modificatio dispo id : " + id + " -> " + e);
                return -1;
            }
        }

        public int RefuserDispo(Guid id)
        {
            try
            {
                Disponibilite dispo = GetDispoById(id);
                if (dispo == null || dispo.Equals(typeof(DisponibiliteNull)))
                    return 0;

                dispo.Valide = -1;
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur refuser dispo id : " + id + " -> " + e);
                return -1;
            }
        }

        public int SupprimerDispo(Guid id)
        {
            try
            {
                Disponibilite toDelete = GetDispoById(id);
                if (toDelete == null || toDelete.Equals(typeof(DisponibiliteNull)))
                    return 0;

                bdd.Disponibilites.Remove(toDelete);
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur suppression dispo id : " + id + " -> " + e);
                return -1;
            }
        }

        public int ValiderDispo(Guid id)
        {
            try
            {
                Disponibilite dispo = GetDispoById(id);
                if (dispo == null || dispo.Equals(typeof(DisponibiliteNull)))
                    return 0;

                dispo.Valide = 1;
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur validation dispo id : " + id + "-> " + e);
                return -1;
            }
        }
    }
}