//
//	SpriteStudio5 Player for Unity
//
//	Copyright(C) Web Technology Corp.
//	All rights reserved.
//
Shader "Custom/SpriteStudio5/Effect/Add" {
	Properties	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

	SubShader	{
		Tags {
				"Queue"="Transparent"
				"IgnoreProjector"="True"
				"RenderType"="Transparent"
		}

		Pass	{
			Lighting Off
			Fog { Mode off }

			Cull Off
			ZTest LEqual
			ZWRITE Off

			Blend SrcAlpha One

			CGPROGRAM
			#pragma vertex VS_main
			#pragma fragment PS_main

			#include "UnityCG.cginc"

			#include "Base/ShaderVertex_Particle_SpriteStudio.cginc"

			#include "Base/ShaderPixel_Particle_SpriteStudio.cginc"

			ENDCG

			SetTexture [_MainTex]	{
				Combine Texture, Texture
			}
		}
	}
	FallBack Off
}
