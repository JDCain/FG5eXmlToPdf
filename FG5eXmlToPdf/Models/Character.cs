using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace FG5eXmlToPDF.Models
{
    public class Character
    {
        public List<Ability> Abilities { get; set; }
        public string Background { get; set; }
        public string Name { get; set; }
        public int ProfBonus { get; set; }

    }
}
