// using System;
// using System.Diagnostics;
// using UnityEngine;
// using System.Linq;
// using Debug = UnityEngine.Debug;

// public class CoreGeoEngine : MonoBehaviour
// {
//     public float PeriodicUpdateInterval = .5f;
//     public int[] GeoZoomLevels;
//     public int GeoAltitudeMinZoom = 500;
//     public int GeoAltitudeaMaxZoom = 5000;
// //    public float GeoZoomChangeThreshold = .75f;
//     public float GeoZoomMinChangeSeconds = 3f;

//     [NonSerialized] public float CameraHeightAboveTerrain;
//     [NonSerialized] public float DesiredZoomLevel;
//     [NonSerialized] public int TargetZoomLevel;

//     public static float MapScale;

//     private double _initialLat;
//     private double _initialLon;
//     private float _timeLastZoomChange;

//     private OnlineMaps _map;

//     private void Start()
//     {
//         // Singleton reference init
//         _map = OnlineMaps.instance;
//         if (!_map)
//         {
//             Debug.LogError("CoreGeoEngine :: Start :: no OnlineMaps.instance found");
//             return;
//         }

//         var tileSetControlNear = OnlineMapsTileSetControl.instance;
//         if (!tileSetControlNear)
//         {
//             Debug.LogError("CoreGeoEngine :: Start :: no OnlineMapsTileSetControl.instance found");
//             return;
//         }

//         // Assign Bing API Key for heightmap access
//         tileSetControlNear.bingAPI = CoreController.Instance.MyConfig.ApiKeyBingMaps;

//         // Subscribe to change zoom
//         OnlineMaps.instance.OnChangeZoom += OnChangeZoom;
//         OnlineMaps.instance.OnChangePosition += OnChangePosition;

//         // Set initial zoom
//         _map.zoom = GeoZoomLevels[GeoZoomLevels.Length - 1];

//         // Set initial position
//         _initialLat = CoreSettings.Instance.GeoDestLat;
//         _initialLon = CoreSettings.Instance.GeoDestLon;
//         if (_initialLat.Equals(0) || _initialLon.Equals(0))
//         {
//             _initialLat = CoreSettings.Instance.GeoDefaultLat;
//             _initialLon = CoreSettings.Instance.GeoDefaultLon;
//         }
//         _map.SetPosition(_initialLon, _initialLat);

//         // Trigger updates
//         InvokeRepeating("_periodicUpdate", 0, PeriodicUpdateInterval);

//         // Showtime
//         _map.RedrawImmediately();
//         OnChangeZoom();
//         OnChangePosition();
//     }

// //    void Update()
// //    {
// //        if (!CoreCamera.Instance || !_apiNear)
// //        {
// //            return;
// //        }
// //
// //    }

//     void OnDisable()
//     {
//         MapScale = 0f;
//     }

//     // Move physical terrain map to center it under camera
//     private void OnChangePosition()
//     {
//         if (!CoreCamera.Instance || !_map)
//         {
//             return;
//         }

//         var myCameraTransform = CoreCamera.Instance.transform;

//         // Calculate translation to physically center map on camera
//         Vector2 mapDistanceBetweenCornersKm = OnlineMapsUtils.DistanceBetweenPoints(_map.topLeftPosition, _map.bottomRightPosition);
//         var mapCameraAlignedPosition = new Vector2(myCameraTransform.position.x, myCameraTransform.position.z);
//         MapScale = mapDistanceBetweenCornersKm.x * 1000;
//         var mapCenterOffset = new Vector2(MapScale * 0.5f, MapScale * -0.5f);
//         var mapCameraCenteredPosition = mapCameraAlignedPosition + mapCenterOffset;

//         // Update map object's physical position in world 
//         _map.transform.position = new Vector3(mapCameraCenteredPosition.x, 0, mapCameraCenteredPosition.y);
//     }

//     // Scale physical terrain map for proper world-space positioning at current zoom level
//     private void OnChangeZoom()
//     {
//         if (!_map)
//         {
//             return;
//         }

//         Vector2 mapDistanceBetweenCornersKm = OnlineMapsUtils.DistanceBetweenPoints(_map.topLeftPosition,
//             _map.bottomRightPosition);

//         _map.tilesetSize = mapDistanceBetweenCornersKm * 1000;
//     }

//     private void _periodicUpdate()
//     {
//         if (!CoreCamera.Instance || !_map)
//         {
//             return;
//         }

//         var myCameraTransform = CoreCamera.Instance.transform;

//         // Ensure MeshRenderer motion vectors are disabled to prevent postfx stack from blurring terran with each new position update
//         var meshRenderer = GetComponent<MeshRenderer>();
//         meshRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.Camera;

//         // Determine camera altitude above terrain
//         double tlx, tly, brx, bry;
//         _map.GetCorners(out tlx, out tly, out brx, out bry);
//         Vector3 cameraTerrainPosition = OnlineMapsTileSetControl.instance.GetWorldPositionWithElevation(myCameraTransform.position, tlx, tly, brx, bry);
//         CameraHeightAboveTerrain = myCameraTransform.position.y - cameraTerrainPosition.y /* + HorizonSubmersion*/;

//         // Never change zoom level more than once every GeoZoomMinChangeSeconds

//         if (Time.time > _timeLastZoomChange + GeoZoomMinChangeSeconds)
//         {
//             // Determine desired zoom level. Include GeoZoomChangeThreshold to overshoot defined min/max levels just enough to trigger a change by exceeding the threshold at min and max zoom levels
//             DesiredZoomLevel = Mathf.Lerp(
//                 GeoZoomLevels[GeoZoomLevels.Length - 1]/* + GeoZoomChangeThreshold*/,
//                 GeoZoomLevels[0]/* - GeoZoomChangeThreshold*/,
//                 (CameraHeightAboveTerrain - GeoAltitudeMinZoom) / (GeoAltitudeaMaxZoom - GeoAltitudeMinZoom)
//             );

//             // Snap desired zoom level to allowed zoom levels
//             // props: https://stackoverflow.com/posts/10120978/revisions
//             TargetZoomLevel = GeoZoomLevels.OrderBy(x => Math.Abs((long) x - DesiredZoomLevel)).First();

// //            TargetZoomLevel = 0;
// //            foreach (var level in GeoZoomLevels)
// //            {
// //                // looking for nearest lower valid zoom level, and level is greater than current target, and level is less than current desired
// //                if (DesiredZoomLevel < _map.zoom && level > TargetZoomLevel && level <= DesiredZoomLevel)
// //                {
// //                    TargetZoomLevel = level;
// //                }
// //                // ditto, but opposite
// //                else if (DesiredZoomLevel > _map.zoom && (level < TargetZoomLevel || TargetZoomLevel == 0) && level > DesiredZoomLevel)
// //                {
// //                    TargetZoomLevel = level;
// //                }
// //            }

//             // @todo don't update zoom if terrain height is updating

//             // Update map zoom if necessary. Skip map updates if desired change doesn't exceed the change threshold. This prevents map from flickering between two zoom levels.
//             if (TargetZoomLevel > 0 && _map.zoom != TargetZoomLevel /* && (DesiredZoomLevel <= _map.zoom - GeoZoomChangeThreshold || DesiredZoomLevel >= _map.zoom + GeoZoomChangeThreshold)*/)
//             {
//                 _map.zoom = TargetZoomLevel;
//                 _timeLastZoomChange = Time.time;
//             }
//         }

//         // Translate camera offset into lat/lon for updated map centerpoint
//         // Props: https://stackoverflow.com/questions/5857523/calculate-latitude-and-longitude-having-meters-distance-from-another-latitude-lo

//         //Earth’s radius, sphere
//         double R = 6378137;

//         //offsets in meters
//         double dn = -myCameraTransform.localPosition.z + CoreSettings.Instance.SceneObjects.localPosition.z;
//         double de = -myCameraTransform.localPosition.x + CoreSettings.Instance.SceneObjects.localPosition.x;

//         //Coordinate offsets in radians
//         double dLat = dn / R;
//         double dLon = de / (R * Math.Cos(Math.PI * _initialLat / 180));

//         //OffsetPosition, decimal degrees
//         double targetLat = _initialLat + dLat * 180 / Mathf.PI;
//         double targetLon = _initialLon + dLon * 180 / Mathf.PI;

// //        double targetLat;
// //        double targetLon;
// //        OnlineMapsTileSetControl.instance.GetCoordsByWorldPosition(out targetLon, out targetLat,
// //            myCameraTransform.localPosition - CoreSettings.Instance.SceneObjects.localPosition);

//         // Update map lat/lon. This will trigger a recentering of the physical map in the scene
//         _map.SetPosition(targetLon, targetLat);

// //        Debug.Log("CoreGeoEngine :: GeoUpdate :: Complete (altitude: " + CameraHeightAboveTerrain + ". terrainZoom: " + _terrainZoomLevel + ")");
// }

//     static public void OpenCacheDirectory()
//     {
//         var path = OnlineMapsCache.instance.GetFileCacheFolder();
//         var dir = OnlineMapsCache.instance.fileCacheTilePath;
//         dir = dir.Split('/')[0];
//         path.Append("/").Append(dir);
//         Debug.Log("CoreGeoEngine :: Opening cache output dir: " + path);
//         Process.Start(path.ToString());
//     }
// }