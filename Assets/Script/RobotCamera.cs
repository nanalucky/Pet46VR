using UnityEngine;
using System.Collections;

public class RobotCamera : MonoBehaviour {

	public float eulerX = 15.0f;
	public float eulerXSpeed = 50.0f;
	public float[] distances = new float[]{1.0f, 2.0f};
	public float distanceSpeed = 50.0f;
	public float lookatSpeed = 50.0f;
	public float timeDelta = 60.0f;

	private GameObject go;
	private GameObject mainCamera;
	private Vector3 curLookat;
	private float curDistance;
	private int curDistanceIndex;
	private float nextTime;


	// Use this for initialization
	void Start()
	{
		go = GameObject.FindGameObjectWithTag ("dog");
		mainCamera = GameObject.Find("OVRCameraRig");

		Plane plane = new Plane( new Vector3(0, 1, 0), go.GetComponent<DogController>().GetDogPivot());
		Vector3 direction = mainCamera.transform.rotation * (new Vector3(0,0,1));
		Ray ray = new Ray(mainCamera.transform.position, direction);
		float rayDist;
		plane.Raycast(ray, out rayDist);
		curLookat = ray.GetPoint(rayDist);
		curDistance = (curLookat - mainCamera.transform.position).magnitude;

		curDistanceIndex = 0;
		nextTime = Time.time + timeDelta;
	}

	// Update is called once per frame
	void Update () {
		if (Time.time >= nextTime) {
			curDistanceIndex = 1 - curDistanceIndex;
			nextTime = Time.time + timeDelta;
		}

		Vector3 dogPivot = go.GetComponent<DogController> ().GetDogPivot ();

		Vector3 lookatDirection = dogPivot - curLookat;
		float lookatDistance = lookatDirection.magnitude;
		float curLookatDistance = Mathf.MoveTowards (0.0f, lookatDistance, lookatSpeed * Time.deltaTime);
		curLookat = curLookat + lookatDirection.normalized * curLookatDistance;

		curDistance = Mathf.MoveTowards (curDistance, distances [curDistanceIndex], distanceSpeed * Time.deltaTime);

		Vector3 euler = mainCamera.transform.rotation.eulerAngles;
		euler.x = Mathf.MoveTowardsAngle (euler.x, eulerX, eulerXSpeed * Time.deltaTime);

		mainCamera.transform.rotation = Quaternion.Euler (euler);
		mainCamera.transform.position = curLookat + mainCamera.transform.rotation * (new Vector3 (0, 0, -curDistance));
	}
}
