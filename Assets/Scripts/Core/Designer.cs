using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HomeBuilder.Core
{
    public class Designer
    {
        private static int id = 0;

        public static int GetId()
        {
            return ++id;
        }
        
        public void evaluate(Appartment appartment)
        {
            float totalSquare    = appartment.GetSquare();
            ModuleInfo[] modules = appartment.GetModules();

            for (int i = 0; i < modules.Length; i++)
            {
                if (appartment.GetFloors() == 2 && i >= Mathf.RoundToInt(modules.Length/2f))
                {
                    modules[i].SetFloor(1);
                }
                else
                {
                    modules[i].SetFloor(0);
                }
                prepareModule(modules[i], appartment.GetFloors());
            }
        }

        void prepareModule(ModuleInfo module, int floors)
        {
            float minSquare = module.GetParams().minSquare;
            float minWidth  = module.GetParams().minWidth;
            float minHeight = module.GetParams().minHeight;

            float spareSqrt = minSquare - minHeight * minWidth;

            if (spareSqrt > 0)
            {
                float additionalM = Mathf.Sqrt(spareSqrt);

                minWidth    += additionalM;
                minHeight   += additionalM;
            }

            module.SetSize(minWidth, minHeight);
        }

        static public Appartment GetRandomAppartment()
        {
            Appartment res = new Appartment("Random Appartmanrt", GetId());
            res.SetSquare(200);
            res.SetFloors(2);
            res.SetStyle("MODERN");

            return res;
        }

    }
}