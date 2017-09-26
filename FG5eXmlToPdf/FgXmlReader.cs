using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using FG5eXmlToPDF.Models;

namespace FG5eXmlToPDF
{
    public class Fg5eXmlReader
    {
        private readonly string _fileString;
        
        public Fg5eXmlReader(string fileString)
        {
            _fileString = fileString;
        }

        public Character Get()
        {
            var xml = XDocument.Load(_fileString);
            var name = xml?.Root?.Element("character")?.Element("name")?.Value;
            return new Character()
            {
                Name = name
            };
        }

    }
}
