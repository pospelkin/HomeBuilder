using UnityEngine;
using UnityEngine.UI;

namespace HomeBuilder.History
{

    public class UpdateContent : MonoBehaviour
    {

        public Text content;

        public void Open(string updateText)
        {
            content.text = updateText;

            GetComponent<Animator>().SetBool("Open", true);
        }

        public void Close()
        {
            GetComponent<Animator>().SetBool("Open", false);
        }

    }


}
