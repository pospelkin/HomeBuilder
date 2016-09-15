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
        public Moduler moduler;

        int step = 0;
        float squareUsed = 0;
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
                        for (int j = 0; j < ms[i].GetCount(); j++)
                        {
                            modules.Add(new ModuleInfo(ms[i].GetName()));
                        }
                    }

                    squareUsed = 0;
                    StartStylerForModule(0);
                    break;
                default:
                    modules[step - 3].SetStyle(styler.GetStyle().name);
                    modules[step - 3].SetSquare(styler.GetSize());

                    squareUsed += styler.GetSize();
                    StartStylerForModule(step - 2);
                    break;
            }
        }

        void StartStyler()
        {
            moduler.gameObject.SetActive(false);
            styler.gameObject.SetActive(true);

            caption.text = "Home Style";
            styler.SetStyles();
            styler.SetMinMax(10, 100);
        }

        void StartModuler()
        {
            moduler.gameObject.SetActive(true);
            styler.gameObject.SetActive(false);

            caption.text = "Select Modules";

            float l = Mathf.Floor(app.GetSquare() / 10f);
            moduler.SetLimit((int) l);
            modules = new List<ModuleInfo>();
        }

        void StartStylerForModule(int index)
        {
            if (index >= modules.Count)
            {
                Finish();
                return;
            }

            moduler.gameObject.SetActive(false);
            styler.gameObject.SetActive(true);
            
            caption.text = "Module Style #" + index;

            int max = (int)(app.GetSquare() - squareUsed - (modules.Count - (index + 1)) * 10f);
            styler.SetMinMax(index == modules.Count-1 ? max : 10, max);
        }

        bool IsReadyToProceed()
        {
            return step != 1 || (moduler.GetTotalCount() > 0);
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
