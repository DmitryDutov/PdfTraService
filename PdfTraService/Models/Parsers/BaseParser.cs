using System;
using PdfTraService.Models.Parsers.Interface;

namespace PdfTraService.Models.Parsers
{
    public class BaseParser : IParser
    {
        public Guid ParserGuid { get; set; }
        public string Name { get; set; }
        public string PathData { get; set; }
        public string PathLog { get; set; }
        public string Encoding { get; set; }
        public int Interval { get; set; }
        public string Mask { get; set; }

        public virtual bool Process(string data)
        {
            return true;
        }
    }
}

