using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using System;
using System.Collections.Generic;
using System.Linq;
using static PortailReserve.Utils.Utils;
using static PortailReserve.Utils.Logger;
using System.Data.Entity;

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
                if (nouvMdp.Equals(old_mdp))
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

        public List<UtilisateurDispo> GetUtilisateursByDispoOK(Guid idEVent)
        {
            try
            {
                IDisponibiliteDal dDal = new DisponibiliteDal();

                List<UtilisateurDispo> toReturn = new List<UtilisateurDispo>();

                List<Utilisateur> all = bdd.Utilisateurs.ToList();
                foreach(Utilisateur u in all)
                {
                    List<Disponibilite> uDispo = dDal.GetDispoByIdUtilAndByIdEvent(u.Id, idEVent);
                    if (uDispo.Count > 0)
                        if (uDispo.ElementAt(0).Disponible)
                            toReturn.Add(new UtilisateurDispo { Util = u, Dispos = uDispo });
                }
                return toReturn;
            }
            catch(Exception e)
            {
                Log("ERROR", "Erreur récupération des users pour la dispo de l'event : " + idEVent + " -> " + e);
                return new List<UtilisateurDispo>();
            }
        }

        public List<UtilisateurDispo> GetUtilisateursByDispoKO(Guid idEVent)
        {
            try
            {
                IDisponibiliteDal dDal = new DisponibiliteDal();

                List<UtilisateurDispo> toReturn = new List<UtilisateurDispo>();

                List<Utilisateur> all = bdd.Utilisateurs.ToList();
                foreach (Utilisateur u in all)
                {
                    List<Disponibilite> uDispo = dDal.GetDispoByIdUtilAndByIdEvent(u.Id, idEVent);
                    if (uDispo.Count > 0)
                        if (!uDispo.ElementAt(0).Disponible)
                            toReturn.Add(new UtilisateurDispo { Util = u, Dispos = uDispo });
                }
                return toReturn;
            }
            catch (Exception e)
            {
                Log("ERROR", "Erreur récupération des users pour la dispo de l'event : " + idEVent + " -> " + e);
                return new List<UtilisateurDispo>();
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

        public List<UtilisateurParticipation> GetUtilisateursByParticipationOK(Guid idEvent)
        {
            try
            {
                List<UtilisateurParticipation> toReturn = new List<UtilisateurParticipation>();

                List<Utilisateur> all = bdd.Utilisateurs.ToList();
                foreach (Utilisateur u in all)
                {
                    Participation part = bdd.Participations.FirstOrDefault(p => p.Evenement.Equals(idEvent) && p.Utilisateur.Equals(u.Id) && p.Participe);
                    if (part != null)
                        toReturn.Add(new UtilisateurParticipation { Util = u, Participation = part });
                }
                return toReturn;
            }
            catch (Exception e)
            {
                Log("ERROR", "Erreur récupération des users pour la participation de l'event : " + idEvent + " -> " + e);
                return new List<UtilisateurParticipation>();
            }
        }

        public List<UtilisateurParticipation> GetUtilisateursByParticipationKO(Guid idEvent)
        {
            try
            {
                List<UtilisateurParticipation> toReturn = new List<UtilisateurParticipation>();

                List<Utilisateur> all = bdd.Utilisateurs.ToList();
                foreach (Utilisateur u in all)
                {
                    Participation part = bdd.Participations.FirstOrDefault(p => p.Evenement.Equals(idEvent) && p.Utilisateur.Equals(u.Id) && !p.Participe);
                    if (part != null)
                        toReturn.Add(new UtilisateurParticipation { Util = u, Participation = part });
                }
                return toReturn;
            }
            catch (Exception e)
            {
                Log("ERROR", "Erreur récupération des users pour la participation de l'event : " + idEvent + " -> " + e);
                return new List<UtilisateurParticipation>();
            }
        }

        public List<Utilisateur> GetUtilisateursSansReponseDispo(Guid idEVent)
        {
            try
            {
                List<Utilisateur> toReturn = new List<Utilisateur>();

                List<Utilisateur> all = bdd.Utilisateurs.ToList();
                List<Guid> byEVent = bdd.Disponibilites.Where(d => d.Evenement.Equals(idEVent)).Select(d => d.Utilisateur).ToList(); ;
                foreach (Utilisateur u in all)
                {
                    if (!byEVent.Contains(u.Id))
                        toReturn.Add(u);

                }
                return toReturn;
            }
            catch (Exception e)
            {
                Log("ERROR", "Erreur récupération des users sans réponse pour la dispo de l'event : " + idEVent + " -> " + e);
                return new List<Utilisateur>();
            }
        }

        public List<Utilisateur> GetUtilisateursSansReponseParticipation(Guid idEvent)
        {
            try
            {
                List<Utilisateur> toReturn = new List<Utilisateur>();

                List<Utilisateur> all = bdd.Utilisateurs.ToList();
                List<Guid> byEVent = bdd.Participations.Where(p => p.Evenement.Equals(idEvent)).Select(p => p.Utilisateur).ToList(); ;
                foreach (Utilisateur u in all)
                {
                    if (!byEVent.Contains(u.Id))
                        toReturn.Add(u);

                }
                return toReturn;
            }
            catch (Exception e)
            {
                Log("ERROR", "Erreur récupération des users sans réponses pour la participation de l'event : " + idEvent + " -> " + e);
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