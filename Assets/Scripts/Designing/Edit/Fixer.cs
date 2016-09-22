using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HomeBuilder.Core;

namespace HomeBuilder.Designing
{
    public class Fixer
    {
        public Rect[][] lastRes;
        public float width, height;

        public Rect[][] Fix(Appartment appartment)
        {
            float width = Mathf.Sqrt(appartment.GetSquare());
            float height = width;

            ModuleInfo[] modules = appartment.GetModules();

            int cols = (int)Mathf.Ceil(Mathf.Sqrt(modules.Length));
            int rows = (int)Mathf.Ceil(modules.Length / (float)cols);

            float lastRow = modules.Length % cols;
            List<Rect>[] levels = null;

            levels = GetScheme1(appartment);

            float[] size = GetAppSize(levels);
            appartment.SetSize(size[0], size[1]);

            Rect[][] res = new Rect[levels.Length][];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = levels[i].ToArray();
            }
            lastRes = res;
            return res;
        }

        public Rect[][] FixOnline(Appartment appartment, List<Rect>[] levels, float[] spares)
        {
            levels = FixScheme(appartment, levels, spares);
           
            Rect[][] res = new Rect[levels.Length][];
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = levels[i].ToArray();
            }
            lastRes = res;
            return res;
        }

        List<Rect>[] GetDefaultScheme(Appartment appartment)
        {
            ModuleInfo[] modules = appartment.GetModules();

            float spareSq = GetSpareSquare(modules, appartment.GetSquare());

            width   = Mathf.Sqrt(appartment.GetSquare());
            height  = width;

            int cols = (int)Mathf.Ceil(Mathf.Sqrt(modules.Length));
            int rows = (int)Mathf.Ceil(modules.Length / (float)cols);

            float mHeight = height / rows;

            int count = 0;

            float spare = spareSq / modules.Length;

            List<Rect>[] levels = new List<Rect>[rows];

            for (int i = 0; i < rows; i++)
            {
                levels[i] = new List<Rect>();

                for (int j = 0; j < cols; j++)
                {
                    if (count >= modules.Length) break;
                    ModuleInfo module = modules[count];
                    count++;

                    float h = Mathf.Max(mHeight, module.GetSize()[1]);
                    float w = ((module.GetSquare() + spare) / h);
                    if (w < module.GetSize()[0])
                    {
                        w = module.GetSize()[0];
                        h = ((module.GetSquare() + spare) / w);
                    }

                    Rect rect = GetRectForModule(module, w, h, levels[i], i > 0 ? levels[i - 1] : null);

                    module.SetSize(rect.width, rect.height);
                    module.SetPosition(rect.x, rect.y);

                    levels[i].Add(rect);
                }
            }

            return levels;
        }

        List<Rect>[] GetScheme1(Appartment appartment)
        {
            ModuleInfo[] modules = appartment.GetModules();

            int defCols = (int)Mathf.Ceil(Mathf.Sqrt(modules.Length));
            int rows = (int)Mathf.Ceil(modules.Length / (float)defCols);

            int cols = defCols;
            float lastRow = modules.Length % defCols;
            if (lastRow == 1 && modules.Length > 3)
            {
                cols = defCols + 1;
                rows--;
            }

            if (modules.Length == 2)
            {
                cols = 1;
                rows = 2;
                lastRow = 0;
            }

            width   = Mathf.RoundToInt(Mathf.Sqrt(appartment.GetSquare()));
            height  = width;

            List<Rect>[] levels = new List<Rect>[rows];

            int totalCount      = 0;
            float shareWidth    = 0;
            for (int i = rows-1; i >= 0; i--)
            {
                bool measuring = shareWidth == 0;
                
                levels[i] = new List<Rect>();

                int end = cols;
                if (lastRow == 1 && modules.Length > 3)
                {
                    end = i == rows - 1 ? cols : defCols;
                } else if (lastRow != 0)
                {
                    end = i == rows - 1 ? (int) lastRow : defCols;
                }

                float rH = 0;
                float[] widths = new float[end];

                int yCount = 0;
                for (int j = 0; j < end; j++)
                {
                    ModuleInfo module = modules[totalCount];

                    if (measuring)
                    {
                        widths[j] = Mathf.Max(width / end, module.GetParams().minWidth);
                    }
                    else
                    {
                        widths[j] = Mathf.Max(shareWidth / end, module.GetParams().minWidth);
                    }
                    rH = Mathf.Max(rH, Mathf.Max(module.GetParams().minSquare / widths[j], module.GetParams().minHeight));

                    yCount++;
                    totalCount++;
                }

                totalCount -= yCount;
                for (int j = 0; j < end; j++)
                {
                    ModuleInfo module = modules[totalCount];

                    float h = rH;
                    float w = widths[j];

                    Rect rect = GetRectForModule(module, w, h, levels[i], i > 0 ? levels[i - 1] : null);

                    module.SetSize(rect.width, rect.height);
                    module.SetPosition(rect.x, rect.y);

                    levels[i].Add(rect);

                    if (measuring) shareWidth += rect.width;
                    totalCount++;
                }
            }

            return FixScheme(appartment, levels);
        }

        List<Rect>[] FixScheme(Appartment appartment, List<Rect>[] rLevels)
        {
            ModuleInfo[] modules = appartment.GetModules();

            float spare = GetSpareSquare(modules, appartment.GetSquare());

            float shareRowWidth = 0;
            List<Rect>[] levels = new List<Rect>[rLevels.Length];
            int totalCount = 0;
            for (int i = 0; i < levels.Length; i++)
            {
                levels[i] = new List<Rect>();
                int end = rLevels[i].Count;

                float spareForRow = spare / levels.Length;

                float rowWidth = 0;
                for (int k = 0; k < end; k++)
                {
                    rowWidth += rLevels[i][k].width;
                }
                float rowHeight = rLevels[i][0].height;

                float newRowHeight  = rowHeight;
                float newRowWidth   = rowWidth;
                float spareForCol   = spareForRow / end;
                float spareWidthForCol = spareForCol / rowHeight;

                float cWidth    = 0;
                float cHeight   = 0;

                if (shareRowWidth == 0)
                {
                    newRowHeight  = rowHeight + spareForRow / (rowWidth + (spareWidthForCol * end) / 2);
                    newRowWidth   = (rowHeight * rowWidth + spareForRow) / newRowHeight;

                    cWidth      = Mathf.RoundToInt( newRowWidth / end );
                    cHeight     = Mathf.RoundToInt( newRowHeight );

                    shareRowWidth = cWidth * end;
                }

                newRowWidth   = shareRowWidth;
                newRowHeight  = (rowHeight * rowWidth + spareForRow) / newRowWidth;

                cWidth   = newRowWidth / end;
                cHeight  = newRowHeight;

                for (int j = 0; j < end; j++)
                {
                    ModuleInfo module = modules[totalCount];

                    float h = cHeight;
                    float w = cWidth;

                    Rect rect = GetRectForModule(module, w, h, levels[i], i > 0 ? levels[i - 1] : null);

                    module.SetSize(rect.width, rect.height);
                    module.SetPosition(rect.x, rect.y);

                    levels[i].Add(rect);

                    totalCount++;
                }
            }

            return levels;
        }

        List<Rect>[] FixScheme(Appartment appartment, List<Rect>[] rLevels, float[] spares)
        {
            ModuleInfo[] modules = appartment.GetModules();

            float maxWidth = 0;
            for (int i = 0; i < rLevels.Length; i++)
            {
                float rowWidth = 0;
                for (int j = 0; j < rLevels[i].Count; j++)
                {
                    rowWidth += rLevels[i][j].width;
                }

                maxWidth = Mathf.Max(maxWidth, rowWidth);
            }

            List<Rect>[] levels = new List<Rect>[rLevels.Length];
            int totalCount = 0;
            for (int i = 0; i < levels.Length; i++)
            {
                levels[i] = new List<Rect>();
                int end = rLevels[i].Count;

                float spareForRow = spares[i];
                if (spareForRow == 0)
                {
                    levels[i].AddRange(rLevels[i]);
                    totalCount += rLevels[i].Count;

                    continue;
                }

                float rowWidth = 0;
                for (int k = 0; k < end; k++)
                {
                    rowWidth += rLevels[i][k].width;
                }
                float rowHeight = rLevels[i][0].height;

                float newRowHeight = rowHeight;
                float newRowWidth = rowWidth;
                float spareForCol = spareForRow / end;
                float spareWidthForCol = spareForCol / rowHeight;

                float cWidth = 0;
                float cHeight = 0;

                newRowWidth     = maxWidth;
                newRowHeight    = (rowHeight * rowWidth + spareForRow) / newRowWidth;

                cWidth = newRowWidth / end;
                cHeight = newRowHeight;

                for (int j = 0; j < end; j++)
                {
                    ModuleInfo module = modules[totalCount];

                    float h = cHeight;
                    float w = cWidth;

                    Rect rect = GetRectForModule(module, w, h, levels[i], i > 0 ? levels[i - 1] : null);

                    module.SetSize(rect.width, rect.height);
                    module.SetPosition(rect.x, rect.y);

                    levels[i].Add(rect);

                    totalCount++;
                }
            }

            return levels;
        }

        List<Rect>[] FixScheme1(Appartment appartment)
        {
            return new List<Rect>[1];
        }

        Rect GetRectForModule(ModuleInfo module, float w, float h, List<Rect> level, List<Rect> upperLevel = null)
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
                if (upperLevel[i].xMin <= x && upperLevel[i].xMax >= x)
                {
                    y1 = upperLevel[i].yMax;
                }
                if (upperLevel[i].xMin <= x + width && upperLevel[i].xMax >= x + width)
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

        public float GetSpareSquare(ModuleInfo[] modules, float square)
        {
            float sq = 0;
            for (int i = 0; i < modules.Length; i++)
            {
                sq += modules[i].GetSquare();
            }

            return square - sq;
        }

    }
}