using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using PortailReserve.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PortailReserve.DAL.Impl
{
    public class SectionDal : ISectionDal
    {
        private BddContext bdd;
        private readonly Logger LOGGER;

        public SectionDal ()
        {
            bdd = new BddContext();
            LOGGER = new Logger(this.GetType());
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
                LOGGER.Log("ERROR", "Erreur ajout de section -> " + e);
                return Guid.Empty;
            }
        }

        public int ChangerCds(Guid id, Guid cds)
        {
            try
            {
                Section section = bdd.Sections.FirstOrDefault(s => s.Id.Equals(id));
                if (section == null || section.Equals(typeof(SectionNull)))
                {
                    LOGGER.Log("EEROR", "La section " + id + " est introuvable pour le changement de cds");
                    return -1;
                }

                section.CDS = cds;
                bdd.SaveChanges();

                return 1;
            }
            catch(Exception e)
            {
                LOGGER.Log("EEROR", "Une erreur est survenue lors du changement de CDS de la section : " + id + " -> " + e);
                return 0;
            }
        }

        public int ChangerSoa(Guid id, Guid soa)
        {
            try
            {
                Section section = bdd.Sections.FirstOrDefault(s => s.Id.Equals(id));
                if (section == null || section.Equals(typeof(SectionNull)))
                {
                    LOGGER.Log("ERROR", "La section " + id + " est introuvable pour le changement de soa.");
                    return -1;
                }

                section.SOA = soa;
                bdd.SaveChanges();

                return 1;
            }
            catch (Exception e)
            {
                LOGGER.Log("EEROR", "Une erreur est survenue lors du changement de SOA de la section : " + id + " -> " + e);
                return 0;
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
                LOGGER.Log("ERROR", "Erreur récupération de toutes les sections -> " + e);
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
                LOGGER.Log("ERROR", "Aucune section trouvee pour l'id : " + id + " -> " + nfe);
                return new SectionNull() { Error = "Section introuvable" };
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur récupération de la section id : " + id + " -> " + e);
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
                LOGGER.Log("ERROR", "Erreur récupération de la section numero " + section + " de la cie " + cie + " -> " + e);
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
                LOGGER.Log("ERROR", "Aucune section trouvee pour le numéro : " + numero + " -> " + nfe);
                return new SectionNull() { Error = "Section introuvable" };
            }
            catch (Exception e)
            {
                LOGGER.Log("ERROR", "Erreur récupération de la section numéro : " + numero + " -> " + e);
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
                LOGGER.Log("ERROR", "Erreur récupération des sections par la compagnie : " + id + " -> " + e);
                return new List<Section>();
            }
        }

        public int ModifierSection(Guid id, Section section)
        {
            try
            {
                Section toModif = GetSectionById(id);
                if (toModif == null || toModif.Equals(typeof(SectionNull)))
                {
                    LOGGER.Log("ERROR", "Aucune section à modifier pour l'id : " + id.ToString());
                    return 0;
                }

                toModif.Devise = section.Devise;
                toModif.Chant = section.Chant;
                toModif.Numero = section.Numero;

                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur modification de la section id : " + id + " -> " + e);
                return -1;
            }
        }

        public int SupprimerSection(Guid id)
        {
            try
            {
                Section toDelete = GetSectionById(id);
                if (toDelete == null || toDelete.Equals(typeof(SectionNull)))
                {
                    LOGGER.Log("ERROR", "Aucune section à supprimer pour l'id : " + id.ToString());
                    return 0;
                }

                bdd.Sections.Remove(toDelete);
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur suppression de la section id : " + id + " -> " + e);
                return -1;
            }
        }
    }
}