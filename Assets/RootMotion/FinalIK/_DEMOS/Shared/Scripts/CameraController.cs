using UnityEngine;
using System.Collections;

namespace RootMotion.FinalIK.Demos {

	/// <summary>
	/// 3rd person camera controller.
	/// </summary>
	public class CameraController : MonoBehaviour {
		
		public Transform target;
		
		public bool lockCursor = true;
		public float distance = 10.0f, minDistance = 4, maxDistance = 10, zoomSpeed = 10f, zoomSensitivity = 1f, 
		rotationSensitivity = 3.5f, yMinLimit = -20, yMaxLimit = 80;
		public Vector3 offset = new Vector3(0, 1.5f, 0.5f);
		public bool rotateAlways = true, rotateOnLeftButton, rotateOnRightButton, rotateOnMiddleButton;
		
		[HideInInspector] public float x, y, distanceTarget;
		
		private Vector3 targetDistance, position;
		private Quaternion rotation = Quaternion.identity;
		
		void Start () {
			Vector3 angles = transform.eulerAngles;
			x = angles.y;
			y = angles.x;
			
			distanceTarget = distance;
		}
		
		public void LateUpdate() {
			if (target == null || !camera.enabled) return;
			if (lockCursor) Screen.lockCursor = true;

			distanceTarget = Mathf.Clamp(distanceTarget + zoomAdd, minDistance, maxDistance);
			distance += (distanceTarget - distance) * zoomSpeed * Time.deltaTime;

			bool rotate = rotateAlways || (rotateOnLeftButton && Input.GetMouseButton(0)) || (rotateOnRightButton && Input.GetMouseButton(1)) || (rotateOnMiddleButton && Input.GetMouseButton(2));

			if (rotate) {
				x += Input.GetAxis("Mouse X") * rotationSensitivity;
				y = ClampAngle(y - Input.GetAxis("Mouse Y") * rotationSensitivity, yMinLimit, yMaxLimit);
			}

			rotation = Quaternion.AngleAxis(x, Vector3.up) * Quaternion.AngleAxis(y, Vector3.right);
			
			position = target.position + rotation * (offset - Vector3.forward * distance);
			
			transform.position = position;
			transform.rotation = rotation;
		}
		
		private float zoomAdd {
			get {
				float scrollAxis = Input.GetAxis("Mouse ScrollWheel");
				if (scrollAxis > 0) return -zoomSensitivity;
				if (scrollAxis < 0) return zoomSensitivity;
				return 0;
			}
		}
		
		private float ClampAngle (float angle, float min, float max) {
			if (angle < -360) angle += 360;
			if (angle > 360) angle -= 360;
			return Mathf.Clamp (angle, min, max);
		}
		
	}
}

