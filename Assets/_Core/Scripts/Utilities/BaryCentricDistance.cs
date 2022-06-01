using UnityEngine;

// Props to Whidoidoit
// from http://answers.unity3d.com/questions/424974/nearest-point-on-mesh.html
public class BaryCentricDistance
{
    public BaryCentricDistance(MeshFilter meshfilter)
    {
        _meshfilter = meshfilter;
        _mesh = _meshfilter.sharedMesh;
        _triangles = _mesh.triangles;
        _vertices = _mesh.vertices;
        _transform = meshfilter.transform;
    }

    public struct Result
    {
        public float distanceSquared;

        public float distance
        {
            get { return Mathf.Sqrt(distanceSquared); }
        }

        public int triangle;
        public Vector3 vert1;
        public Vector3 vert2;
        public Vector3 vert3;
        public Vector3 normal;
        public Vector3 centre;
        public Vector3 closestPoint;
    }

    int[] _triangles;
    Vector3[] _vertices;
    Mesh _mesh;
    MeshFilter _meshfilter;
    Transform _transform;

    public Result GetClosestTriangleAndPoint(Vector3 point)
    {
        point = _transform.InverseTransformPoint(point);
        var minDistance = float.PositiveInfinity;
        var finalResult = new Result();
        var length = (int) (_triangles.Length / 3);
        for (var t = 0; t < length; t++)
        {
            var result = GetTriangleInfoForPoint(point, t);
            if (minDistance > result.distanceSquared)
            {
                minDistance = result.distanceSquared;
                finalResult = result;
            }
        }
        finalResult.centre = _transform.TransformPoint(finalResult.centre);
        finalResult.closestPoint = _transform.TransformPoint(finalResult.closestPoint);
        finalResult.normal = _transform.TransformDirection(finalResult.normal);
        finalResult.distanceSquared = (finalResult.closestPoint - point).sqrMagnitude;
        return finalResult;
    }

    Result GetTriangleInfoForPoint(Vector3 point, int triangle)
    {
        Result result = new Result();

        result.triangle = triangle;
        result.distanceSquared = float.PositiveInfinity;

        if (triangle >= _triangles.Length / 3)
            return result;


        //Get the vertices of the triangle
        result.vert1 = _vertices[_triangles[0 + triangle * 3]];
        result.vert2 = _vertices[_triangles[1 + triangle * 3]];
        result.vert3 = _vertices[_triangles[2 + triangle * 3]];


        result.normal = Vector3.Cross((result.vert2 - result.vert1).normalized, (result.vert3 - result.vert1).normalized);

        //Project our point onto the plane
        var projected = point + Vector3.Dot((result.vert1 - point), result.normal) * result.normal;

        //Calculate the barycentric coordinates
        var u = ((projected.x * result.vert2.y) - (projected.x * result.vert3.y) - (result.vert2.x * projected.y) + (result.vert2.x * result.vert3.y) +
                 (result.vert3.x * projected.y) - (result.vert3.x * result.vert2.y)) /
                ((result.vert1.x * result.vert2.y) - (result.vert1.x * result.vert3.y) - (result.vert2.x * result.vert1.y) + (result.vert2.x * result.vert3.y) +
                 (result.vert3.x * result.vert1.y) - (result.vert3.x * result.vert2.y));
        var v = ((result.vert1.x * projected.y) - (result.vert1.x * result.vert3.y) - (projected.x * result.vert1.y) + (projected.x * result.vert3.y) +
                 (result.vert3.x * result.vert1.y) - (result.vert3.x * projected.y)) /
                ((result.vert1.x * result.vert2.y) - (result.vert1.x * result.vert3.y) - (result.vert2.x * result.vert1.y) + (result.vert2.x * result.vert3.y) +
                 (result.vert3.x * result.vert1.y) - (result.vert3.x * result.vert2.y));
        var w = ((result.vert1.x * result.vert2.y) - (result.vert1.x * projected.y) - (result.vert2.x * result.vert1.y) + (result.vert2.x * projected.y) +
                 (projected.x * result.vert1.y) - (projected.x * result.vert2.y)) /
                ((result.vert1.x * result.vert2.y) - (result.vert1.x * result.vert3.y) - (result.vert2.x * result.vert1.y) + (result.vert2.x * result.vert3.y) +
                 (result.vert3.x * result.vert1.y) - (result.vert3.x * result.vert2.y));

        result.centre = result.vert1 * 0.3333f + result.vert2 * 0.3333f + result.vert3 * 0.3333f;

        //Find the nearest point
        var vector = (new Vector3(u, v, w)).normalized;

        //work out where that point is
        var nearest = result.vert1 * vector.x + result.vert2 * vector.y + result.vert3 * vector.z;
        result.closestPoint = nearest;
        result.distanceSquared = (nearest - point).sqrMagnitude;

        if (float.IsNaN(result.distanceSquared))
        {
            result.distanceSquared = float.PositiveInfinity;
        }
        return result;
    }
}