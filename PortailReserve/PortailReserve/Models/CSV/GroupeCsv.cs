using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models.CSV
{
    public class GroupeCsv
    {
        [Name("Numero")]
        public string Numero { get; set; }
        [Name("Cdg")]
        public string Cdg { get; set; }
        [Name("Section")]
        public string Section { get; set; }
    }
}