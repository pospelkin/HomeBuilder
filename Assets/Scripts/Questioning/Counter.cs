using UnityEngine;
using UnityEngine.UI;


namespace HomeBuilder.Questioning
{
    public class Counter : MonoBehaviour
    {

        public Text     nameText;
        public Text     countText;
        public Button   moreButton;
        public Button   lessButton;

        private int _count = 0;
        public  int count {
            get         { return _count; }
            private set { _count = value; UpdateCount(); }
        }

        private int _max = 0;
        private int _min = 0;
        public int max
        {
            get { return _max; }
            private set { _max = value; }
        }
        public int min
        {
            get { return _min; }
            private set { _min = value; }
        }

        public void SetName(string name)
        {
            nameText.text = name;
        }

        public void SetMin(int value)
        {
            min = value;
            if (count < min) SetCount(min);
        }

        public void SetMax(int value)
        {
            max = value;
            if (count > max) SetCount(max);
        }

        public string GetName()
        {
            return nameText.text;
        }

        public void More()
        {
            if (count + 1 > max) return;

            count++;
        }

        public void Less()
        {
            if (count - 1 < min) return;

            count--;
        }

        public void SetCount(int count)
        {
            if (count < min || count > max) return;

            this.count = count;
        }

        void Start()
        {
            UpdateCount();
        }

        void Update()
        {
            moreButton.enabled = !(count >= max);
            lessButton.enabled = !(count <= min);
        }

        void UpdateCount()
        {
            countText.text = "" + count;
        }
    }
}