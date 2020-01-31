using Microsoft.Ajax.Utilities;
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
    }
}