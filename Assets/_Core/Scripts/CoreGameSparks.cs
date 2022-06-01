// @auhere deprecating GameSparks

// //using System.Collections;
//using UnityEngine;
//using GameSparks;
//using GameSparks.Api;
//using GameSparks.Api.Messages;
//using GameSparks.Api.Requests;
//using GameSparks.Api.Responses;
//using GameSparks.Core;
//
////using GameSparks.Platforms;
////using GameSparks.Platforms.WebGL;
////using GameSparks.Platforms.Native;
//
//public class CoreGameSparks : MonoBehaviour
//{
//    public CoreController myController;
//    private bool _initialized = false;
//
//    void Start()
//    {
////        PlatformInit();
//
//        // Reference: https://support.gamesparks.net/support/discussions/topics/1000070243
//        GameSparks.Core.GS.GameSparksAvailable += GsServiceHandler;
//    }
//
//    void GsServiceHandler(bool available)
//    {
//        if (_initialized)
//        {
//            return;
//        }
//        _initialized = true;
//
//        if (!available)
//        {
//            Debug.LogError("gs service connection lost");
//        }
//        else
//        {
//            // Initially authenticate as a device, ensuring that we can gather location messages and other prefs as soon as possible
//            DeviceAuth();
//        }
//    }
//
////    void PlatformInit()
////    {
////        Debug.Log("CoreGameSparks :: PlatformInit");
////#if ((UNITY_SWITCH || UNITY_XBOXONE || (WINDOWS_UWP && ENABLE_IL2CPP)) && !UNITY_EDITOR) || GS_FORCE_NATIVE_PLATFORM
////        gameObject.AddComponent<NativePlatform>();
////#elif UNITY_WEBGL && !UNITY_EDITOR
////        gameObject.AddComponent<WebGLPlatform>();
////#else
////        gameObject.AddComponent<DefaultPlatform>();
////#endif
////    }
//
//    void DeviceAuth()
//    {
//        Debug.Log("CoreGameSparks :: DeviceAuth :: Authenticating...");
//        new DeviceAuthenticationRequest().Send((response) =>
//        {
//            if (response.HasErrors)
//            {
//                Debug.Log("CoreGameSparks :: DeviceAuth :: Failure");
//            }
//            else
//            {
//                Debug.Log("CoreGameSparks :: DeviceAuth :: Success");
//                Debug.Log("CoreGameSparks :: DeviceAuth :: UserId:" + response.UserId);
//                Debug.Log("CoreGameSparks :: DeviceAuth :: HasErrors:" + response.HasErrors);
//                Debug.Log("CoreGameSparks :: DeviceAuth :: JSON:" + response.JSONString);
//                myController.TriggerOnGameSparksRegistered();
//            }
//        }); // , 10 (Apparently, timeouts are no longer supported)
//    }
//}