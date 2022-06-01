using System;
// using Hydroform;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public enum CoreCameraMode
{
    None, // (used in Lobby) this serves as a default value for WorldData.ForceCameraMode
    AlignWithUi, // (used in Lobby) tracks position of UI camera, which orbits smoothly with cursor position and accelerometer inputs
    Spawning, // spin around player when entering world, paused, or otherwise frozen
    First, // first-person perspective
    Scope, // zoomed first-person perspective
    Third, // third-person perspective
    Stationary, // fixed position, watches player
    Roam // free-roaming
}

[RequireComponent(typeof(Camera))]
public class CoreCamera : MonoBehaviour
{
    public static CoreCamera Instance { get; private set; }

    [NonSerialized] public Camera ThisCamera;
    [NonSerialized] public CoreSettings MySettings;

    private Transform _uiCameraTransform;
    public LayerMask LayerMaskClipAvoidance;
    public CoreCameraMode TargetMode { get; private set; } // Type of movement logic
    public Vector3 TargetPosition { get; private set; } // Position of object the camera is following
    public float TargetDistance { get; private set; } // Desired distance of camera from target position.
    public float TargetRotateX { get; private set; } // Desired horizontal rotation of camera in euler angles around target at given offset
    public float TargetRotateY { get; private set; } // Desired vertial rotation of camera in euler angles around target at given offset
    public bool TargetRotateTracking { get; private set; }
    public bool TargetRotateAuto { get; private set; }
    public float TargetHeight;
    public float RotationSmoothingClose = 1f;
    public float RotationSmoothingFar = .25f;
    public float HorizonSmoothing = 20;
    public float PositionSmoothing = 4;
    public float VelocitySmoothing = 0.25f;
    public float DistanceSmoothing = 4;
    public float VerticalBumpSmoothing = 0.25f;
    public float LookRotationMultiplier = 4;
    public float SpawnRotationSpeed = 4;
    public float LookZoomRotationMultiplier = .75f;
    public float ClipCollisionCheckRadius = 1.0f;
    public int ClipCollisionMaxSolverIterations = 10;
    public float RoamMoveSpeed = 5.0f;
    public float RoamMoveSpeedMin = 1.0f;
    public float RoamMoveSpeedMax = 10.0f;
    public int FarClipMin = 1000;

    public int FarClipMaxProcedural = 3000;

    //    public int FarClipMaxRealTerrain = 13000;
    public float ClipPlaneAltitudeMultiplier = 3;

    private int _farClipBase;
    public float RoamRunMultiplier = 10;
    public float SensitivityX = 15.0f;
    public float SensitivityY = 15.0f;
    public float DelayAutoRotateAfterManual = 3.0f;
    public float VerticalBumpHeighToDistRato = .15f;
    public float SensitivityDistance = 10.0f;
    public float MinDistance = 1f;
    public float MaxDistance = 25f;
    public float SensitivityScope = 2.5f;
    public float MinScope = 1.5f;
    public float MaxScope = 5f;
    public float MinTerrainProximity = 1f;
    public float MaxMoveTerrainCheck = 1f;
    public float DistanceVelocityMultiplier = 0.01f;
    public float OrbitX = 25.0f;
    public float FovDefault = 80.0f;
    public float FovFps = 70.0f;
    public float FovSmoothing = 3;
    public float AutoRotationSmoothing = 3;
    public float AutoRotationDeltaSmoothing = 3;
    public float AutoRotationDeltaMax = 1;
    public float AutoRotationRideSmoothing = 5;
    public float AutoRotationYDamping = .25f;
    public float TimeForceSpawnMode = 5f;
    public float PeriodicUpdateInterval = .5f;

    public float FovTarget { get; private set; }
    public float InputOrbitDistance { get; private set; }
    public float InputScopeZoom { get; private set; }
    public float Velocity { get; private set; }
    public float VelocitySmoothed { get; private set; }
    public float DistanceSmoothed { get; private set; }
    public bool UseFirstPersonPerspective { get; private set; }

    public float fogDensity;
    public float fogDensitySubmerged;
    public float fogDensitySubmergedDeep;
    public float ImmersionSurfaceDistance; // distance from water at which we begin to register an ImmersionSurfaceFactor
    public float SubmersionDeepDepth;
    public float ImmersionSurfaceFactor { get; private set; } // ImmersionSurfaceFactor is 1 at water's surface, and 0 at ImmersionSurfaceDistance distance from surface
    public float SubmersionDeepFactor { get; private set; }

    public bool IsSubmerged { get; private set; }

    private bool _lastDisablePostFX;

    //    private bool _lastDisableSkyFX;
    private bool _lastDisableAtmospherics;

    private bool _lastDisableVisualDOF;
    private bool _lastDisableAA;
    private bool _lastDisableAO;
    private bool _lastDisableSSS;
    private bool _lastDisableWater;
    private float _lastManualRotationUpdate;
    private CoreCameraMode _lastTargetMode;
    private Vector3 _lastTargetPosition;
    private Vector3 _lastPlayerPosition;
    private CoreSkyMode _lastSkyMode;
    private float _worldLoadTime;
    private Vector3 _worldEntryPosition;
    private Quaternion _worldEntryLookRotation;
    private Vector3 _roamMovement;
    private float _verticalBump;
    private float _autoRotationFloatingAngularDeltaLimitX;

    //    private UltimateWater.WaterCamera _PlayWayWaterCamera;
    //    private UltimateWater.WaterCameraIME _PlayWayWaterCameraIME;
    //    private DeepSky.Haze.DS_HazeView _DeepSkyHazeView;

    //    private EnviroSkyRendering _EnviroSkyRendering;
    public PostProcessVolume _postProcessOverrides;
    private PostProcessLayer _postProcessLayer;
    private DynamicRenderingQuality _dynamicRenderingQuality;
    // private SEScreenSpaceShadows _screenSpaceShadows;
    // private HydroformComponent OceanPrefab;

#if TRUESKY
    [NonSerialized] public simul.trueSKY MyTrueSky;
    [NonSerialized] public simul.TrueSkyCamera MyTrueSkyCamera;
#endif

#if POST_FX
// PostProcessing Profile - cloned from Core Core Default
    private PostProcessingProfile _postProcessingProfileDefault;
#endif

    private bool _flagTriggerUpdateFxComponents;

    void OnEnable()
    {
        Debug.Log("CoreCamera :: Enabled");

        Instance = this;

        ThisCamera = GetComponent<Camera>();
        MySettings = FindObjectOfType<CoreSettings>();

        //        _PlayWayWaterCamera = GetComponent<UltimateWater.WaterCamera>();
        //        _PlayWayWaterCameraIME = GetComponent<UltimateWater.WaterCameraIME>();
        //        _DeepSkyHazeView = GetComponent<DeepSky.Haze.DS_HazeView>();

#if POST_FX
        _postProcessingBehaviour = GetComponent<PostProcessingBehaviour>();
        _postProcessingProfileDefault = Instantiate(_postProcessingBehaviour.profile);
#endif
        _postProcessLayer = GetComponent<PostProcessLayer>();
        _dynamicRenderingQuality = GetComponent<DynamicRenderingQuality>();
        //        _uSkyLightShafts = GetComponent<usky.uSkyLightShafts>();
        // _screenSpaceShadows = GetComponent<SEScreenSpaceShadows>();
        //        _ambientOcclusion = GetComponent<AmplifyOcclusionEffect>();

#if TRUESKY
        MyTrueSky = FindObjectOfType<simul.trueSKY>();
        MyTrueSkyCamera = GetComponent<simul.TrueSkyCamera>();
#endif

        InputOrbitDistance = Mathf.Lerp(MinDistance, MaxDistance, .25f);
        InputScopeZoom = Mathf.Lerp(MinScope, MaxScope, .5f);

        CoreCameraFade.FadeMaskOut();

        if (!CoreController.Instance)
        {
            Debug.LogError("CoreCamera :: OnEnable :: No controller detected");
            return;
        }

        CoreController.Instance.OnWorldInitialized += OnWorldInitialized;
        CoreController.Instance.OnQualityUpdate += OnQualityUpdate;
    }

    void Start()
    {
        InvokeRepeating("_periodicUpdate", 0, PeriodicUpdateInterval);
        //        InvokeRepeating("DetectRunawayCommandBuffers", 2.5f, 2.5f);
    }

    void OnDisable()
    {
        if (!CoreController.Instance)
        {
            //            Debug.LogError("CoreCamera :: OnDestroy :: No controller detected");
            return;
        }

        CoreController.Instance.OnWorldInitialized -= OnWorldInitialized;
        CoreController.Instance.OnQualityUpdate -= OnQualityUpdate;
    }

    //    private void DetectRunawayCommandBuffers()
    //    {
    //        if (ThisCamera.GetCommandBuffers(CameraEvent.AfterForwardAlpha).Length > 8)
    //        {
    //            ThisCamera.RemoveAllCommandBuffers();
    //            Debug.LogError(
    //                "CoreCamera :: DetectRunawayCommandBuffers :: There were more than 8 active AfterForwardAlpha command buffers. TrueSky is most likely to blame. Crises averted for now");
    //        }
    //    }

    void OnQualityUpdate(int qualityLevel, bool isSubmerged, bool timeIsDaylight, bool worldIsLoaded)
    {
        Debug.Log("CoreCamera :: OnQualityUpdate :: Triggered (isSubmerged: " + isSubmerged + ")");

        // Scene Fog
        RenderSettings.fog = false;
        //        RenderSettings.fogMode = FogMode.Exponential;
        //        if (!IsSubmerged)
        //        {
        //            RenderSettings.fogColor = RenderSettings.ambientSkyColor;
        //            RenderSettings.fogDensity = fogDensity;
        //        }
        //        else
        //        {
        //            RenderSettings.fogColor = Color.Lerp(RenderSettings.ambientSkyColor, Color.black, SubmersionDeepFactor);
        //            RenderSettings.fogDensity = Mathf.Lerp(fogDensitySubmerged, fogDensitySubmergedDeep,
        //                SubmersionDeepFactor);
        //        }


        // Activate appropriate PostProcessing effects
        _postProcessLayer.antialiasingMode =
            qualityLevel >= 3 && !CoreSettings.Instance.DisableAA ? PostProcessLayer.Antialiasing.TemporalAntialiasing : PostProcessLayer.Antialiasing.None;
        //        _postProcessOverrides.profile.AddSettings()
#if POST_FX
//        _postProcessingBehaviour.profile.ambientOcclusion.enabled = false; // Using third-party VAO lib for now
        _postProcessingBehaviour.profile.ambientOcclusion.enabled = qualityLevel >= 4 && !CoreSettings.Instance.DisableAO;
        _postProcessingBehaviour.profile.screenSpaceReflection.enabled = qualityLevel >= 5;
        _postProcessingBehaviour.profile.depthOfField.enabled =
            qualityLevel >= 3 && !isSubmerged && !CoreSettings.Instance.DisableVisualDOF && TargetMode == CoreCameraMode.Scope;
        _postProcessingBehaviour.profile.motionBlur.enabled = qualityLevel >= 4;
#if TRUESKY
        _postProcessingBehaviour.profile.eyeAdaptation.enabled = qualityLevel >= 3 && !MyTrueSky; // TrueSky just looks better without it
#else
        _postProcessingBehaviour.profile.eyeAdaptation.enabled = qualityLevel >= 3;        
#endif
        _postProcessingBehaviour.profile.bloom.enabled = qualityLevel >= 3 /* && !_trueSky */; //     TrueSky clouds and sun look pixely with bloom enabled
        _postProcessingBehaviour.profile.colorGrading.enabled = qualityLevel >= 1;
        _postProcessingBehaviour.profile.userLut.enabled = false;
        _postProcessingBehaviour.profile.chromaticAberration.enabled = qualityLevel >= 3;
        _postProcessingBehaviour.profile.grain.enabled = false;
        _postProcessingBehaviour.profile.vignette.enabled = qualityLevel >= 3;

        // Customize :: Activated effects
        var fxAaSettings = _postProcessingBehaviour.profile.antialiasing.settings;
        fxAaSettings.method = qualityLevel < 4 ? AntialiasingModel.Method.Fxaa : AntialiasingModel.Method.Taa;
        _postProcessingBehaviour.profile.antialiasing.settings = fxAaSettings;

        // Smart DoF customization
        var fxDofSettings = _postProcessingBehaviour.profile.depthOfField.settings;
        fxDofSettings.focusDistance = TargetMode == CoreCameraMode.Scope ? 1000 : 13;
        _postProcessingBehaviour.profile.depthOfField.settings = fxDofSettings;
#endif
    }

    void OnPreCull()
    {
        // Hydroform Ocean System

        // if (WhirldData.Instance && WhirldData.Instance.SeaEnabled)
        // {
        //     if (OceanPrefab == null)
        //     {
        //         OceanPrefab = (HydroformComponent) FindObjectOfType(typeof(HydroformComponent));
        //     }

        //     if (OceanPrefab && Application.isPlaying)
        //     {
        //         OceanPrefab.UpdateCamData();
        //         OceanPrefab.UpdateReflection();
        //         OceanPrefab.UpdateUnderwaterCam();
        //         OceanPrefab.RenderVolumeMasks();
        //         OceanPrefab.DrawMeshes(ThisCamera);
        //     }
        // }
    }

    void _periodicUpdate()
    {
        // Camera Far Clip Plane :: procedural terrain
        var farClipMax = FarClipMaxProcedural;
        //            if (OnlineMaps.instance != null)
        //            {
        //                farClipMax = FarClipMaxRealTerrain;
        //            }
        _farClipBase = (int)Mathf.Lerp(FarClipMin, farClipMax, QualitySettings.GetQualityLevel() / 5f);

        // Camera Far Clip Plane :: real-world terrain
        // if (CoreGeoEngine.MapScale > 0)
        // {
        //     var baseTerrainViewDist = Mathf.FloorToInt(CoreGeoEngine.MapScale);
        //     if (_farClipBase < baseTerrainViewDist)
        //     {
        //         _farClipBase = baseTerrainViewDist;
        //     }
        // }
    }

    void Update()
    {
        // Force-disable primary post fx stack as desired
        if (false
            || _flagTriggerUpdateFxComponents
            || _lastDisableWater != CoreSettings.Instance.DisableWater
            || _lastDisablePostFX != CoreSettings.Instance.DisableAdvancedPostFX
            //            || _lastDisableSkyFX != CoreSettings.Instance.DisableSkyFX
            || _lastDisableAtmospherics != CoreSettings.Instance.DisableAtmospherics
            || _lastDisableVisualDOF != CoreSettings.Instance.DisableVisualDOF
            || _lastDisableAA != CoreSettings.Instance.DisableAA
            || _lastDisableAO != CoreSettings.Instance.DisableAO
            || _lastDisableSSS != CoreSettings.Instance.DisableSSS
            || _lastSkyMode != CoreSettings.Instance.SkyMode
        )
        {
            _lastDisableWater = CoreSettings.Instance.DisableWater;
            _lastDisablePostFX = CoreSettings.Instance.DisableAdvancedPostFX;
            //            _lastDisableSkyFX = CoreSettings.Instance.DisableSkyFX;
            _lastDisableAtmospherics = CoreSettings.Instance.DisableAtmospherics;
            _lastDisableVisualDOF = CoreSettings.Instance.DisableVisualDOF;
            _lastDisableAA = CoreSettings.Instance.DisableAA;
            _lastDisableAO = CoreSettings.Instance.DisableAO;
            _lastDisableSSS = CoreSettings.Instance.DisableSSS;
            _lastSkyMode = CoreSettings.Instance.SkyMode;

            UpdateFxComponents();
        }

        if (Cursor.lockState == CursorLockMode.Locked)
        {
            // Scrollwheel distance/scope zoom
            if (Input.GetButton("Scope"))
            {
                InputScopeZoom = Mathf.Clamp(InputScopeZoom + (Input.GetAxis("Distance") * SensitivityScope), MinScope, MaxScope);
            }
            else
            {
                InputOrbitDistance = Mathf.Clamp(InputOrbitDistance + (Input.GetAxis("Distance") * SensitivityDistance), MinDistance, MaxDistance);
            }
        }

        // UseFirstPersonPerspective toggling
        //		if (Input.GetButtonDown("Perspective"))
        //		{
        //			UseFirstPersonPerspective = !UseFirstPersonPerspective;
        //		}
        //		else if (Input.GetButtonUp("Fire2"))
        //		{
        //			UseFirstPersonPerspective = false;
        //		}
        if (Input.GetButtonDown("Fire2"))
        {
            UseFirstPersonPerspective = !UseFirstPersonPerspective;
        }

        // Input smoothing
        DistanceSmoothed = Mathf.Lerp(DistanceSmoothed, InputOrbitDistance, Time.deltaTime * DistanceSmoothing);
        if (CoreController.Instance.Player)
        {
            Velocity = CoreController.Instance.PlayerVhicl.MyRigidbody.velocity.magnitude;
        }
        else
        {
            Velocity = 0f;
        }

        VelocitySmoothed = Mathf.Lerp(VelocitySmoothed, VelocitySmoothed, Time.deltaTime * VelocitySmoothing);
    }

    void LateUpdate()
    {
        // Submersion
        if (WhirldData.Instance && WhirldData.Instance.SeaEnabled)
        {
            IsSubmerged = gameObject.transform.position.y <= WhirldData.Instance.SeaAltitude;

            //            IsSubmerged = _PlayWayWaterCamera.SubmersionState == SubmersionState.Full;

            //            ImmersionSurfaceFactor = Mathf.Clamp(
            //                Mathf.Abs(gameObject.transform.position.y) * ImmersionSurfaceDistance, 0,
            //                1);
            //
            //            SubmersionDeepFactor = Mathf.Lerp(0, 1, gameObject.transform.position.y * -1 / SubmersionDeepDepth);
        }
        else
        {
            IsSubmerged = false;
        }

        // Field of View :: Init
        FovTarget = FovDefault;

        // Target State :: Init
        TargetRotateTracking = false; // Individual modes can set (= true) to enable rotation updates with mouse movement delta from current frame
        TargetRotateAuto = false;

        // Mode determination
        if (!WhirldData.Instance || !WhirldData.Instance.IsPlayableWorld)
        {
            TargetMode = CoreCameraMode.AlignWithUi;
        }
        else if ( /*Time.timeSinceLevelLoad < TimeForceSpawnMode || */
            !CoreController.Instance.Player || !CoreController.Instance.PlayerVhicl || !CoreController.Instance.PlayerVhicl.RidePos)
        {
            TargetMode = CoreCameraMode.Spawning;
        }
        else if (CoreSettings.Instance.FreezeMotion)
        {
            TargetMode = CoreCameraMode.Roam;
        }
        else if (Cursor.lockState == CursorLockMode.Locked && Input.GetButton("Scope"))
        {
            TargetMode = CoreCameraMode.Scope;
        }
        else if (Cursor.lockState == CursorLockMode.Locked && (UseFirstPersonPerspective || Input.GetButton("Fire2")))
        {
            TargetMode = CoreCameraMode.First;
        }
        else
        {
            TargetMode = CoreCameraMode.Third;
        }

        // Clear flags
        if (CoreSettings.Instance.SkyMode == CoreSkyMode.Advanced)
        {
            ThisCamera.clearFlags = CameraClearFlags.SolidColor;
        }
        else
        {
            ThisCamera.clearFlags = CameraClearFlags.Skybox;
        }

        // Mode :: Align With UI
        // Sycronize position with UI camera for non-playable "Lobby" worlds
        if (TargetMode == CoreCameraMode.AlignWithUi)
        {
            if (!_uiCameraTransform)
            {
                var go = GameObject.Find("/_Runtime/Interface/Camera UI");

                if (!go)
                {
                    Debug.LogError("CoreCamera :: AlignWithUi :: Camera UI not found");
                }
                else
                {
                    _uiCameraTransform = go.transform;
                }
            }

            if (_uiCameraTransform)
            {
                transform.position = _uiCameraTransform.position;
                transform.rotation = _uiCameraTransform.rotation;

                // early-out, we're done here
                return;
            }
        }

        // Mode :: Spawning
        // Circle player when entering or rebuilding a world
        else if (TargetMode == CoreCameraMode.Spawning)
        {
            //            if (_worldEntryPosition != Vector3.zero)
            //            {
            //                transform.position = _worldEntryPosition;
            //                transform.rotation = _worldEntryLookRotation;
            //                return;
            //            }

            if (CoreController.Instance.Player)
            {
                TargetPosition = CoreController.Instance.Player.transform.position;
            }
            else
            {
                var seaAlt = 0f;

                if (WhirldData.Instance)
                {
                    seaAlt = WhirldData.Instance.SeaAltitude;
                }

                TargetPosition = new Vector3(0, seaAlt + 3, 0);
            }

            TargetDistance = 10;
            TargetRotateY = 0;
            TargetRotateX = (int)((Time.fixedTime * SpawnRotationSpeed) % 360) * 360;
        }

        // Mode :: First-Person
        else if (TargetMode == CoreCameraMode.First || TargetMode == CoreCameraMode.Scope)
        {
            if (_lastTargetMode != TargetMode && _lastTargetMode != CoreCameraMode.First && _lastTargetMode != CoreCameraMode.Scope && _lastTargetMode != CoreCameraMode.Third)
            {
                transform.rotation = CoreController.Instance.PlayerVhicl.RidePos.rotation;
            }

            TargetPosition = CoreController.Instance.PlayerVhicl.RidePos.position;
            TargetDistance = 0;

            if (TargetMode == CoreCameraMode.Scope)
            {
                FovTarget = FovFps / InputScopeZoom;
            }
            else
            {
                FovTarget = FovFps;
            }

            TargetRotateTracking = true;
            TargetRotateAuto = true;
        }

        // Mode :: Third
        else if (TargetMode == CoreCameraMode.Third)
        {
            TargetPosition = CoreController.Instance.Player.transform.position;
            TargetDistance = CoreController.Instance.PlayerVhicl.CamOffset;
            TargetDistance += DistanceSmoothed + (VelocitySmoothed * DistanceSmoothed * DistanceVelocityMultiplier);

            TargetRotateTracking = true;
            TargetRotateAuto = true;
        }

        // Mode :: Free-roaming camera
        else if (TargetMode == CoreCameraMode.Roam)
        {
            // Always reset target position to current, to ensure that we incorporate external position updates (such as the origin reset system)
            TargetPosition = transform.position;

            // Reset target position if we just entered roam mode
            //            if (_lastTargetMode != TargetMode)
            //            {
            //                TargetPosition = transform.position;
            ////				_roamRotationX = transform.localEulerAngles.y;
            ////				_roamRotationY = transform.localEulerAngles.x;
            //            }

            // Read keyboard input directly
            // @todo use Unity's input system
            if (Input.GetKey(KeyCode.W))
            {
                _roamMovement.z = 1.0f;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                _roamMovement.z = -1.0f;
            }
            else
            {
                _roamMovement.z = 0.0f;
            }

            if (Input.GetKey(KeyCode.D))
            {
                _roamMovement.x = 1.0f;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                _roamMovement.x = -1.0f;
            }
            else
            {
                _roamMovement.x = 0.0f;
            }

            if (Input.GetKey(KeyCode.E))
            {
                _roamMovement.y = 1.0f;
            }
            else if (Input.GetKey(KeyCode.Q))
            {
                _roamMovement.y = -1.0f;
            }
            else
            {
                _roamMovement.y = 0.0f;
            }

            float run = Input.GetKey(KeyCode.LeftShift) ? RoamRunMultiplier : 1.0f;
            RoamMoveSpeed = Mathf.Clamp(RoamMoveSpeed + (Input.GetAxis("Distance") * SensitivityScope), RoamMoveSpeedMin, RoamMoveSpeedMax);

            TargetPosition += transform.forward * RoamMoveSpeed * run * Time.deltaTime * _roamMovement.z;
            TargetPosition += transform.right * RoamMoveSpeed * run * Time.deltaTime * _roamMovement.x;
            TargetPosition += transform.up * RoamMoveSpeed * run * Time.deltaTime * _roamMovement.y;

            TargetDistance = 0f;
            TargetRotateTracking = true; // Input.GetButton("Fire1") || Input.GetButton("Fire2");
        }

        // Mode :: Not yet implemeneted
        else
        {
            Debug.Log("CoreCamera :: Unknown CoreCameraMode (" + TargetMode + ")");
        }

        // Move :: Manual rotation
        // Update rotation with mouse deltas only if mode proclaims itself to be eligible, and cursor is locked
        if (TargetRotateTracking && Cursor.lockState == CursorLockMode.Locked)
        {
            var rotationSpeed = TargetMode == CoreCameraMode.Scope ? LookZoomRotationMultiplier : LookRotationMultiplier;
            var rotateX = Input.GetAxis("Mouse X") * rotationSpeed;
            var rotateY = Input.GetAxis("Mouse Y") * rotationSpeed;

            if (Mathf.Abs(rotateX) > 0.001 || Mathf.Abs(rotateY) > 0.001)
            {
                _lastManualRotationUpdate = Time.time;
            }

            TargetRotateX += rotateX;
            TargetRotateY += rotateY;

            if (TargetRotateX < -360) TargetRotateX += 360;
            else if (TargetRotateX > 360)
                TargetRotateX -= 360;
            TargetRotateY = Mathf.Clamp(TargetRotateY, -90, 90);
        }

        // Move :: Automatic rotation
        if (TargetRotateAuto && Time.time - _lastManualRotationUpdate > DelayAutoRotateAfterManual && Velocity > 1f)
        {
            var moveRotation = Quaternion.identity;

            // Ride mode :: lerp to vehicle forward
            if (Math.Abs(TargetDistance) < 0.001)
            {
                moveRotation = Quaternion.Lerp(transform.rotation, CoreController.Instance.Player.transform.rotation, Time.deltaTime * AutoRotationRideSmoothing);
            }

            // Orbit mode :: Smoothly stay behind vehicle's direction of travel
            else
            {
                var moveDirection = CoreController.Instance.PlayerVhicl.MyRigidbody.velocity.normalized;
                if (moveDirection == Vector3.zero)
                {
                    moveDirection = Vector3.back;
                }

                var moveRotationTarget = Quaternion.LookRotation(moveDirection);

                // simple inertial simulation (via rotation limiting) to prevent snap reversal of X axis rotation when vehicle changes direction
                //				var deltaX = Quaternion.Angle(Quaternion.Euler(0, transform.rotation.y, 0), Quaternion.Euler(0, moveRotationTarget.y, 0));
                //				var deltaLimit = Mathf.Clamp(AutoRotationDeltaMax, Quaternion.Angle(transform.rotation, moveRotationTarget));
                //				_autoRotationFloatingAngularDeltaLimitX =
                //					Mathf.Clamp(deltaLimit,
                //					          Mathf.Lerp(_autoRotationFloatingAngularDeltaLimitX, deltaLimit, Time.deltaTime * AutoRotationDeltaSmoothing * (1 + _autoRotationFloatingAngularDeltaLimitX)));
                //
                //				var autoRotationSmoothingLimited = Mathf.Min(_autoRotationFloatingAngularDeltaLimitX, AutoRotationSmoothing);
                //				moveRotation = Quaternion.Lerp(transform.rotation, moveRotationTarget, Time.deltaTime * autoRotationSmoothingLimited);
                //
                moveRotation = Quaternion.Lerp(transform.rotation, moveRotationTarget, Time.deltaTime * AutoRotationSmoothing);
            }

            var moveAngles = moveRotation.eulerAngles;

            // simple inertial simulation (via rotation limitind_to
            //			var deltaLimit = Mathf.Min(AutoRotationDeltaMax, Quaternion.Angle(transform.rotation, moveRotationTarget));
            //			_autoRotationFloatingAngularDeltaLimitX =
            //				Mathf.Min(deltaLimit,
            //				          Mathf.Lerp(_autoRotationFloatingAngularDeltaLimitX, deltaLimit, Time.deltaTime * AutoRotationDeltaSmoothing * (1 + _autoRotationFloatingAngularDeltaLimitX)));
            //
            //			var autoRotationSmoothingLimited = Mathf.Min(_autoRotationFloatingAngularDeltaLimitX, AutoRotationSmoothing);
            //
            //			moveRotation = Quaternion.Lerp(transform.rotation, moveRotationTarget, Time.deltaTime * autoRotationSmoothingLimited);
            //			TargetRotateXDesired = moveAngles.y;
            //			var autoRotationSmoothingLimited = Mathf.Min(_autoRotationFloatingAngularDeltaLimitX, AutoRotationSmoothing);
            //			TargetRotateX = Quaternion.Lerp(TargetRotateX, TargetRotateXDesired, Time.deltaTime * autoRotationSmoothingLimited);

            TargetRotateX = moveAngles.y;
            TargetRotateY = moveAngles.x;

            // Transform range from 0_360 to -90_90
            if (TargetRotateY > 180)
            {
                TargetRotateY = TargetRotateY - 360;
            }

            TargetRotateY *= -AutoRotationYDamping;

            //			testEuler = moveRotation.eulerAngles;
        }
        //		else
        //		{
        //			testEuler = transform.rotation.eulerAngles;
        //		}

        var cameraNearClipPlane = 0.5f;

        // Move :: To position
        if (Math.Abs(TargetDistance) < 0.001)
        {
            // Check for terrain clipping (but only if we are moving a reasonable frame-by-frame amount and not jumping across the world
            // @todo not yet ready for prime time
            //			RaycastHit hit;
            //			var ray = transform.position - TargetPosition;
            //			var moveDist = Vector3.Magnitude(ray);
            //			if (moveDist < MaxMoveTerrainCheck && Physics.SphereCast(TargetPosition, MinTerrainProximity, ray, out hit, moveDist, LayerMaskClipAvoidance, QueryTriggerInteraction.Ignore))
            //			{
            //				TargetPosition = hit.point;
            //			}

            transform.position = TargetPosition;
            transform.localRotation = Quaternion.AngleAxis(TargetRotateX, Vector3.up);
            transform.localRotation *= Quaternion.AngleAxis(TargetRotateY, Vector3.left);
            _verticalBump = 0;

            cameraNearClipPlane = 0.1f;
        }

        // Move :: To projected offset vector
        else
        {
            // Automatic vertical bump to show more sky and collide with the terrain less
            var desiredVerticalBump = TargetDistance * VerticalBumpHeighToDistRato;
            _verticalBump = Mathf.Lerp(desiredVerticalBump, _verticalBump, Time.deltaTime * VerticalBumpSmoothing);
            TargetPosition += Vector3.up * _verticalBump;

            // Calculate initial projected position
            var rotationTarget = Quaternion.Euler(-TargetRotateY, TargetRotateX, 0);
            var rotationSmoothing = Mathf.Lerp(RotationSmoothingClose, RotationSmoothingFar, TargetDistance / MaxDistance);
            var rotation = Quaternion.Lerp(transform.rotation, rotationTarget, Time.deltaTime * rotationSmoothing);
            var v = new Vector3(0.0f, 0.0f, -TargetDistance);
            var position = rotation * v + TargetPosition;

            // Check for terrain clipping
            RaycastHit hit;
            var ray = position - TargetPosition;
            if (Physics.SphereCast(TargetPosition, MinTerrainProximity, ray, out hit, Vector3.Magnitude(ray), LayerMaskClipAvoidance, QueryTriggerInteraction.Ignore))
            {
                v = new Vector3(0.0f, 0.0f, -hit.distance);
                position = rotation * v + TargetPosition;

                // Jump to Ridepos rather than clipping inside the vehicle if terrain is forcing us too near to target vehicle
                if (Vector3.Distance(position, TargetPosition) < CoreController.Instance.PlayerVhicl.CamOffset)
                {
                    position = CoreController.Instance.PlayerVhicl.RidePos.position;
                }

                cameraNearClipPlane = 0.1f;
            }

            transform.rotation = rotation;
            transform.position = position;
        }

        // Far Clip Plane
        ThisCamera.nearClipPlane = cameraNearClipPlane;
        ThisCamera.farClipPlane = _farClipBase + transform.position.y * ClipPlaneAltitudeMultiplier;

        // Field of View
        ThisCamera.fieldOfView = Mathf.Lerp(ThisCamera.fieldOfView, FovTarget, Time.deltaTime * FovSmoothing);

        // Trigger quality update to allow for customized Post FX settings (focal depth, etc) in new view mode
        if (_lastTargetMode != TargetMode)
        {
            CoreController.Instance.TriggerOnQualityUpdate();
        }

        // Track deltas
        _lastTargetMode = TargetMode;
        _lastTargetPosition = TargetPosition;
        if (CoreController.Instance.Player)
        {
            _lastPlayerPosition = CoreController.Instance.Player.transform.position;
        }
    }

    void OnWorldInitialized()
    {
        Debug.Log("CoreCamera :: OnWorldInitialized");

        _worldLoadTime = Time.time;

        // Sun assignment
        // _screenSpaceShadows.sun = RenderSettings.sun;

        // Search for water objects in scene
        //        AquasLensEffects.gameObjects.waterPlanes = new List<GameObject>();
        //        AquasLensEffects.gameObject.SetActive(AquasLensEffects.gameObjects.waterPlanes.Count > 0);
        //        AquasLensEffects.underWater = false;

#if TRUESKY
        MyTrueSky = FindObjectOfType<simul.trueSKY>();
#endif

        _flagTriggerUpdateFxComponents = true;
    }

    void OnWorldUnloaded()
    {
        //#if TRUESKY
        //        _dynamicRenderingQuality.SetForceDisabled(MyTrueSkyCamera, true);
        //#endif
        //        _dynamicRenderingQuality.SetForceDisabled(_uSkyLightShafts, true);
    }

    public void TriggerUpdateFxComponents()
    {
        //        Debug.Log("CoreCamera :: TriggerUpdateFxComponents");
        _flagTriggerUpdateFxComponents = true;
    }

    void UpdateFxComponents()
    {
        _flagTriggerUpdateFxComponents = false;

        // Unified Postprocessing Stack
        _dynamicRenderingQuality.SetForceDisabled(_postProcessLayer, CoreSettings.Instance.DisableAdvancedPostFX);

        // UltimateWater
        var waterForceDisabled = !(WhirldData.Instance && WhirldData.Instance.SeaEnabled) || CoreSettings.Instance.DisableWater;
        //        _dynamicRenderingQuality.SetForceDisabled(_PlayWayWaterCamera, waterForceDisabled);
        //        _dynamicRenderingQuality.SetForceDisabled(_PlayWayWaterCameraIME, waterForceDisabled);

        // uSky Sun Shafts
        //        var lightShaftsDisabled = CoreSettings.Instance.DisableAdvancedPostFX || usky.uSkyTimeline.instance == null ||
        //                                  CoreSettings.Instance.SkyMode == CoreSkyMode.Simple ||
        //                                  CoreSettings.Instance.SkyMode == CoreSkyMode.Custom;
        //        _dynamicRenderingQuality.SetForceDisabled(_uSkyLightShafts, lightShaftsDisabled);

#if TRUESKY
// TrueSky
        var trueSkyForceDisabled = !MyTrueSky || MySettings.SkyMode != CoreSkyMode.Advanced;
        _dynamicRenderingQuality.SetForceDisabled(MyTrueSkyCamera, trueSkyForceDisabled);
        // @dragonhere _trueSkyCameraCubemap component causes scene to go x-ray freaky every second or so
//        _dynamicRenderingQuality.SetForceDisabled(_trueSkyCubemapProbe, trueSkyForceDisabled);
#endif

        // DeepSky Haze
        // @aubhere @deprecated
        //        var deepSky = FindObjectOfType<DeepSky.Haze.DS_HazeCore>();
        //        var deepSkyForceDisabled = deepSky == null || CoreSettings.Instance.DisableAtmospherics;
        ////        Debug.Log("deepSky - ");
        ////        Debug.Log(deepSky);
        ////        Debug.Log(deepSkyForceDisabled);
        //        if (deepSkyForceDisabled == false && WhirldData.Instance)
        //        {
        //            _DeepSkyHazeView.ApplyAirToSkybox = WhirldData.Instance.AtmosphereApplyAirToSkybox;
        //            _DeepSkyHazeView.ApplyFogExtinctionToSkybox = WhirldData.Instance.AtmosphereApplyFogToSkybox;
        //            _DeepSkyHazeView.ApplyFogLightingToSkybox = WhirldData.Instance.AtmosphereApplyFogLightingToSkybox;
        //            _DeepSkyHazeView.ApplyHazeToSkybox = WhirldData.Instance.AtmosphereApplyHazeToSkybox;
        //        }
        //
        //        _dynamicRenderingQuality.SetForceDisabled(_DeepSkyHazeView, deepSkyForceDisabled);

        // Enviro
        //        _EnviroSkyRendering = GetComponent<EnviroSkyRendering>();    // Enviro recreates it's camera components on demand
        //        var enviro = FindObjectOfType<EnviroSky>();
        //        var enviroForceDisabled = !enviro || CoreSettings.Instance.DisableAtmospherics;
        //        _dynamicRenderingQuality.SetForceDisabled(_EnviroSkyRendering, enviroForceDisabled);

        // Ambient Occlusion
        //        _dynamicRenderingQuality.SetForceDisabled(_ambientOcclusion, CoreSettings.Instance.DisableAdvancedPostFX || CoreSettings.Instance.DisableAO);

        // Screen Space Shadows
        // var SSSForceDisabled = CoreSettings.Instance.DisableAdvancedPostFX || CoreSettings.Instance.DisableSSS || !_screenSpaceShadows.sun;
        // _dynamicRenderingQuality.SetForceDisabled(_screenSpaceShadows, SSSForceDisabled);

        Debug.Log("CoreCamera :: UpdateFxComponents :: Triggered (waterForceDisabled: " + waterForceDisabled);
    }

    //    private void _junk()
    //    {
    ////        gyroTation = Quaternion.Euler(0, CoreController.PlayerVhicl.RidePos.rotation.eulerAngles.y, 0);
    //
    ////
    //////            float dist = Vector3.Distance(CoreController.Player.transform.position, transform.position);
    ////       .MyRigidbody rb = CoreController.Player.transform.gameObject.GetComponent.MyRigidbody>();
    ////
    //////            if (rb && rb.velocity.sqrMagnitude > .1 && rb.velocity.normalized.y < .8 && rb.velocity.normalized.y > -.8)
    //////            {
    ////        lastDir =
    ////            Vector3.Lerp
    ////                (lastDir, rb.velocity.normalized, .1f);
    ////
    //////            }
    //////            else
    //////            {
    //////                lastDir = Vector3.Lerp(lastDir, new Vector3(lastDir.x, 0f, lastDir.z), .1f);
    //////            }
    ////        Vector3 newPos = (CoreController.Player.transform.position + lastDir * -targetDist + Vector3.up * (targetDist / 3));
    ////        newPos.y =
    ////            newPos.y +
    ////            (CoreController.Player.transform.position.y -
    ////             lastY) *
    ////            Time.deltaTime;
    ////        transform.position =
    ////            Vector3.Lerp
    ////                (transform.position, newPos, Time.deltaTime * positionSmoothing);
    ////        lastY =
    ////            CoreController.Player.transform.position.y;
    ////
    //////            var upVector = Vector3.Lerp(transform.up, CoreSettings.Instance.flightCam ? CoreController.Player.transform.up : Vector3.up,
    //////                Time.deltaTime * horizonSmoothing);
    ////        var upVector = Vector3.up;
    ////        transform.rotation =
    ////            Quaternion.Slerp
    ////            (transform.rotation,
    ////                Quaternion.LookRotation
    ////                (CoreController.Player.transform.position -
    ////                 transform.position,
    ////                    upVector),
    ////                Time.deltaTime * rotationSmoothing);
    ////        RaycastHit hit;
    ////        if
    ////        (Physics.Linecast
    ////        (transform.position +
    ////         Vector3.up * 50,
    ////            transform.position +
    ////            Vector3.down * 1,
    ////            out
    ////            hit,
    ////            LayerMaskClipAvoidance))
    ////        {
    ////            newPos = transform.position;
    ////            newPos.y += 51 - hit.distance;
    ////            transform.position = newPos;
    ////        }
    ////    }
    //    }


    //    void OnCollisionStay(Collision collisionInfo)
    //    {
    //        Debug.Log("CollisionStay");
    //        foreach (ContactPoint contact in collisionInfo.contacts)
    //        {
    //            Debug.Log(contact.otherCollider.gameObject.name);
    //
    //            if (contact.otherCollider.CompareTag("Water"))
    //            {
    //                IsSubmerged = true;
    //                // @todo dragonhere - we are assuming that this is an infinite ocean, and that it is at zero height
    //                ImmersionSurfaceFactor = Mathf.Clamp(
    //                    Mathf.Abs(gameObject.transform.position.y) * ImmersionSurfaceDistance, 0,
    //                    1);
    //                Debug.DrawRay(contact.point, contact.normal * 10, Color.blue);
    //                break;
    //            }
    //        }
    //    }
    //
    //    void OnCollisionExit(Collision collisionInfo)
    //    {
    //        Debug.Log("CollisionExit");
    //
    //        foreach (ContactPoint contact in collisionInfo.contacts)
    //        {
    //            if (contact.otherCollider.CompareTag("Water"))
    //            {
    //                IsSubmerged = false;
    //                ImmersionSurfaceFactor = 0;
    //                break;
    //            }
    //        }
    //    }
}