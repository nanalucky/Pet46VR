#pragma strict

//PUBLIC VARIABLES
var sceneLightObject : Transform;
var lightDirection : float = 0.25;
var lightIntensity : float = 0.75;

var targetObject : Transform;
var autoDilate : boolean = false;
var lodLevel : float = 0.15;
var parallaxAmt : float = 1.0;
var pupilDilation : float = 0.5;
var scleraSize : float = 0.0;
var irisSize : float = 0.22;
var irisColor : Color = Color(1,1,1,1);
var scleraColor : Color = Color(1,1,1,1);

var irisTexture : int = 0;
var irisTextures : Texture[];

var texTitle : Texture2D;
var texTD : Texture2D;
var texDiv1 : Texture2D;
var texSlideA : Texture2D;
var texSlideB : Texture2D;
var texSlideD : Texture2D;

var lodLevel0 : Transform;
var lodLevel1 : Transform;
var lodLevel2 : Transform;


//PRIVATE VARIABLES
private var camControl : ControllerCamera;
private var currentLodLevel : float = 0.0;
private var doLodSwitch : float = -1.0;
private var lodRot : Vector3;

private var sceneLight : Light;
private var targetRenderer : Renderer;
private var lightAngle : float;
private var ambientFac : float;
private var currTargetDilation : float = -2.0;
private var targetDilation : float = -1.0;
private var dilateTime : float = 0.0;

private var irisTextureF : float = 0.0;
private var irisTextureD : float = 0.0;

private var colorGold = Color(0.79,0.55,0.054,1.0);
private var colorGrey = Color(0.333,0.3,0.278,1.0);



function Start () {

	lodLevel0.gameObject.SetActive(true);
	lodLevel1.gameObject.SetActive(false);
	lodLevel2.gameObject.SetActive(false);

	if (sceneLightObject != null){
		sceneLight = sceneLightObject.GetComponent(Light);
	}

	if (targetObject != null){
		targetRenderer = targetObject.transform.GetComponent(Renderer) as Renderer;
	}

	camControl = gameObject.GetComponent(ControllerCamera) as ControllerCamera;

}



function Update(){


	//set scene light
	lightIntensity = Mathf.Clamp(lightIntensity,0.0,1.0);
	lightDirection = Mathf.Clamp(lightDirection,0.0,1.0);
	sceneLightObject.transform.eulerAngles.y = Mathf.Lerp(0.0,359.0,lightDirection);
	sceneLight.intensity = lightIntensity;

	//handle auto dilation
	if (autoDilate && sceneLightObject != null){

		//calculate look angle
		lightAngle = Vector3.Angle(sceneLightObject.transform.forward,targetObject.transform.forward) / 180.0;
		targetDilation = Mathf.Lerp(1.0,0.0,lightAngle * sceneLight.intensity);

		//handle dilation
		if (currTargetDilation != targetDilation){
			currTargetDilation = targetDilation;
			dilateTime -= Time.deltaTime*2.0;
		}
		if (pupilDilation != targetDilation){
			dilateTime += Time.deltaTime;
			pupilDilation = Mathf.SmoothStep(pupilDilation,targetDilation,dilateTime*0.05);
		} else {
			dilateTime = 0.0;
		}
		
	}


	//clamp values
	irisSize = Mathf.Clamp(irisSize,0.0,1.0);
	parallaxAmt = Mathf.Clamp(parallaxAmt,0.0,1.0);
	pupilDilation = Mathf.Clamp(pupilDilation,0.0,1.0);
	scleraSize = Mathf.Clamp(scleraSize,0.0,1.0);
	irisTextureF = Mathf.Clamp(Mathf.FloorToInt(irisTextureF),0,irisTextures.length-1);
	irisTextureD = irisTextureF/(irisTextures.length-1);
	irisTexture = Mathf.Clamp(Mathf.FloorToInt(irisTextureF),0,irisTextures.length-1);


	//set shader values
	if (targetRenderer != null){

		targetRenderer.material.SetFloat("_irisSize",Mathf.Lerp(1.5,5.0,irisSize));
		targetRenderer.material.SetFloat("_parallax",Mathf.Lerp(0.0,0.05,parallaxAmt));
		targetRenderer.material.SetFloat("_pupilSize",pupilDilation);	
		targetRenderer.material.SetFloat("_scleraSize",Mathf.Lerp(1.1,2.2,scleraSize));	
		targetRenderer.material.SetColor("_irisColor",irisColor);
		targetRenderer.material.SetColor("_scleraColor",scleraColor);	
		targetRenderer.material.SetTexture("_IrisColorTex",irisTextures[irisTexture]);	

	}

	//check and switch LOD level
	if (currentLodLevel != lodLevel){

		doLodSwitch = -1.0;
		if (lodLevel < 0.31 && currentLodLevel > 0.31) doLodSwitch = 0.0;
		if (lodLevel > 0.70 && currentLodLevel > 0.70) doLodSwitch = 2.0;
		if (lodLevel > 0.31 && lodLevel < 0.70){
			if (currentLodLevel < 0.31 || currentLodLevel > 0.70){
				doLodSwitch = 1.0;
			}
		}

		currentLodLevel = lodLevel;
		lodRot = targetObject.transform.eulerAngles;
		if (doLodSwitch >= 0.0){
			if (doLodSwitch == 0.0 && lodLevel0 != null){
				lodLevel0.gameObject.SetActive(true);
				lodLevel1.gameObject.SetActive(false);
				lodLevel2.gameObject.SetActive(false);
				targetObject = lodLevel0;
			}
			if (doLodSwitch == 1.0 && lodLevel1 != null){
				lodLevel0.gameObject.SetActive(false);
				lodLevel1.gameObject.SetActive(true);
				lodLevel2.gameObject.SetActive(false);
				targetObject = lodLevel1;
			}
			if (doLodSwitch == 2.0 && lodLevel2 != null){
				lodLevel0.gameObject.SetActive(false);
				lodLevel1.gameObject.SetActive(false);
				lodLevel2.gameObject.SetActive(true);
				targetObject = lodLevel2;
			}
			if (targetObject != null){
				targetRenderer = targetObject.transform.GetComponent(Renderer) as Renderer;
			}
			targetObject.transform.eulerAngles = lodRot;
			camControl.cameraTarget = targetObject;
		}
	}


}











function OnGUI(){


	//Main Title
	GUI.color = Color(1.0,1.0,1.0,1.0);
	if (texTitle != null) GUI.Label(Rect (25,25, texTitle.width,texTitle.height), texTitle);
	if (texTD != null) GUI.Label(Rect (800,45, texTD.width*2,texTD.height*2), texTD);


	//VIEW MODE
	GUI.color = Color(1.0,1.0,1.0,1.0);
	if (texDiv1 != null) GUI.Label(Rect (150,130, texDiv1.width,texDiv1.height), texDiv1);
	GUI.color = colorGold;
	GUI.Label (Rect (35, 128, 180, 20), "EYEBALL VIEW");
	GUI.color = colorGrey;
	GUI.Label (Rect (160, 128, 280, 20), "FIGURE VIEW (soon)");


	//SETTINGS - LOD 
	GenerateSlider("EYE LOD LEVEL",35,185,false,"lodLevel",293);
	GUI.color = Color(1.0,1.0,1.0,1.0);
	if (texDiv1 != null) GUI.Label(Rect (130,217, texDiv1.width,texDiv1.height), texDiv1);
	if (texDiv1 != null) GUI.Label(Rect (240,217, texDiv1.width,texDiv1.height), texDiv1);
	GUI.color = colorGold;
	if (lodLevel>0.32) GUI.color = colorGrey;
	GUI.Label (Rect(60, 215, 40, 20), "LOD 0");
	GUI.color = colorGold;
	if (lodLevel<0.32 || lodLevel>0.70) GUI.color = colorGrey;
	GUI.Label (Rect(165, 215, 50, 20), "LOD 1");
	GUI.color = colorGold;
	if (lodLevel<0.70) GUI.color = colorGrey;
	GUI.Label (Rect(270, 215, 50, 20), "LOD 2");

	//SETTINGS - Pupil Dilation
	GenerateSlider("PUPIL DILATION",35,248,true,"pupilDilation",293);
	GUI.color = Color(1.0,1.0,1.0,1.0);
	if (texDiv1 != null) GUI.Label(Rect (272,280, texDiv1.width,texDiv1.height), texDiv1);
	GUI.color = colorGold;
	if (!autoDilate) GUI.color = colorGrey;
	GUI.Label (Rect(240, 278, 40, 20), "auto");
	GUI.color = colorGold;
	if (autoDilate) GUI.color = colorGrey;
	GUI.Label (Rect(280, 278, 50, 20), "manual");
	if (Event.current.type == EventType.MouseUp && Rect(240,278,40,20).Contains(Event.current.mousePosition)) autoDilate = true;
	if (Event.current.type == EventType.MouseUp && Rect(280,278,50,20).Contains(Event.current.mousePosition)) autoDilate = false;


	//SETTINGS - Sclera Size
	GenerateSlider("SCLERA SIZE",35,310,true,"scleraSize",293);


	//SETTINGS - Iris Size
	GenerateSlider("IRIS SIZE",35,350,true,"irisSize",293);


	//SETTINGS - Iris Texture
	GenerateSlider("IRIS TEXTURE",35,390,false,"irisTexture",293);
	GUI.color = Color(1.0,1.0,1.0,1.0);
	for (var t : int = 0; t < irisTextures.length; t++){
		if (texDiv1 != null) GUI.Label(Rect (38+(t*26),416, texDiv1.width,texDiv1.height), texDiv1);
	}

	//SETTINGS - Iris Parallax
	GenerateSlider("IRIS PARALLAX",35,440,true,"irisParallax",293);


	// SETTINGS - Iris Color
	GUI.color = colorGold;
	GUI.Label (Rect (35,510, 180, 20), "IRIS COLOR");
	GUI.color = colorGrey;
	GUI.Label (Rect (35,525, 20, 20), "r");
	GUI.Label (Rect (35,538, 20, 20), "g");
	GUI.Label (Rect (35,551, 20, 20), "b");
	GUI.Label (Rect (35,564, 20, 20), "a");
	GenerateSlider("",50,512,false,"irisColorR",278);
	GenerateSlider("",50,525,false,"irisColorG",278);
	GenerateSlider("",50,538,false,"irisColorB",278);
	GenerateSlider("",50,550,false,"irisColorA",278);


	// SETTINGS - Sclera Color
	GUI.color = colorGold;
	GUI.Label (Rect (35,590, 180, 20), "SCLERA COLOR");
	GUI.color = colorGrey;
	GUI.Label (Rect (35,605, 20, 20), "r");
	GUI.Label (Rect (35,618, 20, 20), "g");
	GUI.Label (Rect (35,631, 20, 20), "b");
	GUI.Label (Rect (35,644, 20, 20), "a");
	GenerateSlider("",50,592,false,"scleraColorR",278);
	GenerateSlider("",50,605,false,"scleraColorG",278);
	GenerateSlider("",50,618,false,"scleraColorB",278);
	GenerateSlider("",50,630,false,"scleraColorA",278);


	//LIGHT - Direction
	GUI.color = colorGold;
	GUI.Label (Rect (35,730, 150, 20), "LIGHT DIRECTION");
	GenerateSlider("",160,716,false,"lightDir",820);

}




function GenerateSlider(title : String, sX : int, sY : int, showPercent : boolean, funcName : String, sWidth : int){

	GUI.color = colorGold;
	if (title != "") GUI.Label (Rect (sX,sY, 180, 20), title);

	if (funcName == "lightDir" && showPercent) GUI.Label (Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt((lightDirection*100.0)).ToString()+"%");
	if (funcName == "lodLevel" && showPercent) GUI.Label (Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(100.0-(lodLevel*100.0)).ToString()+"%");
	if (funcName == "pupilDilation" && showPercent) GUI.Label (Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(pupilDilation*100.0).ToString()+"%");
	if (funcName == "scleraSize" && showPercent) GUI.Label (Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(scleraSize*100.0).ToString()+"%");
	if (funcName == "irisSize" && showPercent) GUI.Label (Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(irisSize*100.0).ToString()+"%");
	if (funcName == "irisTexture" && showPercent) GUI.Label (Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(irisTextureD*100.0).ToString()+"%");
	if (funcName == "irisParallax" && showPercent) GUI.Label (Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(parallaxAmt*100.0).ToString()+"%");
	if (funcName == "irisColorR" && showPercent) GUI.Label (Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(irisColor.r*100.0).ToString()+"%");
	if (funcName == "irisColorG" && showPercent) GUI.Label (Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(irisColor.g*100.0).ToString()+"%");
	if (funcName == "irisColorB" && showPercent) GUI.Label (Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(irisColor.b*100.0).ToString()+"%");
	if (funcName == "irisColorA" && showPercent) GUI.Label (Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(irisColor.a*100.0).ToString()+"%");
	if (funcName == "scleraColorR" && showPercent) GUI.Label (Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(scleraColor.r*100.0).ToString()+"%");
	if (funcName == "scleraColorG" && showPercent) GUI.Label (Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(scleraColor.g*100.0).ToString()+"%");
	if (funcName == "scleraColorB" && showPercent) GUI.Label (Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(scleraColor.b*100.0).ToString()+"%");
	if (funcName == "scleraColorA" && showPercent) GUI.Label (Rect (sX+(sWidth-28), sY, 80, 20), Mathf.CeilToInt(scleraColor.a*100.0).ToString()+"%");


	GUI.color = Color(1.0,1.0,1.0,1.0);
	if (texSlideB != null) GUI.DrawTextureWithTexCoords(Rect (sX,sY+22, sWidth+2,7), texSlideB, Rect (sX,sY+22, sWidth+2,7), true);
	
	if (funcName == "lightDir" && texSlideA != null) GUI.DrawTextureWithTexCoords(Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,lightDirection),5), texSlideA, Rect (sX+1,sY+23, sWidth,5), true);
	if (funcName == "lodLevel" && texSlideA != null) GUI.DrawTextureWithTexCoords(Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,lodLevel),5), texSlideA, Rect (sX+1,sY+23, sWidth,5), true);
	if (funcName == "pupilDilation" && texSlideA != null) GUI.DrawTextureWithTexCoords(Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,pupilDilation),5), texSlideA, Rect (sX+1,sY+23, sWidth,5), true);
	if (funcName == "scleraSize" && texSlideA != null) GUI.DrawTextureWithTexCoords(Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,scleraSize),5), texSlideA, Rect (sX+1,sY+23, sWidth,5), true);
	if (funcName == "irisSize" && texSlideA != null) GUI.DrawTextureWithTexCoords(Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,irisSize),5), texSlideA, Rect (sX+1,sY+23, sWidth,5), true);
	if (funcName == "irisTexture" && texSlideA != null) GUI.DrawTextureWithTexCoords(Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,irisTextureD),5), texSlideA, Rect (sX+1,sY+23, sWidth,5), true);
	if (funcName == "irisParallax" && texSlideA != null) GUI.DrawTextureWithTexCoords(Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,parallaxAmt),5), texSlideA, Rect (sX+1,sY+23, sWidth,5), true);
		
	GUI.color = Color(irisColor.r,irisColor.g,irisColor.b,irisColor.a);
	if (funcName == "irisColorR" && texSlideD != null) GUI.DrawTextureWithTexCoords(Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,irisColor.r),5), texSlideD, Rect (sX+1,sY+23, sWidth,5), true);
	if (funcName == "irisColorG" && texSlideD != null) GUI.DrawTextureWithTexCoords(Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,irisColor.g),5), texSlideD, Rect (sX+1,sY+23, sWidth,5), true);
	if (funcName == "irisColorB" && texSlideD != null) GUI.DrawTextureWithTexCoords(Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,irisColor.b),5), texSlideD, Rect (sX+1,sY+23, sWidth,5), true);
	GUI.color = colorGrey*2;
	if (funcName == "irisColorA" && texSlideD != null) GUI.DrawTextureWithTexCoords(Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,irisColor.a),5), texSlideD, Rect (sX+1,sY+23, sWidth,5), true);
	GUI.color = Color(scleraColor.r,scleraColor.g,scleraColor.b,scleraColor.a);
	if (funcName == "scleraColorR" && texSlideD != null) GUI.DrawTextureWithTexCoords(Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,scleraColor.r),5), texSlideD, Rect (sX+1,sY+23, sWidth,5), true);
	if (funcName == "scleraColorG" && texSlideD != null) GUI.DrawTextureWithTexCoords(Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,scleraColor.g),5), texSlideD, Rect (sX+1,sY+23, sWidth,5), true);
	if (funcName == "scleraColorB" && texSlideD != null) GUI.DrawTextureWithTexCoords(Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,scleraColor.b),5), texSlideD, Rect (sX+1,sY+23, sWidth,5), true);
	GUI.color = colorGrey*2;
	if (funcName == "scleraColorA" && texSlideD != null) GUI.DrawTextureWithTexCoords(Rect (sX+1,sY+23, Mathf.Lerp(1,sWidth,scleraColor.a),5), texSlideD, Rect (sX+1,sY+23, sWidth,5), true);
													

	GUI.color = Color(1.0,1.0,1.0,0.0);
	if (funcName == "lightDir") lightDirection = GUI.HorizontalSlider (Rect (sX-4, sY+19, sWidth+17, 10), lightDirection, 0.0, 1.0);
	if (funcName == "lodLevel") lodLevel = GUI.HorizontalSlider (Rect (sX-4, sY+19, sWidth+17, 10), lodLevel, 0.0, 1.0);
	if (funcName == "pupilDilation") pupilDilation = GUI.HorizontalSlider (Rect (sX-4, sY+19, sWidth+17, 10), pupilDilation, 0.0, 1.0);
	if (funcName == "scleraSize") scleraSize = GUI.HorizontalSlider (Rect (sX-4, sY+19, sWidth+17, 10), scleraSize, 0.0, 1.0);
	if (funcName == "irisSize") irisSize = GUI.HorizontalSlider (Rect (sX-4, sY+19, sWidth+17, 10), irisSize, 0.0, 1.0);
	if (funcName == "irisTexture") irisTextureF = GUI.HorizontalSlider (Rect (sX-4, sY+19, sWidth+17, 10), irisTextureF, 0.0, irisTextures.length-1);
	if (funcName == "irisParallax") parallaxAmt = GUI.HorizontalSlider (Rect (sX-4, sY+19, sWidth+17, 10), parallaxAmt, 0.0, 1.0);
	if (funcName == "irisColorR") irisColor.r = GUI.HorizontalSlider (Rect (sX-4, sY+19, sWidth+17, 10), irisColor.r, 0.0, 1.0);
	if (funcName == "irisColorG") irisColor.g = GUI.HorizontalSlider (Rect (sX-4, sY+19, sWidth+17, 10), irisColor.g, 0.0, 1.0);
	if (funcName == "irisColorB") irisColor.b = GUI.HorizontalSlider (Rect (sX-4, sY+19, sWidth+17, 10), irisColor.b, 0.0, 1.0);
	if (funcName == "irisColorA") irisColor.a = GUI.HorizontalSlider (Rect (sX-4, sY+19, sWidth+17, 10), irisColor.a, 0.0, 1.0);
	if (funcName == "scleraColorR") scleraColor.r = GUI.HorizontalSlider (Rect (sX-4, sY+19, sWidth+17, 10), scleraColor.r, 0.0, 1.0);
	if (funcName == "scleraColorG") scleraColor.g = GUI.HorizontalSlider (Rect (sX-4, sY+19, sWidth+17, 10), scleraColor.g, 0.0, 1.0);
	if (funcName == "scleraColorB") scleraColor.b = GUI.HorizontalSlider (Rect (sX-4, sY+19, sWidth+17, 10), scleraColor.b, 0.0, 1.0);
	if (funcName == "scleraColorA") scleraColor.a = GUI.HorizontalSlider (Rect (sX-4, sY+19, sWidth+17, 10), scleraColor.a, 0.0, 1.0);

}