using System.Collections.Generic;

namespace FG5eXmlToPDF.Models
{
    public class Weapon
    {
        public string Name { get; set; }
        public List<Damage> Damages { get; set;}
        public string AttackStat { get; set; }
        public int? AttackBonus { get; set; }
        public int Type { get; set; }
        public bool Prof { get; set; }

        public Weapon()
        {
            //Damages = new List<Damage>();

        }
    }
    
}