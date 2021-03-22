﻿using System;
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
        private readonly string _sourceDirectory = $@"{Directory.GetCurrentDirectory()}\XML";
        //[TestMethod]
        public void ReadWriteAllTest()
        {               
            foreach (var file in Directory.EnumerateFiles(_sourceDirectory, "*.xml"))
            {
                GenerateSheet(file, _sourceDirectory);
            }
        }

        private static void GenerateSheet(string file, string currentDirectory)
        {
            try
            {
                var xml = XDocument.Load(Path.Combine(currentDirectory, file));
                if (xml?.Root?.Element("character") == null) return;
                var characters = FG5eXml.LoadCharacters(Path.Combine(currentDirectory, file));
                var character = characters.First();
                var charName = character.Properities.FirstOrDefault((x) => x.Name == "Name")?.Value;
                var level = character.Properities.FirstOrDefault((x) => x.Name == "LevelTotal")?.Value;
                FG5ePdf.Write(
                    character,
                    $@"{currentDirectory}\{charName} ({level}).pdf");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [TestMethod]
        public void RitaTest()
        {
            GenerateSheet("rita.xml", _sourceDirectory);
        }
        [TestMethod]
        public void ChuffyTest()
        {
            GenerateSheet("chuffy.xml", _sourceDirectory);
        }
        [TestMethod]
        public void PoogTest()
        {
            GenerateSheet("poog.xml", _sourceDirectory);
        }
        [TestMethod]
        public void MogmurchTest()
        {
            GenerateSheet("mogmurch.xml", _sourceDirectory);
        }
        [TestMethod]
        public void SarahTest()
        {
            GenerateSheet("sarah.xml", _sourceDirectory);
        }
        [TestMethod]
        public void TannerTest()
        {
            GenerateSheet("tanner.xml", _sourceDirectory);
        }
        [TestMethod]
        public void RavinaTest()
        {
            GenerateSheet("ravina.xml", _sourceDirectory);
        }
        [TestMethod]
        public void Faffner20170928Test()
        {
            GenerateSheet("faffner20170928.xml", _sourceDirectory);
        }

        [TestMethod]
        public void Faffner()
        {
            GenerateSheet("faffner.xml", _sourceDirectory);
        }

        [TestMethod]
        public void RandomWarlockTest()
        {
            GenerateSheet("randomWarlock.xml", _sourceDirectory);
        }

        [TestMethod]
        public void JandarTest()
        {
            GenerateSheet("JandarUlmVrass.xml", _sourceDirectory);
        }

        [TestMethod]
        public void Char1Test()
        {
            GenerateSheet("char1.xml", _sourceDirectory);
        }

        [TestMethod]
        public void Char2Test()
        {
            GenerateSheet("char2.xml", _sourceDirectory);
        }

        [TestMethod]
        public void Char3Test()
        {
            GenerateSheet("char3.xml", _sourceDirectory);
        }

        [TestMethod]
        public void Char4Test()
        {
            GenerateSheet("char4.xml", _sourceDirectory);
        }

        [TestMethod]
        public void Char5Test()
        {
            GenerateSheet("char5.xml", _sourceDirectory);
        }
    }
}
