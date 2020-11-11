using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using PortailReserve.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortailReserve.DAL.Impl
{
    public class GroupeDal : IGroupeDal
    {
        private BddContext bdd;
        private readonly Logger LOGGER;

        public GroupeDal ()
        {
            bdd = new BddContext();
            LOGGER = new Logger(this.GetType());
        }

        public Guid AjouterGroupe(Groupe groupe)
        {
            try
            {
                bdd.Groupes.Add(groupe);
                bdd.SaveChanges();

                return groupe.Id;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur ajout de groupe -> " + e);
                return Guid.Empty;
            }
        }

        public int ChangerCdg(Guid id, Guid idNouvCdg)
        {
            try
            {
                Groupe grp = bdd.Groupes.FirstOrDefault(g => g.Id.Equals(id));

                grp.CDG = idNouvCdg;

                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur changement de chef de groupe pour le groupe : " + id + " -> " + e);
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
                LOGGER.Log("ERROR", "Erreur récupration de tous les groupes -> " + e);
                return new List<Groupe>();
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
                LOGGER.Log("ERROR", "Auncun groupe trouve avec l'id : " + id + " -> " + nfe);
                return new GroupeNull() { Error = "Groupe introuvable." };
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur récupération du group id : " + id + " -> " + e);
                return null;
            }
        }

        public Groupe GetGroupeByNumeroAndBySection(int numGrp, int numSection)
        {
            try
            {
                Section section = bdd.Sections.FirstOrDefault(s => s.Numero.Equals(numSection));
                if (section == null)
                    return new GroupeNull() { Error = "Section introuvable." };

                List<Groupe> groupes = GetGroupesBySection(section.Id);
                Groupe toReturn = new Groupe();
                foreach(Groupe g in groupes)
                {
                    if (g.Numero.Equals(numGrp))
                        toReturn = g;
                }

                return toReturn;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur récupération du groupe " + numGrp + "de la section " + numSection + " -> " + e);
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
                LOGGER.Log("ERROR", "Erreur récupération des groupes de la saction : " + idSection + " -> " + e);
                return new List<Groupe>();
            }
        }

        public int ModifierGroupe(Guid id, Groupe groupe)
        {
            try
            {
                Groupe toModif = GetGroupeById(id);
                if (toModif == null || toModif.Equals(typeof(GroupeNull)))
                {
                    LOGGER.Log("ERROR", "Aucun groupe à modifier pour l'id : " + id.ToString());
                    return 0;
                }

                toModif.Numero = groupe.Numero;
                toModif.Section = groupe.Section;
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur modification du groupe : " + id + " -> " + e);
                return -1;
            }
        }

        public int SupprimerGroupe(Guid id)
        {
            try
            {
                Groupe toDelete = GetGroupeById(id);
                if (toDelete == null || toDelete.Equals(typeof(GroupeNull)))
                {
                    LOGGER.Log("ERROR", "Aucun groupe à supprimer pour l'id : " + id.ToString());
                    return 0;
                }

                bdd.Groupes.Remove(toDelete);
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur suppression du groupe : " + id + " -> " + e);
                return -1;
            }
        }
    }
}