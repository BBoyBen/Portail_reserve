﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Cours
    {
        [Key]
        public Guid Id { get; set; }
        public string Nom { get; set; }
        public string Description { get; set; }
        public string Fichier { get; set; }
        public string Theme { get; set; }
        public DateTime Publication { get; set; }
        public DateTime Modification { get; set; }
    }
}