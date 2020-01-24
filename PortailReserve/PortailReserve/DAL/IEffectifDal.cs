using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortailReserve.DAL
{
    public interface IEffectifDal : IDisposable
    {
        Guid AjouterEffectif(Effectif effectif);
        Effectif GetEffectifById(Guid id);
        int ModifierEffectif(Guid id, Effectif effectif);
        int SupprimerEffectif(Guid id);
    }
}
