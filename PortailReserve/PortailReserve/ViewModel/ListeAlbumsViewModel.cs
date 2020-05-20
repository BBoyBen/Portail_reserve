using PortailReserve.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.ViewModel
{
    public class ListeAlbumsViewModel
    {
        public List<Album> Albums { get; set; }
        public int Count { get; set; }
        public int NumCie { get; set; }
    }
}