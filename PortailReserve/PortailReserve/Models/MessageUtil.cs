using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models
{
    public class MessageUtil
    {
        public Message Message { get; set; }
        public string Auteur { get; set; }
        public bool Lu { get; set; }
        public List<ReponseUtil> Reponses { get; set; }
    }
}