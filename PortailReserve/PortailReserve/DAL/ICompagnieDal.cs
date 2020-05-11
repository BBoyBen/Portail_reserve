using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortailReserve.DAL
{
    public interface ICompagnieDal : IDisposable
    {
        Guid AjouterCompagnie(Compagnie compagnie);
        int ModifierCompagnie(Guid id, Compagnie compagnie);
        int SupprimerCompagnie(Guid id);
        int ChangerCdu(Guid id, Guid cdu);
        Compagnie GetCompagnieById(Guid id);
        List<Compagnie> GetAllCompagnie();
        Compagnie GetCompagnieByNumero(int numero);
    }
}
