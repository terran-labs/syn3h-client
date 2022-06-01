using UnityEngine;
using UnityEngine.UI;

public class UITextFromLocalizedMessaging : MonoBehaviour
{
    public string MessageKey = "";
    public string OfflineMessage = "";

    private Text _myText;

    void Start()
    {
        _myText = GetComponent<Text>();

        if (OfflineMessage == "")
        {
            OfflineMessage = _myText.text;
        }

        CoreLanguage.Instance.GetMessage(MessageKey, UpdateMessageCallback);
    }

    public void UpdateMessageCallback(string newMessage)
    {
        Debug.Log("UITextFromLocalizedMessaging :: UpdateMessageCallback (" + MessageKey + "): " + newMessage);

        if (newMessage == "")
        {
            newMessage = OfflineMessage;
        }

        _myText.text = newMessage;
    }
}