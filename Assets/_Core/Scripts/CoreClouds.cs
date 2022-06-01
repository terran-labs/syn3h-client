//using UnityEngine;
//using kode80.Clouds;
//using RenderSettings = UnityEngine.RenderSettings;
//
//public class CoreClouds : MonoBehaviour
//{
//	public kode80Clouds kode80Clouds;
//
//	void Awake()
//	{
//		kode80Clouds.targetCamera = Camera.main;
//	}
//
//	void Update()
//	{
//		var qualityLevel = QualitySettings.GetQualityLevel();
//
////        if (!_coreCamera.CoreController.IsWorldLoaded)
////        {
////            kode80Clouds.maxIterations = 0;
////            return;
////        }
//
//		if (qualityLevel >= 5)
//		{
//			kode80Clouds.maxIterations = 128;
//		}
//		else if (qualityLevel >= 4)
//		{
//			kode80Clouds.maxIterations = 32;
//		}
//		else if (qualityLevel >= 3)
//		{
//			kode80Clouds.maxIterations = 8;
//		}
//		else
//		{
//			kode80Clouds.maxIterations = 0;
//		}
//
//		if (kode80Clouds.maxIterations > 0)
//		{
//			kode80Clouds.cloudTopColor = RenderSettings.ambientSkyColor;
//			kode80Clouds.cloudBaseColor = RenderSettings.ambientEquatorColor;
//			kode80Clouds.sunScalar = Mathf.Lerp(0, 1, (RenderSettings.sun.color.a - 0.5f) * 2);
//		}
//	}
//}