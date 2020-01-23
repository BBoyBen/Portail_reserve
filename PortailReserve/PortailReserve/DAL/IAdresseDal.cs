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
        long AjouterAdresse(Adresse adresse);
        int ModifierAdresse(long id, Adresse adresse);
        Adresse GetAdresseById(long id);
        int SupprimerAdresse(long id);

    }
}
