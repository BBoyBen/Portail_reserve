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
        int ChangerSoa(Guid id, Guid soa);
        int ChangerCds(Guid id, Guid cds);
        List<Section> GetSectionsByCompagnie(Guid id);
        Section GetSectionByNumero(int numero);
        Section GetSectionByNumAndByCie(int section, int cie);
    }
}
