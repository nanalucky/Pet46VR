using UnityEngine;
using System.Collections;

public class BallDogCamera : MonoBehaviour {

	public float minDistance = 0.8f;
	public float maxDistance = 3.0f;
	public float speedEulerX = 1.0f;

	private GameObject mainCamera;
	private GameObject go;

	private float cameraHeight;
	private Vector3 lastLookat;
	private bool firstFrame = true;

	// Use this for initialization
	void Start () {
		mainCamera = GameObject.Find("OVRCameraRig");
		go = GameObject.FindGameObjectWithTag ("dog");
	}
	
	// Update is called once per frame
	void Update () {
		if (firstFrame) {
			firstFrame = false;

			Plane plane = new Plane( new Vector3(0, 1, 0), go.GetComponent<DogController>().GetDogPivot());
			Vector3 direction = mainCamera.transform.rotation * (new Vector3(0,0,1));
			Ray ray = new Ray(mainCamera.transform.position, direction);
			float rayDist;
			plane.Raycast(ray, out rayDist);
			lastLookat = ray.GetPoint(rayDist);
			
			cameraHeight = mainCamera.transform.position.y - go.transform.position.y;
		}

		bool inMove = false;
		if (lastLookat != go.GetComponent<DogController>().GetDogPivot()) {
			lastLookat = go.GetComponent<DogController>().GetDogPivot();
			inMove = true;
		}

		float distance = (lastLookat - mainCamera.transform.position).magnitude;
		Vector3 euler = Quaternion.LookRotation((lastLookat - mainCamera.transform.position).normalized).eulerAngles;
		if (distance < minDistance || distance > maxDistance) {
			inMove = true;
			distance = minDistance;
			float eulerX = Mathf.Rad2Deg * Mathf.Asin(cameraHeight / distance);
			//euler.x = Mathf.MoveTowardsAngle(mainCamera.transform.rotation.eulerAngles.x, eulerX, speedEulerX * Time.deltaTime);
			//euler.x = eulerX;
			distance = cameraHeight / Mathf.Sin(Mathf.Deg2Rad * euler.x);
		}

		// check collision
		RaycastHit hit;
		Vector3 direction1 = Quaternion.Euler(euler) * (new Vector3(0,0,-1));
		if (Physics.Raycast(go.GetComponent<DogController>().GetDogPivot(), direction1, out hit, distance, 8))
		{
			inMove = true;
			distance = hit.distance;
			float eulerX = Mathf.Rad2Deg * Mathf.Asin(cameraHeight / distance);
			euler.x = eulerX;
			//euler.x = Mathf.MoveTowardsAngle(mainCamera.transform.rotation.eulerAngles.x, eulerX, speedEulerX * Time.deltaTime);
			distance = cameraHeight / Mathf.Sin(Mathf.Deg2Rad * euler.x);
		}

		if (inMove) {
			if(euler.IsValid())
			{
				mainCamera.transform.rotation = Quaternion.Euler(euler);
				mainCamera.transform.position = go.GetComponent<DogController>().GetDogPivot() + mainCamera.transform.rotation * (new Vector3(0, 0, -distance));
			}
		}
	}
}
