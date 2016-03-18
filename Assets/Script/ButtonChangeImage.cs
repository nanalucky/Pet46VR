using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonChangeImage : MonoBehaviour {

	public Sprite normal;
	public Sprite pressed;
	private Button btn;

	// Use this for initialization
	void Start () {
		btn = gameObject.GetComponent<Button> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!btn.interactable)
			return;

		RectTransform rectTransform = (btn.transform) as RectTransform;
		bool overButton = RectTransformUtility.RectangleContainsScreenPoint(rectTransform, new Vector2(Input.mousePosition.x, Input.mousePosition.y), null);
		if (Input.GetMouseButton (0) && overButton) {
			btn.image.sprite = pressed;
		} else {
			btn.image.sprite = normal;
		}
	}
}
