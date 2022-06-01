using UnityEngine;
using UnityEngine.UI;

public class LobbyVersionText : MonoBehaviour {
    public Text textVersion;

    void Update () {
        textVersion.text = "build version " + Application.version;

        if (CoreController.Instance.MyConfig.Updated)
        {
            if (CoreController.Instance.MyConfig.VersionLatest == Application.version)
            {
                textVersion.text += " (latest)";
            }
            else
            {
                textVersion.text += "\n (" + CoreController.Instance.MyConfig.VersionLatest + " available)";
            }
        }
    }
}
