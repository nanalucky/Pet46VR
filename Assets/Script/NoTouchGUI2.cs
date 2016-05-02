using UnityEngine;
using System.Collections;
using System;


public class NoTouchGUI2 : MonoBehaviour {

	public enum State
	{
		Robot = 0,
		Interact,
		PlayBall,
		ThrowBall,
		Speech,
		SitDown,
		FallDown,
		StandUp,
		LeftArm,
		RightArm,
		Max,
	}
	
	public OVRCameraRig cameraController = null;
	public GameObject hintSpeechRecognizer2;
	public GameObject hintSpeechRecognizerBall2;
	public GameObject hintSpeechRecognizerSpeech2;
	public string textRobot;
	public string textInteract;
	public string textPlayBall;
	public string textThrowBall;
	public string textSpeech;
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
	private State curState;

	
	void Start()
	{
		// refresh any children
		BroadcastMessage("OnRefresh", SendMessageOptions.DontRequireReceiver);

		toast = FindObjectOfType (typeof(Toast)) as Toast;

		texts [(int)(State.Robot)] = textRobot;
		texts [(int)(State.Interact)] = textInteract;
		texts [(int)(State.PlayBall)] = textPlayBall;
		texts [(int)(State.ThrowBall)] = textThrowBall;
		texts [(int)(State.Speech)] = textSpeech;
		texts [(int)(State.SitDown)] = textSitDown;
		texts [(int)(State.FallDown)] = textFallDown;
		texts [(int)(State.StandUp)] = textStandUp;
		texts [(int)(State.LeftArm)] = textLeftArm;
		texts [(int)(State.RightArm)] = textRightArm;
		
		goDog = GameObject.FindGameObjectWithTag ("dog");

		speechPlugin = SpeechPlugin.GetInstance();
		speechPlugin.SetDebug(0);
		speechPlugin.setSpeechEventListener(onReadyForSpeech,onBeginningOfSpeech,onEndOfSpeech,onError,onResults);
	}

	void Update()
	{
		transform.position = cameraController.transform.position;
		transform.rotation = cameraController.transform.rotation;
	}
	
	public void StartListening(){
		toast.ShowToastMessage ("请说出命令...");
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

			hintSpeechRecognizer2.SetActive (true);
			if(curState == State.PlayBall)
			{
				ThrowBallVR throwBall = FindObjectOfType (typeof(ThrowBallVR)) as ThrowBallVR;
				if(throwBall != null)
					hintSpeechRecognizerBall2.SetActive(true);
			}
			if(curState == State.Speech)
			{
				if(GameObject.Find("EnterSpeech") == null)
					hintSpeechRecognizerSpeech2.SetActive(true);
			}

		}else{
			toast.ShowToastMessage("本手机不支持语音识别");
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
		toast.ShowToastMessage(String.Format("识别失败，请再来一次"));

		hintSpeechRecognizer2.SetActive (false);
		hintSpeechRecognizerBall2.SetActive (false);
		hintSpeechRecognizerSpeech2.SetActive (false);
	}
	
	private void onResults(string data){
		hintSpeechRecognizer2.SetActive (false);
		hintSpeechRecognizerBall2.SetActive (false);
		hintSpeechRecognizerSpeech2.SetActive (false);

		string[] results =  data.Split(',');
		Debug.Log(" result length " + results.Length);
		if (results.Length == 0) {
			toast.ShowToastMessage (String.Format ("识别失败，请再来一次"));
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
		switch (state) 
		{
		case State.Robot:
			curState = state;
			goDog.GetComponent<DogController>().ToRobot();
			break;
		case State.Interact:
			curState = state;
			goDog.GetComponent<DogController>().ToInteract();
			break;
		case State.PlayBall:
			curState = state;
			goDog.GetComponent<DogController>().ToBall();
			break;
		case State.ThrowBall:
			if(curState == State.PlayBall)
			{
				ThrowBallVR throwBall = FindObjectOfType (typeof(ThrowBallVR)) as ThrowBallVR;
				if(throwBall != null)
					throwBall.ThrowBall();
			}
			break;
		case State.Speech:
			curState = state;
			goDog.GetComponent<DogController>().ToSpeech();
			break;
		case State.SitDown:
			if(curState == State.Speech)
			{
				if(GameObject.Find("EnterSpeech") == null)
					goDog.GetComponent<DogController>().SitDown ();
			}
			break;
		case State.FallDown:
			if(curState == State.Speech)
			{
				if(GameObject.Find("EnterSpeech") == null)
					goDog.GetComponent<DogController>().FallDown ();
			}
			break;
		case State.StandUp:
			if(curState == State.Speech)
			{
				if(GameObject.Find("EnterSpeech") == null)
					goDog.GetComponent<DogController>().StandUp ();
			}
			break;
		case State.LeftArm:
			if(curState == State.Speech)
			{
				if(GameObject.Find("EnterSpeech") == null)
					goDog.GetComponent<DogController>().LeftRawUp ();
			}
			break;
		case State.RightArm:
			if(curState == State.Speech)
			{
				if(GameObject.Find("EnterSpeech") == null)
					goDog.GetComponent<DogController>().RightRawUp ();
			}
			break;
		}
	}
}
