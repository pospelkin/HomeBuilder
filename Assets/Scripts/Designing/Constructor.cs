using UnityEngine;
using System.Collections;
using HomeBuilder.Core;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace HomeBuilder.Designing
{
    public class Constructor : MonoBehaviour
    {

        public Transform mainTransform;

        Color[] colors = new Color[] { Color.blue, Color.green, Color.red, Color.yellow, Color.white, Color.magenta, Color.black };
        Appartment appartment;
        List<Cube> cubes;

        public void Menu()
        {
            SceneManager.LoadScene(Configuration.Scenes.menuScene);
        }

        public void Save()
        {
            Master.GetInstance().history.Save(appartment.GetName(), appartment);
        }

        void Start()
        {
            //SetObserverCam();

            appartment = Master.GetInstance().GetCurrent();
            if (appartment == null)
            {
                Menu();
            }

            GenerateCubes();
        }

        void Update()
        {
            
        }

        void GenerateCubes()
        {
            if (!appartment.IsAllSet())
            {
                Master.GetInstance().designer.evaluate(appartment);
            } 

            cubes = new List<Cube>();
            ModuleInfo[] modules = appartment.GetModules();
            for (int i = 0; i < modules.Length; i++)
            {
                cubes.Add( GetCube(modules[i].GetSize()[0], modules[i].GetSize()[1], modules[i].GetPosition()[0], modules[i].GetPosition()[1], GetColor(i)) );
            }
        }

        Cube GetCube(float w, float h, float x, float y, Color color)
        {
            GameObject obj = Instantiate(Resources.Load(Assets.GetInstance().prefabs.cube), mainTransform) as GameObject;

            float width     = appartment.GetSize()[0];
            float height    = appartment.GetSize()[1];

            Cube cube = obj.GetComponent<Cube>();
            cube.SetSize(w, h);
            cube.SetPosition(-width / 2f + x + w / 2f, -height / 2f + y + h / 2f);

            obj.GetComponent<MeshRenderer>().material.color = color;

            return cube;
        }

        Color GetColor(int index)
        {
            if (index >= 0 && index < colors.Length)
            {
                return colors[index];
            } else
            {
                return new Color(Random.Range(0, 1), Random.Range(0, 1), Random.Range(0, 1), 1);
            }
        }

        //void SetObserverCam()
        //{
        //    Camera cam = GetComponentInChildren<Camera>();

        //    cam.transform.localPosition = new Vector3(-5, 6, -5);
        //    cam.transform.LookAt(mainTransform.position);
        //}

        //void EditCam()
        //{
        //    Camera cam = GetComponentInChildren<Camera>();

        //    cam.transform.localPosition = new Vector3(0, 10, 0);
        //    cam.transform.LookAt(mainTransform.position);
        //}
    }
}