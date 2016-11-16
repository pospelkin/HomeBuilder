using UnityEngine;
using System.Collections;

namespace HomeBuilder.Core
{
    public class ModuleInfo
    {

        string  name;
        private bool _styleSet = false;
        Configuration.Appartment.Styles         style;
        Configuration.Appartment.ModuleParams   param;

        float width     = -1;
        float height    = -1;
        float x         = -1;
        float y         = -1;
        int   floor     = -1;

        public ModuleInfo(string name)
        {
            _styleSet = false;
            this.name = name;
        }

        public void SetParams(Configuration.Appartment.ModuleParams p)
        {
            param = p;
        }

        public Configuration.Appartment.ModuleParams GetParams()
        {
            return param;
        }

        public void SetSize(float w, float h)
        {
            width   = w;
            height  = h;
        }

        public void SetPosition(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public void SetFloor(int floor)
        {
            this.floor = floor;
        }

        public void ResetStyle()
        {
            _styleSet = false;
        }

        public bool IsStyleSet()
        {
            return _styleSet;
        }

        public void SetStyle(Configuration.Appartment.Styles style)
        {
            _styleSet  = true;
            this.style = style;
        }

        public Configuration.Appartment.Styles GetStyle()
        {
            return style;
        }

        public bool IsPositioned()
        {
            return x >= 0 && y >= 0 && floor >= 0;
        }

        public bool IsSized()
        {
            return width >= 0 && height >= 0;
        }

        public string GetName()
        {
            return name;
        }

        public float GetSquare()
        {
            return width * height;
        }

        public int GetFloor()
        {
            return floor;
        }

        public float[] GetSize()
        {
            return new float[] { width, height };
        }

        public float[] GetPosition()
        {
            return new float[] { x, y };
        }
    }
}