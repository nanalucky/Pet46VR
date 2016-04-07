using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour {
	
	private Dictionary <string, UnityEvent> eventDictionary;
	
	private static EventManager instance;
	private static GameObject container;


	public static EventManager GetInstance(){
		if(instance == null){
			container = new GameObject();
			container.name="EventManager";
			instance = container.AddComponent(typeof(EventManager)) as EventManager;
			DontDestroyOnLoad(instance.gameObject);
		}

		return instance;
	}
	
	public void Init (){
		if (eventDictionary == null)
		{
			eventDictionary = new Dictionary<string, UnityEvent>();
		}
	}
	
	public void StartListening (string eventName, UnityAction listener)
	{
		UnityEvent thisEvent = null;
		if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
		{
			thisEvent.AddListener (listener);
		} 
		else
		{
			thisEvent = new UnityEvent ();
			thisEvent.AddListener (listener);
			instance.eventDictionary.Add (eventName, thisEvent);
		}
	}
	
	public void StopListening (string eventName, UnityAction listener)
	{
		if (instance == null) return;
		UnityEvent thisEvent = null;
		if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
		{
			thisEvent.RemoveListener (listener);
		}
	}
	
	public void TriggerEvent (string eventName)
	{
		UnityEvent thisEvent = null;
		if (instance.eventDictionary.TryGetValue (eventName, out thisEvent))
		{
			thisEvent.Invoke ();
		}
	}
}