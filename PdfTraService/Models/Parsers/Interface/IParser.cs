using System;

namespace PdfTraService.Models.Parsers.Interface
{
    public interface IParser
    {
        public Guid ParserGuid { get; set; }
        public string Name { get; set; }
        public string PathData { get; set; }
        public string PathLog { get; set; }
        public string Encoding { get; set; }
        public int Interval { get; set; }
        public string Mask { get; set; }
        public bool Process(string data);
    }
}

