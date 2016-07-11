//
//	SpriteStudio5 Player for Unity
//
//	Copyright(C) Web Technology Corp.
//	All rights reserved.
//
#define	LIMIT_ALPHA	0.0038

sampler2D	_MainTex;

#ifdef SV_Target
fixed4	PS_main(InputPS Input) : SV_Target
#else
fixed4	PS_main(InputPS Input) : COLOR0
#endif
{
	fixed4	Output;

	fixed4	Pixel = tex2D(_MainTex, Input.Texture00UV.xy);
	Pixel *= Input.ColorMain;
	Output = Pixel;

	return(Output);
}
