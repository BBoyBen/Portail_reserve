using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortailReserve.DAL
{
    public interface IGroupeDal : IDisposable
    {
        Guid AjouterGroupe(Groupe groupe);
        Groupe GetGroupeById(Guid id);
        int ModifierGroupe(Guid id, Groupe groupe);
        Groupe GetGroupeByCdg(Guid idCdg);
        List<Groupe> GetAllGroupes();
        List<Groupe> GetGroupesBySection(Guid idSection);
        int SupprimerGroupe(Guid id);
    }
}
