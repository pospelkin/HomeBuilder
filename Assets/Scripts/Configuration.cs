using UnityEngine;
using System.Collections;

namespace HomeBuilder
{
    public class Configuration
    {

        static public string name       = "HomeBuilder";
        static public string version    = "0.1";
        
        public class Scenes
        {
            static public string menuScene = "Menu";
            static public string questioningScene = "Questioning";
            static public string designingScene = "Designing";
            static public string historyScene = "History";
        }

        public class Appartment
        {
            static public int minSquare = 10;
            static public int maxModuleOfType = 2;

            public enum Styles { MODERN, CLASSIC, OLD }
            static public string GetStyle(Styles style)
            {
                switch (style)
                {
                    case Styles.CLASSIC:
                        return "Classic";
                    case Styles.MODERN:
                        return "Modern";
                    case Styles.OLD:
                        return "Old";
                    default:
                        return "Noname";
                }
            }
            
        }
    }
}
