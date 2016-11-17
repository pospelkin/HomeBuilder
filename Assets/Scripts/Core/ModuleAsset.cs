using System.Collections.Generic;
using UnityEngine;

namespace HomeBuilder.Core
{

    public class ModuleAsset : MonoBehaviour
    {

        public new string name;

        public float minSquare;
        public float minWidth;
        public float minHeight;

        public StyleAsset[] styles;


        public House.Module Export()
        {
            List<House.Style> stls = new List<House.Style>();
            for (int i = 0; i < styles.Length; i++)
            {
                stls.Add(styles[i].Export());
            }

            House.Module m = new House.Module(name, minSquare, minWidth, minHeight);
            m.styles.AddRange(stls);
            return m;
        }
    }

}
