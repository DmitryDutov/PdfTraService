using System;
using System.IO;

namespace PdfTraService.Models.Parsers
{
    class GC2010Plus : BaseParser
    {
        public GC2010Plus()
        {
            ParserGuid = new Guid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 });
            Name = "GC2010Plus_n12095100214Parser";
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
