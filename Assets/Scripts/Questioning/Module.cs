using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace HomeBuilder.Questioning
{
    public class Module : MonoBehaviour
    {
        
        public Text nameText;
        public Text countText;

        public Button moreButton;
        public Button lessButton;

        int count = 0;
        Moduler moduler;

        public void SetModuler(Moduler m)
        {
            moduler = m;
        }

        public void More()
        {
            if (moduler != null && moduler.IsMax()) return;
            count++;

            UpdateCount();
        }

        public void Less()
        {
            count--;

            UpdateCount();
        }

        public void SetName(string name)
        {
            nameText.text = name;
        }

        public int GetCount()
        {
            int count = 0;
            int.TryParse(countText.text, out count);
            return count;
        }

        public string GetName()
        {
            return nameText.text;
        }

        void UpdateCount()
        {
            if (count < 0) count = 0;
            if (count > 5) count = 5;

            countText.text = "" + count;

            moreButton.enabled = !(count == 5 || (moduler != null && moduler.IsMax()));
            lessButton.enabled = !(count == 0);
        }

    }
}