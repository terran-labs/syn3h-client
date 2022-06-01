// using System;
// using System.Collections;
// using System.Text;
// using UnityEngine;
// using UnityEngine.UI;

// public class LobbyCustomLocationSearch : MonoBehaviour
// {
//     public GameObject NoSearchObject;
//     public GameObject SearchMatchesObject;
//     public GameObject SearchNoMatchesObject;
//     public GameObject SearchLoadingObject;
//     public GameObject SearchLoadingFailedObject;
//     public InputField SearchInputField;
//     public GameObject PrefabButtonMatchingLocation;
//     public LobbyScreens MyLobby;
//     public float AutoSearchDelaySeconds = 0.5f;

//     private enum CustomLocationSearchState
//     {
//         NoSearch,
//         SearchMatches,
//         SearchNoMatches,
//         Loading,
//         LoadingFailed
//     }

//     private CustomLocationSearchState searchState = CustomLocationSearchState.NoSearch;
//     private string lastSearchString;
//     private float lastSearchUpdate;

//     void Update()
//     {
//         // Autosearch Watching
//         if (lastSearchString != SearchInputField.text)
//         {
//             lastSearchUpdate = Time.realtimeSinceStartup;
//             lastSearchString = SearchInputField.text;
//         }

//         // Autosearch Triggering
//         if (lastSearchUpdate > 0 && Time.realtimeSinceStartup - lastSearchUpdate > AutoSearchDelaySeconds)
//         {
//             lastSearchUpdate = 0;
//             StartCoroutine("UpdateSearchResults", SearchInputField.text);
//         }

//         // View state
//         NoSearchObject.SetActive(searchState == CustomLocationSearchState.NoSearch);
//         SearchMatchesObject.SetActive(searchState == CustomLocationSearchState.SearchMatches);
//         SearchNoMatchesObject.SetActive(searchState == CustomLocationSearchState.SearchNoMatches);
//         SearchLoadingObject.SetActive(searchState == CustomLocationSearchState.Loading);
//         SearchLoadingFailedObject.SetActive(searchState == CustomLocationSearchState.LoadingFailed);
//     }

//     IEnumerator UpdateSearchResults(String input)
//     {
//         Debug.Log("LobbyCustomLocationSearch :: UpdateSearchResults :: Initiated (s: " + SearchInputField.text + ")");

//         // Early-out if no search query is specified
//         if (input == "")
//         {
//             searchState = CustomLocationSearchState.NoSearch;
//             yield break;
//         }

//         var url = new StringBuilder("https://maps.googleapis.com/maps/api/place/autocomplete/xml?sensor=false");
//         url.Append("&input=").Append(OnlineMapsWWW.EscapeURL(input));
//         url.Append("&key=").Append(CoreController.Instance.MyConfig.ApiKeyGoogleMaps);

// //        if (lnglat != default(Vector2)) url.AppendFormat("&location={0},{1}", lnglat.y, lnglat.x);
// //        if (radius != -1) url.Append("&radius=").Append(radius);
// //        if (offset != -1) url.Append("&offset=").Append(offset);
// //        if (!string.IsNullOrEmpty(types)) url.Append("&types=").Append(types);
// //        if (!string.IsNullOrEmpty(components)) url.Append("&components=").Append(components);
// //        if (!string.IsNullOrEmpty(language)) url.Append("&language=").Append(language);

//         WWW www = new WWW(url.ToString());
//         yield return www;
//         if (www.error != null && www.error != "")
//         {
//             searchState = CustomLocationSearchState.NoSearch;
//             Debug.Log("LobbyCustomLocationSearch :: UpdateSearchResults :: WWW Error ( " + www.error + ")");
//             yield break;
//         }

//         // Parse response
//         OnlineMapsGooglePlacesAutocompleteResult[] results = OnlineMapsGooglePlacesAutocomplete.GetResults(www.text);

//         if (results == null)
//         {
//             Debug.Log("LobbyCustomLocationSearch :: UpdateSearchResults :: no results found");
//             searchState = CustomLocationSearchState.SearchNoMatches;
//             yield break;
//         }

//         Debug.Log("LobbyCustomLocationSearch :: UpdateSearchResults :: Complete (results: " + results.Length + ")");

//         ClearLocationMatches();
//         searchState = CustomLocationSearchState.SearchMatches;

//         // Log description of each result.
//         foreach (OnlineMapsGooglePlacesAutocompleteResult result in results)
//         {
//             var obj = Instantiate(PrefabButtonMatchingLocation, SearchMatchesObject.transform);
//             obj.name = result.description;
// //            obj.transform.parent = SearchMatchesObject.transform;
// //            obj.transform.localScale = Vector3.one;
// //            obj.transform.localRotation = Quaternion.identity;

//             // @dragonhere toggle object activation to reset animation root scale.
//             // This shouldn't be necessary
// //            obj.SetActive(false);
// //            obj.SetActive(true);

//             var objButton = obj.GetComponent<Button>();
//             objButton.onClick.AddListener(delegate { OpenLocation(result.description); });
//         }

//         // Poll the Google Places Autocomplete API
// //        OnlineMapsGooglePlacesAutocomplete.Find(
// //            SearchInputField.text,
// //            CoreController.Instance.MyConfig.ApiKeyGoogleMaps
// //        ).OnComplete += OnCompleteLocationSearch;
//     }

//     public void OpenLocation(String location)
//     {
//         searchState = CustomLocationSearchState.Loading;

//         StartCoroutine("_openLocation", location);
//     }

//     IEnumerator _openLocation(String location)
//     {
//         Debug.Log("LobbyCustomLocationSearch :: _openLocation :: Initiated (location: " + location + ")");

//         StringBuilder url = new StringBuilder("https://maps.googleapis.com/maps/api/geocode/xml?sensor=false");
//         if (!string.IsNullOrEmpty(location)) url.Append("&address=").Append(OnlineMapsWWW.EscapeURL(location));
// //        if (!string.IsNullOrEmpty(latlng)) url.Append("&latlng=").Append(latlng.Replace(" ", ""));
// //        if (!string.IsNullOrEmpty(lang)) url.Append("&language=").Append(lang);

//         WWW www = new WWW(url.ToString());
//         yield return www;
//         if (www.error != null && www.error != "")
//         {
//             searchState = CustomLocationSearchState.NoSearch;
//             Debug.Log("LobbyCustomLocationSearch :: _openLocation :: WWW Error ( " + www.error + ")");
//             yield break;
//         }

//         Vector2 position = OnlineMapsGoogleGeocoding.GetCoordinatesFromResult(www.text);

//         MyLobby.LoadEarthLocationLatLon(position.y, position.x);

// //        if (!success)
// //        {
// //            searchState = CustomLocationSearchState.LoadingFailed;
// //        }
// //        OnlineMapsGoogleGeocoding query = OnlineMapsGoogleGeocoding.Find("Chicago");
//     }

//     private void ClearLocationMatches()
//     {
//         foreach (Transform child in SearchMatchesObject.transform)
//         {
//             Destroy(child.gameObject);
//         }
//     }
// }