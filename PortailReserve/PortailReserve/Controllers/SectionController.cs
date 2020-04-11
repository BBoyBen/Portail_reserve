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
        private IAdresseDal aDal;

        public SectionController()
        {
            uDal = new UtilisateurDal();
            gDal = new GroupeDal();
            sDal = new SectionDal();
            cDal = new CompagnieDal();
            aDal = new AdresseDal();
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
                return new HttpUnauthorizedResult();
            }
            if (u.Equals(typeof(UtilisateurNull)))
            {
                FormsAuthentication.SignOut();
                ViewBag.Erreur = ((UtilisateurNull)u).Error;
                return new HttpUnauthorizedResult();
            }
            if (u.PremiereCo)
                return new HttpUnauthorizedResult();

            Section userSection = sDal.GetSectionByNumAndByCie(u.Section, u.Compagnie);

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
                util = new Utilisateur { 
                    Id = Guid.Empty
                };

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

        [HttpPost]
        public ActionResult ModifierGrade()
        {
            try
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

                Guid idModif = Guid.Parse(Request.Form["idUtilChGrade"]);
                if (idModif.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400, "Id utilisateur changement de grade");

                string grade = Request.Form["Util.Grade"];

                int retour = uDal.ModifierGrade(idModif, grade);
                if (!retour.Equals(1))
                    return new HttpStatusCodeResult(400, "Une erreur est survenu lors du changement de grade.");

                return RedirectToAction("AfficherPersonnelSection");
            }
            catch(Exception e)
            {
                return new HttpStatusCodeResult(400, "Une erreur est survenu lors du changement de grade.");
            }
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
        [HttpPost]
        public ActionResult SupprimerUtilisateurSection()
        {
            try
            {
                Guid id = Guid.Parse(Request.Form["idUtil"]);
                if (id.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400, "Id pour suppresion vide");

                Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
                if (u == null)
                {
                    FormsAuthentication.SignOut();
                    return new HttpUnauthorizedResult();
                }
                if (u.Equals(typeof(UtilisateurNull)))
                {
                    FormsAuthentication.SignOut();
                    ViewBag.Erreur = ((UtilisateurNull)u).Error;
                    return new HttpUnauthorizedResult();
                }
                if (u.PremiereCo)
                    return new HttpUnauthorizedResult();

                if (u.Role > 3)
                    return new HttpUnauthorizedResult();

                int retour = uDal.SupprimerUtilisateurSection(id);

                if (retour != 1)
                    return new HttpStatusCodeResult(400, "Erreur de suppression du personnel.");

                return RedirectToAction("AfficherPersonnelSection");
            }
            catch(Exception e)
            {
                return new HttpStatusCodeResult(400, "Id utilisateur incorrect pour suppression");
            }
        }

        /***
         * Affichage de la pop-up de suppression d'un soldat d'une section
        ***/

        [Authorize]
        public ActionResult AfficherPopUpChgmtGroupe(Guid id)
        {
            Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);

            Utilisateur pourChange = uDal.GetUtilisateurById(id);
            if (pourChange == null)
                pourChange = new Utilisateur
                {
                    Id = Guid.Empty,
                    Prenom = "Empty",
                    Nom = "Empry",
                    Grade = "Soldat"
                };

            Section section = sDal.GetSectionByNumAndByCie(u.Section, u.Compagnie);
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
        [HttpPost]
        public ActionResult ChangerUtilisateurGroupe()
        {
            try
            {
                Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
                if (u == null)
                {
                    FormsAuthentication.SignOut();
                    return new HttpUnauthorizedResult();
                }
                if (u.Equals(typeof(UtilisateurNull)))
                {
                    FormsAuthentication.SignOut();
                    ViewBag.Erreur = ((UtilisateurNull)u).Error;
                    return new HttpUnauthorizedResult();
                }
                if (u.PremiereCo)
                    return new HttpUnauthorizedResult();

                if (u.Role > 3)
                    return new HttpUnauthorizedResult();

                Guid id = Guid.Parse(Request.Form["idUtilChgmnt"]);
                if (id.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400, "Id Utilisateur incorrect pour changement groupe.");

                Guid grp = Guid.Parse(Request.Form["nouveauGroupe"]);
                if (grp.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400, "Id groupe incorrect pour changement groupe.");

                int retour = uDal.ModifierGroupe(id, grp);

                if (retour != 1)
                    return new HttpNotFoundResult("Erreur changement de groupe.");

                return RedirectToAction("AfficherPersonnelSection");
            }
            catch(Exception e)
            {
                return new HttpStatusCodeResult(400, "Erreur de parsing des ids changement utilisateur.");
            }
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
                return new HttpUnauthorizedResult();
            }
            if (u.Equals(typeof(UtilisateurNull)))
            {
                FormsAuthentication.SignOut();
                ViewBag.Erreur = ((UtilisateurNull)u).Error;
                return new HttpUnauthorizedResult();
            }
            if (u.PremiereCo)
                return new HttpUnauthorizedResult();

            Section section = sDal.GetSectionByNumAndByCie(u.Section, u.Compagnie);

            List<Groupe> groupes = gDal.GetGroupesBySection(section.Id);
            List<SelectListItem> selectGroupe = new List<SelectListItem>();
            bool premier = true;
            foreach (Groupe g in groupes)
            {
                if (premier)
                {
                    selectGroupe.Add(new SelectListItem { Text = "Groupe " + g.Numero, Value = g.Id.ToString(), Selected = true });
                    premier = false;
                }
                else
                {
                    selectGroupe.Add(new SelectListItem { Text = "Groupe " + g.Numero, Value = g.Id.ToString() });
                }
            }

            List<Utilisateur> sansSection = uDal.GetUtilisateursSansSection();
            List<SelectListItem> selectSansSection = new List<SelectListItem>();
            selectSansSection.Add(new SelectListItem { Text = "--- Choix ---", Value = Guid.Empty.ToString(), Selected = true });
            foreach(Utilisateur util in sansSection)
            {
                selectSansSection.Add(new SelectListItem { Text = util.Grade + " " + util.Nom + " " + util.Prenom, Value = util.Id.ToString() });
            }

            List<SelectListItem> grades = new List<SelectListItem>();
            grades.Add(new SelectListItem { Text = "Soldat", Value = "Soldat", Selected = true });
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

        [HttpPost]
        [Authorize]
        public ActionResult AjouterPersonnel()
        {
            try
            {
                Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
                if (u == null)
                {
                    FormsAuthentication.SignOut();
                    return new HttpUnauthorizedResult();
                }
                if (u.Equals(typeof(UtilisateurNull)))
                {
                    FormsAuthentication.SignOut();
                    ViewBag.Erreur = ((UtilisateurNull)u).Error;
                    return new HttpUnauthorizedResult();
                }
                if (u.PremiereCo)
                    return new HttpUnauthorizedResult();

                if (u.Role > 3)
                    return new HttpUnauthorizedResult();

                int numSection = u.Section;
                int numCie = u.Compagnie;

                var groupe = Request.Form["selectGroupe"];
                Guid idGroupe = Guid.Parse(groupe);
                if (idGroupe.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400, "Erreur id groupe ajout utilisateur.");

                var creerNouveau = Request.Form["creerPersonnel"];
                if (creerNouveau != null && creerNouveau.Equals("on"))
                {
                    // Récupération des champs du formulaire
                    var grade = Request.Form["gradePersonne"];
                    var nom = Request.Form["nomPersonne"];
                    var prenom = Request.Form["prenomPersonne"];
                    var matricule = Request.Form["matriculePersonne"];
                    var mail = Request.Form["mailPersonne"];
                    var naissanceForm = Request.Form["naissancePersonne"];
                    var motDePasse = Request.Form["MotDePasse"];

                    DateTime naissance = DateTime.Parse(naissanceForm);

                    //Validation des valeurs
                    //TO-DO

                    // Création d'une adresse vide
                    Adresse adresse = new Adresse
                    {
                        CodePostal = "",
                        Pays = "France",
                        Ville = "",
                        Voie = ""
                    };
                    Guid idAdresse = aDal.AjouterAdresse(adresse);
                    if (idAdresse.Equals(Guid.Empty))
                        return new HttpNotFoundResult("Erreur ajout adresse pour nouvel utilisateur.");

                    //Création de l'utilisateur
                    Utilisateur pourAjout = new Utilisateur
                    {
                        Grade = grade,
                        Nom = nom,
                        Prenom = prenom,
                        Matricule = matricule,
                        Naissance = naissance,
                        Groupe = idGroupe,
                        Section = numSection,
                        Compagnie = numCie,
                        Email = mail,
                        Adresse = idAdresse,
                        Telephone = "",
                        MotDePasse = motDePasse,
                        PremiereCo = true,
                        Role = 4
                    };
                    Guid idAjout = uDal.AjouterUtilisateur(pourAjout);
                    if (idAjout.Equals(Guid.Empty))
                        return new HttpNotFoundResult("Erreur ajout nouvel utilisateur. ");
                }
                else
                {
                    var utilisateur = Request.Form["personnelExistant"];
                    Guid idUtil = Guid.Parse(utilisateur);
                    if (idUtil.Equals(Guid.Empty))
                    {
                        return new HttpStatusCodeResult(400, "Id utilisateur vide.");
                    }
                    else
                    {
                        Utilisateur pourModif = uDal.GetUtilisateurById(idUtil);
                        if (pourModif == null)
                            return new HttpNotFoundResult("Erreur récupération utilisateur à ajouter. ");

                        pourModif.Compagnie = numCie;
                        pourModif.Section = numSection;
                        pourModif.Groupe = idGroupe;

                        int retour = uDal.AjouterUtilisateurSection(idUtil, pourModif);
                        if (retour != 1)
                            return new HttpNotFoundResult("Erreur ajout utilisateur existant.");
                    }
                }

                return RedirectToAction("AfficherPersonnelSection");
            }
            catch(Exception e)
            {
                return new HttpStatusCodeResult(400, "Erreur de l'ajout d'un utilisateur.");
            }
        }

        /***
         * Affichage de la pop-up de changement de chef de groupe
        ***/

        [Authorize]
        public ActionResult AffciherPopUpChangementCdg(Guid id, Guid grp)
        {
            Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
            if (u == null)
            {
                FormsAuthentication.SignOut();
                return new HttpUnauthorizedResult();
            }
            if (u.Equals(typeof(UtilisateurNull)))
            {
                FormsAuthentication.SignOut();
                ViewBag.Erreur = ((UtilisateurNull)u).Error;
                return new HttpUnauthorizedResult();
            }
            if (u.PremiereCo)
                return new HttpUnauthorizedResult();

            Utilisateur ancienCdg = uDal.GetUtilisateurById(id);
            if (ancienCdg == null || ancienCdg.Equals(typeof(UtilisateurNull)))
                return new HttpNotFoundResult("Ancien chef de groupe introuvable");

            Section section = sDal.GetSectionByNumAndByCie(ancienCdg.Section, ancienCdg.Compagnie);
            Groupe groupe = gDal.GetGroupeById(grp);
            if (groupe == null || groupe.Equals(typeof(GroupeNull)))
                return new HttpNotFoundResult("Groupe non trouvé.");

            List<SelectListItem> grades = new List<SelectListItem>();
            grades.Add(new SelectListItem { Text = "Soldat", Value = "Soldat" });
            grades.Add(new SelectListItem { Text = "1ère classe", Value = "1ère classe" });
            grades.Add(new SelectListItem { Text = "Caporal", Value = "Caporal" });
            grades.Add(new SelectListItem { Text = "Caporal-chef", Value = "Caporal-chef", Selected = true });
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

            List<Utilisateur> sansSection = uDal.GetUtilisateursSansSection();
            List<SelectListItem> selectSansSection = new List<SelectListItem>();
            selectSansSection.Add(new SelectListItem { Text = "--- Choix ---", Value = Guid.Empty.ToString(), Selected = true });
            foreach (Utilisateur util in sansSection)
            {
                selectSansSection.Add(new SelectListItem { Text = util.Grade + " " + util.Nom + " " + util.Prenom, Value = util.Id.ToString() });
            }

            ChangementCdgViewModel vm = new ChangementCdgViewModel
            {
                AncienCdg = ancienCdg,
                Groupe = groupe,
                Section = section,
                Grades = grades,
                SansSection = selectSansSection,
                MotDePasse = "changeme"
            };

            return PartialView("AffciherPopUpChangementCdg", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangerCdg()
        {
            try
            {
                Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
                if (u == null)
                {
                    FormsAuthentication.SignOut();
                    return new HttpUnauthorizedResult();
                }
                if (u.Equals(typeof(UtilisateurNull)))
                {
                    FormsAuthentication.SignOut();
                    ViewBag.Erreur = ((UtilisateurNull)u).Error;
                    return new HttpUnauthorizedResult();
                }
                if (u.PremiereCo)
                    return new HttpUnauthorizedResult();

                if (u.Role > 3)
                    return new HttpUnauthorizedResult();

                int numSection = u.Section;
                int numCie = u.Compagnie;

                Guid ancienCdg = Guid.Parse(Request.Form["AncienCdg.Id"]);
                if (ancienCdg.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400, "Erreur sur l'ancien cdg.");

                Guid groupe = Guid.Parse(Request.Form["Groupe.Id"]);
                if (groupe.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400, "Erreur groupe.");

                Guid section = Guid.Parse(Request.Form["Section.Id"]);
                if (section.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400, "Erreur sur le section.");

                var creerNouveau = Request.Form["creerCdg"];
                if (creerNouveau != null && creerNouveau.Equals("on"))
                {
                    // Récupération des champs du formulaire
                    var grade = Request.Form["gradeCdg"];
                    var nom = Request.Form["nomCdg"];
                    var prenom = Request.Form["prenomCdg"];
                    var matricule = Request.Form["matriculeCdg"];
                    var mail = Request.Form["mailCdg"];
                    var naissanceForm = Request.Form["naissanceCdg"];
                    var motDePasse = Request.Form["MotDePasse"];

                    DateTime naissance = DateTime.Parse(naissanceForm);

                    //Validation des valeurs
                    //TO-DO

                    // Création d'une adresse vide
                    Adresse adresse = new Adresse
                    {
                        CodePostal = "",
                        Pays = "France",
                        Ville = "",
                        Voie = ""
                    };
                    Guid idAdresse = aDal.AjouterAdresse(adresse);
                    if (idAdresse.Equals(Guid.Empty))
                        return new HttpNotFoundResult("Erreur ajout adresse pour nouveau cdg.");

                    //Création de l'utilisateur
                    Utilisateur pourAjout = new Utilisateur
                    {
                        Grade = grade,
                        Nom = nom,
                        Prenom = prenom,
                        Matricule = matricule,
                        Naissance = naissance,
                        Groupe = Guid.Empty,
                        Section = numSection,
                        Compagnie = numCie,
                        Email = mail,
                        Adresse = idAdresse,
                        Telephone = "",
                        MotDePasse = motDePasse,
                        PremiereCo = true,
                        Role = 3
                    };
                    Guid idAjout = uDal.AjouterUtilisateur(pourAjout);
                    if (idAjout.Equals(Guid.Empty))
                        return new HttpNotFoundResult("Erreur ajout nouvel utilisateur. ");

                    int retour = gDal.ChangerCdg(groupe, idAjout);
                    if (retour != 1)
                        return new HttpStatusCodeResult(500, "Erreur changement de cdg.");

                    retour = uDal.SupprimerUtilisateurSection(ancienCdg);
                    if (retour != 1)
                        return new HttpStatusCodeResult(500, "Erreur suppresion d'ancien cdg");
                }
                else
                {
                    Guid nouveauCdg = Guid.Parse(Request.Form["cdgExistant"]);
                    if (nouveauCdg.Equals(Guid.Empty))
                        return new HttpStatusCodeResult(400, "Erreur sur le nouveau cdg.");

                    int retour = uDal.PasserCadre(nouveauCdg, numSection, numCie);
                    if (retour != 1)
                        return new HttpStatusCodeResult(500, "Erreur passage cadre.");

                    retour = uDal.SupprimerUtilisateurSection(ancienCdg);
                    if (retour != 1)
                        return new HttpStatusCodeResult(500, "Erreur suppression ancien cdg.");

                    retour = gDal.ChangerCdg(groupe, nouveauCdg);
                    if (retour != 1)
                        return new HttpStatusCodeResult(500, "Erreur changement de chef de groupe.");
                }

                    return RedirectToAction("AfficherPersonnelSection");
            }
            catch(Exception e)
            {
                return new HttpStatusCodeResult(400, "Erreur changement de chef de groupe.");
            }
        }
    }
}