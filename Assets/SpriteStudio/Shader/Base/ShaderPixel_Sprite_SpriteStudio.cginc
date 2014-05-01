//
//	SpriteStudio5 Player for Unity
//
//	Copyright(C) Web Technology Corp.
//	All rights reserved.
//
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
	Pixel.a = PixelAlpha;
	Output = Pixel;

	return(Output);
}
