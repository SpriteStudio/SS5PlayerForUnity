/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
#define DRAWPARTS_ORDER_SOLVINGJUSTINTIME
#define DRAWPARTS_POOLEFFECT_GENERATEJUSTINTIME

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static partial class Library_SpriteStudio
{
	public delegate bool FunctionCallBackPlayEnd(	Script_SpriteStudio_Root InstanceRoot,
													GameObject ObjectControl
												);
	public delegate bool FunctionCallBackPlayEndEffect(	Script_SpriteStudio_RootEffect InstanceRoot
													);

	public delegate void FunctionCallBackUserData(	Script_SpriteStudio_Root InstanceRoot,
													string PartsName,
													int PartsID,
													int AnimationNo,
													int FrameNoDecode,
													int FrameNoKeyData,
													Library_SpriteStudio.Data.AttributeUserData Data,
													bool FlagWayBack
												);
	public delegate void FunctionCallBackCollider(	Script_SpriteStudio_Root InstanceRoot,
													string PartsName,
													int PartsID,
													Collider Self,
													Collider Pair
												);
	public delegate void FunctionCallBackCollision(	Script_SpriteStudio_Root InstanceRoot,
													string PartsName,
													int PartsID,
													Collider Self,
													Collision Contacts
												);

	public enum KindParts
	{
		NON = -1,				/* ERROR-Code */

		ROOT = 0,				/* Root-Parts (Subspecies of "NULL"-Parts) */
		NULL,					/* NULL-Parts */
		NORMAL_TRIANGLE2,		/* No use Vertex-Collection Sprite-Parts */
		NORMAL_TRIANGLE4,		/* Use Vertex-Collection Sprite-Parts */

		INSTANCE,				/* Instance-Parts */
		EFFECT,					/* Effect-Parts */

		TERMINATOR,
		NORMAL = TERMINATOR		/* (Dummy: NORMAL_TRIANGLE2 or NORMAL_TRIANGLE4) */
	}
	public enum KindPartsEffect
	{
		NON = -1,				/* ERROR-Code */

		ROOT = 0,				/* Root-Parts (Subspecies of "Particle"-Parts) */
		EMITTER,				/* Emitter */
		PARTICLE,				/* Particle */

		TERMINATOR
	}
	public enum KindInterpolation
	{
		NON = 0,
		LINEAR,
		HERMITE,
		BEZIER,
		ACCELERATE,
		DECELERATE,
	}
	public enum KindColorOperation
	{
		NON = 0,

		MIX,
		ADD,
		SUB,
		MUL,

		TERMINATOR,
	}
	public enum KindColorOperationEffect
	{
		NON = 0,

		MIX,
		ADD,
		ADD2,

		TERMINATOR_KIND = ADD2,

		TERMINATOR,
	}
	public enum KindColorLabel
	{
		NON = 0,

		RED = 1,
		ORANGE,
		YELLOW,
		GREEN,
		BLUE,
		VIOLET,
		GRAY,
	}
	public enum KindColorBound
	{
		NON = 0,
		OVERALL,
		VERTEX
	}
	public enum KindCollision
	{
		NON = 0,
		SQUARE,
		AABB,
		CIRCLE,
		CIRCLE_SCALEMINIMUM,
		CIRCLE_SCALEMAXIMUM
	}
	public enum KindVertexNo
	{
		LU = 0,
		RU,
		RD,
		LD,
		C,

		TERMINATOR4,
		TERMINATOR2 = C
	}
	public enum KindWrapTexture
	{
		CLAMP = 0,
		REPEAT,
		MIRROR,
	}
	public enum KindFilterTexture
	{
		NEAREST = 0,
		LINEAR,
	}
	public enum KindFormat
	{
		PLAIN = 0,				/* Data-Format: Plain-Data */
		FIX						/* Data-Format: Deformation of "Mesh" and "Collider" are Calculated-In-Advance. */
	}
	public enum KindPack
	{
		STANDARD_UNCOMPRESSED = 0,	/* Data-Compress: Standard (Uncompressed/Linear) */
		STANDARD_CPE,				/* Data-Compress: Standaed (Changing-Point Extracting) */
		FLYWEIGHT,					/* GoF Flyweight-Pattern */

		TERMINATOR,
	}
	public enum KindLabelAnimationReserved
	{
		START = 0,				/* (START + INDEX_RESERVED): "_start" */
		END,					/* (END + INDEX_RESERVED): "_end" */

		TERMINATOR,
		INDEX_RESERVED = 0x10000000,
	}

	public readonly static Shader[] Shader_SpriteStudioTriangleX = new Shader[(int)Library_SpriteStudio.KindColorOperation.TERMINATOR - 1]
	{
		Shader.Find("Custom/SpriteStudio5/Mix"),
		Shader.Find("Custom/SpriteStudio5/Add"),
		Shader.Find("Custom/SpriteStudio5/Sub"),
		Shader.Find("Custom/SpriteStudio5/Mul")
	};
	public readonly static Shader[] Shader_SpriteStudioEffect = new Shader[(int)Library_SpriteStudio.KindColorOperationEffect.TERMINATOR - 1]
	{
		Shader.Find("Custom/SpriteStudio5/Effect/Mix"),
		Shader.Find("Custom/SpriteStudio5/Effect/Add"),
		Shader.Find("Custom/SpriteStudio5/Effect/Add2"),
	};

	public readonly static int[] ArrayVertexIndex_Triangle2 =
	{
		(int)KindVertexNo.LU, (int)KindVertexNo.RU, (int)KindVertexNo.RD,
		(int)KindVertexNo.RD, (int)KindVertexNo.LD, (int)KindVertexNo.LU
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
	public readonly static Vector3[] ArrayCoordinate_Triangle2 = new Vector3[]
	{
		new Vector3(-0.5f, 0.5f, 0.0f),
		new Vector3(0.5f, 0.5f, 0.0f),
		new Vector3(0.5f, -0.5f, 0.0f),
		new Vector3(-0.5f, -0.5f, 0.0f)
	};
	public readonly static Color32[] ArrayColor32_Triangle2 = new Color32[]
	{
		new Color32(0xff, 0xff, 0xff, 0xff),
		new Color32(0xff, 0xff, 0xff, 0xff),
		new Color32(0xff, 0xff, 0xff, 0xff),
		new Color32(0xff, 0xff, 0xff, 0xff)
	};

	public readonly static int[] ArrayVertexIndex_Triangle4 =
	{
		(int)KindVertexNo.C, (int)KindVertexNo.LU, (int)KindVertexNo.RU,
		(int)KindVertexNo.C, (int)KindVertexNo.RU, (int)KindVertexNo.RD,
		(int)KindVertexNo.C, (int)KindVertexNo.RD, (int)KindVertexNo.LD,
		(int)KindVertexNo.C, (int)KindVertexNo.LD, (int)KindVertexNo.LU,
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
	public readonly static Vector3[] ArrayCoordinate_Triangle4 = new Vector3[]
	{
		new Vector3(-0.5f, 0.5f, 0.0f),
		new Vector3(0.5f, 0.5f, 0.0f),
		new Vector3(0.5f, -0.5f, 0.0f),
		new Vector3(-0.5f, -0.5f, 0.0f),
		new Vector3(0.0f, 0.0f, 0.0f)
	};
	public readonly static Color32[] ArrayColor32_Triangle4 = new Color32[]
	{
		new Color32(0xff, 0xff, 0xff, 0xff),
		new Color32(0xff, 0xff, 0xff, 0xff),
		new Color32(0xff, 0xff, 0xff, 0xff),
		new Color32(0xff, 0xff, 0xff, 0xff),
		new Color32(0xff, 0xff, 0xff, 0xff)
	};
	public readonly static int[,] VertexCollectionOrderVertex = new int[4, (int)Library_SpriteStudio.KindVertexNo.TERMINATOR2]
	{
		{	/* Normal */
			(int)Library_SpriteStudio.KindVertexNo.LU,
			(int)Library_SpriteStudio.KindVertexNo.RU,
			(int)Library_SpriteStudio.KindVertexNo.RD,
			(int)Library_SpriteStudio.KindVertexNo.LD,
		},
		{	/* Flip-X */
			(int)Library_SpriteStudio.KindVertexNo.RU,
			(int)Library_SpriteStudio.KindVertexNo.LU,
			(int)Library_SpriteStudio.KindVertexNo.LD,
			(int)Library_SpriteStudio.KindVertexNo.RD,
		},
		{	/* Flip-Y */
			(int)Library_SpriteStudio.KindVertexNo.LD,
			(int)Library_SpriteStudio.KindVertexNo.RD,
			(int)Library_SpriteStudio.KindVertexNo.RU,
			(int)Library_SpriteStudio.KindVertexNo.LU,
		},
		{	/* FlipX&Y */
			(int)Library_SpriteStudio.KindVertexNo.RD,
			(int)Library_SpriteStudio.KindVertexNo.LD,
			(int)Library_SpriteStudio.KindVertexNo.LU,
			(int)Library_SpriteStudio.KindVertexNo.RU,
		}
	};
	public readonly static string[] ListNameLabelAnimationReserved = new string[(int)Library_SpriteStudio.KindLabelAnimationReserved.TERMINATOR]
	{
		"_start",
		"_end",
	};

	public static partial class Data
	{
		/* Dummy Attribute Datas */
		public readonly static AttributeStatus DummyStatus = new AttributeStatus
		{
			Flags = AttributeStatus.FlagBit.CLEAR
		};
		public readonly static AttributeVertexCorrection DummyVertexCorrection = new AttributeVertexCorrection
		{
			Coordinate = new Vector2[(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2]
			{
				Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero
			}
		};

		[System.Serializable]
		public class CellMap
		{
			public string Name;
			public Vector2 SizeOriginal;
			public Library_SpriteStudio.Data.Cell[] ListCell;

			public void CleanUp()
			{
				Name = "";
				SizeOriginal = Vector2.zero;
				ListCell = null;
			}

			public int CountGetCell()
			{
				return((null != ListCell) ? ListCell.Length : -1);
			}

			public int IndexGetCell(string NameCell)
			{
				if((true == string.IsNullOrEmpty(NameCell)) || (null == ListCell))
				{
					return(-1);
				}

				int Count = ListCell.Length;
				for(int i=0; i<Count; i++)
				{
//					if(0 == string.Compare(NameCell, ListCell[i].Name))
					if(NameCell == ListCell[i].Name)
					{
						return(i);
					}
				}
				return(-1);
			}

			public Library_SpriteStudio.Data.Cell DataGetCell(int Index)
			{
				if(null != ListCell)
				{
					if((0 <= Index) && (ListCell.Length > Index))
					{
						return(ListCell[Index]);
					}
				}
				return(null);
			}

			public void Duplicate(Library_SpriteStudio.Data.CellMap Source)
			{
				Name = string.Copy(Source.Name);
				SizeOriginal = Source.SizeOriginal;
				ListCell = Source.ListCell;
			}
		}
		[System.Serializable]
		public class Cell
		{
			public string Name;
			public Rect Rectangle;
			public Vector2 Pivot;

			public void CleanUp()
			{
				Name = "";
				Rectangle.x = 0.0f;
				Rectangle.y = 0.0f;
				Rectangle.width = 0.0f;
				Rectangle.height = 0.0f;
				Pivot = Vector2.zero;
			}

			public void Duplicate(Library_SpriteStudio.Data.Cell Source)
			{
				Name = string.Copy(Source.Name);
				Rectangle = Source.Rectangle;
				Pivot = Source.Pivot;
			}
		}

		[System.Serializable]
		public class Parts
		{
			public string Name;

			public int ID;
			public int IDParent;
			public int[] ListIDChild;

			public Library_SpriteStudio.KindParts Kind;
			public Library_SpriteStudio.KindColorOperation KindBlendTarget;
			public Library_SpriteStudio.KindColorLabel KindLabelColor;

			public Library_SpriteStudio.KindCollision KindShapeCollision;
			public float SizeCollisionZ;

			public Object PrefabUnderControl;
			public string NameAnimationUnderControl;

			public void CleanUp()
			{
				Name = "";

				ID = -1;
				IDParent = -1;

				Kind = Library_SpriteStudio.KindParts.NON;
				KindBlendTarget = Library_SpriteStudio.KindColorOperation.NON;
				KindLabelColor = Library_SpriteStudio.KindColorLabel.NON;

				KindShapeCollision = Library_SpriteStudio.KindCollision.NON;
				SizeCollisionZ = 0.0f;

				PrefabUnderControl = null;
				NameAnimationUnderControl = "";
			}
		}

		[System.Serializable]
		public class Animation
		{
			public string Name;
			public int FramePerSecond;
			public int CountFrame;
			public Library_SpriteStudio.Data.Label[] ListLabel;
			public Library_SpriteStudio.Data.AnimationParts[] ListAnimationParts;

			public void CleanUp()
			{
				Name = "";
				FramePerSecond = 0;
				CountFrame = 0;
				ListLabel = null;
				ListAnimationParts = null;
			}

			public int CountGetLabel()
			{
				return((null != ListLabel) ? ListLabel.Length : 0);
			}

			public int IndexGetLabel(string NameLabel)
			{
				if((true == string.IsNullOrEmpty(NameLabel)) || (null == ListLabel))
				{
					return(-1);
				}

				int Count;
				Count = (int)Library_SpriteStudio.KindLabelAnimationReserved.TERMINATOR;
				for(int i=0; i<Count; i++)
				{
//					if(0 == string.Compare(NameLabel, ListNameLabelAnimationReserved[i]))
					if(NameLabel == ListNameLabelAnimationReserved[i])
					{
						return((int)Library_SpriteStudio.KindLabelAnimationReserved.INDEX_RESERVED + i);
					}
				}

				Count = ListLabel.Length;
				for(int i=0; i<Count; i++)
				{
//					if(0 == string.Compare(NameLabel, ListLabel[i].Name))
					if(NameLabel ==  ListLabel[i].Name)
					{
						return(i);
					}
				}
				return(-1);
			}

			public Library_SpriteStudio.Data.Label DataGetLabel(int Index)
			{
				return(((0 <= Index) && (ListLabel.Length > Index)) ? ListLabel[Index] : null);
			}
			public int FrameNoGetLabel(int Index)
			{
				if((int)Library_SpriteStudio.KindLabelAnimationReserved.INDEX_RESERVED <= Index)
				{	/* Reserved-Index */
					Index -= (int)Library_SpriteStudio.KindLabelAnimationReserved.INDEX_RESERVED;
					switch(Index)
					{
						case (int)Library_SpriteStudio.KindLabelAnimationReserved.START:
							return(0);

						case (int)Library_SpriteStudio.KindLabelAnimationReserved.END:
							return(CountFrame - 1);

						default:
							break;
					}
					return(-1);
				}

//				if((0 <= Index) && (ListLabel.Length > Index))
				if((0 > Index) || (ListLabel.Length <= Index))
				{
					return(-1);
				}

				Label DataLabel = DataGetLabel(Index);
				if(null == DataLabel)
				{
					return(-1);
				}
				return(DataLabel.FrameNo);
			}
			public string NameGetLabel(int Index)
			{
				if((int)Library_SpriteStudio.KindLabelAnimationReserved.INDEX_RESERVED <= Index)
				{	/* Reserved-Index */
					Index -= (int)Library_SpriteStudio.KindLabelAnimationReserved.INDEX_RESERVED;
					if((0 > Index) || ((int)Library_SpriteStudio.KindLabelAnimationReserved.TERMINATOR <= Index))
					{	/* Error */
						return(null);
					}
					return(Library_SpriteStudio.ListNameLabelAnimationReserved[Index]);
				}

				Label DataLabel = DataGetLabel(Index);
				if(null == DataLabel)
				{
					return(null);
				}
				return(DataLabel.Name);
			}

			public Library_SpriteStudio.Data.AnimationParts DataGetAnimationParts(int ID)
			{
				return(((0 <= ID) && (ListAnimationParts.Length > ID)) ? ListAnimationParts[ID] : null);
			}

			public void FrameRangeGet(	out int FrameNoStart,
										out int FrameNoEnd,
										out int IndexLabelStart,
										out int IndexLabelEnd,
										string LabelStart,
										int FrameOffsetStart,
										string LabelEnd,
										int FrameOffsetEnd
									)
			{
				int Index;
				int FrameNo;
				int FrameNoLast = CountFrame - 1;
				string Name = null;

				/* Start-Frame */
				Name = (true == string.IsNullOrEmpty(LabelStart))
						? Library_SpriteStudio.ListNameLabelAnimationReserved[(int)Library_SpriteStudio.KindLabelAnimationReserved.START]
						: LabelStart;
				Index = IndexGetLabel(Name);
				if(0 > Index)
				{	/* Error */
					Index = (int)(Library_SpriteStudio.KindLabelAnimationReserved.START | Library_SpriteStudio.KindLabelAnimationReserved.INDEX_RESERVED);
				}
				FrameNo = FrameNoGetLabel(Index);
				IndexLabelStart = Index;
				FrameNoStart = (0 > FrameNo) ? 0 : FrameNo;
				FrameNoStart += FrameOffsetStart;
				FrameNoStart = ((0 > FrameNoStart) || (FrameNoLast < FrameNoStart)) ? 0 : FrameNoStart;

				/* End-Frame */
				Name = (true == string.IsNullOrEmpty(LabelEnd))
						? Library_SpriteStudio.ListNameLabelAnimationReserved[(int)Library_SpriteStudio.KindLabelAnimationReserved.END]
						: LabelEnd;
				Index = IndexGetLabel(Name);
				if(0 > Index)
				{	/* Error */
					Index = (int)(Library_SpriteStudio.KindLabelAnimationReserved.END | Library_SpriteStudio.KindLabelAnimationReserved.INDEX_RESERVED);
				}
				FrameNo = FrameNoGetLabel(Index);
				IndexLabelEnd = Index;
				FrameNoEnd = (0 > FrameNo) ? FrameNoLast : FrameNo;
				FrameNoEnd += FrameOffsetEnd;
				FrameNoEnd = ((0 > FrameNoEnd) || (FrameNoLast < FrameNoEnd)) ? FrameNoLast : FrameNoEnd;
			}

			public void Fix()
			{
				int Count = ListAnimationParts.Length;
				for(int i=0; i<Count; i++)
				{
					ListAnimationParts[i].Fix(CountFrame);
				}
			}

			public void Compress()
			{
				int Count = ListAnimationParts.Length;
				for(int i=0; i<Count; i++)
				{
					ListAnimationParts[i].Compress(CountFrame);
				}
			}
			public void Decompress()
			{
				int Count = ListAnimationParts.Length;
				for(int i=0; i<Count; i++)
				{
					ListAnimationParts[i].Decompress(CountFrame);
				}
			}
		}

		[System.Serializable]
		public class Label
		{
			public string Name;
			public int FrameNo;

			public void CleanUp()
			{
				Name = "";
				FrameNo = -1;
			}
		}

		[System.Serializable]
		public class AnimationParts
		{
			[System.Flags]
			public enum FlagBitStatus
			{
				HIDE_FULL = 0x40000000,

				CLEAR = 0x00000000
			}
			public Library_SpriteStudio.KindFormat KindFormat;

			public FlagBitStatus StatusParts;

			public Library_SpriteStudio.Data.Pack.Standard.ListAttributeStatus Status;

			public Library_SpriteStudio.Data.Pack.Standard.ListAttributeVector3 Position;	/* Always Compressed */
			public Library_SpriteStudio.Data.Pack.Standard.ListAttributeVector3 Rotation;	/* Always Compressed */
			public Library_SpriteStudio.Data.Pack.Flyweight.ListAttributeVector2 Scaling;	/* Always Compressed */

			public Library_SpriteStudio.Data.Pack.Standard.ListAttributeFloat RateOpacity;
			public Library_SpriteStudio.Data.Pack.Standard.ListAttributeFloat Priority;

			public Library_SpriteStudio.Data.Pack.Flyweight.ListAttributeVector2 PositionAnchor;	/* Reserved. */
			public Library_SpriteStudio.Data.Pack.Flyweight.ListAttributeVector2 SizeForce;

			public Library_SpriteStudio.Data.Pack.Standard.ListAttributeUserData UserData;	/* Always Compressed */
			public Library_SpriteStudio.Data.Pack.Standard.ListAttributeInstance Instance;	/* Always Compressed */
			public Library_SpriteStudio.Data.Pack.Standard.ListAttributeEffect Effect;	/* Always Compressed */

			public Library_SpriteStudio.Data.AnimationPartsPartsPlain DataPlain;
			public Library_SpriteStudio.Data.AnimationPartsPartsFix DataFix;

			public void CleanUp()
			{
				StatusParts = FlagBitStatus.CLEAR;

				KindFormat = Library_SpriteStudio.KindFormat.PLAIN;

				Status = null;

				Position = null;
				Rotation = null;
				Scaling = null;

				RateOpacity = null;
				Priority = null;

				PositionAnchor = null;
				SizeForce = null;

				UserData = null;
				Instance = null;
				Effect = null;

				DataPlain = null;
				DataFix = null;
			}

			public bool Fix(int CountFrame)
			{
				/* MEMO: Always Compressed */
				if(null != Position)
				{
					Position.CompressCPE(CountFrame);
				}
				if(null != Rotation)
				{
					Rotation.CompressCPE(CountFrame);
				}

				/* MEMO: Always Compressed */
				/* MEMO: Stored in Compressed-State from scratch. */
//				if(null != UserData)
//				{
//					UserData.CompressCPE(CountFrame);
//				}
				/* MEMO: Always Compressed */
				/* MEMO: Stored in Compressed-State from scratch. */
//				if(null != Instance)
//				{
//					Instance.CompressCPE(CountFrame);
//				}
				/* MEMO: Always Compressed */
				/* MEMO: Stored in Compressed-State from scratch. */
//				if(null != Effect)
//				{
//					Effect.CompressCPE(CountFrame);
//				}

				if(null != DataPlain)
				{
					DataPlain.Fix(CountFrame);
				}
				if(null != DataFix)
				{
					DataFix.Fix(CountFrame);
				}

				return(true);
			}

			public bool Compress(int CountFrame)
			{
				if(null != Status)
				{
					Status.CompressCPE(CountFrame);
				}

				/* MEMO: Always Compressed */
//				if(null != Position)
//				{
//					Position.CompressCPE(CountFrame);
//				}
				/* MEMO: Always Compressed */
//				if(null != Rotation)
//				{
//					Rotation.CompressCPE(CountFrame);
//				}
				if(null != RateOpacity)
				{
					RateOpacity.CompressCPE(CountFrame);
				}
				if(null != Priority)
				{
					Priority.CompressCPE(CountFrame);
				}

				/* MEMO: Always Compressed */
//				if(null != UserData)
//				{
//					UserData.CompressCPE(CountFrame);
//				}
				/* MEMO: Always Compressed */
//				if(null != Instance)
//				{
//					Instance.CompressCPE(CountFrame);
//				}
				/* MEMO: Always Compressed */
//				if(null != Effect)
//				{
//					Effect.CompressCPE(CountFrame);
//				}

				if(null != DataPlain)
				{
					DataPlain.Compress(CountFrame);
				}
				if(null != DataFix)
				{
					DataFix.Compress(CountFrame);
				}

				return(true);
			}
			public bool Decompress(int CountFrame)
			{
				if(null != Status)
				{
					Status.DecompressCPE(CountFrame);
				}

				/* MEMO: Always Compressed */
//				if(null != Position)
//				{
//					Position.DecompressCPE(CountFrame);
//				}
				/* MEMO: Always Compressed */
//				if(null != Rotation)
//				{
//					Rotation.DecompressCPE(CountFrame);
//				}

				if(null != RateOpacity)
				{
					RateOpacity.DecompressCPE(CountFrame);
				}
				if(null != Priority)
				{
					Priority.DecompressCPE(CountFrame);
				}

				/* MEMO: Always Compressed */
//				if(null != UserData)
//				{
//					UserData.DecompressCPE();
//				}
				/* MEMO: Always Compressed */
//				if(null != Instance)
//				{
//					Instance.DecompressCPE();
//				}
				/* MEMO: Always Compressed */
//				if(null != Effect)
//				{
//					Effect.DecompressCPE();
//				}

				if(null != DataPlain)
				{
					DataPlain.Decompress(CountFrame);
				}
				if(null != DataFix)
				{
					DataFix.Decompress(CountFrame);
				}

				return(true);
			}
		}
		[System.Serializable]
		public class AnimationPartsPartsPlain
		{
			public Library_SpriteStudio.Data.Pack.Standard.ListAttributeCell Cell;

			public Library_SpriteStudio.Data.Pack.Standard.ListAttributeColorBlend ColorBlend;
			public Library_SpriteStudio.Data.Pack.Standard.ListAttributeVertexCorrection VertexCorrection;
			public Library_SpriteStudio.Data.Pack.Standard.ListAttributeVector2 OffsetPivot;

			public Library_SpriteStudio.Data.Pack.Standard.ListAttributeVector2 PositionTexture;
			public Library_SpriteStudio.Data.Pack.Standard.ListAttributeVector2 ScalingTexture;
			public Library_SpriteStudio.Data.Pack.Standard.ListAttributeFloat RotationTexture;

			public Library_SpriteStudio.Data.Pack.Standard.ListAttributeFloat RadiusCollision;	/* for Sphere-Collider *//* Always Compressed */

			public void CleanUp()
			{
				Cell = null;

				ColorBlend = null;
				VertexCorrection = null;
				OffsetPivot = null;

				PositionTexture = null;
				ScalingTexture = null;
				RotationTexture = null;

				RadiusCollision = null;
			}

			public bool Fix(int CountFrame)
			{
				/* MEMO: Always Compressed */
				if(null != RadiusCollision)
				{
					RadiusCollision.CompressCPE(CountFrame);
				}

				return(true);
			}

			public bool Compress(int CountFrame)
			{
				if(null != Cell)
				{
					Cell.CompressCPE(CountFrame);
				}

				if(null != ColorBlend)
				{
					ColorBlend.CompressCPE(CountFrame);
				}

				if(null != VertexCorrection)
				{
					VertexCorrection.CompressCPE(CountFrame);
				}

				if(null != OffsetPivot)
				{
					OffsetPivot.CompressCPE(CountFrame);
				}

				if(null != PositionTexture)
				{
					PositionTexture.CompressCPE(CountFrame);
				}

				if(null != ScalingTexture)
				{
					ScalingTexture.CompressCPE(CountFrame);
				}

				if(null != RotationTexture)
				{
					RotationTexture.CompressCPE(CountFrame);
				}

				/* MEMO: Always Compressed */
//				if(null != RadiusCollision)
//				{
//					RadiusCollision.CompressCPE(CountFrame);
//				}

				return(true);
			}
			public bool Decompress(int CountFrame)
			{
				if(null != Cell)
				{
					Cell.DecompressCPE(CountFrame);
				}

				if(null != ColorBlend)
				{
					ColorBlend.DecompressCPE(CountFrame);
				}

				if(null != VertexCorrection)
				{
					VertexCorrection.DecompressCPE(CountFrame);
				}

				if(null != OffsetPivot)
				{
					OffsetPivot.DecompressCPE(CountFrame);
				}

				if(null != PositionTexture)
				{
					PositionTexture.DecompressCPE(CountFrame);
				}

				if(null != ScalingTexture)
				{
					ScalingTexture.DecompressCPE(CountFrame);
				}

				if(null != RotationTexture)
				{
					RotationTexture.DecompressCPE(CountFrame);
				}

				/* MEMO: Always Compressed */
//				if(null != RadiusCollision)
//				{
//					RadiusCollision.DecompressCPE(CountFrame);
//				}

				return(true);
			}
		}
		[System.Serializable]
		public class AnimationPartsPartsFix
		{
			public Library_SpriteStudio.Data.Pack.Standard.ListAttributeIndexCellMapFix IndexCellMapMesh;
			public Library_SpriteStudio.Data.Pack.Flyweight.ListAttributeCoordinateMeshFix CoordinateMesh;
			public Library_SpriteStudio.Data.Pack.Flyweight.ListAttributeColorBlendMeshFix ColorBlendMesh;
			public Library_SpriteStudio.Data.Pack.Flyweight.ListAttributeUVMeshFix UV0Mesh;

			public Library_SpriteStudio.Data.Pack.Flyweight.ListAttributeVector2 SizeCollision;	/* for Box-Collider *//* Always Compressed */
			public Library_SpriteStudio.Data.Pack.Flyweight.ListAttributeVector2 PivotCollision;	/* for Box-Collider *//* Always Compressed */

			public Library_SpriteStudio.Data.Pack.Standard.ListAttributeFloat RadiusCollision;	/* for Sphere-Collider *//* Always Compressed */

			public void CleanUp()
			{
				CoordinateMesh = null;
				ColorBlendMesh = null;
				UV0Mesh = null;

				SizeCollision = null;
				PivotCollision = null;
				RadiusCollision = null;
			}

			public bool Fix(int CountFrame)
			{
				/* MEMO: Always Compressed */
				if(null != IndexCellMapMesh)
				{
					IndexCellMapMesh.CompressCPE(CountFrame);
				}
				/* MEMO: Always Compressed */
				if(null != RadiusCollision)
				{
					RadiusCollision.CompressCPE(CountFrame);
				}

				return(true);
			}

			public bool Compress(int CountFrame)
			{
				/* MEMO: Always Compressed */
//				if(null != IndexCellMapMesh)
//				{
//					IndexCellMapMesh.CompressCPE(CountFrame);
//				}

				/* MEMO: Always Compressed */
//				if(null != RadiusCollision)
//				{
//					RadiusCollision.CompressCPE(CountFrame);
//				}

				return(true);
			}
			public bool Decompress(int CountFrame)
			{
				/* MEMO: Always Compressed */
//				if(null != IndexCellMapMesh)
//				{
//					IndexCellMapMesh.DecompressCPE(CountFrame);
//				}

				/* MEMO: Always Compressed */
//				if(null != RadiusCollision)
//				{
//					RadiusCollision.DecompressCPE(CountFrame);
//				}

				return(true);
			}
		}

		[System.Serializable]
		public class EmitterEffect
		{
			[System.Serializable]
			public class PatternEmit
			{
				public int IndexGenerate;
//				public int Offset;
				public int Duration;
				public int Cycle;

				public void CleanUp()
				{
					IndexGenerate = -1;
//					Offset = -1;
					Duration = -1;
					Cycle = -1;
				}
			}

			public enum Constant
			{
				SEED_MAGIC = 7573,

				LIFE_EXTEND_SCALE = 8,
				LIFE_EXTEND_MIN = 64,
			}

			[System.Flags]
			public enum FlagBit
			{
				/* for Particle */
				BASIC = 0x00000001,	/* Reserved */
				TANGENTIALACCELATION = 0x00000002,
				TURNDIRECTION = 0x00000004,
				SEEDRANDOM = 0x00000008,
				DELAY = 0x00000010,

				POSITION = 0x00000100,
				POSITION_FLUCTUATION = 0x00000200,	/* Reserved */
				ROTATION = 0x00000400,
				ROTATION_FLUCTUATION = 0x00000800,
				SCALE_START = 0x00001000,
				SCALE_END = 0x00002000,

				SPEED = 0x00010000,	/* Reserved */
				SPEED_FLUCTUATION = 0x00020000,
				GRAVITY_DIRECTION = 0x00040000,
				GRAVITY_POINT = 0x00080000,

				COLORVERTEX = 0x00100000,
				COLORVERTEX_FLUCTUATION = 0x00200000,
				FADEALPHA = 0x00400000,

				/* for Emitter */
				EMIT_INFINITE = 0x01000000,

				/* Mask-Bit and etc. */
				CLEAR = 0x00000000,
				MASK_EMITTER = 0x7f000000,
				MASK_PARTICLE = 0x00ffffff,
				MASK_VALID = 0x7fffffff,
			}
			public FlagBit FlagData;

			/* Datas for Particle */
			public Library_SpriteStudio.KindColorOperationEffect KindBlendTarget;
			public int IndexCellMap;
			public int IndexCell;

			public AttributeEffectRangeFloat Angle;

			public Vector2 GravityDirectional;
			public Vector2 GravityPointPosition;
			public float GravityPointPower;

			public AttributeEffectRangeVector2 Position;

			public AttributeEffectRangeFloat Rotation;
			public AttributeEffectRangeFloat RotationFluctuation;
			public float RotationFluctuationRate;
			public float RotationFluctuationRateTime;

			public AttributeEffectRangeFloat RateTangentialAcceleration;
	
			public AttributeEffectRangeVector2 ScaleStart;
			public AttributeEffectRangeFloat ScaleRateStart;

			public AttributeEffectRangeVector2 ScaleEnd;
			public AttributeEffectRangeFloat ScaleRateEnd;

			public int Delay;

			public AttributeEffectRangeColor ColorVertex;
			public AttributeEffectRangeColor ColorVertexFluctuation;

			public float AlphaFadeStart;
			public float AlphaFadeEnd;

			public AttributeEffectRangeFloat Speed;
			public AttributeEffectRangeFloat SpeedFluctuation;

			public float TurnDirectionFluctuation;

			public long SeedRandom;

			/* Datas for Emitter */
			public int DurationEmitter;
			public int Interval;
			public AttributeEffectRangeFloat DurationParticle;

			public float PriorityParticle;
			public int CountParticleMax;
			public int CountParticleEmit;

			public int CountPartsMaximum;	/* DisUse?? */
			public PatternEmit[] TablePatternEmit;
			public int[] TablePatternOffset;
			public long[] TableSeedParticle;

			public void CleanUp()
			{
				FlagData = FlagBit.CLEAR;

				KindBlendTarget = KindColorOperationEffect.MIX;
				IndexCellMap = -1;
				IndexCell = -1;

				DurationParticle = new AttributeEffectRangeFloat();
				DurationParticle.Main = 0.0f;
				DurationParticle.Sub = 0.0f;

				Angle = new AttributeEffectRangeFloat();
				Angle.Main = 0.0f;
				Angle.Sub = 0.0f;

				GravityDirectional = Vector2.zero;
				GravityPointPosition = Vector2.zero;
				GravityPointPower = 0.0f;

				Position = new AttributeEffectRangeVector2();
				Position.Main = Vector2.zero;
				Position.Sub = Vector2.zero;

				Rotation = new AttributeEffectRangeFloat();
				Rotation.Main = 0.0f;
				Rotation.Sub = 0.0f;

				RotationFluctuation = new AttributeEffectRangeFloat();
				RotationFluctuation.Main = 0.0f;
				RotationFluctuation.Sub = 0.0f;
				RotationFluctuationRate = 0.0f;
				RotationFluctuationRateTime = 0.0f;

				RateTangentialAcceleration = new AttributeEffectRangeFloat();
				RateTangentialAcceleration.Main = 0.0f;
				RateTangentialAcceleration.Sub = 0.0f;
	
				ScaleStart = new AttributeEffectRangeVector2();
				ScaleStart.Main = Vector2.zero;
				ScaleStart.Sub = Vector2.zero;
				ScaleRateStart = new AttributeEffectRangeFloat();
				ScaleRateStart.Main = 0.0f;
				ScaleRateStart.Sub = 0.0f;

				ScaleEnd = new AttributeEffectRangeVector2();
				ScaleEnd.Main = Vector2.zero;
				ScaleEnd.Sub = Vector2.zero;
				ScaleRateEnd = new AttributeEffectRangeFloat();
				ScaleRateEnd.Main = 0.0f;
				ScaleRateEnd.Sub = 0.0f;

				Delay = 0;

				ColorVertex = new AttributeEffectRangeColor();
				ColorVertex.Main = new Color(1.0f, 1.0f, 1.0f, 1.0f);
				ColorVertex.Sub = new Color(1.0f, 1.0f, 1.0f, 1.0f);
				ColorVertexFluctuation = new AttributeEffectRangeColor();
				ColorVertexFluctuation.Main = new Color(1.0f, 1.0f, 1.0f, 1.0f);
				ColorVertexFluctuation.Sub = new Color(1.0f, 1.0f, 1.0f, 1.0f);

				AlphaFadeStart = 0.0f;
				AlphaFadeEnd = 0.0f;

				Speed = new AttributeEffectRangeFloat();
				Speed.Main = 0.0f;
				Speed.Sub = 0.0f;
				SpeedFluctuation = new AttributeEffectRangeFloat();
				SpeedFluctuation.Main = 0.0f;
				SpeedFluctuation.Sub = 0.0f;

				TurnDirectionFluctuation = 0.0f;

				SeedRandom = (int)Constant.SEED_MAGIC;

				DurationEmitter = 15;
				Interval = 1;
				DurationParticle = new AttributeEffectRangeFloat();
				DurationParticle.Main = 15.0f;
				DurationParticle.Sub = 15.0f;

				PriorityParticle = 64.0f;
				CountParticleEmit = 2;
				CountParticleMax = 32;

				CountPartsMaximum = 0;
				TablePatternEmit = null;
				TablePatternOffset = null;
				TableSeedParticle = null;
			}

			public void Copy(Library_SpriteStudio.Data.EmitterEffect Source)
			{
				FlagData = Source.FlagData;

				KindBlendTarget = Source.KindBlendTarget;
				IndexCellMap = Source.IndexCellMap;
				IndexCell = Source.IndexCell;

				DurationParticle = Source.DurationParticle;

				Angle = Source.Angle;

				GravityDirectional = Source.GravityDirectional;
				GravityPointPosition = Source.GravityPointPosition;
				GravityPointPower = Source.GravityPointPower;

				Position = Source.Position;

				Rotation = Source.Rotation;
				RotationFluctuation = Source.RotationFluctuation;
				RotationFluctuationRate = Source.RotationFluctuationRate;
				RotationFluctuationRateTime = Source.RotationFluctuationRateTime;

				RateTangentialAcceleration = Source.RateTangentialAcceleration;

				ScaleStart = Source.ScaleStart;
				ScaleRateStart = Source.ScaleRateStart;
				ScaleEnd = Source.ScaleEnd;
				ScaleRateEnd = Source.ScaleRateEnd;
			
				Delay = Source.Delay;

				ColorVertex = Source.ColorVertex;
				ColorVertexFluctuation = Source.ColorVertexFluctuation;

				AlphaFadeStart = Source.AlphaFadeStart;
				AlphaFadeEnd = Source.AlphaFadeEnd;

				Speed = Source.Speed;
				SpeedFluctuation = Source.SpeedFluctuation;

				TurnDirectionFluctuation = Source.TurnDirectionFluctuation;
				SeedRandom = Source.SeedRandom;

				DurationEmitter = Source.DurationEmitter;
				Interval = Source.Interval;
				DurationParticle = Source.DurationParticle;

				PriorityParticle = Source.PriorityParticle;
				CountParticleMax = Source.CountParticleMax;
				CountParticleEmit = Source.CountParticleEmit;

				CountPartsMaximum = Source.CountPartsMaximum;
				TablePatternEmit = Source.TablePatternEmit;
				TablePatternOffset = Source.TablePatternOffset;
				TableSeedParticle = Source.TableSeedParticle;
			}

			public static void TableGetOffset(	ref int[] DataTablePatternOffset,
												EmitterEffect DataEmitter
											)
			{
				int CountEmitMax = DataEmitter.CountParticleMax;
				int CountEmit = DataEmitter.CountParticleEmit;
				CountEmit = (1 > CountEmit) ? 1 : CountEmit;

				/* Create Offset-Pattern Table */
				/* MEMO: This Table will be solved at Importing. */
				int Shot = 0;
				int Offset = DataEmitter.Delay;
				int Count = CountEmitMax;
				DataTablePatternOffset = new int[Count];
				for(int i=0; i<Count; i++)
				{
					if(Shot >= CountEmit)
					{
						Shot = 0;
						Offset += DataEmitter.Interval;
					}
					DataTablePatternOffset[i] = Offset;
					Shot++;
				}
			}
			public static void TableGet(	ref PatternEmit[] DataTablePatternEmit,
											ref long[] DataTableSeedParticle,
											EmitterEffect DataEmitter,
											Library_SpriteStudio.Utility.Random.Generator InstanceRandom,
											uint SeedRandom
									)
			{	/* CAUTION!: Obtain "TablePatternOffset" before executing this function. */
				int CountEmitMax = DataEmitter.CountParticleMax;

				List<Library_SpriteStudio.Data.EmitterEffect.PatternEmit> ListPatternEmit = new List<Library_SpriteStudio.Data.EmitterEffect.PatternEmit>();
				ListPatternEmit.Clear();

				int CountEmit = DataEmitter.CountParticleEmit;
				CountEmit = (1 > CountEmit) ? 1 : CountEmit;

				int Cycle = (int)(((float)(CountEmitMax * DataEmitter.Interval) / (float)CountEmit) + 0.5f);
				int Count;

				/* Create Emit-Pattern Table */
				/* MEMO: This Table will be solved at Importing (at SeedRandom is fixed). */
				InstanceRandom.InitSeed(SeedRandom);
				Count = CountEmitMax * (int)Constant.LIFE_EXTEND_SCALE;
				if((int)Constant.LIFE_EXTEND_MIN > Count)
				{
					Count = (int)Constant.LIFE_EXTEND_MIN;
				}
				DataTablePatternEmit = new PatternEmit[Count];
				int Duration;
				for(int i=0; i<Count; i++)
				{
					DataTablePatternEmit[i] = new PatternEmit();
					DataTablePatternEmit[i].IndexGenerate = i;
					Duration = (int)((float)DataEmitter.DurationParticle.Main + InstanceRandom.RandomFloat((float)DataEmitter.DurationParticle.Sub));
					DataTablePatternEmit[i].Duration = Duration;
					DataTablePatternEmit[i].Cycle = (Duration > Cycle) ? Duration : Cycle;
				}

				/* Create Random-Seed Table */
				/* MEMO: This Table will be solved at Importing (at SeedRandom is fixed). */
				Count = CountEmitMax * 3;
				DataTableSeedParticle = new long[Count];
				InstanceRandom.InitSeed(SeedRandom);
				for(int i=0; i<Count; i++)
				{
					DataTableSeedParticle[i] = (long)((ulong)InstanceRandom.RandomUint32());
				}
			}
			private static int TableGetSortPattern(Library_SpriteStudio.Data.EmitterEffect.PatternEmit Left, Library_SpriteStudio.Data.EmitterEffect.PatternEmit Right)
			{
				int KeyDuration = Left.Duration - Right.Duration;
				int KeyID = Left.IndexGenerate - Right.IndexGenerate;
				if(0 > KeyDuration)
				{
					return(-1);
				}
				if(0 < KeyDuration)
				{
					return(1);
				}
				/* MEMO: Left.Duration == Right.Duration */
				if(0 > KeyID)
				{
					return(-1);
				}
				if(0 < KeyID)
				{
					return(1);
				}
				return(0);			 
			}
		}

		[System.Serializable]
		public class PartsEffect
		{
			public string Name;

			public int ID;
			public int IDParent;
			public int[] ListIDChild;

			public Library_SpriteStudio.KindPartsEffect Kind;	/* Preliminary ... "Root"or"Emitter" */
			public int IndexEmitter;	/* -1 == Not "Emitter" */

			public void CleanUp()
			{
				Name = "";

				ID = -1;
				IDParent = -1;
				ListIDChild = null;

				Kind = Library_SpriteStudio.KindPartsEffect.NON;
				IndexEmitter = -1;
			}
		}

		public static int NameCheckLabelReserved(string Name)
		{
			if(false == string.IsNullOrEmpty(Name))
			{
				for(int i=0; i<(int)Library_SpriteStudio.KindLabelAnimationReserved.TERMINATOR; i++)
				{
					if(Name == Library_SpriteStudio.ListNameLabelAnimationReserved[i])
					{
						return(i);
					}
				}
			}
			return(-1);
		}

		public static partial class Pack
		{
		}
	}

	public partial class Control
	{
		[System.Serializable]
		public class Parts
		{
			[System.Flags]
			internal enum FlagBitStatus
			{
				VALID = 0x40000000,
				RUNNING = 0x20000000,

				HIDEFORCE = 0x08000000,

				CHANGE_TRANSFORM_POSITION = 0x00100000,
				CHANGE_TRANSFORM_ROTATION = 0x00200000,
				CHANGE_TRANSFORM_SCALING = 0x00400000,

				OVERWRITE_CELL_UNREFLECTED = 0x00080000,
				OVERWRITE_CELL_IGNOREATTRIBUTE = 0x00040000,

				INSTANCE_VALID = 0x00008000,
				INSTANCE_PLAYINDEPENDENT = 0x00004000,

				EFFECT_VALID = 0x00000800,
				EFFECT_PLAYINDEPENDENT = 0x00000400,

				CLEAR = 0x00000000
			}
			internal FlagBitStatus Status;
			internal bool StatusHideForce
			{
				get
				{
					return(0 != (Status & FlagBitStatus.HIDEFORCE));
				}
				set
				{
					if(true == value)
					{
						Status |= FlagBitStatus.HIDEFORCE;
					}
					else
					{
						Status &= ~FlagBitStatus.HIDEFORCE;
					}
				}
			}

			public bool FlagHideForceInitial;

			public Library_SpriteStudio.Data.Parts DataParts;
			internal Library_SpriteStudio.Data.AnimationParts DataAnimationParts;
#if DRAWPARTS_ORDER_SOLVINGJUSTINTIME
			internal int PartsIDNext;
#endif

			public GameObject InstanceGameObject;
			internal Transform InstanceTransform;
			internal int IndexPreviousPosition;
			internal int IndexPreviousRotation;
			internal int IndexPreviousScaling;

			internal Script_SpriteStudio_Collider InstanceCollider;
			internal Collider InstanceComponentCollider;

			internal ParameterParts BufferParameterParts;
			internal ParameterMesh BufferParameterMesh;
			internal Library_SpriteStudio.ManagerDraw.DataParts DataPartsDrawManager;

			public Object PrefabUnderControl;
			public string NameAnimationUnderControl;
			public GameObject InstanceGameObjectUnderControl;
			internal Script_SpriteStudio_Root InstanceRootUnderControl;
			internal Script_SpriteStudio_RootEffect InstanceRootUnderControlEffect;
			internal int FrameNoPreviousUpdateUnderControl;

			internal int IndexPreviousCell;
			internal int IndexCellMapOverwrite;
			internal int IndexCellOverwrite;

			public void CleanUp()
			{
//				Status =

				FlagHideForceInitial = false;

				DataParts = null;
//				DataAnimationParts =
#if DRAWPARTS_ORDER_SOLVINGJUSTINTIME
//				PartsIDNext = 
#endif
				InstanceGameObject = null;
//				InstanceTransform =
//				IndexPreviousPosition =
//				IndexPreviousRotation =
//				IndexPreviousScaling =

//				InstanceCollider =
//				InstanceComponentCollider =

//				BufferParameterMesh =
//				DataPartsDrawManager =

				PrefabUnderControl = null;
				NameAnimationUnderControl = "";
//				InstanceGameObjectUnderControl =
//				InstanceRootUnderControl =
//				InstanceRootUnderControlEffect =

//				FrameNoPreviousUpdateUnderControl = -1;
				CleanUpWorkArea();
			}

			private void CleanUpWorkArea()
			{
				Status = FlagBitStatus.CLEAR;

//				FlagHideForceInitial =

//				DataParts =
				DataAnimationParts = null;
#if DRAWPARTS_ORDER_SOLVINGJUSTINTIME
//				PartsIDNext = 
#endif

//				InstanceGameObject =
				InstanceTransform = null;
//				IndexPreviousPosition =
//				IndexPreviousRotation =
//				IndexPreviousScaling =

				InstanceCollider = null;
				InstanceComponentCollider = null;

				BufferParameterMesh = null;
				DataPartsDrawManager = null;

//				PrefabUnderControl =
//				NameAnimationUnderControl =
//				InstanceGameObjectUnderControl = 
//				InstanceRootUnderControl = 
//				InstanceRootUnderControlEffect = 

//				FrameNoPreviousUpdateUnderControl =
				CleanUpWorkAreaAnimationSet();
			}

			private void CleanUpWorkAreaAnimationSet()
			{
//				Status =

//				FlagHideForceInitial =

//				DataParts =
//				DataAnimationParts =
#if DRAWPARTS_ORDER_SOLVINGJUSTINTIME
				PartsIDNext = 0;
#endif

//				InstanceGameObject =
//				InstanceTransform =
				IndexPreviousPosition = -1;
				IndexPreviousRotation = -1;
				IndexPreviousScaling = -1;

//				InstanceCollider =
//				InstanceComponentCollider =

//				BufferParameterMesh =
//				DataPartsDrawManager =

//				PrefabUnderControl =
//				NameAnimationUnderControl =
//				InstanceGameObjectUnderControl =
//				InstanceRootUnderControl =
//				InstanceRootUnderControlEffect =

				FrameNoPreviousUpdateUnderControl = -1;

				IndexPreviousCell = -1;
				if(0 == (Status & FlagBitStatus.OVERWRITE_CELL_IGNOREATTRIBUTE))
				{
					IndexCellMapOverwrite = -1;
					IndexCellOverwrite = -1;
				}
			}

			public bool BootUp(	Script_SpriteStudio_Root InstanceRootInitial,
								int IDPartsInitial,
								GameObject InstanceGameObjectInitial
							)
			{
				CleanUpWorkArea();

				if(null == DataParts)
				{
					DataParts = InstanceRootInitial.DataAnimation.DataGetParts(IDPartsInitial);
				}
				Library_SpriteStudio.KindParts KindParts = DataParts.Kind;

				DataAnimationParts = null;

				InstanceGameObject = InstanceGameObjectInitial;
				if(null == PrefabUnderControl)
				{
					switch(KindParts)
					{
						case Library_SpriteStudio.KindParts.ROOT:
						case Library_SpriteStudio.KindParts.NULL:
						case Library_SpriteStudio.KindParts.NORMAL_TRIANGLE2:
						case Library_SpriteStudio.KindParts.NORMAL_TRIANGLE4:
							PrefabUnderControl = null;
							break;

						case Library_SpriteStudio.KindParts.INSTANCE:
							PrefabUnderControl = DataParts.PrefabUnderControl;
							break;

						case Library_SpriteStudio.KindParts.EFFECT:
							PrefabUnderControl = DataParts.PrefabUnderControl;
							break;

						default:
							return(false);
					}
				}

				BufferParameterMesh = null;
				DataPartsDrawManager = null;
//				InstanceGameObjectUnderControl = null;
//				InstanceRootUnderControl = null;
//				InstanceRootUnderControlEffect = null;

				return(true);

//			BootUp_ErrorEnd:;
//				CleanUpWorkArea();
//				return(false);
			}
			public bool BootUpRuntime(	Script_SpriteStudio_Root InstanceRootInitial,
										int IDPartsInitial
									)
			{
				CleanUpWorkArea();
				Library_SpriteStudio.KindParts Kind = DataParts.Kind;
				switch(Kind)
				{
					case Library_SpriteStudio.KindParts.ROOT:	/* Root-Parts (Subspecies of "NULL"-Parts) */
					case Library_SpriteStudio.KindParts.NULL:	/* NULL-Parts */
						break;

					case Library_SpriteStudio.KindParts.NORMAL_TRIANGLE2:	/* No use Vertex-Collection Sprite-Parts */
					case Library_SpriteStudio.KindParts.NORMAL_TRIANGLE4:	/* Use Vertex-Collection Sprite-Parts */
						{
							/* DrawManager WorkArea Create */
							DataPartsDrawManager = new Library_SpriteStudio.ManagerDraw.DataParts();
							DataPartsDrawManager.CleanUp();
							DataPartsDrawManager.BootUp(InstanceRootInitial, Kind, InstanceGameObject);

							/* Mesh-Parameter-Buffer Create */
							BufferParameterMesh = new Library_SpriteStudio.Control.ParameterMesh();
							BufferParameterMesh.CleanUp();
							BufferParameterMesh.BootUp(Kind);
						}
						break;

					case Library_SpriteStudio.KindParts.INSTANCE:	/* Instance-Parts */
						{
							/* DrawManager WorkArea Create */
							DataPartsDrawManager = new Library_SpriteStudio.ManagerDraw.DataParts();
							DataPartsDrawManager.CleanUp();
							DataPartsDrawManager.BootUp(InstanceRootInitial, Kind, InstanceGameObject);

							/* Instance-Parts Get */
							if(null == PrefabUnderControl)
							{
								PrefabUnderControl = (InstanceRootInitial.DataAnimation.DataGetParts(IDPartsInitial)).PrefabUnderControl;
							}
							if(true == string.IsNullOrEmpty(NameAnimationUnderControl))
							{
								NameAnimationUnderControl = DataParts.NameAnimationUnderControl;
							}
							RebootPrefabInstance(InstanceRootInitial, IDPartsInitial, false);
						}
						break;

					case Library_SpriteStudio.KindParts.EFFECT:	/* Effect-Parts */
						{
							/* DrawManager WorkArea Create */
							DataPartsDrawManager = new Library_SpriteStudio.ManagerDraw.DataParts();
							DataPartsDrawManager.CleanUp();
							DataPartsDrawManager.BootUp(InstanceRootInitial, Kind, InstanceGameObject);

							/* Instance-Parts Get */
							if(null == PrefabUnderControl)
							{
								PrefabUnderControl = (InstanceRootInitial.DataAnimation.DataGetParts(IDPartsInitial)).PrefabUnderControl;
							}
							RebootPrefabInstanceEffect(InstanceRootInitial, IDPartsInitial, false);
						}
						break;

					default:
						break;
				}

				InstanceTransform = InstanceGameObject.transform;

				InstanceCollider = InstanceGameObject.GetComponent<Script_SpriteStudio_Collider>();
				InstanceComponentCollider = InstanceGameObject.GetComponent<Collider>();

				Status |= FlagBitStatus.VALID;
				Status |= FlagBitStatus.RUNNING;
				Status |= (true == FlagHideForceInitial) ? FlagBitStatus.HIDEFORCE : FlagBitStatus.CLEAR;

				return(true);

			}
			internal bool RebootPrefabInstance(	Script_SpriteStudio_Root InstanceRootInitial,
												int IDPartsInitial,
												bool FlagRenew = false
											)
			{
				if(null != PrefabUnderControl)
				{
					/* Create UnderControl-Instance */
					InstanceGameObjectUnderControl = Library_SpriteStudio.Miscellaneousness.Asset.PrefabInstantiateChild(InstanceGameObject, (GameObject)PrefabUnderControl, InstanceGameObjectUnderControl, FlagRenew);
					if(null != InstanceGameObjectUnderControl)
					{
						InstanceRootUnderControl = InstanceGameObjectUnderControl.GetComponent<Script_SpriteStudio_Root>();
						InstanceRootUnderControl.InstanceRootParent = InstanceRootInitial;

						int IndexAnimation = (true == string.IsNullOrEmpty(NameAnimationUnderControl)) ? 0 : InstanceRootUnderControl.IndexGetAnimation(NameAnimationUnderControl);
						IndexAnimation = (-1 == IndexAnimation) ? 0 : IndexAnimation;
						InstanceRootUnderControl.AnimationPlay(IndexAnimation);
						InstanceRootUnderControl.AnimationStop();
					}
				}
				return(true);
			}
			internal bool RebootPrefabInstanceEffect(	Script_SpriteStudio_Root InstanceRootInitial,
														int IDPartsInitial,
														bool FlagRenew = false
													)
			{
				if(null != PrefabUnderControl)
				{
					/* Create UnderControl-Instance */
					InstanceGameObjectUnderControl = Library_SpriteStudio.Miscellaneousness.Asset.PrefabInstantiateChild(InstanceGameObject, (GameObject)PrefabUnderControl, InstanceGameObjectUnderControl, FlagRenew);
					if(null != InstanceGameObjectUnderControl)
					{
						InstanceRootUnderControlEffect = InstanceGameObjectUnderControl.GetComponent<Script_SpriteStudio_RootEffect>();
						InstanceRootUnderControlEffect.InstanceRootParent = InstanceRootInitial;
						InstanceRootUnderControlEffect.FlagUnderControl = true;
					}
				}
				return(true);
			}

			public bool AnimationSet(Script_SpriteStudio_Root InstanceRoot, int AnimationNo, int IDParts)
			{
				CleanUpWorkAreaAnimationSet();

				Library_SpriteStudio.Data.Animation DataAnimation = InstanceRoot.DataAnimation.DataGetAnimation(AnimationNo);
				if(null == DataAnimation)
				{
					DataAnimationParts = null;
					return(false);
				}
				DataAnimationParts = DataAnimation.DataGetAnimationParts(IDParts);
				if(null == DataAnimationParts)
				{
					return(false);
				}
				return(true);
			}

			internal int PartIDGetDrawNext(int FrameNo)
			{
#if DRAWPARTS_ORDER_SOLVINGJUSTINTIME
				return(PartsIDNext);
#else
				return(-1);
#endif
			}

			internal bool UpdateGameObject(Script_SpriteStudio_Root InstanceRoot, int FrameNo)
			{
				int IndexAttribute;
				int FrameNoOrigin;

				/* Update Transform */
				/* MEMO: No Transform-Datas, Not Changing "Transform" */
				IndexAttribute = DataAnimationParts.Position.IndexGetValue(out FrameNoOrigin, FrameNo);
				if(0 <= IndexAttribute)
				{	/* Has Data */
					if(IndexPreviousPosition != IndexAttribute)
					{
						InstanceTransform.localPosition = DataAnimationParts.Position.ListValue[IndexAttribute];
						IndexPreviousPosition = IndexAttribute;
						Status |= FlagBitStatus.CHANGE_TRANSFORM_POSITION;
					}
				}
				else
				{	/* Has no Data */
					if(0 != (Status & FlagBitStatus.CHANGE_TRANSFORM_POSITION))
					{
						InstanceTransform.localPosition = Vector3.zero;
						Status &= ~FlagBitStatus.CHANGE_TRANSFORM_POSITION;
					}
				}
				IndexAttribute = DataAnimationParts.Rotation.IndexGetValue(out FrameNoOrigin, FrameNo);
				if(0 <= IndexAttribute)
				{	/* Has Data */
					if(IndexPreviousRotation != IndexAttribute)
					{
						Quaternion QuaternionTemp = Quaternion.Euler(DataAnimationParts.Rotation.ListValue[IndexAttribute]);
						InstanceTransform.localRotation = QuaternionTemp;
						IndexPreviousRotation = IndexAttribute;
						Status |= FlagBitStatus.CHANGE_TRANSFORM_ROTATION;
					}
				}
				else
				{	/* Has no Data */
					if(0 != (Status & FlagBitStatus.CHANGE_TRANSFORM_ROTATION))
					{
						InstanceTransform.localRotation = Quaternion.identity;
						Status &= ~FlagBitStatus.CHANGE_TRANSFORM_ROTATION;
					}
				}
				IndexAttribute = DataAnimationParts.Scaling.IndexGetValue(out FrameNoOrigin, FrameNo);
				if(0 <= IndexAttribute)
				{	/* Has Data */
					if(IndexPreviousScaling != IndexAttribute)
					{
						Vector3 VectorTemp = DataAnimationParts.Scaling.GetValue(InstanceRoot.DataAnimation.Flyweight, IndexAttribute);
						VectorTemp.z = 1.0f;
						InstanceTransform.localScale = VectorTemp;
						IndexPreviousScaling = IndexAttribute;
						Status |= FlagBitStatus.CHANGE_TRANSFORM_SCALING;
					}
				}
				else
				{	/* Has no Data */
					if(0 != (Status & FlagBitStatus.CHANGE_TRANSFORM_SCALING))
					{
						InstanceTransform.localScale = Vector3.one;
						Status &= ~FlagBitStatus.CHANGE_TRANSFORM_SCALING;
					}
				}

				/* Status Get */
				IndexAttribute = DataAnimationParts.Status.IndexGetValue(out FrameNoOrigin, FrameNo);
				Library_SpriteStudio.Data.AttributeStatus DataStatus = (0 <= IndexAttribute) ? DataAnimationParts.Status.ListValue[IndexAttribute] : Library_SpriteStudio.Data.DummyStatus;
#if DRAWPARTS_ORDER_SOLVINGJUSTINTIME
				PartsIDNext = DataStatus.PartsIDNext;	/* Cache */
#endif
				BufferParameterParts.FlagHide = DataStatus.IsHide;
				BufferParameterParts.IndexVertexCollectionTable = 0;
				if(true == DataStatus.IsFlipX)
				{
					BufferParameterParts.RateScaleMesh.x = -1.0f;
					BufferParameterParts.IndexVertexCollectionTable += 1;
				}
				else
				{
					BufferParameterParts.RateScaleMesh.x = 1.0f;
				}
				if(true == DataStatus.IsFlipY)
				{
					BufferParameterParts.RateScaleMesh.y = -1.0f;
					BufferParameterParts.IndexVertexCollectionTable += 2;
				}
				else
				{
					BufferParameterParts.RateScaleMesh.y = 1.0f;
				}
				BufferParameterParts.RateScaleTexture.x = (true == DataStatus.IsTextureFlipX) ? -1.0f : 1.0f;
				BufferParameterParts.RateScaleTexture.y = (true == DataStatus.IsTextureFlipY) ? -1.0f : 1.0f;

				/* Parts-Size Data Set (Default) */
				BufferParameterParts.IndexCellMap = -1;
				BufferParameterParts.DataCell = null;
				BufferParameterParts.SizePixelMesh = Vector2.zero;
				BufferParameterParts.PivotMesh = Vector2.zero;
				BufferParameterParts.SizeTextureOriginal = new Vector2(64.0f, 64.0f);

				/* Update Sprite-Base Data & Collider */
				switch(DataAnimationParts.KindFormat)
				{
					case Library_SpriteStudio.KindFormat.PLAIN:
						{
							/* Cell Get */
							/* MEMO: It is able to perform "Cell-Table Change" and "Cell Overwrite", only when data is "Plain" format. */
							bool FlagCellOverwrite = (0 != (Status & (FlagBitStatus.OVERWRITE_CELL_IGNOREATTRIBUTE | FlagBitStatus.OVERWRITE_CELL_UNREFLECTED))) ? true : false;
							Status &= ~FlagBitStatus.OVERWRITE_CELL_UNREFLECTED;
							IndexAttribute = DataAnimationParts.DataPlain.Cell.IndexGetValue(out FrameNoOrigin, FrameNo);
							if(FrameNoOrigin != IndexPreviousCell)
							{
								IndexPreviousCell = FrameNoOrigin;
								if(false == FlagCellOverwrite)
								{
									IndexCellMapOverwrite = -1;
									IndexCellOverwrite = -1;
								}
							}

							if(0 <= IndexAttribute)
							{
								int IndexCellMap = IndexCellMapOverwrite;
								int IndexCell = IndexCellOverwrite;
								if((0 > IndexCellMap) || (0 > IndexCell))
								{	/* Attribute */
									Library_SpriteStudio.Data.AttributeCell AttributeCell = DataAnimationParts.DataPlain.Cell.ListValue[IndexAttribute];
									IndexCellMap = AttributeCell.IndexCellMap;
									IndexCell = AttributeCell.IndexCell;
								}

								Library_SpriteStudio.Data.CellMap DataCellMap = null;
								Library_SpriteStudio.Data.Cell DataCell = null;
								Library_SpriteStudio.Control.CellChange[][] TableCellChange = InstanceRoot.TableCellChange;
								if(null != TableCellChange)
								{	/* Cell-Table Changed */
									if((0 <= IndexCellMap) && (TableCellChange.Length > IndexCellMap))
									{
										Library_SpriteStudio.Control.CellChange[] ListCellChange = TableCellChange[IndexCellMap];
										if((0 <= IndexCell) && (ListCellChange.Length > IndexCell))
										{
											ListCellChange[IndexCell].DataGet(ref IndexCellMap, ref DataCellMap, ref DataCell);
										}
									}
								}
								else
								{	/* Default */
									DataCellMap = InstanceRoot.DataCellMap.DataGetCellMap(IndexCellMap);
									if(null != DataCellMap)
									{
										DataCell = DataCellMap.DataGetCell(IndexCell);
									}
								}

								if(null == DataCell)
								{	/* Invalid */
									BufferParameterParts.IndexCellMap = -1;
									BufferParameterParts.SizeTextureOriginal = Vector2.zero;

									BufferParameterParts.DataCell = null;
									BufferParameterParts.PivotMesh = Vector2.zero;
									BufferParameterParts.SizePixelMesh.x = 64.0f;
									BufferParameterParts.SizePixelMesh.y = 64.0f;
								}
								else
								{	/* Valid */
									BufferParameterParts.IndexCellMap = IndexCellMap;
									BufferParameterParts.SizeTextureOriginal = DataCellMap.SizeOriginal;

									BufferParameterParts.DataCell = DataCell;
									BufferParameterParts.PivotMesh = DataCell.Pivot;
									BufferParameterParts.SizePixelMesh.x = DataCell.Rectangle.width;
									BufferParameterParts.SizePixelMesh.y = DataCell.Rectangle.height;
								}
							}

							/* Recalc Mesh Size & Pivot (Considering SizeForce-X/Y & OffsetPivot-X/Y) */
							MeshRecalcSizeAndPivot(	ref BufferParameterParts.PivotMesh,
													ref BufferParameterParts.SizePixelMesh,
													ref BufferParameterParts.RateScaleMesh,
													FrameNo,
													InstanceRoot.DataAnimation.Flyweight
												);
						}

						/* Update Collider */
						if(null != InstanceComponentCollider)
						{
							goto UpdateGameObject_Collison_Plain;
						}
						break;

					case Library_SpriteStudio.KindFormat.FIX:
						/* Parts-Size Data Set (Default) */
						IndexAttribute = DataAnimationParts.DataFix.IndexCellMapMesh.IndexGetValue(out FrameNoOrigin, FrameNo);
						if(0 <= IndexAttribute)
						{
							BufferParameterParts.IndexCellMap = DataAnimationParts.DataFix.IndexCellMapMesh.ListValue[IndexAttribute];
						}

						/* Update Collider */
						if(null != InstanceComponentCollider)
						{
							goto UpdateGameObject_Collison_Fix;
						}
						break;

					default:
						break;
				}

			UpdateGameObject_Collison_End:;
				return(true);

			UpdateGameObject_Collison_Plain:;
				switch(DataParts.KindShapeCollision)
				{
					case Library_SpriteStudio.KindCollision.SQUARE:
						{
							/* Calculate Sprite-Parts size */
							Vector2 SizeNew = BufferParameterParts.SizePixelMesh;
							Vector2 PivotNew = BufferParameterParts.PivotMesh;
							PivotNew.x = -(PivotNew.x - (SizeNew.x * 0.5f)) * BufferParameterParts.RateScaleMesh.x;
							PivotNew.y = (PivotNew.y - (SizeNew.y * 0.5f)) * BufferParameterParts.RateScaleMesh.y;
							if((PivotNew != InstanceCollider.ColliderRectPivotPrevious) || (SizeNew != InstanceCollider.ColliderRectSizePrevious))
							{	/* Update */
								/* Update Previous Buffer */
								InstanceCollider.ColliderRectPivotPrevious = PivotNew;
								InstanceCollider.ColliderRectSizePrevious = SizeNew;

								/* Update Collider */
								BoxCollider ColliderBox = InstanceComponentCollider as BoxCollider;
								ColliderBox.enabled = true;
								ColliderBox.size = new Vector3(SizeNew.x, SizeNew.y, DataParts.SizeCollisionZ);
								ColliderBox.center = new Vector3(PivotNew.x, PivotNew.y, 0.0f);
							}
						}
						break;

					case Library_SpriteStudio.KindCollision.CIRCLE:
						{
							IndexAttribute = DataAnimationParts.DataPlain.RadiusCollision.IndexGetValue(out FrameNoOrigin, FrameNo);
							float RadiusNew = (0 <= IndexAttribute) ? DataAnimationParts.DataPlain.RadiusCollision.ListValue[IndexAttribute] : 1.0f;
							if(RadiusNew != InstanceCollider.ColliderRadiusPrevious)
							{	/* Update */
								/* Update Previous Buffer */
								InstanceCollider.ColliderRadiusPrevious = RadiusNew;

								/* Update Collider */
								CapsuleCollider ColliderCupsule = InstanceComponentCollider as CapsuleCollider;
								ColliderCupsule.enabled = true;
								ColliderCupsule.radius = RadiusNew;
								ColliderCupsule.center = Vector3.zero;
							}
						}
						break;

					default:
						break;
				}
				goto UpdateGameObject_Collison_End;

			UpdateGameObject_Collison_Fix:;
				switch(DataParts.KindShapeCollision)
				{
					case Library_SpriteStudio.KindCollision.SQUARE:
						{
							IndexAttribute = DataAnimationParts.DataFix.SizeCollision.IndexGetValue(out FrameNoOrigin, FrameNo);
							int IndexAttributeCollisionPivot = DataAnimationParts.DataFix.PivotCollision.IndexGetValue(out FrameNoOrigin, FrameNo);
							if((0 <= IndexAttribute) && (0 <= IndexAttributeCollisionPivot))
							{
								Vector2 DataSizeCollision = DataAnimationParts.DataFix.SizeCollision.GetValue(InstanceRoot.DataAnimation.Flyweight, IndexAttribute);
								Vector2 DataPivotCollision = DataAnimationParts.DataFix.PivotCollision.GetValue(InstanceRoot.DataAnimation.Flyweight, IndexAttributeCollisionPivot);
								if((DataPivotCollision != InstanceCollider.ColliderRectPivotPrevious) || (DataSizeCollision != InstanceCollider.ColliderRectSizePrevious))
								{	/* Update */
									/* Update Previous Buffer */
									InstanceCollider.ColliderRectPivotPrevious = DataPivotCollision;
									InstanceCollider.ColliderRectSizePrevious = DataSizeCollision;

									/* Update Collider */
									BoxCollider ColliderBox = InstanceComponentCollider as BoxCollider;
									ColliderBox.enabled = true;
									ColliderBox.size = new Vector3(DataSizeCollision.x, DataSizeCollision.y, DataParts.SizeCollisionZ);
									ColliderBox.center = new Vector3(DataPivotCollision.x, DataPivotCollision.y, 0.0f);
								}
							}
						}
						break;

					case Library_SpriteStudio.KindCollision.CIRCLE:
						{
							IndexAttribute = DataAnimationParts.DataFix.RadiusCollision.IndexGetValue(out FrameNoOrigin, FrameNo);
							float RadiusNew = (0 <= IndexAttribute) ? DataAnimationParts.DataFix.RadiusCollision.ListValue[IndexAttribute] : 1.0f;
							if(RadiusNew != InstanceCollider.ColliderRadiusPrevious)
							{	/* Update */
								/* Update Previous Buffer */
								InstanceCollider.ColliderRadiusPrevious = RadiusNew;

								/* Update Collider */
								CapsuleCollider ColliderCupsule = InstanceComponentCollider as CapsuleCollider;
								ColliderCupsule.enabled = true;
								ColliderCupsule.radius = RadiusNew;
								ColliderCupsule.center = Vector3.zero;
							}
						}
						break;

					default:
						break;
				}
				goto UpdateGameObject_Collison_End;
			}

			internal bool UpdateMesh(Script_SpriteStudio_Root InstanceRoot, int FrameNo)
			{
				Library_SpriteStudio.Control.ColorBlendOverwrite DataColorBlendOverwrite = InstanceRoot.DataColorBlendOverwrite;
				Mesh InstanceMesh = DataPartsDrawManager.DrawParts.Data.InstanceMesh;
				if(null == InstanceMesh)
				{
					return(false);
				}

				/* Hide Check */
				if(0 != (Status & FlagBitStatus.HIDEFORCE))
				{	/* Force-Hide */
					return(true);
				}
				if(true == BufferParameterParts.FlagHide)
				{	/* Animation-Data's Hide */
					return(true);
				}

				/* Update Sprite-Base Data & Collider */
				Library_SpriteStudio.Control.ParameterMesh InstanceParameterMesh = BufferParameterMesh;
				Vector3[] nextCoordinate;
				Vector2[] nextUV;
				Vector2[] nextUV2;
				Color32[] nextColor;

				int FrameNoOrigin;
				int IndexAttribute;
				switch(DataAnimationParts.KindFormat)
				{
					case Library_SpriteStudio.KindFormat.PLAIN:
						goto UpdateMesh_Plain;

					case Library_SpriteStudio.KindFormat.FIX:
						goto UpdateMesh_Fix;

					default:
						break;
				}
				return(true);

			UpdateMesh_Plain:;
				/* for PLAIN */
				{
					int CountVertexData = InstanceParameterMesh.Coordinate.Length;
					nextCoordinate = InstanceParameterMesh.Coordinate;
					nextUV = InstanceParameterMesh.UV;
					nextUV2 = InstanceParameterMesh.UV2;
					nextColor = InstanceParameterMesh.ColorOverlay;

					int	VertexCollectionIndexTableNo = BufferParameterParts.IndexVertexCollectionTable;
					Vector2 PivotMesh = BufferParameterParts.PivotMesh;
					Vector2 RateScaleMesh = BufferParameterParts.RateScaleMesh;
					Vector2 PivotTexture = Vector2.zero;
					Vector2 RateScaleTexture = Vector2.one;
					Rect RectCell = Rect.MinMaxRect(0.0f, 0.0f, 64.0f, 64.0f);
					Vector2 SizeTextureOriginal = new Vector2(64.0f, 64.0f);
					Matrix4x4 MatrixTexture;	/* = Matrix4x4.identity; */

					/* Cell Data Get */
					if(null != BufferParameterParts.DataCell)
					{
						SizeTextureOriginal = BufferParameterParts.SizeTextureOriginal;

						RectCell = BufferParameterParts.DataCell.Rectangle;
						PivotTexture.x = RectCell.width * 0.5f;
						PivotTexture.y = RectCell.height * 0.5f;

						RateScaleTexture = BufferParameterParts.RateScaleTexture;
						IndexAttribute = DataAnimationParts.DataPlain.ScalingTexture.IndexGetValue(out FrameNoOrigin, FrameNo);
						if(0 <= IndexAttribute)
						{
							Vector2 DataScalingTexture = DataAnimationParts.DataPlain.ScalingTexture.ListValue[IndexAttribute];
							RateScaleTexture.x *= DataScalingTexture.x;
							RateScaleTexture.y *= DataScalingTexture.y;
						}
					}

					/* Calculate Matrix-Texture & Set Mapping-UV */
					{
						IndexAttribute = DataAnimationParts.DataPlain.RotationTexture.IndexGetValue(out FrameNoOrigin, FrameNo);
						float Rotate = (0 <= IndexAttribute) ? DataAnimationParts.DataPlain.RotationTexture.ListValue[IndexAttribute] : 0.0f;

						IndexAttribute = DataAnimationParts.DataPlain.PositionTexture.IndexGetValue(out FrameNoOrigin, FrameNo);
						Vector2 TextureOffset = (0 <= IndexAttribute) ? DataAnimationParts.DataPlain.PositionTexture.ListValue[IndexAttribute] : Vector2.zero;

						Vector3 Translation = new Vector3(	((RectCell.xMin + PivotTexture.x) / SizeTextureOriginal.x) + TextureOffset.x,
															((SizeTextureOriginal.y - (RectCell.yMin + PivotTexture.y)) / SizeTextureOriginal.y) - TextureOffset.y,
															0.0f
														);
						Vector3 Scaling = new Vector3(	(RectCell.width / SizeTextureOriginal.x) * RateScaleTexture.x,
														(RectCell.height / SizeTextureOriginal.y) * RateScaleTexture.y,
														1.0f
													);
						Quaternion Rotation = Quaternion.Euler(0.0f, 0.0f, -Rotate);
						MatrixTexture = Matrix4x4.TRS(Translation, Rotation, Scaling);
					}
					for(int i=0; i<CountVertexData; i++)
					{	/* Memo: "ArrayUVMappingUV0_Triangle4" of the data up to the "VertexNo.TERMINATOR2"-th elements are same as those of "ArrayUVMappingUV0_Triangle2". */
						InstanceParameterMesh.UV[i] = MatrixTexture.MultiplyPoint3x4(ArrayUVMappingUV0_Triangle4[i]);
					}

					/* ColorBlend & RateOpacity */
					{
						IndexAttribute = DataAnimationParts.RateOpacity.IndexGetValue(out FrameNoOrigin, FrameNo);
						float RateOpacity = (0 <= IndexAttribute) ? DataAnimationParts.RateOpacity.ListValue[IndexAttribute] : 1.0f;
						RateOpacity *= InstanceRoot.RateOpacity;

						Vector2 DataUV2 = new Vector2(	RateOpacity,
														(float)Library_SpriteStudio.KindColorOperation.NON + 0.01f	/* "+0.01f" for Rounding-off-Error */
													);

						if((null != DataColorBlendOverwrite) && (KindColorOperation.NON != DataColorBlendOverwrite.Operation))
						{	/* Overwrite */
							DataUV2.y = (float)DataColorBlendOverwrite.Operation + 0.01f;	/* "+0.01f" for Rounding-off-Error */
							for(int i=0; i<(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2; i++)
							{
								InstanceParameterMesh.UV2[i] = DataUV2;
								InstanceParameterMesh.ColorOverlay[i] = DataColorBlendOverwrite.VertexColor[i];
							}
							goto UpdateMesh_Plain_ColorBlend_End;
						}
						else
						{	/* Animation Data */
							IndexAttribute = DataAnimationParts.DataPlain.ColorBlend.IndexGetValue(out FrameNoOrigin, FrameNo);
							if(0 <= IndexAttribute)
							{
								Library_SpriteStudio.Data.AttributeColorBlend DataColorBlend = DataAnimationParts.DataPlain.ColorBlend.ListValue[IndexAttribute];
								DataUV2.y = (float)DataColorBlend.Operation + 0.01f;	/* "+0.01f" for Rounding-off-Error */
								if(Library_SpriteStudio.KindColorBound.NON != DataColorBlend.Bound)
								{
									for(int i=0; i<(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2; i++)
									{
										InstanceParameterMesh.UV2[i] = DataUV2;
										InstanceParameterMesh.UV2[i].x *= DataColorBlend.RatePixelAlpha[i];
										InstanceParameterMesh.ColorOverlay[i] = DataColorBlend.VertexColor[i];
									}
									goto UpdateMesh_Plain_ColorBlend_End;
								}
							}
						}

						/* MEMO: Trapping "No-Operation" */
						for(int i=0; i<(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2; i++)
						{
							InstanceParameterMesh.UV2[i] = DataUV2;
							InstanceParameterMesh.ColorOverlay[i] = Color.white;
						}
						/* MEMO: Fall-Through */
					UpdateMesh_Plain_ColorBlend_End:;
					}

					/* Mesh Coordinate */
					if((int)Library_SpriteStudio.KindVertexNo.TERMINATOR4 == CountVertexData)	/* Vertex-Coordinate */
					{	/* 4-Triangles Mesh */
						/* Get Color Blend (Center) */
						InstanceParameterMesh.UV2[(int)Library_SpriteStudio.KindVertexNo.C] = InstanceParameterMesh.UV2[0];
						Color DataColor = InstanceParameterMesh.ColorOverlay[0];
						DataColor += InstanceParameterMesh.ColorOverlay[1];
						DataColor += InstanceParameterMesh.ColorOverlay[2];
						DataColor += InstanceParameterMesh.ColorOverlay[3];
						DataColor *= 0.25f;
						InstanceParameterMesh.ColorOverlay[(int)Library_SpriteStudio.KindVertexNo.C] = DataColor;

						Vector2 DataUV2 = InstanceParameterMesh.UV2[0];
						DataUV2 += InstanceParameterMesh.UV2[1];
						DataUV2 += InstanceParameterMesh.UV2[2];
						DataUV2 += InstanceParameterMesh.UV2[3];
						DataUV2 *= 0.25f;
						InstanceParameterMesh.UV2[(int)Library_SpriteStudio.KindVertexNo.C] = DataUV2;

						/* Get Coordinates */
						/* MEMO: No Check "AnimationDataVertexCorrection.Length", 'cause 4-Triangles-Mesh necessarily has "AnimationDataVertexCorrection" */
						float Left = (-PivotMesh.x) * RateScaleMesh.x;
						float Right = (BufferParameterParts.SizePixelMesh.x - PivotMesh.x) * RateScaleMesh.x;
						float Top = -((-PivotMesh.y) * RateScaleMesh.y);	/* * -1.0f ... Y-Axis Inverse */
						float Bottom = -((BufferParameterParts.SizePixelMesh.y - PivotMesh.y) * RateScaleMesh.y);	/* * -1.0f ... Y-Axis Inverse */

						IndexAttribute = DataAnimationParts.DataPlain.VertexCorrection.IndexGetValue(out FrameNoOrigin, FrameNo);
						Library_SpriteStudio.Data.AttributeVertexCorrection VertexCorrection = (0 <= IndexAttribute) ? DataAnimationParts.DataPlain.VertexCorrection.ListValue[IndexAttribute] : Library_SpriteStudio.Data.DummyVertexCorrection;

						InstanceParameterMesh.Coordinate[(int)Library_SpriteStudio.KindVertexNo.LU] = new Vector3(	Left + VertexCorrection.Coordinate[VertexCollectionOrderVertex[VertexCollectionIndexTableNo, (int)Library_SpriteStudio.KindVertexNo.LU]].x,
																													Top + VertexCorrection.Coordinate[VertexCollectionOrderVertex[VertexCollectionIndexTableNo, (int)Library_SpriteStudio.KindVertexNo.LU]].y,
																													0.0f
																												);
						InstanceParameterMesh.Coordinate[(int)Library_SpriteStudio.KindVertexNo.RU] = new Vector3(	Right + VertexCorrection.Coordinate[VertexCollectionOrderVertex[VertexCollectionIndexTableNo, (int)Library_SpriteStudio.KindVertexNo.RU]].x,
																													Top + VertexCorrection.Coordinate[VertexCollectionOrderVertex[VertexCollectionIndexTableNo, (int)Library_SpriteStudio.KindVertexNo.RU]].y,
																													0.0f
																												);
						InstanceParameterMesh.Coordinate[(int)Library_SpriteStudio.KindVertexNo.RD] = new Vector3(	Right + VertexCorrection.Coordinate[VertexCollectionOrderVertex[VertexCollectionIndexTableNo, (int)Library_SpriteStudio.KindVertexNo.RD]].x,
																													Bottom + VertexCorrection.Coordinate[VertexCollectionOrderVertex[VertexCollectionIndexTableNo, (int)Library_SpriteStudio.KindVertexNo.RD]].y,
																													0.0f
																												);
						InstanceParameterMesh.Coordinate[(int)Library_SpriteStudio.KindVertexNo.LD] = new Vector3(	Left + VertexCorrection.Coordinate[VertexCollectionOrderVertex[VertexCollectionIndexTableNo, (int)Library_SpriteStudio.KindVertexNo.LD]].x,
																													Bottom + VertexCorrection.Coordinate[VertexCollectionOrderVertex[VertexCollectionIndexTableNo, (int)Library_SpriteStudio.KindVertexNo.LD]].y,
																													0.0f
																												);
						Vector3 CoordinateLURU = (InstanceParameterMesh.Coordinate[(int)Library_SpriteStudio.KindVertexNo.LU] + InstanceParameterMesh.Coordinate[(int)Library_SpriteStudio.KindVertexNo.RU]) * 0.5f;
						Vector3 CoordinateLULD = (InstanceParameterMesh.Coordinate[(int)Library_SpriteStudio.KindVertexNo.LU] + InstanceParameterMesh.Coordinate[(int)Library_SpriteStudio.KindVertexNo.LD]) * 0.5f;
						Vector3 CoordinateLDRD = (InstanceParameterMesh.Coordinate[(int)Library_SpriteStudio.KindVertexNo.LD] + InstanceParameterMesh.Coordinate[(int)Library_SpriteStudio.KindVertexNo.RD]) * 0.5f;
						Vector3 CoordinateRURD = (InstanceParameterMesh.Coordinate[(int)Library_SpriteStudio.KindVertexNo.RU] + InstanceParameterMesh.Coordinate[(int)Library_SpriteStudio.KindVertexNo.RD]) * 0.5f;
						Library_SpriteStudio.Miscellaneousness.Math.CoordinateGetDiagonalIntersection(	out InstanceParameterMesh.Coordinate[(int)Library_SpriteStudio.KindVertexNo.C],
																										ref CoordinateLURU,
																										ref CoordinateRURD,
																										ref CoordinateLULD,
																										ref CoordinateLDRD
																									);
					}
					else
					{	/* 2-Triangles Mesh */
						/* Get Coordinates */
						float Left = (-PivotMesh.x) * RateScaleMesh.x;
						float Right = (BufferParameterParts.SizePixelMesh.x - PivotMesh.x) * RateScaleMesh.x;
						float Top = -((-PivotMesh.y) * RateScaleMesh.y);	/* * -1.0f ... Y-Axis Inverse */
						float Bottom = -((BufferParameterParts.SizePixelMesh.y - PivotMesh.y) * RateScaleMesh.y);	/* * -1.0f ... Y-Axis Inverse */

						InstanceParameterMesh.Coordinate[(int)Library_SpriteStudio.KindVertexNo.LU] = new Vector3(Left, Top, 0.0f);
						InstanceParameterMesh.Coordinate[(int)Library_SpriteStudio.KindVertexNo.RU] = new Vector3(Right, Top, 0.0f);
						InstanceParameterMesh.Coordinate[(int)Library_SpriteStudio.KindVertexNo.RD] = new Vector3(Right, Bottom, 0.0f);
						InstanceParameterMesh.Coordinate[(int)Library_SpriteStudio.KindVertexNo.LD] = new Vector3(Left, Bottom, 0.0f);
					}
				}
				goto UpdateMesh_End;

			UpdateMesh_Fix:;
				/* for FIX */
				{
					IndexAttribute = DataAnimationParts.DataFix.CoordinateMesh.IndexGetValue(out FrameNoOrigin, FrameNo);
					if(0 <= IndexAttribute)
					{
						Library_SpriteStudio.Data.AttributeCoordinateMeshFix CoordinateMeshFix = DataAnimationParts.DataFix.CoordinateMesh.GetValue(InstanceRoot.DataAnimation.Flyweight, IndexAttribute);
						nextCoordinate = CoordinateMeshFix.Coordinate;
						int CountVertexData = nextCoordinate.Length;

						IndexAttribute = DataAnimationParts.DataFix.ColorBlendMesh.IndexGetValue(out FrameNoOrigin, FrameNo);
						Library_SpriteStudio.Data.AttributeColorBlendMeshFix ColorBlendMeshFix = DataAnimationParts.DataFix.ColorBlendMesh.GetValue(InstanceRoot.DataAnimation.Flyweight, IndexAttribute);
						if((null != DataColorBlendOverwrite) && (KindColorOperation.NON != DataColorBlendOverwrite.Operation))
						{	/* Overwrite */
							nextColor = InstanceParameterMesh.ColorOverlay;
							nextUV2 = InstanceParameterMesh.UV2;

							float KindOperation = (float)DataColorBlendOverwrite.Operation + 0.01f;	/* "+0.01f" for Rounding-off-Error */
							Color ColorAverage = Color.clear;
							Color ColorData;
							for(int i=0; i<(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2; i++)
							{
								ColorData = DataColorBlendOverwrite.VertexColor[i];
								InstanceParameterMesh.ColorOverlay[i] = ColorData;
								ColorAverage += ColorData;
								InstanceParameterMesh.UV2[i].x = ColorBlendMeshFix.UV[i].x * InstanceRoot.RateOpacity;
								InstanceParameterMesh.UV2[i].y = KindOperation;
							}
							ColorAverage *= (float)Library_SpriteStudio.KindVertexNo.TERMINATOR2;
							for(int i=(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2; i<CountVertexData; i++)
							{
								InstanceParameterMesh.ColorOverlay[i] = ColorAverage;
								InstanceParameterMesh.UV2[i].x = ColorBlendMeshFix.UV[i].x * InstanceRoot.RateOpacity;
								InstanceParameterMesh.UV2[i].y = KindOperation;
							}
						}
						else
						{
							nextColor = ColorBlendMeshFix.ColorOverlay;
							if (InstanceRoot.RateOpacity == 1f)
							{
								nextUV2 = ColorBlendMeshFix.UV;
							}
							else
							{
								nextUV2 = InstanceParameterMesh.UV2;
								for (int i = 0; i < CountVertexData; i++)
								{
									InstanceParameterMesh.UV2[i].x = ColorBlendMeshFix.UV[i].x * InstanceRoot.RateOpacity;
									InstanceParameterMesh.UV2[i].y = ColorBlendMeshFix.UV[i].y;
								}
							}
						}

						Library_SpriteStudio.Data.AttributeUVMeshFix UVMeshFix;
						DataAnimationParts.DataFix.UV0Mesh.TryGetValue(out UVMeshFix, FrameNo, InstanceRoot.DataAnimation.Flyweight);
						nextUV = UVMeshFix.UV;
					}
					else
					{
						nextCoordinate = InstanceParameterMesh.Coordinate;
						nextUV = InstanceParameterMesh.UV;
						nextUV2 = InstanceParameterMesh.UV2;
						nextColor = InstanceParameterMesh.ColorOverlay;
					}
				}
				/* goto UpdateMesh_End; */
				/* MEMO: Fall-Through */

			UpdateMesh_End:;
				/* Update Mesh */
//				int[] TrianglesMesh = InstanceMesh.triangles;
//				Object.DestroyImmediate(InstanceMesh);
//				InstanceMesh = new Mesh();
//				InstanceMesh.Clear();
				bool updateCoordinateAlways = (nextCoordinate == InstanceParameterMesh.Coordinate);
				if (updateCoordinateAlways || DataPartsDrawManager.DrawParts.Data.CurrentMeshVertices != nextCoordinate)
				{
					DataPartsDrawManager.DrawParts.Data.CurrentMeshVertices = nextCoordinate;
					InstanceMesh.vertices = nextCoordinate;
				}
//				InstanceMesh.triangles = TrianglesMesh;
				bool updateUVAlways = (nextUV == InstanceParameterMesh.UV);
				if (updateUVAlways || DataPartsDrawManager.DrawParts.Data.CurrentMeshUv != nextUV)
				{
					DataPartsDrawManager.DrawParts.Data.CurrentMeshUv = nextUV;
					InstanceMesh.uv = nextUV;
				}
				bool updateUV2Always = (nextUV2 == InstanceParameterMesh.UV2);
				if (updateUV2Always || DataPartsDrawManager.DrawParts.Data.CurrentMeshUv2 != nextUV2)
				{
					DataPartsDrawManager.DrawParts.Data.CurrentMeshUv2 = nextUV2;
					InstanceMesh.uv2 = nextUV2;
				}
				bool updateColorAlways = (nextColor == InstanceParameterMesh.ColorOverlay);
				if (updateColorAlways || DataPartsDrawManager.DrawParts.Data.CurrentMeshColor != nextColor)
				{
					DataPartsDrawManager.DrawParts.Data.CurrentMeshColor = nextColor;
					InstanceMesh.colors32 = nextColor;
				}
//				InstanceMesh.RecalculateBounds();

				/* Draw Mesh */
				Material InstanceMaterial = InstanceRoot.MaterialGet(BufferParameterParts.IndexCellMap, DataParts.KindBlendTarget);
				if((null != InstanceMaterial) && (null != DataPartsDrawManager))
				{
					/* Priority Get */
					IndexAttribute = DataAnimationParts.Priority.IndexGetValue(out FrameNoOrigin, FrameNo);
					float KeyPriority = (0 <= IndexAttribute) ? DataAnimationParts.Priority.ListValue[IndexAttribute] : 0.0f;
					KeyPriority += (float)DataParts.ID * 0.00001f;

					/* Set to Parts-Cluster */
					DataPartsDrawManager.DrawParts.Data.InstanceRoot = InstanceRoot;
#if DRAWPARTS_ORDER_SOLVINGJUSTINTIME
					DataPartsDrawManager.PartsSetDrawFixed(InstanceRoot, InstanceMaterial, KeyPriority);
#else
					DataPartsDrawManager.PartsSetDraw(InstanceRoot, InstanceMaterial, KeyPriority);
#endif
				}
				return(true);
			}
			
			public void MeshRecalcSizeAndPivot(ref Vector2 Pivot, ref Vector2 Size, ref Vector2 RateScale, int FrameNo, Library_SpriteStudio.Data.Pack.Flyweight.Factory flyweight)
			{
				int FrameNoOrigin;
				int IndexAttribute;
				IndexAttribute = DataAnimationParts.DataPlain.OffsetPivot.IndexGetValue(out FrameNoOrigin, FrameNo);
				Vector2 PivotOffset = (0 <= IndexAttribute) ? DataAnimationParts.DataPlain.OffsetPivot.ListValue[IndexAttribute] : Vector2.zero;
				Pivot.x += (Size.x * PivotOffset.x) * RateScale.x;
				Pivot.y -= (Size.y * PivotOffset.y) * RateScale.y;

				/* Arbitrate Anchor-Size */
				Vector2 SizeForce;
				if (DataAnimationParts.SizeForce.TryGetValue(out SizeForce, FrameNo, flyweight))
				{
					float RatePivot;
					if(0.0f <= SizeForce.x)
					{
						RatePivot = Pivot.x / Size.x;
						Size.x = SizeForce.x;
						Pivot.x = SizeForce.x * RatePivot;
					}
					if(0.0f <= SizeForce.y)
					{
						RatePivot = Pivot.y / Size.y;
						Size.y = SizeForce.y;
						Pivot.y = SizeForce.y * RatePivot;
					}
				}
			}
			internal bool UpdateUserData(Script_SpriteStudio_Root InstanceRoot, int FrameNo)
			{
				if(false == DataAnimationParts.UserData.IsValid)
				{	/* No-Data */
					return(true);
				}

				int LoopCount = InstanceRoot.CountLoopNow;
				bool FlagFirst = InstanceRoot.StatusIsPlayingStart;
				bool FlagReverse = InstanceRoot.StatusIsPlayingReverse;
				bool FlagReversePrevious = InstanceRoot.StatusIsPlayingReversePrevious;
				bool FlagTurn = InstanceRoot.StatusIsPlayingTurn;
				bool FlagLoop = (0 < LoopCount) ? true : false;
				bool FlagStylePingPong = InstanceRoot.StatusIsPlayStylePingpong;
				int FrameNoStart = InstanceRoot.FrameNoStart;
				int FrameNoEnd = InstanceRoot.FrameNoEnd;
				int FrameNoPrevious = -1;

				/* Decode Top-Frame Get */
				if(true == FlagFirst)
				{
					FrameNoPrevious = FrameNo;
				}
				else
				{
					FrameNoPrevious = InstanceRoot.FrameNoPrevious;
					if(true== FlagReversePrevious)
					{
						FrameNoPrevious--;
						if((false == FlagTurn) && (FrameNoPrevious < FrameNo))
						{
							return(true);
						}
					}
					else
					{
						FrameNoPrevious++;
						if((false == FlagTurn) && (FrameNoPrevious > FrameNo))
						{
							return(true);
						}
					}
				}

				/* Decoding User-Datas */
				if(true == FlagStylePingPong)
				{	/* Play-Style: PingPong */
					bool FlagStyleReverse = InstanceRoot.StatusIsPlayStyleReverse;

					/* Decoding Skipped Frame */
					if(true == FlagStyleReverse)
					{	/* Reverse */
						if(true == FlagLoop)
						{
							/* Part-Head */
							if(true == FlagReversePrevious)
							{
								FrameNoPrevious = InstanceRoot.FrameNoPrevious - 1;	/* Force */
								UpdateUserDataReverse(InstanceRoot, FrameNoPrevious, FrameNoStart, FrameNo, false);
								UpdateUserDataFoward(InstanceRoot, FrameNoStart, FrameNoEnd, FrameNo, true);
							}
							else
							{
								FrameNoPrevious = InstanceRoot.FrameNoPrevious + 1;	/* Force */
								UpdateUserDataFoward(InstanceRoot, FrameNoPrevious, FrameNoEnd, FrameNo, true);
							}

							/* Part-Loop */
							for(int i=1; i<LoopCount; i++)
							{
								UpdateUserDataReverse(InstanceRoot, FrameNoEnd, FrameNoStart, FrameNo, false);
								UpdateUserDataFoward(InstanceRoot, FrameNoStart, FrameNoEnd, FrameNo, true);
							}

							/* Part-Tail & Just-Now */
							if(true == FlagReverse)
							{	/* Now-Reverse */
								UpdateUserDataReverse(InstanceRoot, FrameNoEnd, FrameNo, FrameNo, false);
							}
							else
							{	/* Now-Foward */
								UpdateUserDataReverse(InstanceRoot, FrameNoEnd, FrameNoStart, FrameNo, false);
								UpdateUserDataFoward(InstanceRoot, FrameNoStart, FrameNo, FrameNo, true);
							}
						}
						else
						{	/* Normal */
							if(true == FlagTurn)
							{	/* Turn-Back */
								/* MEMO: No-Loop & Turn-Back ... Always "Reverse to Foward" */
								FrameNoPrevious = InstanceRoot.FrameNoPrevious - 1;	/* Force */
								UpdateUserDataReverse(InstanceRoot, FrameNoPrevious, FrameNoStart, FrameNo, false);
								UpdateUserDataFoward(InstanceRoot, FrameNoStart, FrameNo, FrameNo, true);
							}
							else
							{	/* Normal */
								if(true == FlagReverse)
								{	/* Reverse */
									UpdateUserDataReverse(InstanceRoot, FrameNoPrevious, FrameNo, FrameNo, false);
								}
								else
								{	/* Foward */
									UpdateUserDataFoward(InstanceRoot, FrameNoPrevious, FrameNo, FrameNo, true);
								}
							}
						}
					}
					else
					{	/* Normal */
						if(true == FlagLoop)
						{
							/* Part-Head */
							if(true == FlagReversePrevious)
							{
								FrameNoPrevious = InstanceRoot.FrameNoPrevious - 1;	/* Force */
								UpdateUserDataReverse(InstanceRoot, FrameNoPrevious, FrameNoStart, FrameNo, true);
							}
							else
							{
								FrameNoPrevious = InstanceRoot.FrameNoPrevious + 1;	/* Force */
								UpdateUserDataFoward(InstanceRoot, FrameNoPrevious, FrameNoEnd, FrameNo, false);
								UpdateUserDataReverse(InstanceRoot, FrameNoEnd, FrameNoStart, FrameNo, true);
							}

							/* Part-Loop */
							for(int i=1; i<LoopCount; i++)
							{
								UpdateUserDataFoward(InstanceRoot, FrameNoStart, FrameNoEnd, FrameNo, false);
								UpdateUserDataReverse(InstanceRoot, FrameNoEnd, FrameNoStart, FrameNo, true);
							}

							/* Part-Tail & Just-Now */
							if(true == FlagReverse)
							{	/* Now-Reverse */
								UpdateUserDataFoward(InstanceRoot, FrameNoStart, FrameNoEnd, FrameNo, false);
								UpdateUserDataReverse(InstanceRoot, FrameNoEnd, FrameNo, FrameNo, true);
							}
							else
							{	/* Now-Foward */
								UpdateUserDataFoward(InstanceRoot, FrameNoStart, FrameNo, FrameNo, false);
							}
						}
						else
						{	/* Normal */
							if(true == FlagTurn)
							{	/* Turn-Back */
								/* MEMO: No-Loop & Turn-Back ... Always "Foward to Revese" */
								FrameNoPrevious = InstanceRoot.FrameNoPrevious + 1;	/* Force */
								UpdateUserDataFoward(InstanceRoot, FrameNoPrevious, FrameNoEnd, FrameNo, false);
								UpdateUserDataReverse(InstanceRoot, FrameNoEnd, FrameNo, FrameNo, true);
							}
							else
							{	/* Normal */
								if(true == FlagReverse)
								{	/* Reverse */
									UpdateUserDataReverse(InstanceRoot, FrameNoPrevious, FrameNo, FrameNo, true);
								}
								else
								{	/* Foward */
									UpdateUserDataFoward(InstanceRoot, FrameNoPrevious, FrameNo, FrameNo, false);
								}
							}
						}
					}
				}
				else
				{	/* Play-Style: OneWay */
					/* Decoding Skipped Frame */
					if(true == FlagReverse)
					{	/* backwards */
						if(true == FlagLoop)
						{	/* Wrap-Around */
							/* Part-Head */
							UpdateUserDataReverse(InstanceRoot, FrameNoPrevious, FrameNoStart, FrameNo, false);

							/* Part-Loop */
							for(int j=1; j<LoopCount ; j++)
							{
								UpdateUserDataReverse(InstanceRoot, FrameNoEnd, FrameNoStart, FrameNo, false);
							}

							/* Part-Tail & Just-Now */
							UpdateUserDataReverse(InstanceRoot, FrameNoEnd, FrameNo, FrameNo, false);
						}
						else
						{	/* Normal */
							UpdateUserDataReverse(InstanceRoot, FrameNoPrevious, FrameNo, FrameNo, false);
						}
					}
					else
					{	/* foward */
						if(true == FlagLoop)
						{	/* Wrap-Around */
							/* Part-Head */
							UpdateUserDataFoward(InstanceRoot, FrameNoPrevious, FrameNoEnd, FrameNo, false);

							/* Part-Loop */
							for(int j=1; j<LoopCount; j++)
							{
								UpdateUserDataFoward(InstanceRoot, FrameNoStart, FrameNoEnd, FrameNo, false);
							}

							/* Part-Tail & Just-Now */
							UpdateUserDataFoward(InstanceRoot, FrameNoStart, FrameNo, FrameNo, false);
						}
						else
						{	/* Normal */
							UpdateUserDataFoward(InstanceRoot, FrameNoPrevious, FrameNo, FrameNo, false);
						}
					}
				}
				return(true);
			}
			private void UpdateUserDataFoward(Script_SpriteStudio_Root InstanceRoot, int FrameNoRangeStart, int FrameNoRangeEnd, int FrameNoDecode, bool FlagTurnBack)
			{
				int CountData = DataAnimationParts.UserData.ListStatus.Length;
				int FrameNoKey = -1;
				int IndexData = -1;
				for(int i=0; i<CountData; i++)
				{
					IndexData = DataAnimationParts.UserData.IndexGetForce(out FrameNoKey, i);
					if((FrameNoRangeStart <= FrameNoKey) && (FrameNoRangeEnd >= FrameNoKey))
					{	/* In-Range */
						InstanceRoot.FunctionUserData(	InstanceRoot,
														DataParts.Name,
														DataParts.ID,
														InstanceRoot.IndexAnimation,
														FrameNoDecode,
														FrameNoKey,
														DataAnimationParts.UserData.ListValue[IndexData],
														FlagTurnBack
													);
					}
				}
			}
			private void UpdateUserDataReverse(Script_SpriteStudio_Root InstanceRoot, int FrameNoRangeEnd, int FrameNoRangeStart, int FrameNoDecode, bool FlagTurnBack)
			{
				int CountData = DataAnimationParts.UserData.ListStatus.Length;
				int FrameNoKey = -1;
				int IndexData = -1;
				for(int i=(CountData-1); i>=0; i--)
				{
					IndexData = DataAnimationParts.UserData.IndexGetForce(out FrameNoKey, i);
					if((FrameNoRangeStart <= FrameNoKey) && (FrameNoRangeEnd >= FrameNoKey))
					{	/* In-Range */
						InstanceRoot.FunctionUserData(	InstanceRoot,
															DataParts.Name,
															DataParts.ID,
															InstanceRoot.IndexAnimation,
															FrameNoDecode,
															FrameNoKey,
															DataAnimationParts.UserData.ListValue[IndexData],
															FlagTurnBack
														);
					}
				}
			}

			internal bool UpdateInstance(Script_SpriteStudio_Root InstanceRoot, int FrameNo)
			{
				if(null == InstanceRootUnderControl)
				{
					return(true);
				}

				int FrameNoOrigin;
				int IndexAttribute;
				Library_SpriteStudio.Data.AttributeInstance DataInstance = null;
				bool FlagDecode = InstanceRoot.StatusIsDecodeInstance;
				bool FlagPlayIndependentNowInstance = (0 != (Status & FlagBitStatus.INSTANCE_PLAYINDEPENDENT)) ? true : false;
				bool FlagPlayReverse = InstanceRoot.StatusIsPlayingReverse;
				bool FlagPlayTurn = InstanceRoot.StatusIsPlayingTurn;
				bool FlagTopFrame = false;
				bool FlagTimeWrap = false;
				float TimeOffset = 0.0f;

				/* Top Frame Check */
				if (true == FlagPlayReverse)
				{
					FlagTopFrame = (InstanceRoot.TimeRange <= (InstanceRoot.TimeElapsed + InstanceRoot.TimePerFrameConsideredRateSpeed)) ? true : false;
				}
				else
				{
					FlagTopFrame = (0.0f > (InstanceRoot.TimeElapsed - InstanceRoot.TimePerFrameConsideredRateSpeed)) ? true : false;
				}

				/* Force-Applying Check */
				if(true == FlagPlayTurn)
				{	/* Turn */
					if(false == FlagPlayIndependentNowInstance)
					{	/* Instance-Animation is depending on parent. */
						FrameNoPreviousUpdateUnderControl = -1;
					}
				}

				/* Decode Instance-Attribute */
				if((true == FlagDecode) || (-1 == FrameNoPreviousUpdateUnderControl))
				{
					/* Data Index (& Frame-No) Get */
					IndexAttribute = DataAnimationParts.Instance.IndexGetValue(out FrameNoOrigin, FrameNo);
					if(0 <= IndexAttribute)
					{   /* Valid Data */
						if(FrameNoPreviousUpdateUnderControl != FrameNoOrigin)
						{	/* Different Attribute */
							DataInstance = DataAnimationParts.Instance.ListValue[IndexAttribute];

							bool FlagPlayReverseInstanceData = (0.0f > DataInstance.RateTime) ? true : false;
							bool FlagPlayReverseInstance = FlagPlayReverseInstanceData ^ FlagPlayReverse;

							InstanceRootUnderControl.AnimationPlay(	InstanceRootUnderControl.IndexAnimation,
																	DataInstance.PlayCount,
																	0,
																	(DataInstance.RateTime * InstanceRoot.RateSpeed) * ((true == FlagPlayReverse) ? -1.0f : 1.0f),
																	((0 != (DataInstance.Flags & Library_SpriteStudio.Data.AttributeInstance.FlagBit.PINGPONG)) ? Script_SpriteStudio_Root.PlayStyle.PINGPONG : Script_SpriteStudio_Root.PlayStyle.NORMAL),
																	DataInstance.LabelStart,
																	DataInstance.OffsetStart,
																	DataInstance.LabelEnd,
																	DataInstance.OffsetEnd
																);

							/* Adjust Starting-Time */
							/* MEMO: Necessary to set time, not frame. Because parent's elapsed time has a small excess. */
							if(true == FlagPlayReverse)
							{   /* Play-Reverse */
								FlagTimeWrap = FlagTopFrame & FlagPlayReverseInstanceData;
								if(FrameNoOrigin <= FrameNo)
								{   /* Immediately */
									TimeOffset = (float)(FrameNoOrigin - InstanceRoot.FrameNoStart);
									TimeOffset = InstanceRoot.TimeElapsed - (TimeOffset * InstanceRoot.TimePerFrame);
									InstanceRootUnderControl.TimeElapsedSetForce(TimeOffset, FlagPlayReverse, FlagTimeWrap);
								}
								else
								{	/* Wait */
									if(true == FlagPlayReverseInstance)
									{	/* Instance: Play-Reverse */
										InstanceRootUnderControl.TimeElapsedSetForce(0.0f, FlagPlayReverse, FlagTimeWrap);
										InstanceRootUnderControl.TimeDelay = 0.0f;
										InstanceRootUnderControl.AnimationStop();
									}
									else
									{	/* Instance: Play-Foward */
										TimeOffset = (float)FrameNoOrigin * InstanceRoot.TimePerFrame - InstanceRoot.TimeElapsed;
										InstanceRootUnderControl.TimeElapsedSetForce(0.0f, FlagPlayReverse, FlagTimeWrap);
										InstanceRootUnderControl.TimeDelay = TimeOffset;
									}
								}
							}
							else
							{   /* Play-Foward */
								FlagTimeWrap = FlagTopFrame & FlagPlayReverseInstanceData;
								if(FrameNoOrigin <= FrameNo)
								{   /* Immediately */
									TimeOffset = (float)(FrameNoOrigin - InstanceRoot.FrameNoStart);
									TimeOffset = InstanceRoot.TimeElapsed - (TimeOffset * InstanceRoot.TimePerFrame);
									InstanceRootUnderControl.TimeElapsedSetForce(TimeOffset, FlagPlayReverse, FlagTimeWrap);
								}
								else
								{	/* Wait */
									if(true == FlagPlayReverseInstance)
									{	/* Instance: Play-Reverse */
										InstanceRootUnderControl.TimeElapsedSetForce(0.0f, FlagPlayReverse, FlagTimeWrap);
										InstanceRootUnderControl.TimeDelay = 0.0f;
										InstanceRootUnderControl.AnimationStop();
									}
									else
									{	/* Instance: Play-Foward */
										TimeOffset = (float)FrameNoOrigin * InstanceRoot.TimePerFrame - InstanceRoot.TimeElapsed;
										InstanceRootUnderControl.TimeElapsedSetForce(0.0f, FlagPlayReverse, FlagTimeWrap);
										InstanceRootUnderControl.TimeDelay = TimeOffset;
									}
								}
							}

							/* Status Update */
							FrameNoPreviousUpdateUnderControl = FrameNoOrigin;
							Status = (0 != (DataInstance.Flags & Library_SpriteStudio.Data.AttributeInstance.FlagBit.INDEPENDENT)) ? (Status | FlagBitStatus.INSTANCE_PLAYINDEPENDENT) : (Status & ~FlagBitStatus.INSTANCE_PLAYINDEPENDENT);
						}
					}
				}

				/* Update Instance */
				InstanceRootUnderControl.LateUpdateMain();

				/* Draw Instance */
				if((null != InstanceRootUnderControl) && (null != DataPartsDrawManager))
				{
					if(false == (BufferParameterParts.FlagHide | InstanceRootUnderControl.FlagHideForce))
					{
						/* Alpha Get */
						IndexAttribute = DataAnimationParts.RateOpacity.IndexGetValue(out FrameNoOrigin, FrameNo);
						float RateOpacity = (0 <= IndexAttribute) ? DataAnimationParts.RateOpacity.ListValue[IndexAttribute] : 1.0f;
						InstanceRootUnderControl.RateOpacity = RateOpacity * InstanceRoot.RateOpacity;

						/* Priority Get */
						IndexAttribute = DataAnimationParts.Priority.IndexGetValue(out FrameNoOrigin, FrameNo);
						float KeyPriority = (0 <= IndexAttribute) ? DataAnimationParts.Priority.ListValue[IndexAttribute] : 0.0f;
						KeyPriority += (float)DataParts.ID * 0.00001f;

						/* Set to Parts-Cluster ("Call Sub-Cluster" Set) */
						DataPartsDrawManager.DrawParts.Data.InstanceRoot = InstanceRootUnderControl;
						DataPartsDrawManager.PartsSetDraw(InstanceRoot, null, KeyPriority);
					}
				}

				return(true);
			}

			internal bool UpdateEffect(Script_SpriteStudio_Root InstanceRoot, int FrameNo)
			{
				/* CAUTION!: Ver.SS5.6 Unsupported. */
				/* Ver.SS5.7 */
				int FrameNoOrigin;
				int IndexAttribute;
				bool FlagPlayReverse = InstanceRoot.StatusIsPlayingReverse;
				float TimeOffset = 0.0f;

				bool FlagUpdatingForce = InstanceRoot.StatusIsPlayingTurn;
				if((true == FlagUpdatingForce) && (0 == (Status & FlagBitStatus.EFFECT_PLAYINDEPENDENT)))
				{
					FrameNoPreviousUpdateUnderControl = -1;
				}

				/* Decode Instance-Data */
				if(true == InstanceRoot.StatusIsDecodeEffect)
				{
					/* Data Index (& Frame-No) Get */
					IndexAttribute = DataAnimationParts.Effect.IndexGetValue(out FrameNoOrigin, FrameNo);

					/* MEMO: Play-Foward */
					if(0 <= IndexAttribute)
					{	/* Valid Data */
						if(FrameNoPreviousUpdateUnderControl != FrameNoOrigin)
						{	/* Animation Set */
							Library_SpriteStudio.Data.AttributeEffect DataEffect = null;

							/* Wait Set */
							if(FrameNoOrigin <= FrameNo)
							{	/* Immediately */
								/* Play-Start */
								DataEffect = DataAnimationParts.Effect.ListValue[IndexAttribute];
								InstanceRootUnderControlEffect.AnimationPlay(	false,
																				(DataEffect.RateTime * InstanceRoot.RateSpeed) * ((true == FlagPlayReverse) ? -1.0f : 1.0f)
																			);
								InstanceRootUnderControlEffect.SeedOffsetSet((uint)InstanceRoot.CountLoop);

								/* Adjust Time */
								TimeOffset = InstanceRoot.TimeElapsed - ((float)((FrameNoOrigin - InstanceRoot.FrameNoStart) - DataEffect.FrameStart) * InstanceRoot.TimePerFrame);
								InstanceRootUnderControlEffect.TimeElapsedSetForce(TimeOffset, false);
								InstanceRootUnderControlEffect.TimeDelay = 0.0f;

								/* Status Update */
								FrameNoPreviousUpdateUnderControl = FrameNoOrigin;
								Status = (0 != (DataEffect.Flags & Library_SpriteStudio.Data.AttributeEffect.FlagBit.INDEPENDENT)) ? (Status | FlagBitStatus.EFFECT_PLAYINDEPENDENT) : (Status & ~FlagBitStatus.EFFECT_PLAYINDEPENDENT);
							}
						}
					}
				}

				/* Update Instance */
				InstanceRootUnderControlEffect.LateUpdateMain();

				/* Draw Instance */
				if((null != InstanceRootUnderControlEffect) && (null != DataPartsDrawManager))
				{
					if(false == (BufferParameterParts.FlagHide | InstanceRootUnderControlEffect.FlagHideForce))
					{
						/* Alpha Get */
						IndexAttribute = DataAnimationParts.RateOpacity.IndexGetValue(out FrameNoOrigin, FrameNo);
						float RateOpacity = (0 <= IndexAttribute) ? DataAnimationParts.RateOpacity.ListValue[IndexAttribute] : 1.0f;
						InstanceRootUnderControlEffect.RateOpacity = RateOpacity * InstanceRoot.RateOpacity;

						/* Priority Get */
						IndexAttribute = DataAnimationParts.Priority.IndexGetValue(out FrameNoOrigin, FrameNo);
						float KeyPriority = (0 <= IndexAttribute) ? DataAnimationParts.Priority.ListValue[IndexAttribute] : 0.0f;
						KeyPriority += (float)DataParts.ID * 0.00001f;

						/* Set to Parts-Cluster ("Call Sub-Cluster" Set) */
						DataPartsDrawManager.DrawParts.Data.InstanceRoot = InstanceRootUnderControlEffect;
						DataPartsDrawManager.PartsSetDraw(InstanceRoot, null, KeyPriority);
					}
				}
				return(true);
			}
		}

		internal struct ParameterParts
		{
			/* MEMO: Common */
			internal bool FlagHide;
			internal Vector2 RateScaleMesh;
			internal Vector2 RateScaleTexture;

			/* MEMO: Only "Data-Plain" */
			internal int IndexCellMap;
			internal Vector2 SizeTextureOriginal;
			internal Library_SpriteStudio.Data.Cell DataCell;
			internal Vector2 SizePixelMesh;
			internal Vector2 PivotMesh;
			internal int IndexVertexCollectionTable;

			internal void CleanUp()
			{
				FlagHide = false;
				RateScaleMesh = Vector2.one;
				RateScaleTexture = Vector2.one;

				IndexCellMap = -1;
				SizeTextureOriginal = Vector2.one;
				DataCell = null;
				SizePixelMesh = Vector2.zero;
				PivotMesh = Vector2.zero;
				IndexVertexCollectionTable = 0;
			}
		}

		internal class ParameterMesh
		{
			internal Vector3[] Coordinate;
			internal Color32[] ColorOverlay;
			internal Vector2[] UV;
			internal Vector2[] UV2;

			internal void CleanUp()
			{
				Coordinate = null;
				ColorOverlay = null;
				UV = null;
				UV2 = null;
			}

			internal bool BootUp(Library_SpriteStudio.KindParts Kind)
			{
				int Count = 0;

				switch(Kind)
				{
					case Library_SpriteStudio.KindParts.NORMAL_TRIANGLE2:
						Count = (int)Library_SpriteStudio.KindVertexNo.TERMINATOR2;
						break;

					case Library_SpriteStudio.KindParts.NORMAL_TRIANGLE4:
						Count = (int)Library_SpriteStudio.KindVertexNo.TERMINATOR4;
						break;

					default:
						goto BootUp_ErrorEnd;
				}

				Coordinate = new Vector3[Count];
				if(null == Coordinate)
				{
					goto BootUp_ErrorEnd;
				}

				ColorOverlay = new Color32[Count];
				if(null == ColorOverlay)
				{
					goto BootUp_ErrorEnd;
				}

				UV = new Vector2[Count];
				if(null == UV)
				{
					goto BootUp_ErrorEnd;
				}

				UV2 = new Vector2[Count];
				if(null == UV2)
				{
					goto BootUp_ErrorEnd;
				}

				return(true);

			BootUp_ErrorEnd:;
				CleanUp();
				return(false);
			}
		}

		internal class PoolPartsEffect
		{
			internal struct ParameterMesh
			{
				internal Vector3[] Coordinate;
				internal Color32[] ColorVertex;
				internal Vector2[] UV;
				internal Vector2[] UV2;
				internal Library_SpriteStudio.ManagerDraw.DataParts DataPartsDrawManager;

				internal void CleanUp()
				{
					Coordinate = null;
					ColorVertex = null;
					UV = null;
					UV2 = null;
					DataPartsDrawManager = null;
				}

				internal bool BootUp(Script_SpriteStudio_RootEffect InstanceRoot)
				{
					int Count = (int)Library_SpriteStudio.KindVertexNo.TERMINATOR2;

					Coordinate = new Vector3[Count];
					if(null == Coordinate)
					{
						goto BootUp_ErrorEnd;
					}

					ColorVertex = new Color32[Count];
					if(null == ColorVertex)
					{
						goto BootUp_ErrorEnd;
					}

					UV = new Vector2[Count];
					if(null == UV)
					{
						goto BootUp_ErrorEnd;
					}

					UV2 = new Vector2[Count];
					if(null == UV2)
					{
						goto BootUp_ErrorEnd;
					}
#if DRAWPARTS_POOLEFFECT_GENERATEJUSTINTIME
					DataPartsDrawManager = null;
#else
					DataPartsDrawManager = new Library_SpriteStudio.ManagerDraw.DataParts();
					DataPartsDrawManager.CleanUp();
					DataPartsDrawManager.BootUp(InstanceRoot, Library_SpriteStudio.KindParts.NORMAL_TRIANGLE2, InstanceRoot.gameObject);
#endif
					return(true);

				BootUp_ErrorEnd:;
					CleanUp();
					return(false);
				}

				internal void DrawSet(	Script_SpriteStudio_RootEffect InstanceRoot,
										ref PartsEffectEmitter InstanceEmitter,
										ref PartsEffectEmitter.ParameterParticle InstanceParameterParticle,
										ref Matrix4x4 MatrixTransform
									)
				{
					/* Transform & Set Parameter-Mesh */
					Coordinate[(int)Library_SpriteStudio.KindVertexNo.LU] = MatrixTransform.MultiplyPoint3x4(InstanceEmitter.CoordinateMeshParticle[(int)Library_SpriteStudio.KindVertexNo.LU]);
					Coordinate[(int)Library_SpriteStudio.KindVertexNo.RU] = MatrixTransform.MultiplyPoint3x4(InstanceEmitter.CoordinateMeshParticle[(int)Library_SpriteStudio.KindVertexNo.RU]);
					Coordinate[(int)Library_SpriteStudio.KindVertexNo.RD] = MatrixTransform.MultiplyPoint3x4(InstanceEmitter.CoordinateMeshParticle[(int)Library_SpriteStudio.KindVertexNo.RD]);
					Coordinate[(int)Library_SpriteStudio.KindVertexNo.LD] = MatrixTransform.MultiplyPoint3x4(InstanceEmitter.CoordinateMeshParticle[(int)Library_SpriteStudio.KindVertexNo.LD]);

					UV[(int)Library_SpriteStudio.KindVertexNo.LU] = InstanceEmitter.UVMeshParticle[(int)Library_SpriteStudio.KindVertexNo.LU];
					UV[(int)Library_SpriteStudio.KindVertexNo.RU] = InstanceEmitter.UVMeshParticle[(int)Library_SpriteStudio.KindVertexNo.RU];
					UV[(int)Library_SpriteStudio.KindVertexNo.RD] = InstanceEmitter.UVMeshParticle[(int)Library_SpriteStudio.KindVertexNo.RD];
					UV[(int)Library_SpriteStudio.KindVertexNo.LD] = InstanceEmitter.UVMeshParticle[(int)Library_SpriteStudio.KindVertexNo.LD];

					ColorVertex[(int)Library_SpriteStudio.KindVertexNo.LU] = 
					ColorVertex[(int)Library_SpriteStudio.KindVertexNo.RU] =
					ColorVertex[(int)Library_SpriteStudio.KindVertexNo.RD] =
					ColorVertex[(int)Library_SpriteStudio.KindVertexNo.LD] = InstanceParameterParticle.ColorVertex;

					UV2[(int)Library_SpriteStudio.KindVertexNo.LU].x =
					UV2[(int)Library_SpriteStudio.KindVertexNo.RU].x =
					UV2[(int)Library_SpriteStudio.KindVertexNo.RD].x =
					UV2[(int)Library_SpriteStudio.KindVertexNo.LD].x = InstanceRoot.RateOpacity;

					UV2[(int)Library_SpriteStudio.KindVertexNo.LU].y =
					UV2[(int)Library_SpriteStudio.KindVertexNo.RU].y =
					UV2[(int)Library_SpriteStudio.KindVertexNo.RD].y =
					UV2[(int)Library_SpriteStudio.KindVertexNo.LD].y = (float)KindColorOperation.NON + 0.01f;	/* Disuse */

					/* Update Mesh */
#if DRAWPARTS_POOLEFFECT_GENERATEJUSTINTIME
					if(null == DataPartsDrawManager)
					{
						DataPartsDrawManager = new Library_SpriteStudio.ManagerDraw.DataParts();
						DataPartsDrawManager.CleanUp();
						DataPartsDrawManager.BootUp(InstanceRoot, Library_SpriteStudio.KindParts.NORMAL_TRIANGLE2, InstanceRoot.gameObject);
					}
#else
#endif
					Mesh InstanceMesh = DataPartsDrawManager.DrawParts.Data.InstanceMesh;
					InstanceMesh.uv = UV;
					InstanceMesh.uv2 = UV2;
					InstanceMesh.colors32 = ColorVertex;
					InstanceMesh.vertices = Coordinate;

					DataPartsDrawManager.DrawParts.Data.CurrentMeshVertices = Coordinate;
					DataPartsDrawManager.DrawParts.Data.CurrentMeshUv = UV;
					DataPartsDrawManager.DrawParts.Data.CurrentMeshUv2 = UV2;
					DataPartsDrawManager.DrawParts.Data.CurrentMeshColor = ColorVertex;

					/* Draw Mesh */
					Material InstanceMaterial = InstanceRoot.MaterialGet(InstanceEmitter.IndexCellMapParticle, InstanceEmitter.InstanceDataEmitter.KindBlendTarget);
					if((null != InstanceMaterial) && (null != DataPartsDrawManager))
					{
						/* Set to Parts-Cluster */
						float Priority = InstanceEmitter.InstanceDataEmitter.PriorityParticle;	/* + Offset */
						DataPartsDrawManager.DrawParts.Data.InstanceRoot = InstanceRoot;
						DataPartsDrawManager.PartsSetDraw(InstanceRoot, InstanceMaterial, Priority);
					}
				}
			}

			internal enum FlagBitStatus
			{
				RUNNING = 0x40000000,
				PLAYING = 0x02000000,

				LOCKSEED = 0x08000000,
				INFINITE = 0x04000000,
				LOOP = 0x02000000,

				CLEAR = 0x00000000
			}
			internal FlagBitStatus Status = FlagBitStatus.CLEAR;

			internal PartsEffectEmitter[] PoolEmitter = null;
			internal int CountMeshParticle = 0;
			internal ParameterMesh[] ParameterMeshParticle = null;

			internal Script_SpriteStudio_RootEffect InstanceRootEffect = null;
			internal int EffectDurationFull;

			internal uint Seed;
			internal uint SeedOffset;

			internal void CleanUp()
			{
				Status = FlagBitStatus.CLEAR;

				PoolEmitter = null;
				CountMeshParticle = 0;
				ParameterMeshParticle = null;

				InstanceRootEffect = null;
				EffectDurationFull = -1;

				Seed = 0;
				SeedOffset = 0;
			}

			internal void ParticleReset()
			{
				CountMeshParticle = 0;
			}

			internal void SeedSet(uint Value)
			{
				Seed = Value * (uint)Library_SpriteStudio.Data.EmitterEffect.Constant.SEED_MAGIC;
			}
			internal void SeedOffsetSet(uint Value)
			{
				SeedOffset = (0 != (Status & FlagBitStatus.LOCKSEED)) ? 0 : Value;
			}

			internal void StatusSetLoop(bool Value)
			{
				Status = (true == Value) ? (Status | FlagBitStatus.LOOP) : (Status & ~FlagBitStatus.LOOP);
			}

			internal bool BootUpWorkArea(Script_SpriteStudio_RootEffect InstanceRoot, int CountParticle)
			{
				/* Error Check */
				if((null == InstanceRoot) || (null == InstanceRoot.DataCellMap) || (null == InstanceRoot.DataEffect))
				{
					return(false);
				}

				InstanceRootEffect = InstanceRoot;

				/* Initialize Emitter Buffer */
				int Count = InstanceRoot.DataEffect.CountGetEmitter();
				PoolEmitter = new PartsEffectEmitter[Count];
				if(null == PoolEmitter)
				{
					goto BootUp_ErrorEnd;
				}
//				for(int i=0; i<Count; i++)
//				{
//					PoolEmitter[i].BootUp();
//				}

				/* Initialize Draw-Pool (for Particle) */
				ParameterMeshParticle = new ParameterMesh[CountParticle];
				if(null == ParameterMeshParticle)
				{
					goto BootUp_ErrorEnd;
				}
				for(int i=0; i<CountParticle; i++)
				{
					ParameterMeshParticle[i].BootUp(InstanceRoot);
				}
				ParticleReset();

				return(true);

			BootUp_ErrorEnd:;
				CleanUp();
				return(false);
			}

			internal bool BootUp(Script_SpriteStudio_RootEffect InstanceRoot)
			{
				/* Error Check */
				if((null == InstanceRoot) || (null == InstanceRoot.DataCellMap) || (null == InstanceRoot.DataEffect))
				{
					return(false);
				}

				/* Emitters Boot-Up */
				Status = FlagBitStatus.CLEAR;

				SeedSet(Script_SpriteStudio_RootEffect.KeyCreateRandom());

				Library_SpriteStudio.Data.PartsEffect DataParts = null;
				Library_SpriteStudio.Data.PartsEffect DataPartsParent = null;
				Library_SpriteStudio.Data.EmitterEffect DataEmitter = null;
				int IndexEmitter;
				int IndexEmitterParent;
				int Count = InstanceRoot.DataEffect.CountGetParts();
				int CountEmitter = InstanceRoot.DataEffect.CountGetEmitter();
				bool FlagInfinite = false;
				for(int i=1; i<Count; i++)	/* MEMO: 0 == Root (has no emitter) */
				{
					DataParts = InstanceRoot.DataEffect.DataGetParts(i);
					if(null != DataParts)
					{
						IndexEmitter = DataParts.IndexEmitter;

						DataParts = InstanceRoot.DataEffect.DataGetParts(i);
						DataEmitter = InstanceRoot.DataEffect.DataGetEmitter(IndexEmitter);

						IndexEmitterParent = DataParts.IDParent;	/* Temp. */
						if(0 >= IndexEmitterParent)
						{	/* Root has no emitter */
							IndexEmitterParent = -1;
						}
						else
						{
							DataPartsParent = InstanceRoot.DataEffect.DataGetParts(IndexEmitterParent);
							IndexEmitterParent = DataPartsParent.IndexEmitter;
						}
						PoolEmitter[IndexEmitter].CleanUp();
						PoolEmitter[IndexEmitter].BootUp(InstanceRoot, DataParts, DataEmitter, IndexEmitterParent, this);

						FlagInfinite |= (0 != (DataEmitter.FlagData & Data.EmitterEffect.FlagBit.EMIT_INFINITE)) ? true : false;
					}
				}
				Status |= (true == FlagInfinite) ? FlagBitStatus.INFINITE : FlagBitStatus.CLEAR;
				InstanceRoot.StatusIsPlayingInfinity = FlagInfinite;
				Status |= (0 != (InstanceRoot.DataEffect.FlagData & Script_SpriteStudio_DataEffect.FlagBit.SEEDRANDOM_LOCK)) ? FlagBitStatus.LOCKSEED : FlagBitStatus.CLEAR;

				/* Effect's Length Get */
				int IDParent;
				int FrameGlobalNow;
				EffectDurationFull = 0;
				for (int i=0; i<CountEmitter; i++)
				{
					IDParent = PoolEmitter[i].IndexParent;
					FrameGlobalNow = PoolEmitter[i].FrameFull;
					if(0 < IDParent)    /* (0 > IDParent) */
					{	/* Sub-Emitters */
						FrameGlobalNow += PoolEmitter[IDParent].FrameFull;
					}
					PoolEmitter[i].FrameGlobal = FrameGlobalNow;

					EffectDurationFull = (FrameGlobalNow > EffectDurationFull) ? FrameGlobalNow : EffectDurationFull;
				}

				Status |= FlagBitStatus.RUNNING;
				return(true);
			}

			internal bool Update(Script_SpriteStudio_RootEffect InstanceRoot)
			{
				/* Clear Draw-Pool (for Particle) */
				ParticleReset();

				/* Emitters' Random-Seed Refresh */
				int FrameNo = (int)(InstanceRoot.FrameNow);
				int FrameNoTarget = FrameNo;
				int CountLoop = 0;
				if(0 == (Status & FlagBitStatus.INFINITE))
				{
					if(0 != (Status & FlagBitStatus.LOOP))
					{
						if(FrameNo > EffectDurationFull)
						{
							FrameNoTarget = FrameNo % EffectDurationFull;
							CountLoop = FrameNo / EffectDurationFull;
							SeedOffsetSet((uint)CountLoop);
						}
					}
				}

				/* Update Emitters */
				int CountEmitter = PoolEmitter.Length;
				bool FlagChangeCellTable = InstanceRoot.StatusIsCellTableChanged;
				for(int i=0; i<CountEmitter; i++)
				{
					if((true == FlagChangeCellTable) || (0 != (PoolEmitter[i].Status & PartsEffectEmitter.FlagBitStatus.OVERWRITE_CELL_UNREFLECTED)))
					{
						PoolEmitter[i].ParticleCellSet(InstanceRoot);
					}
				}

				/* Update Emitters */
				int IndexEmitterParent;
				for(int i=0; i<CountEmitter; i++)
				{
					PoolEmitter[i].SeedOffset = SeedOffset;	/* Update Random-Seed-Offset */

					IndexEmitterParent = PoolEmitter[i].IndexParent;
					if(0 <= IndexEmitterParent)
					{   /* Has Parent-Emitter */
						PoolEmitter[IndexEmitterParent].UpdateSubEmitters(ref PoolEmitter[i], FrameNoTarget, InstanceRoot, this, IndexEmitterParent);
					}
					else
					{	/* Has no Parent-Emitter */
						PoolEmitter[i].Update(FrameNoTarget, InstanceRoot, this , -1);
					}
				}
				return(true);
			}

			internal void DrawSet(	ref PartsEffectEmitter.ParameterParticle InstanceParameterParticle,
									Script_SpriteStudio_RootEffect InstanceRoot,
									ref PartsEffectEmitter InstanceEmitter
								)
			{
				/* Get MeshParticle-Index */
				int IndexMesh = CountMeshParticle;
				if(IndexMesh >= ParameterMeshParticle.Length)
				{
					return;
				}
				CountMeshParticle++;

				/* Transforming */
				Vector2 ScaleLayout = InstanceRoot.DataEffect.ScaleLayout;
				Matrix4x4 MatrixTransform = Matrix4x4.TRS(	new Vector3(	(InstanceParameterParticle.PositionX * ScaleLayout.x),
																			(InstanceParameterParticle.PositionY * ScaleLayout.y),
																			0.0f
																		),
															Quaternion.Euler(	0.0f,
																				0.0f,
																				(InstanceParameterParticle.RotateZ + InstanceParameterParticle.Direction)
																			),
															InstanceParameterParticle.Scale
														);

				/* Draw Mesh */
				ParameterMeshParticle[IndexMesh].DrawSet(InstanceRoot, ref InstanceEmitter, ref InstanceParameterParticle, ref MatrixTransform);
			}
		}

		internal struct PartsEffectEmitter
		{
			internal struct StatusParticle
			{
				[System.Flags]
				internal enum FlagBitStatus
				{
					EXIST = 0x40000000,	/* RUNNING */
					BORN = 	0x20000000, /* GETUP */

					CLEAR = 0x00000000,
				}
				internal FlagBitStatus Status;
				internal int ID;
				internal int Cycle;
				internal int FrameStart;
				internal int FrameEnd;

				internal void CleanUp()
				{
					Status = FlagBitStatus.CLEAR;
					ID = -1;
					Cycle = -1;
					FrameStart = -1;
					FrameEnd = -1;
				}

				internal void Update(	int Frame,
										ref PartsEffectEmitter InstanceEmitter,
										Library_SpriteStudio.Data.EmitterEffect.PatternEmit InstancePatternEmit,
										int PatternOffset,
										Library_SpriteStudio.Data.EmitterEffect.PatternEmit InstancePatternEmitTarget
									)
				{
					int DurationEmitter = InstanceEmitter.Duration;
					int FrameNow = (int)(Frame - PatternOffset);
					Status &= ~(FlagBitStatus.BORN | FlagBitStatus.EXIST);

					int CycleTarget = InstancePatternEmitTarget.Cycle;
					int DurationTarget = InstancePatternEmitTarget.Duration;
					if(0 != CycleTarget)
					{
						int CountLoop = FrameNow / CycleTarget;
						int CycleTop = CountLoop * CycleTarget;

						Cycle = CountLoop;
						FrameStart = CycleTop + PatternOffset;
						FrameEnd = FrameStart + DurationTarget;
							
						if((Frame >= FrameStart) && (Frame < FrameEnd))
						{
							Status |= (FlagBitStatus.BORN | FlagBitStatus.EXIST);
						}

						if(0 == (InstanceEmitter.InstanceDataEmitter.FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.EMIT_INFINITE))
						{
							if(FrameStart >= DurationEmitter)
							{
								Status &= ~StatusParticle.FlagBitStatus.EXIST;

								int FrameNow2 = DurationEmitter - PatternOffset;
								int CountLoop2 = FrameNow2 / CycleTarget;
								int CycleTop2 = CountLoop2 * CycleTarget;

								FrameStart = CycleTop2 + PatternOffset;
								FrameEnd = FrameStart + DurationTarget;
								Status &= ~StatusParticle.FlagBitStatus.BORN;
							}
							else
							{
								Status |= StatusParticle.FlagBitStatus.BORN;
							}
						}

						if(0 > FrameNow)
						{
							Status &= ~(StatusParticle.FlagBitStatus.BORN | StatusParticle.FlagBitStatus.EXIST);
						}
					}
				}
			}

			internal struct ParameterParticle
			{
				internal int ID;
				internal int IDParent;
				internal int FrameStart;
				internal int FrameDuration;

				internal float PositionX;
				internal float PositionY;
				internal float RotateZ;
				internal float Direction;

				internal Color ColorVertex;
				internal Vector3 Scale;

				internal void CleanUp()
				{
					ID = -1;
					IDParent = -1;
					FrameStart = -1;
					FrameDuration = -1;
				}

				internal bool Update(	float Frame,
										int Index,
										Script_SpriteStudio_RootEffect InstanceRoot,
										PoolPartsEffect InstancePoolParts,
										ref PartsEffectEmitter InstanceEmitter,
										ref StatusParticle InstanceStatusParticle,
										int IndexEmitterParent,
										ref ParameterParticle InstanceParameterParticleParent
									)
				{	/* CAUTION: "InstanceParameterParticleParent" will be broken. */
					StatusParticle.FlagBitStatus FlagStatus = InstanceStatusParticle.Status;

					if(0 == (FlagStatus & StatusParticle.FlagBitStatus.BORN))
					{
						return(true);
					}

					float FrameTarget = Frame;
					FrameStart = InstanceStatusParticle.FrameStart;
					FrameDuration = InstanceStatusParticle.FrameEnd;
					ID = Index + InstanceStatusParticle.Cycle;
					IDParent = (0 <= IndexEmitterParent) ? InstanceParameterParticleParent.ID : 0;

					if(0 != (FlagStatus & StatusParticle.FlagBitStatus.EXIST))
					{
						if(0 <= IndexEmitterParent)
						{	/* Has Parent */
							InstanceParameterParticleParent.PositionX = InstanceParameterParticleParent.PositionY = 0.0f;

//							int FrameParent = FrameStart + InstanceParameterParticleParent.FrameStart;
//							FrameParent = (FrameParent > FrameDuration) ? FrameDuration : FrameParent;

							InstanceParameterParticleParent.Calculate(	(FrameStart + InstanceParameterParticleParent.FrameStart),
																		InstanceRoot,
																		InstancePoolParts,
																		ref InstancePoolParts.PoolEmitter[IndexEmitterParent],
																		false
																	);
							InstanceEmitter.Position.x = InstanceParameterParticleParent.PositionX;
							InstanceEmitter.Position.y = InstanceParameterParticleParent.PositionY;
						}

						if(true == Calculate(	FrameTarget,
												InstanceRoot,
												InstancePoolParts,
												ref InstanceEmitter,
												false
											)
							)
						{
							InstancePoolParts.DrawSet(ref this, InstanceRoot, ref InstanceEmitter);
						}
					}

					return(true);
				}

				internal bool Calculate(	float Frame,
											Script_SpriteStudio_RootEffect InstanceRoot,
											PoolPartsEffect InstancePoolParts,
											ref PartsEffectEmitter InstanceEmitter,
											bool FlagSimplicity = false
										)
				{
					Library_SpriteStudio.Utility.Random.Generator InstanceRandom = InstanceEmitter.InstanceRandom;
					Library_SpriteStudio.Data.EmitterEffect InstanceDataEmitter = InstanceEmitter.InstanceDataEmitter;
					float FrameRelative = (Frame - (float)FrameStart);
//					float FrameRelativePrevious = FrameRelative - 1.0f;
					float FramePower2 = FrameRelative * FrameRelative;
					float Life = (float)(FrameDuration - FrameStart);

					if(0.0f >= Life)	/* (0 == Life) */
					{
						return(false);
					}

					float RateLife = FrameRelative / Life;
					long SeedParticle = InstanceEmitter.TableSeedParticle[ID % InstanceEmitter.TableSeedParticle.Length];
					InstanceRandom.InitSeed((uint)(	(ulong)SeedParticle
													+ (ulong)InstanceEmitter.SeedRandom
													+ (ulong)IDParent
													+ (ulong)InstanceEmitter.SeedOffset
												)
											);

					/* Calc Parameters */
					Library_SpriteStudio.Data.EmitterEffect.FlagBit FlagData = InstanceDataEmitter.FlagData;

					float RadianSub = InstanceDataEmitter.Angle.Sub * Mathf.Deg2Rad;
					float Radian = InstanceRandom.RandomFloat(RadianSub);
					Radian = Radian - (RadianSub * 0.5f);
					Radian += ((InstanceDataEmitter.Angle.Main + 90.0f) * Mathf.Deg2Rad);

					float Speed = InstanceDataEmitter.Speed.Main + InstanceRandom.RandomFloat(InstanceDataEmitter.Speed.Sub);

					float RadianOffset = 0;
					if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.TANGENTIALACCELATION))
					{
						float accel = InstanceDataEmitter.RateTangentialAcceleration.Main + InstanceRandom.RandomFloat(InstanceDataEmitter.RateTangentialAcceleration.Sub);
						float SpeedTemp = Speed;
						SpeedTemp = (0.0f >= SpeedTemp) ? 0.1f : SpeedTemp;
						RadianOffset = (accel / (3.14f * (Life * SpeedTemp * 0.2f))) * FrameRelative;
					}

					float AngleTemp = Radian + RadianOffset;
					float Cos = Mathf.Cos(AngleTemp);
					float Sin = Mathf.Sin(AngleTemp);
					float SpeedX = Cos * Speed;
					float SpeedY = Sin * Speed;
					float X = SpeedX * FrameRelative;
					float Y = SpeedY * FrameRelative;
					if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.SPEED_FLUCTUATION))
					{
						float SpeedFluctuation = InstanceDataEmitter.SpeedFluctuation.Main + InstanceRandom.RandomFloat(InstanceDataEmitter.SpeedFluctuation.Sub);
						float SpeedOffset = SpeedFluctuation / Life;

						X = (((Cos * SpeedOffset) * FrameRelative) + SpeedX) * ((FrameRelative + 1.0f) * 0.5f);
						Y = (((Sin * SpeedOffset) * FrameRelative) + SpeedY) * ((FrameRelative + 1.0f) * 0.5f);
					}

					if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.GRAVITY_DIRECTION))
					{
						X += (0.5f * InstanceDataEmitter.GravityDirectional.x * FramePower2);
						Y += (0.5f * InstanceDataEmitter.GravityDirectional.y * FramePower2);
					}
					float OffsetX = 0.0f;
					float OffsetY = 0.0f;
					if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.POSITION))
					{
						OffsetX = InstanceDataEmitter.Position.Main.x + InstanceRandom.RandomFloat(InstanceDataEmitter.Position.Sub.x);
						OffsetY = InstanceDataEmitter.Position.Main.y + InstanceRandom.RandomFloat(InstanceDataEmitter.Position.Sub.y);
					}

					RotateZ = 0.0f;
					if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.ROTATION))
					{
						RotateZ = InstanceDataEmitter.Rotation.Main + InstanceRandom.RandomFloat(InstanceDataEmitter.Rotation.Sub);

						float RotationFluctuation = InstanceDataEmitter.RotationFluctuation.Main + InstanceRandom.RandomFloat(InstanceDataEmitter.RotationFluctuation.Sub);
						if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.ROTATION_FLUCTUATION))
						{
							float FrameLast = Life * InstanceDataEmitter.RotationFluctuationRateTime;

							float RateRotationFluctuation = 0.0f;
							if(0.0f >= FrameLast)	/* Minus??? */
							{
								RotateZ += (RotationFluctuation * InstanceDataEmitter.RotationFluctuationRate) * FrameRelative;
							}
							else
							{
								RateRotationFluctuation = ((RotationFluctuation * InstanceDataEmitter.RotationFluctuationRate) - RotationFluctuation) / FrameLast;

								float FrameModuration = FrameRelative - FrameLast;
								FrameModuration = (0.0f > FrameModuration) ? 0.0f : FrameModuration;
							
								float FrameRelativeNow = FrameRelative;
								FrameRelativeNow = (FrameRelativeNow > FrameLast) ? FrameLast : FrameRelativeNow;

								float RotateOffsetTemp = RateRotationFluctuation * FrameRelativeNow;
								RotateOffsetTemp += RotationFluctuation;
								float RotateOffset = (RotateOffsetTemp + RotationFluctuation) * (FrameRelativeNow + 1.0f) * 0.5f;
								RotateOffset -= RotationFluctuation;
								RotateOffset += (FrameModuration * RotateOffsetTemp);
								RotateZ += RotateOffset;
							}
						}
						else
						{
							RotateZ += (RotationFluctuation * FrameRelative);
						}
					}

					/* ColorVertex/AlphaFade */
					{
						ColorVertex.r =
						ColorVertex.g =
						ColorVertex.b =
						ColorVertex.a = 1.0f;

						if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.COLORVERTEX))
						{
							ColorVertex.a = InstanceDataEmitter.ColorVertex.Main.a + InstanceRandom.RandomFloat(InstanceDataEmitter.ColorVertex.Sub.a);
							ColorVertex.r = InstanceDataEmitter.ColorVertex.Main.r + InstanceRandom.RandomFloat(InstanceDataEmitter.ColorVertex.Sub.r);
							ColorVertex.g = InstanceDataEmitter.ColorVertex.Main.g + InstanceRandom.RandomFloat(InstanceDataEmitter.ColorVertex.Sub.g);
							ColorVertex.b = InstanceDataEmitter.ColorVertex.Main.b + InstanceRandom.RandomFloat(InstanceDataEmitter.ColorVertex.Sub.b);
						}
						if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.COLORVERTEX_FLUCTUATION))
						{
							Color ColorFluctuation;
							ColorFluctuation.a = InstanceDataEmitter.ColorVertexFluctuation.Main.a + InstanceRandom.RandomFloat(InstanceDataEmitter.ColorVertexFluctuation.Sub.a);
							ColorFluctuation.r = InstanceDataEmitter.ColorVertexFluctuation.Main.r + InstanceRandom.RandomFloat(InstanceDataEmitter.ColorVertexFluctuation.Sub.r);
							ColorFluctuation.g = InstanceDataEmitter.ColorVertexFluctuation.Main.g + InstanceRandom.RandomFloat(InstanceDataEmitter.ColorVertexFluctuation.Sub.g);
							ColorFluctuation.b = InstanceDataEmitter.ColorVertexFluctuation.Main.b + InstanceRandom.RandomFloat(InstanceDataEmitter.ColorVertexFluctuation.Sub.b);

							ColorVertex = Color.Lerp(ColorVertex, ColorFluctuation, RateLife);
						}

						if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.FADEALPHA))
						{
							float RateStart = InstanceDataEmitter.AlphaFadeStart;
							float RateEnd = InstanceDataEmitter.AlphaFadeEnd;
							if(RateLife < RateStart)
							{
								ColorVertex.a *= (1.0f - ((RateStart - RateLife) / RateStart));
							}
							else
							{
								if(RateLife > RateEnd)
								{
									if(1.0f <= RateEnd)
									{
										ColorVertex.a = 0.0f;
									}
									else
									{
										float Alpha = (RateLife - RateEnd) / (1.0f - RateEnd);
										Alpha = (1.0f <= Alpha) ? 1.0f : Alpha;
										ColorVertex.a *= (1.0f - Alpha);
									}
								}
							}
						}
					}

					Scale.x = 
					Scale.y = 1.0f;
//					Scale.z = 1.0f;
					float ScaleRate = 1.0f;

					if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.SCALE_START))
					{
						Scale.x = InstanceDataEmitter.ScaleStart.Main.x + InstanceRandom.RandomFloat(InstanceDataEmitter.ScaleStart.Sub.x);
						Scale.y = InstanceDataEmitter.ScaleStart.Main.y + InstanceRandom.RandomFloat(InstanceDataEmitter.ScaleStart.Sub.y);
						ScaleRate = InstanceDataEmitter.ScaleRateStart.Main + InstanceRandom.RandomFloat(InstanceDataEmitter.ScaleRateStart.Sub);
					}
					if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.SCALE_END))
					{
						Vector3 ScaleEnd;
						float ScaleRateEnd;
						ScaleEnd.x = InstanceDataEmitter.ScaleEnd.Main.x + InstanceRandom.RandomFloat(InstanceDataEmitter.ScaleEnd.Sub.x);
						ScaleEnd.y = InstanceDataEmitter.ScaleEnd.Main.y + InstanceRandom.RandomFloat(InstanceDataEmitter.ScaleEnd.Sub.y);
						ScaleEnd.z = 1.0f;
						ScaleRateEnd = InstanceDataEmitter.ScaleRateEnd.Main + InstanceRandom.RandomFloat(InstanceDataEmitter.ScaleRateEnd.Sub);

						Scale = Vector2.Lerp(Scale, ScaleEnd, RateLife);
						ScaleRate = Mathf.Lerp(ScaleRate, ScaleRateEnd, RateLife);
					}
					Scale *= ScaleRate;
					Scale.z = 1.0f;	/* Overwrite, force */

					float PosisionBaseX = InstanceEmitter.Position.x + OffsetX;
					float PosisionBaseY = InstanceEmitter.Position.y + OffsetY;
					PositionX = X + PosisionBaseX;
					PositionY = Y + PosisionBaseY;

					if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.GRAVITY_POINT))
					{
						Vector2 VectorPosition;
						float PositionXGravity = InstanceDataEmitter.GravityPointPosition.x;
						float PositionYGravity = InstanceDataEmitter.GravityPointPosition.y;
						VectorPosition.x = PositionXGravity - (OffsetX + PositionX);
						VectorPosition.y = PositionYGravity - (OffsetY + PositionY);
						Vector2 VectorNormal = VectorPosition.normalized;
						float GravityPower = InstanceDataEmitter.GravityPointPower;
						if(0.0f < GravityPower)
						{
//							Vector2 VectorPosition2;
//							VectorPosition2.x = PositionX;
//							VectorPosition2.y = PositionY;

							float eFrame = (VectorPosition.magnitude / GravityPower) * 0.9f;
							float gFrame = (Frame >= (int)eFrame) ? (eFrame * 0.9f) : Frame;

							VectorNormal = VectorNormal * GravityPower * gFrame;
							PositionX += VectorNormal.x;
							PositionY += VectorNormal.y;

							float Blend = OutQuad(gFrame, eFrame, 0.9f, 0.0f);
							Blend += (Frame / Life * 0.1f);

							PositionX = PositionX + (PositionXGravity - PositionX) * Blend;	/* CAUTION!: Don't use "Mathf.Lerp" */
							PositionY = PositionY + (PositionYGravity - PositionY) * Blend;	/* CAUTION!: Don't use "Mathf.Lerp" */
						}
						else
						{
							/* MEMO: In the case negative power, Simply repulsion. Attenuation due to distance is not taken into account. */
							VectorNormal = VectorNormal * GravityPower * Frame;
							PositionX += VectorNormal.x;
							PositionY += VectorNormal.y;
						}
					}

					Direction = 0.0f;
					if((0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.TURNDIRECTION)) && (false == FlagSimplicity))
					{
						InstanceEmitter.InstanceParameterParticleTempolary2 = this;
						InstanceEmitter.InstanceParameterParticleTempolary2.Calculate(	(Frame + 1.0f),	/* (Frame + 0.1f), */
																						InstanceRoot,
																						InstancePoolParts,
																						ref InstanceEmitter,
																						true
																					);
						float RadianDirection = AngleGetCCW(	new Vector2(1.0f, 0.0f),
																new Vector2((PositionX - InstanceEmitter.InstanceParameterParticleTempolary2.PositionX), (PositionY - InstanceEmitter.InstanceParameterParticleTempolary2.PositionY))
															);
						Direction = (RadianDirection * Mathf.Rad2Deg) + 90.0f + InstanceDataEmitter.TurnDirectionFluctuation;
					}

					return (true);
				}
				private static float OutQuad(float Time, float TimeFull, float ValueMax, float ValueMin)
				{
					if(0.0f >= TimeFull)
					{
						return(0.0f);
					}
					if(Time > TimeFull) Time = TimeFull;

					ValueMax -= ValueMin;
					Time /= TimeFull;
					return(-ValueMax * Time * (Time - 2.0f) + ValueMin);
				}
				private static float AngleGetCCW(Vector2 Start, Vector2 End)
				{
					Vector2 StartNormalized = Start.normalized;
					Vector2 EndNormalized = End.normalized;

					float Dot = Vector2.Dot(StartNormalized, EndNormalized);
					Dot = Mathf.Clamp(Dot, -1.0f, 1.0f);
					float Angle = Mathf.Acos(Dot);
					float Cross = (StartNormalized.x * EndNormalized.y) - (EndNormalized.x * StartNormalized.y);
					Angle = (0.0f > Cross) ? ((2.0f * Mathf.PI) - Angle) : Angle;
					return(Angle);
				}
			}

			[System.Flags]
			internal enum FlagBitStatus
			{
				VALID = 0x40000000,
				RUNNING = 0x20000000,

				OVERWRITE_CELL_UNREFLECTED = 0x00080000,
				OVERWRITE_CELL_IGNOREATTRIBUTE = 0x00040000,

				CLEAR = 0x00000000
			}
			internal FlagBitStatus Status;

			internal Library_SpriteStudio.Data.PartsEffect InstanceDataParts;
			internal Library_SpriteStudio.Data.EmitterEffect InstanceDataEmitter;

			internal int IndexParent;

			internal int IndexCellMapParticle;
			internal int IndexCellParticle;
			internal int IndexCellMapOverwrite;
			internal int IndexCellOverwrite;
			internal Vector3[] CoordinateMeshParticle;
			internal Vector2[] UVMeshParticle;

			internal Library_SpriteStudio.Utility.Random.Generator InstanceRandom;
			internal Library_SpriteStudio.Data.EmitterEffect.PatternEmit[] TablePatternEmit;
			internal int[] TablePatternOffset;
			internal long[] TableSeedParticle;

			internal uint SeedRandom;
			internal uint SeedOffset;

			internal int Duration;
			internal Vector2 Position;
			internal int FrameGlobal;

			internal StatusParticle[] ListStatusParticle;

			internal ParameterParticle InstanceParameterParticleTempolary2;	/* (mainly) for TurnToDirection (mainly) */
			internal ParameterParticle InstanceParameterParticleTempolary;	/* (mainly) for Parent */
			internal ParameterParticle InstanceParameterParticle;

			internal int FrameFull
			{
				get
				{
					return(InstanceDataEmitter.DurationEmitter + (int)(InstanceDataEmitter.DurationParticle.Main + InstanceDataEmitter.DurationParticle.Sub));
				}
			}

			internal void CleanUp()
			{
				Status = FlagBitStatus.CLEAR;

				InstanceDataParts = null;
				InstanceDataEmitter = null;

				IndexParent = -1;

				IndexCellMapParticle = -1;
				IndexCellParticle = -1;
				if(0 == (Status & FlagBitStatus.OVERWRITE_CELL_IGNOREATTRIBUTE))
				{
					IndexCellMapOverwrite = -1;
					IndexCellOverwrite = -1;
				}
				CoordinateMeshParticle = null;
				UVMeshParticle = null;

				InstanceRandom = null;
				TablePatternEmit = null;
				TableSeedParticle = null;

				SeedRandom = (uint)Library_SpriteStudio.Data.EmitterEffect.Constant.SEED_MAGIC;
				SeedOffset = 0;

				Duration = 0;
				Position = Vector2.zero;
				FrameGlobal = 0;

				ListStatusParticle = null;
			}

			internal bool BootUp(	Script_SpriteStudio_RootEffect InstanceRoot,
									Library_SpriteStudio.Data.PartsEffect InstanceParts,
									Library_SpriteStudio.Data.EmitterEffect InstanceEmitter,
									int IndexEmitterParent,
									PoolPartsEffect InstancePoolParts
								)
			{
				Status = FlagBitStatus.CLEAR;

				InstanceDataParts = InstanceParts;
				InstanceDataEmitter = InstanceEmitter;
				IndexParent = IndexEmitterParent;

				/* Random Initialize */
				if(null == InstanceRandom)
				{
					InstanceRandom = Script_SpriteStudio_RootEffect.InstanceCreateRandom();
				}

				Library_SpriteStudio.Data.EmitterEffect.FlagBit FlagData = InstanceDataEmitter.FlagData;

				SeedRandom = InstancePoolParts.Seed;
				if(0 != (FlagData & Data.EmitterEffect.FlagBit.SEEDRANDOM))
				{	/* Seed Overwrite */
					/* MEMO: Overwritten to the Emitter's Seed. */
					SeedRandom = (uint)InstanceDataEmitter.SeedRandom + (uint)Library_SpriteStudio.Data.EmitterEffect.Constant.SEED_MAGIC;
				}
				else
				{
					if(0 != (InstanceRoot.DataEffect.FlagData & Script_SpriteStudio_DataEffect.FlagBit.SEEDRANDOM_LOCK))
					{	/* Seed Locked */
						/* MEMO: Overwritten to the Effect's Seed. */
						SeedRandom = ((uint)InstanceRoot.DataEffect.SeedRandom + 1) * (uint)Library_SpriteStudio.Data.EmitterEffect.Constant.SEED_MAGIC;
					}
				}

				/* Emitter DataTable Get */
				TablePatternOffset = InstanceDataEmitter.TablePatternOffset;

				TablePatternEmit = InstanceDataEmitter.TablePatternEmit;
				TableSeedParticle = InstanceDataEmitter.TableSeedParticle;
				if(((null == TablePatternEmit) || (0 >= TablePatternEmit.Length)) || ((null == TableSeedParticle) || (0 >= TableSeedParticle.Length)))
				{	/* Calculate on Runtime ... Not Fixed Random-Seed */
					Library_SpriteStudio.Data.EmitterEffect.TableGet(	ref TablePatternEmit,
																		ref TableSeedParticle,
																		InstanceDataEmitter,
																		InstanceRandom,
																		SeedRandom
																	);
				}

				/* Particle UV Pre-Calculate */
				ParticleCellSet(InstanceRoot);

				/* Particle Work-Area Create */
				ListStatusParticle = new StatusParticle[InstanceEmitter.CountParticleMax];	/* Check. */
				for(int i=0; i<InstanceEmitter.CountParticleMax; i++)
				{
					ListStatusParticle[i].CleanUp();
				}

				/* Parameter Set */
				Duration = InstanceDataEmitter.DurationEmitter + InstanceDataEmitter.Delay;

				/* Status Set */
				Status |= (FlagBitStatus.VALID | FlagBitStatus.RUNNING);

				return(true);
			}

			internal bool Update(	float Frame,
									Script_SpriteStudio_RootEffect InstanceRoot,
									PoolPartsEffect InstancePoolParts,
									int IndexEmitterParent
								)
			{
				if((null == InstanceDataEmitter) || (null == InstanceDataParts))
				{
					return(false);
				}

				/* Particle-Status Update */
				int CountParticle = TablePatternEmit.Length;
				int CountOffset = TablePatternOffset.Length;
				int FrameNo = (int)Frame;
				uint Slide = (0 <= IndexEmitterParent) ? (uint)InstanceParameterParticleTempolary.ID : 0;
				Slide = Slide * (uint)Library_SpriteStudio.Data.EmitterEffect.Constant.SEED_MAGIC;
				uint IndexSlide;
				for(int i=0; i<CountOffset; i++)
				{
					IndexSlide = ((uint)i + Slide) % (uint)CountParticle;
					ListStatusParticle[i].Update(FrameNo, ref this, TablePatternEmit[i], TablePatternOffset[i], TablePatternEmit[IndexSlide]);
				}

				/* Update Particles */
				/* MEMO: particleDraw */
				CountParticle = InstanceDataEmitter.CountParticleMax;
				for(int i=0; i<CountParticle; i++)
				{
					InstanceParameterParticle.Update(	FrameNo,
														i,
														InstanceRoot,
														InstancePoolParts,
														ref this,
														ref ListStatusParticle[i],
														IndexEmitterParent,
														ref InstanceParameterParticleTempolary
													); 
				}
				return(true);
			}
			internal bool UpdateSubEmitters(	ref PartsEffectEmitter InstanceEmitterTarget,
												float Frame,
												Script_SpriteStudio_RootEffect InstanceRoot,
												PoolPartsEffect InstancePoolParts,
												int IndexEmitterMine
											)
			{
				/* Particle-Status Update */
				int CountParticle = TablePatternEmit.Length;
				int CountOffset = TablePatternOffset.Length;
				int FrameNo = (int)Frame;
				for(int i=0; i<CountOffset; i++)
				{
					/* MEMO: Slide is always 0. */
					ListStatusParticle[i].Update(FrameNo, ref this, TablePatternEmit[i], TablePatternOffset[i], TablePatternEmit[i]);
				}

				/* Update Sub-Emitters */
				int FrameTop;
				CountParticle = InstanceDataEmitter.CountParticleMax;
				for(int i=0; i<CountParticle; i++)
				{
					if(0 != (ListStatusParticle[i].Status & StatusParticle.FlagBitStatus.BORN))
					{
						/* MEMO: "InstanceParameterParticleTempolary" is parent's parameter. */
						FrameTop = ListStatusParticle[i].FrameStart;
						InstanceEmitterTarget.InstanceParameterParticleTempolary.FrameStart = FrameTop;
						InstanceEmitterTarget.InstanceParameterParticleTempolary.FrameDuration = ListStatusParticle[i].FrameEnd;
						InstanceEmitterTarget.InstanceParameterParticleTempolary.ID = i;
						InstanceEmitterTarget.InstanceParameterParticleTempolary.IDParent = 0;

						/* CAUTION: "InstanceParameterParticleTempolary" will be broken. */
						InstanceEmitterTarget.Update(	(Frame - (float)FrameTop),
														InstanceRoot,
														InstancePoolParts,
														IndexEmitterMine
													);
					}
				}

				return(true);
			}

			internal bool ParticleCellSet(Script_SpriteStudio_RootEffect InstanceRoot)
			{
				int IndexCellMap = IndexCellMapOverwrite;
				int IndexCell = IndexCellOverwrite;
				if((0 > IndexCellMap) || (0 > IndexCell))
				{
					IndexCellMapOverwrite = -1;
					IndexCellOverwrite = -1;

					IndexCellMap = InstanceDataEmitter.IndexCellMap;
					IndexCell = InstanceDataEmitter.IndexCell;
				}

				Library_SpriteStudio.Data.CellMap DataCellMap = InstanceRoot.DataCellMap.DataGetCellMap(IndexCellMap);
				if(null == DataCellMap)
				{
					IndexCellMapParticle = -1;
					IndexCellParticle = -1;
				}
				else
				{
					Library_SpriteStudio.Data.Cell DataCell = DataCellMap.DataGetCell(IndexCell);
					if(null == DataCell)
					{
						IndexCellMapParticle = -1;
						IndexCellParticle = -1;
					}
					else
					{
						IndexCellMapParticle = IndexCellMap;
						IndexCellParticle = IndexCell;

						if(null == CoordinateMeshParticle)
						{
							CoordinateMeshParticle = new Vector3[(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2];
						}
						if(null == UVMeshParticle)
						{
							UVMeshParticle = new Vector2[(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2];
						}

						float PivotXCell = DataCell.Pivot.x;
						float PivotYCell = DataCell.Pivot.y;
						float CoordinateLUx = -PivotXCell;
						float CoordinateLUy = PivotYCell;
						float CoordinateRDx = DataCell.Rectangle.width - PivotXCell;
						float CoordinateRDy = -(DataCell.Rectangle.height - PivotYCell);
						CoordinateMeshParticle[(int)Library_SpriteStudio.KindVertexNo.LD].x = 
						CoordinateMeshParticle[(int)Library_SpriteStudio.KindVertexNo.LU].x = CoordinateLUx;
						CoordinateMeshParticle[(int)Library_SpriteStudio.KindVertexNo.RU].y = 
						CoordinateMeshParticle[(int)Library_SpriteStudio.KindVertexNo.LU].y = CoordinateLUy;
						CoordinateMeshParticle[(int)Library_SpriteStudio.KindVertexNo.RD].x = 
						CoordinateMeshParticle[(int)Library_SpriteStudio.KindVertexNo.RU].x = CoordinateRDx;
						CoordinateMeshParticle[(int)Library_SpriteStudio.KindVertexNo.LD].y = 
						CoordinateMeshParticle[(int)Library_SpriteStudio.KindVertexNo.RD].y = CoordinateRDy;
						CoordinateMeshParticle[(int)Library_SpriteStudio.KindVertexNo.LU].z = 
						CoordinateMeshParticle[(int)Library_SpriteStudio.KindVertexNo.RU].z = 
						CoordinateMeshParticle[(int)Library_SpriteStudio.KindVertexNo.RD].z = 
						CoordinateMeshParticle[(int)Library_SpriteStudio.KindVertexNo.LD].z = 0.0f;

						float SizeXTexture = DataCellMap.SizeOriginal.x;
						float SizeYTexture = DataCellMap.SizeOriginal.y;
						float CoordinateL = DataCell.Rectangle.xMin / SizeXTexture;
						float CoordinateR = DataCell.Rectangle.xMax / SizeXTexture;
						float CoordinateU = (SizeYTexture - DataCell.Rectangle.yMin) / SizeYTexture;
						float CoordinateD = (SizeYTexture - DataCell.Rectangle.yMax) / SizeYTexture;
						UVMeshParticle[(int)Library_SpriteStudio.KindVertexNo.LU] = new Vector2(CoordinateL, CoordinateU);
						UVMeshParticle[(int)Library_SpriteStudio.KindVertexNo.RU] = new Vector2(CoordinateR, CoordinateU);
						UVMeshParticle[(int)Library_SpriteStudio.KindVertexNo.RD] = new Vector2(CoordinateR, CoordinateD);
						UVMeshParticle[(int)Library_SpriteStudio.KindVertexNo.LD] = new Vector2(CoordinateL, CoordinateD);
					}
				}

				Status &= ~FlagBitStatus.OVERWRITE_CELL_UNREFLECTED;
				return(true);
			}
		}

		public class ColorBlendOverwrite
		{
			public Library_SpriteStudio.KindColorBound Bound;
			public Library_SpriteStudio.KindColorOperation Operation;
			public Color[] VertexColor;

			public ColorBlendOverwrite()
			{
				Bound = Library_SpriteStudio.KindColorBound.NON;
				Operation = Library_SpriteStudio.KindColorOperation.MIX;
				VertexColor = new Color[(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2];
				for(int i=0; i<VertexColor.Length; i++)
				{
					VertexColor[i] = Color.white;
				}
			}

			/* ******************************************************** */
			//! Set single Overlay-Color
			/*!
			@param	KindOperation
				kind of Color-Blending Operation
				Library_SpriteStudio.KindColorOperation.NON: Not Overwrite
			@param	DataColor
				Blending Color
			@retval	Return-Value
				(none)

			Set single Overlay-Color.<br>
			This setting affects individually to all of Sprite-Parts (managed in the instance of "Script_SpriteStudio_PartsRoot").<br>
			<br>
			If you specify a "Library_SpriteStudio.KindColorOperation.NON" to "KindOperation", the result will follow the setting of the original animation data.<br>
			<br>
			Members of the "DataColor", correspond to the Color's-Parameter of "Color-Blend" of "OPTPiX SpriteStudio (SpriteStudio5)".<br>
			DataColor.r (0.0f to 1.0f) : "R" (0 to 255)<br>
			DataColor.g (0.0f to 1.0f) : "G" (0 to 255)<br>
			DataColor.b (0.0f to 1.0f) : "B" (0 to 255)<br>
			DataColor.a (0.0f to 1.0f) : "%" (0 to 255)<br>
			*/
			public void SetOverall(Library_SpriteStudio.KindColorOperation KindOperation, ref Color DataColor)
			{
				if(Library_SpriteStudio.KindColorOperation.NON == KindOperation)
				{	/* Error */
					Bound = Library_SpriteStudio.KindColorBound.NON;
					Operation = KindOperation;
					return;
				}

				Bound = Library_SpriteStudio.KindColorBound.OVERALL;
				Operation = KindOperation;
//				for(int i=0; i<(int)Library_SpriteStudio.KindVertexNo.TERMINATOR4; i++)
				for(int i=0; i<(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2; i++)
				{
					VertexColor[i] = DataColor;
				}
			}

			/* ******************************************************** */
			//! Set separately the Overlay-Color of the 4-vertices
			/*!
			@param	KindOperation
				kind of Color-Blending Operation
				Library_SpriteStudio.KindColorOperation.NON: Not Overwrite
			@param	DataColor
				Blending Color
			@retval	Return-Value
				(none)

			Set separately the value of the 4-vertices.<br>
			This setting affects individually to all of Sprite-Parts (managed in the instance of "Script_SpriteStudio_PartsRoot").<br>
			<br>
			If you specify a "Library_SpriteStudio.KindColorOperation.NON" to "KindOperation", the result will follow the setting of the original animation data.<br>
			<br>
			Part of the "XX" in the "DataColorXX", mean each vertex.<br>
			(LU: Upper-Left / RU: Upper-Right / LD: Lower-Left / RD: Lower-Right)<br>
			<br>
			Members of the "DataColorXX", correspond to the Color's-Parameter of "Color-Blend" of "OPTPiX SpriteStudio (SpriteStudio5)".<br>
			DataColorXX.r (0.0f to 1.0f) : "R" (0 to 255)<br>
			DataColorXX.g (0.0f to 1.0f) : "G" (0 to 255)<br>
			DataColorXX.b (0.0f to 1.0f) : "B" (0 to 255)<br>
			DataColorXX.a (0.0f to 1.0f) : "%" (0 to 255)<br>
			*/
			public void SetVertex(	Library_SpriteStudio.KindColorOperation KindOperation,
									ref Color DataColorLU,
									ref Color DataColorRU,
									ref Color DataColorRD,
									ref Color DataColorLD
								)
			{
				if(Library_SpriteStudio.KindColorOperation.NON == KindOperation)
				{	/* Error */
					Bound = Library_SpriteStudio.KindColorBound.NON;
					Operation = KindOperation;
					return;
				}

				Bound = Library_SpriteStudio.KindColorBound.OVERALL;
				Operation = KindOperation;
				VertexColor[(int)Library_SpriteStudio.KindVertexNo.LU] = DataColorLU;
				VertexColor[(int)Library_SpriteStudio.KindVertexNo.RU] = DataColorRU;
				VertexColor[(int)Library_SpriteStudio.KindVertexNo.RD] = DataColorRD;
				VertexColor[(int)Library_SpriteStudio.KindVertexNo.LD] = DataColorLD;
			}
		}

		public struct CellChange
		{
			public int IndexTexture;
			public Library_SpriteStudio.Data.CellMap DataCellMap;
			public Library_SpriteStudio.Data.Cell DataCell;

			public void CleanUp()
			{
				IndexTexture = -1;
				DataCellMap = null;
				DataCell = null;
			}

			public void DataGet(	ref int IndexTextureOut,
									ref Library_SpriteStudio.Data.CellMap CellMapOut,
									ref Library_SpriteStudio.Data.Cell CellOut
								)
			{
				IndexTextureOut = IndexTexture;
				CellMapOut = DataCellMap;
				CellOut = DataCell;
			}

			public void DataSet(	int IndexTextureIn,
									Library_SpriteStudio.Data.CellMap CellMapIn,
									Library_SpriteStudio.Data.Cell CellIn
								)
			{
				IndexTexture = IndexTextureIn;
				DataCellMap = CellMapIn;
				DataCell = CellIn;
			}
		}
	}

	public class ManagerDraw
	{
		public enum Constant
		{
			MAX_MESHPARAMETERBUFFER = 2,
		}

		/* MEMO: These Defines for only Simplified-SpriteDrawManager */
		public enum KindDrawQueue
		{
			NON = -1,
			SHADER_SETTING = 0,
			USER_SETTING,
			BACKGROUND,
			GEOMETRY,
			ALPHATEST,
			TRANSPARENT,
			OVERLAY,
		}
		public static readonly int[] ValueKindDrawQueue =
		{			// Unity 3.5.x/4.x.x upper
			-1,		// SHADER_SETTING
			0,		// USER_SETTING
			1000,	// BACKGROUND
			2000,	// GEOMETRY
			2450,	// ALPHATEST
			3000,	// TRANSPARENT
			4000,	// OVERLAY
			5000,	// (TERMINATOR)
		};

		internal struct DataDrawParts
		{
			internal Library_SpriteStudio.KindParts Kind;
			internal Transform InstanceTransform;
			internal Library_SpriteStudio.Script.Root InstanceRoot;
//			internal FragmentClusterDrawParts InstanceFragmentClusterDrawPartsPartsPair;
			internal Mesh InstanceMesh;	/* null == Instance/Effect Parts */
			internal Vector3[] CurrentMeshVertices;
			internal Vector2[] CurrentMeshUv;
			internal Vector2[] CurrentMeshUv2;
			internal Color32[] CurrentMeshColor;
		}
		internal class FragmentDrawParts : Miscellaneousness.Chain<DataDrawParts>.Fragment
		{
			internal void CleanUp()
			{
				ChainCleanUp();
				Data.Kind = KindParts.NON;
				Data.InstanceTransform = null;
				Data.InstanceRoot = null;
//				Data.InstanceFragmentClusterDrawPartsPartsPair = null;
				Data.InstanceMesh = null;
				Data.CurrentMeshVertices = null;
				Data.CurrentMeshUv = null;
				Data.CurrentMeshUv2 = null;
				Data.CurrentMeshColor = null;
			}

			internal bool BootUp(	Library_SpriteStudio.Script.Root InstanceRootInitial,
									Library_SpriteStudio.KindParts KindParts,
									GameObject InstanceGameObjectInitial
								)
			{
				Data.Kind = KindParts;
				Data.InstanceTransform = InstanceGameObjectInitial.transform;
				Data.InstanceRoot = InstanceRootInitial;
//				Data.InstanceFragmentClusterDrawPartsPartsPair = InstanceFragmentClusterDrawPartsPartsPairInitial;
				Data.InstanceMesh = null;
				Data.CurrentMeshVertices = null;
				Data.CurrentMeshUv = null;
				Data.CurrentMeshUv2 = null;
				Data.CurrentMeshColor = null;

				switch(KindParts)
				{
					case KindParts.NORMAL_TRIANGLE2:
						Data.InstanceMesh = new Mesh();
						if(null == Data.InstanceMesh)
						{
							goto BootUp_ErrorEnd;
						}

						Data.InstanceMesh.Clear();
						Data.InstanceMesh.vertices = Library_SpriteStudio.ArrayCoordinate_Triangle2;
//						Data.InstanceMesh.uv = Library_SpriteStudio.ArrayUVMappingUV0_Triangle2;
//						Data.InstanceMesh.uv2 = Library_SpriteStudio.ArrayUVMappingUV0_Triangle2;
						Data.InstanceMesh.colors32 = Library_SpriteStudio.ArrayColor32_Triangle2;
						Data.InstanceMesh.normals = Library_SpriteStudio.ArrayNormal_Triangle2;
						Data.InstanceMesh.triangles = Library_SpriteStudio.ArrayVertexIndex_Triangle2;

						break;

					case KindParts.NORMAL_TRIANGLE4:
						Data.InstanceMesh = new Mesh();
						if(null == Data.InstanceMesh)
						{
							goto BootUp_ErrorEnd;
						}

						Data.InstanceMesh.Clear();
						Data.InstanceMesh.vertices = Library_SpriteStudio.ArrayCoordinate_Triangle4;
//						Data.InstanceMesh.uv = Library_SpriteStudio.ArrayUVMappingUV0_Triangle4;
//						Data.InstanceMesh.uv2 = Library_SpriteStudio.ArrayUVMappingUV0_Triangle4;
						Data.InstanceMesh.colors32 = Library_SpriteStudio.ArrayColor32_Triangle4;
						Data.InstanceMesh.normals = Library_SpriteStudio.ArrayNormal_Triangle4;
						Data.InstanceMesh.triangles = Library_SpriteStudio.ArrayVertexIndex_Triangle4;

						break;

					default:
						Data.InstanceMesh = null;
						break;
				}
				return(true);

			BootUp_ErrorEnd:;
				return(false);
			}
		}
		internal class TerminalDrawParts : Miscellaneousness.Chain<DataDrawParts>.Terminal
		{
			internal void CleanUp()
			{
				ChainCleanUp();
			}
		}

		internal struct DataClusterDrawParts
		{
			internal TerminalDrawParts ChainDrawParts;
			internal Material InstanceMaterial;	/* null == Instance/Effect Parts */
			internal Library_SpriteStudio.Script.Root InstanceRoot;
		}
		internal class FragmentClusterDrawParts : Miscellaneousness.Chain<DataClusterDrawParts>.Fragment
		{
			internal void CleanUp()
			{
				ChainCleanUp();
				Data.ChainDrawParts = null;
				Data.InstanceMaterial = null;
				Data.InstanceRoot = null;
			}

			internal bool BootUp()
			{
				CleanUp();

				Data.ChainDrawParts = new Library_SpriteStudio.ManagerDraw.TerminalDrawParts();
				Data.ChainDrawParts.CleanUp();
				return(true);
			}
		}
		internal class TerminalClusterDrawParts : Miscellaneousness.Chain<DataClusterDrawParts>.Terminal
		{
			internal int IndexPoolMax;
			internal int IndexPoolUse;
			internal FragmentClusterDrawParts[] PoolFragmentClusterDrawParts;

			internal new void ChainCleanUp()
			{
				IndexPoolUse = 0;
				base.ChainCleanUp();
			}

			internal void CleanUp()
			{
				IndexPoolMax = 0;
				IndexPoolUse = 0;
				PoolFragmentClusterDrawParts = null;
				base.ChainCleanUp();
			}

			internal bool BootUp(int CountMax)
			{
				CleanUp();
				if(0 < CountMax)
				{
					IndexPoolMax = CountMax - 1;

					PoolFragmentClusterDrawParts = new FragmentClusterDrawParts[CountMax];
					for(int i=0; i<CountMax; i++)
					{
						PoolFragmentClusterDrawParts[i] = new FragmentClusterDrawParts();
						PoolFragmentClusterDrawParts[i].CleanUp();

						PoolFragmentClusterDrawParts[i].Data.ChainDrawParts = new TerminalDrawParts();
						PoolFragmentClusterDrawParts[i].Data.ChainDrawParts.ChainCleanUp();
					}
				}

				return(true);
			}

			internal FragmentClusterDrawParts InsntaceAdmitClusterDrawParts()
			{
				if(IndexPoolUse > IndexPoolMax)
				{
					return(null);	
				}

				FragmentClusterDrawParts InstanceCluster = PoolFragmentClusterDrawParts[IndexPoolUse];
				InstanceCluster.ChainCleanUp();
				InstanceCluster.Data.ChainDrawParts.ChainCleanUp();
				IndexPoolUse++;
				return(InstanceCluster);
			}

			internal void InsntaceDenyClusterDrawParts()
			{
				if(0 < IndexPoolUse)
				{
					IndexPoolUse--;
				}
			}
		}

		internal class DataParts
		{
			internal FragmentDrawParts DrawParts;

			internal void CleanUp()
			{
				DrawParts = null;
			}

			internal bool BootUp(	Library_SpriteStudio.Script.Root InstanceRoot,
									Library_SpriteStudio.KindParts KindParts,
									GameObject InstanceGameObject
								)
			{
				DrawParts = new Library_SpriteStudio.ManagerDraw.FragmentDrawParts();
				DrawParts.CleanUp();
				DrawParts.BootUp(InstanceRoot, KindParts, InstanceGameObject);
				return(true);
			}

			internal bool PartsSetDraw(Library_SpriteStudio.Script.Root InstanceRoot, Material InstanceMaterial, float Key)
			{
				/* FragmentClusterDrawParts */	Library_SpriteStudio.Miscellaneousness.Chain<DataClusterDrawParts>.Fragment ClusterNow = InstanceRoot.ChainClusterDrawParts.ChainTop;
				/* FragmentClusterDrawParts */	Library_SpriteStudio.Miscellaneousness.Chain<DataClusterDrawParts>.Fragment ClusterPrevious = null;
				/* FragmentClusterDrawParts */	Library_SpriteStudio.Miscellaneousness.Chain<DataClusterDrawParts>.Fragment ClusterAdd = null;
				/* FragmentDrawParts */	Library_SpriteStudio.Miscellaneousness.Chain<DataDrawParts>.Fragment FragmentDrawPartsPrevious = null;

				DrawParts.ChainCleanUp();
				DrawParts.Key = Key;

				float KeyBottom;
				while(null != ClusterNow)
				{
					KeyBottom = ClusterNow.Data.ChainDrawParts.KeyBottom;
					if(Key <= KeyBottom)
					{
						if(null != InstanceMaterial)
						{	/* Material Valid (Normal-Parts) */
							if(InstanceMaterial == ClusterNow.Data.InstanceMaterial)
							{	/* Add in Now-Cluster */
								goto PartsSetDraw_End_PartsInsert;
							}

							if((Key <= ClusterNow.Data.ChainDrawParts.KeyTop) && ((null != ClusterPrevious) && (InstanceMaterial == ClusterPrevious.Data.InstanceMaterial)))
							{	/* Add to Previous-Cluster's Bottom */
								ClusterNow = ClusterPrevious;
								goto PartsSetDraw_End_PartsInsert;
							}
						}

						if(Key < KeyBottom)
						{	/* Interrupt ("Instance","Effect" and Another-Material-Sprite) ... not attach Draw-Parts' bottom */
							if(Key < ClusterNow.Data.ChainDrawParts.KeyTop)
							{
								goto PartsSetDraw_End_ClusterNewAdd;
							}

							/* Split-Point Get */
							FragmentDrawPartsPrevious = ClusterNow.Data.ChainDrawParts.ChainFragmentGetInsert(Key);

							/* Get Split-Cluster-Buffer (After) */
							ClusterAdd = InstanceRoot.ChainClusterDrawParts.InsntaceAdmitClusterDrawParts();
							if(null == ClusterAdd)
							{
								return(false);
							}
							ClusterAdd.Data.InstanceRoot = ClusterNow.Data.InstanceRoot;
							ClusterAdd.Data.InstanceMaterial = ClusterNow.Data.InstanceMaterial;

							/* Split Chain (Create After-Cluster) */
							if(true == ClusterNow.Data.ChainDrawParts.ChainSplit(FragmentDrawPartsPrevious, ClusterAdd.Data.ChainDrawParts))
							{	/* Sprit */
								/* Add Cluster */
								InstanceRoot.ChainClusterDrawParts.ChainInsert(ClusterAdd, ClusterNow);

								/* Add New Cluster */
								ClusterPrevious = ClusterNow;
								goto PartsSetDraw_End_ClusterNewAdd;
							}
							else
							{	/* No-Sprit ... Error */
								InstanceRoot.ChainClusterDrawParts.InsntaceDenyClusterDrawParts();
								goto PartsSetDraw_End_ClusterNewAdd;
							}
						}
					}
					ClusterPrevious = ClusterNow;
					ClusterNow = ClusterPrevious.ChainNext;
				}

				ClusterNow = ClusterPrevious;
				if((null == ClusterNow) || (null == InstanceMaterial) || (InstanceMaterial != ClusterNow.Data.InstanceMaterial))
				{	/* Add New Cluster */
					goto PartsSetDraw_End_ClusterNewAdd;
				}
				/* Fall-Through */
			PartsSetDraw_End_PartsInsert:;
				/* Insert Draw-Parts to Cluster */
				FragmentDrawPartsPrevious = ClusterNow.Data.ChainDrawParts.ChainFragmentGetInsert(Key);
				ClusterNow.Data.ChainDrawParts.ChainInsert(DrawParts, FragmentDrawPartsPrevious);
				return(true);

			PartsSetDraw_End_ClusterNewAdd:;
				/* Initialize Cluster */
				ClusterAdd = InstanceRoot.ChainClusterDrawParts.InsntaceAdmitClusterDrawParts();
				ClusterAdd.Data.InstanceRoot = DrawParts.Data.InstanceRoot;
				ClusterAdd.Data.InstanceMaterial = InstanceMaterial;

				/* Add Draw-Parts to Cluster */
				ClusterAdd.Data.ChainDrawParts.ChainInsert(DrawParts, null);

				/* Add Cluster to Cluster-Chain */
				InstanceRoot.ChainClusterDrawParts.ChainInsert(ClusterAdd, ClusterPrevious);

				return(true);
			}

			internal bool PartsSetDrawFixed(Library_SpriteStudio.Script.Root InstanceRoot, Material InstanceMaterial, float Key)
			{
				/* FragmentClusterDrawParts */	Library_SpriteStudio.Miscellaneousness.Chain<DataClusterDrawParts>.Fragment ClusterNow = InstanceRoot.ChainClusterDrawParts.ChainBottom;
				/* FragmentClusterDrawParts */	Library_SpriteStudio.Miscellaneousness.Chain<DataClusterDrawParts>.Fragment ClusterAdd = null;

				DrawParts.ChainCleanUp();
				DrawParts.Key = Key;

				if((null != InstanceMaterial) && (null != ClusterNow) && (ClusterNow.Data.InstanceMaterial == InstanceMaterial))
				{
					/* MEMO: Add DrawParts to Now-Cluster */
					ClusterNow.Data.ChainDrawParts.ChainAddForce(DrawParts);
					return(true);
				}

				/* MEMO: Add Cluster */
				/* Initialize Cluster */
				ClusterAdd = InstanceRoot.ChainClusterDrawParts.InsntaceAdmitClusterDrawParts();
				ClusterAdd.Data.InstanceRoot = DrawParts.Data.InstanceRoot;
				ClusterAdd.Data.InstanceMaterial = InstanceMaterial;

				/* Add Draw-Parts to Cluster */
				ClusterAdd.Data.ChainDrawParts.ChainAddForce(DrawParts);

				/* Add Cluster to Cluster-Chain */
				InstanceRoot.ChainClusterDrawParts.ChainInsert(ClusterAdd, ClusterNow);

				return(true);
			}
		}

		internal struct DataDrawObject
		{
			internal Library_SpriteStudio.Script.Root InstanceRoot;
		}
		internal class FragmentDrawObject : Miscellaneousness.Chain<DataDrawObject>.Fragment
		{
			internal Library_SpriteStudio.Script.Root InstanceRoot
			{
				get
				{
					return(Data.InstanceRoot);
				}
				set
				{
					Data.InstanceRoot = value;
				}
			}

			internal void CleanUp()
			{
				ChainCleanUp();
			}

			internal bool BootUp()
			{
				CleanUp();
				Data.InstanceRoot = null;

				return(true);
			}
		}
		internal class TerminalDrawObject : Miscellaneousness.Chain<DataDrawObject>.Terminal
		{
			internal void CleanUp()
			{
				ChainCleanUp();
			}

			internal void DrawObjectSort(Camera InstanceCamera)
			{
				/* FragmentDrawObject */	Miscellaneousness.Chain<DataDrawObject>.Fragment InstanceFragmentNow = ChainTop;
				/* FragmentDrawObject */	Miscellaneousness.Chain<DataDrawObject>.Fragment InstanceFragmentNext = null;
				/* FragmentDrawObject */	Miscellaneousness.Chain<DataDrawObject>.Fragment InstanceFragmentPrevious = null;
				Library_SpriteStudio.Script.Root InstanceRoot = null;
				Vector3 TransformCoordinate;
				float ClipPlaneNear;
				float ClipPlaneFar;
				ChainCleanUp();
				while(null != InstanceFragmentNow)
				{
					InstanceFragmentNext = InstanceFragmentNow.ChainNext;

					InstanceRoot = InstanceFragmentNow.Data.InstanceRoot;
					if(null != InstanceRoot)
					{
						if(null == InstanceCamera)
						{
							TransformCoordinate = Vector3.zero;
							ClipPlaneNear = 0.0f;	/* for now ... "Camera Ortho"'s Default */
							ClipPlaneFar = 10000.0f;	/* for now ... "Camera Ortho"'s Default */
						}
						else
						{
							TransformCoordinate = InstanceCamera.WorldToScreenPoint(InstanceRoot.InstanceTransform.position);
							ClipPlaneNear = InstanceCamera.nearClipPlane;
							ClipPlaneFar = InstanceCamera.farClipPlane;
						}
						if((ClipPlaneNear <= TransformCoordinate.z) && (ClipPlaneFar >= TransformCoordinate.z))
						{	/* In Sight */
							/* Sort from Back to Front */
							InstanceFragmentNow.Key = -TransformCoordinate.z;
							InstanceFragmentPrevious = ChainFragmentGetInsert(InstanceFragmentNow.Key);
							ChainInsert(InstanceFragmentNow, InstanceFragmentPrevious);
						}
					}

					InstanceFragmentNow = InstanceFragmentNext;
				}
			}

			internal void ClusterConstruct(TerminalClusterDrawParts ClusterTerminal)
			{
				/* Cluster-Chain Clear */
				ClusterTerminal.ChainCleanUp();

				/* Cluster Collect */
				/* FragmentDrawObject */	Miscellaneousness.Chain<DataDrawObject>.Fragment InstanceDrawObjectNow = null;
				Library_SpriteStudio.ManagerDraw.TerminalClusterDrawParts ChainClusterDrawParts = null;
				Library_SpriteStudio.Script.Root InstanceRoot = null;
				InstanceDrawObjectNow = ChainTop;
				while(null != InstanceDrawObjectNow)
				{
					/* Target Cluster DataParts Collect */
					InstanceRoot = InstanceDrawObjectNow.Data.InstanceRoot;
					if(null != InstanceRoot)
					{
						/* Fix DrawObject's Cluster-Chain */
						ChainClusterDrawParts = InstanceRoot.ChainClusterDrawParts;
						if(null != ChainClusterDrawParts)
						{
							if(0 < ChainClusterDrawParts.CountChain)
							{
								/* Cluster Merge */
								if(null == ClusterTerminal.ChainTop)
								{
									ClusterTerminal.ChainTop = ChainClusterDrawParts.ChainTop;
								}
								else
								{
									ClusterTerminal.ChainBottom.ChainNext = ChainClusterDrawParts.ChainTop;
								}
								ClusterTerminal.ChainBottom = ChainClusterDrawParts.ChainBottom;
								ClusterTerminal.CountChain += ChainClusterDrawParts.CountChain;
							}

							/* Original Cluster Clear */
							ChainClusterDrawParts.ChainCleanUp();
						}
					}

					InstanceDrawObjectNow = InstanceDrawObjectNow.ChainNext;
				}
			}
		}

		internal static void ClusterFix(TerminalClusterDrawParts ClusterTerminal)
		{
			/* FragmentClusterDrawParts */	Library_SpriteStudio.Miscellaneousness.Chain<DataClusterDrawParts>.Fragment ClusterPrevious = null;
			/* FragmentClusterDrawParts */	Library_SpriteStudio.Miscellaneousness.Chain<DataClusterDrawParts>.Fragment ClusterNow = ClusterTerminal.ChainTop;
			Library_SpriteStudio.Script.Root InstanceRootSub = null;
			Library_SpriteStudio.ManagerDraw.TerminalClusterDrawParts ChainClusterSub = null;
			/* FragmentClusterDrawParts */	Library_SpriteStudio.Miscellaneousness.Chain<DataClusterDrawParts>.Fragment ClusterTopSub = null;
			/* FragmentClusterDrawParts */	Library_SpriteStudio.Miscellaneousness.Chain<DataClusterDrawParts>.Fragment ClusterBottomSub = null;
			int CountClusterSub = 0;
			while(null != ClusterNow)
			{
				if(null == ClusterNow.Data.InstanceMaterial)
				{	/* Sub-ClusterChain Call */
					InstanceRootSub = ClusterNow.Data.InstanceRoot;
					CountClusterSub = 0;
					ClusterTopSub = null;
					ClusterBottomSub = null;
					if(null != InstanceRootSub)
					{
						/* Get Sub-Clusters */
						ChainClusterSub = InstanceRootSub.ChainClusterDrawParts;
						if(null != ChainClusterSub)
						{
							CountClusterSub = ChainClusterSub.CountChain;
							if(0 < CountClusterSub)
							{
								/* Original Sub-ClusterChain Get */
								ClusterTopSub = ChainClusterSub.ChainTop;
								ClusterBottomSub = ChainClusterSub.ChainBottom;

								/* Insert Clusters */
								ClusterBottomSub.ChainNext = ClusterNow.ChainNext;
								if(ClusterNow == ClusterTerminal.ChainBottom)
								{
									ClusterTerminal.ChainBottom = ClusterBottomSub;
								}
								ClusterNow.ChainNext = ClusterTopSub;
								ClusterTerminal.CountChain += CountClusterSub;
							}

							/* Original Sub-ClusterChain Clear */
							ChainClusterSub.ChainCleanUp();
						}
					}

					/* Remove Cluster */
					/* MEMO: "ClusterBottomSub" is Temporary */
					ClusterBottomSub = ClusterNow.ChainNext;
					if(null == ClusterPrevious)
					{
						ClusterTerminal.ChainTop = ClusterNow.ChainNext;
					}
					else
					{
						ClusterPrevious.ChainNext = ClusterNow.ChainNext;
					}
					if(ClusterNow == ClusterTerminal.ChainBottom)
					{
						ClusterTerminal.ChainBottom = ClusterPrevious;
					}
					ClusterTerminal.CountChain--;

					ClusterNow.ChainCleanUp();

					ClusterNow = (null != ClusterTopSub) ? ClusterTopSub : ClusterBottomSub;
					continue;
				}
				else
				{	/* Normal-Cluster */
					if(null != ClusterPrevious)
					{
						if(ClusterPrevious.Data.InstanceMaterial == ClusterNow.Data.InstanceMaterial)
						{
							/* Merge Cluster */
							ClusterPrevious.Data.ChainDrawParts.ChainBottom.ChainNext = ClusterNow.Data.ChainDrawParts.ChainTop;
							ClusterPrevious.Data.ChainDrawParts.ChainBottom = ClusterNow.Data.ChainDrawParts.ChainBottom;
							ClusterPrevious.Data.ChainDrawParts.CountChain += ClusterNow.Data.ChainDrawParts.CountChain;

							/* Remove Cluster */
							ClusterPrevious.ChainNext = ClusterNow.ChainNext;
							if(ClusterNow == ClusterTerminal.ChainBottom)
							{
								ClusterTerminal.ChainBottom = ClusterPrevious;
							}
							ClusterTerminal.CountChain--;

							ClusterNow.ChainCleanUp();

							ClusterNow = ClusterPrevious.ChainNext;
							continue;
						}
					}
				}

				ClusterPrevious = ClusterNow;
				ClusterNow = ClusterNow.ChainNext;
			}
		}

#if UNITY_5_3_OR_NEWER
		static System.WeakReference VertexNoTriangleBuffer;
#endif
#if UNITY_5_6_OR_NEWER
/* Unity5.5.1p1 or newer*/
		static System.WeakReference TriangleBuffer;
#endif
		static System.WeakReference TableIndexTriangleBuffer;

		internal static void MeshCreate(	TerminalClusterDrawParts ClusterTerminal,
											ref Mesh InstanceMeshWrite,
											ref Material[] InstanceMaterialWrite,
											ref Mesh InstanceMeshDraw,
											ref Material[] InstanceMaterialDraw,
											ref CombineInstance[] CombineMesh,
											MeshRenderer InstanceMeshRenderer,
											MeshFilter InstanceMeshFilter,
											Transform InstanceTrasnformDrawManager,
											Camera InstanceCamera
										)
		{
			/* FragmentClusterDrawParts */	Library_SpriteStudio.Miscellaneousness.Chain<DataClusterDrawParts>.Fragment ClusterNow = null;
			/* FragmentDrawParts */	Library_SpriteStudio.Miscellaneousness.Chain<DataDrawParts>.Fragment DataPartsNow = null;
			int Index;

			/* Mesh Clear */
			Mesh InstanceMesh = InstanceMeshWrite;
			InstanceMesh.Clear();

			/* Count Mesh & Material-Table Create */
			int CountMaterial = ClusterTerminal.CountChain;
			if(0 >= CountMaterial)
			{
				goto MeshCreate_MeshNoDraw;
			}

			if(InstanceMaterialWrite == null || InstanceMaterialWrite.Length != CountMaterial)
			{
				InstanceMaterialWrite = new Material[CountMaterial];
			}
			Material[] TableMaterial = InstanceMaterialWrite;
			int CountMesh = 0;
			Index = 0;
			ClusterNow = ClusterTerminal.ChainTop;
			while(null != ClusterNow)
			{
				CountMesh += ClusterNow.Data.ChainDrawParts.CountChain;
				TableMaterial[Index] = ClusterNow.Data.InstanceMaterial;
				Index++;
				ClusterNow = ClusterNow.ChainNext;
			}
			if(0 >= CountMesh)
			{
				goto MeshCreate_MeshNoDraw;
			}
			if(null != InstanceMaterialDraw)
			{
				InstanceMeshRenderer.sharedMaterials = InstanceMaterialDraw;
			}

			/* Create Combined Mesh */
			Matrix4x4 MatrixCollect = (null != InstanceTrasnformDrawManager) ? InstanceTrasnformDrawManager.localToWorldMatrix.inverse : Matrix4x4.identity;

			if((null == CombineMesh) || (CombineMesh.Length != CountMesh))
			{
				CombineMesh = new CombineInstance[CountMesh];
			}
			List<int> TableIndexTriangle = null;
			if(1 < CountMaterial)
			{
				if(TableIndexTriangleBuffer != null)
				{
					TableIndexTriangle = TableIndexTriangleBuffer.Target as List<int>;
				}
				if(TableIndexTriangle == null)
				{
					TableIndexTriangle = new List<int>(CountMaterial + 1);  /* +1 ... Total Data */
					TableIndexTriangleBuffer = new System.WeakReference(TableIndexTriangle);
				}
				else
				{
					TableIndexTriangle.Clear();
				}
			}
			DataPartsNow = null;
			ClusterNow = ClusterTerminal.ChainTop;
			Index = 0;
			int IndexTriangle = 0;
			int MaxTriangleCountForSubmesh = 0;
			for(int i=0; i<CountMaterial; i++)
			{
				DataPartsNow = ClusterNow.Data.ChainDrawParts.ChainTop;
				if(TableIndexTriangle != null)
				{
					TableIndexTriangle.Add(IndexTriangle);
					if(i == 0)
					{
						MaxTriangleCountForSubmesh = IndexTriangle;
					}
					else
					{
						if(MaxTriangleCountForSubmesh < IndexTriangle - TableIndexTriangle[TableIndexTriangle.Count - 2])
						{
							MaxTriangleCountForSubmesh = IndexTriangle - TableIndexTriangle[TableIndexTriangle.Count - 2];
						}
					}
				}

				while(null != DataPartsNow)
				{
					if(null != DataPartsNow.Data.InstanceMesh)
					{
						CombineMesh[Index].mesh = DataPartsNow.Data.InstanceMesh;
						CombineMesh[Index].transform = MatrixCollect * DataPartsNow.Data.InstanceTransform.localToWorldMatrix;
						Index++;
						IndexTriangle += (KindParts.NORMAL_TRIANGLE4 == DataPartsNow.Data.Kind) ? 4 : 2;    /* ArrayCoordinate_TriangleX.Length / 3 */
					}

					DataPartsNow = DataPartsNow.ChainNext;
				}
				ClusterNow = ClusterNow.ChainNext;
			}

			for(int i=Index; i<CombineMesh.Length; i++)
			{
				CombineMesh[i].mesh = null;
			}
			InstanceMesh.CombineMeshes(CombineMesh);

			if(TableIndexTriangle != null)
			{
				TableIndexTriangle.Add(IndexTriangle);
				if(MaxTriangleCountForSubmesh < IndexTriangle - TableIndexTriangle[TableIndexTriangle.Count - 2])
				{
					MaxTriangleCountForSubmesh = IndexTriangle - TableIndexTriangle[TableIndexTriangle.Count - 2];
				}
			}

			/* SubMesh Construct */
			if(1 < CountMaterial)
			{
#if UNITY_5_6_OR_NEWER
/* Unity5.5.1p1 or newer*/
				List<int> Triangles = null;
				if (TriangleBuffer != null)
				{
					Triangles = TriangleBuffer.Target as List<int>;
				}
				if (Triangles == null)
				{
					Triangles = new List<int>(IndexTriangle * 3);
					TriangleBuffer = new System.WeakReference(Triangles);
				}
				InstanceMesh.GetTriangles(Triangles, 0);
#else
				int[] Triangles = InstanceMesh.triangles;
#endif

#if UNITY_5_3_OR_NEWER
				List<int> VertexNoTriangle = null;
				if (VertexNoTriangleBuffer != null)
				{
					VertexNoTriangle = VertexNoTriangleBuffer.Target as List<int>;
				}
				if (VertexNoTriangle == null)
				{
					VertexNoTriangle = new List<int>(MaxTriangleCountForSubmesh * 3);
					VertexNoTriangleBuffer = new System.WeakReference(VertexNoTriangle);
				}
#else
				int[] VertexNoTriangle = null;
				int intSize = sizeof(int);
#endif
				InstanceMesh.triangles = null;
				InstanceMesh.subMeshCount = CountMaterial;
				for (int i=0; i<CountMaterial; i++)
				{
#if UNITY_5_3_OR_NEWER
					VertexNoTriangle.Clear();
					for(int j = TableIndexTriangle[i]; j < TableIndexTriangle[i + 1]; ++j)
					{
						VertexNoTriangle.Add(Triangles[j * 3]);
						VertexNoTriangle.Add(Triangles[j * 3 + 1]);
						VertexNoTriangle.Add(Triangles[j * 3 + 2]);
					}
					InstanceMesh.SetTriangles(VertexNoTriangle, i);
#else
					CountMesh = TableIndexTriangle[i + 1] - TableIndexTriangle[i];
					if (VertexNoTriangle == null || VertexNoTriangle.Length != CountMesh * 3)
					{
						VertexNoTriangle = new int[CountMesh * 3];
					}
					System.Buffer.BlockCopy(	Triangles, TableIndexTriangle[i] * 3 * intSize,
												VertexNoTriangle, 0,
												CountMesh * 3 * intSize
											);
					InstanceMesh.SetTriangles(VertexNoTriangle, i);
#endif
				}
			}

			InstanceMesh.name = "BatchedMesh";
//			InstanceMeshWrite = InstanceMesh;
			InstanceMeshFilter.sharedMesh = InstanceMeshDraw;

			/* Clear Draw-Entries */
			ClusterTerminal.ChainCleanUp();
			return;

		MeshCreate_MeshNoDraw:;
			InstanceMeshFilter.sharedMesh = null;

			/* Clear Draw-Entries */
			ClusterTerminal.ChainCleanUp();
			return;
		}
	}

	public class Script
	{
		[System.Serializable]
		public class Root : MonoBehaviour
		{
			/* Base-Datas */
			public Material[] TableMaterial;
			public Script_SpriteStudio_DataCell DataCellMap;

			/* WorkArea: DrawManager */
			public Script_SpriteStudio_ManagerDraw InstanceManagerDraw;
			internal Library_SpriteStudio.ManagerDraw.FragmentDrawObject DrawObject = null;
			internal Library_SpriteStudio.ManagerDraw.TerminalClusterDrawParts ChainClusterDrawParts = null;
			internal int CountPartsDraw;

			/* WorkArea: Cell-Table Changing Datas */
			internal Library_SpriteStudio.Control.CellChange[][] TableCellChange = null;

			/* Relation Datas */
			internal Script_SpriteStudio_Root InstanceRootParent = null;
			internal Transform InstanceTransform = null;

			/* Playing Datas */
			public bool FlagHideForce;
			public float RateSpeed;
			internal bool FlagUpdatingAnimation;

			/* Playing Datas: for Runtime (WorkArea) */
			internal float TimePerFrame = 1.0f;
			internal float TimeElapsed = 0.0f;
			internal float TimeDelay = 0.0f;
			internal float RateOpacity = 1.0f;

			internal bool StartBase(int CountPartsDrawInitial)
			{
				DrawObject = new Library_SpriteStudio.ManagerDraw.FragmentDrawObject();
				DrawObject.CleanUp();

				InstanceTransform = transform;
				CountPartsDraw = CountPartsDrawInitial;

				return(true);
			}

			internal bool LateUpdateBase()
			{
				if(null == ChainClusterDrawParts)
				{
					ChainClusterDrawParts = new Library_SpriteStudio.ManagerDraw.TerminalClusterDrawParts();
					ChainClusterDrawParts.BootUp(CountPartsDraw);
				}
				return(true);
			}
		}
	}

	public class Miscellaneousness
	{
		public class Chain<_TypeDataFragment>
			where _TypeDataFragment : struct
		{
			internal class Fragment
			{
				internal Fragment ChainNext;
				internal float Key;
				internal _TypeDataFragment Data;

				internal void ChainCleanUp()
				{
					ChainNext = null;
					Key = 0.0f;
				}
			}

			internal class Terminal
			{
				internal int CountChain;
				internal Fragment ChainTop;
				internal Fragment ChainBottom;
				internal float KeyTop;
				internal float KeyBottom;

				internal void ChainCleanUp()
				{
					CountChain = 0;
					ChainTop = null;
					ChainBottom = null;
					KeyTop = float.MaxValue;
					KeyBottom = -float.MaxValue;
				}

				internal Fragment ChainFragmentGetInsert(float TargetKey)
				{
					if(null == ChainTop)
					{	/* Chain Empty */
						return(null);
					}

					if(TargetKey < KeyTop)
					{	/* Top */
						return(null);
					}

					if(TargetKey >= KeyBottom)
					{	/* Bottom */
						return(ChainBottom);
					}

					/* MEMO: (2 <= CountChain) */
					Fragment Previous = ChainTop;
					Fragment Next = Previous.ChainNext;
					while(null != Next)
					{
						if(TargetKey < Next.Key)
						{
							break;
						}
						Previous = Next;
						Next = Previous.ChainNext;
					}
					return(Previous);
				}

				internal void ChainInsert(Fragment FragmentTarget, Fragment FragmentPrevious)
				{
					if(null == ChainTop)
					{	/* Chain Empty */
						FragmentTarget.ChainNext = null;

						CountChain = 1;
						ChainTop = FragmentTarget;
						KeyTop = FragmentTarget.Key;
						ChainBottom = FragmentTarget;
						KeyBottom = FragmentTarget.Key;
						return;
					}

					if(null == FragmentPrevious)
					{	/* Top */
						FragmentTarget.ChainNext = ChainTop;

						CountChain++;
						ChainTop = FragmentTarget;
						KeyTop = FragmentTarget.Key;
						return;
					}

					if(null == FragmentPrevious.ChainNext)
					{	/* Bottom */
						FragmentTarget.ChainNext = null;

						FragmentPrevious.ChainNext = FragmentTarget;

						CountChain++;
						ChainBottom = FragmentTarget;
						KeyBottom = FragmentTarget.Key;
						return;
					}

					CountChain++;
					FragmentTarget.ChainNext = FragmentPrevious.ChainNext;
					FragmentPrevious.ChainNext = FragmentTarget;
				}

				internal void ChainAddForce(Fragment FragmentTarget)
				{	/* CAUTION!: Use in the case of no-sort only. */
					if(null == FragmentTarget)
					{	/* Invalid Fragment */
						return;
					}

					FragmentTarget.ChainNext = null;
					if(null == ChainTop)
					{
						ChainTop = FragmentTarget;
						ChainBottom = null;
					}

					CountChain++;
					if(null != ChainBottom)
					{
						ChainBottom.ChainNext = FragmentTarget;
					}
					ChainBottom = FragmentTarget;
				}

				internal bool ChainSplit(Fragment FragmentTarget, Terminal TerminalNew)
				{
					TerminalNew.ChainCleanUp();

					if(null == FragmentTarget)
					{	/* No Sprit */
						return(false);
					}

					Fragment FragmentNext = FragmentTarget.ChainNext;
					if(null != FragmentNext)
					{	/* Move Part-of-Chain */
						TerminalNew.ChainTop = FragmentNext;
						TerminalNew.KeyTop = FragmentNext.Key;
						TerminalNew.ChainBottom = ChainBottom;
						TerminalNew.KeyBottom = KeyBottom;
						TerminalNew.CountChainReacquire();
					}

					FragmentTarget.ChainNext = null;
					ChainBottom = FragmentTarget;
					KeyBottom = FragmentTarget.Key;
					CountChainReacquire();
					return(true);
				}

				internal void ChainMerge(Terminal TerminalFollow)
				{	/* CAUTION!: Merge, sort regardless. */
					if(null == TerminalFollow.ChainTop)
					{
//						TerminalFollow.ChainPurge();
						return;
					}

					if(null == ChainTop)
					{
						CountChain = TerminalFollow.CountChain;
						ChainTop = TerminalFollow.ChainTop;
						ChainBottom = TerminalFollow.ChainBottom;
						KeyTop = TerminalFollow.KeyTop;
						KeyBottom = TerminalFollow.KeyBottom;

						TerminalFollow.ChainPurge();
						return;
					}

					CountChain += TerminalFollow.CountChain;
					ChainBottom.ChainNext = TerminalFollow.ChainTop;
					ChainBottom = TerminalFollow.ChainBottom;
					KeyBottom = TerminalFollow.KeyBottom;
				}

				internal void ChainPurge()
				{
					ChainCleanUp();
				}

				private void CountChainReacquire()
				{
					int Count = 0;
					Fragment FragmentNow = ChainTop;
					while(null != FragmentNow)
					{
						Count++;
						FragmentNow = FragmentNow.ChainNext;
					}
					CountChain = Count;
				}
			}
		}

		public class Asset
		{
			public static GameObject GameObjectCreate(string Name, bool FlagActive, GameObject InstanceGameObjectParent)
			{
				GameObject InstanceGameObject = new GameObject(Name);
				if(null != InstanceGameObject)
				{
					ActiveSetGameObject(InstanceGameObject, FlagActive);
					Transform InstanceTransform = InstanceGameObject.transform;
					if(null != InstanceGameObjectParent)
					{
						InstanceTransform.parent = InstanceGameObjectParent.transform;
					}
					InstanceTransform.localPosition = Vector3.zero;
					InstanceTransform.localEulerAngles = Vector3.zero;
					InstanceTransform.localScale = Vector3.one;
				}
				return(InstanceGameObject);
			}

			public static void ActiveSetGameObject(GameObject InstanceGameObject, bool FlagSwitch)
			{
#if UNITY_3_5
				InstanceGameObject.active = FlagSwitch;
#else
				InstanceGameObject.SetActive(FlagSwitch);
#endif
			}

			public static GameObject PrefabInstantiateChild(	GameObject InstanceGameObjectParent,
																GameObject GameObjectPrefab,
																GameObject InstanceGameObjectOld,
																bool FlagInstanceUnderControlRenew
															)
			{
				/* Error-Check */
				if(null == GameObjectPrefab)
				{
					return(null);
				}

				GameObject InstanceGameObject = InstanceGameObjectOld;
				Transform InstanceTransformParent = InstanceGameObjectParent.transform;
				Transform InstanceTransform;

				if(null == InstanceGameObject)
				{	/* Lost (Not-Found) */
					InstanceTransform = InstanceTransformParent.Find(GameObjectPrefab.name);
					if(null != InstanceTransform)
					{	/* Found */
						InstanceGameObject = InstanceTransform.gameObject;
					}
				}

				if(true == FlagInstanceUnderControlRenew)
				{	/* Renew Force */
					if(null != InstanceGameObject)
					{	/* Exist */
						Object.DestroyImmediate(InstanceGameObject);
					}
					InstanceGameObject = null;
				}

				if(null == InstanceGameObject)
				{	/* Instantiate */
#if UNITY_EDITOR
					InstanceGameObject = UnityEditor.PrefabUtility.InstantiatePrefab(GameObjectPrefab) as GameObject;
					if(null == InstanceGameObject)
					{	/* for not-prefab */
						InstanceGameObject = Object.Instantiate(GameObjectPrefab) as GameObject;
						InstanceGameObject.name = GameObjectPrefab.name;	/* Remove "(clone)" */
					}
#else
					InstanceGameObject = Object.Instantiate(GameObjectPrefab) as GameObject;
					InstanceGameObject.name = GameObjectPrefab.name;	/* Remove "(clone)" */
#endif
					InstanceTransform = InstanceGameObject.transform;

					if (null != InstanceGameObjectParent)
					{
						InstanceTransform = InstanceGameObject.transform;
						InstanceTransform.parent = InstanceTransformParent;
					}
					if(null != InstanceGameObject)
					{
						InstanceTransform.localPosition = Vector3.zero;
						InstanceTransform.localEulerAngles = Vector3.zero;
						InstanceTransform.localScale = Vector3.one;
					}
				}

				return(InstanceGameObject);
			}

			public static GameObject[] GameObjectBuildUpRoot(	Script_SpriteStudio_Root InstanceRoot,
																bool FlagColliderRigidBody,
																float ColliderThicknessZ
															)
			{
				GameObject[] ListGameObject = null;

				if(null == InstanceRoot.DataAnimation)
				{
					return(null);
				}
				int CountParts = InstanceRoot.DataAnimation.CountGetParts();
				if(0 >= CountParts)
				{
					return(null);
				}
				ListGameObject = new GameObject[CountParts];
				for(int i=0; i<CountParts; i++)
				{
					ListGameObject[i] = null;
				}

				ListGameObject[0] = InstanceRoot.gameObject;
				Transform InstanceTransformChild = null;
				while(0 < InstanceRoot.transform.childCount)
				{
					InstanceTransformChild = InstanceRoot.transform.GetChild(0);
					Object.DestroyImmediate(InstanceTransformChild.gameObject);
				}

				Library_SpriteStudio.Data.Parts DataParts = null;
				GameObject InstanceGameObjectParts = null;
				for(int i=1; i<CountParts; i++)
				{
					DataParts = InstanceRoot.DataAnimation.DataGetParts(i);
					InstanceGameObjectParts = GameObjectCreate(	DataParts.Name,
																false,
																ListGameObject[DataParts.IDParent]
															);
					ListGameObject[i] = InstanceGameObjectParts;

					switch(DataParts.KindShapeCollision)
					{
						case Library_SpriteStudio.KindCollision.NON:
							break;

						case Library_SpriteStudio.KindCollision.SQUARE:
							{
								Script_SpriteStudio_Collider InstanceScriptCollider = InstanceGameObjectParts.AddComponent<Script_SpriteStudio_Collider>();
								InstanceScriptCollider.InstanceRoot = InstanceRoot;
								InstanceScriptCollider.IDParts = i;

								if(true == FlagColliderRigidBody)
								{
									Rigidbody InstanceRigidbody = InstanceGameObjectParts.AddComponent<Rigidbody>();
									InstanceRigidbody.isKinematic = false;
									InstanceRigidbody.useGravity = false;
								}

								BoxCollider InstanceColliderBox = InstanceGameObjectParts.AddComponent<BoxCollider>();
								InstanceColliderBox.enabled = false;
								InstanceColliderBox.size = new Vector3(1.0f, 1.0f, ColliderThicknessZ);
								InstanceColliderBox.isTrigger = false;
							}
							break;

						case Library_SpriteStudio.KindCollision.AABB:
							/* Not Supprted. */
							goto case Library_SpriteStudio.KindCollision.NON;

						case Library_SpriteStudio.KindCollision.CIRCLE:
							{
								Script_SpriteStudio_Collider InstanceScriptCollider = InstanceGameObjectParts.AddComponent<Script_SpriteStudio_Collider>();
								InstanceScriptCollider.InstanceRoot = InstanceRoot;
								InstanceScriptCollider.IDParts = i;

								if(true == FlagColliderRigidBody)
								{
									Rigidbody InstanceRigidbody = InstanceGameObjectParts.AddComponent<Rigidbody>();
									InstanceRigidbody.isKinematic = false;
									InstanceRigidbody.useGravity = false;
								}

								CapsuleCollider InstanceColliderCapsule = InstanceGameObjectParts.AddComponent<CapsuleCollider>();
								InstanceColliderCapsule.enabled = false;
								InstanceColliderCapsule.radius = 1.0f;
								InstanceColliderCapsule.height = ColliderThicknessZ;
								InstanceColliderCapsule.direction = 2;
								InstanceColliderCapsule.isTrigger = false;
							}
							break;

						case Library_SpriteStudio.KindCollision.CIRCLE_SCALEMINIMUM:
							/* Not Supprted. */
							goto case Library_SpriteStudio.KindCollision.NON;

						case Library_SpriteStudio.KindCollision.CIRCLE_SCALEMAXIMUM:
							/* Not Supprted. */
							goto case Library_SpriteStudio.KindCollision.NON;
					}

					/* GameObject Active */
					ActiveSetGameObject(InstanceGameObjectParts, true);
				}
				return(ListGameObject);
			}
		}

		public static class Math
		{
			public static void CoordinateGetDiagonalIntersection(out Vector3 Output, ref Vector3 LU, ref Vector3 RU, ref Vector3 LD, ref Vector3 RD)
			{
				/* MEMO: Z-Values are ignored. */
				Output = Vector3.zero;

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
		}
	}

	public partial class Utility
	{
		public static partial class Math
		{
		}

		public static partial class Parts
		{
		}

		public static partial class TableMaterial
		{
		}

		public static partial class TableCellChange
		{
		}

		public static partial class Random
		{
			/* Common Interface class */
			public interface Generator
			{
				uint[] ListSeed
				{
					get;
				}

				void InitSeed(uint Seed);
				uint RandomUint32();
				double RandomDouble(double Limit=1.0);
				float RandomFloat(float Limit=1.0f);
				int RandomN(int Limit);
			}
		}
	}
}
