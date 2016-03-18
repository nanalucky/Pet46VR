Shader "FurFX/Mobile/FurFX Advanced 10 Layer"
{
	Properties
	{
		_Color ("Color (RGB)", Color) = (1,1,1,1)
	  	_SpecColor ("Specular Material Color (RGB)", Color) = (1,1,1,1) 
	  	_RimColor ("Rim Color", Color) = (0.0,0.0,0.0,0.0)
      	_RimPower ("Rim Power", Range(0.5,8.0)) = 4.0
	  	_Shininess ("Shininess", Range (0.01, 10)) = 8		
		_FurLength ("Fur Length", Range (.0002, 0.5)) = .05
		_MainTex ("Base (RGB) Mask(A)", 2D) = "white" { }
		_NoiseTex ("Noise (RGB)", 2D) = "white" { }
		_Cube ("Reflection Map", Cube) = "" {}
		_Reflection("Reflection Power", Range (0.00, 1)) = 0.0
		_Cutoff ("Alpha Cutoff", Range (0, 1)) = .0001
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

				#pragma vertex VertexProgram_adv
				#pragma fragment fragBase
				#define FUR_MULTIPLIER 0.0
				#include "FurFX Mobile.cginc"
				ENDCG
			}
			Pass
			{
				CGPROGRAM
				#pragma only_renderers opengl gles d3d9

				#pragma vertex VertexProgram_adv
				#pragma fragment frag_adv
				#pragma fragmentoption ARB_precision_hint_fastest
				#define FUR_MULTIPLIER 0.1
				#include "FurFX Mobile.cginc"
				ENDCG
			}
			Pass
			{
				CGPROGRAM
				#pragma only_renderers opengl gles d3d9

				#pragma vertex VertexProgram_adv
				#pragma fragment frag_adv
				#pragma fragmentoption ARB_precision_hint_fastest
				#define FUR_MULTIPLIER 0.2
				#include "FurFX Mobile.cginc"
				ENDCG
			}
			Pass
			{
				CGPROGRAM
				#pragma only_renderers opengl gles d3d9

				#pragma vertex VertexProgram_adv
				#pragma fragment frag_adv
				#pragma fragmentoption ARB_precision_hint_fastest
				#define FUR_MULTIPLIER 0.3
				#include "FurFX Mobile.cginc"
				ENDCG
			}
			Pass
			{
				CGPROGRAM
				#pragma only_renderers opengl gles d3d9

				#pragma vertex VertexProgram_adv
				#pragma fragment frag_adv
				#pragma fragmentoption ARB_precision_hint_fastest
				#define FUR_MULTIPLIER 0.4
				#include "FurFX Mobile.cginc"
				ENDCG
			}
			Pass
			{
				CGPROGRAM
				#pragma only_renderers opengl gles d3d9

				#pragma vertex VertexProgram_adv
				#pragma fragment frag_adv
				#pragma fragmentoption ARB_precision_hint_fastest
				#define FUR_MULTIPLIER 0.5
				#include "FurFX Mobile.cginc"
				ENDCG
			}
			Pass
			{
				CGPROGRAM
				#pragma only_renderers opengl gles d3d9

				#pragma vertex VertexProgram_adv
				#pragma fragment frag_adv
				#pragma fragmentoption ARB_precision_hint_fastest
				#define FUR_MULTIPLIER 0.6
				#include "FurFX Mobile.cginc"
				ENDCG
			}
			Pass
			{
				CGPROGRAM
				#pragma only_renderers opengl gles d3d9

				#pragma vertex VertexProgram_adv
				#pragma fragment frag_adv
				#pragma fragmentoption ARB_precision_hint_fastest
				#define FUR_MULTIPLIER 0.7
				#include "FurFX Mobile.cginc"
				ENDCG
			}
			Pass
			{
				CGPROGRAM
				#pragma only_renderers opengl gles d3d9

				#pragma vertex VertexProgram_adv
				#pragma fragment frag_adv
				#pragma fragmentoption ARB_precision_hint_fastest
				#define FUR_MULTIPLIER 0.8
				#include "FurFX Mobile.cginc"
				ENDCG
			}
			Pass
			{
				CGPROGRAM
				#pragma only_renderers opengl gles d3d9

				#pragma vertex VertexProgram_adv
				#pragma fragment frag_adv
				#pragma fragmentoption ARB_precision_hint_fastest
				#define FUR_MULTIPLIER 0.9
				#include "FurFX Mobile.cginc"
				ENDCG
			}
			Pass
			{
				CGPROGRAM
				#pragma only_renderers opengl gles d3d9

				#pragma vertex VertexProgram_adv
				#pragma fragment frag_adv
				#pragma fragmentoption ARB_precision_hint_fastest
				#define FUR_MULTIPLIER 1.00
				#include "FurFX Mobile.cginc"
				ENDCG

			}
		}		Fallback "Diffuse", 1
	}
}
