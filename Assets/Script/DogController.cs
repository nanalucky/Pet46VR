using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;
using UnityEngine.UI;

public class DogController : MonoBehaviour {

	public Vector3 zoneMin = new Vector3(-3, 0, 0);
	public Vector3 zoneMax = new Vector3(3, 0, 7);
	public float walkSpeed = 0.85f;
	public float runSpeed = 1.8f;
	public float rushSpeed = 1.5f;
	public float ballDistance = 0.48f;

	public GameObject word;
	public Button btnRecord;
	public Button btnPlay;
	public Button btnOrder;
	public Button btnInteractOral;
	public Button[] btnRecords;
	public Button[] btnPlays;
	public Button btnVolume;
	public Button btnHelp;
	public Image imgHelp;
	[HideInInspector]public float timeImgHelp = 0;
	public Text textDebug;
	public Image imgQuestion;
	[HideInInspector]public float timeImgQuestion = 0;

	public Button btnRobot;
	public Button btnInteract;
	public Button btnBall;

	private float lookatBodyWeight;
	private float lookatHeadWeight;
	private float lookatEyeWeight;

	private Animator animator;
	private GameObject mainCamera;

	[HideInInspector] public bool volumeEnabled = true;
	[HideInInspector] public bool volumeMute = false;
	public Vector3[] lookats = new Vector3[]{new Vector3(7f,0f,5f),new Vector3(5f,0f,6f),new Vector3(3.5f,0f,4f),new Vector3(5f,0f,4f)};

	// Use this for initialization
	void Start () {
		animator = gameObject.GetComponent<Animator> ();
		mainCamera = GameObject.Find ("OVRCameraRig");
		lookatBodyWeight = gameObject.GetComponent<LookAtIK> ().solver.bodyWeight;
		lookatHeadWeight = gameObject.GetComponent<LookAtIK> ().solver.headWeight;
		lookatEyeWeight = gameObject.GetComponent<LookAtIK> ().solver.eyesWeight;
	}

	void Update(){
		if (Input.GetKeyUp(KeyCode.A)) {
			gameObject.GetComponent<Animator>().Play("Blink", 2);	
			gameObject.GetComponent<Animator>().Play("Blink", 5);	
		}

		if(Input.GetMouseButtonUp(0))
		{
			if(imgHelp.IsActive() && Time.time > timeImgHelp + 1.0f)
				imgHelp.gameObject.SetActive(false);
		}

		if(timeImgQuestion != 0.0f && Time.time > timeImgQuestion + 1.0f)
		{
			imgQuestion.gameObject.SetActive(false);
			timeImgQuestion = 0.0f;
		}

		if(btnInteractOral.interactable)
		{
			RectTransform rectTransform = (btnInteractOral.transform) as RectTransform;
			bool overButton = RectTransformUtility.RectangleContainsScreenPoint(rectTransform, new Vector2(Input.mousePosition.x, Input.mousePosition.y), null);
			if (Input.GetMouseButton (0) && overButton) {
				if(!word.GetComponent<Word>().IsEnableDetect())
				{
					word.GetComponent<Word>().EnableDetect(true);
					word.GetComponent<Word>().interact = true;
					EnableMusicAndEffect(false);
				}
			}
			else
			{
				if(word.GetComponent<Word>().IsEnableDetect())
				{
					word.GetComponent<Word>().EnableDetect(false);
					word.GetComponent<Word>().interact = false;
					EnableMusicAndEffect(true);
				}
			}
		}
	}

	public void EnableLookatIK(bool enabled)
	{
		LookAtIK lookatik = gameObject.GetComponent<LookAtIK> ();
		if (enabled) {
			lookatik.enabled = true;
			//Debug.LogWarning(string.Format("lookatid enable:{0}", Time.time));
		} else {
			lookatik.solver.bodyWeight = lookatBodyWeight;
			lookatik.solver.headWeight = lookatHeadWeight;
			lookatik.solver.eyesWeight = lookatEyeWeight;
			lookatik.Disable();
			StartCoroutine(PlayAnimInterval(2, 0.5f));
		}
	}

	IEnumerator PlayAnimInterval(int n, float time)
	{
		while (n > 0)
		{
			if(animator.GetCurrentAnimatorStateInfo(2).IsName("Blink")){
				yield return new WaitForSeconds(0);
			} else {
				//Debug.LogWarning(string.Format("Blink:{0}", Time.time));
				animator.Play ("Blink", 2);
				animator.Play ("Blink", 5);
				--n;
				yield return new WaitForSeconds(time);
			}
		}
	}

	public Vector3 ChooseLookat()
	{
		Vector3 nearest = lookats [0];
		float distance = (nearest - transform.position).magnitude;
		foreach (Vector3 pt in lookats) {
			float dis = (pt - transform.position).magnitude;
			if(dis > distance)
			{
				distance = dis;
				nearest = pt;
			}
		}
		ArrayList validLookats = new ArrayList();
		foreach (Vector3 pt in lookats) {
			if(pt != nearest)
			{
				validLookats.Add(pt);
			}
		}

		return (Vector3)(validLookats[(int)(Random.value * validLookats.Count)]);
	}

	public bool IsStand()
	{
		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("Stand") 
		    || animator.GetCurrentAnimatorStateInfo (0).IsName ("Idle1") 
		    || animator.GetCurrentAnimatorStateInfo (0).IsName ("Idle2")) {
			return true;
		}
		
		return false;
	}
	
	public void SitDown()
	{
		if (IsStand ()) {
			animator.CrossFade ("SitDown", 0.25f);
		}
	}
	
	public void FallDown()
	{
		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("SitIdle")) {
			animator.CrossFade ("SitToFall", 0.25f);
		}
		else
		{
			if (IsStand ()) {
				animator.CrossFade ("StandToFall", 0.25f);
			}
		}
	}
	
	public void StandUp()
	{
		if (animator.GetCurrentAnimatorStateInfo (0).IsName ("SitIdle")) {
			animator.CrossFade ("SitToStand", 0.25f);
		}
		else if (animator.GetCurrentAnimatorStateInfo (0).IsName ("FallIdle")) {
			animator.CrossFade ("FallToSit", 0.25f);
		}
		else if(animator.GetCurrentAnimatorStateInfo(0).IsName("SleepIdle")) {
			animator.CrossFade("Wake", 0.25f);
		}
	}
	
	public void RightRawUp()
	{
		if (IsStand ()) {
			animator.CrossFade("RightRawUp", 0.25f);
		}else{
            StandUp();
            StartCoroutine("PlayRightRawUp");
		}
	}
    IEnumerator PlayRightRawUp()
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Stand"))
        {
			yield return null;
		}
        animator.CrossFade("RightRawUp", 0.25f);
    }
	
	public void LeftRawUp()
	{
		if (IsStand ()) {
			animator.CrossFade("LeftRawUp", 0.25f);
		}else{
            StandUp();
            StartCoroutine("PlayLeftRawUp");
		}
	}

    IEnumerator PlayLeftRawUp()
    {
		while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Stand"))
		{
			yield return null;
		}
		animator.CrossFade("LeftRawUp", 0.25f);
    }

	void ClearAll()
	{
		Time.fixedDeltaTime = 0.0166666f;
		GetComponent<StartIdle> ().enabled = false;
		btnInteractOral.interactable = true;

		// robot
		Destroy(GameObject.FindGameObjectWithTag("RobotScript"));

		// interact
		Destroy(GameObject.FindGameObjectWithTag("EnterInteract"));
		Destroy(GameObject.FindGameObjectWithTag("Interact"));


		// ball
		Destroy(GameObject.FindGameObjectWithTag("EnterBall"));
		Destroy(GameObject.FindGameObjectWithTag("Ball"));
		Destroy (GameObject.FindGameObjectWithTag ("Mouth").GetComponent<BallCollideMouth> ());
		EnableLookatIK (false);

		// exercise
		Destroy(GameObject.FindGameObjectWithTag("EnterExercise"));
		Destroy(GameObject.FindGameObjectWithTag("Exercise"));

		// order
		Destroy(GameObject.FindGameObjectWithTag("EnterOrder"));
		Destroy(GameObject.FindGameObjectWithTag("Order"));

		//record.SetActive (false);
		word.GetComponent<Word> ().EnableDetect (false);
		word.GetComponent<Word>().interact = false;

		btnInteract.interactable = true;
		btnInteractOral.interactable = true;
		btnRobot.interactable = true;
		btnBall.interactable = true;
		btnRecord.interactable = true;
		btnPlay.interactable = true;
	}

	public void ToRobot()
	{
		ClearAll ();
		Instantiate(Resources.Load("Prefabs/RobotScript"));
		EnableMusicAndEffect (true);
		btnRobot.interactable = false;
		GameObject.Find ("NoTouchGUI").GetComponent<NoTouchGUI> ().Show (false);
	}

	public void ToInteract()
	{
		ClearAll ();
		Instantiate(Resources.Load("Prefabs/EnterInteract"));
		EnableMusicAndEffect (true);

		btnInteract.interactable = false;
		btnInteractOral.interactable = false;
		GameObject.Find ("NoTouchGUI").GetComponent<NoTouchGUI> ().Show (false);
	}

	public void ToInteract2()
	{
		if (IsStand ()) {
			animator.CrossFade("Interact", 0.25f);
			animator.CrossFade("Interact", 0.25f, 2);
		}else{
			StandUp();
			StartCoroutine("PlayToInteract2");
		}
	}
	
	IEnumerator PlayToInteract2()
	{
		while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Stand"))
		{
			yield return null;
		}
		animator.CrossFade("Interact", 0.25f);
		animator.CrossFade("Interact", 0.25f, 2);
	}

	public void ToBall()
	{
		ClearAll ();
		Instantiate(Resources.Load("Prefabs/EnterBall"));
		EnableMusicAndEffect (true);
		//btnBall.interactable = false;
		GameObject.Find ("NoTouchGUI").GetComponent<NoTouchGUI> ().Show (false);
	}

	public void ToExercise()
	{
		ClearAll ();
		Instantiate(Resources.Load("Prefabs/EnterExercise"));
		EnableMusicAndEffect (false);
		btnInteractOral.interactable = false;
		btnRecord.interactable = false;
	}

	public void ToOrder()
	{
		ClearAll ();
		Instantiate(Resources.Load("Prefabs/EnterOrder"));
		EnableMusicAndEffect (false);
		btnInteractOral.interactable = false;
		btnPlay.interactable = false;
	}

	public void PlayAudioEffect(string clipname)
	{
		string[] ret = clipname.Split (',');
		float volume = 1.0f;
		if(ret.Length >= 2)
			float.TryParse(ret[1], out volume);
		AudioClip audioClip = Resources.Load<AudioClip> (string.Format("Audio/{0}", ret[0]));
		AudioSource audioSource = gameObject.GetComponent<AudioSource> ();
		audioSource.PlayOneShot (audioClip, volume);
	}

	public void EnableMusicAndEffect(bool enable)
	{
		volumeEnabled = enable;
		btnVolume.interactable = enable;

		if(enable)
		{
			mainCamera.GetComponent<AudioSource>().mute = volumeMute;
			gameObject.GetComponent<AudioSource> ().mute = false;
		}
		else
		{
			mainCamera.GetComponent<AudioSource>().mute = true;
			gameObject.GetComponent<AudioSource> ().mute = false;
		}
	}

	public Vector3 GetDogPivot()
	{
		return transform.FindChild("pivot").position;
	}

	public void DebugShow(string label)
	{
		textDebug.text = label;
	}

	public void ShowQuestion()
	{
		imgQuestion.gameObject.SetActive(true);
		timeImgQuestion = Time.time;
	}
}
