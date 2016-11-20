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

        public void Open()
        {
            GetComponent<CanvasGroup>().alpha = 1;
            GetComponent<CanvasGroup>().blocksRaycasts = true;
            GetComponent<CanvasGroup>().interactable = true;
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
            foreach (House.Module m in modules)
            {
                buttons.Add(AddModule(m));    
            }
        }

        Button AddModule(House.Module module)
        {
            GameObject obj = Instantiate(Resources.Load(Assets.GetInstance().prefabs.moduleBtn), content) as GameObject;
            obj.transform.localScale = new Vector3(1,1,1);
            obj.transform.position   = new Vector3(0,0,0);

            obj.GetComponentInChildren<Text>().text = module.name;
            obj.GetComponent<Button>().onClick.AddListener(() => { OnClick(module); });

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
