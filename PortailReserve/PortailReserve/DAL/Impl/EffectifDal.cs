using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.DAL.Impl
{
    public class EffectifDal : IEffectifDal
    {
        private BddContext bdd;

        public EffectifDal () 
        {
            bdd = new BddContext();    
        }

        public Guid AjouterEffectif(Effectif effectif)
        {
            try
            {
                bdd.Effectifs.Add(effectif);
                bdd.SaveChanges();

                return bdd.Effectifs.ToList().Last().Id;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur ajout effectif -> " + e);
                return Guid.Empty;
            }
        }

        public void Dispose()
        {
            bdd.Dispose();
        }

        public Effectif GetEffectifById(Guid id)
        {
            try
            {
                Effectif effectif = bdd.Effectifs.FirstOrDefault(e => e.Id.Equals(id));

                return effectif;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur récupération effectif par id : " + id + " -> " + e);
                return null;
            }
        }

        public int ModifierEffectif(Guid id, Effectif effectif)
        {
            try
            {
                Effectif toModif = GetEffectifById(id);
                if (toModif == null)
                    return 0;

                toModif.Officier = effectif.Officier;
                toModif.SousOfficier = effectif.SousOfficier;
                toModif.Militaire = effectif.Militaire;
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur modification effectif id : " + id + " -> " + e);
                return -1;
            }
        }

        public int SupprimerEffectif(Guid id)
        {
            try
            {
                Effectif toDelete = GetEffectifById(id);
                if (toDelete == null)
                    return 0;

                bdd.Effectifs.Remove(toDelete);
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur suppression effectif id : " + id + " -> " + e);
                return -1;
            }
        }
    }
}