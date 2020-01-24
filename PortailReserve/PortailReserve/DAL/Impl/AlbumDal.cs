using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.DAL.Impl
{
    public class AlbumDal : IAlbumDal
    {
        private BddContext bdd;

        public AlbumDal()
        {
            bdd = new BddContext();
        }

        public Guid AjouterAlbum(Album album)
        {
            try
            {
                bdd.Albums.Add(album);
                bdd.SaveChanges();

                return bdd.Albums.ToList().Last().Id;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur créer nouvel album -> " + e);
                return Guid.Empty;
            }
        }

        public void Dispose()
        {
            bdd.Dispose();
        }

        public Album GetAlbumById(Guid id)
        {
            try
            {
                Album album = bdd.Albums.FirstOrDefault(a => a.Id.Equals(id));
                return album;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur récupération albums id : " + id + " -> " + e);
                return null;
            }
        }

        public int ModifierAlbum(Guid id, Album album)
        {
            try
            {
                Album toModify = bdd.Albums.FirstOrDefault(a => a.Id.Equals(id));
                if (toModify == null)
                    return 0;

                toModify.Titre = album.Titre;
                toModify.Description = album.Description;

                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur modiifer album id : " + id + " -> " + e);
                return -1;
            }
        }

        public int SupprimerAlbum(Guid id)
        {
            try
            {
                Album toDelete = bdd.Albums.FirstOrDefault(a => a.Id.Equals(id));
                if (toDelete == null)
                    return 0;

                List<Photo> phToDelete = bdd.Photos.Where(p => p.Album.Id.Equals(id)).ToList();
                foreach(Photo ph in phToDelete)
                {
                    bdd.Photos.Remove(ph);
                    bdd.SaveChanges();
                }

                bdd.Albums.Remove(toDelete);

                return 1;
            }catch(Exception e)
            {
                Console.WriteLine("Erreur suppression album id : " + id + " -> " + e);
                return -1;
            }
        }
    }
}