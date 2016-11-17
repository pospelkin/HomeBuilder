using UnityEngine;

namespace HomeBuilder.Core
{

    public class StyleAsset : MonoBehaviour
    {

        public string type;

        public Sprite image;

        public House.Style Export()
        {
            return new House.Style(type, image);
        }

    }

}