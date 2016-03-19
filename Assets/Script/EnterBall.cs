using UnityEngine;
using System.Collections;

public class EnterBall : EnterInteract {

	private GameObject mainCamera;

	// Use this for initialization
	void Start () {
		mainCamera = GameObject.Find ("OVRCameraRig");
		GameObject goDog = GameObject.FindGameObjectWithTag("dog");
		goDog.GetComponent<Animator> ().Play ("Stand");
		Vector3 direction = mainCamera.transform.position - goDog.GetComponent<DogController>().GetDogPivot();
		if (!Physics.Raycast (goDog.GetComponent<DogController>().GetDogPivot(), direction.normalized, direction.magnitude)
		    && (mainCamera.transform.position - goDog.GetComponent<DogController>().GetDogPivot()).magnitude > 1.0f) {
			ToBall();
			return;
		}

		// choose the farest lookat
		lookat = goDog.GetComponent<DogController> ().ChooseLookat ();

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
		goBall.transform.position = mainCamera.transform.position + mainCamera.transform.rotation * (new Vector3(0, 0, 0.4f + 0.08f));
		
		// script
		GameObject goPlay = Instantiate(Resources.Load("Prefabs/BallPlay")) as GameObject;
		goPlay.transform.parent = goBall.transform;
		
		Destroy(gameObject);
	}
}
