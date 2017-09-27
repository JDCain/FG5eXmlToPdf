using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FG5eXmlToPdf;
using FG5eXmlToPDF.Models;
using iTextSharp.text.pdf;

namespace FG5eXmlToPDF
{
    public static class Fg5ePdf
    {
        public static void Write(string template, Character5e character, string outFile)
        {
            var pdfReader = new PdfReader(template);
            var stamper = new PdfStamper(pdfReader, new FileStream(outFile, FileMode.Create));
            var form = stamper.AcroFields;

            foreach (var prop in character.Properties)
            {
                form.SetField(prop.Key, prop.Value);
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
                //ability.Name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(ability.Name);
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
            stamper.Close();
        }


        private static Ability GetAbilityByName(Character5e character, string abulity)
        {
            return character.Abilities.FirstOrDefault(x => x.Name == abulity);
        }
    }
}
