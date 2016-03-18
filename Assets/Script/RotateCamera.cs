using UnityEngine;
using System.Collections;

public class RotateCamera : MonoBehaviour {

	public float MouseSensitivity = 2f;   
	
	private GameObject go;
	private float mouseX = 0f;
	private float mouseY = 0f;

	// Use this for initialization
	void Start () {
		go = GameObject.FindGameObjectWithTag ("dog");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton(1))
		{
			Plane plane = new Plane( new Vector3(0, 1, 0), go.GetComponent<DogController>().GetDogPivot());
			Vector3 direction = Camera.main.transform.rotation * (new Vector3(0,0,1));
			Ray ray = new Ray(Camera.main.transform.position, direction);
			float rayDist;
			plane.Raycast(ray, out rayDist);
			Vector3 lookat = ray.GetPoint(rayDist);

			mouseX = Camera.main.transform.rotation.eulerAngles.x;
			mouseY = Camera.main.transform.rotation.eulerAngles.y;	

			//mouseX -= Input.GetAxis("Mouse Y") * MouseSensitivity;
			mouseY += Input.GetAxis("Mouse X") * MouseSensitivity;
			
			Camera.main.transform.rotation = Quaternion.Euler(mouseX, mouseY, Camera.main.transform.rotation.eulerAngles.z);
			Camera.main.transform.position = lookat + Camera.main.transform.rotation * (new Vector3(0, 0, -rayDist));
		}	
	}
}
