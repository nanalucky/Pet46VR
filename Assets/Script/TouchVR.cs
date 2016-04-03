using UnityEngine;
using System.Collections;

public class TouchVR : MonoBehaviour {

	public enum State
	{
		Touch,
		Enjoy,
		NotTouch,
		None,
	}

	public string partName;
	public string animationName;
	public float maxStillTime = 1.0f;
	public float timeIntoEnjoy = 1.0f;
	public float timeOutEnjoy = 1.0f;
	public State state;

	public Color colorTouch = Color.white;
	public Color colorNotTouch = Color.red;

	private GameObject goDog;
	private GameObject go;
	private Collider co;
	private SkinnedCollisionHelper skinHelper;
	private GameObject goCrosshairTouch;

	private float timeInTouch;
	private float timeNotInTouch;
	private Quaternion lastRotation;
	private float lastRotationTime;

	private bool lastInTouch = false;
	private int aniset = 0;
	private static int headTimes = 0;
	private static int backTimes = 0;

	private TouchVR[] touches;
	private bool firstFrame = true;

	// Use this for initialization
	void Start () {
		goDog = GameObject.FindGameObjectWithTag ("dog");
		go = GameObject.Find (partName);
		go.AddComponent<MeshCollider> ();
		skinHelper = go.AddComponent<SkinnedCollisionHelper> ();
		skinHelper.updateOncePerFrame = false;
		co = go.GetComponent<MeshCollider> ();
		goCrosshairTouch = goDog.GetComponent<DogController> ().goCrosshairTouch;

		timeInTouch = 0.0f;
		timeNotInTouch = 0.0f;
		lastRotation = Quaternion.identity;
		lastRotationTime = 0.0f;

		SetCrosshairColor (colorNotTouch);
	}

	bool InTouch()
	{
		if (lastRotation == Quaternion.identity || goCrosshairTouch.transform.rotation != lastRotation) 
		{
			lastRotation = goCrosshairTouch.transform.rotation;
			lastRotationTime = Time.time;
			return true;
		}

		if (Time.time - lastRotationTime <= maxStillTime)
			return true;

		return false;
	}

	void SetCrosshairColor(Color color)
	{
		SpriteRenderer sr = goCrosshairTouch.GetComponent<SpriteRenderer> ();
		sr.color = color;
	}

	void DisableAllTouchesButThis()
	{
		foreach( TouchVR obj in touches )
		{
			if(obj != this)
				obj.enabled = false;
		}
	}

	void EnableAllTouches()
	{
		foreach( TouchVR obj in touches )
		{
			obj.enabled = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (firstFrame) 
		{
			firstFrame = false;
			touches = FindObjectsOfType(typeof(TouchVR)) as TouchVR[];
		}

		Vector3 fwd = goCrosshairTouch.transform.TransformDirection(Vector3.forward);
		Ray ray = new Ray (goCrosshairTouch.transform.position, fwd);
		RaycastHit hit;
		bool ret;// = co.Raycast (ray, out hit, 100.0f) && Input.GetMouseButton(0);
		switch (state) 
		{
		case State.None:
			ret = false;
			if(!lastInTouch)
			{
				skinHelper.UpdateCollisionMesh();
				ret = co.Raycast (ray, out hit, 100.0f);
				lastInTouch = ret;
			}

			if(ret)
			{
				timeInTouch = Time.time;
				state = State.Touch;
				lastRotation = goCrosshairTouch.transform.rotation;
				lastRotationTime = Time.time;
				lastInTouch = false;
				DisableAllTouchesButThis();
			}

			if(ret)
				SetCrosshairColor(colorTouch);
			else
				SetCrosshairColor(colorNotTouch);
			break;
		case State.Touch:
			ret = false;
			skinHelper.UpdateCollisionMesh();
			ret = co.Raycast (ray, out hit, 100.0f);

			if(ret && InTouch())
			{
				if(Time.time - timeInTouch >= timeIntoEnjoy)
				{
					state = State.Enjoy;
					aniset = 0;
					if(string.Compare(animationName, "TouchHead") == 0)
					{
						headTimes ++;
						if((headTimes % 3) <= 1)
							aniset = 1;
					}
					else
					{
						backTimes ++;
						if((backTimes % 3) == 0)
							aniset = 1;
					}
	
					switch(aniset)
					{
					case 0:
						goDog.GetComponent<Animator>().SetBool("toempty", false);
						if(string.Compare(animationName, "TouchHead") == 0)
						{
							goDog.GetComponent<Animator>().CrossFade ("TouchHeadHead2", 0.25f, 1);
							goDog.GetComponent<Animator>().CrossFade("TongueOut", 0.25f, 3);
						}
						else
						{
							goDog.GetComponent<Animator>().CrossFade ("TouchBackHead2", 0.25f, 1);
							goDog.GetComponent<Animator>().Play ("EyeClose", 2);
						}
						break;
					case 1:
						goDog.GetComponent<Animator>().SetBool("toempty", false);
						if(string.Compare(animationName, "TouchHead") == 0)
						{
							if(goDog.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("SitIdle"))
								goDog.GetComponent<Animator>().CrossFade("TouchHeadHeadForSit", 0.25f, 1);
							else
								goDog.GetComponent<Animator>().CrossFade ("TouchHeadHead", 0.25f, 1);
						}
						else
						{
							if(goDog.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("SitIdle"))
								goDog.GetComponent<Animator>().CrossFade ("TouchBackHeadForSit", 0.25f, 1);
							else
								goDog.GetComponent<Animator>().CrossFade ("TouchBackHead", 0.25f, 1);
						}
						break;
					}
				}
			}
			else
			{
				state = State.None;
				EnableAllTouches();
			}

			if(ret)
				SetCrosshairColor(colorTouch);
			else
				SetCrosshairColor(colorNotTouch);

			break;
		case State.Enjoy:
			ret = false;
			skinHelper.UpdateCollisionMesh();
			ret = co.Raycast (ray, out hit, 100.0f);

			if(!(ret && InTouch ()))
			{
				state = State.NotTouch;
				timeNotInTouch = Time.time;
			}

			if(ret)
				SetCrosshairColor(colorTouch);
			else
				SetCrosshairColor(colorNotTouch);
			break;
		case State.NotTouch:
			ret = false;
			skinHelper.UpdateCollisionMesh();
			ret = co.Raycast (ray, out hit, 100.0f);

			if(ret && InTouch())
			{
				state = State.Enjoy;
			}
			else
			{
				if(Time.time - timeNotInTouch > timeOutEnjoy)
				{
					state = State.None;
					EnableAllTouches();
					switch(aniset)
					{
					case 0:
						if(string.Compare(animationName, "TouchHead") == 0)
						{
							goDog.GetComponent<Animator>().SetBool("toempty", true);
							goDog.GetComponent<Animator>().CrossFade("TongueIn", 0.25f, 3);
						}
						else
						{
							goDog.GetComponent<Animator>().SetBool("toempty", true);
							goDog.GetComponent<Animator>().Play ("EyeOpen", 2);
						}
						break;
					case 1:
						if(string.Compare(animationName, "TouchHead") == 0)
						{
							goDog.GetComponent<Animator>().SetBool("toempty", true);
						}
						else
						{
							goDog.GetComponent<Animator>().SetBool("toempty", true);
						}
						break;
					}
				}
			}

			if(ret)
				SetCrosshairColor(colorTouch);
			else
				SetCrosshairColor(colorNotTouch);
			break;
		}
	}

	void OnDestroy() {
		go = GameObject.Find (partName);
		if (go != null) {
			Destroy (go.GetComponent<MeshCollider> ());
			Destroy (go.GetComponent<SkinnedCollisionHelper> ());
		}
	}
}
