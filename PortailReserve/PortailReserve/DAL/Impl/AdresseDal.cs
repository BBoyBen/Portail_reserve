using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PortailReserve.Utils.Logger;

namespace PortailReserve.DAL.Impl
{
    public class AdresseDal : IAdresseDal
    {
        private BddContext bdd;

        public AdresseDal ()
        {
            bdd = new BddContext();
        }

        public Guid AjouterAdresse(Adresse adresse)
        {
            try
            {
                bdd.Adresses.Add(adresse);
                bdd.SaveChanges();

                return bdd.Adresses.ToList().Last().Id;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur ajout d'une nouvelle adresse -> " + e);
                return Guid.Empty;
            }
        }

        public void Dispose()
        {
            bdd.Dispose();
        }

        public Adresse GetAdresseById(Guid id)
        {
            try
            {
                Adresse adresse = bdd.Adresses.FirstOrDefault(a => a.Id.Equals(id));

                return adresse;
            }catch(NullReferenceException nfe)
            {
                Log("ERROR", "Aucune adresse trouvee avec l'id : " + id + " -> " + nfe);
                return new AdresseNull() { Error = "Aucune adresse trouvée."};
            }
            catch(Exception e)
            {
                Log("ERROR", "Erreur récupéation de l'adresse id : " + id + " -> " + e);
                return null;
            }
        }

        public int ModifierAdresse(Guid id, Adresse adresse)
        {
            try
            {
                Adresse toMod = GetAdresseById(id);
                if (toMod == null || toMod.Equals(typeof(AdresseNull)))
                    return 0;

                toMod.Pays = adresse.Pays;
                toMod.Ville = adresse.Ville;
                toMod.Voie = adresse.Voie;
                toMod.CodePostal = adresse.CodePostal;

                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Log("ERROR", "Errerur modiication adresse id : " + id + " -> " + e);
                return -1;
            }
        }

        public int SupprimerAdresse(Guid id)
        {
            try
            {
                Adresse toDelete = bdd.Adresses.FirstOrDefault(a => a.Id.Equals(id));
                if (toDelete == null || toDelete.Equals(typeof(AdresseNull)))
                    return 0;

                bdd.Adresses.Remove(toDelete);

                return 1;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur suppression adress id : " + id + " -> " + e);
                return -1;
            }
        }
    }
}