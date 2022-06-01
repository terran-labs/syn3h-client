using UnityEngine;

public class VhiclMe : MonoBehaviour
{
    public Vhicl Vhicl;
    public float AutoBrakeMaxVelocitySquared = 1.0f;
    private float _laserFireTime = 0.00f;

    //PhotonView photonView;

    void Update()
    {
        //Input Assignments
        Vhicl.Input.x = Input.GetAxis("Horizontal");
        Vhicl.Input.y = Input.GetAxis("Vertical");
        Vhicl.Input.z = Input.GetAxis("Throttle");
        Vhicl.Input.w = Input.GetAxis("Yaw");
        if (Vhicl.InputThrottle)
        {
            Vhicl.Input.z = (Vhicl.Input.z + 1) * .5f;
        }
        else
        {
            if (Vhicl.Input.x > -.1 && Vhicl.Input.x < .1) Vhicl.Input.x = Vhicl.Input.w;
            if (Vhicl.Input.y > -.1 && Vhicl.Input.y < .1) Vhicl.Input.y = Vhicl.Input.z;
        }

        // Brakes
        if (Input.GetButton("Interact") ||
            (Vhicl.Input == Vector4.zero && Vhicl.MyRigidbody.velocity.sqrMagnitude < AutoBrakeMaxVelocitySquared))
        {
            Vhicl.Brakes = true;
        }
        else
        {
            Vhicl.Brakes = false;
        }

        // "Special"
        if (Input.GetButtonDown("Special") /*&& !Settings.chatting && Time.time > Settings.kpTime*/)
        {
            Vhicl.SpecialInput = !Vhicl.SpecialInput;
//            photonView.RPC("sI", PhotonTargets.All, Vhicl.specialInput ? false : true);
//            Settings.kpTime = Time.time + Settings.kpDur;
        }


//        if(Input.GetButtonDown("Brakes") && (!Game.Messaging || !Settings.chatting)) photonView.RPC("sB", PhotonTargets.All, Vhicl.brakes ? false : true);
//        if (Input.GetButton("Brakes") && !Vhicl.brakes) MonoBehaviour.photonView.RPC("sB", PhotonTargets.All, true);
//        else if (!Input.GetButton("Brakes") && Vhicl.brakes) photonView.RPC("sB", PhotonTargets.All, false);

        //Weapons Locking
//        GameObject laserLock;
//        if (Settings.laserLocking)
//        {
//            RaycastHit hit;
//            if (Physics.Raycast(
//                    transform.position + transform.forward *
//                    (((Settings.laserLock[Vhicl.vehId]) + Vhicl.camOffset * .1f) * 15), transform.forward, out hit,
//                    Mathf.Infinity, 1 << 14) && (Vhicl.isIt || hit.transform.gameObject.GetComponent<Vhicl>().isIt))
//            {
//                laserLock = hit.transform.gameObject;
//                Vhicl.laserAimer.active = false;
//                Vhicl.laserAimerLocked.active = true;
//            }
//            else
//            {
//                laserLock = null;
//                Vhicl.laserAimer.active = true;
//                Vhicl.laserAimerLocked.active = false;
//            }
//        }
//        else
//        {
//            Vhicl.laserAimer.active = Vhicl.laserAimerLocked.active = false;
//            laserLock = null;
//        }
//
//        //Firing
//        if (
//            Vhicl.ridePos &&
//            Settings.lasersAllowed &&
//            _laserFireTime < Time.time &&
//            Settings.firepower[Vhicl.vehId] > 0 &&
//            (laserLock || //Autofiring With Weapons Lock
//             (Input.GetButton("Fire") && !Input.GetMouseButton(0)) || //Firing with Joystick
//             (Input.GetButton("Fire") && (Input.GetButton("Ride") || Input.GetButton("Snipe") ||
//                                          Settings.camMode == 0)) || //Firing with mouse while inside Vhicl
//             (Input.GetButton("Fire") && ((Input.mousePosition.y > 50 &&
//                                           Input.mousePosition.x <
//                                           Screen.width - 200))) //Firing with mouse while outside Vhicl
//            )
//        )
//        {
//            if (laserLock)
//            {
//                bool snipe = (Settings.firepower[Vhicl.vehId] > 2 ||
//                              (Settings.firepower[Vhicl.vehId] > 1 &&
//                               (laserLock.MyRigidbody.velocity.sqrMagnitude > 500 ||
//                                Vector3.Distance(transform.position, laserLock.transform.position) > 500)));
//                photonView.RPC((snipe ? "fSl" : "fRl"), PhotonTargets.All, photonView.viewID, PhotonNetwork.time + "",
//                    Vhicl.ridePos.position + Vhicl.transform.up * -.1f, laserLock.photonView.viewID);
//            }
//            else
//            {
//                if (Settings.camMode == 0 || Input.GetButton("Ride") || Input.GetButton("Snipe"))
//                {
//                    Quaternion rang = camera.main.transform.rotation;
//                }
//                else
//                {
//                    rang = Vhicl.ridePos.rotation;
//                }
//                snipe = ((Input.GetButton("Snipe") && Settings.firepower[Vhicl.vehId] > 1) ||
//                         Settings.firepower[Vhicl.vehId] > 2);
//                photonView.RPC((snipe ? "fS" : "fR"), PhotonTargets.All, photonView.viewID, PhotonNetwork.time + "",
//                    Vhicl.ridePos.position + Vhicl.transform.up * -.1, rang.eulerAngles);
//            }
//            _laserFireTime = Time.time + ((snipe ? (Settings.firepower[Vhicl.vehId] > 2 ? .5 : 2) : .25));
//        }

        //Bounds Checking
        if (Vhicl.MyRigidbody.position.y < -300)
        {
            Vhicl.MyRigidbody.velocity = Vhicl.MyRigidbody.velocity.normalized;
            Vhicl.MyRigidbody.isKinematic = true;
//			Vhicl.transform.position = WhirldController.S.whirldBase.position;
            Vhicl.transform.position = Vector3.up * 100;
            Vhicl.MyRigidbody.isKinematic = false;
//            Messaging.Broadcast(name + " fell off the planet");
        }

        // Velocity clamping
//        if (Vhicl.MyRigidbody.velocity.magnitude > 500) Vhicl.MyRigidbody.velocity = Vhicl.MyRigidbody.velocity * .5;
    }

    void FixedUpdate()
    {
//		if(Vhicl.ramoSphere && Vhicl.zorbBall) {
//			if(Vhicl.input.y || Vhicl.input.x) {
//			.MyRigidbody.AddForce(Vector3.Scale(Vector3(1,0,1), Camera.main.transform.TransformDirection(Vector3(Vhicl.input.x * Mathf.Max(0,Settings.zorbSpeed + Settings.zorbAgility),0,Vhicl.input.y * Settings.zorbSpeed))), ForceMode.Acceleration);
//			.MyRigidbody.AddTorque(Camera.main.transform.TransformDirection(Vector3(Vhicl.input.y,0,-Vhicl.input.x)) * Settings.zorbSpeed, ForceMode.Acceleration);
//			}
//		}
    }

    void OnPrefsUpdated()
    {
//		if(Settings.renderLevel > 4) {
//			Light lt = gameObject.GetComponentInChildren<Light>();
//			if(lt) lt.enabled = true;
//		}
//		if(Settings.renderLevel > 3) {
//		    foreach (TrailRenderer tr in gameObject.GetComponentsInChildren<TrailRenderer>()) {
//				tr.enabled = true;
//			}
//		}
    }
}