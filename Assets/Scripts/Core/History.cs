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
            Memo memo = new Memo(GetUniqueID());

            memo.description    = descr;
            memo.appartment     = app;

            memories.Add(memo);
        }

        int GetUniqueID()
        {
            return id++;
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
            readonly public int id;

            public string       description;
            public Appartment   appartment;

            public Memo(int id)
            {
                this.id = id;
            }
        }

    }
}