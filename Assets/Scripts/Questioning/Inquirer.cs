using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using HomeBuilder.Core;
using System.Collections.Generic;

namespace HomeBuilder.Questioning
{
    public class Inquirer : MonoBehaviour
    {

        public ScreenController screen;

        public Button nextButton;
        public Button prevButton;

        public Moduler moduler;

        private Appartment       _app;

        public string BundleURL = "file://D:/Users/Glukozavr/Workspace/HomeBuilder/Assets/AssetBundles/house";
        public string AssetName = "Bundle";
        public int version = 1;

        public void Next()
        {
            if (!IsReadyToProceed()) return;

            Moduler.ModuleInfo[] modules = moduler.GetModules();
            for (int i = 0; i < modules.Length; i++)
            {
                int length = modules[i].count;
                if (modules[i].name == "Floors")
                {
                    _app.SetFloors(length);
                }
                else
                {
                    for (int j = 0; j < length; j++)
                    {
                        ModuleInfo module = new ModuleInfo(modules[i].name);
                        module.SetParams(modules[i].param);
                        _app.AddModule(module);
                    }
                }
            }

            Master.FLOW  = true;
            Master.SLIDE = true;

            Master.GetInstance().SetCurrentModule(_app.GetModules()[0]);
            screen.OpenStyle();
        }

        public void Prev()
        {
            Master.FLOW  = false;
            Master.SLIDE = true;

            _app.ResetStyle();

            screen.OpenStyle();
        }

        public void Home()
        {
            Master.FLOW  = false;
            Master.SLIDE = true;

            screen.OpenHistory();
        }

        void Start()
        {
            _app     = Master.GetInstance().GetCurrent();

            StartModuler();
        }

        void StartModuler()
        {
            ModuleInfo[] modules = _app.GetModules();
            _app.ResetModules();

            moduler.SetLimits(_app);
            moduler.SetState(modules, _app.GetFloors());
        }

        bool IsReadyToProceed()
        {
            return moduler.GetTotalCount() >= _app.GetFloors();
        }

        public void UpdateBundle()
        {
            DownloadBundle();
        }

        void DownloadBundle()
        {
            StartCoroutine(Downloading());
        }

        IEnumerator Downloading()
        {
            moduler.GetComponent<Animator>().SetBool("Sync", true);
            // Wait for the Caching system to be ready
            while (!Caching.ready)
                yield return null;

            // Load the AssetBundle file from Cache if it exists with the same version or download and store it in the cache
            using (WWW www = WWW.LoadFromCacheOrDownload(BundleURL, version))
            {
                yield return www;
                if (www.error != null)
                {
                    moduler.GetComponent<Animator>().SetBool("Sync", false);
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

                moduler.GetComponent<Animator>().SetBool("Sync", false);
            } // memory is freed from the web stream (www.Dispose() gets called implicitly)
            moduler.GetComponent<Animator>().SetBool("Sync", false);
        }

        void ProcessBundle(BundleAsset bundle)
        {
            if (bundle == null) return;

            Master.GetInstance().SetHouse(bundle.house.Export());
            moduler.Reset();
        }

    }
}
