using System.Collections;
//using ProBuilder2.Common;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

#if MAPMAGIC
using MapMagic;
#endif

//public enum CoreWhirldType
//{
//    CustomWorld,
//    RealTerrainWorld,
//}

public enum CoreWhirldStatus
{
	Good,
	Loading,
	LoadingHardReset,
	Generating,
}

public class CoreWhirld : MonoBehaviour
{
	public static CoreWhirld Instance { get; private set; }
	public CoreWhirldStatus Status; // { get; private set; }

	public CoreController MyController;
	public CoreSettings MySettings;

	//    public GameObject DefaultHazeController;

	//    public GameObject OceanPrefab;

	public float Progress; // { get; private set; }

	//    private DS_HazeCore _activeHazeCore;
	//    private DS_HazeZone _defaultHazeZone;

#if USKY
    private usky.uSkyTimeline _uSkyTimeline;
#endif
#if TRUESKY
    private simul.trueSKY _trueSky;
#endif
	public bool TimeControlEnabled { get; private set; }

	//    public float WorldVisibilityDistance { get; private set; }
	public bool TimeFreeze;

	public float TimeSpeed;
	public bool TimeIsDaylight = true;

	public int BaseMetersAboveGround = 5;
	public int BaseMetersAboveGroundTerrainWorld = 300;

	public int TerrainTransitionHighAltitude;
	public float TerrainTransitionHighTimeDebounce;
	public int TerrainUltraHighAltitudeViewDistance;
	public int TerrainUltraHighAltitudeResolution;
	public int TerrainHighAltitudeViewDistance;
	public int TerrainHighAltitudeResolution;
	private int _terrainIsHighAltitude;
	private float _terrainHighLastTransitionTime;
	private int _terrainDefaultViewDistance;
	private int _terrainDefaultResolution;

	public float WhirldTime
	{
		get
		{
#if TRUESKY
            if (_trueSky && _trueSky.enabled)
            {
                return _trueSky.time - Mathf.FloorToInt(_trueSky.time);
            }
#endif
#if USKY
            if (_uSkyTimeline && _uSkyTimeline.enabled)
            {
                return _uSkyTimeline.Timeline / 24;
            }
#endif
#if ENVIRO
            if (EnviroSky.instance)
            {
                return EnviroSky.instance.GetUniversalTimeOfDay() / 24f;
            }
#endif

			return .5f;
		}
	}

	public LayerMask LayerMaskTerrain;
	public LayerMask LayerMaskDontClip;

	private bool _customBase;

	void Start()
	{
		Debug.Log("CoreWhirld :: LayerMaskTerrain: " + (int)LayerMaskTerrain);
		Debug.Log("CoreWhirld :: LayerMaskDontClip: " + (int)LayerMaskDontClip);
	}

	void OnEnable()
	{
		Instance = this;
		MyController.OnWorldLoaded += OnWorldLoaded;
		MyController.OnWorldInitialized += OnWorldInitialized;
		MyController.OnQualityUpdate += OnQualityUpdate;
	}

	void OnDisable()
	{
		MyController.OnWorldLoaded -= OnWorldLoaded;
		MyController.OnWorldInitialized -= OnWorldInitialized;
		MyController.OnQualityUpdate -= OnQualityUpdate;
	}

	public void Update()
	{
		if (!CoreCamera.Instance || !Camera.main)
		{
			return;
		}

		// Time management :: TrueSKY
#if TRUESKY
        if (_trueSky && _trueSky.enabled)
        {
            _trueSky.speed = (TimeFreeze ? 0 : TimeSpeed);

            if (_uSkyTimeline)
            {
                _uSkyTimeline.PlayAtRuntime = false;
            }
        }

        // Time management :: uSky
        else if (_uSkyTimeline && _uSkyTimeline.enabled)
        {
            _uSkyTimeline.PlayAtRuntime = !TimeFreeze;
        }
#endif
#if ENVIRO
        //        // Time management :: uSky
        //        if (_uSkyTimeline && _uSkyTimeline.enabled)
        //        {
        //            _uSkyTimeline.PlayAtRuntime = !TimeFreeze;
        //            _uSkyTimeline.PlaySpeed = (MySettings.TimeSpeedMultiplier * TimeSpeed);
        //        }

        // Time management :: Enviro
        if (EnviroSky.instance)
        {
            EnviroSky.instance.GameTime.ProgressTime = (TimeFreeze ? EnviroTime.TimeProgressMode.None : EnviroTime.TimeProgressMode.Simulated);
            EnviroSky.instance.GameTime.DayLengthInMinutes = EnviroSky.instance.GameTime.NightLengthInMinutes = (60 * 60 * 24) / TimeSpeed / MySettings.TimeSpeedMultiplier;
        }
#endif

		// Time management :: Daylight
		TimeIsDaylight = WhirldTime > MySettings.TimeSunrise && WhirldTime < MySettings.TimeSunset;

		// Camera Following :: Default haze zone
		//        if (_defaultHazeZone)
		//        {
		//            _defaultHazeZone.transform.position = Camera.main.transform.position;
		//        }

		// Set terrain view distance & resolution for camera height
		if (Time.time - _terrainHighLastTransitionTime > TerrainTransitionHighTimeDebounce)
		{
			var deltaTerrainIsHighAltitude = _terrainIsHighAltitude;
			if (CoreCamera.Instance.transform.position.y > TerrainTransitionHighAltitude * 2)
			{
				// ultra-high-altitude
				_terrainIsHighAltitude = 2;
			}
			else if (CoreCamera.Instance.transform.position.y > TerrainTransitionHighAltitude)
			{
				// high altitude
				_terrainIsHighAltitude = 1;
			}
			else
			{
				_terrainIsHighAltitude = 0;
			}

			if (deltaTerrainIsHighAltitude != _terrainIsHighAltitude)
			{
				_terrainHighLastTransitionTime = Time.time;
				Debug.Log("CoreWhirld :: Update :: _terrainIsHighAltitude set to (" + _terrainIsHighAltitude + ")");
			}
		}

		// @temporarily disabled MM view range increase system, it was slowing things down too much
		//#if MAPMAGIC
		//        if (MapMagic.MapMagic.instance)
		//        {
		//            var desiredTerrainViewDistance = _terrainDefaultViewDistance;
		//            var desiredTerrainResolution = _terrainDefaultResolution;
		//
		//            if (_terrainIsHighAltitude > 0)
		//            {
		//                desiredTerrainViewDistance = _terrainIsHighAltitude == 2 ? TerrainUltraHighAltitudeViewDistance : TerrainHighAltitudeViewDistance;
		//                desiredTerrainResolution = _terrainIsHighAltitude == 2 ? TerrainUltraHighAltitudeResolution : TerrainHighAltitudeResolution;
		//            }
		//
		//            if (MapMagic.MapMagic.instance.generateRange != desiredTerrainViewDistance || MapMagic.MapMagic.instance.resolution != desiredTerrainResolution)
		//            {
		//              //  MapMagic.MapMagic.instance.ClearResults();
		//                MapMagic.MapMagic.instance.generateRange = desiredTerrainViewDistance;
		//                MapMagic.MapMagic.instance.enableRange = desiredTerrainViewDistance + 100;
		//                MapMagic.MapMagic.instance.removeRange = desiredTerrainViewDistance + 200;
		//                MapMagic.MapMagic.instance.resolution = desiredTerrainResolution;
		//                MapMagic.MapMagic.instance.Generate(force: false);
		//                Debug.Log("CoreWhirld :: Update :: Setting MM view distance, resolution to (" + desiredTerrainViewDistance + ", " + desiredTerrainResolution + ")");
		//            }
		//        }
		//#endif
	}

	public float GetTime()
	{
		return WhirldTime;
	}

	public void SetTime(float time)
	{
#if TRUESKY
        if (_trueSky)
        {
            _trueSky.time = Mathf.FloorToInt(_trueSky.time) + time;
            Debug.Log("CoreWhirld :: SetTime :: _trueSky time set to " + _trueSky.time + " (" + time + ")");
            return;
        }
#endif

		//        if (_uSkyTimeline)
		//        {
		//            _uSkyTimeline.Timeline = time * 24;
		//            Debug.Log("CoreWhirld :: SetTime :: _uSkyTimeline time set to " + _uSkyTimeline.Timeline);
		//            return;
		//        }

		if (EnviroSky.instance)
		{
			EnviroSky.instance.SetInternalTimeOfDay(time * 24);
			return;
		}

		Debug.Log("CoreWhirld :: SetTime :: No active time management systems found");
	}

	public void ResetPlayer()
	{
		// Reset world origin to ensure we set the base at 0,0,0
		ResetCameraPosition();

		// Ensure we have a base, and it is properly positioned in relation to the new terrain we just generated
		SetBasePosition();

		// Instantiate player vehicle
		var vehIndex = MyController.DefaultVehicleIndex;
		// if (OnlineMaps.instance)
		// {
		//     // If this is a "real terrain" world, we should start in the sky. With a jet.
		//     vehIndex = MyController.DefaultVehicleIndexTerrainWorld;
		// }

		MyController.LoadVehicle(vehIndex);
	}

	public void RandomizeEverything(bool regenerateEverything = true)
	{
		StartCoroutine(_randomizeEverything(regenerateEverything));
	}

	private void RandomizeTime()
	{
		// Random time of day
		float randomTime = Random.Range(MySettings.TimeSunrise, MySettings.TimeSunset);
		Debug.Log("CoreWhirld :: RandomizeTime :: Setting time to (" + randomTime + ")");
		SetTime(randomTime);
	}

	private IEnumerator _randomizeEverything(bool regenerateEverything = true)
	{
		Status = CoreWhirldStatus.Generating;

		// Yield execution of this coroutine and return to the main loop until next frame
		yield return null;

		//        CoreCameraFade.FadeMaskIn();

		// Assign a random seed
		Random.seed = System.Environment.TickCount;

		// Provide MapMagic and other embedded libs a chance to initialize
		//        yield return true;

		// Reset world origin and set camera to a nice place to watch the world load (in case it is indeed loading)
		ResetCameraPosition();

		// Destroy dynamic objects which will soon be recreated
		if (regenerateEverything)
		{
			Debug.Log("CoreWhirld :: _randomizeEverything :: destroying dynamic objects");
			MyController.DestroyVehicle();
			MyController.RemoveAllBots();
		}

		//        var baseObj = GameObject.Find("Base");
		//        if (baseObj)
		//        {
		//            Destroy(baseObj);
		//        }

		// Deactivate settings which could prove confusing in the new world
		MySettings.FreezeMotion = false;

		//        if (regenerateEverything)
		//        {
		RandomizeTime();
		//        }

		//        yield return true;

		var mmInScene = false;
		var vlInScene = false;

#if MAPMAGIC && VOXELAND //// If this world contains a MapMagic instance,
// regenerate it with a new random seed
// ...and wait for it to finish generating before instantiating a player vehicle
// ... but don't regenerate it if this is the first world load, and a pinned terrain instance is present
        mmInScene = MapMagic.MapMagic.instance != null;
        vlInScene = Voxeland5.Voxeland.instances.Count > 0;
        var terrainInScene = FindObjectOfType<Terrain>() != null;
        if (mmInScene || vlInScene)
        {
            if (regenerateEverything || !terrainInScene)
            {
                if (MapMagic.MapMagic.instance != null)
                {
                    Debug.Log("CoreWhirld :: _randomize :: Initiating MapMagic rebuild...");

                    var seed = Random.Range(0, 99999);
                    MapMagic.MapMagic.instance.seed = seed;
                    MapMagic.MapMagic.instance.ClearResults();

                    if (MapMagic.MapMagic.instance != null)
                    {
                        foreach (MapMagic.Chunk chunk in MapMagic.MapMagic.instance.chunks.All())
                        {
                            if (chunk.terrain != null)
                            {
                                MapMagic.Extensions.RemoveChildren(chunk.terrain.transform);
                            }
                        }
                    }

                    yield return 0;

                    MapMagic.MapMagic.instance.Generate(force: true);
                }

                if (Voxeland5.Voxeland.instances.Count > 0)
                {
                    Debug.Log("CoreWhirld :: _randomize :: Initiating Voxeland rebuild...");

                    foreach (Voxeland5.Voxeland instance in Voxeland5.Voxeland.instances)
                    {
                        instance.data.generator.seed++;
                        instance.ClearResults();
                        yield return 0;
                        instance.Generate(true);
                    }

                    // @dragonhere - voxeland sucks, keeps creating a terrain then immediatley clearing it.
                    yield return new WaitForSecondsRealtime(5f);
                }
            }
        }
#endif

		// WRLD
#if WRLD
        if (Wrld.Api.Instance != null)
        {
            yield return new WaitForSecondsRealtime(2f);
        }
#endif

		// Runtime-generated terrains
#if WRLD
        if (mmInScene || vlInScene || OnlineMaps.instance || Wrld.Api.Instance != null)
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(1f);

                //                if (OnlineMaps.instance.bufferStatus != OnlineMapsBufferStatus.complete)
                //                {
                //                    Debug.Log("CoreWhirld :: _randomize :: Waiting for earth terrain to finish working...");
                //                    continue;
                //                }

                var terrainHeight = CheckTerrainHeight(Vector2.zero);
                if (terrainHeight == null)
                {
                    Debug.Log("CoreWhirld :: _randomize :: Waiting for terrain under player to generate...");
                    continue;
                }

                Debug.Log("CoreWhirld :: _randomize :: World generation successful.");
                break;
            }
        }
#endif

		// Haze
		// @deprecated
		//        if (!WhirldData.Instance || WhirldData.Instance.EnableAtmospherics)
		//        {
		//            // TODO this code section previously created a Haze Core if none was present in scene. This behavior may ultimatley prove diserable, but it is disabled for now as it conflists with Enviro-enabled scenes. 
		//
		//            // Find default (lowest priority) Haze Zone so we can move it with camera
		//            var hazeZones = FindObjectsOfType<DS_HazeZone>();
		//            foreach (var zone in hazeZones)
		//            {
		//                if (_defaultHazeZone && zone.Priority > _defaultHazeZone.Priority)
		//                {
		//                    continue;
		//                }
		//
		//                _defaultHazeZone = zone;
		//            }
		//
		//            // Ensure there is a Haze Controller object present in scene
		//            if (_defaultHazeZone)
		//            {
		//                _activeHazeCore = FindObjectOfType<DS_HazeCore>();
		//                if (!_activeHazeCore)
		//                {
		//                    GameObject hazeController = Instantiate(DefaultHazeController, Vector3.zero, Quaternion.identity);
		//                    hazeController.name = "DZ_HazeController";
		//                    hazeController.transform.parent = MySettings.DynamicObjects;
		//                    _activeHazeCore = hazeController.GetComponent<DS_HazeCore>();
		//                }
		//            }
		//        }

		MyController.TriggerOnWorldInitialized();

		// Reset player position
		ResetPlayer();
	}

	public void ResetCameraPosition()
	{
		if (!MyController || !CoreCamera.Instance)
		{
			return;
		}

		CoreCamera.Instance.transform.position = Vector3.up * 1;
		CoreCamera.Instance.transform.rotation = Quaternion.identity;

		if (CoreFloatingOrigin.Instance)
		{
			CoreFloatingOrigin.Instance.CheckOriginOffset();
		}
		else
		{
			Debug.LogWarning("CoreWhirld :: ResetCameraPosition :: CoreFloatingOrigin.Instance not found");
		}
	}

	public void SetBasePosition()
	{
		var baseunitsAboveGround = BaseMetersAboveGround;

#if WRLD
        if (OnlineMaps.instance)
        {
            // If this is a "real terrain" world, we should start in the sky. With a jet.
            baseunitsAboveGround = BaseMetersAboveGroundTerrainWorld;
        }
#endif

		var basePos = Vector3.up * baseunitsAboveGround;

		var terrainHeight = CheckTerrainHeight(Vector2.zero);
		if (terrainHeight != null)
		{
			basePos.y = (float)terrainHeight + baseunitsAboveGround;
		}

		var baseObj = GameObject.Find("_Base");
		if (!baseObj)
		{
			Debug.Log("CoreWhirld :: SetBasePosition :: Creating Base @ " + basePos);

			var whirldBase = Instantiate(Resources.Load("_Base"), basePos, Quaternion.identity);
			whirldBase.name = "_Base";
			GameObject.Find("_Base").transform.parent = MySettings.DynamicObjects;
		}
		else if (!_customBase)
		{
			Debug.Log("CoreWhirld :: SetBasePosition :: Updating base position to " + basePos);
			baseObj.transform.position = basePos;
		}
		else
		{
			Debug.Log("CoreWhirld :: SetBasePosition :: World already contains custom base");
		}
	}

	public float? CheckTerrainHeight(Vector2 pos)
	{
		RaycastHit hit;
		if (Physics.SphereCast(new Vector3(pos.x, 1000000, pos.y), 0.1f, transform.up * -1, out hit, Mathf.Infinity, LayerMaskTerrain))
		{
			//            if (Plugins.ThreadWorker.IsWorking("MapMagic") && Mathf.Approximately(hit.point.y, hit.transform.position.y))
			//            {
			//                Debug.Log("CoreWhirld :: CheckTerrainHeight :: Detected terrain intersection @ " + hit.point.y + ". Ignoring, as the terrain appears to still be generating.");
			//                return null;
			//            }
			Debug.Log("CoreWhirld :: CheckTerrainHeight :: Detected terrain intersection @ " + hit.point.y + " (collider name: " + hit.transform.name + ")");
			return hit.point.y;
		}

		return null;
	}

	void OnWorldLoaded()
	{
		Debug.Log("CoreWhirld :: OnWorldLoaded");

		// Nest all objects inside loaded scene to facilitate resetting the global origin if player wanders too far
		// Don't nest the _Runtime object, because the bootstrap script will just make another copy of it 

		// Create a scene object container
		if (!MySettings.SceneObjects || MySettings.SceneObjects.Equals(null))
		{
			var sO = new GameObject("_World");
			MySettings.SceneObjects = sO.transform;
		}

		// Create a dynamic object container
		if (!MySettings.DynamicObjects || MySettings.DynamicObjects.Equals(null))
		{
			var dO = new GameObject("_Dynamic");
			MySettings.DynamicObjects = dO.transform;
			MySettings.DynamicObjects.parent = MySettings.SceneObjects;
		}

		var rootGOs = SceneManager.GetActiveScene().GetRootGameObjects();
		foreach (GameObject go in rootGOs)
		{
			if (go.name == "_Runtime" || go.transform == MySettings.SceneObjects)
			{
				continue;
			}

			go.transform.parent = MySettings.SceneObjects;
		}

		if (!WhirldData.Instance || !WhirldData.Instance.IsPlayableWorld)
		{
			// We just loaded the lobby
			Debug.Log("CoreWhirld :: OnWorldLoaded :: Loaded world doesn't appear to be playable, not spawning player vehicle");
			MyController.TriggerOnWorldInitialized();
			return;
		}

		// Controller Detection :: Time Management
		TimeControlEnabled = false;
#if TRUESKY
        _trueSky = FindObjectOfType<simul.trueSKY>();
        if (!TimeControlEnabled && _trueSky)
        {
            TimeControlEnabled = true;
        }
#endif
		//        _uSkyTimeline = FindObjectOfType<usky.uSkyTimeline>();
		//        if (!TimeControlEnabled && _uSkyTimeline)
		//        {
		//            TimeControlEnabled = true;
		//        }

		//        if (!TimeControlEnabled && EnviroSky.instance)
		//        {
		TimeControlEnabled = true;
		//        }

		// Entity Detection :: Predefined base location(s)
		_customBase = GameObject.Find("_Base") != null;

		// @todo:IMPORTFIXME
		// #if MAPMAGIC
		//         // MapMagic init
		//         if (MapMagic.MapMagic.instance != null)
		//         {
		//             // MM V1
		//             // _terrainDefaultViewDistance = MapMagic.MapMagic.instance.generateRange;
		//             // _terrainDefaultResolution = MapMagic.MapMagic.instance.resolution;

		//             // MM v2
		// 						_terrainDefaultViewDistance = MapMagic.MapMagic.instance.tiles.generateRange;
		// 						_terrainDefaultResolution = (int)MapMagic.MapMagic.instance.resolution;
		//         }
		// #endif

		RandomizeEverything(false);
	}

	void OnWorldInitialized()
	{
		Debug.Log("CoreWhirld :: OnWorldInitialized");

		UpdateDynamicOcclusionCulling(MySettings.SceneObjects);

		//        CoreCameraFade.FadeMaskOut();

		Status = CoreWhirldStatus.Good;
	}

	void OnQualityUpdate(int qualityLevel, bool isSubmerged, bool timeIsDaylight, bool worldIsLoaded)
	{
		if (MySettings.DynamicObjects != null)
		{
			UpdateDynamicOcclusionCulling(MySettings.DynamicObjects);
		}
	}

	void UpdateDynamicOcclusionCulling(Transform tr)
	{
		// // @todo finish this system
		// return 0;
		// var freeChildColliders = 0;
		// var updates = 0;
		// var go = tr.gameObject;

		// // Early-out if this object is already being culled
		// if (go.GetComponent<IOClod>() != null)
		// {
		//     return 0;
		// }

		// // Recursively process children
		// foreach (Transform ch in tr)
		// {
		//     // Child colliders are considered to be "free" if their associated GameObject does not have a mesh renderer.
		//     // This implies that they are compound colliders, and that their parent is the object which should be culled as a group.
		//     freeChildColliders += UpdateDynamicOcclusionCulling(ch);
		// }

		// if (go.GetComponent<Light>() != null)
		// {
		//     if (go.GetComponent<IOClight>() == null)
		//     {
		//         go.AddComponent<IOClight>();
		//         updates++;
		//     }
		// }
		// else if (go.GetComponent<Terrain>() != null)
		// {
		//     if (go.GetComponent<IOCterrain>() == null)
		//     {
		//         go.AddComponent<IOCterrain>();
		//         updates++;
		//     }
		// }
		// else
		// {
		//     if (freeChildColliders > 0 || go.GetComponent<Collider>() != null)
		//     {
		//         if ((freeChildColliders > 0 || go.GetComponent<MeshRenderer>() != null) && go.GetComponent<IOClod>() == null)
		//         {
		//             go.AddComponent<IOClod>();
		//             updates++;
		//         }
		//     }
		// }

		// if (updates > 0)
		// {
		//     Debug.Log("CoreWhirld :: UpdateDynamicOcclusionCulling :: " + go.name + ": " + updates + " updates");

		//     // Don't pass freeChildColliders to parent if this object is being culled as an independent object.
		//     return 0;
		// }

		// return freeChildColliders;
	}
}