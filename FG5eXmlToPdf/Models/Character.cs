using System.Collections.Generic;
using FG5eXmlToPdf.Models;

namespace FG5eXmlToPDF.Models
{
    public class Character5e
    {
        public List<Skill> Skills { get;  }
        public List<Ability> Abilities { get; }
        public Dictionary<string,string> Properties { get; }

        public Character5e()
        {
            Properties = new Dictionary<string, string>()
            {
                {"Name", "" },
                {"Race", "" },
                {"Background", "" },
                {"ProfBonus", "" },
                {"Alignment", "" },
            };

            Abilities = new List<Ability>();
            Skills = new List<Skill>();

        }
    }
}
