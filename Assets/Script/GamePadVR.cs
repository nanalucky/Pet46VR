using UnityEngine;
using System.Collections;

public class GamePadVR : MonoBehaviour {

	public float cameraSpeed = 1.0f;
	private float cameraHeightMin = 0.1f;
	private float cameraHeightMax = 1.0f;
	private float cameraHeightSpeed = 0.1f;
	private float cameraRotateSpeed = 60.0f;

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
		//if (GameObject.FindGameObjectWithTag("RobotScript") != null)
		{
			// camera position
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
			if(Mathf.Abs(primaryAxis.y) > Mathf.Abs(primaryAxis.x))
			{
				Vector3 euler = cameraController.transform.rotation.eulerAngles;
				euler.x = euler.z = 0.0f;
				Vector3 fwd = Matrix4x4.TRS( Vector3.zero, Quaternion.Euler(euler), new Vector3(1,1,1)).MultiplyVector((new Vector3(0.0f, 0.0f, primaryAxis.y)).normalized);

				Vector3 pos = mainCamera.transform.position + fwd * (cameraSpeed * Time.deltaTime);
				float x = Mathf.Clamp(pos.x, dogController.zoneMin.x, dogController.zoneMax.x);
				float z = Mathf.Clamp(pos.z, dogController.zoneMin.z, dogController.zoneMax.z);
				mainCamera.transform.position = new Vector3(x, mainCamera.transform.position.y, z);
			}

			// rotate
			if(Mathf.Abs(primaryAxis.x) > Mathf.Abs(primaryAxis.y))
			{
				Vector3 euler = cameraController.transform.rotation.eulerAngles;
				euler.y += primaryAxis.x * cameraRotateSpeed * Time.deltaTime;
				cameraController.transform.rotation = Quaternion.Euler(euler);
			}

			// camera height
			float deltaHeight = 0.0f;
			if(OVRInput.GetDown(OVRInput.RawButton.DpadUp))
				deltaHeight = cameraHeightSpeed;
			if(OVRInput.GetDown(OVRInput.RawButton.DpadDown))
				deltaHeight += -1 * cameraHeightSpeed;
			float y = mainCamera.transform.position.y + deltaHeight;
			y = Mathf.Clamp(y, cameraHeightMin, cameraHeightMax);
			mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, y, mainCamera.transform.position.z);
		
			// turn
/*			if(Input.GetButtonDown (OVRGamepadController.ButtonNames [(int)OVRGamepadController.Button.LeftShoulder]))
			{
				Vector3 euler = mainCamera.transform.rotation.eulerAngles;
				euler.y = cameraController.centerEyeAnchor.rotation.eulerAngles.y;
				mainCamera.transform.rotation = Quaternion.Euler(euler);
			}
*/		}
	}
}
