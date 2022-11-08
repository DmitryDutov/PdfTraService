using System;
using System.IO;

namespace PdfTraService.Models.Parsers
{
    public class TestParser : BaseParser
    {
        public TestParser()
        {
            //ParserGuid = Guid.Parse("741A9D5D-54E7-4A6A-B7DB-608C34276A56");
            ParserGuid = new Guid(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 });
            Name = "TestParser";
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

