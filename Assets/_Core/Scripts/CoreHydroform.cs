// using System;
// using UnityEngine;
// using WAPI;
// using Hydroform;

// [ExecuteInEditMode]
// public class CoreHydroform : MonoBehaviour
// {
//     public static CoreHydroform Instance { get; private set; }

//     public HydroformComponent Hydroform { get; private set; }

//     public CoreEnviro CoreEnviro { get; private set; }

//     public float MinWaveAmplitude = 0.25f;
//     public float MaxWaveAmplitude = 7f;
//     public float WaveAmplitudeWindMultiplier = 5f;

//     public float UpdateTimeSmoothing = .25f;

//     private float _targetWaterHeight;
//     private float _targetWaveAmplitude;
//     private Color _targetColor;
//     private Color _targetFoamColor;

//     void OnEnable()
//     {
//         Instance = this;
//         CoreEnviro = FindObjectOfType<CoreEnviro>();
//     }

//     void Start()
//     {
//         if (CoreController.Instance)
//         {
//             CoreController.Instance.OnQualityUpdate += OnQualityUpdate;
//         }
//     }

//     void OnDestroy()
//     {
//         Instance = null;

//         if (CoreController.Instance)
//         {
//             CoreController.Instance.OnQualityUpdate -= OnQualityUpdate;
//         }
//     }

//     void Update()
//     {
//         if (!Hydroform)
//         {
//             Hydroform = GetComponent<HydroformComponent>();
//             Hydroform.setUpdateInterval(15);
//         }

//         Hydroform.enabled = true;
//         if (CoreSettings.Instance && CoreSettings.Instance.DisableWater)
//         {
//             Hydroform.enabled = false;
//         }
//         else if (WhirldData.Instance && !WhirldData.Instance.SeaEnabled)
//         {
//             Hydroform.enabled = false;
//         }

// //        if (Application.isPlaying)
// //        {
// //            Hydroform.mMultiCam = true;
// //        }
// //        else
// //        {
// //            Hydroform.mMultiCam = false;
// //        }

//         if (CoreEnviro && CoreEnviro.EnvironmentalReflectionCubemap != null)
//         {
//             Hydroform.reflectFX.skybox = CoreEnviro.EnvironmentalReflectionCubemap;
//         }

//         if (!WhirldData.Instance)
//         {
//             return;
//         }

// //        if (RenderSettings.ambientMode == AmbientMode.Flat)
// //        {
// //        if (CoreWhirld.Instance.TimeIsDaylight)
// //        {
//         _targetColor = Color.Lerp(Color.black, RenderSettings.ambientGroundColor, .33f);
//         _targetFoamColor = Color.Lerp(Color.black, RenderSettings.ambientSkyColor, .75f);
// //        }
// //        else
// //        {
// //            _targetColor = Color.black;
// //        }
// //        }
// //        else if (RenderSettings.ambientMode == AmbientMode.Flat)
// //        {
// //            _hydroform.surfaceFX.waterColor = RenderSettings.ambientEquatorColor;
// //        }

//         _targetWaterHeight = WhirldData.Instance.SeaAltitude;
//         _targetWaveAmplitude = Mathf.Lerp(MinWaveAmplitude, MaxWaveAmplitude, UnityEngine.XR.WSA.WorldManager.Instance.WindSpeed * WaveAmplitudeWindMultiplier);

//         Hydroform.waveSettings.waterHeight = Mathf.Lerp(Hydroform.waveSettings.waterHeight, _targetWaterHeight, Time.deltaTime * UpdateTimeSmoothing);
//         Hydroform.waveSettings.amplitude = Mathf.Lerp(Hydroform.waveSettings.amplitude, _targetWaveAmplitude, Time.deltaTime * UpdateTimeSmoothing);
//         Hydroform.waveSettings.frequency = .8f - (Hydroform.waveSettings.amplitude / 20f);
//         Hydroform.waveSettings.opposingWaves = _targetWaveAmplitude < 2f;
//         Hydroform.waveSettings.direction = UnityEngine.XR.WSA.WorldManager.Instance.WindDirection;
//         Hydroform.surfaceFX.waterColor = Color.Lerp(Hydroform.surfaceFX.waterColor, _targetColor, Time.deltaTime * UpdateTimeSmoothing);
//         Hydroform.shoreFX.foamColor = Color.Lerp(Hydroform.shoreFX.foamColor, _targetFoamColor, Time.deltaTime * UpdateTimeSmoothing);
//         Hydroform.underwater.fogTop = Hydroform.surfaceFX.waterColor;
//         Hydroform.underwater.fogBottom = Hydroform.shoreFX.foamColor;
//         Hydroform.underwater.overlayColor = Hydroform.shoreFX.foamColor;
//         Hydroform.underwater.lipColor = Hydroform.shoreFX.foamColor;
//         Hydroform.underwater.waveColor = Hydroform.shoreFX.foamColor;

//         if (CoreWhirld.Instance)
//         {
//             Hydroform.subsurfaceFX.enableSSS = CoreWhirld.Instance.TimeIsDaylight;
//             Hydroform.shoreFX.deepFoam.enableDeepFoam = CoreWhirld.Instance.TimeIsDaylight;
//         }
//     }

//     void OnQualityUpdate(int qualityLevel, bool isSubmerged, bool timeIsDaylight, bool worldIsLoaded)
//     {
//         if (!Hydroform)
//         {
//             return;
//         }

//         if (RenderSettings.sun)
//         {
//             Hydroform.surfaceFX.specularLight = RenderSettings.sun;
//         }

// //        if (RenderSettings.skybox)
// //        {
// //            _hydroform.reflectFX.skybox = RenderSettings.skybox;
// //        }

//         Hydroform.subsurfaceFX.enableSSS = timeIsDaylight;

//         var targetVertDensity = Mathf.Lerp(.5f, 2, qualityLevel / 6f);
//         var targetPatchSize = (int) Mathf.Lerp(100, 600, qualityLevel / 6f);

//         if (Math.Abs(Hydroform.waveConstruction.vertDensity - targetVertDensity) > 0.01 || Hydroform.waveConstruction.patchSize != targetPatchSize)
//         {
//             Hydroform.waveConstruction.vertDensity = targetVertDensity;
//             Hydroform.waveConstruction.patchSize = targetPatchSize;
//             Hydroform.ResetData();
//         }
//     }
// }