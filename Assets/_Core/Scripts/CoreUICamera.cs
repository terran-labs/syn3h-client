using UnityEngine;

public class CoreUICamera : MonoBehaviour
{
    public Transform InterfaceTransform;
    public Transform UiCameraTransform;
    public Transform OrbitGUIFocalPoint;

    public Transform OrbitCanvasTransform;
    public Vector2 OrbitCanvasRange = new Vector2(-7f, -3f);
    private Quaternion OrbitCanvasRotationStart;

    public Vector2 OrbitOffsetWorld;
    public Vector2 OrbitOffsetGUI;
    public float orbitSmoothSpeedCursor;

    public Vector2 OrbitOffsetGyro;
    public float gyroSmoothDownSpeed;
    private Vector2 gyroRotationSmoothed;
    public float orbitSmoothSpeedGyro;

    private bool isOrbitingUsingGyro = false;

    public float shakeRange;
    public float shakeTime;
    private float shakeTimeLast = 0;
    private Vector2 shakePositionLast;
    private Vector2 shakePositionTarget;
    private Vector2 shakePositionEased;

    private Vector2 orbitAmount;
    private int cameraMode = 0;

    void Start()
    {
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }
        else
        {
            var mousePos = Input.mousePosition;
            mousePos.x = Screen.width / 2;
            mousePos.y = Screen.height / 2;
        }

        OrbitCanvasRotationStart = OrbitCanvasTransform.localRotation;

		if (!CoreController.Instance)
		{
			return;
		}

		CoreController.Instance.OnWorldLoaded += OnWorldLoaded;
	}

	void OnDestroy()
	{
		if (!CoreController.Instance)
		{
			return;
		}

		CoreController.Instance.OnWorldLoaded -= OnWorldLoaded;
	}

	void OnWorldLoaded()
	{
		Debug.Log("LobbyCamera :: OnWorldLoaded");

		// Set UI position to position of first custom camera in loaded world
		// This makes non-playable worlds look much nicer when viewed in the background of the lobby ui
		if (WhirldData.Instance && WhirldData.Instance.CameraTransforms != null && WhirldData.Instance.CameraTransforms.Length > 0)
		{
			InterfaceTransform.position = WhirldData.Instance.CameraTransforms[0].position;
			InterfaceTransform.rotation = WhirldData.Instance.CameraTransforms[0].rotation;
		}
	}

    void LateUpdate()
    {
        // if they ever get an accelerometer reading, they are using a gyro-enabled device and shouldn't be orbiting using the cursor position
        if (!isOrbitingUsingGyro && Input.gyro.enabled && Input.acceleration != Vector3.zero)
        {
            isOrbitingUsingGyro = true;
        }

        Vector2 newOrbitAmount;

        // gyroscopic camera drift
        if (isOrbitingUsingGyro)
        {
            // gyroRotationSmoothed drifts "up" instantly, but drifts back down smoothly again.
            gyroRotationSmoothed += new Vector2(Input.gyro.rotationRateUnbiased.x, Input.gyro.rotationRateUnbiased.y);
            gyroRotationSmoothed = Vector3.Lerp(gyroRotationSmoothed, Vector3.zero,
                Time.deltaTime * gyroSmoothDownSpeed);

            /*
            if(Mathf.Abs(Input.gyro.rotationRateUnbiased.y) > Mathf.Abs(gyroRotationSmoothed.y)) {
                gyroRotationSmoothed.y = Input.gyro.rotationRateUnbiased.y;
            }
            else {
                gyroRotationSmoothed.y = Mathf.Lerp(gyroRotationSmoothed.y, Input.gyro.rotationRateUnbiased.y, Time.deltaTime * gyroSmoothDownSpeed);
            }
            if(Mathf.Abs(Input.gyro.rotationRateUnbiased.x) > Mathf.Abs(gyroRotationSmoothed.x)) {
                gyroRotationSmoothed.x = Input.gyro.rotationRateUnbiased.x;
            }
            else {
                gyroRotationSmoothed.x = Mathf.Lerp(gyroRotationSmoothed.x, Input.gyro.rotationRateUnbiased.x, Time.deltaTime * gyroSmoothDownSpeed);
            }
            */

            var gyroRotationNormalized = gyroRotationSmoothed;
            gyroRotationNormalized.x = gyroRotationNormalized.x * OrbitOffsetGyro.x;
            gyroRotationNormalized.y = gyroRotationNormalized.y * OrbitOffsetGyro.y;
            gyroRotationNormalized = Vector2.Max(gyroRotationNormalized, Vector2.one * -1);
            gyroRotationNormalized = Vector2.Min(gyroRotationNormalized, Vector2.one);

            // assign desired orbit amount from smoothed gyro rotation
            newOrbitAmount.x = gyroRotationNormalized.y * -1;
            newOrbitAmount.y = gyroRotationNormalized.x;

            // smooth current orbit position towards desired orbit position
            orbitAmount = Vector2.Lerp(orbitAmount, newOrbitAmount, Time.deltaTime * orbitSmoothSpeedGyro);
        }

        else
        {
            // calculate desired orbit amount from cursor position
            if (Input.mousePosition.x > 0 || Input.mousePosition.y > 0)
            {
                newOrbitAmount = new Vector2(
                    Mathf.Lerp(1, -1, (Screen.width - Input.mousePosition.x) / Screen.width),
                    Mathf.Lerp(1, -1, (Screen.height - Input.mousePosition.y) / Screen.height)
                );

                // smooth current orbit position towards desired orbit position
                orbitAmount = Vector2.Lerp(orbitAmount, newOrbitAmount, Time.deltaTime * orbitSmoothSpeedCursor);
            }
        }

        // add a bit of camera shake to keep things moving
        if (shakeTimeLast == 0f || Time.time - shakeTime > shakeTimeLast)
        {
            shakePositionLast = shakePositionTarget;
            shakeTimeLast = Time.time;
            shakePositionTarget.x = Random.Range(-shakeRange, shakeRange);
            shakePositionTarget.y = Random.Range(-shakeRange, shakeRange);
        }
        shakePositionEased.x = easeInOutQuad(
            Time.time - shakeTimeLast, // t = Time (current)
            shakePositionLast.x, // b = Beginning (value)
            shakePositionTarget.x - shakePositionLast.x, // c = Change (in value)
            shakeTime // d = Duration (desired in totality)
        );
        shakePositionEased.y = easeInOutQuad(
            Time.time - shakeTimeLast, // t = Time (current)
            shakePositionLast.y, // b = Beginning (value)
            shakePositionTarget.y - shakePositionLast.y, // c = Change (in value)
            shakeTime // d = Duration (desired in totality)
        );

        //LobbyCameraTransform.localPosition.x = orbitAmount.x * OrbitOffsetWorld.x;
        //LobbyCameraTransform.localPosition.y = orbitAmount.y * OrbitOffsetWorld.y;
//        LobbyCameraTransform.Rotate(
//            -1 * ( orbitAmount.y - shakePositionEased.y ) * OrbitOffsetWorld.y,
//            ( orbitAmount.x - shakePositionEased.x ) * OrbitOffsetWorld.x,
//            0
//        );

        // orbit camera
        UiCameraTransform.localPosition = new Vector3(shakePositionEased.x + orbitAmount.x * OrbitOffsetGUI.x,
            shakePositionEased.y + orbitAmount.y * OrbitOffsetGUI.y, UiCameraTransform.localPosition.z);
        UiCameraTransform.LookAt(OrbitGUIFocalPoint.position);

        // tilt canvas
        OrbitCanvasTransform.localRotation = OrbitCanvasRotationStart * Quaternion.Euler(-orbitAmount.y * OrbitCanvasRange.y, orbitAmount.x * OrbitCanvasRange.x, 0f);

/*
	// out to new level
	if(cameraMode < -1 && gravCloud.active == false) {
		//DontDestroyOnLoad (gravCloud);
		gravCloud.active = true;
	}

	// Standard Spawning
	if(cameraMode == 0) {
		if(timeLastPosReset == 0) {
			timeLastPosReset = Time.timeSinceLevelLoad;
			wantedDist = 0.8f;
			wantedHeight = 0.1f;
			spinSpeed = .2;
		}
		else if(Time.timeSinceLevelLoad > timeLastPosReset + resetPosTimer) {
			timeLastPosReset = Time.timeSinceLevelLoad;
			wantedDist = Random.Range(.6, 1.5f);
			wantedHeight = Random.Range(-.6, .6);
			spinSpeed = Random.Range(.01, .3);
		}
	}

	// About page
	else if(cameraMode > 0) {
		wantedDist = 3;
		wantedHeight = .4;
		spinSpeed = .03;
	}

	// Zooming out to new map
	else if (cameraMode < -1) {
		wantedDist = 5;
		wantedHeight = 20;
		spinSpeed = 0;
	}

	distDiff = Vector3.Distance(transform.position, Vector3.zero) - wantedDist;

	// smooth rotation
	transform.rotation = Quaternion.Slerp(
		transform.rotation,
		Quaternion.LookRotation(transform.position * -1 + transform.TransformDirection(angularOffset)),
		Time.deltaTime * rotationLockSpeed
	);

	// instantaneous rotation
	//transform.rotation = Quaternion.LookRotation(transform.position * -1 + transform.TransformDirection(angularOffset));

	transform.position += transform.TransformDirection(
		spinSpeed * Time.deltaTime,
		(transform.position.y - wantedHeight) * -Time.deltaTime,
		(distDiff) * distLockSpeed * Time.deltaTime
	);
	*/
    }

// easing functions!
// http://gizma.com/easing/
// sample usage: currentValue = startValue + easeFunction(x, t, b, c, d) * (endValue - startValue)
// t = Time (current), b = Beginning (value), c = Change (in value), d = Duration (desired in totality)
    float easeInOutLinear(float t, float b, float c, float d)
    {
        return c * t / d + b;
    }

    float easeInOutQuad(float t, float b, float c, float d)
    {
        t = t / (d / 2);
        if (t < 1) return c / 2 * t * t + b;
        t--;
        return -c / 2 * (t * (t - 2) - 1) + b;
    }
}