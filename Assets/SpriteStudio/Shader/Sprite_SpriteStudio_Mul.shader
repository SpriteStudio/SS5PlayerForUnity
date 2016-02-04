//
//	SpriteStudio5 Player for Unity
//
//	Copyright(C) Web Technology Corp.
//	All rights reserved.
//
Shader "Custom/SpriteStudio5/Mul" {
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

			Blend Zero SrcColor

			CGPROGRAM
			#pragma vertex VS_main
			#pragma fragment PS_main

			#include "UnityCG.cginc"

			#include "Base/ShaderVertex_Sprite_SpriteStudio.cginc"

//			#include "Base/ShaderPixel_Sprite_SpriteStudio.cginc"
			#define	LIMIT_ALPHA	0.0038

			sampler2D	_MainTex;

			fixed4	PS_main(InputPS Input) : COLOR0
			{
				fixed4	Output;

				fixed4	Pixel = tex2D(_MainTex, Input.Texture00UV.xy);
				Pixel *= Input.ColorMain;

				fixed4	OverlayParameter = Input.ParameterOverlay;
				fixed4	ColorOverlay = Input.ColorOverlay;
				fixed	ColorAlpha = ColorOverlay.a;
				fixed	PixelAlpha = Pixel.a;

				fixed4	PixelCoefficientColorOvelay = ((0.0f > OverlayParameter.z) ? fixed4(1.0f, 1.0f, 1.0f, 1.0f) : (Pixel * OverlayParameter.z));
				ColorOverlay *= ColorAlpha;

				Pixel = ((Pixel * (1.0f - (ColorAlpha * OverlayParameter.y))) * OverlayParameter.x) + (PixelCoefficientColorOvelay * ColorOverlay * OverlayParameter.w);
				Pixel *= PixelAlpha;											// Mul Only.
				Pixel += fixed4(1.0f, 1.0f, 1.0f, 1.0f) * (1.0 - PixelAlpha);	// Mul Only.
				Pixel.w = 1.0f;													// Mul Only.
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
