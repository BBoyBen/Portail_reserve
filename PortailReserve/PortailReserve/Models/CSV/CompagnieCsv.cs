using CsvHelper.Configuration.Attributes;

namespace PortailReserve.Models.CSV
{
    public class CompagnieCsv
    {
        [Name("Numero")]
        public string Numero { get; set; }
        [Name("Cdu")]
        public string Cdu { get; set; }
        [Name("Adu")]
        public string Adu { get; set; }
        [Name("Chant")]
        public string Chant { get; set; }
        [Name("Devise")]
        public string Devise { get; set; }
    }
}