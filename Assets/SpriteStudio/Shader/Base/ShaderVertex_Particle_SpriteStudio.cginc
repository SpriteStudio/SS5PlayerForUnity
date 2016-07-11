//
//	SpriteStudio5 Player for Unity
//
//	Copyright(C) Web Technology Corp.
//	All rights reserved.
//

float4	_OverlayParameter_Non;
float4	_OverlayParameter_Mix;
float4	_OverlayParameter_Add;
float4	_OverlayParameter_Sub;
float4	_OverlayParameter_Mul;

struct	InputVS	{
	float4	vertex : POSITION;
	float4	color : COLOR0;
	float4	texcoord : TEXCOORD0;
	float4	texcoord1 : TEXCOORD1;
};

struct	InputPS	{
#ifdef SV_POSITION
	float4	Position : SV_POSITION;
#else
	float4	Position : POSITION;
#endif
	float4	ColorMain : COLOR0;
	float4	Texture00UV : TEXCOORD0;
	float4	PositionDraw : TEXCOORD7;
};

InputPS	VS_main(InputVS Input)
{
	InputPS	Output;
	float4	Temp;

	Temp.xy = Input.texcoord.xy;
	Temp.z = 0.0f;
	Temp.w = 0.0f;
	Output.Texture00UV = Temp;

	Temp = Input.color;
	Temp.w *= Input.texcoord1.x;
	Output.ColorMain = Temp;

	Temp = Input.vertex;
	Temp = mul(UNITY_MATRIX_MVP, Temp);
	Output.PositionDraw = Temp;
	Output.Position = Temp;

    return Output;
}
