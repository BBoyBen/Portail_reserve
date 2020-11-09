using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.DAL
{
    public interface IChantDal : IDisposable
    {
        Guid AjouterChant(Chant chant);
        Chant GetChantById(Guid id);
        List<Chant> GetAllChants();
        List<Chant> GetChantsByType(string type);
        int ModifierChant(Guid id, Chant chant);
        int SupprimerChant(Guid id);
        Chant GetChantByTitre(string titre);
        bool ValiderTitreChant(string titre);
    }
}