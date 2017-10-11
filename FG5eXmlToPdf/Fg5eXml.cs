using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.XPath;
using FG5eXmlToPdf;
using FG5eXmlToPdf.Models;
using FG5eXmlToPDF.Models;
// ReSharper disable All

namespace FG5eXmlToPDF
{
    public static class FG5eXml
    {

        public static ICharacter LoadCharacter(string fileString)
        {
            var xml = XDocument.Load(fileString);
            _charElement = xml?.Root?.Element("character");
            
            ICharacter character = new Character5e();
            GetProperties(character);

            var abilityList = _charElement?.Element("abilities").Elements().ToList();
            GetAbulites(abilityList, character);

            var skillList = _charElement?.Element("skilllist").Elements().ToList();
            GetSkills(skillList, character);

            var classList = _charElement?.XPathSelectElement("classes").Elements().ToList();
            GetCharClasses(classList, character);

            var weaponList = _charElement?.XPathSelectElement("weaponlist").Elements().ToList();
            GetWeapons(weaponList, character);

            var proficiencyList = _charElement?.XPathSelectElement("proficiencylist").Elements().ToList();
            GetProf(proficiencyList, character);

            var languageList = _charElement?.XPathSelectElement("languagelist").Elements().ToList();
            GetLanguage(languageList, character);

            var traitList = _charElement?.XPathSelectElement("traitlist").Elements().ToList();
            PopulateGenericList(traitList, character.Traits);

            var featList = _charElement?.XPathSelectElement("featlist").Elements().ToList();
            PopulateGenericList(featList, character.Feats);

            var featuresList = _charElement?.XPathSelectElement("featurelist").Elements().ToList();
            PopulateGenericList(featuresList, character.Features);

            var inventoryList = _charElement?.XPathSelectElement("inventorylist").Elements().ToList();
            foreach (var item in inventoryList)
            {
                character.Inventory.Add(new GenericItem()
                {
                    Name = item.Element("name").Value,
                    Text = item.Element("count").Value
                });
            }

            return character;
        }

        private static void PopulateGenericList(List<XElement> traitList, List<GenericItem> itemList)
        {
            traitList.ForEach(x => itemList.Add(GenericItemMaker(x)));
        }
        private static GenericItem GenericItemMaker(XElement features)
        {
            var text = string.Empty;
            foreach (var line in features.Element("text").Elements().ToList())
            {
                text += line.Value + Environment.NewLine;
            }

            return new GenericItem()
            {
                Name = features.Element("name").Value,
                Text = text,
            };
        }

        private static void GetLanguage(List<XElement> languageList, ICharacter character)
        {
            foreach (var language in languageList)
            {
                character.Languages.Add(language.Value);
            }
        }

        private static void GetProf(List<XElement> proficiencyList, ICharacter character)
        {
            foreach (var proficiency in proficiencyList)
            {
                character.Proficiencies.Add(proficiency.Value);
            }
        }

        private static void GetWeapons(List<XElement> weaponList, ICharacter character)
        {
            foreach (var weapon in weaponList)
            {
                var damageList = weapon.Element("damagelist").Elements().ToList();
                var damages = new List<Damage>();
                foreach (var danage in damageList)
                {
                    damages.Add(new Damage()
                    {
                        Type = danage?.Element("type")?.Value,
                        Stat = danage?.Element("stat")?.Value,
                        Dice = GetDice(danage?.Element("dice")?.Value),
                        Bonus = danage?.Element("bonus")?.Value
                    });
                }
                character.Weapons.Add(new Weapon()
                {
                    Name = weapon.Element("name").Value,
                    AttackStat = weapon?.Element("attackstat")?.Value ?? String.Empty,
                    AttackBonus = int.Parse(weapon.Element("attackbonus").Value),
                    Type = int.Parse(weapon.Element("type").Value),
                    Prof = Helper.StringIntToBool(weapon.Element("prof").Value),
                    Damages = damages
                });
            }
        }

        private static void GetCharClasses(List<XElement> classList, ICharacter character)
        {
            foreach (var charClass in classList)
            {
                character.Classes.Add(new Class()
                {
                    Name = charClass.Element("name").Value,
                    Level = charClass.Element("level").Value,
                });
            }
        }

        private static void GetSkills(List<XElement> skillList, ICharacter character)
        {
            foreach (var skill in skillList)
            {
                character.Skills.Add(new Skill()
                {
                    Name = skill.Element("name").Value,
                    Misc = int.Parse(skill.Element("misc").Value),
                    Prof = Helper.StringIntToBool(skill.Element("prof").Value),
                    Stat = skill.Element("stat").Value,
                    Total = int.Parse(skill.Element("total").Value)
                });
            }
        }

        private static void GetAbulites(List<XElement> abilityList, ICharacter character)
        {
            foreach (var attrib in abilityList)
            {
                character.Abilities.Add(new Ability()
                {
                    Name = attrib.Name.ToString(),
                    Score = int.Parse(attrib.Element("score").Value),
                    Bonus = int.Parse(attrib.Element("bonus").Value),
                    Save = int.Parse(attrib.Element("save").Value),
                    Savemodifier = int.Parse(attrib.Element("savemodifier").Value),
                    Saveprof = Helper.StringIntToBool(attrib.Element("saveprof").Value)
                });
            }
        }

        private static void GetProperties(ICharacter character)
        {
            var props = character.Properities;
            foreach (var prop in props)
            {
                prop.Value = GetCharValue(prop.XmlPath);
            }
        }

        private static string GetDice(string diceString)
        {
            var result = string.Empty;
            var array = diceString.Split(',');
            var count = array
                .GroupBy(e => e).ToList();
            foreach (var dieSet in count)
            {
                result = string.IsNullOrEmpty(result) ? "" : result + ", ";
                result += $"{dieSet.Count()}{dieSet.Key}";
                
            }
            return result;
        }

        private static string GetCharValue(string name)
        {
            //everything is lowercase as far as I can tell in the xml
            return _charElement.XPathSelectElement(name.ToLower())?.Value.TrimStart('0') ?? string.Empty;
        }

        private static XElement _charElement;
    }
}
