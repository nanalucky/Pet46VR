using UnityEngine;
using System.Collections;

public class TestEvent : MonoBehaviour {

	private EventManager eventManager;

	// Use this for initialization
	void Start (){
		eventManager = EventManager.GetInstance();
		eventManager.Init();

		eventManager.StartListening ("test", someListener);
		eventManager.TriggerEvent("test");
	}
	
	// Update is called once per frame
	void someListener () {
		Debug.Log("test event!");
	}
}
