using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public class UIController : MonoBehaviour {

	private GameObject mainCamera;

	void Start()
	{
		mainCamera = GameObject.Find ("OVRCameraRig");
	}

	public void OnClickRobot()
	{
		GameObject.FindGameObjectWithTag ("dog").GetComponent<DogController> ().ToRobot ();
	}

	public void OnClickInteract()
	{
		GameObject.FindGameObjectWithTag ("dog").GetComponent<DogController> ().ToInteract ();
	}

	public void OnClickBall()
	{
		GameObject.FindGameObjectWithTag ("dog").GetComponent<DogController> ().ToBall ();
	}

	public void OnClickRecord()
	{
		GameObject.FindGameObjectWithTag ("dog").GetComponent<DogController> ().ToExercise ();
	}

	public void OnClickPlay()
	{
		GameObject.FindGameObjectWithTag ("dog").GetComponent<DogController> ().ToOrder ();
	}

	public void OnClickOrder()
	{

	}

	public void OnClickInteractOral()
	{
		
	}
	
	public void OnClickSitDownPlay()
	{
		GameObject.FindGameObjectWithTag ("Word").GetComponent<Word> ().PlayProfile (Word.WORD_SITDOWN);
	}

	public void OnClickFallDownPlay()
	{
		GameObject.FindGameObjectWithTag ("Word").GetComponent<Word> ().PlayProfile (Word.WORD_FALLDOWN);
	}

	public void OnClickStandUpPlay()
	{
		GameObject.FindGameObjectWithTag ("Word").GetComponent<Word> ().PlayProfile (Word.WORD_STANDUP);
	}

	public void OnClickRightRawUpPlay()
	{
		GameObject.FindGameObjectWithTag ("Word").GetComponent<Word> ().PlayProfile (Word.WORD_RIGHTRAWUP);
	}

	public void OnClickLeftRawUpPlay()
	{
		GameObject.FindGameObjectWithTag ("Word").GetComponent<Word> ().PlayProfile (Word.WORD_LEFTRAWUP);
	}

	public void OnClickNoisePlay()
	{
		GameObject.FindGameObjectWithTag ("Word").GetComponent<Word> ().PlayProfile (Word.WORD_NOISE);
	}

	public void OnClickLoad()
	{
		GameObject.FindGameObjectWithTag ("Word").GetComponent<Word> ().ProfileLoad();
	}

	public void OnClickSave()
	{
		GameObject.FindGameObjectWithTag ("Word").GetComponent<Word> ().ProfileSave();
	}

	public void OnClickVolume()
	{
		DogController dogController = GameObject.FindGameObjectWithTag ("dog").GetComponent<DogController> ();
		dogController.volumeMute = !dogController.volumeMute;

		mainCamera.GetComponent<AudioSource> ().mute = dogController.volumeMute; 
		//GameObject.FindGameObjectWithTag ("dog").GetComponent<AudioSource> ().enabled = !GameObject.FindGameObjectWithTag ("dog").GetComponent<AudioSource> ().enabled;
		Button btn = dogController.btnVolume.GetComponent<Button> ();
		if(!dogController.volumeMute)
			btn.image.sprite = Resources.Load<Sprite>("UI/volume");
		else
			btn.image.sprite = Resources.Load<Sprite>("UI/mute");
	}

	public void OnClickHelp()
	{
		var dogController = GameObject.FindGameObjectWithTag ("dog").GetComponent<DogController> ();
		Image imgHelp = dogController.imgHelp;
		if(!imgHelp.IsActive())
		{
			imgHelp.gameObject.SetActive(true);
			dogController.timeImgHelp = Time.time;
		}
	}


	/*
	public void OnClick()
	{
		if (GameObject.FindGameObjectWithTag ("RobotScript") != null) {
			Destroy(GameObject.FindGameObjectWithTag("RobotScript"));
			Instantiate(Resources.Load("Prefabs/EnterInteract"));
		} else {
			Destroy(GameObject.FindGameObjectWithTag("EnterInteract"));
			Destroy(GameObject.FindGameObjectWithTag("Interact"));
			Instantiate(Resources.Load("Prefabs/RobotScript"));
		}
	}

	public void onClickBall()
	{
		GameObject go = GameObject.FindGameObjectWithTag ("Ball");
		if (go != null) {
			Destroy(go);
		} else {
			go = Instantiate(Resources.Load("Prefabs/Ball")) as GameObject;
			go.GetComponent<Rigidbody>().isKinematic = true;
			go.transform.position = mainCamera.transform.position + mainCamera.transform.rotation * (new Vector3(0, 0, 0.4f + 0.08f));
			GameObject goPlay = Instantiate(Resources.Load("Prefabs/BallPlay")) as GameObject;
		}
	}
	*/
}
