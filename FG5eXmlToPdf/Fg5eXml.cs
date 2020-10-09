﻿using System;
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
        public static List<ICharacter> LoadCharacters(string filePath)
        {
            var theXML = XDocument.Load(filePath);
            if (theXML == null)
                throw new Exception("FG5eXml: xml not loaded!");
            var characters = from elem in theXML.Root.Elements() where elem.Name == "character" select elem;
            var chars = new List<ICharacter>();
            foreach (var character in characters)
            { 
                chars.Add(LoadCharacter(character));
            }
            return chars;
        }
        private static ICharacter LoadCharacter(XElement charElement)
        {
            // Not overly elegant, but will do for minimal code changes
            _charElement = charElement ?? throw new Exception("characterElement is null");

            ICharacter character = new Character5e();
            GetProperties(character);

            var abilityList = XPathElementList("abilities");
            GetAbilites(abilityList, character);

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
                    try
                    {
                        character.Inventory.Add(new GenericItem()
                        {
                            Name = item.Element("name").Value,
                            Text = item.Element("count").Value
                        });
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Could not parse Inventory Item: {0}", item);
                    }
                }
            }
            var powerGroupList = XPathElementList("powergroup");
            if (powerGroupList != null)
            {
                foreach (var powerGroup in powerGroupList)
                {
                    try
                    {
                        character.PowerGroup.Add(new PowerGroup()
                        {
                            Name = powerGroup.Element("name")?.Value,
                            Stat = powerGroup.Element("stat")?.Value,
                        });
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Could not parse PowerGroup: {0}", powerGroup);
                    }
                }
                var powerList = XPathElementList("powers");

                foreach (var power in powerList)
                {
                    try
                    {
                        getPower(character, power);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Could not parse power: {0}", power);
                    }
                }
            }
            var powerSlotsList = XPathElementList("powermeta");
            foreach (var slotElement in powerSlotsList)
            {
                try
                {
                    character.PowerSlots.Add(new GenericItem()
                    {
                        Name = slotElement.Name.ToString(),
                        Text = slotElement.Elements("max").First().Value

                    });
                }
                catch (Exception)
                {
                    Console.WriteLine("Could not parse PowerMeta: {0}", slotElement);
                }
            }
            
            return character;

        }

        private static List<XElement> XPathElementList(string xpath)
        {
            return _charElement?.XPathSelectElement(xpath)?.Elements()?.ToList();
        }

        private static void PopulateGenericList(List<XElement> gList, List<GenericItem> itemList)
        {
            gList?.ForEach(x =>
            {
                if (x.HasElements)
                {
                    itemList.Add(GenericItemMaker(x));
                }
            });
        }
        private static GenericItem GenericItemMaker(XElement features)
        {
            if (features.HasElements)
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
            else
            {
                return null;
            }
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

        private static void getPower(ICharacter inputCharacter, XElement power)
        {
            var powerGroup = inputCharacter.PowerGroup.FirstOrDefault(x =>
                string.Equals(x.Name, power.Element("group").Value, StringComparison.OrdinalIgnoreCase));
            powerGroup.Powers.Add(new Power()
            {
                Name = power.Element("name").Value,
                Level = int.Parse((power.Element("level").Value)),
                Prepaired = Helper.StringIntToBool(((power.Element("prepared").Value))),
                Group = power.Element("group").Value,

            });
        }

        private static void GetWeapons(List<XElement> weaponList, ICharacter character)
        {
            foreach (var weapon in weaponList)
            {
                try
                {

                    var damageList = weapon.Element("damagelist").Elements().ToList();
                    var damages = new List<Damage>();
                    foreach (var damage in damageList)
                    {
                        damages.Add(new Damage()
                        {
                            Type = damage?.Element("type")?.Value,
                            Stat = damage?.Element("stat")?.Value,
                            Dice = GetDice(damage?.Element("dice")?.Value),
                            Bonus = damage?.Element("bonus")?.Value ?? "0"
                        });
                    }
                    character.Weapons.Add(new Weapon()
                    {
                        Name = weapon.Element("name").Value,
                        AttackStat = weapon?.Element("attackstat")?.Value ?? String.Empty,
                        AttackBonus = int.Parse(weapon?.Element("attackbonus")?.Value ?? "0"),
                        Type = int.Parse(weapon.Element("type")?.Value ?? "0"),
                        Prof = Helper.StringIntToBool(weapon.Element("prof")?.Value ?? "0"),
                        Damages = damages
                    });
                }
                catch (Exception)
                {
                    Console.WriteLine("Could not parse Weapon: {0}", weapon);
                }
            }
        }

        private static void GetCharClasses(List<XElement> classList, ICharacter character)
        {
            foreach (var charClass in classList)
            {
                int.TryParse(charClass.Element("casterlevelinvmult")?.Value, out int casterlevelinvmult);
                int.TryParse(charClass.Element("casterpactmagic")?.Value, out int casterpactmagic);
                character.Classes.Add(new Class()
                {
                    Name = charClass.Element("name").Value,
                    Level = charClass.Element("level").Value,
                    CasterLevelinvmult = casterlevelinvmult,
                    CasterPactMagic = casterpactmagic
                });
            }
        }

        private static void GetSkills(List<XElement> skillList, ICharacter character)
        {
            foreach (var skill in skillList)
            {
                try
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
                catch (Exception)
                {
                    Console.WriteLine("Could not parse Skill: {0}", skill);
                }
            }
        }

        private static void GetAbilites(List<XElement> abilityList, ICharacter character)
        {
            foreach (var attrib in abilityList)
            {
                try
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
                catch (Exception)
                {
                    Console.WriteLine("Could not parse Ability: {0}", attrib);
                }
            }
        }

        private static void GetProperties(ICharacter character)
        {
            var props = character.Properities;
            foreach (var prop in props)
            {
                try
                {
                    prop.Value = GetCharValue(prop.XmlPath);
                }
                catch (Exception)
                {
                    Console.WriteLine("Could not parse Property: {0}", prop);
                }
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
