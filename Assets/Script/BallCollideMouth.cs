using UnityEngine;
using System.Collections;

public class BallCollideMouth : MonoBehaviour {

	private GameObject goBall;

	// Use this for initialization
	void Start () {
		goBall = GameObject.FindGameObjectWithTag ("Ball");
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject == goBall)
			Destroy(this);
	}
}
