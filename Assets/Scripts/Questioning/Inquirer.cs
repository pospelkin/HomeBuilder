using UnityEngine;
using UnityEngine.UI;
using HomeBuilder.Core;

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
                    int count = 0;
                    ModuleInfo[] apMs = _app.GetModules();
                    foreach (ModuleInfo m in apMs)
                    {
                        if (modules[i].name == m.GetName()) count++;
                    }
                    length -= count;
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

            ModuleInfo[] ms = _app.GetModules();
            foreach (ModuleInfo m in ms)
            {
                if (!m.IsStyleSet())
                {
                    Master.GetInstance().SetCurrentModule(m);
                    screen.OpenStyle();
                    return;
                }
            }

            screen.OpenDesigning();
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

            if (_app.IsEditing())
            {
                prevButton.gameObject.SetActive(false);
            }

            StartModuler();
        }

        void StartModuler()
        {
            ModuleInfo[] modules = _app.GetModules();
            if (!_app.IsEditing()) _app.ResetModules();

            moduler.SetLimits(_app);
            moduler.SetState(modules, _app.GetFloors());
        }

        bool IsReadyToProceed()
        {
            return moduler.GetTotalCount() >= _app.GetFloors();
        }

    }
}
