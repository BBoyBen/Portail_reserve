using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Evenement
    {
        public long Id { get; set; }
        public String Nom { get; set; }
        public String Description { get; set; }
        public DateTime Debut { get; set; }
        public DateTime Fin { get; set; }
        public DateTime LimiteReponse { get; set; }
        public String Patracdr { get; set; }
        public int Duree { get; set; }
        public String Type { get; set; }
        public String Lieu { get; set; }
    }
}