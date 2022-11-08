using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfTraService.Models
{
    public class Override
    {
        public string Microsoft { get; set; }
        public string System { get; set; }
    }

    public class MinimumLevel
    {
        public string Default { get; set; }
        public Override Override { get; set; }
    }

    public class Args
    {
        public string Theme { get; set; }
        public string OutputTemplate { get; set; }
        public string Path { get; set; }
        public string RollingInterval { get; set; }
        public bool? Shared { get; set; }
    }

    public class WriteTo
    {
        public string Name { get; set; }
        public Args Args { get; set; }
    }

    public class Properties
    {
        public string Application { get; set; }
    }

    public class Serilog
    {
        public IList<string> Using { get; set; }
        public MinimumLevel MinimumLevel { get; set; }
        public IList<string> Enrich { get; set; }
        public IList<WriteTo> WriteTo { get; set; }
        public Properties Properties { get; set; }
    }

    public class Inform
    {
        public string Name { get; set; }
        public string Current { get; set; }
        public string Target { get; set; }
        public string Filter { get; set; }
        public bool Active { get; set; }
    }

    public class Rootobject
    {
        public Serilog Serilog { get; set; }
        public string MainPath { get; set; }
        public IList<Inform> InformList { get; set; }
    }


}
