using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using PortailReserve.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortailReserve.DAL.Impl
{
    public class CoursDal : ICoursDal
    {
        private BddContext bdd;
        private readonly Logger LOGGER;

        public CoursDal ()
        {
            LOGGER = new Logger(this.GetType());
            bdd = new BddContext();
        }

        public Guid AjouterCours(Cours cours)
        {
            try
            {
                cours.Publication = DateTime.Now;
                cours.Modification = DateTime.Now;

                bdd.Cours.Add(cours);
                bdd.SaveChanges();

                return cours.Id;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur ajouter un cours -> " + e);
                return Guid.Empty;
            }
        }

        public void Dispose()
        {
            bdd.Dispose();
        }

        public List<Cours> GetAllCours()
        {
            try
            {
                List<Cours> all = bdd.Cours.ToList();
                return all;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur rcupération de tous les cours -> " + e);
                return new List<Cours>();
            }
        }

        public Cours GetCoursById(Guid id)
        {
            try
            {
                Cours cours = bdd.Cours.FirstOrDefault(c => c.Id.Equals(id));
                return cours;
            }catch(NullReferenceException nfe)
            {
                LOGGER.Log("ERROR", "Aucun cours trouve pour l'id : " + id + " -> " + nfe);
                return new CoursNull() { Error = "Cours introuvable." };
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur récupération cours id : " + id + " -> " + e);
                return null;
            }
        }

        public List<Cours> GetCoursByTheme(string theme)
        {
            try
            {
                List<Cours> coursByTheme = bdd.Cours.Where(a => a.Theme.Equals(theme)).ToList();
                return coursByTheme;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur récupération des cours par theme : " + theme + " -> " + e);
                return new List<Cours>();
            }
        }

        public int ModifierCours(Guid id, Cours cours)
        {
            try
            {
                Cours toModify = GetCoursById(id);
                if (toModify == null || toModify.Equals(typeof(CoursNull)))
                {
                    LOGGER.Log("ERROR", "Aucun cours à modifier pour l'id : " + id.ToString());
                    return 0;
                }

                toModify.Description = cours.Description;
                toModify.Fichier = cours.Fichier;
                toModify.Nom = cours.Nom;
                toModify.Modification = DateTime.Now;
                toModify.Theme = cours.Theme;
                toModify.Extension = cours.Extension;

                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur modification cours id : " + id + " -> " + e);
                return -1;
            }
        }

        public int SupprimerCours(Guid id)
        {
            try
            {
                Cours toDelete = GetCoursById(id);
                if (toDelete == null || toDelete.Equals(typeof(CoursNull)))
                {
                    LOGGER.Log("ERROR", "Aucun cours à supprimer pour l'id : " + id.ToString());
                    return 0;
                }

                bdd.Cours.Remove(toDelete);
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur suppression cours id : " + id + " -> " + e);
                return -1;
            }
        }
    }
}