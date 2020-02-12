using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PortailReserve.Models.CSV
{
    public class SectionCsv
    {
        [Name("Numero")]
        public string Numero { get; set; }
        [Name("Cds")]
        public string Cds { get; set; }
        [Name("Soa")]
        public string Soa { get; set; }
        [Name("Chant")]
        public string Chant { get; set; }
        [Name("Devise")]
        public string Devise { get; set; }
        [Name("Compagnie")]
        public string Compagnie { get; set; }
    }
}