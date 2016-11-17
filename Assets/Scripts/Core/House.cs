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
    }

}

