using UnityEngine;
using System.Collections;

public class BallCamera : MonoBehaviour {

	private GameObject goBall;
	private GameObject mainCamera;

	// Use this for initialization
	void Start () {
		goBall = GameObject.FindGameObjectWithTag ("Ball");
		mainCamera = GameObject.Find ("OVRCameraRig");
	}
	
	// Update is called once per frame
	void Update () {
		mainCamera.transform.rotation = Quaternion.LookRotation ((goBall.transform.position - mainCamera.transform.position).normalized);
		mainCamera.transform.position = mainCamera.transform.position;
		if (goBall.GetComponent<Rigidbody>().velocity == Vector3.zero)
		{
			this.enabled = false;
		}
	}
}
