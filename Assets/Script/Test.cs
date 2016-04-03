using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (GameObject.Find ("Crosshair") != null) 
		{
			Debug.LogWarning ("********find crosshair*************");
		}
		else
		{
			Debug.LogWarning ("********not find crosshair*************");
		}

		BoxCollider[] boxes = FindObjectsOfType(typeof(BoxCollider)) as BoxCollider[];
		foreach(BoxCollider obj in boxes)
		{
			Debug.LogWarning(obj.gameObject.name);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
