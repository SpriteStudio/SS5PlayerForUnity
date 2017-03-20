/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static partial class Library_SpriteStudio
{
	public static partial class Data
	{
		/* ------------------------------------------------------------ for NULL/Normal(Sprite)-Parts */
		[System.Serializable]
		public class AttributeStatus
		{
			[System.Flags]
			public enum FlagBit : int
			{
				VALID = 0x00000001,
				HIDE = 0x00000002,

				FLIPX = 0x00000010,
				FLIPY = 0x00000020,
				FLIPXTEXTURE = 0x00000040,
				FLIPYTEXTURE = 0x00000080,

				PARTSIDNEXT = 0x7fff0000,

//				CLEAR = PARTSIDNEXT
				CLEAR = 0x00000000
			}

			public FlagBit Flags;

			public bool IsValid
			{
				get
				{
					return(0 != (Flags & FlagBit.VALID));
				}
			}
			public bool IsHide
			{
				get
				{
					return(0 != (Flags & FlagBit.HIDE));
				}
			}
			public bool IsFlipX
			{
				get
				{
					return(0 != (Flags & FlagBit.FLIPX));
				}
			}
			public bool IsFlipY
			{
				get
				{
					return(0 != (Flags & FlagBit.FLIPY));
				}
			}
			public bool IsTextureFlipX
			{
				get
				{
					return(0 != (Flags & FlagBit.FLIPXTEXTURE));
				}
			}
			public bool IsTextureFlipY
			{
				get
				{
					return(0 != (Flags & FlagBit.FLIPYTEXTURE));
				}
			}
			public int PartsIDNext
			{
				get
				{
					FlagBit Data = Flags & FlagBit.PARTSIDNEXT;
					return((0 == Data) ? (-1) : ((int)Data >> 16));
				}
			}

			public void CleanUp()
			{
				Flags = FlagBit.CLEAR;
			}

//			public void Duplicate(AttributeStatus Original)

			public override bool Equals(System.Object Target)
			{
				if((null == Target) || (GetType() != Target.GetType()))
				{
					return(false);
				}

				AttributeStatus TargetData = (AttributeStatus)Target;
				if(	(IsValid == TargetData.IsValid)
					&& (IsHide == TargetData.IsHide)
					&& (IsFlipX == TargetData.IsFlipX)
					&& (IsFlipY == TargetData.IsFlipY)
					&& (IsTextureFlipX == TargetData.IsTextureFlipX)
					&& (IsTextureFlipY == TargetData.IsTextureFlipY)
					&& (PartsIDNext == TargetData.PartsIDNext)
					)
				{
					return(true);
				}
				return(false);
			}

			public override int GetHashCode()
			{
				return(base.GetHashCode());
			}
		}

		[System.Serializable]
		public class AttributeColorBlend
		{
			public KindColorBound Bound;
			public KindColorOperation Operation;
			public Color[] VertexColor;
			public float[] RatePixelAlpha;

			public void CleanUp()
			{
				Bound = KindColorBound.NON;
				Operation = KindColorOperation.NON;
				VertexColor = null;
				RatePixelAlpha = null;
			}

			public void Duplicate(AttributeColorBlend Original)
			{
				Bound = Original.Bound;
				Operation = Original.Operation;
				for(int i=0; i<(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2; i++)
				{
					VertexColor[i] = Original.VertexColor[i];
					RatePixelAlpha[i] = Original.RatePixelAlpha[i];
				}
			}

			public override bool Equals(System.Object Target)
			{
				if((null == Target) || (GetType() != Target.GetType()))
				{
					return(false);
				}

				AttributeColorBlend TargetData = (AttributeColorBlend)Target;
				int Count;

				if(Bound != TargetData.Bound)
				{
					return(false);
				}
				if(Operation == TargetData.Operation)
				{
					return(false);
				}

				Count = VertexColor.Length;
				if(Count != TargetData.VertexColor.Length)
				{
					return(false);
				}
				for(int i=0; i<Count; i++)
				{
					if(VertexColor[i] != TargetData.VertexColor[i])
					{
						return(false);
					}
				}

				Count = RatePixelAlpha.Length;
				if(Count != TargetData.RatePixelAlpha.Length)
				{
					return(false);
				}
				for(int i=0; i<Count; i++)
				{
					if(RatePixelAlpha[i] != TargetData.RatePixelAlpha[i])
					{
						return(false);
					}
				}

				return(true);
			}

			public override int GetHashCode()
			{
				return(base.GetHashCode());
			}
		}

		[System.Serializable]
		public class AttributeVertexCorrection
		{
			public Vector2[] Coordinate;

			public void CleanUp()
			{
				Coordinate = null;
			}

			public void Duplicate(AttributeVertexCorrection Original)
			{
				for(int i=0; i<Coordinate.Length; i++)
				{
					Coordinate[i] = Original.Coordinate[i];
				}
			}

			public override bool Equals(System.Object Target)
			{
				if((null == Target) || (GetType() != Target.GetType()))
				{
					return(false);
				}

				AttributeVertexCorrection TargetData = (AttributeVertexCorrection)Target;
				int Count;

				Count = Coordinate.Length;
				if(Count != TargetData.Coordinate.Length)
				{
					return(false);
				}
				for(int i=0; i<Count; i++)
				{
					if(Coordinate[i] != TargetData.Coordinate[i])
					{
						return(false);
					}
				}
				return(true);
			}

			public override int GetHashCode()
			{
				return(base.GetHashCode());
			}
		}

		[System.Serializable]
		public class AttributeCell
		{
			public int IndexCellMap;
			public int IndexCell;

			public void CleanUp()
			{
				IndexCellMap = -1;
				IndexCell = -1;
			}

			public void Duplicate(AttributeCell Original)
			{
				IndexCellMap = Original.IndexCellMap;
				IndexCell = Original.IndexCell;
			}

			public override bool Equals(System.Object Target)
			{
				if((null == Target) || (GetType() != Target.GetType()))
				{
					return(false);
				}

				AttributeCell TargetData = (AttributeCell)Target;
				return(((IndexCellMap == TargetData.IndexCellMap) && (IndexCell == TargetData.IndexCell)) ? true : false);
			}

			public override int GetHashCode()
			{
				return(base.GetHashCode());
			}
		}

		[System.Serializable]
		public class AttributeUserData
		{
			[System.Flags]
			public enum FlagBit
			{
				CLEAR = 0x00000000,
				NUMBER = 0x00000001,
				RECTANGLE = 0x00000002,
				COORDINATE = 0x00000004,
				TEXT = 0x00000008,
			}

			public FlagBit Flags;
			public int NumberInt;
			public Rect Rectangle;
			public Vector2 Coordinate;
			public string Text;

			public bool IsNumber
			{
				get
				{
					return(0 != (Flags & FlagBit.NUMBER));
				}
			}
			public bool IsRectangle
			{
				get
				{
					return(0 != (Flags & FlagBit.RECTANGLE));
				}
			}
			public bool IsCoordinate
			{
				get
				{
					return(0 != (Flags & FlagBit.COORDINATE));
				}
			}
			public bool IsText
			{
				get
				{
					return(0 != (Flags & FlagBit.TEXT));
				}
			}
			public uint Number
			{
				get
				{
					return((uint)NumberInt);
				}
			}

			public void CleanUp()
			{
				Flags = FlagBit.CLEAR;
				NumberInt = 0;
				Rectangle.xMin = 0.0f;
				Rectangle.yMin = 0.0f;
				Rectangle.xMax = 0.0f;
				Rectangle.yMax = 0.0f;
				Coordinate = Vector2.zero;
				Text = "";
			}

			public void Duplicate(AttributeUserData Original)
			{
				Flags = Original.Flags;
				NumberInt = Original.NumberInt;
				Rectangle.xMin = Original.Rectangle.xMin;
				Rectangle.yMin = Original.Rectangle.yMin;
				Rectangle.xMax = Original.Rectangle.xMax;
				Rectangle.yMax = Original.Rectangle.yMax;
				Coordinate = Original.Coordinate;
				Text = (true == string.IsNullOrEmpty(Original.Text)) ? "" : string.Copy(Original.Text);
			}

			public override bool Equals(System.Object Target)
			{
				if((null == Target) || (GetType() != Target.GetType()))
				{
					return(false);
				}

				AttributeUserData TargetData = (AttributeUserData)Target;
				return((	(Flags == TargetData.Flags)
							&& (NumberInt == TargetData.NumberInt)
							&& (Rectangle.xMin == TargetData.Rectangle.xMin)
							&& (Rectangle.yMin == TargetData.Rectangle.yMin)
							&& (Rectangle.xMax == TargetData.Rectangle.xMax)
							&& (Rectangle.yMax == TargetData.Rectangle.yMax)
							&& (Coordinate == TargetData.Coordinate)
							&& (Text == TargetData.Text)
						) ? true : false);
			}

			public override int GetHashCode()
			{
				return(base.GetHashCode());
			}
		}

		[System.Serializable]
		public class AttributeInstance
		{
			[System.Flags]
			public enum FlagBit
			{
				PINGPONG = 0x00000001,
				INDEPENDENT = 0x00000002,

				CLEAR = 0x00000000,
			}

			public FlagBit Flags;
			public int PlayCount;
			public float RateTime;
			public int OffsetStart;
			public int OffsetEnd;
			public string LabelStart;
			public string LabelEnd;

			public void CleanUp()
			{
				Flags = FlagBit.CLEAR;
				PlayCount = 1;
				RateTime = 1.0f;
				OffsetStart = 0;
				OffsetEnd = 0;
				LabelStart = "";
				LabelEnd = "";
			}

			public void Duplicate(AttributeInstance Original)
			{
				Flags = Original.Flags;
				PlayCount = Original.PlayCount;
				RateTime = Original.RateTime;
				OffsetStart = Original.OffsetStart;
				OffsetEnd = Original.OffsetEnd;
				LabelStart = string.Copy(Original.LabelStart);
				LabelEnd = string.Copy(Original.LabelEnd);
			}

			public override bool Equals(System.Object Target)
			{
				if((null == Target) || (GetType() != Target.GetType()))
				{
					return(false);
				}

				AttributeInstance TargetData = (AttributeInstance)Target;
				return((	(Flags == TargetData.Flags)
							&& (PlayCount == TargetData.PlayCount)
							&& (RateTime == TargetData.RateTime)
							&& (OffsetStart == TargetData.OffsetStart)
							&& (OffsetEnd == TargetData.OffsetEnd)
//							&& (0 == string.Compare(LabelStart, TargetData.LabelStart))
							&& (LabelStart == TargetData.LabelStart)
//							&& (0 == string.Compare(LabelEnd, TargetData.LabelEnd))
							&& (LabelEnd == TargetData.LabelEnd)
						) ? true : false);
			}

			public override int GetHashCode()
			{
				return(base.GetHashCode());
			}
		}

		[System.Serializable]
		public class AttributeEffect
		{
			[System.Flags]
			public enum FlagBit
			{
				PINGPONG = 0x00000001,	/* Reserved. */
				INDEPENDENT = 0x00000002,

				CLEAR = 0x00000000,
			}

			public FlagBit Flags;
			public int FrameStart;
			public float RateTime;

			public void CleanUp()
			{
				Flags = FlagBit.CLEAR;
				FrameStart = 0;
				RateTime = 1.0f;
			}

			public void Duplicate(AttributeEffect Original)
			{
				Flags = Original.Flags;
				FrameStart = Original.FrameStart;
				RateTime = Original.RateTime;
			}

			public override bool Equals(System.Object Target)
			{
				if((null == Target) || (GetType() != Target.GetType()))
				{
					return(false);
				}

				AttributeEffect TargetData = (AttributeEffect)Target;
				return(((Flags == TargetData.Flags) && (FrameStart == TargetData.FrameStart) && (RateTime == TargetData.RateTime)) ? true : false);
			}

			public override int GetHashCode()
			{
				return(base.GetHashCode());
			}
		}

		[System.Serializable]
		public class AttributeCoordinateMeshFix
		{
			public Vector3[] Coordinate;

			public void CleanUp()
			{
				Coordinate = null;
			}

			public override bool Equals(System.Object Target)
			{
				if((null == Target) || (GetType() != Target.GetType()))
				{
					return(false);
				}

				AttributeCoordinateMeshFix TargetData = (AttributeCoordinateMeshFix)Target;
				int Count = Coordinate.Length;
				if(Count != TargetData.Coordinate.Length)
				{
					return(false);
				}
				for(int i=0; i<Count; i++)
				{
					if(Coordinate[i] != TargetData.Coordinate[i])
					{
						return(false);
					}
				}
				return(true);
			}

			public override int GetHashCode()
			{
				return(base.GetHashCode());
			}
		}

		[System.Serializable]
		public class AttributeUVMeshFix
		{
			public Vector2[] UV;

			public void CleanUp()
			{
				UV = null;
			}

			public override bool Equals(System.Object Target)
			{
				if((null == Target) || (GetType() != Target.GetType()))
				{
					return(false);
				}

				AttributeUVMeshFix TargetData = (AttributeUVMeshFix)Target;
				int Count = UV.Length;
				if(Count != TargetData.UV.Length)
				{
					return(false);
				}
				for(int i=0; i<Count; i++)
				{
					if(UV[i] != TargetData.UV[i])
					{
						return(false);
					}
				}
				return(true);
			}

			public override int GetHashCode()
			{
				return(base.GetHashCode());
			}
		}

		[System.Serializable]
		public class AttributeColorBlendMeshFix : Library_SpriteStudio.Data.AttributeUVMeshFix
		{
			public Color32[] ColorOverlay;

			public new void CleanUp()
			{
				base.CleanUp();
				ColorOverlay = null;
			}

			public override bool Equals(System.Object Target)
			{
				if((null == Target) || (GetType() != Target.GetType()))
				{
					return(false);
				}

				AttributeColorBlendMeshFix TargetData = (AttributeColorBlendMeshFix)Target;
				if(false == base.Equals(TargetData))
				{
					return(false);
				}
				int Count = ColorOverlay.Length;
				if(Count != TargetData.ColorOverlay.Length)
				{
					return(false);
				}
				for(int i=0; i<Count; i++)
				{
					if(false == ColorOverlay[i].Equals(TargetData.ColorOverlay[i]))
					{
						return(false);
					}
				}
				return(true);
			}

			public override int GetHashCode()
			{
				return(base.GetHashCode());
			}
		}

		/* ------------------------------------------------------------ for Effect-Data */
		[System.Serializable]
		public class AttributeEffectRangeFloat
		{
			public float Main;
			public float Sub;
		}
		[System.Serializable]
		public class AttributeEffectRangeVector2
		{
			public Vector2 Main;
			public Vector2 Sub;
		}
		[System.Serializable]
		public class AttributeEffectRangeColor
		{
			public Color Main;
			public Color Sub;
		}
	}
}
