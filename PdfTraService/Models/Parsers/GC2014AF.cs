using System;
using System.IO;

namespace PdfTraService.Models.Parsers
{
    class GC2014AF : BaseParser
    {
        public GC2014AF()
        {
            ParserGuid = new Guid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 });
            Name = "GC2014AF_n11484533028Parser";
        }

        public override bool Process(string data)
        {
            string str = data + "\n";
            if (data != String.Empty && data != "" && data != " " && data is not null)
            {
                File.AppendAllText(PathLog, str);
            }
            return true;
        }
    }
}

