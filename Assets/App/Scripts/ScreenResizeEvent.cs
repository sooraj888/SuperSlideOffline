using UnityEngine;
using System;

public class ScreenResizeEvent : MonoBehaviour
{
    public static event Action OnScreenResize;

    private static int lastWidth;
    private static int lastHeight;

    void Awake()
    {
        lastWidth = Screen.width;
        lastHeight = Screen.height;
    }

    void Update()
    {
        if (Screen.width != lastWidth || Screen.height != lastHeight)
        {
            lastWidth = Screen.width;
            lastHeight = Screen.height;

            OnScreenResize?.Invoke();
        }
    }
}
