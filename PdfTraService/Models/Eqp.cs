using System;
using System.Collections.Generic;

namespace PdfTraService.Models
{
    public class Rootobject
    {
        public string MainPath { get; set; }
        public List<Eqp> Eqps { get; set; }
    }

    public class Eqp
    {
        public string Name { get; set; }
        public Guid EqpGuid { get; set; }
        public string Path { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public bool Active { get; set; }
        public string Encoding { get; set; } //todo: переименовать все "Encoding" класса Eqp в "EncodingEqp"
        public int Interval { get; set; }
        public string Mask { get; set; }
    }
}

