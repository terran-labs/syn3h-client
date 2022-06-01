//using UnityEngine;
//using UnityEngine.UI;
//
//public class LobbyMessageText : MonoBehaviour
//{
//    public Text TextMessage;
//    public GameObject UpdateButton;
//
//    void Update()
//    {
//        var updateNeeded = CoreController.Instance.MyConfig.VersionLatest != "" && CoreController.Instance.MyConfig.VersionLatest != Application.version;
//
//        if (UpdateButton.activeInHierarchy != updateNeeded)
//        {
//            UpdateButton.SetActive(updateNeeded);
//        }
//
//        if (updateNeeded)
//        {
//            TextMessage.text = CoreController.Instance.MyConfig.LobbyMessageOutdated + "\n\nYou are running version: " + Application.version + "\nThe new version is: " +
//                               CoreController.Instance.MyConfig.VersionLatest + "";
//        }
//        else
//        {
//            TextMessage.text = CoreController.Instance.MyConfig.LobbyMessageLatest;
//        }
//    }
//}