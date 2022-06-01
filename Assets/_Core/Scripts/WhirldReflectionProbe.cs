using UnityEngine;

public class WhirldReflectionProbe : MonoBehaviour
{
    public float UpdateReflectionFrequency = 2.35f;
    public float ProbePositionY = 250;
    public ReflectionProbe Probe;

//	private PlayWay.Water.Water _water;
    private Transform _cameraTransform;

    void Start()
    {
        InvokeRepeating("UpdateReflectionProbe", Random.Range(0, UpdateReflectionFrequency), UpdateReflectionFrequency);

        Probe = GetComponent<ReflectionProbe>();
//		_water = GetComponent<PlayWay.Water.Water>();

        if (Camera.main)
        {
            _cameraTransform = Camera.main.transform;
        }
    }

    void UpdateReflectionProbe()
    {
        var _qualityLevel = QualitySettings.GetQualityLevel();

        if (_qualityLevel < 3)
        {
            Probe.enabled = false;
            return;
        }

        if (_cameraTransform)
        {
            transform.position = new Vector3(_cameraTransform.position.x, ProbePositionY, _cameraTransform.position.z);
        }

        Probe.enabled = true;
        Probe.RenderProbe();
    }
}