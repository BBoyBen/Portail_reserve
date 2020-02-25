using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.DAL
{
    public interface IDisponibiliteDal : IDisposable
    {
        Guid AjouterDispo(Disponibilite dispo);
        Disponibilite GetDispoById(Guid id);
        List<Disponibilite> GetDispoByIdUtilAndByIdEvent(Guid idUtil, Guid idEvent);
        List<Disponibilite> GetAllDispoByEvent(Guid idEvent);
        List<Disponibilite> GetAllDispoByUser(Guid idUtil);
        int ModifierDispo(Guid id, Disponibilite dispo);
        int SupprimerDispo(Guid id);
        int ValiderDispo(Guid id);
        int RefuserDispo(Guid id);
    }
}