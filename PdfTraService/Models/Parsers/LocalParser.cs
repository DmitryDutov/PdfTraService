using System;
using System.IO;

namespace PdfTraService.Models.Parsers
{
    public class LocalParser : BaseParser
    {
        public LocalParser()
        {
            ParserGuid = new Guid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2 });
            Name = "LocalParser";
        }

        public override bool Process(string data)
        {
            var time = DateTime.Now.ToString("yyyy-MM-dd HH.mm.ss.fff");
            string str = time + " -> " + data + "\n";
            if (data != String.Empty && data != "" && data != " " && data is not null)
            {
                File.AppendAllText(PathLog, str);
            }
            return true;
        }

    }
}

