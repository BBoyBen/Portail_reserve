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

                return section.Id;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur ajout de section -> " + e);
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
                Log("ERROR", "Erreur récupération de toutes les sections -> " + e);
                return new List<Section>();
            }
        }

        public Section GetSectionById(Guid id)
        {
            try
            {
                Section section = bdd.Sections.FirstOrDefault(s => s.Id.Equals(id));
                return section;
            }catch(NullReferenceException nfe)
            {
                Log("ERROR", "Aucune section trouvee pour l'id : " + id + " -> " + nfe);
                return new SectionNull() { Error = "Section introuvable" };
            }
            catch(Exception e)
            {
                Log("ERROR", "Erreur récupération de la section id : " + id + " -> " + e);
                return null;
            }
        }

        public Section GetSectionByNumAndByCie(int section, int cie)
        {
            try
            {
                Section sec = bdd.Sections.FirstOrDefault(s => s.Numero.Equals(section) && s.NumCie.Equals(cie));
                return sec;
            }
            catch(Exception e)
            {
                Log("ERROR", "Erreur récupération de la section numero " + section + " de la cie " + cie + " -> " + e);
                return null;
            }
        }

        public Section GetSectionByNumero(int numero)
        {
            try
            {
                Section section = bdd.Sections.FirstOrDefault(s => s.Numero.Equals(numero));
                return section;
            }
            catch (NullReferenceException nfe)
            {
                Log("ERROR", "Aucune section trouvee pour le numéro : " + numero + " -> " + nfe);
                return new SectionNull() { Error = "Section introuvable" };
            }
            catch (Exception e)
            {
                Log("ERROR", "Erreur récupération de la section numéro : " + numero + " -> " + e);
                return null;
            }
        }

        public List<Section> GetSectionsByCompagnie(Guid id)
        {
            try
            {
                List<Section> sections = bdd.Sections.Where(s => s.Compagnie.Equals(id)).ToList();

                return sections;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur récupération des sections par la compagnie : " + id + " -> " + e);
                return new List<Section>();
            }
        }

        public int ModifierSection(Guid id, Section section)
        {
            try
            {
                Section toModif = GetSectionById(id);
                if (toModif == null || toModif.Equals(typeof(SectionNull)))
                    return 0;

                toModif.Devise = section.Devise;
                toModif.Chant = section.Chant;
                toModif.Numero = section.Numero;

                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur modification de la section id : " + id + " -> " + e);
                return -1;
            }
        }

        public int SupprimerSection(Guid id)
        {
            try
            {
                Section toDelete = GetSectionById(id);
                if (toDelete == null || toDelete.Equals(typeof(SectionNull)))
                    return 0;

                bdd.Sections.Remove(toDelete);
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur suppression de la section id : " + id + " -> " + e);
                return -1;
            }
        }
    }
}