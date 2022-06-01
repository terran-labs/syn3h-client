//using UnityEngine;
//using UnityStandardAssets.ImageEffects;
//
//[ExecuteInEditMode]
//public class MiniMapMask : ImageEffectBase {
//	public Texture  mask;
//	void OnRenderImage (RenderTexture source, RenderTexture destination) {
//		material.SetTexture("_Mask", mask);
//		Graphics.Blit (source, destination, material);
//	}
//}