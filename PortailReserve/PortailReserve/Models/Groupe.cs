﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Groupe
    {
        public long Id { get; set; }
        public long Id_Section { get; set; }
        public int Numero { get; set; }
        public long CDG { get; set; }
    }
}