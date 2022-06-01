using UnityEngine;
using UnityEngine.UI;

public class LobbyLoadingProgressText : MonoBehaviour
{
    public Text TextVersion;
    public int SnarkyMessageUpdateSeconds = 5;

    private string _snarkyMessage = "";
    private float _snarkyMessageLastUpdated;

    void Update()
    {
        if (_snarkyMessage == "" || Time.fixedTime - _snarkyMessageLastUpdated > SnarkyMessageUpdateSeconds)
        {
            CoreLanguage.Instance.GetMessage("loading_message", UpdateMessageCallback);
            _snarkyMessageLastUpdated = Time.time;
        }

        var status = CoreWhirld.Instance.Progress;

        if (status == 1)
        {
            TextVersion.text = _snarkyMessage + "...";
        }
        else
        {
            TextVersion.text = "" + Mathf.Round(status * 100) + "%";
        }
    }

    public void UpdateMessageCallback(string newMessage)
    {
        _snarkyMessage = newMessage;
    }
}