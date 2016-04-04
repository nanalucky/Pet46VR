using UnityEngine;
using System.Collections;
using System;

public class GestureVR : MonoBehaviour {

	public enum State
	{
		SlideDownOnce,
		SlideDownTwice,
		SlideUpOnce,
		SlideUpTwice,
		TapLeftArm,
		TapRightArm,
		None,
	}

	public float twiceInterval = 0.5f;
	private float lastUpTime = 0.0f;
	private float lastDownTime = 0.0f;
	private GameObject goDog;
	private Interact interact;

	void Start()
	{
		OVRTouchpad.TouchHandler += LocalTouchEventCallback;
		goDog = GameObject.FindGameObjectWithTag ("dog");
		interact = FindObjectOfType (typeof(Interact)) as Interact;
	}

	void Update()
	{
		State state = State.None;
		if (lastUpTime != 0.0f && (Time.time - lastUpTime) >= twiceInterval) 
		{
			state = State.SlideUpOnce;
			lastUpTime = 0.0f;
		}
		if (lastDownTime != 0.0f && (Time.time - lastDownTime) >= twiceInterval) 
		{
			state = State.SlideDownOnce;
			lastDownTime = 0.0f;
		}
		ApplyState (state);
	}

	void OnDestroy()
	{
		OVRTouchpad.TouchHandler -= LocalTouchEventCallback;
	}
	
	void LocalTouchEventCallback(object sender, EventArgs args)
	{
		var touchArgs = (OVRTouchpad.TouchArgs)args;
		OVRTouchpad.TouchEvent touchEvent = touchArgs.TouchType;

		State state = State.None;
		switch(touchEvent)
		{
		case OVRTouchpad.TouchEvent.SingleTap:
			break;
			
		case OVRTouchpad.TouchEvent.Left:
			state = State.TapLeftArm;
			break;
			
		case OVRTouchpad.TouchEvent.Right:
			state = State.TapRightArm;
			break;
			
		case OVRTouchpad.TouchEvent.Up:
			if(lastUpTime == 0.0f)
				lastUpTime = Time.time;

			if(lastUpTime != 0.0f && (Time.time - lastUpTime) < twiceInterval)
			{
				lastUpTime = 0.0f;
				state = State.SlideUpTwice;
			}
			break;
			
		case OVRTouchpad.TouchEvent.Down:
			if(lastDownTime == 0.0f)
				lastDownTime = Time.time;
			
			if(lastDownTime != 0.0f && (Time.time - lastDownTime) < twiceInterval)
			{
				lastDownTime = 0.0f;
				state = State.SlideDownTwice;
			}
			break;
		}

		ApplyState (state);				
	}

	void ApplyState(State state)
	{
		if (interact.IsCrosshairInState ())
			return;

		switch (state) 
		{
		case State.SlideDownOnce:
			goDog.GetComponent<DogController>().SitDown ();
			break;
		case State.SlideDownTwice:
			goDog.GetComponent<DogController>().FallDown();
			break;
		case State.SlideUpOnce:
			break;
		case State.SlideUpTwice:
			goDog.GetComponent<DogController>().StandUp();
			break;
		case State.TapLeftArm:
			goDog.GetComponent<DogController>().LeftRawUp();
			break;
		case State.TapRightArm:
			goDog.GetComponent<DogController>().RightRawUp();
			break;
		}
	}
}