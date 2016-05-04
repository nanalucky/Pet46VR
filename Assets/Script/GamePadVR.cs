using UnityEngine;
using System.Collections;

public class GamePadVR : MonoBehaviour {

	private float cameraSpeed = 1.0f;

	private GameObject mainCamera;
	private OVRCameraRig cameraController;
	private GameObject goDog;
	private DogController dogController;


	// Use this for initialization
	void Start () {
		mainCamera = GameObject.Find ("OVRCameraRig");
		cameraController = mainCamera.GetComponent<OVRCameraRig> ();
		goDog = GameObject.FindGameObjectWithTag("dog");
		dogController = goDog.GetComponent<DogController> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (GameObject.FindGameObjectWithTag("RobotScript") != null)
		{
			Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
			if(primaryAxis.x == 0)
			{
				if(Input.GetKeyDown(KeyCode.A))
					primaryAxis.x = -1.0f;
				if(Input.GetKeyDown(KeyCode.D))
					primaryAxis.x = 1.0f;
			}
			if(primaryAxis.y == 0)
			{
				if(Input.GetKeyDown(KeyCode.W))
					primaryAxis.y = 1.0f;
				if(Input.GetKeyDown(KeyCode.S))
					primaryAxis.y = -1.0f;
			}
			if(primaryAxis.x != 0.0f || primaryAxis.y != 0.0f)
			{
				Vector3 fwd = cameraController.centerEyeAnchor.TransformDirection((new Vector3(primaryAxis.x, 0.0f, primaryAxis.y)).normalized);
				fwd.y = 0.0f;
				fwd.Normalize();

				Vector3 pos = mainCamera.transform.position + fwd * (cameraSpeed * Time.deltaTime);
				float x = Mathf.Clamp(pos.x, dogController.zoneMin.x, dogController.zoneMax.x);
				float z = Mathf.Clamp(pos.z, dogController.zoneMin.z, dogController.zoneMax.z);
				mainCamera.transform.position = new Vector3(x, mainCamera.transform.position.y, z);
			}
		}
	}
}
