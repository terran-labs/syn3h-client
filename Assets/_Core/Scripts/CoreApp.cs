using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoreApp : MonoBehaviour
{
    public CoreSettings MySettings;

    public static CoreApp Instance { get; private set; }
    public List<Resolution> ScreenResolutionsAll { get; private set; }
    public List<Resolution> ScreenResolutionsValid { get; private set; }
    private float _FpsUpdateInterval = 0.5f;
    public int FramesPerSecond { get; protected set; }

    // set everything up at CoreController launch
    // reestablish continuity after a Unity script update
    // can be safely called multiple times
    public void OnEnable()
    {
        Instance = this;

        // update available screen resolutions
        ScreenResolutionsAll = Screen.resolutions.OrderBy(r => r.width).ThenByDescending(r => r.height).ToList();
        ScreenResolutionsValid = new List<Resolution>();
//        Debug.Log(ScreenResolutionsAll);
//        Debug.Log(ScreenResolutionsValid);

        // Create list of only resolutions which match the aspect ratio of the largest resolution supported by this display
        var fullscreenRatio = 0f;
        for (var i = ScreenResolutionsAll.Count - 1; i >= 0; i--)
        {
            var ratio = (float) ScreenResolutionsAll[i].width / ScreenResolutionsAll[i].height;
            if (Math.Abs(fullscreenRatio) < 0.01)
            {
                fullscreenRatio = ratio;
            }

            if (Math.Abs(ratio - fullscreenRatio) > 0.01)
            {
                continue;
            }

            _pushSafeScreenResolution(ScreenResolutionsAll[i]);
        }

        // if this display's max resolution doesn't match the aspect ratio of any smaller resolutions,
        // fallback to allowing any supported resolution to be switched to
        // @todo we have the opportunity to do something smarter here
        if (ScreenResolutionsValid.Count <= 1)
        {
            for (var i = ScreenResolutionsAll.Count - 1; i >= 0; i--)
            {
                _pushSafeScreenResolution(ScreenResolutionsAll[i]);
            }
        }
    }

    private void Start()
    {
        StartCoroutine(FPS());
    }

    public void ToggleFullscreen()
    {
        if (Screen.fullScreen)
        {
            Screen.SetResolution(ScreenResolutionsValid[ScreenResolutionsValid.Count - 2].width,
                ScreenResolutionsValid[ScreenResolutionsValid.Count - 2].height, false);
        }
        else
        {
            Screen.SetResolution(ScreenResolutionsValid[ScreenResolutionsValid.Count - 1].width,
                ScreenResolutionsValid[ScreenResolutionsValid.Count - 1].height, true);
        }
    }

    public void IncreaseResolution()
    {
        Debug.Log("CoreApp :: Attempting to increase screen resolution...");
        ChangeResolution(true);
    }

    public void DecreaseResolution()
    {
        Debug.Log("CoreApp :: Attempting to decrease screen resolution...");
        ChangeResolution(false);
    }

    public void ChangeResolution(bool makeItBigger)
    {
        var targetResIndex = 0;
        if (makeItBigger)
        {
            for (var i = 0; i < ScreenResolutionsValid.Count; i++)
            {
                targetResIndex = i;
                if (ScreenResolutionsValid[i].width > Screen.width /* ||
                    ScreenResolutionsValid[i].height > Screen.height*/)
                {
                    break;
                }
            }
        }

        // find next-smallest resolution
        else
        {
            targetResIndex = ScreenResolutionsValid.Count - 1;
            for (var i = ScreenResolutionsValid.Count - 1; i >= 0; i--)
            {
                targetResIndex = i;
                if (ScreenResolutionsValid[i].width < Screen.width /* ||
                    ScreenResolutionsValid[i].height < Screen.height*/)
                {
                    break;
                }
            }
        }


        Debug.Log("CoreApp :: Setting rendering resolution for primary display from " + Screen.width +
                  "X" + Screen.height + " to " + ScreenResolutionsValid[targetResIndex].width + "X" +
                  ScreenResolutionsValid[targetResIndex].height);

        // Set both Display and Screen resolutionl. This should resolve issues with windowed players experiencing resolutions far higher then the width of their window in pixels
        Display.main.SetRenderingResolution(ScreenResolutionsValid[targetResIndex].width,
            ScreenResolutionsValid[targetResIndex].height);
        Screen.SetResolution(ScreenResolutionsValid[targetResIndex].width,
            ScreenResolutionsValid[targetResIndex].height, Screen.fullScreen);
    }

    private void _pushSafeScreenResolution(Resolution res)
    {
        if (res.width < res.height)
        {
            return;
        }

        foreach (Resolution t in ScreenResolutionsValid)
        {
            if (t.width == res.width && t.height == res.height)
                return;
        }

        ScreenResolutionsValid.Insert(0, res);
    }

    private IEnumerator FPS()
    {
        while (true)
        {
            // Capture frame-per-second
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;

            yield return new WaitForSeconds(_FpsUpdateInterval);

            float timeSpan = Time.realtimeSinceStartup - lastTime;
            int frameCount = Time.frameCount - lastFrameCount;

            // Display it
            FramesPerSecond = Mathf.RoundToInt(frameCount / timeSpan);
        }
    }
}