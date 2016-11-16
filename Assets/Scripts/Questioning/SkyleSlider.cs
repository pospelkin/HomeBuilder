using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

namespace HomeBuilder.Questioning
{

    public class SkyleSlider : MonoBehaviour
    {
        public delegate void StyleChange(int index, StyleManager.Style style);
        public StyleChange OnStyleChange;

        public int shift;
        public InputHandler input;

        public Image[] images;

        private float time = 0.25f;
        private List<StyleManager.Style> styles;
        private int currentImage;
        private int currentStyle;

        void OnEnable()
        {
            input.onSwap += OnSwap;
        }

        void OnSwap(Vector3 position, Vector3 shift)
        {
            if (shift.x < 0)
            {
                NextStyle();
            }
            else
            {
                PrevStyle();
            }
        }

        void OnDisable()
        {
            input.onSwap -= OnSwap;
        }

        public void SetStyles(List<StyleManager.Style> imgs)
        {
            styles = imgs;

            SetStyle(0);
        }

        public void NextStyle()
        {
            currentStyle++;
            if (currentStyle >= styles.Count) currentStyle = 0;

            if (OnStyleChange != null)
            {
                OnStyleChange(currentStyle, styles[currentStyle]);
            }
            AnimateNextImage(styles[currentStyle]);
        }

        public void PrevStyle()
        {
            currentStyle--;
            if (currentStyle < 0) currentStyle = styles.Count - 1;

            if (OnStyleChange != null)
            {
                OnStyleChange(currentStyle, styles[currentStyle]);
            }
            AnimatePrevImage(styles[currentStyle]);
        }

        void SetStyle(int index)
        {
            currentStyle = index;

            SetImage(styles[currentStyle]);
        }

        void AnimateNextImage(StyleManager.Style style)
        {
            Image prev = images[currentImage];
            Image next = GetNextImage();

            ((RectTransform)prev.transform).DOAnchorPos(new Vector2(-shift, 0), time);
            next.sprite = styles[currentStyle].image;
            ((RectTransform)next.transform).anchoredPosition = new Vector2(shift, 0);
            ((RectTransform)next.transform).DOAnchorPos(new Vector2(0, 0), time);
        }

        void AnimatePrevImage(StyleManager.Style style)
        {
            Image prev = images[currentImage];
            Image next = GetNextImage();

            ((RectTransform)prev.transform).DOAnchorPos(new Vector2(shift, 0), time);
            next.sprite = styles[currentStyle].image;
            ((RectTransform)next.transform).anchoredPosition = new Vector2(-shift, 0);
            ((RectTransform)next.transform).DOAnchorPos(new Vector2(0, 0), time);
        }

        void SetImage(StyleManager.Style style)
        {
            Image img = null;
            

            if (img != null)
            {
                img.sprite = style.image;
                ((RectTransform)img.transform).anchoredPosition = new Vector2(0, 0);
            }
        }

        Image GetNextImage()
        {
            for (int i = 0; i < images.Length; i++)
            {

                if (i != currentImage)
                {
                    currentImage = i;
                    break;
                }
            }
            return images[currentImage];
        }
    }


}
