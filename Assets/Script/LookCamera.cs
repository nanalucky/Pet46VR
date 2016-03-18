using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class LookCamera : MonoBehaviour {

	private LookAtIK lookatIK;
	private float lookatBodyWeight;
	private float lookatHeadWeight;
	private float lookatEyeWeight;

	private float velBody;
	private float velHead;
	private float velEye;
	public float smooth = 0.2f;

	// Use this for initialization
	void Start () {
		GameObject goDog = GameObject.FindGameObjectWithTag ("dog");
		lookatIK = goDog.GetComponent<LookAtIK> ();
		goDog.GetComponent<DogController> ().EnableLookatIK (true);
		lookatIK.solver.IKPosition = Camera.main.transform.position;

		lookatBodyWeight = lookatIK.solver.bodyWeight;
		lookatHeadWeight = lookatIK.solver.headWeight;
		lookatEyeWeight = lookatIK.solver.eyesWeight;
		lookatIK.solver.bodyWeight = 0;
		lookatIK.solver.headWeight = 0;
		lookatIK.solver.eyesWeight = 0;
		velBody = 0;
		velHead = 0;
		velEye = 0;
	}
	
	// Update is called once per frame
	void Update () {
		lookatIK.solver.IKPosition = Camera.main.transform.position;

		if (lookatIK.solver.bodyWeight != lookatBodyWeight)
			lookatIK.solver.bodyWeight = Mathf.SmoothDamp (lookatIK.solver.bodyWeight, lookatBodyWeight, ref velBody, smooth);
		if (lookatIK.solver.headWeight != lookatHeadWeight)
			lookatIK.solver.headWeight = Mathf.SmoothDamp(lookatIK.solver.headWeight, lookatHeadWeight, ref velHead, smooth);
		if (lookatIK.solver.eyesWeight != lookatEyeWeight)
			lookatIK.solver.eyesWeight = Mathf.SmoothDamp(lookatIK.solver.eyesWeight, lookatEyeWeight, ref velEye, smooth);

	}

	void OnDestroy () {
		if(GameObject.FindGameObjectWithTag ("dog") != null)
			GameObject.FindGameObjectWithTag ("dog").GetComponent<DogController> ().EnableLookatIK (false);
	}
}
