using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortailReserve.DAL
{
    public interface IReponseDal : IDisposable
    {
        Guid AjouterReponse(Reponse reponse);
        Reponse GetReponseById(Guid id);
        List<Reponse> GetAllReponse();
        List<Reponse> GetReponsesByMessage(Guid idMessage);
        int SupprimerReponse(Guid id);
    }
}
