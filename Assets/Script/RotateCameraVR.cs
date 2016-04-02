using UnityEngine;
using System.Collections;
using System;

public class RotateCameraVR : MonoBehaviour {

	public float MouseSensitivity = 10;   
	
	private GameObject goDog;
	private GameObject mainCamera;
	private float mouseX = 0f;
	private float mouseY = 0f;

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	// Use this for initialization
	void Start () {
		goDog = GameObject.FindGameObjectWithTag ("dog");
		mainCamera = GameObject.Find ("OVRCameraRig");
		OVRTouchpad.TouchHandler += LocalTouchEventCallback;
	}
	
	// Update is called once per frame
	void Update () {
		OVRTouchpad.Update();

		if (Input.GetMouseButton(1))
		{
			Plane plane = new Plane( new Vector3(0, 1, 0), goDog.GetComponent<DogController>().GetDogPivot());
			Vector3 direction = mainCamera.transform.rotation * (new Vector3(0,0,1));
			Ray ray = new Ray(mainCamera.transform.position, direction);
			float rayDist;
			plane.Raycast(ray, out rayDist);
			Vector3 lookat = ray.GetPoint(rayDist);

			mouseX = mainCamera.transform.rotation.eulerAngles.x;
			mouseY = mainCamera.transform.rotation.eulerAngles.y;	

			//mouseX -= Input.GetAxis("Mouse Y") * MouseSensitivity;
			mouseY += Input.GetAxis("Mouse X") * MouseSensitivity;
			
			mainCamera.transform.rotation = Quaternion.Euler(mouseX, mouseY, mainCamera.transform.rotation.eulerAngles.z);
			mainCamera.transform.position = lookat + mainCamera.transform.rotation * (new Vector3(0, 0, -rayDist));
		}	
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
			//Debug.Log("SINGLE CLICK\n");
			break;
			
		case OVRTouchpad.TouchEvent.Left:
			//Debug.Log("LEFT SWIPE\n");
			mouseYOffset += MouseSensitivity;
			break;
			
		case OVRTouchpad.TouchEvent.Right:
			//Debug.Log("RIGHT SWIPE\n");
			mouseYOffset -= MouseSensitivity;
			break;
			
		case OVRTouchpad.TouchEvent.Up:
			//Debug.Log("UP SWIPE\n");
			break;
			
		case OVRTouchpad.TouchEvent.Down:
			//Debug.Log("DOWN SWIPE\n");
			break;
		}

//		Plane plane = new Plane( new Vector3(0, 1, 0), goDog.GetComponent<DogController>().GetDogPivot());
//		Vector3 direction = mainCamera.transform.rotation * (new Vector3(0,0,1));
//		Ray ray = new Ray(mainCamera.transform.position, direction);
//		float rayDist;
//		plane.Raycast(ray, out rayDist);
//		Vector3 lookat = ray.GetPoint(rayDist);
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
