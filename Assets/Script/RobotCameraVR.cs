using UnityEngine;
using System.Collections;

public class RobotCameraVR : MonoBehaviour {

	private GameObject mainCamera;

	// Use this for initialization
	void Start () {
		mainCamera = GameObject.Find("OVRCameraRig");
		//mainCamera.transform.position = GameObject.Find("CameraPosRobot").transform.position;
		//mainCamera.transform.rotation = GameObject.Find("CameraPosRobot").transform.rotation;
	}
}
