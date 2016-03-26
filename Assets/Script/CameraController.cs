using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Runtime.InteropServices;		// required for DllImport


public class CameraController : MonoBehaviour {

	public Canvas canvas;

	#if (UNITY_ANDROID && !UNITY_EDITOR)
	[DllImport("OculusPlugin")]
	// Support to fix 60/30/20 FPS frame rate for consistency or power savings
	private static extern void OVR_TW_SetMinimumVsyncs( OVRTimeWarpUtils.VsyncMode mode );
	#endif	// Use this for initialization


	void Start () {
#if (UNITY_ANDROID && !UNITY_EDITOR)
		// delay one frame because OVRCameraController initializes TimeWarp in Start()
		Invoke("UpdateVSyncMode", 0.01666f);
#endif
#if (UNITY_ANDROID && !UNITY_EDITOR)
		OVR_TW_SetMinimumVsyncs(OVRTimeWarpUtils.VsyncMode.VSYNC_60FPS);
#endif	
	}
	
	// Update is called once per frame
	void Update () {
		if(GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("empty"))
		{
			canvas.gameObject.SetActive(true);
			GetComponent<Animator>().enabled = false;
			this.enabled = false;
		}
	}
}
