// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;

public class VhiclLaser : MonoBehaviour {
//	float lag;
//	Transform explosion;
//	Vhicl launchVhicl;
//	Vhicl targetVhicl;
//	int speedX;
//	string laserID;
//	private Transform strtPos;
//	private int stage = -1;
//	LayerMask mask = -1;
//	LayerMask maskOpt = -1;
//
//	IEnumerable Start (){
//		//Init
//		name = "lsr#" + laserID;
//		velocity = Settings.laserSpeed * speedX + Mathf.Max(0,
//			launchVhicl.transform.InverseTransformDirection(launchVhicl.velocity).z *
//			(targetVhicl ? 1 : Vector3.Dot(transform.forward, launchVhicl.transform.forward))
//		);
//
//		//AutoTargeting
//		//if(Game.QuarryVeh && !launchVhicl || Game.QuarryVeh != launchVhicl) targetVhicl = Game.QuarryVeh;
//		if(targetVhicl) {
//			Vector3 bgn = transform.position;
//			if(targetVhicl.photonView.isMine) { //Local Vhicl, high precision
//				Vector3 refvel = targetVhicl.gameObject.GetComponent<Rigidbody>().velocity;
//				float reftme = Time.time;
//				yield return new WaitForSeconds(Time.fixedDeltaTime * 5); //Allow target to move so we can analyze it's acceleration
//				Vector3 pos = targetVhicl.transform.position;
//				Vector3 vel = targetVhicl.gameObject.GetComponent<Rigidbody>().velocity;
//				Vector3 accel = (vel - refvel) * 1 / (Time.time - reftme); //Accelleration over 1 second
//			}
//			else {	//Remote Vhicl, just make it look good
//				Vector3 refpos = targetVhicl.transform.position;
//				reftme = Time.time;
//				yield return new WaitForSeconds(Time.fixedDeltaTime * 5); //Allow target to move so we can analyze it's velocity
//				refvel = (targetVhicl.transform.position - refpos) * 1 / (Time.time - reftme); //velocity over 1 second
//				refpos = targetVhicl.transform.position;
//				reftme = Time.time;
//				yield return new WaitForSeconds(Time.fixedDeltaTime * 5); //Allow target to move so we can analyze it's acceleration
//				pos = targetVhicl.transform.position;
//				vel = (targetVhicl.transform.position - refpos) * 1 / (Time.time - reftme); //velocity over 1 second
//				accel = (vel - refvel) * 1 / (Time.time - reftme); //Accelleration over 1 second
//			}
//
//			float dur = 0;
//			while(true) {
//				pos += vel * Time.fixedDeltaTime;
//				vel += accel * Time.fixedDeltaTime;
//				dur += Time.fixedDeltaTime;
//				if(dur > 10 || Vector3.Distance(bgn, pos) < dur * velocity) break;
//			}
//			transform.LookAt(pos);
//		}
//
//		//Lag Compensation
//		//Unnecessary when autotargeting, as hits are calculated on target client
//		else if(lag > 0) GetComponent<Rigidbody>().position += GetComponent<Rigidbody>().transform.TransformDirection(0, 0, velocity * lag);	//Position extrapolation for non authoratative firing instances
//
//		//Begin Run
//		GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().transform.TransformDirection(0, 0, velocity);
//		strtPos = GetComponent<Rigidbody>().position;
//		stage = 0;
//
//		//Cleanup
//		yield return new WaitForSeconds(Mathf.Lerp(13, 5, Settings.laserSpeed * speedX * .003));
//		Destroy(gameObject);
//	}
//
//	void  FixedUpdate (){
//		if(stage != 0 || !launchVhicl || !Settings) return;
//		if(Settings.laserGrav != 0) GetComponent<Rigidbody>().velocity.y -= Settings.laserGrav * speedX * Time.deltaTime * 20;
//		RaycastHit[] hits;
//		Vector3 vector = transform.position - strtPos;
//	    hits = Physics.RaycastAll (strtPos, vector, vector.magnitude, (Settings.lasersOptHit ? maskOpt : mask ));
//	    for (int i=0; i<hits.length; i++) {
//	        RaycastHit collision = hits[i];
//
//			//Launch Vhicl is immune to hits
//			if(collision.collider.transform.root.gameObject == launchVhicl.gameObject) continue;
//
//			//Non-authoratative game instances DO NOT detect Vhicl laser hits. Hits are detected on authoratative instance, and then broadcast to other clients with lH RPC
//			if(((targetVhicl && !targetVhicl.photonView.isMine) || (!targetVhicl && !launchVhicl.photonView.isMine)) && collision.rigidbody) continue;
//
//			//We are the instance if this laser on the firer's computer, & we tagged another Vhicl
//			if(((targetVhicl && targetVhicl.photonView.isMine) || (!targetVhicl && launchVhicl.photonView.isMine)) && collision.rigidbody) {
//				Vhicl veh = collision.transform.root.gameObject.GetComponent<Vhicl>();
//				if(veh && veh.isResponding) {
//					//Determine where we hit them
//					veh.gameObject.photonView.RPC("lH", PhotonTargets.Others, laserID, collision.transform.InverseTransformPoint(collision.point));
//					//We are quarry, and made a tag
//					if(launchVhicl.isIt == 1 && (Time.time - veh.lastTag) > 3) {
//						launchVhicl.gameObject.photonView.RPC("iS", PhotonTargets.All, veh.gameObject.name);
//						veh.lastTag = Time.time;
//					}
//					//They were quarry, and we tagged them
//					else if(veh.isIt == 1 && (Time.time - launchVhicl.lastTag) > 3 && (Time.time - veh.lastTag) > 3) launchVhicl.gameObject.photonView.RPC("sQ", PhotonTargets.All, 2);
//					//They weren't quarry, and we weren't supposed to shoot them
//					else if(veh.isIt == 0 && launchVhicl.isIt == 0) launchVhicl.gameObject.photonView.RPC("dS", PhotonTargets.All, veh.gameObject.name);
//				}
//			}
//
//			if(collision.rigidbody || Settings.laserRico == 0) {
//				laserHit(collision.transform.root.gameObject, collision.point, collision.normal);
//			}
//			else {
//				GetComponent<Rigidbody>().position = collision.point /*+ rigidbody.velocity.normalized * -.1*/;// + collision.normal;
//				GetComponent<Rigidbody>().velocity = Settings.laserRico * Vector3.Lerp(Vector3.Scale(GetComponent<Rigidbody>().velocity, collision.normal), Vector3.Reflect(GetComponent<Rigidbody>().velocity, collision.normal), Settings.laserRico);
//			}
//	    }
//
//	    strtPos = GetComponent<Rigidbody>().position;
//	}
//
//	void  laserHit ( GameObject go ,   Vector3 pos ,   Vector3 norm  ){
//		//if(stage == 1) return; //We already collided with a Vhicl on a non-authoratative instance - ignore the collision message that the authoratative Vhicl is sending us...
//		stage = 1;
//
//		GetComponent<Rigidbody>().position = pos;
//		GetComponent<Rigidbody>().velocity = Vector3.zero;
//		go.BroadcastMessage("OnLaserHit", launchVhicl.photonView.isMine, SendMessageOptions.DontRequireReceiver);
//		Collider[] colliders = Physics.OverlapSphere(pos, 10);
//		foreach(var hit in colliders) {
//		 	if (hit.attachedRigidbody) {
//			  	hit.attachedRigidbody.AddExplosionForce(350 + speedX * 300, pos, 1, 2);
//			}
//		}
//		Instantiate(explosion, pos, Quaternion.FromToRotation(Vector3.up, norm)); //transform.rotation
//	}
}