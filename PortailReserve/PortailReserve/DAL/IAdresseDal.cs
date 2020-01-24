using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortailReserve.DAL
{
    public interface IAdresseDal : IDisposable
    {
        Guid AjouterAdresse(Adresse adresse);
        int ModifierAdresse(Guid id, Adresse adresse);
        Adresse GetAdresseById(Guid id);
        int SupprimerAdresse(Guid id);

    }
}
