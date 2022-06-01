//using System;
//using UnityEngine;

// @aubhere deprecated in favor of Enviro

//public class CoreUSky : MonoBehaviour
//{
//    [NonSerialized] public CoreController MyController;
//    [NonSerialized] public CoreSettings MySettings;
//
//    public uSkyPro MyUSky;
//    public usky.uSkyLighting MyUSkyLighting;
//    public usky.uSkyClouds2D MyUSkyClouds;
//    public DynamicRenderingQuality MyDynamicRenderingQuality;
//
//    void Start()
//    {
////        Debug.Log("CoreSky :: Start");
//
//        MyController = FindObjectOfType<CoreController>();
//        MySettings = FindObjectOfType<CoreSettings>();
//
//        if (!MyController || !MySettings)
//        {
//            Debug.Log("CoreUSky :: Start :: No CoreController found, disabling Sky3hSky manager.");
//            this.enabled = false;
//            return;
//        }
//
//        MyController.OnWorldDataApplied += OnWorldDataApplied; // Reapply sky settings after world is fully initialized
//        MyController.OnQualityChange += OnQualityChange;
//    }
//
//    private void OnDestroy()
//    {
//        if (!MyController)
//        {
//            return;
//        }
//
//        MyController.OnWorldDataApplied -= OnWorldDataApplied;
//        MyController.OnQualityChange -= OnQualityChange;
//    }
//
//    void OnWorldDataApplied()
//    {
//        UpdateFxComponents();
//    }
//
//    void OnQualityChange(int qualityLevel)
//    {
//        UpdateFxComponents();
//    }
//
//    void UpdateFxComponents()
//    {
//        if (!MyController || !MySettings || !WhirldData.Instance)
//        {
//            Debug.Log("CoreUSky :: UpdateFxComponents :: No WhirldData found, skipping update");
//            return;
//        }
//
//        Debug.Log("CoreUSky :: UpdateFxComponents :: Triggered");
//
//        var uSkyDisabled = MySettings.SkyMode != CoreSkyMode.Intermediate;
//        MyDynamicRenderingQuality.SetForceDisabled(MyUSky, uSkyDisabled);
//
//        var cloudsEnabled = WhirldData.Instance.SkyCloudsCumulus;
//        MyDynamicRenderingQuality.SetForceDisabled(MyUSkyClouds, uSkyDisabled || !cloudsEnabled);
//
//        MyUSkyLighting.SunIntensity = WhirldData.Instance.SkySunIntensity;
//        MyUSkyLighting.MoonIntensity = WhirldData.Instance.SkyMoonIntensity;
//    }
//}