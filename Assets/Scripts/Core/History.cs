using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HomeBuilder.Core
{
    public class History
    {
        static int id = 0;

        List<Memo> memories;

        public History()
        {
            memories = new List<Memo>();
        }

        public Memo[] GetSaved()
        {
            return memories.ToArray();
        }

        public void Save(string descr, Appartment app)
        {
            Memo memo = null;
            foreach (Memo m in memories)
            {
                if (m.appartment.id == app.id)
                {
                    memo = m;
                    break;
                }
            }
            if (memo == null)
            {
                memo = new Memo();
            }
            else
            {
                memories.Remove(memo);
            }

            memo.description    = descr;
            memo.appartment     = app;

            memories.Add(memo);
        }

        public void SimulateHistory()
        {
            for (int i = 0; i < 1; i++)
            {
                Save("Random appartment", Designer.GetRandomAppartment());
            }
        }

        public class Memo
        {
            public string       description;
            public Appartment   appartment;

            public Memo()
            {
            }
        }

    }
}