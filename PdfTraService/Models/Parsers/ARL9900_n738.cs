using System;
using System.IO;

namespace PdfTraService.Models.Parsers
{
    class ARL9900_n738 : BaseParser
    {
        public ARL9900_n738()
        {
            ParserGuid = new Guid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5 });
            Name = "ARL9900_n738Parser";
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
