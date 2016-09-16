using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HomeBuilder.Core
{
    public class Designer
    {
        
        public void evaluate(Appartment appartment)
        {
            float totalSquare    = appartment.GetSquare();
            ModuleInfo[] modules = appartment.GetModules();

            float spare = GetSpareSquare(modules, totalSquare);
            if (spare < 0)
            {
                Debug.Log("Modules Square is not equal to Appartment square.");
            }
            
            for (int i = 0; i < modules.Length; i++)
            {
                prepareModule(modules[i]);
            }

            createSheme(modules, appartment, spare);
        }

        public float GetSpareSquare(ModuleInfo[] modules, float square)
        {
            float sq = 0;
            for (int i = 0; i < modules.Length; i++)
            {
                sq += modules[i].GetSquare();
            }

            return square - sq;
        }

        void prepareModule(ModuleInfo module)
        {
            float minSquare = module.GetSquare();
            float minWidth  = module.GetSize()[0];
            float minHeight = module.GetSize()[1];

            float spareSqrt = minSquare - minHeight * minWidth;
            if (spareSqrt < 0)
            {
                Debug.Log("WTF, for module with w = " + minWidth + ", h = " + minHeight + ", square is " + minSquare + "?!");
            }

            if (spareSqrt > 0)
            {
                float additionalM = Mathf.Sqrt(spareSqrt);

                minWidth    += additionalM;
                minHeight   += additionalM;
            }

            module.SetSize(minWidth, minHeight);
        }

        void createSheme(ModuleInfo[] modules, Appartment appartment, float spareSq = 0)
        {
            float width     = Mathf.Sqrt(appartment.GetSquare());
            float height    = width;

            int cols = (int) Mathf.Floor(Mathf.Sqrt( modules.Length ) ) + 1 ;
            int rows = (int) Mathf.Ceil(modules.Length / (float) cols);

            float mHeight = height / rows;

            int count   = 0;

            float spare = spareSq / modules.Length;

            List<Rect>[] levels = new List<Rect>[rows];

            for (int i = 0; i < rows; i++)
            {
                levels[i] = new List<Rect>();

                for (int j = 0; j < cols; j++)
                {
                    if (count >= modules.Length) break;
                    ModuleInfo module = modules[count];
                    count++;

                    float h = Mathf.Max(mHeight, module.GetSize()[1]);
                    float w = ((module.GetSquare() + spare) / h);
                    if (w < module.GetSize()[0])
                    {
                        w = module.GetSize()[0];
                        h = ((module.GetSquare() + spare) / w);
                    }

                    Rect rect = GetRectForModule(module, w, h, levels[i], i > 0 ? levels[i-1] : null);

                    module.SetSize(rect.width, rect.height);
                    module.SetPosition(rect.x, rect.y);

                    levels[i].Add(rect);
                }
            }

            float[] size = GetAppSize(levels);
            appartment.SetSize(size[0], size[1]);
        }

        Rect GetRectForModule(ModuleInfo module, float w, float h, List<Rect> level, List<Rect> upperLevel = null)
        {
            float x = GetX(level);
            float y = upperLevel == null ? 0 : GetY(x, w, upperLevel);

            return new Rect(new Vector2(x, y), new Vector2(w, h));
        }

        float GetX(List<Rect> level)
        {
            return level.Count > 0 ? level[level.Count - 1].xMax : 0;
        }

        float GetY(float x, float width, List<Rect> upperLevel)
        {
            float   y1 = 0,
                    y2 = 0;

            int l = upperLevel.Count;
            for (int i = 0; i < l; i++)
            {
                if (upperLevel[i].xMin <= x && upperLevel[i].xMax >= x)
                {
                    y1 = upperLevel[i].yMax;
                }
                if (upperLevel[i].xMin <= x + width && upperLevel[i].xMax >= x + width)
                {
                    y2 = upperLevel[i].yMax;
                }
            }

            return Mathf.Max(y1, y2);
        }

        float[] GetAppSize(List<Rect>[] rects)
        {
            float xMax = 0, yMax = 0;
            for (int i = 0; i < rects.Length; i++)
            {
                int l = rects[i].Count;
                for (int j = 0; j < l; j++)
                {
                    if (rects[i][j].xMax > xMax) xMax = rects[i][j].xMax;
                    if (rects[i][j].yMax > yMax) yMax = rects[i][j].yMax;
                }
            }

            return new float[] { xMax, yMax };
        }

        static public Appartment GetRandomAppartment()
        {
            Appartment res = new Appartment("Random Appartmanrt", 0);
            res.SetSquare(100);
            res.SetStyle(Configuration.Appartment.Styles.MODERN);

            ModuleInfo kitchen = new ModuleInfo("Kitchen");
            kitchen.SetStyle(Configuration.Appartment.Styles.MODERN);
            kitchen.SetSquare(50);
            kitchen.SetSize(10, 5);
            kitchen.SetPosition(0, 0);

            ModuleInfo bathroom = new ModuleInfo("Bathroom");
            bathroom.SetStyle(Configuration.Appartment.Styles.CLASSIC);
            bathroom.SetSquare(50);
            bathroom.SetSize(10, 5);
            bathroom.SetPosition(0, 5);

            res.AddModule(kitchen);
            res.AddModule(bathroom);

            res.SetSize(10,10);

            return res;
        }

    }
}