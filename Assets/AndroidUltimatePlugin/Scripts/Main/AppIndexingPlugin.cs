using UnityEngine;
using System.Collections;
using System;

public class AppIndexingPlugin : MonoBehaviour {
	
	private static AppIndexingPlugin instance;
	private static GameObject container;
	private static AUPHolder aupHolder;
	private const string TAG="[AppIndexingPlugin]: ";
	
	#if UNITY_ANDROID
	private static AndroidJavaObject jo;
	#endif	
	
	public bool isDebug =true;
	
	public static AppIndexingPlugin GetInstance(){
		if(instance==null){
			container = new GameObject();
			container.name="AppIndexingPlugin";
			instance = container.AddComponent( typeof(AppIndexingPlugin) ) as AppIndexingPlugin;
			DontDestroyOnLoad(instance.gameObject);
			aupHolder = AUPHolder.GetInstance();
			instance.gameObject.transform.SetParent(aupHolder.gameObject.transform);
		}
		
		return instance;
	}
	
	private void Awake(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo = new AndroidJavaObject("com.gigadrillgames.androidplugin.appindexing.AppIndexingPlugin");
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
		}else{
			AUP.Utils.Message(TAG,"warning: must run in actual android device");
		}
		#endif
	}	

	/// <summary>
	/// Initialize the google indexing.
	/// Note: calls this on start once
	/// </summary>
	/// <param name="appName">App name.</param>
	/// <param name="appUrlLink">appUrlLink should follow this format android-app://{package_name}/{scheme}/{host_path} </param>
	/// <param name="appWebUrl">App web URL.</param>
	public void InitGoogleIndexing(string appName,string appUrlLink, string appWebUrl){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("initGoogleIndexing",appName,appUrlLink,appWebUrl);
		}else{
			AUP.Utils.Message(TAG,"warning: must run in actual android device");
		}
		#endif
	}
	
	/// <summary>
	/// Starts the google indexing.
	/// Note: call this on start once
	/// </summary>
	public void StartGoogleIndexing(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("startGoogleIndexing");
		}else{
			AUP.Utils.Message(TAG,"warning: must run in actual android device");
		}
		#endif
	}
	
	
	/// <summary>
	/// Stops the google indexing.
	/// call this on destoy
	/// </summary>
	public void StopGoogleIndexing(){
		#if UNITY_ANDROID
		if(Application.platform == RuntimePlatform.Android){
			jo.CallStatic("stopGoogleIndexing");
		}else{
			AUP.Utils.Message(TAG,"warning: must run in actual android device");
		}
		#endif
	}
}