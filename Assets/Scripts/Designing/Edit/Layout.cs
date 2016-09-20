using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HomeBuilder.Core;

namespace HomeBuilder.Designing
{
    public class Layout
    {
        public delegate void OnChange();
        public OnChange onChange;

        readonly public float square;
        List<LayoutElement> elements;
        LayoutElement[][] eMatrix;
        Rect[][] rMatrix; 

        Appartment appartment;

        public Layout(Appartment ap, Cube[] cubes, ModuleInfo[] infos)
        {
            appartment  = ap;

            List<LayoutElement> elems = new List<LayoutElement>();
            for (int i = 0; i < infos.Length; i++)
            {
                LayoutElement elem = new LayoutElement(cubes[i], infos[i], this);
                elem.SetMinParams(
                    infos[i].GetParams().minWidth,
                    infos[i].GetParams().minHeight,
                    infos[i].GetParams().minSquare
                    );
                elem.SetPosition(new Vector2(infos[i].GetPosition()[0], infos[i].GetPosition()[1]));
                elem.SetSize(infos[i].GetSize()[0], infos[i].GetSize()[1], true);

                elems.Add(elem);
            }

            elements = elems;

            FixLayout();
            UpdatePositions();
        }

        public void Interchange(LayoutElement l1, LayoutElement l2)
        {
            int i1 = elements.IndexOf(l1);
            int i2 = elements.IndexOf(l2);

            if (i1 >= 0 && i2 >=0  && i1 != i2)
            {
                LayoutElement temp = elements[i1];
                elements[i1] = elements[i2];
                elements[i2] = temp;

                FixLayout();
                UpdatePositions();

                if (onChange != null) onChange();
            }
        }

        public void Destory()
        {

            LayoutElement[] elements = GetElements();

            for (int i = 0; i < elements.Length; i++)
            {
                elements[i].Destroy();
            }
        }

        public LayoutElement[] GetElements()
        {
            return elements.ToArray();
        }

        public List<LayoutElement> GetElementsLists()
        {
            return elements;
        }

        public float GetSquare()
        {
            return appartment.GetSquare();
        }

        public float GetElemetsSquare()
        {
            LayoutElement[] els = GetElements();
            float sq = 0;
            for (int i = 0; i < els.Length; i++)
            {
                sq += els[i].GetSquare();
            }
            return sq;
        }

        public void UpdatePositions()
        {
            int l1 = elements.Count;
            for (int i = 0; i < l1; i++)
            {
                    if (elements[i] != null)
                    {
                        UpdateElementPosition(elements[i]);
                    }
            }
        }

        void UpdateElementPosition(LayoutElement element)
        {
            float width     = appartment.GetSize()[0];
            float height    = appartment.GetSize()[1];

            Cube cube = element.cube;
            Vector2 size = element.GetSize();
            cube.SetSize(size.x, size.y);

            Vector2 pos = element.GetPosition();
            cube.SetPosition(-width / 2f + pos.x + size.x / 2f, -height / 2f + pos.y + size.y / 2f);
        }

        void OnSizeChanged(LayoutElement element)
        {
            float d = GetSpareSquare();
            if (d > 0)
            {
                Debug.Log("Spare " + d + " m2.");
            } else if (d > 0) 
            {
                Debug.Log("Not enought " + d + " m2.");
            }

            FixLayout(element);
            UpdatePositions();

            if (onChange != null) onChange();
        }

        float GetSpareSquare()
        {
            return GetSquare() - GetElemetsSquare();
        }

        float GetMinSquare()
        {
            LayoutElement[] elements = GetElements();

            float sq = 0;
            for (int i = 0; i < elements.Length; i++)
            {
                sq += elements[i].mSquare;
            }

            return sq;
        }

        public void RequestChange(LayoutElement el, float newWidth, float newHeight)
        {
            float width = el.GetSize().x, height = el.GetSize().y;
            float newSquare = newWidth * newHeight;

            int x = -1, y = -1;
            for (int i = 0; i < eMatrix.Length; i++)
            {
                for (int j = 0; j < eMatrix[i].Length; j++)
                {
                    if (eMatrix[i][j] == el)
                    {
                        x = i;
                        y = j;
                    }
                }
            }

            if (height != newHeight)
            {
                float change = newHeight - height;
                if (change <= GetSpareHeight(x, y) && HeightChangeAllowed(x, y))
                {
                    float changed = CompensateHeight(x, y, change);
                    el.SetSize(width, height + changed, true);
                }
            }

            FixLayout();
            UpdatePositions();

            if (width != newWidth)
            {
                float change = newWidth - width;
                if (change <= GetSpareWidth(x, y) && WidthChangeAllowed(x, y))
                {
                    float changed = CompensateWidth(x, y, change);
                    el.SetSize(width + changed, height, true);
                }
            }

            FixLayout();
            UpdatePositions();

            if (onChange != null) onChange();

        }

        bool HeightChangeAllowed(int x, int y)
        {
            for (int i = x - 1; i <= x + 1; i++)
            {
                if (i == x || i < 0 || i >= rMatrix.Length) continue;

                if (rMatrix[x][y].width != rMatrix[i][y].width)
                {
                    return false;
                }
            }
            return true;
        }

        bool WidthChangeAllowed(int x, int y)
        {
            for (int i = y - 1; i <= y + 1; i++)
            {
                if (i == y || i < 0 || i >= rMatrix[x].Length) continue;

                if (rMatrix[x][y].height != rMatrix[x][i].height)
                {
                    return false;
                }
            }
            return true;
        }

        float GetSpareWidth(int x, int y)
        {
            float spare = 0;
            for (int i = y - 1; i <= y + 1; i++)
            {
                if (i == y || i < 0 || i >= rMatrix[x].Length) continue;

                if (rMatrix[x][y].height == rMatrix[x][i].height)
                {
                    spare += (rMatrix[x][i].width - eMatrix[x][i].mWidth);
                } else {

                }
            }
            return spare;
        }

        float GetSpareHeight(int x, int y)
        {
            float spare = 0;
            for (int i = x - 1; i <= x + 1; i++)
            {
                if (i == x || i < 0 || i >= rMatrix.Length) continue;

                if (rMatrix[x][y].width == rMatrix[i][y].width)
                {
                    spare += (rMatrix[i][y].height - eMatrix[i][y].mHeight);
                } else
                {

                }
            }
            return spare;
        }

        float CompensateHeight(int x, int y, float value)
        {
            float left = value;
            for (int i = x - 1; i <= x + 1; i++)
            {
                if (i == x || i < 0 || i >= rMatrix.Length) continue;

                float newHeight = Mathf.Max(eMatrix[i][y].mHeight, rMatrix[i][y].height - left);
                eMatrix[i][y].SetSize(rMatrix[i][y].width, newHeight, true);
                left -= (rMatrix[i][y].height - newHeight);
            }
            return value - left;
        }

        float CompensateWidth(int x, int y, float value)
        {
            float left = value;
            for (int i = y - 1; i <= y + 1; i++)
            {
                if (i == y || i < 0 || i >= rMatrix[x].Length) continue;

                float newWidth = Mathf.Max(eMatrix[x][i].mWidth, rMatrix[x][i].width - left);
                eMatrix[x][i].SetSize(newWidth, rMatrix[x][i].height, true);
                left -= (rMatrix[x][i].width - newWidth);
            }
            return value - left;
        }

        public void Remove(LayoutElement element)
        {
            MonoBehaviour.Destroy(element.cube.gameObject);

            elements.Remove(element);

            FixLayout();
            UpdatePositions();

            if (onChange != null) onChange();
        }


        public int[] matrix;
        public void FixLayout(LayoutElement fixedElement = null)
        {
            LayoutElement[] elements = GetElements();

            float width = Mathf.Sqrt(appartment.GetSquare());
            float height = width;

            int cols = (int)Mathf.Floor(Mathf.Sqrt(elements.Length));
            int rows = (int)Mathf.Ceil(elements.Length / (float)cols);

            float mHeight = height / rows;

            int count = 0;

            float spare = GetSpareSquare();

            List<Rect>[] levels = new List<Rect>[rows];
            List<LayoutElement>[] lElems = new List<LayoutElement>[rows];

            matrix = new int[rows];
            this.elements = new List<LayoutElement>();
            for (int i = 0; i < rows; i++)
            {
                levels[i] = new List<Rect>();
                lElems[i] = new List<LayoutElement>();

                int colCount = 0;
                for (int j = 0; j < cols; j++)
                {
                    if (count >= elements.Length) break;

                    colCount++;
                    LayoutElement element = elements[count];
                    count++;

                    float h = 1, w = 1;
                    //if (element != fixedElement)
                    //{
                    //    float sq = element.GetSquare();
                    //    float spareUsed = Mathf.Max(element.mSquare, sq + spare) - sq;
                    //    h = Mathf.Max(mHeight, element.GetSize()[1]);
                    //    w = ((element.GetSquare() + spareUsed) / h);
                    //    if (w < element.mWidth)
                    //    {
                    //        w = element.mWidth;
                    //        h = ((element.GetSquare() + spareUsed) / w);
                    //    }
                    //    spare -= spareUsed;
                    //} else
                    //{
                        w = element.GetSize()[0];
                        h = element.GetSize()[1];
                    //}

                    Rect rect = GetRectForModule(w, h, levels[i], i > 0 ? levels[i - 1] : null);

                    element.SetSize(rect.width, rect.height, true);
                    element.SetPosition(new Vector2(rect.x, rect.y));
                    element.SetIndexes(j, i);

                    levels[i].Add(rect);
                    lElems[i].Add(element);
                    this.elements.Add(element);
                }
                matrix[i] = colCount;
            }

            rMatrix = new Rect[levels.Length][];
            eMatrix = new LayoutElement[levels.Length][];
            for (int i = 0; i < levels.Length; i++)
            {
                rMatrix[i]    = levels[i].ToArray();
                eMatrix[i]    = lElems[i].ToArray();
            }

            float[] size = GetAppSize(levels);
            appartment.SetSize(size[0], size[1]);
        }

        public Vector2 GetAppSize()
        {
            return new Vector2(appartment.GetSize()[0], appartment.GetSize()[1]);
        }

        Rect GetRectForModule(float w, float h, List<Rect> level, List<Rect> upperLevel = null)
        {
            float x = GetX(level);
            float y = upperLevel == null ? 0 : GetY(x, w, upperLevel);

            return new Rect(new Vector2(x, y), new Vector2(w, h));
        }

        float GetX(List<Rect> level)
        {
            return level.Count > 0 ? level[level.Count - 1].xMax : 0;
        }

        float GetY(float x, float width, List<Rect> upperLevel)
        {
            float y1 = 0,
                    y2 = 0;

            int l = upperLevel.Count;
            for (int i = 0; i < l; i++)
            {
                if (upperLevel[i].xMin <= x && upperLevel[i].xMax > x)
                {
                    y1 = upperLevel[i].yMax;
                }
                if (upperLevel[i].xMin < x + width && upperLevel[i].xMax > x + width)
                {
                    y2 = upperLevel[i].yMax;
                }
            }

            return Mathf.Max(y1, y2);
        }

        float[] GetAppSize(List<Rect>[] rects)
        {
            float xMax = 0, yMax = 0;
            for (int i = 0; i < rects.Length; i++)
            {
                int l = rects[i].Count;
                for (int j = 0; j < l; j++)
                {
                    if (rects[i][j].xMax > xMax) xMax = rects[i][j].xMax;
                    if (rects[i][j].yMax > yMax) yMax = rects[i][j].yMax;
                }
            }

            return new float[] { xMax, yMax };
        }

    }
}