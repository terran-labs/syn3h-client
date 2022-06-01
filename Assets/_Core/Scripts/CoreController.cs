using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class CoreController : MonoBehaviour
{
    public static CoreController Instance { get; private set; }

    public GameObject[] Vehicles;
    public int DefaultVehicleIndex = 0;
    public int DefaultVehicleIndexTerrainWorld = 1;
    public CoreConfig MyConfig = new CoreConfig();
    public CoreSettings MySettings;
    public CoreLanguage MyLanguage;
    public CoreQualityChangeDetection QualityChangeDetection;
    public float StatUpdateFrequency = 1.0f;
    private readonly int _sceneLobbyIndex = 0;
    private bool _invocationFlagTriggerOnQualityUpdate;

    /*[System.NonSerialized] */
    public GameObject Player;

    /*[System.NonSerialized] */
    public Vhicl PlayerVhicl;

    [System.NonSerialized] public List<GameObject> PlayerBots = new List<GameObject>();

    public event EventWorldLoaded OnWorldLoaded;

    // Triggered when a world is loaded
    public delegate void EventWorldLoaded();

    public event EventWorldLoaded OnWorldUnloaded;

    public delegate void EventWorldUnloaded();

    // Triggered when a loaded world is fully initialized and ready to play
    public event EventWorldInitialized OnWorldInitialized;

    public delegate void EventWorldInitialized();

    // Triggered the frame that Whirld Data is applied. This provides a hook for initializing custom skyboxes/etc 
    public event EventWorldDataApplied OnWorldDataApplied;

    public delegate void EventWorldDataApplied();

    public event EventOriginReset OnOriginReset;

    public delegate void EventOriginReset();

    public event EventQualityChange OnQualityChange;

    public delegate void EventQualityChange(int qualityLevel);

    public event EventQualityUpdate OnQualityUpdate;

    public delegate void EventQualityUpdate(int qualityLevel, bool isSubmerged, bool timeIsDaylight, bool worldIsLoaded);

    public event EventGameSparksRegistered OnGameSparksRegistered;

    public delegate void EventGameSparksRegistered();

    public bool IsGameSparksAlreadyRegistered { get; private set; }

    // public Reporter StatsLogger;

    // public StatsMonitor.StatsMonitorWidget StatsMonitor;

    void Awake()
    {
        // Suicide in case there is already an active Core object.
        // This can occur if we are testing, and leave a _Controller prefab in the lobby scene for convenience 
        if (Instance != null)
        {
            Debug.Log("CoreController :: Detected existing instance, self-destruct initiated...");
            DestroyImmediate(this.gameObject, false);
            return;
        }

        Instance = this;
        
        // Immortalize ourself
        DontDestroyOnLoad(gameObject);
    }

    private void OnDisable()
    {
        // Remove all bots during live script reload, since we will loose references to them anyway
        RemoveAllBots();
    }

    void Start()
    {
        // World init
        StartCoroutine(_worldLoaded());

        // Analytics Init
        Analytics.CustomEvent("Core.initController", new Dictionary<string, object>
        {
            {"Version", Application.version},
            {"Platform", Application.platform.ToString()},
            {"UnityVersion", Application.unityVersion}
        });

        // Stats init
        InvokeRepeating("UpdateStats", StatUpdateFrequency, StatUpdateFrequency);
    }

    public static void Bootstrap()
    {
        Debug.Log("CoreController :: Bootstrap :: Initiated");
        
        // Core should be present in all scenes, and will survive scene changes
        if (!Instance) /*Resources.FindObjectsOfTypeAll(typeof(CoreController)).Length == 0*/
        {
            Debug.Log("CoreController :: Bootstrap :: Instantiating core");

            var core = Instantiate(Resources.Load("_Controller"));
            core.name = "_Controller";
        }

        
//        GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>() ;
//        foreach(object go in allObjects)
////            if (go.activeInHierarchy)
//                Debug.Log(go +" is an active object") ;
        
        // Runtime doesn't survive scene changes. Recreate as necessary
        if (!CoreRuntime.Instance) /*Resources.FindObjectsOfTypeAll(typeof(CoreRuntime)).Length == 0*/
        {
            Debug.Log("Core Controller :: Bootstrap :: No _Runtime found - instantiating");

            var runtimeObj = Instantiate(Resources.Load("_Runtime"));
            runtimeObj.name = "_Runtime";
        }
    }

    void UpdateStats()
    {
        if (PlayerVhicl && PlayerVhicl.MyRigidbody)
        {
            MySettings.PlayerVelocity = PlayerVhicl.MyRigidbody.velocity.magnitude;
            MySettings.PlayerAltitude = PlayerVhicl.transform.position.y;
            MySettings.PlayerOriginDistance = (PlayerVhicl.transform.position - MySettings.SceneObjects.position).magnitude;
        }
        else
        {
            MySettings.PlayerVelocity = 0;
            MySettings.PlayerAltitude = 0;
            MySettings.PlayerOriginDistance = 0;
        }

//		Debug.Log("CoreController :: UpdateStats (IsWorldLoaded: " + IsWorldLoaded + ")");
    }

    void Update()
    {
        if (Input.GetButtonDown("Freeze"))
        {
            MySettings.FreezeMotion = !MySettings.FreezeMotion;
        }

        // Invocation flags :: Quality change triggering
        if (_invocationFlagTriggerOnQualityUpdate)
        {
//            Debug.Log("CoreController :: Update :: invocationFlagTriggerOnQualityUpdate triggered");
            _invocationFlagTriggerOnQualityUpdate = false;
            _triggerOnQualityUpdate();
        }
    }

    void FixedUpdate()
    {
        // Prevent player's vehicle from freezing in midair
        if (Player != null)
        {
            if (!PlayerVhicl.MyRigidbody.isKinematic && PlayerVhicl.MyRigidbody.IsSleeping())
            {
                PlayerVhicl.MyRigidbody.WakeUp();
//                PlayerVhicl.MyRigidbody.AddForce(Vector3.down * 10f, ForceMode.VelocityChange);
                Debug.Log("CoreController :: Update :: Player.MyRigidbody was sleeping. WakeUp() initiaited.");
            }
        }
    }

    public void LoadScene(string sceneName)
    {
        if (sceneName == "")
        {
            Debug.LogError("CoreController :: LoadScene :: Scene name is blank. Aborting.");
            return;
        }

        // @todo this safety net doesn't appear to be necessary, as CoreWhirld.Instance.Status should be "Loading" if a world is being loaded
//        if (SceneManager.sceneCount != 1)
//        {
//            Debug.LogError("CoreController :: LoadScene :: world load attempted with an active world already loaded. Aborting.");
//            return;
//        }

        if (CoreWhirld.Instance.Status == CoreWhirldStatus.Loading || CoreWhirld.Instance.Status == CoreWhirldStatus.LoadingHardReset)
        {
            Debug.LogError("CoreController :: LoadScene :: Scene is already loading. Aborting.");
            return;
        }

        StartCoroutine(_loadScene(sceneName));
    }

    public void LoadWorld(string url)
    {
        Debug.LogError("CoreController :: LoadWorld :: Not yet implemented");
    }

    public void UnloadWorld()
    {
//        string lobbyName = SceneManager.GetSceneByBuildIndex(_sceneLobbyIndex).name;
        string lobbyName = "_Lobby";

        Debug.Log("CoreController :: UnloadWorld :: Target: " + lobbyName);

        StartCoroutine(_loadScene(lobbyName, true));
//        StartCoroutine(_unloadWorld());
    }

//    public float GetWhirldStatus()
//    {
//        // 0 = chilling in lobby
////        if (SceneManager.GetActiveScene().name == _sceneLobbyName)
////        {
////            return 0;
////        }
//
//        // > 0 && < 1 = World loading, show progress bar
//        if (SceneUpdating != null && SceneUpdating.isDone != true)
//        {
//            // Scene loading progress returns 0 while the editor is initializing a scene.
//            if (SceneUpdating.progress == 0)
//            {
//                return 0.01f;
//            }
//
//            return SceneUpdating.progress;
//        }
//
//        // 1 = Custom world loaded, lobby should not be rendered
//        return 1;
//    }

    // Unload active world, transition back to Lobby
//    IEnumerator _unloadWorld()
//    {
//        CoreCameraFade.FadeMaskIn();
//
//        yield return new WaitForSeconds(CoreCameraFade.TransitionTime);
//
//        if (OnWorldUnloaded != null)
//        {
//            OnWorldUnloaded();
//        }
//
//        // destroy runtime object
//        var runtime = GameObject.Find("/_Runtime");
//        if (runtime)
//        {
//            Debug.Log("Core Controller :: _unloadWorld :: tearing down runtime");
//            Destroy(runtime);
//        }
//
//        IsWorldLoaded = false;
//
//        // Unload active world and load Lobby. Core has already immmortalized itself via DontDestroyOnLoad, so we don't need to bootstrap it again.
//        SceneManager.LoadScene(_sceneLobbyIndex);
//
//        yield return true;
//
//        if (OnWorldLoaded != null)
//        {
//            OnWorldLoaded();
//        }
//    }

    // Load new scene, by name
    // hardReset is used when loading the lobby. When true, it prevents the "world loading" banner from appearing
    // and also ensures the _Controller is reconstructed from scratch rather than persisting indefinitley and possibly causing issues.
    IEnumerator _loadScene(string sceneName, bool hardReset = false)
    {
        Debug.Log("CoreController :: _loadScene :: Loading: " + sceneName + " (hardReset: " + hardReset + ")...");

        CoreWhirld.Instance.Progress = 0;
        if (hardReset)
        {
            CoreWhirld.Instance.Status = CoreWhirldStatus.LoadingHardReset;
        }
        else
        {
            CoreWhirld.Instance.Status = CoreWhirldStatus.Loading;
        }

        CoreCameraFade.FadeMaskIn();

//        Scene oldScene = SceneManager.GetActiveScene();
//        Scene newScene = SceneManager.GetSceneByName(sceneName);
//        Camera oldMainCamera = Camera.main; // we need this later
//        oldMainCamera.tag = "Untagged";

        yield return new WaitForSeconds(CoreCameraFade.TransitionTime);

        AsyncOperation operationLoadScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
//        operationLoadScene.allowSceneActivation = false;
//        CoreWhirld.Instance.Progress = 0.01f;

//        // Deactivate contents of previous scene
//        // Leave the scene itself 'till we have the new scene loaded so Unity doesn't freak out
//        foreach (var go in oldScene.GetRootGameObjects())
//        {
//            go.SetActive(false);
//        }

        // Load new scene
        while (!operationLoadScene.isDone) // note: this will crash at 0.9 if you're not careful
        {
            CoreWhirld.Instance.Progress = operationLoadScene.progress;

//            if (operationLoadScene.progress.Equals(0.9f))
//            {
//                CoreWhirld.Instance.Progress = 1;
//                operationLoadScene.allowSceneActivation = true;
//                yield return operationLoadScene;
//            }

            yield return null;
        }

        Debug.Log("CoreController :: _loadScene :: loaded...");

        // Unload previous scene
//        AsyncOperation operationUnloadScene = SceneManager.UnloadSceneAsync(oldScene);
//        while (!operationLoadScene.isDone)
//        {
//            CoreWhirld.Instance.Progress = 0.9f + (operationUnloadScene.progress / 10);
//
//            yield return null;
//        }

        // Free unused resources
        Resources.UnloadUnusedAssets();

        Debug.Log("CoreController :: _loadScene :: prev scene unloaded...");

//        SceneManager.SetActiveScene(newScene);

        // If we are transitioning back to Lobby, destroy the Controller and start over
        if (hardReset)
        {
            var runtime = GameObject.Find("/_Runtime");
            DestroyImmediate(runtime);
//            CoreController.Bootstrap();
        }

        StartCoroutine(_worldLoaded());
    }

    IEnumerator _worldLoaded()
    {
        // Provide the _Runtime object a chance to be detectable in the scene. (@dragonhere: This shouldn't be necessary)
        yield return true;
        
        // Ensure that a _Runtime is present for each new world (and Lobby)
        Bootstrap();

        CoreWhirld.Instance.Status = CoreWhirldStatus.Good;

        // Provide world elements a chance to initialize and register OnWorldLoaded delegate hooks 
        yield return true;

        if (OnWorldLoaded != null)
        {
            OnWorldLoaded();
        }

        Analytics.CustomEvent("core.loadWorld", new Dictionary<string, object>
        {
            {"name", (WhirldData.Instance && WhirldData.Instance.Name != "" ? WhirldData.Instance.Name : "unknown")}
        });

        CoreCameraFade.FadeMaskOut();
    }

    public void TriggerOnWorldInitialized()
    {
        if (WhirldData.Instance && WhirldData.Instance.IsPlayableWorld)
        {
            Debug.Log("CoreController :: Scene " + SceneManager.GetActiveScene().name + " initialized");
        }
        else
        {
            Debug.Log("CoreController :: Lobby initialized");
        }

        if (OnWorldInitialized != null)
        {
            OnWorldInitialized();
        }

        TriggerOnQualityUpdate();
    }

    public void TriggerOnWorldDataApplied()
    {
        Debug.Log("CoreController :: TriggerOnWorldDataApplied");

        if (OnWorldDataApplied != null)
        {
            OnWorldDataApplied();
        }
    }

    public void TriggerOnQualityChange()
    {
        var qualityLevel = QualitySettings.GetQualityLevel();

        Debug.Log("CoreController :: Triggering TriggerOnQualityChange... " +
                  "(qualityLevel: " + qualityLevel + ")");

        if (OnQualityChange != null)
        {
            OnQualityChange(qualityLevel);
        }
    }

    public void TriggerOnQualityUpdate()
    {
        // Debounce system to ensure we don't call with the same settings multiple times in a row
        // Even if multiple TriggerOnQualityUpdate() calls are made each frame, OnQualityUpdate will be triggered once, and on the following frame.
        // This will occur whether or not time is paused.
        // This approach solves many problems, including the case fo our primary camera entering and exiting water in cinema mode with a motion freeze activated.
        _invocationFlagTriggerOnQualityUpdate = true;
//        Debug.Log("CoreController :: TriggerOnQualityUpdate() activated");
    }


    public void TriggerOnGameSparksRegistered()
    {
        if (OnGameSparksRegistered != null)
        {
            OnGameSparksRegistered();
        }

        IsGameSparksAlreadyRegistered = true;
    }

    private void _triggerOnQualityUpdate()
    {
        var qualityLevel = QualitySettings.GetQualityLevel();
        var isSubmerged = CoreCamera.Instance && CoreCamera.Instance.IsSubmerged;
        var isDaylight = CoreWhirld.Instance && CoreWhirld.Instance.TimeIsDaylight;
        var isPlayableWorld = WhirldData.Instance && WhirldData.Instance.IsPlayableWorld;

        Debug.Log("CoreController :: Triggering OnQualityUpdate... " +
                  "(qualityLevel: " + qualityLevel +
                  ", isSubmerged: " + isSubmerged +
                  ", isWorldLoaded: " + isPlayableWorld +
                  ", isDaylight: " + isDaylight + ")");

        if (OnQualityUpdate != null)
        {
            OnQualityUpdate(qualityLevel, isSubmerged, isDaylight, isPlayableWorld);
        }
    }

    public void DestroyVehicle()
    {
        Debug.Log("CoreController :: DestroyVehicle");

        if (Player)
        {
            Destroy(Player);
        }

        Player = null;
        PlayerVhicl = null;
    }

    public void LoadVehicle(int vehicleIndex, bool isPlayer = true)
    {
        StartCoroutine(_loadVehicle(vehicleIndex, isPlayer));
    }

    private IEnumerator _loadVehicle(int vehicleIndex, bool isPlayer = true)
    {
        if (!WhirldData.Instance || !WhirldData.Instance.IsPlayableWorld)
        {
            Debug.Log("CoreController :: LoadVehicle :: world not loaded, aborting");
            yield break;
        }

//        while (!IsWorldLoaded)
//        {
//            Debug.Log("CoreController :: LoadVehicle :: waiting for world load...");
//            yield return new WaitForSeconds(.25f);
//        }

        var spawnVehiclePosition = Vector3.zero;
        var spawnVehicleVelocity = Vector3.zero;
        var spawnVehicleRotation = Quaternion.identity;

        if (isPlayer)
        {
            if (Player && PlayerVhicl)
            {
                spawnVehiclePosition = Player.transform.position;
                spawnVehicleVelocity = PlayerVhicl.MyRigidbody.velocity;
                spawnVehicleRotation = PlayerVhicl.transform.rotation;
                DestroyVehicle();
            }

            Debug.Log("CoreController :: LoadVehicle :: Loading vehicle " + vehicleIndex);

            MySettings.FreezeMotion = false;
        }

        // Find spawn point
        if (spawnVehiclePosition == Vector3.zero)
        {
            GameObject baseObject = GameObject.Find("_Base");
            if (baseObject)
            {
                Transform baseTransform = baseObject.transform;
                spawnVehiclePosition = baseTransform.position;
                spawnVehicleRotation = baseTransform.rotation;

                // Ensure we don't spawn inside another player
                while (Physics.CheckSphere(spawnVehiclePosition, 3))
                {
                    spawnVehiclePosition += Vector3.up;
                }
            }
        }

        // Instantiate Vehicle
        GameObject vehObj = Instantiate(Vehicles[vehicleIndex], spawnVehiclePosition, spawnVehicleRotation);
        vehObj.name = Vehicles[vehicleIndex].name;
        vehObj.transform.parent = MySettings.DynamicObjects;
        Vhicl veh = vehObj.GetComponent<Vhicl>();
        var rigidBody = vehObj.GetComponent<Rigidbody>();
        rigidBody.velocity = spawnVehicleVelocity;
        veh.IsPlayer = isPlayer;
        veh.IsBot = !isPlayer;
        veh.Init(this, MySettings);

        // Update Internal Linkages
        if (isPlayer)
        {
            Player = vehObj;
            PlayerVhicl = veh;

            Debug.Log("CoreController :: LoadVehicle :: Vehicle " + vehObj.name + " loaded");
        }
        else
        {
            PlayerBots.Add(vehObj);

            Debug.Log("CoreController :: LoadVehicle :: Bot " + vehObj.name + " added");
        }

        QualityChangeDetection.SceneUpdated();

        Analytics.CustomEvent("core.loadVehicle", new Dictionary<string, object>
        {
            {"name", vehObj.name}
        });
    }

    public bool AddBot()
    {
        LoadVehicle(0, false);

        return true;
    }

    public bool RemoveBot()
    {
        if (PlayerBots.Count <= 0)
        {
            return false;
        }

        if (PlayerBots.Count == 1)
        {
            Resources.UnloadUnusedAssets();
        }

        Destroy(PlayerBots[0]);
        PlayerBots.RemoveAt(0);

        return true;
    }

    public void RemoveAllBots()
    {
        while (true)
        {
            if (!RemoveBot())
            {
                break;
            }
        }
    }

    public void InstallUpdate()
    {
        Application.OpenURL(MyConfig.InstallerAuto);
    }
}