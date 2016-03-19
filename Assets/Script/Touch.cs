using UnityEngine;
using System.Collections;

public class Touch : MonoBehaviour {

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

	private GameObject goDog;
	private GameObject go;
	private Collider co;
	private SkinnedCollisionHelper skinHelper;

	private float timeInTouch;
	private float timeNotInTouch;
	private Vector3 lastPosition;
	private float lastPositionTime;

	private bool lastMouseDown = false;
	private int aniset = 0;
	private static int headTimes = 0;
	private static int backTimes = 0;

	// Use this for initialization
	void Start () {
		goDog = GameObject.FindGameObjectWithTag ("dog");
		go = GameObject.Find (partName);
		go.AddComponent<MeshCollider> ();
		skinHelper = go.AddComponent<SkinnedCollisionHelper> ();
		skinHelper.updateOncePerFrame = false;
		co = go.GetComponent<MeshCollider> ();

		timeInTouch = 0.0f;
		timeNotInTouch = 0.0f;
		lastPosition = Vector3.zero;
		lastPositionTime = 0.0f;
	}

	bool InTouch()
	{
		if (lastPosition == Vector3.zero || Input.mousePosition != lastPosition) 
		{
			lastPosition = Input.mousePosition;
			lastPositionTime = Time.time;
			return true;
		}

		if (Time.time - lastPositionTime <= maxStillTime)
			return true;

		return false;
	}
	
	// Update is called once per frame
	void Update () {
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		bool ret;// = co.Raycast (ray, out hit, 100.0f) && Input.GetMouseButton(0);
		switch (state) 
		{
		case State.None:
			ret = false;
			if(!lastMouseDown && Input.GetMouseButton(0))
			{
				skinHelper.UpdateCollisionMesh();
				ret = co.Raycast (ray, out hit, 100.0f);
			}
			lastMouseDown = Input.GetMouseButton(0);

			if(ret)
			{
				timeInTouch = Time.time;
				state = State.Touch;
				lastPosition = Input.mousePosition;
				lastPositionTime = Time.time;
			}
			break;
		case State.Touch:
			ret = false;
			if(Input.GetMouseButton(0))
			{
				skinHelper.UpdateCollisionMesh();
				ret = co.Raycast (ray, out hit, 100.0f);
			}

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
			}
			break;
		case State.Enjoy:
			ret = false;
			if(Input.GetMouseButton(0))
			{
				skinHelper.UpdateCollisionMesh();
				ret = co.Raycast (ray, out hit, 100.0f);
			}

			if(!(ret && InTouch ()))
			{
				state = State.NotTouch;
				timeNotInTouch = Time.time;
			}
			break;
		case State.NotTouch:
			ret = false;
			if(Input.GetMouseButton(0))
			{
				skinHelper.UpdateCollisionMesh();
				ret = co.Raycast (ray, out hit, 100.0f);
			}

			if(ret && InTouch())
			{
				state = State.Enjoy;
			}
			else
			{
				if(Time.time - timeNotInTouch > timeOutEnjoy)
				{
					state = State.None;
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
