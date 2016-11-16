using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using DG.Tweening;
using HomeBuilder.Core;

public class GlobalScreenManager : MonoBehaviour
{
    public static GlobalScreenManager Instance { private set; get; }

    private List<ScreenController> screens;
    private float time  = 0.25f;

    void Awake()
    {
        Instance = this;
        screens = new List<ScreenController>();
    }

    public void AddScreen(ScreenController screen)
    {
        ScreenController prev = null;
        ScreenController next = screen;

        screens.Add(screen);

        if (screens.Count > 1)
        {
            prev = screens[0];
            screens.Remove(prev);
        }

        switch (Master.GetEffect(false))
        {
            case "Fade":
                StartCoroutine(OpenCloseFade(prev, next));
                break;
            case "Left":
                StartCoroutine(OpenCloseLeft(prev, next));
                break;
            case "Right":
                StartCoroutine(OpenCloseRight(prev, next));
                break;
        }
    }

    IEnumerator OpenCloseRight(ScreenController prev, ScreenController next)
    {
        float w1 = next.screens[0].rect.width;
        float w2 = w1;
        if (prev != null) w2 = prev.screens[0].rect.width;

        SetPosition(next.screens, new Vector2(-w1, 0));

        if (prev != null) Move(prev.screens, new Vector2(w2, 0), time);
        Move(next.screens, new Vector2(0, 0), time);

        yield return new WaitForSeconds(time);

        if (prev != null) Destroy(prev.gameObject);
    }

    IEnumerator OpenCloseLeft(ScreenController prev, ScreenController next)
    {
        float w1 = next.screens[0].rect.width;
        float w2 = w1;
        if (prev != null) w2 = prev.screens[0].rect.width;

        SetPosition(next.screens, new Vector2(w1, 0));

        if (prev != null) Move(prev.screens, new Vector2(-w2, 0), time);
        Move(next.screens, new Vector2(0, 0), time);

        yield return new WaitForSeconds(time);

        if (prev != null) Destroy(prev.gameObject);
    }

    IEnumerator OpenCloseFade(ScreenController prev, ScreenController next)
    {
        SetFade(next.screens, 0);

        if (prev != null) Fade(prev.screens, 0, time);
        Fade(next.screens, 1, time);

        yield return new WaitForSeconds(time);

        if (prev != null) Destroy(prev.gameObject);
    }

    void Move(RectTransform[] transforms, Vector2 position, float time)
    {
        foreach (RectTransform rect in transforms)
        {
            rect.DOAnchorPos(position, time);
        }
    }

    void SetPosition(RectTransform[] transforms, Vector2 position)
    {
        foreach (RectTransform rect in transforms)
        {
            rect.anchoredPosition = position;
        }
    }

    void SetFade(RectTransform[] transforms, float alpha)
    {
        foreach (RectTransform rect in transforms)
        {
            CanvasGroup cg = rect.gameObject.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = alpha;
            }
        }
    }

    void Fade(RectTransform[] transforms, float alpha, float time)
    {
        foreach (RectTransform rect in transforms)
        {
            CanvasGroup cg = rect.gameObject.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.DOFade(alpha, time);
            }
        }
    }

}
