using UnityEngine;
using System.Collections;

public class EnterSpeechVR: EnterInteractVR {

	// Use this for initialization
	void Start () {
		aiMoveCamera = new AIMoveCamera ();
		aiMoveCamera.Start (this);
		lastAI = new AIStandUp ();
		lastAI.Start (this);

		GameObject mainCamera = GameObject.Find("OVRCameraRig");
		dogDestPosition = mainCamera.transform.position + mainCamera.transform.TransformDirection(Vector3.forward) * dogDestDistance;
		dogDestPosition.y = GameObject.FindGameObjectWithTag ("dog").transform.position.y;
		dogDestEuler = Quaternion.LookRotation (mainCamera.transform.TransformDirection (Vector3.back)).eulerAngles;
	}
	
	// Update is called once per frame
	void Update () {
		aiMoveCamera.Update ();
		lastAI.Update ();
		if (lastAI.IsFinished ()) 
		{
			ChangeAI();
		}
	}
	
	void ChangeAI()
	{
		AIState state = lastAI.GetNextState ();
		lastAI.Finish ();
		
		if (state == AIState.None)
		{
			//(FindObjectOfType (typeof(NoTouchGUI)) as NoTouchGUI).ShowSpeechRecognizer (true);
			Destroy(gameObject);
			return;
		}
		
		AI ai;
		switch (state) {
		case AIState.Turn:
			ai = new AITurn();
			break;
		case AIState.Stay:
			ai = new AIStay();
			break;
		case AIState.Run:
			ai = new AIRun();
			break;
		case AIState.Turn2:
			ai = new AITurn2();
			break;
		case AIState.Stay2:
			ai = new AIStay2();
			break;
		case AIState.Sit:
			ai = new AISit();
			break;
		case AIState.Idle3:
			ai = new AIIdle3();
			break;
		default:
			ai = new AISit();
			break;
		}
		
		ai.Start(this);
		lastAI = ai;
	}

}
