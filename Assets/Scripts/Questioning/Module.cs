using UnityEngine;

namespace HomeBuilder.Questioning
{
    public class Module : MonoBehaviour
    {

        public Counter main;
        public Counter square;
        public Counter width;
        public Counter height;

        Moduler moduler;

        public void SetModuler(Moduler m)
        {
            moduler = m;
        }

        public void SetLimits()
        {
        }

        public void SetName(string name)
        {
            main.SetName(name);
        }

        public int GetCount()
        {
            return main.count;
        }

        public string GetName()
        {
            return main.GetName();
        }

        public int GetSquare()
        {
            return square.count;
        }

        public int GetWidth()
        {
            return square.count;
        }

        public int GetHeight()
        {
            return square.count;
        }

        void Update()
        {
            main    .SetMax   ((int) Mathf.Min( main.count + moduler.countAvailable, Configuration.Appartment.maxModuleOfType) );

            square  .SetMin(Configuration.Appartment.minSquare);
            square  .SetMax   (square.count + (int) (moduler.squareAvailable));

            if (square.count > 0)
            {
                width   .SetMin(1);
                height  .SetMin(1);
                width   .SetMax(1);
                height  .SetMax(1);

                if (width.count < 1)    width   .SetCount(1);
                if (height.count < 1)   height  .SetCount(1);


                width   .SetMax((int)   (width.count + square.count / (float) height.count));
                height  .SetMax((int)   (height.count + square.count / (float) width.count));
            } else
            {
                width   .SetCount(0);
                height  .SetCount(0);

                width   .SetMin(0);
                height  .SetMin(0);
                width   .SetMax(0);
                height  .SetMax(0);
            }

        }

    }
}