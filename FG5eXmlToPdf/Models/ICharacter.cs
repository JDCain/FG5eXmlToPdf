using System.Collections.Generic;
using FG5eXmlToPdf.Models;

namespace FG5eXmlToPDF.Models
{
    public interface ICharacter
    {
        List<Ability> Abilities { get; }
        List<Class> Classes { get; }
        List<GenericItem> Feats { get; }
        List<GenericItem> Features { get; }
        List<GenericItem> Inventory { get; }
        List<string> Languages { get; }
        List<string> Proficiencies { get; }
        List<Properity> Properities { get; }
        List<Skill> Skills { get; }
        List<GenericItem> Traits { get; }
        List<Weapon> Weapons { get; }
    }
}