using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.ViewModel
{
    public class ModifMdpViewModel
    {
        public string Old { get; set; }
        public string New { get; set; }
        public string NewBis { get; set; }
        public Guid IdUtil { get; set; }
    }
}