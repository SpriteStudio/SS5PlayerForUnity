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
	public delegate void FunctionCallBackUserData(GameObject ObjectControl, string PartsName, Library_SpriteStudio.AnimationData AnimationDataParts, int AnimationNo, int FrameNoDecode, int FrameNoKeyData, Library_SpriteStudio.KeyFrame.ValueUser Data);
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

		public static AnimationData SpriteStudioDataGetParent(GameObject GameObjectParts)
		{
			GameObject GameObjectParent = GameObjectParts.transform.parent.gameObject;
			AnimationData DataSpriteStudio = null;

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

	public static class KeyFrame
	{
		[System.Serializable]
		public class ValueColor
		{
			public KindColorBound Bound;
			public KindColorOperation Operation;
			public Color[] VertexColor;
			public float[] RatePixelAlpha;

			public ValueColor()
			{
				Bound = KindColorBound.NON;
				Operation = KindColorOperation.MIX;
				VertexColor = new Color[(int)VertexNo.TERMINATOR4];
				RatePixelAlpha = new float[(int)VertexNo.TERMINATOR4];
				for(int i=0; i<VertexColor.Length; i++)
				{
					VertexColor[i] = Color.white;
					RatePixelAlpha[i] = 0.0f;
				}
			}
		}

		[System.Serializable]
		public class ValueQuadrilateral
		{
			public Vector2[] Coordinate;

			public ValueQuadrilateral()
			{
				Coordinate = new Vector2[(int)VertexNo.TERMINATOR2];
				for(int i=0; i<(int)VertexNo.TERMINATOR2; i++)
				{
					Coordinate[i] = Vector2.zero;
				}
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
				TextureNo = -1;
				Rectangle.x = 0.0f;
				Rectangle.y = 0.0f;
				Rectangle.width = 0.0f;
				Rectangle.height = 0.0f;
				Pivot = Vector2.zero;
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
					return(0 != (Flag & FlagData.NUMBER));
				}
			}
			public bool IsRectangle
			{
				get
				{
					return(0 != (Flag & FlagData.RECTANGLE));
				}
			}
			public bool IsCoordinate
			{
				get
				{
					return(0 != (Flag & FlagData.COORDINATE));
				}
			}
			public bool IsText
			{
				get
				{
					return(0 != (Flag & FlagData.TEXT));
				}
			}
			public int NumberInt;
			public uint Number
			{
				get
				{
					return((uint)NumberInt);
				}
			}
			public Rect Rectangle;
			public Vector2 Coordinate;
			public string Text;

			public ValueUser()
			{
				Flag = FlagData.CLEAR;
				NumberInt = 0;
				Rectangle.xMin = 0.0f;
				Rectangle.yMin = 0.0f;
				Rectangle.xMax = 0.0f;
				Rectangle.yMax = 0.0f;
				Coordinate = Vector2.zero;
				Text = "";
			}
		}

		[System.Serializable]
		public class ValueBools
		{
			public enum FlagData
			{
				CLEAR = 0x00000000,
				HIDE = 0x00000001,
				FLIPX = 0x00000010,
				FLIPY = 0x00000020,
				FLIPXTEXTURE = 0x00000100,
				FLIPYTEXTURE = 0x00000200,
			};

			public FlagData Flag;
			public bool IsHide
			{
				get
				{
					return(0 != (Flag & FlagData.HIDE));
				}
			}
			public bool IsFlipX
			{
				get
				{
					return(0 != (Flag & FlagData.FLIPX));
				}
			}
			public bool IsFlipY
			{
				get
				{
					return(0 != (Flag & FlagData.FLIPY));
				}
			}
			public bool IsTextureFlipX
			{
				get
				{
					return(0 != (Flag & FlagData.FLIPXTEXTURE));
				}
			}
			public bool IsTextureFlipY
			{
				get
				{
					return(0 != (Flag & FlagData.FLIPYTEXTURE));
				}
			}

			public ValueBools()
			{
				Flag = FlagData.CLEAR;
			}
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
	public class AnimationData
	{
		public int ID;
		public KindCollision CollisionKind;
		public Collider CollisionComponent;

		public KeyFrame.ValueBools[] AnimationDataFlags;

		public Vector3[] AnimationDataPosition;
		public Vector3[] AnimationDataRotation;
		public Vector2[] AnimationDataScaling;

		public float[] AnimationDataOpacityRate;
		public float[] AnimationDataPriority;

		public KeyFrame.ValueColor[] AnimationDataColorBlend;
		public KeyFrame.ValueQuadrilateral[] AnimationDataVertexCorrection;
		public Vector2[] AnimationDataOriginOffset;

		public Vector2[] AnimationDataAnchorPosition;
		public Vector2[] AnimationDataAnchorSize;

		public Vector2[] AnimationDataTextureTranslate;
		public float[] AnimationDataTextureRotate;
		public Vector2[] AnimationDataTextureScale;
		public Vector2[] AnimationDataTextureExpand;

		public float[] AnimationDataCollisionRadius;

		public KeyFrame.ValueCell[] AnimationDataCell;
		public KeyFrame.ValueUser[] AnimationDataUser;

		/* Buffer for Runtime-Speed-Optimize */
		private float ColliderRadiusPrevious = -1.0f;	/* for Radius-Collision */
		private Vector2 ColliderRectSizePrevious = Vector2.zero;	/* for Rectangle-Collision */
		private Vector2 ColliderRectPivotPrevious = Vector2.zero;	/* for Rectangle-Collision */

		public AnimationData()
		{
			ID = -1;
			CollisionKind = KindCollision.NON;
			CollisionComponent = null;

			AnimationDataFlags = null;

			AnimationDataPosition = null;
			AnimationDataRotation = null;
			AnimationDataScaling = null;

			AnimationDataOpacityRate = null;
			AnimationDataPriority = null;

			AnimationDataColorBlend = null;
			AnimationDataVertexCorrection = null;
			AnimationDataOriginOffset = null;

			AnimationDataAnchorPosition = null;
			AnimationDataAnchorSize = null;

			AnimationDataTextureTranslate = null;
			AnimationDataTextureRotate = null;
			AnimationDataTextureScale = null;
			AnimationDataTextureExpand = null;

			AnimationDataCollisionRadius = null;

			AnimationDataCell = null;
			AnimationDataUser = null;
		}

		public void UpdateUserData(int FrameNo, GameObject GameObjectNow, Script_SpriteStudio_PartsRoot ScriptRoot)
		{
			if(0 < AnimationDataUser.Length)
			{
				if(0 != (ScriptRoot.Status & Script_SpriteStudio_PartsRoot.BitStatus.DECODE_USERDATA))
				{
					int FrameNoPrevious = (-1 == ScriptRoot.FrameNoPrevious) ? FrameNo : ScriptRoot.FrameNoPrevious;
					KeyFrame.ValueUser UserData = null;

					/* Decoding Skipped Frame */
					if(0.0f > ScriptRoot.RateTimeAnimation)
					{	/* backwards */
						if(FrameNo > FrameNoPrevious)
						{	/* Loop */
							for(int i=(FrameNoPrevious - 1); i>=ScriptRoot.FrameNoStart; i--)
							{
								UserData = AnimationDataUser[i];
								if(Library_SpriteStudio.KeyFrame.ValueUser.FlagData.CLEAR != UserData.Flag)
								{
									ScriptRoot.CallBackExecUserData(GameObjectNow.name, this, i, UserData);
								}
							}
							for(int i=ScriptRoot.FrameNoEnd; i>FrameNo; i--)
							{
								UserData = AnimationDataUser[i];
								if(Library_SpriteStudio.KeyFrame.ValueUser.FlagData.CLEAR != UserData.Flag)
								{
									ScriptRoot.CallBackExecUserData(GameObjectNow.name, this, i, UserData);
								}
							}
						}
						else
						{	/* Normal */
							for(int i=(FrameNoPrevious - 1); i>FrameNo; i--)
							{
								UserData = AnimationDataUser[i];
								if(Library_SpriteStudio.KeyFrame.ValueUser.FlagData.CLEAR != UserData.Flag)
								{
									ScriptRoot.CallBackExecUserData(GameObjectNow.name, this, i, UserData);
								}
							}
						}
					}
					else
					{	/* foward */
						if(FrameNo < FrameNoPrevious)
						{	/* Loop */
							for(int i=(FrameNoPrevious + 1); i<=ScriptRoot.FrameNoEnd; i++)
							{
								UserData = AnimationDataUser[i];
								if(Library_SpriteStudio.KeyFrame.ValueUser.FlagData.CLEAR != UserData.Flag)
								{
									ScriptRoot.CallBackExecUserData(GameObjectNow.name, this, i, UserData);
								}
							}
							for(int i=ScriptRoot.FrameNoStart; i<FrameNo; i++)
							{
								UserData = AnimationDataUser[i];
								if(Library_SpriteStudio.KeyFrame.ValueUser.FlagData.CLEAR != UserData.Flag)
								{
									ScriptRoot.CallBackExecUserData(GameObjectNow.name, this, i, UserData);
								}
							}
						}
						else
						{	/* Normal */
							for(int i=(FrameNoPrevious + 1); i<FrameNo; i++)
							{
								UserData = AnimationDataUser[i];
								if(Library_SpriteStudio.KeyFrame.ValueUser.FlagData.CLEAR != UserData.Flag)
								{
									ScriptRoot.CallBackExecUserData(GameObjectNow.name, this, i, UserData);
								}
							}
						}
					}

					/* Decoding Just-Now Frame */
					UserData = AnimationDataUser[FrameNo];
					if(Library_SpriteStudio.KeyFrame.ValueUser.FlagData.CLEAR != UserData.Flag)
					{
						ScriptRoot.CallBackExecUserData(GameObjectNow.name, this, FrameNo, UserData);
					}
				}
			}
		}

		public bool UpdateGameObject(GameObject GameObjectNow, int FrameNo)
		{
			/* MEMO: No Transform-Datas, Not Changing "GameObject" */
			if((0 >= AnimationDataPosition.Length) && (0 >= AnimationDataRotation.Length) && (0 >= AnimationDataScaling.Length))
			{
				return(false);	/* Hide */
			}

			/* Transform Update */
			GameObjectNow.transform.localPosition = (0 < AnimationDataPosition.Length) ? AnimationDataPosition[FrameNo] : Vector3.zero;
			GameObjectNow.transform.localEulerAngles = (0 < AnimationDataRotation.Length) ? AnimationDataRotation[FrameNo] : Vector3.zero;
			Vector3 Scale = Vector3.one;
			if(0 < AnimationDataScaling.Length)
			{
				Scale.x = AnimationDataScaling[FrameNo].x;
				Scale.y = AnimationDataScaling[FrameNo].y;
			}
			GameObjectNow.transform.localScale = Scale;

			/* Collider-Setting */
			if(null != CollisionComponent)
			{
				switch(CollisionKind)
				{
					case KindCollision.SQUARE:
						{
							/* Calculate Sprite-Parts size */
							Vector2 SizeNew = Vector2.one;
							Vector2 PivotNew = Vector2.zero;
							{
								Rect RectCell = Rect.MinMaxRect(0.0f, 0.0f, 64.0f, 64.0f);
								if(0 < AnimationDataCell.Length)
								{
									RectCell = AnimationDataCell[FrameNo].Rectangle;
								}

								Vector2 RateScaleMesh = Vector2.one;
								if(0 < AnimationDataFlags.Length)
								{
									RateScaleMesh.x = (true == AnimationDataFlags[FrameNo].IsFlipX) ? -1.0f : 1.0f;
									RateScaleMesh.y = (true == AnimationDataFlags[FrameNo].IsFlipY) ? -1.0f : 1.0f;
								}

								/* Accommodate Pivot's-Offset */
								Vector2 PivotOffset = (0 < AnimationDataOriginOffset.Length) ? AnimationDataOriginOffset[FrameNo] : Vector2.zero;
								PivotNew.x += (RectCell.width * PivotOffset.x) * RateScaleMesh.x;
								PivotNew.y -= (RectCell.height * PivotOffset.y) * RateScaleMesh.y;

								/* Get Collision-Size */
								SizeNew.x = RectCell.width;
								SizeNew.y = RectCell.height;
							}

							if((PivotNew != ColliderRectPivotPrevious) || (SizeNew != ColliderRectSizePrevious))
							{	/* Update */
								/* Update Previous Buffer */
								ColliderRectPivotPrevious = PivotNew;
								ColliderRectSizePrevious = SizeNew;

								/* Update Collider */
								BoxCollider InstanceCollider = CollisionComponent as BoxCollider;
								InstanceCollider.enabled = true;
								InstanceCollider.size = ColliderRectSizePrevious;
								InstanceCollider.center = ColliderRectPivotPrevious;
							}
						}
						break;

					case KindCollision.CIRCLE:
						{
							float RadiusNew = (0 < AnimationDataCollisionRadius.Length) ? AnimationDataCollisionRadius[FrameNo] : 1.0f;
							if(RadiusNew != ColliderRadiusPrevious)
							{	/* Update */
								/* Update Previous Buffer */
								ColliderRadiusPrevious = RadiusNew;

								/* Update Collider */
								CapsuleCollider InstanceCollider = CollisionComponent as CapsuleCollider;
								InstanceCollider.enabled = true;
								InstanceCollider.radius = ColliderRadiusPrevious;
								InstanceCollider.center = Vector3.zero;
							}
						}
						break;

					default:
						break;
				}
			}

			/* Return-Value is "Inversed Hide-Flag"  */
			return(!AnimationDataFlags[FrameNo].IsHide);
		}
	}

	[System.Serializable]
	public class AnimationDataSprite : AnimationData
	{
		public KindColorOperation KindBlendTarget;

		public void UpdateMesh(Mesh MeshNow, int FrameNo, Script_SpriteStudio_PartsRoot ScriptRoot)
		{
			Matrix4x4 MatrixTexture = Matrix4x4.identity;
			Vector2 SizeTexture = Vector2.one;
			Vector2 RateScaleTexture = Vector2.one;
			Vector2 PivotTexture = Vector2.zero;
			Vector2 RateScaleMesh = Vector2.one;
			Vector2 PivotMesh = Vector2.zero;
			Rect RectCell = Rect.MinMaxRect(0.0f, 0.0f, 64.0f, 64.0f);

			/* Main-Texture Data Get */
			Material MaterialNow = ScriptRoot.MaterialGet(AnimationDataCell[FrameNo].TextureNo, KindBlendTarget);
			if(null != MaterialNow)
			{
				Texture InstanceTexture = MaterialNow.mainTexture;
				SizeTexture.x = (float)InstanceTexture.width;
				SizeTexture.y = (float)InstanceTexture.height;
			}

			/* Cell-Data Get */
			if(0 < AnimationDataCell.Length)
			{
				RectCell = AnimationDataCell[FrameNo].Rectangle;
				PivotTexture = new Vector2(RectCell.width * 0.5f, RectCell.height * 0.5f);

				PivotMesh = AnimationDataCell[FrameNo].Pivot;
			}

			/* Disolve Flipping & Texture-Scaling */
			if(0 < AnimationDataFlags.Length)
			{
				RateScaleTexture.x = (true == AnimationDataFlags[FrameNo].IsTextureFlipX) ? -1.0f : 1.0f;
				RateScaleTexture.y = (true == AnimationDataFlags[FrameNo].IsTextureFlipY) ? -1.0f : 1.0f;
				RateScaleMesh.x = (true == AnimationDataFlags[FrameNo].IsFlipX) ? -1.0f : 1.0f;
				RateScaleMesh.y = (true == AnimationDataFlags[FrameNo].IsFlipY) ? -1.0f : 1.0f;
			}
			if(0 < AnimationDataTextureScale.Length)
			{
				RateScaleTexture.x *= AnimationDataTextureScale[FrameNo].x;
				RateScaleTexture.y *= AnimationDataTextureScale[FrameNo].y;
			}

			/* Calculate Matrix-Texture */
			float Rotate = (0 < AnimationDataTextureRotate.Length) ? AnimationDataTextureRotate[FrameNo] :  0.0f;
			Vector2 TextureOffset = (0 < AnimationDataTextureTranslate.Length) ? AnimationDataTextureTranslate[FrameNo] : Vector2.zero;
			Vector3 Translation = new Vector3(	((RectCell.xMin + PivotTexture.x) / SizeTexture.x) + TextureOffset.x,
												((SizeTexture.y - (RectCell.yMin + PivotTexture.y)) / SizeTexture.y) - TextureOffset.y,
												0.0f
											);
			Vector3 Scaling = new Vector3(	(RectCell.width / SizeTexture.x) * RateScaleTexture.x,
											(RectCell.height / SizeTexture.y) * RateScaleTexture.y,
											1.0f
										);
			Quaternion Rotation = Quaternion.Euler(0.0f, 0.0f, Rotate);
			MatrixTexture = Matrix4x4.TRS(Translation, Rotation, Scaling);

			/* Set Vertex-Datas */
			int CountVertexData = MeshNow.vertexCount;

			Vector2[] DataUV = new Vector2[CountVertexData];
			Vector3 Coodinate = Vector3.zero;
			for(int i=0; i<CountVertexData; i++)	/* Texture-UV */
			{	/* Memo: "ArrayUVMappingUV0_Triangle4" of the data up to the "VertexNo.TERMINATOR2"-th elements are same as those of "ArrayUVMappingUV0_Triangle2". */
				Coodinate = MatrixTexture.MultiplyPoint3x4(ArrayUVMappingUV0_Triangle4[i]);
				DataUV[i] = new Vector2(Coodinate.x, Coodinate.y);
			}
			MeshNow.uv = DataUV;

			float RateOpacity = (0 < AnimationDataOpacityRate.Length) ? AnimationDataOpacityRate[FrameNo] : 1.0f;
			Vector2[] DataUV2 = new Vector2[CountVertexData];
			Color32[] DataColor32 = new Color32[CountVertexData];
			if(0 < AnimationDataColorBlend.Length)	/* Blending-Color & Opacity*/
			{	/* Animation-Data */
				for(int i=0; i<CountVertexData; i++)
				{
					DataUV2[i] = new Vector2(	AnimationDataColorBlend[FrameNo].RatePixelAlpha[i] * RateOpacity,
												(float)AnimationDataColorBlend[FrameNo].Operation + 0.01f	/* "+0.01f" for Rounding-off-Error */
											);
					DataColor32[i] = AnimationDataColorBlend[FrameNo].VertexColor[i];
				}
			}
			else
			{	/* Default (No Datas) */
				Color32 ColorDefault = Color.white;
				float OperationDefault = (float)KindColorOperation.NON + 0.01f;	/* "+0.01f" for Rounding-off-Error */
				for(int i=0; i<CountVertexData; i++)
				{
					DataUV2[i] = new Vector2(RateOpacity, OperationDefault);
					DataColor32[i] = ColorDefault;
				}
			}
			MeshNow.colors32 = DataColor32;
			MeshNow.uv2 = DataUV2;

			Vector3[] DataCoordinate = new Vector3[CountVertexData];
			if((int)VertexNo.TERMINATOR4 == CountVertexData)	/* Vertex-Coordinate */
			{	/* 4-Triangles Mesh */
				/* Accommodate Pivot's-Offset */
				Vector2 PivotOffset = (0 < AnimationDataOriginOffset.Length) ? AnimationDataOriginOffset[FrameNo] : Vector2.zero;
				PivotMesh.x += (RectCell.width * PivotOffset.x) * RateScaleMesh.x;
				PivotMesh.y -= (RectCell.height * PivotOffset.y) * RateScaleMesh.y;

				/* Get Coordinates */
				/* MEMO: No Check "AnimationDataVertexCorrection.Length", 'cause 4-Triangles-Mesh necessarily has "AnimationDataVertexCorrection" */
				float Left = (-PivotMesh.x) * RateScaleMesh.x;
				float Right = (RectCell.width - PivotMesh.x) * RateScaleMesh.x;
				float Top = (-PivotMesh.y) * RateScaleMesh.y;
				float Bottom = (RectCell.height - PivotMesh.y) * RateScaleMesh.y;

				DataCoordinate[(int)VertexNo.LU] = new Vector3(	Left + AnimationDataVertexCorrection[FrameNo].Coordinate[(int)VertexNo.LU].x,
																-Top + AnimationDataVertexCorrection[FrameNo].Coordinate[(int)VertexNo.LU].y,
																0.0f
															);
				DataCoordinate[(int)VertexNo.RU] = new Vector3(	Right + AnimationDataVertexCorrection[FrameNo].Coordinate[(int)VertexNo.RU].x,
																-Top + AnimationDataVertexCorrection[FrameNo].Coordinate[(int)VertexNo.RU].y,
																0.0f
															);
				DataCoordinate[(int)VertexNo.RD] = new Vector3(	Right + AnimationDataVertexCorrection[FrameNo].Coordinate[(int)VertexNo.RD].x,
																-Bottom + AnimationDataVertexCorrection[FrameNo].Coordinate[(int)VertexNo.RD].y,
																0.0f
															);
				DataCoordinate[(int)VertexNo.LD] = new Vector3(	Left + AnimationDataVertexCorrection[FrameNo].Coordinate[(int)VertexNo.LD].x,
																-Bottom + AnimationDataVertexCorrection[FrameNo].Coordinate[(int)VertexNo.LD].y,
																0.0f
															);
				Vector3 CoordinateLURU = (DataCoordinate[(int)VertexNo.LU] + DataCoordinate[(int)VertexNo.RU]) * 0.5f;
				Vector3 CoordinateLULD = (DataCoordinate[(int)VertexNo.LU] + DataCoordinate[(int)VertexNo.LD]) * 0.5f;
				Vector3 CoordinateLDRD = (DataCoordinate[(int)VertexNo.LD] + DataCoordinate[(int)VertexNo.RD]) * 0.5f;
				Vector3 CoordinateRURD = (DataCoordinate[(int)VertexNo.RU] + DataCoordinate[(int)VertexNo.RD]) * 0.5f;
				CoordinateGetDiagonalIntersection(	out DataCoordinate[(int)VertexNo.C],
													ref CoordinateLURU,
													ref CoordinateRURD,
													ref CoordinateLULD,
													ref CoordinateLDRD
												);
			}
			else
			{	/* 2-Triangles Mesh */
				/* Accommodate Pivot's-Offset */
				Vector2 PivotOffset = (0 < AnimationDataOriginOffset.Length) ? AnimationDataOriginOffset[FrameNo] : Vector2.zero;
				PivotMesh.x += (RectCell.width * PivotOffset.x) * RateScaleMesh.x;
				PivotMesh.y -= (RectCell.height * PivotOffset.y) * RateScaleMesh.y;

				/* Get Coordinates */
				float Left = (-PivotMesh.x) * RateScaleMesh.x;
				float Right = (RectCell.width - PivotMesh.x) * RateScaleMesh.x;
				float Top = (-PivotMesh.y) * RateScaleMesh.y;
				float Bottom = (RectCell.height - PivotMesh.y) * RateScaleMesh.y;

				DataCoordinate[(int)VertexNo.LU] = new Vector3(Left, -Top, 0.0f);
				DataCoordinate[(int)VertexNo.RU] = new Vector3(Right, -Top, 0.0f);
				DataCoordinate[(int)VertexNo.RD] = new Vector3(Right, -Bottom, 0.0f);
				DataCoordinate[(int)VertexNo.LD] = new Vector3(Left, -Bottom, 0.0f);
			}
			MeshNow.vertices = DataCoordinate;
		}
		private static void CoordinateGetDiagonalIntersection(out Vector3 Output, ref Vector3 LU, ref Vector3 RU, ref Vector3 LD, ref Vector3 RD)
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

		public void DrawEntry(Script_SpriteStudio_PartsRoot.InformationMeshData MeshDataInformation, int FrameNo, Script_SpriteStudio_PartsRoot ScriptRoot)
		{
			float Priority = (0 < AnimationDataPriority.Length) ? AnimationDataPriority[FrameNo] : 0.0f;
			int TextureNo = (0 < AnimationDataCell.Length) ? AnimationDataCell[FrameNo].TextureNo : -1;

			MeshDataInformation.Priority = Priority + ((float)ID * (1.0f / 1000.0f));
			ScriptRoot.MeshAdd(TextureNo, KindBlendTarget, MeshDataInformation);
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

		protected Script_SpriteStudio_PartsRoot.InformationMeshData DataMeshInformation = null;
	}
}
