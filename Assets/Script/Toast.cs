using UnityEngine;
using System.Collections;

public class Toast : MonoBehaviour {

	public TextMesh text;
	public float timeInterval;
	private float timeStart;

	// Use this for initialization
	void Start () {
		text.text = "";
		timeStart = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (timeStart != 0.0f && (Time.time - timeStart) >= timeInterval) {
			timeStart = 0.0f;
			text.text = "";
		}
	}

	public void ShowToastMessage(string str)
	{
		text.text = str;
		timeStart = Time.time;
	}
}
