using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HomeBuilder.Core;

namespace HomeBuilder.Designing
{
    public class Layout
    {
        public Appartment appartment;

        public delegate void OnChange();
        public OnChange onChange;

        readonly public float square;
        List<LayoutElement> elements;
        LayoutElement[][] eMatrix;
        Rect[][] rMatrix;
        //Cube[] cubes;
        Fixer fixer;

        public Layout(Appartment ap, /*Cube[] cubes, */ModuleInfo[] infos)
        {
            fixer       = new Fixer();
            appartment  = ap;
            //this.cubes  = cubes;

            if (!appartment.IsSaved()) fixer.Fix(appartment);
            Restart(appartment, /*cubes, */infos);
        }

        void Restart(Appartment ap, /*Cube[] cubes, */ModuleInfo[] infos)
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
                    elem = new LayoutElement(/*cubes[i], */infos[i], this);
                }
                elem.SetMinParams(
                    infos[i].GetParams().minWidth,
                    infos[i].GetParams().minHeight,
                    infos[i].GetParams().minSquare
                    );
                elem.SetPosition(new Vector2(infos[i].GetPosition()[0], infos[i].GetPosition()[1]));
                elem.SetSize(infos[i].GetSize()[0], infos[i].GetSize()[1], true, false);

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
                //for (int i = 0; i < cubes.Length; i++)
                //{
                //    if (i == i1)
                //    {
                //        cs.Add(cubes[i2]);
                //    } else if (i == i2)
                //    {
                //        cs.Add(cubes[i1]);
                //    } else
                //    {
                //        cs.Add(cubes[i]);
                //    }
                //} 
                //cubes = cs.ToArray();
            }

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
            //float width     = appartment.GetSize()[0];
            //float height    = appartment.GetSize()[1];

            //Cube cube = element.cube;
            //Vector2 size = element.GetSize();
            //cube.SetSize(size.x, size.y);

            //Vector2 pos = element.GetPosition();
            //cube.SetPosition(-width / 2f + pos.x + size.x / 2f, -height / 2f + pos.y + size.y / 2f);
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

        public void RequestChange(LayoutElement el, float newWidth, float newHeight, Vector2 type)
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

            Vector2 rounded = RoundToNearest(newWidth, newHeight, new Vector2(x, y), type);
            newWidth        = rounded.x;
            newHeight       = rounded.y;

            if (newWidth < el.mWidth || newHeight < el.mHeight)
            {
                Debug.Log("CATCHED");
                return;
            }

            if (height != newHeight)
            {
                float change = newHeight - height;

                bool[][] involved = GetBoolMatrix(GetAllElementsInvolvedInHeightChange(x, y, null, (int) type.y).ToArray());

                float spareInRow = GetSpareInRow(x, involved[x]);
                if (change < 0 && spareInRow < Mathf.Abs(change))
                {
                    change = -spareInRow;
                }

                if (change <= GetSpareHeight(x, y, involved, (int) type.y) /*&& HeightChangeAllowed(x, y)*/)
                {
                    float changed = CompensateHeight(x, y, change, involved, (int) type.y);

                    ChangeHeightInvolved(x, y, involved, changed);
                }
            }

            FixLayout();
            UpdatePositions();

            if (width != newWidth)
            {
                float change = newWidth - width;

                bool[][] involved = GetBoolMatrix(GetAllElementsInvolvedInWidthChange(x, y, null, (int) type.x).ToArray());

                float spareInCol = GetSpareInCol(y, involved);
                if (change < 0 && spareInCol < Mathf.Abs( change ) )
                {
                    change = -spareInCol;
                }

                if (change <= GetSpareWidth(x, y, involved, (int)type.x)/* && WidthChangeAllowed(x, y)*/)
                {
                    float changed = CompensateWidth(x, y, change, involved, (int)type.x);

                    ChangeWidthInvolved(x, y, involved, changed);
                }
            }

            FixLayout();
            UpdatePositions();

            if (onChange != null) onChange();

        }

        Vector2 RoundToNearest(float width, float height, Vector2 position, Vector2 type)
        {
            float w = width,
                    h = height;

            float nearingLimit = 0.4f;

            int posX = (int)position.x,
                posY = (int)position.y;

            float x = rMatrix[posX][posY].x,
                    y = rMatrix[posX][posY].y;

            for (int i = 0; i <= rMatrix.Length; i++)
            {
                if (i < 0 || i >= eMatrix.Length || posY < 0 || posY >= eMatrix[i].Length || i == posX) continue;

                float tX = 0;
                if (type.x > 0)
                {
                    tX = x + width;
                }
                else if (type.x < 0)
                {
                    tX = x + (rMatrix[posX][posY].width - width);
                }
                if (Mathf.Abs(rMatrix[i][posY].xMin - tX) < nearingLimit)
                {
                    w = width + type.x * (rMatrix[i][posY].xMin - tX);
                    break;
                }
                if (Mathf.Abs(rMatrix[i][posY].xMax - tX) < nearingLimit)
                {
                    w = width + type.x * (rMatrix[i][posY].xMax - tX);
                    break;
                }
            }

            for (int j = posY - 1; j <= posY + 1; j++)
            {
                if (j < 0 || j >= rMatrix[posX].Length || j == posY) continue;

                float tY = 0;
                if (type.y > 0)
                {
                    tY = y + height;
                }
                else if (type.y < 0)
                {
                    tY = y + (rMatrix[posX][posY].height - height);
                }

                if (Mathf.Abs(rMatrix[posX][j].yMin - tY) < nearingLimit)
                {
                    h = height + type.y * (rMatrix[posX][j].yMin - tY);
                    break;
                }
                if (Mathf.Abs(rMatrix[posX][j].yMax - tY) < nearingLimit)
                {
                    h = height + type.y * (rMatrix[posX][j].yMax - tY);
                    break;
                }
            }

            return new Vector2(w, h);
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

        int chahgeRoundCount = 0;
        void ChangeWidthInvolved(int x, int y, bool[][] involved, float changed)
        {
            Debug.Log("------------CHANGE ROUND________" + chahgeRoundCount);
            for (int j = 0; j < rMatrix.Length; j++)
            {
                int[] index = GetAnalogForCol(x, y, j);
                Debug.Log("Index " + x + ", " + y + " have " + index.Length + " analogs.");
                for (int l = 0; l < index.Length; l++)
                {
                    int ind = index[l];
                    if (ind < involved[j].Length && involved[j][ind])
                    {
                        float width = eMatrix[j][ind].GetSize().x;
                        float changePerUnit = changed / index.Length;
                        eMatrix[j][ind].SetSize(width + changePerUnit, eMatrix[j][ind].GetSize().y, true);
                        Debug.Log("Changed cube (" + j + ", " + ind + ") " + (eMatrix[j][ind].GetSize().x - width) + " to " + width + " + " + changePerUnit + " = " + eMatrix[j][ind].GetSize().x);
                    }
                }
            }
            Debug.Log("------------CHANGE ROUND END________" + chahgeRoundCount++);
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

        float GetSpareWidth(int x, int y, bool[][] involved, int dir)
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
            List<Vector2> changed = new List<Vector2>();
            if (dir > 0)
            {
                for (int i = y + 1; i < rMatrix[x].Length; i++)
                {
                    spare += GetSpareInCol(i, involved, changed);
                }
            } else if (dir < 0)
            {
                for (int i = 0; i < y; i++)
                {
                    spare += GetSpareInCol(i, involved, changed);
                }
            }

            return spare;
        }

        float GetSpareHeight(int x, int y, bool[][] involved, int dir)
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

            if (dir > 0)
            {
                for (int i = x + 1; i < rMatrix.Length; i++)
                {
                    spare += GetSpareInRow(i, involved[i]);
                }
            } else if (dir < 0)
            {
                for (int i = 0; i < x; i++)
                {
                    spare += GetSpareInRow(i, involved[i]);
                }
            }

            return spare;
        }

        float GetSpareInRow(int i, bool[] involved = null)
        {
            float spareInRow = float.PositiveInfinity;

            for (int j = 0; j < rMatrix[i].Length; j++)
            {
                if (involved == null || involved[j])
                {
                    spareInRow = Mathf.Min(spareInRow, rMatrix[i][j].height - eMatrix[i][j].mHeight);
                }
            }
            
            return spareInRow;
        }

        float GetSpareInCol(int i, bool[][] involved, List<Vector2> changed = null)
        {
            float spareInCol = float.PositiveInfinity;
            for (int j = 0; j < rMatrix.Length; j++)
            {
                int[] index = GetAnalogForCol(0, i, j, true);

                for (int l = 0; l < index.Length; l++)
                {
                    int ind = index[l];
                    if (ind < involved[j].Length && involved[j][ind] && (changed == null || !changed.Contains(new Vector2(j, ind))))
                    {
                        if (changed != null)
                        {
                            changed.Add(new Vector2(j, ind));
                        }
                        spareInCol = Mathf.Min(spareInCol, rMatrix[j][ind].width - eMatrix[j][ind].mWidth);
                    }
                }
            }

            return spareInCol;
        }
        

        float CompensateHeight(int x, int y, float value, bool[][] involved, int dir)
        {
            float left = value;
            //for (int i = x - 1; i <= x + 1; i++)
            //{
            //    if (i == x || i < 0 || i >= rMatrix.Length) continue;

            //    float newHeight = Mathf.Max(eMatrix[i][y].mHeight, rMatrix[i][y].height - left);
            //    eMatrix[i][y].SetSize(rMatrix[i][y].width, newHeight, true);
            //    left -= (rMatrix[i][y].height - newHeight);
            //}
            
            if (dir > 0)
            {
                for (int i = x+1; i < rMatrix.Length; i++)
                {
                    left -= GetHeightCompensated(x, y, i, involved, left);
                }
            } else if (dir < 0)
            {
                for (int i = x-1; i >= 0; i--)
                {
                    left -= GetHeightCompensated(x, y, i, involved, left);
                }
            }

            return value - left;
        }

        float GetHeightCompensated(int x, int y, int i, bool[][] involved, float value)
        {
            float available = GetSpareInRow(i, involved[i]);
            float used = Mathf.Min(value, available);
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
                return used;
            }
            return 0;
        }

        int compRoundCount = 0;
        float CompensateWidth(int x, int y, float value, bool[][] involved, int dir)
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

            //Debug.Log("---------------Compensation round--------------" + compRoundCount++);
            List<Vector2> changed = new List<Vector2>();
            if (dir > 0)
            {
                for (int j = y+1; j < rMatrix[0].Length; j++)
                {
                    left -= GetWidthCompensated(x, y, j, involved, left, null);
                }
            } else if (dir < 0)
            {
                for (int j = y-1; j >= 0; j--)
                {
                    left -= GetWidthCompensated(x, y, j, involved, left, null);
                }
            }
            //Debug.Log("---------------Compensation end--------------");
            //}

            return value - left;
        }

        float GetWidthCompensated(int x, int y, int j, bool[][] involved, float value, List<Vector2> changed)
        {
            float available = GetSpareInCol(j, involved);
            float used = Mathf.Min(value, available);
            if (used == 0) return 0;
            bool reallyUsed = false;
            for (int k = 0; k < rMatrix.Length; k++)
            {
                int[] index = GetAnalogForCol(x, j, k);

                for (int l = 0; l < 1; l++)
                {
                    int ind = index[l];
                    if (rMatrix[k].Length > ind && involved[k][ind] && (changed == null || !changed.Contains(new Vector2(k, ind))))
                    {
                        //if (changed != null)
                        //{
                        //    changed.Add(new Vector2(k, l));
                        //}
                        reallyUsed = true;
                        float width = eMatrix[k][ind].GetSize().x;
                        float newWidth = Mathf.Max(eMatrix[k][ind].mWidth, width - used);
                        eMatrix[k][ind].SetSize(newWidth, rMatrix[k][ind].height, true);
                        //Debug.Log("Compensated in cube (" + k + ", " + ind + ") " + (newWidth - width) + " to " + width + " + " + used + " = " + newWidth);
                    } else
                    {
                        //Debug.Log("BLOCKED " + k + ", " + ind);
                    }
                }
            }
            if (reallyUsed)
            {
                return used;
            }
            return 0;
        }

        List<Vector2> GetAllElementsInvolvedInHeightChange(int x, int y, List<Vector2> elements = null, int dir = 0)
        {
            if (elements == null)
            {
                elements = new List<Vector2>();
            }
            Vector2 v = new Vector2(x, y);
            if (!elements.Contains(v))
                elements.Add(v);

            int from = dir  > 0 ? x : x - 1;
            int to = dir    < 0 ? x : x + 1;
            for (int i = from; i <= to; i++)
            {
                if (i == x || i < 0 || i >= rMatrix.Length /*|| y >= rMatrix[i].Length*/) continue;

                for (int j = 0; j < rMatrix[i].Length; j++)
                {
                    if (IsInXRange(rMatrix[i][j], rMatrix[x][y]) && (elements == null || !elements.Contains(new Vector2(i, j))))
                    {
                        GetAllElementsInvolvedInHeightChange(i,j, elements);
                    }
                }

            }

            return elements;
        }

        List<Vector2> GetAllElementsInvolvedInWidthChange(int x, int y, List<Vector2> elements = null, int dir = 0)
        {
            if (elements == null)
            {
                elements = new List<Vector2>();
            }
            Vector2 v = new Vector2(x, y);
            if (!elements.Contains(v))
                elements.Add(v);

            int from    = dir > 0 ? y : y - 1;
            int to      = dir < 0 ? y : y + 1;
            for (int i = from; i <= to; i++)
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
                            int[] index = GetAnalogForCol(x, i, j);

                            for (int l = 0; l < index.Length; l++)
                            {

                                int ind = index[l];
                                if (ind >= rMatrix[j].Length) continue;

                                if (IsInYRange(rMatrix[j][ind], rMatrix[x][y]) && (elements == null || !elements.Contains(new Vector2(j, ind))))
                                {
                                    GetAllElementsInvolvedInWidthChange(j, ind, elements);
                                }
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

        int[] GetAnalogForCol(int x, int y, int j, bool hz = false)
        {
            if (rMatrix[x].Length == rMatrix[j].Length || (hz && y < rMatrix[j].Length)) return new int[] { y };
            if (y >= rMatrix[x].Length) return new int[] { rMatrix[x].Length - 1 };
            List<int> res = new List<int>();
            for (int i = 0; i < rMatrix[j].Length; i++)
            {
                if (IsInXRange(rMatrix[x][y], rMatrix[j][i]))
                {
                    if (res.Count > 0 && x == 1 && y == 1 && j == 2)
                    {
                        bool whaaaat = IsInXRange(rMatrix[x][y], rMatrix[j][i]);
                        Debug.Log("SecondComming");
                    }
                    res.Add(i);
                }
            }
            if (res.Count > 0) return res.ToArray();
            return y < rMatrix[j].Length ? new int[] { y } : new int[] { rMatrix[j].Length - 1 };
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

        public bool Remove(LayoutElement element)
        {
            int x = 0, y = 0;

            float[] spares = new float[eMatrix.Length];
            for (int i = 0; i < eMatrix.Length; i++)
            {
                spares[i] = 0;
                for (int j = 0; j < eMatrix[i].Length; j++)
                {
                    if (eMatrix[i][j] == element)
                    {
                        x = i;
                        y = j;

                        spares[i] = rMatrix[i][j].width * rMatrix[i][j].height;
                    }
                }
            }

            for (int j = 0; j < rMatrix[x].Length; j++)
            {
                if (rMatrix[x][j].y != rMatrix[x][0].y || rMatrix[x][j].height != rMatrix[x][0].height)
                {
                    return false;
                }
            }

            //List<Cube> cs = new List<Cube>();
            //for (int i = 0; i < cubes.Length; i++)
            //{
            //    if (cubes[i] != element.cube)
            //    {
            //        cs.Add(cubes[i]);
            //    }
            //}
            //cubes = cs.ToArray();

            int toDelete = -1;

            List<Rect>[] list = new List<Rect>[rMatrix.Length];
            for (int i = 0; i < list.Length; i++)
            {
                list[i] = new List<Rect>();
                for (int j = 0; j < rMatrix[i].Length; j++)
                {
                    if (eMatrix[i][j] != element)
                    {
                        list[i].Add(rMatrix[i][j]);
                    }
                }

                if (list[i].Count == 0)
                {
                    toDelete = i;
                }
            }

            if (toDelete >= 0)
            {
                return false;
                //float[] spares2 = new float[rMatrix.Length - 1];
                //int c = 0;
                //for (int i = 0; i < spares.Length; i++)
                //{
                //    if (i == toDelete) continue;
                //    if (c == 0)
                //    {
                //        spares2[c] = spares[toDelete];
                //    } else
                //    {
                //        spares2[c] = spares[i];
                //    }
                //    c++;
                //}
                //List<Rect>[] list2 = new List<Rect>[rMatrix.Length - 1];
                //c = 0;
                //for (int i = 0; i < list.Length; i++)
                //{
                //    if (i != toDelete)
                //    {
                //        list2[c] = list[i];
                //        c++;
                //    }
                //}
                //list = new List<Rect>[list2.Length];
                //list = list2;
            }

            //MonoBehaviour.Destroy(element.cube.gameObject);

            elements.Remove(element);

            appartment.RemoveModule(element.info);

            fixer.FixOnline(appartment, list, spares);
            Restart(appartment, /*cubes, */appartment.GetModules());

            if (onChange != null) onChange();

            return true;
        }

        public void Apply()
        {
            for (int i = 0; i < elements.Count; i++)
            {
                ModuleInfo info = elements[i].info;

                info.SetSize(elements[i].GetSize()[0], elements[i].GetSize()[1]);
                info.SetPosition(elements[i].GetPosition().x, elements[i].GetPosition().y);
            }
        }

        public int[] matrix;
        public void FixLayout(LayoutElement fixedElement = null, bool superFix = false)
        {
            LayoutElement[] elements = GetElements();

            float width     = fixer.width;
            float height    = fixer.height;

            int rows = fixer.lastRes.Length;

            float mHeight = height / rows;

            int count = 0;

            float spare = GetSpareSquare();

            List<Rect>[] levels             = new List<Rect>[rows];
            List<LayoutElement>[] lElems    = new List<LayoutElement>[rows];

            matrix = new int[rows];
            this.elements = new List<LayoutElement>();
            for (int i = 0; i < rows; i++)
            {
                int cols = fixer.lastRes[i].Length;

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
                rMatrix[i] = levels[i].ToArray();
                eMatrix[i] = lElems[i].ToArray();
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
            float y = 0;

            float x1 = x,
                    x2 = x + width;


            int l = upperLevel.Count;
            for (int i = 0; i < l; i++)
            {
                if (IsInXRange(x1, x2, upperLevel[i].xMin, upperLevel[i].xMax))
                {
                    y = Mathf.Max(y, upperLevel[i].yMax);
                }
            }

            return y;
        }

        bool IsInXRange(float x11, float x12, float x21, float x22)
        {
            float tX = x21;
            while (tX < x22)
            {
                if (tX > x11 && tX < x12) return true;
                tX += 1f;
            }
            return false;
        }

        bool IsInRange(float x11, float x12, float x21, float x22)
        {
            float tX = x21;
            while (tX < x22)
            {
                if (tX == x21)
                {
                    if (tX >= x11 && tX < x12) return true;
                }
                else
                {
                    if (tX > x11 && tX <= x12) return true;
                }
                tX += 0.1f;
            }
            return false;
        }

        bool IsInXRange(Rect r1, Rect r2)
        {
            return IsInRange(RoundTo100(r1.xMin), RoundTo100(r1.xMax) , RoundTo100(r2.xMin), RoundTo100(r2.xMax));
        }

        float RoundTo100 (float value)
        {
            return Mathf.RoundToInt(value * 100f) / 100f;
        }

        bool IsInYRange(Rect r1, Rect r2)
        {
            return IsInRange(RoundTo100(r1.yMin), RoundTo100(r1.yMax), RoundTo100(r2.yMin), RoundTo100(r2.yMax));
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