using PortailReserve.Models;
using PortailReserve.Utils;
using System;
using System.Linq;

namespace PortailReserve.DAL.Impl
{
    public class LectureDal : ILectureDal
    {
        private BddContext bdd;
        private readonly Logger LOGGER;

        public LectureDal()
        {
            bdd = new BddContext();
            LOGGER = new Logger(this.GetType());
        }

        public Guid AjouterLecture(Guid util, Guid message)
        {
            try
            {
                Lecture lecture = new Lecture { Message = message, Utilisateur = util };

                bdd.Lectures.Add(lecture);
                bdd.SaveChanges();

                return lecture.Id;
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur lors de l'ajout d'une lecture -> " + e);
                return Guid.Empty;
            }
        }

        public Lecture GetLectureByMessageAndByUtil(Guid message, Guid util)
        {
            try
            {
                Lecture lecture = bdd.Lectures.FirstOrDefault(l => l.Message.Equals(message) && l.Utilisateur.Equals(util));

                return lecture;
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur récupération de la lecture pour un message -> " + e);
                return null;
            }
        }
    }
}