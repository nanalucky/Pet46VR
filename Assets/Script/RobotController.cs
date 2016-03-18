using UnityEngine;
using System.Collections;

/**
 * todo: animate turn,finish sniffer
 **/
public class RobotController : MonoBehaviour {

	public class RobotAI
	{
		protected RobotController controller;

		public virtual void Start(RobotController ctrl){}
		public virtual void Update(){}
		public virtual bool IsFinished(){return true;}
		public virtual void Finish(){}
		public virtual RobotState GetNextState(){return RobotState.Sleep;}
	}

	public class RobotAISleep : RobotAI
	{
		private float timeEnd;
		private bool eyeClose = false;

		public override void Start(RobotController ctrl)
		{
			controller = ctrl;
			timeEnd = Time.time + Random.Range (2.0f, 10.0f);

			controller.go.GetComponent<Animator> ().Play ("StandToSleep");
		}

		public override void Update()
		{
			var animator = controller.go.GetComponent<Animator> ();
			if (!eyeClose && animator.GetCurrentAnimatorStateInfo (0).IsName ("Sleep")) {
				eyeClose = true;
				animator.Play("EyeClose", 2);
				animator.Play("EyeClose", 5);
			}
		}

		public override bool IsFinished()
		{
			return Time.time > timeEnd ? true : false;
		}

		public override RobotState GetNextState()
		{
			return RobotState.Wake;
		}
	}

/*	public class RobotAIWake : RobotAI
	{
		private float timeEnd;
		
		public override void Start(RobotController ctrl)
		{
			controller = ctrl;
			timeEnd = -1.0f;

			controller.go.GetComponent<Animator> ().Play ("Wake");
		}

		public override void Update()
		{
			if (timeEnd < 0.0f) 
			{
				var clipInfo = controller.go.GetComponent<Animator> ().GetCurrentAnimatorClipInfo (0);
				timeEnd = Time.time + clipInfo [0].clip.length;
			}
		}
		
		public override bool IsFinished()
		{
			if(timeEnd >= 0.0f)
				return Time.time > timeEnd ? true : false;
			return false;
		}
		
		public override RobotState GetNextState()
		{
			var states = new RobotState[]{RobotState.Walk, RobotState.Sniffer};
			return states [ (int)(Random.value * 10.0f) % states.Length ];
		}
	}
*/

	public class RobotAIWake : RobotAI
	{
		public override void Start(RobotController ctrl)
		{
			controller = ctrl;			
			controller.go.GetComponent<Animator> ().Play ("Wake");
			controller.go.GetComponent<Animator> ().Play ("EyeOpen", 2);
			controller.go.GetComponent<Animator> ().Play ("EyeOpen", 5);
		}
		
		public override bool IsFinished()
		{
			if (controller.go.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("Stand"))
				return true;

			return false;
		}
		
		public override RobotState GetNextState()
		{
			var states = new RobotState[]{RobotState.WalkWithNoShake};
			return states [ (int)(Random.value * 10.0f) % states.Length ];
		}
	}

	public class RobotAIWalkWithNoShake : RobotAI
	{
		private Vector3 vecStart;
		private Vector3 vecEnd;
		private float timeStart;

		private Vector3 direction;
		private float timeTotal;
		private float dstEulerY;
		private float eulerSmooth = 0.2f;
		private float velEulerY;

		public override void Start(RobotController ctrl)
		{
			controller = ctrl;
			vecStart = controller.go.transform.position;
			vecEnd = new Vector3 (Random.Range (controller.go.GetComponent<DogController>().zoneMin.x, 
			                                    controller.go.GetComponent<DogController>().zoneMax.x), 
			                      Random.Range (controller.go.GetComponent<DogController>().zoneMin.y, 
			              						controller.go.GetComponent<DogController>().zoneMax.y), 
			                      Random.Range (controller.go.GetComponent<DogController>().zoneMin.z, 
			              						controller.go.GetComponent<DogController>().zoneMax.z));
			timeStart = Time.time;
			controller.go.GetComponent<Animator> ().Play ("WalkWithNoShake");

			direction = (vecEnd - vecStart).normalized;
			timeTotal = (vecEnd - vecStart).magnitude / controller.go.GetComponent<DogController>().walkSpeed;
			dstEulerY = Quaternion.LookRotation (direction).eulerAngles.y;
			velEulerY = 0.0f;
		}

		public override void Update()
		{
			Vector3 curEuler = controller.go.transform.rotation.eulerAngles;
			if (curEuler.y != dstEulerY) {
				curEuler.y = Mathf.SmoothDampAngle(curEuler.y, dstEulerY, ref velEulerY, eulerSmooth);
				controller.go.transform.rotation = Quaternion.Euler(curEuler);
			}
			controller.go.transform.position = vecStart + direction * controller.go.GetComponent<DogController>().walkSpeed * (Time.time - timeStart);
		}

		public override bool IsFinished()
		{
			return Time.time > timeStart + timeTotal ? true : false;
		}

		public override RobotState GetNextState()
		{
			var states = new RobotState[]{RobotState.Sleep, RobotState.Run, RobotState.Sniffer};
			return states [ (int)(Random.value * 10.0f) % states.Length ];
		}
	}

	public class RobotAIWalkWithLargeShake : RobotAI
	{
		private Vector3 vecStart;
		private Vector3 vecEnd;
		private float timeStart;
		
		private Vector3 direction;
		private float timeTotal;
		private float dstEulerY;
		private float eulerSmooth = 0.2f;
		private float velEulerY;
		
		public override void Start(RobotController ctrl)
		{
			controller = ctrl;
			vecStart = controller.go.transform.position;
			vecEnd = new Vector3 (Random.Range (controller.go.GetComponent<DogController>().zoneMin.x, 
			                                    controller.go.GetComponent<DogController>().zoneMax.x), 
			                      Random.Range (controller.go.GetComponent<DogController>().zoneMin.y, 
			              controller.go.GetComponent<DogController>().zoneMax.y), 
			                      Random.Range (controller.go.GetComponent<DogController>().zoneMin.z, 
			              controller.go.GetComponent<DogController>().zoneMax.z));
			timeStart = Time.time;
			controller.go.GetComponent<Animator> ().Play ("WalkWithLargeShake");
			
			direction = (vecEnd - vecStart).normalized;
			timeTotal = (vecEnd - vecStart).magnitude / controller.go.GetComponent<DogController>().walkSpeed;
			dstEulerY = Quaternion.LookRotation (direction).eulerAngles.y;
			velEulerY = 0.0f;
		}
		
		public override void Update()
		{
			Vector3 curEuler = controller.go.transform.rotation.eulerAngles;
			if (curEuler.y != dstEulerY) {
				curEuler.y = Mathf.SmoothDampAngle(curEuler.y, dstEulerY, ref velEulerY, eulerSmooth);
				controller.go.transform.rotation = Quaternion.Euler(curEuler);
			}
			controller.go.transform.position = vecStart + direction * controller.go.GetComponent<DogController>().walkSpeed * (Time.time - timeStart);
		}
		
		public override bool IsFinished()
		{
			return Time.time > timeStart + timeTotal ? true : false;
		}
		
		public override RobotState GetNextState()
		{
			var states = new RobotState[]{RobotState.WalkWithNoShake, RobotState.Run};
			return states [ (int)(Random.value * 10.0f) % states.Length ];
		}
	}


	public class RobotAIRun : RobotAI
	{
		private Vector3 vecStart;
		private Vector3 vecEnd;
		private float timeStart;
		
		private Vector3 direction;
		private float timeTotal;
		private float dstEulerY;
		private float eulerSmooth = 0.2f;
		private float velEulerY;

		public override void Start(RobotController ctrl)
		{
			controller = ctrl;
			vecStart = controller.go.transform.position;
			vecEnd = new Vector3 (Random.Range (controller.go.GetComponent<DogController>().zoneMin.x, 
			                                    controller.go.GetComponent<DogController>().zoneMax.x), 
			                      Random.Range (controller.go.GetComponent<DogController>().zoneMin.y, 
						     		            controller.go.GetComponent<DogController>().zoneMax.y), 
			                      Random.Range (controller.go.GetComponent<DogController>().zoneMin.z, 
			              						controller.go.GetComponent<DogController>().zoneMax.z));
			timeStart = Time.time;
			controller.go.GetComponent<Animator> ().Play ("Run");
			
			direction = (vecEnd - vecStart).normalized;
			timeTotal = (vecEnd - vecStart).magnitude / controller.go.GetComponent<DogController>().runSpeed;
			dstEulerY = Quaternion.LookRotation (direction).eulerAngles.y;
			velEulerY = 0.0f;
		}
		
		public override void Update()
		{
			Vector3 curEuler = controller.go.transform.rotation.eulerAngles;
			if (curEuler.y != dstEulerY) {
				curEuler.y = Mathf.SmoothDampAngle(curEuler.y, dstEulerY, ref velEulerY, eulerSmooth);
				controller.go.transform.rotation = Quaternion.Euler(curEuler);
			}
			controller.go.transform.position = vecStart + direction * controller.go.GetComponent<DogController>().runSpeed * (Time.time - timeStart);
		}
		
		public override bool IsFinished()
		{
			return Time.time > timeStart + timeTotal ? true : false;
		}
		
		public override RobotState GetNextState()
		{
			var states = new RobotState[]{RobotState.Run, RobotState.WalkWithLargeShake};
			return states [ (int)(Random.value * 10.0f) % states.Length ];
		}
	}
	
	public enum RobotState
	{ 
		Sleep,
		Wake,
		WalkWithNoShake,
		WalkWithLargeShake,
		Run,
		Sniffer,
	}

	
	public RobotState State;


	private RobotAI lastAI;
	private GameObject go;

	// Use this for initialization
	void Start () {
		go = GameObject.FindGameObjectWithTag("dog");
		ChangeRobotAI ();
	}
	
	// Update is called once per frame
	void Update () {
		lastAI.Update ();
		if (lastAI.IsFinished ()) 
		{
			ChangeRobotAI();
		}
	}

	void ChangeRobotAI()
	{
		RobotState state = RobotState.WalkWithNoShake;
		if (lastAI != null)
			state = lastAI.GetNextState ();

		RobotAI ai;
		switch (state) {
		case RobotState.Sleep:
			ai = new RobotAISleep();
			break;
		case RobotState.Wake:
			ai = new RobotAIWake();
			break;
		case RobotState.WalkWithNoShake:
			ai = new RobotAIWalkWithNoShake();
			break;
		case RobotState.WalkWithLargeShake:
			ai = new RobotAIWalkWithLargeShake();
		break;
		case RobotState.Run:
			ai = new RobotAIRun();
			break;
		case RobotState.Sniffer:
			ai = new RobotAIWalkWithNoShake();
			break;
		default:
			ai = new RobotAIWalkWithNoShake();
			break;
		}

		if(lastAI != null)
			lastAI.Finish ();

		ai.Start(this);
		lastAI = ai;
	}
}
