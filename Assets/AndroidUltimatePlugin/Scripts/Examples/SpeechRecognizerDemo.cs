using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class SpeechRecognizerDemo : MonoBehaviour {

	private SpeechPlugin speechPlugin;	
	private bool hasInit = false;
	public Text resultText;
	public Text statusText;

	private TextToSpeechPlugin textToSpeechPlugin;

	// Use this for initialization
	void Start (){		
		speechPlugin = SpeechPlugin.GetInstance();
		speechPlugin.SetDebug(0);
		speechPlugin.setSpeechEventListener(onReadyForSpeech,onBeginningOfSpeech,onEndOfSpeech,onError,onResults);

		textToSpeechPlugin = TextToSpeechPlugin.GetInstance();
		textToSpeechPlugin.SetDebug(0);
		textToSpeechPlugin.Init();
		textToSpeechPlugin.SetTextToSpeechCallbackListener(OnInit,OnGetLocaleCountry,OnSetLocale,OnStartSpeech,OnDoneSpeech,OnErrorSpeech);
	}

	private void OnApplicationPause(bool val){
		//for text to speech events
		if(textToSpeechPlugin!=null){
			if(hasInit){
				if(val){
					textToSpeechPlugin.UnRegisterBroadcastEvent();
				}else{
					textToSpeechPlugin.RegisterBroadcastEvent();
				}
			}
		}
	}

	public void StartListening(){
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
			Debug.Log("Speech Recognizer not supported by this Android device ");
		}
	}

	private void OnDestroy(){
		speechPlugin.RemoveSpeechRecognizerListener();
		speechPlugin.DestroySpeechController();

		//call this of your not going to used TextToSpeech Service anymore
		textToSpeechPlugin.ShutDownTextToSpeechService();
	}

	private void onReadyForSpeech(string data){
		if(statusText!=null){
			statusText.text =  String.Format("Status: {0}",data.ToString()); 
		}
	}

	private void onBeginningOfSpeech(string data){
		if(statusText!=null){
			statusText.text =  String.Format("Status: {0}",data.ToString()); 
		}
	}

	private void onEndOfSpeech(string data){
		if(statusText!=null){
			statusText.text =  String.Format("Status: {0}",data.ToString()); 
		}
	}

	private void onError(string data){
		if(statusText!=null){
			statusText.text =  String.Format("Status: {0}",data.ToString()); 
		}

		if(resultText!=null){
			resultText.text =  "Result:"; 
		}
	}

	private void onResults(string data){
		if(resultText!=null){
			string[] results =  data.Split(',');
			Debug.Log(" result length " + results.Length);
			
			//when you set morethan 1 results index zero is always the closest to the words the you said
			//but it's not always the case so if you are not happy with index zero result you can always 
			//check the other index
			
			//sample on checking other results
			foreach( string possibleResults in results ){
				Debug.Log( " possibleResults " + possibleResults );
			}
			
			//sample showing the nearest result
			string whatToSay  = results.GetValue(0).ToString();
			string utteranceId  = "test-utteranceId";
			resultText.text =  string.Format("Result: {0}",whatToSay); 
			
			//check if Text to speech has initialized
			if(hasInit){
				//Text To Speech Sample Usage
				textToSpeechPlugin.SpeakOut(whatToSay,utteranceId);
			}
		}
	}

	private void OnInit(int status){
		Debug.Log("[SpeechRecognizerDemo] OnInit status: " + status);
		
		if(status == 1){
			hasInit = true;
			textToSpeechPlugin.SetLocale(SpeechLocale.US);
		}
	}

	private void OnGetLocaleCountry(string localeCountry){
		Debug.Log("[SpeechRecognizerDemo] OnGetLocaleCountry localeCountry: " + localeCountry);
	}
	
	private void OnSetLocale(int status){
		Debug.Log("[SpeechRecognizerDemo] OnSetLocale status: " + status);
		if(status == 1){
			textToSpeechPlugin.SetPitch(1f);
		}
	}
	
	private void OnStartSpeech(string utteranceId){
		Debug.Log("[SpeechRecognizerDemo] OnStartSpeech utteranceId: " + utteranceId);
	}
	
	private void OnDoneSpeech(string utteranceId){
		Debug.Log("[SpeechRecognizerDemo] OnDoneSpeech utteranceId: " + utteranceId);
	}
	
	private void OnErrorSpeech(string utteranceId){
		Debug.Log("[SpeechRecognizerDemo] OnErrorSpeech utteranceId: " + utteranceId);
	}
}
