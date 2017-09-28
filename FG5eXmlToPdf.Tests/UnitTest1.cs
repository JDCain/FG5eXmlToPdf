using System;
using System.Linq;
using FG5eXmlToPDF;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FG5eXmlToPdf.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ReadWriteTest()
        {           
            var currentDirectory = System.IO.Directory.GetCurrentDirectory();
            var character = FG5eXml.LoadCharacter($@"{currentDirectory}\rita.xml");
            var charName = character.Properities.FirstOrDefault((x) => x.Name == "Name")?.Value;
            var level = character.Properities.FirstOrDefault((x) => x.Name == "LevelTotal")?.Value;
            FG5ePdf.Write( 
                character,
                $@"{currentDirectory}\{charName} ({level}).pdf");
        }
    }
}
