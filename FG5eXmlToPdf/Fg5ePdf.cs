using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FG5eXmlToPdf;
using FG5eXmlToPDF.Models;
using iTextSharp.text.pdf;

namespace FG5eXmlToPDF
{
    public static class FG5ePdf
    {
        public static void Write(Character5e character, string outFile)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FG5eXmlToPdf.DnD_5E_CharacterSheet - Form Fillable.pdf");
            var pdfReader = new PdfReader(stream);
            var stamper = new PdfStamper(pdfReader, new FileStream(outFile, FileMode.Create));
            var form = stamper.AcroFields;

            var levels = string.Empty;
            foreach (var charClass in character.Classes)
            {
                if (levels != string.Empty)
                {
                    levels += " / ";
                }
                levels += $"{charClass.Name} ({charClass.Level})";
            }
            form.SetField("ClassLevel", levels);

            foreach (var prop in character.Properities)
            {
                form.SetField(prop.Name, prop.Value);
            }

            var saveCheckBoxMap = new Dictionary<string,string>()
            {
                { "strength", "Check Box 11" },
                { "dexterity", "Check Box 18" },
                { "constitution", "Check Box 19" },
                { "intelligence", "Check Box 20" },
                { "wisdom", "Check Box 21" },
                { "charisma", "Check Box 22" }
            };
            foreach (var ability in character.Abilities)
            {
                var threeLetter = ability.Name.Substring(0, 3).ToUpper();
                form.SetField(threeLetter, $"{ability.Score}");
                form.SetField($"{threeLetter}mod", $"{ability.Bonus}");
                form.SetField($"ST {CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ability.Name)}", $"{ability.Save}");
                form.SetField(saveCheckBoxMap[ability.Name], Helper.BoolToYesNo(ability.Saveprof));
            }
            foreach (var skill in character.Skills)
            {
                form.SetField(skill.Name, $"{skill.Total}");
                form.SetField($"{skill.Name} Check Box", Helper.BoolToYesNo(skill.Prof));
            }
            var n = 1;
            foreach (var weapon in character.Weapons.Take(3))
            {
                form.SetField($"Wpn{n} Name", weapon.Name);
                var statSearch = StatSearch(weapon);

                var attack = character.Abilities.FirstOrDefault(x => x.Name == statSearch)?.Bonus +weapon.AttackBonus;
                
                if (weapon.Prof)
                {
                    if (int.TryParse(character.Properities?.FirstOrDefault(x => x.Name == "ProfBonus")?.Value, out var intOut))
                    {
                        attack += intOut;
                    }
                    
                }
                form.SetField($"Wpn{n} AtkBonus",$"{attack}");

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
            var proficienciesLang = MakeTextBlock("Proficiencies", character.Proficiencies, Environment.NewLine) + MakeTextBlock("Languages", character.Languages, ", " );
            form.SetField("ProficienciesLang", proficienciesLang.Trim().TrimEnd(','));
            var featuresTraits = GenericItemListToTextBox("Features", character.Features, Environment.NewLine) + GenericItemListToTextBox("Traits", character.Traits, Environment.NewLine);
            form.SetField("Features and Traits", featuresTraits);
            var feats = GenericItemListToTextBox("Feats", character.Features, Environment.NewLine);
            form.SetField("Feat+Traits", feats);
            var y = string.Empty;
            var inventory =
                character.Inventory.Aggregate(y, (current, item) => current + $"{item.Name} ({item.Text}), ");
            form.SetField("Equipment", inventory.Trim().TrimEnd(','));
            stamper.Close();
        }

        public static string GenericItemListToTextBox(string title, List<GenericItem> list, string seperator)
        {
            var result = $"[{title}]{Environment.NewLine}";
            list.ForEach(x=> result += $"{x.Name}: {x.Text}{seperator}");
            return result;
        }

        public static string MakeTextBlock(string title, IEnumerable<string> list, string seperator)
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
    }
}
