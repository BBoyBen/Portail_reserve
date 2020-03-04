﻿using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PortailReserve.Utils.Logger;

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

                return effectif.Id;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur ajout effectif -> " + e);
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
            }catch(NullReferenceException nfe)
            {
                Log("ERROR", "Aucun effectif trouve pour l'id : " + id + " -> " + nfe);
                return null;
            }
            catch(Exception e)
            {
                Log("ERROR", "Erreur récupération effectif par id : " + id + " -> " + e);
                return null;
            }
        }

        public int ModifierEffectif(Guid id, Effectif effectif)
        {
            try
            {
                Effectif toModif = GetEffectifById(id);
                if (toModif == null || toModif.Equals(typeof(EffectifNull)))
                    return 0;

                toModif.Officier = effectif.Officier;
                toModif.SousOfficier = effectif.SousOfficier;
                toModif.Militaire = effectif.Militaire;
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur modification effectif id : " + id + " -> " + e);
                return -1;
            }
        }

        public int SupprimerEffectif(Guid id)
        {
            try
            {
                Effectif toDelete = GetEffectifById(id);
                if (toDelete == null || toDelete.Equals(typeof(EffectifNull)))
                    return 0;

                bdd.Effectifs.Remove(toDelete);
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur suppression effectif id : " + id + " -> " + e);
                return -1;
            }
        }
    }
}