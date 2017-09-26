using System;
using FG5eXmlToPDF;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FG5eXmlToPdf.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var reader = new Fg5eXmlReader(@"c:\source\repo\FG5eXmlToPDF\FG5eXmlToPDF.Tests\rita.xml");
            var character = reader.Get();

            var writer = new PdfSheetWriter();
            //writer.FixPdf();
            writer.Write(@"C:\Source\repo\FG5eXmlToPdf\FG5eXmlToPdf\DnD_5E_CharacterSheet - Form Fillable.pdf", character);
        }
    }
}
