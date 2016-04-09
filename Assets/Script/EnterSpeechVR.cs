using UnityEngine;
using System.Collections;

public class EnterSpeechVR: EnterInteractVR {

	// Use this for initialization
	void Start () {
		aiMoveCamera = new AIMoveCamera ();
		aiMoveCamera.Start (this);
		lastAI = new AIStandUp ();
		lastAI.Start (this);
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
			(FindObjectOfType (typeof(NoTouchGUI)) as NoTouchGUI).ShowSpeechRecognizer (true);
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
