using UnityEngine;
using System.Collections;

namespace HomeBuilder
{
    public class Configuration
    {

        readonly static public string name       = "HomeBuilder";
        readonly static public string version    = "0.10";
        
        public class Scenes
        {
            readonly static public string menuScene     = "Menu";
            readonly static public string styleScene    = "Style";
            readonly static public string modulesScene  = "Modules";
            readonly static public string designingScene = "Designing";
            readonly static public string historyScene   = "History";
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

            public class ModuleParams
            {
                public readonly string name;
                public readonly float minSquare;
                public readonly float minWidth;
                public readonly float minHeight;

                public ModuleParams(string name, float sq, float w, float h)
                {
                    this.name   = name;
                    minSquare   = sq;
                    minWidth    = w;
                    minHeight   = h;
                }
            }
            
        }
    }
}
