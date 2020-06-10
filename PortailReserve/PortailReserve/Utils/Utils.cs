using Microsoft.Ajax.Utilities;
using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace PortailReserve.Utils
{
    public static class Utils
    {
        public static string EncodeSHA256(string motDePasse)
        {
            string motDePasseSel = "PortailReserve" + motDePasse + "ASP.NET MVC";
            return BitConverter.ToString(new SHA256CryptoServiceProvider().ComputeHash(ASCIIEncoding.Default.GetBytes(motDePasseSel)));
        }

        public static bool ValideMatricule(string matricule)
        {
            if (matricule.IsNullOrWhiteSpace())
                return false;

            Regex rgx = new Regex(@"^[0-9]{10}$");
            return rgx.IsMatch(matricule);
        }

        public static bool ValideMotDePasse(string mdp, string mdpBis)
        {
            if (mdp.IsNullOrWhiteSpace() || mdpBis.IsNullOrWhiteSpace())
                return false;

            if (!mdp.Equals(mdpBis))
                return false;

            return true;
        }

        public static bool ValideMail (string mail)
        {
            if (mail.IsNullOrWhiteSpace())
                return false;

            Regex rgx = new Regex(@".+@.+\..+");
            return rgx.IsMatch(mail);
        }

        public static bool ValideTel (string tel)
        {
            if (tel.IsNullOrWhiteSpace())
                return false;

            Regex rgx = new Regex(@"^(\+\d+(\s|-))?0\d(\s|-)?(\d{2}(\s|-)?){4}$");
            return rgx.IsMatch(tel);
        }

        public static bool ValideCodePostal (string cp)
        {
            if (cp.IsNullOrWhiteSpace())
                return false;

            Regex rgx = new Regex(@"^(([0-8][0-9])|(9[0-5]))[0-9]{3}$");
            return rgx.IsMatch(cp);
        }

        public static List<Groupe> TrierGroupes(List<Groupe> pasTrie)
        {
            List<Groupe> trie = new List<Groupe>();

            int numGroupe = 1;
            while(pasTrie.Count > 0)
            {
                Groupe aTrie = null;
                foreach (Groupe g in pasTrie)
                {
                    if (g.Numero == numGroupe)
                        aTrie = g;
                }
                if(aTrie != null)
                {
                    trie.Add(aTrie);
                    pasTrie.Remove(aTrie);
                }
                numGroupe++;
            }

            return trie;
        }

        public static List<Evenement> TrieEventAVenir(List<Evenement> aVenir)
        {
            List<Evenement> trie = new List<Evenement>();

            while(aVenir.Count > 0)
            {
                Evenement aTrie = aVenir.ElementAt(0);
                foreach(Evenement e in aVenir)
                {
                    if (e.Debut <= aTrie.Debut)
                        aTrie = e;
                }
                trie.Add(aTrie);
                aVenir.Remove(aTrie);
            }

            return trie;
        }

        public static List<Evenement> TrieEventPasse(List<Evenement> passe)
        {
            List<Evenement> trie = new List<Evenement>();

            while(passe.Count > 0)
            {
                Evenement aTrie = passe.ElementAt(0);
                foreach(Evenement e in passe)
                {
                    if (e.Debut >= aTrie.Debut)
                        aTrie = e;
                }
                trie.Add(aTrie);
                passe.Remove(aTrie);
            }

            return trie;
        }

        public static string GenererMotDePasse()
        {
            string caracs = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890-#@%";
            string mdp = "";

            Random rand = new Random();
            for(int i = 0; i < 10; i++)
            {
                mdp += caracs[rand.Next(0, caracs.Length)];
            }

            return mdp;
        }

        public static string FormatTitreAlbum(string titre)
        {
            string nveauTitre = titre.Replace(" ", "_");
            nveauTitre = nveauTitre.Replace("'", "");
            nveauTitre = nveauTitre.Replace("é", "e");
            nveauTitre = nveauTitre.Replace("è", "e");
            nveauTitre = nveauTitre.Replace("ê", "e");
            nveauTitre = nveauTitre.Replace("à", "a");
            nveauTitre = nveauTitre.Replace("ô", "o");
            nveauTitre = nveauTitre.Replace("î", "i");

            return nveauTitre;
        }
    }
}