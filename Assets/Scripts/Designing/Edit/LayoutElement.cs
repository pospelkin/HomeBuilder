﻿using HomeBuilder.Core;
using UnityEngine;

namespace HomeBuilder.Designing
{
    public class LayoutElement
    {

        readonly public Cube cube;
        readonly public ModuleInfo info;

        float width, height;
        public float mWidth, mHeight;
        public float mSquare;

        Vector2 position;
        Layout layout;

        public int i, j;

        public LayoutElement(Cube c, ModuleInfo i, Layout l)
        {
            cube = c;
            info = i;
            layout = l;
        }

        public void SetIndexes(int i, int j)
        {
            this.i = i;
            this.j = j;
        }

        public void SetMinParams(float mW, float mH, float mSq)
        {
            mWidth  = mW;
            mHeight = mH;
            mSquare = mSq;
        }

        public void SetPosition(Vector2 pos)
        {
            position = pos;
        }

        public Vector2 GetPosition()
        {
            return position;
        }

        public void SetSize(float w, float h, bool force = false)
        {
            if (force)
            {
                width   = w;
                height  = h;
            } else
            {
                float tW = width,
                    tH = height;

                //w = Mathf.RoundToInt(w);
                //h = Mathf.RoundToInt(h);

                if (w >= mWidth)
                {
                    tW = w;
                }
                if (h >= mHeight)
                {
                    tH = h;
                }

                if (GetSquare(tW, tH) >= mSquare) {
                    layout.RequestChange(this, tW, tH);
                }
            }   
        }

        public void Destroy()
        {

        }

        public Vector2 GetSize()
        {
            return new Vector2(width, height);
        }

        public float GetSquare()
        {
            return GetSquare(width, height);
        }


        float GetSquare(float w, float h)
        {
            return w * h;
        }
    }
}