using UnityEngine;
using System.Collections;

public class BNG_NonVRWalk : MonoBehaviour
{

    private static FPSInputControllerForNoTouchGUI fpsRef;

	// Use this for initialization
	void Start () {

        fpsRef = transform.GetComponent<FPSInputControllerForNoTouchGUI>();
	}

    void ActivateAutoWalk()
    {
        fpsRef.autowalk = 1;
    }

    void DeactivateAutoWalk()
    {
        fpsRef.autowalk = 0;
    }
}
