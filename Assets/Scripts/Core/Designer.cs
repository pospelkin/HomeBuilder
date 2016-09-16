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

            createSheme(modules, appartment);
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

        void createSheme(ModuleInfo[] modules, Appartment appartment)
        {
            float width     = Mathf.Sqrt(appartment.GetSquare());
            float height    = width;

            int rows = (int) Mathf.Floor(modules.Length / 2);
            int cols = modules.Length - rows;

            float mHeight = height / rows;

            int count   = 0;
            float y     = 0;

            float maxWidth = 0;
            for (int i = 0; i < rows; i++)
            {
                y = i * mHeight;
                float x = 0;
                for (int j = 0; j < cols; j++)
                {
                    if (count >= modules.Length) break;
                    ModuleInfo module = modules[count];
                    count++;

                    float mWidht = module.GetSquare() / mHeight;

                    module.SetSize(mWidht, mHeight);
                    module.SetPosition(x, y);

                    x += mWidht;
                }
                if (x > maxWidth)
                {
                    maxWidth = x;
                }
            }

            appartment.SetSize(maxWidth, height);
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