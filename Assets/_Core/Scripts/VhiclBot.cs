using UnityEngine;

public class VhiclBot : MonoBehaviour
{
    private enum VehicleBotMovement
    {
        PursueQuarry,
        ResetFromCorner
    }

    public Vhicl Vhicl;
    private GameObject _quarry;
    private VehicleBotMovement moveMode;
    public float UpdateFrequency = 1.5f;
    public float StuckMinDeltaMoveDistance = 1f;
    private Vector3 _lastPosition;

//	private int botMovement;
//	private int botEnemySelection;
//	private float enemyUpdateTime;
//	private float laserFireTime = 0.00f;

    private void Start()
    {
        InvokeRepeating("_updateMovementStrategy", Random.Range(0, UpdateFrequency), UpdateFrequency);
    }

    private void FixedUpdate()
    {
        if (!_quarry)
        {
            return;
        }

        var distance = Vector3.Distance(_quarry.transform.position, Vhicl.MyRigidbody.position);
        var rotation = (Quaternion.LookRotation(_quarry.transform.position - Vhicl.MyRigidbody.position).eulerAngles.y - transform.localRotation.eulerAngles.y);

        if (rotation > 180) rotation = rotation - 360; //180;
        if (rotation < -180) rotation = rotation + 360;

        // Movement :: Buggy
        if (moveMode == VehicleBotMovement.PursueQuarry)
        {
            rotation = rotation / 20;
            if (distance < 15) Vhicl.Input.x = (Mathf.Abs(rotation) < .1 ? 0 : Mathf.Clamp(rotation, -1, 1));
            else Vhicl.Input.x = (Mathf.Abs(rotation) < .1 ? 0 : Mathf.Clamp(rotation, -.6f, .6f));
            Vhicl.Input.y = 1;
            //if(Vhicl.MyRigidbody.velocity.magnitude < 100) specialInput = true;
            Vhicl.SpecialInput = false;
        }
        else if (moveMode == VehicleBotMovement.ResetFromCorner)
        {
            rotation = rotation / 20;
            Vhicl.Input.x = Mathf.Clamp(rotation * -1, -1f, 1f);
            Vhicl.Input.y = -1;
        }
    }

    private void _updateMovementStrategy()
    {
        _quarry = Vhicl.MyController.Player;

        var moveDist = Vector3.Distance(Vhicl.MyRigidbody.position, _lastPosition);

        if (moveDist < StuckMinDeltaMoveDistance)
        {
            moveMode = VehicleBotMovement.ResetFromCorner;
        }
        else
        {
            moveMode = VehicleBotMovement.PursueQuarry;
        }

        _lastPosition = Vhicl.MyRigidbody.position;
    }


//		//Enemy updates
//		/*if(enemyUpdateTime == 0.00f || Time.time - 2 > enemyUpdateTime) {
//			enemyUpdateTime = Time.time;
//
//			if(botEnemySelection == 1) {
//				float distance = Mathf.Infinity;
//				foreach(DictionaryEntry plrE in Game.Players) {
//					if(!plrE.Value.gameObject) continue;
//					GameObject go = plrE.Value.gameObject;
//				    float diff = (go.transform.position - transform.position);
//				    float curDistance = diff.sqrMagnitude;
//				    if (curDistance < distance && go.name != name) {
//				    	_quarry = go;
//				        distance = curDistance;
//	   				}
//				}
//			}
//			else if(botEnemySelection == 2) {
//				foreach(DictionaryEntry plrE in Game.Players) {
//	   	 			if(plrE.Value.isIt == 1) {
//	   	 				_quarry = plrE.Value.gameObject;
//	   	 				break;
//	   	 			}
//				}
//			}
//		}
//
//		//Rabbit Hunt Mode
//		//if(true) {
//			if(Vhicl.isIt) {
//				botMovement = 1;
//				botEnemySelection = 1;
//			}
//			else {
//				botMovement = 2;
//				botEnemySelection = 2;
//			}
//		/*}
//
//		//Tag Mode
//		else {
//			if(!Vhicl.isIt) {
//				botMovement = 1;
//			}
//			else {
//				botMovement = 2;
//				botEnemySelection = 1;
//			}
//		}*/
//
//		if(Settings.botsCanDrive) {
//			//Hiding movement
//			if(botMovement == 1) {
//				//Tank
//				if(Vhicl.vehId == 2) {
//					Vhicl.input.x = (Time.time % 2 == 0 ? 0 : Random.value * 2 - 1);
//					Vhicl.input.y = 1;
//				}
//				//All others
//				else {
//					Vhicl.input.x = Random.value * 2 - 1;
//					Vhicl.input.y = 1;
//				}
//			}
//
//			//Persuing movement
//			else if(botMovement == 2) {
//				if(_quarry) {
//					float distance = Vector3.Distance(_quarry.transform.position, Vhicl.MyRigidbody.position);
//					float rotation = (Quaternion.LookRotation(_quarry.transform.position - Vhicl.MyRigidbody.position).eulerAngles.y - transform.localRotation.eulerAngles.y);
//					if(rotation > 180) rotation = rotation - 360; //180;
//					if(rotation < -180) rotation = rotation + 360;
//
//					//Buggy
//					if(Vhicl.vehId == 0) {
//						rotation = rotation / 20;
//						if(distance < 15) Vhicl.input.x = (Mathf.Abs(rotation) < .1 ? 0 : Mathf.Clamp(rotation, -1, 1));
//						else Vhicl.input.x = (Mathf.Abs(rotation) < .1 ? 0 : Mathf.Clamp(rotation, -.6f, .6f));
//						Vhicl.input.y = 1;
//						//if(Vhicl.MyRigidbody.velocity.magnitude < 100) specialInput = true;
//						Vhicl.specialInput = false;
//					}
//					//Hovercraft
//					else if(Vhicl.vehId == 1) {
//						rotation = rotation / 15;
//						Vhicl.input.x = (Mathf.Abs(rotation) < .1 ? 0 : Mathf.Clamp(rotation, -1, 1));
//						Vhicl.input.y = 1;
//						Vhicl.specialInput = false;
//					}
//					//Tank
//					else if(Vhicl.vehId == 2) {
//						rotation = rotation / 80;
//						Vhicl.input.x = (Mathf.Abs(rotation) < .3 ? 0 : Mathf.Clamp(rotation, -1, 1));
//						Vhicl.input.y = (Mathf.Abs(rotation) > 1 ? 0 : 1);
//						Vhicl.specialInput = false;
//					}
//					//Jet
//					else if(Vhicl.vehId == 3) {
//						rotation = rotation / 10;
//						Vhicl.input.x = (Mathf.Abs(rotation) < .1 ? 0 : Mathf.Clamp(rotation, -1, 1));
//						Vhicl.input.y = 1;
//						Vhicl.input.z = (Physics.Raycast(transform.position, Vector3.up * -1, 4) ? 1 : .3f);
//						Vhicl.specialInput = true;
//					}
//				}
//			}
//		}
//		else {
//			Vhicl.input.x = 0;
//			Vhicl.input.y = 0;
//		}
//
//		//Fire!
//		if (_quarry && Settings.botsCanFire && (Time.time - 1 > laserFireTime && !Physics.Linecast(transform.position, _quarry.transform.position,  1 << 8))) {
//			laserFireTime = Time.time;
//			//DRAGONHEREphotonView.RPC("fRl", PhotonTargets.All, photonView.viewID, PhotonNetwork.time + "", Vhicl.ridePos.position + Vhicl.transform.up * -.1f, _quarry.photonView.viewID/*Quaternion.LookRotation((_quarry.transform.position + Vhicl.transform.up * -.1) - Vhicl.ridePos.position).eulerAngles*/);
//
//			//FIXME_VAR_TYPE btemp;
//			//btemp = Instantiate(laser, ridePos.position + transform.TransformDirection(Vector3.up) * .08, brang);
//			//btemp.GetComponent<"VhiclLaser">().launchedBy = gameObject;
//			//if(networked) {
//				//FIXME_VAR_TYPE viewID= PhotonNetwork.AllocateViewID();
//			//	transform.root.photonView.RPC("FireVhiclLaser" + "" , PhotonTargets.Others, Vhicl.MyRigidbody.transform.root.photonView.viewID, brang);
//			//}
//		}
//
//		//Bounds Checking
//		if(Vhicl.MyRigidbody.position.y < -300) {
//			Vhicl.MyRigidbody.velocity = Vhicl.MyRigidbody.velocity.normalized;
//			Vhicl.MyRigidbody.isKinematic = true;
//			//DRAGONHEREvhicl.transform.position = WhirldController.S.whirldBase.position;
//			Vhicl.MyRigidbody.isKinematic = false;
//			//DRAGONHERECore.S.Broadcast(name + " fell off the planet");
//		}
//}
}