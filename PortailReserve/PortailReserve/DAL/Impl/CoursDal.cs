using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.DAL.Impl
{
    public class CoursDal : ICoursDal
    {
        private BddContext bdd;

        public CoursDal ()
        {
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

                return bdd.Cours.ToList().Last().Id;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur ajouter un cours -> " + e);
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
                Console.WriteLine("Erreur rcupération de tous les cours -> " + e);
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
                Console.WriteLine("Aucun cours trouve pour l'id : " + id + " -> " + nfe);
                return new CoursNull() { Error = "Cours introuvable." };
            }
            catch(Exception e)
            {
                Console.WriteLine("Erreur récupération cours id : " + id + " -> " + e);
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
                Console.WriteLine("Erreur récupération des cours par theme : " + theme + " -> " + e);
                return new List<Cours>();
            }
        }

        public int ModifierCours(Guid id, Cours cours)
        {
            try
            {
                Cours toModify = GetCoursById(id);
                if (toModify == null || toModify.Equals(typeof(CoursNull)))
                    return 0;

                toModify.Description = cours.Description;
                toModify.Fichier = cours.Fichier;
                toModify.Nom = cours.Nom;
                toModify.Theme = cours.Theme;
                toModify.Modification = DateTime.Now;

                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur modification cours id : " + id + " -> " + e);
                return -1;
            }
        }

        public int SupprimerCours(Guid id)
        {
            try
            {
                Cours toDelete = GetCoursById(id);
                if (toDelete == null || toDelete.Equals(typeof(CoursNull)))
                    return 0;

                bdd.Cours.Remove(toDelete);
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur suppression cours id : " + id + " -> " + e);
                return -1;
            }
        }
    }
}