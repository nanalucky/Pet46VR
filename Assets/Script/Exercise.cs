using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Exercise : MonoBehaviour {

	void Start () {
		GameObject goLookCamera = Instantiate (Resources.Load ("Prefabs/LookCamera")) as GameObject;
		goLookCamera.transform.parent = gameObject.transform;
		
		DogController dogController = GameObject.FindGameObjectWithTag("dog").GetComponent<DogController>();
		foreach (Button btn in dogController.btnRecords) {
			btn.gameObject.SetActive (true);
		}
	}

	void OnDestroy() {
		if(GameObject.FindGameObjectWithTag ("dog") != null)
		{
			DogController dogController = GameObject.FindGameObjectWithTag ("dog").GetComponent<DogController> ();
			if (dogController != null) {
				foreach (Button btn in dogController.btnRecords) {
					btn.gameObject.SetActive(false);
				}		
			}
		}
	}
}
