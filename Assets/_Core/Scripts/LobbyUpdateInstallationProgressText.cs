using UnityEngine;
using UnityEngine.UI;

public class LobbyUpdateInstallationProgressText : MonoBehaviour {
    public Text TextVersion;
    public CoreUpdate CoreUpdate;

    void  Update () {
        TextVersion.text = "" + Mathf.Round(CoreUpdate.UpdateDownloadStatus * 100) + "%";
    }
}