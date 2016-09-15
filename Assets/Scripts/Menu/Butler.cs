using UnityEngine;
using UnityEngine.SceneManagement;

namespace HomeBuilder.Menu
{
    public class Butler : MonoBehaviour
    {

        public void OnCreate()
        {
            LoadScene(Configuration.Scenes.questioningScene);
        }

        public void OnHistory()
        {
            LoadScene(Configuration.Scenes.historyScene);
        }

        void LoadScene(string name)
        {
            SceneManager.LoadScene(name);
        }

        void Start() { }

        void Update() { }
    }
}