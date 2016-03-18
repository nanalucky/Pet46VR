using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class ThrowBall : MonoBehaviour {

	public enum State
	{
		Grasp,
		None,
	}

	public State state = State.None;
	public float forceMultiplier = 50.0f;
	public float forceThreshold = 0;

	private GameObject go;
	private Collider co;
	private Rigidbody rb;
	private Vector3 startPosition;
	private Vector3 lastPosition;
	private float screenz;

	private GameObject goDog;
	private LookAtIK lookatIK;

	private bool firstFrame = true;

	// Use this for initialization
	void Start () {
		Time.fixedDeltaTime = 0.01f;
	}
	
	// Update is called once per frame
	void Update () {
		if (firstFrame) {
			firstFrame = false;
			go = GameObject.FindGameObjectWithTag ("Ball");
			co = go.GetComponent<SphereCollider> ();
			rb = go.GetComponent<Rigidbody> ();
			startPosition = go.transform.position;
			screenz = Camera.main.WorldToScreenPoint(go.transform.position).z;

			goDog = GameObject.FindGameObjectWithTag("dog");
			lookatIK = goDog.GetComponent<LookAtIK>();
			goDog.GetComponent<DogController>().EnableLookatIK(true);
		}

		lookatIK.solver.IKPosition = go.transform.position;

		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		bool ret = co.Raycast (ray, out hit, 100.0f) && Input.GetMouseButton(0);

		switch(state)
		{
		case State.None:
			if(ret)
			{
				state = State.Grasp;
				go.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenz));
				lastPosition = go.transform.position;
			}
			break;
		case State.Grasp:
			if(Input.GetMouseButton(0))
			{
				go.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenz));
				lastPosition = go.transform.position;
			}
			else
			{
				Vector3 curPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenz));
				Vector3 force = curPosition - lastPosition;
				force += Camera.main.transform.rotation * (new Vector3(0,0,force.magnitude));
				force /= Time.deltaTime;
				force *= forceMultiplier;
				if(force.magnitude < forceThreshold)
				{
					state = State.None;
					go.transform.position = startPosition;
				}
				else
				{
					rb.isKinematic = false;
					rb.AddForce(force);
					this.gameObject.GetComponent<BallCamera>().enabled = true;
					this.gameObject.GetComponent<BallDogController>().enabled = true;					
					this.enabled = false;
				}
			}
			break;
		}
	}
}
