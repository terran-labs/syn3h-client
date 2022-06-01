using System;
using UnityEngine;

public class VhiclBuggy : MonoBehaviour
{
    public MeshFilter Wing0;
    public MeshFilter Wing1;
    public GameObject WheelPrefab;
    public GameObject SkidmarksPrefab;
    public Mesh FloatMesh;
    public Vector3 WheelPos;
    public Vector3 AxelPos;
    public float WingOpenSpeed = 12f;
    private Transform[] _wheels = new Transform[4];
    private Transform[] _axels = new Transform[4];
    private Transform[] _wheelGraphics = new Transform[4];
    private Vector3[] _baseVertices;
    private Mesh _wingMesh;
    private float _wingState;
    private bool _shouldWingsBeOpen;
    private bool _areWingsOpen;
    private bool _isInverted;
    private Vhicl _vhicl;
    private float _suspensionRange;
    private float _friction;
    private float[] _hitDistance = new float[4];
    private float[] _hitCompress = new float[4];
    private Vector3[] _hitVelocity = new Vector3[4];
    private Vector3[] _wheelPositn = new Vector3[4];
    private Vector3[] _hitForce = new Vector3[4];
    private float[] _hitFriction = new float[4];
    private VhiclSkidmarks _skidmarks;
    private int[] _skidmarkIndex = new int[4];
    private float _frictionTotal;
    private float _brakePower;
    private float _motorTorque;
    private float _motorSpeed;
    private float _motorSpd;
    private float _motorInputSmoothed;
    private float _wheelRadius = 0.5f;
    private float _motorMass = 1f;
    private float _motorDrag = 1f;
    private float _maxAcceleration = 60f;
    private float _motorAccel = 60f;
    private float _motorSpeedNew;
    private float _totalSuspensionCompression;
    private bool _wheelsAreTouchingGround;

    public void InitVhicl(Vhicl veh)
    {
        _vhicl = veh;

        // Init Tinting
//        Array MaterialMains = new Array();
//        Array MaterialAccents = new Array();
//        _vhicl.MaterialBright = new Material[2];
//        _vhicl.MaterialMain = MaterialMains.ToBuiltin(Material);
//        _vhicl.MaterialAccent = MaterialAccents.ToBuiltin(Material);
//        _vhicl.MaterialBright[0] = wing0.GetComponent.<Renderer>().material;
//        _vhicl.MaterialBright[1] = wing1.GetComponent.<Renderer>().material;
//        MeshRenderer bodyMesh = transform.Find("Lod0/Body").GetComponent<MeshRenderer>();
//        MaterialMains.Add(bodyMesh.materials[1]);
//        MaterialAccents.Add(bodyMesh.materials[3]);
//    	bodyMesh = transform.Find("Lod1/Body").GetComponent<MeshRenderer>();
//    	MaterialMains.Add(bodyMesh.materials[1]);
//    	MaterialAccents.Add(bodyMesh.materials[3]);

        // Instantiate Wheels
        for (var i = 0; i < 4; i++)
        {
            _wheelPositn[i] = new Vector3(WheelPos.x * (i % 2 != 0 ? 1 : -1), WheelPos.y, WheelPos.z * (i < 2 ? 1 : -1));
            GameObject go = Instantiate(WheelPrefab, transform.TransformPoint(_wheelPositn[i]), transform.rotation);
            _wheelPositn[i] = new Vector3((float) (_wheelPositn[i].x + (i % 2 != 0 ? .22 : -.22)), _wheelPositn[i].y, _wheelPositn[i].z);
            _wheels[i] = go.transform;
            _wheelGraphics[i] = _wheels[i].Find("Roll").transform;
//            MaterialAccents.Add(_wheels[i].Find("Rim").GetComponent<MeshRenderer>().material);
//            MaterialAccents.Add(_wheels[i].Find("RimSim").GetComponent<MeshRenderer>().material);
//            MaterialAccents.Add(wheelGraphics[i].Find("WheelPrefab").GetComponent<MeshRenderer>().materials[2]);
//            MaterialAccents.Add(wheelGraphics[i].Find("WheelSim").GetComponent<MeshRenderer>().materials[2]);
            _axels[i] = _wheels[i].Find("Axle").transform;
//            MaterialAccents.Add(axels[i].gameObject.GetComponent<MeshRenderer>().material);
            _wheels[i].parent = transform;
        }

        // Initialize Bouyancy Points
        /*new Transform[floatPoints.childCount];
        foreach(Transform pt in floatPoints) {
            bouyancyPoints[i] = pt;
            i++;
        }*/

        OnPrefsUpdated();
    }

    public void Update()
    {
        // Wings
        _vhicl.InputThrottle = _areWingsOpen;
        Wing0.gameObject.SetActive(_areWingsOpen);
        Wing1.gameObject.SetActive(_areWingsOpen);
        if (_areWingsOpen)
        {
//           wingMesh = (lod.level == 0 ? wing0 : wing1).mesh;
            _wingMesh = Wing0.mesh;
            if (_baseVertices == null) _baseVertices = _wingMesh.vertices;
            var vertices = new Vector3[_baseVertices.Length];
            for (var i = 0; i < vertices.Length; i++)
            {
                var pos = _baseVertices[i];

                // Open/close wings
                if (_wingState < 1)
                {
                    pos.y = pos.y * _wingState;
                    pos.x = pos.x * _wingState;
                }

                // Warp wings per user input
                else
                {
                    var t = pos.z * ((_vhicl.InputSmoothed.x * 0.05f) + (_motorInputSmoothed * 0.05f * (pos.x > 0 ? -1 : 1)));
                    t *= (Mathf.Abs(pos.x) / 10);
                    t += pos.x * (_motorInputSmoothed * 0.01f);
                    var st = Mathf.Sin(t);
                    var ct = Mathf.Cos(t);
                    pos.x = pos.y * st + pos.x * ct;
                    pos.y = pos.y * ct - pos.x * st;
                }
                vertices[i] = pos;
            }
            _wingMesh.vertices = vertices;
        }

        // Wheels
        var wheelCircumference = _wheelRadius * Mathf.PI * 2;
        for (var i = 0; i < 4; i++)
        {
            var pos = new Vector3(WheelPos.x * (i % 2 != 0 ? 1 : -1), WheelPos.y - (_hitDistance[i] == -1 ? _suspensionRange : (_hitDistance[i] - _wheelRadius)),
                WheelPos.z * (i < 2 ? 1 : -1));
            _wheels[i].transform.position = transform.TransformPoint(pos);
            //_wheels[i].Rotate((_vhicl.MyRigidbody.GetPointVelocity(pos).magnitude / wheelCircumference) * Time.deltaTime, 0, 0); //360 * (_motorSpeed / wheelCircumference)
            _wheelGraphics[i].transform.Rotate(360 * (_motorSpeed / wheelCircumference) * Time.deltaTime * .5f, 0f, 0f);
            if (_axels[i].gameObject.activeInHierarchy)
            {
                _axels[i].rotation = Quaternion.LookRotation(
                    transform.TransformPoint(new Vector3(AxelPos.x * (i % 2 != 0 ? 1 : -1), AxelPos.y, AxelPos.z * (i < 2 ? 1 : -1))) -
                    _wheels[i].transform.position,
                    transform.up);
            }
        }
    }

    public void FixedUpdate()
    {
        _wheelsAreTouchingGround = false;
        _totalSuspensionCompression = 0f;
        RaycastHit hit;
        var myVelocity = _vhicl.MyRigidbody.velocity.magnitude;

        // Smart suspension
        var desiredSuspensionRange = .5f;
        if (_areWingsOpen)
        {
            // Auto gear retract if we are sufficiently high above the terrain
            if (!Physics.Raycast(transform.position, Vector3.up * -1, 5, _vhicl.TerrainMask))
            {
                desiredSuspensionRange = 0;
//                    _vhicl.MyRigidbody.centerOfMass.z = -.2;
            }
            // If, on the other hand, we are near the terrain... Leave the gear at full extension
        }
        else
        {
            desiredSuspensionRange = Mathf.Lerp(0.4f, 0.1f, Mathf.Min(1, myVelocity / _vhicl.MySettings.buggySpeed));
        }
        _suspensionRange = Mathf.Lerp(_suspensionRange, desiredSuspensionRange, Time.deltaTime * 1);

        var com = new Vector3(_vhicl.MyRigidbody.centerOfMass.x, _vhicl.MySettings.buggyCG * _suspensionRange * .5f, .2f);
        _vhicl.MyRigidbody.centerOfMass = com;
        //_vhicl.MyRigidbody.mass = 30;
        //_vhicl.MyRigidbody.inertiaTensor = Vector3(7, 7, 3);

        // Early-out for non-authorative networked vehicles
        // Update wheel positions to match ground, and call it a day
        if (_vhicl.MyRigidbody.isKinematic)
        {
            for (int i = 0; i < 4; i++)
            {
                if (Physics.Raycast(transform.TransformPoint(_wheelPositn[i]), transform.up * -1, out hit, _suspensionRange + _wheelRadius, _vhicl.TerrainMask))
                {
                    _motorSpeed = _hitVelocity[i].z;
                    _hitDistance[i] = hit.distance;
                }
                else _hitDistance[i] = -1;
            }
            return;
        }

        // Wing state determination
        _shouldWingsBeOpen = _vhicl.SpecialInput;
        if (_shouldWingsBeOpen && _wingState > .95)
        {
            _wingState = 1;
        }
        else if (!_shouldWingsBeOpen && _wingState < .05)
        {
            _wingState = 0;
        }
        else
        {
            _wingState = Mathf.Lerp(_wingState, _shouldWingsBeOpen ? 1 : 0, Time.fixedDeltaTime * WingOpenSpeed);
        }
        _areWingsOpen = _wingState > 0.01;

        // Steering
        var steeringAngle = Mathf.Lerp(30, 25, myVelocity / _vhicl.MySettings.buggySpeed);
        _wheels[0].localRotation = _wheels[1].localRotation =
            Quaternion.LookRotation(new Vector3(_vhicl.InputSmoothed.x * (steeringAngle / 90), 0, 1 + (-1 * Mathf.Abs(_vhicl.InputSmoothed.x * (steeringAngle / 90)))));
        steeringAngle = Mathf.Lerp(30, 10, myVelocity / _vhicl.MySettings.buggySpeed);
        _wheels[2].localRotation = _wheels[3].localRotation =
            Quaternion.LookRotation(new Vector3(-_vhicl.InputSmoothed.x * (steeringAngle / 90), 0, 1 + (-1 * Mathf.Abs(_vhicl.InputSmoothed.x * (steeringAngle / 90)))));

        // Experimental Motor Physics
        if (_vhicl.MySettings.buggyNewPhysics)
        {
//            _motorTorque = -_vhicl.InputSmoothed.y * Mathf.Lerp(_vhicl.Settings.buggyPower * 3, 0, _hitVelocity[0].z / _vhicl.Settings.buggySpeed);
//
////Apply wheel force
//            frictionTotal = 0;
//            for (i = 0; i < 4; i++)
//            {
////		    var hit RaycastHit;
//                if (Physics.Raycast(transform.TransformPoint(_wheelPositn[i]), transform.up * -1, hit, _suspensionRange + _wheelRadius, _vhicl.TerrainMask))
//                {
//                    if (_motorTorque == 0 || _motorTorque < (hitFriction[i] * hitForce[i].z)) _motorSpeed = _hitVelocity[i].z; //Static Friction
//                    else _motorSpeed = Mathf.Lerp(_vhicl.Settings.buggySpeed, 0, (_motorTorque - (hitFriction[i] * hitForce[i].z)) / _motorTorque); //Dynamic Friction
////_motorSpeed += -_motorSpeed * _motorDrag / _motorTorque * Time.fixedDeltaTime;
//                    motorSpd = (frictionTotal - _vhicl.Settings.buggyPower * 3) / (_vhicl.Settings.buggyPower * 3 / _vhicl.Settings.buggySpeed);
//                    _wheelsAreTouchingGround = true;
//                    isDynamic = ((_motorTorque > hitFriction[i]) || (Mathf.Abs(_hitVelocity[i].x) > Mathf.Abs(_hitVelocity[i].z) * .3));
//                    _hitDistance[i] = hit.distance;
//                    _hitCompress[i] = -((hit.distance) / (_suspensionRange + _wheelRadius)) + 1;
//                    _hitVelocity[i] = _wheels[i].InverseTransformDirection(_vhicl.MyRigidbody.GetPointVelocity(transform.TransformPoint(_wheelPositn[i])));
//                    if (isDynamic)
//                    {
//                        hitFriction[i] = _vhicl.Settings.buggyTr * 60;
////Debug.DrawRay(transform.TransformPoint(_wheelPositn[i]),transform.up * 5, Color.red);
////getSpringForce(comp, vel.y) *				//Spring Compression position, normalized (0-1)
////Mathf.Lerp(1, 1, Mathf.Min(comp * 4, 1))	//Static tire friction coeffecient, as function of downforce*/
//                    }
//                    else
//                    {
//                        hitFriction[i] = _vhicl.Settings.buggyTr * 150 * Mathf.Lerp(1.5f, .5, Mathf.Min(_hitCompress[i] * 3, 1));
//                    }
//                    Vector3 dir = Vector3(_hitVelocity[i].x, 0, (_vhicl.Settings.buggyAWD == true || i > 1 ? (_hitVelocity[i].z - _motorSpeed) : 0));
//                    if (dir.magnitude > 1) dir = dir.normalized;
//                    hitForce[i] = dir;
////Debug.DrawRay(transform.TransformPoint(_wheelPositn[i]),transform.right * dir.x, Color.blue);
////Debug.DrawRay(transform.TransformPoint(_wheelPositn[i]),transform.forward * dir.z, Color.blue);
//                    Vector3 force = _wheels[i].TransformDirection(dir * -hitFriction[i]);
////Debug.DrawRay(hit.point,force / 50);
//                    _vhicl.MyRigidbody.AddForceAtPosition(force, hit.point);
//                    if (_skidmarks)
//                        _skidmarkIndex[i] = _skidmarks.AddSkidMark(hit.point, hit.normal, (isDynamic ? 1 : Mathf.Min(.5, force.magnitude * .0025)),
//                            _skidmarkIndex[i]); //Do Tire Tracks
//                    frictionTotal += hitFriction[i];
//                }
//                else
//                {
//                    _hitDistance[i] = -1;
//                    _skidmarkIndex[i] = -1;
//                }
//            }
        }

        // Modified Yoggy physics
        else
        {
            // Motor
            var motorTorqueMax = Mathf.Lerp(_vhicl.MySettings.buggyPower * 5, 0, _motorSpeed / (_vhicl.MySettings.buggySpeed * 10)) *
                                 Mathf.Abs((_areWingsOpen ? 0 : _vhicl.InputSmoothed.y));
            _motorTorque = Mathf.Max(1, motorTorqueMax);
            _motorAccel = Mathf.Lerp(_maxAcceleration, 0, _motorSpeed / (_vhicl.MySettings.buggySpeed * 10));
            _motorSpeed += _vhicl.InputSmoothed.y * _motorAccel / _motorMass * Time.fixedDeltaTime;
            _motorSpeed += -_motorSpeed * (_vhicl.Brakes ? 50 : _motorDrag) / _motorTorque * Time.fixedDeltaTime;

            // WheelPrefab / Terrain Collisions
            for (var i = 0; i < 4; i++)
            {
                if (Physics.Raycast(transform.TransformPoint(_wheelPositn[i]), transform.up * -1, out hit, _suspensionRange + _wheelRadius, _vhicl.TerrainMask,
                    QueryTriggerInteraction.Ignore))
                {
//		        Debug.DrawRay(transform.TransformPoint(_wheelPositn[i]),transform.up * -1, Color.red);
//                Debug.Log("WheelPrefab (" + i + ") hit: " + hit.transform.name);
                    _wheelsAreTouchingGround = true;
                    _hitCompress[i] = -((hit.distance) / (_suspensionRange + _wheelRadius)) + 1;
                    _totalSuspensionCompression += _hitCompress[i];
                    _hitVelocity[i] = _wheels[i].InverseTransformDirection(_vhicl.MyRigidbody.GetPointVelocity(transform.TransformPoint(_wheelPositn[i])));
                    if (hit.rigidbody)
                    {
                        _vhicl.MyRigidbody.AddForceAtPosition((_hitVelocity[i] - _wheels[i].InverseTransformDirection(hit.rigidbody.GetPointVelocity(hit.point))) / 4, hit.point,
                            ForceMode.VelocityChange);
                        //_vhicl.transform.position += (hit.rigidbody.GetPointVelocity(hit.point) * Time.fixedDeltaTime) / 4;
                        //_hitVelocity[i] = hit.rigidbody.GetPointVelocity(hit.point);
                        //_hitVelocity[i] = hit.rigidbody.GetPointVelocity(hit.point);
                    }
                    var friction = _vhicl.MySettings.buggyTr * 13 * Mathf.Lerp(.5f, 1, _hitCompress[i]) * Mathf.Max(1, (20 - _hitVelocity[i].magnitude) / 4);
                    _vhicl.MyRigidbody.AddForceAtPosition(_wheels[i]
                        .TransformDirection(Vector3.Min(new Vector3(
                            -_hitVelocity[i].x * friction, //Sideslip
                            0,
                            -(_hitVelocity[i].z - _motorSpeed) * friction //Motor
                        ), new Vector3(1000, 1000, 1000))), hit.point);
                    _motorSpeed += ((_hitVelocity[i].z - _motorSpeed) * friction * Time.fixedDeltaTime) / _motorTorque;
                    var hitOffset = transform.TransformDirection(new Vector3(i % 2 != 0 ? -.25f : .25f, 0f, .1f));
                    if (_skidmarks)
                    {
                        _skidmarkIndex[i] = _skidmarks.AddSkidMark(hit.point + hitOffset, hit.normal,
                            (Mathf.Abs(_hitVelocity[i].x) > Mathf.Abs(_hitVelocity[i].z) * .3f
                                ? Mathf.Abs(_vhicl.InputSmoothed.y) * .5f + .25f
                                : Mathf.Min(.5f, friction * .05f)),
                            _skidmarkIndex[i]); //Do Tire Tracks
                    }
                }
                else
                {
//                Debug.DrawRay(transform.TransformPoint(_wheelPositn[i]),transform.up * -1, Color.green);
                    hit.distance = -1;
                    _skidmarkIndex[i] = -1;
                }
                _hitDistance[i] = hit.distance;
            }
        }

        // Suspension
        for (var i = 0; i < 4; i++)
        {
            if (_hitDistance[i] == -1 || _isInverted) continue;
//print(getSpringForce(_hitCompress[i], _hitVelocity[i].y) + "===" + i);
            float wheelForce = (-_hitVelocity[i].y * _vhicl.MySettings.buggySh * .8f * (_areWingsOpen ? 3 : 1) +
                                _hitCompress[i] * (20 * _vhicl.MyRigidbody.mass) * (_areWingsOpen && i < 2 ? 8 : 1));
            _vhicl.MyRigidbody.AddForceAtPosition(transform.up * wheelForce, transform.TransformPoint(_wheelPositn[i]));
//Debug.DrawRay(transform.TransformPoint(_wheelPositn[i]),transform.up * getSpringForce(_hitCompress[i], _hitVelocity[i].y) / 50, Color.green);
        }

//Floating
//	if((transform.position.y < _vhicl.Settings.lavaAlt + .1 && transform.position.y - _vhicl.Settings.lavaAlt > -3) || Physics.Raycast(transform.position + (Vector3.up * 3), Vector3.down, hit, 3.1f,  1 << 4)) {
//
//		//Vars
//		if(_areWingsOpen && hit.distance < 2) _vhicl.MyRigidbody.AddForce(Vector3.up * 400);
//		roll = (transform.eulerAngles.z > 180 ? transform.eulerAngles.z - 360 : transform.eulerAngles.z);
//		pitch = (transform.eulerAngles.x > 180 ? transform.eulerAngles.x - 360 : transform.eulerAngles.x);
//		_vhicl.MyRigidbody.angularDrag = 2;
//		//_vhicl.MyRigidbody.angularVelocity = Vector3(_vhicl.MyRigidbody.angularVelocity.x,0,0);
//
//		//Flowing Lava
//		float waterAngle;
//		Vector3 waterAxis;
//		if(hit.distance && hit.transform) {
//			hit.transform.rotation.ToAngleAxis(waterAngle, waterAxis);
//			if(waterAngle != 0) {
//				_vhicl.MyRigidbody.AddForce(hit.transform.rotation.eulerAngles * .8);
//			}
//		}
//
//		//BouyancyPoints
//		foreach(Vector3 m in floatCollider.vertices) {
//			m = transform.TransformPoint(m);
//			if (m.y < _vhicl.Settings.lavaAlt || Physics.Raycast (m + (Vector3.up * 3), Vector3.down, hit, 3,  1 << 4)) {
//				float bouyancyY = (hit.distance ? hit.distance - 5 : m.y - 2 - _vhicl.Settings.lavaAlt);
//				if(bouyancyY < -1.8f) bouyancyY = -1.8f;
//				_vhicl.MyRigidbody.AddForceAtPosition((
//					Vector3(0, -bouyancyY * (100 + _vhicl.MyRigidbody.GetPointVelocity(m).magnitude * (_vhicl.MyRigidbody.GetPointVelocity(m).magnitude > 15 ? 100 : 15)), 0) //skip force : bouyancy force
//					+ _vhicl.MyRigidbody.GetPointVelocity(m) * -200 //Drag
//				) / floatCollider.vertices.length
//				, m);
//			}
//		}
//		if(_vhicl.InputSmoothed.y >= 0) _vhicl.MyRigidbody.AddRelativeTorque(Vector3(-_vhicl.InputSmoothed.y * 500 * ((70 - Mathf.Min(70, Mathf.Max(1,pitch * -1))) / 70)/* + (Mathf.PingPong(Time.time, 200) * 2)*/, _vhicl.InputSmoothed.y * _vhicl.InputSmoothed.x * 300, (roll * -3) + _vhicl.InputSmoothed.y * _vhicl.InputSmoothed.x * -50));
//		if(!_areWingsOpen && hit.distance < 3) _vhicl.MyRigidbody.AddRelativeForce(Vector3.forward * _vhicl.InputSmoothed.y * 1200);
//	}
//
//	//Diving
//	else if(transform.position.y < _vhicl.Settings.lavaAlt || Physics.Raycast(transform.position + (Vector3.up * 200), Vector3.down, 200,  1 << 4)) {
//		_vhicl.MyRigidbody.AddForce(_vhicl.MyRigidbody.velocity * -8 + Vector3.up * (_areWingsOpen ? 400 : 200));
//		_vhicl.MyRigidbody.angularDrag = 2;
//	}

        // Flight
        if (_shouldWingsBeOpen)
        {
            _motorInputSmoothed = Mathf.Lerp(_vhicl.InputSmoothed.y, Mathf.Clamp(_motorInputSmoothed + (_vhicl.Brakes ? -.8f : 0f), -1.2f, 1), .8f);
            var stallSpeed = 16f;
            var locVel = transform.InverseTransformDirection(_vhicl.MyRigidbody.velocity);
            var roll = (transform.eulerAngles.z > 180 ? transform.eulerAngles.z - 360 : transform.eulerAngles.z);
            var pitch = (transform.eulerAngles.x > 180 ? transform.eulerAngles.x - 360 : transform.eulerAngles.x);
            if (locVel.sqrMagnitude > stallSpeed)
            {
                _vhicl.MyRigidbody.drag = myVelocity / _vhicl.MySettings.buggyFlightDrag * 0.3f;

                // Airbrakes
                if (_vhicl.Brakes)
                {
                    if (_brakePower < 1f) _brakePower += Time.deltaTime * .15f;
                    float multiplier = -_brakePower * 2;
                    _vhicl.MyRigidbody.AddRelativeForce(locVel.x * multiplier * 5, locVel.y * multiplier * 100, locVel.z * 150 * multiplier);
                    _vhicl.MyRigidbody.AddRelativeTorque(new Vector3((pitch + (_vhicl.InputSmoothed.y * -100)) * -2, _vhicl.InputSmoothed.x * 280, (roll) * -1));
                }

                // Standard Flight
                else
                {
                    _brakePower = 0;
                    float angDelta = Vector3.Angle(_vhicl.MyRigidbody.velocity, transform.TransformDirection(Vector3.forward));
                    if (angDelta > 10 && _vhicl.MySettings.buggyFlightSlip)
                    {
                        _vhicl.MyRigidbody.velocity = _vhicl.MyRigidbody.transform.TransformDirection(locVel.x * .95f, locVel.y * .95f,
                            locVel.z + (Mathf.Abs(locVel.x) + Mathf.Abs(locVel.y)) * .1f * (angDelta / 360));
                    }
                    else
                    {
                        _vhicl.MyRigidbody.velocity = _vhicl.MyRigidbody.transform.TransformDirection(0, 0,
                            locVel.magnitude + (Time.deltaTime * 50 * (_vhicl.MySettings.buggyFlightLooPower
                                                    ? Mathf.Abs(_motorInputSmoothed) / 10
                                                    : (_motorInputSmoothed < .999 && _motorInputSmoothed >
                                                       -.999
                                                        ? Mathf.Abs(_motorInputSmoothed) / 10
                                                        : 0)))
                        );
                    }
                    _vhicl.MyRigidbody.AddRelativeTorque(
                        new Vector3(_motorInputSmoothed * 10 * _vhicl.MySettings.buggyFlightAgility, 0, _vhicl.InputSmoothed.x * -13 * _vhicl.MySettings.buggyFlightAgility),
                        ForceMode.Acceleration);
                }

                // Slideslip - "Dihedral"
                if (Math.Abs(_vhicl.InputSmoothed.x) < 0.01f && (transform.eulerAngles.z < 90 || transform.eulerAngles.z > 270))
                {
                    _vhicl.MyRigidbody.AddRelativeTorque(
                        ((transform.eulerAngles.x < 10 || transform.eulerAngles.x > 350) ? pitch - .95f : 0f) * -0, //4
                        roll * -.6f,
                        ((transform.eulerAngles.z < 20 || transform.eulerAngles.z > 340) ? roll * -0.5f : 0f)
                    );
                }
                else if (Math.Abs(_vhicl.InputSmoothed.x) < 0.01f)
                {
                    _vhicl.MyRigidbody.AddRelativeTorque(
                        0f,
                        (transform.eulerAngles.z - 180) * .4f, 0f
                    );
                }

                // Turbulence
                //if(Time.time % .125 == 0 && (!_vhicl.networked || photonView.isMine)) {
                //	FIXME_VAR_TYPE trb= 15;
                //	_vhicl.MyRigidbody.AddRelativeTorque(Vector3(Random.Range(-trb, trb), Random.Range(-trb, trb), Random.Range(-trb, trb)));
                //}

                // Lava "Thermals"
                if (transform.position.y < _vhicl.MySettings.lavaAlt + 10)
                {
                    if (Physics.Raycast(transform.position, Vector3.down, out hit, 10f, 1 << 4))
                    {
                        _vhicl.MyRigidbody.AddForce(Vector3.up * (10 - hit.distance) * 40);
                    }
                }

                //_vhicl.MyRigidbody.MoveRotation(_vhicl.MyRigidbody.rotation * Quaternion.Euler(Vector3 (_vhicl.InputSmoothed.y * 60, 0, _vhicl.InputSmoothed.x * -200 + _vhicl.MyRigidbody.rotation.y * 0.5f) * Time.deltaTime));
                // _vhicl.MyRigidbody.AddRelativeForce(Vector3.up * 200);
                _vhicl.MyRigidbody.angularDrag = 5;
            }

            // Stalling
            else
            {
                _vhicl.MyRigidbody.angularDrag = 1;
                _vhicl.MyRigidbody.drag = myVelocity / _vhicl.MySettings.buggyFlightDrag * 9;
                _vhicl.MyRigidbody.AddRelativeTorque(new Vector3(_vhicl.InputSmoothed.y + 0.5f * 100, 0, _vhicl.InputSmoothed.x * -30));
            }
        }

        // Non-flight Drag & Brakes
        else
        {
            /*if(_vhicl.Settings.buggyNewPhysics) {
                _vhicl.MyRigidbody.angularDrag = .2;
                _vhicl.MyRigidbody.drag = .01;
            } else {*/

            var eligibleforBrakes = _wheelsAreTouchingGround;

            // Prevent brakes from freezing vehicle in a crouch during world spawn
            if (_totalSuspensionCompression > 1)
            {
                eligibleforBrakes = false;
            }

            _vhicl.MyRigidbody.maxAngularVelocity = 7f;

            if (_vhicl.Brakes && eligibleforBrakes && Math.Abs(_vhicl.InputSmoothed.y) > 0.01)
            {
                _vhicl.MyRigidbody.drag = 2;
                _vhicl.MyRigidbody.angularDrag = 1;
            }
            else if (_vhicl.Brakes && eligibleforBrakes && myVelocity < 1.5f)
            {
                _vhicl.MyRigidbody.drag = 50;
                _vhicl.MyRigidbody.maxAngularVelocity = 0f;
                _motorSpeed = 0;
            }
            else if (_vhicl.Brakes && eligibleforBrakes && myVelocity < 10)
            {
                _vhicl.MyRigidbody.drag = 10;
            }
            else
            {
                _vhicl.MyRigidbody.angularDrag = .2f;
                _vhicl.MyRigidbody.drag = .01f;
            }
        }

        // Collision Friction
        // No need for this code now that we have a dedicated skidplate collider
//		if (_areWingsOpen || !_isInverted)
//		{
//			//We are cruising along, bounce right over bumps in the road
//			BodyCollider.material.dynamicFriction = 0;
//			BodyCollider.material.staticFriction = 0;
//			BodyCollider.material.frictionCombine = PhysicMaterialCombine.Minimum;
//			BodyCollider.material.bounceCombine = PhysicMaterialCombine.Average;
//		}
//		else
//		{
//			//We are crashing - apply much more friction on collisions
//			BodyCollider.material.dynamicFriction = .8f;
//			BodyCollider.material.staticFriction = .9f;
//			BodyCollider.material.frictionCombine = PhysicMaterialCombine.Average;
//			BodyCollider.material.bounceCombine = PhysicMaterialCombine.Multiply; //.5 * .5 = .25
//		}

//    Debug.Log("Buggy simulated (_wheelsAreTouchingGround: " + _wheelsAreTouchingGround + ")");    //  + LayerMask.LayerToName(_vhicl.TerrainMask)
    }

    // Self-righting
    void OnCollisionStay(Collision collision)
    {
        if (_vhicl.ZorbBall)
        {
            return;
        }

        foreach (ContactPoint contact in collision.contacts)
        {
            //if(contact.otherCollider.gameObject.layer != 8) continue;
            if (_isInverted && Vector3.Angle(transform.up, contact.normal) < 50) _isInverted = false;
            else if (!_isInverted && Vector3.Angle(transform.up, contact.normal) > 90) _isInverted = true;
            if (_isInverted && _vhicl.MyRigidbody.angularVelocity.sqrMagnitude < 2 && _vhicl.MyRigidbody.velocity.sqrMagnitude < 8)
            {
                _vhicl.MyRigidbody.AddTorque(Vector3.Cross(transform.up, Vector3.up) * Vector3.Angle(transform.up, Vector3.up) * .5f, ForceMode.Acceleration);
            }
        }
    }

    public void OnDisable()
    {
        if (_skidmarks) Destroy(_skidmarks.gameObject);
    }

    public void OnPrefsUpdated()
    {
        _updateSkidmarks();
    }

    private void _updateSkidmarks()
    {
        int qualityLevel = QualitySettings.GetQualityLevel();
        if (qualityLevel < 3)
        {
            if (_skidmarks)
            {
                Destroy(_skidmarks.gameObject);
                _skidmarks = null;
            }
        }
        else
        {
            if (!_skidmarks)
            {
                var go = Instantiate(SkidmarksPrefab, Vector3.zero, Quaternion.identity);
                go.layer = 11;
                _skidmarks = go.GetComponentInChildren<VhiclSkidmarks>();
                go.transform.parent = _vhicl.MySettings.DynamicObjects;
            }
        }
    }

    // Networked instance updates
//    public VhiclBuggy(bool _areWingsOpen, Transform[] wheelGraphics, Transform[] axels, float _motorInputSmoothed)
//    {
//        this._areWingsOpen = _areWingsOpen;
//        _wheelGraphics = wheelGraphics;
//        _axels = axels;
//        this._motorInputSmoothed = _motorInputSmoothed;
//    }

//
//    /*
//    void  OnSetColor (){
//        wing0.renderer.material.color.a = 0;
//        Vector4 colBright = wing0.renderer.material.color;
//        colBright = Vector4.Normalize(colBright);
//        wing0.renderer.material.color = colBright;
//    }
//    */
//
//    //void  OnLOD ( int level  ){
//    //	int i;
//    //	for(i=0; i<4; i++) {
//    //		axels[i].gameObject.SetActiveRecursively(level == 0);
//    //		wheelGraphics[i].Find("WheelPrefab").gameObject.active = (level == 0);
//    //		_wheels[i].Find("Rim").gameObject.active = (level == 0);
//    //		wheelGraphics[i].Find("WheelSim").gameObject.active = !(level == 0);
//    //		_wheels[i].Find("RimSim").gameObject.active = !(level == 0);
//    //	}
//    //}
}