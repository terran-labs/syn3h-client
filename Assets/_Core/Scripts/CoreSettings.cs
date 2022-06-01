using UnityEngine;

public enum CoreSkyMode
{
    Simple,
    Intermediate,
    Advanced,
    Custom,
    nil
}

public class CoreSettings : MonoBehaviour
{
    public static CoreSettings Instance { get; private set; }

    public string WrldApiKey = "54cbc90b28b0ffe741aa29818bf0f20b";

    public float TimeSunrise = .25f;
    public float TimeSunset = .75f;
    public float TimeSpeedMax = 2500;
    public float TimeSpeedDeadzone = 50;
    public float TimeSpeedMultiplier = 1000;

    public float SeaAltitudeMin = -500;
    public float SeaAltitudeMax = 500;
    public float SeaAltitudeStepSmall = 1;
    public float SeaAltitudeStepBig = 25;

    public bool gyroCam = false;
    public bool flightCam = true;
    public bool FreezeMotion;

    public float lavaAlt = 0;
    public float worldGrav = -9.81f;
    public float worldFog = 0.001f;
    public float worldViewDist = 5000;

    public bool buggyFlightSlip = false;
    public bool buggySmartSuspension = true;
    public bool buggyNewPhysics = false;
    public bool buggyAWD = true;
    public bool buggyFlightLooPower = false;
    public float buggyCG = -.40f;
    public float buggyPower = 1.00f;
    public float buggySpeed = 30.00f;
    public float buggyFlightDrag = 300.00f;
    public float buggyFlightAgility = 1.00f;
    public float buggyTr = 1.00f;
    public float buggySh = 70.00f;
    public float buggySl = 50.00f;

    public float jetHDrag = 0.01f;
    public float jetDrag = .0001f;
    public float jetSteer = 20f;
    public float jetLift = .5f;
    public float jetStall = 20f;

    public Transform SceneObjects;
    public Transform DynamicObjects;

    public float PlayerVelocity;
    public float PlayerOriginDistance;
    public float PlayerAltitude;

    public bool DisableAdvancedPostFX;
    public bool DisableVisualDOF;
    public bool DisableAA;
    public bool DisableAO;
    public bool DisableSSS;

    public double GeoDestLat;
    public double GeoDestLon;
    public double GeoDefaultLat = 51.47722222; // GMT

    public double GeoDefaultLon = 0; // 
//    public int GeoZoomMin = 10;
//    public int GeoZoomMax = 19;
//    public float GeoZoomMaxAltitude = 2500;


    public CoreSkyMode SkyMode = CoreSkyMode.Intermediate;

    public bool DisableAtmospherics;
    public bool DisableWater;

    public void updatePrefs()
    {
//        /*
//        0 Fastest
//        1 Fast
//        2 Simple
//        3 Good
//        4 Beautiful
//        5 Fantastic
//        */
//
//        //LOD Distance
//        if(QualitySettings.currentLevel + 1 > 4)		World.lodDist = 1000;
//        else if(QualitySettings.currentLevel + 1 > 3)	World.lodDist = 400;
//        else if(QualitySettings.currentLevel + 1 > 2)	World.lodDist = 75;
//        else World.lodDist = 0;
//
//        Time.fixedDeltaTime = (QualitySettings.currentLevel > 2 ? 0.02f : 0.025f);
//
//        //RenderSettings.fog = (QualitySettings.currentLevel + 1 > 1 ? true : false);
//        Camera.main.farClipPlane = 3500 - ((5 - int.Parse(QualitySettings.currentLevel)) * 300);
//        worldFog = Mathf.Lerp(.007, .0003, Camera.main.farClipPlane / 6000);
//
//        if(World.terrains) foreach(Terrain trn in World.terrains) {
//
//            //Details: Rocks, trees, etc
//            trn.treeCrossFadeLength = 30;
//            if(QualitySettings.currentLevel + 1 > 4) {	//Fantastic, Beautiful
//                trn.detailObjectDistance = 300;
//                trn.treeDistance = 600;
//                trn.treeMaximumFullLODCount = 100;
//                trn.treeBillboardDistance = 150;
//            }
//            else if(QualitySettings.currentLevel + 1 > 3) {	//Good
//                trn.detailObjectDistance = 200;
//                trn.treeDistance = 500;
//                trn.treeMaximumFullLODCount = 50;
//                trn.treeBillboardDistance = 100;
//            }
//            else if(QualitySettings.currentLevel + 1 > 2) {	//Simple
//                trn.detailObjectDistance = 150;
//                trn.treeDistance = 300;
//                trn.treeMaximumFullLODCount = 10;
//                trn.treeBillboardDistance = 75;
//            }
//            else {					//Fast, Fastest
//                trn.detailObjectDistance = 0;
//                trn.treeDistance = 0;
//                trn.treeMaximumFullLODCount = 0;
//                trn.treeBillboardDistance = 0;
//            }
//
//            //Textures
//            trn.basemapDistance = 1500;
//            /*if(QualitySettings.currentLevel + 1 > 3) {
//                trn.basemapDistance = 900;
//            }
//            else if(QualitySettings.currentLevel + 1 > 1) {
//                trn.basemapDistance = 300;
//            }
//            else {
//                trn.basemapDistance = 150;
//            }*/
//
//            //Heightmap Resolution
//            if(QualitySettings.currentLevel + 1 > 5) {
//                trn.heightmapMaximumLOD = 0;
//                trn.heightmapPixelError = 5;
//            }
//            else if(QualitySettings.currentLevel + 1 > 2) {
//                trn.heightmapMaximumLOD = 0;
//                trn.heightmapPixelError = 15;
//            }
//            else if(QualitySettings.currentLevel + 1 > 1) {
//                trn.heightmapMaximumLOD = 0;
//                trn.heightmapPixelError = 50;
//            }
//            else {
//                trn.heightmapMaximumLOD = 1;
//                trn.heightmapPixelError = 50;
//            }

//		if(QualitySettings.currentLevel + 1 > 4) trn.lighting = TerrainLighting.Pixel;
//		else trn.lighting = TerrainLighting.Lightmap;
    }
//
//        Physics.gravity = Vector3(0, worldGrav, 0);
//        if(World.sea) World.sea.position.y = lavaAlt;

    void OnEnable()
    {
        Instance = this;
    }

    public void UpdateObjects()
    {
    }
}