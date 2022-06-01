// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class CoreCameraUnderwater : MonoBehaviour
// {
//     public Shader HydroformUnderwaterShader;
//     private Material hydroformUnderwaterMaterial;

//     void Start()
//     {
//         if (hydroformUnderwaterMaterial == null)
//         {
//             hydroformUnderwaterMaterial = new Material(HydroformUnderwaterShader);
//             hydroformUnderwaterMaterial.hideFlags = HideFlags.HideAndDontSave;
//         }
//     }

//     void OnDestroy()
//     {
//         if (hydroformUnderwaterMaterial != null)
//         {
//             DestroyImmediate(hydroformUnderwaterMaterial);
//         }
//     }

//     void OnPreCull()
//     {
//         // Hydroform ocean management
//         // (This functionality replicates the Hydroform MultiCamComp system, in a slightly cleaner way)
//         if (CoreHydroform.Instance && CoreHydroform.Instance.Hydroform && Application.isPlaying)
//         {
//             CoreHydroform.Instance.Hydroform.UpdateCamData();
//             CoreHydroform.Instance.Hydroform.UpdateReflection();
//             CoreHydroform.Instance.Hydroform.DrawMeshes(CoreCamera.Instance.ThisCamera);
//             CoreHydroform.Instance.Hydroform.UpdateUnderwaterCam();
//         }
//     }

//     void OnRenderImage(RenderTexture source, RenderTexture destination)
//     {
//         if (!source)
//         {
//             Debug.Log("CoreCamera :: OnRenderImage :: skipping due to null source");
//         }

//         // Hydroform Ocean system
//         // (code borrowed from Hydroform.UnderwaterFilter

//         if (hydroformUnderwaterMaterial == null) return;

//         Transform camtr = CoreCamera.Instance.ThisCamera.transform;
//         float camNear = CoreCamera.Instance.ThisCamera.nearClipPlane;
//         float camFar = CoreCamera.Instance.ThisCamera.farClipPlane;
//         float camFov = CoreCamera.Instance.ThisCamera.fieldOfView;
//         float camAspect = CoreCamera.Instance.ThisCamera.aspect;

//         Matrix4x4 frustumCorners = Matrix4x4.identity;
//         float fovWHalf = camFov * 0.5f;

//         Vector3 toRight = camtr.right * camNear * Mathf.Tan(fovWHalf * Mathf.Deg2Rad) * camAspect;
//         Vector3 toTop = camtr.up * camNear * Mathf.Tan(fovWHalf * Mathf.Deg2Rad);

//         Vector3 topLeft = (camtr.forward * camNear - toRight + toTop);
//         float camScale = topLeft.magnitude * camFar / camNear;

//         topLeft.Normalize();
//         topLeft *= camScale;

//         Vector3 topRight = (camtr.forward * camNear + toRight + toTop);
//         topRight.Normalize();
//         topRight *= camScale;

//         Vector3 bottomRight = (camtr.forward * camNear + toRight - toTop);
//         bottomRight.Normalize();
//         bottomRight *= camScale;

//         Vector3 bottomLeft = (camtr.forward * camNear - toRight - toTop);
//         bottomLeft.Normalize();
//         bottomLeft *= camScale;

//         frustumCorners.SetRow(0, topLeft);
//         frustumCorners.SetRow(1, topRight);
//         frustumCorners.SetRow(2, bottomRight);
//         frustumCorners.SetRow(3, bottomLeft);

//         //			var camPos= camtr.position;
//         hydroformUnderwaterMaterial.SetMatrix("_FrustumCornersWS", frustumCorners);
//         hydroformUnderwaterMaterial.SetVector("_CameraWS", CoreCamera.Instance.ThisCamera.transform.position);

//         if (CoreHydroform.Instance && CoreHydroform.Instance.Hydroform)
//         {
//             var camPos = camtr.position;
//             float waterHeight = CoreHydroform.Instance.Hydroform.waveSettings.waterHeight + 1;
//             float FdotC = camPos.y - waterHeight;
//             float paramK = (FdotC <= 0.0f ? 1.0f : 0.0f);
//             float heightDensity = CoreHydroform.Instance.Hydroform.underwater.fogHeightDensity;
//             hydroformUnderwaterMaterial.SetVector("_HeightParams", new Vector4(waterHeight, FdotC, paramK, heightDensity));

//             hydroformUnderwaterMaterial.SetColor("_UnderwaterFogTopColor", CoreHydroform.Instance.Hydroform.underwater.fogTop);
//             hydroformUnderwaterMaterial.SetColor("_UnderwaterFogBottomColor", CoreHydroform.Instance.Hydroform.underwater.fogBottom);
//             hydroformUnderwaterMaterial.SetColor("_UnderwaterOverlayColor", CoreHydroform.Instance.Hydroform.underwater.overlayColor);
//             hydroformUnderwaterMaterial.SetColor("_UnderwaterLipColor", CoreHydroform.Instance.Hydroform.underwater.lipColor);

//             Vector4 fogData = new Vector4(CoreHydroform.Instance.Hydroform.underwater.fogDensity, 0, 0, 0);
//             hydroformUnderwaterMaterial.SetVector("_UnderwaterData", fogData);


//             Debug.Log("---");
//             Debug.Log(waterHeight);
//             Debug.Log(FdotC);
//             Debug.Log(paramK);
//             Debug.Log(heightDensity);
//             Debug.Log(fogData);
//         }

//         //            Graphics.Blit(source, destination, hydroformUnderwaterMaterial, 0 );
//         CustomGraphicsBlit(source, destination, hydroformUnderwaterMaterial, 0);
//     }

//     static void CustomGraphicsBlit(RenderTexture source, RenderTexture dest, Material fxMaterial, int passNr)
//     {
//         RenderTexture.active = dest;

//         fxMaterial.SetTexture("_MainTex", source);

//         GL.PushMatrix();
//         GL.LoadOrtho();

//         fxMaterial.SetPass(passNr);

//         GL.Begin(GL.QUADS);

//         GL.MultiTexCoord2(0, 0.0f, 0.0f);
//         GL.Vertex3(0.0f, 0.0f, 3.0f); // BL

//         GL.MultiTexCoord2(0, 1.0f, 0.0f);
//         GL.Vertex3(1.0f, 0.0f, 2.0f); // BR

//         GL.MultiTexCoord2(0, 1.0f, 1.0f);
//         GL.Vertex3(1.0f, 1.0f, 1.0f); // TR

//         GL.MultiTexCoord2(0, 0.0f, 1.0f);
//         GL.Vertex3(0.0f, 1.0f, 0.0f); // TL

//         GL.End();
//         GL.PopMatrix();
//     }
// }