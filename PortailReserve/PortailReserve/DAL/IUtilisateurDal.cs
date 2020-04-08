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
        List<Utilisateur> GetUtilisateursBySectionByGroupe(int section, int cie);
        Utilisateur Authentifier(string matricule, string motDePasse);
        int ChangerMotDePasse(Guid id, string oldMdp, string nouvMdp, string nouvMdpBis);
        int PremierChangementMotDePasse(Guid id, string mdp, string mdpBis);
        int MotDePassePerdu(string nom, string matricule, DateTime naissance);
        int ModifierUtilisateur(Guid id, Utilisateur utilisateur);
        int AjouterUtilisateurSection(Guid id, Utilisateur utiisateur);
        int ModifierGrade(Guid id, String grade);
        int ModifierGroupe(Guid id, Guid grp);
        int PremiereCoOk(Guid id);
        int PremiereCoKO(Guid id);
        Guid AjouterUtilisateur(Utilisateur utilisateur);
        int SupprimerUtilisateur(Guid id);
        int SupprimerUtilisateurSection(Guid id);
        List<UtilisateurDispo> GetUtilisateursByDispoOK(Guid idEVent, int section, int cie);
        List<UtilisateurDispo> GetUtilisateursByDispoKO(Guid idEVent, int section, int cie);
        List<UtilisateurParticipation> GetUtilisateursByParticipationOK(Guid idEvent, int section, int cie);
        List<UtilisateurParticipation> GetUtilisateursByParticipationKO(Guid idEvent, int section, int cie);
        List<Utilisateur> GetUtilisateursSansReponseDispo(Guid idEVent, int section, int cie);
        List<Utilisateur> GetUtilisateursSansReponseParticipation(Guid idEvent, int section, int cie);
        List<Utilisateur> GetUtilisateursSansSection();

    }
}
