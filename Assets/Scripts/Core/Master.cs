using UnityEngine;
using System.Collections;

namespace HomeBuilder.Core
{
    public class Master
    {

        static private Master instance = null;
        static public Master GetInstance()
        {
            if (instance == null)
            {
                instance = new Master();
            }

            return instance;
        }

        public History  history;
        public Designer designer;

        Appartment current = null;

        private Master()
        {
            history     = new History();
            designer    = new Designer();
            history.SimulateHistory();

            SetCurrent(Designer.GetRandomAppartment());
        }

        public void SetCurrent(Appartment app)
        {
            current = app;
        }

        public Appartment GetCurrent()
        {
            return current;
        }

    }
}