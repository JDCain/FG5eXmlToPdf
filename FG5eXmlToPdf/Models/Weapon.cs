using System.Collections.Generic;

namespace FG5eXmlToPDF.Models
{
    public class Weapon
    {
        public string Name { get; set; }
        public List<Damage> Damages { get; set;}
        public string AttackStat { get; set; }
        public string AttackBonus { get; set; }
        public string Type { get; internal set; }

        public Weapon()
        {
            //Damages = new List<Damage>();

        }
    }
    
}