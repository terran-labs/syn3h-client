// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// [ExecuteInEditMode]
// public class CoreEnviro : MonoBehaviour
// {
//     public GameObject EnviroGameObject;

//     public Transform EnviroDirectionalLightTransform;

//     public ReflectionProbe EnvironmentalReflectionProbe;

//     public Cubemap EnvironmentalReflectionCubemap { get; private set; }

//     private float _lastReflectionUpdateTime;
//     private bool _reflectionsAreUpdating;

//     void Start()
//     {
//         Debug.Log("CoreEnviro :: Start");

//         if (CoreController.Instance)
//         {
// //            CoreController.Instance.OnWorldLoaded += OnWorldLoaded;
//             CoreController.Instance.OnWorldInitialized += OnWorldInitialized;
//             CoreController.Instance.OnQualityUpdate += OnQualityUpdate;
//         }

// //        StartCoroutine("OnWeatherInitialized");

//         InvokeRepeating("UpdateReflections", 1f, 1f);
//     }

//     void OnDestroy()
//     {
//         if (CoreController.Instance)
//         {
// //            CoreController.Instance.OnWorldLoaded -= OnWorldLoaded;
//             CoreController.Instance.OnWorldInitialized -= OnWorldInitialized;
//             CoreController.Instance.OnQualityUpdate -= OnQualityUpdate;
//         }
//     }

// //    IEnumerator OnWeatherInitialized()
// //    {
// //        Debug.Log("CoreEnviro :: OnWeatherInitialized");
// //
// //        var i = 0;
// //        while (i < 100)
// //        {
// //            yield return 0;
// //            i += 1;
// //            if (EnviroSky.instance.Weather.WeatherPrefabs.Count > 0)
// //            {
// //                Debug.Log("CoreEnviro :: OnWeatherInitialized :: Assigning Enviro Weather");
// //
// //                yield return 0;
// //                EnviroSky.instance.ChangeWeather(EnviroSky.instance.Weather.WeatherPrefabs.Count - 1);
// //                break;
// //            }
// //        }
// //    }

//     void Update()
//     {
//         // Cancel out infinite origin transform reset to mantain cloud positions 
// //        if (CoreSettings.Instance && CoreSettings.Instance.SceneObjects)
// //        {
// //            EnviroDirectionalLightTransform.localPosition = CoreSettings.Instance.SceneObjects.localPosition * 2;
// //        }
//     }

//     void OnWorldInitialized()
//     {
//         Debug.Log("CoreEnviro :: OnWorldInitialized");

//         EnviroSky.instance.AssignAndStart(CoreCamera.Instance.gameObject, CoreCamera.Instance.ThisCamera);

// //        EnviroSky.instance.ChangeWeather(EnviroSky.instance.Weather.WeatherPrefabs.Count - 1);
//     }

//     void UpdateReflections()
//     {
//         if (!EnvironmentalReflectionProbe)
//         {
//             EnvironmentalReflectionCubemap = null;
//             return;
//         }

//         var qualityLevel = QualitySettings.GetQualityLevel();
//         var reflectionCopyTime = 30;
//         if (qualityLevel >= 5)
//         {
//             reflectionCopyTime = 1;
//         }
//         else if (qualityLevel >= 4)
//         {
//             reflectionCopyTime = 3;
//         }
//         else if (qualityLevel >= 3)
//         {
//             reflectionCopyTime = 10;
//         }

//         if (Time.realtimeSinceStartup - reflectionCopyTime < _lastReflectionUpdateTime)
//         {
//             return;
//         }

//         _lastReflectionUpdateTime = Time.realtimeSinceStartup;

//         StartCoroutine(_updateReflections());
//     }

//     IEnumerator _updateReflections()
//     {
//         if (_reflectionsAreUpdating)
//         {
//             yield return false;
//         }

//         if (!EnvironmentalReflectionProbe.texture)
//         {
//             Debug.Log("CoreEnviro :: Skipped _updateReflections, cubemap texture has not yet initialized");
//         }

//         _reflectionsAreUpdating = true;

// //        Debug.Log("CoreEnviro :: _updateReflections");

//         Cubemap tmp = new Cubemap(EnvironmentalReflectionProbe.resolution, TextureFormat.RGBAHalf, true);

//         for (int mip = 0; mip < tmp.mipmapCount; ++mip)
//         {
//             for (int sourceFace = 0; sourceFace < 6; ++sourceFace)
//             {
//                 Graphics.CopyTexture(EnvironmentalReflectionProbe.texture, sourceFace, mip, tmp, sourceFace, mip);
//                 yield return true;
//             }
//         }

//         EnvironmentalReflectionCubemap = tmp;
//         _reflectionsAreUpdating = false;
//     }

// //    void OnWorldInitialized()
// //    {
// //        Debug.Log("CoreEnviro :: OnWorldInitialized");
// //        EnviroSky.instance.SetWeatherOverwrite(3); 
// //    }

//     void OnQualityUpdate(int qualityLevel, bool isSubmerged, bool timeIsDaylight, bool worldIsLoaded)
//     {
//         EnviroSky.instance.volumeLighting = qualityLevel > 3;
//         EnviroSky.instance.globalFog = qualityLevel > 1;
//         EnviroSky.instance.LightShafts.sunLightShafts = timeIsDaylight && !isSubmerged && qualityLevel >= 3;
//         EnviroSky.instance.LightShafts.moonLightShafts = false;

//         if (qualityLevel >= 5)
//         {
//             EnviroSky.instance.cloudsMode = EnviroSky.EnviroCloudsMode.Both;
//             EnviroSky.instance.ChangeCloudsQuality(EnviroCloudSettings.CloudQuality.High);
//         }
//         else if (qualityLevel >= 4)
//         {
//             EnviroSky.instance.cloudsMode = EnviroSky.EnviroCloudsMode.Both;
//             EnviroSky.instance.ChangeCloudsQuality(EnviroCloudSettings.CloudQuality.Medium);
//         }
//         else if (qualityLevel >= 3)
//         {
//             EnviroSky.instance.cloudsMode = EnviroSky.EnviroCloudsMode.Both;
//             EnviroSky.instance.ChangeCloudsQuality(EnviroCloudSettings.CloudQuality.Low);
//         }
//         else
//         {
//             EnviroSky.instance.cloudsMode = EnviroSky.EnviroCloudsMode.Flat;
//             EnviroSky.instance.ChangeCloudsQuality(EnviroCloudSettings.CloudQuality.Low);
//         }
//     }
// }