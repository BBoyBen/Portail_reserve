using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.DAL.Impl
{
    public class UtilisateurDal : IUtilisateurDal
    {
        private BddContext bdd;

        public UtilisateurDal ()
        {
            bdd = new BddContext();
        }

        public long AjouterUtilisateur(Utilisateur utilisateur)
        {
            try
            {
                bdd.Utilisateurs.Add(utilisateur);
                bdd.SaveChanges();

                return bdd.Utilisateurs.ToList().Last().Id;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur ajout nouvel utilisateur -> " + e);
                return -1;
            }
        }

        public Utilisateur Authentifier(string matricule, string motDePasse)
        {
            throw new NotImplementedException();
        }

        public int ChangerMotDePasse(long id, string old_mdp, string new_mdp)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            bdd.Dispose();
        }

        public Utilisateur GetUtilisateurById(long id)
        {
            try
            {
                Utilisateur utilisateur = bdd.Utilisateurs.FirstOrDefault(u => u.Id == id);
                return utilisateur;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur récupération utilisateurs par id : " + id + " -> " + e);
                return null;
            }
        }

        public List<Utilisateur> GetUtilisateursByGroupe(long id_groupe)
        {
            throw new NotImplementedException();
        }

        public int ModifierUtilisateur(long id, Utilisateur utilisateur)
        {
            throw new NotImplementedException();
        }

        public int MotDePassePerdu(long id, string nom, string matricule)
        {
            throw new NotImplementedException();
        }

        public int PremiereCoKO(long id)
        {
            throw new NotImplementedException();
        }

        public int PremiereCoOk(long id)
        {
            throw new NotImplementedException();
        }

        public int SupprimerUtilisateur(long id)
        {
            throw new NotImplementedException();
        }
    }
}