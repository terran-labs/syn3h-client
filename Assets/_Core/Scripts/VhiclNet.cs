using UnityEngine;

class State : System.Object
{
    public Vector3 p;
    public Quaternion r;
    public float t = 0.0f;
    public float m = 0.0f;

    public float n = 0.0f;

    //float s = 0.0f;
    public State(Vector3 pi, Quaternion ri, float ti, float mi, float ni)
    {
        //, s : float) {
        this.p = pi;
        this.r = ri;
        this.t = ti;
        //this.s = s;
        this.m = mi;
        this.n = ni;
    }
}

public class VhiclNet : MonoBehaviour
{
    public Vhicl vhicl;
    bool simulatePhysics = true;
    bool updatePosition = true;
    float physInterp = 0.1f;
    float ping;
    float jitter;
    float calcPing = 0.00f;
    float rpcPing = 0.00f;
    float lastPing = -1.00f;
    int pingCheck = Random.Range(15, 20);
    bool wePinged = false;
    float autoInterp = 0.00f;
    private int m;
    private Vector3 p;
    private Quaternion r;
    State[] states = new State[15];

    private CoreSettings Settings;

    void Start()
    {
//		Vhicl.photonView.observed = this;
    }

    void Update()
    {
//        //try {
//
//        if (!updatePosition || states[14] == null || states[14].t == 0.0f || !Vhicl.Rigidbody) return;
//
//        if (Settings.networkPhysics == 0)
//        {
//            physInterp = .1f;
//            simulatePhysics = (Vector3.Distance(Vhicl.Rigidbody.position, Camera.main.transform.position) < 40);
//        }
//        else if (Settings.networkPhysics == 1)
//        {
//            physInterp = .2f;
//            simulatePhysics = (Vector3.Distance(Vhicl.Rigidbody.position, Camera.main.transform.position) < 40);
//        }
//        else simulatePhysics = false;
//
//        /*
//        //jitter = Mathf.Lerp(jitter, Mathf.Abs(ping - (PhotonNetwork.time - states[0].t)), Time.deltaTime * .3);
//        float prevJitter = jitter;
//        //jitter = 0.0f;
//        //for (i=0; i<states.length-2; i++) if(states[i].t - states[i+1].t > jitter) jitter = states[i].t - states[i+1].t;
//        //jitter = Mathf.Lerp(prevJitter, jitter, Time.deltaTime * .3);
//        FIXME_VAR_TYPE jitterSum= 0.0f;
//        for (i=0; i<states.length-1; i++) jitterSum += states[i].t - states[i+1].t;
//        jitter = Mathf.Lerp(prevJitter, (jitterSum / 14) - (1 / PhotonNetwork.sendRate), Time.deltaTime * .3);
//        ping = Mathf.Lerp(ping, PhotonNetwork.time - states[0].s, Time.deltaTime * .3);
//        */
//
//        Vhicl.Rigidbody.isKinematic = !simulatePhysics;
//        Vhicl.Rigidbody.interpolation =
//            RigidbodyInterpolation
//                .None; //(simulatePhysics || Settings.networkPhysics == 1 ? RigidbodyInterpolation.Interpolate : RigidbodyInterpolation.None);
//
//        //Interpolation
//        float interpolationTime;
//        if (Settings.networkInterpolation > 0)
//        {
//            interpolationTime = (float) PhotonNetwork.time - Settings.networkInterpolation;
//        }
//        else
//        {
//            autoInterp = Mathf.Max(.1f, Mathf.Lerp(autoInterp, ping * 1.5f + (jitter * 8), Time.deltaTime * .15f));
//            interpolationTime = (float) PhotonNetwork.time - autoInterp;
//        }
//
//        if (states[0].t > interpolationTime)
//        {
//            // Target playback time should be present in the buffer
//            for (var i = 1; i < states.Length - 1; i++)
//            {
//                // Go through buffer and find correct state to play back
//                if (states[i] != null && (states[i].t <= interpolationTime || i == states.Length - 3))
//                {
//                    State rhs = states[i - 1]; // The state one slot newer than the best playback state
//                    State lhs = states[i]; // The best playback state (closest to .1 seconds old)
//                    float l =
//                        rhs.t - lhs.t; // Use the time between the two slots to determine if interpolation is necessary
//                    float t =
//                        0.0f; // As the time difference gets closer to 100 ms, t gets closer to 1 - in which case rhs is used
//                    if (l > 0.0001f) t = ((interpolationTime - lhs.t) / l); // if t=0 => lhs is used directly
//                    Vhicl.velocity = Vector3.Lerp(Vhicl.velocity,
//                        ((rhs.p - states[i + 2].p) / (rhs.t - states[i + 2].t)), Time.deltaTime * .3f);
//                    if (simulatePhysics)
//                    {
//                        //Vhicl.transform.position = Vector3.Lerp(Vhicl.transform.position, Vector3.Lerp(lhs.p, rhs.p, t), physInterp);
//                        //Vhicl.transform.rotation = Quaternion.Slerp(Vhicl.transform.rotation, Quaternion.Slerp(lhs.r, rhs.r, t), physInterp);
//                        Vhicl.Rigidbody.MovePosition(
//                            Vector3.Lerp(Vhicl.transform.position, Vector3.Lerp(lhs.p, rhs.p, t), physInterp));
//                        Vhicl.Rigidbody.MoveRotation(Quaternion.Slerp(Vhicl.transform.rotation,
//                            Quaternion.Slerp(lhs.r, rhs.r, t), physInterp));
//                        Vhicl.Rigidbody.velocity = Vhicl.velocity;
//                    }
//                    else
//                    {
//                        Vhicl.Rigidbody.position = Vector3.Lerp(lhs.p, rhs.p, t);
//                        Vhicl.Rigidbody.rotation = Quaternion.Slerp(lhs.r, rhs.r, t);
//                    }
//                    Vhicl.isResponding = true;
//                    Vhicl.netCode = "";
//                    return;
//                }
//            }
//        }
//
//        //Extrapolation
//        else
//        {
//            float extrapolationLength = (interpolationTime - states[0].t);
//            Vhicl.velocity = Vector3.Lerp(Vhicl.velocity, ((states[0].p - states[2].p) / (states[0].t - states[2].t)),
//                Time.deltaTime * .3f);
//            if (extrapolationLength < 1)
//            {
//                if (!simulatePhysics)
//                {
//                    Vhicl.Rigidbody.position = states[0].p + (Vhicl.velocity * extrapolationLength);
//                    Vhicl.Rigidbody.rotation = states[0].r;
//                }
//                Vhicl.isResponding = true;
//                if (extrapolationLength < .3) Vhicl.netCode = ">";
//                else Vhicl.netCode = " (Delayed)";
//            }
//            else
//            {
//                Vhicl.netCode = " (Not Responding)";
//                Vhicl.isResponding = false;
//            }
//            /*Vhicl.velocity = ((states[0].p - states[1].p) / (states[0].t - states[1].t));
//            if (extrapolationLength < .5 && states[0] && states[1]) {
//                if(!simulatePhysics) {
//                    Vhicl.transform.position = states[0].p + (Vhicl.velocity * extrapolationLength);
//                    Vhicl.transform.rotation = states[0].r;
//                }
//                Vhicl.isResponding = true;
//                if(extrapolationLength < .3) Vhicl.netCode = ">";
//                else Vhicl.netCode = " (Delayed)";
//            }
//            else {
//                Vhicl.netCode = " (Not Responding)";
//                Vhicl.isResponding = false;
//            }*/
//            //if(simulatePhysics && states[0].t > states[1].t) Vhicl.Rigidbody.velocity = Vhicl.velocity;
//        }
//
//        /*}
//        catch ( System.Exception e  ) {
//            Debug.Log("Interpolation Error in " + Vhicl.gameObject.name + ": " + e.ToString());
//        }*/
    }

//	void  OnPhotonSerializeView ( PhotonStream stream ,   PhotonMessageInfo info  ){
//
//		//We are the server, and have to keep track of relaying messages between connected clients
//		if(stream.isWriting) {
//			if(states[0] == null) return;
//			p = states[0].p;
//			r = states[0].r;
//			m = (int)((PhotonNetwork.time - states[0].t) * 1000);	//m is the number of milliseconds that transpire between the packet's original send time and the time it is resent from the server to all the other clients
//			stream.Serialize(ref p);
//			stream.Serialize(ref r);
//			stream.Serialize(ref m);
//		}
//
//		//New packet recieved - add it to the states array for interpolation!
//		else {
//
//			stream.Serialize(ref p);
//			stream.Serialize(ref r);
//			stream.Serialize(ref m);
//
//			State state = new State(p, r, info.timestamp - (m > 0 ? ((float)m / 1000.0f) : 0), m, (float)PhotonNetwork.time - info.timestamp);
//
//			if(states[0] != null && state.t == states[0].t) state.t += .01f;	//Bizarre - dragonhere
//
//			if(states[0] == null || state.t > states[0].t) {
//
//				float png = (float)PhotonNetwork.time - state.t;
//				jitter = Mathf.Lerp(jitter, Mathf.Abs(ping - png), 1 / PhotonNetwork.sendRate);
//				ping = Mathf.Lerp(ping, png, 1 / PhotonNetwork.sendRate);
//
//				for (int k = states.Length-1; k>0; k--) states[k] = states[k-1];
//				states[0] = state;
//
//			}
//			//else Debug.Log(Vhicl.name + ": Out-of-order state received and ignored (" + ((states[0].t - state.t) * 1000) + ")" + states[0].t + "---" + state.t + "---" + m);
//
//			/*Vector3 lastI = i;
//			stream.Serialize(i);
//			if(i == Vector3.zero) i = lastI;	//Vector3 not included in stream
//			else f = true;						//Vector3 was included in stream
//			if(i.x == -2) i = Vector3.zero;		//Vector3 is zero, but flag has been set so we can differentiate it from unset Vector3
//			Vhicl.input = i;*/
//		}
//
//	}
}