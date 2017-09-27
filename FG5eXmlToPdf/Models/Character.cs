using System.Collections.Generic;
using FG5eXmlToPdf.Models;

namespace FG5eXmlToPDF.Models
{
    public class Character5e
    {
        public List<Skill> Skills { get; set; }
        public List<Ability> Abilities { get; set; }
        public string Background { get; set; }
        public string Name { get; set; }
        public int ProfBonus { get; set; }
    }
}
