Shader "FurFX/Mobile/FurFX Shell 5 Layer"
{
	Properties
	{
		_Color ("Color (RGB)", Color) = (1,1,1,1)
	  	_SpecColor ("Specular Material Color (RGB)", Color) = (1,1,1,1) 
	  	_Shininess ("Shininess", Range (0.01, 10)) = 8		
		_FurLength ("Fur Length", Range (.0002, 0.5)) = .05
		_MainTex ("Base (RGB) Mask(A)", 2D) = "white" { }
		_NoiseTex ("Noise (RGB)", 2D) = "white" { }
		_EdgeFade ("Edge Fade", Range(0,1)) = 0.15
		_HairHardness ("Fur Hardness", Range(0.1,1)) = 1
		_HairThinness ("Fur Thinness", Range(0.01,10)) = 2
		_HairShading ("Fur Shading", Range(0.0,1)) = 0.25
		_SkinAlpha ("Mask Alpha Factor", Range(0.0,1)) = 0.5
		_ForceGlobal ("Force Global",Vector) = (0,0,0,0)		
		_ForceLocal ("Force Local",Vector) = (0,0,0,0)	
	}
	Category
	{
		ZWrite On
		Tags {"Queue" = "Transparent"}
		Tags { "LightMode" = "ForwardBase" }
		Blend SrcAlpha OneMinusSrcAlpha
		
		SubShader
		{

			Pass
			{
				CGPROGRAM
				#pragma only_renderers opengl gles d3d9

				#pragma vertex VertexProgram
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#define FUR_MULTIPLIER 0.2
				#include "FurFX Mobile.cginc"
				ENDCG
			}
			Pass
			{
				CGPROGRAM
				#pragma only_renderers opengl gles d3d9

				#pragma vertex VertexProgram
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#define FUR_MULTIPLIER 0.4
				#include "FurFX Mobile.cginc"
				ENDCG
			}
			Pass
			{
				CGPROGRAM
				#pragma only_renderers opengl gles d3d9

				#pragma vertex VertexProgram
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#define FUR_MULTIPLIER 0.6
				#include "FurFX Mobile.cginc"
				ENDCG
			}
			Pass
			{
				CGPROGRAM
				#pragma only_renderers opengl gles d3d9

				#pragma vertex VertexProgram
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#define FUR_MULTIPLIER 0.8
				#include "FurFX Mobile.cginc"
				ENDCG
			}
			Pass
			{
				CGPROGRAM
				#pragma only_renderers opengl gles d3d9

				#pragma vertex VertexProgram
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#define FUR_MULTIPLIER 1.00
				#include "FurFX Mobile.cginc"
				ENDCG

			}
		}		Fallback "Diffuse", 1
	}
}
