using System;

using UnityEngine;
using System.Collections;

[System.Serializable][ExecuteInEditMode]
public class VhiclPrefab : ScriptableObject
{
    //public static VhiclPrefab S;
//    public static VhiclPrefab S {
//        get { return this; }
//    }
    public static string DefaultCloudServerUrl = "app.exitgamescloud.com";
    public static string DefaultServerAddress = "127.0.0.1";
    public static int DefaultMasterPort = 5055;  // default port for master server
    public static string DefaultAppID = "Master";

    public string ServerAddress = DefaultServerAddress;
    public int ServerPort = 5055;
    public string AppIDDDDDD = "";

    /*public override string ToString()
    {
        return "ServerSettings: " + HostType + " " + ServerAddress;
    }*/
}


/*
class WhirldIn extends MonoBehaviour
{
    static var m : WhirldIn = null;	//Static Singleton Reference
    //static var M : WhirldIn = null;	//Static Singleton Reference
    static function get M () : WhirldIn {
		if (m == null) {
		    Debug.Log("Initializing WhirldIn GameObject");
		    var go : GameObject = new GameObject();
		    m = go.AddComponent(WhirldIn);
		    go.name = "_WhirldIn";
		    DontDestroyOnLoad(go);	//Ensure the GameObject host of this Singleton class survives scene changes
		}
		return m;
	}
}*/

/*static var S : WhirldIn = null;	//Static Singleton Reference
static function Init() {
	if (S != null) {
		//Debug.Log("Reinitializing WhirldController...");
		var go : GameObject = S.gameObject;
		DestroyImmediate(S);
	}
	else {
		//Debug.Log("Initializing WhirldController...");
    	go = new GameObject();
		DontDestroyOnLoad(go);	//Ensure the GameObject host of this Singleton class survives scene changes
		go.name = "_WhirldIn";
    }
    S = go.AddComponent(WhirldIn);
}*/
