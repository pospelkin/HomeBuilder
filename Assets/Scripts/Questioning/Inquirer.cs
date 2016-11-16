using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using HomeBuilder.Core;
using System.Collections.Generic;

namespace HomeBuilder.Questioning
{
    public class Inquirer : MonoBehaviour
    {

        public ScreenController screen;

        public Button nextButton;
        public Button prevButton;

        public Moduler moduler;

        private Appartment       _app;

        public void Next()
        {
            if (!IsReadyToProceed()) return;

            Moduler.ModuleInfo[] modules = moduler.GetModules();
            for (int i = 0; i < modules.Length; i++)
            {
                int length = modules[i].count;
                if (modules[i].name == "Floors")
                {
                    _app.SetFloors(length);
                }
                else
                {
                    for (int j = 0; j < length; j++)
                    {
                        ModuleInfo module = new ModuleInfo(modules[i].name);
                        module.SetParams(modules[i].param);
                        _app.AddModule(module);
                    }
                }
            }

            Master.FLOW  = true;
            Master.SLIDE = true;

            Master.GetInstance().SetCurrentModule(_app.GetModules()[0]);
            screen.OpenStyle();
        }

        public void Prev()
        {
            Master.FLOW  = false;
            Master.SLIDE = true;

            _app.ResetStyle();

            screen.OpenStyle();
        }

        public void Home()
        {
            Master.FLOW  = false;
            Master.SLIDE = true;

            screen.OpenHistory();
        }

        void Start()
        {
            _app     = Master.GetInstance().GetCurrent();

            StartModuler();
        }

        void StartModuler()
        {
            ModuleInfo[] modules = _app.GetModules();
            _app.ResetModules();
            moduler.SetLimits(_app);
            moduler.SetState(modules);
        }

        bool IsReadyToProceed()
        {
            return moduler.GetTotalCount() > 0;
        }

    }
}
