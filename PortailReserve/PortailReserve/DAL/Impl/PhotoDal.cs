using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.DAL.Impl
{
    public class PhotoDal : IPhotoDal
    {
        private BddContext bdd;

        public PhotoDal()
        {
            bdd = new BddContext();
        }

        public Guid AjouterPhoto(Photo photo)
        {
            try
            {
                photo.Ajout = DateTime.Now;

                bdd.Photos.Add(photo);
                bdd.SaveChanges();

                return bdd.Photos.ToList().Last().Id;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur ajout de photo -> " + e);
                return Guid.Empty;
            }
        }

        public void Dispose()
        {
            bdd.Dispose();
        }

        public List<Photo> GetAllPhotos()
        {
            try
            {
                List<Photo> all = bdd.Photos.ToList();
                return all;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur récupération de toutes les photos -> " + e);
                return new List<Photo>();
            }
        }

        public Photo GetPhotoById(Guid id)
        {
            try
            {
                Photo photo = bdd.Photos.FirstOrDefault(p => p.Id.Equals(id));
                return photo;
            }catch(NullReferenceException nfe)
            {
                Console.WriteLine("Aucune photo trouvee pour l'id : " + id + " -> " + nfe);
                return new PhotoNull() { Error = "Photo introuvable." };
            }
            catch(Exception e)
            {
                Console.WriteLine("Erreur récupération photo par id : " + id + " -> " + e);
                return null;
            }
        }

        public List<Photo> GetPhotosByAlbum(Guid idAlbum)
        {
            try
            {
                List<Photo> byAlbum = bdd.Photos.Where(p => p.Album.Id.Equals(idAlbum)).ToList();
                return byAlbum;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur récupération des photos de l'album id : " + idAlbum + " -> " + e);
                return new List<Photo>();
            }
        }

        public int SupprimerPhoto(Guid id)
        {
            try
            {
                Photo toDelete = GetPhotoById(id);
                if (toDelete == null || toDelete.Equals(typeof(PhotoNull)))
                    return 0;

                bdd.Photos.Remove(toDelete);
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur suppression photo id : " + id + " -> " + e);
                return -1;
            }
        }
    }
}