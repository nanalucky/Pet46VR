using UnityEngine;
using System.Collections;
using RootMotion.FinalIK;

public class GraspTailVR : CrosshairHand {

	public enum State
	{
		Touch,
		Grasp,
		None,
	}

	public enum GraspMethod
	{
		ProjectPointLine,
	}
	
	
	public State state;
	public string partName;
	public string[] boneNames;
	public string rootName;
	public float touchTime = 1.0f;
	public int animatorLayer = 0;
	public string animName;
	public GraspMethod method = GraspMethod.ProjectPointLine;
	public float graspFarthestDistance = 0.15f;

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

	private CCDIK ccdIK;
	private RotationLimit[] rotationLimits;
	private float lastTouchTime;
	private Vector3 firstPosition;
	private float screenz;

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
		ccdIK = go.AddComponent<CCDIK> ();
		ccdIK.solver.SetChain (new Transform[]{GameObject.Find (boneNames [0]).transform, 
		                        GameObject.Find (boneNames [1]).transform, 
								GameObject.Find (boneNames [2]).transform}, 
		                        GameObject.Find (rootName).transform);
		ccdIK.solver.IKPositionWeight = 0.0f;
		rotationLimits = GameObject.Find (boneNames [0]).GetComponentsInChildren<RotationLimit> ();

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
					ccdIK.solver.IKPositionWeight = 1.0f;
					ccdIK.solver.IKPosition = ccdIK.solver.bones[ccdIK.solver.bones.Length - 1].transform.position;
					screenz = Camera.main.WorldToScreenPoint(ccdIK.solver.IKPosition).z;
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
			Vector3 posCur = PetHelper.ProjectPointLine(ccdIK.solver.IKPosition, rayCur.GetPoint(0), rayCur.GetPoint(100));
			if((posCur - firstPosition).magnitude > graspFarthestDistance)
			{
				state = State.None;
				interact.EnableAllCrosshairHand();
				SetCrosshair(false);
				ccdIK.solver.IKPositionWeight = 0.0f;
				if(!string.IsNullOrEmpty(animName))
				{
					goDog.GetComponent<Animator>().CrossFade(animName, 0.25f, animatorLayer);
				}
			}
			else
			{
				ccdIK.solver.IKPosition = posCur;
			}
			break;
		}
	}

	void LateUpdate()
	{
		foreach (RotationLimit rl in rotationLimits) {
			rl.Apply();
		}
	}
	
	void OnDestroy() {
		go = GameObject.Find (partName);
		if (go != null) {
			Destroy (go.GetComponent<MeshCollider> ());
			Destroy (go.GetComponent<SkinnedCollisionHelper> ());
			Destroy (go.GetComponent<CCDIK> ());
		}
	}

}
