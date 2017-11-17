using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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

            var abilityList = XPathElementList("abilities");
            GetAbulites(abilityList, character);

            var skillList = XPathElementList("skilllist");
            GetSkills(skillList, character);

            var classList = XPathElementList("classes");
            GetCharClasses(classList, character);

            var weaponList = XPathElementList("weaponlist");
            GetWeapons(weaponList, character);

            var proficiencyList = XPathElementList("proficiencylist");
            GetProf(proficiencyList, character);

            var languageList = XPathElementList("languagelist");
            GetLanguage(languageList, character);

            var traitList = XPathElementList("traitlist");
            PopulateGenericList(traitList, character.Traits);

            var featList = XPathElementList("featlist");
            PopulateGenericList(featList, character.Feats);

            var featuresList = XPathElementList("featurelist");
            PopulateGenericList(featuresList, character.Features);

            var inventoryList = XPathElementList("inventorylist");
            if (inventoryList != null)
            {
                foreach (var item in inventoryList)
                {
                    character.Inventory.Add(new GenericItem()
                    {
                        Name = item.Element("name").Value,
                        Text = item.Element("count").Value
                    });
                }
            }
            var powerList = XPathElementList("powers");
            foreach (var power in powerList)
            {
                character.Powers.Add(new Power()
                {
                    Name = power.Element("name").Value,
                    Level = int.Parse((power.Element("level").Value)),
                    Prepaired = Helper.StringIntToBool(((power.Element("prepared").Value))),
                    Group = power.Element("group").Value,

                });
            }
            var powerSlotsList = XPathElementList("powermeta");
            foreach (var slotElement in powerSlotsList)
            {
                character.PowerSlots.Add(new GenericItem()
                {
                    Name = slotElement.Name.ToString(),
                    Text = slotElement.Elements("max").First().Value
                    
                });
            }
            
            return character;
        }

        private static List<XElement> XPathElementList(string xpath)
        {
            return _charElement?.XPathSelectElement(xpath)?.Elements()?.ToList();
        }

        private static void PopulateGenericList(List<XElement> traitList, List<GenericItem> itemList)
        {
            traitList?.ForEach(x => itemList.Add(GenericItemMaker(x)));
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
                        Bonus = danage?.Element("bonus")?.Value ?? "0"
                    });
                }
                character.Weapons.Add(new Weapon()
                {
                    Name = weapon.Element("name").Value,
                    AttackStat = weapon?.Element("attackstat")?.Value ?? String.Empty,
                    AttackBonus = int.Parse(weapon?.Element("attackbonus")?.Value ?? "0"),
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
                    CasterLevelinvmult = int.Parse(charClass.Element("casterlevelinvmult").Value),
                    CasterPactMagic = int.Parse(charClass.Element("casterpactmagic").Value)
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
            return _charElement.XPathSelectElement(name.ToLower())?.Value.TrimStart('0').Replace(@"\n", Environment.NewLine) ?? string.Empty;
        }

        private static XElement _charElement;
    }
}
