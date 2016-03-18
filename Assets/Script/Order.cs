using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Order : MonoBehaviour {

	private Button btn;
	private Word word;

	void Start () {
		GameObject goLookCamera = Instantiate (Resources.Load ("Prefabs/LookCamera")) as GameObject;
		goLookCamera.transform.parent = gameObject.transform;
		
		DogController dogController = GameObject.FindGameObjectWithTag("dog").GetComponent<DogController>();
		dogController.btnPlay.gameObject.SetActive (false);
		dogController.btnOrder.gameObject.SetActive (true);
		btn = dogController.btnOrder;
		word = dogController.word.GetComponent<Word> ();
	}
	
	
	// Update is called once per frame
	void Update () {
		RectTransform rectTransform = (btn.transform) as RectTransform;
		bool overButton = RectTransformUtility.RectangleContainsScreenPoint(rectTransform, new Vector2(Input.mousePosition.x, Input.mousePosition.y), null);
		if (Input.GetMouseButton (0) && overButton) {
			if(!word.IsEnableDetect())
				word.EnableDetect(true);
		}
		else
		{
			if(word.IsEnableDetect())
			{
				word.EnableDetect(false);
				btn.gameObject.SetActive(false);
				btn.gameObject.SetActive(true);
			}
		}

	}
	
	void OnDestroy() {
		DogController dogController = GameObject.FindGameObjectWithTag ("dog").GetComponent<DogController> ();
		if (dogController != null) {
			dogController.btnPlay.gameObject.SetActive(true);
			dogController.btnOrder.gameObject.SetActive (false);
		}
	}
}
