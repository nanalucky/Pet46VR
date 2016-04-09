using UnityEngine;
using System.Collections;
using System;

public class SpeechRecognizer : MonoBehaviour {

	public enum State
	{
		SitDown = 0,
		FallDown,
		StandUp,
		LeftArm,
		RightArm,
		Max,
	}

	public string textSitDown;
	public string textFallDown;
	public string textStandUp;
	public string textLeftArm;
	public string textRightArm;
	private string[] texts = new string[(int)(State.Max)];

	private GameObject goDog;
	private Interact interact;
	private SpeechPlugin speechPlugin;	
	private Toast toast;

	void Start()
	{
		toast = FindObjectOfType (typeof(Toast)) as Toast;

		texts [(int)(State.SitDown)] = textSitDown;
		texts [(int)(State.FallDown)] = textFallDown;
		texts [(int)(State.StandUp)] = textStandUp;
		texts [(int)(State.LeftArm)] = textLeftArm;
		texts [(int)(State.RightArm)] = textRightArm;

		goDog = GameObject.FindGameObjectWithTag ("dog");
		interact = FindObjectOfType (typeof(Interact)) as Interact;

		speechPlugin = SpeechPlugin.GetInstance();
		speechPlugin.SetDebug(0);
		speechPlugin.setSpeechEventListener(onReadyForSpeech,onBeginningOfSpeech,onEndOfSpeech,onError,onResults);
	}

	public void StartListening(){
		toast.ShowToastMessage ("StartListening");
		bool isSupported = speechPlugin.CheckSpeechRecognizerSupport();
		
		if(isSupported){
			//number of possible results
			//Note: sometimes even you put 5 numberOfResults, there's a chance that it will be only 3 or 2
			//it is not constant.
			
			int numberOfResults = 5;
			speechPlugin.StartListening(numberOfResults);
			
			//by activating this, the Speech Recognizer will start and you can start Speaking or saying something 
			//speech listener will stop automatically especially when you stop speaking or when you are speaking 
			//for a long time
		}else{
			toast.ShowToastMessage("Speech Recognizer not supported by this Android device");
			Debug.Log("Speech Recognizer not supported by this Android device ");
		}
	}
	
	private void OnDestroy(){
		speechPlugin.RemoveSpeechRecognizerListener();
		speechPlugin.DestroySpeechController();
	}
	
	private void onReadyForSpeech(string data){
		//toast.ShowToastMessage(String.Format("Status: {0}",data.ToString())); 
	}
	
	private void onBeginningOfSpeech(string data){
		//toast.ShowToastMessage(String.Format("Status: {0}",data.ToString())); 
	}
	
	private void onEndOfSpeech(string data){
		//toast.ShowToastMessage(String.Format("Status: {0}",data.ToString())); 
	}
	
	private void onError(string data){
		//toast.ShowToastMessage(String.Format("Status: {0}",data.ToString())); 
		toast.ShowToastMessage(String.Format("Fail"));
	}
	
	private void onResults(string data){
		string[] results =  data.Split(',');
		Debug.Log(" result length " + results.Length);
		if (results.Length == 0) {
			toast.ShowToastMessage (String.Format ("Fail"));
			return;
		}
		//when you set morethan 1 results index zero is always the closest to the words the you said
		//but it's not always the case so if you are not happy with index zero result you can always 
		//check the other index
		
		//sample on checking other results
		int index = 0;
		int validIndex = -1;
		foreach( string possibleResults in results )
		{
			Debug.Log( " possibleResults " + possibleResults );
			index = 0;
			foreach(string commander in texts)
			{
				if(String.Compare(possibleResults.ToString(), commander.ToString()) == 0)
				{
					validIndex = index;
					break;
				}
				index++;
			}
			if(validIndex != -1)
				break;
		}
		if (validIndex != -1)
			toast.ShowToastMessage (texts [validIndex]);
		else 
			toast.ShowToastMessage (results [0]);
		//toast.ShowToastMessage (String.Format ("validindex:{0}, result:{1}", validIndex, data.ToString()));

		if(validIndex != -1)
		{
			ApplyState((State)validIndex);
		}	

	}

	void ApplyState(State state)
	{
		if (interact && interact.IsCrosshairInState ())
			return;

		switch (state) 
		{
		case State.SitDown:
			goDog.GetComponent<DogController>().SitDown ();
			break;
		case State.FallDown:
			goDog.GetComponent<DogController>().FallDown();
			break;
		case State.StandUp:
			goDog.GetComponent<DogController>().StandUp();
			break;
		case State.LeftArm:
			goDog.GetComponent<DogController>().LeftRawUp();
			break;
		case State.RightArm:
			goDog.GetComponent<DogController>().RightRawUp();
			break;
		}
	}
}