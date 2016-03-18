using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class BallDogController : MonoBehaviour {

	public enum State
	{
		Look,
		Chase,
		Rush,
		Play,
	}

	public State state;
	public float rushDistance = 0.5f;
	public float rushSuccessRate = 0.8f;
	public float eulerSmooth = 0.2f;
	public float force = 10;

	private GameObject goDog;
	private GameObject goBall;
	private GameObject goMouth;
	private LookAtIK lookatIK;
	private float timeEndPlay;
	private float velY;
	private float mouthDistance;

	// Use this for initialization
	void Start () {
		goDog = GameObject.FindGameObjectWithTag ("dog");
		goBall = GameObject.FindGameObjectWithTag ("Ball");
		goMouth = GameObject.FindGameObjectWithTag ("Mouth");
		lookatIK = goDog.GetComponent<LookAtIK> ();

		state = State.Look;
		goDog.GetComponent<DogController> ().EnableLookatIK (true);
		Vector3 bodyToMouth = goMouth.transform.position - goDog.transform.position;
		bodyToMouth.y = 0.0f;
		mouthDistance = bodyToMouth.magnitude; 
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
					state = State.Play;
					goDog.GetComponent<Animator>().Play("PlayBall");
					timeEndPlay = 0.0f;
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
					goDog.GetComponent<Animator>().Play("RushToStand");
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
