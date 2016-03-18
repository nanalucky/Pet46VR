using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonStartRecord : MonoBehaviour {

	public string label;
	private Button btn;
	private bool lastBtnDown = false;
	private WordRecord wordRecord;

	// Use this for initialization
	void Start () {
		btn = gameObject.GetComponent<Button> ();
		wordRecord = GameObject.FindGameObjectWithTag ("dog").GetComponent<DogController> ().word.GetComponent<WordRecord> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (string.IsNullOrEmpty (label))
			return;

		RectTransform rectTransform = (btn.transform) as RectTransform;
		bool overButton = RectTransformUtility.RectangleContainsScreenPoint(rectTransform, new Vector2(Input.mousePosition.x, Input.mousePosition.y), null);
		bool btnDown = Input.GetMouseButton (0) && overButton;
		if (!lastBtnDown && btnDown) 
		{
			if(!wordRecord.IsRecording())
			{
				wordRecord.StartRecord(btn, label);
			}
		}
		lastBtnDown = btnDown;
	}
}
