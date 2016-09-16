using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace HomeBuilder.Questioning
{
    public class Styler : MonoBehaviour
    {

        public Image    styleImage;
        public Text     styleText;
        public Button   nextStyle;
        public Button   prevStyle;
        public Button   sizeButton;
        public Slider   sizeSlider;

        List<Style> styles;
        int current     = 1;

        public void NextStyle()
        {
            SetStyle(current + 1);
            UpdateButtons();
        }

        public void PrevStyle()
        {
            SetStyle(current - 1);
            UpdateButtons();
        }

        public void ToggleSizeSlider()
        {
            sizeSlider.enabled = !sizeSlider.enabled;
            if (sizeSlider.enabled)
            {
                sizeSlider.transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                sizeSlider.transform.localScale = new Vector3(0, 0, 0);
            }
        }

        public void OnSliderChange()
        {
            sizeButton.gameObject.GetComponentInChildren<Text>().text = "Size: " + sizeSlider.value;
        }

        public void SetMinMax(int min, int max)
        {
            sizeSlider.minValue = min;
            sizeSlider.maxValue = max;
        }

        public Style GetStyle()
        {
            return styles[current];
        }

        public int GetSize()
        {
            return (int) sizeSlider.value;
        }

        public void SetStyles()
        {
            styles = new List<Style>();

            styles.Add(new Style(Configuration.Appartment.Styles.MODERN, Resources.Load<Sprite>(Assets.GetInstance().sprites.styleModern)));
            styles.Add(new Style(Configuration.Appartment.Styles.CLASSIC, Resources.Load<Sprite>(Assets.GetInstance().sprites.styleClassic)));
            styles.Add(new Style(Configuration.Appartment.Styles.OLD, Resources.Load<Sprite>(Assets.GetInstance().sprites.styleOld)));

            SetStyle(current);
            UpdateButtons();

            if (sizeSlider != null)
            {
                sizeSlider.enabled = true;
            }
        }

        void SetStyle(int index)
        {
            if (index >= styles.Count) index = 0;
            if (index < 0) index = styles.Count-1;

            current = index;
            styleImage.sprite   = styles[current].image;
            styleText.text      = Configuration.Appartment.GetStyle(styles[current].name);
        }

        void UpdateButtons()
        {
            //prevStyle.enabled = !(current == 0);
            //nextStyle.enabled = !(current == (styles.Count - 1));
        }

        public class Style
        {
            readonly public Configuration.Appartment.Styles name;
            readonly public Sprite   image;

            public Style(Configuration.Appartment.Styles name, Sprite image)
            {
                this.name   = name;
                this.image  = image;
            }
        }

    }
}