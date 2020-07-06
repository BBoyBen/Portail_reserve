using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.ViewModel
{
    public class MessageViewModel
    {
        public MessageUtil Message { get; set; }
        public Utilisateur Util { get; set; }
    }
}