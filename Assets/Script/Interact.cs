using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;
using UnityEngine.UI;

public class Interact : MonoBehaviour {

	private CrosshairHand[] crosshairHand;

	// Use this for initialization
	void Start () {
		var goDog = GameObject.FindGameObjectWithTag ("dog");
		var goCrosshair = goDog.GetComponent<DogController> ().goCrosshair;
		var goCrosshairTouch = goDog.GetComponent<DogController> ().goCrosshairTouch;
		goCrosshair.SetActive (false);
		goCrosshairTouch.SetActive (true);

		GameObject goLookCamera = Instantiate (Resources.Load ("Prefabs/LookCamera")) as GameObject;
		goLookCamera.transform.parent = gameObject.transform;

		GameObject goHead = Instantiate (Resources.Load ("Prefabs/TouchHead")) as GameObject;
		goHead.transform.parent = gameObject.transform;

		GameObject goBack = Instantiate (Resources.Load ("Prefabs/TouchBack")) as GameObject;
		goBack.transform.parent = gameObject.transform;

		GameObject goLeftArm = Instantiate (Resources.Load ("Prefabs/GraspLeftArm")) as GameObject;
		goLeftArm.transform.parent = gameObject.transform;

		GameObject goRightArm = Instantiate (Resources.Load ("Prefabs/GraspRightArm")) as GameObject;
		goRightArm.transform.parent = gameObject.transform;

		GameObject goTail = Instantiate (Resources.Load ("Prefabs/GraspTail")) as GameObject;
		goTail.transform.parent = gameObject.transform;

		GameObject goLeftEar = Instantiate (Resources.Load ("Prefabs/GraspLeftEar")) as GameObject;
		goLeftEar.transform.parent = gameObject.transform;

		GameObject goRightEar = Instantiate (Resources.Load ("Prefabs/GraspRightEar")) as GameObject;
		goRightEar.transform.parent = gameObject.transform;

		GameObject goGesture = Instantiate (Resources.Load ("Prefabs/Gesture")) as GameObject;
		goGesture.transform.parent = gameObject.transform;

		crosshairHand = FindObjectsOfType (typeof(CrosshairHand)) as CrosshairHand[];
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void DisableAllCrosshairHandButThis(CrosshairHand ch)
	{
		foreach (CrosshairHand obj in crosshairHand) 
		{
			if(obj != ch)
				obj.enabled = false;
		}
	}

	public void EnableAllCrosshairHand()
	{
		foreach (CrosshairHand obj in crosshairHand) 
		{
			obj.enabled = true;
		}
	}
}
