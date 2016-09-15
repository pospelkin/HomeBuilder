using UnityEngine;
using System.Collections.Generic;

namespace HomeBuilder.Questioning
{
    public class Moduler : MonoBehaviour
    {

        public Module[] modules;

        int limit;

        public ModuleInfo[] GetModules()
        {
            List<ModuleInfo> infos = new List<ModuleInfo>();

            for (int i = 0; i < modules.Length; i++)
            {
                if (modules[i].GetCount() > 0)
                {
                    infos.Add(new ModuleInfo(modules[i].GetName(), modules[i].GetCount()));
                }
            }

            return infos.ToArray();
        }

        public void SetLimit(int limit)
        {
            this.limit = limit;
        }

        public bool IsMax()
        {
            return GetTotalCount() >= limit;
        }

        public int GetTotalCount()
        {
            int count = 0;
            for (int i = 0; i < modules.Length; i++)
            {
                count += modules[i].GetCount();
            }
            return count;
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
            string name;
            int count;

            public ModuleInfo(string name, int count)
            {
                SetName ( name  );
                SetCount( count );
            }

            public void SetName(string name)
            {
                this.name = name;
            }

            public void SetCount(int count)
            {
                this.count = count;
            }

            public string GetName()
            {
                return name;
            }

            public int GetCount()
            {
                return count;
            }
        }

    }
}