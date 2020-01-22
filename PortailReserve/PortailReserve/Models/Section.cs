using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Section
    {
        public long Id { get; set; }
        public int Numero { get; set; }
        public long CDS { get; set; }
        public long SOA { get; set; }
        public string Chant { get; set; }
        public string Devise { get; set; }
    }
}