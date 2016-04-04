using UnityEngine;
using System.Collections;
using System;

public class RotateCameraVR : MonoBehaviour {

	public float MouseSensitivity = 10;   
	
	private GameObject goDog;
	private GameObject mainCamera;
	private float mouseX = 0f;
	private float mouseY = 0f;

	// Use this for initialization
	void Start () {
		goDog = GameObject.FindGameObjectWithTag ("dog");
		mainCamera = GameObject.Find ("OVRCameraRig");
		OVRTouchpad.TouchHandler += LocalTouchEventCallback;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton(1))
		{
			RotateCamera(Input.GetAxis("Mouse X") * MouseSensitivity);
		}

		Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
		Vector2 secondaryAxis = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
		RotateCamera ((primaryAxis.x + secondaryAxis.x));
	}

	void OnDestroy()
	{
		OVRTouchpad.TouchHandler -= LocalTouchEventCallback;
	}
	
	void LocalTouchEventCallback(object sender, EventArgs args)
	{
		var touchArgs = (OVRTouchpad.TouchArgs)args;
		OVRTouchpad.TouchEvent touchEvent = touchArgs.TouchType;

		float mouseYOffset = 0.0f;
		switch(touchEvent)
		{
		case OVRTouchpad.TouchEvent.SingleTap:
			mouseYOffset += MouseSensitivity;
			break;
			
		case OVRTouchpad.TouchEvent.Left:
			//mouseYOffset += MouseSensitivity;
			break;
			
		case OVRTouchpad.TouchEvent.Right:
			//mouseYOffset -= MouseSensitivity;
			break;
			
		case OVRTouchpad.TouchEvent.Up:
			break;
			
		case OVRTouchpad.TouchEvent.Down:
			break;
		}

		RotateCamera (mouseYOffset);
	}

	void RotateCamera(float mouseYOffset)
	{
		Vector3 lookat = goDog.transform.position;
		lookat.y = mainCamera.transform.position.y;
		float rayDist = (lookat - mainCamera.transform.position).magnitude;
		
		mouseX = mainCamera.transform.rotation.eulerAngles.x;
		mouseY = mainCamera.transform.rotation.eulerAngles.y;	
		
		//mouseX -= Input.GetAxis("Mouse Y") * MouseSensitivity;
		mouseY += mouseYOffset;
		
		mainCamera.transform.rotation = Quaternion.Euler(mouseX, mouseY, mainCamera.transform.rotation.eulerAngles.z);
		mainCamera.transform.position = lookat + mainCamera.transform.rotation * (new Vector3(0, 0, -rayDist));
	}
}
