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
        Guid CreerEvenement(Evenement evenement);
        Evenement GetEvenementById(Guid id);
        List<Evenement> GetAll();
        int ModifierEvenement(Guid id, Evenement evenement);
        int SupprimerEvenement(Guid id);
        List<Evenement> GetEvenementsByType(string type);
        List<Evenement> GetEvenementsAVenir();
        List<Evenement> GetEvenementsPasse();
        Evenement GetProchainEvenement();
        List<Evenement> GetEvenementsPourPlanning();

    }
}
