/************************************************************************************

Filename    :   HomeMenu.cs
Content     :   An example of the required home/dashboard/back button menu
Created     :   June 30, 2014
Authors     :   Andrew Welch

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.


************************************************************************************/

using UnityEngine;
using System.Collections;

public class NoTouchGUI : MonoBehaviour
{

	public OVRCameraRig			cameraController = null;
	public float				distanceFromViewer = 0.0f;
	public float				doubleTapDelay = 0.25f;
	public float				longPressDelay = 0.75f;
	public GameObject			panel = null;

	private bool				isVisible = false;
	//private bool				homeButtonPressed = false;
	private float				homeButtonDownTime = 0.0f;

	/// <summary>
	/// Initialization
	/// </summary>
	void Awake()
	{
		if (cameraController == null)
		{
			Debug.LogError("ERROR: Missing camera controller reference on " + name);
			enabled = false;
			return;
		}
		if (panel == null)
		{
			Debug.LogError("ERROR: Missing panel reference on " + name);
			enabled = false;
			return;
		}
		// hide the menu to start
		ShowRenderers( false );
	}

	/// <summary>
	/// Shows and hides the menu
	/// </summary>
	public float Show(bool show)
	{
		if ((show && isVisible) || (!show && !isVisible))
		{
			return 0.0f;
		}
		if (show)
		{
			transform.position = cameraController.transform.position;
			transform.rotation = cameraController.transform.rotation;

			// refresh any children
			BroadcastMessage("OnRefresh", SendMessageOptions.DontRequireReceiver);
			// show the menu elements and play the animation
			ShowRenderers(true);
		}
		isVisible = show;
		//homeButtonPressed = false;
		homeButtonDownTime = 0.0f;
		// don't allow Show/Hide until this anim is done
		if (!isVisible)
		{
			// hide the renderers now that the hide anim is finished
			ShowRenderers(false);
		}

		return 0.0f;
	}

	/// <summary>
	/// Shows and hides the menu renderer elements
	/// </summary>
	void ShowRenderers(bool show)
	{
		panel.SetActive (show);
	}

	/// <summary>
	/// Processes input and handles menu interaction
	/// as per the Unity integration doc, the back button responds to "mouse 1" button down/up/etc
	/// </summary>
	void Update()
	{
		if (!isVisible)
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				CancelInvoke("DelayedShowMenu");
				if (Time.realtimeSinceStartup < (homeButtonDownTime + doubleTapDelay))
				{
					// reset so the menu doesn't pop up after resetting orientation
					homeButtonDownTime = 0f;
					// reset the HMT orientation
					//OVRManager.display.RecenterPose();
				}
				else
				{
					homeButtonDownTime = Time.realtimeSinceStartup;
				}
			}
			else if (Input.GetKey(KeyCode.Escape) && ((Time.realtimeSinceStartup - homeButtonDownTime) >= longPressDelay))
			{
				Debug.Log("[PlatformUI] Showing @ " + Time.time);
				// reset so something else doesn't trigger afterwards
				Input.ResetInputAxes();
				homeButtonDownTime = 0.0f;
				CancelInvoke("DelayedShowMenu");
#if UNITY_ANDROID && !UNITY_EDITOR
				// show the platform UI
				OVRPluginEvent.Issue(RenderEventType.PlatformUI);
#endif
			}
			else if (Input.GetKeyUp(KeyCode.Escape))
			{
				float elapsedTime = (Time.realtimeSinceStartup - homeButtonDownTime);
				if (elapsedTime < longPressDelay)
				{
					if (elapsedTime >= doubleTapDelay)
					{
						Show(true);
					}
					else
					{
						Invoke("DelayedShowMenu", (doubleTapDelay - elapsedTime));
					}
				}
			}
		}
		else
		{
			// menu is visible, check input
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				// back out of the menu
				Show(false);
			}
		}
	}

	void DelayedShowMenu()
	{
		Show(true);
	}

}
