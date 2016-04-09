using UnityEngine;
using System.Collections;

public class Speech : MonoBehaviour {

	// Use this for initialization
	void Start () {
		(FindObjectOfType (typeof(NoTouchGUI)) as NoTouchGUI).ShowSpeechRecognizer (true);
	}
	
	// Update is called once per frame
	void Update () {
	}
	
}
