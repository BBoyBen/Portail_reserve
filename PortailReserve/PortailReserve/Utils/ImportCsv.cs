using CsvHelper;
using PortailReserve.DAL;
using PortailReserve.DAL.Impl;
using System.IO;
using System.Globalization;
using PortailReserve.Models.CSV;
using PortailReserve.Models;
using System;
using PortailReserve.Models.NullObject;

namespace PortailReserve.Utils
{
    public class ImportCsv
    {
        public static void InitEncadrement()
        {
            IUtilisateurDal uDal = new UtilisateurDal();
            IAdresseDal aDal = new AdresseDal();

            using (var reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/Content/import/encadrement.csv")))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<UtilisateurCsv>();

                foreach(UtilisateurCsv u in records)
                {
                    Guid idAdresse = aDal.AjouterAdresse(new Adresse() { 
                        Ville = u.Ville,
                        Voie = u.Voie,
                        CodePostal = u.CodePostal,
                        Pays = u.Pays
                    });

                    CultureInfo myCulture = new CultureInfo("fr-FR");
                    DateTime date = DateTime.Parse(u.Naissance, myCulture);

                    uDal.AjouterUtilisateur(new Utilisateur() {
                        Matricule = u.Matricule,
                        MotDePasse = u.Motdepasse,
                        Grade = u.Grade,
                        Nom = u.Nom,
                        Prenom = u.Prenom,
                        Email = u.Email,
                        Groupe = Guid.Empty,
                        Telephone = u.Telephone,
                        Role = Int32.Parse(u.Role),
                        Adresse = idAdresse,
                        Naissance = date,
                        PremiereCo = true,
                        Section = Int32.Parse(u.Section),
                        Compagnie = Int32.Parse(u.Compagnie)
                    });
                }
            }
        }

        public static void InitCompagnie()
        {
            IUtilisateurDal uDal = new UtilisateurDal();
            ICompagnieDal cDal = new CompagnieDal();

            using (var reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/Content/import/compagnie.csv")))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<CompagnieCsv>();

                foreach(CompagnieCsv c in records)
                {
                    Utilisateur cdu = uDal.GetUtilisateurByMatricule(c.Cdu);
                    Guid idCdu = Guid.Empty;
                    if (cdu != null && !cdu.Equals(typeof(UtilisateurNull)))
                        idCdu = cdu.Id;

                    Utilisateur adu = uDal.GetUtilisateurByMatricule(c.Adu);
                    Guid idAdu = Guid.Empty;
                    if (adu != null && !adu.Equals(typeof(UtilisateurNull)))
                        idAdu = adu.Id;

                    cDal.AjouterCompagnie(new Compagnie()
                    {
                        Numero = Int32.Parse(c.Numero),
                        CDU = idCdu,
                        ADU = idAdu,
                        Chant = c.Chant,
                        Devise = c.Devise
                    });
                }
            }
        }

        public static void InitSection ()
        {
            IUtilisateurDal uDal = new UtilisateurDal();
            ICompagnieDal cDal = new CompagnieDal();
            ISectionDal sDal = new SectionDal();

            using (var reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/Content/import/section.csv")))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<SectionCsv>();

                foreach(SectionCsv s in records)
                {
                    Compagnie cie = cDal.GetCompagnieByNumero(Int32.Parse(s.Compagnie));
                    Guid idCie = Guid.Empty;
                    if (cie != null && !cie.Equals(typeof(CompagnieNull)))
                        idCie = cie.Id;

                    Utilisateur cds = uDal.GetUtilisateurByMatricule(s.Cds);
                    Guid idCds = Guid.Empty;
                    if (cds != null && !cds.Equals(typeof(UtilisateurNull)))
                        idCds = cds.Id;

                    Utilisateur soa = uDal.GetUtilisateurByMatricule(s.Soa);
                    Guid idSoa = Guid.Empty;
                    if (soa != null && !soa.Equals(typeof(UtilisateurNull)))
                        idSoa = soa.Id;

                    sDal.AjouterSection(new Section()
                    {
                        Numero = Int32.Parse(s.Numero),
                        CDS = idCds,
                        SOA = idSoa,
                        Compagnie = idCie,
                        Chant = s.Chant,
                        Devise = s.Devise,
                        NumCie = Int32.Parse(s.NumCie)
                    });
                }
            }
        }

        public static void InitGroupe()
        {
            IUtilisateurDal uDal = new UtilisateurDal();
            ISectionDal sDal = new SectionDal();
            IGroupeDal gDal = new GroupeDal();

            using (var reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/Content/import/groupe.csv")))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<GroupeCsv>();

                foreach(GroupeCsv g in records)
                {
                    Section section = sDal.GetSectionByNumero(Int32.Parse(g.Section));
                    Guid idSection = Guid.Empty;
                    if (section != null && !section.Equals(typeof(SectionNull)))
                        idSection = section.Id;

                    Utilisateur cdg = uDal.GetUtilisateurByMatricule(g.Cdg);
                    Guid idCdg = Guid.Empty;
                    if (cdg != null && !cdg.Equals(typeof(UtilisateurNull)))
                        idCdg = cdg.Id;

                    gDal.AjouterGroupe(new Groupe()
                    {
                        Numero = Int32.Parse(g.Numero),
                        Section = idSection,
                        CDG = idCdg
                    });
                }
            }
        }

        public static void InitPersonnel ()
        {
            IUtilisateurDal uDal = new UtilisateurDal();
            IAdresseDal aDal = new AdresseDal();
            IGroupeDal gDal = new GroupeDal();

            using (var reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/Content/import/personnel.csv")))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<UtilisateurCsv>();

                foreach(UtilisateurCsv u in records)
                {
                    Guid idAdresse = aDal.AjouterAdresse(new Adresse()
                    {
                        Ville = u.Ville,
                        Voie = u.Voie,
                        Pays = u.Pays,
                        CodePostal = u.CodePostal
                    });

                    Groupe grp = gDal.GetGroupeByNumeroAndBySection(Int32.Parse(u.Groupe), Int32.Parse(u.Section));
                    Guid idGrp = Guid.Empty;
                    if (grp != null && !grp.Equals(typeof(GroupeNull)))
                        idGrp = grp.Id;

                    CultureInfo myCulture = new CultureInfo("fr-FR");
                    DateTime date = DateTime.Parse(u.Naissance, myCulture);

                    uDal.AjouterUtilisateur(new Utilisateur()
                    {
                        Matricule = u.Matricule,
                        MotDePasse = u.Motdepasse,
                        Grade = u.Grade,
                        Nom = u.Nom,
                        Prenom = u.Prenom,
                        Email = u.Email,
                        Groupe = idGrp,
                        Telephone = u.Telephone,
                        Role = Int32.Parse(u.Role),
                        Adresse = idAdresse,
                        Naissance = date,
                        PremiereCo = true,
                        Section = Int32.Parse(u.Section),
                        Compagnie = Int32.Parse(u.Compagnie)
                    });
                }
            }
        }

        public static void InitEvent()
        {
            IEvenementDal eDal = new EvenementDal();
            IEffectifDal effDal = new EffectifDal();

            using (var reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/Content/import/event.csv")))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                var records = csv.GetRecords<EvenementCsv>();
                foreach(EvenementCsv e in records)
                {
                    Guid idEff = Guid.Empty;
                    if (!e.Officier.Equals("-1") && !e.SousOfficier.Equals("-1") && !e.Militaire.Equals("-1"))
                    {
                        idEff = effDal.AjouterEffectif(new Effectif()
                        {
                            Officier = Int32.Parse(e.Officier),
                            SousOfficier = Int32.Parse(e.SousOfficier),
                            Militaire = Int32.Parse(e.Militaire)
                        });
                    }

                    CultureInfo myCulture = new CultureInfo("fr-FR");

                    DateTime debut = DateTime.Parse(e.Debut, myCulture);
                    DateTime fin = DateTime.Parse(e.Fin, myCulture);

                    DateTime limite = debut;
                    if(!e.LimiteReponse.Equals("-1"))
                    {
                        limite = DateTime.Parse(e.LimiteReponse, myCulture);
                    }

                    eDal.CreerEvenement(new Evenement()
                    {
                        Debut = debut,
                        Fin = fin,
                        LimiteReponse = limite,
                        Nom = e.Nom,
                        Description = e.Description,
                        Lieu = e.Lieu,
                        Effectif = idEff,
                        Patracdr = e.Patracdr,
                        Type = e.Type
                    });
                }
            }
        }

        public static void InitBddByCsv()
        {
            InitEncadrement();
            InitCompagnie();
            InitSection();
            InitGroupe();
            InitPersonnel();
            InitEvent();
        }
    }
}