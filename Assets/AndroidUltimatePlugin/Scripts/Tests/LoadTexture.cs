using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadTexture : MonoBehaviour {

	public RawImage rawImage;

	//private string filename = "unity3d_logo.png";
	private string filename = "apple_logo.jpg";

	private string webUrl = "http://www.gigadrillgames.com/wp-content/uploads/2015/07/315x250_itchio.png";

	// Use this for initialization
	void Start () {
		//LoadExternalTestImage();
		LoadImageFromWeb(webUrl);
	}

	private void LoadInternalTestImage(){		
		string imagePath = Application.dataPath + "/AndroidUltimatePlugin/RawAssets/Textures/"+filename;
		rawImage.texture = AUP.Utils.LoadTexture(imagePath);
	}

	private void LoadExternalTestImage(){		
		//string path = Application.persistentDataPath + "/" + filename;
		//string path = "/storage/emulated/0/Pictures/MyCameraApp/" + filename;
		string path = "storage/emulated/0/Pictures/MyCameraApp/IMG_20150731_153203.jpg";
		
		Debug.Log("device imagepath " + path);
		
		rawImage.texture = AUP.Utils.LoadTexture(path);
	}

	private void LoadImageFromWeb(string webUrl){		
		Debug.Log("LoadImageFromWeb " + webUrl);
		StartCoroutine(AUP.Utils.LoadTextureFromWeb(webUrl,OnLoadImageComplete,OnLoadImageFail));
	}

	private void OnLoadImageComplete(Texture2D texture ){
		rawImage.texture =texture;
		Debug.Log("Load Image From Web compete texture " + texture);
	}

	private void OnLoadImageFail(){
		Debug.Log("Load Image From Web  fail! ");
	}
}
