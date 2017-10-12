using FG5eXmlToPdf;
using FG5eXmlToPDF.Models;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FG5eXmlToPDF
{
    public static class FG5ePdf
    {
        public static void Write(ICharacter character, string outFile)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FG5eXmlToPdf.DnD_5E_CharacterSheet - Form Fillable.pdf");
            var pdfReader = new PdfReader(stream);
            var stamper = new PdfStamper(pdfReader, new FileStream(outFile, FileMode.Create));
            var form = stamper.AcroFields;

            var levels = GetLevels(character);
            form.SetField("ClassLevel", levels);

            SetProperties(character, form);
            SetAbulities(character, form);
            SetSkills(character, form);
            SetWeapons(character, form);
            SetProfLang(character, form);
            SetFeatureTraits(character, form);
            SetFeats(character, form);
            SetEquipment(character, form);
            SetDetail(character, form);
            if (character.Powers.Count > 0)
            {
                for (var level = 0; level <= character.Powers.Max(x => x.Level); level++)
                {
                    var n = 0;
                    //var take = level == 0 ? 8 : 11;
                    foreach (var spell in character.Powers.Where(x => x.Level == level))
                    {
                        form.SetField($"Spell-{level}-{n}", spell.Name);
                        form.SetField($"Prepaired_Spell-{level}-{n}", Helper.BoolToYesNo(spell.Prepaired));
                        n++;
                    }
                }
            }

            stamper.Close();
        }

        private static void SetDetail(ICharacter character, AcroFields form)
        {
            form.SetField("Text1", GenericItemListToTextBox("Features", character.Features, Environment.NewLine)
                                   + GenericItemListToTextBox("Traits", character.Traits, Environment.NewLine)
                                   + GenericItemListToTextBox("Feats", character.Features, Environment.NewLine));
        }

        private static void SetEquipment(ICharacter character, AcroFields form)
        {
            var y = string.Empty;
            var inventory =
                character.Inventory.Aggregate(y, (current, item) => current + $"{item.Name} ({item.Text}), ");
            form.SetField("Equipment", inventory.Trim().TrimEnd(','));
        }

        private static void SetFeats(ICharacter character, AcroFields form)
        {
            var feats = GenericItemListToTitles("Feats", character.Features, Environment.NewLine);
            form.SetField("Feat+Traits", feats);
        }

        private static void SetFeatureTraits(ICharacter character, AcroFields form)
        {
            var featuresTraits = GenericItemListToTitles("Features", character.Features, Environment.NewLine) +
                                 GenericItemListToTitles("Traits", character.Traits, Environment.NewLine);
            form.SetField("Features and Traits", featuresTraits);
        }

        private static void SetProfLang(ICharacter character, AcroFields form)
        {
            var proficienciesLang = MakeTextBlock("Proficiencies", character.Proficiencies, Environment.NewLine) +
                                    MakeTextBlock("Languages", character.Languages, ", ");
            form.SetField("ProficienciesLang", proficienciesLang.Trim().TrimEnd(','));
        }

        private static void SetWeapons(ICharacter character, AcroFields form)
        {
            var n = 1;
            foreach (var weapon in character.Weapons.Take(3))
            {
                form.SetField($"Wpn{n} Name", weapon.Name);
                var statSearch = StatSearch(weapon);

                var attack = character.Abilities.FirstOrDefault(x => x.Name == statSearch)?.Bonus + weapon.AttackBonus;

                if (weapon.Prof)
                {
                    if (int.TryParse(character.Properities?.FirstOrDefault(x => x.Name == "ProfBonus")?.Value, out var intOut))
                    {
                        attack += intOut;
                    }
                }
                form.SetField($"Wpn{n} AtkBonus", $"{attack}");

                var damageString = string.Empty;
                foreach (var damage in weapon.Damages)
                {
                    damageString = string.IsNullOrWhiteSpace(damageString) ? "" : damageString + " & ";
                    damageString +=
                        $"{damage.Dice} + {character.Abilities.FirstOrDefault(x => x.Name == (damage.Stat == "base" ? StatSearch(weapon) : damage.Stat))?.Bonus + int.Parse(damage.Bonus)} {damage.Type}";
                }
                form.SetField($"Wpn{n} Damage", damageString);
                n++;
            }
        }

        private static void SetSkills(ICharacter character, AcroFields form)
        {
            foreach (var skill in character.Skills)
            {
                form.SetField(skill.Name, $"{skill.Total}");
                form.SetField($"{skill.Name} Check Box", Helper.BoolToYesNo(skill.Prof));
            }
        }

        private static void SetAbulities(ICharacter character, AcroFields form)
        {
            var saveCheckBoxMap = new Dictionary<string, string>()
            {
                {"strength", "Check Box 11"},
                {"dexterity", "Check Box 18"},
                {"constitution", "Check Box 19"},
                {"intelligence", "Check Box 20"},
                {"wisdom", "Check Box 21"},
                {"charisma", "Check Box 22"}
            };
            foreach (var ability in character.Abilities)
            {
                var threeLetter = ability.Name.Substring(0, 3).ToUpper();
                form.SetField(threeLetter, $"{ability.Score}");
                form.SetField($"{threeLetter}mod", $"{ability.Bonus}");
                form.SetField($"ST {CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ability.Name)}", $"{ability.Save}");
                form.SetField(saveCheckBoxMap[ability.Name], Helper.BoolToYesNo(ability.Saveprof));
            }
        }

        private static void SetProperties(ICharacter character, AcroFields form)
        {
            foreach (var prop in character.Properities)
            {
                form.SetField(prop.Name, prop.Value);
            }
            form.SetField("CharacterName 2", character.Properities.FirstOrDefault(x => x.Name == "Name")?.Value);
        }

        private static string GetLevels(ICharacter character)
        {
            var levels = string.Empty;
            foreach (var charClass in character.Classes)
            {
                if (levels != string.Empty)
                {
                    levels += " / ";
                }
                levels += $"{charClass.Name} ({charClass.Level})";
            }
            return levels;
        }

        #region HelperFunctions
        private static string GenericItemListToTitles(string title, List<GenericItem> list, string seperator)
        {
            var result = $"[{title}]{Environment.NewLine}";
            list.ForEach(x => result += $"{x.Name}{seperator}");
            return result;
        }

        private static string GenericItemListToTextBox(string title, List<GenericItem> list, string seperator)
        {
            var result = $"[{title}]{Environment.NewLine}";
            list.ForEach(x => result += $"{x.Name}: {x.Text}{seperator}");
            return result;
        }

        private static string MakeTextBlock(string title, IEnumerable<string> list, string seperator)
        {
            var result = $"[{title}]{Environment.NewLine}";
            return list.Aggregate(result, (current, item) => current + (item + seperator));
        }

        private static string StatSearch(Weapon weapon)
        {
            string result = null;
            if (string.IsNullOrEmpty(weapon.AttackStat))
            {
                result = weapon.Type == 0 ? "strength" : "dexterity";
            }
            else
            {
                result = weapon.AttackStat;
            }
            return result;
        }

        private static Ability GetAbilityByName(Character5e character, string abulity)
        {
            return character.Abilities.FirstOrDefault(x => x.Name == abulity);
        } 
        #endregion
    }
}
