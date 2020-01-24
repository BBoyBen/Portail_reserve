using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
                Console.WriteLine("Erreur ajout dispo -> " + e);
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
                List<Disponibilite> byEvent = bdd.Disponibilites.Where(d => d.Evenement.Id.Equals(idEvent)).ToList();

                return byEvent;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur récupration dispo par evenet : " + idEvent + " -> " + e);
                return new List<Disponibilite>();
            }
        }

        public List<Disponibilite> GetAllDispoByUser(Guid idUtil)
        {
            try
            {
                List<Disponibilite> byUtil = bdd.Disponibilites.Where(d => d.Utilisateur.Id.Equals(idUtil)).ToList();

                return byUtil;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur récupération des dispo de l'util : " + idUtil + " -> " + e);
                return new List<Disponibilite>();
            }
        }

        public Disponibilite GetDispoById(Guid id)
        {
            try
            {
                Disponibilite dispo = bdd.Disponibilites.FirstOrDefault(d => d.Id.Equals(id));

                return dispo;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur récupération de la dispo par id : " + id + " -> " + e);
                return null;
            }
        }

        public int ModifierDispo(Guid id, Disponibilite dispo)
        {
            try
            {
                Disponibilite toModif = GetDispoById(id);
                if (toModif == null)
                    return 0;

                toModif.TouteLaPeriode = dispo.TouteLaPeriode;
                toModif.Debut = dispo.Debut;
                toModif.Fin = dispo.Fin;
                toModif.Disponible = dispo.Disponible;
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur modificatio dispo id : " + id + " -> " + e);
                return -1;
            }
        }

        public int RefuserDispo(Guid id)
        {
            try
            {
                Disponibilite dispo = GetDispoById(id);
                if (dispo == null)
                    return 0;

                dispo.Valide = -1;
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur refuser dispo id : " + id + " -> " + e);
                return -1;
            }
        }

        public int SupprimerDispo(Guid id)
        {
            try
            {
                Disponibilite toDelete = GetDispoById(id);
                if (toDelete == null)
                    return 0;

                bdd.Disponibilites.Remove(toDelete);
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur suppression dispo id : " + id + " -> " + e);
                return -1;
            }
        }

        public int ValiderDispo(Guid id)
        {
            try
            {
                Disponibilite dispo = GetDispoById(id);
                if (dispo == null)
                    return 0;

                dispo.Valide = 1;
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur validation dispo id : " + id + "-> " + e);
                return -1;
            }
        }
    }
}