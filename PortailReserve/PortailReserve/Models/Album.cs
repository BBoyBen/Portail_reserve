using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Album
    {
        public long Id { get; set; }
        public String Titre { get; set; }
        public String Description { get; set; }
        public DateTime Creation { get; set; }
    }
}