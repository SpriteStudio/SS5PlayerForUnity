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
			// MEMO: Blend "Add", "PreMultiplied-Alpha"
			Lighting Off
			Fog { Mode off }

			Cull Off
			ZTest LEqual
			ZWRITE Off

			Blend SrcAlpha One
//			Blend SrcColor One

			CGPROGRAM
			#pragma vertex VS_main
			#pragma fragment PS_main

			#include "UnityCG.cginc"

			#include "Base/ShaderVertex_Particle_SpriteStudio.cginc"

//			#include "Base/ShaderPixel_Particle_SpriteStudio.cginc"
			#define	LIMIT_ALPHA	0.0038
			sampler2D	_MainTex;
			fixed4	PS_main(InputPS Input) : COLOR0
//			fixed4	PS_main(InputPS Input) : SV_Target
			{
				fixed4	Output;

				fixed4	Pixel = tex2D(_MainTex, Input.Texture00UV.xy);
				Pixel *= Input.ColorMain;
				Pixel *= Pixel.a;
//				Pixel.a = 1.0f;
				Output = Pixel;

				return(Output);
			}

			ENDCG

			SetTexture [_MainTex]	{
				Combine Texture, Texture
			}
		}
	}
	FallBack Off
}
