using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortailReserve.DAL
{
    public interface ICoursDal : IDisposable
    {
        Guid AjouterCours(Cours cours);
        Cours GetCoursById(Guid id);
        int ModifierCours(Guid id, Cours cours);
        int SupprimerCours(Guid id);
        List<Cours> GetAllCours();
        List<Cours> GetCoursByTheme(string theme);
    }
}
