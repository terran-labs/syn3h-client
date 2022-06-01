using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreRuntime : MonoBehaviour
{
    public static CoreRuntime Instance { get; private set; }
    
    void OnEnable()
    {
        // Suicide in case there is already an active Core object.
        // This can occur if we are testing, and leave a _Controller prefab in the lobby scene for convenience 
        if (Instance != null)
        {
            Debug.Log("CoreRuntime :: Detected existing instance, self-destruct initiated...");
            DestroyImmediate(this.gameObject, false);
            return;
        }

        Instance = this;
        
        // Immortalize ourself
        DontDestroyOnLoad(gameObject);
    }
}