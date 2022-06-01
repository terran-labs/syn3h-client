using UnityEngine;
using System.Collections;

public class CoreCameraFade : MonoBehaviour
{
    public static CoreCameraFade Instance;
    public UnityEngine.Rendering.PostProcessing.PostProcessVolume MyPostProcessVolume;
    [System.NonSerialized] public static float TransitionTime = 1.5f;
    [System.NonSerialized] public float TransitionSpeed = 1.5f;
    public Animator LogoAnimator;

    private void Awake()
    {
        FadeMaskOut();
    }

    private void OnEnable()
    {
        Instance = this;
    }

    public static void FadeMaskOut()
    {
        if (!Instance || !Instance.gameObject.activeInHierarchy)
        {
            Debug.Log("CoreCameraFade :: FadeMaskOut called, but mask is inactive");
            return;
        }

        Instance.StartCoroutine("_FadeOut");
    }

    private IEnumerator _FadeOut()
    {
        StopCoroutine("_FadeIn");

        LogoAnimator.SetBool("logo_visible", true);

        MyPostProcessVolume.enabled = true;

        while (MyPostProcessVolume.weight > .01f)
        {
            MyPostProcessVolume.weight -= Time.deltaTime * TransitionSpeed;
            yield return null;
        }

        MyPostProcessVolume.weight = 0;
        MyPostProcessVolume.enabled = false;
    }

    public static void FadeMaskIn()
    {
        if (!Instance)
        {
            return;
        }

        Instance.StartCoroutine("_FadeIn");
    }

    private IEnumerator _FadeIn()
    {
        StopCoroutine("_FadeOut");

        LogoAnimator.SetBool("logo_visible", false);

        MyPostProcessVolume.enabled = true;

        while (MyPostProcessVolume.weight < .99f)
        {
            MyPostProcessVolume.weight += Time.deltaTime * TransitionSpeed;
            yield return null;
        }

        MyPostProcessVolume.weight = 1;
    }

    // Returns 0 when mask is hidden, and 1 when mask is blocking everything
    public float GetMaskStatus()
    {
        return MyPostProcessVolume.weight;
    }
}