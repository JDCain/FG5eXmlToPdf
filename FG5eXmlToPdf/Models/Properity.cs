namespace FG5eXmlToPDF.Models
{
    public class Properity
    {
        public string Name { get; set; }
        public string XmlPath { get; set; }
        public string Value { get; set; }

        public Properity()
        {
            
        }

        public Properity(string name, string xpath = "")
        {
            Name = name;
            XmlPath = xpath.Contains("/") ? xpath : Name.ToLower();
        }
    }
}