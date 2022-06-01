using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CoreScreenshot : MonoBehaviour
{
//	public bool IsCapturingScreenshot { get; private set; }
//	private int _captureState;
//	private int _originalQualityLevel;

    public AudioClip screenshotSound;

    public string OutputDirectory { get; private set; }

    void Start()
    {
        OutputDirectory = Application.persistentDataPath + "/Screenshots/";
    }

    void LateUpdate()
    {
        if (Input.GetButtonDown("Screenshot"))
        {
            Capture();
        }
    }

    public void Capture()
    {
        _playSound();

        var fileName = OutputDirectory + "Core";
        var resWidth = Screen.width;
        var resHeight = Screen.height;
        var scale = (resWidth > 3000 ? 1 : 2);
        var camera = Camera.main;
        var tFormat = TextureFormat.RGB24;

        string date = System.DateTime.Now.ToString();
        date = "_" + date.Replace("/", "-");
        date = date.Replace(" ", "_");
        date = date.Replace("\\", "_");
        date = date.Replace(":", "-");
        fileName += date;
        fileName += "_" + Random.Range(0, 1000);
        fileName += ".jpg";
        fileName = fileName.Replace("&", "_");

        int resWidthN = resWidth * scale;
        int resHeightN = resHeight * scale;

        RenderTexture rt = new RenderTexture(resWidthN, resHeightN, 24);
        camera.targetTexture = rt;
        camera.Render();
        RenderTexture.active = rt;

        var screenShot = new Texture2D(resWidthN, resHeightN, tFormat, false);
        screenShot.ReadPixels(new Rect(0, 0, resWidthN, resHeightN), 0, 0);
        camera.targetTexture = null;
        RenderTexture.active = null;

        byte[] bytes = screenShot.EncodeToJPG();

        System.IO.File.WriteAllBytes(fileName, bytes);

        Debug.Log("ScreenShot :: New shot saved to " + fileName);
    }

    public void OpenOutputDirectory()
    {
        Debug.Log("CoreScreenshot :: Opening screenshot output dir: " + OutputDirectory);
//		Application.OpenURL("file://" + OutputDirectory);
        var path = OutputDirectory.TrimEnd(new[] {'\\', '/'}); // Mac doesn't like trailing slash
        Process.Start(path);
    }

    private void _playSound()
    {
        StartCoroutine(_destroyIfNotPlaying());
    }

    private IEnumerator _destroyIfNotPlaying()
    {
        AudioSource sc = gameObject.AddComponent<AudioSource>();
        sc.clip = screenshotSound;
        sc.loop = false;
        sc.Play();

        while (sc.isPlaying)
        {
            yield return new WaitForSeconds(0.1f);
        }

#if !UNITY_EDITOR
			Destroy(sc);
		#endif
#if UNITY_EDITOR
        DestroyImmediate(sc);
#endif

        yield return 0;
    }

//
//	private IEnumerator OnPostRender()
//	{
//		if (!IsCapturingScreenshot)
//		{
//			_captureState = 0;
//			return;
//		}
//
//		yield return new WaitForEndOfFrame();
//
//		// Set up screenshot
//		if (_captureState == 0)
//		{
//			_originalQualityLevel
//
//		}
//
//		// Set up screenshot
//		if (_captureState == 3)
//		{
//			QualitySettings.SetQualityLevel(iQ, true);
//
//		}
//	}
}