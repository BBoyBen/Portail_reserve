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
        List<Utilisateur> GetUtilisateursByGroupe(long id_groupe);
        Utilisateur Authentifier(String matricule, String motDePasse);
        int ChangerMotDePasse(long id, String old_mdp, String new_mdp);
        int MotDePassePerdu(long id, String nom, String matricule);
        int ModifierUtilisateur(long id, Utilisateur utilisateur);
        int PremiereCoOk(long id);
        int PremiereCoKO(long id);
        long AjouterUtilisateur(Utilisateur utilisateur);
        int SupprimerUtilisateur(long id);
    }
}
