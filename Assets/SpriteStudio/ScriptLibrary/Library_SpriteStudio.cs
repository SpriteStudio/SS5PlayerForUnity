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
	public delegate bool FunctionCallBackPlayEnd(GameObject ObjectControl);
	public delegate void FunctionCallBackUserData(GameObject ObjectControl, string PartsName, Library_SpriteStudio.AnimationData AnimationDataParts, int AnimationNo, int FrameNoDecode, int FrameNoKeyData, Library_SpriteStudio.KeyFrame.ValueUser.Data Data, bool FlagWayBack);
	public delegate void FunctionCallBackOnTrigger(Collider Self, Collider Pair);
	public delegate void FunctionCallBackOnCollision(Collider Self, Collision Contacts);

	public enum KindParts
	{
		NORMAL = 0,
		ROOT,
		NULL,
		BOUND,					/* Disused */
		SOUND,					/* DisUsed */
		INSTANCE,

		TERMINATOR
	};

	public enum KindSprite
	{
		NON = -1,				/* Not Sprite-Parts: ROOT/NULL/INSTANCE */
		TRIANGLE2 = 0,			/* No use Vertex-Collection Sprite-Parts */
		TRIANGLE4,				/* Use Vertex-Collection Sprite-Parts */
	}

	public enum KindAnimationData
	{
		PLAIN = 0,				/* Data-Format: Plain-Data */
		FIX,					/* Data-Format: Deformation of Mesh" and "Collider" are Calculated */
	}

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

	public readonly static Shader[] Shader_SpriteStudioTriangleX = new Shader[(int)Library_SpriteStudio.KindColorOperation.TERMINATOR - 1]
	{
		Shader.Find("Custom/SpriteStudio5/Mix"),
		Shader.Find("Custom/SpriteStudio5/Add"),
		Shader.Find("Custom/SpriteStudio5/Sub"),
		Shader.Find("Custom/SpriteStudio5/Mul")
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

	public readonly static int[,] VertexCollectionOrderVertex = new int[4, (int)VertexNo.TERMINATOR2]
	{
		{	/* Normal */
			(int)VertexNo.LU,
			(int)VertexNo.RU,
			(int)VertexNo.RD,
			(int)VertexNo.LD,
		}, {	/* Flip-X */
			(int)VertexNo.RU,
			(int)VertexNo.LU,
			(int)VertexNo.LD,
			(int)VertexNo.RD,
		}, {	/* Flip-Y */
			(int)VertexNo.LD,
			(int)VertexNo.RD,
			(int)VertexNo.RU,
			(int)VertexNo.LU,
		}, {	/* FlipX&Y */
			(int)VertexNo.RD,
			(int)VertexNo.LD,
			(int)VertexNo.LU,
			(int)VertexNo.RU,
		}
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

		public static Camera CameraGetParent(GameObject InstanceGameObject)
		{
			Transform InstanceTransform = InstanceGameObject.transform.parent;
			Camera InstanceCamera = null;
			while(null != InstanceTransform)
			{
				InstanceCamera = InstanceTransform.camera;
				if(null != InstanceCamera)
				{
					break;
				}
				InstanceTransform = InstanceTransform.parent;
			}
			return(InstanceCamera);
		}

		public static Script_SpriteStudio_DrawManagerView DrawManagerViewGetParent(GameObject InstanceGameObject)
		{
			Transform InstanceTransform = InstanceGameObject.transform.parent;
			Script_SpriteStudio_DrawManagerView InstanceView = null;
			while(null != InstanceTransform)
			{
				InstanceView = InstanceTransform.gameObject.GetComponent<Script_SpriteStudio_DrawManagerView>();
				if(null != InstanceView)
				{
					break;
				}
				InstanceTransform = InstanceTransform.parent;
			}
			return(InstanceView);
		}

		public static Script_SpriteStudio_PartsRoot PartsRootGetParent(GameObject InstanceGameObject)
		{
			Transform InstanceTransform = InstanceGameObject.transform.parent;
			Script_SpriteStudio_PartsRoot InstanceRoot = null;
			while(null != InstanceTransform)
			{
				InstanceRoot = InstanceTransform.gameObject.GetComponent<Script_SpriteStudio_PartsRoot>();
				if(null != InstanceRoot)
				{
					break;
				}
				InstanceTransform = InstanceTransform.parent;
			}
			return(InstanceRoot);
		}

		public static void HideSetForce(GameObject InstanceGameObject, bool FlagSwitch, bool FlagSetChild, bool FlagSetInstance)
		{
			GameObject InstanceGameObjectNow = InstanceGameObject;
			Transform InstanceTransform = InstanceGameObjectNow.transform;
			Script_SpriteStudio_PartsRoot ScriptRoot = InstanceGameObjectNow.GetComponent<Script_SpriteStudio_PartsRoot>();
			if(null != ScriptRoot)
			{
				if((false == FlagSetInstance) && (null != ScriptRoot.PartsRootOrigin))
				{	/* "Instance"-Object */
					return;
				}
				else
				{
					ScriptRoot.FlagHideForce = FlagSwitch;
				}
			}
			else
			{
				Script_SpriteStudio_Triangle2 ScriptTriangle2 = InstanceGameObjectNow.GetComponent<Script_SpriteStudio_Triangle2>();
				if(null != ScriptTriangle2)
				{
					ScriptTriangle2.FlagHideForce = FlagSwitch;
				}
				else
				{
					Script_SpriteStudio_Triangle4 ScriptTriangle4 = InstanceGameObjectNow.GetComponent<Script_SpriteStudio_Triangle4>();
					if(null != ScriptTriangle4)
					{
						ScriptTriangle4.FlagHideForce = FlagSwitch;
					}
					else
					{
						Script_SpriteStudio_PartsInstance ScriptInstasnce = InstanceGameObjectNow.GetComponent<Script_SpriteStudio_PartsInstance>();
						if(null != ScriptInstasnce)
						{
							ScriptInstasnce.FlagHideForce = FlagSwitch;
						}
					}
				}
				if(true == FlagSetChild)
				{
					for(int i=0; i<InstanceTransform.childCount; i++)
					{
						HideSetForce(InstanceTransform.GetChild(i).gameObject, FlagSwitch, FlagSetChild, FlagSetInstance);
					}
				}
			}
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
					RatePixelAlpha[i] = 1.0f;
				}
			}

			public bool Equals(ValueColor target)
			{
				if (this.Bound != target.Bound)
				{
					return false;
				}
				if (this.Operation != target.Operation)
				{
					return false;
				}
				if (this.VertexColor.Length != target.VertexColor.Length)
				{
					return false;
				}
				for(int i=0; i<this.VertexColor.Length ; ++i)
				{
					if (this.VertexColor[i] != target.VertexColor[i])
					{
						return false;
					}
				}
				if (this.RatePixelAlpha.Length != target.RatePixelAlpha.Length)
				{
					return false;
				}
				for(int i=0; i<this.RatePixelAlpha.Length ; ++i)
				{
					if (this.RatePixelAlpha[i] != target.RatePixelAlpha[i])
					{
						return false;
					}
				}
				return true;
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
			[System.Serializable]
			public class  Data
			{
				/* Renewal, as same as ValueInstance */
				public int FrameNoBase;
				public int TextureNo;
				public Rect Rectangle;
				public Vector2 Pivot;
				public Vector2 SizeOriginal;

				public Data()
				{
					FrameNoBase = -1;
					TextureNo = -1;
					Rectangle.x = 0.0f;
					Rectangle.y = 0.0f;
					Rectangle.width = 0.0f;
					Rectangle.height = 0.0f;
					Pivot = Vector2.zero;
					SizeOriginal = Vector2.zero;
				}
			}
		}

		[System.Serializable]
		public class ValueUser
		{
			[System.Serializable]
			public class Data
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

				public Data()
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
		}

		[System.Serializable]
		public class ValueBools
		{
			public enum FlagData : int
			{
				VALID = 0x40000000,
				HIDE = 0x00010000,
				FLIPX = 0x00100000,			/* Caution: Does not affect the display (State in "ssae" / Baked to Datas) */
				FLIPY = 0x00200000,			/* Caution: Does not affect the display (State in "ssae" / Baked to Datas) */
				FLIPXTEXTURE = 0x00400000,	/* Caution: Does not affect the display (State in "ssae" / Baked to Datas) */
				FLIPYTEXTURE = 0x00800000,	/* Caution: Does not affect the display (State in "ssae" / Baked to Datas) */
				TEXTURENO = 0x0000ffff,

				CLEAR = TEXTURENO,
			};

			public FlagData Flag;

			public ValueBools()
			{
				Flag = FlagData.CLEAR;
			}

			public bool IsValid
			{
				get
				{
					return(0 != (Flag & FlagData.VALID));
				}
			}
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
			public int TextureNo
			{
				get
				{
					FlagData Data = Flag & FlagData.TEXTURENO;
					return((Data == FlagData.TEXTURENO) ? (-1) : (int)Data);
				}
			}
		}

		[System.Serializable]
		public class ValueInstance
		{
			[System.Serializable]
			public class  Data
			{
				public enum FlagData
				{
					PINGPONG = 0x00000001,
					INDEPENDENT = 0x00000002,
	
					CLEAR = 0x00000000,
				};
				
				public int FrameNoBase;
				public FlagData Flag;
				public int PlayCount;
				public float RateTime;
				public int OffsetStart;
				public int OffsetEnd;
				public string LabelStart;
				public string LabelEnd;

				public Data()
				{
					FrameNoBase = -1;
					Flag = FlagData.CLEAR;
					PlayCount = 1;
					RateTime = 1.0f;
					OffsetStart = 0;
					OffsetEnd = 0;
					LabelStart = "";
					LabelEnd = "";
				}
			}
		}

		/* Dummy Datas */
		public readonly static ValueCell.Data DummyDataCell = new ValueCell.Data();
		public readonly static ValueUser.Data DummyDataUser = new ValueUser.Data();
		public readonly static ValueInstance.Data DummyDataInstance = new ValueInstance.Data();
	}

	public class DrawManager
	{
		/* MEMO: These Defines for only Simplified-SpriteDrawManager */
		public enum KindDrawQueue
		{
			SHADER_SETTING = 0,
			USER_SETTING,
			BACKGROUND,
			GEOMETRY,
			ALPHATEST,
			TRANSPARENT,
			OVERLAY,
		};
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

		/* Drawing-Mesh-Information */
		public class InformationMeshData
		{
			public InformationMeshData ChainNext = null;
			public float Priority = 0.0f;
			public Mesh DataMesh = null;	/* null == Instance Node */
			public Script_SpriteStudio_PartsInstance PartsInstance = null;
			public Transform DataTransform = null;
		}

		/* Drawing-Mesh-Chain Class */
		public class ListMeshDraw
		{
			public Material MaterialOriginal = null;
			public InformationMeshData MeshDataTop;

			public int Count = 0;
			public float PriorityMinimum = float.MaxValue;
			public float PriorityMaximum = float.MinValue;

			public void MeshAdd(InformationMeshData DataNew)
			{
				DataNew.ChainNext = null;

				if(null == MeshDataTop)
				{
					MeshDataTop = DataNew;
					PriorityMinimum = DataNew.Priority;
					PriorityMaximum = DataNew.Priority;
					Count = 1;
					return;
				}
				if(PriorityMinimum > DataNew.Priority)
				{
					DataNew.ChainNext = MeshDataTop;
					MeshDataTop = DataNew;
					PriorityMinimum = DataNew.Priority;
					Count++;
					return;
				}

				InformationMeshData DataNext = MeshDataTop;
				InformationMeshData DataPrevious = null;
				while(null != DataNext)
				{
					if(DataNext.Priority > DataNew.Priority)
					{
						break;
					}
					DataPrevious = DataNext;
					DataNext = DataNext.ChainNext;
				}
				DataPrevious.ChainNext = DataNew;
				DataNew.ChainNext = DataNext;
				Count++;
				if(null == DataNext)
				{
					PriorityMaximum = DataNew.Priority;
				}
			}

			public ListMeshDraw ListSplit(float Priority)
			{
				if(null == MeshDataTop)
				{
					return(null);
				}

				InformationMeshData DataNext = MeshDataTop;
				InformationMeshData DataPrevious = null;
				int CountNow = 0;
				while(null != DataNext)
				{
					if(DataNext.Priority > Priority)
					{
						ListMeshDraw ListNew = new ListMeshDraw();
						ListNew.MaterialOriginal = MaterialOriginal;
						ListNew.Count = Count - CountNow;
						ListNew.MeshDataTop = DataNext;
						ListNew.PriorityMinimum = DataNext.Priority;

						Count -= ListNew.Count;
						if(null == DataPrevious)
						{
							PriorityMinimum = 0;
							PriorityMaximum = 0;
							MeshDataTop = null;
						}
						else
						{
							DataPrevious.ChainNext = null;
							ListNew.PriorityMaximum = PriorityMaximum;
							PriorityMaximum = DataPrevious.Priority;
						}
						return(ListNew);
					}
					CountNow++;
					DataPrevious = DataNext;
					DataNext = DataNext.ChainNext;
				}
				return(null);
			}

			public void ListMerge(ListMeshDraw ListNext)
			{
				if(0 == ListNext.Count)
				{
					return;
				}
				if(0 == Count)
				{
					MeshDataTop = ListNext.MeshDataTop;
					PriorityMinimum = ListNext.PriorityMinimum;
					PriorityMaximum = ListNext.PriorityMaximum;
					Count = ListNext.Count;
					return;
				}
				InformationMeshData DataLast = MeshDataTop;
				while(null != DataLast.ChainNext)
				{
					DataLast = DataLast.ChainNext;
				}
				DataLast.ChainNext = ListNext.MeshDataTop;
				PriorityMaximum = ListNext.PriorityMaximum;
				Count += ListNext.Count;
			}
		}

		public class ArrayListMeshDraw
		{
			public Library_SpriteStudio.DrawManager.KindDrawQueue KindRenderQueueBase;
			public int OffsetDrawQueue;

#if false
			/* MEMO: Non-Generic List-Class */
			private ArrayList tableListMesh;
			public ArrayList TableListMesh
			{
				get
				{
					return(tableListMesh);
				}
			}
#else
			private List<ListMeshDraw> tableListMesh;
			public List<ListMeshDraw> TableListMesh
			{
				get
				{
					return(tableListMesh);
				}
			}
#endif

			public void BootUp()
			{
#if false
				/* MEMO: Non-Generic List-Class */
				tableListMesh = new ArrayList();
				tableListMesh.Clear();
#else
				tableListMesh = new List<ListMeshDraw>();
				tableListMesh.Clear();
#endif

				KindRenderQueueBase = Library_SpriteStudio.DrawManager.KindDrawQueue.SHADER_SETTING;
				OffsetDrawQueue = 0;
			}

			public void RenderQueueSet(Library_SpriteStudio.DrawManager.KindDrawQueue RenderQueueBase, int DrawQueue)
			{
				KindRenderQueueBase = RenderQueueBase;
				OffsetDrawQueue = DrawQueue;
			}

			public void BootCheck()
			{
				if(null == tableListMesh)
				{
					BootUp();
				}
			}

			public void Clear()
			{
				if(null != tableListMesh)
				{
					tableListMesh.Clear();
				}
			}

			public void ShutDown()
			{
				if(null != tableListMesh)
				{
					tableListMesh.Clear();
				}
				tableListMesh = null;
			}

			public void MeshAdd(Material MaterialOriginal, InformationMeshData DataMeshInformation)
			{
				BootCheck();

				int CountList = tableListMesh.Count;
				int ListNo = -1;
				ListMeshDraw ListMesh = null;
				if(0 == tableListMesh.Count)
				{
					goto MeshAdd_NewListAdd;
				}
				else
				{
					ListNo = 0;
#if false
					/* MEMO: Non-Generic List-Class */
					ListMesh = tableListMesh[0] as ListMeshDraw;
#else
					ListMesh = tableListMesh[0];
#endif
					for(int i=1; i<CountList; i++)
					{
#if false
						/* MEMO: Non-Generic List-Class */
						ListMesh = tableListMesh[i] as ListMeshDraw;
#else
						ListMesh = tableListMesh[i];
#endif
						if(DataMeshInformation.Priority < ListMesh.PriorityMinimum)
						{
							ListNo = i - 1;
							break;
						}
						ListMesh = null;
					}
					if(null == ListMesh)
					{	/* Highest-Priority */
						ListNo = CountList - 1;
#if false
						/* MEMO: Non-Generic List-Class */
						ListMesh = tableListMesh[ListNo] as ListMeshDraw;
#else
						ListMesh = tableListMesh[ListNo];
#endif
						if(null != DataMeshInformation.PartsInstance)
						{	/* Instance-Parts */
							if(DataMeshInformation.Priority < ListMesh.PriorityMaximum)
							{
								goto MeshAdd_NewListInsertSplit;
							}
							else
							{
								goto MeshAdd_NewListAdd;
							}
						}
						else
						{	/* Sprite-Parts */
							if(ListMesh.MaterialOriginal != MaterialOriginal)
							{
								if(DataMeshInformation.Priority < ListMesh.PriorityMaximum)
								{
									goto MeshAdd_NewListInsertSplit;
								}
								else
								{
									goto MeshAdd_NewListAdd;
								}
							}
						}
					}
					else
					{
#if false
						/* MEMO: Non-Generic List-Class */
						ListMesh = tableListMesh[ListNo] as ListMeshDraw;
#else
						ListMesh = tableListMesh[ListNo];
#endif
						if(null != DataMeshInformation.PartsInstance)
						{	/* Instance-Parts */
							if(DataMeshInformation.Priority < ListMesh.PriorityMaximum)
							{
								goto MeshAdd_NewListInsertSplit;
							}
							else
							{
								ListNo++;
								if(CountList <= ListNo)
								{	/* List Buttom */
									goto MeshAdd_NewListAdd;
								}
								else
								{	/* List Insert */
									ListNo--;
									goto MeshAdd_NewListInsert;
								}
							}
						}
						else
						{	/* Sprite-Parts */
							if(ListMesh.MaterialOriginal != MaterialOriginal)
							{
								if(DataMeshInformation.Priority < ListMesh.PriorityMaximum)
								{
									goto MeshAdd_NewListInsertSplit;
								}
								else
								{
									ListNo++;
									if(CountList <= ListNo)
									{
										goto MeshAdd_NewListAdd;
									}
									else
									{
#if false
										/* MEMO: Non-Generic List-Class */
										ListMeshDraw ListMeshNext = tableListMesh[ListNo] as ListMeshDraw;
#else
										ListMeshDraw ListMeshNext = tableListMesh[ListNo];
#endif
										if(ListMeshNext.MaterialOriginal != MaterialOriginal)
										{
											ListNo--;
											goto MeshAdd_NewListInsert;
										}
										else
										{
											ListMesh = ListMeshNext;
										}
									}
								}
							}
						}
					}
					ListMesh.MeshAdd(DataMeshInformation);
				}
				return;

			MeshAdd_NewListAdd:
				ListMesh = new ListMeshDraw();
				ListMesh.MaterialOriginal = MaterialOriginal;
				tableListMesh.Add(ListMesh);

				ListMesh.MeshAdd(DataMeshInformation);
				return;

			MeshAdd_NewListInsert:
				ListMesh = new ListMeshDraw();
				ListMesh.MaterialOriginal = MaterialOriginal;
				tableListMesh.Insert(ListNo + 1, ListMesh);

				ListMesh.MeshAdd(DataMeshInformation);
				return;

			MeshAdd_NewListInsertSplit:
				{
					ListMeshDraw ListMeshSplit = ListMesh.ListSplit(DataMeshInformation.Priority);
					int ListNoNext = ListNo + 1;
					if(CountList <= ListNoNext)
					{
						tableListMesh.Add(ListMeshSplit);
					}
					else
					{
						tableListMesh.Insert(ListNoNext, ListMeshSplit);
					}
					int CountOld = ListMesh.Count;

					ListMesh = new ListMeshDraw();
					ListMesh.MaterialOriginal = MaterialOriginal;
					tableListMesh.Insert(ListNoNext, ListMesh);

					if(0 >= CountOld)
					{
						tableListMesh.RemoveAt(ListNo);
					}
				}

				ListMesh.MeshAdd(DataMeshInformation);
				return;
			}

			public void MeshSetCombine(MeshFilter InstanceMeshFilter, MeshRenderer InstanceMeshRenderer, Camera InstanceCamera, Transform InstanceTransform)
			{
				if(null != tableListMesh)
				{
					ListMeshDraw ListMesh = null;
					int Count = tableListMesh.Count;

					Material[] MaterialTable = new Material[Count];
					int ValueRenderQueue = Library_SpriteStudio.DrawManager.ValueKindDrawQueue[(int)KindRenderQueueBase];
					int MaterialRenderQueue = 0;

					/* Material-Table Set */
					int CountMesh = 0;
					for(int i=0; i<Count; i++)
					{
#if false
						/* MEMO: Non-Generic List-Class */
						ListMesh = tableListMesh[i] as ListMeshDraw;
#else
						ListMesh = tableListMesh[i];
#endif
						
						CountMesh += ListMesh.Count;

						MaterialTable[i] = new Material(ListMesh.MaterialOriginal);
						MaterialRenderQueue = (-1 == ValueRenderQueue) ? MaterialTable[i].shader.renderQueue : ValueRenderQueue;
						MaterialRenderQueue += (OffsetDrawQueue + i);
						MaterialTable[i].renderQueue = MaterialRenderQueue;
					}

					Material[] TableMaterialOld = InstanceMeshRenderer.sharedMaterials;
					InstanceMeshRenderer.sharedMaterials = MaterialTable;
					for(int i=0; i<TableMaterialOld.Length; i++)
					{
						Object.DestroyImmediate(TableMaterialOld[i]);
					}

					/* Combine Meshes */
					int IndexVertexNow = 0;
					int IndexTriangleNow = 0;
					int[] IndexVertex = new int[Count];
					int[] IndexTriangle = new int[Count+1];
					CombineInstance[] CombineMesh = new CombineInstance[CountMesh];
					InformationMeshData DataMeshInformation = null;

					Matrix4x4 MatrixCorrect = InstanceTransform.localToWorldMatrix.inverse;
					int IndexMesh = 0;
					for(int i=0; i<Count; i++)
					{
#if false
						/* MEMO: Non-Generic List-Class */
						ListMesh = tableListMesh[i] as ListMeshDraw;
#else
						ListMesh = tableListMesh[i];
#endif
						IndexVertex[i] = IndexVertexNow;
						IndexTriangle[i] = IndexTriangleNow;
						DataMeshInformation = ListMesh.MeshDataTop;
						while(null != DataMeshInformation)
						{
							CombineMesh[IndexMesh].mesh = DataMeshInformation.DataMesh;
							if(null != DataMeshInformation.DataTransform)
							{
								CombineMesh[IndexMesh].transform = MatrixCorrect * DataMeshInformation.DataTransform.localToWorldMatrix;
								IndexMesh++;

								IndexVertexNow += DataMeshInformation.DataMesh.vertexCount;
								IndexTriangleNow += DataMeshInformation.DataMesh.triangles.Length / 3;
							}

							DataMeshInformation = DataMeshInformation.ChainNext;
						}
					}
					IndexTriangle[Count] = IndexTriangleNow;
					Mesh MeshNew = new Mesh();
					MeshNew.CombineMeshes(CombineMesh);

					/* Vertex-Index Set */
					int[] TriangleBuffer = MeshNew.triangles;
					int[] VertexNoTriangle = null;
					MeshNew.triangles = null;
					MeshNew.subMeshCount = Count;
					for(int i=0; i<Count; i++)
					{
						CountMesh = IndexTriangle[i + 1] - IndexTriangle[i];
						VertexNoTriangle = new int[CountMesh * 3];
						for(int j=0; j<CountMesh; j++)
						{
							IndexTriangleNow = (j + IndexTriangle[i]) * 3;
							IndexVertexNow = j * 3;

							VertexNoTriangle[IndexVertexNow] = TriangleBuffer[IndexTriangleNow];
							VertexNoTriangle[IndexVertexNow + 1] = TriangleBuffer[IndexTriangleNow + 1];
							VertexNoTriangle[IndexVertexNow + 2] = TriangleBuffer[IndexTriangleNow + 2];
						}
						MeshNew.SetTriangles(VertexNoTriangle, i);
					}

					if(null != InstanceMeshFilter.sharedMesh)
					{
						InstanceMeshFilter.sharedMesh.Clear();
						Object.DestroyImmediate(InstanceMeshFilter.sharedMesh);
					}
					MeshNew.name = "BatchedMesh";
					InstanceMeshFilter.sharedMesh = MeshNew;

					/* Clear Draw-Entries */
					tableListMesh.Clear();
				}
			}
		}
	}

	[System.Serializable]
	public class AnimationInformationPlay
	{
		[System.Serializable]
		public class InformationLabel
		{
			public string Name;
			public int FrameNo;
		}

		public string Name;
		public int FrameStart;
		public int FrameEnd;
		public int FramePerSecond;
		public InformationLabel[] Label;

		public AnimationInformationPlay()
		{
			Name = "";
			FrameStart = 0;
			FrameEnd = 0;
			FramePerSecond = 0;
			Label = null;
		}

		public static readonly string LabelDefaultStart = "_start";
		public static readonly string LabelDefaultEnd = "_end";
		public static bool NameCheckDefault(string Name)
		{
			if(0 == string.Compare(Name, LabelDefaultStart))
			{
				return(true);
			}
			if(0 == string.Compare(Name, LabelDefaultEnd))
			{
				return(true);
			}
			return(false);
		}

		public int FrameNoGetLabel(string Name)
		{
			/* Default Labels */
			if(0 == string.Compare(Name, LabelDefaultStart))
			{
				return(FrameStart);
			}
			if(0 == string.Compare(Name, LabelDefaultEnd))
			{
				return(FrameEnd);
			}

			/* Voluntary Labels */
			if(null != Label)
			{
				int Count = Label.Length;
				for(int i=0; i<Count; i++)
				{
					if(0 == string.Compare(Name, Label[i].Name))
					{
						return(Label[i].FrameNo);
					}
				}
			}
			return(-1);
		}

		internal bool RangeGet(	ref int FrameCount,
								ref int FrameNoStart,
								ref int FrameNoEnd,
								string LabelStart,
								int OffsetStart,
								string LabelEnd,
								int OffsetEnd
							)
		{
			int	FrameNo = 0;

			if(true == string.IsNullOrEmpty(LabelStart))
			{
				LabelStart = LabelDefaultStart;
			}
			FrameNo = FrameNoGetLabel(LabelStart);
			if(-1 == FrameNo)
			{	/* Label Not Found */
				FrameNo = FrameStart;
			}
			FrameNo += OffsetStart;
			if((FrameStart > FrameNo) || (FrameEnd < FrameNo))
			{
				FrameNo = FrameStart;
			}
			FrameNoStart = FrameNo;

			if(true == string.IsNullOrEmpty(LabelEnd))
			{
				LabelEnd = LabelDefaultEnd;
			}
			FrameNo = FrameNoGetLabel(LabelEnd);
			if(-1 == FrameNo)
			{	/* Label Not Found */
				FrameNo = FrameEnd;
			}
			FrameNo += OffsetEnd;
			if((FrameStart > FrameNo) || (FrameEnd < FrameNo))
			{
				FrameNo = FrameEnd;
			}
			FrameNoEnd = FrameNo;
			FrameCount = (FrameEnd - FrameStart) + 1;
			return(true);
		}
	}

	[System.Serializable]
	public class AnimationData
	{
		public int ID;
		public KindColorOperation KindBlendTarget;

		public KindCollision CollisionKind;
		public float CollisionSizeZ;

		public KeyFrame.ValueBools[] AnimationDataFlags;

		public Vector3[] AnimationDataPosition;
		public CompressedVector3Array CompressedAnimationDataPosition;
		public Vector3[] AnimationDataRotation;
		public CompressedVector3Array CompressedAnimationDataRotation;
		public Vector2[] AnimationDataScaling;
		public CompressedVector2Array CompressedAnimationDataScaling;

		public float[] AnimationDataOpacityRate;
		public CompressedFloatArray CompressedAnimationDataOpacityRate;

		public float[] AnimationDataPriority;
		public CompressedFloatArray CompressedAnimationDataPriority;

		public int[] AnimationDataUser;
		public int[] AnimationDataInstance;

		public KeyFrame.ValueUser.Data[] ArrayDataBodyUser;
		public KeyFrame.ValueInstance.Data[] ArrayDataBodyInstance;

		public KindAnimationData KindDataFormat;
		public Plain DataPlain;
		public Fix DataFix;

		public AnimationData()
		{
			ID = -1;
			KindBlendTarget = Library_SpriteStudio.KindColorOperation.NON;
			CollisionKind = KindCollision.NON;

			AnimationDataFlags = null;
			CollisionSizeZ = 0.0f;

			AnimationDataPosition = null;
			CompressedAnimationDataPosition = null;
			AnimationDataRotation = null;
			CompressedAnimationDataRotation = null;
			AnimationDataScaling = null;
			CompressedAnimationDataScaling = null;

			AnimationDataOpacityRate = null;
			CompressedAnimationDataOpacityRate = null;
			AnimationDataPriority = null;
			CompressedAnimationDataPriority = null;

			AnimationDataUser = null;
			AnimationDataInstance = null;

			ArrayDataBodyUser = null;
			ArrayDataBodyInstance = null;

			KindDataFormat = Library_SpriteStudio.KindAnimationData.PLAIN;
			DataPlain = null;
			DataFix = null;
		}

		public void Decompress()
		{
			if (CompressedAnimationDataPosition != null && CompressedAnimationDataPosition.Length > 0)
			{
				AnimationDataPosition = CompressedAnimationDataPosition.ToArray();
				CompressedAnimationDataPosition = null;
			}

			if (CompressedAnimationDataRotation != null && CompressedAnimationDataRotation.Length > 0)
			{
				AnimationDataRotation = CompressedAnimationDataRotation.ToArray();
				CompressedAnimationDataRotation = null;
			}

			if (CompressedAnimationDataScaling != null && CompressedAnimationDataScaling.Length > 0)
			{
				AnimationDataScaling = CompressedAnimationDataScaling.ToArray();
				CompressedAnimationDataScaling = null;
			}

			if (CompressedAnimationDataOpacityRate != null && CompressedAnimationDataOpacityRate.Length > 0)
			{
				AnimationDataOpacityRate = CompressedAnimationDataOpacityRate.ToArray();
				CompressedAnimationDataOpacityRate = null;
			}

			if (CompressedAnimationDataPriority != null && CompressedAnimationDataPriority.Length > 0)
			{
				AnimationDataPriority = CompressedAnimationDataPriority.ToArray();
				CompressedAnimationDataPriority = null;
			}

			if (DataPlain != null)
			{
				DataPlain.Decompress();
			}
			if (DataFix != null)
			{
				DataFix.Decompress();
			}
		}

		public void Compress()
		{
			if (AnimationDataPosition != null && AnimationDataPosition.Length > 0)
			{
				CompressedAnimationDataPosition = CompressedVector3Array.Build(AnimationDataPosition);
				AnimationDataPosition = null;
			}

			if (AnimationDataRotation != null && AnimationDataRotation.Length > 0)
			{
				CompressedAnimationDataRotation = CompressedVector3Array.Build(AnimationDataRotation);
				AnimationDataRotation = null;
			}

			if (AnimationDataScaling != null && AnimationDataScaling.Length > 0)
			{
				CompressedAnimationDataScaling = CompressedVector2Array.Build(AnimationDataScaling);
				AnimationDataScaling = null;
			}

			if (AnimationDataOpacityRate != null && AnimationDataOpacityRate.Length > 0)
			{
				CompressedAnimationDataOpacityRate = CompressedFloatArray.Build(AnimationDataOpacityRate);
				AnimationDataOpacityRate = null;
			}

			if (AnimationDataPriority != null && AnimationDataPriority.Length > 0)
			{
				CompressedAnimationDataPriority = CompressedFloatArray.Build(AnimationDataPriority);
				AnimationDataPriority = null;
			}

			if (DataPlain != null)
			{
				DataPlain.Compress();
			}
			if (DataFix != null)
			{
				DataFix.Compress();
			}
		}

		public bool AnimationDataPositionIsNullOrEmpty
		{
			get
			{
				if ((null != CompressedAnimationDataPosition) && (0 < CompressedAnimationDataPosition.Length))
				{
					return false;
				}
				if ((null != AnimationDataPosition) && (0 < AnimationDataPosition.Length))
				{
					return false;
				}
				return true;
			}
		}
		public bool AnimationDataRotationIsNullOrEmpty
		{
			get
			{
				if ((null != CompressedAnimationDataRotation) && (0 < CompressedAnimationDataRotation.Length))
				{
					return false;
				}
				if ((null != AnimationDataRotation) && (0 < AnimationDataRotation.Length))
				{
					return false;
				}
				return true;
			}
		}
		public bool AnimationDataScalingIsNullOrEmpty
		{
			get
			{
				if ((null != CompressedAnimationDataScaling) && (0 < CompressedAnimationDataScaling.Length))
				{
					return false;
				}
				if ((null != AnimationDataScaling) && (0 < AnimationDataScaling.Length))
				{
					return false;
				}
				return true;
			}
		}
		public bool AnimationDataOpacityRateIsNullOrEmpty
		{
			get
			{
				if ((null != CompressedAnimationDataOpacityRate) && (0 < CompressedAnimationDataOpacityRate.Length))
				{
					return false;
				}
				if ((null != AnimationDataOpacityRate) && (0 < AnimationDataOpacityRate.Length))
				{
					return false;
				}
				return true;
			}
		}

		public bool UpdateGameObject(GameObject GameObjectNow, int FrameNo, Collider ComponentCollider, WorkAreaRuntime WorkArea)
		{
			/* Update Transform */
			/* MEMO: No Transform-Datas, Not Changing "GameObject" */
			bool FlagUpdateTransformPosition = !AnimationDataPositionIsNullOrEmpty;
			bool FlagUpdateTransformRotate = !AnimationDataRotationIsNullOrEmpty;
			bool FlagUpdateTransformScale = !AnimationDataScalingIsNullOrEmpty;

			if((true == FlagUpdateTransformPosition) || (true == FlagUpdateTransformRotate) || (true == FlagUpdateTransformScale))
			{	/* No-Update Transform */
				Vector3 Vector3Temp = Vector3.zero;
				if(true == FlagUpdateTransformPosition)
				{
					Vector3Temp = PositionGet(FrameNo);
					GameObjectNow.transform.localPosition = Vector3Temp;
				}

				if(true == FlagUpdateTransformRotate)
				{
					Vector3Temp = RotationGet(FrameNo);
					GameObjectNow.transform.localEulerAngles = Vector3Temp;
				}

				if(true == FlagUpdateTransformScale)
				{
					Vector3Temp.x = ScalingGet(FrameNo).x;
					Vector3Temp.y = ScalingGet(FrameNo).y;
					Vector3Temp.z = 1.0f;

					GameObjectNow.transform.localScale = Vector3Temp;
				}
			}

			/* Display Flag */
			bool FlagDisplay = false;
			bool FlagAnimationDataFlags = ((null != AnimationDataFlags) && (0 < AnimationDataFlags.Length)) ? true : false;
			if(true == FlagAnimationDataFlags)
			{
				/* Return-Value is "Inversed Hide-Flag"  */
				FlagDisplay = !(AnimationDataFlags[FrameNo].IsHide);
			}

			/* Update Collider */
			/* MEMO: No-Update, if the part is "HIDE" */
			if((null != ComponentCollider) && (true == FlagDisplay))
			{
				switch(KindDataFormat)
				{
					case KindAnimationData.PLAIN:
						DataPlain.UpdateCollider(ComponentCollider, CollisionKind, CollisionSizeZ, AnimationDataFlags, FrameNo, FlagAnimationDataFlags, WorkArea);
						break;
					case KindAnimationData.FIX:
						DataFix.UpdateCollider(ComponentCollider, CollisionKind, CollisionSizeZ, AnimationDataFlags, FrameNo, FlagAnimationDataFlags, WorkArea);
						break;
					default:
						break;
				}
			}

			return(FlagDisplay);
		}

		Vector3 PositionGet(int FrameNo)
		{
			if ((null != CompressedAnimationDataPosition) && (0 < CompressedAnimationDataPosition.Length))
			{
				return CompressedAnimationDataPosition[FrameNo];
			}
			return AnimationDataPosition[FrameNo];
		}
		
		Vector3 RotationGet(int FrameNo)
		{
			if (CompressedAnimationDataRotation != null && CompressedAnimationDataRotation.Length > 0)
			{
				return CompressedAnimationDataRotation[FrameNo];
			}
			return AnimationDataRotation[FrameNo];
		}
		Vector3 ScalingGet(int FrameNo)
		{
			if (CompressedAnimationDataScaling != null && CompressedAnimationDataScaling.Length > 0)
			{
				return CompressedAnimationDataScaling[FrameNo];
			}
			return AnimationDataScaling[FrameNo];
		}

		public float OpacityRateGet(int FrameNo)
		{
			if ((null != CompressedAnimationDataOpacityRate) && (0 < CompressedAnimationDataOpacityRate.Length))
			{
				return CompressedAnimationDataOpacityRate[FrameNo];
			}
			else if ((null != AnimationDataOpacityRate) && (0 < AnimationDataOpacityRate.Length))
			{
				return AnimationDataOpacityRate[FrameNo];
			}
			return 1f;
		}

		float PriorityGet(int FrameNo)
		{
			if ((null != CompressedAnimationDataPriority) && (0 < CompressedAnimationDataPriority.Length))
			{
				return CompressedAnimationDataPriority[FrameNo];
			}
			if ((null != AnimationDataPriority) && (0 < AnimationDataPriority.Length))
			{
				return AnimationDataPriority[FrameNo];
			}
			return 0f;
		}

		public void UpdateMesh(SmartMesh MeshNow, int FrameNo, Script_SpriteStudio_PartsRoot ScriptRoot)
		{
			switch(KindDataFormat)
			{
				case KindAnimationData.PLAIN:
					DataPlain.UpdateMesh(MeshNow.Mesh, AnimationDataFlags, OpacityRateGet(FrameNo), FrameNo, ScriptRoot);
					break;
				case KindAnimationData.FIX:
					DataFix.UpdateMesh(MeshNow, AnimationDataFlags, FrameNo, ScriptRoot);
					break;
				default:
					break;
			}
		}

		public void UpdateUserData(int FrameNo, GameObject GameObjectNow, Script_SpriteStudio_PartsRoot ScriptRoot)
		{
			
			if((null != ArrayDataBodyUser) && (null != AnimationDataUser) && (0 < ArrayDataBodyUser.Length) && (0 < AnimationDataUser.Length))
			{
				if(0 != (ScriptRoot.Status & Script_SpriteStudio_PartsRoot.BitStatus.DECODE_USERDATA))
				{
					int LoopCount = ScriptRoot.CountLoopThisTime;
					bool FlagFirst = (0 != (ScriptRoot.Status & Script_SpriteStudio_PartsRoot.BitStatus.PLAY_FIRST)) ? true : false;
					bool FlagReverse = (0 != (ScriptRoot.Status & Script_SpriteStudio_PartsRoot.BitStatus.PLAYING_REVERSE)) ? true : false;
					bool FlagReversePrevious = ScriptRoot.FlagReversePrevious;
					int FrameNoPrevious = (true == FlagFirst) ? ScriptRoot.FrameNoStart : (ScriptRoot.FrameNoPrevious + ((true == FlagReverse) ? -1 : 1));
					int FrameNoStart = ScriptRoot.FrameNoStart;
					int FrameNoEnd = ScriptRoot.FrameNoEnd;

					if(false == ScriptRoot.FlagStylePingpong)
					{	/* Play One-Way */
						/* Decoding Skipped Frame */
						if(true == FlagReverse)
						{	/* backwards */
							if((FrameNo > FrameNoPrevious) || (0 < LoopCount))
							{	/* Wrap-Around */
								/* Part-Head */
								UpdateUserDataReverse(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNoStart, false);

								/* Part-Loop */
								for(int j=1; j<LoopCount ; j++)
								{
									UpdateUserDataReverse(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoEnd, FrameNoStart, false);
								}
							
								/* Part-Tail & Just-Now */
								UpdateUserDataReverse(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoEnd, FrameNo, false);
							}
							else
							{	/* Normal */
								UpdateUserDataReverse(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNo, false);
							}
						}
						else
						{	/* foward */
							if((FrameNo < FrameNoPrevious) || (0 < LoopCount))
							{	/* Wrap-Around */
								/* Part-Head */
								UpdateUserDataFoward(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNoEnd, false);

								/* Part-Loop */
								for(int j=1; j<LoopCount; j++)
								{
									UpdateUserDataFoward(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoStart, FrameNoEnd, false);
								}

								/* Part-Tail & Just-Now */
								UpdateUserDataFoward(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoStart, FrameNo, false);
							}
							else
							{	/* Normal */
								UpdateUserDataFoward(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNo, false);
							}
						}
					}
					else
					{	/* Play PingPong */
						bool FlagStyleReverse = (0 != (ScriptRoot.Status & Script_SpriteStudio_PartsRoot.BitStatus.STYLE_REVERSE)) ? true : false;
						bool FlagTurnBackPingPong = ScriptRoot.FlagTurnBackPingPong;

						/* Decoding Skipped Frame */
						if(true == FlagStyleReverse)
						{	/* Reverse */
							if(0 < LoopCount)
							{
								/* Part-Head */
								if(true == FlagReversePrevious)
								{
									FrameNoPrevious = ScriptRoot.FrameNoPrevious - 1;	/* Force */
									UpdateUserDataReverse(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNoStart, false);
									UpdateUserDataFoward(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoStart, FrameNoEnd, true);
								}
								else
								{
									FrameNoPrevious = ScriptRoot.FrameNoPrevious + 1;	/* Force */
									UpdateUserDataFoward(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNoEnd, true);
								}

								/* Part-Loop */
								for(int i=1; i<LoopCount; i++)
								{
									UpdateUserDataReverse(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoEnd, FrameNoStart, false);
									UpdateUserDataFoward(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoStart, FrameNoEnd, true);
								}

								/* Part-Tail & Just-Now */
								if(true == FlagReverse)
								{	/* Now-Reverse */
									UpdateUserDataReverse(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoEnd, FrameNo, false);
								}
								else
								{	/* Now-Foward */
									UpdateUserDataReverse(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoEnd, FrameNoStart, false);
									UpdateUserDataFoward(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoStart, FrameNo, true);
								}
							}
							else
							{	/* Normal */
								if(true == FlagTurnBackPingPong)
								{	/* Turn-Back */
									/* MEMO: No-Loop & Turn-Back ... Always "Reverse to Foward" */
									FrameNoPrevious = ScriptRoot.FrameNoPrevious - 1;	/* Force */
									UpdateUserDataReverse(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNoStart, false);
									UpdateUserDataFoward(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoStart, FrameNo, true);
								}
								else
								{	/* Normal */
									if(true == FlagReverse)
									{	/* Reverse */
										UpdateUserDataReverse(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNo, false);
									}
									else
									{	/* Foward */
										UpdateUserDataFoward(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNo, true);
									}
								}
							}
						}
						else
						{	/* Normal */
							if(0 < LoopCount)
							{
								/* Part-Head */
								if(true == FlagReversePrevious)
								{
									FrameNoPrevious = ScriptRoot.FrameNoPrevious - 1;	/* Force */
									UpdateUserDataReverse(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNoStart, true);
								}
								else
								{
									FrameNoPrevious = ScriptRoot.FrameNoPrevious + 1;	/* Force */
									UpdateUserDataFoward(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNoEnd, false);
									UpdateUserDataReverse(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoEnd, FrameNoStart, true);
								}

								/* Part-Loop */
								for(int i=1; i<LoopCount; i++)
								{
									UpdateUserDataFoward(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoStart, FrameNoEnd, false);
									UpdateUserDataReverse(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoEnd, FrameNoStart, true);
								}
									
								/* Part-Tail & Just-Now */
								if(true == FlagReverse)
								{	/* Now-Reverse */
									UpdateUserDataFoward(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoStart, FrameNoEnd, false);
									UpdateUserDataReverse(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoEnd, FrameNo, true);
								}
								else
								{	/* Now-Foward */
									UpdateUserDataFoward(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoStart, FrameNo, false);
								}
							}
							else
							{	/* Normal */
								if(true == FlagTurnBackPingPong)
								{	/* Turn-Back */
									/* MEMO: No-Loop & Turn-Back ... Always "Foward to Revese" */
									FrameNoPrevious = ScriptRoot.FrameNoPrevious + 1;	/* Force */
									UpdateUserDataFoward(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNoEnd, false);
									UpdateUserDataReverse(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoEnd, FrameNo, true);
								}
								else
								{	/* Normal */
									if(true == FlagReverse)
									{	/* Reverse */
										UpdateUserDataReverse(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNo, true);
									}
									else
									{	/* Foward */
										UpdateUserDataFoward(AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNo, false);
									}
								}
							}
						}
					}
				}
			}
		}
		private void UpdateUserDataFoward(int[] ArrayData, Script_SpriteStudio_PartsRoot ScriptRoot, GameObject InstanceGameObject, int FrameNoStart, int FrameNoEnd, bool FlagTurnBackPingPong)
		{
			int Index = -1;
			KeyFrame.ValueUser.Data UserData = null;
			for(int i=FrameNoStart; i<=FrameNoEnd; i++)
			{
				Index = ArrayData[i];
				UserData = (0 <= Index) ? ArrayDataBodyUser[Index] : KeyFrame.DummyDataUser;
				if(Library_SpriteStudio.KeyFrame.ValueUser.Data.FlagData.CLEAR != UserData.Flag)
				{
					ScriptRoot.CallBackExecUserData(InstanceGameObject.name, this, i, UserData, FlagTurnBackPingPong);
				}
			}
		}
		private void UpdateUserDataReverse(int[] ArrayData, Script_SpriteStudio_PartsRoot ScriptRoot, GameObject InstanceGameObject, int FrameNoStart, int FrameNoEnd, bool FlagTurnBackPingPong)
		{
			int Index = -1;
			KeyFrame.ValueUser.Data UserData = null;
			for(int i=FrameNoStart; i>=FrameNoEnd; i--)
			{
				Index = ArrayData[i];
				UserData = (0 <= Index) ? ArrayDataBodyUser[Index] : KeyFrame.DummyDataUser;
				if(Library_SpriteStudio.KeyFrame.ValueUser.Data.FlagData.CLEAR != UserData.Flag)
				{
					ScriptRoot.CallBackExecUserData(InstanceGameObject.name, this, i, UserData, FlagTurnBackPingPong);
				}
			}
		}
		private void UpdateUserDataJustNow(int[] ArrayData, Script_SpriteStudio_PartsRoot ScriptRoot, GameObject InstanceGameObject, int FrameNo, bool FlagTurnBackPingPong)
		{
			int Index = ArrayData[FrameNo];
			KeyFrame.ValueUser.Data UserData = (0 <= Index) ? ArrayDataBodyUser[Index] : KeyFrame.DummyDataUser;
			if(Library_SpriteStudio.KeyFrame.ValueUser.Data.FlagData.CLEAR != UserData.Flag)
			{
				ScriptRoot.CallBackExecUserData(InstanceGameObject.name, this, FrameNo, UserData, FlagTurnBackPingPong);
			}
		}

		public bool UpdateInstanceData(int FrameNo, GameObject GameObjectNow, Script_SpriteStudio_PartsRoot ScriptRoot, Script_SpriteStudio_PartsInstance PartsInstance)
		{
			KeyFrame.ValueInstance.Data DataBody = null;
			bool FlagIndipendent = false;
			int FrameNoInstanceBase = 0;
			Script_SpriteStudio_PartsRoot ScriptPartsRootSub = PartsInstance.ScriptPartsRootSub;
			bool FlagPlayReverse = (0 != (ScriptRoot.Status & Script_SpriteStudio_PartsRoot.BitStatus.PLAYING_REVERSE)) ? true : false;

			ScriptPartsRootSub.RateOpacity = OpacityRateGet(FrameNo);

			if(0 >= AnimationDataInstance.Length)
			{	/* Error ... Force Play */
				goto UpdateInstanceData_PlayCommand_Force;
			}

			int IndexInstanceBody = AnimationDataInstance[FrameNo];
			DataBody = ((0 <= IndexInstanceBody) && (0 < ArrayDataBodyInstance.Length)) ? ArrayDataBodyInstance[IndexInstanceBody] : KeyFrame.DummyDataInstance;
			FrameNoInstanceBase = DataBody.FrameNoBase;
			if(-1 == FrameNoInstanceBase)
			{
				return(false);
			}

			FlagIndipendent = (0 != (DataBody.Flag & KeyFrame.ValueInstance.Data.FlagData.INDEPENDENT)) ? true : false;
			int FramePreviousUpdateInstance = PartsInstance.FrameNoPreviousUpdate;
			if(false == FlagIndipendent)
			{	/* Non-Indipendent */
				if(0 != (ScriptRoot.Status & Script_SpriteStudio_PartsRoot.BitStatus.REDECODE_INSTANCE))
				{
					PartsInstance.FrameNoPreviousUpdate = FramePreviousUpdateInstance = -1;
				}
			}
			if(-1 == FramePreviousUpdateInstance)
			{
				goto UpdateInstanceData_PlayCommand_Initial;
			}
			if(FrameNoInstanceBase != FramePreviousUpdateInstance)
			{	/* New Attribute */
				goto UpdateInstanceData_PlayCommand_Update;
			}

			return(true);

		UpdateInstanceData_PlayCommand_Force:;
			{
				if(0 != (ScriptRoot.Status & Script_SpriteStudio_PartsRoot.BitStatus.REDECODE_INSTANCE))
				{
					PartsInstance.FrameNoPreviousUpdate = -1;
				}
				if(-1 == PartsInstance.FrameNoPreviousUpdate)
				{
					DataBody = new KeyFrame.ValueInstance.Data();
#if false
					if(true == UpdateInstanceCheckRange(ScriptPartsRootSub, FrameNo - FrameNoInstanceBase, PartsInstance.AnimationNo, DataBody))
					{
						goto UpdateInstanceData_PlayCommand_Update;
					}
#else
					goto UpdateInstanceData_PlayCommand_Update;
#endif
				}
			}
			return(true);

		UpdateInstanceData_PlayCommand_Initial:;
			if(false == UpdateInstanceCheckRange(ScriptPartsRootSub, FrameNo - FrameNoInstanceBase, PartsInstance.AnimationNo, DataBody))
			{
				return((-1 == FramePreviousUpdateInstance) ? false : true);
			}
		UpdateInstanceData_PlayCommand_Update:;
			{
				float RateTime = DataBody.RateTime;
				RateTime *= (true == FlagPlayReverse) ? -1.0f : 1.0f;
				ScriptPartsRootSub.AnimationPlay(	PartsInstance.AnimationNo,
													DataBody.PlayCount,
													0,
													((0 != (DataBody.Flag & KeyFrame.ValueInstance.Data.FlagData.INDEPENDENT)) ? RateTime : RateTime * ScriptRoot.RateTimePlay),
													((0 != (DataBody.Flag & KeyFrame.ValueInstance.Data.FlagData.PINGPONG)) ? Script_SpriteStudio_PartsRoot.PlayStyle.PINGPONG : Script_SpriteStudio_PartsRoot.PlayStyle.NORMAL),
													DataBody.LabelStart,
													DataBody.OffsetStart,
													DataBody.LabelEnd,
													DataBody.OffsetEnd
												);

				int FrameCount = FrameNo - FrameNoInstanceBase;
				FrameCount = (0 > FrameCount) ? 0 : FrameCount;
				ScriptPartsRootSub.TimeElapsedSetForce(FrameCount * ScriptRoot.TimeFramePerSecond, FlagIndipendent);
				PartsInstance.FrameNoPreviousUpdate = FrameNoInstanceBase;
				ScriptPartsRootSub.AnimationPause(false);
			}
			return(true);
		}
		private bool UpdateInstanceCheckRange(Script_SpriteStudio_PartsRoot PartsRoot, int FrameCountNow, int AnimationNo, KeyFrame.ValueInstance.Data DataBody)
		{
			if(0 == DataBody.PlayCount)
			{
				return(true);
			}
			AnimationInformationPlay Information = PartsRoot.ListInformationPlay[AnimationNo];
			int FrameCount = -1;
			int FrameNoStart = -1;
			int FrameNoEnd = -1;
			Information.RangeGet(	ref FrameCount,
									ref FrameNoStart,
									ref FrameNoEnd,
									DataBody.LabelStart,
									DataBody.OffsetStart,
									DataBody.LabelEnd,
									DataBody.OffsetEnd
								);
			int FrameCountRate = (int)(((float)FrameCount * (float)(Mathf.Abs(DataBody.PlayCount))) / Mathf.Abs(DataBody.RateTime));
			return((((FrameCountNow - FrameNoStart) >= FrameCountRate) || (0 > FrameCountNow)) ? false : true);
		}

		public void DrawEntryInstance(Library_SpriteStudio.DrawManager.InformationMeshData MeshDataInformation, int FrameNo, Script_SpriteStudio_PartsRoot ScriptRoot)
		{
			float Priority = PriorityGet(FrameNo);

			MeshDataInformation.Priority = PriorityGet(Priority, ID);
			Library_SpriteStudio.DrawManager.ArrayListMeshDraw ArrayListMeshDraw = ScriptRoot.ArrayListMeshDraw;
			if(null != ArrayListMeshDraw)
			{
				ArrayListMeshDraw.MeshAdd(null, MeshDataInformation);
			}
		}

		public void DrawEntry(Library_SpriteStudio.DrawManager.InformationMeshData MeshDataInformation, int FrameNo, Script_SpriteStudio_PartsRoot ScriptRoot)
		{
			if(false == ScriptRoot.FlagHideForce)
			{
				float Priority = PriorityGet(FrameNo);
				int TextureNo = -1;
				switch(KindDataFormat)
				{
					case KindAnimationData.PLAIN:
						{
							int IndexCell = (0 < DataPlain.AnimationDataCell.Length) ? DataPlain.AnimationDataCell[FrameNo] : -1;
							KeyFrame.ValueCell.Data DataBodyCell = ((0 <= IndexCell) && (0 < DataPlain.ArrayDataBodyCell.Length)) ? DataPlain.ArrayDataBodyCell[IndexCell] : KeyFrame.DummyDataCell;
							TextureNo = DataBodyCell.TextureNo;
						}
						break;
					case KindAnimationData.FIX:
						TextureNo = AnimationDataFlags[FrameNo].TextureNo;
						break;
					default:
						break;
				}
				MeshDataInformation.Priority = PriorityGet(Priority, ID);
				Library_SpriteStudio.DrawManager.ArrayListMeshDraw ArrayListMeshDraw = ScriptRoot.ArrayListMeshDraw;
				if(null != ArrayListMeshDraw)
				{
					Material MaterialNow = ScriptRoot.MaterialGet(TextureNo, KindBlendTarget);
					if(null == MaterialNow)
					{	/* has Illegal-Material */
						return;
					}
					ArrayListMeshDraw.MeshAdd(MaterialNow, MeshDataInformation);
				}
			}
		}

		public static float PriorityGet(float Priority, int ID)
		{
			return(Mathf.Floor(Priority) + ((float)ID * 0.001f));
		}

		[System.Serializable]
		public class Plain
		{
			public KeyFrame.ValueColor[] AnimationDataColorBlend;
			public CompressedValueColorArray CompressedAnimationDataColorBlend;

			public KeyFrame.ValueQuadrilateral[] AnimationDataVertexCorrection;
			public Vector2[] AnimationDataOriginOffset;

			public Vector2[] AnimationDataAnchorPosition;
			public Vector2[] AnimationDataAnchorSize;

			public Vector2[] AnimationDataTextureTranslate;
			public float[] AnimationDataTextureRotate;
			public Vector2[] AnimationDataTextureScale;
			public Vector2[] AnimationDataTextureExpand;

			public float[] AnimationDataCollisionRadius;	/* for Sphere-Collider */

			public int[] AnimationDataCell;

			public KeyFrame.ValueCell.Data[] ArrayDataBodyCell;

			public Plain()
			{
				AnimationDataColorBlend = null;
				CompressedAnimationDataColorBlend = null;
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

				ArrayDataBodyCell = null;
			}

			public void Compress()
			{
				if (AnimationDataColorBlend != null && AnimationDataColorBlend.Length > 0)
				{
					CompressedAnimationDataColorBlend = CompressedValueColorArray.Build(AnimationDataColorBlend);
					AnimationDataColorBlend = null;
				}
			}
			public void Decompress()
			{
				if (CompressedAnimationDataColorBlend != null && CompressedAnimationDataColorBlend.Length > 0)
				{
					AnimationDataColorBlend = CompressedAnimationDataColorBlend.ToArray();
					CompressedAnimationDataColorBlend = null;
				}
			}

			public void UpdateCollider(Collider ComponentCollider, KindCollision CollisionKind, float ColliderSizeZ, KeyFrame.ValueBools[] AnimationDataFlags, int FrameNo, bool FlagAnimationDataFlags, WorkAreaRuntime WorkArea)
			{
				switch(CollisionKind)
				{
					case KindCollision.SQUARE:
						{
							/* Calculate Sprite-Parts size */
							Vector2 SizeNew = Vector2.one;
							Vector2 PivotNew = Vector2.zero;
							int IndexCell = AnimationDataCell[FrameNo];
							KeyFrame.ValueCell.Data DataBody = ((null !=ArrayDataBodyCell) && (0 < ArrayDataBodyCell.Length) && (0 <= IndexCell)) ? ArrayDataBodyCell[IndexCell] : KeyFrame.DummyDataCell;
							{
								Rect RectCell = Rect.MinMaxRect(0.0f, 0.0f, 64.0f, 64.0f);
								Vector2 PivotCollide = Vector2.zero;
								Vector2 PivotCell = Vector2.zero;
								if(0 < AnimationDataCell.Length)
								{
									RectCell = DataBody.Rectangle;
									PivotCell = DataBody.Pivot;
									PivotCell.x -= (RectCell.width * 0.5f);
									PivotCell.y -= (RectCell.height * 0.5f);
								}

								Vector2 RateScaleMesh = Vector2.one;
								if(true == FlagAnimationDataFlags)
								{
									RateScaleMesh.x = (true == AnimationDataFlags[FrameNo].IsFlipX) ? -1.0f : 1.0f;
									RateScaleMesh.y = (true == AnimationDataFlags[FrameNo].IsFlipY) ? -1.0f : 1.0f;
								}

								/* Accommodate Pivot's-Offset */
								SpriteRecalcSizeAndPivot(ref PivotCollide, ref RectCell, ref RateScaleMesh, FrameNo);
								PivotNew.x = -(PivotCollide.x + PivotCell.x) * RateScaleMesh.x;
								PivotNew.y = (PivotCollide.y + PivotCell.y) * RateScaleMesh.y;

								/* Get Collision-Size */
								SizeNew.x = RectCell.width;
								SizeNew.y = RectCell.height;
							}

							if((PivotNew != WorkArea.ColliderRectPivotPrevious) || (SizeNew != WorkArea.ColliderRectSizePrevious))
							{	/* Update */
								/* Update Previous Buffer */
								WorkArea.ColliderRectPivotPrevious = PivotNew;
								WorkArea.ColliderRectSizePrevious = SizeNew;

								/* Update Collider */
								BoxCollider InstanceCollider = ComponentCollider as BoxCollider;
								InstanceCollider.enabled = true;
								InstanceCollider.size = new Vector3(SizeNew.x, SizeNew.y, ColliderSizeZ);
								InstanceCollider.center = new Vector3(PivotNew.x, PivotNew.y, 0.0f);
							}
						}
						break;

					case KindCollision.CIRCLE:
						{
							float RadiusNew = ((null != AnimationDataCollisionRadius) && (0 < AnimationDataCollisionRadius.Length)) ? AnimationDataCollisionRadius[FrameNo] : 1.0f;
							if(RadiusNew != WorkArea.ColliderRadiusPrevious)
							{	/* Update */
								/* Update Previous Buffer */
								WorkArea.ColliderRadiusPrevious = RadiusNew;

								/* Update Collider */
								CapsuleCollider InstanceCollider = ComponentCollider as CapsuleCollider;
								InstanceCollider.enabled = true;
								InstanceCollider.radius = WorkArea.ColliderRadiusPrevious;
								InstanceCollider.center = Vector3.zero;
							}
						}
						break;

					default:
						break;
				}
			}


			public bool AnimationDataColorBlendIsNullOrEmpty
			{
				get
				{
					if ((null != CompressedAnimationDataColorBlend) && (0 < CompressedAnimationDataColorBlend.Length))
					{
						return false;
					}
					if ((null != AnimationDataColorBlend) && (0 < AnimationDataColorBlend.Length))
					{
						return false;
					}
					return true;
				}
			}

			public KeyFrame.ValueColor AnimationDataColorBlendGet(int FrameNo)
			{
				if ((null != CompressedAnimationDataColorBlend) && (0 < CompressedAnimationDataColorBlend.Length))
				{
					return CompressedAnimationDataColorBlend[FrameNo];
				}
				return AnimationDataColorBlend[FrameNo];
			}

			public void UpdateMesh(Mesh MeshNow, KeyFrame.ValueBools[] AnimationDataFlags, float RateOpacity, int FrameNo, Script_SpriteStudio_PartsRoot ScriptRoot)
			{
				Matrix4x4 MatrixTexture = Matrix4x4.identity;
				Vector2 SizeTexture = Vector2.one;
				Vector2 RateScaleTexture = Vector2.one;
				Vector2 PivotTexture = Vector2.zero;
				Vector2 RateScaleMesh = Vector2.one;
				Vector2 PivotMesh = Vector2.zero;
				Rect RectCell = Rect.MinMaxRect(0.0f, 0.0f, 64.0f, 64.0f);
				int	VertexCollectionIndexTableNo = 0;

				/* Main-Texture Data Get */
				int IndexCell = AnimationDataCell[FrameNo];
				KeyFrame.ValueCell.Data DataBodyCell = ((null != ArrayDataBodyCell) && (0 < ArrayDataBodyCell.Length) && (0 <= IndexCell) && (0 < ArrayDataBodyCell.Length)) ? ArrayDataBodyCell[IndexCell] : KeyFrame.DummyDataCell;
				SizeTexture.x = DataBodyCell.SizeOriginal.x;
				SizeTexture.y = DataBodyCell.SizeOriginal.y;

				/* Cell-Data Get */
				RectCell = DataBodyCell.Rectangle;
				PivotTexture = new Vector2(RectCell.width * 0.5f, RectCell.height * 0.5f);

				PivotMesh = DataBodyCell.Pivot;

				/* Disolve Flipping & Texture-Scaling */
				if((null != AnimationDataFlags) && (0 < AnimationDataFlags.Length))
				{
					RateScaleTexture.x = (true == AnimationDataFlags[FrameNo].IsTextureFlipX) ? -1.0f : 1.0f;
					RateScaleTexture.y = (true == AnimationDataFlags[FrameNo].IsTextureFlipY) ? -1.0f : 1.0f;
					if(true == AnimationDataFlags[FrameNo].IsFlipX)
					{
						RateScaleMesh.x *= -1.0f;
						VertexCollectionIndexTableNo += 1;
					}
					else
					{
						RateScaleMesh.x *= 1.0f;
					}
					if(true == AnimationDataFlags[FrameNo].IsFlipY)
					{
						RateScaleMesh.y *= -1.0f;
						VertexCollectionIndexTableNo += 2;
					}
					else
					{
						RateScaleMesh.y *= 1.0f;
					}
				}
				if((null != AnimationDataTextureScale) && (0 < AnimationDataTextureScale.Length))
				{
					RateScaleTexture.x *= AnimationDataTextureScale[FrameNo].x;
					RateScaleTexture.y *= AnimationDataTextureScale[FrameNo].y;
				}

				/* Calculate Matrix-Texture */
				float Rotate = ((null != AnimationDataTextureRotate) && (0 < AnimationDataTextureRotate.Length)) ? AnimationDataTextureRotate[FrameNo] :  0.0f;
				Vector2 TextureOffset = ((null != AnimationDataTextureTranslate) && (0 < AnimationDataTextureTranslate.Length)) ? AnimationDataTextureTranslate[FrameNo] : Vector2.zero;
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

				RateOpacity *= ScriptRoot.RateOpacity;

				Vector2[] DataUV2 = new Vector2[CountVertexData];
				Color32[] DataColor32 = new Color32[CountVertexData];
				bool FlagExistAnimationDataColorBlend = !AnimationDataColorBlendIsNullOrEmpty;
				Script_SpriteStudio_PartsRoot.ColorBlendOverwrite DataColorBlendOverwrite = ScriptRoot.DataColorBlendOverwrite;
				if(null == DataColorBlendOverwrite)
				{	/* No-Overwrite */
					goto UpdateMesh_ColorBlend_Animation;
				}
				else
				{	/* Overwrite */
					if(Library_SpriteStudio.KindColorBound.NON != DataColorBlendOverwrite.Bound)
					{	/* Overwite */
						for(int i=0; i<CountVertexData; i++)
						{
							DataUV2[i] = new Vector2(	RateOpacity,
														(float)DataColorBlendOverwrite.Operation + 0.01f	/* "+0.01f" for Rounding-off-Error */
													);
							DataColor32[i] = DataColorBlendOverwrite.VertexColor[i];
						}
						goto UpdateMesh_ColorBlend_End;
					}
					else
					{	/* Default (Same as "No-Overwrite" ) */
						goto UpdateMesh_ColorBlend_Animation;
					}
				}

			UpdateMesh_ColorBlend_Animation:;
				if(true == FlagExistAnimationDataColorBlend)	/* Blending-Color & Opacity*/
				{	/* Animation-Data */
					if(Library_SpriteStudio.KindColorBound.NON != AnimationDataColorBlendGet(FrameNo).Bound)
					{
						for(int i=0; i<CountVertexData; i++)
						{
							DataUV2[i] = new Vector2(	AnimationDataColorBlendGet(FrameNo).RatePixelAlpha[i] * RateOpacity,
														(float)AnimationDataColorBlendGet(FrameNo).Operation + 0.01f	/* "+0.01f" for Rounding-off-Error */
													);
							DataColor32[i] = AnimationDataColorBlendGet(FrameNo).VertexColor[i];
						}
					}
					else
					{	/* Default (Same as "No Datas" ) */
						goto UpdateMesh_ColorBlend_NoData;
					}
				}
				else
				{	/* Default (No Datas) */
					goto UpdateMesh_ColorBlend_NoData;
				}
				goto UpdateMesh_ColorBlend_End;

			UpdateMesh_ColorBlend_NoData:;
				{
					Color32 ColorDefault = Color.white;
					float OperationDefault = (float)KindColorOperation.NON + 0.01f;	/* "+0.01f" for Rounding-off-Error */
					for(int i=0; i<CountVertexData; i++)
					{
						DataUV2[i] = new Vector2(RateOpacity, OperationDefault);
						DataColor32[i] = ColorDefault;
					}
				}
/*				goto UpdateMesh_ColorBlend_End;	*/	/* Fall-Through */

			UpdateMesh_ColorBlend_End:;

				MeshNow.colors32 = DataColor32;
				MeshNow.uv2 = DataUV2;

				Vector3[] DataCoordinate = new Vector3[CountVertexData];
				if((int)VertexNo.TERMINATOR4 == CountVertexData)	/* Vertex-Coordinate */
				{	/* 4-Triangles Mesh */
					/* Get SpriteSize & Pivot */
					SpriteRecalcSizeAndPivot(ref PivotMesh, ref RectCell, ref RateScaleMesh, FrameNo);

					/* Get Coordinates */
					/* MEMO: No Check "AnimationDataVertexCorrection.Length", 'cause 4-Triangles-Mesh necessarily has "AnimationDataVertexCorrection" */
					float Left = (-PivotMesh.x) * RateScaleMesh.x;
					float Right = (RectCell.width - PivotMesh.x) * RateScaleMesh.x;
					float Top = (-PivotMesh.y) * RateScaleMesh.y;
					float Bottom = (RectCell.height - PivotMesh.y) * RateScaleMesh.y;

					DataCoordinate[(int)VertexNo.LU] = new Vector3(	Left + AnimationDataVertexCorrection[FrameNo].Coordinate[VertexCollectionOrderVertex[VertexCollectionIndexTableNo, (int)VertexNo.LU]].x,
																	-Top + AnimationDataVertexCorrection[FrameNo].Coordinate[VertexCollectionOrderVertex[VertexCollectionIndexTableNo, (int)VertexNo.LU]].y,
																	0.0f
																);
					DataCoordinate[(int)VertexNo.RU] = new Vector3(	Right + AnimationDataVertexCorrection[FrameNo].Coordinate[VertexCollectionOrderVertex[VertexCollectionIndexTableNo, (int)VertexNo.RU]].x,
																	-Top + AnimationDataVertexCorrection[FrameNo].Coordinate[VertexCollectionOrderVertex[VertexCollectionIndexTableNo, (int)VertexNo.RU]].y,
																	0.0f
																);
					DataCoordinate[(int)VertexNo.RD] = new Vector3(	Right + AnimationDataVertexCorrection[FrameNo].Coordinate[VertexCollectionOrderVertex[VertexCollectionIndexTableNo, (int)VertexNo.RD]].x,
																	-Bottom + AnimationDataVertexCorrection[FrameNo].Coordinate[VertexCollectionOrderVertex[VertexCollectionIndexTableNo, (int)VertexNo.RD]].y,
																	0.0f
																);
					DataCoordinate[(int)VertexNo.LD] = new Vector3(	Left + AnimationDataVertexCorrection[FrameNo].Coordinate[VertexCollectionOrderVertex[VertexCollectionIndexTableNo, (int)VertexNo.LD]].x,
																	-Bottom + AnimationDataVertexCorrection[FrameNo].Coordinate[VertexCollectionOrderVertex[VertexCollectionIndexTableNo, (int)VertexNo.LD]].y,
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
					/* Get SpriteSize & Pivot */
					SpriteRecalcSizeAndPivot(ref PivotMesh, ref RectCell, ref RateScaleMesh, FrameNo);

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

			public void SpriteRecalcSizeAndPivot(ref Vector2 PivotMesh, ref Rect RectCell, ref Vector2 RateScaleMesh, int FrameNo)
			{
				Vector2 PivotOffset = ((null != AnimationDataOriginOffset) && (0 < AnimationDataOriginOffset.Length)) ? AnimationDataOriginOffset[FrameNo] : Vector2.zero;
				PivotMesh.x += (RectCell.width * PivotOffset.x) * RateScaleMesh.x;
				PivotMesh.y -= (RectCell.height * PivotOffset.y) * RateScaleMesh.y;

				/* Arbitrate Anchor-Size */
				if((null !=  AnimationDataAnchorSize) && (0 < AnimationDataAnchorSize.Length))
				{
					float RatePivot;
					Vector2 AnchorSize = AnimationDataAnchorSize[FrameNo];
					if(0.0f <= AnchorSize.x)
					{
						RatePivot = PivotMesh.x / RectCell.width;
						RectCell.x = 0.0f;
						RectCell.width = AnchorSize.x;
						PivotMesh.x = AnchorSize.x * RatePivot;
					}
					if(0.0f <= AnchorSize.y)
					{
						RatePivot = PivotMesh.y / RectCell.height;
						RectCell.y = 0.0f;
						RectCell.height = AnchorSize.y;
						PivotMesh.y = AnchorSize.y * RatePivot;
					}
				}
			}
		}

		[System.Serializable]
		public class Fix
		{
			[System.Serializable]
			public class MeshAlies
			{
				public Vector3[] Coordinate;
				public Color32[] ColorOverlay;
				public Vector2[] UV;
				public Vector2[] UV2;

				public MeshAlies()
				{
					Coordinate = null;
					ColorOverlay = null;
					UV = null;
					UV2 = null;
				}

				public MeshAlies(int Count)
				{
					Coordinate = new Vector3[Count];
					ColorOverlay = new Color32[Count];
					UV = new Vector2[Count];
					UV2 = new Vector2[Count];
				}

				public bool Equals(MeshAlies rhs)
				{
					if (this.Coordinate.Length != rhs.Coordinate.Length)
					{
						return false;
					}
					for(int i=0 ; i<this.Coordinate.Length ; ++i)
					{
						if (this.Coordinate[i] != rhs.Coordinate[i])
						{
							return false;
						}
					}
					if (this.ColorOverlay.Length != rhs.ColorOverlay.Length)
					{
						return false;
					}
					for(int i=0 ; i<this.ColorOverlay.Length ; ++i)
					{
						if (this.ColorOverlay[i].r != rhs.ColorOverlay[i].r)
						{
							return false;
						}
						if (this.ColorOverlay[i].g != rhs.ColorOverlay[i].g)
						{
							return false;
						}
						if (this.ColorOverlay[i].b != rhs.ColorOverlay[i].b)
						{
							return false;
						}
						if (this.ColorOverlay[i].a != rhs.ColorOverlay[i].a)
						{
							return false;
						}
					}
					if (this.UV.Length != rhs.UV.Length)
					{
						return false;
					}
					for(int i=0 ; i<this.UV.Length ; ++i)
					{
						if (this.UV[i] != rhs.UV[i])
						{
							return false;
						}
					}
					if (this.UV2.Length != rhs.UV2.Length)
					{
						return false;
					}
					for(int i=0 ; i<this.UV2.Length ; ++i)
 					{
						if (this.UV2[i] != rhs.UV2[i])
						{
							return false;
						}
					}
					return true;
				}
			};

			public MeshAlies[] AnimationDataMesh;
			public CompressedMeshArray CompressedAnimationDataMesh;

			public Vector2[] AnimationDataCollisionSize;	/* for Box-Collider */
			public Vector2[] AnimationDataCollisionPivot;	/* for Box-Collider */
			public float[] AnimationDataCollisionRadius;	/* for Sphere-Collider */

			public Fix()
			{
				AnimationDataMesh = null;
				CompressedAnimationDataMesh = null;

				AnimationDataCollisionSize = null;
				AnimationDataCollisionPivot = null;
				AnimationDataCollisionRadius = null;
			}

			public void Compress()
			{
				if (AnimationDataMesh != null && AnimationDataMesh.Length > 0)
				{
					CompressedAnimationDataMesh = CompressedMeshArray.Build(AnimationDataMesh);
					AnimationDataMesh = null;
				}
			}
			public void Decompress()
			{
				if (CompressedAnimationDataMesh != null && CompressedAnimationDataMesh.Length > 0)
				{
					AnimationDataMesh = CompressedAnimationDataMesh.ToArray();
					CompressedAnimationDataMesh = null;
				}
			}

			public bool AnimationDataMeshIsNull
			{
				get
				{
					if (CompressedAnimationDataMesh != null)
					{
						return false;
 					}
					if (AnimationDataMesh != null)
					{
						return false;
					}
					return true;
				}
			}

			public void UpdateCollider(Collider ComponentCollider, KindCollision CollisionKind, float ColliderSizeZ, KeyFrame.ValueBools[] AnimationDataFlags, int FrameNo, bool FlagAnimationDataFlags, WorkAreaRuntime WorkArea)
			{
				/* Collider-Setting */
				switch(CollisionKind)
				{
					case KindCollision.SQUARE:
						{
							BoxCollider InstanceCollider = ComponentCollider as BoxCollider;
							Vector2 SizeNew = Vector3.one;
							Vector2 PivotNew = Vector3.zero;
							if((null != AnimationDataCollisionSize) && (0 < AnimationDataCollisionSize.Length))
							{
								SizeNew = AnimationDataCollisionSize[FrameNo];
							}
							if((null != AnimationDataCollisionPivot) && (0 < AnimationDataCollisionPivot.Length))
							{
								PivotNew = AnimationDataCollisionPivot[FrameNo];
							}

							if((PivotNew != WorkArea.ColliderRectPivotPrevious) || (SizeNew != WorkArea.ColliderRectSizePrevious))
							{	/* Update */
								/* Update Previous Buffer */
								WorkArea.ColliderRectPivotPrevious = PivotNew;
								WorkArea.ColliderRectSizePrevious = SizeNew;

								/* Update Collider */
								InstanceCollider.enabled = true;
								InstanceCollider.size = new Vector3(SizeNew.x, SizeNew.y, ColliderSizeZ);
								InstanceCollider.center = new Vector3(PivotNew.x, PivotNew.y, 0.0f);
							}
						}
						break;

					case KindCollision.CIRCLE:
						{
							CapsuleCollider InstanceCollider = ComponentCollider as CapsuleCollider;
							float RadiusNew = 1.0f;
							if((null != AnimationDataCollisionRadius) && (0 < AnimationDataCollisionRadius.Length))
							{
								RadiusNew = AnimationDataCollisionRadius[FrameNo];
							}
							if(RadiusNew != WorkArea.ColliderRadiusPrevious)
							{	/* Update */
								/* Update Previous Buffer */
								WorkArea.ColliderRadiusPrevious = RadiusNew;

								/* Update Collider */
								InstanceCollider.enabled = true;
								InstanceCollider.radius = WorkArea.ColliderRadiusPrevious;
								InstanceCollider.center = Vector3.zero;
							}
						}
						break;

					default:
						break;
				}
			}

			public MeshAlies AnimationDataMeshGet(int FrameNo)
			{
				if (CompressedAnimationDataMesh != null && CompressedAnimationDataMesh.Length > 0)
				{
					return CompressedAnimationDataMesh[FrameNo];
				};
				return AnimationDataMesh[FrameNo];
			}

			public void UpdateMesh(SmartMesh MeshNow, KeyFrame.ValueBools[] AnimationDataFlags, int FrameNo, Script_SpriteStudio_PartsRoot ScriptRoot)
			{
				float RateOpacity = (null != ScriptRoot) ? ScriptRoot.RateOpacity : 1.0f;
				MeshAlies DataMeshAlies = AnimationDataMeshGet(FrameNo);
				Vector2[] DataUV2 = new Vector2[DataMeshAlies.UV2.Length];
				Script_SpriteStudio_PartsRoot.ColorBlendOverwrite DataColorBlendOverwrite = ScriptRoot.DataColorBlendOverwrite;
				if(null == DataColorBlendOverwrite)
				{	/* No-Overwrite */
					goto UpdateMesh_ColorBlend_Animation;
				}
				else
				{	/* Overwrite */
					int CountVertexData = DataMeshAlies.ColorOverlay.Length;
					Color32[] DataColor32 = new Color32[CountVertexData];
					if(Library_SpriteStudio.KindColorBound.NON != DataColorBlendOverwrite.Bound)
					{	/* Overwite */
						for(int i=0; i<CountVertexData; i++)
						{
							DataUV2[i] = new Vector2(	RateOpacity,
														(float)DataColorBlendOverwrite.Operation + 0.01f	/* "+0.01f" for Rounding-off-Error */
													);
							DataColor32[i] = DataColorBlendOverwrite.VertexColor[i];
						}
						MeshNow.ColorSet(DataColor32);
					}
					else
					{	/* Default (Same as "No-Overwrite" ) */
						goto UpdateMesh_ColorBlend_Animation;
					}
				}
				goto UpdateMesh_ColorBlend_End;

			UpdateMesh_ColorBlend_Animation:;
				if (RateOpacity == 1f)
				{
					DataUV2 = DataMeshAlies.UV2;
				}
				else
				{
					for(int i=0; i<DataUV2.Length; i++)
					{
						DataUV2[i] = DataMeshAlies.UV2[i];
						DataUV2[i].x *= RateOpacity;
					}
				}
				MeshNow.ColorSet(DataMeshAlies.ColorOverlay);
/*				goto UpdateMesh_ColorBlend_End;	*/	/* Fall-Through */

			UpdateMesh_ColorBlend_End:;
				MeshNow.VerticesSet(DataMeshAlies.Coordinate);
				MeshNow.UVSet(DataMeshAlies.UV);
				MeshNow.UV2Set(DataUV2);
			}
		}

		public class WorkAreaRuntime
		{
			/* Buffer for Runtime-Speed-Optimize */
			internal float ColliderRadiusPrevious = -1.0f;	/* for Radius-Collision */
			internal Vector2 ColliderRectSizePrevious = Vector2.zero;	/* for Rectangle-Collision */
			internal Vector2 ColliderRectPivotPrevious = Vector2.zero;	/* for Rectangle-Collision */

			public WorkAreaRuntime()
			{
				ColliderRadiusPrevious = -1.0f;
				ColliderRectSizePrevious = Vector2.zero;
				ColliderRectPivotPrevious = Vector2.zero;
			}
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
			if(null != functionOnTriggerEnter)
			{
				functionOnTriggerEnter(collider, Pair);
			}
		}

		void OnTriggerExit(Collider Pair)
		{
			if(null != functionOnTriggerEnd)
			{
				functionOnTriggerEnter(collider, Pair);
			}
		}

		void OnTriggerStay(Collider Pair)
		{
			if(null != functionOnTriggerStay)
			{
				functionOnTriggerStay(collider, Pair);
			}
		}

		void OnCollisionEnter(Collision Contacts)
		{
			if(null != functionOnCollisionEnter)
			{
				functionOnCollisionEnter(collider, Contacts);
			}
		}

		void OnCollisionExit(Collision Contacts)
		{
			if(null != functionOnCollisionEnd)
			{
				functionOnCollisionEnd(collider, Contacts);
			}
		}

		void OnCollisionStay(Collision Contacts)
		{
			if(null != functionOnCollisionStay)
			{
				functionOnCollisionStay(collider, Contacts);
			}
		}
	}

	public class SmartMesh
	{
		public Mesh Mesh{get; protected set;}
		Vector2[] _uv = new Vector2[0];
		Vector2[] _uv2 = new Vector2[0];
		Color32[] _colors = new Color32[0];
		Vector3[] _vertices = new Vector3[0];

		public SmartMesh()
		{
			Mesh = new Mesh();
		}

		public void UV2Set(Vector2[] uv2)
		{
			if (_uv2 == uv2)
			{
				return;
			}
			_uv2 = uv2;
			Mesh.uv2 = uv2;
		}

		public void ColorSet(Color32[] colors)
		{
			if (_colors == colors)
			{
				return;
			}
			_colors = colors;
			Mesh.colors32 = colors;
		}

		public void VerticesSet(Vector3[] vertices)
		{
			if (_vertices == vertices)
			{
				return;
			}
			_vertices = vertices;
			Mesh.vertices = vertices;
		}
		public void UVSet(Vector2[] uv)
		{
			if (_uv == uv)
			{
				return;
			}
			_uv = uv;
			Mesh.uv = uv;
		}
	
	}

	[System.Serializable]
	public class SpriteBase : PartsBase
	{

		protected SmartMesh dataMesh;
		public SmartMesh DataMesh
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

		protected Library_SpriteStudio.DrawManager.InformationMeshData DataMeshInformation = null;
	}

	[System.Serializable]
	public class CompressedVector3Array
	{
		[SerializeField]
		int[] _indices;
		
		[SerializeField]
		Vector3[] _values;
		
		static public CompressedVector3Array Build(Vector3[] src)
		{
			if (src == null)
			{
				return null;
			}
			var result = new CompressedVector3Array(src);
			
			return result;
		}
		
		CompressedVector3Array(Vector3[] src)
		{
			if (src == null)
			{
				return;
			}
			var indices = new List<int>();
			var values = new List<Vector3>();
			
			values.Add(src[0]);
			for(int i=1; i<src.Length; ++i)
			{
				if (!values[values.Count - 1].Equals(src[i]))
				{
					values.Add(src[i]);
					indices.Add(i);
				}
			}
			indices.Add(src.Length);
			
			_indices = indices.ToArray();
			_values = values.ToArray();
		}
		
		public Vector3 this [int index]
		{
			get
			{
				var i = System.Array.BinarySearch<int>(_indices, index);
				if (i < 0)
				{
					i = ~i;
				}
				else
				{
					++i;
				}
				return _values[i];
			}
		}
		
		public int Length
		{
			get
			{
				if (_indices == null)
				{
					return 0;
				}
				if (_indices.Length == 0)
				{
					return 0;
				}
				return _indices[_indices.Length - 1];
			}
		}
		public Vector3[] ToArray()
		{
			var result = new Vector3[Length];
			for(int i=0; i<Length; ++i)
			{
				result[i] = this[i];
			}
			return result;
		}
	}
	
	[System.Serializable]
	public class CompressedFloatArray
	{
		[SerializeField]
		int[] _indices;
		
		[SerializeField]
		float[] _values;
		
		static public CompressedFloatArray Build(float[] src)
		{
			if (src == null)
			{
				return null;
			}
			var result = new CompressedFloatArray(src);
			
			return result;
		}
		
		CompressedFloatArray(float[] src)
		{
			if (src == null)
			{
				return;
			}
			Set(src);
		}
		
		public void Set(float[] src)
		{
			var indices = new List<int>();
			var values = new List<float>();
			
			values.Add(src[0]);
			for(int i=1; i<src.Length; ++i)
			{
				if (values[values.Count - 1] != src[i])
				{
					values.Add(src[i]);
					indices.Add(i);
				}
			}
			indices.Add(src.Length);
			
			_indices = indices.ToArray();
			_values = values.ToArray();
		}
		
		public float this [int index]
		{
			get
			{
				var i = System.Array.BinarySearch<int>(_indices, index);
				if (i < 0)
				{
					i = ~i;
				}
				else
				{
					++i;
				}
				return _values[i];
			}
		}
		
		public int Length
		{
			get
			{
				if (_indices == null)
				{
					return 0;
				}
				if (_indices.Length == 0)
				{
					return 0;
				}
				return _indices[_indices.Length - 1];
			}
		}
		
		public float[] ToArray()
		{
			var result = new float[Length];
			for(int i=0; i<Length; ++i)
			{
				result[i] = this[i];
			}
			return result;
		}
	}
	
	[System.Serializable]
	public class CompressedVector2Array
	{
		[SerializeField]
		int[] _indices;
		
		[SerializeField]
		Vector2[] _values;
		
		static public CompressedVector2Array Build(Vector2[] src)
		{
			if (src == null)
			{
				return null;
			}
			var result = new CompressedVector2Array(src);
			
			return result;
		}
		
		CompressedVector2Array(Vector2[] src)
		{
			if (src == null)
			{
				return;
			}
			var indices = new List<int>();
			var values = new List<Vector2>();
			
			values.Add(src[0]);
			for(int i=1; i<src.Length; ++i)
			{
				if (!values[values.Count - 1].Equals(src[i]))
				{
					values.Add(src[i]);
					indices.Add(i);
				}
			}
			indices.Add(src.Length);
			
			_indices = indices.ToArray();
			_values = values.ToArray();
		}
		
		public Vector2 this [int index]
		{
			get
			{
				var i = System.Array.BinarySearch<int>(_indices, index);
				if (i < 0)
				{
					i = ~i;
				}
				else
				{
					++i;
				}
				return _values[i];
			}
		}
		
		public int Length
		{
			get
			{
				if (_indices == null)
				{
					return 0;
				}
				if (_indices.Length == 0)
				{
					return 0;
				}
				return _indices[_indices.Length - 1];
			}
		}
		
		public Vector2[] ToArray()
		{
			var result = new Vector2[Length];
			for(int i=0; i<Length; ++i)
			{
				result[i] = this[i];
			}
			return result;
		}
	}
	
	[System.Serializable]
	public class CompressedMeshArray
	{
		[SerializeField]
		int[] _indices;
		
		[SerializeField]
		Library_SpriteStudio.AnimationData.Fix.MeshAlies[] _values;
		
		static public CompressedMeshArray Build(Library_SpriteStudio.AnimationData.Fix.MeshAlies[] src)
		{
			if (src == null)
			{
				return null;
			}
			var result = new CompressedMeshArray(src);
			
			return result;
		}
		
		CompressedMeshArray(Library_SpriteStudio.AnimationData.Fix.MeshAlies[] src)
		{
			if (src == null)
			{
				return;
			}
			var indices = new List<int>();
			var values = new List<Library_SpriteStudio.AnimationData.Fix.MeshAlies>();
			
			values.Add(src[0]);
			for(int i=1; i<src.Length; ++i)
			{
				if (!values[values.Count - 1].Equals(src[i]))
				{
					values.Add(src[i]);
					indices.Add(i);
				}
			}
			indices.Add(src.Length);
			
			_indices = indices.ToArray();
			_values = values.ToArray();
		}
		
		public Library_SpriteStudio.AnimationData.Fix.MeshAlies this [int index]
		{
			get
			{
				var i = System.Array.BinarySearch<int>(_indices, index);
				if (i < 0)
				{
					i = ~i;
				}
				else
				{
					++i;
				}
				return _values[i];
			}
		}
		
		public int Length
		{
			get
			{
				if (_indices == null)
				{
					return 0;
				}
				if (_indices.Length == 0)
				{
					return 0;
				}
				return _indices[_indices.Length - 1];
			}
		}
		public Library_SpriteStudio.AnimationData.Fix.MeshAlies[] ToArray()
		{
			var result = new Library_SpriteStudio.AnimationData.Fix.MeshAlies[Length];
			for(int i=0; i<Length; ++i)
			{
				result[i] = this[i];
			}
			return result;
		}
		
	}
	[System.Serializable]
	public class CompressedValueColorArray
	{
		[SerializeField]
		int[] _indices;
		
		[SerializeField]
		Library_SpriteStudio.KeyFrame.ValueColor[] _values;
		
		static public CompressedValueColorArray Build(Library_SpriteStudio.KeyFrame.ValueColor[] src)
		{
			if (src == null)
			{
				return null;
			}
			var result = new CompressedValueColorArray(src);
			
			return result;
		}
		
		CompressedValueColorArray(Library_SpriteStudio.KeyFrame.ValueColor[] src)
		{
			if (src == null)
			{
				return;
			}
			var indices = new List<int>();
			var values = new List<Library_SpriteStudio.KeyFrame.ValueColor>();
			
			values.Add(src[0]);
			for(int i=1; i<src.Length; ++i)
			{
				if (!values[values.Count - 1].Equals(src[i]))
				{
					values.Add(src[i]);
					indices.Add(i);
				}
			}
			indices.Add(src.Length);
			
			_indices = indices.ToArray();
			_values = values.ToArray();
		}
		
		public Library_SpriteStudio.KeyFrame.ValueColor this [int index]
		{
			get
			{
				var i = System.Array.BinarySearch<int>(_indices, index);
				if (i < 0)
				{
					i = ~i;
				}
				else
				{
					++i;
				}
				return _values[i];
			}
		}
		
		public int Length
		{
			get
			{
				if (_indices == null)
				{
					return 0;
				}
				if (_indices.Length == 0)
				{
					return 0;
				}
				return _indices[_indices.Length - 1];
			}
		}
		public Library_SpriteStudio.KeyFrame.ValueColor[] ToArray()
		{
			var result = new Library_SpriteStudio.KeyFrame.ValueColor[Length];
			for(int i=0; i<Length; ++i)
			{
				result[i] = this[i];
			}
			return result;
		}
	}
}
