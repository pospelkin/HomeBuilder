using UnityEngine;
using System.Collections.Generic;

namespace HomeBuilder.Questioning
{
    public class Moduler : MonoBehaviour
    {

        public Module[] modules;

        float _squareTotal  = 0;
        float _squareUsed {
            get { return GetTotalSquare(); }
        }
        public float squareAvailable {
            get { return _squareTotal - _squareUsed;  }
        }

        public float countAvailable
        {
            get { return Mathf.Floor(squareAvailable / Configuration.Appartment.minSquare); }
        }

        public ModuleInfo[] GetModules()
        {
            List<ModuleInfo> infos = new List<ModuleInfo>();

            for (int i = 0; i < modules.Length; i++)
            {
                if (modules[i].GetCount() > 0)
                {
                    ModuleInfo info = new ModuleInfo();
                    info.name       = modules[i].GetName();
                    info.count      = modules[i].GetCount();
                    info.minSquare  = modules[i].GetSquare();
                    info.minWidth   = modules[i].GetWidth();
                    info.minHeight  = modules[i].GetHeight();

                    infos.Add(info);
                }
            }

            return infos.ToArray();
        }

        public void SetLimits(Core.Appartment app)
        {
            _squareTotal    = app.GetSquare();

            for (int i = 0; i < modules.Length; i++)
            {
                modules[i].SetLimits();
            }
        }

        public int GetTotalCount()
        {
            int count = 0;

            for (int i = 0; i < modules.Length; i++)
            {
                count += modules[i].main.count;
            }

            return count;
        }

        public int GetTotalSquare()
        {
            int sqaure = 0;

            for (int i = 0; i < modules.Length; i++)
            {
                sqaure += modules[i].main.count * modules[i].square.count;
            }

            return sqaure;
        }

        void Start()
        {
            for (int i = 0; i < modules.Length; i++)
            {
                modules[i].SetModuler(this);
            }
        }

        public class ModuleInfo
        {
            public string name;
            public int count;
            public int minSquare;
            public int minWidth;
            public int minHeight;
        }

    }
}