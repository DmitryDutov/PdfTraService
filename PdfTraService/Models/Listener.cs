using System;

namespace PdfTraService.Models
{
    public class Listener
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public int Port { get; set; }
        public bool RunState { get; set; }
    }
}

