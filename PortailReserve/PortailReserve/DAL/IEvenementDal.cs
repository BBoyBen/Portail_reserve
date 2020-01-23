using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortailReserve.DAL
{
    public interface IEvenementDal : IDisposable
    {
        long CreerEvenement(Evenement evenement);
        Evenement GetEvenementById(long id);
        List<Evenement> GetAll();
        int ModifierEvenement(long id, Evenement evenement);
        int SupprimerEvenement(long id);
        List<Evenement> GetEvenementsByType(string type);
        List<Evenement> GetEvenementsAVenir();
        List<Evenement> GetEvenementsPasse();
        Evenement GetProchainEvenement();

    }
}
