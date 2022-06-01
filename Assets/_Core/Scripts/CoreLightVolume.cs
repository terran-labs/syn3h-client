// using System.Collections;
// using UnityEngine;

// public class CoreLightVolume : MonoBehaviour
// {
//     private EnviroVolumeLight _lightVolumeEV;

//     void Start()
//     {
//         StartCoroutine(Init());
//     }

//     IEnumerator Init()
//     {
//         yield return true;

//         if (EnviroSky.instance)
//         {
//             _lightVolumeEV = gameObject.AddComponent<EnviroVolumeLight>();
//         }
//     }

//     void OnEnable()
//     {
//         if (CoreController.Instance)
//         {
//             CoreController.Instance.OnQualityUpdate += OnQualityUpdate;
//         }
//     }

//     void OnDisable()
//     {
//         if (CoreController.Instance)
//         {
//             CoreController.Instance.OnQualityUpdate -= OnQualityUpdate;
//         }
//     }

//     void OnQualityUpdate(int qualityLevel, bool isSubmerged, bool timeIsDaylight, bool worldIsLoaded)
//     {
// //        if (_lightVolumeDS)
// //        {
// //            _lightVolumeDS.Scattering = 0.5f;
// //            
// //            if (qualityLevel < 3)
// //            {
// //                _lightVolumeDS.enabled = false;
// //            }
// //            else
// //            {
// //                _lightVolumeDS.enabled = true;
// //
// //                if (qualityLevel >= 5)
// //                {
// //                    _lightVolumeDS.Samples = DS_SamplingQuality.x16;
// //                }
// //                else if (qualityLevel >= 4)
// //                {
// //                    _lightVolumeDS.Samples = DS_SamplingQuality.x8;
// //                }
// //                else
// //                {
// //                    _lightVolumeDS.Samples = DS_SamplingQuality.x4;
// //                }
// //            }
// //        }

//         if (_lightVolumeEV)
//         {
//             if (qualityLevel < 4)
//             {
//                 _lightVolumeEV.enabled = false;
//             }
//             else
//             {
//                 _lightVolumeEV.enabled = true;
// //                _lightVolumeEV.ExtinctionCoef = 0.08f; // Turn down the visible light intensity

//                 if (qualityLevel >= 5)
//                 {
//                     _lightVolumeEV.SampleCount = 16;
//                     _lightVolumeEV.Noise = true;
//                 }
//                 else if (qualityLevel >= 4)
//                 {
//                     _lightVolumeEV.SampleCount = 8;
//                     _lightVolumeEV.Noise = true;
//                 }
//                 else
//                 {
//                     _lightVolumeEV.SampleCount = 4;
//                     _lightVolumeEV.Noise = false;
//                 }
//             }
//         }
//     }
// }