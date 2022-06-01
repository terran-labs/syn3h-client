// using System.Collections;
// using UnityEngine;
// //using AwesomeTechnologies;

// public class CoreVegitationStudio : MonoBehaviour
// {
//     public AwesomeTechnologies.VegetationSystem.VegetationSystemPro MyVegetationSystem;
//     private MapMagic.MapMagic _mapMagic;

//     private float _lastSeaLevel;
//     private int _lastQualityLevel;

//     void Start()
//     {
//         if (CoreController.Instance)
//         {
//             CoreController.Instance.OnQualityUpdate += OnQualityUpdate;
//         }

//         InvokeRepeating("_UpdateEnvironment", 1f, 1f);
//     }

//     void OnDestroy()
//     {
//         if (CoreController.Instance)
//         {
//             CoreController.Instance.OnQualityUpdate -= OnQualityUpdate;
//         }
//     }

//     void OnQualityUpdate(int qualityLevel, bool isSubmerged, bool timeIsDaylight, bool worldIsLoaded)
//     {
//         if (qualityLevel == _lastQualityLevel)
//         {
//             return;
//         }

//         var qualityFactor = Mathf.Max(0, qualityLevel - 2) / 3f;

//         Debug.Log("CoreVegitationStudio :: OnQualityUpdate (qualityFactor: " + qualityFactor + ")");



// //        MyVegetationSystem.VegetationSettings.RenderVegetation = qualityLevel >= 2;
// //        MyVegetationSystem.VegetationSettings.RenderTrees = qualityLevel >= 3;
// //        MyVegetationSystem.VegetationSettings.UseTouchReact = qualityLevel >= 4;
//         MyVegetationSystem.VegetationSettings.GrassShadows = qualityLevel >= 3;
//         MyVegetationSystem.VegetationSettings.PlantShadows = qualityLevel >= 3;
//         MyVegetationSystem.VegetationSettings.TreeShadows = qualityLevel >= 2;
//         MyVegetationSystem.VegetationSettings.LargeObjectShadows = qualityLevel >= 2;

//         MyVegetationSystem.VegetationSettings.GrassDensity = Mathf.Lerp(0, 1, qualityFactor);
//         MyVegetationSystem.VegetationSettings.PlantDensity = Mathf.Lerp(0, 1, qualityFactor);
//         MyVegetationSystem.VegetationSettings.PlantDistance = Mathf.Lerp(50, 300, qualityFactor);
//         MyVegetationSystem.VegetationSettings.AdditionalTreeMeshDistance = Mathf.Lerp(0, 100, qualityFactor);
//         MyVegetationSystem.VegetationSettings.AdditionalBillboardDistance = Mathf.Lerp(100, 3000, qualityFactor);

//         MyVegetationSystem.ClearCache();
// //        MyVegetationSystem.RefreshTerrainHeightmap();

// //        MyVegetationSystem.SetupCullingGroup();
// //        MyVegetationSystem.SetDirty();

//         _lastQualityLevel = qualityLevel;
//     }

//     IEnumerator _UpdateEnvironment()
//     {
//         WhirldData _myWhirld = WhirldData.Instance;
//         float thisSeaLevel = _myWhirld.SeaEnabled ? _myWhirld.SeaAltitude : -9999;

//         if (thisSeaLevel == _lastSeaLevel)
//         {
//             return null;
//         }

//         _lastSeaLevel = MyVegetationSystem.SeaLevel = thisSeaLevel;
//         MyVegetationSystem.ClearCache();
// //        MyVegetationSystem.RefreshTerrainHeightmap();

//         return null;
//     }
// }