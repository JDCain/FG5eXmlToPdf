using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using FG5eXmlToPdf;
using FG5eXmlToPdf.Models;
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
            
            var abilityList = xml.Root.Element("character")?.Element("abilities").Elements().ToList();
            var abilitys = new List<Ability>();
            foreach (var attrib in abilityList)
            {
                abilitys.Add(new Ability()
                {
                    Name = attrib.Name.ToString(),
                    Score = int.Parse(attrib.Element("score").Value),
                    Bonus = int.Parse(attrib.Element("bonus").Value),
                    Save = int.Parse(attrib.Element("save").Value),
                    Savemodifier = int.Parse(attrib.Element("savemodifier").Value),
                    Saveprof = Helper.StringIntToBool(attrib.Element("saveprof").Value)
                });
            }

            var skillList = xml.Root.Element("character")?.Element("skilllist").Elements().ToList();
            var skills = new List<Skill>();
            foreach (var skill in skillList)
            {
                skills.Add(new Skill()
                {
                    Name = skill.Element("name").Value,
                    Misc = int.Parse(skill.Element("misc").Value),
                    Prof = Helper.StringIntToBool(skill.Element("prof").Value),
                    Stat = skill.Element("stat").Value,
                    Total = int.Parse(skill.Element("total").Value)
                });
            }

            return new Character5e()
            {
                Name = name,
                Abilities = abilitys,
                Skills = skills
            };
        }

    }
}
