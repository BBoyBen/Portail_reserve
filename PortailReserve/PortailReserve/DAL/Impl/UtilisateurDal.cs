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
                utilisateur.MotDePasse = Utils.Utils.EncodeSHA256(utilisateur.MotDePasse);
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
            try
            {
                String encodeMdp = Utils.Utils.EncodeSHA256(motDePasse);
                Utilisateur utilisateur = bdd.Utilisateurs.FirstOrDefault(u => u.MotDePasse.Equals(encodeMdp) && u.Matricule.Equals(matricule));

                return utilisateur;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur authentification matricule : " + matricule + " -> " + e);
                return null;
            }
        }

        public int ChangerMotDePasse(long id, string old_mdp, string nouvMdp, string nouvMdpBis)
        {
            try
            {
                if (!nouvMdp.Equals(nouvMdpBis))
                    return 0;

                Utilisateur utilisateur = bdd.Utilisateurs.FirstOrDefault(u => u.Id == id);
                if (utilisateur == null)
                    return -1;

                string encodeOldMdp = Utils.Utils.EncodeSHA256(old_mdp);
                if (!utilisateur.MotDePasse.Equals(encodeOldMdp))
                    return -2;

                string encodeNewMdp = Utils.Utils.EncodeSHA256(nouvMdp);
                utilisateur.MotDePasse = encodeNewMdp;
                bdd.SaveChanges();

                return 1;
            }catch (Exception e)
            {
                Console.WriteLine("Erreur changement de mot de mot pour l'utilisateur : " + id + " -> " + e);
                return -10;
            }
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

        public Utilisateur GetUtilisateurByMatricule(string matricule)
        {
            try
            {
                if (Utils.Utils.ValideMatricule(matricule))
                {
                    Utilisateur utilisateur = bdd.Utilisateurs.FirstOrDefault(u => u.Matricule.Equals(matricule));
                    return utilisateur;
                }
                return null;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur récupération utilisateur par matricule : " + matricule + " -> " + e);
                return null;
            }
        }

        public List<Utilisateur> GetUtilisateursByGroupe(long idGroupe)
        {
            try
            {
                List<Utilisateur> utilisateurs = bdd.Utilisateurs.Where(u => u.Groupe.Id == idGroupe).ToList();
                return utilisateurs;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur récupération utilisateur par id groupe : " + idGroupe + " -> " + e);
                return new List<Utilisateur>();
            }
        }

        public int ModifierUtilisateur(long id, Utilisateur utilisateur)
        {
            try
            {
                Utilisateur util = bdd.Utilisateurs.FirstOrDefault(u => u.Id == id);
                if (util == null)
                    return 0;

                util.Nom = utilisateur.Nom;
                util.Prenom = utilisateur.Prenom;
                util.Grade = utilisateur.Grade;
                util.Matricule = utilisateur.Grade;
                util.Naissance = utilisateur.Naissance;
                util.Telephone = utilisateur.Telephone;
                util.Email = utilisateur.Email;
                util.Adresse.CodePostal = utilisateur.Adresse.CodePostal;
                util.Adresse.Voie = utilisateur.Adresse.Voie;
                util.Adresse.Ville = utilisateur.Adresse.Ville;
                util.Adresse.Pays = utilisateur.Adresse.Pays;

                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur modification utilisateur id : " + id + " -> " + e);
                return -1;
            }
        }

        public int MotDePassePerdu(string nom, string matricule, DateTime naissance)
        {
            try
            {
                Utilisateur utilisateur = bdd.Utilisateurs.FirstOrDefault(u => u.Nom.Equals(nom) && u.Matricule.Equals(matricule) && u.Naissance.Equals(naissance));

                if (utilisateur == null)
                    return 0;

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur mot de passe oublié nom : " + nom + " matricule : " + matricule + " date de naissance " + naissance.ToString() + " -> " + e);
                return -1;
            }
        }

        public int PremiereCoKO(long id)
        {
            try
            {
                Utilisateur utilisateur = bdd.Utilisateurs.FirstOrDefault(u => u.Id == id);
                if (utilisateur == null)
                    return 0;

                utilisateur.PremiereCo = true;
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur reset de la premiere co id : " + id + " -> " + e);
                return -1;
            }
        }

        public int PremiereCoOk(long id)
        {
            try
            {
                Utilisateur utilisateur = bdd.Utilisateurs.FirstOrDefault(u => u.Id == id);
                if (utilisateur == null)
                    return 0;

                utilisateur.PremiereCo = false;
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur passage de la premier co a false pour id : " + id + " -> " + e);
                return -1;
            }
        }

        public int SupprimerUtilisateur(long id)
        {
            try
            {
                Utilisateur utilisateur = bdd.Utilisateurs.FirstOrDefault(u => u.Id == id);
                if (utilisateur == null)
                    return 0;

                bdd.Utilisateurs.Remove(utilisateur);
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur suppression utilisateur id : " + id + " -> " + e);
                return -1;
            }
        }
    }
}