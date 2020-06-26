using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.ViewModel
{
    public class MessagerieViewModel
    {
        public Utilisateur Util { get; set; }
        public List<MessageUtil> Messages { get; set; }
    }
}