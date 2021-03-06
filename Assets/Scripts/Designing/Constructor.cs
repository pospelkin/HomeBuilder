﻿using UnityEngine;
using UnityEngine.UI;
using HomeBuilder.Core;
using DG.Tweening;
using HomeBuilder.Questioning;

namespace HomeBuilder.Designing
{
    public class Constructor : MonoBehaviour
    {

        public GCameraController cameraController;
        public Transform mainTransform;
        public Transform[] containers;
        public Text floorText;
        public Canvas canvas;
        public Button toggler;
        public ContextModules contextModules;
        public Animator saveAnimator;
        public Text caption;
        public GameObject modules;
        public GameObject shareBtn;
        public InputHandler input;
        public ScreenController screen;

        float[] oldValues = new float[] { 0, 0, 0 };

        Editor[]      editors;
        Layout[]      layouts;
        Appartment[]  appartments;
        Appartment  appartment;

        private int current = 0;
        //List<Cube>  cubes;

        public void Menu()
        {
            appartment.SetEditing(false);

            Master.FLOW  = false;
            Master.SLIDE = false;

            screen.OpenHistory();
        }

        public void EditModules()
        {
            if (!contextModules.IsOpen)
            {
                contextModules.Open();
            }
            else
            {
                contextModules.Close();
            }
        }

        public Layout GetCurrentLayout()
        {
            return layouts[current];
        }

        public Editor GetCurrentEditor()
        {
            return editors[current];
        }

        public void Save()
        {
            SaveAppartment();

            saveAnimator.SetTrigger("Accept");
        }

        public void SaveAppartment()
        {
            for (int i = 0; i < layouts.Length; i++)
            {
                layouts[i].Apply();
                layouts[i].appartment.SetSaved();
            }
            appartment.SavePlan(layouts);
            appartment.SetSaved();
            Master.GetInstance().history.Save(appartment.GetName(), appartment);
        }

        public void AddModule(House.Module module)
        {

            Debug.Log("Adding module: " + module.name);

            contextModules.Close();

            ModuleInfo moduleInfo = new ModuleInfo(module.name);
            moduleInfo.SetParams(new Moduler.ModuleInfo(module).param);

            moduleInfo.SetFloor(current);
            Layout layout = layouts[current];
            layout.AddModule(moduleInfo);

            editors[current].RecreatePlans();
        }

        public void Toggle2D()
        {
            cameraController.enabled = !cameraController.enabled;

            if (!cameraController.enabled)
            {
                OpenPlan();
            } else
            {
                OpenModel();
            }
        }

        public void OpenPlan()
        {
            caption.text = "Edit Mode";

            SphereCamera cam = (SphereCamera)cameraController.gCamera;

            oldValues[0] = cam.GetTita();
            oldValues[1] = cam.GetPhi();
            oldValues[2] = cam.GetRadius();

            cam.SetTita(0);
            cam.SetPhi(0);

            cameraController.gCamera.UpdatePosition();

            foreach (Editor editor in editors)
            {
                editor.TurnOn();
            }
            toggler.GetComponentInChildren<Text>().text = "View";
            SetFloor();
            modules.SetActive(true);
            //shareBtn.SetActive(true);
        }

        public void OpenModel()
        {
            caption.text = "View Mode";

            SphereCamera cam = (SphereCamera)cameraController.gCamera;

            cam.SetTita(oldValues[0]);
            cam.SetPhi(oldValues[1]);
            cam.SetRadius(oldValues[2]);

            cameraController.gCamera.UpdatePosition();

            foreach (Editor editor in editors)
            {
                editor.TurnOff();
            }
            toggler.GetComponentInChildren<Text>().text = "Modify";
            modules.SetActive(false);
            //shareBtn.SetActive(false);

        }

        void Start()
        {
            appartment = Master.GetInstance().GetCurrent();
            if (appartment == null)
            {
                Menu();
            }

            if (!appartment.IsAllSet())
            {
                Master.GetInstance().designer.evaluate(appartment);
            }
            layouts = new Layout[appartment.GetFloors()];
            for (int i = 0; i < layouts.Length; i++)
            {
                if (!appartment.IsSaved() || appartment.IsEditing())
                {
                    layouts[i] = CreateLayout(appartment.GetFloor(i));
                }
                else
                {
                    layouts[i] = appartment.GetPlan(i);
                }
                
                layouts[i].UpdatePositions();
            }
            editors = new Editor[appartment.GetFloors()];
            for (int i = 0; i < editors.Length; i++)
            {
                GameObject obj = Instantiate(Resources.Load(Assets.GetInstance().prefabs.editor), transform) as GameObject;
                editors[i] = obj.GetComponent<Editor>();
                editors[i].SetContainer(containers[i]);
                editors[i].SetCanvas(canvas);
                editors[i].SetLayout(layouts[i]);
            }

            editors[current].TurnOff();
        }

        void Update()
        {
            
        }

        void OnEnable()
        {
            input.onSwap += OnSwap;
        }

        void OnDisable()
        {
            input.onSwap -= OnSwap;
        }

        void OnSwap(Vector3 position, Vector3 shift)
        {
            if (cameraController.enabled) return;

            bool flow = true;
            Editor prev = editors[current];
            if (shift.x < 0)
            {
                current++;
            }
            else
            {
                flow = false;
                current--;
            }

            if (current >= editors.Length) current = 0;
            if (current < 0) current = editors.Length-1;

            Editor next = editors[current];
            if (next != prev)
            {
                AnimateChange(prev, next, flow);    
            }
        }

        public void OpenFloor(int i)
        {
            if (i >= editors.Length || i < 0) return;

            Editor prev = editors[current];
            current = i;
            Editor next = editors[current];

            RectTransform t1 = (RectTransform)prev.container;
            RectTransform t2 = (RectTransform)next.container;

            t1.anchoredPosition = new Vector2(-2730, 0);
            t2.anchoredPosition = new Vector2(0, 0);
        }

        void AnimateChange(Editor prev, Editor next, bool flow = true)
        {
            RectTransform t1 = (RectTransform) prev.container;
            RectTransform t2 = (RectTransform) next.container;

            if (flow)
            {
                t1.DOAnchorPos(new Vector2(-2730, 0), 0.25f);
                t2.anchoredPosition = new Vector2(2730, 0);
                t2.DOAnchorPos(new Vector2(0, 0), 0.25f);
            }
            else
            {
                t1.DOAnchorPos(new Vector2(2730, 0), 0.25f);
                t2.anchoredPosition = new Vector2(-2730, 0);
                t2.DOAnchorPos(new Vector2(0, 0), 0.25f);
            }

            SetFloor();
        }

        void SetFloor()
        {
            floorText.text = "Floor " + (current + 1) + "/" + appartment.GetFloors();
        }

        Layout CreateLayout(Appartment app)
        {
            //cubes = new List<Cube>();
            ModuleInfo[] modules = app.GetModules();
            //for (int i = 0; i < modules.Length; i++)
            //{
            //    cubes.Add(GetCube(modules[i].GetParams().asset));
            //}

            return new Layout(app, /*cubes.ToArray(), */modules);
        }

        void OnDestroy()
        {
        }

        Cube GetCube(string prefab)
        {
            GameObject obj = Instantiate(Resources.Load(prefab), mainTransform) as GameObject;

            Cube cube = obj.GetComponent<Cube>();
            cube.SetSize(1, 1);
            cube.SetPosition(0, 0);

            return cube;
        }
        
    }
}