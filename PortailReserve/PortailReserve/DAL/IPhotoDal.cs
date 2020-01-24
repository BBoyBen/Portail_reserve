using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortailReserve.DAL
{
    public interface IPhotoDal : IDisposable
    {
        Guid AjouterPhoto(Photo photo);
        Photo GetPhotoById(Guid id);
        List<Photo> GetAllPhotos();
        List<Photo> GetPhotosByAlbum(Guid idAlbum);
        int SupprimerPhoto(Guid id);
    }
}
