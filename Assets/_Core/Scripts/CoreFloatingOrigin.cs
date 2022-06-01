using UnityEngine;

public class CoreFloatingOrigin : MonoBehaviour
{
    static public CoreFloatingOrigin Instance;

    public float OriginCheckSeconds = 1;
    public float OriginResetThreshold = 1000.0f;
    public bool MoveActiveParticles = true;
    public GameObject[] GlobalObjectsToFloat;

    private ParticleSystem.Particle[] parts;

    void Awake()
    {
        Instance = this;

        InvokeRepeating("CheckOriginOffset", OriginCheckSeconds, OriginCheckSeconds);
    }

    public void CheckOriginOffset()
    {
        if (!Camera.main || !WhirldData.Instance || !CoreSettings.Instance.SceneObjects || !WhirldData.Instance.EnableInfiniteOriginReset)
        {
            Debug.Log("CoreFloatingOrigin :: CheckOriginOffset skipped");
            return;
        }

        var offsetDelta = Camera.main.transform.position;
        offsetDelta.y = 0f;

        if (!(offsetDelta.sqrMagnitude > OriginResetThreshold * OriginResetThreshold))
        {
            return;
        }

        Debug.Log("CoreFloatingOrigin :: CheckOriginOffset :: Threshold exceeded: " + OriginResetThreshold + " meters");

        ResetObjectsOrigin();
    }

    void ResetObjectsOrigin()
    {
        var offsetDelta = Camera.main.transform.position;
        offsetDelta.y = 0f;

        Debug.Log("CoreFloatingOrigin :: Resetting scene and dynamic objects relative to global origin: " + offsetDelta + " meters");

        // Move scene objects
        CoreSettings.Instance.SceneObjects.position -= offsetDelta;

        // Move camera, environment, etc
        foreach (GameObject go in GlobalObjectsToFloat)
        {
//			if (!go || !go.transform)
//			{
//				Debug.Log("CoreFloatingOrigin :: Warning :: Encountered gameO");
//			}
//
            go.transform.position -= offsetDelta;

            Debug.Log("CoreFloatingOrigin :: Moved global object (name: " + go.name + ", offset: " + offsetDelta + ", newpos: " + go.transform.position + ")");
        }

        // Move active particles
        if (MoveActiveParticles)
        {
            var objects = FindObjectsOfType(typeof(ParticleSystem));
            foreach (Object o in objects)
            {
                var sys = (ParticleSystem) o;
                var particlesNeeded = sys.main.maxParticles;

                if (sys.simulationSpace != ParticleSystemSimulationSpace.World || sys.main.maxParticles <= 0)
                {
                    continue;
                }

                var wasPaused = sys.isPaused;
                var wasPlaying = sys.isPlaying;

                if (!wasPaused)
                {
                    sys.Pause();
                }

                // ensure a sufficiently large array in which to store the particles
                if (parts == null || parts.Length < particlesNeeded)
                {
                    parts = new ParticleSystem.Particle[particlesNeeded];
                }

                // now get the particles
                var num = sys.GetParticles(parts);

                for (var i = 0; i < num; i++)
                {
                    parts[i].position -= offsetDelta;
                }

                sys.SetParticles(parts, num);

                if (wasPlaying)
                {
                    sys.Play();
                }
            }
        }
    }
}