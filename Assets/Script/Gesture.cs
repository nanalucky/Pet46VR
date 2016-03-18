using UnityEngine;
using System.Collections;

public class Gesture : MonoBehaviour {

	public enum State
	{
		SlideDownOnce,
		SlideDownTwice,
		SlideUpOnce,
		SlideUpTwice,
		None,
	}

	public class gesture
	{
		public State state = State.None;

		public virtual void Start(){}
		public virtual void Update(){}
		public virtual State GetState(){return state;}
	}

	public class SlideDown : gesture
	{
		private int times = 0;
		private Vector3 lastPosition = Vector3.zero;
		private float lastUpTime = 0.0f;

		void Reset()
		{
			times = 0;
			lastUpTime = 0.0f;
			lastPosition = Vector3.zero;
		}

		public override void Update()
		{
			state = State.None;

			if (Input.GetMouseButtonUp (0)) 
			{
				if(lastPosition != Vector3.zero)
				{
					if (Mathf.Abs(Input.mousePosition.x - lastPosition.x) <= 50 && (Input.mousePosition.y - lastPosition.y) <= -100)
					{
						times += 1;
						lastUpTime = Time.time;

						if (times == 2)
						{
							state = State.SlideDownTwice;
							Reset ();
						}
					}
				}
			}

			if (Input.GetMouseButtonDown (0)) 
			{
				if (lastPosition == Vector3.zero) 
				{
					lastPosition = Input.mousePosition;
					lastUpTime = 0.0f;
				}
				else
				{
					if (Mathf.Abs(Input.mousePosition.x - lastPosition.x) > 50 || (Input.mousePosition.y - lastPosition.y) > 0)
					{
						state = State.None;
						Reset();
					}
				}
			} 
			else 
			{
				if(times == 1)
				{
					if(lastUpTime != 0.0f && (Time.time - lastUpTime) >= 1.0f)
					{
						state = State.SlideDownOnce;
						Reset ();
					}
				}
			}
		}
	}

	public class SlideUp : gesture
	{
		private int times = 0;
		private Vector3 lastPosition = Vector3.zero;
		private float lastUpTime = 0.0f;
		
		void Reset()
		{
			times = 0;
			lastUpTime = 0.0f;
			lastPosition = Vector3.zero;
		}
		
		public override void Update()
		{
			state = State.None;
			
			if (Input.GetMouseButtonUp (0)) 
			{
				if(lastPosition != Vector3.zero)
				{
					if (Mathf.Abs(Input.mousePosition.x - lastPosition.x) <= 50 && (Input.mousePosition.y - lastPosition.y) >= 100)
					{
						times += 1;
						lastUpTime = Time.time;
						
						if (times == 2)
						{
							state = State.SlideUpTwice;
							Reset ();
						}
					}
				}
			}
			
			if (Input.GetMouseButtonDown (0)) 
			{
				if (lastPosition == Vector3.zero) 
				{
					lastPosition = Input.mousePosition;
					lastUpTime = 0.0f;
				}
				else
				{
					if (Mathf.Abs(Input.mousePosition.x - lastPosition.x) > 50 || (Input.mousePosition.y - lastPosition.y) < 0)
					{
						state = State.None;
						Reset();
					}
				}
			} 
			else 
			{
				if(times == 1)
				{
					if(lastUpTime != 0.0f && (Time.time - lastUpTime) >= 1.0f)
					{
						state = State.SlideUpOnce;
						Reset ();
					}
				}
			}
		}
	}

	private GameObject goDog;
	private Animator animator;
	private SlideDown slideDown = new SlideDown();
	private SlideUp slideUp = new SlideUp();

	private Collider coLeft;
	private Collider coRight;
	private float lastRawTime;

	// Use this for initialization
	void Start () {
		goDog = GameObject.FindGameObjectWithTag ("dog");
		animator = goDog.GetComponent<Animator>();
		coLeft = GameObject.Find ("lz").GetComponent<MeshCollider> ();
		coRight = GameObject.Find ("rz").GetComponent<MeshCollider> ();
		slideUp.Start ();
		slideDown.Start ();
		lastRawTime = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (lastRawTime != 0.0f) {
			if(Time.time - lastRawTime > 5.0f){
				lastRawTime = 0.0f;
				if(animator.GetCurrentAnimatorStateInfo(0).IsName("RightRawUp") 
				   || animator.GetCurrentAnimatorStateInfo(0).IsName("LeftRawUp"))
				{
					animator.CrossFade("Stand", 0.25f);
				}
			}
		}

		slideUp.Update ();
		slideDown.Update ();

		State state = GetState ();
		switch (state) {
		case State.SlideDownOnce:
			goDog.GetComponent<DogController>().SitDown ();
			break;
		case State.SlideDownTwice:
			goDog.GetComponent<DogController>().FallDown();
			break;
		case State.SlideUpTwice:
			goDog.GetComponent<DogController>().StandUp();
			break;
		case State.None:
			if(Input.GetMouseButtonUp(0))
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if(coRight.Raycast(ray, out hit, 100.0f))
				{
					goDog.GetComponent<DogController>().RightRawUp();
					lastRawTime = Time.time;
				}

				if(coLeft.Raycast(ray, out hit, 100.0f))
				{
					goDog.GetComponent<DogController>().LeftRawUp();
					lastRawTime = Time.time;
				}
			}
			break;
		}
	}

	public State GetState()
	{
		if (slideUp.GetState () != State.None)
			return slideUp.GetState ();

		return slideDown.GetState ();
	}


}