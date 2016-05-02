using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;
using System;

public class ThrowBallVR : MonoBehaviour {

	public float forceMagnitude = 1000.0f;

	private OVRCameraRig cameraController;

	private GameObject goBall;
	private Rigidbody rb;

	private GameObject goDog;
	private LookAtIK lookatIK;

	private bool firstFrame = true;

	void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}

	// Use this for initialization
	void Start () {
		Time.fixedDeltaTime = 0.01f;
	
		cameraController = GameObject.Find ("OVRCameraRig").GetComponent<OVRCameraRig>();

		goBall = GameObject.FindGameObjectWithTag ("Ball");
		rb = goBall.GetComponent<Rigidbody> ();
	
		goDog = GameObject.FindGameObjectWithTag("dog");
		lookatIK = goDog.GetComponent<LookAtIK>();

		OVRTouchpad.TouchHandler += LocalTouchEventCallback;
	}
	
	// Update is called once per frame
	void Update () {
		if (firstFrame) {
			firstFrame = false;
			goDog.GetComponent<DogController>().EnableLookatIK(true);
		}

		lookatIK.solver.IKPosition = goBall.transform.position;

		goBall.transform.position = cameraController.centerEyeAnchor.position + cameraController.centerEyeAnchor.rotation * (new Vector3(0, 0, goDog.GetComponent<DogController>().ballDistance));
	}

	void OnDestroy()
	{
		OVRTouchpad.TouchHandler -= LocalTouchEventCallback;
	}

	public void ThrowBall()
	{
		Vector3 force = cameraController.centerEyeAnchor.rotation * (new Vector3(0,0,forceMagnitude));
		//force /= Time.deltaTime;
		//force *= forceMultiplier;
		rb.isKinematic = false;
		rb.AddForce(force);
		this.gameObject.GetComponent<BallDogControllerVR>().enabled = true;					
		this.enabled = false;
	}

	void LocalTouchEventCallback(object sender, EventArgs args)
	{
		var touchArgs = (OVRTouchpad.TouchArgs)args;
		OVRTouchpad.TouchEvent touchEvent = touchArgs.TouchType;
		
		switch(touchEvent)
		{
		case OVRTouchpad.TouchEvent.SingleTap:
			//Debug.Log("SINGLE CLICK\n");
			ThrowBall();
			break;
			
		case OVRTouchpad.TouchEvent.Left:
			//Debug.Log("LEFT SWIPE\n");
			break;
			
		case OVRTouchpad.TouchEvent.Right:
			//Debug.Log("RIGHT SWIPE\n");
			break;
			
		case OVRTouchpad.TouchEvent.Up:
			//Debug.Log("UP SWIPE\n");
			break;
			
		case OVRTouchpad.TouchEvent.Down:
			//Debug.Log("DOWN SWIPE\n");
			break;
		}
	}
}
