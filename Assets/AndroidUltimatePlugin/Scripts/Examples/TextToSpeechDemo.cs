using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class TextToSpeechDemo : MonoBehaviour {

	public InputField inputField;

	public Text statusText;
	public Text ttsDataActivityStatusText;
	public Text localeText;
	public Slider localeSlider;

	public Text pitchText;
	public Slider pitchSlider;

	public Text speechRateText;
	public Slider speechRateSlider;


	private SpeechPlugin speechPlugin;
	private bool hasInit = false;
	private TextToSpeechPlugin textToSpeechPlugin;

	private float waitingInterval = 2f;

	private string[] localeCountryISO2AlphaSet = null;

	// Use this for initialization
	void Start (){		
		speechPlugin = SpeechPlugin.GetInstance();
		speechPlugin.SetDebug(0);


		textToSpeechPlugin = TextToSpeechPlugin.GetInstance();
		textToSpeechPlugin.SetDebug(0);
		textToSpeechPlugin.LoadLocaleCountry();
		textToSpeechPlugin.LoadCountryISO2AlphaCountryName();
		textToSpeechPlugin.Init();
		textToSpeechPlugin.SetTextToSpeechCallbackListener(OnInit,OnGetLocaleCountry,OnSetLocale,OnStartSpeech,OnDoneSpeech,OnErrorSpeech);

		CheckTTSDataActivity();
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

	private void WaitingMode(){
		UpdateStatus("Waiting...");
	}

	private void UpdateStatus(string status){
		if(statusText!=null){
			statusText.text = String.Format("Status: {0}",status);	
		}
	}

	private void UpdateTTSDataActivityStatus(string status){
		if(ttsDataActivityStatusText!=null){
			ttsDataActivityStatusText.text = String.Format("TTS Data Activity Status: {0}",status);
		}
	}

	private void UpdateLocale(SpeechLocale locale){
		if(localeText!=null){
			localeText.text = String.Format("Locale: {0}",locale);
			textToSpeechPlugin.SetLocale(locale);
		}
	}

	private void UpdatePitch(float pitch){
		if(pitchText!=null){
			pitchText.text = String.Format("Pitch: {0}",pitch);
			textToSpeechPlugin.SetPitch(pitch);
		}
	}

	private void UpdateSpeechRate(float speechRate){
		if(speechRateText!=null){
			speechRateText.text = String.Format("Speech Rate: {0}",speechRateSlider.value);
			textToSpeechPlugin.SetSpeechRate(speechRate);
		}
	}

	public void SpeakOut(){
		if(inputField!=null){
			string whatToSay = inputField.text;
			string utteranceId  = "test-utteranceId";

			if(hasInit){
				UpdateStatus("Trying to speak...");
				Debug.Log("[TextToSpeechDemo] SpeakOut whatToSay: " + whatToSay  + " utteranceId " + utteranceId);
				textToSpeechPlugin.SpeakOut(whatToSay,utteranceId);	
			}
		}
	}

	//checks if speaking
	public bool IsSpeaking(){
		return textToSpeechPlugin.IsSpeaking();
	}

	private void CheckTTSDataActivity(){
		if(textToSpeechPlugin!=null){
			if(textToSpeechPlugin.CheckTTSDataActivity()){
				UpdateTTSDataActivityStatus("Available");
			}else{
				UpdateTTSDataActivityStatus("Not Available");
			}
		}
	}

	public void SpeakUsingAvailableLocaleOnDevice(){

		//on this example we will use spain locale
		TTSLocaleCountry ttsLocaleCountry = TTSLocaleCountry.SPAIN;
		
		//check if available
		bool isLanguageAvailanble =  CheckLocale(ttsLocaleCountry);
		
		if(isLanguageAvailanble){
			string countryISO2Alpha = textToSpeechPlugin.GetCountryISO2Alpha(ttsLocaleCountry);
			
			//set spain language
			textToSpeechPlugin.SetLocaleByCountry(countryISO2Alpha);
			Debug.Log("[TextToSpeechDemo] locale set," + ttsLocaleCountry.ToString() + "locale is available");

			SpeakOut();
		}else{
			Debug.Log("[TextToSpeechDemo] locale not set," + ttsLocaleCountry.ToString() + "locale is  notavailable");
		}
	}

	private void OnDestroy(){
		//call this of your not going to used TextToSpeech Service anymore
		textToSpeechPlugin.ShutDownTextToSpeechService();
	}

	public void OnLocaleSliderChange(){
		Debug.Log("[TextToSpeechDemo] OnLocaleSliderChange");
		if(localeSlider!=null){
			SpeechLocale locale = (SpeechLocale)localeSlider.value;
			UpdateLocale(locale);
		}
	}

	public void OnPitchSliderChange(){
		Debug.Log("[TextToSpeechDemo] OnPitchSliderChange");
		if(pitchSlider!=null){
			float pitch = pitchSlider.value;
			UpdatePitch(pitch);
		}
	}

	public void OnSpeechRateSliderChange(){
		Debug.Log("[TextToSpeechDemo] OnSpeechRateSliderChange");
		if(speechRateSlider!=null){
			float speechRate = speechRateSlider.value;
			UpdateSpeechRate(speechRate);
		}
	}

	private void OnInit(int status){
		Debug.Log("[TextToSpeechDemo] OnInit status: " + status);

		if(status == 1){
			UpdateStatus("init speech service successful!");
			hasInit = true;

			//get available locale on android device
			textToSpeechPlugin.GetAvailableLocale();

			UpdateLocale(SpeechLocale.US);
			UpdatePitch(1f);
			UpdateSpeechRate(1f);

			CancelInvoke("WaitingMode");
			Invoke("WaitingMode",waitingInterval);
		}else{
			UpdateStatus("init speech service failed!");

			CancelInvoke("WaitingMode");
			Invoke("WaitingMode",waitingInterval);
		}
	}

	private void OnGetLocaleCountry(string localeCountry){
		Debug.Log("[TextToSpeechDemo] OnGetLocaleCountry localeCountry: " + localeCountry);
		localeCountryISO2AlphaSet = localeCountry.Split(',');
	}

	private bool CheckLocale(TTSLocaleCountry ttsCountry){
		bool found = false;
		if(localeCountryISO2AlphaSet!=null){
			string countryISO2Alpha = textToSpeechPlugin.GetCountryISO2Alpha(ttsCountry);
			
			foreach(string country in localeCountryISO2AlphaSet){
				if(country.Equals(countryISO2Alpha,StringComparison.Ordinal)){
					found = true;
					break;
				}
				//Debug.Log("get country: " + country);
			}
		}

		return found;
	}
	
	private void OnSetLocale(int status){
		Debug.Log("[TextToSpeechDemo] OnSetLocale status: " + status);
		if(status == 1){
			//float pitch = Random.Range(0.1f,2f);
			//textToSpeechPlugin.SetPitch(pitch);
		}
	}
	
	private void OnStartSpeech(string utteranceId){
		UpdateStatus("Start Speech...");
		Debug.Log("[TextToSpeechDemo] OnStartSpeech utteranceId: " + utteranceId);

		if(IsSpeaking()){
			UpdateStatus("speaking...");
		}
	}
	
	private void OnDoneSpeech(string utteranceId){
		UpdateStatus("Done Speech...");
		Debug.Log("[TextToSpeechDemo] OnDoneSpeech utteranceId: " + utteranceId);

		CancelInvoke("WaitingMode");
		Invoke("WaitingMode",waitingInterval);
	}
	
	private void OnErrorSpeech(string utteranceId){
		UpdateStatus("Error Speech...");

		CancelInvoke("WaitingMode");
		Invoke("WaitingMode",waitingInterval);

		Debug.Log("[TextToSpeechDemo] OnErrorSpeech utteranceId: " + utteranceId);
	}
}
