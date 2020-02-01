using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using System;
using System.Collections.Generic;
using System.Linq;
using static PortailReserve.Utils.Utils;
using static PortailReserve.Utils.Logger;

namespace PortailReserve.DAL.Impl
{
    public class UtilisateurDal : IUtilisateurDal
    {
        private BddContext bdd;

        public UtilisateurDal ()
        {
            bdd = new BddContext();
        }

        public Guid AjouterUtilisateur(Utilisateur utilisateur)
        {
            try
            {
                utilisateur.MotDePasse = EncodeSHA256(utilisateur.MotDePasse);
                bdd.Utilisateurs.Add(utilisateur);
                bdd.SaveChanges();

                return bdd.Utilisateurs.ToList().Last().Id;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur ajout nouvel utilisateur -> " + e);
                return Guid.Empty;
            }
        }

        public Utilisateur Authentifier(string matricule, string motDePasse)
        {
            try
            {
                string encodeMdp = EncodeSHA256(motDePasse);
                Utilisateur utilisateur = bdd.Utilisateurs.FirstOrDefault(u => u.MotDePasse.Equals(encodeMdp) && u.Matricule.Equals(matricule));

                return utilisateur;
            }catch(NullReferenceException nfe)
            {
                Log("ERROR", "Authentification échouer pour matricule : " + matricule + " -> " + nfe);
                return new UtilisateurNull() { Error = "Matricule ou mot de passe incorrect." };
            }
            catch(Exception e)
            {
                Log("ERROR", "Erreur authentification matricule : " + matricule + " -> " + e);
                return null;
            }
        }

        public int ChangerMotDePasse(Guid id, string old_mdp, string nouvMdp, string nouvMdpBis)
        {
            try
            {
                if (!nouvMdp.Equals(nouvMdpBis))
                    return 0;

                Utilisateur utilisateur = GetUtilisateurById(id);
                if (utilisateur == null || utilisateur.Equals(typeof(UtilisateurNull)))
                    return -1;

                string encodeOldMdp = EncodeSHA256(old_mdp);
                if (!utilisateur.MotDePasse.Equals(encodeOldMdp))
                    return -2;

                string encodeNewMdp = EncodeSHA256(nouvMdp);
                utilisateur.MotDePasse = encodeNewMdp;
                bdd.SaveChanges();

                return 1;
            }catch (Exception e)
            {
                Log("ERROR", "Erreur changement de mot de mot pour l'utilisateur : " + id + " -> " + e);
                return -10;
            }
        }

        public void Dispose()
        {
            bdd.Dispose();
        }

        public Utilisateur GetUtilisateurById(Guid id)
        {
            try
            {
                Utilisateur utilisateur = bdd.Utilisateurs.FirstOrDefault(u => u.Id.Equals(id));
                return utilisateur;
            }catch(NullReferenceException nfe)
            {
                Log("ERROR", "Aucun utilisateur trouve pour l'id : " + id + " -> " + nfe);
                return new UtilisateurNull() { Error = "Utilisateur introuvable." };
            }
            catch(Exception e)
            {
                Log("ERROR", "Erreur récupération utilisateurs par id : " + id + " -> " + e);
                return null;
            }
        }

        public Utilisateur GetUtilisateurById(string id)
        {
            try
            {
                Utilisateur utilisateur = bdd.Utilisateurs.FirstOrDefault(u => u.Id.ToString().Equals(id));
                return utilisateur;
            }
            catch (NullReferenceException nfe)
            {
                Log("ERROR", "Aucun utilisateur trouve pour l'id : " + id + " -> " + nfe);
                return new UtilisateurNull() { Error = "Utilisateur introuvable." };
            }
            catch (Exception e)
            {
                Log("ERROR", "Erreur récupération utilisateurs par id : " + id + " -> " + e);
                return null;
            }
        }

        public Utilisateur GetUtilisateurByMatricule(string matricule)
        {
            try
            {
                if (ValideMatricule(matricule))
                {
                    Utilisateur utilisateur = bdd.Utilisateurs.FirstOrDefault(u => u.Matricule.Equals(matricule));
                    return utilisateur;
                }
                return null;
            }catch(NullReferenceException nfe)
            {
                Log("ERROR", "Aucun utilisateur trouve avec le matricule : " + matricule + " -> " + nfe);
                return new UtilisateurNull() { Error = "Utilisateur introuvable." };
            }
            catch(Exception e)
            {
                Log("ERROR", "Erreur récupération utilisateur par matricule : " + matricule + " -> " + e);
                return null;
            }
        }

        public List<Utilisateur> GetUtilisateursByGroupe(Guid idGroupe)
        {
            try
            {
                List<Utilisateur> utilisateurs = bdd.Utilisateurs.Where(u => u.Groupe.Equals(idGroupe)).ToList();
                return utilisateurs;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur récupération utilisateur par id groupe : " + idGroupe + " -> " + e);
                return new List<Utilisateur>();
            }
        }

        public int ModifierUtilisateur(Guid id, Utilisateur utilisateur)
        {
            try
            {
                Utilisateur util = GetUtilisateurById(id);
                if (util == null || util.Equals(typeof(UtilisateurNull)))
                    return 0;

                util.Nom = utilisateur.Nom;
                util.Prenom = utilisateur.Prenom;
                util.Grade = utilisateur.Grade;
                util.Matricule = utilisateur.Matricule;
                util.Naissance = utilisateur.Naissance;
                util.Telephone = utilisateur.Telephone;
                util.Email = utilisateur.Email;

                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur modification utilisateur id : " + id + " -> " + e);
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
            }catch(NullReferenceException nfe)
            {
                Log("ERROR", "Aucun utilisateur trouvé pour le matricule : " + matricule + " et le nom : " + nom + " et nait le " + naissance.ToString() + " -> " + nfe);
                return 0;
            }
            catch(Exception e)
            {
                Log("ERROR", "Erreur mot de passe oublié nom : " + nom + " matricule : " + matricule + " date de naissance " + naissance.ToString() + " -> " + e);
                return -1;
            }
        }

        public int PremierChangementMotDePasse(Guid id, string mdp, string mdpBis)
        {
            try
            {
                if (!mdp.Equals(mdpBis))
                    return 0;

                Utilisateur utilisateur = GetUtilisateurById(id);
                if (utilisateur == null || utilisateur.Equals(typeof(UtilisateurNull)))
                    return -1;

                string encodeNewMdp = EncodeSHA256(mdp);
                utilisateur.MotDePasse = encodeNewMdp;
                bdd.SaveChanges();

                return 1;
            }
            catch (Exception e)
            {
                Log("ERROR", "Erreur premier changement de mot de mot pour l'utilisateur : " + id + " -> " + e);
                return -10;
            }
        }

        public int PremiereCoKO(Guid id)
        {
            try
            {
                Utilisateur utilisateur = GetUtilisateurById(id);
                if (utilisateur == null || utilisateur.Equals(typeof(UtilisateurNull)))
                    return 0;

                utilisateur.PremiereCo = true;
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur reset de la premiere co id : " + id + " -> " + e);
                return -1;
            }
        }

        public int PremiereCoOk(Guid id)
        {
            try
            {
                Utilisateur utilisateur = GetUtilisateurById(id);
                if (utilisateur == null || utilisateur.Equals(typeof(UtilisateurNull)))
                    return 0;

                utilisateur.PremiereCo = false;
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur passage de la premier co a false pour id : " + id + " -> " + e);
                return -1;
            }
        }

        public int SupprimerUtilisateur(Guid id)
        {
            try
            {
                Utilisateur utilisateur = GetUtilisateurById(id);
                if (utilisateur == null || utilisateur.Equals(typeof(UtilisateurNull)))
                    return 0;

                bdd.Utilisateurs.Remove(utilisateur);
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur suppression utilisateur id : " + id + " -> " + e);
                return -1;
            }
        }
    }
}