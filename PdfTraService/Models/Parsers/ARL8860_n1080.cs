using System;
using System.IO;

namespace PdfTraService.Models.Parsers
{
    class ARL8860_n1080 : BaseParser
    {
        public ARL8860_n1080()
        {
            ParserGuid = new Guid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7 });
            Name = "ARL8860_n1080Parser";
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
