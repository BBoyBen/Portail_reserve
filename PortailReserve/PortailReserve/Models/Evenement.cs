﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Evenement
    {
        public long Id { get; set; }
        public string Nom { get; set; }
        public string Description { get; set; }
        public DateTime Debut { get; set; }
        public DateTime Fin { get; set; }
        public DateTime LimiteReponse { get; set; }
        public string Patracdr { get; set; }
        public int Duree { get; set; }
        public string Type { get; set; }
        public string Lieu { get; set; }
    }
}