using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PortailReserve.Utils.Logger;

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
                album.Creation = DateTime.Now;

                bdd.Albums.Add(album);
                bdd.SaveChanges();

                return album.Id;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur créer nouvel album -> " + e);
                return Guid.Empty;
            }
        }

        public void Dispose()
        {
            bdd.Dispose();
        }

        public bool ExisteParDossierEtCie(string dossier, int cie)
        {
            try
            {
                Album trouve = bdd.Albums.FirstOrDefault(a => a.Dossier.Equals(dossier) && a.Cie.Equals(cie));

                if (trouve.Equals(null))
                    return false;

                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public Album GetAlbumById(Guid id)
        {
            try
            {
                Album album = bdd.Albums.FirstOrDefault(a => a.Id.Equals(id));
                return album;
            }catch(NullReferenceException nfe)
            {
                Log("ERROR", "Aucun album trouve pour l'id : " + id + " -> " + nfe);
                return new AlbumNull() { Error = "Album introuvable." };
            }
            catch(Exception e)
            {
                Log("ERROR", "Erreur récupération albums id : " + id + " -> " + e);
                return null;
            }
        }

        public List<Album> GetAlbumsByCie(int cie)
        {
            try
            {
                List<Album> albums = bdd.Albums.Where(a => a.Cie.Equals(cie)).ToList();

                return albums;
            }
            catch(Exception e)
            {
                Log("ERROR", "Erreur récupération des albums de la cie : " + cie + " -> " + e);
                return new List<Album>();
            }
        }

        public int ModifierAlbum(Guid id, Album album)
        {
            try
            {
                Album toModify = GetAlbumById(id);
                if (toModify == null || toModify.Equals(typeof(AlbumNull)))
                    return 0;

                toModify.Titre = album.Titre;
                toModify.Description = album.Description;

                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur modiifer album id : " + id + " -> " + e);
                return -1;
            }
        }

        public int SupprimerAlbum(Guid id)
        {
            try
            {
                Album toDelete = GetAlbumById(id);
                if (toDelete == null || toDelete.Equals(typeof(AlbumNull)))
                    return 0;

                List<Photo> phToDelete = bdd.Photos.Where(p => p.Album.Equals(id)).ToList();
                foreach(Photo ph in phToDelete)
                {
                    bdd.Photos.Remove(ph);
                    bdd.SaveChanges();
                }

                bdd.Albums.Remove(toDelete);
                bdd.SaveChanges();

                return 1;
            }catch(Exception e)
            {
                Log("ERROR", "Erreur suppression album id : " + id + " -> " + e);
                return -1;
            }
        }
    }
}