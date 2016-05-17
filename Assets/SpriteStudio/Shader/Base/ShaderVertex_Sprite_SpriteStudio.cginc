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
	float4	Position : POSITION;
//	float4	Position : SV_POSITION;
	float4	ColorMain : COLOR0;
	float4	ColorOverlay : COLOR1;
	float4	Texture00UV : TEXCOORD0;
	float4	ParameterOverlay : TEXCOORD1;
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

	Temp = float4(1.0f, 1.0f, 1.0f, Input.texcoord1.x);
	Output.ColorMain = Temp;

	Output.ColorOverlay = Input.color;

	float Index = Input.texcoord1.y;
	Temp = (0.1f > Index) ? _OverlayParameter_Non
							: ((3.0f > Index) ? ((2.0f > Index) ? _OverlayParameter_Mix : _OverlayParameter_Add)
												: ((4.0 > Index) ? _OverlayParameter_Sub : _OverlayParameter_Mul)
							  );
	Output.ParameterOverlay = Temp;

	Temp = Input.vertex;
	Temp = mul(UNITY_MATRIX_MVP, Temp);
	Output.PositionDraw = Temp;
	Output.Position = Temp;

    return Output;
}
