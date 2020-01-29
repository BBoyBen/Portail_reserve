using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.DAL.Impl
{
    public class GroupeDal : IGroupeDal
    {
        private BddContext bdd;

        public GroupeDal ()
        {
            bdd = new BddContext();
        }

        public Guid AjouterGroupe(Groupe groupe)
        {
            try
            {
                bdd.Groupes.Add(groupe);
                bdd.SaveChanges();

                return bdd.Groupes.ToList().Last().Id;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur ajout de groupe -> " + e);
                return Guid.Empty;
            }
        }

        public void Dispose()
        {
            bdd.Dispose();
        }

        public List<Groupe> GetAllGroupes()
        {
            try
            {
                List<Groupe> all = bdd.Groupes.ToList();
                return all;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur récupration de tous les groupes -> " + e);
                return new List<Groupe>();
            }
        }

        public Groupe GetGroupeByCdg(Guid idCdg)
        {
            try
            {
                Groupe byCdg = bdd.Groupes.FirstOrDefault(g => g.CDG.Equals(idCdg));
                return byCdg;
            }catch(NullReferenceException nfe)
            {
                Console.WriteLine("Aucun groupe trouve avec comme cdg : " + idCdg + " -> " + nfe);
                return new GroupeNull() { Error = "Goupe avec ce CDG introuvable." };
            }
            catch(Exception e)
            {
                Console.WriteLine("Erreur récupération du groupe du cdg : " + idCdg + " -> " + e);
                return null;
            }
        }

        public Groupe GetGroupeById(Guid id)
        {
            try
            {
                Groupe groupe = bdd.Groupes.FirstOrDefault(g => g.Id.Equals(id));
                return groupe;
            }catch(NullReferenceException nfe)
            {
                Console.WriteLine("Auncun groupe trouve avec l'id : " + id + " -> " + nfe);
                return new GroupeNull() { Error = "Groupe introuvable." };
            }
            catch(Exception e)
            {
                Console.WriteLine("Erreur récupération du group id : " + id + " -> " + e);
                return null;
            }
        }

        public List<Groupe> GetGroupesBySection(Guid idSection)
        {
            try
            {
                List<Groupe> bySection = bdd.Groupes.Where(g => g.Section.Id.Equals(idSection)).ToList();
                return bySection;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur récupération des groupes de la saction : " + idSection + " -> " + e);
                return new List<Groupe>();
            }
        }

        public int ModifierGroupe(Guid id, Groupe groupe)
        {
            try
            {
                Groupe toModif = GetGroupeById(id);
                if (toModif == null || toModif.Equals(typeof(GroupeNull)))
                    return 0;

                toModif.CDG = groupe.CDG;
                toModif.Numero = groupe.Numero;
                toModif.Section = groupe.Section;
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur modification du groupe : " + id + " -> " + e);
                return -1;
            }
        }

        public int SupprimerGroupe(Guid id)
        {
            try
            {
                Groupe toDelete = GetGroupeById(id);
                if (toDelete == null || toDelete.Equals(typeof(GroupeNull)))
                    return 0;

                bdd.Groupes.Remove(toDelete);
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur suppression du groupe : " + id + " -> " + e);
                return -1;
            }
        }
    }
}