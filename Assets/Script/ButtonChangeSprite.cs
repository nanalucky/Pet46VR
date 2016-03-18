using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonChangeSprite : MonoBehaviour {

	public string label;
	public Sprite saved;
	private bool bSaved = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (GameObject.FindGameObjectWithTag ("Word")) {
			Word word = GameObject.FindGameObjectWithTag ("Word").GetComponent<Word> ();
			if (word.HasProfile (label) != bSaved) {
				gameObject.GetComponent<Button>().image.sprite = saved;
				bSaved = !bSaved;
			}
		}
	}
}
