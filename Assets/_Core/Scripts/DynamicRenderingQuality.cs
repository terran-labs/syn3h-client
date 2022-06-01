using UnityEngine;

public enum ActiveWhenSubmerged
{
    AlwaysActive,
    AirOnly,
    SubmergedOnly
}

public enum ActiveInScene
{
    AlwaysActive,
    LobbyOnly,
    WorldOnly
}

public enum ActiveInLighting
{
    AlwaysActive,
    Day,
    Night
}

public enum ActiveInEditor
{
    AlwaysActive,
    EditorOnly,
    BuildOnly
}

[System.Serializable]
public class DynamicQualityObject
{
    public Object Object;
    public bool ForceEnabled;
    public bool ForceDisabled;
    public int MinLevel;
    public int MaxLevel;
    public ActiveWhenSubmerged Submersion;
    public ActiveInScene Scene;
    public ActiveInLighting Lighting;
    public ActiveInEditor Editor;
}

public class DynamicRenderingQuality : MonoBehaviour
{
    public DynamicQualityObject[] Objects;

    void Awake()
    {
        Debug.Log("DynamicRenderingQuality :: Awake (" + gameObject.name + ")");
        
        var qualityLevel = QualitySettings.GetQualityLevel();
        var isSubmerged = CoreCamera.Instance && CoreCamera.Instance.IsSubmerged;
        var isDaylight = CoreWhirld.Instance && CoreWhirld.Instance.TimeIsDaylight;
        var isPlayableWorld = WhirldData.Instance && WhirldData.Instance.IsPlayableWorld;

        OnQualityUpdate(qualityLevel, isSubmerged, isDaylight, isPlayableWorld);
    }

    private void OnEnable()
    {
        if (CoreController.Instance)
        {
            CoreController.Instance.OnQualityUpdate += OnQualityUpdate;

//            // When testing in Editor, force an OnQualityUpdate whenever this script is enabled.
//            // This allows us to fast-toggle this script in Unity's inspector for testing   
//            if (Application.isEditor)
//            {
//                CoreController.Instance.TriggerOnQualityUpdate();
//
//                Debug.LogWarning("DynamicRenderingQuality :: OnEnable :: Triggered quality update to facilitate debugging efforts)");
//            }
        }
        else
        {
            Debug.LogWarning("DynamicRenderingQuality :: OnEnable :: Controller not found (" + gameObject.name + ")");
        }
    }

    void OnDisable()
    {
//        Debug.Log("DynamicRenderingQuality :: OnDestroy (" + gameObject.name + ")");
        if (CoreController.Instance)
        {
            CoreController.Instance.OnQualityUpdate -= OnQualityUpdate;
        }
    }

    void OnQualityUpdate(int qualityLevel, bool isSubmerged, bool timeIsDaylight, bool worldIsLoaded)
    {
        Debug.Log("DynamicRenderingQuality :: OnQualityUpdate (" + gameObject.name + ")");

        foreach (var qualityObject in Objects)
        {
            if (!qualityObject.Object)
            {
                continue;
            }
            var enabled = false;

            if (qualityObject.ForceEnabled)
            {
                // This object has been force-enabled
                enabled = true;
            }
            else if (qualityObject.ForceDisabled)
            {
                // This object has been force-disabled for debugging purposes
            }
            else
            {
                // Conditional Activation :: Quality Level
                enabled = qualityObject.MinLevel == 0 || qualityLevel >= qualityObject.MinLevel;

                // Conditional Activation :: Maximum quality level
                if (enabled && qualityObject.MaxLevel > 0 && qualityLevel > qualityObject.MaxLevel)
                {
                    enabled = false;
                }

                // Conditional Activation :: Submersion
                if (enabled)
                {
                    if (qualityObject.Submersion == ActiveWhenSubmerged.AirOnly && isSubmerged == true)
                    {
                        enabled = false;
                    }
                    else if (qualityObject.Submersion == ActiveWhenSubmerged.SubmergedOnly && isSubmerged == false)
                    {
                        enabled = false;
                    }
                }

                // Conditional Activation :: Lighting
                if (enabled)
                {
                    if (qualityObject.Lighting == ActiveInLighting.Day && timeIsDaylight == false)
                    {
                        enabled = false;
                    }
                    else if (qualityObject.Lighting == ActiveInLighting.Night && timeIsDaylight == true)
                    {
                        enabled = false;
                    }
                }

                // Conditional Activation :: Current Scene
                if (enabled)
                {
                    if (qualityObject.Scene == ActiveInScene.LobbyOnly && worldIsLoaded == true)
                    {
                        enabled = false;
                    }
                    else if (qualityObject.Scene == ActiveInScene.WorldOnly && worldIsLoaded == false)
                    {
                        enabled = false;
                    }
                }

                // Conditional Activation :: Editor vs Build
                if (enabled)
                {
                    if (qualityObject.Editor == ActiveInEditor.BuildOnly && Application.isEditor)
                    {
                        enabled = false;
                    }
                    else if (qualityObject.Editor == ActiveInEditor.EditorOnly && !Application.isEditor)
                    {
                        enabled = false;
                    }
                }
            }

            SetObjectActive(qualityObject.Object, enabled);
        }

        Debug.Log("DynamicRenderingQuality (" + gameObject.name + ") :: Change Complete");
    }

    public void SetObjectActive(Object obj, bool shouldBeActive)
    {
        var objectType = obj.GetType();

        if (objectType == typeof(GameObject))
        {
            GameObject objO = (GameObject) obj;
            if (objO.activeInHierarchy != shouldBeActive)
            {
                objO.SetActive(shouldBeActive);
            }
        }
        else
        {
            //Debug.Log("DynamicRenderingQuality (" + gameObject.name + ") :: " + obj.name + ": " + enabled);

            var objEnabled = objectType.GetProperty("enabled");
            if (objEnabled == null)
            {
                Debug.LogWarning("Unknown Object Type: " + objectType + "(" + obj.name + ")");
            }
            else
            {
                objEnabled.SetValue(obj, shouldBeActive, null);
            }
        }
    }

    // Set "Force Disabled" flag for given element
    // Note: Intentionally sets flag for multiple elements if they all have the same name. This allows multiple postfx components with different settings to be controlled as a group
    public void SetForceDisabled(Object targetObject, bool forceDisabled)
    {
        var componentsUpdated = 0;

        foreach (var qualityObject in Objects)
        {
            // Iterate through objects 'till we find the correct one
            if (!qualityObject.Object || qualityObject.Object.GetType() != targetObject.GetType())
            {
                continue;
            }

//            Debug.Log("DynamicRenderingQuality :: SetForceDisabled triggered (target type:" + targetObject.GetType() + ", forceDisabled: " + forceDisabled + ")");

            qualityObject.ForceDisabled = forceDisabled;

            // Trigger controller-initiated global quality update
            if (CoreController.Instance)
            {
                CoreController.Instance.TriggerOnQualityUpdate();
            }

            componentsUpdated++;
        }

        if (componentsUpdated == 0)
        {
            Debug.LogError("DynamicRenderingQuality :: SetForceDisabled target " + targetObject.name + "component not found");
        }
    }
}