using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            form.SetField("CharacterName", character.Name);
            form.SetField("STR", $"{GetAbilityByName(character, "strength").Score}");
            form.SetField("STRmod", $"{GetAbilityByName(character, "strength").Bonus}");
            form.SetField("ST Strength", $"{GetAbilityByName(character, "strength").Save}");
            form.SetField("Check Box 11", OnOrOff(GetAbilityByName(character, "strength").Saveprof));
            stamper.Close();
        }

        private static string OnOrOff(int s)
        {
            return s == 1 ? "Yes" : "No";
        }

        private static Ability GetAbilityByName(Character5e character, string abulity)
        {
            return character.Abilities.FirstOrDefault(x => x.Name == abulity);
        }
    }
}
