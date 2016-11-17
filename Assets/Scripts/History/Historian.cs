using HomeBuilder.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HomeBuilder.History
{
    public class Historian : MonoBehaviour
    {

        public Transform        container;
        public ScreenController screen;

        List<Note> notes;
        private static int currentName = 0;
        private static int count       = 0;
        private static string[] names = new string[] { "Appartment", "Dream House", "The Vault", "Villa", "Castle" };

        public void CreateNewProject()
        {
            Master.FLOW  = true;
            Master.SLIDE = true;

            Appartment appartment = new Appartment(GetName(), Designer.GetId());
            Master.GetInstance().SetCurrent(appartment);

            screen.OpenStyle();
        }

        string GetName()
        {
            string name = names[currentName];
            if (count > 0)
            {
                name += " " + (count + 1);
            }
            currentName++;
            if (currentName >= names.Length)
            {
                currentName = 0;
                count++;
            }
            return name;
        }

        void Start()
        {
            Note.onTriggered += OnNoteTriggered;

            notes = new List<Note>();
            Core.History.Memo[] memos = Master.GetInstance().history.GetSaved(); 
            for (int i = 0; i < memos.Length; i++)
            {
                notes.Add( CreateNote(memos[i], new Vector3(0, -i * 220, 0)) );
            }
        }

        Note CreateNote(Core.History.Memo memo, Vector3 pos)
        {
            GameObject obj = Instantiate(Resources.Load(Assets.GetInstance().prefabs.note), container) as GameObject;
            obj.transform.localScale        = new Vector3(1, 1, 1);
            ((RectTransform) obj.transform).anchoredPosition = pos;

            Note note = obj.GetComponent<Note>();
            note.SetDescription(memo.description);
            note.SetAppartment(memo.appartment);

            return note;
        }

        void OnNoteTriggered(Note note)
        {
            Master.GetInstance().SetCurrent(note.GetAppartment());

            Master.SLIDE = false;

            Master.GetInstance().GetCurrent().SetEditing(false);

            screen.OpenDesigning();
        }

        void OnDestroy()
        {
            Note.onTriggered -= OnNoteTriggered;
        }
    }
}