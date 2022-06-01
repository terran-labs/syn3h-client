// @aubhere deprecated, we're going to use Enviro for everything.

//using System;
//using UnityEngine;
//
//public class CoreSky : MonoBehaviour
//{
//    [NonSerialized] public CoreController MyController;
//    [NonSerialized] public CoreSettings MySettings;
//    [NonSerialized] public CoreCamera MyCamera;
//
//    public Material WorldSkyboxHigh;
//    public Material WorldSkyboxMed;
//    public Material WorldSkyboxLow;
//    public Material WorldSkyboxCustom;
//    public Material LobbySkybox;
//
//    private bool _worldSupportsAdvancedSkies;
//
//    public bool WorldSupportsAdvancedSkies
//    {
//        get { return _worldSupportsAdvancedSkies; }
//    }
//
//    private CoreSkyMode _lastSkyMode = CoreSkyMode.nil;
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
//            Debug.Log("CoreSky :: Start :: No CoreController found, disabling Sky3hSky manager.");
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
//    // CoreCamera object may not be enabled when scene is initially loaded, so we will check for it here
//    private CoreCamera _getMyCamera()
//    {
//        if (!MyCamera)
//        {
//            MyCamera = FindObjectOfType<CoreCamera>();
//
//            if (!MyCamera)
//            {
//                Debug.LogError("CoreSky :: _getMyCamera :: No CoreCamera found, ignoring sky update.");
//            }
//        }
//
//        return MyCamera;
//    }
//
//    void OnWorldDataApplied()
//    {
////        Debug.Log("CoreSky :: OnWorldDataApplied");
////        Debug.Log("CoreSky :: OnWorldDataApplied :: Checking for custom Skybox: " + RenderSettings.skybox.name);
//
//        // Sky Init :: Check if scene has a custom skybox specified in Render Settings
//        if (RenderSettings.skybox != LobbySkybox && RenderSettings.skybox != WorldSkyboxHigh && RenderSettings.skybox != WorldSkyboxMed && RenderSettings.skybox != WorldSkyboxLow)
//        {
//            WorldSkyboxCustom = RenderSettings.skybox;
////            Debug.Log("CoreSky :: OnWorldDataApplied :: Detected custom Skybox: " + RenderSettings.skybox.name);
//        }
//        else
//        {
//            WorldSkyboxCustom = null;
//        }
//
//        SelectOptimalSkyLevel();
//    }
//
//    void OnQualityChange(int qualityLevel)
//    {
////        Debug.Log("CoreSky :: OnQualityUpdate :: Triggered (worldIsLoaded: " + worldIsLoaded + ")");
//
//        SelectOptimalSkyLevel();
//    }
//
//    private void SelectOptimalSkyLevel()
//    {
//        // Sky Init :: Determine appropriate default sky mode
//        var skySet = false;
//        _worldSupportsAdvancedSkies = false;
//
//#if TRUESKY
//        var trueSky = FindObjectOfType<simul.trueSKY>();
//        if (trueSky)
//        {
//            _worldSupportsAdvancedSkies = true;
//        }
//#endif
//
//        if (!skySet && WorldSkyboxCustom != null)
//        {
//            MySettings.SkyMode = CoreSkyMode.Custom;
//            skySet = true;
//        }
//
//#if TRUESKY
//        string gv = SystemInfo.graphicsDeviceVersion;
//        if (!skySet && trueSky && gv.Contains("Direct3D 11") && QualitySettings.GetQualityLevel() >= 3)
//        {
//            MySettings.SkyMode = CoreSkyMode.Advanced;
//            skySet = true;
//        }
//#endif
//        if (!skySet && Application.isMobilePlatform)
//        {
//            MySettings.SkyMode = CoreSkyMode.Simple;
//            skySet = true;
//        }
//        if (!skySet)
//        {
//            MySettings.SkyMode = CoreSkyMode.Intermediate;
//        }
//
//        Debug.Log("CoreSky :: SelectOptimalSkyLevel :: Complete (Custom Skybox: " + (WorldSkyboxCustom != null ? WorldSkyboxCustom.name : "null") + ", Sky Mode: " +
//                  MySettings.SkyMode + ")");
//
//        // Trigger UpdateFxComponents() on the next update frame
////        _lastSkyMode = CoreSkyMode.nil;
//    }
//
//    void Update()
//    {
//        // @todo finish resolving the missing skybox assignment
//        
////        // Active Skybox
////        if (!WhirldData.Instance || !WhirldData.Instance.IsPlayableWorld)
////        {
////            RenderSettings.skybox = LobbySkybox;
////        }
////        else if (MySettings.SkyMode == CoreSkyMode.Advanced)
////        {
////            RenderSettings.skybox = WorldSkyboxHigh;
////        }
////        else if (MySettings.SkyMode == CoreSkyMode.Intermediate)
////        {
////            RenderSettings.skybox = WorldSkyboxMed;
////        }
////        else if (MySettings.SkyMode == CoreSkyMode.Simple || !WorldSkyboxCustom)
////        {
////            RenderSettings.skybox = WorldSkyboxLow;
////        }
////        else
////        {
////            RenderSettings.skybox = WorldSkyboxCustom;
////        }
////
////        // Force-update primary post fx stack if we just changed the SkyMode
////        if (_lastSkyMode != MySettings.SkyMode)
////        {
////            _lastSkyMode = MySettings.SkyMode;
////            var cam = _getMyCamera();
////            if (cam)
////            {
////                cam.TriggerUpdateFxComponents();
////            }
////        }
//    }
//
//    // Note: replaced this logic with a call to _getMyCamera().TriggerUpdateFxComponents() above.
////    void UpdateFxComponents()
////    {
////        // CoreCamera object may not be enabled when scene is initially loaded, so we will check for it here
////        if (!MyCamera)
////        {
////            MyCamera = FindObjectOfType<CoreCamera>();
////
////            if (!MyCamera)
////            {
////                Debug.Log("CoreSky :: UpdateFxComponents :: No CoreCamera found, ignoring sky update.");
////                return;
////            }
////        }
////
//////        Debug.Log("CoreSky :: UpdateFxComponents (SkyMode: " + MySettings.SkyMode + ", _lastSkyMode: " + _lastSkyMode + ")");
////
////        _lastSkyMode = MySettings.SkyMode;
////
////        // TrueSky
////#if TRUESKY
////        if (!MyCamera.MyDynamicRenderingQuality)
////        {
////            Debug.Log("CoreSky :: UpdateFxComponents :: No \"MyDynamicRenderingQuality\" assigned, terminating.");
////            return;
////        }
////        if (!MyCamera.MyTrueSkyCamera)
////        {
////            Debug.Log("CoreSky :: UpdateFxComponents :: No \"MyTrueSkyCamera\" assigned, terminating.");
////            return;
////        }
////
////        bool trueSkyForceDisabled = !MyCamera.MyTrueSky || MySettings.SkyMode != CoreSkyMode.Advanced;
////        MyCamera.MyDynamicRenderingQuality.SetForceDisabled(MyCamera.MyTrueSkyCamera, trueSkyForceDisabled);
//////        MyCamera.MyDynamicRenderingQuality.SetForceDisabled(MyCamera.MyTrueSkyCubemapProbe, trueSkyForceDisabled);
////#endif
////    }
//}