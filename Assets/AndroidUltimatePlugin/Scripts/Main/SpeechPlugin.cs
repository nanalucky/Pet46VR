using UnityEngine;
using System.Collections;
using System;

public class SpeechPlugin : MonoBehaviour {

	private static SpeechPlugin instance;
	private static GameObject container;
	private const string TAG="[SpeechPlugin]: ";
	private static AUPHolder aupHolder;

	#if UNITY_ANDROID
	private static AndroidJavaObject jo;
	#endif	
	
	public bool isDebug =true;
	
	public static SpeechPlugin GetInstance(){
		if(instance==null){
			container = new GameObject();
			container.name="SpeechPlugin";
			instance = container.AddComponent( typeof(SpeechPlugin) ) as SpeechPlugin;
			DontDestroyOnLoad(instance.gameObject);
			aupHolder = AUPHolder.GetInstance();
			instance.gameObject.transform.SetParent(aupHolder.gameObject.transform);
		}
		
		return instance;
	}
	
	private void Awake(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo = new AndroidJavaObject("com.gigadrillgames.androidplugin.speech.SpeechPlugin");
		}
		#endif
	}
	
	/// <summary>
	/// Sets the debug.
	/// 0 - false, 1 - true
	/// </summary>
	/// <param name="debug">Debug.</param>
	public void SetDebug(int debug){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("SetDebug",debug);
			AUP.Utils.Message(TAG,"SetDebug");
		}else{
			AUP.Utils.Message(TAG,"warning: must run in actual android device");
		}
		#endif
	}

	public bool CheckSpeechRecognizerSupport(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			return jo.CallStatic<bool>("checkSpeechRecognizer");
		}else{
			AUP.Utils.Message(TAG,"warning: must run in actual android device");
		}
		#endif
		
		return false;
	}
	
	public void setSpeechEventListener(
		Action <string>onReadyForSpeech
		,Action <string>onBeginningOfSpeech
		,Action <string>onEndOfSpeech
		,Action <string>onError
		,Action <string>onResults
		){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			SpeechCallback speechCallback = new SpeechCallback();
			speechCallback.onReadyForSpeech = onReadyForSpeech;
			speechCallback.onBeginningOfSpeech = onBeginningOfSpeech;
			speechCallback.onEndOfSpeech = onEndOfSpeech;
			speechCallback.onError = onError;
			speechCallback.onResults = onResults;
			
			
			jo.CallStatic("setSpeechEventListener",speechCallback);
		}else{
			AUP.Utils.Message(TAG,"warning: must run in actual android device");
		}
		#endif
	}
	
	/// <summary>
	/// Activate the listener for your speech or voice
	/// it will now detect what words you said
	/// although it's not always correct but it is alway nearest
	/// to the words that you said
	/// </summary>
	/// <param name="numberOfResults">Number of results.</param>
	public void StartListening(int numberOfResults){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("startListening",numberOfResults);
		}else{
			AUP.Utils.Message(TAG,"warning: must run in actual android device");
		}
		#endif
	}
	
	/// <summary>
	/// Removes Speech Recognizer listener.
	/// </summary>
	public void RemoveSpeechRecognizerListener(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("removeSpeechRecognizerListener");
		}else{
			AUP.Utils.Message(TAG,"warning: must run in actual android device");
		}
		#endif
	}
	
	/// <summary>
	/// Destroys the speech Recognizer controller
	/// Note: Call this when you are done using it.
	/// </summary>
	public void DestroySpeechController(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("destroySpeechController");
		}else{
			AUP.Utils.Message(TAG,"warning: must run in actual android device");
		}
		#endif
	}
}