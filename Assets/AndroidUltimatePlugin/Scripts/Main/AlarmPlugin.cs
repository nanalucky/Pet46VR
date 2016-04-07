using UnityEngine;
using System.Collections;
using System;
using AUP;

public class AlarmPlugin : MonoBehaviour {
	
	private static AlarmPlugin instance;
	private static GameObject container;
	private const string TAG="[AlarmPlugin]: ";

	private static AUPHolder aupHolder;
	
	#if UNITY_ANDROID
	private static AndroidJavaObject jo;
	#endif	
	
	public bool isDebug =true;
	
	public static AlarmPlugin GetInstance(){
		if(instance==null){
			aupHolder = AUPHolder.GetInstance();

			container = new GameObject();
			container.name="AlarmPlugin";
			instance = container.AddComponent( typeof(AlarmPlugin) ) as AlarmPlugin;
			DontDestroyOnLoad(instance.gameObject);
			instance.gameObject.transform.SetParent(aupHolder.gameObject.transform);
		}
		
		return instance;
	}
	
	private void Awake(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo = new AndroidJavaObject("com.gigadrillgames.androidplugin.alarm.AlarmPlugin");
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
	

	public void Init(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("init");
		}else{
			AUP.Utils.Message(TAG,"warning: must run in actual android device");
		}
		#endif
	}

	/// <summary>
	/// Sets one time Alarm. ex. 2:30PM = 14 Hours and 30 Minutes
	/// </summary>
	/// <param name="hour">Hour range: 1 to 24 </param> 
	/// <param name="minute">Minute range 1 to 59</param>
	/// <param name="alarmTile">alarm title</param>
	/// <param name="alarmMessage">alarm message</param>
	/// <param name="alarmTickerMessage">alarm ticker message</param>

	public void SetOneTimeAlarm(int hour, int minute,string alarmTile,string alarmMessage,string alarmTickerMessage,int requestCode){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("setOneTimeAlarm",hour,minute,alarmTile,alarmMessage,alarmTickerMessage,requestCode);
		}else{
			AUP.Utils.Message(TAG,"warning: must run in actual android device");
		}
		#endif
	}

	/// <summary>
	/// Repeats the alarm once per day
	/// </summary>
	/// <param name="hour">Hour.</param>
	/// <param name="minute">Minute.</param>
	/// <param name="alarmTile">Alarm tile.</param>
	/// <param name="alarmMessage">Alarm message.</param>
	/// <param name="alarmTickerMessage">Alarm ticker message.</param>
	public void SetRepeatingAlarm(int hour, int minute,string alarmTile,string alarmMessage,string alarmTickerMessage,int requestCode){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("setRepeatingAlarm",hour,minute,alarmTile,alarmMessage,alarmTickerMessage,requestCode);
		}else{
			AUP.Utils.Message(TAG,"warning: must run in actual android device");
		}
		#endif
	}

	/// <summary>
	/// Starts the alarm based on your set time then repeat alarm based on repeat delay
	/// </summary>
	/// <param name="hour">Hour.</param>
	/// <param name="minute">Minute.</param>
	/// <param name="repeatDelay">Repeat delay.</param>
	/// <param name="alarmTile">Alarm tile.</param>
	/// <param name="alarmMessage">Alarm message.</param>
	/// <param name="alarmTickerMessage">Alarm ticker message.</param>
	public void SetRepeatingAlarmWithInterval(int hour, int minute,int repeatDelay,string alarmTile,string alarmMessage,string alarmTickerMessage,int requestCode){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("setRepeatingAlarmWithInterval",hour,minute,repeatDelay,alarmTile,alarmMessage,alarmTickerMessage,requestCode);
		}else{
			AUP.Utils.Message(TAG,"warning: must run in actual android device");
		}
		#endif
	}

	/// <summary>
	/// Cancels the alarm
	/// </summary>
	/// <returns><c>true</c> if this instance cancel alarm; otherwise, <c>false</c>.</returns>
	public void CancelAlarm(int requestCode){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("cancelAlarm",requestCode);
		}else{
			AUP.Utils.Message(TAG,"warning: must run in actual android device");
		}
		#endif
	}

	/// <summary>
	/// Stops the alarm tone
	/// </summary>
	public void StopAlarm(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("stopAlarm");
		}else{
			AUP.Utils.Message(TAG,"warning: must run in actual android device");
		}
		#endif
	}

	/// <summary>
	/// Plays the alarm tone
	/// </summary>
	public void PlayAlarm(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("playAlarm");
		}else{
			AUP.Utils.Message(TAG,"warning: must run in actual android device");
		}
		#endif
	}
}