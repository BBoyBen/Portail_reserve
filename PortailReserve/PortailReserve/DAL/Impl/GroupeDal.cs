using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using static PortailReserve.Utils.Logger;

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

        public int ChangerCdg(Guid id, Guid idNouvCdg)
        {
            try
            {
                Utilisateur cdgActu = GetCdg(id);
                Utilisateur nouvCdg = bdd.Utilisateurs.FirstOrDefault(u => u.Id.Equals(idNouvCdg));
                if (nouvCdg == null)
                    return 0;

                nouvCdg.EstCDG = true;
                nouvCdg.Groupe = id;
                cdgActu.EstCDG = false;

                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur changement de chef de groupe pour le groupe : " + id + " -> " + e);
                return -1;
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

        public Utilisateur GetCdg(Guid id)
        {
            try
            {
                Utilisateur cdg = bdd.Utilisateurs.FirstOrDefault(u => u.Groupe.Equals(id) && u.EstCDG);

                return cdg;
            }catch(NullReferenceException nfe)
            {
                Log("ERROR", "Aucun chef de groupe trouvé pour le groupe : " + id + " -> " + nfe);
                return new UtilisateurNull() { Error = "Chef de groupe introuvable" };
            }
            catch(Exception e)
            {
                Log("ERROR", "Erreur récupération du chef de groupe pour le groupe : " + id + " -> " + e);
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
                List<Groupe> bySection = bdd.Groupes.Where(g => g.Section.Equals(idSection)).ToList();
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