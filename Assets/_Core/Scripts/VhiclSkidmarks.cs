using UnityEngine;

// props: https://www.reddit.com/r/Unity3D/comments/3r9gg8/efficient_skidmarks_script_based_on_the_old_car/
public class VhiclSkidmarks : MonoBehaviour
{
    // Variables for each mark created. Needed to generate the correct mesh.
    class MarkSection
    {
        public Vector3 Pos = Vector3.zero;
        public Vector3 Normal = Vector3.zero;
        public Vector4 Tangent = Vector4.zero;
        public Vector3 Posl = Vector3.zero;
        public Vector3 Posr = Vector3.zero;
        public byte Intensity;
        public int LastIndex;
    };

    public float MarkWidth = 0.5f; // Width of the skidmarks. Should match the width of the wheels

    const int MAX_MARKS = 1024; // Max number of marks total for everyone together
    const float GROUND_OFFSET = 0.04f; // Distance above surface in metres
    const float MIN_DISTANCE = .75f; // Distance between points in metres. Bigger = more clunky, straight-line skidmarks
    const float MIN_SQR_DISTANCE = MIN_DISTANCE * MIN_DISTANCE;

    int markIndex;
    MarkSection[] skidmarks;
    Mesh marksMesh;
    MeshRenderer mr;
    MeshFilter mf;

    Vector3[] vertices;
    Vector3[] normals;
    Vector4[] tangents;
    Color32[] colors;
    Vector2[] uvs;
    int[] triangles;

    bool _updated;
    bool _haveSetBounds;

    // #### UNITY INTERNAL METHODS ####

    protected void OnEnable()
    {
        // Disable skidmark rendering if atmsopherics are disabled in this world.
        // Skidmarks use a DeepSky Haze shader, and will render as solid black if there is no HazeCore in scene
        // @todo deprecating ds_haze
//        if (WhirldData.Instance && !WhirldData.Instance.EnableAtmospherics)
//        {
//            gameObject.SetActive(false);
//        }

        skidmarks = new MarkSection[MAX_MARKS];
        for (int i = 0; i < MAX_MARKS; i++)
        {
            skidmarks[i] = new MarkSection();
        }

        mf = GetComponent<MeshFilter>();
        mr = GetComponent<MeshRenderer>();

        marksMesh = new Mesh();
        marksMesh.MarkDynamic();
        mf.sharedMesh = marksMesh;

        vertices = new Vector3[MAX_MARKS * 4];
        normals = new Vector3[MAX_MARKS * 4];
        tangents = new Vector4[MAX_MARKS * 4];
        colors = new Color32[MAX_MARKS * 4];
        uvs = new Vector2[MAX_MARKS * 4];
        triangles = new int[MAX_MARKS * 6];
    }

    protected void LateUpdate()
    {
        if (!_updated) return;
        _updated = false;

        // Reassign the mesh if it has changed this frame
        marksMesh.vertices = vertices;
        marksMesh.normals = normals;
        marksMesh.tangents = tangents;
        marksMesh.triangles = triangles;
        marksMesh.colors32 = colors;
        marksMesh.uv = uvs;

        if (!_haveSetBounds)
        {
            // Could use RecalculateBounds here each frame instead, but it uses about 0.1-0.2ms each time.
            // Save time by just making the mesh bounds huge, so the skidmarks will always draw.
            // Not sure why I only need to do this once, yet can't do it in Start (it resets to zero).
            marksMesh.bounds = new Bounds(new Vector3(0, 0, 0), new Vector3(10000, 10000, 10000));
            _haveSetBounds = true;
        }

        mf.sharedMesh = marksMesh;
    }

    // #### PUBLIC METHODS ####

    // Function called by the wheels that is skidding. Gathers all the information needed to
    // create the mesh later. Sets the intensity of the skidmark section b setting the alpha
    // of the vertex color.
    public int AddSkidMark(Vector3 pos, Vector3 normal, float intensity, int lastIndex)
    {
        if (!gameObject.activeSelf || skidmarks == null)
        {
            return 0;
        }

        intensity = Mathf.Clamp01(intensity) / 2;

        if (lastIndex > 0)
        {
            float sqrDistance = (pos - skidmarks[lastIndex].Pos).sqrMagnitude;
            if (sqrDistance < MIN_SQR_DISTANCE) return lastIndex;
        }

        MarkSection curSection = skidmarks[markIndex];

        curSection.Pos = pos + normal * GROUND_OFFSET;
        curSection.Normal = normal;
        curSection.Intensity = (byte) (intensity * 255f);
        curSection.LastIndex = lastIndex;

        if (lastIndex != -1)
        {
            MarkSection lastSection = skidmarks[lastIndex];
            Vector3 dir = (curSection.Pos - lastSection.Pos);
            Vector3 xDir = Vector3.Cross(dir, normal).normalized;

            curSection.Posl = curSection.Pos + xDir * MarkWidth * 0.5f;
            curSection.Posr = curSection.Pos - xDir * MarkWidth * 0.5f;
            curSection.Tangent = new Vector4(xDir.x, xDir.y, xDir.z, 1);

            if (lastSection.LastIndex == -1)
            {
                lastSection.Tangent = curSection.Tangent;
                lastSection.Posl = curSection.Pos + xDir * MarkWidth * 0.5f;
                lastSection.Posr = curSection.Pos - xDir * MarkWidth * 0.5f;
            }
        }

        UpdateSkidmarksMesh();

        int curIndex = markIndex;
        // Update circular index
        markIndex = ++markIndex % MAX_MARKS;

        return curIndex;
    }

    // #### PROTECTED/PRIVATE METHODS ####

    // Update part of the mesh for the current markIndex
    void UpdateSkidmarksMesh()
    {
        MarkSection curr = skidmarks[markIndex];

        // Nothing to connect to yet
        if (curr.LastIndex == -1) return;

        MarkSection last = skidmarks[curr.LastIndex];
        vertices[markIndex * 4 + 0] = last.Posl;
        vertices[markIndex * 4 + 1] = last.Posr;
        vertices[markIndex * 4 + 2] = curr.Posl;
        vertices[markIndex * 4 + 3] = curr.Posr;

        normals[markIndex * 4 + 0] = last.Normal;
        normals[markIndex * 4 + 1] = last.Normal;
        normals[markIndex * 4 + 2] = curr.Normal;
        normals[markIndex * 4 + 3] = curr.Normal;

        tangents[markIndex * 4 + 0] = last.Tangent;
        tangents[markIndex * 4 + 1] = last.Tangent;
        tangents[markIndex * 4 + 2] = curr.Tangent;
        tangents[markIndex * 4 + 3] = curr.Tangent;

        colors[markIndex * 4 + 0] = new Color32(0, 0, 0, last.Intensity);
        colors[markIndex * 4 + 1] = new Color32(0, 0, 0, last.Intensity);
        colors[markIndex * 4 + 2] = new Color32(0, 0, 0, curr.Intensity);
        colors[markIndex * 4 + 3] = new Color32(0, 0, 0, curr.Intensity);

        uvs[markIndex * 4 + 0] = new Vector2(0, 0);
        uvs[markIndex * 4 + 1] = new Vector2(1, 0);
        uvs[markIndex * 4 + 2] = new Vector2(0, 1);
        uvs[markIndex * 4 + 3] = new Vector2(1, 1);

        triangles[markIndex * 6 + 0] = markIndex * 4 + 0;
        triangles[markIndex * 6 + 2] = markIndex * 4 + 1;
        triangles[markIndex * 6 + 1] = markIndex * 4 + 2;

        triangles[markIndex * 6 + 3] = markIndex * 4 + 2;
        triangles[markIndex * 6 + 5] = markIndex * 4 + 1;
        triangles[markIndex * 6 + 4] = markIndex * 4 + 3;

        _updated = true;
    }
}