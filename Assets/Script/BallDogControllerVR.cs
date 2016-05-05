using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class BallDogControllerVR : MonoBehaviour {

	public enum State
	{
		Look,
		Chase,
		Rush,
		back,
		Play,
	}

	public State state;
	public float rushDistance = 0.5f;
	public float rushSuccessRate = 1.0f;
	public float eulerSmooth = 0.2f;
	public float force = 10;
	public float turnEulerYSpeed = 480.0f;
	public float dogDestDistance = 1.0f;

	private GameObject goDog;
	private GameObject goBall;
	private GameObject goMouth;
	//private GameObject goDest;
	private LookAtIK lookatIK;
	private float timeEndPlay;
	private float velY;
	private float mouthDistance;

	[HideInInspector]
	public Vector3 dogDestPosition;
	[HideInInspector]
	public Vector3 dogDestEuler; 

	// Use this for initialization
	void Start () {
		goDog = GameObject.FindGameObjectWithTag ("dog");
		goBall = GameObject.FindGameObjectWithTag ("Ball");
		goMouth = GameObject.FindGameObjectWithTag ("Mouth");
		//goDest = GameObject.Find ("DogPosInteract");
		lookatIK = goDog.GetComponent<LookAtIK> ();

		state = State.Look;
		goDog.GetComponent<DogController> ().EnableLookatIK (true);
		Vector3 bodyToMouth = goMouth.transform.position - goDog.transform.position;
		bodyToMouth.y = 0.0f;
		mouthDistance = bodyToMouth.magnitude; 

		GameObject mainCamera = GameObject.Find("OVRCameraRig");
		dogDestPosition = mainCamera.transform.position + mainCamera.transform.TransformDirection(Vector3.forward) * dogDestDistance;
		dogDestPosition.y = goDog.transform.position.y;
		dogDestEuler = Quaternion.LookRotation (mainCamera.transform.TransformDirection (Vector3.back)).eulerAngles;
	}
	
	// Update is called once per frame
	void Update () {
		switch (state) {
		case State.Look:
			lookatIK.solver.IKPosition = goBall.transform.position;
			if(goBall.transform.position.y < 0.05f)
			{
				goDog.GetComponent<DogController> ().EnableLookatIK (false);
				state = State.Chase;
				goDog.GetComponent<Animator>().Play("Run");
				velY = 0.0f;

				gameObject.GetComponent<BallCamera>().enabled = false;
			}
			break;
		case State.Chase:
			Vector3 direction = goBall.transform.position - goDog.transform.position;
			direction.y = 0.0f;
			direction.Normalize();
			float distance = direction.magnitude;
			distance -= rushDistance + mouthDistance;
			if (distance < 0)
				distance = 0;
			float delta = goDog.GetComponent<DogController>().runSpeed * Time.deltaTime;
			if(delta > distance)
				delta = distance;
			goDog.transform.position = goDog.transform.position + direction * delta;

			direction = goBall.transform.position - goDog.transform.position;
			direction.y = 0.0f;
			distance = direction.magnitude;
			direction.Normalize();

			Vector3 euler = goDog.transform.rotation.eulerAngles;
			euler.y = Mathf.SmoothDampAngle(euler.y, Quaternion.LookRotation(direction).eulerAngles.y, ref velY, eulerSmooth);
			goDog.transform.rotation = Quaternion.Euler(euler);

			if(distance <= rushDistance + mouthDistance)
			{
				goDog.transform.rotation = Quaternion.LookRotation(direction);
				state = State.Rush;
				goDog.GetComponent<Animator>().Play ("Rush");
			}
			break;
		case State.Rush:
			direction = goBall.transform.position - goDog.transform.position;
			direction.y = 0.0f;
			direction.Normalize();
			goDog.transform.position = goDog.transform.position + direction * goDog.GetComponent<DogController>().rushSpeed * Time.deltaTime;
			direction = goBall.transform.position - goDog.transform.position;
			direction.y = 0.0f;
			distance = direction.magnitude;
			direction.Normalize();
			if(distance <= mouthDistance)
			{
				if(Random.value <= rushSuccessRate)
				{
					goBall.GetComponent<Rigidbody>().isKinematic = true;
					goBall.transform.parent = goMouth.transform;
					goBall.transform.localPosition = Vector3.zero;
					//state = State.Play;
					//goDog.GetComponent<Animator>().Play("PlayBall");
					//timeEndPlay = 0.0f;
					state = State.back;
					goDog.GetComponent<Animator>().Play("RushToRun");
					velY = 0.0f;
				}
				else
				{
					goBall.transform.parent = null;
					Rigidbody rb = goBall.GetComponent<Rigidbody>();
					rb.isKinematic = false;
					rb.AddForce(goDog.transform.rotation * (new Vector3(0, 0, force)));
					goDog.GetComponent<Animator>().Play("RushToStand");
					this.enabled = false;
				}
			}
			break;
		case State.back:
			direction = dogDestPosition - goDog.transform.position;
			direction.y = 0.0f;
			direction.Normalize();
			distance = direction.magnitude;
			delta = goDog.GetComponent<DogController>().runSpeed * Time.deltaTime;
			if(delta > distance)
				delta = distance;
			goDog.transform.position = goDog.transform.position + direction * delta;
			
			direction = dogDestPosition - goDog.transform.position;
			direction.y = 0.0f;
			distance = direction.magnitude;
			direction.Normalize();
			
			if(distance < 0.05f)
			{
				goDog.transform.position = dogDestPosition;
				state = State.Play;
				goDog.GetComponent<Animator>().CrossFade("PlayBall", 0.25f);
				timeEndPlay = 0.0f;
			}
			else if(distance > 0.1f)
			{
				euler = goDog.transform.rotation.eulerAngles;
				float dstEulerY = Quaternion.LookRotation(direction).eulerAngles.y;
				if(Mathf.Abs(euler.y - dstEulerY) < 0.1f)
				{
					euler.y = dstEulerY;
				}
				else
				{
					euler.y = Mathf.SmoothDampAngle(euler.y, Quaternion.LookRotation(direction).eulerAngles.y, ref velY, eulerSmooth);
				}
				
				goDog.transform.rotation = Quaternion.Euler(euler);
			}
			break;
		case State.Play:
			if(timeEndPlay == 0.0f)
			{
				timeEndPlay = Time.time + goDog.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length * Random.Range(1,3);
			}
			else
			{
				if(Time.time >= timeEndPlay)
				{
					goBall.transform.parent = null;
					Rigidbody rb = goBall.GetComponent<Rigidbody>();
					rb.isKinematic = false;
					rb.AddForce(goDog.transform.rotation * (new Vector3(0, 0, force)));
					goDog.GetComponent<Animator>().CrossFade("RushToStand", 0.25f);
					this.enabled = false;
				}
			}
			break;
		}
	}

	void OnDestroy()
	{
		GameObject goDog = GameObject.FindGameObjectWithTag ("dog");
		if(goDog != null)
			goDog.GetComponent<DogController> ().EnableLookatIK (false);
	}
}
