using UnityEngine;
using System.Collections.Generic;

namespace HomeBuilder.Questioning
{
    public class Moduler : MonoBehaviour
    {

        public Transform content;
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
                    info.param      = modules[i].GetParams();

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
                sqaure += (int) (modules[i].main.count * (modules[i].IsExtended() ? modules[i].square.count : modules[i].GetParams().minSquare));
            }

            return sqaure;
        }

        void Start()
        {
            modules = new Module[Configuration.Appartment.approvedModules.Length];
            for (int i = 0; i < modules.Length; i++)
            {
                Configuration.Appartment.ModuleParams p = Configuration.Appartment.approvedModules[i];
                GameObject obj = Instantiate(Resources.Load(Assets.GetInstance().prefabs.moduleS), content) as GameObject;
                obj.transform.localScale = new Vector3(1, 1, 1);
                obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, 0);

                modules[i] = obj.GetComponent<Module>();
                modules[i].SetModulerAndParams(this, p);
            }
        }

        public class ModuleInfo
        {
            public string name;
            public int count;
            public Configuration.Appartment.ModuleParams param;
        }

    }
}