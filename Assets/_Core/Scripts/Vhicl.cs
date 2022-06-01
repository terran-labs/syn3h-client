using System;
using UnityEngine;

public class Vhicl : MonoBehaviour
{
    //Global Settings
    // public static Dictionary<string, Dictionary<string, float>> Settings; // = new IDictionary<string, IDictionary>;

    //Vehicle Settings
    public string ShortName;

    public int CamOffset = 2;
    //MeshRenderer[] materialMain;
    //MeshRenderer[] materialAccent;

    //Object Links
    public Transform RidePos;

    [HideInInspector] public CoreSettings MySettings; // Not NonSerialized, or references will break when scripts are recompiled in editor
    [HideInInspector] public CoreController MyController; // Not NonSerialized, or references will break when scripts are recompiled in editor
    [NonSerialized] public LayerMask TerrainMask = 257; // hardcoded from editor inspector selection typecast to int
//    public LayerMask TerrainMaskEditor = 17; // hardcoded from editor inspector selection typecast to int

    public GameObject LaserAimer;
    public GameObject LaserAimerLocked;
    public GameObject LaserLock;
//    public ParticleEmitter Bubbles;
    [HideInInspector] public Rigidbody MyRigidbody;
    public GameObject RamoSphere;

    public float _ramoSphereScale;
    private GameObject _marker;
    private GameObject _markerQuarry;
    public VhiclNet VhiclNet = null;

    //Input
    public Vector4 Input;

    public Vector4 InputSmoothed;
    public float InputSmoothingFactor = 10f;

    public bool InputThrottle;

    public bool ZorbBall = false;
    public bool Brakes = false;
    public bool SpecialInput = false;
    public bool CamSmooth = true;

    //Networking Settings
    public int VehId = 0;

    public bool IsIt;
    public float LastTag = 0.00f;
    private float _startTime;
    public float LastReset = 0.00f;
    public int Score;
    public int ScoreTime;
    public Vector3 Velocity;
    public bool IsBot;
    public bool IsPlayer;
    public bool IsResponding;
    public string NetCode = "(No Connection)";
    public int NetKillMode = 0;
    private float _updateTick = 0;

    //Color Control
    public Color VhiclColor;

    public Color VhiclAccent;
    public Cubemap VhiclCubemap;
    public Material[] MaterialMain;
    public Material[] MaterialAccent;
    public Material[] MaterialBright;
    private Camera _cubemapCam;
    private bool _updateColor;

//    [NonSerialized] public float timeInit;
//    public const float MaterializationSuspensionTime = 3;

    private void Awake()
    {
//        Debug.Log("Vhicl :: Awake :: TerrainMaskEditor: " + (int)TerrainMaskEditor);
        
        // Prevent vehicle from freezing in midair when brakes are applied
        MyRigidbody = GetComponent<Rigidbody>();
        //Rigidbody.sleepThreshold = 0;

        //if(photonView.viewID.isMine) {
        VhiclLocal vhiclLocal = gameObject.AddComponent<VhiclLocal>();
        vhiclLocal.vhicl = this;
//        Rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
//        gameObject.AddComponent<SphereCollider>().radius = .1f;
//		}
//		if(photonView.viewID.isMine && !isBot) {
//			marker = Instantiate(VhiclPrefab.S.objectMarker, transform.position, transform.rotation);
//			marker.transform.parent = transform;
//		IsPlayer = true;
//			AppData.PlayerVeh = this;
//		VhiclMe vhiclMe = gameObject.AddComponent<VhiclMe>();
//		vhiclMe.Vhicl = this;
//			vhiclColor.r = PlayerPrefs.GetFloat("vehColR");
//			vhiclColor.g = PlayerPrefs.GetFloat("vehColG");
//			vhiclColor.b = PlayerPrefs.GetFloat("vehColB");
//			vhiclAccent.r = PlayerPrefs.GetFloat("vehColAccR");
//			vhiclAccent.g = PlayerPrefs.GetFloat("vehColAccG");
//			vhiclAccent.b = PlayerPrefs.GetFloat("vehColAccB");
//			if(Settings.colorCustom) Settings.saveVhiclColor();
//			setColor();
//		}
//		else {
//			Destroy(laserAimer);
//			Destroy(laserAimerLocked);
//			marker = Instantiate(AppData.objectMarker, transform.position, transform.rotation);
//			marker.transform.parent = transform;
//			markerQuarry = Instantiate(AppData.objectMarkerQuarry, transform.position, transform.rotation);
//			markerQuarry.transform.parent = transform;
//
//			if(isBot && photonView.viewID.isMine) {
//				VhiclBot vhiclBot = gameObject.AddComponent<VhiclBot>();
//				vhiclBot.Vhicl = this;
//				vhiclColor = AppData.PlayerVeh.vhiclColor;
//				vhiclAccent = AppData.PlayerVeh.vhiclAccent;
//			}
//			else {
//				vhiclNet = gameObject.AddComponent<VhiclNet>();
//				vhiclNet.Vhicl = this;
//			}
//		}

//		if(AppData.Players.ContainsKey(name)) AppData.Players.Remove(name);
//		AppData.Players.Add(name, this);
//
//		//Make sure everyone is colored correctly
//		foreach(DictionaryEntry plrE in AppData.Players) plrE.Value.setColor();

        LastTag = Time.time;
        _startTime = Time.time;
    }

    public void Init(CoreController ControllerReference, CoreSettings SettingsReference)
    {        
        MyController = ControllerReference;
        MySettings = SettingsReference;

//        timeInit = Time.time;

        if (IsPlayer)
        {
            VhiclMe vhiclMe = gameObject.AddComponent<VhiclMe>();
            vhiclMe.Vhicl = this;
        }
        else if (IsBot)
        {
            VhiclBot vhiclBot = gameObject.AddComponent<VhiclBot>();
            vhiclBot.Vhicl = this;
        }

        gameObject.BroadcastMessage("InitVhicl", this);
        MySettings.UpdateObjects();
    }

    void Update()
    {
        // Submersion

//		bubbles.emit = (transform.position.y < Settings.lavaAlt - 2 || Physics.Raycast(transform.position + (Vector3.up * 200), Vector3.down, 198,  1 << 4));
//		bubbles.maxEnergy = bubbles.minEnergy = (bubbles.emit ? 5 : 0);

//		if(isBot || !photonView.isMine) {
//			if(isIt && markerQuarry != null && !markerQuarry.active) {
//				markerQuarry.SetActiveRecursively(true);
//				marker.SetActiveRecursively(false);
//			}
//			else if(!isIt && markerQuarry && markerQuarry.active) {
//				markerQuarry.SetActiveRecursively(false);
//				marker.SetActiveRecursively(true);
//			}
//			if(isIt && AppData.Player) AppData.quarryDist = Vector3.Distance(transform.position, AppData.Player.transform.position);
//		}

//		if(updateColor) {
//			updateColor = false;
//
//			bool isGreen = (isIt && AppData.Players.Count > 1 ? true : false);
//			if(materialMain.length > 0) {
//				Color targetColor = (isGreen ? AppData.vhiclIsItColor : vhiclColor);
//				materialMain[0].color = Color.Lerp(materialMain[0].color, targetColor, Time.deltaTime * 2);
//				materialMain[0].color.a = .5;
//				if(materialAccent.length > 0) materialMain[0].SetColor ("_SpecColor", materialAccent[0].color);
//				else materialMain[0].SetColor ("_SpecColor", vhiclAccent);
//				if(materialMain[0].color.r < targetColor.r - .05 || materialMain[0].color.r > targetColor.r + .05 || materialMain[0].color.g < targetColor.g - .05 || materialMain[0].color.g > targetColor.g + .05 || materialMain[0].color.b < targetColor.b - .05 || materialMain[0].color.b > targetColor.b + .05) updateColor = true;
//				if(materialMain.length > 1) for(i = 1; i < materialMain.length; i++) materialMain[i].color = materialMain[0].color;
//			}
//			if(materialAccent.length > 0) {
//				targetColor = (isGreen ? AppData.vhiclIsItAccent : vhiclAccent);
//				materialAccent[0].color = Color.Lerp(materialAccent[0].color, targetColor, Time.deltaTime * 2);
//				materialAccent[0].color.a = .5;
//				materialAccent[0].SetColor ("_SpecColor", materialMain[0].color);
//				if(materialAccent[0].color.r < targetColor.r - .05 || materialAccent[0].color.r > targetColor.r + .05 || materialAccent[0].color.g < targetColor.g - .05 || materialAccent[0].color.g > targetColor.g + .05 || materialAccent[0].color.b < targetColor.b - .05 || materialAccent[0].color.b > targetColor.b + .05) updateColor = true;
//				if(materialAccent.length > 1) for(i = 1; i < materialAccent.length; i++) materialAccent[i].color = materialAccent[0].color;
//			}
//			if(materialBright.length > 0) {
//				targetColor = (isGreen ? AppData.vhiclIsItAccent : vhiclAccent);
//				targetColor.a = .33;
//				targetColor = Vector4.Normalize(targetColor);
//				materialBright[0].color = Color.Lerp(materialBright[0].color, targetColor, Time.deltaTime * 2);
//				if(!updateColor && (materialBright[0].color.r < targetColor.r - .05 || materialBright[0].color.r > targetColor.r + .05 || materialBright[0].color.g < targetColor.g - .05 || materialBright[0].color.g > targetColor.g + .05 || materialBright[0].color.b < targetColor.b - .05 || materialBright[0].color.b > targetColor.b + .05)) updateColor = true;
//				if(materialBright.length > 1) for(i = 1; i < materialBright.length; i++) materialBright[i].color = materialBright[0].color;
//			}
//		}

//		if(Time.time > updateTick) {
//			updateTick = Time.time + 1;
//			if(!AppData.Players.ContainsKey(name)) AppData.Players.Add(name, this);	//Dragonhere: this helps to prevent multi-quarry syndrome
//		}
    }

    //Cubemap Reflections
    void LateUpdate()
    {
//        if (vhiclCubemap && isPlayer && QualitySettings.GetQualityLevel() > 4)
//        {
//            if (!cubemapCam)
//            {
//                GameObject go = new GameObject("VhiclCubemapCamera");
//                go.AddComponent<Camera>();
//                go.transform.rotation = Quaternion.identity;
//                cubemapCam = go.GetComponent<Camera>();
//                cubemapCam.farClipPlane = 100;
//                cubemapCam.cullingMask = 1 << 0 | 1 << 4;
//                cubemapCam.enabled = false;
//            }
//            if (vhiclCubemap == WhirldController.S.whirldCubemap)
//            {
//                vhiclCubemap = new Cubemap(128, TextureFormat.RGB24, false); //Don't overwrite Whirld cubemap
//                setCubemap(); //Relink materials to new map
//            }
//
//            cubemapCam.transform.position = transform.position;
//            cubemapCam.RenderToCubemap(vhiclCubemap, 1 << (Time.frameCount % 6));
//        }
//        else if (WhirldController.S.whirldCubemap && vhiclCubemap != WhirldController.S.whirldCubemap)
//        {
//            vhiclCubemap = WhirldController.S.whirldCubemap;
//            setCubemap();
//        }
    }

    void FixedUpdate()
    {
        // Suspend vehicle in midair for a few seconds after init.
        // Looks cool, and also handy to keep it from falling through freshly-generated terrain.
//        if (timeInit > 0)
//        {
//            if (Time.time - timeInit < MaterializationSuspensionTime)
//            {
//                MyRigidbody.isKinematic = true;
//            }
//            else
//            {
//                MyRigidbody.isKinematic = false;
//                timeInit = 0;
//            }
//        }

        // Input smoothing
        if (!MyRigidbody.isKinematic)
        {
            InputSmoothed = Vector4.Lerp(InputSmoothed, Input, Time.fixedDeltaTime * InputSmoothingFactor);
        }

//		if(ramoSphereScale != 0 && ramoSphere) {
//			ramoSphere.transform.localScale = Vector3.Lerp (ramoSphere.transform.localScale, Vector3.one * ramoSphereScale, Time.fixedDeltaTime);
//			if(ramoSphere.transform.localScale.x > ramoSphereScale - .01 && ramoSphere.transform.localScale.x < ramoSphereScale + .01) ramoSphereScale = 0;
//		}
    }

    /*DRAGONHERE: Rewrite in game
    void  OnGUI (){

         if(
            !Rigidbody ||
            !Camera.main ||
            (netCode != "" && Time.time < startTime + 5) ||
            Settings.hideNames ||
            (photonView.isMine && !isBot)
            ) return;


        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);

        if(pos.z <= 0 || Vector3.Distance(Vector3(pos.x,pos.y,0), Input.mousePosition) > 40 || Physics.Linecast(transform.position, Camera.main.transform.position, 1 << 8)) return;

        GUI.skin = AppData.Skin;
        GUI.color.a = AppData.GUIAlpha;
        GUI.depth = 5;

        float sizeX = Mathf.Max(50, Mathf.Min(150,Screen.width * .16) - pos.z / 1.5f);
        float sizeY = Mathf.Max(20, Mathf.Min(50,Screen.width * .044) - (pos.z * 0.2f));

        if((pos.z <= 1 || pos.y < sizeY * 1.9f) && (photonView.isMine && !isBot)) {
            if(pos.z <= 1) pos.x = Screen.width / 2;
            pos.y = sizeY + 100;
        }
        GUI.Button( new Rect(pos.x - sizeX*.5,Screen.height - pos.y + (sizeY*1), sizeX, sizeY),
            name + "\n" + shortName + " " + score + netCode, "player_nametag" + (isIt ? "_it" : "")
        );
    }
    */

    void OnPrefsUpdated()
    {
//        if (laserAimer) laserAimer.GetComponent<ParticleEmitter>().emit = true;
//        if (laserAimerLocked) laserAimerLocked.GetComponent<ParticleEmitter>().emit = true;
//		if(Settings.ramoSpheres != 0) {
//			//DRAGONHEREyield return new WaitForSeconds (1);
//			Vector3 tnsor = GetComponent<Rigidbody>().inertiaTensor;
//			Vector3 cg = GetComponent<Rigidbody>().centerOfMass;
//			if(!ramoSphere) {
//				ramoSphere = Instantiate(ramoSphereObj, transform.position, transform.rotation);
//				ramoSphere.transform.parent = transform;
//				//(ramoSphere.collider as SphereCollider).attachedRigidbody = Rigidbody;
//				Collider[] colliders = vehObj.GetComponentsInChildren<Collider>();
//				foreach(Collider cldr in colliders) {
//					Physics.IgnoreCollision(ramoSphere.GetComponent<Collider>(), cldr);
//				}
//			}
//			ramoSphere.GetComponent<Collider>().enabled = false; //DRAGONHERE - MAJOR UNITY BUG: We need to set this all the time, as colliders that are instantiated using a prefab and are then thrown inside of rightbodies are not properly initialized until some of their settings are toggled
//			ramoSphereScale = (((Settings.ramoSpheres) * 15) + camOffset * 1);
//			if(ramoSphere.GetComponent<Collider>().isTrigger == zorbBall) {
//				ramoSphere.GetComponent<Collider>().isTrigger = !zorbBall;
//				ramoSphere.transform.localScale = Vector3.zero;
//				ramoSphere.GetComponent<Collider>().enabled = true;
//				//ramoSphere.SendMessage("colorSet", zorbBall); //ANOTHER UNITY BUG - for some reason, SendMessage isn't working like it should...
//				RamoSphere sphere = ramoSphere.GetComponent<RamoSphere>();
//				sphere.colorSet(zorbBall);
//			}
//			else ramoSphere.GetComponent<Collider>().enabled = true;
//			GetComponent<Rigidbody>().inertiaTensor = tnsor;
//			GetComponent<Rigidbody>().centerOfMass = cg;
//		}
//		else if(ramoSphere) {
//			ramoSphereScale = 0.1f;
//			//DRAGONHEREyield return new WaitForSeconds (2);
//			Destroy(ramoSphere);
//		}

//		if(Settings.laserLock[vehId] > 0) {
//			laserLock.active = true;
//			laserLock.transform.localScale = Vector3.one * ((((Settings.laserLock[vehId]) + camOffset * .1) * 10));
//		}
//		else {
//			laserLock.active = false;
//			laserLock.transform.localScale = Vector3.zero;
//		}
    }

    void OnCollisionEnter(Collision collision)
    {
//        if (ramoSphere && ramoSphere.GetComponent<Collider>().isTrigger == false)
//            ramoSphere.SendMessage("OnCollisionEnter", collision);
    }

    //Called when we ram a quarry, and will become quarry
    public void OnRam(GameObject other)
    {
//		Vhicl veh = other.GetComponent<Vhicl>();
//		if(!veh || veh.isIt != 1 || !veh.isResponding || (Time.time - lastTag) < 3 || (Time.time - veh.lastTag) < 3) return;
//		lastTag = Time.time;
//		photonView.RPC("sQ", PhotonTargets.All, 1);
    }

    void OnLaserHit(bool isFatal)
    {
//		 if(isFatal && Settings.lasersFatal && Vector3.Distance(transform.position, WhirldController.S.whirldBase.position) > 10) {
//			Rigidbody.isKinematic = true;
//			photonView.RPC("lR", PhotonTargets.All);
//		}
    }

//  [RPC]
    void lR()
    {
//		if(Time.time - lastReset < 3 || !Rigidbody || !WhirldController.S.whirldBase) return; //We are already resetting...
//		lastReset = Time.time;
//		if(isPlayer || isBot) {
//			Rigidbody.isKinematic = true;
//		}
//		AppData.mE(transform.position);
//		AppData.mE(WhirldController.S.whirldBase.position);
//		ramoSphereScale = 0.01f;
//		//DRAGONHEREyield return new WaitForSeconds(2);
//		if(ramoSphere) Destroy(ramoSphere);
//		if(isPlayer || isBot) {
//			transform.position = WhirldController.S.whirldBase.position;
//			Rigidbody.isKinematic = false;
//		}
//		OnPrefsUpdated(); //Rebuild a new ramosphere
    }

//	[RPC]
//	void  fR ( PhotonViewID LaunchedByViewID ,   string id ,   Vector3 pos ,   Vector3 ang ,   PhotonMessageInfo info  ){
//		GameObject btemp = Instantiate(AppData.objectVhiclLaser, pos, Quaternion.Euler(ang));
//		VhiclLaser r = btemp.GetComponent<VhiclLaser>();
//		r.laserID = id;
//		if(info.photonView.isMine != true) r.lag = (PhotonNetwork.time - info.timestamp);
//		r.launchVhicl = this;
//	}
//
//	[RPC]
//	void  fS ( PhotonViewID LaunchedByViewID ,   string id ,   Vector3 pos ,   Vector3 ang ,   PhotonMessageInfo info  ){
//		GameObject btemp = Instantiate(AppData.objectVhiclLaserSnipe, pos, Quaternion.Euler(ang));
//		VhiclLaser r = btemp.GetComponent<VhiclLaser>();
//		r.laserID = id;
//		if(info.photonView.isMine != true) r.lag = (PhotonNetwork.time - info.timestamp);
//		r.launchVhicl = this;
//	}
//
//	[RPC]
//	void  fRl ( PhotonViewID LaunchedByViewID ,   string id ,   Vector3 pos ,   PhotonViewID targetViewID ,   PhotonMessageInfo info  ){
//		GameObject btemp = Instantiate(AppData.objectVhiclLaser, pos, Quaternion.identity);
//		VhiclLaser r = btemp.GetComponent<VhiclLaser>();
//		r.laserID = id;
//		if(info.photonView.isMine != true) r.lag = (PhotonNetwork.time - info.timestamp);
//		r.launchVhicl = this;
//		foreach(DictionaryEntry plrE in AppData.Players) if(plrE.Value.photonView.viewID == targetViewID) {
//			r.targetVhicl = plrE.Value;
//			break;
//		}
//	}
//
//	[RPC]
//	void  fSl ( PhotonViewID LaunchedByViewID ,   string id ,   Vector3 pos ,   PhotonViewID targetViewID ,   PhotonMessageInfo info  ){
//		GameObject btemp = Instantiate(AppData.objectVhiclLaserSnipe, pos, Quaternion.identity);
//		VhiclLaser r = btemp.GetComponent<VhiclLaser>();
//		r.laserID = id;
//		if(info.photonView.isMine != true) r.lag = (PhotonNetwork.time - info.timestamp);
//		r.launchVhicl = this;
//		foreach(DictionaryEntry plrE in AppData.Players) if(plrE.Value.photonView.viewID == targetViewID) {
//			r.targetVhicl = plrE.Value;
//			break;
//		}
//	}
//
//	[RPC]
//	void  lH ( string n ,   Vector3 pos  ){
//		GameObject go = gameObject.Find("lsr#" + n);
//		if(go) go.GetComponent<VhiclLaser>().laserHit(gameObject, transform.TransformPoint(pos), Vector3.up);
//		//else Debug.Log("LaserHitFail");
//	}
//
//	[RPC]
//	void  sP ( Vector3 pos ,   Quaternion rot ,   PhotonMessageInfo info  ){
//		if(!vhiclNet) return;
//		vhiclNet.rpcPing = (PhotonNetwork.time - info.timestamp);
//
//		if(vhiclNet.states[0] && vhiclNet.states[0].t >= info.timestamp) {
//			Debug.Log("sP OoO: " + vhiclNet.states[0].t + " * " + Time.time);
//			return;
//		}
//
//		for (k=vhiclNet.states.length-1; k>0; k--) vhiclNet.states[k] = vhiclNet.states[k-1];
//		vhiclNet.states[0] = new State(pos, rot, info.timestamp, 0, 0);
//
//		float png = PhotonNetwork.time - vhiclNet.states[0].t;
//		vhiclNet.jitter = Mathf.Lerp(vhiclNet.jitter, Mathf.Abs(vhiclNet.ping - png), 1 / PhotonNetwork.sendRate);
//		vhiclNet.ping = Mathf.Lerp(vhiclNet.ping, png, 1 / PhotonNetwork.sendRate);
//
//		//vhiclNet.states[0] = new State(pos, rot, Time.time, ((PhotonNetwork.time - info.timestamp) > 0 && (PhotonNetwork.time - info.timestamp) < 10 ? info.timestamp : PhotonNetwork.time - vhiclNet.calcPing));
//	}
//
//	[RPC]
//	void  sT ( float time ,   PhotonMessageInfo info  ){
//		if(!vhiclNet && !photonView.isMine) return;
//		/*Messaging.Broadcast(gameObject.name);
//		Messaging.Broadcast(time + "t");
//		Messaging.Broadcast(PhotonNetwork.time + "t");
//		Messaging.Broadcast(PhotonNetwork.time - info.timestamp + "");
//		float nTime = PhotonNetwork.time;*/
//
//		//We are recieving the ping back
//		//Debug.Log(name + time + photonView.viewID);
//
//		if(time > 0) {
//			vhiclNet.calcPing = Mathf.Lerp(vhiclNet.calcPing, (Time.time - vhiclNet.lastPing) / (vhiclNet.wePinged ? 1 : 2), .5);
//			vhiclNet.wePinged = false;
//		}
//
//		//We are authoratative instance, and are being "pinged".
//		else if(photonView.isMine) {
//			photonView.RPC("sT", PhotonTargets.Others, 1.0f);
//		}
//
//		//We are a non authorative instance. Get ready to measure the time diff!
//		else {
//			vhiclNet.lastPing = Time.time;
//			vhiclNet.wePinged = (info.sender == PhotonNetwork.player ? true : false);
//		}
//	}

//  [RPC]
    void s4(int x, int y, int z, int w)
    {
        Input = new Vector4(x / 10, y / 10, z / 10, w / 10);
    }

//  [RPC]
    void sI(bool input)
    {
        SpecialInput = input;
        gameObject.BroadcastMessage("OnSetSpecialInput", SendMessageOptions.DontRequireReceiver);
    }

//  [RPC]
    void sB(bool input)
    {
        Brakes = input;
    }

//  [RPC]
    void sZ(bool input)
    {
        ZorbBall = input;
        if (input == true) setCubemap();
        OnPrefsUpdated();
    }

//  [RPC]
    void sQ(int mode)
    {
//		foreach(DictionaryEntry plrE in AppData.Players) {
//			//if(plrE.Value.isIt == 1) string prevName = go.name;
//			plrE.Value.isIt = false;
//			plrE.Value.setColor();
//		}
//		isIt = true;
//		AppData.QuarryVeh = this;
//		setColor();
//		if(mode) {
//			if(mode == 1) Messaging.Broadcast(gameObject.name + " rammed the Quarry", chatOrigins.Server);
//			else if(mode == 2) Messaging.Broadcast(gameObject.name + " is now the Quarry", chatOrigins.Server);
//			else if(mode == 3) Messaging.Broadcast(gameObject.name + " Defaulted to Quarry", chatOrigins.Server);
//			lastTag = Time.time;
//		}
    }

//  [RPC]
    void iS(string name)
    {
        Score += 1;
//		Messaging.Broadcast(gameObject.name + " Got  " + name, int.Parse(chatOrigins.Server));
    }

//  [RPC]
    void dS(string name)
    {
        Score -= 1;
        //Messaging.Broadcast(gameObject.name + " Lasered NQ (" + name + ")", 0);
    }

//  [RPC]
    void iT()
    {
        ScoreTime += 1;
    }

//  [RPC]
    void sS(int s)
    {
        Score = s;
    }

//  [RPC]
    void sC(float cR, float cG, float cB, float aR, float aG, float aB)
    {
        VhiclColor.r = cR;
        VhiclColor.g = cG;
        VhiclColor.b = cB;
        VhiclAccent.r = aR;
        VhiclAccent.g = aG;
        VhiclAccent.b = aB;
        _updateColor = true;
    }

    void setColor()
    {
        _updateColor = true;
        setCubemap();
        //gameObject.BroadcastMessage("OnSetColor", SendMessageOptions.DontRequireReceiver);
    }

    void setCubemap()
    {
        //Assign cubemap for reflections
        //DRAGONHEREyield return 0; 	//Give stuff a chance to generate
        /*foreach (Renderer rnd in gameObject.GetComponentsInChildren(Renderer, true)) {
            foreach(Material mat in rnd.materials) if(mat.HasProperty("_Cube")) mat.SetTexture("_Cube", vhiclCubemap);
        }*/
    }

//  [RPC]
    void dN(int rsn)
    {
        NetKillMode = rsn;
    }
}