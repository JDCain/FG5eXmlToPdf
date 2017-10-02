﻿using System.Collections.Generic;
using FG5eXmlToPdf.Models;

namespace FG5eXmlToPDF.Models
{
    public class Character5e
    {
        public List<Skill> Skills { get;  }
        public List<Ability> Abilities { get; }
        public List<Properity> Properities { get; } 
        public List<Class> Classes { get;  }
        public List<Weapon> Weapons { get; }
        public Character5e()
        {
            Properities = new List<Properity>()
            {
                //Pdf Field Name, xpath from char if different than name.
                new Properity("AC","defenses/ac/total"),
                new Properity("Name"),
                new Properity("Race"),
                new Properity("Background"),
                new Properity("ProfBonus"),
                new Properity("Alignment"),
                new Properity("Age"),
                new Properity("Bonds"),
                new Properity("Exp"),
                new Properity("Flaws"),
                new Properity("Height"),
                new Properity("Ideals"),
                new Properity("PersonalityTraits"),
                new Properity("Initiative"),
                new Properity("HPMax", "hp/total"),
                new Properity("Passive", "perception"),
                new Properity("Speed", "speed/total"),
                new Properity("Exp"),
                new Properity("LevelTotal", "level")
            };
            Abilities = new List<Ability>();
            Skills = new List<Skill>();
            Classes = new List<Class>();
            Weapons = new List<Weapon>();
           
        }
    }
}