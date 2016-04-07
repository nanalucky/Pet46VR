using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GoogleIndexingDemo : MonoBehaviour {

	private AppIndexingPlugin appIndexingPlugin;
	private UtilsPlugin utilsPlugin;

	// Use this for initialization
	void Start (){		
		appIndexingPlugin = AppIndexingPlugin.GetInstance();
		appIndexingPlugin.SetDebug(0);

		utilsPlugin = UtilsPlugin.GetInstance();
		utilsPlugin.SetDebug(0);

		InitGoogleIndexing();
	}

	public void InitGoogleIndexing(){
		string appTitle = "AndroidUltimatePlugin";

		string appPackage = utilsPlugin.GetPackageId();
		Debug.Log(" packageId " + appPackage);

		string androidOSVersion = utilsPlugin.GetAndroidVersion();
		Debug.Log(" androidOSVersion " + androidOSVersion);


		// app url link should follow this format android-app://{package_name}/{scheme}/{host_path}
		
		/*
		Definitions:
		
		package_name: application ID as specified in the Android Play Store.
		scheme: the scheme to pass to the application. Can be http, or a custom scheme.
		host_path: identifies the specific content within your application.
		
		*/


		//in my example i dont have any hostpath that's why i didn't add any host path
		//string appUrlLink = "android-app://" + appPackage + "/play.google.com/store/apps/details?id=" + appPackage;

		string scheme = "/play.google.com/store/apps/details?id=";
		string appUrlLink = "android-app://" + appPackage + scheme + appPackage;

		//sample app web url you need to change this with your own website url or link inside your website
		string appWebUrl = "http://www.google.com";

		appIndexingPlugin.InitGoogleIndexing(appTitle,appUrlLink,appWebUrl);
	}

	public void StartGoogleIndexing(){		
		appIndexingPlugin.StartGoogleIndexing();
	}

	public void StopGoogleIndexing(){		
		appIndexingPlugin.StopGoogleIndexing();
	}
}
