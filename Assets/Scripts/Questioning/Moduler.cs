using UnityEngine;
using System.Collections.Generic;
using HomeBuilder.Core;
using UnityEngine.UI;

namespace HomeBuilder.Questioning
{
    public class Moduler : MonoBehaviour
    {

        public Transform content;

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

            int length = allModules.Count;
            for (int i = 0; i < length; i++)
            {
                Module module = allModules[i].GetComponent<Module>();
                if (module != null && module.GetCount() > 0)
                {
                    ModuleInfo info = new ModuleInfo();
                    info.name       = module.GetName();
                    info.count      = module.GetCount();
                    info.param      = module.GetParams();

                    infos.Add(info);
                }
            }

            return infos.ToArray();
        }

        public void SetLimits(Core.Appartment app)
        {
            _squareTotal    = app.GetSquare();

            int length = allModules.Count;
            for (int i = 0; i < length; i++)
            {
                Module module = allModules[i].GetComponent<Module>();
                if (module != null)
                {
                    if (module.main.GetName() == "Floors")
                    {
                        module.main.SetMin(app.GetFloors());
                    }
                    else
                    {
                        int count = 0;
                        foreach (Core.ModuleInfo m in app.GetModules())
                        {
                            if (m.GetName() == module.main.GetName()) count++;
                        }
                        module.main.SetMin(count);
                    }
                    module.SetLimits();
                }
            }
        }

        public void SetState(Core.ModuleInfo[] appModules, int floors)
        {
            int length = allModules.Count;
            for (int i = 0; i < length; i++)
            {
                Module module = allModules[i].GetComponent<Module>();
                if (module != null)
                {
                    if (module.main.GetName() == "Floors")
                    {
                        module.main.SetCount(floors);
                    }
                    else
                    {
                        int count = 0;
                        foreach (Core.ModuleInfo m in appModules)
                        {
                            if (m.GetName().Equals(module.main.GetName())) count++;
                        }
                        module.main.SetCount(count);
                    }
                }
            }
        }

        public int GetTotalCount()
        {
            int count = 0;

            int length = allModules.Count;
            for (int i = 0; i < length; i++)
            {
                Module module = allModules[i].GetComponent<Module>();
                if (module != null)
                {
                    if (module.main.GetName().Contains("Floors")) continue;
                    count += module.main.count;
                }
            }

            return count;
        }

        public int GetTotalSquare()
        {
            int sqaure = 0;

            int length = allModules.Count;
            for (int i = 0; i < length; i++)
            {
                Module module = allModules[i].GetComponent<Module>();
                if (module != null)
                {
                    if (module.IsExtended())
                    {
                        sqaure += module.main.count * module.square.count;
                    }
                    else
                    {
                        if (module.GetParams() != null) sqaure += (int)(module.main.count * module.GetParams().minSquare);
                    }
                }
            }

            return sqaure;
        }

        private List<GameObject> allModules;

        void Start()
        {
            allModules = new List<GameObject>();

            Reset();
        }

        public void Reset()
        {
            if (allModules.Count > 0)
            {
                foreach (GameObject obj in allModules)
                {
                    Destroy(obj);
                }
            }
            allModules.Clear();


            AddSeparator("Floors");

            AddFloorModule();

            AddSeparator("Modules");

            House.Module[] mdls = Master.GetInstance().House.modules.ToArray();
            for (int i = 0; i < mdls.Length; i++)
            {
                AddModule(mdls[i]);
            }
        }

        void AddSeparator(string caption)
        {
            GameObject obj = Instantiate(Resources.Load(Assets.GetInstance().prefabs.separator), content) as GameObject;
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, 0);
            obj.GetComponentInChildren<Text>().text = caption;
            allModules.Add(obj);
        }

        void AddModule(House.Module module)
        {
            GameObject obj = Instantiate(Resources.Load(Assets.GetInstance().prefabs.moduleS), content) as GameObject;
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, 0);
            Module m = obj.GetComponent<Module>();
            m.SetModuler(this);
            m.SetParams(
                new Configuration.Appartment.ModuleParams(module.name, module.minSquare, module.minHeight, module.minHeight)
                );
            m.SetCaption(module.name);
            allModules.Add(obj);
        }

        void AddFloorModule()
        {
            GameObject obj = Instantiate(Resources.Load(Assets.GetInstance().prefabs.moduleS), content) as GameObject;
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, 0);
            Module m = obj.GetComponent<Module>();
            m.SetModuler(this);
            m.SetCaption("Floors");
            m.minCount = 1;
            m.maxCount = 2;
            m.fixesCount = true;

            allModules.Add(obj);
        }

        public class ModuleInfo
        {
            public string name;
            public int count;
            public Configuration.Appartment.ModuleParams param;

            public ModuleInfo()
            {
            }

            public ModuleInfo(House.Module m)
            {
                name = m.name;
                count = 1;
                param = new Configuration.Appartment.ModuleParams(m.name, m.minSquare, m.minWidth, m.minHeight);
            }
        }

    }
}