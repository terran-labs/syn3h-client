using UnityEngine;

public class CoreQualityChangeDetection : MonoBehaviour
{
    public CoreController MyController;
    public CoreSettings MySettings;
    public CoreWhirld MyWhirld;

    private int? _lastQualityLevel;
    private bool _lastSubmerged;
    private bool _lastWorldLoaded;
    private bool _lastLightingIsDaylight;
    private bool _sceneUpdated;

    private const float UpdateCheckFrequency = .25f;

    void Start()
    {
        InvokeRepeating("UpdateCheck", Random.Range(0, UpdateCheckFrequency), UpdateCheckFrequency);
    }

    public void SceneUpdated()
    {
        _sceneUpdated = true;
    }

    void UpdateCheck()
    {
        var _qualityLevel = QualitySettings.GetQualityLevel();
        var _isSubmerged = CoreCamera.Instance && CoreCamera.Instance.IsSubmerged;
        var _isWorldLoaded = WhirldData.Instance && WhirldData.Instance.IsPlayableWorld;
        var _lightingIsDaylight = MySettings && MyWhirld.TimeIsDaylight;

        if (_qualityLevel != _lastQualityLevel)
        {
            _lastQualityLevel = _qualityLevel;
            MyController.TriggerOnQualityChange();
            
            // Ensure that TriggerOnQualityUpdate() fires as well
            _sceneUpdated = true;
        }

        if (_sceneUpdated || _isWorldLoaded != _lastWorldLoaded || _isSubmerged != _lastSubmerged || _lightingIsDaylight != _lastLightingIsDaylight)
        {
            _sceneUpdated = false;
            _lastSubmerged = _isSubmerged;
            _lastWorldLoaded = _isWorldLoaded;
            _lastLightingIsDaylight = _lightingIsDaylight;
            MyController.TriggerOnQualityUpdate();
        }
    }
}