using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HomeBuilder.Designing;

namespace HomeBuilder.Core
{
    public class Appartment
    {

        public readonly int    id;
        string          name;
        string style;
        private Layout[] floorPlans = null;
        private bool _styleSet = false;
        float           square;
        float           width;
        float           height;
        int             floors = 1;
        bool            saved   = false;
        bool            editing = false;

        List<ModuleInfo> modules;

        public Appartment(string name, int id)
        {
            _styleSet = false;
            this.id = id;

            SetName( name );
            modules = new List<ModuleInfo>();
        }

        public void SetSquare(float square)
        {
            this.square = square;
        }

        public void SetFloors(int value)
        {
            if (value < 1) return;

            floors = value;
        }

        public bool IsEditing()
        {
            return editing;
        }

        public void SetEditing(bool value)
        {
            editing = value;
            if (editing == false && IsSaved())
            {
                SetFloors(floorPlans.Length);

                ResetModules();
                for (int i = 0; i < floorPlans.Length; i++)
                {
                    ModuleInfo[] modules = floorPlans[i].appartment.GetModules();
                    for (int j = 0; j < modules.Length; j++)
                    {
                        AddModule(modules[j]);
                    }
                }
            }
        }

        public bool IsSaved()
        {
            return saved;
        }

        public void SetSaved()
        {
            saved = true;
        }

        public void SavePlan(Layout[] apps)
        {
            floorPlans = apps;
            editing    = false;
        }

        public Layout GetPlan(int floor)
        {
            return floorPlans[floor];
        }

        public Appartment GetFloor(int floor)
        {
            Appartment app = new Appartment("Floor " + floor, floor);
            app.SetFloors(1);
            app.SetSquare(GetSquare() / GetFloors());
            app.SetSize(GetSize()[0], GetSize()[1]);
            app.SetStyle(GetStyle());

            ModuleInfo[] modules = GetModules(floor);
            for (int i = 0; i < modules.Length; i++)
            {
                app.AddModule(modules[i]);
            }

            return app;
        }

        public int GetFloors()
        {
            return floors;
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

        public bool IsStyleSet()
        {
            return _styleSet;
        }

        public bool IsAllStyleSet()
        {
            for (int i = 0; i < modules.Count; i++)
            {
                if (!modules[i].IsStyleSet()) return false;
            }

            return true;
        }

        public void ResetStyle()
        {
            _styleSet = false;
        }

        public void SetStyle(string style)
        {
            _styleSet = true;
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

        public void RemoveModule(ModuleInfo moduleInfo)
        {
            modules.Remove(moduleInfo);
        }

        public ModuleInfo[] GetModules(int floor)
        {
            List<ModuleInfo> mdls = new List<ModuleInfo>();

            foreach (ModuleInfo mod in modules)
            {
                if (mod.GetFloor() == floor) mdls.Add(mod);
            }

            return mdls.ToArray();
        }

        public ModuleInfo[] GetModules()
        {
            return modules.ToArray();
        }

        public void ResetModules()
        {
            modules.Clear();
        }

        public void Interchange(ModuleInfo m1, ModuleInfo m2)
        {
            int i1 = modules.IndexOf(m1);
            int i2 = modules.IndexOf(m2);

            if (i1 >= 0 && i2 >= 0 && i1 != i2)
            {
                ModuleInfo temp = modules[i1];
                modules[i1] = modules[i2];
                modules[i2] = temp;
            }
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