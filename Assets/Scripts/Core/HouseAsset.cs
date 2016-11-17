using System.Collections.Generic;
using UnityEngine;

namespace HomeBuilder.Core
{

    public class HouseAsset : MonoBehaviour
    {

        public ModuleAsset[] modules;
        public ModuleAsset module;

        public House Export()
        {
            List<House.Module> ms = new List<House.Module>();
            for (int i = 0; i < modules.Length; i++)
            {
                ms.Add(modules[i].Export());
            }

            House h = new House(module.Export());
            h.modules.AddRange(ms);
            return h;
        }

    }

}
