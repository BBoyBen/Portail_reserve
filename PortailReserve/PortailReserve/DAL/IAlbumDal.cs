using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortailReserve.DAL
{
    public interface IAlbumDal : IDisposable
    {
        Guid AjouterAlbum(Album album);
        int ModifierAlbum(Guid id, Album album);
        int SupprimerAlbum(Guid id);
        Album GetAlbumById(Guid id);
    }
}
