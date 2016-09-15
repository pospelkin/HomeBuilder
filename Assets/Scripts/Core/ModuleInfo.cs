using UnityEngine;
using System.Collections;

namespace HomeBuilder.Core
{
    public class ModuleInfo
    {

        string  name;
        float   square;
        string  style;

        float width     = -1;
        float height    = -1;
        float x         = -1;
        float y         = -1;

        public ModuleInfo(string name)
        {
            this.name = name;
        }

        public void SetSquare(float square)
        {
            this.square = square;
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

        public void SetStyle(string style)
        {
            this.style = style;
        }

        public string GetStyle()
        {
            return style;
        }

        public bool IsPositioned()
        {
            return x >= 0 && y >= 0;
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
            return square;
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