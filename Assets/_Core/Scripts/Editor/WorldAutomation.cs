// using System.Linq;
// using UnityEngine;
// using UnityEditor;
// //using _Decal;

// public class WorldAutomation : MonoBehaviour
// {
//     [MenuItem("Automation/Suspend Asset Importing")]
//     public static void Suspend()
//     {
//         AssetDatabase.StartAssetEditing();
//     }

//     [MenuItem("Automation/Resume Asset Importing")]
//     public static void Resume()
//     {
//         AssetDatabase.StopAssetEditing();
//         AssetDatabase.Refresh();
//     }

//     [MenuItem("Automation/Eliminate empty objects in scene")]
//     static void DeleteEmptyObjects()
//     {
//         GameObject[] gos = FindObjectsOfType(typeof(GameObject)) as GameObject[];

//         if (gos == null)
//         {
//             return;
//         }

//         foreach (var go in gos)
//         {
//             if (go.transform.childCount > 0)
//             {
//                 continue;
//             }

//             Component[] allComponents = go.GetComponents<Component>();
//             if (allComponents.Length > 1)
//             {
//                 continue;
//             }

//             Undo.RecordObject(go, "Deleted empty gameobject");
//             DestroyImmediate(go);
//         }
//     }

// //    [MenuItem("Automation/RebuildAllDecals")]
// //    static void RebuildAllDecals()
// //    {
// //        GameObject[] gos = FindObjectsOfType(typeof(GameObject)) as GameObject[];
// //
// //        foreach (var go in gos)
// //        {
// //            var decal = go.GetComponent<Decal>();
// //            if (!decal)
// //            {
// //                continue;
// //            }
// //
// //            Undo.RecordObject(go, "Rebuilt Kdecal");
// //            DecalBuilder.BuildAndSetDirty(decal);
// //        }
// //    }

//     [MenuItem("Automation/Snap select objects to nearest surface")]
//     static void SnapSelectionToNearestSurface()
//     {
//         var maxSnapDistance = 50f;
//         var desiredSnapDistance = .05f;

//         var gos = Selection.gameObjects;
//         foreach (var go in gos)
//         {
//             var startPosition = go.transform.position;
//             var bounds = new Bounds(startPosition, new Vector3(maxSnapDistance, maxSnapDistance, maxSnapDistance));
//             var meshRenderers = FindObjectsOfType<MeshRenderer>()
//                 .Where(obj => obj.gameObject != go)
// //                .Where(obj => obj.gameObject.isStatic)
// //                .Where( obj => HasLayer( decal.affectedLayers, obj.gameObject.layer ) )
// //                .Where( obj => obj.GetComponent<Decal>() == null )
//                 .Where(obj => bounds.Intersects(obj.bounds))
// //                .Select(obj => obj.GetComponent<MeshFilter>())
// //                .Where(obj => obj != null && obj.sharedMesh != null)
//                 .ToArray();

//             Debug.DrawRay(startPosition, Vector3.up * 10, Color.green, 5, false);

//             bool closestFound = false;
//             BaryCentricDistance.Result closestResult = new BaryCentricDistance.Result();
//             GameObject closestObject = new GameObject();
//             foreach (var meshRenderer in meshRenderers)
//             {
//                 var meshFilter = meshRenderer.gameObject.GetComponent<MeshFilter>();
//                 if (meshFilter.sharedMesh == null)
//                 {
//                     continue;
//                 }

//                 var closestPointCalculator = new BaryCentricDistance(meshFilter);
//                 var result = closestPointCalculator.GetClosestTriangleAndPoint(meshRenderer.gameObject.transform.InverseTransformPoint(startPosition));

//                 Debug.Log("SnapSelectionToNearestSurface :: Evaluating " + go.name + " :: " + result.distance + " meters to " + meshFilter.name);

//                 if (closestFound && result.distance > closestResult.distance)
//                 {
// //                    Debug.DrawRay(result.closestPoint, result.normal * 10, Color.yellow, 5, false);
//                     continue;
//                 }

//                 closestFound = true;
//                 closestResult = result;
//                 closestObject = meshRenderer.gameObject;
//             }

//             if (!closestFound)
//             {
//                 continue;
//             }

//             Debug.DrawRay(closestObject.transform.TransformPoint(closestResult.vert1), closestObject.transform.TransformDirection(closestResult.normal) * 10, Color.yellow, 10,
//                 false);
//             Debug.DrawRay(closestObject.transform.TransformPoint(closestResult.vert2), closestObject.transform.TransformDirection(closestResult.normal) * 10, Color.yellow, 10,
//                 false);
//             Debug.DrawRay(closestObject.transform.TransformPoint(closestResult.vert3), closestObject.transform.TransformDirection(closestResult.normal) * 10, Color.yellow, 10,
//                 false);

//             var newPosition = closestObject.transform.TransformPoint(closestResult.closestPoint) +
//                               (closestObject.transform.TransformDirection(closestResult.normal) * -desiredSnapDistance);
//             Debug.DrawRay(closestResult.closestPoint, Vector3.up, Color.cyan, 10, false);
//             Debug.DrawRay(newPosition, Vector3.up, Color.red, 10, false);

// //            Undo.RecordObject(go, "Snapped gameobject to nearest surface");
// //            go.transform.position = newPosition;

// //            Debug.DrawRay(go.transform.position, Vector3.up * 10, Color.blue, 5, false);
// //            var hits = Physics.SphereCastAll(go.transform.position, maxSnapRadius, Vector3.up, 0);
// //
// //            Debug.Log(hits.Length + " hits found for " + go.name);
// //
// //            RaycastHit closest = new RaycastHit();
// //            foreach (var hit in hits)
// //            {
// //                var point = hit.collider.ClosestPoint(go.transform.position);
// //                var distance = Vector3.Distance(hit.point, go.transform.position);
// //
// //                if (closest.collider && distance > closest.distance)
// //                {
// //                    continue;
// //                }
// //
// //                closest = hit;
// //                closest.point = point;
// //                closest.distance = distance;
// //            }
// //
// //            if (!closest.collider)
// //            {
// //                continue;
// //            }
// //
// //            var vector = Vector3.Normalize(go.transform.position - closest.point);
// //            var newPosition = closest.point + (vector * desiredSnapDistance);
// //
// //            Debug.DrawRay(go.transform.position, Vector3.forward, Color.red, 10, false);
// //            Debug.DrawRay(closest.point, Vector3.up, Color.green, 10, false);
// //            Debug.DrawRay(newPosition, Vector3.left, Color.yellow, 10, false);
// //
// //            Debug.Log("Closest hit for " + go.name + " was " + closest.distance + " meters, against " + closest.collider.name);
// //
// //
// ////            go.transform.position = newPosition;
// //            Undo.RecordObject(go, "Snapped gameobject to nearest surface");
//         }
//     }

//     [MenuItem("Automation/Adjust range of all lights in scene")]
//     static void AdjustSceneLightRanges()
//     {
//         Light[] lights = FindObjectsOfType(typeof(Light)) as Light[];

//         if (lights == null)
//         {
//             return;
//         }

//         foreach (var light in lights)
//         {
//             var newRange = light.range * .5f;

//             Undo.RecordObject(light, "Adjusted light range");
//             light.range = newRange;
//         }
//     }

//     [MenuItem("Automation/Reconstruct all lights in scene")]
//     static void ReconstructSceneLights()
//     {
//         Light[] lights = FindObjectsOfType(typeof(Light)) as Light[];

//         if (lights == null)
//         {
//             return;
//         }

//         foreach (var light in lights)
//         {
//             var lightObject = light.gameObject;

//             Undo.RecordObject(lightObject, "Cloned light component");

//             Instantiate(lightObject, lightObject.transform.parent);
//             DestroyImmediate(light);

//             Debug.Log("light reconstructed");
//         }
//     }
// }