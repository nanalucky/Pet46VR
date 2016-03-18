furFX Mobile - Physics-based Fur Shaders for Mobile 3.0
-------------------------------------

Information
-----------
Pack contains multipass mobile FUR shaders with many extra features like physics based fur movement, fur gravity, custom coloring etc. Shaders are compiled.

Main features
-------------
PC and MAC compatible, Unity 3.5.x, 4.x and 5.x
Works on SM2.0
Tested on iOS and Android devices
Fur Wind Simulation
Fur Gravity Simulation
Fur Rigidbody Forces Simulation
Fur Thickness, Fur Coloring, Fur Shading, Fur Length, Hair Hardness
Fur Length Masking
Fur Rim Light
Fur Cubemap Reflection
Directional Light + up to 4 vertex lights
5 sample fur textures with alpha mask
5,10,20 Layer Basic Shaders
5,10,20 Layer Basic No Clip Shaders (not using clip instruction - for some Android devices)
5,10,20 Layer Advanced Shaders
5,10,20 Layer Unlit Shaders
5,10,20 Layer Shell Shaders
Directional Light, Ambient Light and Specular
Model Cast Shadow

Imporant
--------
When creating new project - make sure to turn off DX11 mode and set DX9
In Unity 5.x you can find it under Player Settings / Automatic Graphics API
In Unity 4.x is under Player Settings / Use DirectX 11

Use instruction
---------------
Assign fur shader for your material. If you wanna extra effects like fur gravity, movement with rigidbody velocity or wind - you need to add <FurForce.js> script to object. 
You can also control fur movement by yourself - look into FurForce.js to see how to pass fur movement Vector into shader.

Shader Parameters Description
-----------------------------
Color (RGB) - material color
Specular Material Color (RGB) - specular light color
Shininess - specular shininess factor
Fur Length - length of fur
Base (RGB) - main texture (RGB). Alpha works like fur length mask.
Noise (RGB) - noise texture. Assign one of prepared 2 or make yours.
Alpha Cutoff - alpha cutoff factor
Edge Fade - how much fur go into transparent when more far from core
Fur Hardness - how much fur hold on place after applying gravity or other forces
Fur Thinness - thinness of fur
Fur Shading - how much fur color go darker when closer to fur core
Mask Alpha Factor - factor of alpha mask taked from main texture. Can override alpha mask from texture alpha.
Force Global - global force working of fur - keep inside (-1,1) - NOT affected by object rotation - use for gravity, wind etc
Force Local - local force working of fur - keep inside (-1,1) - AFFECTED by object rotation - use to shape fur on objects
	
Advanced Shaders Bonus Parameters 
-------------------
Rim Color - color of rim lightning (more dark = less visible)
Rim Power - power of rim
Reflection Map - cubemap reflection
Reflection Power - alpha of reflection cubemap
			
FurForce.js
-----------
smoothing - higher value makes your fur faster rotate in desire direction
addRigidbodyForce - add force from rigidbody velocity
rigidbodyForceFactor - factor for above
addGravityToForce - add gravity force to fur
gravityFactor - factor for above
addWindForce - add wind force to fur
windForceFactor - scale of wind - if you wanna have wind not blowing from up and down - set it as (1,0,1)
windForceSpeed - speed of wind changing direction
		
Version History
---------------
1.0 Initial Version
2.0 Uncompiled shaders
	Added Shell Shaders
3.0 Added Unity 5.x support
	
				
Requests
--------
This pack of shaders is constantly updated by shaders that you have requested. If you got some request - feel free to contact me or write on Unity3d forum.		

Unity Forums thread
-------------------
<soon>

Credits
-------
Red Dot Games 2013
http://www.reddotgames.pl