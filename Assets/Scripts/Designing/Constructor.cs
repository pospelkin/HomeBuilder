using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using HomeBuilder.Core;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

namespace HomeBuilder.Designing
{
    public class Constructor : MonoBehaviour
    {

        public GCameraController cameraController;
        public Transform mainTransform;
        public Transform[] containers;
        public Canvas canvas;
        public Button toggler;
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
            Master.FLOW  = false;
            Master.SLIDE = false;

            screen.OpenHistory();
        }

        public void EditModules()
        {
            Master.FLOW = false;
            Master.SLIDE = false;

            screen.OpenModules();
        }

        public void Save()
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

        public void Toggle2D()
        {
            cameraController.enabled = !cameraController.enabled;

            if (!cameraController.enabled)
            {
                SphereCamera cam = (SphereCamera)cameraController.gCamera;

                oldValues[0] = cam.GetTita();
                oldValues[1] = cam.GetPhi();
                oldValues[2] = cam.GetRadius();

                cam.SetTita(0);
                cam.SetPhi(0);
                cam.SetRadius(14);

                cameraController.gCamera.UpdatePosition();

                foreach (Editor editor in editors)
                {
                    editor.TurnOn();
                }
                toggler.GetComponentInChildren<Text>().text = "View";
            } else
            {
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
            }
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
                if (!appartment.IsSaved())
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
            foreach (Layout l in layouts)
            {
                l.Destory();
            }
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