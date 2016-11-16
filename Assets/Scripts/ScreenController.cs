using UnityEngine;
using System.Collections;
using HomeBuilder;
using UnityEngine.SceneManagement;

public class ScreenController : MonoBehaviour
{

    public RectTransform[] screens;
    public GameObject[]    toActivate;
    
    private bool _loading   = false;

    void SetActiveObjects(bool value)
    {
        foreach (GameObject obj in toActivate)
        {
            obj.SetActive(value);
        }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (GlobalScreenManager.Instance != null) GlobalScreenManager.Instance.AddScreen(this);
    }

    public void Open()
    {
        SetActiveObjects(true);
    }

    public void Close()
    {
    }

    public void OpenStyle()
    {
        LoadScene(Configuration.Scenes.styleScene);
    }

    public void OpenHistory()
    {
        LoadScene(Configuration.Scenes.historyScene);
    }

    public void OpenDesigning()
    {
        LoadScene(Configuration.Scenes.designingScene);
    }

    public void OpenModules()
    {
        LoadScene(Configuration.Scenes.modulesScene);
    }

    public void LoadScene(string level)
    {
        if (_loading) return;

        _loading = true;
        SetActiveObjects(false);
        SceneManager.LoadScene(level);
    }
}
