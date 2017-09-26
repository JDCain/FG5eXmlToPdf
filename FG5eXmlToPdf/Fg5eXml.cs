using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using FG5eXmlToPDF.Models;
// ReSharper disable All

namespace FG5eXmlToPDF
{
    public static class Fg5eXml
    { 
        public static Character5e LoadCharacter(string fileString)
        {
            var xml = XDocument.Load(fileString);
            var name = xml?.Root?.Element("character")?.Element("name")?.Value;
            var ab = xml?.Root?.Element("character")?.Element("abilities");
            var dec = ab.Descendants();
            //IEnumerable<XElement> childList =
            //    from el in xml?.Root?.Element("character")?.Element("abilities").Elements()
            //    select el;
            
            var list = xml.Root.Element("character")?.Element("abilities").Elements().ToList();
            var a = new List<Ability>();
            foreach (var attrib in list)
            {
                a.Add(new Ability()
                {
                    Name = attrib.Name.ToString(),
                    Score = int.Parse(attrib.Element("score").Value),
                    Bonus = int.Parse(attrib.Element("bonus").Value),
                    Save = int.Parse(attrib.Element("save").Value),
                    Savemodifier = int.Parse(attrib.Element("savemodifier").Value),
                    Saveprof = int.Parse(attrib.Element("saveprof").Value),
                });
            }
            return new Character5e()
            {
                Name = name,
                Abilities = a
            };
        }

    }
}
