// @aubhere deprecating GameSparks

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CoreLanguage : MonoBehaviour
{
    public static CoreLanguage Instance { get; private set; }

    private string _languageKey = "lang_en";
    private float _maxLoadWaitTime = 10f;

//    private GameSparks.Core.GSData _propertyData;

    public delegate void CoreLanguageUpdate(string newMessage);

    private void OnEnable()
    {
        Instance = this;
    }

    void Start()
    {
        if (!CoreController.Instance)
        {
            Debug.Log("CoreLanguage :: Start :: Aborting - no controller detected");
            return;
        }

//        CoreController.Instance.OnGameSparksRegistered += _updateProperties;

        if (CoreController.Instance.IsGameSparksAlreadyRegistered)
        {
            _updateProperties();
        }
    }

    public void GetMessage(string messageKey, CoreLanguageUpdate updateCallback)
    {
        StartCoroutine(_getMessage(messageKey, updateCallback));
    }

    private IEnumerator _getMessage(string messageKey, CoreLanguageUpdate updateCallback)
    {
        string messageValue = "";

//        while (_propertyData == null && Time.timeSinceLevelLoad < _maxLoadWaitTime)
//        {
//            yield return true;
//        }
//
//        if (_propertyData != null)
//        {
//            if (messageKey == "loading_message" || messageKey == "teaser_message")
//            {
//                var messageValues = _propertyData.GetStringList(messageKey + "s");
//                if (messageValues.Count > 0)
//                {
//                    Random.seed = System.Environment.TickCount;
//                    messageValue = messageValues[Random.Range(0, messageValues.Count)];
//                }
//            }
//
//            else
//            {
//                messageValue = _propertyData.GetString(messageKey);
//            }
//        }
        
        yield return new WaitForSeconds(1);

        updateCallback(messageValue);
    }

    private void _updateProperties()
    {
//        new GameSparks.Api.Requests.GetPropertyRequest()
//            .SetPropertyShortCode(_languageKey)
//            .Send(response =>
//            {
//                Debug.Log("CoreLanguage :: GetPropertyRequest");
//
//                _propertyData = response.Property;
//            });
    }
}