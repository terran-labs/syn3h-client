// using UnityEngine;

// public class CoreMapMagic : MonoBehaviour
// {
//     // public AwesomeTechnologies.VegetationSystem.VegetationSystemPro MyVegetationSystem;
//     private MapMagic.MapMagic _mapMagic;

//     private int _lastQualityLevel;

//     void Start()
//     {
//         _mapMagic = MapMagic.MapMagic.instance;

//         if (!_mapMagic)
//         {
//             gameObject.SetActive(false);
//         }

//         if (CoreController.Instance)
//         {
//             CoreController.Instance.OnQualityUpdate += OnQualityUpdate;
//         }
//     }

//     void OnDestroy()
//     {
//         if (CoreController.Instance)
//         {
//             CoreController.Instance.OnQualityUpdate -= OnQualityUpdate;
//         }
//     }

// //    public void OnEnable()
//     void OnQualityUpdate(int qualityLevel, bool isSubmerged, bool timeIsDaylight, bool worldIsLoaded)
//     {
//         if (qualityLevel == _lastQualityLevel)
//         {
//             return;
//         }

//         Debug.Log("CoreMapMagic :: OnQualityUpdate");

//         _mapMagic.enableRange = (int) Mathf.Lerp(50, 900, qualityLevel / 5f);

//         // MM V1
//         _mapMagic.generateRange = (int) Mathf.Lerp(100, 1000, qualityLevel / 5f);
//         _mapMagic.removeRange = (int) Mathf.Lerp(150, 1100, qualityLevel / 5f);

//         // MM V2
// //        _mapMagic.tiles.generateRange = (int) Mathf.Lerp(100, 1000, qualityLevel / 5f);
// //        _mapMagic.tiles.removeRange = (int) Mathf.Lerp(150, 1100, qualityLevel / 5f);
// //        _mapMagic.tiles.generateInfinite = true;

//         _mapMagic.enableRange = (int) Mathf.Lerp(50, 900, qualityLevel / 5f);
//         _mapMagic.hideFarTerrains = true;

//         // MM V1
//         if (qualityLevel > 4)
//         {
//             _mapMagic.castShadows = true;
//             _mapMagic.pixelError = 3;
//         }
//         else if (qualityLevel > 3)
//         {
//             _mapMagic.castShadows = false;
//             _mapMagic.pixelError = 10;
//         }
//         else if (qualityLevel > 2)
//         {
//             _mapMagic.castShadows = false;
//             _mapMagic.pixelError = 20;
//         }
//         else if (qualityLevel > 1)
//         {
//             _mapMagic.castShadows = false;
//             _mapMagic.pixelError = 30;
//         }

//         // MM V2
// //        if (qualityLevel > 4)
// //        {
// //            _mapMagic.terrainSettings.castShadows = true;
// //            _mapMagic.terrainSettings.pixelError = 3;
// //        }
// //        else if (qualityLevel > 3)
// //        {
// //            _mapMagic.terrainSettings.castShadows = false;
// //            _mapMagic.terrainSettings.pixelError = 10;
// //        }
// //        else if (qualityLevel > 2)
// //        {
// //            _mapMagic.terrainSettings.castShadows = false;
// //            _mapMagic.terrainSettings.pixelError = 20;
// //        }
// //        else if (qualityLevel > 1)
// //        {
// //            _mapMagic.terrainSettings.castShadows = false;
// //            _mapMagic.terrainSettings.pixelError = 30;
// //        }

//         // MM V1
//         foreach (MapMagic.Chunk tw in MapMagic.MapMagic.instance.chunks.All())
//         {
//             tw.SetSettings();
//         }

//         // MM V2
// //        _mapMagic.GenerateAll();

//         _lastQualityLevel = qualityLevel;
//     }

// //    public void OnEnable()
// //    {
// //        MapMagic.MapMagic.OnApplyCompleted -= ProcessTerrainTile; // in case it was not called on disable 
// //        MapMagic.MapMagic.OnApplyCompleted += ProcessTerrainTile;
// //    }
// //
// //    public void OnDisable()
// //    {
// //        MapMagic.MapMagic.OnApplyCompleted -= ProcessTerrainTile;
// //    }
// //
// //    public void ProcessTerrainTile(Terrain terrain)
// //    {
// //        Debug.Log("CoreMapMagic :: ProcessTerrainTile");
// //        
// //        if (MyVegetationSystem)
// //        {
// //            AwesomeTechnologies.VegetationSystem.UnityTerrain myUnityTerrain = terrain.gameObject.AddComponent<AwesomeTechnologies.VegetationSystem.UnityTerrain>();
// //            myUnityTerrain.AutoAddToVegegetationSystem = true;
// //            myUnityTerrain.AddTerrainToVegetationSystem();
// //            Debug.Log("CoreMapMagic :: ProcessTerrainTile :: AddTerrainToVegetationSystem() complete");
// ////            MyVegetationSystem.AddAllUnityTerrains();
// //        }
// //    }
// }