using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class LocalNotificationDemo : MonoBehaviour{

	private LocalNotificationPlugin localNotificationPlugin;

	//store request code of all local notification
	//tip save this on playerpref so that you can still access it when your player quit and the open your application
	private List<int> notificationRequestCodeCollection = new List<int>();
	
	// Use this for initialization
	void Start (){
		localNotificationPlugin = LocalNotificationPlugin.GetInstance();
		localNotificationPlugin.SetDebug(0);
		
		int isOpenUsingNotification = localNotificationPlugin.IsOpenUsingNotification();
		Debug.Log("[ShareAndExperienceDemo]: isOpenUsingNotification " + isOpenUsingNotification);
		
		if(isOpenUsingNotification == 1){
			//do something here
		}else{
			//do something here
		}
	}
	
	public void ScheduleLocalNotification(){
		Debug.Log("ScheduleLocalNotification");
		//schedule notification
		//1000 = 1 sec
		int delay = 3000;
		//request code is the unique id of local notification
		int requestCode = notificationRequestCodeCollection.Count;
		localNotificationPlugin.ScheduleNotification("my notification title","my notification message","my notification ticker message",delay,true,true,requestCode);
		notificationRequestCodeCollection.Add(requestCode);
		
		Debug.Log("added scheduled notification with requestCode " + requestCode );
	}

	public void ScheduleMultipleNotification(){
		Debug.Log("ScheduleMultipleNotification");

		int notificationCount = 3;

		for(int index=0;index<notificationCount;index++){
			//schedule notification
			//1000 = 1 sec
			int delay = 3000 + (index * 3000);
			//request code is the unique id of local notification
			int requestCode = notificationRequestCodeCollection.Count;
			localNotificationPlugin.ScheduleNotification("my notification title_" + index,"my notification message_"+index,"my notification ticker message_"+index,delay,true,true,requestCode);
			notificationRequestCodeCollection.Add(requestCode);
			
			Debug.Log("added scheduled notification with requestCode " + requestCode );
		}

	}
	
	public void CancelScheduledNotification(){
		//this is how we cancel or remove specific local notification 
		//assuming that in this scenario we want to cancel the previous local notifcation that we scheduled 
		//before it get fired,please try to avoid canceling or removing an already fired local notification because its useless
		
		if(notificationRequestCodeCollection.Count > 0){
			int prevRequestCode = notificationRequestCodeCollection.Count - 1;
			CancelSpecificNotification(prevRequestCode);
		}
	}
	
	
	//request code is the unique id of local notification use this to cancel or remove specific scheduled pending notification 
	private void CancelSpecificNotification(int requestCode){
		int len = notificationRequestCodeCollection.Count;
		Debug.Log("trying to cancel scheduled notification requestCode " + requestCode );
		
		for(int index=0;index<len;index++){
			int currentRequestCode = notificationRequestCodeCollection[index];
			if(currentRequestCode == requestCode){
				Debug.Log("found Scheduled notification with requestCode " + requestCode + " cancelling... ");
				localNotificationPlugin.CancelScheduledNotification(requestCode);
				break;
			}
		}
	}
	
	public void CancelAllNotification(){
		localNotificationPlugin.ClearAllScheduledNotification();
	}
}