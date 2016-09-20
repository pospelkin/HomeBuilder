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

        public Button nextButton;
        public Button prevButton;
        public Text caption;

        public Styler styler;
        public Styler stylerOnly;
        public Moduler moduler;

        int step = 0;
        Appartment app;
        List<ModuleInfo> modules;

        public void Next()
        {
            if (!IsReadyToProceed()) return;
            step++;

            UpdateStep();
        }

        public void Prev()
        {
            step--;

            UpdateStep();
        }

        void Start()
        {
            app     = new Appartment("New Appartment", 0);
            step    = 0;
            UpdateStep();
        }

        void UpdateStep()
        {
            if (step < 0)
            {
                SceneManager.LoadScene(Configuration.Scenes.menuScene);
                return;
            }

            if (step == 0)
            {
                prevButton.GetComponentInChildren<Text>().text = "Menu";
            } else
            {
                prevButton.GetComponentInChildren<Text>().text = "Prev";
            }

            switch (step)
            {
                case 0:
                    StartStyler();
                    break;
                case 1:
                    app.SetStyle(styler.GetStyle().name);
                    app.SetSquare(styler.GetSize());

                    StartModuler();
                    break;
                case 2:
                    Moduler.ModuleInfo[] ms = moduler.GetModules();
                    for (int i = 0; i < ms.Length; i++)
                    {
                        for (int j = 0; j < ms[i].count; j++)
                        {
                            ModuleInfo info = new ModuleInfo(ms[i].name);
                            info.SetParams(ms[i].param);

                            modules.Add(info);
                        }
                    }

                    StartStylerForModule(0);
                    break;
                default:
                    modules[step - 3].SetStyle(stylerOnly.GetStyle().name);

                    StartStylerForModule(step - 2);
                    break;
            }
        }

        void StartStyler()
        {
            TurnOn(styler.gameObject);

            caption.text = "Home Style";
            styler.SetStyles();
            styler.SetMinMax(Configuration.Appartment.minSquare, Configuration.Appartment.maxSquare);
        }

        void StartModuler()
        {
            TurnOn(moduler.gameObject);

            caption.text = "Select Modules";

            moduler.SetLimits(app);
            modules = new List<ModuleInfo>();
        }

        void StartStylerForModule(int index)
        {
            if (index >= modules.Count)
            {
                Finish();
                return;
            }

            TurnOn(stylerOnly.gameObject);

            stylerOnly.SetStyles(modules[index].GetName());
            caption.text = "Module Style: " + modules[index].GetName();
        }

        bool IsReadyToProceed()
        {
            return step != 1 || (moduler.GetTotalCount() > 0);
        }

        void TurnOn(GameObject obj)
        {
            moduler     .gameObject.SetActive(false);
            styler      .gameObject.SetActive(false);
            stylerOnly  .gameObject.SetActive(false);

            obj.SetActive(true);

        }

        void Finish()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                app.AddModule(modules[i]);
            }

            Master.GetInstance().SetCurrent(app);
            SceneManager.LoadScene(Configuration.Scenes.designingScene);
        }
    }
}
