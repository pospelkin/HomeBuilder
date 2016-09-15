﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HomeBuilder.Core
{
    public class Appartment
    {

        readonly int    id;
        string          name;
        string          style;
        float           square;
        float           width;
        float           height;

        List<ModuleInfo> modules;

        public Appartment(string name, int id)
        {
            this.id = id;

            SetName( name );
            modules = new List<ModuleInfo>();
        }

        public void SetSquare(float square)
        {
            this.square = square;
        }

        public float GetSquare()
        {
            return square;
        }

        public void SetSize(float w, float h)
        {
            width = w;
            height = h;
        }

        public float[] GetSize()
        {
            return new float[] { width, height };
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return name;
        }

        public void SetStyle(string style)
        {
            this.style = style;
        }

        public string GetStyle()
        {
            return style;
        }

        public void AddModule(ModuleInfo moduleInfo)
        {
            if (modules.Contains(moduleInfo)) return;

            modules.Add(moduleInfo);
        }

        public ModuleInfo[] GetModules()
        {
            return modules.ToArray();
        }

        public bool IsAllSet()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                if (!modules[i].IsPositioned() || !modules[i].IsSized()) return false;
            }

            return true;
        }
    }
}