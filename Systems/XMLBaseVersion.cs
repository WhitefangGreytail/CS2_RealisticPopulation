using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace WG_CS2_RealisticPopulation.Systems
{
    public abstract class WG_XMLBaseVersion
    {
        public WG_XMLBaseVersion()
        {
        }

        public abstract void readXML(XmlDocument doc);
        public abstract bool writeXML(string fullPathFileName);
    }
}
