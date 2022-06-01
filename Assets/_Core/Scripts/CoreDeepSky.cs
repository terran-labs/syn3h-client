// @deprecated

//using System;
//using UnityEngine;
//
//public class CoreDeepSky : MonoBehaviour
//{
////#if TRUESKY
////    private simul.trueSKY _trueSky;
////    #endif
//    private DeepSky.Haze.DS_HazeCore _deepSkyHazeCore;
//
//    void Start()
//    {
////#if TRUESKY
////        _trueSky = FindObjectOfType<simul.trueSKY>();
////
////        if (_trueSky)
////        {
////            Debug.LogWarning("CoreDeepSky :: Awake :: _trueSky found, activating time syncronization");
////        }
////        else
////        {
////            Debug.LogWarning("CoreDeepSky :: Awake :: _trueSky not found, disabling time syncronization");
////        }
////    #endif
//        _deepSkyHazeCore = GetComponent<DeepSky.Haze.DS_HazeCore>();
//
//        if (!_deepSkyHazeCore)
//        {
//            Debug.LogWarning("CoreDeepSky :: Awake ::  _deepSkyHazeCore not found");
//            this.gameObject.SetActive(false);
//        }
//    }
//
//    void Update()
//    {
//        if (!CoreWhirld.Instance)
//        {
//            return;
//        }
//
//        _deepSkyHazeCore.Time = CoreWhirld.Instance.GetTime();
//
////#if TRUESKY
////        if (_trueSky)
////        {
////            _deepSkyHazeCore.Time = _trueSky.time - (float) Math.Truncate(_trueSky.time);
////        }
////    #endif
//    }
//}