using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortailReserve.DAL
{
    public interface IUtilisateurDal : IDisposable
    {
        Utilisateur GetUtilisateurById(Guid id);
        Utilisateur GetUtilisateurById(string id);
        Utilisateur GetUtilisateurByMatricule(string matricule);
        List<Utilisateur> GetUtilisateursByGroupe(Guid idGroupe);
        Utilisateur Authentifier(string matricule, string motDePasse);
        int ChangerMotDePasse(Guid id, string oldMdp, string nouvMdp, string nouvMdpBis);
        int PremierChangementMotDePasse(Guid id, string mdp, string mdpBis);
        int MotDePassePerdu(string nom, string matricule, DateTime naissance);
        int ModifierUtilisateur(Guid id, Utilisateur utilisateur);
        int PremiereCoOk(Guid id);
        int PremiereCoKO(Guid id);
        Guid AjouterUtilisateur(Utilisateur utilisateur);
        int SupprimerUtilisateur(Guid id);
        List<Utilisateur> GetUtilisateursByDispo(Guid idEVent);
        List<Utilisateur> GetUtilisateursByParticipation(Guid idEvent);
        List<Utilisateur> GetUtilisateursSansReponseDispo(Guid idEVent);
        List<Utilisateur> GetUtilisateursSansReponseParticipation(Guid idEvent);

    }
}
