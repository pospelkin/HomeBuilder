﻿using HomeBuilder.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HomeBuilder.History
{
    public class Historian : MonoBehaviour
    {

        public Transform container;
        List<Note> notes;

        public void OnMenu()
        {
            SceneManager.LoadScene(Configuration.Scenes.menuScene);
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
            Debug.Log("Note #" + note.GetId() + "is triggered.");

            Master.GetInstance().SetCurrent(note.GetAppartment());
            SceneManager.LoadScene(Configuration.Scenes.designingScene);
        }
    }
}