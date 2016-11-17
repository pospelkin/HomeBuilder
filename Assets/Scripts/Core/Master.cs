using UnityEngine;
using System.Collections;

namespace HomeBuilder.Core
{
    public class Master
    {

        private static Master instance = null;
        public  static Master GetInstance()
        {
            if (instance == null)
            {
                instance = new Master();
            }

            return instance;
        }

        public History  history;
        public Designer designer;

        public House House { private set; get; }

        Appartment current       = null;
        ModuleInfo currentModule = null;

        private Master()
        {
            history     = new History();
            designer    = new Designer();
            //history.SimulateHistory();
            //SetCurrent(Designer.GetRandomAppartment());
        }

        public void SetCurrent(Appartment app)
        {
            current = app;
        }

        public Appartment GetCurrent()
        {
            return current;
        }

        public void SetCurrentModule(ModuleInfo module)
        {
            currentModule = module;
        }

        public ModuleInfo GetCurrentModule()
        {
            return currentModule;
        }

        public void SetHouse(House h)
        {
            House = h;
        }

        public static bool FLOW  = true;
        public static bool SLIDE = false;

        public static string GetEffect(bool start)
        {
            return SLIDE ? (FLOW  ? (start ? "Right" : "Left") : (start ? "Left" : "Right")) : "Fade";
        }

    }
}