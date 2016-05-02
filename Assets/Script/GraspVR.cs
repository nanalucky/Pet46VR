using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;
using System;

public class GraspVR : CrosshairHand {

	public enum State
	{
		Touch,
		Grasp,
		GraspFade,
		None,
	}


	public State state;
	public string partName;
	public string[] boneNames;
	public string rootName;
	public float touchTime = 1.0f;
	public Vector3 minOffset = new Vector3 (0.0f, 0.0f, 0.0f);
	public Vector3 maxOffset = new Vector3 (0.0f, 0.15f, 0.15f);
	public float smooth = 0.1f;
	public float graspFarthestDistanceMultiply = 1.5f;

	public Color colorTouch = Color.white;
	public Color colorNotTouch = Color.red;
	private Sprite spriteTouch;
	private Sprite spriteNotTouch;

	private GameObject goDog;
	private GameObject go;
	private Collider co;
	private SkinnedCollisionHelper skinHelper;
	private GameObject goCrosshairTouch;
	private GameObject goPointer;

	private LimbIK limbIK;
	private float lastTouchTime;
	private Vector3 firstPosition;
	private float velPosition;
	private float velRotation;

	private bool lastInTouch = false;
	private Interact interact;

	// Use this for initialization
	void Start () {
		goDog = GameObject.FindGameObjectWithTag ("dog");
		go = GameObject.Find (partName);
		go.AddComponent<MeshCollider> ();
		skinHelper = go.AddComponent<SkinnedCollisionHelper> ();
		skinHelper.updateOncePerFrame = false;
		co = go.GetComponent<MeshCollider> ();
		goPointer = goDog.GetComponent<DogController> ().goPointer;
		goCrosshairTouch = goDog.GetComponent<DogController> ().goCrosshairTouch;
		interact = FindObjectOfType (typeof(Interact)) as Interact;

		state = State.None;
		limbIK = go.AddComponent<LimbIK> ();
		limbIK.solver.SetChain (GameObject.Find (boneNames [0]).transform, 
		                        GameObject.Find (boneNames [1]).transform, 
		                        GameObject.Find (boneNames [2]).transform, 
		                        GameObject.Find (rootName).transform);
		limbIK.solver.IKPositionWeight = 0.0f;
		limbIK.solver.IKRotationWeight = 0.0f;

		spriteTouch = Resources.Load<Sprite> ("UI/vrpointer2");
		spriteNotTouch = Resources.Load<Sprite> ("UI/vrpointer1");
		SetCrosshair (false);
	}
	
	public virtual bool IsInState()
	{
		if (state == State.None)
			return false;
		return true;
	}

	void SetCrosshairColor(Color color)
	{
		SpriteRenderer sr = goCrosshairTouch.GetComponent<SpriteRenderer> ();
		sr.color = color;
	}

	void SetCrosshairPosition(Vector3 pos)
	{
		goCrosshairTouch.transform.position = pos;
	}

	void SetCrosshair(bool bTouch)
	{
		SpriteRenderer sr = goCrosshairTouch.GetComponent<SpriteRenderer> ();
		if (bTouch) {
			sr.sprite = spriteTouch;
		} else {
			sr.sprite = spriteNotTouch;
		}
	}

	// Update is called once per frame
	void Update () {
		Vector3 fwd = goPointer.transform.TransformDirection(Vector3.forward);
		Ray ray = new Ray (goPointer.transform.position, fwd);
		RaycastHit hit;
		bool ret;// = co.Raycast (ray, out hit, 100.0f) && Input.GetMouseButton(0);
		switch (state) {
		case State.None:
			ret = false;
			if(!lastInTouch)
			{
				skinHelper.UpdateCollisionMesh();
				ret = co.Raycast (ray, out hit, 100.0f);
				lastInTouch = ret;

				if(ret)
					SetCrosshairPosition(hit.point - fwd * 0.05f);
			}

			if(ret)
			{
				lastTouchTime = Time.time;
				state = State.Touch;
				lastInTouch = false;
				interact.DisableAllCrosshairHandButThis(this);
			}

			if(ret)
				SetCrosshair(true);
			else
				SetCrosshair(false);
			break;
		case State.Touch:
			ret = false;
			skinHelper.UpdateCollisionMesh();
			ret = co.Raycast (ray, out hit, 100.0f);

			if(ret)
			{
				SetCrosshairPosition(hit.point - fwd * 0.05f);

				if(Time.time - lastTouchTime >= touchTime)
				{
					state = State.Grasp;
					limbIK.solver.IKPositionWeight = 1.0f;
					limbIK.solver.IKRotationWeight = 0.5f;
					limbIK.solver.IKPosition = hit.point;
					firstPosition = hit.point;
				}
			}
			else
			{
				state = State.None;
				interact.EnableAllCrosshairHand();
				SetCrosshair(false);
			}
			break;
		case State.Grasp:
			ret = false;
			skinHelper.UpdateCollisionMesh();
			ret = co.Raycast (ray, out hit, 100.0f);

			if(ret)
				SetCrosshairPosition(hit.point - fwd * 0.05f);

			Ray rayCur = ray;
			Vector3 posCur = PetHelper.ProjectPointLine(limbIK.solver.IKPosition, rayCur.GetPoint(0), rayCur.GetPoint(100));
			Vector3 firstInLocal = Quaternion.Inverse(goDog.transform.rotation) * firstPosition;
			Vector3 curInLocal = Quaternion.Inverse(goDog.transform.rotation) * posCur;
			if((curInLocal - firstInLocal).magnitude > (maxOffset - minOffset).magnitude * graspFarthestDistanceMultiply)
			{
				state = State.GraspFade;
				velPosition = 0.0f;
				velRotation = 0.0f;
				SetCrosshair(false);
			}
			else
			{
				curInLocal.x = Mathf.Clamp(curInLocal.x, firstInLocal.x + minOffset.x, firstInLocal.x + maxOffset.x);
				curInLocal.y = Mathf.Clamp(curInLocal.y, firstInLocal.y + minOffset.y, firstInLocal.y + maxOffset.y);
				curInLocal.z = Mathf.Clamp(curInLocal.z, firstInLocal.z + minOffset.z, firstInLocal.z + maxOffset.z);
				Vector3 curInWorld = goDog.transform.rotation * curInLocal;
				limbIK.solver.IKPosition = curInWorld;
			}
			break;
		case State.GraspFade:
			if (limbIK.solver.IKPositionWeight != 0.0f)
				limbIK.solver.IKPositionWeight = Mathf.SmoothDamp(limbIK.solver.IKPositionWeight, 0.0f, ref velPosition, smooth);
			if (limbIK.solver.IKRotationWeight != 0.0f)
				limbIK.solver.IKRotationWeight = Mathf.SmoothDamp(limbIK.solver.IKRotationWeight, 0.0f, ref velRotation, smooth);
			if (limbIK.solver.IKPositionWeight <= 0.01f && limbIK.solver.IKRotationWeight <= 0.01f)
			{
				limbIK.solver.IKPositionWeight = 0.0f;
				limbIK.solver.IKRotationWeight = 0.0f;
				state = State.None;
				interact.EnableAllCrosshairHand();
			}
			break;
		}
	}

	void OnDestroy() {
		go = GameObject.Find (partName);
		if (go != null) {
			Destroy (go.GetComponent<MeshCollider> ());
			Destroy (go.GetComponent<SkinnedCollisionHelper> ());
			Destroy (go.GetComponent<LimbIK> ());
		}
	}
}
