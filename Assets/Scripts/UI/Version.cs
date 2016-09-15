using UnityEngine;
using UnityEngine.UI;

namespace HomeBuilder.UI
{
    public class Version : MonoBehaviour
    {

        void Start()
        {
            GetComponent<Text>().text = Configuration.name + " " + Configuration.version;
        }
    }
}
