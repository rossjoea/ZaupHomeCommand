using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Zamirathe_HomeCommand
{
    public class HomeGroup
    {
        [XmlAttribute]
        public string Id { get; set; }
        [XmlAttribute]
        public byte Wait { get; set; }
    }
}
