// using UnityEngine;

// public class CoreAudio : MonoBehaviour
// {
//     public AudioSource MyAudio;
//     public float VolumeTransitionSpeed;

//     private CoreUi coreUi;

//     void Update()
//     {
//         if (!coreUi)
//         {
//             coreUi = CoreUi.Instance;
//         }

//         if (!coreUi)
//         {
//             return;
//         }

//         // Follow main camera
//         if (Camera.main)
//         {
//             transform.position = Camera.main.transform.position;
//         }

//         // Title track :: determine desired volume
//         var UiMode = coreUi.GetUiMode();

//         int desiredVolume = UiMode == CoreUiMode.Fullscreen || UiMode == CoreUiMode.Working ? 1 : 0;

//         // Title track :: pause when volume is 0
//         if (MyAudio.volume.Equals(0f))
//         {
//             if (MyAudio.isPlaying)
//             {
//                 MyAudio.Pause();
//             }
//         }
//         else
//         {
//             if (MyAudio.isPlaying == false)
//             {
//                 MyAudio.Play();
//             }
//         }

//         // Title track :: smoothly transition to desired volume
//         if (!MyAudio.volume.Equals(desiredVolume))
//         {
//             MyAudio.volume += Time.deltaTime * VolumeTransitionSpeed * (MyAudio.volume > desiredVolume ? -1 : 1);
//             MyAudio.volume = Mathf.Clamp(MyAudio.volume, 0, 1);
//         }
//     }
// }