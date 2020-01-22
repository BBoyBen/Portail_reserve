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
        Utilisateur GetUtilisateurById(long id);
        Utilisateur GetUtilisateurByMatricule(string matricule);
        List<Utilisateur> GetUtilisateursByGroupe(long id_groupe);
        Utilisateur Authentifier(string matricule, string motDePasse);
        int ChangerMotDePasse(long id, string old_mdp, string new_mdp);
        int MotDePassePerdu(long id, string nom, string matricule);
        int ModifierUtilisateur(long id, Utilisateur utilisateur);
        int PremiereCoOk(long id);
        int PremiereCoKO(long id);
        long AjouterUtilisateur(Utilisateur utilisateur);
        int SupprimerUtilisateur(long id);
    }
}
