using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.DAL.Impl
{
    public class SectionDal : ISectionDal
    {
        private BddContext bdd;

        public SectionDal ()
        {
            bdd = new BddContext();
        }

        public Guid AjouterSection(Section section)
        {
            try
            {
                bdd.Sections.Add(section);
                bdd.SaveChanges();

                return bdd.Sections.ToList().Last().Id;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur ajout de section -> " + e);
                return Guid.Empty;
            }
        }

        public void Dispose()
        {
            bdd.Dispose();
        }

        public List<Section> GetAllSection()
        {
            try
            {
                List<Section> all = bdd.Sections.ToList();
                return all;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur récupération de toutes les sections -> " + e);
                return new List<Section>();
            }
        }

        public Section GetSectionById(Guid id)
        {
            try
            {
                Section section = bdd.Sections.FirstOrDefault(s => s.Id.Equals(id));
                return section;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur récupération de la section id : " + id + " -> " + e);
                return null;
            }
        }

        public int ModifierSection(Guid id, Section section)
        {
            try
            {
                Section toModif = GetSectionById(id);
                if (toModif == null)
                    return 0;

                toModif.CDS = section.CDS;
                toModif.SOA = section.SOA;
                toModif.Devise = section.Devise;
                toModif.Chant = toModif.Chant;
                toModif.Numero = toModif.Numero;
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur modification de la section id : " + id + " -> " + e);
                return -1;
            }
        }

        public int SupprimerSection(Guid id)
        {
            try
            {
                Section toDelete = GetSectionById(id);
                if (toDelete == null)
                    return 0;

                bdd.Sections.Remove(toDelete);
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur suppression de la section id : " + id + " -> " + e);
                return -1;
            }
        }
    }
}