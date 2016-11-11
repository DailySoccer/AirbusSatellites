using UnityEngine;
using System.Collections;

public enum CameraState {
	FreeRotation,
	Fixed
}
public class SmoothCameraOrbit : MonoBehaviour {

	public static SmoothCameraOrbit Instance;

	public Transform target;
	public Transform currentCameraOrigin;

	public float distance 	= 75.0f;

	public float freeRotationSpeed 	= 250.0f;

	public int pitchBottomLimit = -80;
	public int pitchTopLimit 	= 20;

	public float freeRotationMaxSpeed 	= 20f;
	public float limitSpeedChangeDir 	= 10f;
	public float cameraTransitionTime 	= 1f;

	void Awake() {
		if (Instance == null)
			Instance = this;
	}

	void Start () 
	{
		_moveToRotation = transform.eulerAngles;
		_currentSmoothPosition = target.position;
		//_currentSmoothOrbitalDistance = distance;
		//_orbitalDistance = distance;
		
		var rot = Quaternion.Euler(_moveToRotation.y, _moveToRotation.x, 0);
    	var pos = rot * new Vector3(0.0f, 0.0f, -distance) + target.position;
		
        transform.rotation = rot;
        transform.position = pos;
		SetCameraState(CameraState.FreeRotation);
	}
	
	void LateUpdate () 
	{
		switch (cameraState) 
		{
		case CameraState.Fixed:
			_currentSmoothRotation = Vector3.SmoothDamp(_currentSmoothRotation, _moveToRotation, ref _lastSmoothAngleVelocity, _timeLeft/2);
			
			_currentSmoothPosition = Vector3.SmoothDamp(_currentSmoothPosition, _moveToPosition, ref _lastSmoothPositionVelocity, _timeLeft);


			
			//Debug.Log("POS --> Curr:" + _currentSmoothPosition + "MovetTo:" + _moveToPosition + " || ROT --> Curr:" + _currentSmoothRotation + "MovetTo:" + _moveToRotation);
			break;
		case CameraState.FreeRotation:
			bool isAcelerated = false;

			_aceleration = Vector3.zero;
			isAcelerated = GetInputAceleration(ref _aceleration);

			_aceleration += DesiredAcceleration;
			isAcelerated = isAcelerated || (DesiredAcceleration != Vector3.zero);
			DesiredAcceleration = Vector3.zero;


			if (isAcelerated) {
				if ( Mathf.Abs(_aceleration.y) > limitSpeedChangeDir && Mathf.Sign(freeRotationSpeed) != Mathf.Sign(_aceleration.y) ) {
					freeRotationSpeed *= -1;
				}
			} else {
				_aceleration.y += Time.fixedDeltaTime * freeRotationSpeed * 0.2f ;
			}


			_timeLeft = cameraTransitionTime / 2;


			_moveToRotation += _aceleration;
			_moveToRotation.x = ClampAngle(_moveToRotation.x, pitchBottomLimit, pitchTopLimit);
			_moveToPosition = target.position;
			//_orbitalDistance = distance;

			
			_currentSmoothRotation = Vector3.SmoothDamp(_currentSmoothRotation, _moveToRotation, ref _lastSmoothAngleVelocity, _timeLeft);
			
			_currentSmoothPosition = Vector3.SmoothDamp(_currentSmoothPosition, _moveToPosition, ref _lastSmoothPositionVelocity, _timeLeft/2);
			break;
		}

		// Movimiento de la cámara	
		if (target) 
		{	

			//_currentSmoothOrbitalDistance = Mathf.SmoothDamp(_currentSmoothOrbitalDistance, _orbitalDistance, ref _lastSmoothOrbitalVelocity, _timeLeft);

			Quaternion rotation = Quaternion.Euler(_currentSmoothRotation.x, _currentSmoothRotation.y, _currentSmoothRotation.z);

			if (_currentSmoothRotation.y > 360.0f && _moveToRotation.y > 360.0f) {
				_currentSmoothRotation.y %= 360;
				_moveToRotation.y %= 360;
			} else if (_currentSmoothRotation.y < 0.0f && _moveToRotation.y < 0.0f) {
				_currentSmoothRotation.y = 360 - (Mathf.Abs(_currentSmoothRotation.y) % 360.0f);
				_moveToRotation.y = 360 - (Mathf.Abs(_moveToRotation.y) % 360.0f);
			}

			transform.rotation = rotation;
			transform.position = _currentSmoothPosition + (rotation * new Vector3(0.0f, 0.0f, -distance));
		}

		_timeLeft = Mathf.Max(_timeLeft - Time.deltaTime, 0.0f);
		//Debug.Log("Time to target: " + _timeLeft.ToString());
		
	}

	public SmoothCameraOrbit ChangeCameraSettings(Transform newPosition, CameraState camState = CameraState.FreeRotation, float offsetDist = 0f, Transform newTarget = null)
	{
		_isDraggingMouse = false;

		if (offsetDist > 0) {
			float Dist = Vector3.Distance(newTarget.position, newPosition.position);
			Vector3 newPos = newPosition.position  + (newPosition.position - newTarget.position).normalized * (offsetDist);

			GameObject n = new GameObject ("temp");
			n.transform.position = newPos;
			n.transform.LookAt (newTarget);
			SetNewPosition(n.transform);
			Destroy(n);

		} else {
			if (newPosition != null) {
				SetNewPosition (newPosition);
			}
		}
		SetNewTarget (newTarget);
		SetCameraState (camState);

		return this;
	}

	public SmoothCameraOrbit SetNewPosition(Transform transf)
	{
		currentCameraOrigin = transf;

		// Make the rigid body not change rotation
		if (GetComponent<Rigidbody>())
			GetComponent<Rigidbody>().freezeRotation = false;

		return this;
	}
	
	public SmoothCameraOrbit SetNewTarget(Transform transf)
	{
		if (transf != null) {
			target = transf;
			_moveToRotation = transform.eulerAngles;
		 
		    // Make the rigid body not change rotation
		    if (GetComponent<Rigidbody>())
		        GetComponent<Rigidbody>().freezeRotation = false;
		}
		
		return this;
	}

	private static float ClampAngle (float angle, float min, float max) {
		float retAngle = angle;

		if ( retAngle > 180.0f ){
			retAngle -= 360.0f;

			retAngle = Mathf.Clamp (retAngle, min, max);
			retAngle += 360.0f;
		} else {
			retAngle = Mathf.Clamp (retAngle, min, max);
		}


		return retAngle; //Mathf.Clamp (angle, min, max);
	}
	
	private bool GetInputAceleration(ref Vector3 increment) {
		increment = Vector3.zero;
		bool thereIsInput = false;
		
		if (Input.touches.Length > 0) {
			thereIsInput = true;
			Touch touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Moved) 
			{
				increment.y = Mathf.Clamp(touch.deltaPosition.x * 0.3f, -freeRotationMaxSpeed, freeRotationMaxSpeed);
				increment.x = -touch.deltaPosition.y * 0.3f;
			}
		}
		
		// Control de Ratón
		if (Input.GetMouseButtonDown(0)) {
			_isDraggingMouse = true;
			_dragPosition = Input.mousePosition;
		}
		if(_isDraggingMouse) 
		{
			thereIsInput = true;
			increment.y = Mathf.Clamp(((Input.mousePosition.x - _dragPosition.x) / Time.deltaTime * 0.01f), -freeRotationMaxSpeed, freeRotationMaxSpeed);
			increment.x = -(Input.mousePosition.y - _dragPosition.y) / Time.deltaTime * 0.01f;
			_dragPosition = Input.mousePosition;					
			
		}				
		if(Input.GetMouseButtonUp(0)) {
			_isDraggingMouse = false;
		}
		
		return thereIsInput;
	}
	
	private void SetCameraState(CameraState newState)
	{
		switch (newState)
		{
		case CameraState.Fixed:
			if (currentCameraOrigin)
			{

				/*
				// Create a rotation based on the relative position of the player being the forward vector.
				_moveToRotation = Quaternion.LookRotation(relPlayerPosition, Vector3.up).eulerAngles;
				/*/

				_moveToRotation = currentCameraOrigin.rotation.eulerAngles;
				
				aproximateRotations();

				_moveToPosition = -(-currentCameraOrigin.position + (currentCameraOrigin.rotation * new Vector3(0.0f, 0.0f, -distance)));
				
				_timeLeft = cameraTransitionTime;

				//_orbitalDistance = 0.0f;
			}
			break;
		case CameraState.FreeRotation:
			/*if(cameraState != newState) {
				_moveToRotation = Vector3.zero;
			}*/
			aproximateRotations();
			break;
		}
		cameraState = newState;
	}

	private void aproximateRotations() {

		_currentSmoothRotation = currentSmoothRotationNormalized();
		_moveToRotation = moveToRotationNormalized();
		Vector3 diffNorm = _moveToRotation - _currentSmoothRotation;
		
		if (diffNorm.x > 180.0f) {
			_moveToRotation.x -= 360.0f;
		}
		if (diffNorm.x < -180.0f) {
			_moveToRotation.x += 360.0f;
		}
		
		if (diffNorm.y > 180.0f) {
			_moveToRotation.y -= 360.0f;
		}
		if (diffNorm.y < -180.0f) {
			_moveToRotation.y += 360.0f;
		}
		
		if (diffNorm.z > 180.0f) {
			_moveToRotation.z -= 360.0f;
		}
		if (diffNorm.z < -180.0f) {
			_moveToRotation.z += 360.0f;
		}

	}

	private Vector3 currentSmoothRotationNormalized() {
		Vector3 v = new Vector3(_currentSmoothRotation.x, _currentSmoothRotation.y, _currentSmoothRotation.z);
		return normalizeRotationVector(v);
	}

	private Vector3 moveToRotationNormalized() {
		Vector3 v = new Vector3(_moveToRotation.x, _moveToRotation.y, _moveToRotation.z);
		return normalizeRotationVector(v);
	}

	private Vector3 normalizeRotationVector(Vector3 vec) {
		Vector3 v = new Vector3(vec.x, vec.y, vec.z);

		while (v.x < -0.01f) {
			v.x += 360.0f;
		}
		while (v.y < -0.01f) {
			v.y += 360.0f;
		}
		while (v.z < -0.01f) {
			v.z += 360.0f;
		}

		v.x %= 360.0f;
		v.y %= 360.0f;
		v.z %= 360.0f;

		return v;
	}

	public SmoothCameraOrbit SetDesiredPitch(float inc)
	{
		float currAngleX = _moveToRotation.x;

		if (currAngleX > 180.0f) {
			currAngleX -= 360.0f;
		}

		float clampedInc = currAngleX < inc ? (inc - currAngleX) : 0.0f;

		DesiredAcceleration = new Vector3(clampedInc, 0, 0);

		return this;
	}

	public float GetTimeLeftToTarget() {
		return _timeLeft;
	}
	
	private Vector3 _aceleration = Vector3.zero;
	private Vector3 DesiredAcceleration = Vector3.zero;

	private Vector3 _moveToRotation;
	private Vector3 _currentSmoothRotation;
	private Vector3 _lastSmoothAngleVelocity;
	
	private Vector3 _moveToPosition;
	private Vector3 _currentSmoothPosition;
	private Vector3 _lastSmoothPositionVelocity;

	//private float _orbitalDistance;
	//private float _currentSmoothOrbitalDistance;
	private float _lastSmoothOrbitalVelocity;
	
	private float _timeLeft;

	private bool _isDraggingMouse;
	private Vector3 _dragPosition;

	private CameraState cameraState;
}
