using UnityEngine;
using System.Collections;

namespace HomeBuilder.Core
{
    public class Designer
    {
        
        public void evaluate(Appartment appartment)
        {
            float totalSquare    = appartment.GetSquare();
            ModuleInfo[] modules = appartment.GetModules();
            
            if (!IsModulesSquareOK(modules, totalSquare))
            {
                Debug.Log("Modules Square is not equal to Appartment square.");
            }

            float x = 0;
            float height = 0;
            for (int i = 0; i < modules.Length; i++)
            {
                float width = Mathf.Sqrt(modules[i].GetSquare());
                modules[i].SetSize(width, width);
                modules[i].SetPosition(x, 0);
                x += width;
                if (width > height)
                {
                    height = width;
                }
            }

            appartment.SetSize(x, height);
        }

        public bool IsModulesSquareOK(ModuleInfo[] modules, float square)
        {
            float sq = 0;
            for (int i = 0; i < modules.Length; i++)
            {
                sq += modules[i].GetSquare();
            }

            return sq == square;
        }

        static public Appartment GetRandomAppartment()
        {
            Appartment res = new Appartment("Random Appartmanrt", 0);
            res.SetSquare(100);
            res.SetStyle("Modern");

            ModuleInfo kitchen = new ModuleInfo("Kitchen");
            kitchen.SetStyle("Modern");
            kitchen.SetSquare(50);
            kitchen.SetSize(10, 5);
            kitchen.SetPosition(0, 0);

            ModuleInfo bathroom = new ModuleInfo("Bathroom");
            bathroom.SetStyle("Modern");
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