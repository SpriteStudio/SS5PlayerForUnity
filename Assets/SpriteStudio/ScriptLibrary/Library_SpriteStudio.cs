/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
#define EFFECTUPDATE_CONFORMtoSS5_5

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

	public class Data
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
					if(0 == string.Compare(NameCell, ListCell[i].Name))
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
					if(0 == string.Compare(NameLabel, ListNameLabelAnimationReserved[i]))
					{
						return((int)Library_SpriteStudio.KindLabelAnimationReserved.INDEX_RESERVED + i);
					}
				}

				Count = ListLabel.Length;
				for(int i=0; i<Count; i++)
				{
					if(0 == string.Compare(NameLabel, ListLabel[i].Name))
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

			public Library_SpriteStudio.Data.ListAttributeStatus Status;

			public Library_SpriteStudio.Data.ListAttributeVector3 Position;		/* Always Compressed */
			public Library_SpriteStudio.Data.ListAttributeVector3 Rotation;		/* Always Compressed */
			public Library_SpriteStudio.Data.ListAttributeVector2 Scaling;		/* Always Compressed */

			public Library_SpriteStudio.Data.ListAttributeFloat RateOpacity;
			public Library_SpriteStudio.Data.ListAttributeFloat Priority;

			public Library_SpriteStudio.Data.ListAttributeVector2 PositionAnchor;	/* Reserved. */
			public Library_SpriteStudio.Data.ListAttributeVector2 SizeForce;

			public Library_SpriteStudio.Data.ListAttributeUserData UserData;	/* Always Compressed */
			public Library_SpriteStudio.Data.ListAttributeInstance Instance;	/* Always Compressed */
			public Library_SpriteStudio.Data.ListAttributeEffect Effect;		/* Always Compressed */

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
				if(null != Scaling)
				{
					Scaling.CompressCPE(CountFrame);
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
				/* MEMO: Always Compressed */
//				if(null != Scaling)
//				{
//					Scaling.CompressCPE(CountFrame);
//				}

				if(null != RateOpacity)
				{
					RateOpacity.CompressCPE(CountFrame);
				}
				if(null != Priority)
				{
					Priority.CompressCPE(CountFrame);
				}

				if(null != PositionAnchor)
				{
					PositionAnchor.CompressCPE(CountFrame);
				}
				if(null != SizeForce)
				{
					SizeForce.CompressCPE(CountFrame);
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
				/* MEMO: Always Compressed */
//				if(null != Scaling)
//				{
//					Scaling.DecompressCPE(CountFrame);
//				}

				if(null != RateOpacity)
				{
					RateOpacity.DecompressCPE(CountFrame);
				}
				if(null != Priority)
				{
					Priority.DecompressCPE(CountFrame);
				}

				if(null != PositionAnchor)
				{
					PositionAnchor.DecompressCPE(CountFrame);
				}
				if(null != SizeForce)
				{
					SizeForce.DecompressCPE(CountFrame);
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
			public Library_SpriteStudio.Data.ListAttributeCell Cell;

			public Library_SpriteStudio.Data.ListAttributeColorBlend ColorBlend;
			public Library_SpriteStudio.Data.ListAttributeVertexCorrection VertexCorrection;
			public Library_SpriteStudio.Data.ListAttributeVector2 OffsetPivot;

			public Library_SpriteStudio.Data.ListAttributeVector2 PositionTexture;
			public Library_SpriteStudio.Data.ListAttributeVector2 ScalingTexture;
			public Library_SpriteStudio.Data.ListAttributeFloat RotationTexture;

			public Library_SpriteStudio.Data.ListAttributeFloat RadiusCollision;	/* for Sphere-Collider *//* Always Compressed */

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
			public Library_SpriteStudio.Data.ListAttributeIndexCellMapFix IndexCellMapMesh;
			public Library_SpriteStudio.Data.ListAttributeCoordinateMeshFix CoordinateMesh;
			public Library_SpriteStudio.Data.ListAttributeColorBlendMeshFix ColorBlendMesh;
			public Library_SpriteStudio.Data.ListAttributeUVMeshFix UV0Mesh;

			public Library_SpriteStudio.Data.ListAttributeVector2 SizeCollision;	/* for Box-Collider *//* Always Compressed */
			public Library_SpriteStudio.Data.ListAttributeVector2 PivotCollision;	/* for Box-Collider *//* Always Compressed */

			public Library_SpriteStudio.Data.ListAttributeFloat RadiusCollision;	/* for Sphere-Collider *//* Always Compressed */

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
				if(null != SizeCollision)
				{
					SizeCollision.CompressCPE(CountFrame);
				}
				/* MEMO: Always Compressed */
				if(null != PivotCollision)
				{
					PivotCollision.CompressCPE(CountFrame);
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

				if(null != CoordinateMesh)
				{
					CoordinateMesh.CompressCPE(CountFrame);
				}

				if(null != ColorBlendMesh)
				{
					ColorBlendMesh.CompressCPE(CountFrame);
				}

				if(null != UV0Mesh)
				{
					UV0Mesh.CompressCPE(CountFrame);
				}

				/* MEMO: Always Compressed */
//				if(null != SizeCollision)
//				{
//					SizeCollision.CompressCPE(CountFrame);
//				}
				/* MEMO: Always Compressed */
//				if(null != PivotCollision)
//				{
//					PivotCollision.CompressCPE(CountFrame);
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

				if(null != CoordinateMesh)
				{
					CoordinateMesh.DecompressCPE(CountFrame);
				}

				if(null != ColorBlendMesh)
				{
					ColorBlendMesh.DecompressCPE(CountFrame);
				}

				if(null != UV0Mesh)
				{
					UV0Mesh.DecompressCPE(CountFrame);
				}

				/* MEMO: Always Compressed */
//				if(null != SizeCollision)
//				{
//					SizeCollision.DecompressCPE(CountFrame);
//				}
				/* MEMO: Always Compressed */
//				if(null != PivotCollision)
//				{
//					PivotCollision.DecompressCPE(CountFrame);
//				}
				/* MEMO: Always Compressed */
//				if(null != RadiusCollision)
//				{
//					RadiusCollision.DecompressCPE(CountFrame);
//				}

				return(true);
			}
		}

		public class ListAttribute_Base<_TypeValue>
		{
			[System.Flags]
			public enum FlagBit : int
			{
				FRAMENO = 0x00007fff,
				INDEX = 0x3fff8000,

				CLEAR = 0x00000000,
			}

			public FlagBit[] ListStatus;
			public _TypeValue[] ListValue;

			public bool IsValid
			{
				get
				{
					return(0 < ListValue.Length);
				}
			}

			public void CleanUp()
			{
				ListStatus = null;
				ListValue = null;
			}

			public void BootUp(int CountFrame)
			{	/* MEMO: FrameCount==0 ... No-Data *//* MEMO: Always Uncompressed */
				ListStatus = new FlagBit[0];
				ListValue = new _TypeValue[CountFrame];
			}

			public void BootUpCompressedForce(int CountData)
			{	/* MEMO: Always Uncompressed */
				ListStatus = new FlagBit[CountData];
				ListValue = new _TypeValue[CountData];
				for(int i=0; i<CountData; i++)
				{
					ListStatus[i] = FlagBit.INDEX;	/* .CLEAR; */	/* Invalid Data */
				}
			}
			public static FlagBit GetFlagBit(int FrameNo, int IndexData)
			{
				FlagBit FlagBitNow = ((FlagBit)FrameNo) & FlagBit.FRAMENO;
				FlagBitNow |= ((FlagBit)(IndexData << 15)) & FlagBit.INDEX;
				return(FlagBitNow);
			}

			public int IndexGetForce(out int FrameNo, int IndexStatus)
			{
				FlagBit StatusTemp = ListStatus[IndexStatus];
				FrameNo = (int)(StatusTemp & FlagBit.FRAMENO);
				StatusTemp &= FlagBit.INDEX;
				return((FlagBit.INDEX == StatusTemp) ? -1 : ((int)StatusTemp >> 15));
			}

			public int IndexGetValue(out int FrameNoOrigin, int FrameNo)
			{
				int Index = -1;
				int FrameNoKey = -1;

				if(0 >= ListStatus.Length)
				{	/* MEMO: Uncompressed or No-Data */
					if(0 >= ListValue.Length)
					{	/* MEMO: No-Data */
						goto IndexGetValue_End;
					}

					FrameNoKey = FrameNo;
					Index = FrameNo;
					goto IndexGetValue_End;
				}

				int Count = ListStatus.Length;
				FlagBit StatusTemp;
				for(int i=1; i<Count; i++)
				{
					StatusTemp = ListStatus[i] & FlagBit.FRAMENO;
					FrameNoKey = (FlagBit.FRAMENO == StatusTemp) ? -1 : (int)StatusTemp;
					if(FrameNo < FrameNoKey)
					{
						StatusTemp = ListStatus[i - 1] & FlagBit.INDEX;
						Index = (FlagBit.INDEX == StatusTemp) ? -1 : ((int)StatusTemp >> 15);
						goto IndexGetValue_End;
					}
				}

				Count--;	/* Last Data */
				StatusTemp = ListStatus[Count] & FlagBit.FRAMENO;
				FrameNoKey = (FlagBit.FRAMENO == StatusTemp) ? -1 : (int)StatusTemp;
				StatusTemp = ListStatus[Count] & FlagBit.INDEX;
				Index = (FlagBit.INDEX == StatusTemp) ? -1 : ((int)StatusTemp >> 15);

			IndexGetValue_End:;
				FrameNoOrigin = FrameNoKey;
				return(Index);
			}

			public bool CompressCPE(int CountFrame)
			{	/* MEMO: CPE(Changing-Point Extracting) */
				if(0 < ListStatus.Length)
				{	/* Already Compressed */
					return(true);
				}

				if(0 >= ListValue.Length)
				{	/* No-Data */
					return(true);
				}

				/* WorkArea Boot-Up */
				List<int> ArrayFrameNo = new List<int>();
				List<int> ArrayIndex = new List<int>();
				List<_TypeValue> ArrayValue = new List<_TypeValue>();
				ArrayFrameNo.Clear();
				ArrayIndex.Clear();
				ArrayValue.Clear();

				/* Top-Data Store-Force */
				ArrayFrameNo.Add(0);
				ArrayIndex.Add(0);
				ArrayValue.Add(ListValue[0]);

				/* Extructing Changing Point */
				int Count = ListValue.Length;
				int CountArray = -1;
				int IndexExist = -1;
				for(int i=1; i<Count; i++)
				{
					if(true == ListValue[i].Equals(ListValue[i - 1]))
					{	/* Unchanging */
						continue;
					}

					CountArray = ArrayValue.Count;
					IndexExist = -1;
					for(int j=0; j<CountArray; j++)
					{
						if(true == ListValue[i].Equals(ArrayValue[j]))
						{
							IndexExist = j;
							break;
						}
					}
					if(-1 == IndexExist)
					{	/* Data-New */
						ArrayValue.Add(ListValue[i]);
						ArrayFrameNo.Add(i);
						ArrayIndex.Add(ArrayValue.Count - 1);
					}
					else
					{	/* Data-Exist */
						ArrayFrameNo.Add(i);
						ArrayIndex.Add(IndexExist);
					}
				}

				/* Rebuilding Array */
				CountArray = ArrayFrameNo.Count;
				if(CountArray >= Count)
				{	/* All value is Changing-Point ... Uncompress */
					ListStatus = new FlagBit[0];
					return(true);
				}
				ListStatus = new FlagBit[CountArray];
				for(int i=0; i<CountArray; i++)
				{
					ListStatus[i] = GetFlagBit((int)ArrayFrameNo[i], (int)ArrayIndex[i]);
				}
				ListValue = ArrayValue.ToArray();

				return(true);
			}

			public bool DecompressCPE(int CountFrame)
			{	/* MEMO: CPE(Changing-Point Extracting) */
				if(0 >= ListStatus.Length)
				{	/* Already Decompressed */
					return(true);
				}

				if(0 >= ListValue.Length)
				{	/* No Data */
					return(true);
				}

				_TypeValue[] ListValueNew = new _TypeValue[CountFrame];
				int Index;
				int FrameNoOriginDummy;
				for(int i=0; i<CountFrame; i++)
				{
					Index = IndexGetValue(out FrameNoOriginDummy, i);
					ListValueNew[i] = ListValue[Index];
				}
				ListStatus = new FlagBit[0];
				ListValue = ListValueNew;
				return(true);
			}
		}

		[System.Serializable]
		public class ListAttributeInt : ListAttribute_Base<int>
		{
		}
		[System.Serializable]
		public class ListAttributeFloat : ListAttribute_Base<float>
		{
		}
		[System.Serializable]
		public class ListAttributeVector2 : ListAttribute_Base<Vector2>
		{
		}
		[System.Serializable]
		public class ListAttributeVector3 : ListAttribute_Base<Vector3>
		{
		}
		[System.Serializable]
		public class ListAttributeStatus : ListAttribute_Base<Library_SpriteStudio.Data.AttributeStatus>
		{
		}
		[System.Serializable]
		public class ListAttributeColorBlend : ListAttribute_Base<Library_SpriteStudio.Data.AttributeColorBlend>
		{
		}
		[System.Serializable]
		public class ListAttributeVertexCorrection : ListAttribute_Base<Library_SpriteStudio.Data.AttributeVertexCorrection>
		{
		}
		[System.Serializable]
		public class ListAttributeCell : ListAttribute_Base<Library_SpriteStudio.Data.AttributeCell>
		{
		}
		[System.Serializable]
		public class ListAttributeUserData : ListAttribute_Base<Library_SpriteStudio.Data.AttributeUserData>
		{
		}
		[System.Serializable]
		public class ListAttributeInstance : ListAttribute_Base<Library_SpriteStudio.Data.AttributeInstance>
		{
		}
		[System.Serializable]
		public class ListAttributeEffect : ListAttribute_Base<Library_SpriteStudio.Data.AttributeEffect>
		{
		}
		[System.Serializable]
		public class ListAttributeIndexCellMapFix : ListAttribute_Base<int>
		{
		}
		[System.Serializable]
		public class ListAttributeCoordinateMeshFix : ListAttribute_Base<Library_SpriteStudio.Data.AttributeCoordinateMeshFix>
		{
		}
		[System.Serializable]
		public class ListAttributeColorBlendMeshFix : ListAttribute_Base<Library_SpriteStudio.Data.AttributeColorBlendMeshFix>
		{
		}
		[System.Serializable]
		public class ListAttributeUVMeshFix : ListAttribute_Base<Library_SpriteStudio.Data.AttributeUVMeshFix>
		{
		}

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

				CLEAR = PARTSIDNEXT
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
					return((FlagBit.PARTSIDNEXT == Data) ? (-1) : ((int)Data >> 16));
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
							&& (0 == string.Compare(Text, TargetData.Text))
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
							&& (0 == string.Compare(LabelStart, TargetData.LabelStart))
							&& (0 == string.Compare(LabelEnd, TargetData.LabelEnd))
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
				CLEAR = 0x00000000,
			}

			public FlagBit Flags;

			public void CleanUp()
			{
				Flags = FlagBit.CLEAR;
			}

			public void Duplicate(AttributeEffect Original)
			{
				Flags = Original.Flags;
			}

			public override bool Equals(System.Object Target)
			{
				if((null == Target) || (GetType() != Target.GetType()))
				{
					return(false);
				}

				AttributeEffect TargetData = (AttributeEffect)Target;
				return(((Flags == TargetData.Flags)) ? true : false);
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

		public static int NameCheckLabelReserved(string Name)
		{
			if(false == string.IsNullOrEmpty(Name))
			{
				for(int i=0; i<(int)Library_SpriteStudio.KindLabelAnimationReserved.TERMINATOR; i++)
				{
					if(0 == string.Compare(Name, Library_SpriteStudio.ListNameLabelAnimationReserved[i]))
					{
						return(i);
					}
				}
			}
			return(-1);
		}

		[System.Serializable]
		public class EmitterEffect
		{
			[System.Flags]
			public enum FlagBit
			{
				BASIC = 0x00000001,
				DELAY = 0x00000002,
				SEEDRANDOM = 0x00000004,
//				LOCKRANDOMSEED = 0x00000008,	/* Reserved */

				POSITION = 0x00000010,
//				POSITION_FLUCTUATION = 0x00000020,	/* Reserved */
				ROTATIONFLUCTUATION_START = 0x00000040,
				ROTATIONFLUCTUATION_END = 0x00000080,
				SCALE_START = 0x00000100,
				SCALE_END = 0x00000200,
				SPEED_START = 0x00000400,	/* Reserved */
				SPEED_END = 0x00000800,

				GRAVITY_DIRECTION = 0x00001000,
				GRAVITY_POINT = 0x00002000,
				TANGENTIALACCELATION = 0x00004000,
				TURNDIRECTION = 0x00008000,

				COLORVERTEX_START = 0x01000000,
				COLORVERTEX_END = 0x02000000,
				FADEALPHA = 0x04000000,

				CLEAR = 0x00000000
			}
			public FlagBit FlagData;

			public Library_SpriteStudio.KindColorOperationEffect KindBlendTarget;
			public int IndexCellMap;
			public int IndexCell;

			public float CountFramePerSecond;
			public float TimeInterval;
			public float TimeDurationEmitter;
			public AttributeEffectRangeFloat TimeDurationParticle;

			public float Priority;
			public int CountParticleMax;
			public int CountParticleEmit;

			public AttributeEffectRangeVector2 PositionStart;

			public AttributeEffectRangeFloat RotationStart;
			public AttributeEffectRangeFloat RotationFluctuation;
			public float RotationFluctuationRate;
			public float RotationFluctuationRateTime;

			public AttributeEffectRangeVector2 ScaleStart;
			public AttributeEffectRangeFloat ScaleStartRate;
			public AttributeEffectRangeVector2 ScaleEnd;
			public AttributeEffectRangeFloat ScaleEndRate;

			public AttributeEffectRangeFloat SpeedStart;
			public AttributeEffectRangeFloat SpeedEnd;

			public int RandomSeed;
			public float TimeDelay;
			public AttributeEffectRangeFloat RateTangentialAcceleration;
			public Vector2 GravityDirectional;
			public Vector2 GravityPointPosition;
			public float GravityPointPower;

			public AttributeEffectRangeColor ColorVertexStart;
			public AttributeEffectRangeColor ColorVertexEnd;
			public float AlphaRateStart;
			public float AlphaRateEnd;

			public AttributeEffectRangeFloat Angle;
			public bool FlagTurnDirection;

			public void CleanUp()
			{
				FlagData = FlagBit.CLEAR;

				IndexCellMap = -1;
				IndexCell = -1;
				KindBlendTarget = Library_SpriteStudio.KindColorOperationEffect.NON;

				CountFramePerSecond = 60.0f;
				TimeDurationEmitter = 30.0f;
				TimeInterval = 1.0f;

				Priority = 64.0f;
				CountParticleMax = 50;
				CountParticleEmit = 1;

				SpeedStart = new AttributeEffectRangeFloat();
				SpeedStart.Main = 5.0f;
				SpeedStart.Sub = 5.0f;
				TimeDurationParticle = new AttributeEffectRangeFloat();
				TimeDurationParticle.Main = 30.0f;
				TimeDurationParticle.Sub = 30.0f;
				Angle = new AttributeEffectRangeFloat();
				Angle.Main = 0.0f;
				Angle.Sub = 0.0f;	// 45.0f;

				RandomSeed = -1;	// 0;
				TimeDelay = 0;
				GravityDirectional = Vector2.zero;	// new Vector2(0.0f, -3.0f);
				PositionStart = new AttributeEffectRangeVector2();
				PositionStart.Main = new Vector2(0.0f, 0.0f);
				PositionStart.Sub = new Vector2(0.0f, 0.0f);
				SpeedEnd = new AttributeEffectRangeFloat();
				SpeedEnd.Main = 0.0f;
				SpeedEnd.Sub = 0.0f;
				RotationStart = new AttributeEffectRangeFloat();
				RotationStart.Main = 0.0f;
				RotationStart.Sub = 0.0f;
				RotationFluctuation = new AttributeEffectRangeFloat();
				RotationFluctuation.Main = 0.0f;
				RotationFluctuation.Sub = 0.0f;
				RotationFluctuationRate = 0.0f;
				RotationFluctuationRateTime = 1.0f;	// 0.75f;
				RateTangentialAcceleration = new AttributeEffectRangeFloat();
				RateTangentialAcceleration.Main = 0.0f;
				RateTangentialAcceleration.Sub = 0.0f;
				GravityPointPosition = new Vector2(0.0f, 0.0f);
				GravityPointPower = 0.0f;

				ColorVertexStart = new AttributeEffectRangeColor();
				ColorVertexStart.Main = new Color(1.0f, 1.0f, 1.0f, 1.0f);
				ColorVertexStart.Sub = new Color(1.0f, 1.0f, 1.0f, 1.0f);
				ColorVertexEnd = new AttributeEffectRangeColor();
				ColorVertexEnd.Main = new Color(1.0f, 1.0f, 1.0f, 1.0f);
				ColorVertexEnd.Sub = new Color(1.0f, 1.0f, 1.0f, 1.0f);

				ScaleStart = new AttributeEffectRangeVector2();
				ScaleStart.Main = new Vector2(1.0f, 1.0f);
				ScaleStart.Sub = new Vector2(1.0f, 1.0f);
				ScaleStartRate = new AttributeEffectRangeFloat();
				ScaleStartRate.Main = 1.0f;
				ScaleStartRate.Sub = 1.0f;
				ScaleEnd = new AttributeEffectRangeVector2();
				ScaleEnd.Main = new Vector2(1.0f, 1.0f);
				ScaleEnd.Sub = new Vector2(1.0f, 1.0f);
				ScaleEndRate = new AttributeEffectRangeFloat();
				ScaleEndRate.Main = 1.0f;
				ScaleEndRate.Sub = 1.0f;

				AlphaRateStart = 0.0f;	// 0.25f;
				AlphaRateEnd = 1.0f;	// 0.75f;
				FlagTurnDirection = false;
			}

			public void Copy(Library_SpriteStudio.Data.EmitterEffect Source)
			{
				FlagData = Source.FlagData;

				CountFramePerSecond = Source.CountFramePerSecond;
				IndexCellMap = Source.IndexCellMap;
				IndexCell = Source.IndexCell;
				KindBlendTarget = Source.KindBlendTarget;

				TimeInterval = Source.TimeInterval;
				TimeDurationEmitter = Source.TimeDurationEmitter;

				Priority = Source.Priority;
				CountParticleMax = Source.CountParticleMax;
				CountParticleEmit = Source.CountParticleEmit;

				SpeedStart.Main = Source.SpeedStart.Main;
				SpeedStart.Sub = Source.SpeedStart.Sub;
				TimeDurationParticle.Main = Source.TimeDurationParticle.Main;
				TimeDurationParticle.Sub = Source.TimeDurationParticle.Sub;
				Angle.Main = Source.Angle.Main;
				Angle.Sub = Source.Angle.Sub;

				RandomSeed = Source.RandomSeed;
				TimeDelay = Source.TimeDelay;
				GravityDirectional = Source.GravityDirectional;
				PositionStart.Main = Source.PositionStart.Main;
				PositionStart.Sub = Source.PositionStart.Sub;
				SpeedEnd.Main = Source.SpeedEnd.Main;
				SpeedEnd.Sub = Source.SpeedEnd.Sub;
				RotationStart.Main = Source.RotationStart.Main;
				RotationStart.Sub = Source.RotationStart.Sub;
				RotationFluctuation.Main = Source.RotationFluctuation.Main;
				RotationFluctuation.Sub = Source.RotationFluctuation.Sub;
				RotationFluctuationRate = Source.RotationFluctuationRate;
				RotationFluctuationRateTime = Source.RotationFluctuationRateTime;
				RateTangentialAcceleration.Main = Source.RateTangentialAcceleration.Main;
				RateTangentialAcceleration.Sub = Source.RateTangentialAcceleration.Sub;
				GravityPointPosition = Source.GravityPointPosition;
				GravityPointPower = Source.GravityPointPower;

				ColorVertexStart.Main = Source.ColorVertexStart.Main;
				ColorVertexStart.Sub = Source.ColorVertexStart.Sub;
				ColorVertexEnd.Main = Source.ColorVertexEnd.Main;
				ColorVertexEnd.Sub = Source.ColorVertexEnd.Sub;

				ScaleStart.Main = Source.ScaleStart.Main;
				ScaleStart.Sub = Source.ScaleStart.Sub;
				ScaleStartRate.Main = Source.ScaleStartRate.Main;
				ScaleStartRate.Sub = Source.ScaleStartRate.Sub;
				ScaleEnd.Main = Source.ScaleEnd.Main;
				ScaleEnd.Sub = Source.ScaleEnd.Sub;
				ScaleEndRate.Main = Source.ScaleEndRate.Main;
				ScaleEndRate.Sub = Source.ScaleEndRate.Sub;

				AlphaRateStart = Source.AlphaRateStart;
				AlphaRateEnd = Source.AlphaRateEnd;
				FlagTurnDirection = Source.FlagTurnDirection;
			}
		}
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
	}

	public class Control
	{
		[System.Serializable]
		public class Parts
		{
			[System.Flags]
			internal enum FlagBitStatus
			{
				VALID = 0x40000000,
				RUNNING = 0x20000000,
				REFRESH_INSTANCEUNDERCONTROL = 0x10000000,

				HIDEFORCE = 0x08000000,

				CHANGE_TRANSFORM_POSITION = 0x00100000,
				CHANGE_TRANSFORM_ROTATION = 0x00200000,
				CHANGE_TRANSFORM_SCALING = 0x00400000,

				INSTANCE_VALID = 0x00008000,
				INSTANCE_PLAYINDEPENDENT = 0x00004000,

				EFFECT_PLAYING = 0x000000800,

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
			internal GameObject InstanceGameObjectUnderControl;
			internal Script_SpriteStudio_Root InstanceRootUnderControl;
			internal Script_SpriteStudio_RootEffect InstanceRootUnderControlEffect;	/* Interim */
			internal int FrameNoPreviousUpdateUnderControl;

			public void CleanUp()
			{
//				Status =

				FlagHideForceInitial = false;

				DataParts = null;
//				DataAnimationParts =

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
				InstanceGameObjectUnderControl = null;
				InstanceRootUnderControl = null;
				InstanceRootUnderControlEffect = null;

//				FrameNoPreviousUpdateUnderControl =
				CleanUpWorkAreaAnimationSet();
			}

			private void CleanUpWorkAreaAnimationSet()
			{
//				Status =

//				FlagHideForceInitial =

//				DataParts =
//				DataAnimationParts =

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
				InstanceGameObjectUnderControl = null;
				InstanceRootUnderControl = null;
				InstanceRootUnderControlEffect = null;

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
							Status |= FlagBitStatus.REFRESH_INSTANCEUNDERCONTROL;
							RebootPrefabInstance(InstanceRootInitial, IDPartsInitial);
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
							Status |= FlagBitStatus.REFRESH_INSTANCEUNDERCONTROL;
							RebootPrefabInstanceEffect(InstanceRootInitial, IDPartsInitial);
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
												int IDPartsInitial
											)
			{
				if(null != PrefabUnderControl)
				{
					/* Create UnderControl-Instance */
					if(0 != (Status & FlagBitStatus.REFRESH_INSTANCEUNDERCONTROL))
					{
						InstanceGameObjectUnderControl = Library_SpriteStudio.Miscellaneousness.Asset.PrefabInstantiateChild(InstanceGameObject, (GameObject)PrefabUnderControl, InstanceGameObjectUnderControl, true);
						Status &= ~FlagBitStatus.REFRESH_INSTANCEUNDERCONTROL;
					}
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
														int IDPartsInitial
													)
			{
				if(null != PrefabUnderControl)
				{
					/* Create UnderControl-Instance */
					if(0 != (Status & FlagBitStatus.REFRESH_INSTANCEUNDERCONTROL))
					{
						InstanceGameObjectUnderControl = Library_SpriteStudio.Miscellaneousness.Asset.PrefabInstantiateChild(InstanceGameObject, (GameObject)PrefabUnderControl, InstanceGameObjectUnderControl, true);
						Status &= ~FlagBitStatus.REFRESH_INSTANCEUNDERCONTROL;
					}
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

			internal bool UpdateGameObject(Script_SpriteStudio_Root InstanceRoot, int FrameNo)
			{
				int IndexAttribute;
				int FrameNoOrigin;

				/* Update Transform */
				/* MEMO: No Transform-Datas, Not Changing "Transform" */
				IndexAttribute = DataAnimationParts.Position.IndexGetValue(out FrameNoOrigin, FrameNo);
#if false
				if((0 <= IndexAttribute) && (IndexPreviousPosition != IndexAttribute))
				{
					InstanceTransform.localPosition = DataAnimationParts.Position.ListValue[IndexAttribute];
					IndexPreviousPosition = IndexAttribute;
				}
				IndexAttribute = DataAnimationParts.Rotation.IndexGetValue(out FrameNoOrigin, FrameNo);
				if((0 <= IndexAttribute) && (IndexPreviousRotation != IndexAttribute))
				{
					Quaternion QuaternionTemp = Quaternion.Euler(DataAnimationParts.Rotation.ListValue[IndexAttribute]);
					InstanceTransform.localRotation = QuaternionTemp;
					IndexPreviousRotation = IndexAttribute;
				}
				IndexAttribute = DataAnimationParts.Scaling.IndexGetValue(out FrameNoOrigin, FrameNo);
				if((0 <= IndexAttribute) && (IndexPreviousScaling != IndexAttribute))
				{
					InstanceTransform.localScale = DataAnimationParts.Scaling.ListValue[IndexAttribute];
					IndexPreviousScaling = IndexAttribute;
				}
#else
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
						InstanceTransform.localScale = DataAnimationParts.Scaling.ListValue[IndexAttribute];
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
#endif

				/* Status Get */
				IndexAttribute = DataAnimationParts.Status.IndexGetValue(out FrameNoOrigin, FrameNo);
				Library_SpriteStudio.Data.AttributeStatus DataStatus = (0 <= IndexAttribute) ? DataAnimationParts.Status.ListValue[IndexAttribute] : Library_SpriteStudio.Data.DummyStatus;
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
							IndexAttribute = DataAnimationParts.DataPlain.Cell.IndexGetValue(out FrameNoOrigin, FrameNo);
							if(0 <= IndexAttribute)
							{
								Library_SpriteStudio.Data.AttributeCell AttributeCell = DataAnimationParts.DataPlain.Cell.ListValue[IndexAttribute];
								BufferParameterParts.IndexCellMap = AttributeCell.IndexCellMap;
								int IndexCell = AttributeCell.IndexCell;
								Library_SpriteStudio.Data.CellMap DataCellMap = InstanceRoot.DataCellMap.DataGetCellMap(BufferParameterParts.IndexCellMap);
								if(null != DataCellMap)
								{
									BufferParameterParts.SizeTextureOriginal = DataCellMap.SizeOriginal;

									Library_SpriteStudio.Data.Cell DataCell = DataCellMap.DataGetCell(IndexCell);
									BufferParameterParts.DataCell = DataCell;
									if(null == DataCell)
									{	/* Invalid */
										BufferParameterParts.PivotMesh = Vector2.zero;
										BufferParameterParts.SizePixelMesh.x = 64.0f;
										BufferParameterParts.SizePixelMesh.y = 64.0f;
									}
									else
									{	/* Valid */
										BufferParameterParts.PivotMesh = DataCell.Pivot;
										BufferParameterParts.SizePixelMesh.x = DataCell.Rectangle.width;
										BufferParameterParts.SizePixelMesh.y = DataCell.Rectangle.height;
									}
								}
							}

							/* Recalc Mesh Size & Pivot (Considering SizeForce-X/Y & OffsetPivot-X/Y) */
							MeshRecalcSizeAndPivot(	ref BufferParameterParts.PivotMesh,
													ref BufferParameterParts.SizePixelMesh,
													ref BufferParameterParts.RateScaleMesh,
													FrameNo
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
								Vector2 DataSizeCollision = DataAnimationParts.DataFix.SizeCollision.ListValue[IndexAttribute];
								Vector2 DataPivotCollision = DataAnimationParts.DataFix.PivotCollision.ListValue[IndexAttributeCollisionPivot];
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

				int CountVertexData = InstanceParameterMesh.Coordinate.Length;
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
//						PivotTexture = new Vector2(RectCell.width * 0.5f, RectCell.height * 0.5f);
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
						Library_SpriteStudio.Data.AttributeCoordinateMeshFix CoordinateMeshFix = DataAnimationParts.DataFix.CoordinateMesh.ListValue[IndexAttribute];
						for(int i=0; i<CountVertexData; i++)
						{
							InstanceParameterMesh.Coordinate[i] = CoordinateMeshFix.Coordinate[i];
						}

						IndexAttribute = DataAnimationParts.DataFix.ColorBlendMesh.IndexGetValue(out FrameNoOrigin, FrameNo);
						Library_SpriteStudio.Data.AttributeColorBlendMeshFix ColorBlendMeshFix = DataAnimationParts.DataFix.ColorBlendMesh.ListValue[IndexAttribute];
						if((null != DataColorBlendOverwrite) && (KindColorOperation.NON != DataColorBlendOverwrite.Operation))
						{	/* Overwrite */
							float KindOperation = (float)DataColorBlendOverwrite.Operation + 0.01f;	/* "+0.01f" for Rounding-off-Error */
							Color ColorAverage = Color.clear;
							Color ColorData;
							for(int i=0; i<(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2; i++)
							{
								ColorData = DataColorBlendOverwrite.VertexColor[i];
								InstanceParameterMesh.ColorOverlay[i] = ColorData;
								ColorAverage += ColorData;
								InstanceParameterMesh.UV2[i] = ColorBlendMeshFix.UV[i];
								InstanceParameterMesh.UV2[i].x *= InstanceRoot.RateOpacity;
								InstanceParameterMesh.UV2[i].y = KindOperation;
							}
							ColorAverage *= (float)Library_SpriteStudio.KindVertexNo.TERMINATOR2;
							for(int i=(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2; i<CountVertexData; i++)
							{
								InstanceParameterMesh.ColorOverlay[i] = ColorAverage;
								InstanceParameterMesh.UV2[i] = ColorBlendMeshFix.UV[i];
								InstanceParameterMesh.UV2[i].x *= InstanceRoot.RateOpacity;
								InstanceParameterMesh.UV2[i].y = KindOperation;
							}
						}
						else
						{
							for(int i=0; i<CountVertexData; i++)
							{
								InstanceParameterMesh.ColorOverlay[i] = ColorBlendMeshFix.ColorOverlay[i];
								InstanceParameterMesh.UV2[i] = ColorBlendMeshFix.UV[i];
								InstanceParameterMesh.UV2[i].x *= InstanceRoot.RateOpacity;
							}
						}

						IndexAttribute = DataAnimationParts.DataFix.UV0Mesh.IndexGetValue(out FrameNoOrigin, FrameNo);
						Library_SpriteStudio.Data.AttributeUVMeshFix UVMeshFix = DataAnimationParts.DataFix.UV0Mesh.ListValue[IndexAttribute];
						for(int i=0; i<CountVertexData; i++)
						{
							InstanceParameterMesh.UV[i] = UVMeshFix.UV[i];
						}
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
				InstanceMesh.vertices = InstanceParameterMesh.Coordinate;
//				InstanceMesh.triangles = TrianglesMesh;
				InstanceMesh.uv = InstanceParameterMesh.UV;
				InstanceMesh.uv2 = InstanceParameterMesh.UV2;
				InstanceMesh.colors32 = InstanceParameterMesh.ColorOverlay;
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
					DataPartsDrawManager.PartsSetDraw(InstanceRoot, InstanceMaterial, KeyPriority);
				}
				return(true);
			}

			public void MeshRecalcSizeAndPivot(ref Vector2 Pivot, ref Vector2 Size, ref Vector2 RateScale, int FrameNo)
			{
				int FrameNoOrigin;
				int IndexAttribute;
				IndexAttribute = DataAnimationParts.DataPlain.OffsetPivot.IndexGetValue(out FrameNoOrigin, FrameNo);
				Vector2 PivotOffset = (0 <= IndexAttribute) ? DataAnimationParts.DataPlain.OffsetPivot.ListValue[IndexAttribute] : Vector2.zero;
				Pivot.x += (Size.x * PivotOffset.x) * RateScale.x;
				Pivot.y -= (Size.y * PivotOffset.y) * RateScale.y;

				/* Arbitrate Anchor-Size */
				IndexAttribute = DataAnimationParts.SizeForce.IndexGetValue(out FrameNoOrigin, FrameNo);
				if(0 <= IndexAttribute)
				{
					float RatePivot;
					Vector2 SizeForce = DataAnimationParts.SizeForce.ListValue[IndexAttribute];
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
				int FrameNoOrigin;
				int IndexAttribute;
				Library_SpriteStudio.Data.AttributeStatus DataStatus = null;
				bool FlagPlayReverse = InstanceRoot.StatusIsPlayingReverse;
				float TimeOffset = 0.0f;

				if((true == InstanceRoot.StatusIsPlayingTurn) && (0 == (Status & FlagBitStatus.INSTANCE_PLAYINDEPENDENT)))
				{
					FrameNoPreviousUpdateUnderControl = -1;
				}

				/* Status-Data Get */
				IndexAttribute = DataAnimationParts.Status.IndexGetValue(out FrameNoOrigin, FrameNo);
				DataStatus = (0 <= IndexAttribute) ? DataAnimationParts.Status.ListValue[IndexAttribute] : Library_SpriteStudio.Data.DummyStatus;

				/* Decode Instance-Data */
				if(true == InstanceRoot.StatusIsDecodeInstance)
				{
					/* Data Index (& Frame-No) Get */
					IndexAttribute = DataAnimationParts.Instance.IndexGetValue(out FrameNoOrigin, FrameNo);

					/* MEMO: Play-Foward */
					if(0 <= IndexAttribute)
					{	/* Valid Data */
						if(FrameNoPreviousUpdateUnderControl != FrameNoOrigin)
						{	/* Animation Set */
							Library_SpriteStudio.Data.AttributeInstance DataInstance = DataAnimationParts.Instance.ListValue[IndexAttribute];
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

							if(true == FlagPlayReverse)
							{	/* Reverse */
								if(FrameNoOrigin <= FrameNo)
								{	/* Immediately */
									TimeOffset = InstanceRoot.TimeElapsed - ((float)(FrameNoOrigin - InstanceRoot.FrameNoStart) * InstanceRoot.TimePerFrame);
									InstanceRootUnderControl.TimeElapsedSetForce(TimeOffset, true);
//									InstanceRootUnderControl.TimeDelayStart =
								}
								else
								{	/* Wait (Stop-Motion) */
									InstanceRootUnderControl.TimeElapsedSetForce(0.0f, true);
									InstanceRootUnderControl.TimeDelay = 0.0f;
									InstanceRootUnderControl.AnimationStop();
								}
							}
							else
							{	/* Foward */
								/* Wait Set */
								if(FrameNoOrigin <= FrameNo)
								{	/* Immediately */
									TimeOffset = InstanceRoot.TimeElapsed - ((float)(FrameNoOrigin - InstanceRoot.FrameNoStart) * InstanceRoot.TimePerFrame);
									InstanceRootUnderControl.TimeElapsedSetForce(TimeOffset, false);
									InstanceRootUnderControl.TimeDelay = 0.0f;
								}
								else
								{	/* Wait */
									TimeOffset = (float)FrameNoOrigin * InstanceRoot.TimePerFrame - InstanceRoot.TimeElapsed;
									InstanceRootUnderControl.TimeElapsedSetForce(0.0f, false);
									InstanceRootUnderControl.TimeDelay = TimeOffset;
								}
							}

							FrameNoPreviousUpdateUnderControl = FrameNoOrigin;
							Status = (0 != (DataInstance.Flags & Library_SpriteStudio.Data.AttributeInstance.FlagBit.INDEPENDENT)) ? (Status | FlagBitStatus.INSTANCE_PLAYINDEPENDENT) : (Status & ~FlagBitStatus.INSTANCE_PLAYINDEPENDENT);
						}
					}
				}

				/* Draw Instance */
				if((null != InstanceRootUnderControl) && (null != DataPartsDrawManager) && (false == DataStatus.IsHide))
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

				return(true);
			}

			internal bool UpdateEffect(Script_SpriteStudio_Root InstanceRoot, int FrameNo)
			{
				int FrameNoOrigin;
				int IndexAttribute;
				bool FlagInitializeForce = false;
//				bool FlagPlayReverse = (0 == (Status & FlagBitStatus.EFFECT_PLAYING)) ? true : false;
				bool FlagPlayReverse = InstanceRoot.StatusIsPlayingReverse;
				bool FlagDecodeEffect = InstanceRoot.StatusIsDecodeEffect;
				Library_SpriteStudio.Data.AttributeStatus DataStatus = null;

				if((true == InstanceRoot.StatusIsPlayingTurn) || ((true == InstanceRoot.StatusIsPlayingStart) && (true == FlagDecodeEffect)))
				{
					FlagInitializeForce = true;
					FrameNoPreviousUpdateUnderControl = (false == FlagPlayReverse) ? InstanceRoot.FrameNoStart : ((InstanceRoot.FrameNoEnd - InstanceRoot.FrameNoStart) + 1);
					Status &= ~FlagBitStatus.EFFECT_PLAYING;
				}

				/* Status-Data Get */
				IndexAttribute = DataAnimationParts.Status.IndexGetValue(out FrameNoOrigin, FrameNo);
				DataStatus = (0 <= IndexAttribute) ? DataAnimationParts.Status.ListValue[IndexAttribute] : Library_SpriteStudio.Data.DummyStatus;
				bool FlagHide = DataStatus.IsHide;

				/* Playing Control */
				if(null != InstanceRootUnderControlEffect)
				{
					if(false == FlagHide)
					{	/* Play */
						if((true == FlagInitializeForce) || (0 == (Status & FlagBitStatus.EFFECT_PLAYING)))
						{	/* Stopping */
							FrameNoPreviousUpdateUnderControl = FrameNoOrigin;

							Status |= FlagBitStatus.EFFECT_PLAYING;

							float RateSpeed = InstanceRoot.RateSpeed;	/* Always Plus */
							InstanceRootUnderControlEffect.TimePerFrame = InstanceRoot.TimePerFrame;
							InstanceRootUnderControlEffect.AnimationPlay(0, RateSpeed);

							float TimeOffset;
							if(false == FlagPlayReverse)
							{	/* Play-Foward */
								TimeOffset = InstanceRoot.TimeElapsed - ((float)FrameNoOrigin * InstanceRoot.TimePerFrame);
							}
							else
							{
								TimeOffset = ((float)FrameNoPreviousUpdateUnderControl * InstanceRoot.TimePerFrame) - InstanceRoot.TimeElapsed;
							}

#if false
							/* MEMO: Avoid the "Effect"'s malfunction in the time error of less than frame. */
							TimeOffset = Mathf.Floor(TimeOffset / InstanceRoot.TimePerFrame) * InstanceRoot.TimePerFrame;
#endif
							InstanceRootUnderControlEffect.TimeElapsedSetForce(TimeOffset, FlagPlayReverse);
						}
					}
					else
					{	/* Stop */
						if((true == FlagInitializeForce) || (0 != (Status & FlagBitStatus.EFFECT_PLAYING)))
						{	/* Playing */
							FrameNoPreviousUpdateUnderControl = FrameNoOrigin;

							Status &= ~FlagBitStatus.EFFECT_PLAYING;

							InstanceRootUnderControlEffect.AnimationStop();
						}
					}

					if((null != DataPartsDrawManager) && ((0 == (Status & FlagBitStatus.HIDEFORCE)) && (0 != (Status & FlagBitStatus.EFFECT_PLAYING))))
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

		internal class PoolEffectData<_TypeData>
			where _TypeData : class, new()
		{
			internal int CountMax;
			internal List<_TypeData> ListDataRunning;
			internal List<_TypeData> ListDataWaiting;
			internal _TypeData[] Data;

			internal _TypeData InstanceGetWaiting()
			{	/* MEMO: This Function is "Pop" (not Peek). */
				if(0 >= ListDataWaiting.Count)
				{
					return(null);
				}

				_TypeData InstanceData = ListDataWaiting[0];
				ListDataWaiting.RemoveAt(0);
				return(InstanceData);
			}

			internal bool InstanceSetRunning(_TypeData InstanceData)
			{
				if(CountMax <= ListDataRunning.Count)
				{
					return(false);
				}
				ListDataRunning.Add(InstanceData);
				return(true);
			}

			internal void InstanceSetWaiting(_TypeData InstanceData)
			{
				ListDataRunning.Remove(InstanceData);
				ListDataWaiting.Add(InstanceData);
			}

			internal void InstanceSetWaiting(ref int Index)
			{
				if(0 >= Index)
				{	/* Error */
					return;
				}
				Index--;
				_TypeData InstanceData = ListDataRunning[Index];
				ListDataRunning.RemoveAt(Index);
				ListDataWaiting.Add(InstanceData);
			}

			internal _TypeData InstancePeekRunning(ref int Index)
			{	/* MEMO: Use for Updating. */
				_TypeData InstanceData = null;
				if((null != ListDataRunning) && (Index < ListDataRunning.Count))
				{
					InstanceData = ListDataRunning[Index];
					Index++;
				}
				return(InstanceData);
			}
		}

		internal class PoolPartsEffect : PoolEffectData<PartsEffect>
		{
			internal int CountIndexEmitter;
			 
			internal PoolPartsEffect(int Count)
			{
				CountMax = Count;
				ListDataRunning = null;
				ListDataWaiting = null;
				Data = null;

				CountIndexEmitter = 0;
			}

			internal bool BootUp(Script_SpriteStudio_RootEffect InstanceRoot)
			{
				ListDataRunning = new List<PartsEffect>(CountMax);
				ListDataRunning.Clear();
				ListDataWaiting = new List<PartsEffect>(CountMax);
				ListDataWaiting.Clear();

				Data = new PartsEffect[CountMax];
				for(int i=0; i<CountMax; i++)
				{
					Data[i] = new PartsEffect();
					Data[i].BootUp(InstanceRoot);
					ListDataWaiting.Add(Data[i]);
				}

				return(true);
			}

			internal int PartsGenerate(PartsEffect InstancePartsParent, Script_SpriteStudio_RootEffect InstanceRoot)
			{
				int IDPartsParent = -1;
				Library_SpriteStudio.Data.EmitterEffect InstanceDataEmitterParticle = null;
				Library_SpriteStudio.Control.PartsEffect.FlagBitStatus FlagBitStatusRoot = Library_SpriteStudio.Control.PartsEffect.FlagBitStatus.CLEAR;

				if(null == InstancePartsParent)
				{	/* Has no parent ... Generate "Root-Emitter"s */
					InstanceDataEmitterParticle = null;
					IDPartsParent = 0;
					FlagBitStatusRoot = Library_SpriteStudio.Control.PartsEffect.FlagBitStatus.EMITTER_ROOT;
				}
				else
				{	/* Has parent ... Generate "Particle" + "(Sub-)Emitter"s */
					InstanceDataEmitterParticle = InstancePartsParent.DataEmitter;
					IDPartsParent = InstancePartsParent.DataParts.ID;
//					FlagBitStatusRoot = Library_SpriteStudio.Control.PartsEffect.FlagBitStatus.CLEAR;
				}

				Library_SpriteStudio.Data.PartsEffect InstanceDataPartsEffect = null;
				InstanceDataPartsEffect = InstanceRoot.DataEffect.DataGetParts(IDPartsParent);
				int[] ListIDParts = InstanceDataPartsEffect.ListIDChild;
				int CountEmit = ListIDParts.Length;
				int Count = 0;

				/* Generate "Particle" */
				Library_SpriteStudio.Control.PartsEffect InstanceControlPartsEffectParticle = null;
				bool FlagGenerateParticle = true;
				if(null != InstanceDataEmitterParticle)
				{
					InstanceControlPartsEffectParticle = InstanceGetWaiting();
					if(null != InstanceControlPartsEffectParticle)
					{
						FlagGenerateParticle = InstanceControlPartsEffectParticle.Awake(	Library_SpriteStudio.KindPartsEffect.PARTICLE,
																							InstanceDataEmitterParticle,
																							InstanceDataPartsEffect,
																							InstancePartsParent,
																							CountEmit,
																							InstancePartsParent.IndexEmitter,
																							InstanceRoot,
																							InstancePartsParent
																						);
						if(false == FlagGenerateParticle)
						{
							InstanceSetWaiting(InstanceControlPartsEffectParticle);
						}
						else
						{
							InstanceSetRunning(InstanceControlPartsEffectParticle);
							Count++;
						}
					}
				}

				/* Generate "(Sub-)Emitter"s */
				if(true == FlagGenerateParticle)
				{
					Library_SpriteStudio.Control.PartsEffect InstanceControlPartsEffect = null;
					Library_SpriteStudio.Data.EmitterEffect InstanceDataEmitterEffect = null;
					int IDParts;

					for(int i=0; i<CountEmit; i++)
					{
						InstanceControlPartsEffect = InstanceGetWaiting();
						if(null != InstanceControlPartsEffect)
						{
							IDParts = ListIDParts[i];
							InstanceDataPartsEffect = InstanceRoot.DataEffect.DataGetParts(IDParts);
							InstanceDataEmitterEffect = InstanceRoot.DataEffect.DataGetEmitter(InstanceDataPartsEffect.IndexEmitter);

							if(false == InstanceControlPartsEffect.Awake(	Library_SpriteStudio.KindPartsEffect.EMITTER,
																			InstanceDataEmitterEffect,
																			InstanceDataPartsEffect,
																			InstanceControlPartsEffectParticle,
																			0,
																			CountIndexEmitter,
																			InstanceRoot,
																			InstancePartsParent
																		)
								)
							{	/* Failure */
								InstanceSetWaiting(InstanceControlPartsEffect);
								InstanceControlPartsEffectParticle.CountSynchronousDecrease();
							}
							else
							{	/* Succeed */
								InstanceControlPartsEffect.Status |= FlagBitStatusRoot;
								InstanceSetRunning(InstanceControlPartsEffect);
								Count++;
								CountIndexEmitter++;
							}
						}
					}
				}

				return(Count);
			}

			internal bool PartsFlush()
			{
				ListDataRunning.Clear();
				ListDataWaiting.Clear();
				for(int i=0; i<CountMax; i++)
				{
					ListDataWaiting.Add(Data[i]);
				}
				return(true);
			}
		}
		internal class PartsEffect
		{
			[System.Flags]
			internal enum FlagBitStatus
			{
				RUNNING = 0x40000000,
				GETUP = 0x20000000,

				PARTICLE_WAITDELETE = 0x08000000,
				PARTICLE_TURNDIRECTION = 0x04000000,

				EMITTER_ROOT = 0x00008000,
				EMITTER_LOOP = 0x00004000,
				EMITTER_EMITTABLE = 0x00002000,

				CLEAR = 0x00000000,

				AREA_COMMON = 0x70000000,
				AREA_PARTICLE = 0x0fff0000,
				AREA_EMITTER = 0x0000ffff,
			}

			/* WorkArea: Common */
			internal FlagBitStatus Status;

			internal Script_SpriteStudio_RootEffect InstanceRootEffect = null;
			internal Library_SpriteStudio.KindPartsEffect Kind;
			internal Library_SpriteStudio.Data.EmitterEffect DataEmitter;
			internal Library_SpriteStudio.Data.PartsEffect DataParts;
			internal Library_SpriteStudio.Control.PartsEffect InstancePartsSynchronous;
			internal int CountEmitterSynchronous;

			internal float CountFramePerSecond;
			internal float TimeElapsed;
			internal float TimeDuration;

			internal Vector2 Position;
			internal Vector2 Scale;
			internal float Rotation;
			internal float Priority;
			internal float Speed;
			internal float SpeedStart;
			internal float SpeedEnd;
			internal Vector2 PositionStart;
			internal Vector2 VectorPosition;
			internal Vector2 Force;
			internal Vector2 ForceBase;
			internal Vector2 GravityDirectional;
			internal Vector2 GravityPoint;
			internal float RateTangentialAcceleration;
			internal Vector2 ScaleStart;
			internal Vector2 ScaleEnd;

			/* WorkArea: Particle */
			internal float RotationFluctuation;
			internal float RotationFluctuationStart;
			internal float RotationFluctuationEnd;
			internal float Direction;
			internal Color ColorVertex;
			internal Color ColorVertexStart;
			internal Color ColorVertexEnd;
			internal Library_SpriteStudio.Data.Cell InstanceDataCell;
			internal int IndexCellMap;
			internal Vector3[] CoordinateMesh = null;
			internal ParameterMeshEffect BufferParameterMesh = null;
			internal Library_SpriteStudio.ManagerDraw.DataParts DataPartsDrawManager = null;

			/* WorkArea: (Sub-)Emitter */
			internal Library_SpriteStudio.Utility.Random.Generator InstanceRandom = null;
			internal float TimeIntervalData;
			internal float TimeInterval;
			internal float TimeDelay;
			internal int IndexEmitter;
			internal int CountRemainEmit;
			internal int CountFramePrevious;

			internal void CleanUp()
			{
				Status = FlagBitStatus.CLEAR;

//				InstanceRootEffect = null;	/* Initialized in BootUp */
				Kind = Library_SpriteStudio.KindPartsEffect.NON;
				DataEmitter = null;
				DataParts = null;
				InstancePartsSynchronous = null;
				CountEmitterSynchronous = 0;

				CountFramePerSecond = 0.0f;
				TimeElapsed = 0.0f;
				TimeDuration = 0.0f;

				Position = Vector2.zero;
				Scale = Vector2.one;
				Rotation = 0.0f;
				Priority = 0.0f;
				Speed = 0.0f;
				SpeedStart = 0.0f;
				SpeedEnd = 0.0f;
				PositionStart = Vector2.zero;
				VectorPosition = Vector2.zero;
				Force = Vector2.zero;
				ForceBase = Vector2.zero;
				GravityDirectional = Vector2.zero;
				GravityPoint = Vector2.zero;
				RotationFluctuation = 0.0f;
				RotationFluctuationStart = 0.0f;
				RotationFluctuationEnd = 0.0f;
				Direction = 0.0f;
				RateTangentialAcceleration = 0.0f;
				ScaleStart = Vector2.one;
				ScaleEnd = Vector2.one;

				ColorVertex = Color.white;
				ColorVertexStart = Color.white;
				ColorVertexEnd = Color.white;
				IndexCellMap = -1;
				InstanceDataCell = null;
//				BufferParameterMesh = null;	/* Initialized in BootUp */
//				DataPartsDrawManager = null;	/* Initialized in BootUp */

//				InstanceRandom = null;	/* Initialized in BootUp */
//				CountParticleMax = 0;
//				CountParticleEmit = 0;
				TimeIntervalData = 0.0f;
				TimeInterval = 0.0f;
				TimeDelay = 0.0f;
				IndexEmitter = 0;
				CountRemainEmit = 0;
				CountFramePrevious = -1;
			}

			internal bool BootUp(Script_SpriteStudio_RootEffect InstanceRoot)
			{
				CleanUp();

				InstanceRootEffect = InstanceRoot;

				InstanceRandom = Script_SpriteStudio_RootEffect.InstanceCreateRandom();

				CoordinateMesh = new Vector3[(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2];

				BufferParameterMesh = new ParameterMeshEffect();
				BufferParameterMesh.BootUp();

				DataPartsDrawManager = new Library_SpriteStudio.ManagerDraw.DataParts();
				DataPartsDrawManager.BootUp(InstanceRoot, Library_SpriteStudio.KindParts.NORMAL_TRIANGLE2, InstanceRoot.gameObject);

				return(true);
			}

			internal bool Awake(	Library_SpriteStudio.KindPartsEffect KindParts,
									Library_SpriteStudio.Data.EmitterEffect InstanceDataEmitter,
									Library_SpriteStudio.Data.PartsEffect InstanceDataParts,
									PartsEffect InstancePartsEffectSynchronousParticle,
									int CountSynchronousEmitter,
									int IndexInstanceEmitter,
									Script_SpriteStudio_RootEffect InstanceRoot,
									PartsEffect InstancePartsParent
								)
			{
				CleanUp();

				if(null == InstanceDataEmitter)
				{	/* No-Datas */
					return(false);
				}

				Kind = KindParts;
				DataEmitter = InstanceDataEmitter;
				DataParts = InstanceDataParts;
				InstancePartsSynchronous = InstancePartsEffectSynchronousParticle;
				CountEmitterSynchronous = CountSynchronousEmitter;
				IndexEmitter = IndexInstanceEmitter;
				float TimePerFrame;
				if((null != InstanceRoot) && (null != InstanceRoot.InstanceRootParent))
				{
					TimePerFrame = InstanceRoot.InstanceRootParent.TimePerFrame;
					CountFramePerSecond = 1.0f / TimePerFrame;
				}
				else
				{
					CountFramePerSecond = DataEmitter.CountFramePerSecond;
					TimePerFrame = 1.0f / CountFramePerSecond;
				}

				Library_SpriteStudio.Utility.Random.Generator InstanceRandomParent = null;
				if(null == InstancePartsParent)
				{	/* Create by Root ... Not-Move */
					InstanceRandomParent = InstanceRoot.InstanceRandom;
				}
				else
				{	/* Create by Parts */
					InstanceRandomParent = InstancePartsParent.InstanceRandom;
				}

				Library_SpriteStudio.Data.EmitterEffect.FlagBit FlagData = DataEmitter.FlagData;
				switch(Kind)
				{
					case KindPartsEffect.PARTICLE:
						goto Awake_Particle;

					case KindPartsEffect.EMITTER:
						goto Awake_Emitter;

					default:
						return(false);
				}
			Awake_End:;
				Status |= FlagBitStatus.RUNNING;
				Status &= ~FlagBitStatus.GETUP;
				return(true);

			Awake_Particle:;
				/* Set Data-"Perticle" */
				Priority = DataEmitter.Priority + ((float)IndexEmitter * 0.0001f);

				PositionStart = InstancePartsParent.Position;
				Position = PositionStart;
				Scale = Vector2.one;

				ColorVertexStart = Color.white;
				ColorVertexEnd = Color.white;
				ColorVertex = ColorVertexStart;

				TimeDuration = RandomGetRange(	InstanceRandomParent,
												(DataEmitter.TimeDurationParticle.Main * TimePerFrame),
												(DataEmitter.TimeDurationParticle.Sub * TimePerFrame)
											);
				float temp_angle = RandomGetRangeFin(	InstanceRandomParent,
														DataEmitter.Angle.Main,	// + (eAngle = 0.0f),
														DataEmitter.Angle.Sub
													);

				float angle_rad = Mathf.Deg2Rad * (temp_angle + 90.0f);
				VectorPosition.x = Mathf.Cos(angle_rad);
				VectorPosition.y = Mathf.Sin(angle_rad);
				VectorPosition *= CountFramePerSecond;	/* /= TimePerFrame */

				SpeedStart = RandomGetRange(	InstanceRandomParent,
												DataEmitter.SpeedStart.Main,
												DataEmitter.SpeedStart.Sub
											);
				SpeedEnd = SpeedStart;
				Speed = SpeedStart;

				Force = Vector2.zero;
				Direction = 0.0f;
				Status &= ~FlagBitStatus.PARTICLE_TURNDIRECTION;

				RotationFluctuation = 0.0f;
				RotationFluctuationStart = 0.0f;
				RotationFluctuationEnd = 0.0f;

				if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.GRAVITY_DIRECTION))
				{
					GravityDirectional = DataEmitter.GravityDirectional;
				}
				if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.POSITION))
				{
					Position.x = RandomGetRange(	InstanceRandomParent,
													DataEmitter.PositionStart.Main.x,
													DataEmitter.PositionStart.Sub.x
												);
					Position.y = RandomGetRange(	InstanceRandomParent,
													DataEmitter.PositionStart.Main.y,
													DataEmitter.PositionStart.Sub.y
												);
					Position += PositionStart;
				}
				if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.ROTATIONFLUCTUATION_START))
				{
					Rotation = RandomGetRange(	InstanceRandomParent,
												DataEmitter.RotationStart.Main,
												DataEmitter.RotationStart.Sub
											);
					RotationFluctuationStart = RandomGetRange(	InstanceRandomParent,
																DataEmitter.RotationFluctuation.Main,
																DataEmitter.RotationFluctuation.Sub
															);
					RotationFluctuationEnd = RotationFluctuationStart;
					RotationFluctuation = RotationFluctuationStart;
				}
				if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.ROTATIONFLUCTUATION_END))
				{
					RotationFluctuationEnd = RotationFluctuationStart * DataEmitter.RotationFluctuationRate;
				}
				RotationFluctuationEnd *= CountFramePerSecond;
				RotationFluctuationStart *= CountFramePerSecond;
				RotationFluctuation *= CountFramePerSecond;
				if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.SPEED_END))
				{
					SpeedEnd = RandomGetRange(	InstanceRandomParent,
												DataEmitter.SpeedEnd.Main,
												DataEmitter.SpeedEnd.Sub
											);
				}
				if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.TANGENTIALACCELATION))
				{
					RateTangentialAcceleration = RandomGetRange(	InstanceRandomParent,
																	DataEmitter.RateTangentialAcceleration.Main,
																	DataEmitter.RateTangentialAcceleration.Sub
																);
				}
				if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.COLORVERTEX_START))
				{
					RandomGetRangeColor(	InstanceRandomParent,
											ref ColorVertexStart,
											ref DataEmitter.ColorVertexStart.Main,
											ref DataEmitter.ColorVertexStart.Sub
										);
					ColorVertex = ColorVertexStart;
				}
				if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.COLORVERTEX_END))
				{
					RandomGetRangeColor(	InstanceRandomParent,
											ref ColorVertexEnd,
											ref DataEmitter.ColorVertexEnd.Main,
											ref DataEmitter.ColorVertexEnd.Sub
										);
				}
//				if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.FADEALPHA))
				if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.SCALE_START))
				{
					ScaleStart.x = RandomGetRange(	InstanceRandomParent,
													DataEmitter.ScaleStart.Main.x,
													DataEmitter.ScaleStart.Sub.x
												);
					ScaleStart.y = RandomGetRange(	InstanceRandomParent,
													DataEmitter.ScaleStart.Main.y,
													DataEmitter.ScaleStart.Sub.y
												);
					float Rate = RandomGetRange(	InstanceRandomParent,
													DataEmitter.ScaleStartRate.Main,
													DataEmitter.ScaleStartRate.Sub
												);
					ScaleStart *= Rate;
					Scale = ScaleStart;
				}
				if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.SCALE_END))
				{
					ScaleEnd.x = RandomGetRange(	InstanceRandomParent,
													DataEmitter.ScaleEnd.Main.x,
													DataEmitter.ScaleEnd.Sub.x
												);
					ScaleEnd.y = RandomGetRange(	InstanceRandomParent,
													DataEmitter.ScaleEnd.Main.y,
													DataEmitter.ScaleEnd.Sub.y
												);
					float Rate = RandomGetRange(	InstanceRandomParent,
													DataEmitter.ScaleEndRate.Main,
													DataEmitter.ScaleEndRate.Sub
												);
					ScaleEnd *= Rate;
				}

				if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.GRAVITY_POINT))
				{
					GravityPoint = Vector2.zero;
				}
				Status |= (true == DataEmitter.FlagTurnDirection) ? FlagBitStatus.PARTICLE_TURNDIRECTION : FlagBitStatus.CLEAR;

				InstanceDataCell = null;
				IndexCellMap = DataEmitter.IndexCellMap;
				Library_SpriteStudio.Data.CellMap InstanceDataCellMap = InstanceRoot.DataCellMap.DataGetCellMap(IndexCellMap);
				if(null != InstanceDataCellMap)
				{
					InstanceDataCell = InstanceDataCellMap.DataGetCell(DataEmitter.IndexCell);
					if(null != InstanceDataCell)
					{
						/* Calcurate Mesh-Base-Coordinate (Before Transform) */
						float PivotXCell = InstanceDataCell.Pivot.x;
						float PivotYCell = InstanceDataCell.Pivot.y;
						float CoordinateLUx = -PivotXCell;
						float CoordinateLUy = PivotYCell;
						float CoordinateRDx = InstanceDataCell.Rectangle.width - PivotXCell;
						float CoordinateRDy = -(InstanceDataCell.Rectangle.height - PivotYCell);
						CoordinateMesh[(int)Library_SpriteStudio.KindVertexNo.LU].x = CoordinateLUx;
						CoordinateMesh[(int)Library_SpriteStudio.KindVertexNo.LU].y = CoordinateLUy;
						CoordinateMesh[(int)Library_SpriteStudio.KindVertexNo.LU].z = 0.0f;
						CoordinateMesh[(int)Library_SpriteStudio.KindVertexNo.RU].x = CoordinateRDx;
						CoordinateMesh[(int)Library_SpriteStudio.KindVertexNo.RU].y = CoordinateLUy;
						CoordinateMesh[(int)Library_SpriteStudio.KindVertexNo.RU].z = 0.0f;
						CoordinateMesh[(int)Library_SpriteStudio.KindVertexNo.RD].x = CoordinateRDx;
						CoordinateMesh[(int)Library_SpriteStudio.KindVertexNo.RD].y = CoordinateRDy;
						CoordinateMesh[(int)Library_SpriteStudio.KindVertexNo.RD].z = 0.0f;
						CoordinateMesh[(int)Library_SpriteStudio.KindVertexNo.LD].x = CoordinateLUx;
						CoordinateMesh[(int)Library_SpriteStudio.KindVertexNo.LD].y = CoordinateRDy;
						CoordinateMesh[(int)Library_SpriteStudio.KindVertexNo.LD].z = 0.0f;

						/* Calcurate Fixed-Mesh-Data */
						Library_SpriteStudio.Control.ParameterMeshEffect InstanceParameterMesh = BufferParameterMesh;

						InstanceParameterMesh.Coordinate[(int)Library_SpriteStudio.KindVertexNo.LU] =
						InstanceParameterMesh.Coordinate[(int)Library_SpriteStudio.KindVertexNo.RU] =
						InstanceParameterMesh.Coordinate[(int)Library_SpriteStudio.KindVertexNo.RD] =
						InstanceParameterMesh.Coordinate[(int)Library_SpriteStudio.KindVertexNo.LD] = Vector3.zero;

						float SizeXTexture = InstanceDataCellMap.SizeOriginal.x;
						float SizeYTexture = InstanceDataCellMap.SizeOriginal.y;
						float CoordinateL = InstanceDataCell.Rectangle.xMin / SizeXTexture;
						float CoordinateR = InstanceDataCell.Rectangle.xMax / SizeXTexture;
						float CoordinateU = (SizeYTexture - InstanceDataCell.Rectangle.yMin) / SizeYTexture;
						float CoordinateD = (SizeYTexture - InstanceDataCell.Rectangle.yMax) / SizeYTexture;
						InstanceParameterMesh.UV[(int)Library_SpriteStudio.KindVertexNo.LU] = new Vector2(CoordinateL, CoordinateU);
						InstanceParameterMesh.UV[(int)Library_SpriteStudio.KindVertexNo.RU] = new Vector2(CoordinateR, CoordinateU);
						InstanceParameterMesh.UV[(int)Library_SpriteStudio.KindVertexNo.RD] = new Vector2(CoordinateR, CoordinateD);
						InstanceParameterMesh.UV[(int)Library_SpriteStudio.KindVertexNo.LD] = new Vector2(CoordinateL, CoordinateD);

						InstanceParameterMesh.UV2[(int)Library_SpriteStudio.KindVertexNo.LU].y =
						InstanceParameterMesh.UV2[(int)Library_SpriteStudio.KindVertexNo.RU].y =
						InstanceParameterMesh.UV2[(int)Library_SpriteStudio.KindVertexNo.RD].y =
						InstanceParameterMesh.UV2[(int)Library_SpriteStudio.KindVertexNo.LD].y = (float)KindColorOperation.NON + 0.01f;	/* Disuse */
					}
				}
				else
				{
					CleanUp();
					return(false);
				}

				Status &= ~FlagBitStatus.PARTICLE_WAITDELETE;

				goto Awake_End;

			Awake_Emitter:;
				/* Set Data-"Emitter" */
				CountEmitterSynchronous = 0;	/* Has no sub-emitter */

				FlagData = DataEmitter.FlagData;

				TimeDuration = DataEmitter.TimeDurationEmitter * TimePerFrame;
				TimeDelay = DataEmitter.TimeDelay * TimePerFrame;

				Status |= (0.0f >= TimeDuration) ? FlagBitStatus.EMITTER_LOOP : FlagBitStatus.CLEAR;
				TimeDuration += TimeDelay;

				TimeIntervalData = DataEmitter.TimeInterval * TimePerFrame;
				TimeInterval = TimeIntervalData;
				CountRemainEmit = DataEmitter.CountParticleMax;
				CountFramePrevious = -1;

				InstanceRandom = Script_SpriteStudio_RootEffect.InstanceCreateRandom();
				SeedSetRandom();
				if(0 == (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.SEEDRANDOM))
				{	/* MEMO: CAUTION. When have no data. */
					RandomTrash(InstanceRoot.PoolParts.ListDataRunning.Count % 9);
				}

				Status &= ~FlagBitStatus.EMITTER_EMITTABLE;

				goto Awake_End;
			}

			internal bool Update(ref int CountExecute, float TimeDelta, PoolPartsEffect InstancePoolParts, bool FlagPause)
			{
				bool FlagTimeUpdate = (true == FlagPause) ? false : true;
				TimeDelta = (0 == (Status & FlagBitStatus.GETUP)) ? 0.0f : TimeDelta;
				Status |= FlagBitStatus.GETUP;

				float RateDuration;
				Library_SpriteStudio.Data.EmitterEffect.FlagBit FlagData = DataEmitter.FlagData;
				switch(Kind)
				{
					case KindPartsEffect.PARTICLE:
						goto Update_Particle;

					case KindPartsEffect.EMITTER:
						goto Update_Emitter;

					default:
						break;
				}
				return(false);

			Update_Particle:;
				RateDuration = Mathf.Clamp((TimeElapsed / TimeDuration), 0.0f, 1.0f);

				if(true == FlagTimeUpdate)
				{
					/* "Particle" Update */
					if(0 != (Status & FlagBitStatus.PARTICLE_WAITDELETE))
					{
						goto Update_Particle_CheckSynchronous;
					}

					TimeElapsed += TimeDelta;
					if((0.0f > TimeElapsed) || (TimeDuration < TimeElapsed))
					{
						Status |= FlagBitStatus.PARTICLE_WAITDELETE;
						goto Update_Particle_CheckSynchronous;
					}
					else
					{
						/* Update Deltas */
						/* MEMO: Reflecting "RotationFluctuation" is 1-timing delay?? */
						Rotation += RotationFluctuation * TimeDelta;

						Vector2 Radial = Position;
						Radial.Normalize();
						Vector2 TangentialAcceleration = Radial;
						Radial *= 0.0f;	/* _radialAccel */
						float FloatTemp = TangentialAcceleration.x;
						TangentialAcceleration.x = -TangentialAcceleration.y;
						TangentialAcceleration.y = FloatTemp;
						TangentialAcceleration = TangentialAcceleration * RateTangentialAcceleration;
						Force = Radial + TangentialAcceleration;

//						if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.SEEDRANDOM))
//						if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.DELAY))
						if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.GRAVITY_DIRECTION))
						{
							GravityDirectional = DataEmitter.GravityDirectional;
							float TimePowerBase = TimeElapsed * CountFramePerSecond;
							GravityDirectional *= TimePowerBase;
						}
//						if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.POSITION))
//						if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.ROTATIONFLUCTUATION_START))
						if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.ROTATIONFLUCTUATION_END))
						{
							float RotationTimeRate = DataEmitter.RotationFluctuationRateTime;
#if false
							float RateRotation = (0.0f < RotationTimeRate) ? (RateDuration / RotationTimeRate) : 1.0f;
#else
							float RateRotation = RateDuration / RotationTimeRate;
							RateRotation = ((true == float.IsNaN(RateRotation)) || (true == float.IsInfinity(RateRotation))) ? 1.0f : Mathf.Clamp01(RateRotation);
#endif
							RotationFluctuation = Mathf.Lerp(RotationFluctuationStart, RotationFluctuationEnd, RateRotation);
						}
						if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.SPEED_END))
						{
							Speed = Mathf.Lerp(SpeedStart, SpeedEnd, RateDuration);
						}
						else
						{
							Speed = SpeedStart;
						}
//						if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.TANGENTIALACCELATION))
//						if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.COLORVERTEX_START))
						if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.COLORVERTEX_END))
						{
							ColorVertex = Color.Lerp(ColorVertexStart, ColorVertexEnd, RateDuration);
						}
//						if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.SCALE_START))
						if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.SCALE_END))
						{
							Scale = Vector3.Lerp(ScaleStart, ScaleEnd, RateDuration);
						}
						if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.GRAVITY_POINT))
						{
							Vector2 GravityPointNow = Vector2.zero;
							GravityPointNow.x = DataEmitter.GravityPointPosition.x + PositionStart.x;
							GravityPointNow.y = DataEmitter.GravityPointPosition.y + PositionStart.y;
							GravityPointNow -= Position;
							GravityPointNow.Normalize();
							GravityPointNow *= (DataEmitter.GravityPointPower * TimeDelta * CountFramePerSecond);
							GravityPoint += GravityPointNow;
						}

						Vector2 PositionOffset = (VectorPosition * Speed) + ((Force + GravityDirectional) * CountFramePerSecond);
						PositionOffset *= TimeDelta;
						PositionOffset += GravityPoint;

//						if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.TURNDIRECTION))
						if(0 != (Status & FlagBitStatus.PARTICLE_TURNDIRECTION))
						{
							Direction = (AngleGetCCW(Vector2.right, PositionOffset) * Mathf.Rad2Deg) - 90.0f;
						}
						else
						{
							Direction = 0.0f;
						}
						Position += PositionOffset;
					}
				}

				/* MEMO: "Alpha-Fade" always be updated (for Pausing) */
				float RateAlpha = 1.0f;
				if(0 != (FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.FADEALPHA))
				{
					float RateAlphaStart = DataEmitter.AlphaRateStart;
					float RateAlphaEnd = DataEmitter.AlphaRateEnd;
					if(RateAlphaStart > RateDuration)
					{
						RateAlpha = 1.0f - ((RateAlphaStart - RateDuration) / RateAlphaStart);
					}
					else
					{
						if(RateAlphaEnd < RateDuration)
						{
							if(1.0f <= RateAlphaEnd)
							{
								RateAlpha = 0.0f;
							}
							else
							{
								RateAlpha = 1.0f - ((RateDuration - RateAlphaEnd) / (1.0f - RateAlphaEnd));
							}
						}
					}
				}

				/* Transforming */
				Matrix4x4 MatrixTransform = Matrix4x4.TRS(Position, Quaternion.Euler(0.0f, 0.0f, (Rotation + Direction)), Scale);

				/* Transform & Set Parameter-Mesh */
				Library_SpriteStudio.Control.ParameterMeshEffect InstanceParameterMesh = BufferParameterMesh;

				InstanceParameterMesh.Coordinate[(int)Library_SpriteStudio.KindVertexNo.LU] = MatrixTransform.MultiplyPoint3x4(CoordinateMesh[(int)Library_SpriteStudio.KindVertexNo.LU]);
				InstanceParameterMesh.Coordinate[(int)Library_SpriteStudio.KindVertexNo.RU] = MatrixTransform.MultiplyPoint3x4(CoordinateMesh[(int)Library_SpriteStudio.KindVertexNo.RU]);
				InstanceParameterMesh.Coordinate[(int)Library_SpriteStudio.KindVertexNo.RD] = MatrixTransform.MultiplyPoint3x4(CoordinateMesh[(int)Library_SpriteStudio.KindVertexNo.RD]);
				InstanceParameterMesh.Coordinate[(int)Library_SpriteStudio.KindVertexNo.LD] = MatrixTransform.MultiplyPoint3x4(CoordinateMesh[(int)Library_SpriteStudio.KindVertexNo.LD]);

//				InstanceParameterMesh.UV[(int)Library_SpriteStudio.KindVertexNo.LU] =
//				InstanceParameterMesh.UV[(int)Library_SpriteStudio.KindVertexNo.RU] =
//				InstanceParameterMesh.UV[(int)Library_SpriteStudio.KindVertexNo.RD] =
//				InstanceParameterMesh.UV[(int)Library_SpriteStudio.KindVertexNo.LD] =

				InstanceParameterMesh.ColorVertex[(int)Library_SpriteStudio.KindVertexNo.LU] =
				InstanceParameterMesh.ColorVertex[(int)Library_SpriteStudio.KindVertexNo.RU] =
				InstanceParameterMesh.ColorVertex[(int)Library_SpriteStudio.KindVertexNo.RD] =
				InstanceParameterMesh.ColorVertex[(int)Library_SpriteStudio.KindVertexNo.LD] = ColorVertex;

				InstanceParameterMesh.UV2[(int)Library_SpriteStudio.KindVertexNo.LU].x =
				InstanceParameterMesh.UV2[(int)Library_SpriteStudio.KindVertexNo.RU].x =
				InstanceParameterMesh.UV2[(int)Library_SpriteStudio.KindVertexNo.RD].x =
				InstanceParameterMesh.UV2[(int)Library_SpriteStudio.KindVertexNo.LD].x = RateAlpha * InstanceRootEffect.RateOpacity;

//				InstanceParameterMesh.UV2[(int)Library_SpriteStudio.KindVertexNo.LU].y =
//				InstanceParameterMesh.UV2[(int)Library_SpriteStudio.KindVertexNo.RU].y =
//				InstanceParameterMesh.UV2[(int)Library_SpriteStudio.KindVertexNo.RD].y =
//				InstanceParameterMesh.UV2[(int)Library_SpriteStudio.KindVertexNo.LD].y =

				/* Update Mesh */
				Mesh InstanceMesh = DataPartsDrawManager.DrawParts.Data.InstanceMesh;
				InstanceMesh.uv = InstanceParameterMesh.UV;
				InstanceMesh.uv2 = InstanceParameterMesh.UV2;
				InstanceMesh.colors32 = InstanceParameterMesh.ColorVertex;
				InstanceMesh.vertices = InstanceParameterMesh.Coordinate;
//				InstanceMesh.triangles =

				/* Draw Mesh */
				Material InstanceMaterial = InstanceRootEffect.MaterialGet(IndexCellMap, DataEmitter.KindBlendTarget);
				if((null != InstanceMaterial) && (null != DataPartsDrawManager))
				{
					/* Set to Parts-Cluster */
					DataPartsDrawManager.DrawParts.Data.InstanceRoot = InstanceRootEffect;
					DataPartsDrawManager.PartsSetDraw(InstanceRootEffect, InstanceMaterial, Priority);
				}

				CountExecute++;
				return(true);

			Update_Particle_CheckSynchronous:;
				if(0 >= CountEmitterSynchronous)
				{	/* Delete */
					if(null != InstancePartsSynchronous)
					{
						InstancePartsSynchronous.CountSynchronousReturnParticle();
					}
					return(false);
				}

				CountExecute++;
				return(true);

			Update_Emitter:;
				int CountExecuteIncremental = 1;
				if(true == FlagTimeUpdate)
				{
					TimeElapsed += TimeDelta;
					TimeInterval += TimeDelta;
					int CountFrameNow = (int)(Mathf.Floor(TimeElapsed * CountFramePerSecond));

					bool FlagValid = ((0.0f > TimeElapsed) || (TimeDuration < TimeElapsed)) ? false : true;
					if(false == FlagValid)
					{
#if false
						/* MEMO: "Root-Emitter" is not Self-Destruction. */
						if(0 != (Status & FlagBitStatus.EMITTER_ROOT))
						{	/* Root-Emitter */
							/* MEMO: Not count in "Execute", when "Root-Emitter" is at rest. */
							CountExecuteIncremental = 0;
							goto Update_Emitter_End;
						}
#else
						/* MEMO: "Root-Emitter" is Self-Destruction. */
#endif
						if(null != InstancePartsSynchronous)
						{
							/* Reduce (parent's) Reference-Counter */
							InstancePartsSynchronous.CountSynchronousDecrease();
						}
						return(false);
					}

					if(null != InstancePartsSynchronous)
					{
						Position = InstancePartsSynchronous.Position;
					}

					Status = ((TimeDelay > TimeElapsed) || (false == FlagValid) || (CountFramePrevious == CountFrameNow)) ? (Status & ~FlagBitStatus.EMITTER_EMITTABLE) : (Status | FlagBitStatus.EMITTER_EMITTABLE);
					if(0 != (Status & FlagBitStatus.EMITTER_EMITTABLE))
					{
#if EFFECTUPDATE_CONFORMtoSS5_5
						/* MEMO: Conforms to SSBPLIB(SS5.5) */
						Update_PartsGenerate(InstancePoolParts);
#else
						/* MEMO: Original */
						TimeInterval += TimeDelta;
						while(TimeInterval >= TimeIntervalData)
						{
							TimeInterval -= TimeIntervalData;

							int CountEmitMax = DataEmitter.CountParticleEmit;
							int CountEmitNow = CountRemainEmit;
							CountEmitNow = (CountEmitMax < CountEmitNow) ? CountEmitMax : CountEmitNow;
							for(int i=0; i<CountEmitNow; i++)
							{
								InstancePoolParts.PartsGenerate(this, InstanceRootEffect);
								CountRemainEmit--;
							}
						}
#endif
						CountFramePrevious = CountFrameNow;
					}
				}
				/* Fall-Through */
//			Update_Emitter_End:;
				CountExecute += CountExecuteIncremental;
				return(true);
			}
#if EFFECTUPDATE_CONFORMtoSS5_5
			/* MEMO: Conforms to SSBPLIB(SS5.5) */
			private bool Update_PartsGenerate(PoolPartsEffect InstancePoolParts)
			{
				int CountCreate = DataEmitter.CountParticleEmit;
				CountCreate = (0 >= CountCreate) ? 1 : CountCreate;
				for( ; ; )
				{
					if(TimeInterval >= TimeIntervalData)
					{
						for(int i=0; i<CountCreate; i++)
						{
							if(0 < CountRemainEmit)
							{
								InstancePoolParts.PartsGenerate(this, InstanceRootEffect);
								CountRemainEmit--;
							}
							else
							{
								return(false);
							}
						}

						TimeInterval -= TimeIntervalData;
						if(0.0f >= TimeInterval)
						{
							return(true);
						}
					}
					else
					{
						return(true);
					}
				}
//				return(true);
			}
#else
			/* MEMO: Original */
#endif

			internal bool CountSynchronousDecrease()
			{
				if(0 >= CountEmitterSynchronous)
				{
					return(false);
				}
				CountEmitterSynchronous--;
				return((0 >= CountEmitterSynchronous) ? false : true);
			}
			internal void CountSynchronousReturnParticle()
			{
				CountRemainEmit++;
				if(DataEmitter.CountParticleMax < CountRemainEmit)
				{
					CountRemainEmit = DataEmitter.CountParticleMax;
				}
			}

			private static int SeedMakeID = 123456;
			private int SeedGenerateRandom()
			{
				SeedMakeID++;
				return((int)Time.realtimeSinceStartup + SeedMakeID);
			}
			private void SeedSetRandom()
			{
				int SeedRandom = DataEmitter.RandomSeed;
				if(-1 == DataEmitter.RandomSeed)
				{	/* Generate Seed */
					SeedRandom = SeedGenerateRandom();
				}
				InstanceRandom.InitSeed((uint)SeedRandom);
			}
			private void RandomTrash(int Count)
			{	/* Trash Random */
				for(int i=0; i<Count; i++)
				{
					InstanceRandom.RandomUint32();
				}
			}

			private static float FloatExtractFractionUint(uint Value)
			{
#if false
				/* MEMO: Depends on the "IEEE-754" */
				byte[] Buffer = System.BitConverter.GetBytes((Value >> 9) | 0x3f800000);	/* IEEE-754: Exponent-Part=127 (Exponent=0) */
				return((System.BitConverter.ToSingle(Buffer, 0)) - 1.0f);
#else
				return((float)((Value >> 9) & 0x007fffff) * (1.0f / 8388607.0f));
#endif
			}
			private static float RandomGetRange(	Library_SpriteStudio.Utility.Random.Generator GeneratorRandom,
													float ValueBase,
													float Range
				)
			{
				uint ValueRandom = GeneratorRandom.RandomUint32();
				return(ValueBase + (Range * FloatExtractFractionUint(ValueRandom)));
			}
			private static float RandomGetRangeFin(	Library_SpriteStudio.Utility.Random.Generator GeneratorRandom,
													float ValueBase,
													float Range
				)
			{
				uint ValueRandom = GeneratorRandom.RandomUint32();
				return(ValueBase + ((Range * FloatExtractFractionUint(ValueRandom)) - (Range * 0.5f)));
			}
			private static void RandomGetRangeColor(	Library_SpriteStudio.Utility.Random.Generator GeneratorRandom,
														ref Color Output,
														ref Color ValueBase,
														ref Color Range
				)
			{
				Output.a = RandomGetRange(GeneratorRandom, ValueBase.a, Range.a);
				Output.r = RandomGetRange(GeneratorRandom, ValueBase.r, Range.r);
				Output.g = RandomGetRange(GeneratorRandom, ValueBase.g, Range.g);
				Output.b = RandomGetRange(GeneratorRandom, ValueBase.b, Range.b);
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

		internal class ParameterMeshEffect
		{
			internal Vector3[] Coordinate;
			internal Color32[] ColorVertex;
			internal Vector2[] UV;
			internal Vector2[] UV2;

			internal void CleanUp()
			{
				Coordinate = null;
				ColorVertex = null;
				UV = null;
				UV2 = null;
			}

			internal bool BootUp()
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

				return(true);

			BootUp_ErrorEnd:;
				CleanUp();
				return(false);
			}
		}

		public class ColorBlendOverwrite
		{
			public Library_SpriteStudio.KindColorBound Bound;
			public Library_SpriteStudio.KindColorOperation Operation;
			public Color[] VertexColor;

			public	ColorBlendOverwrite()
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
						{	/* Interrupt ("Interrupt","Effect" and Another-Material-Sprite) ... not attach Draw-Parts' bottom */
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

		internal static void MeshCreate(	TerminalClusterDrawParts ClusterTerminal,
											Mesh InstanceMesh,
											MeshRenderer InstanceMeshRenderer,
											MeshFilter InstanceMeshFilter,
											Transform InstanceTrasnformDrawManager,
											Camera InstanceCamera
										)
		{
			/* FragmentClusterDrawParts */	Library_SpriteStudio.Miscellaneousness.Chain<DataClusterDrawParts>.Fragment ClusterNow = null;
			/* FragmentDrawParts */	Library_SpriteStudio.Miscellaneousness.Chain<DataDrawParts>.Fragment DataPartsNow = null;
			int Index;

			/* Count Mesh & Material-Table Create */
			int CountMaterial = ClusterTerminal.CountChain;
			if(0 >= CountMaterial)
			{
				goto MeshCreate_MeshNoDraw;
			}

			int CountMesh = 0;
			Material[] TableMaterial = new Material[CountMaterial];
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

			InstanceMeshRenderer.sharedMaterials = TableMaterial;

			/* Create Combined Mesh */
			Matrix4x4 MatrixCollect = (null != InstanceTrasnformDrawManager) ? InstanceTrasnformDrawManager.localToWorldMatrix.inverse : Matrix4x4.identity;

			CombineInstance[] CombineMesh = new CombineInstance[CountMesh];
//			int[] TableIndexVertex = new int[CountMaterial];
			int[] TableIndexTriangle = new int[CountMaterial + 1];	/* +1 ... Total Data */
			DataPartsNow = null;
			ClusterNow = ClusterTerminal.ChainTop;
			Index = 0;
			int IndexVertex = 0;
			int IndexTriangle = 0;
			for(int i=0; i<CountMaterial; i++)
			{
				DataPartsNow = ClusterNow.Data.ChainDrawParts.ChainTop;
//				TableIndexVertex[i] = IndexVertex;
				TableIndexTriangle[i] = IndexTriangle;

				while(null != DataPartsNow)
				{
					if(null != DataPartsNow.Data.InstanceMesh)
					{
						CombineMesh[Index].mesh = DataPartsNow.Data.InstanceMesh;
						CombineMesh[Index].transform = MatrixCollect * DataPartsNow.Data.InstanceTransform.localToWorldMatrix;
						Index++;
#if false
						IndexVertex += DataPartsNow.Data.InstanceMesh.vertices.Length;
						IndexTriangle += DataPartsNow.Data.InstanceMesh.triangles.Length / 3;
#else
						IndexVertex += DataPartsNow.Data.InstanceMesh.vertexCount;
						IndexTriangle += (KindParts.NORMAL_TRIANGLE4 == DataPartsNow.Data.Kind) ? 4 : 2;    /* ArrayCoordinate_TriangleX.Length / 3 */
#endif
					}

					DataPartsNow = DataPartsNow.ChainNext;
				}
				ClusterNow = ClusterNow.ChainNext;
			}
			TableIndexTriangle[CountMaterial] = IndexTriangle;
			InstanceMesh.CombineMeshes(CombineMesh);

			/* SubMesh Construct */
			int[] TriangleBuffer = InstanceMesh.triangles;
			int[] VertexNoTriangle = null;
			InstanceMesh.triangles = null;
			InstanceMesh.subMeshCount = CountMaterial;
			for(int i=0; i<CountMaterial; i++)
			{
				CountMesh = TableIndexTriangle[i + 1] - TableIndexTriangle[i];
				VertexNoTriangle = new int[CountMesh * 3];
				for(int j=0; j<CountMesh; j++)
				{
					IndexTriangle = (j + TableIndexTriangle[i]) * 3;
					IndexVertex = j * 3;

					VertexNoTriangle[IndexVertex] = TriangleBuffer[IndexTriangle];
					VertexNoTriangle[IndexVertex + 1] = TriangleBuffer[IndexTriangle + 1];
					VertexNoTriangle[IndexVertex + 2] = TriangleBuffer[IndexTriangle + 2];
				}
				InstanceMesh.SetTriangles(VertexNoTriangle, i);
			}

			InstanceMesh.name = "BatchedMesh";
			InstanceMeshFilter.sharedMesh = InstanceMesh;

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

			/* WorkArea: DrawManager */
			public Script_SpriteStudio_ManagerDraw InstanceManagerDraw;
			internal Library_SpriteStudio.ManagerDraw.FragmentDrawObject DrawObject = null;
			internal Library_SpriteStudio.ManagerDraw.TerminalClusterDrawParts ChainClusterDrawParts = null;
			internal int CountPartsDraw;

			/* Relation Datas */
			internal Script_SpriteStudio_Root InstanceRootParent = null;
			internal Transform InstanceTransform = null;

			/* Playing Datas */
			public bool FlagHideForce;
			public float RateSpeed = 1.0f;

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

				if(null != InstanceGameObjectOld)
				{
					Object.DestroyImmediate(InstanceGameObjectOld);
					InstanceGameObjectOld = null;
				}

				GameObject InstanceGameObject = null;
				Transform InstanceTransform = null;
				Transform InstanceTransformParent = InstanceGameObjectParent.transform;

				/* Get Child-Instance */
				InstanceTransform = InstanceTransformParent.Find(GameObjectPrefab.name);
				if((null == InstanceTransform) || (true == FlagInstanceUnderControlRenew))
				{
					/* Delete Old UnderControl-Instance (Same Name) */
					if(null != InstanceTransform)
					{
						Object.DestroyImmediate(InstanceTransform.gameObject);
					}

					/* Instantiate UnderControl-Instance */
#if UNITY_EDITOR
					InstanceGameObject = UnityEditor.PrefabUtility.InstantiatePrefab(GameObjectPrefab) as GameObject;
#else
					InstanceGameObject = Object.Instantiate(GameObjectPrefab) as GameObject;
					InstanceGameObject.name = GameObjectPrefab.name;	/* Remove "(clone)" */
#endif
					InstanceTransform = InstanceGameObject.transform;

					if(null != InstanceGameObjectParent)
					{
						InstanceTransform = InstanceGameObject.transform;
						InstanceTransform.parent = InstanceGameObjectParent.transform;
					}

					if(null != InstanceGameObject)
					{
						InstanceTransform.localPosition = Vector3.zero;
						InstanceTransform.localEulerAngles = Vector3.zero;
						InstanceTransform.localScale = Vector3.one;
					}
				}
				InstanceGameObject = InstanceTransform.gameObject;

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
				double RandomDouble();
				int RandomN(int Limit);
			}
		}
	}
}
