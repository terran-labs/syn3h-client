using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
#if POST_FX
using UnityEngine.PostProcessing;
#endif
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public enum WhirldVegitationMode
{
    None,
    ATG,
}

[Serializable]
[ExecuteInEditMode]
public class WhirldData : MonoBehaviour
{
    // displayed in the messaging console for each player entering this world 
    public string Name;

    // displayed in the messaging console for each player entering this world 
    public string MessageWelcome;

    // if left blank, syn3h will spawn a random vehicle
    public string DefaultVehicle;

    // false for lobby, true for all other scenes. Non-playable worlds don't spawn a player vehicle, and the World camera locks to "AlignWithUi" mode
    public bool IsPlayableWorld = true;

    // false to disable atmospheric effects for worlds which use other conflicting postfx elements
    public bool EnableAtmospherics = true;

    // false to prevent world origin from automatically resetting to stay near game camera. This feature solves shadow and physics precision issues, but involves a performance penalty and breaks worlds which use static occlusion culling
    public bool EnableInfiniteOriginReset = true;

    // CoreMapMagic is driven by this setting to customize generated terrain tiles' vegitation systems 
    public WhirldVegitationMode CustomVegitationMode;

    // Render settings

    [HideInInspector] public bool RenderSettingsUpdated;
    [HideInInspector] public bool Fog;
    [HideInInspector] public FogMode FogMode;
    [HideInInspector] public Color FogColor;
    [HideInInspector] public float FogDensity;
    [HideInInspector] public float FogStartDistance;
    [HideInInspector] public float FogEndDistance;
    [HideInInspector] public AmbientMode AmbientMode;
    [HideInInspector] public Color AmbientSkyColor;
    [HideInInspector] public Color AmbientEquatorColor;
    [HideInInspector] public Color AmbientGroundColor;
    [HideInInspector] public Color AmbientLight;
    [HideInInspector] public float AmbientIntensity;
    [HideInInspector] public SphericalHarmonicsL2 AmbientProbe;
    [HideInInspector] public Color SubtractiveShadowColor;
    [HideInInspector] public float ReflectionIntensity;
    [HideInInspector] public int ReflectionBounces;
    [HideInInspector] public float HaloStrength;
    [HideInInspector] public float FlareStrength;
    [HideInInspector] public float FlareFadeSpeed;
    [HideInInspector] public Material Skybox;
    [HideInInspector] public Light Sun;
    [HideInInspector] public DefaultReflectionMode DefaultReflectionMode;
    [HideInInspector] public int DefaultReflectionResolution;
    [HideInInspector] public Cubemap CustomReflection;

    // Sky system 

    public float SkySunIntensity = 1;
    public float SkyMoonIntensity = .2f;
    public bool SkyCloudsCumulus = true;

    // Atmosphere settings

    //    [HideInInspector] public bool AtmosphereEnabled;
    //    [HideInInspector] public bool AtmosphereApplyAirToSkybox;
    //    [HideInInspector] public bool AtmosphereApplyFogToSkybox;
    //    [HideInInspector] public bool AtmosphereApplyHazeToSkybox;
    //    [HideInInspector] public bool AtmosphereApplyFogLightingToSkybox;

    // Misc environment settings

    public bool SeaEnabled;
    public float SeaAltitude;

    // Camera Settings

#if POST_FX
    [HideInInspector] public PostProcessingProfile PostProcessingProfile;
#endif

    [HideInInspector] public Transform[] CameraTransforms;

    // Singleton reference
    public static WhirldData Instance;

    // Update state tracking
    [NonSerialized] private const float MinUpdateTime = 1f;

    [NonSerialized] private double _lastUpdateTime;

    void Awake()
    {
        // If app is running, apply internal render settings TO scene
        if (Application.isPlaying)
        {
            CoreController.Bootstrap();
            StartCoroutine("ApplyDataToScene");
        }

        Instance = this;
    }

#if UNITY_EDITOR
    // If we are editing this scene, update render settings and other data at regular intervals
    void Update()
    {
        WhirldFeaturesUpdate();

        if (Application.isPlaying || !(EditorApplication.timeSinceStartup - _lastUpdateTime > MinUpdateTime))
        {
            return;
        }

        _lastUpdateTime = EditorApplication.timeSinceStartup;

        WhirldDataUpdate();
    }
#endif

    //    void OnDestroy()
    //    {
    //        Instance = null;
    //    }

    void WhirldDataUpdate()
    {
#if UNITY_EDITOR
        // Automatically break prefab instance to ensure properties we update are actually saved
        // @todo There is probably a solution which doesn't involve breaking the prefab connection, but this works fine for now.
        PrefabUtility.DisconnectPrefabInstance(gameObject);
#endif

        if (Name == "")
        {
            Name = SceneManager.GetActiveScene().name;
        }

        Fog = RenderSettings.fog;
        FogMode = RenderSettings.fogMode;
        FogMode = RenderSettings.fogMode;
        FogColor = RenderSettings.fogColor;
        FogDensity = RenderSettings.fogDensity;
        FogStartDistance = RenderSettings.fogStartDistance;
        FogEndDistance = RenderSettings.fogEndDistance;
        AmbientMode = RenderSettings.ambientMode;
        AmbientSkyColor = RenderSettings.ambientSkyColor;
        AmbientEquatorColor = RenderSettings.ambientEquatorColor;
        AmbientGroundColor = RenderSettings.ambientGroundColor;
        AmbientLight = RenderSettings.ambientLight;
        AmbientIntensity = RenderSettings.ambientIntensity;
        AmbientProbe = RenderSettings.ambientProbe;
        SubtractiveShadowColor = RenderSettings.subtractiveShadowColor;
        ReflectionIntensity = RenderSettings.reflectionIntensity;
        ReflectionBounces = RenderSettings.reflectionBounces;
        HaloStrength = RenderSettings.haloStrength;
        FlareStrength = RenderSettings.flareStrength;
        FlareFadeSpeed = RenderSettings.flareFadeSpeed;
        Skybox = RenderSettings.skybox;
        Sun = RenderSettings.sun;
        DefaultReflectionMode = RenderSettings.defaultReflectionMode;
        DefaultReflectionResolution = RenderSettings.defaultReflectionResolution;
        RenderSettingsUpdated = true;

				// @todo currently not working - Unity throws the following compile error:
				// "Assets/_Core/Scripts/Data/WhirldData.cs(179,28): error CS0266: Cannot implicitly convert type 'UnityEngine.Texture' to 'UnityEngine.Cubemap'. An explicit conversion exists (are you missing a cast?)"
				// Potential solution: http://docs.unity3d.com/2021.1/Documentation/ScriptReference/RenderSettings-customReflection.html
        // CustomReflection = RenderSettings.customReflection;

        var cameras = GetSceneCameras();
        var cameraTransforms = new List<Transform>();
#if POST_FX
        PostProcessingProfile = null;
#endif
        foreach (Camera cam in cameras)
        {
#if POST_FX
            var postfx = cam.GetComponent<PostProcessingBehaviour>();
            if (PostProcessingProfile == null && postfx && postfx.profile)
            {
//                Debug.Log("WhirldData :: Found Custom Postprocessing Profile (" + postfx.profile.name + ")");

                var _profile = Instantiate<PostProcessingProfile>(postfx.profile);
                PostProcessingProfile = _profile;
            }
#endif

            //            var hazeView = cam.GetComponent<DeepSky.Haze.DS_HazeView>();
            //            if (hazeView)
            //            {
            //                AtmosphereEnabled = true;
            //                AtmosphereApplyAirToSkybox = hazeView.ApplyAirToSkybox;
            //                AtmosphereApplyFogToSkybox = hazeView.ApplyFogExtinctionToSkybox;
            //                AtmosphereApplyFogLightingToSkybox = hazeView.ApplyFogLightingToSkybox;
            //                AtmosphereApplyHazeToSkybox = hazeView.ApplyHazeToSkybox;
            //            }
            //            else
            //            {
            //                AtmosphereEnabled = false;
            //            }

            cameraTransforms.Add(cam.transform);
        }

        CameraTransforms = cameraTransforms.ToArray();

        if (EnableInfiniteOriginReset)
        {
            var activeOcclusionAreas = FindObjectsOfType(typeof(OcclusionArea)) as OcclusionArea[];
            if (activeOcclusionAreas != null && activeOcclusionAreas.Length > 0)
            {
                Debug.Log("WhirldData :: Automatically disabling InfiniteOriginReset, as this world contains a static occlusion area.");
                EnableInfiniteOriginReset = false;
            }
        }

        //        var worldStringRepresentation = JsonUtility.ToJson(this);
        //        Debug.Log("WhirldData :: Update :: " + worldStringRepresentation);
        //        Debug.Log("WhirldData :: Update :: Skybox Name: " + Skybox.name);
    }

    IEnumerator ApplyDataToScene()
    {
        // Wait a frame before triggering the EventWorldDataApplied delegate, as the game may have just loaded and other components may stil be registering their delegate hooks
        // Note - we need to wait a frame PRIOR to updating scene and RenderSetting properties, rather than afterwards. This solves bugs which were caused by other code changing properties in the same frame, before TriggerOnWorldDataApplied hooks could read the proper values
        yield return null;

        //        var worldStringRepresentation = JsonUtility.ToJson(this);
        //        Debug.Log("WhirldData :: ApplyDataToScene :: " + worldStringRepresentation);

        // Disable any active cameras in scene
        var cameras = GetSceneCameras();
        foreach (Camera cam in cameras)
        {
            Debug.Log("Camera Found: " + cam.name);

            // Deactivate placeholder camera. Core Camera will take it from here
            cam.gameObject.SetActive(false);
        }

        // Ensure that we don't overwrite scene render setttings with null values 
        if (!RenderSettingsUpdated)
        {
            yield break;
        }

        RenderSettings.fog = Fog;
        RenderSettings.fogMode = FogMode;
        RenderSettings.fogColor = FogColor;
        RenderSettings.fogDensity = FogDensity;
        RenderSettings.fogStartDistance = FogStartDistance;
        RenderSettings.fogEndDistance = FogEndDistance;
        RenderSettings.ambientMode = AmbientMode;
        RenderSettings.ambientSkyColor = AmbientSkyColor;
        RenderSettings.ambientEquatorColor = AmbientEquatorColor;
        RenderSettings.ambientGroundColor = AmbientGroundColor;
        RenderSettings.ambientLight = AmbientLight;
        RenderSettings.ambientIntensity = AmbientIntensity;
        RenderSettings.ambientProbe = AmbientProbe;
        RenderSettings.subtractiveShadowColor = SubtractiveShadowColor;
        RenderSettings.reflectionIntensity = ReflectionIntensity;
        RenderSettings.reflectionBounces = ReflectionBounces;
        RenderSettings.haloStrength = HaloStrength;
        RenderSettings.flareStrength = FlareStrength;
        RenderSettings.flareFadeSpeed = FlareFadeSpeed;

        // @aubhere @todo need to prevent this from conflicting with Enviro
        //        RenderSettings.skybox = Skybox;
        //        RenderSettings.sun = Sun;

        RenderSettings.defaultReflectionMode = DefaultReflectionMode;
        RenderSettings.defaultReflectionResolution = DefaultReflectionResolution;

				// @todo - currently not working, see comment above inside the "WhirldDataUpdate" method
        //RenderSettings.customReflection = CustomReflection;

        if (CoreController.Instance)
        {
            //            Debug.Log("CoreSky :: ApplyDataToScene :: Assigned custom Skybox (post-yeild): " + RenderSettings.skybox.name);
            CoreController.Instance.TriggerOnWorldDataApplied();
        }
    }

    void WhirldFeaturesUpdate()
    {
#if HYDROFORM
        if (SeaEnabled && !CoreHydroform.Instance)
        {
            Transform parent = null;
            if (CoreSettings.Instance && CoreSettings.Instance.SceneObjects)
            {
                parent = CoreSettings.Instance.SceneObjects;
            }

            var go = Instantiate(Resources.Load("_Ocean"), parent);
            go.name = "_Ocean";
        }
#endif

        //        else if (Application.isPlaying && !SeaEnabled && CoreHydroform.Instance)
        //        {
        //            Destroy(CoreHydroform.Instance.gameObject);
        //        }
    }

    public Transform GetSceneRoot()
    {
        Transform sceneRoot = null;

        if (CoreController.Instance && CoreSettings.Instance.SceneObjects)
        {
            sceneRoot = CoreSettings.Instance.SceneObjects;
        }

        return sceneRoot;
    }

    public List<Camera> GetSceneCameras()
    {
        List<Camera> cameras = new List<Camera>();

        var camObjs = FindObjectsOfType(typeof(Camera)) as Camera[];
        if (camObjs == null)
        {
            return cameras;
        }

        foreach (Camera cam in camObjs)
        {
            // Return only cameras which were intentionally added to this scene 
            if (cam.gameObject.name == "Water Reflection Camera" || cam.transform.root.name == "_Core" || cam.transform.root.name == "_Runtime" ||
                cam.transform.root.name == "_Ocean" || cam.transform.root.name == "_Environment")
            {
                continue;
            }

            cameras.Add(cam);
        }

        return cameras;
    }
}