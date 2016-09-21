﻿using UnityEngine;
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
        Cube[] cubes;

        Appartment appartment;

        public Layout(Appartment ap, Cube[] cubes, ModuleInfo[] infos)
        {
            appartment  = ap;
            this.cubes = cubes;

            Restart(appartment, cubes, infos);
        }

        void Restart(Appartment ap, Cube[] cubes, ModuleInfo[] infos)
        {
            List<LayoutElement> elems = elements;
            if (elems == null)
            {
                elems = new List<LayoutElement>();
            }
            for (int i = 0; i < infos.Length; i++)
            {
                LayoutElement elem;
                if (i < elems.Count) {
                    elem = elems[i];
                } else
                {
                    elem = new LayoutElement(cubes[i], infos[i], this);
                }
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
            appartment.Interchange(l1.info, l2.info);

            int i1 = elements.IndexOf(l1);
            int i2 = elements.IndexOf(l2);

            if (i1 >= 0 && i2 >= 0 && i1 != i2)
            {
                LayoutElement temp = elements[i1];
                elements[i1] = elements[i2];
                elements[i2] = temp;

                List<Cube> cs = new List<Cube>();
                for (int i = 0; i < cubes.Length; i++)
                {
                    if (i == i1)
                    {
                        cs.Add(cubes[i2]);
                    } else if (i == i2)
                    {
                        cs.Add(cubes[i1]);
                    } else
                    {
                        cs.Add(cubes[i]);
                    }
                } 
                cubes = cs.ToArray();
            }

            FixLayout(null, true);
            UpdatePositions();

            FixLayout();
            UpdatePositions();

            //Master.GetInstance().designer.evaluate(appartment);

            //ModuleInfo[] modules = appartment.GetModules();
            //Restart(appartment, cubes, modules);

            if (onChange != null) onChange();
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

        public float GetElemetsSquare(bool min = false)
        {
            LayoutElement[] els = GetElements();
            float sq = 0;
            for (int i = 0; i < els.Length; i++)
            {
                sq += min ? els[i].mSquare : els[i].GetSquare();
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

        float GetSpareSquare(bool min = false)
        {
            return GetSquare() - GetElemetsSquare(min);
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

            if (newWidth < el.mWidth || newHeight < el.mHeight)
            {
                Debug.Log("CATCHED");
                return;
            }

            if (height != newHeight)
            {
                float change = newHeight - height;

                bool[][] involved = GetBoolMatrix(GetAllElementsInvolvedInHeightChange(x, y).ToArray());

                float spareInRow = GetSpareInRow(x, involved[x]);
                if (change < 0 && spareInRow < Mathf.Abs(change))
                {
                    change = -spareInRow;
                }

                if (change <= GetSpareHeight(x, y, involved) /*&& HeightChangeAllowed(x, y)*/)
                {
                    float changed = CompensateHeight(x, y, change, involved);

                    ChangeHeightInvolved(x, y, involved, changed);
                }
            }

            FixLayout();
            UpdatePositions();

            if (width != newWidth)
            {
                float change = newWidth - width;

                bool[][] involved = GetBoolMatrix(GetAllElementsInvolvedInWidthChange(x, y).ToArray());

                float spareInCol = GetSpareInCol(y, involved);
                if (change < 0 && spareInCol < Mathf.Abs( change ) )
                {
                    change = -spareInCol;
                }

                if (change <= GetSpareWidth(x, y, involved) && WidthChangeAllowed(x, y))
                {
                    float changed = CompensateWidth(x, y, change, involved);

                    ChangeWidthInvolved(x, y, involved, changed);
                }
            }

            FixLayout();
            UpdatePositions();

            if (onChange != null) onChange();

        }

        void ChangeHeightInvolved(int x, int y, bool[][] involved, float changed)
        {
            for (int j = 0; j < involved[x].Length; j++)
            {
                if (involved[x][j])
                {
                    eMatrix[x][j].SetSize(rMatrix[x][j].width, rMatrix[x][j].height + changed, true);
                }
            }
        }

        void ChangeWidthInvolved(int x, int y, bool[][] involved, float changed)
        {
            for (int j = 0; j < rMatrix.Length; j++)
            {
                if (y < involved[j].Length && involved[j][y])
                {
                    eMatrix[j][y].SetSize(rMatrix[j][y].width + changed, rMatrix[j][y].height, true);
                }
            }
        }

        bool HeightChangeAllowed(int x, int y)
        {
            for (int i = x - 1; i <= x + 1; i++)
            {
                if (i == x || i < 0 || i >= rMatrix.Length) continue;

                if (rMatrix[x][y].width != rMatrix[i][y].width)
                {
                    return true;
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
                    return true;
                }
            }
            return true;
        }

        float GetSpareWidth(int x, int y, bool[][] involved)
        {
            float spare = 0;
            //for (int i = y - 1; i <= y + 1; i++)
            //{
            //    if (i == y || i < 0 || i >= rMatrix[x].Length) continue;

            //    if (rMatrix[x][y].height == rMatrix[x][i].height)
            //    {
            //        spare += (rMatrix[x][i].width - eMatrix[x][i].mWidth);
            //    } else {

            //    }
            //}
            
            for (int i = 0; i < rMatrix[x].Length; i++)
            {
                if (i == y) continue;

                spare += GetSpareInCol(i, involved);
            }

            return spare;
        }

        float GetSpareHeight(int x, int y, bool[][] involved)
        {
            float spare = 0;
            
            //for (int i = x - 1; i <= x + 1; i++)
            //{
            //    if (i == x || i < 0 || i >= rMatrix.Length) continue;

            //    if (rMatrix[x][y].width == rMatrix[i][y].width)
            //    {
            //        spare += (rMatrix[i][y].height - eMatrix[i][y].mHeight);
            //    }
            //}

            for (int i = 0; i < rMatrix.Length; i++)
            {
                if (i == x) continue;

                spare += GetSpareInRow(i, involved[i]);
            }

            return spare;
        }

        float GetSpareInRow(int i, bool[] involved)
        {
            float spareInRow = float.PositiveInfinity;
            for (int j = 0; j < rMatrix[i].Length; j++)
            {
                if (involved[j])
                {
                    spareInRow = Mathf.Min(spareInRow, rMatrix[i][j].height - eMatrix[i][j].mHeight);
                }
            }
            return spareInRow;
        }

        float GetSpareInCol(int i, bool[][] involved)
        {
            float spareInCol = float.PositiveInfinity;
            for (int j = 0; j < rMatrix.Length; j++)
            {
                if (i < involved[j].Length && involved[j][i])
                {
                    spareInCol = Mathf.Min(spareInCol, rMatrix[j][i].width - eMatrix[j][i].mWidth);
                }
            }

            return spareInCol;
        }
        

        float CompensateHeight(int x, int y, float value, bool[][] involved)
        {
            float left = value;
            //for (int i = x - 1; i <= x + 1; i++)
            //{
            //    if (i == x || i < 0 || i >= rMatrix.Length) continue;

            //    float newHeight = Mathf.Max(eMatrix[i][y].mHeight, rMatrix[i][y].height - left);
            //    eMatrix[i][y].SetSize(rMatrix[i][y].width, newHeight, true);
            //    left -= (rMatrix[i][y].height - newHeight);
            //}

            for (int i = 0; i < rMatrix.Length; i++)
            {
                if (i == x) continue;

                float available = GetSpareInRow(i, involved[i]);
                float used      = Mathf.Min(left, available);
                bool reallyUsed = false;
                for (int j = 0; j < rMatrix[i].Length; j++)
                {
                    if (involved[i][j])
                    {
                        reallyUsed = true;
                        float newHeight = Mathf.Max(eMatrix[i][j].mHeight, rMatrix[i][j].height - used);
                        eMatrix[i][j].SetSize(rMatrix[i][j].width, newHeight, true);
                    }
                }
                if (reallyUsed)
                {
                    left -= used;
                }
            }

            return value - left;
        }

        float CompensateWidth(int x, int y, float value, bool[][] involved)
        {
            float left = value;
            //for (int i = y - 1; i <= y + 1; i++)
            //{
            //    if (i == y || i < 0 || i >= rMatrix[x].Length) continue;

            //    float newWidth = Mathf.Max(eMatrix[x][i].mWidth, rMatrix[x][i].width - left);
            //    eMatrix[x][i].SetSize(newWidth, rMatrix[x][i].height, true);
            //    left -= (rMatrix[x][i].width - newWidth);
            //}

            //for (int i = 0; i < rMatrix.Length; i++)
            //{
                for (int j = 0; j < rMatrix[0].Length; j++)
                {
                    if (j == y) continue;

                    float available = GetSpareInCol(j, involved);
                    float used = Mathf.Min(left, available);
                    bool reallyUsed = false;
                    for (int k = 0; k < rMatrix.Length; k++)
                    {
                        if (rMatrix[k].Length > j && involved[k][j])
                        {
                            reallyUsed = true;
                            float newWidth = Mathf.Max(eMatrix[k][j].mWidth, rMatrix[k][j].width - used);
                            eMatrix[k][j].SetSize(newWidth, rMatrix[k][j].height, true);
                        }
                    }
                    if (reallyUsed)
                    {
                        left -= used;
                    }
                }
            //}

            return value - left;
        }

        List<Vector2> GetAllElementsInvolvedInHeightChange(int x, int y, List<Vector2> elements = null)
        {
            if (elements == null)
            {
                elements = new List<Vector2>();
            }
            Vector2 v = new Vector2(x, y);
            if (!elements.Contains(v))
                elements.Add(v);

            for (int i = x - 1; i <= x + 1; i++)
            {
                if (i == x || i < 0 || i >= rMatrix.Length || y >= rMatrix[i].Length) continue;

                //if (rMatrix[x][y].width == rMatrix[i][y].width)
                //{
                //    Vector2 v2 = new Vector2(i, y);
                //    if (elements == null || !elements.Contains(v2))
                //        elements.Add(v2);
                //}
                //else
                //{
                    //if (rMatrix[x][y].width > rMatrix[i][y].width)
                    //{
                        for (int j = 0; j < rMatrix[i].Length; j++)
                        {
                            if (IsInXRange(rMatrix[i][j], rMatrix[x][y]) && (elements == null || !elements.Contains(new Vector2(i, j))))
                            {
                                GetAllElementsInvolvedInHeightChange(i,j, elements);
                            }
                        }
                    //} else if (elements == null || !elements.Contains(new Vector2(i, y)))
                    //{
                    //    GetAllElementsInvolvedInHeightChange(i, y, elements);
                    //}
                //}
            }

            return elements;
        }

        List<Vector2> GetAllElementsInvolvedInWidthChange(int x, int y, List<Vector2> elements = null)
        {
            if (elements == null)
            {
                elements = new List<Vector2>();
            }
            Vector2 v = new Vector2(x, y);
            if (!elements.Contains(v))
                elements.Add(v);

            for (int i = y - 1; i <= y + 1; i++)
            {
                if (i == y || i < 0 || i >= rMatrix[x].Length) continue;

                //if (rMatrix[x][y].height == rMatrix[x][i].height)
                //{
                //    Vector2 v2 = new Vector2(x, i);
                //    if (elements == null || !elements.Contains(v2))
                //        elements.Add(v2);
                //}
                //else
                //{
                    //if (rMatrix[x][y].height > rMatrix[x][i].height)
                    //{
                        for (int j = 0; j < rMatrix.Length; j++)
                        {
                            if (i >= rMatrix[j].Length) continue;

                            if (IsInYRange(rMatrix[j][i], rMatrix[x][y]) && (elements == null || !elements.Contains(new Vector2(j, i))))
                            {
                                GetAllElementsInvolvedInWidthChange(j, i, elements);
                            }
                        }
                    //}
                    //else if (elements == null || !elements.Contains(new Vector2(x, i)))
                    //{
                    //    GetAllElementsInvolvedInWidthChange(x, i, elements);
                    //}
                //}
            }

            return elements;
        }

        bool[][] GetBoolMatrix(Vector2[] vectors)
        {
            bool[][] res = new bool[rMatrix.Length][];
            for (int i = 0; i < rMatrix.Length; i++)
            {
                res[i] = new bool[rMatrix[i].Length];
                for (int j = 0; j < rMatrix[i].Length; j++)
                {
                    res[i][j] = false;
                    for (int k = 0; k < vectors.Length; k++)
                    {
                        if (vectors[k].x == i && vectors[k].y == j)
                        {
                            res[i][j] = true;
                        }
                    }
                }
            }
            return res;
        }

        bool IsInXRange(Rect r1, Rect r2)
        {
            if (r1.xMin >= r2.xMin && r1.xMin < r2.xMax)
            {
                return true;
            }
            if (r1.xMax > r2.xMin && r1.xMax < r2.xMax)
            {
                return true;
            }

            return false;
        }

        bool IsInYRange(Rect r1, Rect r2)
        {
            if (r1.yMin >= r2.yMin && r1.yMin < r2.yMax)
            {
                return true;
            }
            if (r1.yMax > r2.yMin && r1.yMax < r2.yMax)
            {
                return true;
            }

            return false;
        }

        public void Remove(LayoutElement element)
        {
            List<Cube> cs = new List<Cube>();
            for (int i = 0; i < cubes.Length; i++)
            {
                if (cubes[i] != element.cube)
                {
                    cs.Add(cubes[i]);
                }
            }
            cubes = cs.ToArray();
            MonoBehaviour.Destroy(element.cube.gameObject);

            elements.Remove(element);

            appartment.RemoveModule(element.info);

            //Master.GetInstance().designer.evaluate(appartment);

            //ModuleInfo[] modules = appartment.GetModules();
            //Restart(appartment, cubes, modules);

            FixLayout(null, true);
            UpdatePositions();

            FixLayout();
            UpdatePositions();

            if (onChange != null) onChange();
        }

        public void Apply()
        {
            for (int i = 0; i < elements.Count; i++)
            {
                ModuleInfo info = elements[i].info;

                info.SetSize(elements[i].GetSize()[0], elements[i].GetSize()[1]);
                info.SetSquare(elements[i].GetSquare());
                info.SetPosition(elements[i].GetPosition().x, elements[i].GetPosition().y);
            }
        }

        public int[] matrix;
        public void FixLayout(LayoutElement fixedElement = null, bool superFix = false)
        {
            LayoutElement[] elements = GetElements();

            float width = Mathf.Sqrt(appartment.GetSquare());
            float height = width;

            int cols = (int)Mathf.Ceil(Mathf.Sqrt(elements.Length));
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
                    if (superFix && element != fixedElement)
                    {
                        float sq = element.GetSquare();
                        float spareUsed = Mathf.Max(element.mSquare, sq + spare / elements.Length) - sq;
                        h = Mathf.Max(mHeight, element.mHeight);
                        w = ((sq + spareUsed) / h);
                        if (w < element.mWidth)
                        {
                            w = element.mWidth;
                            h = ((sq + spareUsed) / w);
                        }
                        spare -= spareUsed;
                    }
                    else
                    {
                        w = element.GetSize()[0];
                        h = element.GetSize()[1];
                    }

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
                if (upperLevel[i].xMin < x + width && upperLevel[i].xMax >= x + width)
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