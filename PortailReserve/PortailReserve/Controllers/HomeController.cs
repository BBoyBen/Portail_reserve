using PortailReserve.DAL;
using PortailReserve.DAL.Impl;
using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PortailReserve.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            IUtilisateurDal dal = new UtilisateurDal();
            Utilisateur u = new Utilisateur() {
                Nom = "Admin",
                Prenom = "Admin",
                Telephone = "0000000000",
                Email = "admin.admin@gmail.com",
                Matricule = "1763041044",
                MotDePasse = "admin",
                Role = 1,
                Id_Adresse = 1,
                Id_Groupe = 1,
                PremiereCo = true
            };

            long id = dal.AjouterUtilisateur(u);

            Utilisateur user = dal.GetUtilisateurById(id);
            return View(user);
        }

    }
}