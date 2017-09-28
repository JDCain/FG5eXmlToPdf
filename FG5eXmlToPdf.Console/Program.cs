using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FG5eXmlToPDF;
using static System.Console;

namespace FG5eXmlToPdf.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args == null | args?.Length == 0)
            {
                WriteLine("Requires arg with path to fantasy grounds character xml");
            }
            else
            {
                if (File.Exists(args[0]))
                {
                    try
                    {
                        var currentDirectory = System.IO.Directory.GetCurrentDirectory();
                        var character = FG5eXml.LoadCharacter(args[0]);
                        var charName = character.Properities.FirstOrDefault((x) => x.Name == "Name")?.Value;
                        var level = character.Properities.FirstOrDefault((x) => x.Name == "LevelTotal")?.Value;
                        var outFile = $@"{currentDirectory}\{charName} ({level}).pdf";
                        FG5ePdf.Write(character,outFile);
                        WriteLine($"Wrote: {outFile}");
                    }
                    catch (Exception e)
                    {
                        WriteLine(e);
                        throw;
                    }

                }
                else
                {
                    WriteLine("Can't find the file");
                }
            }

            
        }
    }
}
