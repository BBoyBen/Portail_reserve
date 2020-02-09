using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.ViewModel
{
    public class AcceuilViewModel
    {
        public bool HasProchainEvent { get; set; }
        public Evenement ProchainEvent { get; set; }
    }
}