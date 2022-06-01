using UnityEngine;
using System.Collections;

public class VhiclSphere : MonoBehaviour {
	/*//Transform sphere;
	Transform shield;
	private float offset;
	Color tagColor;
	Color ramColor;
	bool  ram = false;
	public Vhicl Vhicl;

	void  Start (){
		shield.renderer.material.color = tagColor;
		Vhicl = transform.root.gameObject.GetComponent<Vhicl>();
	}

	void  Update (){
	    shield.renderer.material.SetFloat("_Strength", Mathf.Lerp(shield.renderer.material.GetFloat("_Strength"), 1.5f, Time.deltaTime * .5));
	    if(!ram) shield.rotation = Quaternion.identity;
	}

	void  colorSet ( bool r  ){
		 yield return new WaitForFixedUpdate (); //bizare, but necessary...
		if(r) shield.renderer.material.color = ramColor;
		else shield.renderer.material.color = tagColor;
		shield.renderer.material.SetFloat("_Strength", 0);
		shield.renderer.material.SetFloat("_CutRange", (r ? 1.5f : .15));
		ram = r;
	}

	void  OnCollisionEnter ( Collision collision  ){
		float i = collision.relativeVelocity.magnitude * Mathf.Abs(Vector3.Dot(collision.contacts[0].normal,collision.relativeVelocity.normalized));
		if(i > 2) shield.renderer.material.SetFloat("_Strength", Mathf.Min(5, 1.7f + i * .25));
	}

	void  OnTriggerStay ( Collider other  ){
		if(other.gameObject.layer == 14) return;
		if(other.name == "ORB(Clone)") shield.renderer.material.SetFloat("_Strength", 5);
		if(!Vhicl.photonView.isMine) return;
		if(other.attachedRigidbody) {
			Vhicl.OnRam(other.attachedRigidbody.gameObject);
		}
	}

	void  OnLaserHit (){
		shield.renderer.material.SetFloat("_Strength", 5);
	}*/
}