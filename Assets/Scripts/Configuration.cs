using UnityEngine;
using System.Collections;

namespace HomeBuilder
{
    public class Configuration
    {

        readonly static public string name       = "HomeBuilder";
        readonly static public string version    = "0.1";
        
        public class Scenes
        {
            readonly static public string menuScene = "Menu";
            readonly static public string questioningScene = "Questioning";
            readonly static public string designingScene = "Designing";
            readonly static public string historyScene = "History";
        }

        public class Appartment
        {
            readonly static public int minSquare = 50;
            readonly static public int maxSquare = 350;
            readonly static public int maxModuleOfType = 2;

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

            readonly static public ModuleParams[] approvedModules = new ModuleParams[] {
                new ModuleParams("Kitchen"   , 8,    2, 4, Color.red),
                new ModuleParams("Bathroom" , 6,    2, 3, Color.green),
                new ModuleParams("Hall", 12,   3, 4, Color.blue),
                new ModuleParams("Bedroom"  , 16,   4, 4, Color.gray)
            };

            public class ModuleParams
            {
                readonly public string name;
                readonly public float minSquare;
                readonly public float minWidth;
                readonly public float minHeight;
                readonly public Color color;

                public ModuleParams(string name, float sq, float w, float h, Color color)
                {
                    this.name   = name;
                    minSquare   = sq;
                    minWidth    = w;
                    minHeight   = h;
                    this.color  = color;
                }
            }
            
        }
    }
}
