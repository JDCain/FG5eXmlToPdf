using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using FG5eXmlToPDF;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FG5eXmlToPdf.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private object _charElement;

        [TestMethod]
        public void ReadWriteTest()
        {
            try
            {
                var currentDirectory = System.IO.Directory.GetCurrentDirectory();
                foreach (var file in Directory.EnumerateFiles(currentDirectory, "*.xml"))
                {
                    var xml = XDocument.Load(file);
                    if (xml?.Root?.Element("character") == null) continue;
                    var character = FG5eXml.LoadCharacter(file);
                    var charName = character.Properities.FirstOrDefault((x) => x.Name == "Name")?.Value;
                    var level = character.Properities.FirstOrDefault((x) => x.Name == "LevelTotal")?.Value;
                    FG5ePdf.Write(
                        character,
                        $@"{currentDirectory}\{charName} ({level}).pdf");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
