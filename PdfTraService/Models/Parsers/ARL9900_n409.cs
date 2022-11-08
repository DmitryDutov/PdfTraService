using System;
using System.IO;

namespace PdfTraService.Models.Parsers
{
    class ARL9900_n409 : BaseParser
    {
        public ARL9900_n409()
        {
            ParserGuid = new Guid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4 });
            Name = "ARL9900_n409Parser";
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

