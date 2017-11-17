using System.Collections.Generic;

namespace FG5eXmlToPDF.Models
{
    public class Power
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public bool Prepaired { get; set; }
        public string Group { get; set; }
    }

    public class PowerGroup
    {
        public int? AtkMod { get; set; }
        public int? AtkProf { get; set; }
        public string Name { get; set; }
        public int? Prepared { get; set; }
        public int? Savemod { get; set; }
        public int? SaveProf { get; set; }
        public int? Uses { get; set; }
        public List<Power> Powers { get; }
        public string Stat { get; set; }

        public PowerGroup()
        {
            Powers = new List<Power>();
        }
    }
}