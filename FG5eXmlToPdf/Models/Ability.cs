using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FG5eXmlToPDF.Models
{
    public class Ability
    {
        public string Name { get; set; }
        public int Bonus { get; set; }
        public int Save { get; set; }
        public int Savemodifier { get; set; }
        public int Saveprof { get; set; }
        public int Score { get; set; }
    }
}
