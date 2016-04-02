using UnityEngine;
using System.Collections;

public class EnterBallVR : EnterInteractVR {

	private OVRCameraRig cameraController;
	private GameObject goDog;

	// Use this for initialization
	void Start () {
		cameraController = GameObject.Find ("OVRCameraRig").GetComponent<OVRCameraRig>();
		goDog = GameObject.FindGameObjectWithTag("dog");
		goDog.GetComponent<Animator> ().Play ("Stand");
		aiMoveCamera = new AIMoveCamera ();
		aiMoveCamera.Start (this);
	}
	
	// Update is called once per frame
	void Update () {
		aiMoveCamera.Update ();
		if (aiMoveCamera.IsFinished ()) 
		{
			ToBall();
		}
	}

	void ToBall()
	{
		// ball
		GameObject goBall = Instantiate(Resources.Load("Prefabs/Ball")) as GameObject;
		goBall.GetComponent<Rigidbody>().isKinematic = true;
		goBall.transform.position = cameraController.centerEyeAnchor.position + cameraController.centerEyeAnchor.rotation * (new Vector3(0, 0, goDog.GetComponent<DogController>().ballDistance));
		
		// script
		GameObject goPlay = Instantiate(Resources.Load("Prefabs/BallPlay")) as GameObject;
		goPlay.transform.parent = goBall.transform;
		
		Destroy(gameObject);
	}
}
