// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Analytics;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;

// public class LobbyScreens : MonoBehaviour
// {
//     public EventSystem MyEventSystem;
//     public GameObject ScreenIndex;
//     public GameObject ScreenLoading;
//     public GameObject ScreenHidden;
//     public float TransitionSpeed; // time, in seconds, it should take to transition between each slide
//     public float TransitionZOffset = -1000;
//     public float ActivationDelay = 3f;
//     public float MouseMoveSelectSomethingDelay = 5f;

//     private float _transitionZBase;
//     private RectTransform _transitionRect;
//     private CanvasGroup _transitionCanvasGroup;

//     // 0 = sitting pretty
//     // [0 => 1] = transitioning out
//     // [0 => -1] = transitioning back in
//     private float _transitionState;

//     private float _transitionStartTime; // time last transition was activated
//     private GameObject _activeScreen;
//     private GameObject _targetScreen;
//     private float _lastMouseMoveTimeSelectSomething;
//     private Vector3 _lastMousePositionSelectSomething;

//     private string _customWhirld;
// //    private CoreUiMode _lastUiMode;

//     void Awake()
//     {
//         _transitionRect = gameObject.GetComponent<RectTransform>();
//         _transitionCanvasGroup = gameObject.GetComponent<CanvasGroup>();

//         // Hide all screens, to provide a smooth fade-in transition for the lobby
//         foreach (Transform child in transform)
//         {
//             child.gameObject.SetActive(false);
//         }

//         _transitionCanvasGroup.alpha = 0;
//         Invoke("ActivateLobby", ActivationDelay);

//         InvokeRepeating("SelectSomething", 1.0f, 1.0f);
//     }

//     void Update()
//     {
//         // Automatically hide and show fullscreen UI when coreUiMode changes
//         var coreUiMode = CoreUi.Instance.GetUiMode();
// //        _lastUiMode = coreUiMode;

//         // automatically switch between index and loading screens when a world is being loaded
//         if (coreUiMode == CoreUiMode.Working && _targetScreen != ScreenLoading)
//         {
//             SwitchToScreen(ScreenLoading);
//         }
//         else if (coreUiMode != CoreUiMode.Working && _targetScreen == ScreenLoading)
//         {
//             SwitchToScreen(ScreenIndex, true);
//         }

//         // early-out if there are no active transitions
//         if (Math.Abs(_transitionStartTime) <= 0)
//         {
//             return;
//         }

//         var transitionTime = Time.time - _transitionStartTime;

//         // transition out
//         if (transitionTime < TransitionSpeed / 2)
//         {
//             _transitionRect.localPosition = new Vector3(_transitionRect.localPosition.x, _transitionRect.localPosition.y,
//                 easeInOutQuad(
//                     transitionTime, // t = Time (current)
//                     0, // b = Beginning (start value)
//                     TransitionZOffset, // c = Change (dest value)
//                     TransitionSpeed / 2 // d = Duration (desired in totality)
//                 ));

//             _transitionCanvasGroup.alpha = 1 - easeInOutQuad(
//                                                transitionTime, // t = Time (current)
//                                                0, // b = Beginning (start value)
//                                                1, // c = Change (dest value)
//                                                TransitionSpeed / 2 // d = Duration (desired in totality)
//                                            );
//         }

//         // flip to new carousel screen
//         else if (_activeScreen != _targetScreen)
//         {
//             foreach (Transform child in transform)
//             {
//                 if (child.gameObject == _targetScreen)
//                 {
//                     child.gameObject.SetActive(true);
//                 }
//                 else
//                 {
//                     child.gameObject.SetActive(false);
//                 }
//             }
//             _activeScreen = _targetScreen;
//         }

//         // transition back in, we are still within the transition time window
//         else if (transitionTime <= TransitionSpeed)
//         {
//             _transitionRect.localPosition = new Vector3(_transitionRect.localPosition.x, _transitionRect.localPosition.y,
//                 TransitionZOffset - easeOutQuad(
//                     transitionTime - (TransitionSpeed / 2), // t = Time (current)
//                     0, // b = Beginning (start value)
//                     TransitionZOffset, // c = Change (dest value)
//                     TransitionSpeed / 2 // d = Duration (desired in totality)
//                 ));

//             _transitionCanvasGroup.alpha = easeOutQuad(
//                 transitionTime - (TransitionSpeed / 2), // t = Time (current)
//                 0, // b = Beginning (start value)
//                 1, // c = Change (dest value)
//                 TransitionSpeed / 2 // d = Duration (desired in totality)
//             );
//         }

//         else
//         {
//             _transitionRect.localPosition = new Vector3(_transitionRect.localPosition.x, _transitionRect.localPosition.y,
//                 _transitionRect.localPosition.z);
//             _transitionCanvasGroup.alpha = 1;
//             _transitionStartTime = 0;
//         }
//     }

//     public void ActivateLobby()
//     {
//         Debug.Log("LobbyScreens :: ActivateLobby");
//         SwitchToScreen(ScreenIndex, true);
//     }

// // instruct gui carousel to flip to a particular screen
// // "" for index (screen 0)
//     public void SwitchToScreen(GameObject screen)
//     {
//         SwitchToScreen(screen, false);
//     }

//     public void SelectSomething()
//     {
//         if (!isActiveAndEnabled)
//         {
//             return;
//         }

//         if (MyEventSystem.currentSelectedGameObject != null && MyEventSystem.currentSelectedGameObject.activeInHierarchy)
//         {
//             return;
//         }

//         if (_lastMousePositionSelectSomething == null || Input.mousePosition != _lastMousePositionSelectSomething)
//         {
//             _lastMouseMoveTimeSelectSomething = Time.realtimeSinceStartup;
//             _lastMousePositionSelectSomething = Input.mousePosition;
//         }
//         if (Time.realtimeSinceStartup - _lastMouseMoveTimeSelectSomething < MouseMoveSelectSomethingDelay)
//         {
//             return;
//         }

//         // Select first selectable in new view, to ensure that keyboard controls remain functional after a scene switch
//         // @todo we shouldn't need to do this, since Unity should manage selectable focus automatically
//         if (Selectable.allSelectables.Count > 0)
//         {
//             Selectable.allSelectables[0].Select();
//         }

// //        Debug.Log("LobbyScreens :: SelectSomething :: Triggered");
//     }

//     public void SwitchToScreen(GameObject screen, bool fastTransition)
//     {
//         // ensure there isn't already a transition running before accepting a new one
// //        if (transitionStartTime > 0)
// //        {
// //            Debug.Log("CarouselController: ignoring stacked transitions");
// //        }

//         // Fast transitions fade the target screen in immediatley,
//         // instead of first waiting for the previous screen to fade out.
//         if (fastTransition)
//         {
//             _transitionStartTime = Time.time - TransitionSpeed * .5f;
//         }
//         else
//         {
//             _transitionStartTime = Time.time;
//         }

//         _targetScreen = screen;
//         Debug.Log("LobbyScreens: transitioning to " + screen.name);

//         Analytics.CustomEvent("core.lobbyScreen", new Dictionary<string, object>
//         {
//             {"screen", screen.name}
//         });
//     }

//     public void ReturnToGame()
//     {
//         CoreUi.Instance.ModeCurrent = CoreUiMode.HUD;
//     }

//     public void setCustomWhirld(string str)
//     {
//         _customWhirld = str;
//     }

//     public void StreamWhirldCustom()
//     {
//         StreamWhirld(_customWhirld);
//     }

//     public void StreamWhirld(string str)
//     {
//         //yield return new WaitForSeconds(1);
//         //Application.LoadLevel(id);
//         Debug.Log("Whirld :: Streaming: " + str);

//         // @todo stream strs with a protocol; designator, assume all other strs are scenes included in build
//         LoadScene(str);
//     }

//     public void StreamWhirldCancel()
//     {
//         Debug.Log("Whirld :: Cancel ...");
//     }

//     public void OpenUrl(string url)
//     {
//         Application.OpenURL(url);
//     }

//     public void LoadScene(string sceneName)
//     {
//         CoreController.Instance.LoadScene(sceneName);
//     }

//     public void LoadEarthLocation(string location)
//     {
//         // Attempt parsing string as lon,lat pair
//         try
//         {
//             string[] locationArr = location.Split(',');
//             if (locationArr.Length == 2)
//             {
//                 var locationLat = Convert.ToDouble(locationArr[0]);
//                 var locationLon = Convert.ToDouble(locationArr[1]);

//                 if (locationLat.Equals(0) && locationLon.Equals(0))
//                 {
//                     Debug.LogError(
//                         "CoreLobby :: LoadEarthLocation :: lat/lon parsing resulted in an apparently unanticipated visit to the south pole (location: " + location + ")");
// //                    return false;
//                     return;
//                 }

//                 LoadEarthLocationLatLon(locationLat, locationLon);
//                 return;
// //                return true;
//             }
//             Debug.LogError("CoreLobby :: LoadEarthLocation :: location doesn't appear to be a lat,lon pair");
//             return;
// //            return false;
//         }
//         catch (Exception e)
//         {
//             Debug.LogError("CoreLobby :: LoadEarthLocation :: lat/lon parsing failed (location: " + location + ") " + e.ToString());
//             return;
// //            return false;
//         }

//         Debug.LogError("CoreLobby :: LoadEarthLocation :: format not recognized (location: " + location + ")");
// //        return false;
//     }

//     public void LoadEarthLocationLatLon(double lat, double lon, string worldEngine = "")
//     {
//         Debug.Log("CoreLobby :: LoadEarthLocationLatLon :: Loading... (lat: " + lat + ", lon: " + lon + ")");

//         CoreSettings.Instance.GeoDestLat = lat;
//         CoreSettings.Instance.GeoDestLon = lon;

//         var worldSceneName = "Earth";
//         if (worldEngine != "")
//         {
//             worldSceneName += "_" + worldEngine;
//         }

//         CoreController.Instance.LoadScene(worldSceneName);
//     }

//     public void Exit()
//     {
//         Application.Quit();
//     }

// // easing functions! props: http://gizma.com/easing/
//     float easeInOutQuad(float t, float b, float c, float d)
//     {
//         t = t / (d / 2);
//         if (t < 1) return c / 2 * t * t + b;
//         t--;
//         return -c / 2 * (t * (t - 2) - 1) + b;
//     }

//     float easeOutQuad(float t, float b, float c, float d)
//     {
//         t = t / d;
//         return -c * t * (t - 2) + b;
//     }
// }