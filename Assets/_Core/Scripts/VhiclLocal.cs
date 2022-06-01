using System.Collections;
using UnityEngine;

public class VhiclLocal : MonoBehaviour
{
	public Vhicl vhicl;
	private Vector3 p;
	private Quaternion r;
	private int m = 0;
	float syncPosTimer = 0.00f;
	float syncInpTimer = 0.00f;

	private Vector3 _prevVelocity;

	Vector4 inputS;
	//private Vector3 prevPos = Vector3.zero;

	void Start()
	{
		//Vhicl.photonView.observed = this;
		vhicl.NetCode = "";
		vhicl.IsResponding = true;
	}

	void Update()
	{
		/*if(Vhicl.networkMode == 2 && Time.time > syncPosTimer) {
				syncPosTimer = Time.time + (1 / PhotonNetwork.sendRate);
				photonView.RPC("sP", PhotonTargets.Others, Vhicl.MyRigidbody.position, Vhicl.MyRigidbody.rotation);
		}

		if(Time.time > syncInpTimer && Vhicl.input != inputS) {
				syncInpTimer = Time.time + 1;
				inputS = Vhicl.input;
				photonView.RPC("s4", PhotonTargets.Others, Mathf.RoundToInt(inputS.x * 10), Mathf.RoundToInt(inputS.y * 10), Mathf.RoundToInt(inputS.z * 10), Mathf.RoundToInt(inputS.w * 10));
		}*/
	}

	void FixedUpdate()
	{
		if (!vhicl || !vhicl.MySettings/* || vhicl.timeInit > 0f*/)
		{
			return;
		}

		//        Debug.DrawRay(transform.position, Vector3.up * 10, Color.blue, 1f);

		vhicl.Velocity = vhicl.MyRigidbody.velocity;

		// Motion freeze :: physics override
		if (vhicl.MySettings.FreezeMotion)
		{
			vhicl.MyRigidbody.isKinematic = true;
		}

		// Motion freeze :: don't freeze
		else
		{
			vhicl.MyRigidbody.isKinematic = false;
		}

		// Terrain fallthrough detection
		var isVoxeland = false;

		// #if VOXELAND
		//         if (Voxeland5.Voxeland.instances.Count > 0)
		//         {
		//             isVoxeland = true;
		//         }
		// #endif

#if MAPMAGIC
				// @todo:IMPORTFIXME
        if (!vhicl.MyRigidbody.isKinematic /* && (MapMagic.instances != null /* || OnlineMaps.instance || isVoxeland)*/)
        {
            // Terrain fallthrough :: Ensure there is terrain under the vehicle
            float rayLength = 9999999; // Mathf.Infinity;
            if (Physics.SphereCast(new Ray(transform.position, Vector3.down * rayLength), 0.1f, Mathf.Infinity, 1))
            {
                // @todo - suspending the vehicle midgame isn't actually that useful.
                // Instead, we are experimenting with allowing the vehicle to continue moving, and catch it if it falls below the terrain
//                if (vhicl.MyRigidbody.isKinematic)
//                {
//                    Debug.Log("Vhicl (" + vhicl.MyRigidbody.gameObject.name + ") :: Resuming Vehicle vehicle motion, terrain under vehicle has successfully generated.");
//                }
//                vhicl.MyRigidbody.isKinematic = false;
            }

            // Terrain fallthrough :: No terrain below. Check for terrain above.
            else
            {
                // Cast ray from above player downwards, looking for MapMagic terrain intersection
                RaycastHit hitInfo;
                bool hitFound = Physics.Raycast(transform.position + Vector3.up * rayLength, Vector3.down, out hitInfo, rayLength, vhicl.TerrainMask,
                    QueryTriggerInteraction.Ignore);
                if (hitFound)
                {
                    Debug.Log("Vhicl (" + vhicl.MyRigidbody.gameObject.name + ") :: appears to be underground. Resetting vertical position to rest on terrain surface.");
                    vhicl.MyRigidbody.transform.position = hitInfo.point + Vector3.up * vhicl.CamOffset;
                }

//                // No terrain above
//                // Terrain appears to be regenerating - suspend vehicle motion
//                if (!vhicl.MyRigidbody.isKinematic)
//                {
//                    Debug.Log("Vhicl (" + vhicl.MyRigidbody.gameObject.name + ") :: Suspending vehicle motion, terrain under vehicle appears to be regenerating...");
//                }
//                vhicl.MyRigidbody.isKinematic = true;
            }
        }
#endif


		// Automatically preserve velocity when kinematic status is triggered
		if (!vhicl.MyRigidbody.isKinematic)
		{
			if (vhicl.MyRigidbody.velocity == Vector3.zero)
			{
				vhicl.MyRigidbody.velocity = _prevVelocity;
			}
			else
			{
				_prevVelocity = vhicl.MyRigidbody.velocity;
			}
		}
	}

	void OnTriggerStay(Collider other)
	{
		//		if(other.gameObject.layer == 14) return;
		//		if(other.attache.MyRigidbody) Vhicl.OnRam(other.attache.MyRigidbody.gameObject);
	}

	void OnCollisionStay(Collision collision)
	{
		//		Vhicl.vehObj.BroadcastMessage("OnCollisionStay", collision, SendMessageOptions.DontRequireReceiver);
		//		if(collision.collider.transform.root == transform.root) return;
		//		if(collision.transform.parent && collision.transform.parent.gameObject.GetComponent.MyRigidbody>()) {
		//			Vhicl.OnRam(collision.transform.parent.gameObject); //we hit a tank track or something - and it wasn't one of ours
		//		}
		//		else if(collision.MyRigidbody) {
		//			Vhicl.OnRam(collision.gameObject);
		//		}
	}

	//
	//	void  OnPhotonSerializeView ( PhotonStream stream  ) {
	//		p = Vhicl.MyRigidbody.position;
	//		r = Vhicl.MyRigidbody.rotation;
	//		stream.Serialize(ref p);
	//		stream.Serialize(ref r);
	//		m = 0;
	//		stream.Serialize(ref m);
	//
	//		/*if(Time.time - 0.25f > syncInpTimer) {
	//			syncInpTimer = Time.time;
	//			Vector3 i = new Vector3(Vhicl.input.x, Vhicl.input.y, Vhicl.input.z);
	//			if(i==Vector3.zero) i.x = -2;
	//			stream.Serialize(i);
	//		}*/
	//	}
}