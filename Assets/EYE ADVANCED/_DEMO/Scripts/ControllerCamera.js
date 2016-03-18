#pragma strict

//PUBLIC VARIABLES
var setCamera : Transform;
var cameraTarget : Transform;
var followDistance : float = 5.0;
var followHeight : float = 1.0;
var followSensitivity : float = 2.0;
var useRaycast : boolean = true;
var axisSensitivity : Vector2 = Vector2(4.0,4.0);

var camFOV : float = 35.0;

var camRotation = 0.0;
var camHeight = 0.0;
var camYDamp : float;
var camLookOffset : Vector2 = Vector2(0.0,0.0);

//PRIVATE VARIABLES
//var orbitView : boolean = false;
private var targetPosition : Vector3;
private var targetRotation : Quaternion;
private var MouseRotationDistance : float = 0.0;
private var MouseVerticalDistance : float = 0.0;
private var MouseScrollDistance : float = 0.0;

private var isControllable : boolean = true;

private var playerObject : Transform;

private var camVRotation : float = 0.0;
private var CameraHeightDistance : float = 0.0;

private var castHit : boolean = false;




function Start () {

	targetPosition = cameraTarget.position;
	targetRotation = cameraTarget.rotation;

	camLookOffset.x = cameraTarget.transform.localPosition.x;
	camLookOffset.y = cameraTarget.transform.localPosition.y;
}




function LateUpdate(){
	
	
	if (setCamera == null) setCamera = Camera.main.transform;

	//CHECK FOR MOUSE INPUT
	targetPosition = cameraTarget.position;
	var oldMouseRotation = MouseRotationDistance;
	var oldMouseVRotation = MouseVerticalDistance;
	
	//orbitView = true;
	isControllable = true;

	if (Input.mousePosition.x > 365 && Input.mousePosition.y < 648 && Input.mousePosition.y > 50){

		if (Input.GetMouseButton(0)){
			MouseRotationDistance = Input.GetAxisRaw("Mouse X")*2.7;
			MouseVerticalDistance = Input.GetAxisRaw("Mouse Y")*2.7;
		} else {
			MouseRotationDistance = 0.0;
			MouseVerticalDistance = 0.0;
		}
		
		MouseScrollDistance = Input.GetAxisRaw("Mouse ScrollWheel");
		
		if (Input.GetMouseButton(2)){
			camLookOffset.x += Input.GetAxisRaw("Mouse X")*0.001;
			camLookOffset.y += Input.GetAxisRaw("Mouse Y")*0.001;
		}

	} else {
		MouseRotationDistance = 0.0;
		MouseVerticalDistance = 0.0;
	}

	//GET CHARACTER
	//cameraTarget = playerObject;


	//CHECK GAME MODES
	isControllable = false;
	Screen.lockCursor = false;
	followHeight = 1.5;



//rotate target
cameraTarget.transform.eulerAngles.y -= MouseRotationDistance;
cameraTarget.transform.eulerAngles.x -= MouseVerticalDistance;

//move target
cameraTarget.transform.localPosition.y = camLookOffset.y;
cameraTarget.transform.localPosition.x = camLookOffset.x;

//zoom camera
setCamera.localPosition.z = Mathf.Clamp(setCamera.localPosition.z,-9.73,-9.66);
if (setCamera.localPosition.z >= -9.73 && setCamera.localPosition.z <= -9.66){
	if (MouseScrollDistance != 0.0){
		setCamera.transform.Translate(-Vector3.forward*MouseScrollDistance*0.02,transform);
	}
}


/*

	camRotation = Mathf.Lerp(oldMouseRotation,MouseRotationDistance,Time.deltaTime*axisSensitivity.x);
	

	
	camHeight = Mathf.Lerp(camHeight,camHeight+MouseVerticalDistance,Time.deltaTime*axisSensitivity.y);
	camHeight = Mathf.Clamp(camHeight,0.1,8.0);
	
	camHeight += CameraHeightDistance;
	
	//set camera to follow target object
	var followPos : Vector3 = targetPosition;
	followPos.y = targetPosition.y + followHeight;
	
	//rotate figure based on mouse input
	//cameraTarget.transform.eulerAngles.y -= camRotation;

	//set camera distance
	setCamera.transform.position = targetPosition;
	setCamera.transform.position.y += camHeight;//followPos.y;

	//Zoom Camera
	setCamera.transform.Translate(-Vector3.forward*followDistance,transform);
	setCamera.transform.eulerAngles.z += Mathf.Sin(50.0*(Time.deltaTime*0.1));
	setCamera.transform.LookAt(followPos);
	
	//camLookOffset.y = Mathf.Clamp(camLookOffset.y,-2.0,2.0);
	setCamera.transform.position.y += camLookOffset.y;
	setCamera.transform.Translate(Vector3.left * (camLookOffset.x));
	
	//set camera settings
	setCamera.GetComponent.<Camera>().fieldOfView = camFOV;

	//MouseRotationDistance = Input.GetAxisRaw("Mouse X");
	//MouseVerticalDistance = Input.GetAxisRaw("Mouse Y");
*/
}
