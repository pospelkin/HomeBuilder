using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HomeBuilder.Core;
using UnityEngine.UI;


namespace HomeBuilder.Designing
{

    public class ContextModules : MonoBehaviour
    {

        public RectTransform content;
        public Constructor   constructor;

        public bool IsOpen {
            get { return GetComponent<CanvasGroup>().alpha != 0; }
        }

        private List<Button> buttons;
        private List<House.Module> modules;

        public void Open()
        {
            GetComponent<CanvasGroup>().alpha = 1;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<CanvasGroup>().interactable = true;

            for (int i = 0; i < buttons.Count; i++)
            {
                House.Module module = modules[i];
                Button button       = buttons[i];

                float spare = constructor.GetCurrentLayout().GetSpareSquare(true);
                Debug.Log("Spare: " + spare + " and we need " + module.minSquare + " at least for " + module.name);

                button.interactable = module.minSquare <= spare;
            }
        }

        public void Close()
        {
            GetComponent<CanvasGroup>().alpha = 0;
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            GetComponent<CanvasGroup>().interactable = false;
        }

        void Start()
        {
            List<House.Module> modules = Master.GetInstance().House.modules;
            buttons = new List<Button>();
            this.modules = new List<House.Module>();
            foreach (House.Module m in modules)
            {
                AddModule(m);    
            }
        }

        Button AddModule(House.Module module)
        {
            GameObject obj = Instantiate(Resources.Load(Assets.GetInstance().prefabs.moduleBtn), content) as GameObject;
            obj.transform.localScale = new Vector3(1,1,1);
            obj.transform.position   = new Vector3(0,0,0);

            obj.GetComponentInChildren<Text>().text = module.name;
            obj.GetComponent<Button>().onClick.AddListener(() => { OnClick(module); });

            buttons.Add(obj.GetComponent<Button>());
            this.modules.Add(module);

            return obj.GetComponent<Button>();
        }

        void OnDestroy()
        {
            foreach (Button btn in buttons)
            {
                btn.onClick.RemoveAllListeners();
            }
        }

        void OnClick(House.Module module)
        {
            constructor.AddModule(module);
        }

    }


}
