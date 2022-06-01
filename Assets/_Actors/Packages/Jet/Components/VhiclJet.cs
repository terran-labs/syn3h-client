using UnityEngine;
using System.Collections;

public class VhiclJet : MonoBehaviour {

	private Vhicl _vhicl;
	public GameObject[] landingGear;
	private float landingGearScale;
//	public ParticleEmitter[] hoverThrusters;
//	public ParticleEmitter mainThrusterParticleEmitter;
//	public ParticleRenderer mainThrusterParticleRenderer;
//	public ThrustCone mainThrusterCone;
	public Transform[] mainThruster;
	public LayerMask thrustMask = -1;
	public MeshCollider bodyCollider;
//	WhirldLOD lod;

	public float hoverThrustFactor= 1f;
	public float hoverSteerFactor= .1f;
	public float hoverAngDrag= .1f;
	public float hoverLevelForceFactor= .2f;
	public float flightThrustFactor= 5f;
	public float flightAngDrag= .01f;
	public float atmosDensity;
	public Vector3 locvel;
	public float speed;
	public float pitch;
	public float roll;
	public float angleOfAttack;
	public float stallFactor;
	public float lavaFloat = .1f;
	
	private float grav;
	private float mass;
	private Vector3 inertiaTensor;
	private Quaternion inertiaTensorRotation;
	private RaycastHit hit;

	void  InitVhicl ( Vhicl veh  ){
		_vhicl = veh;
		_vhicl.SpecialInput = true; //Start out in landing mode
	
		mass = _vhicl.MyRigidbody.mass;
		grav = -Physics.gravity.y * mass;
		inertiaTensor = _vhicl.MyRigidbody.inertiaTensor;
		inertiaTensorRotation = _vhicl.MyRigidbody.inertiaTensorRotation;
	}

	void  Update (){
		if(!_vhicl) return;
	
		//Thruster Particles
//		var mainThrusterParticleSpeed = Mathf.Min(-10 * (_vhicl.SpecialInput ? .1f : _vhicl.InputSmoothed.z), -.5f);
//		if(mainThrusterParticleSpeed >= -1) mainThrusterParticleRenderer.particleRenderMode = ParticleRenderMode.Billboard;
//		else mainThrusterParticleRenderer.particleRenderMode = ParticleRenderMode.Stretch;
		
//		if(_vhicl.SpecialInput) {
//			foreach(ParticleEmitter thruster in hoverThrusters) {
//				thruster.emit = true;
//				thruster.localVelocity = new Vector3(thruster.localVelocity.x, -1 * _vhicl.InputSmoothed.z, thruster.localVelocity.z);
//				thruster.minSize = thruster.maxSize = Mathf.Max(.1f, _vhicl.InputSmoothed.z * .3f);
//			}
////			if(0 == 0) {
//				mainThrusterParticleEmitter.localVelocity = new Vector3(1 * _vhicl.InputSmoothed.x, -1 * _vhicl.InputSmoothed.y, mainThrusterParticleSpeed);
////			}
////			else mainThrusterCone.magThrottle = 0;
//		}
//		else {
//			if(_vhicl.Brakes) _vhicl.InputSmoothed.z = 0;
//
////			if (0 == 0)
////			{
//			mainThrusterParticleEmitter.localVelocity = new Vector3(0, 0, mainThrusterParticleSpeed);
////			}
////			else mainThrusterCone.magThrottle = 4;
//		
//			foreach(ParticleEmitter thruster in hoverThrusters) {
//				thruster.emit = false;
//			}
//		}
	
//		Camera
//		_vhicl.camSmooth = !_vhicl.SpecialInput;
	}

	void  FixedUpdate (){
		if(!_vhicl) return; //We are materializing, don't try to manipulate physics
	
		//Landing Gear
		if(_vhicl.SpecialInput) {
			if(!landingGear[0].active) landingGear[0].SetActiveRecursively(true);
			if(landingGearScale < 1) landingGearScale = landingGearScale + Time.deltaTime;
		}
		else {
			if(landingGear[0].active && landingGearScale > 0) landingGearScale = landingGearScale - Time.deltaTime;
			else if(landingGear[0].active) landingGear[0].SetActiveRecursively(false);
		}
		if (landingGear[0].active)
		{
			landingGear[0].transform.localScale = new Vector3(landingGearScale, landingGearScale, 1);
		}
	
		if(_vhicl.MyRigidbody.isKinematic == true) return; //We are materializing, don't try to manipulate physics

		_vhicl.MyRigidbody.centerOfMass = Vector3.zero;
		_vhicl.MyRigidbody.inertiaTensor = inertiaTensor * .75f;
		_vhicl.MyRigidbody.inertiaTensorRotation = inertiaTensorRotation;
	
		//Hovering
		if(_vhicl.SpecialInput) {
		
			//We are applying thrusters
			//if(Vhicl.input.z > 0) {
		
			//Force of hover thrusters in atmosphere
			_vhicl.MyRigidbody.AddForce(transform.up * _vhicl.InputSmoothed.z * hoverThrustFactor * grav);
			
			//Ground effect for each individual thruster
			/*foreach(Transform thruster in hoverThrusters) {
				if (Physics.Raycast (transform.position, Vector3.up * -1, hit, 30, thrustMask)) {
					if(hit.distance < _vhicl.MySettings.hoverHeight) {
						Vhicl.MyRigidbody.AddForce(transform.up * (_vhicl.MySettings.hoverHeight - hit.distance) * _vhicl.MySettings.hoverHover);							//Hover force
						if(thrustLast > hit.distance) Vhicl.MyRigidbody.AddForce(hit.normal * Mathf.Min((thrustLast - hit.distance) * _vhicl.MySettings.hoverRepel, 10), ForceMode.VelocityChange);		//Anticollision force
					}
					Vhicl.MyRigidbody.AddTorque(Vector3.Cross(transform.up, hit.normal) * Vector3.Angle(transform.up, hit.normal) * .2 * (40 - Mathf.Min(40,hit.distance)));
					thrustLast = hit.distance;
				}
			}*/
			
			//Autoleveling
			_vhicl.MyRigidbody.AddTorque(Vector3.Cross(transform.up, Vector3.up) * Vector3.Angle(transform.up, Vector3.up) * hoverLevelForceFactor * mass);
			
			//Steering
			_vhicl.MyRigidbody.AddRelativeTorque(new Vector3(
				_vhicl.InputSmoothed.y * mass * hoverSteerFactor,
				Mathf.Clamp(_vhicl.InputSmoothed.x + _vhicl.InputSmoothed.w, -1, 1) * mass * hoverSteerFactor,
				-_vhicl.InputSmoothed.x * mass * hoverSteerFactor
			));
			//}
		
			_vhicl.MyRigidbody.drag = _vhicl.MySettings.jetHDrag * _vhicl.MyRigidbody.velocity.magnitude  * (_vhicl.Brakes ? 7 : 1);
			_vhicl.MyRigidbody.angularDrag = hoverAngDrag * (_vhicl.Brakes ? 5 : 1);
			mainThruster[0].localEulerAngles = Vector3.zero;
		}
	
		//Flying
		else {
			if(_vhicl.Brakes) _vhicl.InputSmoothed.z = 0;
		
			//Pertinent Flight Info
			if(_vhicl.MyRigidbody.transform.position.y < 15240) atmosDensity = Mathf.Lerp(1.2250f, 0.18756f, _vhicl.MyRigidbody.transform.position.y / 15240); //kg/m³ 0-50,00' http://www.aerospaceweb.org/design/scripts/atmosphere/
			else atmosDensity = Mathf.Lerp(0.18756f, 0.017102f, _vhicl.MyRigidbody.transform.position.y / 30480); //50,000'-100,000'
			speed = _vhicl.MyRigidbody.velocity.magnitude;
			pitch = (transform.eulerAngles.x > 180 ? transform.eulerAngles.x - 360 : transform.eulerAngles.x);
			roll = (transform.eulerAngles.z > 180 ? transform.eulerAngles.z - 360 : transform.eulerAngles.z);
			locvel = _vhicl.MyRigidbody.transform.InverseTransformDirection(_vhicl.MyRigidbody.velocity);
			angleOfAttack = locvel.normalized.y;
			if(speed < _vhicl.MySettings.jetStall) stallFactor = Mathf.Lerp(1, 0, ((speed - _vhicl.MySettings.jetStall * .8f) / _vhicl.MySettings.jetStall) * 10); //Blend into wing stall
			else stallFactor = Mathf.Max(0, Mathf.Min(Mathf.Abs(angleOfAttack) - .65f, .1f)) * 10; //Lerp between 1 and 0 as aoa is between .85 and .95
		
			//Thruster
			mainThruster[0].localEulerAngles = new Vector3(-_vhicl.InputSmoothed.y * Mathf.Lerp(20, 5, speed / (_vhicl.MySettings.jetStall * 5)), -_vhicl.InputSmoothed.x * 1 + (_vhicl.InputSmoothed.w == 0 ? Mathf.Clamp(-locvel.x * 1, -10, 10) : Mathf.Clamp(-locvel.x * .5f, -10, 10)) + -_vhicl.InputSmoothed.w * 15, 0);
			_vhicl.MyRigidbody.AddForceAtPosition(mainThruster[0].forward * _vhicl.InputSmoothed.z * flightThrustFactor * grav * .99f, mainThruster[0].position);
		
			//Control Surfaces
			_vhicl.MyRigidbody.AddRelativeTorque(new Vector3(_vhicl.InputSmoothed.y * mass * _vhicl.MySettings.jetSteer * .2f, 0, -_vhicl.InputSmoothed.x * mass * _vhicl.MySettings.jetSteer * .75f) * Mathf.Lerp(0, 1, speed / _vhicl.MySettings.jetStall * .7f) * atmosDensity * (locvel.z > 0 ? 1 : -1));
			_vhicl.MyRigidbody.angularDrag = flightAngDrag;
		
			//Lift
			if(stallFactor < 1) {
				//Nonsense Hack
				//float liftMax = grav * (Mathf.Min(_vhicl.MySettings.jetLevelSpeed * 2, speed) / _vhicl.MySettings.jetLevelSpeed);
				//float lift2 = (-angleOfAttack + .1) * liftMax * 3;
				//Debug.DrawRay (transform.position, transform.up * lift2 * .002, Color.red);
			
				//http://www.aerospaceweb.org/question/aerodynamics/q0015b.shtml
				float wingArea= 15; //m²
				float liftCoefficient= (angleOfAttack > 0 ? -Mathf.Min(.3f, angleOfAttack) : Mathf.Max(-.3f, -angleOfAttack));
				float lift = _vhicl.MySettings.jetLift * atmosDensity * locvel.z * locvel.z * wingArea * liftCoefficient;
				_vhicl.MyRigidbody.AddRelativeForce(Vector3.up * Mathf.Lerp(lift, 0, stallFactor));
				//Debug.DrawRay (transform.position, transform.up * lift * .002, Color.green);
			}
		
			//Sideslip
			//if(stallFactor < .5 && Mathf.Abs(pitch) < 45) {
			//if(Mathf.Abs(roll) < 90) Vhicl.MyRigidbody.AddRelativeTorque(Mathf.Abs(roll) * mass * -.015, roll * mass * -.04, roll * mass * -.01);
			//else Vhicl.MyRigidbody.AddRelativeTorque(Mathf.Abs(roll) * mass * .015, roll * mass * .04, roll * mass * .01);
			//}
			//Vhicl.MyRigidbody.AddRelativeTorque(0, roll * mass * .04 * Mathf.Lerp(1, 0, Mathf.Abs(pitch) / 90) * (Mathf.Abs(roll) < 90 ? -1 : -1), 0);
		
			//Drag
			if(stallFactor >= .5) _vhicl.MyRigidbody.drag = speed * Mathf.Lerp(_vhicl.MySettings.jetDrag, _vhicl.MySettings.jetDrag * 5, Vector3.Angle(_vhicl.MyRigidbody.velocity, _vhicl.MyRigidbody.transform.forward) / 90) * atmosDensity;
			else {
				_vhicl.MyRigidbody.drag = 0;
				_vhicl.MyRigidbody.AddRelativeForce(new Vector3(locvel.x * -_vhicl.MySettings.jetDrag * 3, locvel.y * -_vhicl.MySettings.jetDrag * 3, locvel.z * -_vhicl.MySettings.jetDrag) * atmosDensity * (_vhicl.Brakes ? 5 : 1), ForceMode.VelocityChange);
			}
		}
	
//		//Floating
//		if(transform.position.y < _vhicl.MySettings.lavaAlt + 20 || Physics.Raycast(transform.position + (Vector3.up * 200), Vector3.down, hit, 220,  1 << 4)) {
//			Vector3 up = Vector3.up * 200;
//			Vector3 dn = Vector3.up * -1;
//			RaycastHit lavaHit;
//			foreach(Vector3 pt in bodyCollider.sharedMesh.vertices) {
//				float hitDistance;
//				pt = transform.TransformPoint(pt);
//				if(pt.y < _vhicl.MySettings.lavaAlt) hitDistance = (pt.y - _vhicl.MySettings.lavaAlt) * -1;
//				else if(hit.distance && hit.collider.Raycast(Ray(pt + up, dn), lavaHit, 200)) hitDistance = (200 - lavaHit.distance);
//				else continue;
//				Vector3 ptVel = _vhicl.MyRigidbody.GetPointVelocity(pt);
//				_vhicl.MyRigidbody.AddForceAtPosition((Vector3.up * lavaFloat * Mathf.Min(6, 3 + hitDistance) * Mathf.Lerp(1.3f, 5, Vector2(ptVel.x, ptVel.z).magnitude / 20) + ptVel * -_vhicl.MySettings.jetDrag * 70) / bodyCollider.sharedMesh.vertexCount, pt, ForceMode.VelocityChange);
//			}
//		}
	}

//Lava Collisions
/*void  OnTriggerStay ( Collider collider  ){
	if(collider.gameObject.tag != "BouyantLiquid") return;
	
	float drag= 100;
	float float= 100;
    RaycastHit hit;
    Vector3 up = transform.up * 10;

	foreach(Vector3 pt in bodyCollider.mesh.vertices) {
		if(collider.Raycast (transform.TransformPoint(pt) + up, hit, 100.0f)) {
	}
}*/
}