using System.Collections.Generic;
using HomeBuilder.Core;
using UnityEngine;
using UnityEngine.UI;

namespace HomeBuilder.Questioning
{
    public class StyleManager : MonoBehaviour
    {
        public static bool IS_ACTIVE = false;
        public static bool IS_MODULE = false;

        public ScreenController screen;
        public SkyleSlider styleSlider;

        public Text captionTxt;
        public Text styleTxt;
        public Text sizeTxt;

        public Image image;

        public Button next;
        public Button prev;
        public Button home;

        public Slider slider;

        public void Home()
        {
            Master.FLOW  = false;
            Master.SLIDE = true;

            screen.OpenHistory();
        }

        public void Next()
        {
            if (_isModule)
            {
                if (_module != null)
                {
                    _module.SetStyle(styles[_current].name);
                }
                if (_app.IsAllStyleSet())
                {
                    Master.FLOW  = true;
                    Master.SLIDE = false;

                    screen.OpenDesigning();
                }
                else
                {

                    Master.FLOW  = true;
                    Master.SLIDE = true;

                    Master.GetInstance().SetCurrentModule(GetNextModule());
                    screen.OpenStyle();
                }
            }
            else
            {
                _app.SetStyle(styles[_current].name);
                _app.SetSquare(slider.value);


                Master.FLOW  = true;
                Master.SLIDE = true;

                screen.OpenModules();
            }
        }

        public void Prev()
        {
            if (!_isModule) return;

            Master.FLOW  = false;
            Master.SLIDE = true;

            _module.ResetStyle();
            ModuleInfo module = GetPrevModule();
            if (module == null)
            {
                screen.OpenModules();
            }
            else
            {
                Master.GetInstance().SetCurrentModule(module);
                screen.OpenStyle();
            }
        }

        public void SliderChange()
        {
            sizeTxt.text = "Size: " + GetSize() + " m2";
        }

        public int GetSize()
        {
            return Mathf.RoundToInt(slider.value);
        }

        private Appartment   _app;
        private bool         _isModule = false;
        private List<Style>  styles;
        private int          _current = 0;
        private ModuleInfo   _module;

        void Awake()
        {
            _app = Master.GetInstance().GetCurrent();
            _isModule = _app.IsStyleSet();

            IS_MODULE = _isModule;
        }

        void OnEnable()
        {
            IS_ACTIVE = true;
            styleSlider.OnStyleChange += OnStyleChange;
        }

        void OnDisable()
        {
            IS_ACTIVE = false;
            styleSlider.OnStyleChange -= OnStyleChange;
        }


        void OnStyleChange(int ind, Style st)
        {
            _current = ind;

            UpdateCaption();
        }

        void Start()
        {
            if (!_isModule)
            {
                prev.gameObject.SetActive(false);
            }
            else
            {
                slider .gameObject.SetActive(false);
                sizeTxt.gameObject.SetActive(false);

                _module = Master.GetInstance().GetCurrentModule();
                if (_module == null)
                {
                    Next();
                    return;
                }
            }
            SliderChange();

            SetStyles(_isModule ? _module.GetName() : "");
            UpdateCaption();
            UpdateImage();

            styleSlider.SetStyles(styles);
        }

        ModuleInfo GetNextModule()
        {
            ModuleInfo[] modules = _app.GetModules();

            for (int i = 0; i < modules.Length; i++)
            {
                if (!modules[i].IsStyleSet()) return modules[i];
            }

            return null;
        }

        ModuleInfo GetPrevModule()
        {
            ModuleInfo[] modules = _app.GetModules();

            for (int i = modules.Length-1; i >= 0; i--)
            {
                if (modules[i].IsStyleSet()) return modules[i];
            }

            return null;
        }

        void SetStyles(string module = "")
        {
            styles = new List<Style>();

            House.Style[] stls = null;
            if (module == "")
            {
                stls = Master.GetInstance().House.module.styles.ToArray();
            }
            else
            {
                House.Module[] mdls = Master.GetInstance().House.modules.ToArray();
                for (int i = 0; i < mdls.Length; i++)
                {
                    if (mdls[i].name == module)
                    {
                        stls = mdls[i].styles.ToArray();
                        break;
                    }
                }
            }
            if (stls != null)
            {
                for (int i = 0; i < stls.Length; i++)
                {
                    styles.Add(new Style(stls[i].type, stls[i].image));
                }
            }
        }

        void UpdateImage()
        {
            image.sprite = styles[_current].image;
        }

        void UpdateCaption()
        {
            captionTxt.text = _isModule ? ("Module Style: " + _module.GetName()) : "Home Style";
            styleTxt.text   = styles[_current].name.ToString();
        }

        public class Style
        {
            readonly public string name;
            readonly public Sprite image;

            public Style(string name, Sprite image)
            {
                this.name  = name;
                this.image = image;
            }
        }
    }
}