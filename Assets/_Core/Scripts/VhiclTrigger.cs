using UnityEngine;
using System.Collections;

public class VhiclTrigger : MonoBehaviour {
	public Vhicl vhicl;

	void  Start (){
		vhicl = transform.root.GetComponent<Vhicl>();
	}

	void  OnTriggerStay ( Collider other  ){
//		if(other.gameObject.layer == 14 || !Vhicl.photonView.isMine) return;
//		if(other.attachedRigidbody) Vhicl.OnRam(other.attachedRigidbody.gameObject);
	}
}