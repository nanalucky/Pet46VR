#pragma strict

//
//	Fur Force 
//
// todo : making local and global always at mag 1

var smoothing : float = 10;
private var forceSmoothed : Vector3;
private var forceLocalSmoothed : Vector3;

var addRigidbodyForce : boolean = false;
var rigidbodyForceFactor : float = 1;

var addGravityToForce : boolean = false;
var gravityFactor : float = 0.1;
var addWindForce : boolean = false;
var windForceFactor : Vector3 = Vector3(1,1,1);
var windForceSpeed : float = 1;

private var perlinrandom : Vector3;
private var localDir : Vector3;

function Awake()
{
	//generating perlin random
	perlinrandom = Vector3(Random.Range(0.0f, 65535.0f),Random.Range(0.0f, 65535.0f),Random.Range(0.0f, 65535.0f));
	
	//get starting local direction for fur to calculate normalized vector with global direction
	//localDir = Vector3.ClampMagnitude(renderer.material.GetVector("_ForceLocal"),1.0);
	
}

function Update () {

var sumForce : Vector3 = Vector3.zero;
var newLocalForce : Vector3 = Vector3.zero;

//windForceFactor = Vector3.ClampMagnitude(windForceFactor,1.0);

//adding force based on rigidbody velocity
if (addRigidbodyForce) {
	if (rigidbody) {
		sumForce = -rigidbody.velocity * rigidbodyForceFactor;
	}
}

//add gravity force
if (addGravityToForce) sumForce += Physics.gravity * gravityFactor;

//add wind force
if (addWindForce) sumForce += Vector3.Scale(Noise(windForceSpeed), windForceFactor);

//clamping force magnitude to 1.0
sumForce = Vector3.ClampMagnitude(sumForce,1.0);

//check magnitude of both local and global vectors
/*
Debug.Log(sumForce + " " + localDir);
var mag = Vector3.Magnitude(sumForce+localDir);
if (mag>1) {
	sumForce = sumForce/mag;
	newLocalForce = localDir/mag;
}
if (mag==0) {
	sumForce = Vector3.zero;
	newLocalForce = Vector3.zero;
}

Debug.Log(mag + " " + sumForce + " " + newLocalForce);
Debug.Log("--");
*/

//smoothing force
forceSmoothed = Vector3.Lerp(forceSmoothed, sumForce,Time.deltaTime * smoothing);
//forceLocalSmoothed = Vector3.Lerp(forceLocalSmoothed, newLocalForce,Time.deltaTime * smoothing);

//pass global force for fur
renderer.material.SetVector("_ForceGlobal", forceSmoothed);
//renderer.material.SetVector("_ForceLocal", forceLocalSmoothed);


}

//noise function - using to generate smooth wind
function Noise(speed : float) : Vector3
{
    var noise = Mathf.PerlinNoise(perlinrandom.x, Time.time * speed);
    var x = Mathf.Lerp(-1, 1, noise);
    var noise2 = Mathf.PerlinNoise(perlinrandom.y, Time.time * speed);
    var y = Mathf.Lerp(-1, 1, noise2);
	var noise3 = Mathf.PerlinNoise(perlinrandom.z, Time.time * speed);
    var z = Mathf.Lerp(-1, 1, noise3); 
    
    return Vector3(x,y,z);

}