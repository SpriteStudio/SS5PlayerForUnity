/**
	SpriteStudio5 Player for Unity

	Copyright(C) 2003-2013 Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public static class Library_SpriteStudio
{
	public delegate bool FunctionCallBackPlayEnd(GameObject ObjectControl);
	public delegate void FunctionCallBackUserData(GameObject ObjectControl, string PartsName, Library_SpriteStudio.SpriteData AnimationDataParts, int AnimationNo, int FrameNoDecode, int FrameNoKeyData, Library_SpriteStudio.KeyFrame.ValueUser Data);
	public delegate void FunctionCallBackOnTrigger(Collider Self, Collider Pair);
	public delegate void FunctionCallBackOnCollision(Collider Self, Collision Contacts);

	public enum KindParts
	{
		NORMAL = 0,
		ROOT,
		NULL,
		BOUND,
		SOUND,

		TERMINATOR
	};

	public enum VertexNo
	{
		LU = 0,
		RU,
		RD,
		LD,
		C,

		TERMINATOR4,
		TERMINATOR2 = C,
	};

	public enum KindAttributeKey
	{	/* MEMO: Don't change the order of enumerate, "POSITION_X"-"SHOW_HIDE" */
		POSITION_X = 0,
		POSITION_Y,
		POSITION_Z,
		ROTATE_X,
		ROTATE_Y,
		ROTATE_Z,
		SCALE_X,
		SCALE_Y,

		OPACITY_RATE,
		FLIP_X,
		FLIP_Y,
		SHOW_HIDE,

		PRIORITY,

		COLOR_BLEND,
		VERTEX_CORRECTION,
		ORIGIN_OFFSET_X,
		ORIGIN_OFFSET_Y,

		ANCHOR_POSITION_X,
		ANCHOR_POSITION_Y,
		ANCHOR_SIZE_X,
		ANCHOR_SIZE_Y,

		TEXTURE_FLIP_X,
		TEXTURE_FLIP_Y,

		TEXTURE_TRANSLATE_X,
		TEXTURE_TRANSLATE_Y,
		TEXTURE_ROTATE,
		TEXTURE_SCALE_X,
		TEXTURE_SCALE_Y,
		TEXTURE_EXPAND_WIDTH,
		TEXTURE_EXPAND_HEIGHT,

		COLLISION_RADIUS,

		CELL,
		USER_DATA,

		PALETTE_CHANGE,
		SOUND,

		TERMINATOR,
		TERMINATOR_INHELIT = (SHOW_HIDE + 1),
	};

	public enum FlagAttributeKeyInherit
	{
		POSITION_X = (1 << KindAttributeKey.POSITION_X),
		POSITION_Y = (1 << KindAttributeKey.POSITION_Y),
		POSITION_Z = (1 << KindAttributeKey.POSITION_Z),
		ROTATE_X = (1 << KindAttributeKey.ROTATE_X),
		ROTATE_Y = (1 << KindAttributeKey.ROTATE_Y),
		ROTATE_Z = (1 << KindAttributeKey.ROTATE_Z),
		SCALE_X = (1 << KindAttributeKey.SCALE_X),
		SCALE_Y = (1 << KindAttributeKey.SCALE_Y),

		OPACITY_RATE = (1 << KindAttributeKey.OPACITY_RATE),
		FLIP_X = (1 << KindAttributeKey.FLIP_X),
		FLIP_Y = (1 << KindAttributeKey.FLIP_Y),
		SHOW_HIDE = (1 << KindAttributeKey.SHOW_HIDE),

		CLEAR = 0,
		ALL = ((1 << KindAttributeKey.TERMINATOR_INHELIT) - 1),
		PRESET = POSITION_X
				| POSITION_Y
				| POSITION_Z
				| ROTATE_X
				| ROTATE_Y
				| ROTATE_Z
				| SCALE_X
				| SCALE_Y
	};

	public enum KindTypeKey
	{
		INT,
		FLOAT,
		BOOL,
		HEX,
		DEGREE,
		OTHER,
	};

	public enum KindValueKey
	{
		NUMBER,
		CHECK,
		POINT,
		PALETTE,
		COLOR,
		QUADRILATERRAL,
		USER,
		SOUND,
		CELL,
	};

	public enum KindColorOperation
	{
		NON = 0,
		MIX,
		ADD,
		SUB,
		MUL,

		TERMINATOR
	};

	public enum KindColorBound
	{
		NON = 0,
		OVERALL,
		VERTEX,
	};

	public enum KindCollision
	{
		NON = 0,
		SQUARE,
		AABB,
		CIRCLE,
		CIRCLE_SCALEMINIMUM,
		CIRCLE_SCALEMAXIMUM,
	};

	public enum KindInterpolation
	{
		NON = 0,
		LINEAR,
		HERMITE,
		BEZIER,
		ACCELERATE,
		DECELERATE,
	};

	public enum KindInheritance
	{
		PARENT,
		SELF
	};

	public enum FlagDirection
	{
		JUSTNOW = 0x0001,
		PAST = 0x0002,
		FUTURE = 0x0004,
		ALL = JUSTNOW | PAST | FUTURE,
	};

	public readonly static FlagAttributeKeyInherit[] FlagParameterKeyFrameInherit = new FlagAttributeKeyInherit[(int)KindAttributeKey.TERMINATOR_INHELIT]
	{
		FlagAttributeKeyInherit.POSITION_X,
		FlagAttributeKeyInherit.POSITION_Y,
		FlagAttributeKeyInherit.POSITION_Z,
		FlagAttributeKeyInherit.ROTATE_X,
		FlagAttributeKeyInherit.ROTATE_Y,
		FlagAttributeKeyInherit.ROTATE_Z,
		FlagAttributeKeyInherit.SCALE_X,
		FlagAttributeKeyInherit.SCALE_Y,

		FlagAttributeKeyInherit.OPACITY_RATE,
		FlagAttributeKeyInherit.FLIP_X,
		FlagAttributeKeyInherit.FLIP_Y,
		FlagAttributeKeyInherit.SHOW_HIDE,
	};

	public readonly static int[] ArrayVertexIndex_Triangle2 =
	{
		(int)VertexNo.LU, (int)VertexNo.RU, (int)VertexNo.RD,
		(int)VertexNo.RD, (int)VertexNo.LD, (int)VertexNo.LU
	};
	public readonly static Vector3[] ArrayUVMappingUV0_Triangle2 = new Vector3[]
	{
		new Vector3(-0.5f, 0.5f, 0.0f),
		new Vector3(0.5f, 0.5f, 0.0f),
		new Vector3(0.5f, -0.5f, 0.0f),
		new Vector3(-0.5f, -0.5f, 0.0f)
	};
	public readonly static Vector3[] ArrayNormal_Triangle2 = new Vector3[]
	{
		new Vector3(0.0f, 0.0f, 1.0f),
		new Vector3(0.0f, 0.0f, 1.0f),
		new Vector3(0.0f, 0.0f, 1.0f),
		new Vector3(0.0f, 0.0f, 1.0f)
	};

	public readonly static int[] ArrayVertexIndex_Triangle4 =
	{
		(int)VertexNo.C, (int)VertexNo.LU, (int)VertexNo.RU,
		(int)VertexNo.C, (int)VertexNo.RU, (int)VertexNo.RD,
		(int)VertexNo.C, (int)VertexNo.RD, (int)VertexNo.LD,
		(int)VertexNo.C, (int)VertexNo.LD, (int)VertexNo.LU,
	};
	public readonly static Vector3[] ArrayUVMappingUV0_Triangle4 = new Vector3[]
	{
		new Vector3(-0.5f, 0.5f, 0.0f),
		new Vector3(0.5f, 0.5f, 0.0f),
		new Vector3(0.5f, -0.5f, 0.0f),
		new Vector3(-0.5f, -0.5f, 0.0f),
		new Vector3(0.0f, 0.0f, 0.0f)
	};
	public readonly static Vector3[] ArrayNormal_Triangle4 = new Vector3[]
	{
		new Vector3(0.0f, 0.0f, 1.0f),
		new Vector3(0.0f, 0.0f, 1.0f),
		new Vector3(0.0f, 0.0f, 1.0f),
		new Vector3(0.0f, 0.0f, 1.0f),
		new Vector3(0.0f, 0.0f, 1.0f)
	};

	public static class Utility
	{
		public static int VertexNoSearchMappingUV(Mesh InstanceMesh, ref Vector2 MappingUV)
		{
			for(int i=0; i<InstanceMesh.vertexCount; i++)
			{
				if(InstanceMesh.uv[i] == MappingUV)
				{
					return(i);
				}
			}
			return(-1);
		}

		public static void CoordinateGetDiagonalIntersection(out Vector2 Output, ref Vector2 LU, ref Vector2 RU, ref Vector2 LD, ref Vector2 RD)
		{
			Output = Vector2.zero;

			float c1 = (LD.y - RU.y) * (LD.x - LU.x) - (LD.x - RU.x) * (LD.y - LU.y);
			float c2 = (RD.x - LU.x) * (LD.y - LU.y) - (RD.y - LU.y) * (LD.x - LU.x);
			float c3 = (RD.x - LU.x) * (LD.y - RU.y) - (RD.y - LU.y) * (LD.x - RU.x);
			float ca = c1 / c3;
			float cb = c2 / c3;

			if(((0.0f <= ca) && (1.0f >= ca)) && ((0.0f <= cb) && (1.0f >= cb)))
			{
				Output.x = LU.x + ca * (RD.x - LU.x);
				Output.y = LU.y + ca * (RD.y - LU.y);
			}
		}

		public static Script_SpriteStudio_PartsRoot SpriteStudioDataGetRoot(GameObject GameObjectParts)
		{
			GameObject GameObjectParent = GameObjectParts.transform.parent.gameObject;
			Script_SpriteStudio_PartsRoot ComponentScript_PartsRoot = null;

			while(null == ComponentScript_PartsRoot)
			{
				if(null == GameObjectParent)
				{
					break;
				}
				ComponentScript_PartsRoot = GameObjectParent.GetComponent<Script_SpriteStudio_PartsRoot>();
				if(null != ComponentScript_PartsRoot)
				{
					return(ComponentScript_PartsRoot);
				}
				GameObjectParent = GameObjectParent.transform.parent.gameObject;
			}
			return(null);
		}

		public static SpriteData SpriteStudioDataGetParent(GameObject GameObjectParts)
		{
			GameObject GameObjectParent = GameObjectParts.transform.parent.gameObject;
			SpriteData DataSpriteStudio = null;

			Script_SpriteStudio_Triangle2 ComponentScript_Triangle2 = GameObjectParent.GetComponent<Script_SpriteStudio_Triangle2>();
			if(null != ComponentScript_Triangle2)
			{
				DataSpriteStudio = ComponentScript_Triangle2.SpriteStudioData;
			}
			else
			{
				Script_SpriteStudio_Triangle4 ComponentScript_Triangle4 = GameObjectParent.GetComponent<Script_SpriteStudio_Triangle4>();
				if(null != ComponentScript_Triangle4)
				{
					DataSpriteStudio = ComponentScript_Triangle4.SpriteStudioData;
				}
				else
				{
					Script_SpriteStudio_PartsNULL ComponentScript_PartsNULL = GameObjectParent.GetComponent<Script_SpriteStudio_PartsNULL>();
					if(null != ComponentScript_PartsNULL)
					{
						DataSpriteStudio = ComponentScript_PartsNULL.SpriteStudioData;
					}
					else
					{
						Script_SpriteStudio_PartsRoot ComponentScript_PartsRoot = GameObjectParts.GetComponent<Script_SpriteStudio_PartsRoot>();
						if(null != ComponentScript_PartsRoot)
						{
							DataSpriteStudio = ComponentScript_PartsRoot.SpriteStudioData;
						}
					}
				}
			}
			return(DataSpriteStudio);
		}
	}

	public static class Interpolation
	{
		public static float Linear(float Start, float End, float Point)
		{
			return(((End - Start) * Point) + Start);
		}

		public static float Hermite(float Start, float SpeedStart, float End, float SpeedEnd, float Point)
		{
			float PointPow2 = Point * Point;
			float PointPow3 = PointPow2 * Point;

			return(	(((2.0f * PointPow3) - (3.0f * PointPow2) + 1.0f) * Start)
					+ (((3.0f * PointPow2) - (2.0f * PointPow3)) * End)
					+ ((PointPow3 - (2.0f * PointPow2) + Point) * (SpeedStart - Start))
					+ ((PointPow3 - PointPow2) * (SpeedEnd - End))
				);
		}

		public static float Bezier(ref Vector2 Start, ref Vector2 VectorStart, ref Vector2 End, ref Vector2 VectorEnd, float Point)
		{
			float PointNow = Linear(Start.x, End.x, Point);
			float PointTemp;

			float AreaNow = 0.5f;
			float RangeNow = 0.5f;

			float Base;
			float BasePow2;
			float BasePow3;
			float AreaNowPow2;
			for(int i=0; i<8; i++)
			{
				Base = 1.0f - AreaNow;
				BasePow2 = Base * Base;
				BasePow3 = BasePow2 * Base;
				AreaNowPow2 = AreaNow * AreaNow;
				PointTemp = (BasePow3 * Start.x)
							+ (3.0f * BasePow2 * AreaNow * (VectorStart.x + Start.x))
							+ (3.0f * Base * AreaNowPow2 * (VectorEnd.x + End.x))
							+ (AreaNow * AreaNowPow2 * End.x);

				RangeNow *= 0.5f;
				AreaNow += ((PointTemp > PointNow) ? (-RangeNow) : (RangeNow));
			}

			AreaNowPow2 = AreaNow * AreaNow;
			Base = 1.0f - AreaNow;
			BasePow2 = Base * Base;
			BasePow3 = BasePow2 * Base;
			return(	(BasePow3 * Start.y)
					+ (3.0f * BasePow2 * AreaNow * (VectorStart.y + Start.y))
					+ (3.0f * Base * AreaNowPow2 * (VectorEnd.y + End.y))
					+ (AreaNow * AreaNowPow2 * End.y)
				);
		}

		public static float Accelerate(float Start, float End, float Point)
		{
			return(((End - Start) * (Point * Point)) + Start);
		}

		public static float Decelerate(float Start, float End, float Point)
		{
			float PointInverse = 1.0f - Point;
			float Rate = 1.0f - (PointInverse * PointInverse);
			return(((End - Start) * Rate) + Start);
		}

		public static float Interpolate<_Type>(	KeyFrame.DataCurve Curve,
												int TimeNow,
												_Type ValueStart,
												_Type ValueEnd,
												int TimeStart,
												int TimeEnd)
		{
			if(TimeEnd <= TimeStart)
			{
				return(Convert.ToSingle(ValueEnd));
			}
			float TimeNormalize = ((float)(TimeNow - TimeStart)) / ((float)(TimeEnd - TimeStart));
			TimeNormalize = Mathf.Clamp01(TimeNormalize);

			switch(Curve.Kind)
			{
				case KindInterpolation.NON:
					return(Convert.ToSingle(ValueStart));

				case KindInterpolation.LINEAR:
					return(Interpolation.Linear(Convert.ToSingle(ValueStart), Convert.ToSingle(ValueEnd), TimeNormalize));

				case KindInterpolation.HERMITE:
					return(Interpolation.Hermite(Convert.ToSingle(ValueStart), Curve.ValueStart, Convert.ToSingle(ValueEnd), Curve.ValueEnd, TimeNormalize));

				case KindInterpolation.BEZIER:
					{
						Vector2 Start = new Vector2((float)TimeStart, Convert.ToSingle(ValueStart));
						Vector2 VectorStart = new Vector2(Curve.TimeStart, Curve.ValueStart);
						Vector2 End = new Vector2((float)TimeEnd, Convert.ToSingle(ValueEnd));
						Vector2 VectorEnd = new Vector2(Curve.TimeEnd, Curve.ValueEnd);
						return(Interpolation.Bezier(ref Start, ref VectorStart, ref End, ref VectorEnd, TimeNormalize));
					}

				case KindInterpolation.ACCELERATE:
					return(Interpolation.Accelerate(Convert.ToSingle(ValueStart), Convert.ToSingle(ValueEnd), TimeNormalize));

				case KindInterpolation.DECELERATE:
					return(Interpolation.Decelerate(Convert.ToSingle(ValueStart), Convert.ToSingle(ValueEnd), TimeNormalize));

				default:
					break;
			}
			return(Convert.ToSingle(ValueStart));
		}
	}

	public static class KeyFrame
	{
		[System.Serializable]
		public class DataCurve
		{
			public KindInterpolation Kind;
			public float TimeStart;
			public float ValueStart;
			public float TimeEnd;
			public float ValueEnd;

			public bool CheckEqual(DataCurve Target)
			{
				if(Kind == Target.Kind)
				{
					if(TimeStart == Target.TimeStart)
					{
						if(ValueStart == Target.ValueStart)
						{
							if(TimeEnd == Target.TimeEnd)
							{
								if(ValueEnd != Target.ValueEnd)
								{
									return(true);
								}
							}
						}
					}
				}
				return(false);
			}

			public override string ToString()
			{
				return(	"Type: " + Kind
						+ ", StartT: " + TimeStart
						+ ", StartV: " + ValueStart
						+ ", EndT: " + TimeEnd
						+ ", EndV: " + ValueEnd
					);
			}
		}

		public interface InterfaceData
		{
			KindAttributeKey Kind { get; set; }
			int Time { get; set; }
			object ObjectValue { get; set; }
			DataCurve Curve { get; set; }

			bool ValueCheckEqual(InterfaceData Target);
		}
		public interface InterfaceInterpolatable
		{
			InterfaceInterpolatable GetInterpolated(	DataCurve Curve,
																	int Time,
																	InterfaceInterpolatable ValueStart,
																	InterfaceInterpolatable ValueEnd,
																	int TimeStart,
																	int TimeEnd
																);

			InterfaceInterpolatable Interpolate(	DataCurve Curve,
													int Time,
													InterfaceInterpolatable ValueStart,
													InterfaceInterpolatable ValueEnd,
													int TimeStart,
													int TimeEnd
												);
		}

		[System.Serializable]
		public class DataBase<_Type> : InterfaceData
		{
			[SerializeField]
			protected KindAttributeKey ValueKind;
			public KindAttributeKey	Kind
			{
				get { return(ValueKind); }
				set { ValueKind = value; }
			}
			[SerializeField]
			protected int ValueTime;
			public int Time
			{
				get { return(ValueTime); }
				set { ValueTime = value; }
			}
			[SerializeField]
			protected DataCurve ValueCurve;
			public DataCurve Curve
			{
				get { return(ValueCurve); }
				set { ValueCurve = value; }
			}
			public _Type Value;
			public object ObjectValue
			{
				get{ return(Value); }
				set{ Value = (_Type)value; }
			}

			public bool ValueCheckEqual(InterfaceData Target)
			{
				if((null != Curve) && (null != Target.Curve))
				{
					if(false == Curve.CheckEqual(Target.Curve))
					{
						return(false);
					}
				}
				else
				{
					if(Curve != Target.Curve)
					{
						return(false);
					}
				}

				DataBase<_Type> TargetDerived = (DataBase<_Type>)Target;
				return(Value.Equals(TargetDerived.Value));
			}

			public override string ToString()
			{
				return(	"MyType: " + typeof(_Type)
						+ ", ValueType: " + Kind
						+ ", Time: " + Time
						+ ", Value {" + ObjectValue
						+ "}, Curve {" + Curve + "}\n"
					);
			}
		}

		[System.Serializable]
		public class ValuePoint : InterfaceInterpolatable
		{
			public Vector2 Point;
			public float X
			{
				set	{	Point.x = value;	}
				get	{	return(Point.x);	}
			}
			public float Y
			{
				set	{	Point.y = value;	}
				get	{	return(Point.y);	}
			}

			public ValuePoint()
			{
			}

			public ValuePoint(ValuePoint Value)
			{
				Point = Value.Point;
			}

			public ValuePoint Clone()
			{
				ValuePoint Value = new ValuePoint(this);
				return(Value);
			}

			public override string ToString()
			{
				return(	"X: " + X
						+ ", Y: " + Y
					);
			}

			public static ValuePoint[] CreateArray(int Count)
			{
				var ArrayPoint = new ValuePoint[Count];
				for(int i=0; i<Count; i++)
				{
					ArrayPoint[i] = new ValuePoint();
				}
				return(ArrayPoint);
			}

			public InterfaceInterpolatable GetInterpolated(	DataCurve Curve,
															int TimeNow,
															InterfaceInterpolatable Start,
															InterfaceInterpolatable End,
															int TimeStart,
															int TimeEnd
														)
			{
				ValuePoint Value = new ValuePoint();
				return(Value.Interpolate(Curve, TimeNow, Start, End, TimeStart, TimeEnd));
			}

			public InterfaceInterpolatable Interpolate(	DataCurve Curve,
														int TimeNow,
														InterfaceInterpolatable Start,
														InterfaceInterpolatable End,
														int TimeStart,
														int TimeEnd
													)
			{
				ValuePoint ValueStart = (ValuePoint)Start;
				ValuePoint ValueEnd = (ValuePoint)End;
				Point.x = Interpolation.Interpolate<float>(Curve, TimeNow, ValueStart.Point.x, ValueEnd.Point.x, TimeStart, TimeEnd);
				Point.y = Interpolation.Interpolate<float>(Curve, TimeNow, ValueStart.Point.y, ValueEnd.Point.y, TimeStart, TimeEnd);
				return(this);
			}

			public void Scale(float Ratio)
			{
				Point *= Ratio;
			}
		}
		[System.Serializable]
		public class ValuePalette : InterfaceInterpolatable
		{
			public bool FlagUse;
			public int Page;
			public byte Block;
			public bool Use
			{
				set {	FlagUse = value;	}
				get {	return(FlagUse);	}
			} 

			public ValuePalette()
			{
			}

			public ValuePalette(ValuePalette Value)
			{
				Page = Value.Page;
				Block = Value.Block;
			}

			public ValuePalette Clone()
			{
				ValuePalette Value = new ValuePalette(this);
				return(Value);
			}

			public override string ToString()
			{
				return(	"FlagUse: " + FlagUse
						+ "Page: " + Page
						+ ", Block: " + Block
					);
			}

			public InterfaceInterpolatable GetInterpolated(	DataCurve Curve,
															int TimeNow,
															InterfaceInterpolatable Start,
															InterfaceInterpolatable End,
															int TimeStart,
															int TimeEnd
														)
			{
				ValuePalette Value = new ValuePalette();
				return(Value.Interpolate(Curve, TimeNow, Start, End, TimeStart, TimeEnd));
			}

			public InterfaceInterpolatable Interpolate(	DataCurve Curve,
														int TimeNow,
														InterfaceInterpolatable Start,
														InterfaceInterpolatable End,
														int TimeStart,
														int TimeEnd
													)
			{
				ValuePalette ValueStart = (ValuePalette)Start;
				ValuePalette ValueEnd = (ValuePalette)End;
				FlagUse = (0.5f <= Interpolation.Interpolate<float>(	Curve,
																		TimeNow,
																		((true == ValueStart.FlagUse) ? 1.0f : 0.0f),
																		((true == ValueEnd.FlagUse) ? 1.0f : 0.0f),
																		TimeStart,
																		TimeEnd
																	)) ? true : false;
				Page = (int)Interpolation.Interpolate<int>(Curve, TimeNow, ValueStart.Page, ValueEnd.Page, TimeStart, TimeEnd);
				Block = (byte)(Interpolation.Interpolate<byte>(Curve, TimeNow, ValueStart.Block, ValueEnd.Block, TimeStart, TimeEnd));
				return(this);
			}
		}
		[System.Serializable]
		public class ValueColor : InterfaceInterpolatable
		{
			public KindColorBound Bound;
			public KindColorOperation Operation;
			public Color[] VertexColor;
			public float[] RatePixelAlpha;

			public ValueColor()
			{
				Bound = KindColorBound.NON;
				Operation = KindColorOperation.MIX;
				VertexColor = new Color[4];
				RatePixelAlpha = new float[4];
				for(int i=0; i<VertexColor.Length; i++)
				{
					VertexColor[i] = Color.white;
					RatePixelAlpha[i] = 0.0f;
				}
			}

			public ValueColor(ValueColor Value)
			{
				Bound = Value.Bound;
				Operation = Value.Operation;
				VertexColor = new Color[4];
				RatePixelAlpha = new float[4];
				for(int i=0; i<Value.VertexColor.Length; i++)
				{
					VertexColor[i] = Value.VertexColor[i];
					RatePixelAlpha[i] = Value.RatePixelAlpha[i];
				}
			}

			public ValueColor Clone()
			{
				ValueColor Value = new ValueColor(this);
				return(Value);
			}

			public override string ToString()
			{
				string Text = "Bound: " + Bound + ", Operation: " + Operation;
				if(null != VertexColor)
				{
					for(int i=0; i<VertexColor.Length; i++)
					{
						Text += ", Color("
								+ i
								+ ")["
								+ VertexColor[i].ToString()
								+ "], PixelAlphaRate ["
								+ RatePixelAlpha[i].ToString();
					}
				}
				return(Text);
			}

			public InterfaceInterpolatable GetInterpolated(	DataCurve Curve,
															int TimeNow,
															InterfaceInterpolatable Start,
															InterfaceInterpolatable End,
															int TimeStart,
															int TimeEnd
														)
			{
				ValueColor Value = null;
				Value = new ValueColor();
				return(Value.Interpolate(Curve, TimeNow, Start, End, TimeStart, TimeEnd));
			}

			public InterfaceInterpolatable Interpolate(	DataCurve Curve,
														int TimeNow,
														InterfaceInterpolatable Start,
														InterfaceInterpolatable End,
														int TimeStart,
														int TimeEnd
													)
			{
				ValueColor ValueStart = (ValueColor)Start;
				ValueColor ValueEnd = (ValueColor)End;

				if((KindColorBound.NON == ValueStart.Bound) && (KindColorBound.NON == ValueEnd.Bound))
				{
					Debug.LogError("Sprite Color-Blend Error!!");
				}
				else
				{
					for(int i=0; i<VertexColor.Length; i++)
					{
						VertexColor[i].r = Interpolation.Interpolate<float>(Curve, TimeNow, ValueStart.VertexColor[i].r , ValueEnd.VertexColor[i].r, TimeStart, TimeEnd);
						VertexColor[i].g = Interpolation.Interpolate<float>(Curve, TimeNow, ValueStart.VertexColor[i].g , ValueEnd.VertexColor[i].g, TimeStart, TimeEnd);
						VertexColor[i].b = Interpolation.Interpolate<float>(Curve, TimeNow, ValueStart.VertexColor[i].b , ValueEnd.VertexColor[i].b, TimeStart, TimeEnd);
						VertexColor[i].a = Interpolation.Interpolate<float>(Curve, TimeNow, ValueStart.VertexColor[i].a , ValueEnd.VertexColor[i].a, TimeStart, TimeEnd);

						RatePixelAlpha[i] = Interpolation.Interpolate<float>(Curve, TimeNow, ValueStart.RatePixelAlpha[i], ValueEnd.RatePixelAlpha[i], TimeStart, TimeEnd);
					}

					bool FlagStartParam = (1.0f > TimeNow) ? true : false;
					if(KindColorBound.NON == ValueStart.Bound)
					{
						for(int i=0; i<VertexColor.Length; i++)
						{
							VertexColor[i].r = ValueEnd.VertexColor[i].r;
							VertexColor[i].g = ValueEnd.VertexColor[i].g;
							VertexColor[i].b = ValueEnd.VertexColor[i].b;
							VertexColor[i].a = ValueEnd.VertexColor[i].a;

							RatePixelAlpha[i] = ValueEnd.RatePixelAlpha[i];
						}
						FlagStartParam = false;
					}
					else
					{
						if(KindColorBound.NON == ValueEnd.Bound)
						{
							for(int i=0; i<VertexColor.Length; i++)
							{
								VertexColor[i].r = ValueStart.VertexColor[i].r;
								VertexColor[i].g = ValueStart.VertexColor[i].g;
								VertexColor[i].b = ValueStart.VertexColor[i].b;
								VertexColor[i].a = ValueStart.VertexColor[i].a;

								RatePixelAlpha[i] = ValueStart.RatePixelAlpha[i];
							}
						}
					}
					if (true == FlagStartParam)
					{
						Bound = ValueStart.Bound;
						Operation = ValueStart.Operation;
					}
					else
					{
						Bound = ValueEnd.Bound;
						Operation = ValueEnd.Operation;
					}
				}
				return(this);
			}
		}
		[System.Serializable]
		public class ValueQuadrilateral : InterfaceInterpolatable
		{
			public ValuePoint[] Coordinate;

			public ValueQuadrilateral()
			{
				Coordinate = ValuePoint.CreateArray(4);
			}

			public ValueQuadrilateral(ValueQuadrilateral Value)
			{
				Coordinate = ValuePoint.CreateArray(4);
				for(int i=0; i<Coordinate.Length; i++)
				{
					Coordinate[i] = Value.Coordinate[i];
				}
			}

			public ValueQuadrilateral Clone()
			{
				ValueQuadrilateral Value = new ValueQuadrilateral(this);
				return(Value);
			}

			public override string ToString()
			{
				string Text = "Vertices: ";
				for(int i=0; i<Coordinate.Length; i++)
				{
					Text += "[" + i + "](" + Coordinate[i].ToString() + ") ";
				}
				return(Text);
			}

			public InterfaceInterpolatable GetInterpolated(	DataCurve Curve,
															int TimeNow,
															InterfaceInterpolatable Start,
															InterfaceInterpolatable End,
															int TimeStart,
															int TimeEnd
														)
			{
				ValueQuadrilateral Value = new ValueQuadrilateral();
				return(Value.Interpolate(Curve, TimeNow, Start, End, TimeStart, TimeEnd));
			}

			public InterfaceInterpolatable Interpolate(	DataCurve Curve,
														int TimeNow,
														InterfaceInterpolatable Start,
														InterfaceInterpolatable End,
														int TimeStart,
														int TimeEnd
													)
			{
				ValueQuadrilateral ValueStart = (ValueQuadrilateral)Start;
				ValueQuadrilateral ValueEnd = (ValueQuadrilateral)End;
				for(int i=0; i<Coordinate.Length; i++)
				{
					Coordinate[i].Interpolate(Curve, TimeNow, ValueStart.Coordinate[i], ValueEnd.Coordinate[i], TimeStart, TimeEnd);
				}
				return(this);
			}
		}
		[System.Serializable]
		public class ValueCell
		{
			public int TextureNo;
			public Rect Rectangle;
			public Vector2 Pivot;

			public ValueCell()
			{
			}

			public ValueCell(ValueCell Value)
			{
				TextureNo = Value.TextureNo;
				Rectangle = Value.Rectangle;
				Pivot = Value.Pivot;
			}

			public ValueCell Clone()
			{
				ValueCell Value = new ValueCell(this);
				return(Value);
			}

			public override string ToString()
			{
				return(	"TextureNo: " + TextureNo.ToString()
						+ ", Rectangle: " + Rectangle.ToString()
						+ ", Pivot: " + Pivot.ToString()
					);
			}
		}
		[System.Serializable]
		public class ValueUser
		{
			public enum FlagData
			{
				CLEAR = 0x00000000,
				NUMBER = 0x00000001,
				RECTANGLE = 0x00000002,
				COORDINATE = 0x00000004,
				TEXT = 0x00000008,
			};

			public FlagData Flag;
			public bool IsNumber
			{
				get
				{
					return((0 != FlagData.NUMBER));
				}
			}
			public bool IsRectangle
			{
				get
				{
					return((0 != FlagData.RECTANGLE));
				}
			}
			public bool IsCoordinate
			{
				get
				{
					return((0 != FlagData.COORDINATE));
				}
			}
			public bool IsText
			{
				get
				{
					return((0 != FlagData.TEXT));
				}
			}
			public uint Number;
			public Rect Rectangle;
			public ValuePoint Coordinate;
			public string Text;

			public ValueUser()
			{
			}

			public ValueUser(ValueUser Value)
			{
				Flag = Value.Flag;
				Number = Value.Number;
				Rectangle = Value.Rectangle;
				Coordinate = Value.Coordinate.Clone();
				Text = System.String.Copy(Value.Text);
			}

			public ValueUser Clone()
			{
				ValueUser Value = new ValueUser(this);
				return(Value);
			}

			public override string ToString()
			{
				return(	"IsNum: " + IsNumber.ToString()
						+ ", Num: " + Number
						+ ", IsRect: " + IsRectangle.ToString()
						+ ", Rect: " + Rectangle.ToString()
						+ ", IsPoint: " + IsCoordinate.ToString()
						+ ", Point: X:" + Coordinate.ToString()
						+ ", IsString: " + IsText.ToString()
						+ ", String: " + Text
					);
			}
		}
		[System.Serializable]
		public class ValueSound
		{
			public enum FlagData
			{
				CLEAR = 0x00000000,
				NOTE = 0x00000001,
				VOLUME = 0x00000002,
				USERDATA = 0x00000004,
			};

			[SerializeField]
			public FlagData Flag;
			public uint DataSound;
			public byte SoundId
			{
				set
				{
					DataSound = (DataSound & 0xffffff00) | (((uint)value) & 0x000000ff);
				}
				get
				{
					return((byte)(DataSound & 0x000000ff));
				}
			}
			public byte NoteOn
			{
				set
				{
					DataSound = (DataSound & 0xffff00ff) | ((((uint)value) << 8) & 0x0000ff00);
				}
				get
				{
					return((byte)((DataSound & 0x0000ff00) >> 8));
				}
			}
			public byte Volume
			{
				set
				{
					DataSound = (DataSound & 0xff00ffff) | ((((uint)value) << 16) & 0x00ff0000);
				}
				get
				{
					return((byte)((DataSound & 0x00ff0000) >> 16));
				}
			}
			public byte LoopNum
			{
				set
				{
					DataSound = (DataSound & 0x00ffffff) | ((((uint)value) << 24) & 0x00ffffff);
				}
				get
				{
					return((byte)((DataSound & 0xff000000) >> 24));
				}
			}
			public uint UserData;

			public ValueSound()
			{
			}

			public ValueSound(ValueSound Value)
			{
				Flag = Value.Flag;
				DataSound = Value.DataSound;
				UserData = Value.UserData;
			}

			public ValueSound Clone()
			{
				ValueSound Value = new ValueSound(this);
				return(Value);
			}

			public override string ToString()
			{
				return(	"Flags: " + Flag
						+ ", SoundId: " + SoundId.ToString()
						+ ", NoteOn: " + NoteOn.ToString()
						+ ", Volume: " + Volume.ToString()
						+ ", LoopNum: " + LoopNum.ToString()
						+ ", UserData: " + UserData.ToString()
					);
			}
		}

		[System.Serializable] public class DataBool : DataBase<bool> {}
		[System.Serializable] public class DataInt : DataBase<int> {}
		[System.Serializable] public class DataFloat : DataBase<float> {}
		[System.Serializable] public class DataPoint : DataBase<ValuePoint> {}
		[System.Serializable] public class DataColor : DataBase<ValueColor> {}
		[System.Serializable] public class DataQuadrilateral : DataBase<ValueQuadrilateral> {}
		[System.Serializable] public class DataCell : DataBase<ValueCell> {}
		[System.Serializable] public class DataUser : DataBase<ValueUser> {}
		[System.Serializable] public class DataParette : DataBase<ValuePalette> {}
		[System.Serializable] public class DataSound : DataBase<ValueSound> {}

		public static _Type DataGetIndex<_Type>(_Type[] TableKeyData, int Index)
			where _Type : InterfaceData
		{
			if(null == TableKeyData)
			{
				return(default(_Type));
			}
			if((0 > Index) || (TableKeyData.Length <= Index))
			{
				return(default(_Type));
			}
			return(TableKeyData[Index]);
		}

		public static int DataIndexGetFrame<_Type>(_Type[] TableKeyData, int FrameNo, FlagDirection Direction, int IndexTop)
			where _Type : InterfaceData
		{
			_Type KeyDataNow = default(_Type);
			int IndexPast = -1;
			int IndexFuture = -1;
			if(null != TableKeyData)
			{
				int Count = TableKeyData.Length;
				for(int i=IndexTop; i<Count; i++)
				{
					KeyDataNow = TableKeyData[i];
					if(FrameNo == KeyDataNow.Time)
					{
						if(0 != (Direction & FlagDirection.JUSTNOW))
						{
							return(i);
						}
					}
					else
					{
						if(FrameNo < KeyDataNow.Time)
						{
							IndexFuture = i;
							break;
						}
						IndexPast = i;
						IndexFuture = -1;
					}
				}
				if(0 != (Direction & FlagDirection.PAST))
				{
					if(0 == (Direction & FlagDirection.FUTURE))
					{
						return(IndexPast);
					}
				}
				if(0 != (Direction & FlagDirection.FUTURE))
				{
					if(0 != (Direction & FlagDirection.PAST))
					{
						if(0 <= IndexPast)
						{
							if(0 <= IndexFuture)
							{
								KeyDataNow = TableKeyData[IndexPast];
								int TimePast = KeyDataNow.Time;
								KeyDataNow = TableKeyData[IndexFuture];
								int TimeFuture = KeyDataNow.Time;
								return(((TimePast - FrameNo) <= (TimeFuture - FrameNo)) ? IndexPast : IndexFuture);
							}
							else
							{
								return(IndexPast);
							}
						}
						else
						{
							return(IndexFuture);
						}
					}
					else
					{
						return(IndexFuture);
					}
				}
			}
			return(-1);
		}
	}

	[System.Serializable]
	public class PartsBase : MonoBehaviour
	{
		private FunctionCallBackOnTrigger functionOnTriggerEnter = null;
		public FunctionCallBackOnTrigger FunctionOnTriggerEnter
		{
			set
			{
				functionOnTriggerEnter = value;
			}
			get
			{
				return(functionOnTriggerEnter);
			}
		}
		private FunctionCallBackOnTrigger functionOnTriggerEnd = null;
		public FunctionCallBackOnTrigger FunctionOnTriggerEnd
		{
			set
			{
				functionOnTriggerEnd = value;
			}
			get
			{
				return(functionOnTriggerEnd);
			}
		}
		private FunctionCallBackOnTrigger functionOnTriggerStay = null;
		public FunctionCallBackOnTrigger FunctionOnTriggerStay
		{
			set
			{
				functionOnTriggerStay = value;
			}
			get
			{
				return(functionOnTriggerStay);
			}
		}

		private FunctionCallBackOnCollision functionOnCollisionEnter = null;
		public FunctionCallBackOnCollision FunctionOnCollisionEnter
		{
			set
			{
				functionOnCollisionEnter = value;
			}
			get
			{
				return(functionOnCollisionEnter);
			}
		}
		private FunctionCallBackOnCollision functionOnCollisionEnd = null;
		public FunctionCallBackOnCollision FunctionOnCollisionEnd
		{
			set
			{
				functionOnCollisionEnd = value;
			}
			get
			{
				return(functionOnCollisionEnd);
			}
		}
		private FunctionCallBackOnCollision functionOnCollisionStay = null;
		public FunctionCallBackOnCollision FunctionOnCollisionStay
		{
			set
			{
				functionOnCollisionStay = value;
			}
			get
			{
				return(functionOnCollisionStay);
			}
		}

		void OnTriggerEnter(Collider Pair)
		{
			if(null == functionOnTriggerEnter)
			{
				functionOnTriggerEnter(collider, Pair);
			}
		}

		void OnTriggerExit(Collider Pair)
		{
			if(null == functionOnTriggerEnd)
			{
				functionOnTriggerEnter(collider, Pair);
			}
		}

		void OnTriggerStay(Collider Pair)
		{
			if(null == functionOnTriggerStay)
			{
				functionOnTriggerStay(collider, Pair);
			}
		}

		void OnCollisionEnter(Collision Contacts)
		{
			if(null == functionOnCollisionEnter)
			{
				functionOnCollisionEnter(collider, Contacts);
			}
		}

		void OnCollisionExit(Collision Contacts)
		{
			if(null == functionOnCollisionEnd)
			{
				functionOnCollisionEnd(collider, Contacts);
			}
		}

		void OnCollisionStay(Collision Contacts)
		{
			if(null == functionOnCollisionStay)
			{
				functionOnCollisionStay(collider, Contacts);
			}
		}
	}

	[System.Serializable]
	public class SpriteBase : PartsBase
	{
		public enum BitStatus
		{
			UPDATE_COORDINATE = 0x0001,
			UPDATE_MAPPING = 0x0002,
			UPDATE_COLOR = 0x0004,
			UPDATE_EFFECT = 0x0008,
			UPDATE_MATERIAL = 0x0040,
			UPDATE_RENDERING = 0x0080,

			FLIP_TEXTURE_X = 0x0100,
			FLIP_TEXTURE_Y = 0x0200,

			RENDERING = 0x1000,
			USE_MESHRENDERER = 0x2000,
			AUTOMATIC = 0x4000,
			RUNNING = 0x8000,

			CLEAR = 0x0000,
			UPDATE_ALL = UPDATE_COORDINATE
						| UPDATE_MAPPING
						| UPDATE_COLOR
						| UPDATE_EFFECT
						| UPDATE_MATERIAL
						| UPDATE_RENDERING,
		};

		protected BitStatus Status;

		protected int NameIDMatrixTexture = -1;
		protected int NameIDRateColorVertex = -1;
		protected int NameIDLimitDepth = -1;

		protected Mesh dataMesh;
		public Mesh DataMesh
		{
			set
			{
				dataMesh = value;
			}
			get
			{
				return(dataMesh);
			}
		}

		protected Material[] dataMaterials;
		public Material[] DataMaterials
		{
			set
			{
				Status |= (dataMaterials != value) ? BitStatus.UPDATE_MATERIAL : BitStatus.CLEAR;
				dataMaterials = value;
			}
			get
			{
				return(dataMaterials);
			}
		}

		[SerializeField]
		protected Vector2 parameterEffectLU;
		public Vector2 ParameterEffectLU
		{
			set
			{
				Status |= ((value.x != parameterEffectLU.x) || (value.y != parameterEffectLU.y)) ? BitStatus.UPDATE_EFFECT : BitStatus.CLEAR;
				parameterEffectLU = value;
			}
			get
			{
				return(parameterEffectLU);
			}
		}
		public float RateOpacityLU
		{
			set
			{
				Status |= (value != parameterEffectLU.x) ? BitStatus.UPDATE_EFFECT : BitStatus.CLEAR;
				parameterEffectLU.x = value;
			}
			get
			{
				return(parameterEffectLU.x);
			}
		}
		public float RateEffectLU
		{
			set
			{
				Status |= (value != parameterEffectLU.y) ? BitStatus.UPDATE_EFFECT : BitStatus.CLEAR;
				parameterEffectLU.y = value;
			}
			get
			{
				return(parameterEffectLU.y);
			}
		}
		[SerializeField]
		protected Vector2 parameterEffectRU;
		public Vector2 ParameterEffectRU
		{
			set
			{
				Status |= ((value.x != parameterEffectRU.x) || (value.y != parameterEffectRU.y)) ? BitStatus.UPDATE_EFFECT : BitStatus.CLEAR;
				parameterEffectRU = value;
			}
			get
			{
				return(parameterEffectRU);
			}
		}
		public float RateOpacityRU
		{
			set
			{
				Status |= (value != parameterEffectRU.x) ? BitStatus.UPDATE_EFFECT : BitStatus.CLEAR;
				parameterEffectRU.x = value;
			}
			get
			{
				return(parameterEffectRU.x);
			}
		}
		public float RateEffectRU
		{
			set
			{
				Status |= (value != parameterEffectRU.y) ? BitStatus.UPDATE_EFFECT : BitStatus.CLEAR;
				parameterEffectRU.y = value;
			}
			get
			{
				return(parameterEffectRU.y);
			}
		}
		[SerializeField]
		protected Vector2 parameterEffectLD;
		public Vector2 ParameterEffectLD
		{
			set
			{
				Status |= ((value.x != parameterEffectLD.x) || (value.y != parameterEffectLD.y)) ? BitStatus.UPDATE_EFFECT : BitStatus.CLEAR;
				parameterEffectLD = value;
			}
			get
			{
				return(parameterEffectLD);
			}
		}
		public float RateOpacityLD
		{
			set
			{
				Status |= (value != parameterEffectLD.x) ? BitStatus.UPDATE_EFFECT : BitStatus.CLEAR;
				parameterEffectLD.x = value;
			}
			get
			{
				return(parameterEffectLD.x);
			}
		}
		public float RateEffectLD
		{
			set
			{
				Status |= (value != parameterEffectLD.y) ? BitStatus.UPDATE_EFFECT : BitStatus.CLEAR;
				parameterEffectLD.y = value;
			}
			get
			{
				return(parameterEffectLD.y);
			}
		}
		[SerializeField]
		protected Vector2 parameterEffectRD;
		public Vector2 ParameterEffectRD
		{
			set
			{
				Status |= ((value.x != parameterEffectRD.x) || (value.y != parameterEffectRD.y)) ? BitStatus.UPDATE_EFFECT : BitStatus.CLEAR;
				parameterEffectRD = value;
			}
			get
			{
				return(parameterEffectRD);
			}
		}
		public float RateOpacityRD
		{
			set
			{
				Status |= (value != parameterEffectRD.x) ? BitStatus.UPDATE_EFFECT : BitStatus.CLEAR;
				parameterEffectRD.x = value;
			}
			get
			{
				return(parameterEffectRD.x);
			}
		}
		public float RateEffectRD
		{
			set
			{
				Status |= (value != parameterEffectRD.y) ? BitStatus.UPDATE_EFFECT : BitStatus.CLEAR;
				parameterEffectRD.y = value;
			}
			get
			{
				return(parameterEffectRD.y);
			}
		}

		[SerializeField]
		protected Color32 vertexColorLU;
		public Color32 VertexColorLU
		{
			set
			{
				Status |= ((value.r != vertexColorLU.r) || (value.g != vertexColorLU.g) || (value.b != vertexColorLU.b) || (value.a != vertexColorLU.a))
							? BitStatus.UPDATE_COLOR : BitStatus.CLEAR;
				vertexColorLU = value;
			}
			get
			{
				return(vertexColorLU);
			}
		}
		[SerializeField]
		protected Color32 vertexColorRU;
		public Color32 VertexColorRU
		{
			set
			{
				Status |= ((value.r != vertexColorRU.r) || (value.g != vertexColorRU.g) || (value.b != vertexColorRU.b) || (value.a != vertexColorRU.a))
							? BitStatus.UPDATE_COLOR : BitStatus.CLEAR;
				vertexColorRU = value;
			}
			get
			{
				return(vertexColorRU);
			}
		}
		[SerializeField]
		protected Color32 vertexColorLD;
		public Color32 VertexColorLD
		{
			set
			{
				Status |= ((value.r != vertexColorLD.r) || (value.g != vertexColorLD.g) || (value.b != vertexColorLD.b) || (value.a != vertexColorLD.a))
							? BitStatus.UPDATE_COLOR : BitStatus.CLEAR;
				vertexColorLD = value;
			}
			get
			{
				return(vertexColorLD);
			}
		}
		[SerializeField]
		protected Color32 vertexColorRD;
		public Color32 VertexColorRD
		{
			set
			{
				Status |= ((value.r != vertexColorRD.r) || (value.g != vertexColorRD.g) || (value.b != vertexColorRD.b) || (value.a != vertexColorRD.a))
							? BitStatus.UPDATE_COLOR : BitStatus.CLEAR;
				vertexColorRD = value;
			}
			get
			{
				return(vertexColorRD);
			}
		}

		[SerializeField]
		protected Vector2 texturePivot;
		public Vector2 TexturePivot
		{
			set
			{
				if((texturePivot.x != value.x) || (texturePivot.y != value.y))
				{
					texturePivot = value;
					Status |= BitStatus.UPDATE_MAPPING;
				}
			}
			get
			{
				return(texturePivot);
			}
		}
		[SerializeField]
		protected Rect textureArea;
		public Rect TextureArea
		{
			set
			{
				if((textureArea.x != value.x) || (textureArea.y != value.y) || (textureArea.width != value.width) || (textureArea.height != value.height))
				{
					textureArea = value;
					Status |= BitStatus.UPDATE_MAPPING;
				}
			}
			get
			{
				return(textureArea);
			}
		}
		[SerializeField]
		protected Vector2 textureScale;
		public Vector2 TextureScale
		{
			set
			{
				if((textureScale.x != value.x) || (textureScale.y != value.y))
				{
					textureScale = value;
					Status |= BitStatus.UPDATE_MAPPING;
				}
			}
			get
			{
				return(textureScale);
			}
		}
		[SerializeField]
		protected float textureRotate;
		public float TextureRotate
		{
			set
			{
				if(textureRotate != value)
				{
					textureRotate = value;
					Status |= BitStatus.UPDATE_MAPPING;
				}
			}
			get
			{
				return(textureRotate);
			}
		}
		private Matrix4x4 MatrixTexture;

		[SerializeField]
		protected Vector2 planePivot;
		public Vector2 PlanePivot
		{
			set
			{
				Status |= ((value.x != planePivot.x) || (value.y != planePivot.y)) ? BitStatus.UPDATE_COLOR : BitStatus.CLEAR;
				planePivot = value;
			}
			get
			{
				return(planePivot);
			}
		}

		public void StatusSetRunning(bool FlagSwitch)
		{
			Status = (true == FlagSwitch) ? (Status | BitStatus.RUNNING) : (Status & ~BitStatus.RUNNING);
		}

		public bool StatusGetRunning()
		{
			return((0 != (Status & BitStatus.RUNNING)) ? true : false);
		}

		public void StatusSetAutomatic(bool FlagSwitch)
		{
			Status = (true == FlagSwitch) ? (Status | BitStatus.AUTOMATIC) : (Status & ~BitStatus.AUTOMATIC);
		}

		public bool StatusGetAutomatic()
		{
			return((0 != (Status & BitStatus.AUTOMATIC)) ? true : false);
		}

		public void StatusSetUseMeshRenderer(bool FlagSwitch)
		{
			Status = (true == FlagSwitch) ? (Status | BitStatus.USE_MESHRENDERER) : (Status & ~BitStatus.USE_MESHRENDERER);
		}

		public bool StatusGetUseMeshRenderer()
		{
			return((0 != (Status & BitStatus.USE_MESHRENDERER)) ? true : false);
		}

		public void StatusSetRendering(bool FlagSwitch)
		{
			if(true == FlagSwitch)
			{
				if(0 == (Status & BitStatus.RENDERING))
				{
					Status |= (BitStatus.UPDATE_RENDERING | BitStatus.RENDERING);
				}
			}
			else
			{
				if(0 != (Status & BitStatus.RENDERING))
				{
					Status &= ~BitStatus.RENDERING;
					Status |= BitStatus.UPDATE_RENDERING;
				}
			}
		}

		public bool StatusGetRendering()
		{
			return((0 != (Status & BitStatus.RENDERING)) ? true : false);
		}

		public void FlipXTextureMapping(bool FlagSwitch)
		{
			bool FlagFlip = (0 != (Status & BitStatus.FLIP_TEXTURE_X)) ? true : false;
			if(FlagSwitch != FlagFlip)
			{
				if(true == FlagSwitch)
				{
					Status |= BitStatus.FLIP_TEXTURE_X;
				}
				else
				{
					Status &= ~BitStatus.FLIP_TEXTURE_X;
				}
			}
			Status |= BitStatus.UPDATE_MAPPING;
		}

		public void FlipYTextureMapping(bool FlagSwitch)
		{
			bool FlagFlip = (0 != (Status & BitStatus.FLIP_TEXTURE_Y)) ? true : false;
			if(FlagSwitch != FlagFlip)
			{
				if(true == FlagSwitch)
				{
					Status |= BitStatus.FLIP_TEXTURE_Y;
				}
				else
				{
					Status &= ~BitStatus.FLIP_TEXTURE_Y;
				}
			}
			Status |= BitStatus.UPDATE_MAPPING;
		}

		protected void BaseExecStart()
		{
			Status = BitStatus.CLEAR;
			dataMesh = null;
			dataMaterials = null;

			StatusSetRendering(true);
			StatusSetAutomatic(true);
			StatusSetRunning(true);

			BaseExecUpdate();
		}

		protected bool BaseExecUpdate()
		{
			enabled = (0 != (Status & BitStatus.AUTOMATIC)) ? true : false;

			bool FlagDraw = StatusGetRendering();

			if(true == StatusGetUseMeshRenderer())
			{
				renderer.enabled = FlagDraw;
			}

			Status &= ~BitStatus.UPDATE_ALL;

			return(FlagDraw);
		}

		protected void MeshModify(	Mesh InstanceMesh,
									Texture InstanceTexture,
									Vector3[] ListCoordinateVertex,
									Vector3[] ListCoordinateUV0,
									Vector2[] ListCoordinateUV1,
									Color32[] ListColorVertex,
									Vector3[] ListNormal,
									int[] ListVertexIndex,
									bool FlagRecalcBound,
									bool FlagReoptimize
								)
		{
			int CountVertex = 0;

			if(null != ListCoordinateVertex)
			{
				InstanceMesh.vertices = ListCoordinateVertex;
			}

			if(null != ListCoordinateUV0)
			{
				if(null != InstanceTexture)
				{
					MatrixUpdateTextureMapping(InstanceTexture);
				}
				else
				{
					MatrixTexture = Matrix4x4.identity;
				}

				CountVertex = ListCoordinateUV0.Length;
				Vector2[] ListUVCoordinateNew = new Vector2[CountVertex];
				for(int i=0; i<CountVertex; i++)
				{
					ListUVCoordinateNew[i] = MatrixTexture.MultiplyPoint3x4(ListCoordinateUV0[i]);
				}

				InstanceMesh.uv = ListUVCoordinateNew;
			}

			if(null != ListCoordinateUV1)
			{
				InstanceMesh.uv2 = ListCoordinateUV1;
			}

			if(null != ListColorVertex)
			{
				InstanceMesh.colors32 = ListColorVertex;
			}

			if(null != ListNormal)
			{
				InstanceMesh.normals = ListNormal;
			}

			if(null != ListVertexIndex)
			{
				InstanceMesh.triangles = ListVertexIndex;
			}

			if(true == FlagRecalcBound)
			{
				InstanceMesh.RecalculateBounds();
			}

			if(true == FlagReoptimize)
			{
				InstanceMesh.Optimize();
			}
		}

		protected void NameIDGetShader(Shader InstanceShader)
		{
			if(null != InstanceShader)
			{
				NameIDMatrixTexture = Shader.PropertyToID("_MatrixTexture");
				NameIDRateColorVertex = Shader.PropertyToID("_RateColorVertex");
				NameIDLimitDepth = Shader.PropertyToID("_LimitDepth");
			}
			else
			{
				NameIDMatrixTexture = -1;
				NameIDRateColorVertex = -1;
				NameIDLimitDepth = -1;
			}
		}

		protected bool UpdateExecMaterial()
		{
			bool FlagUpdate = (0 != (Status & BitStatus.UPDATE_MATERIAL)) ? true : false;
			if((true == FlagUpdate) && (true == StatusGetUseMeshRenderer()))
			{
				renderer.sharedMaterials = dataMaterials;
			}
			return(FlagUpdate);
		}

		public void ParameterRenewTexture(ref Rect Area, ref Vector2 OriginTransform, Texture InstanceTexture)
		{
			if(-1 == Area.width)
			{
				Area.width = InstanceTexture.width - Area.x;
				Status |= BitStatus.UPDATE_MAPPING;
			}
			if(-1 == Area.height)
			{
				Area.height = InstanceTexture.height - Area.y;
				Status |= BitStatus.UPDATE_MAPPING;
			}
		}

		public void MatrixUpdateTextureMapping(Texture InstanceTexture)
		{
			float TextureWidth = (float)InstanceTexture.width;
			float TextureHeight = (float)InstanceTexture.height;

			float RateScaleX = (0 == (Status & BitStatus.FLIP_TEXTURE_X)) ? 1.0f : -1.0f;
			float RateScaleY = (0 == (Status & BitStatus.FLIP_TEXTURE_Y)) ? 1.0f : -1.0f;

			MatrixTexture = Matrix4x4.identity;
			Vector3 Translation = new Vector3(	((textureArea.xMin + texturePivot.x) / TextureWidth),
												((TextureHeight  - (textureArea.yMax - texturePivot.y)) / TextureHeight),
												0.0f
											);
			Vector3 Scaling = new Vector3(	((textureArea.width / TextureWidth) * textureScale.x) * RateScaleX,
											((textureArea.height / TextureHeight) * textureScale.y) * RateScaleY,
											1.0f
										);
			Quaternion Rotation = Quaternion.Euler(0.0f, 0.0f, textureRotate);
			MatrixTexture = Matrix4x4.TRS(Translation, Rotation, Scaling);
		}

	}

	[System.Serializable]
	public class AnimationInformationPlay
	{
		public string Name = "";
		public int FrameStart = 0;
		public int FrameEnd = 0;
		public int FramePerSecond = 0;
	}

	[System.Serializable]
	public class AnimationDataBasis
	{
		public KindInheritance Inheritance;
		public FlagAttributeKeyInherit FlagInheritance;
		public FlagAttributeKeyInherit FlagKeyParameter;
	}
	[System.Serializable]
	public class AnimationDataRuntime : AnimationDataBasis
	{
		public enum BitStatus
		{
			PIVOTPLANE_UV = 0x000001,
			TEXTURETRANSLATE_UV = 0x000002,

			CALLFUNCTION_PLAYEND = 0x000010,
			ENDFUNCTION_PLAYEND = 0x000020,
			REFRESH_PLAYFRAMENO = 0x000040,
			REFRESH_PLAYRANGENO = 0x000080,

			DISPLAY = 0x000100,
			FLIP_X = 0x000200,
			FLIP_Y = 0x000400,

			FLIP_TEXTURE_X = 0x001000,
			FLIP_TEXTURE_Y = 0x002000,

			CHANGE_CELL = 0x010000,
			CHANGE_COLLISION = 0x020000,

			FRAMEOVER		 = 0x100000,
			LOOP = 0x200000,
			PAUSING = 0x400000,
			PLAYING = 0x800000,

			CLEAR = 0x000000,
			MASK_RESET = 0x0000ff,
		};
		public enum KindKeyIndex
		{
			TOP = 0,
			NOW,
			NEXT,
			LAST,

			TERMINATOR,
		};

		protected	bool	CompositionedFlipX;
		protected	bool	CompositionedFlipY;
		protected	float	CompositionedOpacity;

		public BitStatus Status;
		public int ID;

		protected Vector3 Position;
		protected Vector3 Scale;
		protected Vector3 Rotate;

		public KindColorOperation KindBlendTarget;
		protected float RateOpacity;
		protected float Priority;

		protected int TextureNo;
		protected Vector2 TextureSizePixel;
		protected Rect TextureRectBase;
		protected Rect TextureRectOffset;
		protected Vector2 TextureScale;
		protected float TextureRotate;

		public Vector2 PlanePivotBase;
		protected Vector2 PlanePivotOffset;
		protected Vector2[] PlaneCoordinateOffset;

		protected KindColorOperation VertexColorOperation;
		protected Color32[] VertexColor = new Color32[4];
		protected float[] VertexColorPower = new float[4];

		protected Vector2 AnchorPosition;
		protected Vector2 AnchorSize;

		public KindCollision CollisionKind;
		public Collider CollisionComponent;
		protected float collisionRadius;
		public float CollisionRadius
		{
			set
			{
				if(collisionRadius != value)
				{
					switch(CollisionKind)
					{
						case KindCollision.CIRCLE:
						case KindCollision.CIRCLE_SCALEMINIMUM:
						case KindCollision.CIRCLE_SCALEMAXIMUM:
							Status |= BitStatus.CHANGE_COLLISION;
							break;
						default:
							break;
					}
					collisionRadius = value;
				}
			}
			get
			{
				return(collisionRadius);
			}
		}

		protected int[,] KeyDataIndex = new int[(int)KindKeyIndex.TERMINATOR, (int)KindAttributeKey.TERMINATOR];

		public KeyFrame.DataFloat[] AnimationDataPositionX;
		public KeyFrame.DataFloat[] AnimationDataPositionY;
		public KeyFrame.DataFloat[] AnimationDataPositionZ;
		public KeyFrame.DataFloat[] AnimationDataRotateX;
		public KeyFrame.DataFloat[] AnimationDataRotateY;
		public KeyFrame.DataFloat[] AnimationDataRotateZ;
		public KeyFrame.DataFloat[] AnimationDataScaleX;
		public KeyFrame.DataFloat[] AnimationDataScaleY;

		public KeyFrame.DataBool[] AnimationDataFlipX;
		public KeyFrame.DataBool[] AnimationDataFlipY;
		public KeyFrame.DataBool[] AnimationDataHide;

		public KeyFrame.DataFloat[] AnimationDataOpacityRate;
		public KeyFrame.DataFloat[] AnimationDataPriority;

		public KeyFrame.DataColor[] AnimationDataColorBlend;
		public KeyFrame.DataQuadrilateral[] AnimationDataVertexCorrection;
		public KeyFrame.DataFloat[] AnimationDataOriginOffsetX;
		public KeyFrame.DataFloat[] AnimationDataOriginOffsetY;

		public KeyFrame.DataFloat[] AnimationDataAnchorPositionX;
		public KeyFrame.DataFloat[] AnimationDataAnchorPositionY;
		public KeyFrame.DataFloat[] AnimationDataAnchorSizeX;
		public KeyFrame.DataFloat[] AnimationDataAnchorSizeY;

		public KeyFrame.DataBool[] AnimationDataTextureFlipX;
		public KeyFrame.DataBool[] AnimationDataTextureFlipY;

		public KeyFrame.DataFloat[] AnimationDataTextureTranslateX;
		public KeyFrame.DataFloat[] AnimationDataTextureTranslateY;
		public KeyFrame.DataFloat[] AnimationDataTextureRotate;
		public KeyFrame.DataFloat[] AnimationDataTextureScaleX;
		public KeyFrame.DataFloat[] AnimationDataTextureScaleY;
		public KeyFrame.DataFloat[] AnimationDataTextureExpandWidth;
		public KeyFrame.DataFloat[] AnimationDataTextureExpandHeight;

		public KeyFrame.DataFloat[] AnimationDataCollisionRadius;

		public KeyFrame.DataCell[] AnimationDataCell;
		public KeyFrame.DataUser[] AnimationDataUser;

		public KeyFrame.DataSound[] AnimationDataSound;
		public KeyFrame.DataParette[] AnimationDataPaletteChange;

		protected Script_SpriteStudio_PartsRoot PartsRoot;
		protected SpriteData PartsParent;
	}

	[System.Serializable]
	public class SpriteData: AnimationDataRuntime
	{
		public KindColorOperation ColorBlendKind;

		public void CleanUp()
		{
			Inheritance = KindInheritance.PARENT;
			FlagInheritance = Library_SpriteStudio.FlagAttributeKeyInherit.CLEAR;
			FlagKeyParameter = Library_SpriteStudio.FlagAttributeKeyInherit.CLEAR;

			CompositionedFlipX = false;
			CompositionedFlipY = false;
			CompositionedOpacity = 0.0f;

			Status = BitStatus.CLEAR;
			ID = -1;
			ColorBlendKind = KindColorOperation.NON;
			RateOpacity = 0.0f;
			Priority = 0.0f;

			TextureNo = -1;
			TextureRectBase = new Rect(0.0f, 0.0f, 0.0f, 0.0f);
			TextureRectOffset = new Rect(0.0f, 0.0f, 0.0f, 0.0f);
			TextureScale = Vector2.one;
			TextureRotate = 0.0f;

			PlanePivotBase = Vector2.zero;
			PlanePivotOffset = Vector2.zero;
			PlaneCoordinateOffset = null;

			VertexColorOperation = Library_SpriteStudio.KindColorOperation.NON;
			for(int i=0; i<4; i++)
			{
				VertexColor[i] = Color.white;
				VertexColorPower[i] = 1.0f;
			}

			AnchorPosition = Vector2.zero;
			AnchorSize = Vector2.one;

			CollisionKind = KindCollision.NON;
			CollisionComponent = null;
			CollisionRadius = 0.0f;

			KeyDataIndex = null;

			AnimationDataPositionX = null;
			AnimationDataPositionY = null;
			AnimationDataPositionZ = null;
			AnimationDataRotateX = null;
			AnimationDataRotateY = null;
			AnimationDataRotateZ = null;
			AnimationDataScaleX = null;
			AnimationDataScaleY = null;

			AnimationDataFlipX = null;
			AnimationDataFlipY = null;
			AnimationDataHide = null;

			AnimationDataOpacityRate = null;
			AnimationDataPriority = null;

			AnimationDataColorBlend = null;
			AnimationDataVertexCorrection = null;
			AnimationDataOriginOffsetX = null;
			AnimationDataOriginOffsetY = null;

			AnimationDataAnchorPositionX = null;
			AnimationDataAnchorPositionY = null;
			AnimationDataAnchorSizeX = null;
			AnimationDataAnchorSizeY = null;

			AnimationDataTextureFlipX = null;
			AnimationDataTextureFlipY = null;

			AnimationDataTextureTranslateX = null;
			AnimationDataTextureTranslateY = null;
			AnimationDataTextureRotate = null;
			AnimationDataTextureScaleX = null;
			AnimationDataTextureScaleY = null;
			AnimationDataTextureScaleY = null;
			AnimationDataTextureExpandWidth = null;
			AnimationDataTextureExpandHeight = null;

			AnimationDataCollisionRadius = null;

			AnimationDataCell = null;
			AnimationDataUser = null;

			AnimationDataSound = null;
			AnimationDataPaletteChange = null;

			PartsRoot = null;
			PartsParent = null;
		}

		public void BootUp()
		{
			Status &= BitStatus.MASK_RESET;
			RateOpacity = 1.0f;
			Priority = 0.0f;

			Position = Vector3.zero;
			Scale = Vector3.one;
			Rotate = Vector3.zero;

			TextureRectOffset = new Rect(0.0f, 0.0f, 0.0f, 0.0f);
			TextureScale = Vector2.one;
			TextureRotate = 0.0f;

			PlanePivotOffset = Vector2.zero;
			VertexColorOperation = Library_SpriteStudio.KindColorOperation.NON;
			for(int i=0; i<4; i++)
			{
				VertexColor[i] = Color.white;
				VertexColorPower[i] = 1.0f;
			}
			AnchorPosition = Vector2.zero;
			AnchorSize = Vector2.one;

			CollisionRadius = 0.0f;

			IndexSetPlayRange(-1);
			IndexSetPlayPoint(-1);

			if(null != AnimationDataVertexCorrection)
			{
				PlaneCoordinateOffset = new Vector2[4]
				{
					Vector2.zero,
					Vector2.zero,
					Vector2.zero,
					Vector2.zero
				};
			}

			CompositionedFlipX = false;
			CompositionedFlipY = false;
			CompositionedOpacity = 1.0f;
		}
		private void IndexSetPlayPoint(int FrameNo)
		{
			if(-1 == FrameNo)
			{	/* Clean-Up */
				for(int i=(int)KindAttributeKey.POSITION_X; i<(int)KindAttributeKey.TERMINATOR; i++)
				{
					KeyDataIndex[(int)KindKeyIndex.NOW, i] = 0;
				}
			}
			else
			{
				if(PartsRoot.FrameNoStart == FrameNo)
				{
					for(int i=(int)KindAttributeKey.POSITION_X; i<(int)KindAttributeKey.TERMINATOR; i++)
					{
						KeyDataIndex[(int)KindKeyIndex.NOW, i] = KeyDataIndex[(int)KindKeyIndex.TOP, i];
					}
				}
				else
				{
					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.POSITION_X] = KeyFrame.DataIndexGetFrame(AnimationDataPositionX, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.POSITION_X]);
					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.POSITION_Y] = KeyFrame.DataIndexGetFrame(AnimationDataPositionY, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.POSITION_Y]);
					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.POSITION_Z] = KeyFrame.DataIndexGetFrame(AnimationDataPositionZ, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.POSITION_Z]);

					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.ROTATE_X] = KeyFrame.DataIndexGetFrame(AnimationDataRotateX, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.ROTATE_X]);
					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.ROTATE_Y] = KeyFrame.DataIndexGetFrame(AnimationDataRotateY, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.ROTATE_Y]);
					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.ROTATE_Z] = KeyFrame.DataIndexGetFrame(AnimationDataRotateZ, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.ROTATE_Z]);

					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.SCALE_X] = KeyFrame.DataIndexGetFrame(AnimationDataScaleX, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.SCALE_X]);
					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.SCALE_Y] = KeyFrame.DataIndexGetFrame(AnimationDataScaleY, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.SCALE_Y]);

					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.FLIP_X] = KeyFrame.DataIndexGetFrame(AnimationDataFlipX, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.FLIP_X]);
					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.FLIP_Y] = KeyFrame.DataIndexGetFrame(AnimationDataFlipY, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.FLIP_Y]);
					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.SHOW_HIDE] = KeyFrame.DataIndexGetFrame(AnimationDataHide, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.SHOW_HIDE]);

					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.OPACITY_RATE] = KeyFrame.DataIndexGetFrame(AnimationDataOpacityRate, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.OPACITY_RATE]);
					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.PRIORITY] = KeyFrame.DataIndexGetFrame(AnimationDataPriority, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.PRIORITY]);

					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.COLOR_BLEND] = KeyFrame.DataIndexGetFrame(AnimationDataColorBlend, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.COLOR_BLEND]);
					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.VERTEX_CORRECTION] = KeyFrame.DataIndexGetFrame(AnimationDataVertexCorrection, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.VERTEX_CORRECTION]);
					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.ORIGIN_OFFSET_X] = KeyFrame.DataIndexGetFrame(AnimationDataOriginOffsetX, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.ORIGIN_OFFSET_X]);
					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.ORIGIN_OFFSET_Y] = KeyFrame.DataIndexGetFrame(AnimationDataOriginOffsetY, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.ORIGIN_OFFSET_Y]);

					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.ANCHOR_POSITION_X] = KeyFrame.DataIndexGetFrame(AnimationDataAnchorPositionX, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.ANCHOR_POSITION_X]);
					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.ANCHOR_POSITION_Y] = KeyFrame.DataIndexGetFrame(AnimationDataAnchorPositionY, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.ANCHOR_POSITION_Y]);
					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.ANCHOR_SIZE_X] = KeyFrame.DataIndexGetFrame(AnimationDataAnchorSizeX, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.ANCHOR_SIZE_X]);
					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.ANCHOR_SIZE_Y] = KeyFrame.DataIndexGetFrame(AnimationDataAnchorSizeY, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.ANCHOR_SIZE_Y]);

					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.TEXTURE_FLIP_X] = KeyFrame.DataIndexGetFrame(AnimationDataTextureTranslateX, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.TEXTURE_FLIP_X]);
					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.TEXTURE_FLIP_Y] = KeyFrame.DataIndexGetFrame(AnimationDataTextureTranslateY, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.TEXTURE_FLIP_Y]);

					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.TEXTURE_TRANSLATE_X] = KeyFrame.DataIndexGetFrame(AnimationDataTextureTranslateX, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.TEXTURE_TRANSLATE_X]);
					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.TEXTURE_TRANSLATE_Y] = KeyFrame.DataIndexGetFrame(AnimationDataTextureTranslateY, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.TEXTURE_TRANSLATE_Y]);
					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.TEXTURE_ROTATE] = KeyFrame.DataIndexGetFrame(AnimationDataTextureRotate, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.TEXTURE_ROTATE]);
					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.TEXTURE_SCALE_X] = KeyFrame.DataIndexGetFrame(AnimationDataTextureScaleX, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.TEXTURE_SCALE_X]);
					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.TEXTURE_SCALE_Y] = KeyFrame.DataIndexGetFrame(AnimationDataTextureScaleY, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.TEXTURE_SCALE_Y]);
					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.TEXTURE_EXPAND_WIDTH] = KeyFrame.DataIndexGetFrame(AnimationDataTextureExpandWidth, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.TEXTURE_EXPAND_WIDTH]);
					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.TEXTURE_EXPAND_HEIGHT] = KeyFrame.DataIndexGetFrame(AnimationDataTextureExpandHeight, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.TEXTURE_EXPAND_HEIGHT]);

					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.COLLISION_RADIUS] = KeyFrame.DataIndexGetFrame(AnimationDataCollisionRadius, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.COLLISION_RADIUS]);
					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.CELL] = KeyFrame.DataIndexGetFrame(AnimationDataCell, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.CELL]);
					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.USER_DATA] = KeyFrame.DataIndexGetFrame(AnimationDataUser, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.USER_DATA]);

					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.SOUND] = KeyFrame.DataIndexGetFrame(AnimationDataSound, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.SOUND]);
					KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.PALETTE_CHANGE] = KeyFrame.DataIndexGetFrame(AnimationDataPaletteChange, FrameNo, FlagDirection.JUSTNOW | FlagDirection.PAST, KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.PALETTE_CHANGE]);
				}
			}
			for(int i=(int)KindAttributeKey.POSITION_X; i<(int)KindAttributeKey.TERMINATOR; i++)
			{
				KeyDataIndex[(int)KindKeyIndex.NEXT, i] = KeyDataIndex[(int)KindKeyIndex.NOW, i] + 1;
				if(KeyDataIndex[(int)KindKeyIndex.LAST, i] < KeyDataIndex[(int)KindKeyIndex.NEXT, i])
				{
					KeyDataIndex[(int)KindKeyIndex.NEXT, i] = -1;
				}
			}
		}
		private void IndexSetPlayRange(int AnimationNo)
		{
			if(-1 == AnimationNo)
			{	/* Clean-Up */
				for(int i=(int)KindAttributeKey.POSITION_X; i<(int)KindAttributeKey.TERMINATOR; i++)
				{
					KeyDataIndex[(int)KindKeyIndex.TOP, i] = 0;
				}

				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.POSITION_X] = KeyIndexGetLast(AnimationDataPositionX);
				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.POSITION_Y] = KeyIndexGetLast(AnimationDataPositionY);
				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.POSITION_Z] = KeyIndexGetLast(AnimationDataPositionZ);

				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.ROTATE_X] = KeyIndexGetLast(AnimationDataRotateX);
				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.ROTATE_Y] = KeyIndexGetLast(AnimationDataRotateY);
				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.ROTATE_Z] = KeyIndexGetLast(AnimationDataRotateZ);

				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.SCALE_X] = KeyIndexGetLast(AnimationDataScaleX);
				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.SCALE_Y] = KeyIndexGetLast(AnimationDataScaleY);

				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.FLIP_X] = KeyIndexGetLast(AnimationDataFlipX);
				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.FLIP_Y] = KeyIndexGetLast(AnimationDataFlipY);
				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.SHOW_HIDE] = KeyIndexGetLast(AnimationDataHide);

				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.OPACITY_RATE] = KeyIndexGetLast(AnimationDataOpacityRate);
				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.PRIORITY] = KeyIndexGetLast(AnimationDataPriority);

				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.COLOR_BLEND] = KeyIndexGetLast(AnimationDataColorBlend);
				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.VERTEX_CORRECTION] = KeyIndexGetLast(AnimationDataVertexCorrection);
				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.ORIGIN_OFFSET_X] = KeyIndexGetLast(AnimationDataOriginOffsetX);
				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.ORIGIN_OFFSET_Y] = KeyIndexGetLast(AnimationDataOriginOffsetY);

				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.ANCHOR_POSITION_X] = KeyIndexGetLast(AnimationDataAnchorPositionX);
				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.ANCHOR_POSITION_Y] = KeyIndexGetLast(AnimationDataAnchorPositionY);
				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.ANCHOR_SIZE_X] = KeyIndexGetLast(AnimationDataAnchorSizeX);
				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.ANCHOR_SIZE_Y] = KeyIndexGetLast(AnimationDataAnchorSizeY);

				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.TEXTURE_FLIP_X] = KeyIndexGetLast(AnimationDataTextureFlipX);
				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.TEXTURE_FLIP_Y] = KeyIndexGetLast(AnimationDataTextureFlipY);

				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.TEXTURE_TRANSLATE_X] = KeyIndexGetLast(AnimationDataTextureTranslateX);
				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.TEXTURE_TRANSLATE_Y] = KeyIndexGetLast(AnimationDataTextureTranslateY);
				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.TEXTURE_ROTATE] = KeyIndexGetLast(AnimationDataTextureRotate);
				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.TEXTURE_SCALE_X] = KeyIndexGetLast(AnimationDataTextureScaleX);
				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.TEXTURE_SCALE_Y] = KeyIndexGetLast(AnimationDataTextureScaleY);
				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.TEXTURE_EXPAND_WIDTH] = KeyIndexGetLast(AnimationDataTextureExpandWidth);
				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.TEXTURE_EXPAND_HEIGHT] = KeyIndexGetLast(AnimationDataTextureExpandHeight);

				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.COLLISION_RADIUS] = KeyIndexGetLast(AnimationDataCollisionRadius);
				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.CELL] = KeyIndexGetLast(AnimationDataCell);
				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.USER_DATA] = KeyIndexGetLast(AnimationDataUser);

				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.SOUND] = KeyIndexGetLast(AnimationDataSound);
				KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.PALETTE_CHANGE] = KeyIndexGetLast(AnimationDataPaletteChange);
			}
			else
			{
				int FrameStart = PartsRoot.ListInformationPlay[AnimationNo].FrameStart;
				int FrameEnd = PartsRoot.ListInformationPlay[AnimationNo].FrameEnd;

				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.POSITION_X], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.POSITION_X], AnimationDataPositionX, FrameStart, FrameEnd);
				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.POSITION_Y], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.POSITION_Y], AnimationDataPositionY, FrameStart, FrameEnd);
				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.POSITION_Z], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.POSITION_Z], AnimationDataPositionZ, FrameStart, FrameEnd);

				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.ROTATE_X], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.ROTATE_X], AnimationDataRotateX, FrameStart, FrameEnd);
				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.ROTATE_Y], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.ROTATE_Y], AnimationDataRotateY, FrameStart, FrameEnd);
				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.ROTATE_Z], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.ROTATE_Z], AnimationDataRotateZ, FrameStart, FrameEnd);

				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.SCALE_X], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.SCALE_X], AnimationDataScaleX, FrameStart, FrameEnd);
				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.SCALE_Y], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.SCALE_Y], AnimationDataScaleY, FrameStart, FrameEnd);

				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.FLIP_X], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.FLIP_X], AnimationDataFlipX, FrameStart, FrameEnd);
				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.FLIP_Y], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.FLIP_Y], AnimationDataFlipY, FrameStart, FrameEnd);
				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.SHOW_HIDE], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.SHOW_HIDE], AnimationDataHide, FrameStart, FrameEnd);

				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.OPACITY_RATE], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.OPACITY_RATE], AnimationDataOpacityRate, FrameStart, FrameEnd);
				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.PRIORITY], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.PRIORITY], AnimationDataPriority, FrameStart, FrameEnd);

				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.COLOR_BLEND], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.COLOR_BLEND], AnimationDataColorBlend, FrameStart, FrameEnd);
				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.VERTEX_CORRECTION], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.VERTEX_CORRECTION], AnimationDataVertexCorrection, FrameStart, FrameEnd);
				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.ORIGIN_OFFSET_X], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.ORIGIN_OFFSET_X], AnimationDataOriginOffsetX, FrameStart, FrameEnd);
				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.ORIGIN_OFFSET_Y], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.ORIGIN_OFFSET_Y], AnimationDataOriginOffsetY, FrameStart, FrameEnd);

				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.ANCHOR_POSITION_X], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.ANCHOR_POSITION_X], AnimationDataAnchorPositionX, FrameStart, FrameEnd);
				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.ANCHOR_POSITION_Y], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.ANCHOR_POSITION_Y], AnimationDataAnchorPositionY, FrameStart, FrameEnd);
				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.ANCHOR_SIZE_X], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.ANCHOR_SIZE_X], AnimationDataAnchorSizeX, FrameStart, FrameEnd);
				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.ANCHOR_SIZE_Y], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.ANCHOR_SIZE_Y], AnimationDataAnchorSizeY, FrameStart, FrameEnd);

				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.TEXTURE_FLIP_X], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.TEXTURE_FLIP_X], AnimationDataTextureFlipX, FrameStart, FrameEnd);
				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.TEXTURE_FLIP_Y], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.TEXTURE_FLIP_Y], AnimationDataTextureFlipY, FrameStart, FrameEnd);

				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.TEXTURE_TRANSLATE_X], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.TEXTURE_TRANSLATE_X], AnimationDataTextureTranslateX, FrameStart, FrameEnd);
				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.TEXTURE_TRANSLATE_Y], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.TEXTURE_TRANSLATE_Y], AnimationDataTextureTranslateY, FrameStart, FrameEnd);
				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.TEXTURE_ROTATE], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.TEXTURE_ROTATE], AnimationDataTextureRotate, FrameStart, FrameEnd);
				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.TEXTURE_SCALE_X], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.TEXTURE_SCALE_X], AnimationDataTextureScaleX, FrameStart, FrameEnd);
				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.TEXTURE_SCALE_Y], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.TEXTURE_SCALE_Y], AnimationDataTextureScaleY, FrameStart, FrameEnd);
				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.TEXTURE_EXPAND_WIDTH], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.TEXTURE_EXPAND_WIDTH], AnimationDataTextureExpandWidth, FrameStart, FrameEnd);
				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.TEXTURE_EXPAND_HEIGHT], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.TEXTURE_EXPAND_HEIGHT], AnimationDataTextureExpandHeight, FrameStart, FrameEnd);

				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.COLLISION_RADIUS], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.COLLISION_RADIUS], AnimationDataCollisionRadius, FrameStart, FrameEnd);
				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.CELL], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.CELL], AnimationDataCell, FrameStart, FrameEnd);
				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.USER_DATA], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.USER_DATA], AnimationDataUser, FrameStart, FrameEnd);

				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.SOUND], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.SOUND], AnimationDataSound, FrameStart, FrameEnd);
				KeyIndexSetRange(ref KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.PALETTE_CHANGE], ref KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.PALETTE_CHANGE], AnimationDataPaletteChange, FrameStart, FrameEnd);
			}
		}

		public void	PartsSetRoot(Script_SpriteStudio_PartsRoot Root)
		{
			PartsRoot = Root;
		}

		public void	PartsSetParent(SpriteData Parent)
		{
			PartsParent = Parent;
		}

		public void AnimationUpdate(GameObject InstanceGameObject)
		{
			int FrameNo = PartsRoot.FrameNoNow;
			int FrameNoStart = PartsRoot.FrameNoStart;
			int FrameNoEnd = PartsRoot.FrameNoEnd;
			bool FlagLoop = (0 != (PartsRoot.SpriteStudioData.Status & BitStatus.LOOP)) ? true : false;
			bool FlagFrameOver = (0 != (PartsRoot.SpriteStudioData.Status & BitStatus.FRAMEOVER)) ? true : false;

			if(0 != (PartsRoot.SpriteStudioData.Status & BitStatus.REFRESH_PLAYRANGENO))
			{
				IndexSetPlayRange(PartsRoot.AnimationNo);
			}
			if(0 != (PartsRoot.SpriteStudioData.Status & BitStatus.REFRESH_PLAYFRAMENO))
			{
				IndexSetPlayPoint(FrameNo);
			}

			KeyFrame.DataFloat KeyDataPositionX = KeyIndexUpdate(	AnimationDataPositionX,
																	KindAttributeKey.POSITION_X,
																	FrameNo,
																	FrameNoStart,
																	FrameNoEnd,
																	FlagFrameOver,
																	FlagLoop
																);
			KeyFrame.DataFloat KeyDataPositionY = KeyIndexUpdate(	AnimationDataPositionY,
																	KindAttributeKey.POSITION_Y,
																	FrameNo,
																	FrameNoStart,
																	FrameNoEnd,
																	FlagFrameOver,
																	FlagLoop
																	);
			KeyFrame.DataFloat KeyDataPositionZ = KeyIndexUpdate(	AnimationDataPositionZ,
																	KindAttributeKey.POSITION_Z,
																	FrameNo,
																	FrameNoStart,
																	FrameNoEnd,
																	FlagFrameOver,
																	FlagLoop
																	);
			KeyFrame.DataFloat KeyDataRotateX = KeyIndexUpdate(	AnimationDataRotateX,
																KindAttributeKey.ROTATE_X,
																FrameNo,
																FrameNoStart,
																FrameNoEnd,
																FlagFrameOver,
																FlagLoop
															);
			KeyFrame.DataFloat KeyDataRotateY = KeyIndexUpdate(	AnimationDataRotateY,
																KindAttributeKey.ROTATE_Y,
																FrameNo,
																FrameNoStart,
																FrameNoEnd,
																FlagFrameOver,
																FlagLoop
															);
			KeyFrame.DataFloat KeyDataRotateZ = KeyIndexUpdate(	AnimationDataRotateZ,
																KindAttributeKey.ROTATE_Z,
																FrameNo,
																FrameNoStart,
																FrameNoEnd,
																FlagFrameOver,
																FlagLoop
															);
			KeyFrame.DataFloat KeyDataScaleX = KeyIndexUpdate(	AnimationDataScaleX,
																KindAttributeKey.SCALE_X,
																FrameNo,
																FrameNoStart,
																FrameNoEnd,
																FlagFrameOver,
																FlagLoop
															);
			KeyFrame.DataFloat KeyDataScaleY = KeyIndexUpdate(	AnimationDataScaleY,
																KindAttributeKey.SCALE_Y,
																FrameNo,
																FrameNoStart,
																FrameNoEnd,
																FlagFrameOver,
																FlagLoop
															);

			KeyFrame.DataBool KeyDataFlipX = KeyIndexUpdate(	AnimationDataFlipX,
																KindAttributeKey.FLIP_X,
																FrameNo,
																FrameNoStart,
																FrameNoEnd,
																FlagFrameOver,
																FlagLoop
															);
			KeyFrame.DataBool KeyDataFlipY = KeyIndexUpdate(	AnimationDataFlipY,
																KindAttributeKey.FLIP_Y,
																FrameNo,
																FrameNoStart,
																FrameNoEnd,
																FlagFrameOver,
																FlagLoop
															);
			KeyFrame.DataBool KeyDataHide = KeyIndexUpdate(	AnimationDataHide,
															KindAttributeKey.SHOW_HIDE,
															FrameNo,
															FrameNoStart,
															FrameNoEnd,
															FlagFrameOver,
															FlagLoop
														);

			KeyFrame.DataFloat KeyDataOpacityRate = KeyIndexUpdate(	AnimationDataOpacityRate,
																	KindAttributeKey.OPACITY_RATE,
																	FrameNo,
																	FrameNoStart,
																	FrameNoEnd,
																	FlagFrameOver,
																	FlagLoop
																);
			KeyFrame.DataFloat KeyDataPriority = KeyIndexUpdate(	AnimationDataPriority,
																	KindAttributeKey.PRIORITY,
																	FrameNo,
																	FrameNoStart,
																	FrameNoEnd,
																	FlagFrameOver,
																	FlagLoop
																);
			KeyFrame.DataColor KeyDataColorBlend = KeyIndexUpdate(	AnimationDataColorBlend,
																	KindAttributeKey.COLOR_BLEND,
																	FrameNo,
																	FrameNoStart,
																	FrameNoEnd,
																	FlagFrameOver,
																	FlagLoop
																);
			KeyFrame.DataQuadrilateral KeyDataVertexCorrection = KeyIndexUpdate(	AnimationDataVertexCorrection,
																					KindAttributeKey.VERTEX_CORRECTION,
																					FrameNo,
																					FrameNoStart,
																					FrameNoEnd,
																					FlagFrameOver,
																					FlagLoop
																				);
			KeyFrame.DataFloat KeyDataOriginOffsetX = KeyIndexUpdate(	AnimationDataOriginOffsetX,
																		KindAttributeKey.ORIGIN_OFFSET_X,
																		FrameNo,
																		FrameNoStart,
																		FrameNoEnd,
																		FlagFrameOver,
																		FlagLoop
																	);
			KeyFrame.DataFloat KeyDataOriginOffsetY = KeyIndexUpdate(	AnimationDataOriginOffsetY,
																		KindAttributeKey.ORIGIN_OFFSET_Y,
																		FrameNo,
																		FrameNoStart,
																		FrameNoEnd,
																		FlagFrameOver,
																		FlagLoop
																	);

			KeyFrame.DataFloat KeyDataAnchorPositionX = KeyIndexUpdate(	AnimationDataAnchorPositionX,
																		KindAttributeKey.ANCHOR_POSITION_X,
																		FrameNo,
																		FrameNoStart,
																		FrameNoEnd,
																		FlagFrameOver,
																		FlagLoop
																	);
			KeyFrame.DataFloat KeyDataAnchorPositionY = KeyIndexUpdate(	AnimationDataAnchorPositionY,
																		KindAttributeKey.ANCHOR_POSITION_Y,
																		FrameNo,
																		FrameNoStart,
																		FrameNoEnd,
																		FlagFrameOver,
																		FlagLoop
																	);
			KeyFrame.DataFloat KeyDataAnchorSizeX = KeyIndexUpdate(	AnimationDataAnchorSizeX,
																	KindAttributeKey.ANCHOR_SIZE_X,
																	FrameNo,
																	FrameNoStart,
																	FrameNoEnd,
																	FlagFrameOver,
																	FlagLoop
																);
			KeyFrame.DataFloat KeyDataAnchorSizeY = KeyIndexUpdate(	AnimationDataAnchorSizeY,
																	KindAttributeKey.ANCHOR_SIZE_Y,
																	FrameNo,
																	FrameNoStart,
																	FrameNoEnd,
																	FlagFrameOver,
																	FlagLoop
																);

			KeyFrame.DataBool KeyDataTextureFlipX = KeyIndexUpdate(	AnimationDataTextureFlipX,
																	KindAttributeKey.TEXTURE_FLIP_X,
																	FrameNo,
																	FrameNoStart,
																	FrameNoEnd,
																	FlagFrameOver,
																	FlagLoop
																);
			KeyFrame.DataBool KeyDataTextureFlipY = KeyIndexUpdate(	AnimationDataTextureFlipY,
																	KindAttributeKey.TEXTURE_FLIP_Y,
																	FrameNo,
																	FrameNoStart,
																	FrameNoEnd,
																	FlagFrameOver,
																	FlagLoop
																);

			KeyFrame.DataFloat KeyDataTextureTranslateX = KeyIndexUpdate(	AnimationDataTextureTranslateX,
																			KindAttributeKey.TEXTURE_TRANSLATE_X,
																			FrameNo,
																			FrameNoStart,
																			FrameNoEnd,
																			FlagFrameOver,
																			FlagLoop
																		);
			KeyFrame.DataFloat KeyDataTextureTranslateY = KeyIndexUpdate(	AnimationDataTextureTranslateY,
																			KindAttributeKey.TEXTURE_TRANSLATE_Y,
																			FrameNo,
																			FrameNoStart,
																			FrameNoEnd,
																			FlagFrameOver,
																			FlagLoop
																		);
			KeyFrame.DataFloat KeyDataTextureRotate = KeyIndexUpdate(	AnimationDataTextureRotate,
																		KindAttributeKey.TEXTURE_ROTATE,
																		FrameNo,
																		FrameNoStart,
																		FrameNoEnd,
																		FlagFrameOver,
																		FlagLoop
																	);
			KeyFrame.DataFloat KeyDataTextureScaleX = KeyIndexUpdate(	AnimationDataTextureScaleX,
																		KindAttributeKey.TEXTURE_SCALE_X,
																		FrameNo,
																		FrameNoStart,
																		FrameNoEnd,
																		FlagFrameOver,
																		FlagLoop
																	);
			KeyFrame.DataFloat KeyDataTextureScaleY = KeyIndexUpdate(	AnimationDataTextureScaleY,
																		KindAttributeKey.TEXTURE_SCALE_Y,
																		FrameNo,
																		FrameNoStart,
																		FrameNoEnd,
																		FlagFrameOver,
																		FlagLoop
																	);
			KeyFrame.DataFloat KeyDataTextureExpandWidth = KeyIndexUpdate(	AnimationDataTextureExpandWidth,
																			KindAttributeKey.TEXTURE_EXPAND_WIDTH,
																			FrameNo,
																			FrameNoStart,
																			FrameNoEnd,
																			FlagFrameOver,
																			FlagLoop
																		);
			KeyFrame.DataFloat KeyDataTextureExpandHeight = KeyIndexUpdate(	AnimationDataTextureExpandHeight,
																			KindAttributeKey.TEXTURE_EXPAND_HEIGHT,
																			FrameNo,
																			FrameNoStart,
																			FrameNoEnd,
																			FlagFrameOver,
																			FlagLoop
																		);

			KeyFrame.DataFloat KeyDataCollisionRadius = KeyIndexUpdate(	AnimationDataCollisionRadius,
																		KindAttributeKey.COLLISION_RADIUS,
																		FrameNo,
																		FrameNoStart,
																		FrameNoEnd,
																		FlagFrameOver,
																		FlagLoop
																	);

			KeyFrame.DataCell KeyDataCell = KeyIndexUpdate(	AnimationDataCell,
															KindAttributeKey.CELL,
															FrameNo,
															FrameNoStart,
															FrameNoEnd,
															FlagFrameOver,
															FlagLoop
														);

			KeyIndexUpdateDataUser(	AnimationDataUser,
									InstanceGameObject,
									FrameNo,
									FrameNoStart,
									FrameNoEnd,
									FlagFrameOver,
									FlagLoop
								);	/* IndexUpdate & Request-CallBack */

			bool	FlagFlipX = KeyDataGetBoolInterpolationNon(KeyDataFlipX, FrameNo, false);
			bool	FlagFlipY = KeyDataGetBoolInterpolationNon(KeyDataFlipY, FrameNo, false);
			Status = (true == KeyDataGetBoolInterpolationNon(KeyDataHide, FrameNo, true)) ? (Status & ~BitStatus.DISPLAY) : (Status | BitStatus.DISPLAY);
			Status = (true == FlagFlipX) ? (Status | BitStatus.FLIP_X) : (Status & ~BitStatus.FLIP_X);
			Status = (true == FlagFlipY) ? (Status | BitStatus.FLIP_Y) : (Status & ~BitStatus.FLIP_Y);

			Position.x = KeyDataGetFloatInterpolation(KeyDataPositionX, AnimationDataPositionX, FrameNo, FrameNoStart, FrameNoEnd, 0.0f);
			Position.y = KeyDataGetFloatInterpolation(KeyDataPositionY, AnimationDataPositionY, FrameNo, FrameNoStart, FrameNoEnd, 0.0f);
			Position.z = KeyDataGetFloatInterpolation(KeyDataPositionZ, AnimationDataPositionZ, FrameNo, FrameNoStart, FrameNoEnd, 0.0f);

			Priority = KeyDataGetFloatInterpolation(KeyDataPriority, AnimationDataPriority, FrameNo, FrameNoStart, FrameNoEnd, 0.0f);

			Scale.x = KeyDataGetFloatInterpolation(KeyDataScaleX, AnimationDataScaleX, FrameNo, FrameNoStart, FrameNoEnd, 1.0f);
			Scale.y = KeyDataGetFloatInterpolation(KeyDataScaleY, AnimationDataScaleY, FrameNo, FrameNoStart, FrameNoEnd, 1.0f);
			Scale.z = InstanceGameObject.transform.localScale.z;

			Rotate.x = KeyDataGetFloatInterpolation(KeyDataRotateX, AnimationDataRotateX, FrameNo, FrameNoStart, FrameNoEnd, 0.0f);
			Rotate.y = KeyDataGetFloatInterpolation(KeyDataRotateY, AnimationDataRotateY, FrameNo, FrameNoStart, FrameNoEnd, 0.0f);
			Rotate.z = KeyDataGetFloatInterpolation(KeyDataRotateZ, AnimationDataRotateZ, FrameNo, FrameNoStart, FrameNoEnd, 0.0f);

			RateOpacity = KeyDataGetFloatInterpolation(KeyDataOpacityRate, AnimationDataOpacityRate, FrameNo, FrameNoStart, FrameNoEnd, 1.0f);

			Status = (true == KeyDataGetBoolInterpolationNon(KeyDataTextureFlipX, FrameNo, false)) ? (Status | BitStatus.FLIP_TEXTURE_X) : (Status & ~BitStatus.FLIP_TEXTURE_X);
			Status = (true == KeyDataGetBoolInterpolationNon(KeyDataTextureFlipY, FrameNo, false)) ? (Status | BitStatus.FLIP_TEXTURE_Y) : (Status & ~BitStatus.FLIP_TEXTURE_Y);

			TextureRectOffset.x = KeyDataGetFloatInterpolation(KeyDataTextureTranslateX, AnimationDataTextureTranslateX, FrameNo, FrameNoStart, FrameNoEnd, 0.0f);
			TextureRectOffset.y = KeyDataGetFloatInterpolation(KeyDataTextureTranslateY, AnimationDataTextureTranslateY, FrameNo, FrameNoStart, FrameNoEnd, 0.0f);
			TextureRectOffset.width = KeyDataGetFloatInterpolation(KeyDataTextureExpandWidth, AnimationDataTextureExpandWidth, FrameNo, FrameNoStart, FrameNoEnd, 0.0f);
			TextureRectOffset.height = KeyDataGetFloatInterpolation(KeyDataTextureExpandHeight, AnimationDataTextureExpandHeight, FrameNo, FrameNoStart, FrameNoEnd, 0.0f);
			TextureRotate = KeyDataGetFloatInterpolation(KeyDataTextureRotate, AnimationDataTextureRotate, FrameNo, FrameNoStart, FrameNoEnd, 0.0f);
			TextureScale.x = KeyDataGetFloatInterpolation(KeyDataTextureScaleX, AnimationDataTextureScaleX, FrameNo, FrameNoStart, FrameNoEnd, 1.0f);
			TextureScale.y = KeyDataGetFloatInterpolation(KeyDataTextureScaleY, AnimationDataTextureScaleY, FrameNo, FrameNoStart, FrameNoEnd, 1.0f);

			PlanePivotOffset.x = KeyDataGetFloatInterpolation(KeyDataOriginOffsetX, AnimationDataOriginOffsetX, FrameNo, FrameNoStart, FrameNoEnd, 0.0f);
			PlanePivotOffset.y = KeyDataGetFloatInterpolation(KeyDataOriginOffsetY, AnimationDataOriginOffsetY, FrameNo, FrameNoStart, FrameNoEnd, 0.0f);

			AnchorPosition.x = KeyDataGetFloatInterpolation(KeyDataAnchorPositionX, AnimationDataAnchorPositionX, FrameNo, FrameNoStart, FrameNoEnd, 0.0f);
			AnchorPosition.y = KeyDataGetFloatInterpolation(KeyDataAnchorPositionY, AnimationDataAnchorPositionY, FrameNo, FrameNoStart, FrameNoEnd, 0.0f);
			AnchorSize.x = KeyDataGetFloatInterpolation(KeyDataAnchorSizeX, AnimationDataAnchorSizeX, FrameNo, FrameNoStart, FrameNoEnd, 1.0f);
			AnchorSize.y = KeyDataGetFloatInterpolation(KeyDataAnchorSizeY, AnimationDataAnchorSizeY, FrameNo, FrameNoStart, FrameNoEnd, 1.0f);

			CollisionRadius = KeyDataGetFloatInterpolation(KeyDataCollisionRadius, AnimationDataCollisionRadius, FrameNo, FrameNoStart, FrameNoEnd, 0.0f);

			KeyDataGetColorBlendInterpolation(ref VertexColorOperation, VertexColor, VertexColorPower, KeyDataColorBlend, AnimationDataColorBlend, FrameNo, FrameNoStart, FrameNoEnd);

			KeyDataGetQuadrilateralInterpolation(PlaneCoordinateOffset, KeyDataVertexCorrection, AnimationDataVertexCorrection, FrameNo, FrameNoStart, FrameNoEnd);

			if((null == PartsParent) || (0 == (FlagInheritance & FlagParameterKeyFrameInherit[(int)KindAttributeKey.FLIP_X])))
			{
				CompositionedFlipX = FlagFlipX;
			}
			else
			{
				CompositionedFlipX = (true == FlagFlipX) ? (!PartsParent.CompositionedFlipX) : (PartsParent.CompositionedFlipX);
			}

			if((null == PartsParent) || (0 == (FlagInheritance & FlagParameterKeyFrameInherit[(int)KindAttributeKey.FLIP_Y])))
			{
				CompositionedFlipY = FlagFlipY;
			}
			else
			{
				CompositionedFlipY = (true == FlagFlipY) ? (!PartsParent.CompositionedFlipY) : (PartsParent.CompositionedFlipY);
			}
			if((null == PartsParent) || (0 == (FlagInheritance & FlagParameterKeyFrameInherit[(int)KindAttributeKey.OPACITY_RATE])))
			{
				CompositionedOpacity = RateOpacity;
			}
			else
			{
				CompositionedOpacity = PartsParent.CompositionedOpacity * RateOpacity;
			}

			if(null != KeyDataCell)
			{
				TextureNo = KeyDataCell.Value.TextureNo;
				TextureRectBase = KeyDataCell.Value.Rectangle;
				PlanePivotBase = KeyDataCell.Value.Pivot;
//				Status |= BitStatus.CHANGE_CELL;

				switch(CollisionKind)
				{
					case KindCollision.SQUARE:
						Status |= BitStatus.CHANGE_COLLISION;
						break;
					default:
						break;
				}
			}
			else
			{
				TextureNo = -1;
				TextureRectBase.x = 0.0f;
				TextureRectBase.y = 0.0f;
				TextureRectBase.width = 0.0f;
				TextureRectBase.height = 0.0f;
				PlanePivotBase = Vector2.zero;
//				Status &= ~BitStatus.CHANGE_CELL;

				switch(CollisionKind)
				{
					case KindCollision.SQUARE:
//						Status |= BitStatus.CHANGE_COLLISION;
						if(null != CollisionComponent)
						{
							CollisionComponent.enabled = false;
						}
						break;
					default:
						break;
				}
			}
		}

		private int KeyIndexGetLast<_Type>(_Type[] TableKeyData)
			where _Type : KeyFrame.InterfaceData
		{
			if(null == TableKeyData)
			{
				return(-1);
			}
			return(TableKeyData.Length - 1);
		}
		private void KeyIndexSetRange<_Type>(ref int IndexNoStart, ref int IndexNoEnd, _Type[] TableKeyData, int FrameNoStart, int FrameNoEnd)
			where _Type : KeyFrame.InterfaceData
		{
			if(0 == TableKeyData.Length)
			{
				IndexNoStart = -1;
				IndexNoEnd = -1;
				return;
			}

			IndexNoStart = KeyFrame.DataIndexGetFrame(	TableKeyData,
														FrameNoStart,
														FlagDirection.JUSTNOW | FlagDirection.FUTURE,
														0
													);
			IndexNoEnd = KeyFrame.DataIndexGetFrame(	TableKeyData,
														FrameNoEnd,
														FlagDirection.JUSTNOW | FlagDirection.PAST,
														0
													);
			if(FrameNoEnd < TableKeyData[IndexNoStart].Time)
			{
				IndexNoStart = -1;
			}
			if(FrameNoStart > TableKeyData[IndexNoEnd].Time)
			{
				IndexNoEnd = -1;
			}
		}

		private _Type KeyIndexUpdate<_Type>(	_Type[] TableKeyData,
												KindAttributeKey Kind,
												int FrameNo,
												int FrameNoStart,
												int FrameNoEnd,
												bool FlagFrameNoOver,
												bool FlagLoop
											)
			where _Type : KeyFrame.InterfaceData
		{
			int IndexTop = KeyDataIndex[(int)KindKeyIndex.TOP, (int)Kind];
			int IndexLast = KeyDataIndex[(int)KindKeyIndex.LAST, (int)Kind];
			int IndexNow = KeyDataIndex[(int)KindKeyIndex.NOW, (int)Kind];
			int IndexNext = KeyDataIndex[(int)KindKeyIndex.NEXT, (int)Kind];
			_Type KeyData = default(_Type);

			if((0 == TableKeyData.Length) || (-1 == IndexTop))
			{
				return(default(_Type));
			}
//			if(1 > (IndexLast - IndexTop))
//			{
//				IndexNow = IndexTop;
//				IndexNext = -1;
//				goto KeyIndexUpdate_End;
//			}
			if(true == FlagFrameNoOver)
			{
				if(true == FlagLoop)
				{
					IndexNow = IndexTop;
					IndexNext = IndexNow + 1;
					if(IndexLast < IndexNext)
					{
						IndexNext = -1;
					}
				}
				else
				{
					IndexNow = IndexLast;
					IndexNext = -1;
				}
				goto KeyIndexUpdate_End;
			}

			if(-1 != IndexNext)
			{
				KeyData = KeyFrame.DataGetIndex(TableKeyData, IndexNext);
				while(FrameNo >= KeyData.Time)
				{
					IndexNow = IndexNext;
					IndexNext++;
					if(IndexLast < IndexNext)
					{
						IndexNext = -1;
						break;
					}
					KeyData = KeyFrame.DataGetIndex(TableKeyData, IndexNext);
				}
			}

		KeyIndexUpdate_End:
			KeyDataIndex[(int)KindKeyIndex.NOW, (int)Kind] = IndexNow;
			KeyDataIndex[(int)KindKeyIndex.NEXT, (int)Kind] = IndexNext;

			KeyData = KeyFrame.DataGetIndex(TableKeyData, IndexNow);
			return((FrameNo < KeyData.Time) ? default(_Type) : KeyData);
		}
		private void KeyIndexUpdateDataUser(	KeyFrame.DataUser[] TableKeyData,
												GameObject InstanceGameObject,
												int FrameNo,
												int FrameNoStart,
												int FrameNoEnd,
												bool FlagFrameNoOver,
												bool FlagLoop
											)
		{
			int IndexTop = KeyDataIndex[(int)KindKeyIndex.TOP, (int)KindAttributeKey.USER_DATA];
			int IndexLast = KeyDataIndex[(int)KindKeyIndex.LAST, (int)KindAttributeKey.USER_DATA];
			int IndexNow = KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.USER_DATA];
			int IndexNext = KeyDataIndex[(int)KindKeyIndex.NEXT, (int)KindAttributeKey.USER_DATA];

			if((0 == TableKeyData.Length) || (-1 == IndexTop))
			{
				return;
			}

			if(-1 != IndexNow)
			{
				KeyFrame.DataUser KeyData = KeyFrame.DataGetIndex(TableKeyData, IndexNow);
				while(FrameNo >= KeyData.Time)
				{
					PartsRoot.CallBackExecUserData(InstanceGameObject.name, this, KeyData);

					IndexNow = IndexNext;
					IndexNext++;
					if(IndexLast < IndexNext)
					{
						IndexNext = -1;
						break;
					}
					KeyData = KeyFrame.DataGetIndex(TableKeyData, IndexNow);
					if(null == KeyData)
					{
						break;
					}
				}
			}

			if(true == FlagFrameNoOver)
			{
				if(true == FlagLoop)
				{
					IndexNow = IndexTop;
					IndexNext = IndexNow + 1;
					if(IndexLast < IndexNext)
					{
						IndexNext = -1;
					}
				}
				else
				{
					IndexNow = -1;
					IndexNext = -1;
				}
			}

			KeyDataIndex[(int)KindKeyIndex.NOW, (int)KindAttributeKey.USER_DATA] = IndexNow;
			KeyDataIndex[(int)KindKeyIndex.NEXT, (int)KindAttributeKey.USER_DATA] = IndexNext;
		}

		private bool KeyDataGetBoolInterpolationNon(KeyFrame.DataBool KeyData, int FrameNo, bool ValueError)
		{
			if(null == KeyData)
			{
				return(ValueError);
			}
			return(KeyData.Value);
		}
		private float KeyDataGetFloatInterpolation(KeyFrame.DataFloat KeyData, KeyFrame.DataFloat[] TableKeyData, int FrameNo, int FrameNoStart, int FrameNoEnd, float ValueError)
		{
			if(null == KeyData)
			{
				return(ValueError);
			}

			KeyFrame.DataFloat KeyDataNext = KeyFrame.DataGetIndex(TableKeyData, KeyDataIndex[(int)KindKeyIndex.NEXT, (int)KeyData.Kind]);
			if(null == KeyDataNext)
			{
				return((float)KeyData.Value);
			}
			return(Interpolation.Interpolate<float>(KeyData.Curve, FrameNo, KeyData.Value, KeyDataNext.Value, KeyData.Time, KeyDataNext.Time));
		}
		private void KeyDataGetColorBlendInterpolation(	ref KindColorOperation Operation,
														Color32[] Vertex,
														float[] Power,
														KeyFrame.DataColor KeyData,
														KeyFrame.DataColor[] TableKeyData,
														int FrameNo, int FrameNoStart, int FrameNoEnd
													)
		{
			if(null == KeyData)
			{
				Operation = Library_SpriteStudio.KindColorOperation.NON;
				for(int i=0; i<4; i++)
				{
					Vertex[i] = Color.white;
					Power[i] = 1.0f;
				}
				return;
			}

			KeyFrame.DataColor KeyDataNext = KeyFrame.DataGetIndex(TableKeyData, KeyDataIndex[(int)KindKeyIndex.NEXT, (int)KeyData.Kind]);
			if(null == KeyDataNext)
			{
				Operation = KeyData.Value.Operation;
				for(int i=0; i<4; i++)
				{
					Vertex[i] = KeyData.Value.VertexColor[i];
					Power[i] = KeyData.Value.RatePixelAlpha[i];
				}
				return;
			}

			KeyFrame.ValueColor Value = (KeyFrame.ValueColor)KeyData.Value.GetInterpolated(KeyData.Curve, FrameNo, KeyData.Value, KeyDataNext.Value, KeyData.Time, KeyDataNext.Time);
			Operation = Value.Operation;
			for(int i=0; i<4; i++)
			{
				Vertex[i] = Value.VertexColor[i];
				Power[i] = Value.RatePixelAlpha[i];
			}
		}
		private void KeyDataGetQuadrilateralInterpolation(	Vector2[] CoordinateOffset,
															KeyFrame.DataQuadrilateral KeyData,
															KeyFrame.DataQuadrilateral[] TableKeyData,
															int FrameNo, int FrameNoStart, int FrameNoEnd
														)
		{
			if(null == CoordinateOffset)
			{
				return;
			}
			if(null == KeyData)
			{
				for(int i=0; i<CoordinateOffset.Length; i++)
				{
					CoordinateOffset[i] = Vector2.zero;
				}
				return;
			}

			KeyFrame.DataQuadrilateral KeyDataNext = KeyFrame.DataGetIndex(TableKeyData, KeyDataIndex[(int)KindKeyIndex.NEXT, (int)KeyData.Kind]);
			if(null == KeyDataNext)
			{
				for(int i=0; i<CoordinateOffset.Length; i++)
				{
					CoordinateOffset[i] = KeyData.Value.Coordinate[i].Point;
				}
				return;
			}
			KeyFrame.ValueQuadrilateral Value = (KeyFrame.ValueQuadrilateral)KeyData.Value.GetInterpolated(KeyData.Curve, FrameNo, KeyData.Value, KeyDataNext.Value, KeyData.Time, KeyDataNext.Time);
			for(int i=0; i<CoordinateOffset.Length; i++)
			{
				CoordinateOffset[i] = Value.Coordinate[i].Point;
			}
		}

		public void AnimationFixTransform(Transform InstanceTransform)
		{
			InstanceTransform.localPosition = Position;
			InstanceTransform.localEulerAngles = Rotate;
			InstanceTransform.localScale = Scale;
		}

		public int AnimationFixCell(Library_SpriteStudio.SpriteBase InstanceSprite)
		{
			if(-1 != TextureNo)
			{
				Material InstanceMaterial = PartsRoot.MaterialGet(TextureNo, KindBlendTarget);
				InstanceSprite.DataMaterials[0] = InstanceMaterial;

//				if(0 != (Status & BitStatus.CHANGE_CELL))
				{
					TextureSizePixel.x = InstanceMaterial.mainTexture.width;
					TextureSizePixel.y = InstanceMaterial.mainTexture.height;
				}
//				Status &= ~BitStatus.CHANGE_CELL;
			}
			return(TextureNo);
		}

		public void AnimationGetTextureMapping(out Rect AreaTexture, out Vector2 Scale, out float Rotation, out Vector2 Pivot)
		{
			AreaTexture = TextureRectBase;
			Pivot.x = AreaTexture.width;
			Pivot.y = AreaTexture.height;
			Pivot = Pivot * 0.5f;
			Scale = TextureScale;
			Rotation = TextureRotate;

			AreaTexture.x += (TextureSizePixel.x * TextureRectOffset.x);
			AreaTexture.y += (TextureSizePixel.y * TextureRectOffset.y);
			AreaTexture.width += (TextureSizePixel.x * TextureRectOffset.width);
			AreaTexture.height += (TextureSizePixel.y * TextureRectOffset.height);
		}

		public void AnimationFixSpriteCommon(Transform InstanceTransform, Library_SpriteStudio.SpriteBase InstanceSprite)
		{
			AnimationFixCell(InstanceSprite);

			AnimationFixTransform(InstanceTransform);

			{
				bool	FlagFlipX = (0 != (Status & BitStatus.FLIP_TEXTURE_X)) ? (!CompositionedFlipX) : (CompositionedFlipX);
				bool	FlagFlipY = (0 != (Status & BitStatus.FLIP_TEXTURE_Y)) ? (!CompositionedFlipY) : (CompositionedFlipY);
				InstanceSprite.FlipXTextureMapping(FlagFlipX);
				InstanceSprite.FlipYTextureMapping(FlagFlipY);
			}

			InstanceSprite.StatusSetRendering((0 != (Status & BitStatus.DISPLAY)) ? true : false);
//			InstanceSprite.RateOpacityLU = RateOpacity * VertexColorPower[0];
//			InstanceSprite.RateOpacityRU = RateOpacity * VertexColorPower[1];
//			InstanceSprite.RateOpacityRD = RateOpacity * VertexColorPower[2];
//			InstanceSprite.RateOpacityLD = RateOpacity * VertexColorPower[3];
			InstanceSprite.RateOpacityLU = CompositionedOpacity * VertexColorPower[0];
			InstanceSprite.RateOpacityRU = CompositionedOpacity * VertexColorPower[1];
			InstanceSprite.RateOpacityRD = CompositionedOpacity * VertexColorPower[2];
			InstanceSprite.RateOpacityLD = CompositionedOpacity * VertexColorPower[3];

			float ValueBlendKind = (float)((int)VertexColorOperation) + 0.01f;
			InstanceSprite.RateEffectLU = ValueBlendKind;
			InstanceSprite.RateEffectRU = ValueBlendKind;
			InstanceSprite.RateEffectRD = ValueBlendKind;
			InstanceSprite.RateEffectLD = ValueBlendKind;

			InstanceSprite.VertexColorLU = VertexColor[0];
			InstanceSprite.VertexColorRU = VertexColor[1];
			InstanceSprite.VertexColorRD = VertexColor[2];
			InstanceSprite.VertexColorLD = VertexColor[3];

			if(null != CollisionComponent)
			{
				if(0 != (Status & BitStatus.CHANGE_COLLISION))
				{
					switch(CollisionKind)
					{
						case KindCollision.SQUARE:
							{
								BoxCollider InstanceCollider = CollisionComponent as BoxCollider;
								InstanceCollider.enabled = true;

								Vector3 SizeBox = InstanceCollider.size;
								Vector3 PivotBox = InstanceCollider.center;
								float SizeCellX = TextureRectBase.width;
								float SizeCellY = TextureRectBase.height;
								SizeBox.x = SizeCellX;
								SizeBox.y = SizeCellY;

								InstanceCollider.size = SizeBox;

								PivotBox.x = -(PlanePivotBase.x - (SizeCellX * 0.5f));
								PivotBox.y = (PlanePivotBase.y - (SizeCellY * 0.5f));
								PivotBox.z = 0.0f;
								InstanceCollider.center = PivotBox;
							}
							break;

						case KindCollision.CIRCLE:
							{
								CapsuleCollider InstanceCollider = CollisionComponent as CapsuleCollider;
								InstanceCollider.enabled = true;

								InstanceCollider.radius = collisionRadius;
								InstanceCollider.center = Vector3.zero;
							}
							break;

						default:
							break;
					}
				}
			}
			Status &= ~BitStatus.CHANGE_COLLISION;
		}

		public bool AnimationGetPlaneTriangle2(out Vector2 SizePlane, out Vector2 PivotPlane, ref Rect AreaTexture)
		{
			SizePlane.x = AreaTexture.width;
			SizePlane.y = AreaTexture.height;

			PivotPlane.x = PlanePivotBase.x + (TextureRectBase.width * PlanePivotOffset.x);
			PivotPlane.y = PlanePivotBase.y - (TextureRectBase.height * PlanePivotOffset.y);

			return(true);
		}

		public bool AnimationGetPlaneTriangle4(out Vector2 CoordinateLU, out Vector2 CoordinateRU, out Vector2 CoordinateLD, out Vector2 CoordinateRD, out Vector2 PivotPlane, ref Rect AreaTexture)
		{
			float SizePlaneX = AreaTexture.width;
			float SizePlaneY = AreaTexture.height;

			if(null != PlaneCoordinateOffset)
			{
				CoordinateLU.x = PlaneCoordinateOffset[0].x;
				CoordinateLU.y = PlaneCoordinateOffset[0].y;

				CoordinateRU.x = SizePlaneX + PlaneCoordinateOffset[1].x;
				CoordinateRU.y = PlaneCoordinateOffset[1].y;

				CoordinateRD.x = SizePlaneX + PlaneCoordinateOffset[2].x;
				CoordinateRD.y = -SizePlaneY + PlaneCoordinateOffset[2].y;

				CoordinateLD.x = PlaneCoordinateOffset[3].x;
				CoordinateLD.y = -SizePlaneY + PlaneCoordinateOffset[3].y;
			}
			else
			{
				CoordinateLU.x = 0.0f;
				CoordinateLU.y = 0.0f;

				CoordinateRU.x = SizePlaneX;
				CoordinateRU.y = 0.0f;

				CoordinateRD.x = SizePlaneX;
				CoordinateRD.y = -SizePlaneY;

				CoordinateLD.x = 0.0f;
				CoordinateLD.y = -SizePlaneY;
			}

			PivotPlane.x = PlanePivotBase.x + (TextureRectBase.width * PlanePivotOffset.x);
			PivotPlane.y = PlanePivotBase.y - (TextureRectBase.height * PlanePivotOffset.y);

			return(true);
		}

		public void DrawEntry(ref Script_SpriteStudio_PartsRoot.InformationMeshData MeshDataInformation)
		{
			MeshDataInformation.Priority = Priority + ((float)ID * (1.0f / 1000.0f));
			PartsRoot.MeshAdd(TextureNo, KindBlendTarget, ref MeshDataInformation);
		}

		private SpriteData DataGetInhelitBase(KindAttributeKey AttributeKey)
		{
			SpriteData DataNow = this;
			while(DataNow != PartsRoot.SpriteStudioData)
			{
				if(0 == (DataNow.FlagInheritance & FlagParameterKeyFrameInherit[(int)AttributeKey]))
				{
					return(DataNow);
				}

				DataNow = DataNow.PartsParent;
			}
			return(DataNow);
		}
	}
}
