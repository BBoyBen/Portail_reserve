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
            if (matricule == null)
                return false;

            Regex rgx = new Regex(@"^[0-9]{10}$");
            return rgx.IsMatch(matricule);
        }
    }
}