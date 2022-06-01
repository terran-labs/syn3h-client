// using System;
// using MapMagic;
// using StatsMonitor;
// using UnityEngine;

// public enum CoreUiMode
// {
//     Auto, // (used by ModeOverride system) Transition automatically to contextually appropriate rendering mode
//     Locked, // Cursor input locked, minimal/no GUI Shown
//     HUD, // Core panel on left, Console panel on right
//     Fullscreen, // fullscreen settings/world switcher/etc
//     Diagnostics, // No GUI, no cursor locking
//     Working, // fullscreen status update for world loads/terran generation/etc
// }

// public class CoreUi : MonoBehaviour
// {
//     public static CoreUi Instance { get; private set; }
//     public CoreUiMode ModeCurrent;
//     public CoreUiMode ModeOverride;
//     public Texture2D CursorTexture;
//     public Vector2 CursorHotspot;

//     public GUISkin GuiSkin;

// //    public CoreSky MySky;
//     public CoreScreenshot ScreenShot;
//     public float TimeDragLockDown = .25f;
//     public GameObject InterfaceObject;
//     private Vector2 _scrollPosition;
//     private float _timeCursorLockChanged;
//     private float _timeGuiVisibilityChanged;
//     private float _guiAlpha;
//     private bool _deltaCursorWasLocked;

//     // set everything up at CoreController launch
//     // reestablish continuity after a Unity script update
//     // can be safely called multiple times
//     void OnEnable()
//     {
//         // Register Singleton reference
//         Instance = this;

// //		CoreController.OnWorldLoaded += OnWorldLoaded;

//         // Set custom cursor
//         Cursor.SetCursor(CursorTexture, CursorHotspot, CursorMode.Auto);
//     }

//     void Start()
//     {
//         // Register delegate hooks
//         CoreController.Instance.OnWorldInitialized += OnWorldInitialized;
//     }

//     void OnDestroy()
//     {
//         CoreController.Instance.OnWorldInitialized -= OnWorldInitialized;
//     }

//     void OnWorldInitialized()
//     {
//         Debug.Log("CoreUi :: OnWorldInitialized");
//         ModeCurrent = CoreUiMode.HUD;
//     }

//     public CoreUiMode GetUiMode()
//     {
//         if (ModeOverride != CoreUiMode.Auto)
//         {
//             return ModeOverride;
//         }

//         return ModeCurrent;
//     }

//     void Update()
//     {
//         // Mode determination
//         if (CoreWhirld.Instance.Status != CoreWhirldStatus.Good && CoreWhirld.Instance.Status != CoreWhirldStatus.LoadingHardReset)
//         {
//             ModeOverride = CoreUiMode.Working;
//         }
//         else if (CoreController.Instance.StatsLogger.show)
//         {
//             ModeOverride = CoreUiMode.Diagnostics;
//         }
//         else if (WhirldData.Instance && !WhirldData.Instance.IsPlayableWorld)
//         {
//             ModeOverride = CoreUiMode.Fullscreen;
//         }
//         else
//         {
//             ModeOverride = CoreUiMode.Auto;
//         }

//         // Mode processing
//         var modeActive = GetUiMode();

// //        Debug.Log("CoreUi :: Update :: modeActive: " + modeActive);

//         // World is loading
//         if (modeActive == CoreUiMode.Working)
//         {
//             Cursor.lockState = CursorLockMode.None;
//             Cursor.visible = false;
//         }

//         // Showing a diagnostic window
//         else if (modeActive == CoreUiMode.Diagnostics)
//         {
//             Cursor.lockState = CursorLockMode.None;
//             Cursor.visible = true;
//         }

//         // Chillin' in the Lobby
//         else if (modeActive == CoreUiMode.Fullscreen)
//         {
//             Cursor.lockState = CursorLockMode.None;
//             Cursor.visible = true;
//         }

//         // Exploring a World
//         else
//         {
//             _handleCursorLocking();
//         }

//         // GUI fade in/out as appropriate
//         _calculateGuiAlpha();

//         // Fullscreen Interface Visibility
//         if ((modeActive == CoreUiMode.Fullscreen || modeActive == CoreUiMode.Working) && !InterfaceObject.activeSelf)
//         {
//             InterfaceObject.SetActive(true);
//         }
//         else if (!(modeActive == CoreUiMode.Fullscreen || modeActive == CoreUiMode.Working) && InterfaceObject.activeSelf)
//         {
//             InterfaceObject.SetActive(false);
//         }
//     }

//     void OnGUI()
//     {
//         // Don't show the controller GUI if we are in the Lobby,
//         // or if we are in an Ansel cinematography session
//         if (_guiAlpha.Equals(0) /*|| ModeCurrent != CoreUiMode.HUD*/)
//         {
//             return;
//         }

//         GUI.skin = GuiSkin;
//         GUI.depth = 1;

// //		var helpRect = new Rect(25, Screen.height - 50, Screen.width - 50, 40);
// //		if (Cursor.lockState == CursorLockMode.Locked)
// //		{
// //			GUI.Label(helpRect, "Esc to unlock. Ctrl to ride. Shift to scope. Mousewheel to zoom.");
// //		}
// //		else
// //		{
// //			GUI.Label(helpRect, "(click to focus)");
// //		}

// //        var windowRect = new Rect(Screen.width - 275, 25, 250, Screen.height - 50);
//         var windowRect = new Rect(25, 25, 250, Screen.height - 50);
//         GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, _guiAlpha);
//         GUI.Window(0, windowRect, WindowController, "Core");
//     }

//     void WindowController(int windowID)
//     {
//         GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, _guiAlpha);

//         if (CoreWhirld.Instance.Status != CoreWhirldStatus.Good)
//         {
// //            GUILayout.FlexibleSpace();
// //            GUILayout.Label("Building planet...\n" + CoreWhirld.Instance.Progress * 100 + "%");
// //            GUILayout.FlexibleSpace();

//             return;
//         }

//         _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

//         // Whirld loading
// //        GUILayout.Space(10);
// //        GUILayout.FlexibleSpace();
// //        GUILayout.Label("Whirld:");
// //        GUILayout.BeginHorizontal();
// //        WhirldIn.url = GUILayout.TextField(WhirldIn.url);
// //        if (GUILayout.Button("GO"))
// //        {
// //            WhirldIn.Load();
// //        }
// //        GUILayout.EndHorizontal();
// //        GUILayout.Label("Info: " + WhirldIn.info);
// //        GUILayout.Label("Summary: " + WhirldIn.status);
// //        GUILayout.Label("Status: " + WhirldIn.statusTxt);
// //        GUILayout.Label("Progress: " + WhirldIn.progress);

// //        if (CoreController.SceneUpdating != null && CoreController.SceneUpdating.isDone != true)
// //        {
// //            GUILayout.FlexibleSpace();
// //
// //            GUILayout.Label("Building planet...\n" + CoreController.SceneUpdating.progress * 100 + "%");
// //
// //            GUILayout.FlexibleSpace();
// //            GUILayout.EndScrollView();
// //            return;
// //        }


//         // GUI minimization
// //        GUILayout.Space(10);
// //        if (GUILayout.Button("Hide GUI (0)"))
// //        {
// //            SetCursorLock(true);
// //        }


//         if (GUILayout.Button("Open Settings"))
//         {
//             ModeCurrent = CoreUiMode.Fullscreen;
//         }

//         // game worlds
//         if (WhirldData.Instance)
//         {
//             GUILayout.Space(20);
//             GUILayout.Label("World: " + WhirldData.Instance.Name);

//             if (GUILayout.Button("« Exit World"))
//             {
//                 CoreController.Instance.UnloadWorld();
//             }

//             if (WhirldData.Instance.IsPlayableWorld)
//             {
//                 if (GUILayout.Button("Regenerate World"))
//                 {
//                     CoreWhirld.Instance.RandomizeEverything(true);
//                 }

// //                if (GUILayout.Button("Regenerate Foilage"))
// //                {
// //                    uNature.Core.Utility.FoliageWorldMaps.ReGenerateGlobally();
// //                }
//             }
//         }

//         // Vehicle switching
//         if (true || CoreController.Instance.Player)
//         {
//             GUILayout.Space(20);
// //            GUILayout.FlexibleSpace();
//             GUILayout.Label("Vehicles:");
//             for (var i = 0; i < CoreController.Instance.Vehicles.Length; i++)
//             {
//                 if (GUILayout.Button((CoreController.Instance.Player && CoreController.Instance.Player.name == CoreController.Instance.Vehicles[i].name ? "» " : "") +
//                                      CoreController.Instance.Vehicles[i].name))
//                 {
//                     CoreController.Instance.LoadVehicle(i);
//                 }
//             }
//         }

//         // Time Control
//         if (CoreWhirld.Instance.TimeControlEnabled)
//         {
//             GUILayout.Space(20);
// //            GUILayout.FlexibleSpace();
//             GUILayout.Label("Time:");

//             // Current time
//             GUILayout.BeginHorizontal();
//             var time = CoreWhirld.Instance.GetTime();
//             var timeDelta = time;
//             time = GUILayout.HorizontalSlider(time, 0F, 1F);
//             if (!time.Equals(timeDelta))
//             {
//                 CoreWhirld.Instance.SetTime(time);
//             }

//             var time24 = time * 24;
//             var hour = Mathf.FloorToInt(time24);
//             var minute = Mathf.Round((time24 - hour) * .6f * 100);
//             GUILayout.Label("(" + hour.ToString("00") + ":" + minute.ToString("00") + ")", GUILayout.Width(75));
//             GUILayout.EndHorizontal();

//             // Time speed
//             GUILayout.BeginHorizontal();
//             CoreWhirld.Instance.TimeSpeed = (int) GUILayout.HorizontalSlider(CoreWhirld.Instance.TimeSpeed, 0, CoreSettings.Instance.TimeSpeedMax);
//             if (CoreWhirld.Instance.TimeSpeed > -CoreSettings.Instance.TimeSpeedDeadzone && CoreWhirld.Instance.TimeSpeed < CoreSettings.Instance.TimeSpeedDeadzone)
//             {
//                 CoreWhirld.Instance.TimeSpeed = 1;
//             }

//             GUILayout.Label("(" + CoreWhirld.Instance.TimeSpeed + ")", GUILayout.Width(75));
//             GUILayout.EndHorizontal();

//             // Time freeze
//             CoreWhirld.Instance.TimeFreeze = GUILayout.Toggle(CoreWhirld.Instance.TimeFreeze, "Freeze Time");
//         }

//         // Environment Contol
// //        if (GUILayout.Button("Enviro Init"))
// //        {
// //            EnviroSky.instance.AssignAndStart(CoreCamera.Instance.gameObject, CoreCamera.Instance.ThisCamera);
// //        }

//         if (EnviroSky.instance != null && EnviroSky.instance.Weather.WeatherPrefabs != null && EnviroSky.instance.Weather.WeatherPrefabs.Count > 0)
//         {
//             GUILayout.Space(20);
// //            GUILayout.FlexibleSpace();
//             GUILayout.Label("Weather:");

//             for (var i = 0; i < EnviroSky.instance.Weather.WeatherPrefabs.Count; i++)
//             {
//                 var isActive = EnviroSky.instance.Weather.currentActiveWeatherPreset != null &&
//                                EnviroSky.instance.Weather.WeatherPrefabs[i].weatherPreset == EnviroSky.instance.Weather.currentActiveWeatherPreset;
//                 var toggleActive = GUILayout.Toggle(isActive, EnviroSky.instance.Weather.WeatherPrefabs[i].name);
//                 if (toggleActive != isActive)
//                 {
//                     Debug.Log("CoreUI :: Initiating Change of Weather :: " + i);
//                     EnviroSky.instance.ChangeWeather(i);
//                 }
//             }
//         }

//         // Ocean Control
//         if (WhirldData.Instance /*|| CoreHydroform.Instance */)
//         {
//             GUILayout.Space(20);
// //            GUILayout.FlexibleSpace();
//             GUILayout.Label("Ocean:");

//             WhirldData.Instance.SeaEnabled = GUILayout.Toggle(WhirldData.Instance.SeaEnabled, "Ocean Enabled");

//             if (WhirldData.Instance.SeaEnabled)
//             {
//                 GUILayout.BeginHorizontal();
//                 WhirldData.Instance.SeaAltitude =
//                     (int) GUILayout.HorizontalSlider(WhirldData.Instance.SeaAltitude, CoreSettings.Instance.SeaAltitudeMin, CoreSettings.Instance.SeaAltitudeMax);
//                 GUILayout.Label("(" + WhirldData.Instance.SeaAltitude + " ALT)", GUILayout.Width(75));
//                 GUILayout.EndHorizontal();

//                 GUILayout.BeginHorizontal();
//                 if (WhirldData.Instance.SeaAltitude > CoreSettings.Instance.SeaAltitudeMin + CoreSettings.Instance.SeaAltitudeStepBig && GUILayout.Button("<<"))
//                 {
//                     WhirldData.Instance.SeaAltitude -= CoreSettings.Instance.SeaAltitudeStepBig;
//                 }

//                 if (WhirldData.Instance.SeaAltitude > CoreSettings.Instance.SeaAltitudeMin + CoreSettings.Instance.SeaAltitudeStepSmall && GUILayout.Button("<"))
//                 {
//                     WhirldData.Instance.SeaAltitude -= CoreSettings.Instance.SeaAltitudeStepSmall;
//                 }

//                 if (Math.Abs(WhirldData.Instance.SeaAltitude) > CoreSettings.Instance.SeaAltitudeStepSmall && GUILayout.Button("0"))
//                 {
//                     WhirldData.Instance.SeaAltitude = 0f;
//                 }

//                 if (WhirldData.Instance.SeaAltitude < CoreSettings.Instance.SeaAltitudeMax - CoreSettings.Instance.SeaAltitudeStepSmall && GUILayout.Button(">"))
//                 {
//                     WhirldData.Instance.SeaAltitude += CoreSettings.Instance.SeaAltitudeStepSmall;
//                 }

//                 if (WhirldData.Instance.SeaAltitude < CoreSettings.Instance.SeaAltitudeMax - CoreSettings.Instance.SeaAltitudeStepBig && GUILayout.Button(">>"))
//                 {
//                     WhirldData.Instance.SeaAltitude += CoreSettings.Instance.SeaAltitudeStepBig;
//                 }

//                 GUILayout.EndHorizontal();
//             }
//         }

//         // Screenshot System
//         if (true)
//         {
//             GUILayout.Space(20);
// //            GUILayout.FlexibleSpace();
//             GUILayout.Label("Photography:");

//             CoreSettings.Instance.FreezeMotion = GUILayout.Toggle(CoreSettings.Instance.FreezeMotion, "Freeze Motion (9)");

//             if (GUILayout.Button("Capture Screenshot (8)"))
//             {
//                 ScreenShot.Capture();
//             }

//             if (GUILayout.Button("Show Screenshots"))
//             {
//                 ScreenShot.OpenOutputDirectory();
//             }

//             /*if (_anselIsAvailable)
//             {
//                 GUILayout.Label("(ALT + F2 to activate Ansel)");
//             }
//             else
//             {
//                 GUILayout.Label("(Ansel not available)");
//             }*/
//         }

//         // Bot Management
//         if (true)
//         {
//             GUILayout.Space(20);
// //            GUILayout.FlexibleSpace();
//             GUILayout.Label("Bots:");

//             GUILayout.Label("(" + CoreController.Instance.PlayerBots.Count + " active bots)");

//             GUILayout.BeginHorizontal();
//             if (CoreController.Instance.PlayerBots.Count > 0)
//             {
//                 if (GUILayout.Button("« Remove Bot"))
//                 {
//                     CoreController.Instance.RemoveBot();
//                 }
//             }

//             GUILayout.Space(10);
//             if (GUILayout.Button("Add Bot »"))
//             {
//                 CoreController.Instance.AddBot();
//             }

//             GUILayout.EndHorizontal();
//             if (CoreController.Instance.PlayerBots.Count > 5 && GUILayout.Button("Remove All Bots"))
//             {
//                 CoreController.Instance.RemoveAllBots();
//             }
//         }

//         // Game Stats
//         GUILayout.Space(20);
//         GUILayout.FlexibleSpace();

// //        var velMps = CoreSettings.PlayerVelocity.ToString("F1");
//         var velKph = (CoreSettings.Instance.PlayerVelocity * 3.6).ToString("F1");
//         var velMph = (CoreSettings.Instance.PlayerVelocity * 2.23694).ToString("F1");
//         var distKm = (CoreSettings.Instance.PlayerOriginDistance * 0.001).ToString("F2");
//         var distMiles = (CoreSettings.Instance.PlayerOriginDistance * 0.000621371).ToString("F2");
//         var altMeters = (CoreSettings.Instance.PlayerAltitude).ToString("n0");
//         var altFeet = (CoreSettings.Instance.PlayerAltitude * 3.28084).ToString("n0");
//         GUILayout.Label("Speed: " + velKph + " kph (" + velMph + " mph)");
//         GUILayout.Label("Distance:  " + distKm + " km (" + distMiles + " miles)");
//         GUILayout.Label("Altitude:  " + altMeters + " m (" + altFeet + " feet)");

//         if (MapMagic.MapMagic.instance != null)
//         {
//             // MM V1
//             if (ThreadWorker.IsWorking("MapMagic") || ThreadWorker.IsWorking("Voxeland"))

//                 // MM V2
// //            if (MapMagic.MapMagic.instance.IsGenerating())
//             {
//                 GUILayout.Label("Environment: Generating...");
//             }
//             else
//             {
//                 GUILayout.Label("Environment: Complete");
//             }
//         }

//         if (OnlineMaps.instance)
//         {
//             GUILayout.Space(20);
//             GUILayout.Label("Earth:");

// //            GUILayout.Label("Status: " + OnlineMaps.instance.bufferStatus + "");

//             if (GUILayout.Button("Show Cache Data"))
//             {
//                 CoreGeoEngine.OpenCacheDirectory();
//             }
//         }


//         // Quality level control
//         GUILayout.Space(20);
//         GUILayout.FlexibleSpace();
//         var qualityLevel = QualitySettings.GetQualityLevel();
//         string[] qualityNames = QualitySettings.names;
//         GUILayout.Label("Visuals:");
//         GUILayout.BeginHorizontal();
//         int iQ = 0;
//         while (iQ < qualityNames.Length)
//         {
//             if (iQ == 0 && iQ == qualityLevel)
//             {
//                 GUILayout.Space(30);
//             }

//             if (iQ == qualityLevel - 1 && GUILayout.Button("«", GUILayout.Width(30)))
//             {
//                 QualitySettings.SetQualityLevel(iQ, true);
//                 CoreSettings.Instance.updatePrefs();
//             }
//             else if (iQ == qualityLevel)
//             {
//                 GUILayout.Label(qualityNames[qualityLevel]);
//             }
//             else if (iQ == qualityLevel + 1 && GUILayout.Button("»", GUILayout.Width(30)))
//             {
//                 QualitySettings.SetQualityLevel(iQ, true);
//                 CoreSettings.Instance.updatePrefs();
//             }

//             if (iQ == qualityNames.Length - 1 && iQ == qualityLevel)
//             {
//                 GUILayout.Space(30);
//             }

//             iQ++;
//         }

//         GUILayout.EndHorizontal();
//         GUILayout.Label("(" + CoreApp.Instance.FramesPerSecond + " FPS)");

//         // Diagnostics
//         GUILayout.Space(20);
//         GUILayout.Label("Diagnostics:");
//         CoreController.Instance.StatsMonitor.Status = GUILayout.Toggle(CoreController.Instance.StatsMonitor.Status == StatsMonitorStatus.Active, "Show Performance Stats")
//             ? StatsMonitorStatus.Active
//             : StatsMonitorStatus.Inactive;

//         if (!CoreController.Instance.StatsLogger.show)
//         {
//             if (GUILayout.Button("Open Log Inspector"))
//             {
//                 CoreController.Instance.StatsLogger.Show();
//             }
//         }

//         GUILayout.Space(20);

//         // @todo sky mode switcher currently disabled 
// //        if (MySky.WorldSkyboxCustom != null)
// //        {
// //            CoreSettings.Instance.SkyMode = GUILayout.Toggle(CoreSettings.Instance.SkyMode == CoreSkyMode.Custom, "World Sky") ? CoreSkyMode.Custom : CoreSettings.Instance.SkyMode;
// //        }
// //        CoreSettings.Instance.SkyMode = GUILayout.Toggle(CoreSettings.Instance.SkyMode == CoreSkyMode.Simple, "Simple Sky") ? CoreSkyMode.Simple : CoreSettings.Instance.SkyMode;
// //        if (MySky.WorldSupportsAdvancedSkies)
// //        {
// //            CoreSettings.Instance.SkyMode = GUILayout.Toggle(CoreSettings.Instance.SkyMode == CoreSkyMode.Intermediate, "Detailed Sky") ? CoreSkyMode.Intermediate : CoreSettings.Instance.SkyMode;
// //        }
// //#if TRUESKY
// //        if (MySky.WorldSupportsAdvancedSkies)
// //        {
// //            CoreSettings.Instance.SkyMode = GUILayout.Toggle(CoreSettings.Instance.SkyMode == CoreSkyMode.Advanced, "Intricate Sky") ? CoreSkyMode.Advanced : CoreSettings.Instance.SkyMode;
// //        }
// //#endif
// //        GUILayout.Space(10);

//         if (!WhirldData.Instance || WhirldData.Instance.EnableAtmospherics)
//         {
//             CoreSettings.Instance.DisableAtmospherics = GUILayout.Toggle(CoreSettings.Instance.DisableAtmospherics, "Disable Atmosphere");
//         }

// //        CoreSettings.DisableSkyFX = GUILayout.Toggle(CoreSettings.DisableSkyFX, "Disable Sky");
//         CoreSettings.Instance.DisableWater = GUILayout.Toggle(CoreSettings.Instance.DisableWater, "Disable Water");
//         CoreSettings.Instance.DisableAdvancedPostFX = GUILayout.Toggle(CoreSettings.Instance.DisableAdvancedPostFX, "Disable Post FX");
//         if (!CoreSettings.Instance.DisableAdvancedPostFX)
//         {
//             CoreSettings.Instance.DisableVisualDOF = GUILayout.Toggle(CoreSettings.Instance.DisableVisualDOF, "Disable DOF");
//             CoreSettings.Instance.DisableAA = GUILayout.Toggle(CoreSettings.Instance.DisableAA, "Disable AA");
//             CoreSettings.Instance.DisableAO = GUILayout.Toggle(CoreSettings.Instance.DisableAO, "Disable AO");
//             CoreSettings.Instance.DisableSSS = GUILayout.Toggle(CoreSettings.Instance.DisableSSS, "Disable SSS");
//         }

//         // fullscreen toggle
//         GUILayout.Space(20);
// //        GUILayout.FlexibleSpace();
//         GUILayout.Label("Resolution:");
//         if (GUILayout.Button((!Screen.fullScreen ? "Enter" : "Exit") + " Fullscreen"))
//         {
//             CoreApp.Instance.ToggleFullscreen();
//         }

//         // screen resolution adjustment
//         if (CoreApp.Instance.ScreenResolutionsValid.Count > 0)
// //            ||Screen.fullScreen /* || Application.isEditor */ /*Application.platform == RuntimePlatform.OSXEditor*/
//         {
//             GUILayout.BeginHorizontal();
//             if (Screen.width > CoreApp.Instance.ScreenResolutionsValid[0].width ||
//                 Screen.height > CoreApp.Instance.ScreenResolutionsValid[0].height)
//             {
//                 if (GUILayout.Button("«", GUILayout.Width(30)))
//                 {
//                     CoreApp.Instance.DecreaseResolution();
//                 }
//             }
//             else
//             {
//                 GUILayout.Space(30);
//             }

//             GUILayout.Label(Screen.width + "X" + Screen.height);
//             if (Screen.width < CoreApp.Instance.ScreenResolutionsValid[CoreApp.Instance.ScreenResolutionsValid.Count - 1].width ||
//                 Screen.height < CoreApp.Instance.ScreenResolutionsValid[CoreApp.Instance.ScreenResolutionsValid.Count - 1].height)
//             {
//                 if (GUILayout.Button("»", GUILayout.Width(30)))
//                 {
//                     CoreApp.Instance.IncreaseResolution();
//                 }
//             }
//             else
//             {
//                 GUILayout.Space(30);
//             }

//             GUILayout.EndHorizontal();
//         }
//         else
//         {
//             GUILayout.Label("Resolution:");
//             GUILayout.Label("(anomaly detected. Please check your display configuration.)");
//         }

// //        for (var i = ScreenResolutionsValid.Count - 1; i >= 0; i--)
// //        {
// //            GUILayout.Label(ScreenResolutionsValid[i].ToString());
// //        }

//         GUILayout.EndScrollView();
//     }

//     void _handleCursorLocking()
//     {
//         // Cursor was externally unlocked
//         if (Cursor.lockState == CursorLockMode.None && _deltaCursorWasLocked)
//         {
//             _timeCursorLockChanged = Time.time;
//             _setCursorLock(false);
//             Debug.Log("Cursor Locking :: External Unlock Detected");
//         }

//         // Cursor is locked
//         else if (Cursor.lockState != CursorLockMode.None)
//         {
//             // Decide if we should unlock the cursor this frame
//             var unlockCursor = false;

//             // Unlock cursor if ESC is pressed
//             if (!unlockCursor)
//             {
//                 unlockCursor = Input.GetButton("Cancel");
//             }

//             // Unlock if left mouse is pressed and we aren't in ride mode
//             if (!unlockCursor)
//             {
//                 unlockCursor = CoreCamera.Instance.TargetMode != CoreCameraMode.First && Input.GetButtonDown("Fire1");
//             }

//             // Unlock if left mouse is released, we aren't in ride mode, and left mouse had been held longer than TimeDragLockDown seconds
//             if (!unlockCursor)
//             {
//                 unlockCursor = CoreCamera.Instance.TargetMode != CoreCameraMode.First && Input.GetButtonUp("Fire1") && Time.time - _timeCursorLockChanged > TimeDragLockDown;
//             }

//             if (unlockCursor)
//             {
//                 Cursor.lockState = CursorLockMode.None;
//                 _timeCursorLockChanged = Time.time;
//                 _setCursorLock(false);
//                 Debug.Log("Cursor Locking :: Unlocked");
//             }
//         }

//         // Cursor is free, think about locking it
//         else
//         {
//             // Lock cursor if we click in the non-gui area of view or activate any of special inputs
//             if ( /*Input.GetButtonDown("Interact") || Input.GetButtonDown("Perspective") || */
//                 ((Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2")) && Input.mousePosition.x > 300 && Input.mousePosition.x < Screen.width - 300))
//             {
//                 Cursor.lockState = CursorLockMode.Locked;
//                 _timeCursorLockChanged = Time.time;
//                 _setCursorLock(true);
//                 Debug.Log("Cursor Locking :: Locked");
//             }
//         }

//         // Cursor visibility
//         if (Cursor.lockState == CursorLockMode.None)
//         {
//             Cursor.visible = true;
//         }
//         else
//         {
//             Cursor.visible = CoreCamera.Instance.TargetMode == CoreCameraMode.First || CoreCamera.Instance.TargetMode == CoreCameraMode.Scope;
//         }

//         // External unlock tracking
//         _deltaCursorWasLocked = Cursor.lockState != CursorLockMode.None;
//     }

//     private void _setCursorLock(bool locked)
//     {
//         Debug.Log("CoreGUI :: SetCursorLock: " + locked);

//         // Early-out if visibility state hasn't changed
//         if (locked == (GetUiMode() == CoreUiMode.Locked))
//         {
//             Debug.Log("CoreGUI :: SetCursorLock :: No action needed");
//             return;
//         }

//         _timeGuiVisibilityChanged = Time.time;
//         if (locked)
//         {
//             ModeCurrent = CoreUiMode.Locked;

//             // Automatically lock cursor if GUI is being hidden
//             Cursor.lockState = CursorLockMode.Locked;
//             _timeCursorLockChanged = Time.time;

//             Debug.Log("CoreGUI :: SetCursorLock :: transitioning to invisible state, locking cursor automatically");
//         }
//         else
//         {
//             ModeCurrent = CoreUiMode.HUD;
//         }
//     }

//     private void _calculateGuiAlpha()
//     {
//         const float transitionFadeDuration = .75f;
//         var maskStatus = CoreCameraFade.Instance.GetMaskStatus();
//         var uiMode = GetUiMode();

//         // Fade manually :: force-hidden
//         if (uiMode == CoreUiMode.Working || uiMode == CoreUiMode.Fullscreen || uiMode == CoreUiMode.Diagnostics || Time.timeSinceLevelLoad < transitionFadeDuration * 2)
//         {
//             _guiAlpha = 0;
//         }

//         // Fade with camera mask
//         else if (maskStatus > 0)
//         {
//             _guiAlpha = 1 - maskStatus;
//         }

//         // Fade manually :: transition
//         else if (Time.time - _timeGuiVisibilityChanged < transitionFadeDuration)
//         {
//             // Fade in
//             if (uiMode == CoreUiMode.HUD)
//             {
//                 _guiAlpha = Mathf.Lerp(0, 1, (Time.time - _timeGuiVisibilityChanged) / transitionFadeDuration);
//             }

//             // Fade out
//             else
//             {
//                 _guiAlpha = Mathf.Lerp(1, 0, (Time.time - _timeGuiVisibilityChanged) / transitionFadeDuration);
//             }
//         }

//         // Fade manually :: static
//         else if (uiMode == CoreUiMode.HUD)
//         {
//             _guiAlpha = 1f;
//         }
//         else
//         {
//             _guiAlpha = 0f;
//         }
//     }
// }