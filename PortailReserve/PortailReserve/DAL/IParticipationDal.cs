using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortailReserve.DAL
{
    public interface IParticipationDal : IDisposable
    {
        Guid AjouterParticipation(Guid util, Guid evenement, bool participe);
        Participation GetParticipationById(Guid id);
        Participation GetParticipationByUtilAndEvent(Guid idUtil, Guid idEvent);
        List<Participation> GetParticipationsByEvent(Guid idEvent);
        int ModifierParticipation(Guid id, bool modif, Guid idUtil);
    }
}
