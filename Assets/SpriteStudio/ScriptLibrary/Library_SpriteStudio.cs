/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Library_SpriteStudio
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
		BOUND,
		SOUND,
		INSTANCE,

		TERMINATOR
	};

	public enum KindSprite
	{
		NON = -1,				/* Not Sprite-Parts: ROOT/NULL/INSTANCE */
		TRIANGLE2 = 0,			/* No use Vertex-Collection Sprite-Parts */
		TRIANGLE4,				/* Use Vertex-Collection Sprite-Parts */
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
				FLIPYTEXTURE = 0x00200000,	/* Caution: Does not affect the display (State in "ssae" / Baked to Datas) */
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
		};

		public int ID;
		public KindSprite SpriteKind;
		public KindCollision CollisionKind;
		public KindColorOperation KindBlendTarget;

		public KeyFrame.ValueBools[] AnimationDataFlags;
		public Vector3[] AnimationDataPosition;
		public Vector3[] AnimationDataRotation;
		public Vector2[] AnimationDataScaling;

		public float[] AnimationDataOpacityRate;
		public float[] AnimationDataPriority;

		public MeshAlies[] AnimationDataMesh;

		public Vector3[] AnimationDataCollisionSize;	/* for Box-Collider */
		public Vector3[] AnimationDataCollisionPivot;	/* for Box-Collider */
		public float[] AnimationDataCollisionRadius;	/* for Sphere-Collider */

		public int[] AnimationDataUser;
		public int[] AnimationDataInstance;

		public KeyFrame.ValueUser.Data[] ArrayDataBodyUser;
		public KeyFrame.ValueInstance.Data[] ArrayDataBodyInstance;

		/* Buffer for Runtime-Speed-Optimize */
		private float ColliderRadiusPrevious = -1.0f;	/* for Radius-Collision */
		private Vector3 ColliderRectSizePrevious = Vector3.zero;	/* for Rectangle-Collision */
		private Vector3 ColliderRectPivotPrevious = Vector3.zero;	/* for Rectangle-Collision */

		public AnimationData()
		{
			ID = -1;
			CollisionKind = Library_SpriteStudio.KindCollision.NON;

			KindBlendTarget = Library_SpriteStudio.KindColorOperation.NON;

			AnimationDataFlags = null;
			AnimationDataPosition = null;
			AnimationDataRotation = null;
			AnimationDataScaling = null;

			AnimationDataOpacityRate = null;
			AnimationDataPriority = null;

			AnimationDataMesh = null;

			AnimationDataCollisionSize = null;
			AnimationDataCollisionPivot = null;
			AnimationDataCollisionRadius = null;

			AnimationDataUser = null;
			AnimationDataInstance = null;

			ArrayDataBodyUser = null;
			ArrayDataBodyInstance = null;
		}

		public void UpdateUserData(int FrameNo, GameObject GameObjectNow, Script_SpriteStudio_PartsRoot ScriptRoot)
		{
			if(null == AnimationDataUser)	return;
			if(0 < AnimationDataUser.Length)
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
								UpdateUserDataReverse(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNoStart, false);

								/* Part-Loop */
								for(int j=1; j<LoopCount ; j++)
								{
									UpdateUserDataReverse(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoEnd, FrameNoStart, false);
								}
							
								/* Part-Tail & Just-Now */
								UpdateUserDataReverse(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoEnd, FrameNo, false);
							}
							else
							{	/* Normal */
								UpdateUserDataReverse(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNo, false);
							}
						}
						else
						{	/* foward */
							if((FrameNo < FrameNoPrevious) || (0 < LoopCount))
							{	/* Wrap-Around */
								/* Part-Head */
								UpdateUserDataFoward(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNoEnd, false);

								/* Part-Loop */
								for(int j=1; j<LoopCount; j++)
								{
									UpdateUserDataFoward(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoStart, FrameNoEnd, false);
								}

								/* Part-Tail & Just-Now */
								UpdateUserDataFoward(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoStart, FrameNo, false);
							}
							else
							{	/* Normal */
								UpdateUserDataFoward(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNo, false);
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
									UpdateUserDataReverse(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNoStart, false);
									UpdateUserDataFoward(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoStart, FrameNoEnd, true);
								}
								else
								{
									FrameNoPrevious = ScriptRoot.FrameNoPrevious + 1;	/* Force */
									UpdateUserDataFoward(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNoEnd, true);
								}

								/* Part-Loop */
								for(int i=1; i<LoopCount; i++)
								{
									UpdateUserDataReverse(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoEnd, FrameNoStart, false);
									UpdateUserDataFoward(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoStart, FrameNoEnd, true);
								}

								/* Part-Tail & Just-Now */
								if(true == FlagReverse)
								{	/* Now-Reverse */
									UpdateUserDataReverse(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoEnd, FrameNo, false);
								}
								else
								{	/* Now-Foward */
									UpdateUserDataReverse(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoEnd, FrameNoStart, false);
									UpdateUserDataFoward(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoStart, FrameNo, true);
								}
							}
							else
							{	/* Normal */
								if(true == FlagTurnBackPingPong)
								{	/* Turn-Back */
									/* MEMO: No-Loop & Turn-Back ... Always "Reverse to Foward" */
									FrameNoPrevious = ScriptRoot.FrameNoPrevious - 1;	/* Force */
									UpdateUserDataReverse(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNoStart, false);
									UpdateUserDataFoward(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoStart, FrameNo, true);
								}
								else
								{	/* Normal */
									if(true == FlagReverse)
									{	/* Reverse */
										UpdateUserDataReverse(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNo, false);
									}
									else
									{	/* Foward */
										UpdateUserDataFoward(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNo, true);
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
									UpdateUserDataReverse(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNoStart, true);
								}
								else
								{
									FrameNoPrevious = ScriptRoot.FrameNoPrevious + 1;	/* Force */
									UpdateUserDataFoward(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNoEnd, false);
									UpdateUserDataReverse(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoEnd, FrameNoStart, true);
								}

								/* Part-Loop */
								for(int i=1; i<LoopCount; i++)
								{
									UpdateUserDataFoward(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoStart, FrameNoEnd, false);
									UpdateUserDataReverse(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoEnd, FrameNoStart, true);
								}
									
								/* Part-Tail & Just-Now */
								if(true == FlagReverse)
								{	/* Now-Reverse */
									UpdateUserDataFoward(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoStart, FrameNoEnd, false);
									UpdateUserDataReverse(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoEnd, FrameNo, true);
								}
								else
								{	/* Now-Foward */
									UpdateUserDataFoward(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoStart, FrameNo, false);
								}
							}
							else
							{	/* Normal */
								if(true == FlagTurnBackPingPong)
								{	/* Turn-Back */
									/* MEMO: No-Loop & Turn-Back ... Always "Foward to Revese" */
									FrameNoPrevious = ScriptRoot.FrameNoPrevious + 1;	/* Force */
									UpdateUserDataFoward(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNoEnd, false);
									UpdateUserDataReverse(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoEnd, FrameNo, true);
								}
								else
								{	/* Normal */
									if(true == FlagReverse)
									{	/* Reverse */
										UpdateUserDataReverse(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNo, true);
									}
									else
									{	/* Foward */
										UpdateUserDataFoward(ref AnimationDataUser, ScriptRoot, GameObjectNow, FrameNoPrevious, FrameNo, false);
									}
								}
							}
						}
					}
				}
			}
		}
		private void UpdateUserDataFoward(ref int[] ArrayData, Script_SpriteStudio_PartsRoot ScriptRoot, GameObject InstanceGameObject, int FrameNoStart, int FrameNoEnd, bool FlagTurnBackPingPong)
		{
			int Index = -1;
			KeyFrame.ValueUser.Data UserData = null;
			for(int i=FrameNoStart; i<=FrameNoEnd; i++)
			{
				Index = ArrayData[i];
				UserData = ((0 <= Index) && (0 < ArrayDataBodyUser.Length)) ? ArrayDataBodyUser[Index] : KeyFrame.DummyDataUser;
				if(Library_SpriteStudio.KeyFrame.ValueUser.Data.FlagData.CLEAR != UserData.Flag)
				{
					ScriptRoot.CallBackExecUserData(InstanceGameObject.name, this, i, UserData, FlagTurnBackPingPong);
				}
			}
		}
		private void UpdateUserDataReverse(ref int[] ArrayData, Script_SpriteStudio_PartsRoot ScriptRoot, GameObject InstanceGameObject, int FrameNoStart, int FrameNoEnd, bool FlagTurnBackPingPong)
		{
			int Index = -1;
			KeyFrame.ValueUser.Data UserData = null;
			for(int i=FrameNoStart; i>=FrameNoEnd; i--)
			{
				Index = ArrayData[i];
				UserData = ((0 <= Index) && (0 < ArrayDataBodyUser.Length)) ? ArrayDataBodyUser[Index] : KeyFrame.DummyDataUser;
				if(Library_SpriteStudio.KeyFrame.ValueUser.Data.FlagData.CLEAR != UserData.Flag)
				{
					ScriptRoot.CallBackExecUserData(InstanceGameObject.name, this, i, UserData, FlagTurnBackPingPong);
				}
			}
		}
		private void UpdateUserDataJustNow(ref int[] ArrayData, Script_SpriteStudio_PartsRoot ScriptRoot, GameObject InstanceGameObject, int FrameNo, bool FlagTurnBackPingPong)
		{
			int Index = ArrayData[FrameNo];
			KeyFrame.ValueUser.Data UserData = ((0 <= Index) && (0 < ArrayDataBodyUser.Length)) ? ArrayDataBodyUser[Index] : KeyFrame.DummyDataUser;
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

//			ScriptPartsRootSub.RateOpacity = (0 < AnimationDataOpacityRate.Length) ? AnimationDataOpacityRate[FrameNo] : 1.0f;
			ScriptPartsRootSub.RateOpacity = 1.0f;
			if(null != AnimationDataOpacityRate)
			{
				ScriptPartsRootSub.RateOpacity = AnimationDataOpacityRate[FrameNo];
			}

//			if(0 >= AnimationDataInstance.Length)
			if(null == AnimationDataInstance)
			{	/* Error ... Force Play */
				goto UpdateInstanceData_PlayCommand_Force;
			}
			if(0 >= AnimationDataInstance.Length)
			{	/* Error ... Force Play */
				goto UpdateInstanceData_PlayCommand_Force;
			}

			int IndexInstanceBody = AnimationDataInstance[FrameNo];
//			DataBody = ((0 <= IndexInstanceBody) && (0 < ArrayDataBodyInstance.Length)) ? ArrayDataBodyInstance[IndexInstanceBody] : KeyFrame.DummyDataInstance;
			DataBody = KeyFrame.DummyDataInstance;
			if(null == ArrayDataBodyInstance)
			{
				if((0 <= IndexInstanceBody) && (0 < ArrayDataBodyInstance.Length))
				{
					DataBody =  ArrayDataBodyInstance[IndexInstanceBody];
				}
			}
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
					if(true == UpdateInstanceCheckRange(ScriptPartsRootSub, FrameNo - FrameNoInstanceBase, PartsInstance.AnimationNo, DataBody))
					{
						goto UpdateInstanceData_PlayCommand_Update;
					}
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
		public bool UpdateGameObject(GameObject GameObjectNow, int FrameNo, bool FlagSprite)
		{
//			bool FlagUpdateTransform = ((0 >= AnimationDataPosition.Length) && (0 >= AnimationDataRotation.Length) && (0 >= AnimationDataScaling.Length)) ? false : true;
			bool FlagUpdateTransform = ((null == AnimationDataPosition) && (null == AnimationDataRotation) && (null == AnimationDataScaling)) ? false : true;
			if(false == FlagUpdateTransform)
			{/* No-Update Transform */
				return(false);	/* Hide */
			}
			/* MEMO: No Transform-Datas, Not Changing "GameObject" */
			FlagUpdateTransform = ((0 >= AnimationDataPosition.Length) && (0 >= AnimationDataRotation.Length) && (0 >= AnimationDataScaling.Length)) ? false : true;
			if(false == FlagUpdateTransform)
			{	/* No-Update Transform */
				if(false == FlagSprite)
				{
					return(false);	/* Hide */
				}
			}
			else
			{	/* Update Transform */
//				GameObjectNow.transform.localPosition = (0 < AnimationDataPosition.Length) ? AnimationDataPosition[FrameNo] : Vector3.zero;
				Vector3 TransformTemp = Vector3.zero;
				if(null != AnimationDataPosition)
				{
					if(0 < AnimationDataPosition.Length)
					{
						TransformTemp = AnimationDataPosition[FrameNo];
						GameObjectNow.transform.localPosition = TransformTemp;
					}
				}

//				GameObjectNow.transform.localEulerAngles = (0 < AnimationDataRotation.Length) ? AnimationDataRotation[FrameNo] : Vector3.zero;
				TransformTemp = Vector3.zero;
				if(null != AnimationDataRotation)
				{
					if(0 < AnimationDataRotation.Length)
					{
						TransformTemp = AnimationDataRotation[FrameNo];
						GameObjectNow.transform.localEulerAngles = TransformTemp;
					}
				}

				TransformTemp = Vector3.one;
				if(null != AnimationDataScaling)
				{
					if(0 < AnimationDataScaling.Length)
					{
						TransformTemp.x = AnimationDataScaling[FrameNo].x;
						TransformTemp.y = AnimationDataScaling[FrameNo].y;
						GameObjectNow.transform.localScale = TransformTemp;
					}
				}
			}

//			if(0 >= AnimationDataFlags.Length)
			if(null == AnimationDataFlags)
			{	/* No-Flags */
				if(0 >= AnimationDataFlags.Length)
				{
					return(false);	/* Hide */
				}
			}

			/* Collider-Setting */
			switch(CollisionKind)
			{
				case KindCollision.SQUARE:
					{
						BoxCollider InstanceCollider = GameObjectNow.GetComponent<BoxCollider>();
						if(null != InstanceCollider)
						{
							Vector3 SizeNew = Vector3.one;
							Vector3 PivotNew = Vector3.zero;
							if(null != AnimationDataCollisionSize)
							{
								if(0 < AnimationDataCollisionSize.Length)
								{
									SizeNew = AnimationDataCollisionSize[FrameNo];
								}
							}
							if(null != AnimationDataCollisionPivot)
							{
								if(0 < AnimationDataCollisionPivot.Length)
								{
									PivotNew = AnimationDataCollisionPivot[FrameNo];
								}
							}

							if((PivotNew != ColliderRectPivotPrevious) || (SizeNew != ColliderRectSizePrevious))
							{	/* Update */
								/* Update Previous Buffer */
								ColliderRectPivotPrevious = PivotNew;
								ColliderRectSizePrevious = SizeNew;

								/* Update Collider */
								InstanceCollider.enabled = true;
								InstanceCollider.size = ColliderRectSizePrevious;
								InstanceCollider.center = ColliderRectPivotPrevious;
							}
						}
					}
					break;

				case KindCollision.CIRCLE:
					{
						CapsuleCollider InstanceCollider = GameObjectNow.GetComponent<CapsuleCollider>();
						if(null != InstanceCollider)
						{
							float RadiusNew = 1.0f;
							if(null != AnimationDataCollisionRadius)
							{
								if(0 < AnimationDataCollisionRadius.Length)
								{
									RadiusNew = AnimationDataCollisionRadius[FrameNo];
								}
							}
							if(RadiusNew != ColliderRadiusPrevious)
							{	/* Update */
								/* Update Previous Buffer */
								ColliderRadiusPrevious = RadiusNew;

								/* Update Collider */
								InstanceCollider.enabled = true;
								InstanceCollider.radius = ColliderRadiusPrevious;
								InstanceCollider.center = Vector3.zero;
							}
						}
					}
					break;

				default:
					break;
			}

			/* Return-Value is "Inversed Hide-Flag"  */
			return(!AnimationDataFlags[FrameNo].IsHide);
		}

		internal static float PriorityGet(float Priority, int ID)
		{
			return(Mathf.Floor(Priority) + ((float)ID * 0.001f));
		}

		public void DrawEntryInstance(Library_SpriteStudio.DrawManager.InformationMeshData MeshDataInformation, int FrameNo, Script_SpriteStudio_PartsRoot ScriptRoot)
		{
//			float Priority = (0 < AnimationDataPriority.Length) ? AnimationDataPriority[FrameNo] : 0.0f;
			float Priority = 0.0f;
			if(null != AnimationDataPriority)
			{
				if(0 < AnimationDataPriority.Length)
				{
					Priority = AnimationDataPriority[FrameNo];
				}
			}

			MeshDataInformation.Priority = PriorityGet(Priority, ID);
			Library_SpriteStudio.DrawManager.ArrayListMeshDraw ArrayListMeshDraw = ScriptRoot.ArrayListMeshDraw;
			if(null != ArrayListMeshDraw)
			{
				ArrayListMeshDraw.MeshAdd(null, MeshDataInformation);
			}
		}

		public void UpdateMesh(Mesh MeshNow, int FrameNo, Script_SpriteStudio_PartsRoot ScriptRoot)
		{
			float RateOpacity = (null != ScriptRoot) ? ScriptRoot.RateOpacity : 1.0f;

			MeshAlies DataMeshAlies = AnimationDataMesh[FrameNo];
			Vector2[] DataUV2 = new Vector2[DataMeshAlies.UV2.Length];
			for(int i=0; i<DataUV2.Length; i++)
			{
				DataUV2[i] = DataMeshAlies.UV2[i];
				DataUV2[i].x *= RateOpacity;
			}
			MeshNow.vertices = DataMeshAlies.Coordinate;
			MeshNow.colors32 = DataMeshAlies.ColorOverlay;
			MeshNow.uv = DataMeshAlies.UV;
			MeshNow.uv2 = DataUV2;
		}

		public void DrawEntry(Library_SpriteStudio.DrawManager.InformationMeshData MeshDataInformation, int FrameNo, Script_SpriteStudio_PartsRoot ScriptRoot)
		{
			if(false == ScriptRoot.FlagHideForce)
			{
//				float Priority = (0 < AnimationDataPriority.Length) ? AnimationDataPriority[FrameNo] : 0.0f;
				float Priority = 0.0f;
				if(null != AnimationDataPriority)
				{
					if(0 < AnimationDataPriority.Length)
					{
						Priority = AnimationDataPriority[FrameNo];
					}
				}

				int TextureNo = AnimationDataFlags[FrameNo].TextureNo;
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

		protected Library_SpriteStudio.DrawManager.InformationMeshData DataMeshInformation = null;
	}
}
