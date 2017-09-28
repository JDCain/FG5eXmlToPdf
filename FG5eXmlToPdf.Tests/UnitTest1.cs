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
            var character = FG5eXml.LoadCharacter($@"{currentDirectory}\rita.xml");
            FG5ePdf.Write( 
                character,
                $@"{currentDirectory}\out.pdf");
        }
    }
}
