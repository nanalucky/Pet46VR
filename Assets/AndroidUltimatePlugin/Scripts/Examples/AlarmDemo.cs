using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public class AlarmDemo : MonoBehaviour {

	private AlarmPlugin alarmPlugin;

	public InputField hourInput;
	public InputField minuteInput;
	public InputField delayIntervalInput;

	private TimePlugin timePlugin;
	private int hour = 0;
	private int minute = 0;
	private int delayInterval = 0;

	//store request code of all Pending Alarm Notifications
	//tip save this on playerpref so that you can still access it when your player quit and the open your application
	private List<int> alarmRequestCodeCollection = new List<int>();


	// Use this for initialization
	void Start (){
		timePlugin = TimePlugin.GetInstance();
		timePlugin.SetDebug(0);

		alarmPlugin = AlarmPlugin.GetInstance();
		alarmPlugin.SetDebug(0);
		alarmPlugin.Init();
	}

	private void getHourMinute(){
		if(hourInput!=null){
			if(hourInput.text.Equals("",StringComparison.Ordinal)){
				//default get current hour
				hour = int.Parse(timePlugin.GetHour());
				hourInput.text =hour.ToString();
			}else{
				//hour =  int.Parse(hourInput.text);
				if(!int.TryParse(hourInput.text,out hour)){
					//default get current hour
					hour = int.Parse(timePlugin.GetHour());
					hourInput.text =hour.ToString();
				}else{
					if(hour>=24){
						hour = 24;
					}
					hourInput.text =hour.ToString();
				}
			}
		}
		
		if(minuteInput!=null){
			if(minuteInput.text.Equals("",StringComparison.Ordinal)){
				//default 5 minutes
				minute = int.Parse(timePlugin.GetMinute()) + 1;
				minuteInput.text =minute.ToString();
			}else{
				//minute =  int.Parse(minuteInput.text);
				if(!int.TryParse(minuteInput.text,out minute)){
					//default 5 minutes
					minute = int.Parse(timePlugin.GetMinute()) + 1;
					minuteInput.text =minute.ToString();
				}else{
					if(minute>=59){
						minute = 59;
					}
					minuteInput.text =minute.ToString();
				}
			}
		}
	}

	private void getDelayInterval(){
		if(delayIntervalInput!=null){
			if(delayIntervalInput.text.Equals("",StringComparison.Ordinal)){
				//default 10 seconds
				delayInterval = 1000 * 10;
				delayIntervalInput.text = delayInterval.ToString();
			}else{
				//delayInterval =  int.Parse(delayIntervalInput.text);
				if(!int.TryParse(delayIntervalInput.text,out delayInterval)){
					delayInterval = 1000 * 10;
					delayIntervalInput.text = delayInterval.ToString();
				}else{
					delayInterval =delayInterval * 1000;
					delayIntervalInput.text = delayInterval.ToString();
				}
			}
		}
	}
	
	public void SetOneTimeAlarm(){
		//sample
		//3:30 PM
		//alarmPlugin.SetOneTimeAlarm(15,30,"Alarm Title - one time alarm","Alarm Message","Alarm Ticker Message");

		getHourMinute();

		//request code is the unique id of alarm pending Notification
		int requestCode = alarmRequestCodeCollection.Count;
		alarmPlugin.SetOneTimeAlarm(hour,minute,"Alarm Title - one time alarm","Alarm Message","Alarm Ticker Message",requestCode);
		alarmRequestCodeCollection.Add(requestCode);

		Debug.Log("added scheduled Alarm Pending Notification with requestCode " + requestCode );
	}

	public void SetRepeatingAlarm(){
		//sample
		//4:30 PM
		//alarmPlugin.SetRepeatingAlarm(16,30,"Alarm Title - repeating alarm","Alarm Message","Alarm Ticker Message");

		getHourMinute();
		//request code is the unique id of alarm pending Notification
		int requestCode = alarmRequestCodeCollection.Count;
		alarmPlugin.SetRepeatingAlarm(hour,minute,"Alarm Title - repeating alarm","Alarm Message","Alarm Ticker Message",requestCode);
		alarmRequestCodeCollection.Add(requestCode);
		
		Debug.Log("added scheduled Alarm Pending Notification with requestCode " + requestCode );
	}

	public void SetRepeatingAlarmWithInterval(){
		//sample
		//1 minute
		//int delayInterval = (1000 * 60);
		// 3:45PM
		//alarmPlugin.SetRepeatingAlarmWithInterval(15,45,delayInterval,"Alarm Title - repeating alarm with interval","Alarm Message","Alarm Ticker Message");

		getHourMinute();
		getDelayInterval();

		//request code is the unique id of alarm pending Notification
		int requestCode = alarmRequestCodeCollection.Count;

		alarmPlugin.SetRepeatingAlarmWithInterval(hour,minute,delayInterval,"Alarm Title - repeating alarm with interval","Alarm Message","Alarm Ticker Message",requestCode);
		alarmRequestCodeCollection.Add(requestCode);
		
		Debug.Log("added scheduled Alarm Pending Notification with requestCode " + requestCode );
	}

	public void CancelAllScheduledAlarm(){
		int len = alarmRequestCodeCollection.Count;
		
		for(int index=0;index<len;index++){
			int currentRequestCode = alarmRequestCodeCollection[index];
			alarmPlugin.CancelAlarm(currentRequestCode);
		}
	}

	public void CancelPrevScheduledAlarm(){
		if(alarmRequestCodeCollection.Count > 0){
			int prevRequestCode = alarmRequestCodeCollection.Count - 1;
			alarmPlugin.CancelAlarm(prevRequestCode);
		}
	}

	public void StopAlarm(){
		alarmPlugin.StopAlarm();
	}

	public void PlayAlarm(){
		alarmPlugin.PlayAlarm();
	}
}
