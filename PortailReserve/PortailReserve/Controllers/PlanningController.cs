using Microsoft.Ajax.Utilities;
using PortailReserve.DAL;
using PortailReserve.DAL.Impl;
using PortailReserve.Models;
using PortailReserve.Models.NullObject;
using PortailReserve.Utils;
using PortailReserve.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using static PortailReserve.Utils.Utils;

namespace PortailReserve.Controllers
{
    public class PlanningController : Controller
    {
        private IUtilisateurDal uDal;
        private IEvenementDal eDal;
        private IEffectifDal effDal;
        private IParticipationDal pDal;
        private IDisponibiliteDal dDal;
        private IMessageDal mDal;
        private ILectureDal lDal;
        private readonly Logger LOGGER;

        public PlanningController()
        {
            uDal = new UtilisateurDal();
            eDal = new EvenementDal();
            effDal = new EffectifDal();
            pDal = new ParticipationDal();
            dDal = new DisponibiliteDal();
            mDal = new MessageDal();
            lDal = new LectureDal();
            LOGGER = new Logger(this.GetType());
        }

        [Authorize]
        public ActionResult Index()
        {
            try
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

                // On créé une liste contenant toutes les dates sur la prochaine année
                DateTime dateDuJour = DateTime.Now;
                int numMois = dateDuJour.Month;
                List<DateTime> listeDesDates = new List<DateTime>();
                List<string> listeMois = new List<string>();

                listeDesDates.Add(dateDuJour);
                listeMois.Add(dateDuJour.ToString("Y").Substring(0, 4));
                DateTime pourAjout = dateDuJour.AddDays(1);

                while (pourAjout.CompareTo(dateDuJour.AddYears(1)) <= 0)
                {
                    listeDesDates.Add(pourAjout);
                    if (pourAjout.Month != numMois)
                    {
                        listeMois.Add(pourAjout.ToString("Y").Substring(0, 4));
                        numMois = pourAjout.Month;
                    }
                    pourAjout = pourAjout.AddDays(1);
                }

                // on récupère tous les événement
                List<Evenement> events = eDal.GetEvenementsPourPlanning();

                // association des jour aux evenement
                List<JourEvenement> eventsParJour = new List<JourEvenement>();
                foreach (DateTime jour in listeDesDates)
                {
                    JourEvenement je = new JourEvenement();
                    je.Jour = jour;
                    je.Evenement = null;
                    je.CarreBlanc = false;
                    je.Type = "";
                    je.AutresEvents = new List<Guid>();

                    foreach (Evenement e in events)
                    {
                        if (jour.CompareTo(e.Debut) >= 0 && jour.CompareTo(e.Fin.AddDays(1)) <= 0)
                        {
                            if (je.Evenement == null)
                            {
                                je.Evenement = e;
                                je.Type = e.Type;
                            }
                            else
                            {
                                je.AutresEvents.Add(e.Id);
                            }
                        }
                    }

                    eventsParJour.Add(je);
                }

                // création d'une liste par jour de la semaine
                List<JourEvenement> lundi = new List<JourEvenement>();
                List<JourEvenement> mardi = new List<JourEvenement>();
                List<JourEvenement> mercredi = new List<JourEvenement>();
                List<JourEvenement> jeudi = new List<JourEvenement>();
                List<JourEvenement> vendredi = new List<JourEvenement>();
                List<JourEvenement> samedi = new List<JourEvenement>();
                List<JourEvenement> dimanche = new List<JourEvenement>();

                bool premierJourAjout = false;
                foreach (JourEvenement je in eventsParJour)
                {
                    // Ajout des lundi
                    if (je.Jour.DayOfWeek.Equals(DayOfWeek.Monday))
                    {
                        lundi.Add(je);
                        premierJourAjout = true;
                    }
                    else if (!premierJourAjout)
                        lundi.Add(new JourEvenement { CarreBlanc = true });

                    // ajout des mardi
                    if (je.Jour.DayOfWeek.Equals(DayOfWeek.Tuesday))
                    {
                        mardi.Add(je);
                        premierJourAjout = true;
                    }
                    else if (!premierJourAjout)
                        mardi.Add(new JourEvenement { CarreBlanc = true });

                    // ajout des mercredi
                    if (je.Jour.DayOfWeek.Equals(DayOfWeek.Wednesday))
                    {
                        mercredi.Add(je);
                        premierJourAjout = true;
                    }
                    else if (!premierJourAjout)
                        mercredi.Add(new JourEvenement { CarreBlanc = true });

                    // ajout des jeudi
                    if (je.Jour.DayOfWeek.Equals(DayOfWeek.Thursday))
                    {
                        jeudi.Add(je);
                        premierJourAjout = true;
                    }
                    else if (!premierJourAjout)
                        jeudi.Add(new JourEvenement { CarreBlanc = true });

                    // ajout des vendredi
                    if (je.Jour.DayOfWeek.Equals(DayOfWeek.Friday))
                    {
                        vendredi.Add(je);
                        premierJourAjout = true;
                    }
                    else if (!premierJourAjout)
                        vendredi.Add(new JourEvenement { CarreBlanc = true });

                    // ajout des samedi
                    if (je.Jour.DayOfWeek.Equals(DayOfWeek.Saturday))
                    {
                        samedi.Add(je);
                        premierJourAjout = true;
                    }
                    else if (!premierJourAjout)
                        samedi.Add(new JourEvenement { CarreBlanc = true });

                    // ajout des dimanche
                    if (je.Jour.DayOfWeek.Equals(DayOfWeek.Sunday))
                    {
                        dimanche.Add(je);
                        premierJourAjout = true;
                    }
                    else if (!premierJourAjout)
                        dimanche.Add(new JourEvenement { CarreBlanc = true });
                }

                Guid idEventAjd = Guid.Empty;
                if (eventsParJour.ElementAt(0).Evenement != null)
                    idEventAjd = eventsParJour.ElementAt(0).Evenement.Id;

                TableauPlanningViewModel vm = new TableauPlanningViewModel
                {
                    Lundi = lundi,
                    Mardi = mardi,
                    Mercredi = mercredi,
                    Jeudi = jeudi,
                    Vendredi = vendredi,
                    Samedi = samedi,
                    Dimanche = dimanche,
                    Mois = listeMois,
                    IdEventDuJour = idEventAjd
                };

                return View(vm);
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur affichage du planning -> " + e);
                return new HttpStatusCodeResult(500, "Exception affichage du planning -> " + e.Message);
            }
        }

        [Authorize]
        public ActionResult AfficherEventPlanning(Guid id, DateTime jour, bool autre)
        {
            try
            {
                Evenement e = eDal.GetEvenementById(id);
                if (e == null || e.Equals(typeof(EvenementNull)))
                    e = new Evenement
                    {
                        Id = Guid.Empty,
                        Effectif = Guid.Empty
                    };

                Effectif eff = effDal.GetEffectifById(e.Effectif);

                List<EventEffectif> autresEvents = new List<EventEffectif>();
                if (autre)
                {
                    foreach(Evenement ev in eDal.GetEvenementsPourPlanning())
                    {
                        if(jour.CompareTo(ev.Debut) >= 0 && jour.CompareTo(ev.Fin.AddDays(1)) <= 0 && !ev.Id.Equals(e.Id))
                        {
                            autresEvents.Add(new EventEffectif
                            {
                                Event = ev,
                                Effectif = effDal.GetEffectifById(ev.Effectif)
                            });
                        }
                    }
                }

                EventPlanningViewModel vm = new EventPlanningViewModel
                {
                    Event = e,
                    Effectif = eff,
                    Jour = jour,
                    AutresEvents = autresEvents
                };

                return PartialView("AfficherEventPlanning", vm);
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur de l'aafichge de l'evenement : " + id + " -> " + e);
                return new HttpStatusCodeResult(500, "Exception affichage de l'evenement -> " + e.Message);
            }
        }

        [Authorize]
        public ActionResult Evenement (Guid id)
        {
            try
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

                Evenement e = eDal.GetEvenementById(id);
                ViewBag.Erreur = "";
                if (e == null || e.Equals(typeof(EvenementNull)))
                    ViewBag.Erreur = "Une erreur s'est produite lors de la récupération de l'événement.";

                Effectif eff = effDal.GetEffectifById(e.Effectif);

                List<Disponibilite> allDispo = dDal.GetDispoByIdUtilAndByIdEvent(u.Id, e.Id);

                Disponibilite dispo = new Disponibilite();
                if (allDispo.Count > 0)
                    dispo = (Disponibilite)allDispo.ToArray().GetValue(0);

                List<Message> messages = mDal.GetMessagesByEvent(id);
                int nbNonLu = 0;
                foreach (Message m in messages)
                {
                    if (lDal.GetLectureByMessageAndByUtil(m.Id, u.Id) == null)
                        nbNonLu++;
                }

                EventViewModel vm = new EventViewModel()
                {
                    Event = e,
                    Util = u,
                    Effectif = eff,
                    Dispo = dispo,
                    NonLu = nbNonLu
                };

                return View(vm);
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur lors de l'affichage de l'evenement : " + id + " -> " + e);
                return new HttpStatusCodeResult(500, "Exception affichage de l'evenement -> " + e.Message);
            }
        }

        [Authorize]
        public ActionResult AfficherBoutonEtListeDispo(Guid id, string erreur = "")
        {
            try
            {
                Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);

                ViewBag.Erreur = erreur;

                Evenement e = eDal.GetEvenementById(id);
                ViewBag.Erreur = "";
                if (e == null || e.Equals(typeof(EvenementNull)))
                    ViewBag.Erreur = "Une erreur s'est produite lors de la récupération de l'événement.";

                Effectif eff = effDal.GetEffectifById(e.Effectif);

                List<Disponibilite> allDispo = dDal.GetDispoByIdUtilAndByIdEvent(u.Id, e.Id);
                bool aUneDispo = false;
                foreach (Disponibilite d in allDispo)
                {
                    if (d.Disponible)
                        aUneDispo = true;
                }

                Disponibilite dispo = new Disponibilite();
                if (allDispo.Count > 0)
                    dispo = (Disponibilite)allDispo.ToArray().GetValue(0);

                dispo.TouteLaPeriode = true;

                EventViewModel vm = new EventViewModel()
                {
                    Event = e,
                    Util = u,
                    Effectif = eff,
                    Dispo = dispo,
                    AllDispo = allDispo,
                    ADispo = aUneDispo,
                    Participation = pDal.GetParticipationByUtilAndEvent(u.Id, e.Id),
                    UtilParticipation = uDal.GetUtilisateursByParticipationOK(e.Id, u.Section, u.Compagnie),
                    UtilDispo = uDal.GetUtilisateursByDispoOK(e.Id, u.Section, u.Compagnie),
                    UtilNonParticipation = uDal.GetUtilisateursByParticipationKO(e.Id, u.Section, u.Compagnie),
                    UtilNonDispo = uDal.GetUtilisateursByDispoKO(e.Id, u.Section, u.Compagnie),
                    NoReponseD = uDal.GetUtilisateursSansReponseDispo(e.Id, u.Section, u.Compagnie),
                    NoReponseP = uDal.GetUtilisateursSansReponseParticipation(e.Id, u.Section, u.Compagnie)
                };

                return PartialView("AfficherBoutonEtListeDispo", vm);
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur affichge de la liste des dispos et des boutons pour l'event : " + id + " -> " + e);
                return new HttpStatusCodeResult(500, "Exception affichage de la liste des dispos -> " + e.Message);
            }
        }

        [Authorize]
        public ActionResult AfficherPopUpAjoutDispo(Guid id)
        {
            try
            {
                Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);

                Evenement e = eDal.GetEvenementById(id);
                ViewBag.Erreur = "";
                if (e == null || e.Equals(typeof(EvenementNull)))
                    e = new Evenement
                    {
                        Id = Guid.Empty,
                        Debut = DateTime.Now,
                        Fin = DateTime.Now
                    };

                Disponibilite dispo = new Disponibilite();

                EventViewModel vm = new EventViewModel
                {
                    Util = u,
                    Event = e,
                    Dispo = dispo
                };

                return PartialView("AfficherPopUpAjoutDispo", vm);
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur affichage de la pop-up ajout de dispo : " + id + " -> " + e);
                return new HttpStatusCodeResult(500, "Exception affichage de la pop-up -> " + e.Message);
            }
        }

        [Authorize]
        public ActionResult AfficherPopUpModifDispo(Guid id)
        {
            try
            {
                Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);

                Evenement e = eDal.GetEvenementById(id);
                ViewBag.Erreur = "";
                if (e == null || e.Equals(typeof(EvenementNull)))
                    e = new Evenement
                    {
                        Id = Guid.Empty,
                        Debut = DateTime.Now,
                        Fin = DateTime.Now
                    };

                List<Disponibilite> allDispo = dDal.GetDispoByIdUtilAndByIdEvent(u.Id, e.Id);

                Disponibilite dispo = new Disponibilite();
                if (allDispo.Count > 0)
                    dispo = (Disponibilite)allDispo.ToArray().GetValue(0);

                EventViewModel vm = new EventViewModel
                {
                    Util = u,
                    Event = e,
                    Dispo = dispo
                };

                return PartialView("AfficherPopUpModifDispo", vm);
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur affichage de la pop-up modif dispo : " + id + " -> " + e);
                return new HttpStatusCodeResult(500, "Exception affichage de la pop-up -> " + e.Message);
            }
        }

        [Authorize]
        public ActionResult Liste ()
        {
            try
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

                return View();
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur de l'affichage de la liste des evenements -> " + e);
                return new HttpStatusCodeResult(500, "Exception affichage de la liste des evenements -> " + e.Message);
            }
        }

        [Authorize]
        public ActionResult AfficherEventAVenir()
        {
            try
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

                List<Evenement> aVenir = eDal.GetEvenementsAVenir();
                aVenir = TrieEventAVenir(aVenir);

                return PartialView("AfficherEventAVenir", aVenir);
            }
            catch (Exception e)
            {
                LOGGER.Log("ERROR", "Erreur affichage des event à venir -> " + e);
                return new HttpStatusCodeResult(500, "Erreur affichage des evenements à venir.");
            }
        }

        [Authorize]
        public ActionResult AfficherEventPasse()
        {
            try
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

                List<Evenement> passe = eDal.GetEvenementsPasse();
                passe = TrieEventPasse(passe);

                return PartialView("AfficherEventPasse", passe);
            }
            catch (Exception e)
            {
                LOGGER.Log("ERROR", "Erreur affichage des event passé -> " + e);
                return new HttpStatusCodeResult(500, "Erreur affichage des evenements passés.");
            }
        }

        [Authorize]
        public ActionResult Ajouter()
        {
            try
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

                List<SelectListItem> types = new List<SelectListItem>();
                types.Add(new SelectListItem { Text = "Instruction", Value = "Instruction", Selected = true });
                types.Add(new SelectListItem { Text = "Exercice", Value = "Exercice" });
                types.Add(new SelectListItem { Text = "Stage", Value = "Stage" });
                types.Add(new SelectListItem { Text = "Mission", Value = "Mission" });

                Effectif eff = new Effectif
                {
                    Officier = 0,
                    SousOfficier = 0,
                    Militaire = 0
                };

                AjouterEventViewModel vm = new AjouterEventViewModel()
                {
                    Event = new Evenement(),
                    Types = types,
                    Effectif = eff
                };

                return View(vm);
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur affichage de la page d'ajout d'evenement -> " + e);
                return new HttpStatusCodeResult(500, "Exception affichage de la page -> " + e.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Ajouter (AjouterEventViewModel vm)
        {
            try
            {
                bool isAllValid = true;
                if (vm.Event.Nom.IsNullOrWhiteSpace())
                {
                    ModelState.AddModelError("Event.Nom", "Le titre est obligatoire.");
                    isAllValid = false;
                }

                if (vm.Event.Description.IsNullOrWhiteSpace())
                {
                    ModelState.AddModelError("Event.Description", "Les informations complémentaires sont obligatoires.");
                    isAllValid = false;
                }

                if (vm.Event.Lieu.IsNullOrWhiteSpace())
                {
                    ModelState.AddModelError("Event.Lieu", "Le lieu est obligatoire.");
                    isAllValid = false;
                }

                if (vm.Event.Debut.Year == 1 && vm.Event.Fin.Year == 1)
                {
                    ModelState.AddModelError("Event.Debut", "Les dates de d&ébut et de fin sont obligatoire.");
                    isAllValid = false;
                }
                else if (vm.Event.Debut.Year == 1)
                {
                    ModelState.AddModelError("Event.Debut", "La date de début est obligatoire.");
                    isAllValid = false;
                }
                else if (vm.Event.Fin.Year == 1)
                {
                    ModelState.AddModelError("Event.Debut", "La date de fin est obligatoire.");
                    isAllValid = false;
                }
                else if (vm.Event.Debut > vm.Event.Fin)
                {
                    ModelState.AddModelError("Event.Debut", "La date de début doit être antérieure à la date de fin.");
                    isAllValid = false;
                }

                var type = Request.Form["typeEvent"];
                if (type.IsNullOrWhiteSpace())
                    type = "Instruction";

                vm.Event.Type = type;
                vm.Types = new List<SelectListItem>();
                vm.Types.Add(new SelectListItem { Text = "Instruction", Value = "Instruction" });
                vm.Types.Add(new SelectListItem { Text = "Exercice", Value = "Exercice" });
                vm.Types.Add(new SelectListItem { Text = "Stage", Value = "Stage" });
                vm.Types.Add(new SelectListItem { Text = "Mission", Value = "Mission" });
                foreach (SelectListItem item in vm.Types)
                {
                    if (item.Value.Equals(type))
                        item.Selected = true;
                    else
                        item.Selected = false;
                }

                if (type.Equals("Mission") || type.Equals("Stage"))
                {
                    if (vm.Event.LimiteReponse < DateTime.Now)
                    {
                        ModelState.AddModelError("Event.LimiteReponse", "La date limite de réponse est déjà passée.");
                        isAllValid = false;
                    }
                }

                var patracdrFile = Request.Files["patracdrFile"];
                int taille = patracdrFile.ContentLength;
                if (taille >= 4096000)
                {
                    ModelState.AddModelError("Event.Patracdr", "La taille du PATRACDR ne doit pas dépasser 4096ko.");
                    isAllValid = false;
                }

                if (!isAllValid)
                    return View(vm);

                string path = HttpContext.Server.MapPath("~/Content/PATRACDR/") + patracdrFile.FileName;
                patracdrFile.SaveAs(path);
                string url = "/Content/PATRACDR/" + patracdrFile.FileName;

                Guid idEffectif = Guid.Empty;
                if (type.Equals("Mission") || type.Equals("Stage"))
                    idEffectif = effDal.AjouterEffectif(vm.Effectif);

                Evenement toCreate = vm.Event;
                toCreate.Effectif = idEffectif;
                toCreate.Type = type;
                toCreate.Patracdr = url;

                Guid idEvent = eDal.CreerEvenement(toCreate);

                return RedirectToAction("Evenement", "Planning", new { id = idEvent });
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur de l'ajout de l'evenement -> " + e);
                return new HttpStatusCodeResult(500, "Exception ajout de l'evenement -> " + e.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Supprimer ()
        {
            try
            {
                Utilisateur u = uDal.GetUtilisateurById(HttpContext.User.Identity.Name);
                if (u == null)
                {
                    FormsAuthentication.SignOut();
                    return new HttpUnauthorizedResult("Veuillez vous connecter.");
                }
                if (u.Equals(typeof(UtilisateurNull)))
                {
                    FormsAuthentication.SignOut();
                    ViewBag.Erreur = ((UtilisateurNull)u).Error;
                    return new HttpUnauthorizedResult("Veuillez vous connecter.");
                }
                if (u.PremiereCo)
                    return new HttpUnauthorizedResult("Ceci est votre première connexion.");

                ViewBag.Grade = u.Grade;
                ViewBag.Nom = u.Nom.ToUpperInvariant();
                ViewBag.Role = u.Role;

                if (u.Role > 3)
                    return new HttpUnauthorizedResult("Vous n'avez pas l'autorisation pour cette action.");

                Guid id = Guid.Parse(Request.Form["idEvent"]);
                if (id.Equals(Guid.Empty))
                {
                    return new HttpStatusCodeResult(400, "Erreur suppression évenement.");
                }

                var type = Request.Form["typeEvent"];

                int retour = eDal.SupprimerEvenement(id);
                if (retour != 1)
                    ViewBag.Erreur = "Un problème est surbenu lors de la suppression.";

                if (type.Equals("avenir"))
                {
                    return RedirectToAction("AfficherEventAVenir");
                }
                else
                {
                    return RedirectToAction("AfficherEventPasse");
                }
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur suppression de l'evenement -> " + e);
                return new HttpStatusCodeResult(500, "Exception suppression de l'evenement -> " + e.Message);
            }
        }

        [Authorize]
        public ActionResult Modifier (Guid id)
        {
            try
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

                if (u.Role > 3)
                    return RedirectToAction("Evenement", "Planning", new { id });

                Evenement ev = eDal.GetEvenementById(id);
                if (ev == null || ev.Equals(typeof(EvenementNull)))
                    ev = new Evenement();

                string[] urlSplit = new string[] { };
                if (!ev.Patracdr.IsNullOrWhiteSpace())
                    urlSplit = ev.Patracdr.Split('/');

                ViewBag.FileName = "";
                if (urlSplit.Length > 0)
                    ViewBag.FileName = urlSplit[urlSplit.Length - 1];

                Effectif eff = new Effectif();
                ViewBag.Display = "none;";
                if (ev.Effectif != null && !ev.Effectif.Equals(Guid.Empty))
                {
                    eff = effDal.GetEffectifById(ev.Effectif);
                    ViewBag.Display = "block;";
                }

                List<SelectListItem> types = new List<SelectListItem>();
                types.Add(new SelectListItem { Text = "Instruction", Value = "Instruction" });
                types.Add(new SelectListItem { Text = "Exercice", Value = "Exercice" });
                types.Add(new SelectListItem { Text = "Stage", Value = "Stage" });
                types.Add(new SelectListItem { Text = "Mission", Value = "Mission" });

                AjouterEventViewModel vm = new AjouterEventViewModel()
                {
                    Event = ev,
                    Effectif = eff,
                    Types = types
                };

                return View(vm);
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur affichage de la page de modification de l'evenement : " + id + " -> " + e);
                return new HttpStatusCodeResult(500, "Exception affichage de la page -> " + e.Message);
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult Modifier(AjouterEventViewModel vm)
        {
            try
            {
                bool isAllValid = true;
                if (vm.Event.Nom.IsNullOrWhiteSpace())
                {
                    ModelState.AddModelError("Event.Nom", "Le titre est obligatoire.");
                    isAllValid = false;
                }

                if (vm.Event.Description.IsNullOrWhiteSpace())
                {
                    ModelState.AddModelError("Event.Description", "Les informations complémentaires sont obligatoires.");
                    isAllValid = false;
                }

                if (vm.Event.Lieu.IsNullOrWhiteSpace())
                {
                    ModelState.AddModelError("Event.Lieu", "Le lieu est obligatoire.");
                    isAllValid = false;
                }

                if (vm.Event.Debut.Year == 1 && vm.Event.Fin.Year == 1)
                {
                    ModelState.AddModelError("Event.Debut", "Les dates de d&ébut et de fin sont obligatoire.");
                    isAllValid = false;
                }
                else if (vm.Event.Debut.Year == 1)
                {
                    ModelState.AddModelError("Event.Debut", "La date de début est obligatoire.");
                    isAllValid = false;
                }
                else if (vm.Event.Fin.Year == 1)
                {
                    ModelState.AddModelError("Event.Debut", "La date de fin est obligatoire.");
                    isAllValid = false;
                }
                else if (vm.Event.Debut > vm.Event.Fin)
                {
                    ModelState.AddModelError("Event.Debut", "La date de début doit être antérieure à la date de fin.");
                    isAllValid = false;
                }

                string type = Request.Form["Event.Type"];
                if (type.IsNullOrWhiteSpace())
                    type = "Instruction";

                if (type.Equals("Mission") || type.Equals("Stage"))
                {
                    if (vm.Event.LimiteReponse < DateTime.Now)
                    {
                        ModelState.AddModelError("Event.LimiteReponse", "La date limite de réponse est déjà passée.");
                        isAllValid = false;
                    }
                }

                var patracdrFile = Request.Files["patracdrFile"];
                string url = vm.Event.Patracdr;
                if (!patracdrFile.FileName.Equals(""))
                {
                    int taille = patracdrFile.ContentLength;
                    if (taille >= 4096000)
                    {
                        ModelState.AddModelError("Event.Patracdr", "La taille du PATRACDR ne doit pas dépasser 4096ko.");
                        isAllValid = false;
                    }
                    else
                    {
                        string path = HttpContext.Server.MapPath("~/Content/PATRACDR/") + patracdrFile.FileName;
                        patracdrFile.SaveAs(path);
                        url = "/Content/PATRACDR/" + patracdrFile.FileName;
                    }
                }

                if (!isAllValid)
                    return View(vm);

                Guid idEffectif = vm.Effectif.Id;
                if (type.Equals("Mission") || type.Equals("Stage"))
                    effDal.ModifierEffectif(vm.Effectif.Id, vm.Effectif);

                Evenement toModif = vm.Event;
                toModif.Effectif = idEffectif;
                toModif.Type = type;
                toModif.Patracdr = url;

                int retour = eDal.ModifierEvenement(vm.Event.Id, toModif);

                if (retour != 1)
                    ViewBag.Erreur = "Une erreure est survenue lors de la modification de lévénement.";

                return RedirectToAction("Evenement", "Planning", new { id = vm.Event.Id });
            }
            catch(Exception e)
            {
                LOGGER.Log("ERROR", "Erreur modification de l'evenement -> " + e);
                return new HttpStatusCodeResult(500, "Exception lors de la modification de l'evenement -> " + e.Message);
            }
        }
    }
}