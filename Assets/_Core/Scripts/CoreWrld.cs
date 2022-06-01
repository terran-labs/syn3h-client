// //using ProBuilder2.MeshOperations;

// using UnityEngine;
// using Wrld;
// using Wrld.Scripts.Utilities;

// public class CoreWrld : MonoBehaviour
// {
//     public Camera StreamingCamera;

//     private Api _api;
//     private double _initialLat;
//     private double _initialLon;

//     private CoreController _myController;
//     private CoreSettings _mySettings;

//     private void Start()
//     {
//         _myController = FindObjectOfType<CoreController>();
//         _mySettings = FindObjectOfType<CoreSettings>();

//         if (!_myController || !_mySettings)
//         {
//             Debug.LogError("WrldEngine :: Start :: Disabling, No CoreController found");
//             enabled = false;
//             return;
//         }

//         _initialLat = _mySettings.GeoDestLat;
//         _initialLon = _mySettings.GeoDestLon;
//         if (_initialLat.Equals(0) || _initialLon.Equals(0))
//         {
//             _initialLat = _mySettings.GeoDefaultLat;
//             _initialLon = _mySettings.GeoDefaultLon;
//         }

//         var defaultConfig = ConfigParams.MakeDefaultConfig();
//         defaultConfig.DistanceToInterest = 0;
//         defaultConfig.LatitudeDegrees = _initialLat;
//         defaultConfig.LongitudeDegrees = _initialLon;
//         defaultConfig.HeadingDegrees = 0;
//         defaultConfig.MaterialsDirectory = "WrldMaterials/";
//         defaultConfig.OverrideLandmarkMaterial = null;
//         defaultConfig.Collisions.TerrainCollision = true;
//         defaultConfig.Collisions.RoadCollision = true;
//         defaultConfig.Collisions.BuildingCollision = true;
//         defaultConfig.StreamingLodBasedOnDistance = true;

//         var rootTransform = transform;
//         if (Api.Instance == null)
//         {
//             try
//             {
//                 Api.Create(_mySettings.WrldApiKey, CoordinateSystem.UnityWorld, rootTransform, defaultConfig);
//             }
//             catch (InvalidApiKeyException)
//             {
//                 Debug.LogError("WrldEngine :: OnWorldLoaded :: Invalid WRLD API Key");
//             }
//         }

//         _api = Api.Instance;

// //        CoreController.Instance.OnQualityChange += OnQualityChange;
//     }

// //    private void OnDisable()
// //    {
// //        // @todo delete transform children(?)
// //
// //        if (!CoreController.Instance)
// //        {
// //            return;
// //        }
// //
// ////        CoreController.Instance.OnQualityChange -= OnQualityChange;
// //    }

//     void Update()
//     {
//         if (!Camera.main)
//         {
//             return;
//         }

//         StreamingCamera.transform.position = Camera.main.transform.position + (Vector3.up * 1000);
//         StreamingCamera.orthographicSize = Camera.main.farClipPlane * .75f;
//         _api.StreamResourcesForCamera(StreamingCamera);

//         _api.Update();
//     }

//     private void OnDestroy()
//     {
//         _api.Destroy();
//     }

// //    void OnWorldLoaded()
// //    {
// //        
// //    }
// //
// //    void OnQualityChange(int qualityLevel)
// //    {
// ////        Debug.Log("WrldEngine :: OnQualityChange");
// //    }
// }