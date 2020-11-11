using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PortailReserve.Utils
{
    public class Logger
    {
        private Type Classe;

        public Logger(Type classe)
        {
            Classe = classe;
        } 
        private static string GetFilePath(string type)
        {
            //C:/Users/ben63/Documents/Projet/Logs/LogPortailReserve/
            //D:/Developpement/Logs/PortailReserve/
            string filePath = "D:/Developpement/Logs/PortailReserve/";

            switch (type)
            {
                case "ERROR":
                    filePath += "Error/";
                    break;
                case "INFO":
                    filePath += "Info/";
                    break;
                case "DEBUG":
                    filePath += "Debug/";
                    break;
                default:
                    filePath += "Autres/";
                    break;
            }
            return filePath;
        }

        private static string GetFileName(string type)
        {
            DateTime today = DateTime.Now;

            string fileName = type + "_" + today.Day + today.Month + today.Year + today.Hour + today.Minute + today.Second + ".txt";

            return fileName;
        }

        public void Log(string type, string message)
        {
            string filePath = GetFilePath(type);
            string fileName = GetFileName(type);

            try
            {
                StreamWriter fichierLog = new StreamWriter(filePath + fileName, true);
                fichierLog.WriteLine(type + " " + DateTime.Now + " -> " + this.Classe + " : " + message);
                fichierLog.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur ouverture fichier log : " + e);
            }

        }
    }
}