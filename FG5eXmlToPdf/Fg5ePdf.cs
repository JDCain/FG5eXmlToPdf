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

                var attack = character.Abilities.FirstOrDefault(x => x.Name == statSearch)?.Bonus +
                                 weapon.AttackBonus;
                
                if (weapon.Prof)
                {
                    if (int.TryParse(character.Properities?.FirstOrDefault(x => x.Name == "ProfBonus")?.Value, out int intOut))
                    {
                        attack += intOut;
                    }
                    
                }
                form.SetField($"Wpn{n} AtkBonus",$"{attack}");

                
                form.SetField($"Wpn{n} Damage", $"{weapon.Damages[0].Dice} + {character.Abilities.FirstOrDefault(x=>x.Name == weapon.Damages[0].Stat)?.Bonus + int.Parse(weapon.Damages[0].Bonus)} {weapon.Damages[0].Type}");
                n++;
            }
            stamper.Close();
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
