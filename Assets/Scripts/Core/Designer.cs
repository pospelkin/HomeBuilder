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

        static public Appartment GetRandomAppartment()
        {
            Appartment res = new Appartment("Random Appartmanrt", GetId());
            res.SetSquare(200);
            res.SetFloors(2);
            res.SetStyle(Configuration.Appartment.Styles.MODERN);

            ModuleInfo kitchen = new ModuleInfo("Kitchen");
            kitchen.SetStyle(Configuration.Appartment.Styles.MODERN);
            kitchen.SetSize(10, 5);
            kitchen.SetPosition(0, 0);
            kitchen.SetParams(Configuration.Appartment.approvedModules[0]);

            ModuleInfo bathroom = new ModuleInfo("Bathroom");
            bathroom.SetStyle(Configuration.Appartment.Styles.CLASSIC);
            bathroom.SetSize(10, 5);
            bathroom.SetPosition(0, 5);
            bathroom.SetParams(Configuration.Appartment.approvedModules[1]);

            ModuleInfo kitchen2 = new ModuleInfo("Kitchen");
            kitchen2.SetStyle(Configuration.Appartment.Styles.MODERN);
            kitchen2.SetSize(10, 5);
            kitchen2.SetPosition(0, 0);
            kitchen2.SetParams(Configuration.Appartment.approvedModules[0]);

            ModuleInfo bathroom2 = new ModuleInfo("Bathroom");
            bathroom2.SetStyle(Configuration.Appartment.Styles.CLASSIC);
            bathroom2.SetSize(10, 5);
            bathroom2.SetPosition(0, 5);
            bathroom2.SetParams(Configuration.Appartment.approvedModules[1]);

            res.AddModule(kitchen);
            res.AddModule(bathroom);
            res.AddModule(kitchen2);
            res.AddModule(bathroom2);

            res.SetSize(10,10);

            return res;
        }

    }
}