using System;
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
            var character = Fg5eXml.LoadCharacter($@"{currentDirectory}\rita.xml");
            Fg5ePdf.Write($@"{currentDirectory}\DnD_5E_CharacterSheet - Form Fillable.pdf", 
                character,
                $@"{currentDirectory}\out.pdf");
        }
    }
}
