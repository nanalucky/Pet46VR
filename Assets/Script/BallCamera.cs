using UnityEngine;
using System.Collections;

public class BallCamera : MonoBehaviour {

	private GameObject goBall;


	// Use this for initialization
	void Start () {
		goBall = GameObject.FindGameObjectWithTag ("Ball");
	}
	
	// Update is called once per frame
	void Update () {
		Camera.main.transform.rotation = Quaternion.LookRotation ((goBall.transform.position - Camera.main.transform.position).normalized);
		Camera.main.transform.position = Camera.main.transform.position;
		if (goBall.GetComponent<Rigidbody>().velocity == Vector3.zero)
		{
			this.enabled = false;
		}
	}
}
