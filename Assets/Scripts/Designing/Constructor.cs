using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using HomeBuilder.Core;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace HomeBuilder.Designing
{
    public class Constructor : MonoBehaviour
    {

        public GCameraController cameraController;
        public Transform mainTransform;
        public Editor editor;
        public Button toggler;

        float[] oldValues = new float[] { 0, 0, 0 };

        Layout      layout;
        Appartment  appartment;
        List<Cube>  cubes;

        public void Menu()
        {
            SceneManager.LoadScene(Configuration.Scenes.menuScene);
        }

        public void Save()
        {
            layout.Apply();
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

                editor.TurnOn();
                toggler.GetComponentInChildren<Text>().text = "View";
            } else
            {
                SphereCamera cam = (SphereCamera)cameraController.gCamera;

                cam.SetTita(oldValues[0]);
                cam.SetPhi(oldValues[1]);
                cam.SetRadius(oldValues[2]);

                cameraController.gCamera.UpdatePosition();

                editor.TurnOff();
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

            layout = CreateLayout();
            layout.UpdatePositions();

            editor.SetLayout(layout);
        }

        void Update()
        {
            
        }

        Layout CreateLayout()
        {
            if (!appartment.IsAllSet())
            {
                Master.GetInstance().designer.evaluate(appartment);
            }

            cubes = new List<Cube>();
            ModuleInfo[] modules = appartment.GetModules();
            for (int i = 0; i < modules.Length; i++)
            {
                cubes.Add(GetCube(modules[i].GetParams().color));
            }

            return new Layout(appartment, cubes.ToArray(), modules);
        }

        void OnDestroy()
        {
            layout.Destory();
        }

        Cube GetCube(Color color)
        {
            GameObject obj = Instantiate(Resources.Load(Assets.GetInstance().prefabs.cube), mainTransform) as GameObject;

            Cube cube = obj.GetComponent<Cube>();
            cube.SetSize(1, 1);
            cube.SetPosition(0, 0);

            obj.GetComponent<MeshRenderer>().material.color = color;

            return cube;
        }
        
    }
}