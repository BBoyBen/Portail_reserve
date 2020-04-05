using PortailReserve.DAL;
using PortailReserve.DAL.Impl;
using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using PortailReserve.ViewModel;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;
using static PortailReserve.Utils.Utils;

namespace PortailReserve.Controllers
{
    public class SectionController : Controller
    {

        private IUtilisateurDal uDal;
        private IGroupeDal gDal;
        private ISectionDal sDal;
        private ICompagnieDal cDal;

        public SectionController()
        {
            uDal = new UtilisateurDal();
            gDal = new GroupeDal();
            sDal = new SectionDal();
            cDal = new CompagnieDal();
        }

        [Authorize]
        public ActionResult Index()
        {
            Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
            if (u == null)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Index", "Login");
            }
            if (u.Equals(typeof(UtilisateurNull)))
            {
                FormsAuthentication.SignOut();
                ViewBag.Erreur = ((UtilisateurNull)u).Error;
                return RedirectToAction("Index", "Login");
            }
            if (u.PremiereCo)
                return RedirectToAction("PremiereCo", "Login");

            ViewBag.Grade = u.Grade;
            ViewBag.Nom = u.Nom.ToUpperInvariant();
            ViewBag.Role = u.Role;
            
            return View(u);
        }

        [Authorize]
        public ActionResult AfficherPersonnelSection()
        {
            Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
            if (u == null)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Index", "Login");
            }
            if (u.Equals(typeof(UtilisateurNull)))
            {
                FormsAuthentication.SignOut();
                ViewBag.Erreur = ((UtilisateurNull)u).Error;
                return RedirectToAction("Index", "Login");
            }
            if (u.PremiereCo)
                return RedirectToAction("PremiereCo", "Login");

            ViewBag.Grade = u.Grade;
            ViewBag.Nom = u.Nom.ToUpperInvariant();
            ViewBag.Role = u.Role;

            Groupe userGrp = gDal.GetGroupeById(u.Groupe);
            Section userSection = sDal.GetSectionById(userGrp.Section);

            List<Groupe> grpSection = gDal.GetGroupesBySection(userSection.Id);
            grpSection = TrierGroupes(grpSection);

            List<Utilisateur> listCdg = new List<Utilisateur>();
            List<Utilisateur> listSdt = new List<Utilisateur>();
            foreach (Groupe g in grpSection)
            {
                listCdg.Add(uDal.GetUtilisateurById(g.CDG));
                listSdt.AddRange(uDal.GetUtilisateursByGroupe(g.Id));
            }

            Compagnie cie = cDal.GetCompagnieById(userSection.Compagnie);

            SectionViewModel vm = new SectionViewModel()
            {
                Cie = cie,
                Section = userSection,
                CDS = uDal.GetUtilisateurById(userSection.CDS),
                SOA = uDal.GetUtilisateurById(userSection.SOA),
                Groupes = grpSection,
                CDGs = listCdg,
                Soldats = listSdt,
                Util = u
            };

            return PartialView("AfficherPersonnelSection", vm);
        }

        [Authorize]
        public ActionResult AfficherGrade(Guid id)
        {
            Utilisateur util = uDal.GetUtilisateurById(id);
            if (util == null)
                util = new Utilisateur();

            string grade = util.Grade;

            return PartialView("AfficherGrade", grade);
        }

        [Authorize]
        public ActionResult AfficherSelectGrade(Guid id)
        {
            Utilisateur util = uDal.GetUtilisateurById(id);
            if (util == null)
                util = new Utilisateur();

            List<SelectListItem> grades = new List<SelectListItem>();
            grades.Add(new SelectListItem { Text = "Soldat", Value = "Soldat" });
            grades.Add(new SelectListItem { Text = "1ère classe", Value = "1ère classe" });
            grades.Add(new SelectListItem { Text = "Caporal", Value = "Caporal" });
            grades.Add(new SelectListItem { Text = "Caporal-chef", Value = "Caporal-chef" });
            grades.Add(new SelectListItem { Text = "Caporal-chef de 1ère classe", Value = "Caporal-chef de 1ère classe" });

            grades.Add(new SelectListItem { Text = "Sergent", Value = "Sergent" });
            grades.Add(new SelectListItem { Text = "Sergent-chef", Value = "Sergent-chef" });
            grades.Add(new SelectListItem { Text = "Adjudant", Value = "Adjudant" });
            grades.Add(new SelectListItem { Text = "Adjudant-chef", Value = "Adjudant-chef" });
            grades.Add(new SelectListItem { Text = "Major", Value = "Major" });

            grades.Add(new SelectListItem { Text = "Sous-lieutenant", Value = "Sous-lieutenant" });
            grades.Add(new SelectListItem { Text = "Lieutenant", Value = "Lieutenant" });
            grades.Add(new SelectListItem { Text = "Capitaine", Value = "Capitaine" });
            grades.Add(new SelectListItem { Text = "Commandant", Value = "Commandant" });
            grades.Add(new SelectListItem { Text = "Lieutenant-colonel", Value = "Lieutenant-colonel" });
            grades.Add(new SelectListItem { Text = "Colonel", Value = "Colonel" });

            SelectGradeViewModel vm = new SelectGradeViewModel
            {
                Util = util,
                Grades = grades
            };

            return PartialView("AfficherSelectGrade", vm);
        }

        public ActionResult ModifierGrade(Guid idModif, string grade)
        {
            Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
            if (u == null)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("AfficherGrade", "Section");
            }
            if (u.Equals(typeof(UtilisateurNull)))
            {
                FormsAuthentication.SignOut();
                ViewBag.Erreur = ((UtilisateurNull)u).Error;
                return RedirectToAction("AfficherGrade", "Section");
            }

            if (u.Role > 3)
                return RedirectToAction("AfficherGrade", "Section");

            int retour = uDal.ModifierGrade(idModif, grade);
            if (!retour.Equals(1))
                ViewBag.Erreur = "Une erreur est survenu lors du changement de grade.";

            return RedirectToAction("AfficherGrade", new { id = idModif });
        }

        /***
         * Affichage de la pop-up de suppression d'un soldat d'une section
        ***/
        [Authorize]
        public ActionResult AfficherPopUpSuppSoldat(Guid id)
        {
            Utilisateur util = uDal.GetUtilisateurById(id);
            if (util == null)
                util = new Utilisateur
                {
                    Id = Guid.Empty,
                    Prenom = "Empty",
                    Nom = "Empry",
                    Grade = "Soldat"
                };

            return PartialView("AfficherPopUpSuppSoldat", util);
        }

        [Authorize]
        public ActionResult SupprimerUtilisateurSection(Guid id)
        {
            Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
            if (u == null)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Index", "Login");
            }
            if (u.Equals(typeof(UtilisateurNull)))
            {
                FormsAuthentication.SignOut();
                ViewBag.Erreur = ((UtilisateurNull)u).Error;
                return RedirectToAction("Index", "Login");
            }
            if (u.PremiereCo)
                return RedirectToAction("PremiereCo", "Login");

            if (u.Role > 3)
                return RedirectToAction("Index", "Section");

            ViewBag.Grade = u.Grade;
            ViewBag.Nom = u.Nom.ToUpperInvariant();
            ViewBag.Role = u.Role;

            int retour = uDal.SupprimerUtilisateurSection(id);

            if (retour != 0)
                ViewBag.Erreur = "Une erreur s'est produite lors de la suppression.";

            return RedirectToAction("AfficherPersonnelSection");
        }

        /***
         * Affichage de la pop-up de suppression d'un soldat d'une section
        ***/

        [Authorize]
        public ActionResult AfficherPopUpChgmtGroupe(Guid id)
        {
            Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
            if (u == null)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Index", "Login");
            }
            if (u.Equals(typeof(UtilisateurNull)))
            {
                FormsAuthentication.SignOut();
                ViewBag.Erreur = ((UtilisateurNull)u).Error;
                return RedirectToAction("Index", "Login");
            }
            if (u.PremiereCo)
                return RedirectToAction("PremiereCo", "Login");

            if (u.Role > 3)
                return RedirectToAction("Index", "Section");

            ViewBag.Grade = u.Grade;
            ViewBag.Nom = u.Nom.ToUpperInvariant();
            ViewBag.Role = u.Role;

            Utilisateur pourChange = uDal.GetUtilisateurById(id);
            if (pourChange == null)
                pourChange = new Utilisateur
                {
                    Id = Guid.Empty,
                    Prenom = "Empty",
                    Nom = "Empry",
                    Grade = "Soldat"
                };

            Section section = sDal.GetSectionById(gDal.GetGroupeById(u.Groupe).Section);
            List<Groupe> groupes = gDal.GetGroupesBySection(section.Id);

            List<SelectListItem> selectGroupe = new List<SelectListItem>();
            foreach(Groupe g in groupes)
            {
                if(g.Id.Equals(pourChange.Groupe))
                    selectGroupe.Add(new SelectListItem { Text = "Groupe " + g.Numero, Value = g.Id.ToString(), Selected = true });
                else
                    selectGroupe.Add(new SelectListItem { Text = "Groupe " + g.Numero, Value = g.Id.ToString() });
            }

            ChangementGroupeViewModel vm = new ChangementGroupeViewModel
            {
                Util = pourChange,
                Groupe = selectGroupe
            };

            return PartialView("AfficherPopUpChgmtGroupe", vm);
        }

        [Authorize]
        public ActionResult ChangerUtilisateurGroupe(Guid id, Guid grp)
        {
            Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
            if (u == null)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Index", "Login");
            }
            if (u.Equals(typeof(UtilisateurNull)))
            {
                FormsAuthentication.SignOut();
                ViewBag.Erreur = ((UtilisateurNull)u).Error;
                return RedirectToAction("Index", "Login");
            }
            if (u.PremiereCo)
                return RedirectToAction("PremiereCo", "Login");

            if (u.Role > 3)
                return RedirectToAction("Index", "Section");

            ViewBag.Grade = u.Grade;
            ViewBag.Nom = u.Nom.ToUpperInvariant();
            ViewBag.Role = u.Role;

            int retour = uDal.ModifierGroupe(id, grp);

            if (retour != 1)
                ViewBag.Erreur = "Une erreur est survenue lors du changement de groupe.";

            return RedirectToAction("AfficherPersonnelSection");
        }

        /***
         * Affichage de la pop-up de ajout d'un personel
        ***/

        [Authorize]
        public ActionResult AfficherPopUpAjouterPerso()
        {
            Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
            if (u == null)
            {
                FormsAuthentication.SignOut();
                return RedirectToAction("Index", "Login");
            }
            if (u.Equals(typeof(UtilisateurNull)))
            {
                FormsAuthentication.SignOut();
                ViewBag.Erreur = ((UtilisateurNull)u).Error;
                return RedirectToAction("Index", "Login");
            }
            if (u.PremiereCo)
                return RedirectToAction("PremiereCo", "Login");

            if (u.Role > 3)
                return RedirectToAction("Index", "Section");

            ViewBag.Grade = u.Grade;
            ViewBag.Nom = u.Nom.ToUpperInvariant();
            ViewBag.Role = u.Role;

            Groupe groupe = gDal.GetGroupeById(u.Groupe);
            Section section = sDal.GetSectionById(groupe.Section);

            List<Groupe> groupes = gDal.GetGroupesBySection(section.Id);
            List<SelectListItem> selectGroupe = new List<SelectListItem>();
            foreach(Groupe g in groupes)
            {
                selectGroupe.Add(new SelectListItem { Text = "Groupe " + g.Numero, Value = g.Id.ToString() });
            }

            List<Utilisateur> sansSection = uDal.GetUtilisateursSansSection();
            List<SelectListItem> selectSansSection = new List<SelectListItem>();
            selectSansSection.Add(new SelectListItem { Text = "--- Choix ---", Value = Guid.Empty.ToString() });
            foreach(Utilisateur util in sansSection)
            {
                selectSansSection.Add(new SelectListItem { Text = util.Grade + " " + util.Nom + " " + util.Prenom, Value = util.Id.ToString() });
            }

            List<SelectListItem> grades = new List<SelectListItem>();
            grades.Add(new SelectListItem { Text = "Soldat", Value = "Soldat" });
            grades.Add(new SelectListItem { Text = "1ère classe", Value = "1ère classe" });
            grades.Add(new SelectListItem { Text = "Caporal", Value = "Caporal" });
            grades.Add(new SelectListItem { Text = "Caporal-chef", Value = "Caporal-chef" });
            grades.Add(new SelectListItem { Text = "Caporal-chef de 1ère classe", Value = "Caporal-chef de 1ère classe" });

            grades.Add(new SelectListItem { Text = "Sergent", Value = "Sergent" });
            grades.Add(new SelectListItem { Text = "Sergent-chef", Value = "Sergent-chef" });
            grades.Add(new SelectListItem { Text = "Adjudant", Value = "Adjudant" });
            grades.Add(new SelectListItem { Text = "Adjudant-chef", Value = "Adjudant-chef" });
            grades.Add(new SelectListItem { Text = "Major", Value = "Major" });

            grades.Add(new SelectListItem { Text = "Sous-lieutenant", Value = "Sous-lieutenant" });
            grades.Add(new SelectListItem { Text = "Lieutenant", Value = "Lieutenant" });
            grades.Add(new SelectListItem { Text = "Capitaine", Value = "Capitaine" });
            grades.Add(new SelectListItem { Text = "Commandant", Value = "Commandant" });
            grades.Add(new SelectListItem { Text = "Lieutenant-colonel", Value = "Lieutenant-colonel" });
            grades.Add(new SelectListItem { Text = "Colonel", Value = "Colonel" });

            string nouveauMdp = "changeme";

            AjouterPersonnelViewModel vm = new AjouterPersonnelViewModel
            {
                Groupes = selectGroupe,
                SansSection = selectSansSection,
                Grades = grades,
                MotDePasse = nouveauMdp
            };

            return PartialView("AfficherPopUpAjouterPerso", vm);
        }
    }
}