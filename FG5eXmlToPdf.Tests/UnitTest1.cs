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
            var reader = new Fg5eXmlReader($@"{currentDirectory}\rita.xml");
            var character = reader.Get();

            var writer = new PdfSheetWriter();
            //writer.FixPdf();
            writer.Write($@"{currentDirectory}\DnD_5E_CharacterSheet - Form Fillable.pdf", 
                character,
                $@"{currentDirectory}\out.pdf");
        }
    }
}
