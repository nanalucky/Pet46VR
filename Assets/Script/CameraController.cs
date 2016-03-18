using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {

	public Canvas canvas;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("empty"))
		{
			canvas.gameObject.SetActive(true);
			GetComponent<Animator>().enabled = false;
			this.enabled = false;
		}
	}
}
