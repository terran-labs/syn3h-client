using System;
using UnityEngine;

public class CoreTrueSky : MonoBehaviour
{
//    public Texture CosmicBackgroundTexture;
//    public Texture MoonTexture;

    private CoreController _myController;
    private CoreSettings _mySettings;

#if TRUESKY
    [NonSerialized] public simul.trueSKY MyTrueSky;
    [NonSerialized] public simul.TrueSkyCubemapProbe MyTrueSkyCubemapProbe;

    void OnEnable()
    {
        MyTrueSky = GetComponent<simul.trueSKY>();
        MyTrueSkyCubemapProbe = GetComponent<simul.TrueSkyCubemapProbe>();

        _myController = FindObjectOfType<CoreController>();
        _mySettings = FindObjectOfType<CoreSettings>();        
        
        if (!_myController || !_mySettings)
        {
            Debug.Log("CoreTrueSky :: Start :: No CoreController found, disabling CoreTrueSky manager.");
            this.enabled = false;
            return;
        }

//        MyTrueSky.SimulationTimeRain = false;

        _myController.OnWorldLoaded += OnWorldLoaded;
    }

    void OnDisable()
    {
        if (_myController)
        {
            _myController.OnWorldLoaded -= OnWorldLoaded;
        }
    }

    public void OnWorldLoaded()
    {
        // TrueSky Init
        // @dragonhere truesky renders day/night cycles out of sync with sun position when custom time or location is assigned :(
        if (MyTrueSky)
        {
            var latitude = _mySettings.GeoDestLat;
            var longitude = _mySettings.GeoDestLon;
            if (latitude.Equals(0) && longitude.Equals(0))
            {
                latitude = _mySettings.GeoDefaultLat;
                longitude = _mySettings.GeoDefaultLon;
            }
            MyTrueSky.SetSkyFloat("LatitudeRadians", (float) (latitude * Mathf.PI / 180)); // Converting to radians
            MyTrueSky.SetSkyFloat("LongitudeRadians", (float) (longitude * Mathf.PI / 180)); // Converting to radians

            var startDate = new DateTime(2000, 01, 01);
            var endDate = DateTime.Now;
            MyTrueSky.SetSkyInt("StartDayNumber", (int) (endDate - startDate).TotalDays);

            Debug.Log("CoreWhirld :: OnWorldLoaded :: TrueSky Init (LatitudeRadians: " + MyTrueSky.GetSkyFloat("LatitudeRadians") + ", LongitudeRadians: " +
                      MyTrueSky.GetSkyFloat("LongitudeRadians") + ", StartDayNumber: " + MyTrueSky.GetSkyInt("StartDayNumber") + ")");
        }
    }

    void Update()
    {
        // Conditional activation
        if (_mySettings.SkyMode != CoreSkyMode.Advanced)
        {
//                MyTrueSky.enabled = 
            MyTrueSkyCubemapProbe.enabled = false;
            return;
        }

//        MyTrueSky.enabled = 
        MyTrueSkyCubemapProbe.enabled = true;

        var qualityLevel = QualitySettings.GetQualityLevel();

        MyTrueSky.CloudSteps = (int) Mathf.Lerp(60, 250, qualityLevel / 5f);
        MyTrueSky.CubemapResolution = (int) Mathf.Lerp(32, 512, qualityLevel / 5f);
        MyTrueSky.CloudThresholdDistanceKm = 0;
//        MyTrueSky.DepthBlending = true;
//        MyTrueSky.Amortization = 3;
//        MyTrueSky.AtmosphericsAmortization = 3;
//        MyTrueSky.SimulationTimeRain = true;
//        MyTrueSky.backgroundTexture = CosmicBackgroundTexture;
//        MyTrueSky.moonTexture = MoonTexture;

        // Offset sky in the opposite direction of all scene shifting infinite origin resets
        // This compensates for a bug in TrueSky which causes it to offset the sky in the wrong direction
        transform.position = transform.root.position * -1;
    }

#endif
}