using UnityEngine;
using System.Collections;
using System;

public class SpeechCallback :  AndroidJavaProxy {	

	public Action <string>onReadyForSpeech;
	public Action <string>onBeginningOfSpeech;
	public Action <string>onEndOfSpeech;
	public Action <string>onError;
	public Action <string>onResults;
	
	public SpeechCallback() : base("com.gigadrillgames.androidplugin.speech.ISpeech") {}

	void ReadyForSpeech(String val){
		onReadyForSpeech(val);
	}

	void BeginningOfSpeech(String val){
		onBeginningOfSpeech(val);
	}


	void EndOfSpeech(String val){
		onEndOfSpeech(val);
	}

	void Error(String val){
		onError(val);
	}

	void Results(String val){
		onResults(val);
	}
}
