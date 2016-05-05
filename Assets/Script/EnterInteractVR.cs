using UnityEngine;
using System.Collections;

// sit down & move camera & run to camera
public class EnterInteractVR : MonoBehaviour {

	public class AI
	{
		protected EnterInteractVR controller;
		
		public virtual void Start(EnterInteractVR ctrl){}
		public virtual void Update(){}
		public virtual bool IsFinished(){return true;}
		public virtual void Finish(){}
		public virtual AIState GetNextState(){return AIState.None;}
	}

	public class AIMoveCamera : AI
	{
		private GameObject mainCamera;

		public override void Start(EnterInteractVR ctrl)
		{
			controller = ctrl;
			mainCamera = GameObject.Find("OVRCameraRig");

			//mainCamera.transform.position = GameObject.Find ("CameraPosInteract").transform.position;
			//mainCamera.transform.rotation = GameObject.Find ("CameraPosInteract").transform.rotation;
		}

		public override bool IsFinished()
		{
			return true;
		}

		public override AIState GetNextState()
		{
			return AIState.StandUp;
		}
	}

	public class AIStandUp : AI
	{
		private GameObject go;
		private DogController dogController;
		private bool needWaitStand;
		
		public override void Start(EnterInteractVR ctrl)
		{
			controller = ctrl;
			go = GameObject.FindGameObjectWithTag ("dog");
			dogController = go.GetComponent<DogController> ();
			Animator animator = go.GetComponent<Animator> ();
			needWaitStand = false;
			if(animator.GetCurrentAnimatorStateInfo (0).IsName ("SitIdle") 
			   || animator.GetCurrentAnimatorStateInfo (0).IsName ("FallIdle")
			   || animator.GetCurrentAnimatorStateInfo (0).IsName ("SleepIdle"))
			{
				needWaitStand = true;
				dogController.StandUp();
			}
		}
		
		public override bool IsFinished ()
		{
			return !needWaitStand || dogController.IsStand();
		}
		
		public override AIState GetNextState()
		{
			return AIState.Turn;
		}
	}

	public class AITurn : AI
	{
		private GameObject go;
		//private GameObject goDest;

		private bool inMove = false;
		private float dstEulerY;

		public override void Start(EnterInteractVR ctrl)
		{
			controller = ctrl;
			go = GameObject.FindGameObjectWithTag ("dog");
			//goDest = GameObject.Find ("DogPosInteract");

			inMove = true;
			Vector3 direction = (ctrl.dogDestPosition - go.transform.position).normalized;
			if (direction != Vector3.zero)
				dstEulerY = Quaternion.LookRotation (direction).eulerAngles.y;
			else
				dstEulerY = go.transform.rotation.eulerAngles.y;

			if(Mathf.Abs(dstEulerY - go.transform.rotation.eulerAngles.y) < 0.1f)
			{
				inMove = false;
				go.transform.rotation = Quaternion.Euler(new Vector3(go.transform.rotation.eulerAngles.x, dstEulerY, go.transform.rotation.z));
			}
			else
			{
				go.GetComponent<Animator> ().Play ("WalkWithNoShake");
			}
		}

		public override void Update()
		{
			if (inMove) 
			{
				Vector3 euler = go.transform.rotation.eulerAngles;
				euler.y = Mathf.MoveTowardsAngle(euler.y, dstEulerY, Time.deltaTime * controller.turnEulerYSpeed);
				if(Mathf.Abs(dstEulerY - euler.y) < 0.1f)
				{
					inMove = false;
					euler.y = dstEulerY;
				}
				go.transform.rotation = Quaternion.Euler(euler);
			}
		}

		public override bool IsFinished ()
		{
			return !inMove;
		}

		public override AIState GetNextState()
		{
			return AIState.Stay;
		}
	}

	public class AIStay : AI
	{
		private GameObject go;
		//private GameObject goDest;
		private float endTime;

		public override void Start(EnterInteractVR ctrl)
		{
			controller = ctrl;
			go = GameObject.FindGameObjectWithTag ("dog");
			//goDest = GameObject.Find ("DogPosInteract");
			if(!go.GetComponent<DogController>().IsStand())
				go.GetComponent<Animator> ().CrossFade ("Stand", 0.25f);
			endTime = Time.time + ctrl.stayTime;
		}

		public override bool IsFinished()
		{
			if (!controller.aiMoveCamera.IsFinished ())
				return false;

			return endTime <= Time.time;
		}

		public override AIState GetNextState()
		{
			if (go.transform.position == controller.dogDestPosition)
				return AIState.Sit;

			return AIState.Run;
		}
	}

	public class AIRun : AI
	{
		private GameObject mainCamera;
		private GameObject go;
		//private GameObject goDest;

		private bool inMove;
		private Vector3 startPosition;
		private float time;

		public override void Start(EnterInteractVR ctrl)
		{
			controller = ctrl;
			mainCamera = GameObject.Find("OVRCameraRig");
			go = GameObject.FindGameObjectWithTag ("dog");
			//goDest = GameObject.Find("DogPosInteract");

			startPosition = go.transform.position;
			time = 0.0f;
			if ((controller.dogDestPosition - startPosition).magnitude <= 0.01f) 
			{
				inMove = false;
				go.transform.position = controller.dogDestPosition;			
			}
			else
			{
				inMove = true;
				go.GetComponent<Animator> ().Play("RunToCamera");
			}
		}

		public override void Update()
		{
			if (!inMove)
				return;

			time = time + Time.deltaTime;
			go.transform.position = startPosition + go.transform.rotation * (new Vector3 (0, 0, time * go.GetComponent<DogController> ().runSpeed));
			if ((go.transform.position - startPosition).magnitude > (controller.dogDestPosition - startPosition).magnitude) 
			{
				inMove = false;
				go.transform.position = controller.dogDestPosition;
			}

			//mainCamera.transform.rotation = Quaternion.LookRotation ((go.GetComponent<DogController>().GetDogPivot() - mainCamera.transform.position).normalized);
		}

		public override bool IsFinished()
		{
			return !inMove;
		}

		public override AIState GetNextState()
		{
			return AIState.Turn2;
		}
	}

	public class AITurn2 : AI
	{
		private GameObject go;
		//private GameObject goDest;
		
		private bool inMove = false;
		private float dstEulerY;
		
		public override void Start(EnterInteractVR ctrl)
		{
			controller = ctrl;
			go = GameObject.FindGameObjectWithTag ("dog");
			//goDest = GameObject.Find ("DogPosInteract");
			
			inMove = true;
			dstEulerY = controller.dogDestEuler.y;
			if(Mathf.Abs(dstEulerY - go.transform.rotation.eulerAngles.y) < 0.1f)
			{
				inMove = false;
				go.transform.rotation = Quaternion.Euler(new Vector3(go.transform.rotation.eulerAngles.x, dstEulerY, go.transform.rotation.z));
			}
			else
			{
				go.GetComponent<Animator> ().Play ("WalkWithNoShake");
			}
		}
		
		public override void Update()
		{
			if (inMove) 
			{
				Vector3 euler = go.transform.rotation.eulerAngles;
				euler.y = Mathf.MoveTowardsAngle(euler.y, dstEulerY, Time.deltaTime * controller.turnEulerYSpeed);
				if(Mathf.Abs(dstEulerY - euler.y) < 0.1f)
				{
					inMove = false;
					euler.y = dstEulerY;
				}
				go.transform.rotation = Quaternion.Euler(euler);
			}
		}
		
		public override bool IsFinished ()
		{
			return !inMove;
		}
		
		public override AIState GetNextState()
		{
			return AIState.Stay2;
		}
	}


	public class AIStay2 : AI
	{
		private GameObject go;
		private float endTime;
		
		public override void Start(EnterInteractVR ctrl)
		{
			controller = ctrl;
			go = GameObject.FindGameObjectWithTag ("dog");
			if(!go.GetComponent<DogController>().IsStand())
				go.GetComponent<Animator> ().CrossFade ("Stand", 0.25f);
			endTime = Time.time + ctrl.stay2Time;
		}
		
		public override bool IsFinished()
		{
			if (!controller.aiMoveCamera.IsFinished ())
				return false;
			
			return endTime <= Time.time;
		}
		
		static bool first = true;
		public override AIState GetNextState()
		{
			//var states = new AIState[]{AIState.Sit, AIState.Idle3};
			//return states [ Random.Range(0, states.Length) ];
			
			if (first)
			{
				first = false;
				return AIState.Idle3;
			}
			return AIState.Sit;
		}
	}

	public class AISit : AI
	{
		private GameObject go;

		public override void Start(EnterInteractVR ctrl)
		{
			controller = ctrl;
			go = GameObject.FindGameObjectWithTag ("dog");
			go.GetComponent<Animator> ().CrossFade ("SitDown", 0.25f);
		}

		public override bool IsFinished()
		{
			if (go.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("SitIdle"))
				return true;

			return false;
		}

		public override AIState GetNextState()
		{
			return AIState.None;
		}
	}

	public class AIIdle3 : AI
	{
		private GameObject go;
		
		public override void Start(EnterInteractVR ctrl)
		{
			controller = ctrl;
			go = GameObject.FindGameObjectWithTag ("dog");
			go.GetComponent<Animator> ().CrossFade ("Idle3", 0.25f);
		}
		
		public override bool IsFinished()
		{
			if (go.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName ("Idle1"))
				return true;
			
			return false;
		}
		
		public override AIState GetNextState()
		{
			return AIState.None;
		}
	}

	public enum AIState
	{
		MoveCamera,
		StandUp,
		Turn,
		Stay,
		Run,
		Turn2,
		Stay2,
		Sit,
		Idle3,
		None,
	}

	public float turnEulerYSpeed = 480.0f;
	public float stayTime = 1.0f;
	public float stay2Time = 0.5f;
	public float dogDestDistance = 1.0f;

	public AI aiMoveCamera;
	protected AI lastAI;

	[HideInInspector]
	public Vector3 dogDestPosition;
	[HideInInspector]
	public Vector3 dogDestEuler; 

	// Use this for initialization
	void Start () {
		aiMoveCamera = new AIMoveCamera ();
		aiMoveCamera.Start (this);
		lastAI = new AIStandUp ();
		lastAI.Start (this);

		GameObject mainCamera = GameObject.Find("OVRCameraRig");
		dogDestPosition = mainCamera.transform.position + mainCamera.transform.TransformDirection(Vector3.forward) * dogDestDistance;
		dogDestPosition.y = GameObject.FindGameObjectWithTag ("dog").transform.position.y;
		dogDestEuler = Quaternion.LookRotation (mainCamera.transform.TransformDirection (Vector3.back)).eulerAngles;
	}
	
	// Update is called once per frame
	void Update () {
		aiMoveCamera.Update ();
		lastAI.Update ();
		if (lastAI.IsFinished ()) 
		{
			ChangeAI();
		}
	}
	
	void ChangeAI()
	{
		AIState state = lastAI.GetNextState ();
		lastAI.Finish ();

		if (state == AIState.None)
		{
			Instantiate(Resources.Load("Prefabs/Interact"));
			Destroy(gameObject);
			return;
		}

		AI ai;
		switch (state) {
		case AIState.Turn:
			ai = new AITurn();
			break;
		case AIState.Stay:
			ai = new AIStay();
			break;
		case AIState.Run:
			ai = new AIRun();
		break;
		case AIState.Turn2:
			ai = new AITurn2();
		break;
		case AIState.Stay2:
			ai = new AIStay2();
		break;
		case AIState.Sit:
			ai = new AISit();
			break;
		case AIState.Idle3:
			ai = new AIIdle3();
		break;
		default:
			ai = new AISit();
			break;
		}

		ai.Start(this);
		lastAI = ai;
	}


}
