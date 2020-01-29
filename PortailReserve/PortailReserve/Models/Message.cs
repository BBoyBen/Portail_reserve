﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class Message
    {
        [Key]
        public Guid Id { get; set; }
        public Evenement Evenement { get; set; }
        public string Texte { get; set; }
        public DateTime Date { get; set; }
        public Utilisateur Envoyeur { get; set; }
    }
}