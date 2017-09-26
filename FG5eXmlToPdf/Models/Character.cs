using System.Collections.Generic;

namespace FG5eXmlToPDF.Models
{
    public class Character5e
    {
        public List<Ability> Abilities { get; set; }
        public string Background { get; set; }
        public string Name { get; set; }
        public int ProfBonus { get; set; }
    }
}
