/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using System.Collections;

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

	private readonly static int[,] VertexCollrctionOrderVertex = new int[4, (int)VertexNo.TERMINATOR2]
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
					else
					{
						Script_SpriteStudio_PartsRoot ScriptRoot = InstanceGameObjectNow.GetComponent<Script_SpriteStudio_PartsRoot>();
						if((false == FlagSetInstance) && (null != ScriptRoot.PartsRootOrigin))
						{	/* "Instance"-Object */
							return;
						}
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
				public int TextureNo;
				public Rect Rectangle;
				public Vector2 Pivot;
				public Vector2 SizeOriginal;

				public Data()
				{
					TextureNo = -1;
					Rectangle.x = 0.0f;
					Rectangle.y = 0.0f;
					Rectangle.width = 0.0f;
					Rectangle.height = 0.0f;
					Pivot = Vector2.zero;
					SizeOriginal = Vector2.zero;
				}
			}

			public int FrameNoBase;
			public Data DataBody;
			
			public ValueCell()
			{
				FrameNoBase = -1;
				DataBody = null;
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

			public Data DataBody;
			
			public ValueUser()
			{
				DataBody = null;
			}
		}

		[System.Serializable]
		public class ValueBools
		{
			public enum FlagData
			{
				HIDE = 0x00000001,
				FLIPX = 0x00000010,
				FLIPY = 0x00000020,
				FLIPXTEXTURE = 0x00000100,
				FLIPYTEXTURE = 0x00000200,

				CLEAR = 0x00000000,
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
				
				public FlagData Flag;
				public int PlayCount;
				public float RateTime;
				public int OffsetStart;
				public int OffsetEnd;
				public string LabelStart;
				public string LabelEnd;

				public Data()
				{
					Flag = FlagData.CLEAR;
					PlayCount = 1;
					RateTime = 1.0f;
					OffsetStart = 0;
					OffsetEnd = 0;
					LabelStart = "";
					LabelEnd = "";
				}
			}

			public int FrameNoBase;
			public Data DataBody;
			
			public ValueInstance()
			{
				FrameNoBase = -1;
				DataBody = null;
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
			public uint Priority = 0;
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
			public uint PriorityMinimum = 0;
			public uint PriorityMaximum = 0;

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

			private ArrayList tableListMesh;
			public ArrayList TableListMesh
			{
				get
				{
					return(tableListMesh);
				}
			}

			public void BootUp()
			{
				tableListMesh = new ArrayList();
				tableListMesh.Clear();

				KindRenderQueueBase = Library_SpriteStudio.DrawManager.KindDrawQueue.SHADER_SETTING;
				OffsetDrawQueue = 0;
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
					ListMesh = tableListMesh[0] as ListMeshDraw;
					for(int i=1; i<CountList; i++)
					{
						ListMesh = tableListMesh[i] as ListMeshDraw;
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
						ListMesh = tableListMesh[ListNo] as ListMeshDraw;
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
						ListMesh = tableListMesh[ListNo] as ListMeshDraw;
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
										ListMeshDraw ListMeshNext = tableListMesh[ListNo] as ListMeshDraw;
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
						ListMesh = tableListMesh[i] as ListMeshDraw;
						
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

					int IndexMesh = 0;
					for(int i=0; i<Count; i++)
					{
						ListMesh = tableListMesh[i] as ListMeshDraw;
						IndexVertex[i] = IndexVertexNow;
						IndexTriangle[i] = IndexTriangleNow;
						DataMeshInformation = ListMesh.MeshDataTop;
						Matrix4x4 MatrixCorrect = InstanceTransform.localToWorldMatrix.inverse;
						while(null != DataMeshInformation)
						{
							CombineMesh[IndexMesh].mesh = DataMeshInformation.DataMesh;
							CombineMesh[IndexMesh].transform = MatrixCorrect * DataMeshInformation.DataTransform.localToWorldMatrix;
							IndexMesh++;

							IndexVertexNow += DataMeshInformation.DataMesh.vertexCount;
							IndexTriangleNow += DataMeshInformation.DataMesh.triangles.Length / 3;

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
		public KeyFrame.ValueInstance[] AnimationDataInstance;

		public KeyFrame.ValueCell.Data[] ArrayDataBodyCell;
		public KeyFrame.ValueUser.Data[] ArrayDataBodyUser;
		public KeyFrame.ValueInstance.Data[] ArrayDataBodyInstance;

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
			AnimationDataInstance = null;
		}

		public void UpdateUserData(int FrameNo, GameObject GameObjectNow, Script_SpriteStudio_PartsRoot ScriptRoot)
		{
			if(0 < AnimationDataUser.Length)
			{
				if(false == ScriptRoot.StatusStylePigpong)
				{	/* Play One-Way */
					if(0 != (ScriptRoot.Status & Script_SpriteStudio_PartsRoot.BitStatus.DECODE_USERDATA))
					{
						int FrameNoPrevious = (-1 == ScriptRoot.FrameNoPrevious) ? FrameNo : ScriptRoot.FrameNoPrevious;
						KeyFrame.ValueUser.Data UserData = null;

						/* Decoding Skipped Frame */
						if(0 == (ScriptRoot.Status & Script_SpriteStudio_PartsRoot.BitStatus.PLAYING_REVERSE))
						{	/* backwards */
							if((FrameNo > FrameNoPrevious) || (0 < ScriptRoot.CountLoopThisTime))
							{	/* Wrap-Around */
								/* Part-Head */
								for(int i=(FrameNoPrevious - 1); i>=ScriptRoot.FrameNoStart; i--)
								{
									UserData = AnimationDataUser[i].DataBody;
									if(Library_SpriteStudio.KeyFrame.ValueUser.Data.FlagData.CLEAR != UserData.Flag)
									{
										ScriptRoot.CallBackExecUserData(GameObjectNow.name, this, i, UserData, false);
									}
								}
							
								/* Part-Loop */
								for(int j=0; j<(ScriptRoot.CountLoopThisTime - 1); j++)
								{
									for(int i=ScriptRoot.FrameNoEnd; i>=ScriptRoot.FrameNoStart; i--)
									{
										UserData = AnimationDataUser[i].DataBody;
										if(Library_SpriteStudio.KeyFrame.ValueUser.Data.FlagData.CLEAR != UserData.Flag)
										{
											ScriptRoot.CallBackExecUserData(GameObjectNow.name, this, i, UserData, false);
										}
									}
								}
							
								/* Part-Tail */
								for(int i=ScriptRoot.FrameNoEnd; i>FrameNo; i--)
								{
									UserData = AnimationDataUser[i].DataBody;
									if(Library_SpriteStudio.KeyFrame.ValueUser.Data.FlagData.CLEAR != UserData.Flag)
									{
										ScriptRoot.CallBackExecUserData(GameObjectNow.name, this, i, UserData, false);
									}
								}
							}
							else
							{	/* Normal */
								for(int i=(FrameNoPrevious - 1); i>FrameNo; i--)
								{
									UserData = AnimationDataUser[i].DataBody;
									if(Library_SpriteStudio.KeyFrame.ValueUser.Data.FlagData.CLEAR != UserData.Flag)
									{
										ScriptRoot.CallBackExecUserData(GameObjectNow.name, this, i, UserData, false);
									}
								}
							}
						}
						else
						{	/* foward */
							if((FrameNo < FrameNoPrevious) || (0 < ScriptRoot.CountLoopThisTime))
							{	/* Wrap-Around */
								/* Part-Head */
								for(int i=(FrameNoPrevious + 1); i<=ScriptRoot.FrameNoEnd; i++)
								{
									UserData = AnimationDataUser[i].DataBody;
									if(Library_SpriteStudio.KeyFrame.ValueUser.Data.FlagData.CLEAR != UserData.Flag)
									{
										ScriptRoot.CallBackExecUserData(GameObjectNow.name, this, i, UserData, false);
									}
								}

								/* Part-Loop */
								for(int j=0; j<(ScriptRoot.CountLoopThisTime - 1); j++)
								{
									for(int i=ScriptRoot.FrameNoStart; i<=ScriptRoot.FrameNoEnd; i++)
									{
										UserData = AnimationDataUser[i].DataBody;
										if(Library_SpriteStudio.KeyFrame.ValueUser.Data.FlagData.CLEAR != UserData.Flag)
										{
											ScriptRoot.CallBackExecUserData(GameObjectNow.name, this, i, UserData, false);
										}
									}
								}

								/* Part-Tail */
								for(int i=ScriptRoot.FrameNoStart; i<FrameNo; i++)
								{
									UserData = AnimationDataUser[i].DataBody;
									if(Library_SpriteStudio.KeyFrame.ValueUser.Data.FlagData.CLEAR != UserData.Flag)
									{
										ScriptRoot.CallBackExecUserData(GameObjectNow.name, this, i, UserData, false);
									}
								}
							}
							else
							{	/* Normal */
								for(int i=(FrameNoPrevious + 1); i<FrameNo; i++)
								{
									UserData = AnimationDataUser[i].DataBody;
									if(Library_SpriteStudio.KeyFrame.ValueUser.Data.FlagData.CLEAR != UserData.Flag)
									{
										ScriptRoot.CallBackExecUserData(GameObjectNow.name, this, i, UserData, false);
									}
								}
							}
						}

						/* Decoding Just-Now Frame */
						UserData = AnimationDataUser[FrameNo].DataBody;
						if(Library_SpriteStudio.KeyFrame.ValueUser.Data.FlagData.CLEAR != UserData.Flag)
						{
							ScriptRoot.CallBackExecUserData(GameObjectNow.name, this, FrameNo, UserData, false);
						}
					}
				}
				else
				{	/* Play PingPong */
				}
			}
		}

		public bool UpdateInstanceData(int FrameNo, GameObject GameObjectNow, Script_SpriteStudio_PartsRoot ScriptRoot, Script_SpriteStudio_PartsInstance PartsInstance)
		{
			Script_SpriteStudio_PartsRoot ScriptPartsRootSub = PartsInstance.ScriptPartsRootSub;

			if(0 >= AnimationDataInstance.Length)
			{	/* Error */
				return(false);
			}
			
			int FrameNoInstanceBase = AnimationDataInstance[FrameNo].FrameNoBase;
			KeyFrame.ValueInstance DataBody = AnimationDataInstance[FrameNoInstanceBase];
//			int FramePreviousRoot = ScriptRoot.FrameNoPrevious;
			int FramePreviousUpdateInstance = PartsInstance.FrameNoPreviousUpdate;
			if(-1 == FramePreviousUpdateInstance)
			{
				goto UpdateInstanceData_PlayCommand_Initial;
			}
			if(FrameNoInstanceBase != FramePreviousUpdateInstance)
			{
				goto UpdateInstanceData_PlayCommand_Initial;
			}

			if(FrameNo >= FrameNoInstanceBase)
			{
				ScriptPartsRootSub.AnimationPause(false);
			}

			return(true);
			
		UpdateInstanceData_PlayCommand_Initial:;
			{
				float RateTime = DataBody.DataBody.RateTime;
				RateTime *= (0 != (ScriptRoot.Status & Script_SpriteStudio_PartsRoot.BitStatus.PLAYING_REVERSE)) ? -1.0f : 1.0f;
				ScriptPartsRootSub.AnimationPlay(	-1,
													DataBody.DataBody.PlayCount,
													0,
													RateTime,
													((0 != (DataBody.DataBody.Flag & KeyFrame.ValueInstance.Data.FlagData.PINGPONG)) ? true : false),
													DataBody.DataBody.LabelStart,
													DataBody.DataBody.OffsetStart,
													DataBody.DataBody.LabelEnd,
													DataBody.DataBody.OffsetEnd
												);
				
				int FrameCount = FrameNo - FrameNoInstanceBase;
				FrameCount = (0 > FrameCount) ? 0 : FrameCount;
				ScriptPartsRootSub.TimeElapsedSetForce(FrameCount * ScriptRoot.TimeFramePerSecond);
				if(FrameNoInstanceBase > FrameNo)
				{
					ScriptPartsRootSub.AnimationPause(true);
				}
				PartsInstance.FrameNoPreviousUpdate = FrameNoInstanceBase;
			}

			return(true);
		}

		public bool UpdateGameObject(GameObject GameObjectNow, int FrameNo, bool FlagSprite)
		{
			bool FlagUpdateTransform = ((0 >= AnimationDataPosition.Length) && (0 >= AnimationDataRotation.Length) && (0 >= AnimationDataScaling.Length)) ? false : true;
			/* MEMO: No Transform-Datas, Not Changing "GameObject" */
			if(false == FlagUpdateTransform)
			{	/* No-Update Transform */
				if(false == FlagSprite)
				{
					return(false);	/* Hide */
				}
			}
			else
			{	/* Update Transform */
				GameObjectNow.transform.localPosition = (0 < AnimationDataPosition.Length) ? AnimationDataPosition[FrameNo] : Vector3.zero;
				GameObjectNow.transform.localEulerAngles = (0 < AnimationDataRotation.Length) ? AnimationDataRotation[FrameNo] : Vector3.zero;
				Vector3 Scale = Vector3.one;
				if(0 < AnimationDataScaling.Length)
				{
					Scale.x = AnimationDataScaling[FrameNo].x;
					Scale.y = AnimationDataScaling[FrameNo].y;
				}
				GameObjectNow.transform.localScale = Scale;
			}

			if(0 >= AnimationDataFlags.Length)
			{	/* No-Flags */
				return(false);	/* Hide */
			}

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
									RectCell = AnimationDataCell[FrameNo].DataBody.Rectangle;
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

		internal static uint PriorityGet(float Priority, int ID)
		{
			return(((((uint)Priority + 0x4000) << 17) & 0xfffe0000) | (((uint)ID << 7) & 0x0001ff80));
		}

		public void DrawEntryInstance(Library_SpriteStudio.DrawManager.InformationMeshData MeshDataInformation, int FrameNo, Script_SpriteStudio_PartsRoot ScriptRoot)
		{
			float Priority = (0 < AnimationDataPriority.Length) ? AnimationDataPriority[FrameNo] : 0.0f;

			MeshDataInformation.Priority = PriorityGet(Priority, ID);
			Library_SpriteStudio.DrawManager.ArrayListMeshDraw ArrayListMeshDraw = ScriptRoot.ArrayListMeshDraw;
			if(null != ArrayListMeshDraw)
			{
				ArrayListMeshDraw.MeshAdd(null, MeshDataInformation);
			}
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
			int	VertexCollectionIndexTableNo = 0;

#if false
			/* Error-Check */
			if((AnimationDataCell.Length <= FrameNo) || (0 > FrameNo))
			{
				FrameNo = FrameNo;
			}
#endif
			/* Main-Texture Data Get */
			Material MaterialNow = ScriptRoot.MaterialGet(AnimationDataCell[FrameNo].DataBody.TextureNo, KindBlendTarget);
			if(null != MaterialNow)
			{
#if false
				Texture InstanceTexture = MaterialNow.mainTexture;
				SizeTexture.x = (float)InstanceTexture.width;
				SizeTexture.y = (float)InstanceTexture.height;
#else
				SizeTexture.x = AnimationDataCell[FrameNo].DataBody.SizeOriginal.x;
				SizeTexture.y = AnimationDataCell[FrameNo].DataBody.SizeOriginal.y;
#endif
			}

			/* Cell-Data Get */
			if(0 < AnimationDataCell.Length)
			{
				RectCell = AnimationDataCell[FrameNo].DataBody.Rectangle;
				PivotTexture = new Vector2(RectCell.width * 0.5f, RectCell.height * 0.5f);

				PivotMesh = AnimationDataCell[FrameNo].DataBody.Pivot;
			}

			/* Disolve Flipping & Texture-Scaling */
			if(0 < AnimationDataFlags.Length)
			{
				RateScaleTexture.x = (true == AnimationDataFlags[FrameNo].IsTextureFlipX) ? -1.0f : 1.0f;
				RateScaleTexture.y = (true == AnimationDataFlags[FrameNo].IsTextureFlipY) ? -1.0f : 1.0f;
				if(true == AnimationDataFlags[FrameNo].IsFlipX)
				{
					RateScaleMesh.x = -1.0f;
					VertexCollectionIndexTableNo += 1;
				}
				else
				{
					RateScaleMesh.x = 1.0f;
				}
				if(true == AnimationDataFlags[FrameNo].IsFlipY)
				{
					RateScaleMesh.y = -1.0f;
					VertexCollectionIndexTableNo += 2;
				}
				else
				{
					RateScaleMesh.y = 1.0f;
				}
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
#if false
				for(int i=0; i<CountVertexData; i++)
				{
					DataUV2[i] = new Vector2(	AnimationDataColorBlend[FrameNo].RatePixelAlpha[i] * RateOpacity,
												(float)AnimationDataColorBlend[FrameNo].Operation + 0.01f	/* "+0.01f" for Rounding-off-Error */
											);
					DataColor32[i] = AnimationDataColorBlend[FrameNo].VertexColor[i];
				}
#else
				if(Library_SpriteStudio.KindColorBound.NON != AnimationDataColorBlend[FrameNo].Bound)
				{
					for(int i=0; i<CountVertexData; i++)
					{
						DataUV2[i] = new Vector2(	AnimationDataColorBlend[FrameNo].RatePixelAlpha[i] * RateOpacity,
													(float)AnimationDataColorBlend[FrameNo].Operation + 0.01f	/* "+0.01f" for Rounding-off-Error */
												);
						DataColor32[i] = AnimationDataColorBlend[FrameNo].VertexColor[i];
					}
				}
				else
				{	/* Default (Same as "No Datas" ) */
					Color32 ColorDefault = Color.white;
					float OperationDefault = (float)KindColorOperation.NON + 0.01f;	/* "+0.01f" for Rounding-off-Error */
					for(int i=0; i<CountVertexData; i++)
					{
						DataUV2[i] = new Vector2(RateOpacity, OperationDefault);
						DataColor32[i] = ColorDefault;
					}
				}
#endif
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
				/* Get SpriteSize & Pivot */
				SpriteRecalcSizeAndPivot(ref PivotMesh, ref RectCell, ref RateScaleMesh, FrameNo);

				/* Get Coordinates */
				/* MEMO: No Check "AnimationDataVertexCorrection.Length", 'cause 4-Triangles-Mesh necessarily has "AnimationDataVertexCorrection" */
				float Left = (-PivotMesh.x) * RateScaleMesh.x;
				float Right = (RectCell.width - PivotMesh.x) * RateScaleMesh.x;
				float Top = (-PivotMesh.y) * RateScaleMesh.y;
				float Bottom = (RectCell.height - PivotMesh.y) * RateScaleMesh.y;

				DataCoordinate[(int)VertexNo.LU] = new Vector3(	Left + AnimationDataVertexCorrection[FrameNo].Coordinate[VertexCollrctionOrderVertex[VertexCollectionIndexTableNo, (int)VertexNo.LU]].x,
																-Top + AnimationDataVertexCorrection[FrameNo].Coordinate[VertexCollrctionOrderVertex[VertexCollectionIndexTableNo, (int)VertexNo.LU]].y,
																0.0f
															);
				DataCoordinate[(int)VertexNo.RU] = new Vector3(	Right + AnimationDataVertexCorrection[FrameNo].Coordinate[VertexCollrctionOrderVertex[VertexCollectionIndexTableNo, (int)VertexNo.RU]].x,
																-Top + AnimationDataVertexCorrection[FrameNo].Coordinate[VertexCollrctionOrderVertex[VertexCollectionIndexTableNo, (int)VertexNo.RU]].y,
																0.0f
															);
				DataCoordinate[(int)VertexNo.RD] = new Vector3(	Right + AnimationDataVertexCorrection[FrameNo].Coordinate[VertexCollrctionOrderVertex[VertexCollectionIndexTableNo, (int)VertexNo.RD]].x,
																-Bottom + AnimationDataVertexCorrection[FrameNo].Coordinate[VertexCollrctionOrderVertex[VertexCollectionIndexTableNo, (int)VertexNo.RD]].y,
																0.0f
															);
				DataCoordinate[(int)VertexNo.LD] = new Vector3(	Left + AnimationDataVertexCorrection[FrameNo].Coordinate[VertexCollrctionOrderVertex[VertexCollectionIndexTableNo, (int)VertexNo.LD]].x,
																-Bottom + AnimationDataVertexCorrection[FrameNo].Coordinate[VertexCollrctionOrderVertex[VertexCollectionIndexTableNo, (int)VertexNo.LD]].y,
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
		private void SpriteRecalcSizeAndPivot(ref Vector2 PivotMesh, ref Rect RectCell, ref Vector2 RateScaleMesh, int FrameNo)
		{
			Vector2 PivotOffset = (0 < AnimationDataOriginOffset.Length) ? AnimationDataOriginOffset[FrameNo] : Vector2.zero;
			PivotMesh.x += (RectCell.width * PivotOffset.x) * RateScaleMesh.x;
			PivotMesh.y -= (RectCell.height * PivotOffset.y) * RateScaleMesh.y;

			/* Arbitrate Anchor-Size */
			if(0 < AnimationDataAnchorSize.Length)
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

		public void DrawEntry(Library_SpriteStudio.DrawManager.InformationMeshData MeshDataInformation, int FrameNo, Script_SpriteStudio_PartsRoot ScriptRoot)
		{
			float Priority = (0 < AnimationDataPriority.Length) ? AnimationDataPriority[FrameNo] : 0.0f;
			int TextureNo = (0 < AnimationDataCell.Length) ? AnimationDataCell[FrameNo].DataBody.TextureNo : -1;

			MeshDataInformation.Priority = PriorityGet(Priority, ID);
			Library_SpriteStudio.DrawManager.ArrayListMeshDraw ArrayListMeshDraw = ScriptRoot.ArrayListMeshDraw;
			if(null != ArrayListMeshDraw)
			{
#if false
				ArrayListMeshDraw.MeshAdd(ScriptRoot.MaterialGet(TextureNo, KindBlendTarget), MeshDataInformation);
#else
				Material MaterialNow = ScriptRoot.MaterialGet(TextureNo, KindBlendTarget);
				if(null == MaterialNow)
				{	/* has Illegal-Material */
					return;
				}
				ArrayListMeshDraw.MeshAdd(MaterialNow, MeshDataInformation);
#endif
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
