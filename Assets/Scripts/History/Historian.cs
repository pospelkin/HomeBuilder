using System;
using System.Collections;
using HomeBuilder.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HomeBuilder.History
{
    public class Historian : MonoBehaviour
    {

        public Transform        container;
        public Animator         syncAnimator;
        public UpdateContent    update;
        public ScreenController screen;

        [HideInInspector]
        public string BundleURL = "file://D:/Users/Glukozavr/Workspace/HomeBuilder/Assets/AssetBundles/house";
        [HideInInspector]
        public string AssetName = "Bundle";
        [HideInInspector]
        public int version = 1;

        List<Note> notes;
        private static int currentName = 0;
        private static int count       = 0;
        private static string[] names = new string[] { "Appartment", "Dream House", "The Vault", "Villa", "Castle" };

        public void Sync()
        {
            StartCoroutine(Downloading());
        }

        IEnumerator Downloading()
        {
            syncAnimator.SetBool("Process", true);
            // Wait for the Caching system to be ready
            while (!Caching.ready)
                yield return null;

            // Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
            using (WWW www = WWW.LoadFromCacheOrDownload(BundleURL, version))
            {
                yield return www;
                if (www.error != null)
                {
                    syncAnimator.SetBool("Process", false);
                    throw new Exception("WWW download had an error:" + www.error);
                }
                AssetBundle bundle = www.assetBundle;
                GameObject obj = null;
                if (AssetName == "")
                    obj = Instantiate(bundle.mainAsset) as GameObject;
                else
                    obj = Instantiate(bundle.LoadAsset(AssetName)) as GameObject;

                if (obj != null) ProcessBundle(obj.GetComponent<BundleAsset>());
                // Unload the AssetBundles compressed contents to conserve memory
                bundle.Unload(false);
            } // memory is freed from the web stream (www.Dispose() gets called implicitly)
            syncAnimator.SetBool("Process", false);
        }

        void ProcessBundle(BundleAsset asset)
        {
            House prevHouse = Master.GetInstance().House;
            House newHouse  = asset.house.Export();

            string upd = prevHouse.GetUpdate(newHouse);
            Master.GetInstance().SetHouse(newHouse);

            update.Open(upd);
        }

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