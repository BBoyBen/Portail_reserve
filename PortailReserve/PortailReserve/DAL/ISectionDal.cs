using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortailReserve.DAL
{
    public interface ISectionDal : IDisposable
    {
        Guid AjouterSection(Section section);
        Section GetSectionById(Guid id);
        List<Section> GetAllSection();
        int ModifierSection(Guid id, Section section);
        int SupprimerSection(Guid id);
        List<Section> GetSectionsByCompagnie(Guid id);
        Section GetSectionByNumero(int numero);
        Section GetSectionByNumAndByCie(int section, int cie);
    }
}
