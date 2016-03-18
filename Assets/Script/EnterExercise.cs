using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnterExercise : EnterInteract {
	
	// Use this for initialization
	void Start () {
		// choose the farest lookat
		lookat = GameObject.FindGameObjectWithTag ("dog").GetComponent<DogController> ().lookats [3];
		
		aiMoveCamera = new AIMoveCamera ();
		aiMoveCamera.Start (this);
		lastAI = new AITurn ();
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
			Instantiate(Resources.Load("Prefabs/Exercise"));
			Destroy(gameObject);
			return;
		}
		
		AI ai;
		switch (state) {
		case AIState.Stay:
			ai = new AIStay();
			break;
		case AIState.Run:
			ai = new AIRun();
			break;
		case AIState.Sit:
			ai = new AISit();
			break;
		default:
			ai = new AISit();
			break;
		}
		
		ai.Start(this);
		lastAI = ai;
	}
}
