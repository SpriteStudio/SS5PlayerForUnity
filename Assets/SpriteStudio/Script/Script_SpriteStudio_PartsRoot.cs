/**
	SpriteStudio5 Player for Unity

	Copyright(C) 2003-2013 Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using System.Collections;

using System.Text;
using System.IO;

[System.Serializable]
[ExecuteInEditMode]
public class Script_SpriteStudio_PartsRoot : Library_SpriteStudio.PartsBase
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

	public class InformationMeshData
	{
		public InformationMeshData ChainNext = null;
		public float Priority = 0.0f;
		public Mesh DataMesh = null;
		public Transform DataTransform = null;
	}
	private class ListMeshDraw
	{
		public int MaterialNo = -1;
		public InformationMeshData MeshDataTop;

		public int Count = 0;
		public float PriorityMinimum = -10000.0f;
		public float PriorityMaximum = -10000.0f;

		public void MeshAdd(ref InformationMeshData DataNew)
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
					ListNew.MaterialNo = MaterialNo;
					ListNew.Count = Count - CountNow;
					ListNew.MeshDataTop = DataNext;
					ListNew.PriorityMinimum = DataNext.Priority;

					Count -= ListNew.Count;
					if(null == DataPrevious)
					{
						PriorityMinimum = -10000.0f;
						PriorityMaximum = -10000.0f;
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
	private class ParameterCallBackUserData
	{
		public string PartsName = null;
		public Library_SpriteStudio.SpriteData AnimationDataParts = null;
		public int FrameNo = -1;
		public Library_SpriteStudio.KeyFrame.ValueUser Data = null;
	}

	public Library_SpriteStudio.SpriteData SpriteStudioData;
	public Library_SpriteStudio.AnimationInformationPlay[] ListInformationPlay;
	public float RateTimeAnimation;

	/* CAUTION!: Don't set values from Code(Read-Only in principle). Use Function"AnimationPlay". */
	/*           "AnimationNo","CountLoopRemain" and "FrameNoStartOffset" are defined public for Setting on Inspector. */
	public int CountLoopRemain;

	[SerializeField]
	protected int animationNo;
	public int AnimationNo
	{
		set
		{
			if((value != animationNo) && (1 < ListInformationPlay.Length))
			{
				SpriteStudioData.Status |= Library_SpriteStudio.AnimationDataRuntime.BitStatus.REFRESH_PLAYRANGENO;
				animationNo = value;
			}
		}
		get
		{
			return(animationNo);
		}
	}

	[SerializeField]
	protected int frameNoInitial;	/* Influences only at first */
	public int FrameNoInitial
	{
		set
		{
			if(value != frameNoInitial)
			{
				SpriteStudioData.Status |= Library_SpriteStudio.AnimationDataRuntime.BitStatus.REFRESH_PLAYFRAMENO;
			}
			frameNoInitial = value;
			TimeAnimation = (ListInformationPlay[animationNo].FrameStart + frameNoInitial) * TimeFramePerSecond;
		}
		get
		{
			return(frameNoInitial);
		}
	}

	protected int frameNoStart;
	public int FrameNoStart
	{
		get
		{
			return(frameNoEnd);
		}
	}
	protected int frameNoEnd;
	public int FrameNoEnd
	{
		get
		{
			return(frameNoEnd);
		}
	}
	protected int frameNoNow;
	public int FrameNoNow
	{
		get
		{
			return(frameNoNow);
		}
	}
	protected int framePerSecond;
	public int FramePerSecond
	{
		get
		{
			return(framePerSecond);
		}
	}
	public float TimeFramePerSecond
	{
		get
		{
			return(1.0f / (float)framePerSecond);
		}
	}

	protected float TimeAnimation;

	private ArrayList ListCallBackUserData;
	private Library_SpriteStudio.FunctionCallBackUserData functionUserData = null;
	public Library_SpriteStudio.FunctionCallBackUserData FunctionUserData
	{
		set
		{
			functionUserData = value;
		}
		get
		{
			return(functionUserData);
		}
	}
	private Library_SpriteStudio.FunctionCallBackPlayEnd functionPlayEnd = null;
	public Library_SpriteStudio.FunctionCallBackPlayEnd FunctionPlayEnd
	{
		set
		{
			functionPlayEnd = value;
		}
		get
		{
			return(functionPlayEnd);
		}
	}

	public KindDrawQueue KindRenderQueueBase;
	public int OffsetDrawQueue;
	public float RateDrawQueueEffectZ;

	private Camera InstanceCameraDraw;
	public Material[] TableMaterial;
	private ArrayList TableListMesh;

	void Start()
	{
		AppendExecStart();
	}

	void Update()
	{
		AppendExecUpdate();
	}

	void LateUpdate()
	{
		AppendExecLateUpdate();
	}

	void OnDestroy()
	{
		/* MEMO: to make sure ... */
		MeshFilter InstanceMeshFilter = GetComponent<MeshFilter>();
		if(null != InstanceMeshFilter)
		{
			if(null != InstanceMeshFilter.sharedMesh)
			{
				InstanceMeshFilter.sharedMesh.Clear();
				Object.DestroyImmediate(InstanceMeshFilter.sharedMesh);
			}
			InstanceMeshFilter.sharedMesh = null;
		}
	}

	/* ******************************************************** */
	//! Get the index from the animation's name
	/*!
	@param	AnimationName
		Animation's name
	@retval	Return-Value
		Index of Animation
		-1 == Error / "AnimationName" is not-found.

	(Especially,) Get the Index by using this function when two or more animation data is recorded in the imported "ssae" data. <br>
	<br>
	The Index is the serial-number (0 origins) in the (imported "ssae") data. <br>
	The Index is needed when you call "AnimationPlay" function.
	*/
	public int AnimationGetIndexNo(string AnimationName)
	{
		if(true == string.IsNullOrEmpty(AnimationName))
		{
			return(-1);
		}

		for(int i=0; i<ListInformationPlay.Length; i++)
		{
			if(0 == AnimationName.CompareTo(ListInformationPlay[i].Name))
			{
				return(i);
			}
		}
		return(-1);
	}

	/* ******************************************************** */
	//! Start playing the animation
	/*!
	@param	No
		Animation's Index
	@param	CountLoop
		Number of looping <br>
		0 == Not looping <br>
		-1 == Infinity <br>
	@param	StartFrameNo
		Offset frame-number of starting Play in animation (0 origins). <br>
		default: 0（Top frame in animation）
	@param	RateFPS
		Coefficient of time-passage of animation (When 0.0 or less is given, the now-setting is not changed) <br>
		default: -1.0f (Setting is not changed)
	@retval	Return-Value
		true == Success <br>
		false == Failure (Error)

	The playing of animation begins. <br>
	<br>
	"No" is the Animation's Index (Get the Index in the "AnimationGetIndexNo" function.). <br>
	Give "0" to "No" when the animation included in (imported "ssae") data is one. <br>
	When the Animation's Index not existing is given, this function returns false. <br>
	<br>
	The update speed of animation quickens when you give a value that is bigger than 1.0f to "RateFPS".
	*/
	public bool AnimationPlay(int No, int CountLoop, int StartFrameNo=0, float RateTime=-1.0f)
	{
		if((0 > No) || (ListInformationPlay.Length <= No))
		{
			return(false);
		}
		if((0 > StartFrameNo) || ((ListInformationPlay[No].FrameEnd - ListInformationPlay[No].FrameStart) < StartFrameNo))
		{
			return(false);
		}

		AnimationNo = -1;	/* Set Status. */
		AnimationNo = No;
		CountLoopRemain = CountLoop;

		frameNoStart = ListInformationPlay[AnimationNo].FrameStart;
		frameNoEnd = ListInformationPlay[AnimationNo].FrameEnd;
		framePerSecond = ListInformationPlay[AnimationNo].FramePerSecond;
		frameNoNow = frameNoStart + StartFrameNo;	/* Set Status */
		RateTimeAnimation = (0.0f <= RateTime) ? RateTime : RateTimeAnimation;
		TimeAnimation = frameNoNow * TimeFramePerSecond;

		SpriteStudioData.Status |= Library_SpriteStudio.AnimationDataRuntime.BitStatus.REFRESH_PLAYFRAMENO;
		SpriteStudioData.Status &= ~Library_SpriteStudio.AnimationDataRuntime.BitStatus.MASK_RESET;
		SpriteStudioData.Status |= Library_SpriteStudio.AnimationDataRuntime.BitStatus.PLAYING;
		SpriteStudioData.Status &= ~Library_SpriteStudio.AnimationDataRuntime.BitStatus.PAUSING;
		if(0 != CountLoopRemain)
		{
			SpriteStudioData.Status |= Library_SpriteStudio.AnimationDataRuntime.BitStatus.LOOP;
		}
		else
		{
			SpriteStudioData.Status &= ~Library_SpriteStudio.AnimationDataRuntime.BitStatus.LOOP;
		}

		if(null == ListCallBackUserData)
		{
			ListCallBackUserData = new ArrayList();
		}
		ListCallBackUserData.Clear();

		return(true);
	}

	/* ******************************************************** */
	//! Stop playing the animation
	/*!
	@param
		(None)
	@retval	Return-Value
		(None)

	The playing of animation stops.
	*/
	public void AnimationStop()
	{
		SpriteStudioData.Status &= ~Library_SpriteStudio.AnimationDataRuntime.BitStatus.PLAYING;
	}

	/* ******************************************************** */
	//! Set the pause-status of the animation
	/*!
	@param	FlagSwitch
		true == Set pause (Suspend)<br>
		false == Rerease pause (Resume)
	@retval	Return-Value
		true == Success <br>
		false == Failure (Error)

	The playing of animation suspends or resumes. <br>
	This function fails if the animation is not playing.
	*/
	public bool AnimationPause(bool FlagSwitch)
	{
		if(true == FlagSwitch)
		{
			SpriteStudioData.Status |= Library_SpriteStudio.AnimationDataRuntime.BitStatus.PAUSING;
		}
		else
		{
			SpriteStudioData.Status &= ~Library_SpriteStudio.AnimationDataRuntime.BitStatus.PAUSING;
		}

		return(true);
	}

	/* ******************************************************** */
	//! Check the animation is playing
	/*!
	@param
		(None)
	@retval	Return-Value
		true == Playing / Pause-true(suspended) <br>
		false == Stopping

	Use this function for checking the animation's play-status. <br>
	*/
	public bool AnimationCheckPlay()
	{
		return((0 != (SpriteStudioData.Status & Library_SpriteStudio.AnimationDataRuntime.BitStatus.PLAYING)) ? true : false);
	}

	/* ******************************************************** */
	//! Check if the animation is being paused (suspended)
	/*!
	@param
		(None)
	@retval	Return-Value
		true == Suspended <br>
		false == Not Suspended or Stopping

	Use this function for checking the animation's pause-status. <br>
	*/
	public bool AnimationCheckPause()
	{
		return(((true == AnimationCheckPlay()) && (0 != (SpriteStudioData.Status & Library_SpriteStudio.AnimationDataRuntime.BitStatus.PAUSING))) ? true : false);
	}

	/* ******************************************************** */
	//! Force Boot-Up
	/*!
	@param
		(None)
	@retval	Return-Value
		(None)

	Don't use this function. <br>
	(This function is for the Importer)
	*/
	public void BootUpForce()
	{
		SpriteStudioData = new Library_SpriteStudio.SpriteData();
		SpriteStudioData.BootUp();

		RateTimeAnimation = 1.0f;
	}

	/* ******************************************************** */
	//! Get Material
	/*!
	@param	TextureNo
		Serial-number of using texture
	@param	Operation
		Color-Blend Operation for the target
	@retval	Return-Value
		(None)

	Don't use this function. <br>
	(This function is for the animation-parts' scripts.)
	*/
	public Material MaterialGet(int TextureNo, Library_SpriteStudio.KindColorOperation Operation)
	{
		int MaterialNo = TextureNo * ((int)Library_SpriteStudio.KindColorOperation.TERMINATOR - 1);
		return(((null != TableMaterial) && (0 <= TextureNo)) ? TableMaterial[MaterialNo] : null);
	}

	/* ******************************************************** */
	//! Add Mesh to Draw-Manager
	/*!
	@param	TextureNo
		Serial-number of using texture
	@param	Operation
		Color-Blend Operation for the target
	@param	DataMeshInformation
		Mesh Information
	@retval	Return-Value
		(None)

	Don't use this function. <br>
	(This function is for the animation-parts' scripts.)
	*/
	public void MeshAdd(int TextureNo, Library_SpriteStudio.KindColorOperation Operation, ref InformationMeshData DataMeshInformation)
	{
		if(0 > TextureNo)
		{
			return;
		}

		if(null == TableListMesh)
		{
			TableListMesh = new ArrayList();
			TableListMesh.Clear();
		}

		int MaterialNo = TextureNo * ((int)Library_SpriteStudio.KindColorOperation.TERMINATOR - 1) + ((int)Operation - 1);
		int CountList = TableListMesh.Count;
		int ListNo = -1;
		ListMeshDraw ListMesh = null;
		if(0 == TableListMesh.Count)
		{
			goto MeshAdd_NewListAdd;
		}
		else
		{
			ListNo = 0;
			ListMesh = TableListMesh[0] as ListMeshDraw;
			for(int i=1; i<CountList; i++)
			{
				ListMesh = TableListMesh[i] as ListMeshDraw;
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
				ListMesh = TableListMesh[ListNo] as ListMeshDraw;
				if(ListMesh.MaterialNo != MaterialNo)
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
			else
			{
				ListMesh = TableListMesh[ListNo] as ListMeshDraw;
				if(ListMesh.MaterialNo != MaterialNo)
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
							ListMeshDraw ListMeshNext = TableListMesh[ListNo] as ListMeshDraw;
							if(ListMeshNext.MaterialNo != MaterialNo)
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
			ListMesh.MeshAdd(ref DataMeshInformation);
		}
		return;

	MeshAdd_NewListAdd:
		ListMesh = new ListMeshDraw();
		ListMesh.MaterialNo = MaterialNo;
		TableListMesh.Add(ListMesh);

		ListMesh.MeshAdd(ref DataMeshInformation);
		return;

	MeshAdd_NewListInsert:
		ListMesh = new ListMeshDraw();
		ListMesh.MaterialNo = MaterialNo;
		TableListMesh.Insert(ListNo + 1, ListMesh);

		ListMesh.MeshAdd(ref DataMeshInformation);
		return;

	MeshAdd_NewListInsertSplit:
		{
			ListMeshDraw ListMeshSplit = ListMesh.ListSplit(DataMeshInformation.Priority);
			int ListNoNext = ListNo + 1;
			if(CountList <= ListNoNext)
			{
				TableListMesh.Add(ListMeshSplit);
			}
			else
			{
				TableListMesh.Insert(ListNoNext, ListMeshSplit);
			}
			int CountOld = ListMesh.Count;

			ListMesh = new ListMeshDraw();
			ListMesh.MaterialNo = MaterialNo;
			TableListMesh.Insert(ListNoNext, ListMesh);

			if(0 >= CountOld)
			{
				TableListMesh.RemoveAt(ListNo);
			}
		}

		ListMesh.MeshAdd(ref DataMeshInformation);
		return;
	}

	/* ******************************************************** */
	//! Registration of calling-back of "User-Data"
	/*!
	@param	PartsName
		Name of animation-part
	@param	AnimationDataParts
		control data for animation-part
	@param	Data
		Instance "User-Data"
	@retval	Return-Value
		(None)

	Don't use this function. <br>
	(This function is for the animation-parts' scripts.)
	*/
	public void CallBackExecUserData(string PartsName, Library_SpriteStudio.SpriteData AnimationDataParts, Library_SpriteStudio.KeyFrame.DataUser Data)
	{
		if(null == ListCallBackUserData)
		{
			ListCallBackUserData = new ArrayList();
			ListCallBackUserData.Clear();
		}

		ParameterCallBackUserData Parameter = new ParameterCallBackUserData();
		Parameter.PartsName = string.Copy(PartsName);
		Parameter.AnimationDataParts = AnimationDataParts;
		Parameter.FrameNo = Data.Time;
		Parameter.Data = Data.Value;
		ListCallBackUserData.Add(Parameter);
	}

	protected void AppendExecStart()
	{

		SpriteStudioData.BootUp();
		SpriteStudioData.PartsSetParent(null);
		SpriteStudioData.PartsSetRoot(this);
		CameraGetRendering();

		ListCallBackUserData = new ArrayList();
		ListCallBackUserData.Clear();

		if(null == ListInformationPlay)
		{
			frameNoStart = 0;
			frameNoEnd = 0;
			framePerSecond = 0;
		}

		AnimationPlay(animationNo, CountLoopRemain, frameNoInitial, RateTimeAnimation);
	}
	private void CameraGetRendering()
	{
		Transform InstanceTransform = transform;
		InstanceCameraDraw = null;
		while(null != InstanceTransform)
		{
			InstanceCameraDraw = InstanceTransform.camera;
			if(null != InstanceCameraDraw)
			{
				break;
			}
			InstanceTransform = InstanceTransform.parent;
		}
	}

	protected void AppendExecUpdate()
	{
		if(null != SpriteStudioData)
		{
			if(null != ListInformationPlay)
			{
				if(0 != (SpriteStudioData.Status & Library_SpriteStudio.AnimationDataRuntime.BitStatus.PLAYING))
				{
					if(0 == (SpriteStudioData.Status & Library_SpriteStudio.AnimationDataRuntime.BitStatus.PAUSING))
					{
						TimeAnimation += (Time.deltaTime * RateTimeAnimation);
						frameNoNow = (int)(TimeAnimation / TimeFramePerSecond);

						if(frameNoNow > frameNoEnd)
						{
							SpriteStudioData.Status |= Library_SpriteStudio.AnimationDataRuntime.BitStatus.FRAMEOVER;

							if(0 <= CountLoopRemain)
							{
								CountLoopRemain--;
								if(0 > CountLoopRemain)
								{
									SpriteStudioData.Status &= ~Library_SpriteStudio.AnimationDataRuntime.BitStatus.LOOP;
								}
							}

							if(0 != (SpriteStudioData.Status & Library_SpriteStudio.AnimationDataRuntime.BitStatus.LOOP))
							{
								int PlayLength = (frameNoEnd - frameNoStart) + 1;
								TimeAnimation -= ((float)PlayLength * TimeFramePerSecond);
								frameNoNow -= PlayLength;
							}
							else
							{
								TimeAnimation = ((float)frameNoEnd) * TimeFramePerSecond;
								frameNoNow = frameNoEnd;

								if(0 == (SpriteStudioData.Status & (Library_SpriteStudio.AnimationDataRuntime.BitStatus.CALLFUNCTION_PLAYEND | Library_SpriteStudio.AnimationDataRuntime.BitStatus.ENDFUNCTION_PLAYEND)))
								{
									SpriteStudioData.Status |= Library_SpriteStudio.AnimationDataRuntime.BitStatus.CALLFUNCTION_PLAYEND;
									SpriteStudioData.Status &= ~Library_SpriteStudio.AnimationDataRuntime.BitStatus.ENDFUNCTION_PLAYEND;
								}
							}
						}
						else
						{
							SpriteStudioData.Status &= ~Library_SpriteStudio.AnimationDataRuntime.BitStatus.FRAMEOVER;
						}
					}
				}

				SpriteStudioData.AnimationUpdate(gameObject);
				SpriteStudioData.AnimationFixTransform(transform);
			}
		}
	}

	protected void AppendExecLateUpdate()
	{
		int Count = 0;
		MeshFilter InstanceMeshFilter = GetComponent<MeshFilter>();
		MeshRenderer InstanceMeshRenderer = GetComponent<MeshRenderer>();

		if(0 != (SpriteStudioData.Status & Library_SpriteStudio.AnimationDataRuntime.BitStatus.PLAYING))
		{
			SpriteStudioData.Status &= ~(	Library_SpriteStudio.AnimationDataRuntime.BitStatus.REFRESH_PLAYRANGENO
											| Library_SpriteStudio.AnimationDataRuntime.BitStatus.REFRESH_PLAYFRAMENO
										);
		}

		if(null != TableListMesh)
		{
			ListMeshDraw ListMesh = null;
			Count = TableListMesh.Count;

			int CountMesh = 0;
			Material[] MaterialTable = new Material[Count];
			int ValueRenderQueue = ValueKindDrawQueue[(int)KindRenderQueueBase];
			int MaterialRenderQueue = 0;

			if(null == InstanceCameraDraw)
			{	/* Camera is lost ? */
				CameraGetRendering();
			}
			int ZOffset = 0;
			if(null != InstanceCameraDraw)
			{
				Vector3 PositionViewPort = (null != InstanceCameraDraw) ? InstanceCameraDraw.WorldToViewportPoint(transform.position) : Vector3.zero;
				float RateLinerZ = 1.0f - ((PositionViewPort.z - InstanceCameraDraw.nearClipPlane) / (InstanceCameraDraw.farClipPlane - InstanceCameraDraw.nearClipPlane));
				ZOffset = ((0.0f > RateLinerZ) || (1.0f < RateLinerZ)) ? -1 : (int)(RateLinerZ * RateDrawQueueEffectZ);
			}
			if(-1 == ZOffset)
			{	/* out of sight (Clipping) */
				InstanceMeshRenderer.enabled = false;
			}
			else
			{	/* in sight */
				InstanceMeshRenderer.enabled = true;
				for(int i=0; i<Count; i++)
				{
					ListMesh = TableListMesh[i] as ListMeshDraw;
					CountMesh += ListMesh.Count;

					MaterialTable[i] = new Material(TableMaterial[ListMesh.MaterialNo]);
					MaterialRenderQueue = (-1 == ValueRenderQueue) ? MaterialTable[i].shader.renderQueue : ValueRenderQueue;
					MaterialRenderQueue += (OffsetDrawQueue + ZOffset + i);
					MaterialTable[i].renderQueue = MaterialRenderQueue;
				}

				Material[] TableMaterialOld = InstanceMeshRenderer.sharedMaterials;
				InstanceMeshRenderer.sharedMaterials = MaterialTable;
				for(int i=0; i<TableMaterialOld.Length; i++)
				{
					Object.DestroyImmediate(TableMaterialOld[i]);
				}

				int IndexVertexNow = 0;
				int IndexTriangleNow = 0;
				int[] IndexVertex = new int[Count];
				int[] IndexTriangle = new int[Count+1];
				CombineInstance[] CombineMesh = new CombineInstance[CountMesh];
				InformationMeshData DataMeshInformation = null;
				CountMesh = 0;
				for(int i=0; i<Count; i++)
				{
					ListMesh = TableListMesh[i] as ListMeshDraw;
					IndexVertex[i] = IndexVertexNow;
					IndexTriangle[i] = IndexTriangleNow;
					DataMeshInformation = ListMesh.MeshDataTop;
					Matrix4x4 MatrixCorrect = transform.localToWorldMatrix.inverse;
					while(null != DataMeshInformation)
					{
						CombineMesh[CountMesh].mesh = DataMeshInformation.DataMesh;
						CombineMesh[CountMesh].transform = MatrixCorrect * DataMeshInformation.DataTransform.localToWorldMatrix;
						CountMesh++;
						IndexVertexNow += DataMeshInformation.DataMesh.vertexCount;
						IndexTriangleNow += DataMeshInformation.DataMesh.triangles.Length / 3;
						DataMeshInformation = DataMeshInformation.ChainNext;
					}
				}
				IndexTriangle[Count] = IndexTriangleNow;
				Mesh MeshNew = new Mesh();
				MeshNew.CombineMeshes(CombineMesh);

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
			}
			TableListMesh.Clear();
		}

		if((null != ListCallBackUserData) && (null != functionUserData))
		{
			Count = ListCallBackUserData.Count;
			ParameterCallBackUserData Parameter = null;
			for(int i=0; i<Count; i++)
			{
				Parameter = ListCallBackUserData[i] as ParameterCallBackUserData;
				functionUserData(	transform.parent.gameObject,
									Parameter.PartsName,
									Parameter.AnimationDataParts,
									AnimationNo,
									frameNoNow,
									Parameter.FrameNo,
									Parameter.Data
								);
			}

			ListCallBackUserData.Clear();
		}

		if((null != functionPlayEnd) && (0 != (SpriteStudioData.Status & Library_SpriteStudio.AnimationDataRuntime.BitStatus.CALLFUNCTION_PLAYEND)))
		{
			SpriteStudioData.Status &= ~Library_SpriteStudio.AnimationDataRuntime.BitStatus.CALLFUNCTION_PLAYEND;
			SpriteStudioData.Status |= Library_SpriteStudio.AnimationDataRuntime.BitStatus.ENDFUNCTION_PLAYEND;
			if(false == (functionPlayEnd(transform.parent.gameObject)))
			{
				InstanceMeshFilter.sharedMesh.Clear();
				Object.DestroyImmediate(InstanceMeshFilter.sharedMesh);
				InstanceMeshFilter.sharedMesh = null;

				Destroy(transform.parent.gameObject);
			}
		}
	}
}