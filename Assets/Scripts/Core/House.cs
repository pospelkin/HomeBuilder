using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HomeBuilder.Core
{

    public class House
    {

        public readonly Module module;
        public readonly List<Module> modules;

        public House(Module m)
        {
            module  = m;
            modules = new List<Module>();
        }

        public class Module
        {
            public readonly string name;
            public readonly List<Style> styles;

            public readonly float minSquare;
            public readonly float minWidth;
            public readonly float minHeight;

            public Module(string n, float ms, float mw, float mh)
            {
                name = n;

                minSquare = ms;
                minWidth  = mw;
                minHeight = mh;

                styles = new List<Style>();
            }

            public string GetUpdate(Module m)
            {
                string result = "";

                if (minSquare != m.minSquare || minHeight != m.minHeight || minWidth != m.minWidth)
                {
                    result += "dimensions changed.";
                }

                //bool changed = false;
                //foreach (Style s1 in m.styles)
                //{
                //    bool found = false;
                //    foreach (Style s2 in styles)
                //    {
                //        if (s2.type == s1.type)
                //        {
                //            if (s1.image != s2.image)
                //            {
                //                changed = true;
                //                break;
                //            }
                //        }
                //    }
                //    if (!found || changed) break;
                //}

                //if (changed)
                //{
                //    result += "Modules changed.";
                //}

                return result;
            }
        }

        public class Style
        {
            public readonly string type;
            public readonly Sprite image;

            public Style(string n, Sprite s)
            {
                type  = n;
                image = s;
            }
        }

        public string GetUpdate(House h)
        {
            string result = "";

            foreach (Module m1 in h.modules)
            {
                bool found = false;
                foreach (Module m2 in modules)
                {
                    if (m1.name == m2.name)
                    {
                        found = true;
                        string mChange = m2.GetUpdate(m1);
                        if (mChange != "")
                        {
                            result += "Module " + m2.name + ": " + mChange + "\n";
                        }
                    }
                }
                if (!found)
                {
                    result += "New module: " + m1.name + ".\n";
                }
            }
            if (result == "")
            {
                result += "Already up to date.";
            }

            return result;
        }
    }

}

