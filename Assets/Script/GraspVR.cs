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

	public Color colorTouch = Color.white;
	public Color colorNotTouch = Color.red;

	private GameObject goDog;
	private GameObject go;
	private Collider co;
	private SkinnedCollisionHelper skinHelper;
	private GameObject goCrosshairTouch;

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

		SetCrosshairColor (colorNotTouch);
		OVRTouchpad.TouchHandler += LocalTouchEventCallback;
	}
	
	void SetCrosshairColor(Color color)
	{
		SpriteRenderer sr = goCrosshairTouch.GetComponent<SpriteRenderer> ();
		sr.color = color;
	}

	// Update is called once per frame
	void Update () {
		OVRTouchpad.Update();

		Vector3 fwd = goCrosshairTouch.transform.TransformDirection(Vector3.forward);
		Ray ray = new Ray (goCrosshairTouch.transform.position, fwd);
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
			}

			if(ret)
			{
				lastTouchTime = Time.time;
				state = State.Touch;
				lastInTouch = false;
				interact.DisableAllCrosshairHandButThis(this);
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

			if(ret)
			{
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
			}

			if(ret)
				SetCrosshairColor(colorTouch);
			else
				SetCrosshairColor(colorNotTouch);

			break;
		case State.Grasp:
			ret = false;
			skinHelper.UpdateCollisionMesh();
			ret = co.Raycast (ray, out hit, 100.0f);

			Ray rayCur = ray;
			Vector3 posCur = PetHelper.ProjectPointLine(limbIK.solver.IKPosition, rayCur.GetPoint(0), rayCur.GetPoint(100));
			Vector3 firstInLocal = Quaternion.Inverse(goDog.transform.rotation) * firstPosition;
			Vector3 curInLocal = Quaternion.Inverse(goDog.transform.rotation) * posCur;
			curInLocal.x = Mathf.Clamp(curInLocal.x, firstInLocal.x + minOffset.x, firstInLocal.x + maxOffset.x);
			curInLocal.y = Mathf.Clamp(curInLocal.y, firstInLocal.y + minOffset.y, firstInLocal.y + maxOffset.y);
			curInLocal.z = Mathf.Clamp(curInLocal.z, firstInLocal.z + minOffset.z, firstInLocal.z + maxOffset.z);
			Vector3 curInWorld = goDog.transform.rotation * curInLocal;
			limbIK.solver.IKPosition = curInWorld;

			if(ret)
				SetCrosshairColor(colorTouch);
			else
				SetCrosshairColor(colorNotTouch);
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

		OVRTouchpad.TouchHandler -= LocalTouchEventCallback;
	}

	void LocalTouchEventCallback(object sender, EventArgs args)
	{
		var touchArgs = (OVRTouchpad.TouchArgs)args;
		OVRTouchpad.TouchEvent touchEvent = touchArgs.TouchType;
		
		switch(touchEvent)
		{
		case OVRTouchpad.TouchEvent.SingleTap:
			//Debug.Log("SINGLE CLICK\n");
			if(state == State.Grasp)
			{
				state = State.GraspFade;
				velPosition = 0.0f;
				velRotation = 0.0f;
			}
			break;
			
		case OVRTouchpad.TouchEvent.Left:
			//Debug.Log("LEFT SWIPE\n");
			break;
			
		case OVRTouchpad.TouchEvent.Right:
			//Debug.Log("RIGHT SWIPE\n");
			break;
			
		case OVRTouchpad.TouchEvent.Up:
			//Debug.Log("UP SWIPE\n");
			break;
			
		case OVRTouchpad.TouchEvent.Down:
			//Debug.Log("DOWN SWIPE\n");
			break;
		}
	}
}
