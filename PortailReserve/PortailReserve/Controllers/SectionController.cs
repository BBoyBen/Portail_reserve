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
using static PortailReserve.Utils.Logger;

namespace PortailReserve.Controllers
{
    public class SectionController : Controller
    {

        private IUtilisateurDal uDal;
        private IGroupeDal gDal;
        private ISectionDal sDal;
        private ICompagnieDal cDal;
        private IAdresseDal aDal;

        private List<SelectListItem> grades;
        private List<SelectListItem> selectSansSection;
        private List<SelectListItem> roles;

        public SectionController()
        {
            uDal = new UtilisateurDal();
            gDal = new GroupeDal();
            sDal = new SectionDal();
            cDal = new CompagnieDal();
            aDal = new AdresseDal();

            grades = new List<SelectListItem>();
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

            roles = new List<SelectListItem>();
            roles.Add(new SelectListItem { Text = "Personnel classique", Value = "4", Selected = true });
            roles.Add(new SelectListItem { Text = "Gestion des groupes", Value = "3" });
            roles.Add(new SelectListItem { Text = "Gestion de section", Value = "2" });

            List<Utilisateur> sansSection = uDal.GetUtilisateursSansSection();
            selectSansSection = new List<SelectListItem>();
            selectSansSection.Add(new SelectListItem { Text = "--- Choix ---", Value = Guid.Empty.ToString(), Selected = true });
            foreach (Utilisateur util in sansSection)
            {
                selectSansSection.Add(new SelectListItem { Text = util.Grade + " " + util.Nom + " " + util.Prenom, Value = util.Id.ToString() });
            }
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

            if (u.Section == -1 && u.Compagnie == -1)
                return PartialView("AfficherSansSection");

            if (u.Section == 0)
                return RedirectToAction("AfficherPersonnelSectionCom");

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

            Utilisateur soa = uDal.GetUtilisateurById(userSection.SOA);
            if (soa == null || soa.Equals(typeof(UtilisateurNull)))
                soa = new Utilisateur
                {
                    Id = Guid.Empty,
                    Nom = "",
                    Prenom = "",
                    Grade = ""
                };

            Utilisateur cds = uDal.GetUtilisateurById(userSection.CDS);
            if (cds == null || cds.Equals(typeof(UtilisateurNull)))
                cds = new Utilisateur
                {
                    Id = Guid.Empty,
                    Nom = "",
                    Prenom = "",
                    Grade = ""
                };

            SectionViewModel vm = new SectionViewModel()
            {
                Cie = cie,
                Section = userSection,
                CDS = cds,
                SOA = soa,
                Groupes = grpSection,
                CDGs = listCdg,
                Soldats = listSdt,
                Util = u
            };

            return PartialView("AfficherPersonnelSection", vm);
        }

        [Authorize]
        public ActionResult AfficherPersonnelSectionCom()
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

            if (u.Section == -1 && u.Compagnie == -1)
                return PartialView("AfficherSansSection");

            if (u.Section != 0)
                return RedirectToAction("AfficherPersonnelSection");

            Section userSection = sDal.GetSectionByNumAndByCie(u.Section, u.Compagnie);
            Compagnie cie = cDal.GetCompagnieById(userSection.Compagnie);

            Utilisateur cdu = uDal.GetUtilisateurById(cie.CDU);
            if (cdu == null || cdu.Equals(typeof(UtilisateurNull)))
                cdu = new Utilisateur
                {
                    Nom = "",
                    Prenom = "",
                    Grade = "",
                    Id = Guid.Empty
                };

            Utilisateur adu = uDal.GetUtilisateurById(cie.ADU);
            if (adu == null || adu.Equals(typeof(UtilisateurNull)))
                adu = new Utilisateur
                {
                    Nom = "",
                    Prenom = "",
                    Grade = "",
                    Id = Guid.Empty
                };

            List<Utilisateur> utilCom = uDal.GetUtilisateursBySectionByGroupe(u.Section, u.Compagnie);
            List<Utilisateur> toSupp = new List<Utilisateur>();
            foreach(Utilisateur us in utilCom)
            {
                if (us.Id.Equals(cdu.Id) || us.Id.Equals(adu.Id))
                    toSupp.Add(us);
            }
            foreach(Utilisateur i in toSupp)
            {
                utilCom.Remove(i);
            }

            SectionCommandementViewModel vm = new SectionCommandementViewModel
            {
                Util = u,
                CDU = cdu,
                ADU = adu,
                UtilCmdt = utilCom,
                Section = userSection,
                Cie = cie
            };

            return PartialView("AfficherPersonnelSectionCom", vm);
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

            SelectGradeViewModel vm = new SelectGradeViewModel
            {
                Util = util,
                Grades = this.grades
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
                    Nom = "Empty",
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
         * Affichage de la pop-up de changement de groupe d'un soldat
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
            groupes = TrierGroupes(groupes);

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
            groupes = TrierGroupes(groupes);
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

            string nouveauMdp = GenererMotDePasse();

            AjouterPersonnelViewModel vm = new AjouterPersonnelViewModel
            {
                Groupes = selectGroupe,
                SansSection = this.selectSansSection,
                Grades = this.grades,
                Roles = this.roles,
                MotDePasse = nouveauMdp,
                NumSection = u.Section
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
                if (idGroupe.Equals(Guid.Empty) && numSection != 0)
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
                    var role = Int32.Parse(Request.Form["rolePersonne"]);

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
                        Role = role
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
        public ActionResult AfficherPopUpChangementCdg(Guid id, Guid grp)
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

            Guid ancienCdg = id;

            Section section = sDal.GetSectionByNumAndByCie(u.Section, u.Compagnie);
            Groupe groupe = gDal.GetGroupeById(grp);
            if (groupe == null || groupe.Equals(typeof(GroupeNull)))
                return new HttpNotFoundResult("Groupe non trouvé.");

            ChangementCdgViewModel vm = new ChangementCdgViewModel
            {
                AncienCdg = ancienCdg,
                Groupe = groupe,
                Section = section,
                Grades = this.grades,
                Roles = this.roles,
                SansSection = this.selectSansSection,
                MotDePasse = GenererMotDePasse()
            };

            return PartialView("AfficherPopUpChangementCdg", vm);
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

                Guid ancienCdg = Guid.Parse(Request.Form["AncienCdg"]);

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
                    var role = Int32.Parse(Request.Form["roleCdg"]);

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
                        Role = role
                    };
                    Guid idAjout = uDal.AjouterUtilisateur(pourAjout);
                    if (idAjout.Equals(Guid.Empty))
                        return new HttpNotFoundResult("Erreur ajout nouvel utilisateur. ");

                    int retour = gDal.ChangerCdg(groupe, idAjout);
                    if (retour != 1)
                        return new HttpStatusCodeResult(500, "Erreur changement de cdg.");

                    if (!ancienCdg.Equals(Guid.Empty))
                    {
                        retour = uDal.SupprimerUtilisateurSection(ancienCdg);
                        if (retour != 1)
                            return new HttpStatusCodeResult(500, "Erreur suppresion d'ancien cdg");
                    }
                }
                else
                {
                    Guid nouveauCdg = Guid.Parse(Request.Form["cdgExistant"]);
                    if (nouveauCdg.Equals(Guid.Empty))
                        return new HttpStatusCodeResult(400, "Erreur sur le nouveau cdg.");

                    int retour = uDal.PasserCadre(nouveauCdg, numSection, numCie);
                    if (retour != 1)
                        return new HttpStatusCodeResult(500, "Erreur passage cadre.");

                    if (!ancienCdg.Equals(Guid.Empty))
                    {
                        retour = uDal.SupprimerUtilisateurSection(ancienCdg);
                        if (retour != 1)
                            return new HttpStatusCodeResult(500, "Erreur suppression ancien cdg.");
                    }

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

        /***
         * Affichage de la pop-up de suppression de chef de groupe
        ***/

        [Authorize]
        public ActionResult AfficherPopUpSuppCdg(Guid id, Guid grp)
        {
            Utilisateur cdg = uDal.GetUtilisateurById(id);
            if (cdg == null || cdg.Equals(typeof(UtilisateurNull)))
                cdg = new Utilisateur
                {
                    Id = Guid.Empty,
                    Nom = "Empty",
                    Prenom = "Empty",
                    Grade = "Empty"
                };

            Groupe groupe = gDal.GetGroupeById(grp);
            if (groupe == null || groupe.Equals(typeof(GroupeNull)))
                groupe = new Groupe
                {
                    Numero = -1,
                    Id = Guid.Empty
                };

            SuppressionCdgViewModel vm = new SuppressionCdgViewModel
            {
                Groupe = groupe,
                AncienCdg = cdg
            };

            return PartialView("AfficherPopUpSuppCdg", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SupprimerCdgSection()
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

                Guid idAncienCdg = Guid.Parse(Request.Form["idAncienCdg"]);
                if (idAncienCdg.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400, "Erreur id ancien cdg.");

                Guid idGroupe = Guid.Parse(Request.Form["idGroupe"]);
                if (idGroupe.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400, "Erreur id groupe.");

                int retour = gDal.ChangerCdg(idGroupe, Guid.Empty);
                if (retour != 1)
                    return new HttpStatusCodeResult(400, "Erreur de changer cdg groupe.");

                retour = uDal.SupprimerUtilisateurSection(idAncienCdg);
                if (retour != 1)
                    return new HttpStatusCodeResult(400, "Erreur supp cdg de la section");


                return RedirectToAction("AfficherPersonnelSection");
            }
            catch(Exception e)
            {
                return new HttpStatusCodeResult(400, "Erreur suppression du chef de groupe");
            }
        }

        /***
         * Affichage de la pop-up de changement de SOA
        ***/

        [Authorize]
        public ActionResult AfficherPopUpChgmntSoa(Guid id, Guid idSection)
        {
            Utilisateur soa = uDal.GetUtilisateurById(id);
            if (soa == null || soa.Equals(typeof(UtilisateurNull)))
                soa = new Utilisateur
                {
                    Grade = "Soldat",
                    Nom = "Empty",
                    Prenom = "Empty",
                    Id = Guid.Empty
                };

            Section section = sDal.GetSectionById(idSection);
            if (section == null || section.Equals(typeof(SectionNull)))
                section = new Section
                {
                    Id = Guid.Empty,
                    Numero = -1
                };

            ChangementSoaViewModel vm = new ChangementSoaViewModel
            {
                Section = section,
                AncienSoa = soa,
                SansSection = this.selectSansSection,
                Grades = this.grades,
                Roles = this.roles,
                MotDePasse = GenererMotDePasse()
            };

            return PartialView("AfficherPopUpChgmntSoa", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangementSoa()
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

                if (u.Role > 2)
                    return new HttpUnauthorizedResult();

                int numSection = u.Section;
                int numCie = u.Compagnie;

                Guid ancienSoa = Guid.Parse(Request.Form["AncienSoa.Id"]);

                Guid section = Guid.Parse(Request.Form["Section.Id"]);
                if (section.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400, "Erreur sur le section.");

                var creerNouveau = Request.Form["creerSoa"];
                if (creerNouveau != null && creerNouveau.Equals("on"))
                {
                    // Récupération des champs du formulaire
                    var grade = Request.Form["gradeSoa"];
                    var nom = Request.Form["nomSoa"];
                    var prenom = Request.Form["prenomSoa"];
                    var matricule = Request.Form["matriculeSoa"];
                    var mail = Request.Form["mailSoa"];
                    var naissanceForm = Request.Form["naissanceSoa"];
                    var motDePasse = Request.Form["MotDePasse"];
                    var role = Int32.Parse(Request.Form["roleSoa"]);

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
                        return new HttpNotFoundResult("Erreur ajout adresse pour nouveau soa.");

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
                        Role = role
                    };
                    Guid idAjout = uDal.AjouterUtilisateur(pourAjout);
                    if (idAjout.Equals(Guid.Empty))
                        return new HttpNotFoundResult("Erreur ajout nouvel utilisateur.");

                    int retour = sDal.ChangerSoa(section, idAjout);
                    if (retour != 1)
                        return new HttpStatusCodeResult(500, "Erreur changement de soa.");

                    if (!ancienSoa.Equals(Guid.Empty))
                    {
                        retour = uDal.SupprimerUtilisateurSection(ancienSoa);
                        if (retour != 1)
                            return new HttpStatusCodeResult(500, "Erreur suppresion d'ancien soa");
                    }
                }
                else
                {
                    Guid nouveauSoa = Guid.Parse(Request.Form["soaExistant"]);
                    if (nouveauSoa.Equals(Guid.Empty))
                        return new HttpStatusCodeResult(400, "Erreur sur le nouveau soa.");

                    int retour = uDal.PasserCadre(nouveauSoa, numSection, numCie);
                    if (retour != 1)
                        return new HttpStatusCodeResult(500, "Erreur passage cadre.");

                    if (!ancienSoa.Equals(Guid.Empty))
                    {
                        retour = uDal.SupprimerUtilisateurSection(ancienSoa);
                        if (retour != 1)
                            return new HttpStatusCodeResult(500, "Erreur suppression ancien soa.");
                    }

                    retour = sDal.ChangerSoa(section, nouveauSoa);
                    if (retour != 1)
                        return new HttpStatusCodeResult(500, "Erreur changement de sous-officier adjoint.");
                }

                return RedirectToAction("AfficherPersonnelSection");
            }
            catch(Exception e)
            {
                return new HttpStatusCodeResult(400, "Erreur lors du changement de SOA");
            }
        }

        /***
         * Affichage de la pop-up de suppression de SOA
        ***/

        [Authorize]
        public ActionResult AfficherPopUpSuppressionSoa(Guid id, Guid idSection)
        {
            Utilisateur soa = uDal.GetUtilisateurById(id);
            if (soa == null || soa.Equals(typeof(UtilisateurNull)))
                soa = new Utilisateur
                {
                    Nom = "Empty",
                    Prenom = "Empty",
                    Grade = "Soldat",
                    Id = Guid.Empty
                };

            Section section = sDal.GetSectionById(idSection);
            if (section == null || section.Equals(typeof(SectionNull)))
                section = new Section
                {
                    Id = Guid.Empty
                };

            SuppressionSoaViewModel vm = new SuppressionSoaViewModel
            {
                Soa = soa,
                Section = section
            };

            return PartialView("AfficherPopUpSuppressionSoa", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SupprimerSoa()
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

                if (u.Role > 2)
                    return new HttpUnauthorizedResult();

                Guid idAncienSoa = Guid.Parse(Request.Form["idAncienSoa"]);
                if (idAncienSoa.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400, "Erreur id ancien soa.");

                Guid idSection = Guid.Parse(Request.Form["idSection"]);
                if (idSection.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400, "Erreur id groupe.");

                int retour = sDal.ChangerSoa(idSection, Guid.Empty);
                if (retour != 1)
                    return new HttpStatusCodeResult(400, "Erreur de changer soa.");

                retour = uDal.SupprimerUtilisateurSection(idAncienSoa);
                if (retour != 1)
                    return new HttpStatusCodeResult(400, "Erreur supp soa de la section");

                return RedirectToAction("AfficherPersonnelSection");
            }
            catch(Exception e)
            {
                return new HttpStatusCodeResult(400, "Erreur suppression SOA.");
            }
        }

        /***
         * Affichage de la pop-up de changement de CDS
        ***/

        [Authorize]
        public ActionResult AfficherPopUpChgmntCds(Guid id, Guid idSection)
        {
            Utilisateur cds = uDal.GetUtilisateurById(id);
            if (cds == null || cds.Equals(typeof(UtilisateurNull)))
                cds = new Utilisateur
                {
                    Grade = "Soldat",
                    Nom = "Empty",
                    Prenom = "Empty",
                    Id = Guid.Empty
                };

            Section section = sDal.GetSectionById(idSection);
            if (section == null || section.Equals(typeof(SectionNull)))
                section = new Section
                {
                    Id = Guid.Empty,
                    Numero = -1
                };

            ChangementSoaViewModel vm = new ChangementSoaViewModel
            {
                Section = section,
                AncienSoa = cds,
                SansSection = this.selectSansSection,
                Grades = this.grades,
                Roles = this.roles,
                MotDePasse = GenererMotDePasse()
            };

            return PartialView("AfficherPopUpChgmntCds", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangementCds()
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

                if (u.Role > 2)
                    return new HttpUnauthorizedResult();

                int numSection = u.Section;
                int numCie = u.Compagnie;

                Guid ancienCds = Guid.Parse(Request.Form["AncienSoa.Id"]);

                Guid section = Guid.Parse(Request.Form["Section.Id"]);
                if (section.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400, "Erreur sur le section.");

                var creerNouveau = Request.Form["creerCds"];
                if (creerNouveau != null && creerNouveau.Equals("on"))
                {
                    // Récupération des champs du formulaire
                    var grade = Request.Form["gradeCds"];
                    var nom = Request.Form["nomCds"];
                    var prenom = Request.Form["prenomCds"];
                    var matricule = Request.Form["matriculeCds"];
                    var mail = Request.Form["mailCds"];
                    var naissanceForm = Request.Form["naissanceCds"];
                    var motDePasse = Request.Form["MotDePasse"];
                    var role = Int32.Parse(Request.Form["roleCds"]);

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
                        return new HttpNotFoundResult("Erreur ajout adresse pour nouveau cds.");

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
                        Role = role
                    };
                    Guid idAjout = uDal.AjouterUtilisateur(pourAjout);
                    if (idAjout.Equals(Guid.Empty))
                        return new HttpNotFoundResult("Erreur ajout nouvel utilisateur. ");

                    int retour = sDal.ChangerCds(section, idAjout);
                    if (retour != 1)
                        return new HttpStatusCodeResult(500, "Erreur changement de cds.");

                    if (!ancienCds.Equals(Guid.Empty))
                    {
                        retour = uDal.SupprimerUtilisateurSection(ancienCds);
                        if (retour != 1)
                            return new HttpStatusCodeResult(500, "Erreur suppresion d'ancien cds");
                    }
                }
                else
                {
                    Guid nouveauCds = Guid.Parse(Request.Form["cdsExistant"]);
                    if (nouveauCds.Equals(Guid.Empty))
                        return new HttpStatusCodeResult(400, "Erreur sur le nouveau cds.");

                    int retour = uDal.PasserCadre(nouveauCds, numSection, numCie);
                    if (retour != 1)
                        return new HttpStatusCodeResult(500, "Erreur passage cadre.");

                    if (!ancienCds.Equals(Guid.Empty))
                    {
                        retour = uDal.SupprimerUtilisateurSection(ancienCds);
                        if (retour != 1)
                            return new HttpStatusCodeResult(500, "Erreur suppression ancien cds.");
                    }

                    retour = sDal.ChangerCds(section, nouveauCds);
                    if (retour != 1)
                        return new HttpStatusCodeResult(500, "Erreur changement de chef de section.");
                }

                return RedirectToAction("AfficherPersonnelSection");
            }
            catch (Exception e)
            {
                return new HttpStatusCodeResult(400, "Erreur lors du changement de CDS");
            }
        }

        /***
         * Affichage de la pop-up de suppression de CDS
        ***/

        [Authorize]
        public ActionResult AfficherPopUpSuppressionCds(Guid id, Guid idSection)
        {
            Utilisateur cds = uDal.GetUtilisateurById(id);
            if (cds == null || cds.Equals(typeof(UtilisateurNull)))
                cds = new Utilisateur
                {
                    Nom = "Empty",
                    Prenom = "Empty",
                    Grade = "Soldat",
                    Id = Guid.Empty
                };

            Section section = sDal.GetSectionById(idSection);
            if (section == null || section.Equals(typeof(SectionNull)))
                section = new Section
                {
                    Id = Guid.Empty
                };

            SuppressionSoaViewModel vm = new SuppressionSoaViewModel
            {
                Soa = cds,
                Section = section
            };

            return PartialView("AfficherPopUpSuppressionCds", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SupprimerCds()
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

                if (u.Role > 2)
                    return new HttpUnauthorizedResult();

                Guid idAncienCds = Guid.Parse(Request.Form["idAncienCds"]);
                if (idAncienCds.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400, "Erreur id ancien cds.");

                Guid idSection = Guid.Parse(Request.Form["idSection"]);
                if (idSection.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400, "Erreur id section.");

                int retour = sDal.ChangerCds(idSection, Guid.Empty);
                if (retour != 1)
                    return new HttpStatusCodeResult(400, "Erreur de changer cds.");

                retour = uDal.SupprimerUtilisateurSection(idAncienCds);
                if (retour != 1)
                    return new HttpStatusCodeResult(400, "Erreur supp cds de la section");

                return RedirectToAction("AfficherPersonnelSection");
            }
            catch (Exception e)
            {
                return new HttpStatusCodeResult(400, "Erreur suppression CDS.");
            }
        }

        /***
         * Affichage de la pop-up d'ajout de nouveau groupe
        ***/

        [Authorize]
        public ActionResult AfficherPopUpAjoutGroupe(Guid id)
        {
            int numSection = 0;
            int numGroupe = 0;
            if (id.Equals(Guid.Empty))
            {
                AjoutGroupeViewModel vmEmpty = new AjoutGroupeViewModel
                {
                    Section = id,
                    NumGroupe = numGroupe,
                    NumSection = numSection
                };

                return PartialView("AfficherPopUpAjoutGroupe", vmEmpty);
            }

            Section section = sDal.GetSectionById(id);
            if (section == null || section.Equals(typeof(SectionNull)))
            {
                AjoutGroupeViewModel vmNull = new AjoutGroupeViewModel
                {
                    Section = Guid.Empty,
                    NumGroupe = numGroupe,
                    NumSection = numSection
                };

                return PartialView("AfficherPopUpAjoutGroupe", vmNull);
            }

            numSection = section.Numero;
            numGroupe = gDal.GetGroupesBySection(id).Count + 1;

            AjoutGroupeViewModel vm = new AjoutGroupeViewModel
            {
                Section = id,
                NumSection = numSection,
                NumGroupe = numGroupe
            };

            return PartialView("AfficherPopUpAjoutGroupe", vm);
        }

        [HttpPost]
        [Authorize]
        public ActionResult AjouterGroupe()
        {
            try
            {
                var idSection = Guid.Parse(Request.Form["idSection"]);
                var numGroupe = Int32.Parse(Request.Form["numGroupe"]);

                Groupe groupe = new Groupe
                {
                    Section = idSection,
                    Numero = numGroupe,
                    CDG = Guid.Empty
                };

                Guid idGroupe = gDal.AjouterGroupe(groupe);

                if (idGroupe.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(500, "Erreur ajout du groupe");

                return RedirectToAction("AfficherPersonnelSection");
            }
            catch(Exception e)
            {
                return new HttpStatusCodeResult(500);
            }
        }

        /***
         * Affichage de la pop-up de changement de CDU
        ***/

        [Authorize]
        public ActionResult AfficherPopUpChgmntCdu(Guid id, Guid idCie)
        {
            Utilisateur cdu = uDal.GetUtilisateurById(id);
            if (cdu == null || cdu.Equals(typeof(UtilisateurNull)))
                cdu = new Utilisateur
                {
                    Grade = "Soldat",
                    Nom = "Empty",
                    Prenom = "Empty",
                    Id = Guid.Empty
                };

            Compagnie cie = cDal.GetCompagnieById(idCie);
            if (cie == null || cie.Equals(typeof(CompagnieNull)))
                cie = new Compagnie
                {
                    Id = Guid.Empty,
                    Numero = -1
                };

            ChangementCadreComViewModel vm = new ChangementCadreComViewModel
            {
                Cie = cie,
                AncienCadre = cdu,
                SansSection = this.selectSansSection,
                Grades = this.grades,
                Roles = this.roles,
                MotDePasse = GenererMotDePasse()
            };

            return PartialView("AfficherPopUpChgmntCdu", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangementCdu()
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

                if (u.Role > 2)
                    return new HttpUnauthorizedResult();

                int numSection = u.Section;
                int numCie = u.Compagnie;

                Guid ancienCdu = Guid.Parse(Request.Form["AncienCadre.Id"]);

                Guid cie = Guid.Parse(Request.Form["Cie.Id"]);
                if (cie.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400, "Erreur sur la compagnie.");

                var creerNouveau = Request.Form["creerCdu"];
                if (creerNouveau != null && creerNouveau.Equals("on"))
                {
                    // Récupération des champs du formulaire
                    var grade = Request.Form["gradeCdu"];
                    var nom = Request.Form["nomCdu"];
                    var prenom = Request.Form["prenomCdu"];
                    var matricule = Request.Form["matriculeCdu"];
                    var mail = Request.Form["mailCdu"];
                    var naissanceForm = Request.Form["naissanceCdu"];
                    var motDePasse = Request.Form["MotDePasse"];
                    var role = Int32.Parse(Request.Form["roleCdu"]);

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
                        return new HttpNotFoundResult("Erreur ajout adresse pour nouveau cdu.");

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
                        Role = role
                    };
                    Guid idAjout = uDal.AjouterUtilisateur(pourAjout);
                    if (idAjout.Equals(Guid.Empty))
                        return new HttpNotFoundResult("Erreur ajout nouvel utilisateur. ");

                    int retour = cDal.ChangerCdu(cie, idAjout);
                    if (retour != 1)
                        return new HttpStatusCodeResult(500, "Erreur changement de cdu.");

                    if (!ancienCdu.Equals(Guid.Empty))
                    {
                        retour = uDal.SupprimerUtilisateurSection(ancienCdu);
                        if (retour != 1)
                            return new HttpStatusCodeResult(500, "Erreur suppresion d'ancien cdu");
                    }
                }
                else
                {
                    Guid nouveauCdu = Guid.Parse(Request.Form["cduExistant"]);
                    if (nouveauCdu.Equals(Guid.Empty))
                        return new HttpStatusCodeResult(400, "Erreur sur le nouveau cdu.");

                    int retour = uDal.PasserCadre(nouveauCdu, numSection, numCie);
                    if (retour != 1)
                        return new HttpStatusCodeResult(500, "Erreur passage cadre.");

                    if (!ancienCdu.Equals(Guid.Empty))
                    {
                        retour = uDal.SupprimerUtilisateurSection(ancienCdu);
                        if (retour != 1)
                            return new HttpStatusCodeResult(500, "Erreur suppression ancien cdu.");
                    }

                    retour = cDal.ChangerCdu(cie, nouveauCdu);
                    if (retour != 1)
                        return new HttpStatusCodeResult(500, "Erreur changement de commandant d'unité.");
                }

                return RedirectToAction("AfficherPersonnelSectionCom");
            }
            catch (Exception e)
            {
                return new HttpStatusCodeResult(400, "Erreur lors du changement de CDU");
            }
        }

        /***
        * Affichage de la pop-up de suppression de CDU
       ***/

        [Authorize]
        public ActionResult AfficherPopUpSuppressionCdu(Guid id, Guid idCie)
        {
            Utilisateur cdu = uDal.GetUtilisateurById(id);
            if (cdu == null || cdu.Equals(typeof(UtilisateurNull)))
                cdu = new Utilisateur
                {
                    Nom = "Empty",
                    Prenom = "Empty",
                    Grade = "Soldat",
                    Id = Guid.Empty
                };

            Compagnie cie = cDal.GetCompagnieById(idCie);
            if (cie == null || cie.Equals(typeof(CompagnieNull)))
                cie = new Compagnie
                {
                    Id = Guid.Empty
                };

            SuppressionCadreComViewModel vm = new SuppressionCadreComViewModel
            {
                Cadre = cdu,
                Cie = cie
            };

            return PartialView("AfficherPopUpSuppressionCdu", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SupprimerCdu()
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

                if (u.Role > 2)
                    return new HttpUnauthorizedResult();

                Guid idAncienCdu = Guid.Parse(Request.Form["idAncienCdu"]);
                if (idAncienCdu.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400, "Erreur id ancien cdu.");

                Guid idCie = Guid.Parse(Request.Form["idCie"]);
                if (idCie.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400, "Erreur id compagnie.");

                int retour = cDal.ChangerCdu(idCie, Guid.Empty);
                if (retour != 1)
                    return new HttpStatusCodeResult(400, "Erreur de changer cdu.");

                retour = uDal.SupprimerUtilisateurSection(idAncienCdu);
                if (retour != 1)
                    return new HttpStatusCodeResult(400, "Erreur supp cdu de la compagnie");

                return RedirectToAction("AfficherPersonnelSectionCom");
            }
            catch (Exception e)
            {
                return new HttpStatusCodeResult(400, "Erreur suppression CDU.");
            }
        }

        /***
         * Affichage de la pop-up de changement de ADU
        ***/

        [Authorize]
        public ActionResult AfficherPopUpChgmntAdu(Guid id, Guid idCie)
        {
            Utilisateur adu = uDal.GetUtilisateurById(id);
            if (adu == null || adu.Equals(typeof(UtilisateurNull)))
                adu = new Utilisateur
                {
                    Grade = "Soldat",
                    Nom = "Empty",
                    Prenom = "Empty",
                    Id = Guid.Empty
                };

            Compagnie cie = cDal.GetCompagnieById(idCie);
            if (cie == null || cie.Equals(typeof(CompagnieNull)))
                cie = new Compagnie
                {
                    Id = Guid.Empty,
                    Numero = -1
                };

            ChangementCadreComViewModel vm = new ChangementCadreComViewModel
            {
                Cie = cie,
                AncienCadre = adu,
                SansSection = this.selectSansSection,
                Grades = this.grades,
                Roles = this.roles,
                MotDePasse = GenererMotDePasse()
            };

            return PartialView("AfficherPopUpChgmntAdu", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangementAdu()
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

                if (u.Role > 2)
                    return new HttpUnauthorizedResult();

                int numSection = u.Section;
                int numCie = u.Compagnie;

                Guid ancienAdu = Guid.Parse(Request.Form["AncienCadre.Id"]);

                Guid cie = Guid.Parse(Request.Form["Cie.Id"]);
                if (cie.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400, "Erreur sur la compagnie.");

                var creerNouveau = Request.Form["creerAdu"];
                if (creerNouveau != null && creerNouveau.Equals("on"))
                {
                    // Récupération des champs du formulaire
                    var grade = Request.Form["gradeAdu"];
                    var nom = Request.Form["nomAdu"];
                    var prenom = Request.Form["prenomAdu"];
                    var matricule = Request.Form["matriculeAdu"];
                    var mail = Request.Form["mailAdu"];
                    var naissanceForm = Request.Form["naissanceAdu"];
                    var motDePasse = Request.Form["MotDePasse"];
                    var role = Int32.Parse(Request.Form["roleAdu"]);

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
                        return new HttpNotFoundResult("Erreur ajout adresse pour nouveau adu.");

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
                        Role = role
                    };
                    Guid idAjout = uDal.AjouterUtilisateur(pourAjout);
                    if (idAjout.Equals(Guid.Empty))
                        return new HttpNotFoundResult("Erreur ajout nouvel utilisateur. ");

                    int retour = cDal.ChangerAdu(cie, idAjout);
                    if (retour != 1)
                        return new HttpStatusCodeResult(500, "Erreur changement de adu.");

                    if (!ancienAdu.Equals(Guid.Empty))
                    {
                        retour = uDal.SupprimerUtilisateurSection(ancienAdu);
                        if (retour != 1)
                            return new HttpStatusCodeResult(500, "Erreur suppresion d'ancien adu");
                    }
                }
                else
                {
                    Guid nouveauAdu = Guid.Parse(Request.Form["aduExistant"]);
                    if (nouveauAdu.Equals(Guid.Empty))
                        return new HttpStatusCodeResult(400, "Erreur sur le nouveau adu.");

                    int retour = uDal.PasserCadre(nouveauAdu, numSection, numCie);
                    if (retour != 1)
                        return new HttpStatusCodeResult(500, "Erreur passage cadre.");

                    if (!ancienAdu.Equals(Guid.Empty))
                    {
                        retour = uDal.SupprimerUtilisateurSection(ancienAdu);
                        if (retour != 1)
                            return new HttpStatusCodeResult(500, "Erreur suppression ancien adu.");
                    }

                    retour = cDal.ChangerAdu(cie, nouveauAdu);
                    if (retour != 1)
                        return new HttpStatusCodeResult(500, "Erreur changement d'adjudant d'unité.");
                }

                return RedirectToAction("AfficherPersonnelSectionCom");
            }
            catch (Exception e)
            {
                return new HttpStatusCodeResult(400, "Erreur lors du changement de ADU");
            }
        }

    /***
     * Affichage de la pop-up de suppression de CDU
    ***/

        [Authorize]
        public ActionResult AfficherPopUpSuppressionAdu(Guid id, Guid idCie)
        {
            Utilisateur adu = uDal.GetUtilisateurById(id);
            if (adu == null || adu.Equals(typeof(UtilisateurNull)))
                adu = new Utilisateur
                {
                    Nom = "Empty",
                    Prenom = "Empty",
                    Grade = "Soldat",
                    Id = Guid.Empty
                };

            Compagnie cie = cDal.GetCompagnieById(idCie);
            if (cie == null || cie.Equals(typeof(CompagnieNull)))
                cie = new Compagnie
                {
                    Id = Guid.Empty
                };

            SuppressionCadreComViewModel vm = new SuppressionCadreComViewModel
            {
                Cadre = adu,
                Cie = cie
            };

            return PartialView("AfficherPopUpSuppressionAdu", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult SupprimerAdu()
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

                if (u.Role > 2)
                    return new HttpUnauthorizedResult();

                Guid idAncienAdu = Guid.Parse(Request.Form["idAncienAdu"]);
                if (idAncienAdu.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400, "Erreur id ancien adu.");

                Guid idCie = Guid.Parse(Request.Form["idCie"]);
                if (idCie.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400, "Erreur id compagnie.");

                int retour = cDal.ChangerAdu(idCie, Guid.Empty);
                if (retour != 1)
                    return new HttpStatusCodeResult(400, "Erreur de changer adu.");

                retour = uDal.SupprimerUtilisateurSection(idAncienAdu);
                if (retour != 1)
                    return new HttpStatusCodeResult(400, "Erreur supp adu de la compagnie");

                return RedirectToAction("AfficherPersonnelSectionCom");
            }
            catch (Exception e)
            {
                return new HttpStatusCodeResult(400, "Erreur suppression ADU.");
            }
        }

        /***
         * Affichage de la pop-up d'ajout de nouveau section
        ***/

        [Authorize]
        public ActionResult AfficherPopUpAjoutSection(Guid id)
        {
            int numSection = 0;
            int numCie = 0;
            if (id.Equals(Guid.Empty))
            {
                AjoutSectionViewModel vmEmpty = new AjoutSectionViewModel
                {
                    Cie = id,
                    NumCie = numCie,
                    NumSection = numSection,
                    Grades = this.grades,
                    SansSection = this.selectSansSection,
                    MotDePasse = GenererMotDePasse()
                };

                return PartialView("AfficherPopUpAjoutSection", vmEmpty);
            }

            Compagnie cie = cDal.GetCompagnieById(id);
            if (cie == null || cie.Equals(typeof(SectionNull)))
            {
                AjoutSectionViewModel vmNull = new AjoutSectionViewModel
                {
                    Cie = Guid.Empty,
                    NumCie = numCie,
                    NumSection = numSection,
                    Grades = this.grades,
                    SansSection = this.selectSansSection,
                    MotDePasse = GenererMotDePasse()
                };

                return PartialView("AfficherPopUpAjoutSection", vmNull);
            }

            numCie = cie.Numero;
            numSection = sDal.GetSectionsByCompagnie(id).Count;

            AjoutSectionViewModel vm = new AjoutSectionViewModel
            {
                Cie = id,
                NumSection = numSection,
                NumCie = numCie,
                Grades = this.grades,
                SansSection = this.selectSansSection,
                MotDePasse = GenererMotDePasse()
            };

            return PartialView("AfficherPopUpAjoutSection", vm);
        }

        [HttpPost]
        [Authorize]
        public ActionResult AjouterSection()
        {
            try
            {
                var idCie = Guid.Parse(Request.Form["Cie"]);
                var numSection = Int32.Parse(Request.Form["NumSection"]);

                Compagnie cie = cDal.GetCompagnieById(idCie);
                int numCie = 0;
                if (cie != null || !cie.Equals(typeof(CompagnieNull)))
                    numCie = cie.Numero;

                Section section = new Section
                {
                    Numero = numSection,
                    Compagnie = idCie,
                    Chant = "",
                    Devise = "",
                    CDS = Guid.Empty,
                    SOA = Guid.Empty,
                    NumCie = numCie
                };

                Guid idSection = sDal.AjouterSection(section);

                if (idSection.Equals(Guid.Empty))
                {
                    Log("ERROR", "Id section empty pour ajout de nouvelle section.");
                    return new HttpStatusCodeResult(500, "Erreur ajout du section");
                }

                var creerNouveau = Request.Form["creerNvCds"];
                if (creerNouveau != null && creerNouveau.Equals("on"))
                {
                    // Récupération des champs du formulaire
                    var grade = Request.Form["gradeNvCds"];
                    var nom = Request.Form["nomNvCds"];
                    var prenom = Request.Form["prenomNvCds"];
                    var matricule = Request.Form["matriculeNvCds"];
                    var mail = Request.Form["mailNvCds"];
                    var naissanceForm = Request.Form["naissanceNvCds"];
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
                    {
                        Log("ERROR", "Erreur ajout de l'adresse pour nouveau CDS lors d'une ajout section.");
                        sDal.SupprimerSection(idSection);
                        return new HttpNotFoundResult("Erreur ajout adresse pour nouveau cds.");
                    }

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
                        Role = 2
                    };
                    Guid idAjout = uDal.AjouterUtilisateur(pourAjout);
                    if (idAjout.Equals(Guid.Empty))
                    {
                        Log("ERROR", "Erreur ajout du noueau CDS lors de la création de nouvelle section.");
                        sDal.SupprimerSection(idSection);
                        aDal.SupprimerAdresse(idAdresse);
                        return new HttpNotFoundResult("Erreur ajout nouvel utilisateur. ");
                    }

                    int retour = sDal.ChangerCds(idSection, idAjout);
                    if (retour != 1)
                    {
                        Log("ERROR", "Erreur changement de CDS lors de la création d'une nouvelle section.");
                        sDal.SupprimerSection(idSection);
                        uDal.SupprimerUtilisateur(idAjout);
                        aDal.SupprimerAdresse(idAdresse);
                        return new HttpStatusCodeResult(500, "Erreur changement de cds.");
                    }
                }
                else
                {
                    Guid nouveauCds = Guid.Parse(Request.Form["nvCdsExistant"]);
                    if (nouveauCds.Equals(Guid.Empty))
                    {
                        Log("ERROR", "Erreur id nouveau CDS pour ajout de nouvelle section.");
                        sDal.SupprimerSection(idSection);
                        return new HttpStatusCodeResult(400, "Erreur sur le nouveau cds.");
                    }

                    int retour = uDal.PasserCadre(nouveauCds, numSection, numCie);
                    if (retour != 1)
                    {
                        Log("ERROR", "Erreur passage cadre nouveau CDS pour ajout nouvelle section.");
                        sDal.SupprimerSection(idSection);
                        return new HttpStatusCodeResult(500, "Erreur passage cadre.");
                    }

                    retour = sDal.ChangerCds(idSection, nouveauCds);
                    if (retour != 1)
                    {
                        sDal.SupprimerSection(idSection);
                        Log("ERROR", "Erreur changement de CDS pour ajout section avec CDS existant.");
                        return new HttpStatusCodeResult(500, "Erreur changement de chef de section.");
                    }
                }

                return RedirectToAction("AfficherPersonnelSectionCom");
            }
            catch (Exception e)
            {
                Log("ERROR", "Erreur lors de l'ajout d'une nouvelle section -> " + e);
                return new HttpStatusCodeResult(500);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult SupprimerDefinitivement()
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

                int typePerso = Int32.Parse(Request.Form["typePerso"]);

                Guid idPerso = Guid.Parse(Request.Form["idPerso"]);
                if (idPerso.Equals(Guid.Empty))
                    return new HttpStatusCodeResult(400);

                switch (typePerso)
                {
                    // personnel classique
                    case 0:
                        int retour_zero = uDal.SupprimerUtilisateur(idPerso);
                        if (retour_zero != 1)
                            return new HttpStatusCodeResult(400, "Erreur suppression définitive de personnel.");

                        return RedirectToAction("AfficherPersonnelSection");

                    // chef de groupe
                    case 1:
                        Guid idGroupe = Guid.Parse(Request.Form["idGroupe"]);
                        if (idGroupe.Equals(Guid.Empty))
                            return new HttpStatusCodeResult(400, "Erreur id groupe pour supp def.");

                        int retour_un = gDal.ChangerCdg(idGroupe, Guid.Empty);
                        if (retour_un != 1)
                            return new HttpStatusCodeResult(400, "Erreur de changer cdg groupe pour supp def.");

                        retour_un = uDal.SupprimerUtilisateur(idPerso);
                        if (retour_un != 1)
                            return new HttpStatusCodeResult(400, "Erreur supp def cdg de la section");

                        return RedirectToAction("AfficherPersonnelSection");

                    // sous-officier adjoint
                    case 2:
                        if (u.Role > 2)
                            return new HttpUnauthorizedResult();

                        Guid idSection = Guid.Parse(Request.Form["idSection"]);
                        if (idSection.Equals(Guid.Empty))
                            return new HttpStatusCodeResult(400, "Erreur id section supp def.");

                        int retour_deux = sDal.ChangerSoa(idSection, Guid.Empty);
                        if (retour_deux != 1)
                            return new HttpStatusCodeResult(400, "Erreur changer soa supp def.");

                        retour_deux = uDal.SupprimerUtilisateur(idPerso);
                        if (retour_deux != 1)
                            return new HttpStatusCodeResult(400, "Erreur supp soa de la section pour supp def.");

                        return RedirectToAction("AfficherPersonnelSection");

                    // chef de section
                    case 3:
                        if (u.Role > 2)
                            return new HttpUnauthorizedResult();

                        Guid idSection_trois = Guid.Parse(Request.Form["idSection"]);
                        if (idSection_trois.Equals(Guid.Empty))
                            return new HttpStatusCodeResult(400, "Erreur id section pour supp dsc def.");

                        int retour_trois = sDal.ChangerCds(idSection_trois, Guid.Empty);
                        if (retour_trois != 1)
                            return new HttpStatusCodeResult(400, "Erreur de changer cds pour supp def.");

                        retour_trois = uDal.SupprimerUtilisateur(idPerso);
                        if (retour_trois != 1)
                            return new HttpStatusCodeResult(400, "Erreur supp def cds.");

                        return RedirectToAction("AfficherPersonnelSection");

                    // adjudant d'unité
                    case 4:
                        if (u.Role > 2)
                            return new HttpUnauthorizedResult();

                        Guid idCie_quatre = Guid.Parse(Request.Form["idCie"]);
                        if (idCie_quatre.Equals(Guid.Empty))
                            return new HttpStatusCodeResult(400, "Erreur id compagnie supp def adu.");

                        int retour_quatre = cDal.ChangerAdu(idCie_quatre, Guid.Empty);
                        if (retour_quatre != 1)
                            return new HttpStatusCodeResult(400, "Erreur de changer adu pour supp def.");

                        retour_quatre = uDal.SupprimerUtilisateur(idPerso);
                        if (retour_quatre != 1)
                            return new HttpStatusCodeResult(400, "Erreur supp def adu de la compagnie");

                        return RedirectToAction("AfficherPersonnelSectionCom");

                    // commendant d'unité
                    case 5:
                        if (u.Role > 2)
                            return new HttpUnauthorizedResult();

                        Guid idCie = Guid.Parse(Request.Form["idCie"]);
                        if (idCie.Equals(Guid.Empty))
                            return new HttpStatusCodeResult(400, "Erreur id compagnie pour supp def cdu.");

                        int retour_cinq = cDal.ChangerCdu(idCie, Guid.Empty);
                        if (retour_cinq != 1)
                            return new HttpStatusCodeResult(400, "Erreur de changer cdu pour supp def.");

                        retour_cinq = uDal.SupprimerUtilisateur(idPerso);
                        if (retour_cinq != 1)
                            return new HttpStatusCodeResult(400, "Erreur supp def cdu.");

                        return RedirectToAction("AfficherPersonnelSectionCom");
                    default:
                        return new HttpStatusCodeResult(400, "Type de perso non pris en compte.");
                }
            }
            catch(Exception e)
            {
                return new HttpStatusCodeResult(500);
            }
        }

        /***
         * Modification des informations complémentaire
        ***/

        [Authorize]
        public ActionResult AfficherPopUpModifInfoSection(Guid id, string info)
        {
            Section section = sDal.GetSectionById(id);
            if (section == null || section.Equals(typeof(SectionNull)))
                section = new Section
                {
                    Id = Guid.Empty,
                    Chant = "",
                    Devise = "",
                    Numero = -1
                };

            ModifInfoSectionViewModel vm = new ModifInfoSectionViewModel
            {
                Section = section,
                Info = info
            };

            return PartialView("AfficherPopUpModifInfoSection", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangerInfoSection(ModifInfoSectionViewModel vm)
        {
            if (vm.Section == null || vm.Section.Id.Equals(Guid.Empty))
                return new HttpStatusCodeResult(400);

            int retour = sDal.ModifierSection(vm.Section.Id, vm.Section);
            if (retour != 1)
                return new HttpStatusCodeResult(500);

            if(vm.Section.Numero.Equals(0))
                return RedirectToAction("AfficherPersonnelSectionCom");

            return RedirectToAction("AfficherPersonnelSection");
        }

        [Authorize]
        public ActionResult AfficherPopUpModifInfoCie(Guid id, string info)
        {
            Compagnie cie = cDal.GetCompagnieById(id);
            if (cie == null || cie.Equals(typeof(CompagnieNull)))
                cie = new Compagnie
                {
                    Id = Guid.Empty,
                    Chant = "",
                    Devise = "",
                    Numero = -1
                };

            ModifInfoCieViewModel vm = new ModifInfoCieViewModel
            {
                Cie = cie,
                Info = info
            };

            return PartialView("AfficherPopUpModifInfoCie", vm);
        }

        [Authorize]
        [HttpPost]
        public ActionResult ChangerInfoCie(ModifInfoCieViewModel vm)
        {
            if (vm.Cie == null || vm.Cie.Id.Equals(Guid.Empty))
                return new HttpStatusCodeResult(400);

            int retour = cDal.ModifierCompagnie(vm.Cie.Id, vm.Cie);
            if (retour != 1)
                return new HttpStatusCodeResult(500);

            if (vm.Cie.Numero.Equals(0))
                return RedirectToAction("AfficherPersonnelSectionCom");

            return RedirectToAction("AfficherPersonnelSection");
        }
    }
}