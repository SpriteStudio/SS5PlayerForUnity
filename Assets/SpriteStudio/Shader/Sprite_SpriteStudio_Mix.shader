//
//	SpriteStudio5 Player for Unity
//
//	Copyright(C) Web Technology Corp.
//	All rights reserved.
//
Shader "Custom/SpriteStudio5/Mix" {
	Properties	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_OverlayParameter_Non ("Parameter(Non)", Vector) = (1.0, 0.0, -1.0, 0.0)
		_OverlayParameter_Mix ("Parameter(Mix)", Vector) = (1.0, 1.0, -1.0, 1.0)
		_OverlayParameter_Add ("Parameter(Add)", Vector) = (1.0, 0.0, -1.0, 1.0)
		_OverlayParameter_Sub ("Parameter(Sub)", Vector) = (1.0, 0.0, -1.0, -1.0)
		_OverlayParameter_Mul ("Parameter(Mul)", Vector) = (1.0, 1.0, 1.0, 1.0)
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

			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex VS_main
			#pragma fragment PS_main

			#include "UnityCG.cginc"

			#include "Base/ShaderVertex_Sprite_SpriteStudio.cginc"

			#include "Base/ShaderPixel_Sprite_SpriteStudio.cginc"

			ENDCG

			SetTexture [_MainTex]	{
				Combine Texture, Texture
			}
		}
	}
	FallBack Off
}
