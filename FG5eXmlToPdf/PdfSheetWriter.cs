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
    public class PdfSheetWriter
    {
        public void Write(string template, Character character)
        {

            PdfReader pdfReader = new PdfReader(template);
            var stamper = new PdfStamper(pdfReader, new FileStream(@"c:\source\repo\FG5eXmlToPDF\FG5eXmlToPDF.Tests\out.pdf", FileMode.Create));
            AcroFields form = stamper.AcroFields;
            form.SetField("CharacterName", character.Name);
            stamper.Close();
        }

        public void FixPdf()
        {
            PdfReader reader = new PdfReader(@"c:\source\repo\FGxmlToPDF\FGxmlToPDF\FGxmlToPDF\DnD_5E_CharacterSheet - Form Fillable.pdf");
            PdfDictionary root = reader.Catalog;
            PdfDictionary form = root.GetAsDict(PdfName.ACROFORM);
            PdfArray fields = form.GetAsArray(PdfName.FIELDS);
            PdfDictionary page;
            PdfArray annots;
            for (int i = 1; i <= reader.NumberOfPages; i++)
            {
                page = reader.GetPageN(i);
                annots = page.GetAsArray(PdfName.ANNOTS);
                for (int j = 0; j < annots.Size; j++)
                {
                    fields.Add(annots.GetAsIndirectObject(j));
                }
            }
            PdfStamper stamper = new PdfStamper(reader, new FileStream(@"c:\source\repo\FGxmlToPDF\FGxmlToPDF\FGxmlToPDF\DnD_5E_CharacterSheet - Form Fillable (corrected).pdf", FileMode.Create));
            stamper.Close();
            reader.Close();
        }
    }
}
