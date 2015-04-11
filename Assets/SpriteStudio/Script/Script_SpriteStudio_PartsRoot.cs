/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[System.Serializable]
public class Script_SpriteStudio_PartsRoot : Library_SpriteStudio.PartsBase
{
	/* Constants */
	public enum BitStatus
	{
		PLAYING = 0x800000,
		PAUSING = 0x400000,
		
/*		STYLE_PINGPONG = 0x080000, *//* MEMO: Disuse */
		STYLE_REVERSE = 0x040000,
		PLAYING_REVERSE = 0x020000,
		PLAY_FIRST = 0x010000,

		REQUEST_PLAYEND = 0x008000,
		DECODE_USERDATA = 0x004000,
		REDECODE_INSTANCE = 0x002000,

		CLEAR = 0x000000,
		MASK_INITIAL = (PLAYING | PAUSING | STYLE_REVERSE | PLAYING_REVERSE | PLAY_FIRST | REQUEST_PLAYEND | DECODE_USERDATA),
	};
	public enum PlayStyle
	{
		NO_CHANGE = -1,
		NORMAL = 0,
		PINGPONG = 1,
	};

	/* Classes */
	private class ParameterCallBackUserData
	{
		public string PartsName = null;
		public Library_SpriteStudio.AnimationData AnimationDataParts = null;
		public int FrameNo = -1;
		public Library_SpriteStudio.KeyFrame.ValueUser.Data Data = null;
		public bool FlagWayBack = false;
	}

	public class ColorBlendOverwrite
	{
		public Library_SpriteStudio.KindColorBound Bound;
		public Library_SpriteStudio.KindColorOperation Operation;
		public Color[] VertexColor;

		public	ColorBlendOverwrite() : base()
		{
			Bound = Library_SpriteStudio.KindColorBound.NON;
			Operation = Library_SpriteStudio.KindColorOperation.MIX;
			VertexColor = new Color[(int)Library_SpriteStudio.VertexNo.TERMINATOR4];
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
			for(int i=0; i<(int)Library_SpriteStudio.VertexNo.TERMINATOR4; i++)
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
			VertexColor[(int)Library_SpriteStudio.VertexNo.LU] = DataColorLU;
			VertexColor[(int)Library_SpriteStudio.VertexNo.RU] = DataColorRU;
			VertexColor[(int)Library_SpriteStudio.VertexNo.RD] = DataColorRD;
			VertexColor[(int)Library_SpriteStudio.VertexNo.LD] = DataColorLD;
			VertexColor[(int)Library_SpriteStudio.VertexNo.C] =  ((DataColorLU + DataColorRU + DataColorRD + DataColorLD) * 0.25f);
		}
	}

	/* Variables & Propaties */
	private Library_SpriteStudio.AnimationInformationPlay[] listInformationPlay;
	public Library_SpriteStudio.AnimationInformationPlay[] ListInformationPlay
	{
		get
		{
			return(listInformationPlay);
		}
		set
		{
			listInformationPlay = value;
		}
	}

	private Library_SpriteStudio.AnimationData spriteStudioData;
	public Library_SpriteStudio.AnimationData SpriteStudioData
	{
		set
		{
			spriteStudioData = value;
		}
		get
		{
			return(spriteStudioData);
		}
	}

	/* CAUTION!: Don't use "ColorBlendOverwrite"-Property (for Player's Internal-Processing */
	private ColorBlendOverwrite dataColorBlendOverwrite = null;
	internal ColorBlendOverwrite DataColorBlendOverwrite
	{
		get
		{
			return(dataColorBlendOverwrite);
		}
	}

	public Collider CollisionComponent;
	private Library_SpriteStudio.AnimationData.WorkAreaRuntime WorkArea = null;

	internal BitStatus Status;
	public int ID;
	public Script_SpriteStudio_AnimationReferenced SpriteStudioDataReferenced;

	public float RateTimeAnimation;
	public string NameLabelStart;
	public string NameLabelEnd;
	public int OffsetFrameStart;
	public int OffsetFrameEnd;

	protected float TimeAnimation;
	protected float rateTimePlay;
	public float RateTimePlay
	{
		get
		{
			return(rateTimePlay);
		}
	}

	public bool FlagHideForce;
	public bool FlagStylePingpong;

	/* CAUTION!: This "rateAlpha" is value for "Instance"-Object. */
	private float rateOpacity;
	internal float RateOpacity
	{
		set
		{
			rateOpacity = value;
		}
		get
		{
			return(rateOpacity);
		}
	}

	/* CAUTION!: Don't set values from Code(Read-Only in principle). Use Function"AnimationPlay". */
	/*           "AnimationNo","CountLoopRemain" and "FrameNoInitial" are defined public for Setting on Inspector. */
	public int CountLoopRemain;
	public int PlayTimes
	{
		set
		{
			CountLoopRemain = (value < 0) ? -1 : (value - 1);
		}
		get
		{
			return(CountLoopRemain + 1);
		}
	}

	[SerializeField]
	protected int animationNo;
	public int AnimationNo
	{
		set
		{
			if((value != animationNo) && ((value < listInformationPlay.Length) && ((0 <= listInformationPlay.Length))))
			{
				AnimationStop();
				FrameNoInitial = 0;
				animationNo = value;
			}
		}
		get
		{
			return(animationNo);
		}
	}

	[SerializeField]
	protected int frameNoInitial;
	public int FrameNoInitial
	{
		set
		{
			if((0 <= value) && ((frameNoEnd - frameNoStart) >= value))
			{	/* Valid */
				frameNoInitial = value;
				TimeAnimation = value * TimeFramePerSecond;
			}
			else
			{	/* Invalid */
				frameNoInitial = 0;
				TimeAnimation = 0.0f;
			}
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
			return(frameNoStart);
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
	protected int frameNoPrevious = -1;
	public int FrameNoPrevious
	{
		get
		{
			return(frameNoPrevious);
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

	private int countLoopThisTime;
	internal int CountLoopThisTime
	{
		get
		{
			return(countLoopThisTime);
		}
	}

	private bool flagReversePrevious;
	internal bool FlagReversePrevious
	{
		get
		{
			return(flagReversePrevious);
		}
	}

	private bool flagTurnBackPingPong;
	internal bool FlagTurnBackPingPong
	{
		get
		{
			return(flagTurnBackPingPong);
		}
	}

#if false
	/* MEMO: Non-Generic List-Class */
	private ArrayList ListCallBackUserData;
#else
	private List<ParameterCallBackUserData> ListCallBackUserData;
#endif
	private Library_SpriteStudio.FunctionCallBackUserData functionUserData;
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
	private Library_SpriteStudio.FunctionCallBackPlayEnd functionPlayEnd;
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

	private Library_SpriteStudio.DrawManager.ArrayListMeshDraw arrayListMeshDraw;
	internal Library_SpriteStudio.DrawManager.ArrayListMeshDraw ArrayListMeshDraw
	{
		get
		{
			return(arrayListMeshDraw);
		}
		set
		{
			arrayListMeshDraw = value;
		}
	}

	private Camera InstanceCameraDraw;
	private Script_SpriteStudio_DrawManagerView InstanceDrawManagerView;
	private GameObject InstanceGameObjectControl = null;
	private Script_SpriteStudio_PartsRoot partsRootOrigin = null;
	public Script_SpriteStudio_PartsRoot PartsRootOrigin
	{	/* CAUTION!: Public-Scope for Editor & Inspector */
		get
		{
			return(partsRootOrigin);
		}
		set
		{
			partsRootOrigin = value;
		}
	}

	public Material[] TableMaterial;

	void Awake()
	{
		var root = gameObject.GetComponent<Script_SpriteStudio_PartsRoot>();
		foreach(var MaterialNow in root.TableMaterial)
		{
			MaterialNow.shader = Shader.Find(MaterialNow.shader.name);
		}
		Status = ~BitStatus.MASK_INITIAL;
	}

	void Start()
	{
		InstanceCameraDraw = Library_SpriteStudio.Utility.CameraGetParent(gameObject);
		InstanceDrawManagerView = Library_SpriteStudio.Utility.DrawManagerViewGetParent(gameObject);
		rateOpacity = 1.0f;

		arrayListMeshDraw = new Library_SpriteStudio.DrawManager.ArrayListMeshDraw();
		arrayListMeshDraw.BootUp();

#if false
		/* MEMO: Non-Generic List-Class */
		ListCallBackUserData = new ArrayList();
		ListCallBackUserData.Clear();
#else
		ListCallBackUserData = new List<ParameterCallBackUserData>();
		ListCallBackUserData.Clear();
#endif

		/* Get Animation-Data-Referenced */
		if(null != SpriteStudioDataReferenced)
		{
			listInformationPlay = SpriteStudioDataReferenced.ListInformationAnimation;
			spriteStudioData = SpriteStudioDataReferenced.DataGetNode(ID);
		}
		if(null == listInformationPlay)
		{
			frameNoStart = 0;
			frameNoEnd = 0;
			framePerSecond = 0;
		}
		if(0 == (Status & BitStatus.PLAYING))
		{
			AnimationPlay();
		}
	}

	private int MainLoopCount = 0;
	void Update()
	{
		MainLoopCount++;

		/* Get Animation-Data-Referenced */
		if((null != SpriteStudioDataReferenced) && (null == spriteStudioData))
		{
			listInformationPlay = SpriteStudioDataReferenced.ListInformationAnimation;
			spriteStudioData = SpriteStudioDataReferenced.DataGetNode(ID);
		}

		/* Boot-Check */
		if(null == InstanceCameraDraw)
		{
			InstanceCameraDraw = Library_SpriteStudio.Utility.CameraGetParent(gameObject);
		}
		if(null == InstanceDrawManagerView)
		{
			InstanceDrawManagerView = Library_SpriteStudio.Utility.DrawManagerViewGetParent(gameObject);
		}
		if(null == arrayListMeshDraw)
		{
			arrayListMeshDraw = new Library_SpriteStudio.DrawManager.ArrayListMeshDraw();
			arrayListMeshDraw.BootUp();
		}
		if(null == ListCallBackUserData)
		{
#if false
			/* MEMO: Non-Generic List-Class */
			ListCallBackUserData = new ArrayList();
			ListCallBackUserData.Clear();
#else
			ListCallBackUserData = new List<ParameterCallBackUserData>();
			ListCallBackUserData.Clear();
#endif
		}
		if(null == WorkArea)
		{
			WorkArea = new Library_SpriteStudio.AnimationData.WorkAreaRuntime();
		}

		/* Entry Object to Draw */
		if((null != InstanceDrawManagerView) && (null == partsRootOrigin))
		{	/* Not "Instance"-Object */
			if(false == FlagHideForce)
			{
				InstanceDrawManagerView.DrawEntryObject(this);
			}
		}

		/* Animation Update */
		if(0 == (Status & BitStatus.PLAYING))
		{
			return;
		}

		if(0 != (Status & BitStatus.PAUSING))
		{
			return;
		}
		
		int FrameCountNow = 0;
		int FrameCountEnd = frameNoEnd - frameNoStart;
		int FrameCountFull = FrameCountEnd + 1;

		float RateTimeProgress = (0 == (Status & BitStatus.PLAYING_REVERSE)) ? 1.0f : -1.0f;
		float TimeAnimationFull = (float)FrameCountFull * TimeFramePerSecond;
		
		bool FlagLoop = false;
		bool FlagReLoop = true;

		/* FrameNo Update */
		Status |= BitStatus.PLAY_FIRST;
		if(-1 != frameNoPrevious)
		{	/* Not Update, Just Starting */
			TimeAnimation += (Time.deltaTime * rateTimePlay) * RateTimeProgress;
			Status &= ~BitStatus.PLAY_FIRST;
			Status &= ~BitStatus.DECODE_USERDATA;
			Status &= ~BitStatus.REDECODE_INSTANCE;
		}

		FrameCountNow = (int)(TimeAnimation / TimeFramePerSecond);
		countLoopThisTime = 0;
		flagTurnBackPingPong = false;
		flagReversePrevious = (0 != (Status & BitStatus.PLAYING_REVERSE)) ? true : false;

		if(false == FlagStylePingpong)
		{	/* One-Way */
			if(0 == (Status & BitStatus.PLAYING_REVERSE))
			{	/* Play foward */
				/* Get Frame Count */
				if(FrameCountEnd < FrameCountNow)
				{	/* Frame-Over */
					FlagLoop = true;
					FlagReLoop = true;
					while(true == FlagReLoop)
					{
						/* Loop-Count Check */
						if(0 <= CountLoopRemain)
						{	/* Limited-Count Loop */
							CountLoopRemain--;
							FlagLoop = (0 > CountLoopRemain) ? false : FlagLoop;
						}
						
						/* ReCalculate Frame */
						if(true == FlagLoop)
						{	/* Loop */
							countLoopThisTime++;

							/* ReCalculate Frame */
							TimeAnimation -= TimeAnimationFull;
							FrameCountNow = (int)(TimeAnimation / TimeFramePerSecond);
							FlagReLoop = (TimeAnimationFull <= TimeAnimation) ? true : false;
							
							/* Set Instance Parts Restart Request */
							Status |= BitStatus.REDECODE_INSTANCE;

							/* Force-Decode UserData */
							Status |= BitStatus.DECODE_USERDATA;
						}
						else
						{	/* End */
							TimeAnimation = ((float)FrameCountEnd) * TimeFramePerSecond;
							FrameCountNow = FrameCountEnd;
//							Status &= ~BitStatus.PLAYING;
							Status |= BitStatus.REQUEST_PLAYEND;
							FlagReLoop = false;
						}
					}
				}
			}
			else
			{	/* Play backwards */
				/* Get Frame Count */
				if(0 > FrameCountNow)
				{	/* Frame-Over */
					FlagLoop = true;
					FlagReLoop = true;
					while(true == FlagReLoop)
					{
						/* Loop-Count Check */
						if(0 <= CountLoopRemain)
						{	/* Limited-Count Loop */
							CountLoopRemain--;
							FlagLoop = (0 > CountLoopRemain) ? false : FlagLoop;
						}
						
						/* ReCalculate Frame */
						if(true == FlagLoop)
						{	/* Loop */
							countLoopThisTime++;

							/* ReCalculate Frame */
							TimeAnimation += TimeAnimationFull;
							FrameCountNow = (int)(TimeAnimation / TimeFramePerSecond);
							FlagReLoop = (0.0f > TimeAnimation) ? true : false;
							
							/* Set Instance Parts Restart Request */
							Status |= BitStatus.REDECODE_INSTANCE;

							/* Force-Decode UserData */
							Status |= BitStatus.DECODE_USERDATA;
						}
						else
						{	/* End */
							TimeAnimation = 0.0f;
							FrameCountNow = 0;
//							Status &= ~BitStatus.PLAYING;
							Status |= BitStatus.REQUEST_PLAYEND;
							FlagReLoop = false;
						}
					}
				}
			}
		}
		else
		{	/* Ping-Pong */
			if(0 == (Status & BitStatus.STYLE_REVERSE))
			{	/* Style-Normaly */
				FlagReLoop = true;
				while(true == FlagReLoop)
				{
					FlagReLoop = false;
					
					if(0 == (Status & BitStatus.PLAYING_REVERSE))
					{	/* Play foward */
						if(FrameCountEnd < FrameCountNow)
						{	/* Frame-Over */
							/* Set Turn-Back */
							Status |= BitStatus.PLAYING_REVERSE;

							/* ReCalculate Frame */
							TimeAnimation -= TimeAnimationFull;
							TimeAnimation = TimeAnimationFull - TimeAnimation;
							FrameCountNow = (int)(TimeAnimation / TimeFramePerSecond);
							flagTurnBackPingPong = true;

							/* Force-Decode UserData */
							Status |= BitStatus.DECODE_USERDATA;

							/* Plural Loop Count Check */
							FlagReLoop = ((TimeAnimationFull > TimeAnimation) && (0.0f <= TimeAnimation)) ? false : true;
						}
					}
					else
					{	/* Play backwards */
						if(0 > FrameCountNow)
						{	/* Frame-Over */
							FlagLoop = true;

							/* Loop-Count Check */
							if(0 <= CountLoopRemain)
							{	/* Limited-Count Loop */
								CountLoopRemain--;
								FlagLoop = (0 > CountLoopRemain) ? false : FlagLoop;
							}
							
							if(true == FlagLoop)
							{	/* Loop */
								countLoopThisTime++;

								/* Set Turn-Back */
								Status &= ~BitStatus.PLAYING_REVERSE;
								
								/* ReCalculate Frame */
								TimeAnimation += TimeAnimationFull;
								TimeAnimation = TimeAnimationFull - TimeAnimation;
								FrameCountNow = (int)(TimeAnimation / TimeFramePerSecond);
								flagTurnBackPingPong = true;
								
								/* Set Instance Parts Restart Request */
								Status |= BitStatus.REDECODE_INSTANCE;

								/* Force-Decode UserData */
								Status |= BitStatus.DECODE_USERDATA;

								/* Plural Loop Count Check */
								FlagReLoop = ((TimeAnimationFull > TimeAnimation) && (0.0f <= TimeAnimation)) ? false : true;
							}
							else
							{	/* End */
								TimeAnimation = 0.0f;
								FrameCountNow = 0;
//								Status &= ~BitStatus.PLAYING;
								Status |= BitStatus.REQUEST_PLAYEND;
								FlagReLoop = false;
							}
						}
					}
				}
			}
			else
			{	/* Style-Reverse */
				FlagReLoop = true;
				while(true == FlagReLoop)
				{
					FlagReLoop = false;
					
					if(0 != (Status & BitStatus.PLAYING_REVERSE))
					{	/* Play backwards */
						if(0 > FrameCountNow)
						{	/* Frame-Over */
							/* Set Turn-Back */
							Status &= ~BitStatus.PLAYING_REVERSE;
							
							/* ReCalculate Frame */
							TimeAnimation += TimeAnimationFull;
							TimeAnimation = TimeAnimationFull - TimeAnimation;
							FrameCountNow = (int)(TimeAnimation / TimeFramePerSecond);
							flagTurnBackPingPong = true;

							/* Force-Decode UserData */
							Status |= BitStatus.DECODE_USERDATA;

							/* Plural Loop Count Check */
							FlagReLoop = ((TimeAnimationFull > TimeAnimation) && (0.0f <= TimeAnimation)) ? false : true;
						}
					}
					else
					{	/* Play foward */
						if(FrameCountEnd < FrameCountNow)
						{	/* Frame-Over */
							FlagLoop = true;

							/* Loop-Count Check */
							if(0 <= CountLoopRemain)
							{	/* Limited-Count Loop */
								CountLoopRemain--;
								FlagLoop = (0 > CountLoopRemain) ? false : FlagLoop;
							}
							
							if(true == FlagLoop)
							{	/* Loop */
								countLoopThisTime++;

								/* Set Turn-Back */
								Status |= BitStatus.PLAYING_REVERSE;
								
								/* ReCalculate Frame */
								TimeAnimation -= TimeAnimationFull;
								TimeAnimation = TimeAnimationFull - TimeAnimation;
								FrameCountNow = (int)(TimeAnimation / TimeFramePerSecond);
								flagTurnBackPingPong = true;

								/* Set Instance Parts Restart Request */
								Status |= BitStatus.REDECODE_INSTANCE;

								/* Force-Decode UserData */
								Status |= BitStatus.DECODE_USERDATA;

								/* Plural Loop Count Check */
								FlagReLoop = ((TimeAnimationFull > TimeAnimation) && (0.0f <= TimeAnimation)) ? false : true;
							}
							else
							{	/* End */
								TimeAnimation = ((float)FrameCountEnd) * TimeFramePerSecond;
								FrameCountNow = FrameCountEnd;
//								Status &= ~BitStatus.PLAYING;
								Status |= BitStatus.REQUEST_PLAYEND;
								FlagReLoop = false;
							}
						}
					}
				}
			}
		}
		
		/* Member-Valiables Update */
		int FrameNoNew = frameNoStart + FrameCountNow;
		if(FrameNoNew != frameNoNow)
		{
			Status |= BitStatus.DECODE_USERDATA;
		}
		frameNoPrevious = frameNoNow;
		frameNoNow = FrameNoNew;
		
		/* Update User-CallBack */
		spriteStudioData.UpdateUserData(frameNoNow, gameObject, this);
		
		/* Update GameObject */
		spriteStudioData.UpdateGameObject(gameObject, frameNoNow, CollisionComponent, WorkArea);
	}

	void LateUpdate()
	{
		/* Excute "UserData CallBack" */
		if((null != ListCallBackUserData) && (null != functionUserData))
		{
			int Count = ListCallBackUserData.Count;
			ParameterCallBackUserData Parameter = null;
			for(int i=0; i<Count; i++)
			{
#if false
				/* MEMO: Non-Generic List-Class */
				Parameter = ListCallBackUserData[i] as ParameterCallBackUserData;
#else
				Parameter = ListCallBackUserData[i];
#endif
#if false
				/* MEMO: Until "Ver.1.2.3" */
				functionUserData(	transform.parent.gameObject,
									Parameter.PartsName,
									Parameter.AnimationDataParts,
									AnimationNo,
									frameNoNow,
									Parameter.FrameNo,
									Parameter.Data,
									Parameter.FlagWayBack
								);
#else
				/* MEMO: Later than "Ver.1.2.3" */
				functionUserData(	((null == InstanceGameObjectControl) ? gameObject : InstanceGameObjectControl),
									Parameter.PartsName,
									Parameter.AnimationDataParts,
									AnimationNo,
									frameNoNow,
									Parameter.FrameNo,
									Parameter.Data,
									Parameter.FlagWayBack
								);
#endif
			}
			ListCallBackUserData.Clear();
		}

		/* Excute "Play-End CallBack" */
		if(0 != (Status & BitStatus.REQUEST_PLAYEND))
		{
			Status = BitStatus.CLEAR;
			if(null != functionPlayEnd)
			{
				if(null == InstanceGameObjectControl)
				{	/* has no Control-Node */
					if(false == functionPlayEnd(gameObject))
					{
						Object.Destroy(gameObject);
					}
				}
				else
				{	/* has Control-Node */
					if(false == functionPlayEnd(InstanceGameObjectControl))
					{
						Object.Destroy(InstanceGameObjectControl);
					}
				}
			}
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

		/* Get Animation-Data-Referenced */
		if(null != SpriteStudioDataReferenced)
		{
			listInformationPlay = SpriteStudioDataReferenced.ListInformationAnimation;
//			spriteStudioData = SpriteStudioDataReferenced.DataGetNode(ID);
		}

		for(int i=0; i<listInformationPlay.Length; i++)
		{
			if(0 == AnimationName.CompareTo(listInformationPlay[i].Name))
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
		Animation's Index<br>
		-1 == Now-Setting Index is not changed<br>
		default: -1
	@param	PlayTimes
		-1 == Now-Setting "CountLoopRemain" is not changed
		0 == Infinite-looping <br>
		1 == Not looping <br>
		2 <= Number of Plays<br>
		default: -1
	@param	FrameInitial
		Offset frame-number of starting Play in animation (0 origins). <br>
		At the time of the first play-loop, Animation is started "LabelStart + FrameOffsetStart + FrameInitial".
		-1 == use "FrameNoInitial" Value<br>
		default: -1
	@param	RateTime
		Coefficient of time-passage of animation.<br>
		Minus Value is given, Animation is played backwards.<br>
		0.0f is given, the now-setting is not changed) <br>
		default: 0.0f (Setting is not changed)
	@param	KindStylePlay
		PlayStyle.NOMAL == Animation is played One-Way.<br>
		PlayStyle.PINGPONG == Animation is played Wrap-Around.<br>
		PlayStyle.NO_CHANGE == use "Play-Pingpong" Setting.
		default: PlayStyle.NO_CHANGE
	@param	LabelStart
		Label-name of starting Play in animation.
		"_start" == Top-frame of Animation (reserved label-name)<br>
		"" == use "NameLabelStart"<br>
		default: ""
	@param	FrameOffsetStart
		Offset frame-number from LabelStart
		Animation's Top-frame is "LabelStart + FrameOffsetStart".<br>
		int.MinValue == use "OffsetFrameStart"
		default: int.MinValue
	@param	LabelEnd
		Label-name of the terminal in animation.
		"_end" == Last-frame of Animation (reserved label-name)<br>
		"" == use "NameLabelEnd"<br>
		default: ""
	@param	FrameOffsetEnd
		Offset frame-number from LabelEnd
		Animation's Last-frame is "LabelEnd + FrameOffsetEnd".<br>
		int.MaxValue == use "OffsetFrameEnd"
		default: int.MaxValue
	@retval	Return-Value
		true == Success <br>
		false == Failure (Error)

	The playing of animation begins. <br>
	<br>
	"No" is the Animation's Index (Get the Index in the "AnimationGetIndexNo" function.). <br>
	Give "0" to "No" when the animation included in (imported "ssae") data is one. <br>
	When the Animation's Index not existing is given, this function returns false. <br>
	<br>
	The update speed of animation quickens when you give a value that is bigger than 1.0f to "RateTime".
	*/
	public bool AnimationPlay(	int No = -1,
								int TimesPlay = -1,
								int FrameInitial = -1,
								float RateTime = 0.0f,
								PlayStyle KindStylePlay = PlayStyle.NO_CHANGE,
								string LabelStart = "",
								int FrameOffsetStart = int.MinValue,
								string LabelEnd = "",
								int FrameOffsetEnd = int.MaxValue
							)
	{
		/* Error-Check */
		animationNo = (-1 != No) ? No : animationNo;	/* Don't Use "AnimationNo" (occur "Stack-Overflow") */
		if((0 > animationNo) || (listInformationPlay.Length <= animationNo))
		{
			return(false);
		}

		/* Set Playing-Datas */
		Status &= ~BitStatus.MASK_INITIAL;
		Status |= BitStatus.PLAYING;
		Status |= BitStatus.DECODE_USERDATA;
		Status |= BitStatus.REDECODE_INSTANCE;
		switch(KindStylePlay)
		{
			case PlayStyle.NO_CHANGE:
				break;
			case PlayStyle.NORMAL:
				FlagStylePingpong = false;
				break;
			case PlayStyle.PINGPONG:
				FlagStylePingpong = true;
				break;
			default:
				goto case PlayStyle.NO_CHANGE;
		}

		/* Set Animation Information */
		int FrameNo;

		Library_SpriteStudio.AnimationInformationPlay InformationAnimation = listInformationPlay[animationNo];
		string Label = "";

		Label = string.Copy((true == string.IsNullOrEmpty(LabelStart)) ? NameLabelStart : LabelStart);
		if(true == string.IsNullOrEmpty(Label))
		{
			Label = string.Copy(Library_SpriteStudio.AnimationInformationPlay.LabelDefaultStart);
		}
		FrameNo = InformationAnimation.FrameNoGetLabel(Label);
		if(-1 == FrameNo)
		{	/* Label Not Found */
			FrameNo = InformationAnimation.FrameStart;
		}
		FrameNo += (int.MinValue == FrameOffsetStart) ? OffsetFrameStart : FrameOffsetStart;
		if((InformationAnimation.FrameStart > FrameNo) || (InformationAnimation.FrameEnd < FrameNo))
		{
			FrameNo = InformationAnimation.FrameStart;
		}
		frameNoStart = FrameNo;

		Label = string.Copy((true == string.IsNullOrEmpty(LabelEnd)) ? NameLabelEnd : LabelEnd);
		if(true == string.IsNullOrEmpty(Label))
		{
			Label = string.Copy(Library_SpriteStudio.AnimationInformationPlay.LabelDefaultEnd);
		}
		FrameNo = InformationAnimation.FrameNoGetLabel(Label);
		if(-1 == FrameNo)
		{	/* Label Not Found */
			FrameNo = InformationAnimation.FrameEnd;
		}
		FrameNo += (int.MaxValue == FrameOffsetEnd) ? OffsetFrameEnd : FrameOffsetEnd;
		if((InformationAnimation.FrameStart > FrameNo) || (InformationAnimation.FrameEnd < FrameNo))
		{
			FrameNo = InformationAnimation.FrameEnd;
		}
		frameNoEnd = FrameNo;

		framePerSecond = (null == partsRootOrigin) ? InformationAnimation.FramePerSecond : partsRootOrigin.FramePerSecond;

		int CountFrame = (frameNoEnd - frameNoStart) + 1;
		if(-1 == FrameInitial)
		{	/* Use "FrameNoInitial" */
			FrameInitial = ((0 <= FrameNoInitial) && (CountFrame > FrameNoInitial)) ? FrameNoInitial : 0;
		}
		else
		{	/* Direct-Frame */
			FrameInitial = ((0 <= FrameInitial) && (CountFrame > FrameInitial)) ? FrameInitial : 0;
		}
//		frameNoNow = FrameInitial;
		frameNoNow = FrameInitial + frameNoStart;
		frameNoPrevious = -1;

		RateTime = (0.0f == RateTime) ? RateTimeAnimation : RateTime;
		if(0.0f > RateTime)
		{
			Status = (0 == (Status & BitStatus.STYLE_REVERSE)) ? (Status | BitStatus.STYLE_REVERSE) : (Status & ~BitStatus.STYLE_REVERSE);
			RateTime *= -1.0f;
		}
		rateTimePlay = RateTime;

		Status |= (0 != (Status & BitStatus.STYLE_REVERSE)) ? BitStatus.PLAYING_REVERSE : 0;
		if(0 != (Status & BitStatus.PLAYING_REVERSE))
		{	/* Play-Reverse & Start Top-Frame */
			frameNoNow = (frameNoNow <= frameNoStart) ? frameNoEnd : frameNoNow;
		}
		else
		{	/* Play-Normal & Start End-Frame */
			frameNoNow = (frameNoNow >= frameNoEnd) ? frameNoStart : frameNoNow;
		}
//		TimeAnimation = frameNoNow * TimeFramePerSecond;
		TimeAnimation = (frameNoNow - frameNoStart) * TimeFramePerSecond;

		if(-1 != TimesPlay)
		{
			/* MEMO: TimesPlay is Invalid, Force Play-Once */
			CountLoopRemain = (0 > TimesPlay) ? 0 : (TimesPlay - 1);
		}

		/* UserData-CallBack Buffer Create */
		if(null == ListCallBackUserData)
		{
#if false
			/* MEMO: Non-Generic List-Class */
			ListCallBackUserData = new ArrayList();
#else
			ListCallBackUserData = new List<ParameterCallBackUserData>();
#endif
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
		Status &= ~BitStatus.PLAYING;
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
		Status = (true == FlagSwitch) ? (Status | BitStatus.PAUSING) : (Status & ~BitStatus.PAUSING);
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
		return((0 != (Status & BitStatus.PLAYING)) ? true : false);
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
		return(((true == AnimationCheckPlay()) && (0 != (Status & BitStatus.PAUSING))) ? true : false);
	}

	/* ******************************************************** */
	//! Force-Hide Set
	/*!
	@param	FlagSwitch
		true == Force-Hide Set (Hide)<br>
		false == Force-Hide Reset (Show. State of animation is followed.)<br>
	@param	FlagSetChild
		true == Children are set same state.<br>
		false == only oneself.<br>
	@param	FlagSetInstance
		true == "Instance"-Objects are set same state.<br>
		false == "Instance"-Objects are ignored.<br>
	@retval	Return-Value
		(None)
	
	The state of "Force-Hide" is set, it is not concerned with the state of animation.
	*/
	public void HideSetForce(bool FlagSwitch, bool FlagSetChild=false, bool FlagSetInstance=false)
	{
		Library_SpriteStudio.Utility.HideSetForce(gameObject, FlagSwitch, FlagSetChild, FlagSetInstance);
	}

	/* ******************************************************** */
	//! Get Material
	/*!
	@param	TextureNo
		Serial-number of using texture
	@param	Operation
		Color-Blend Operation for the target
	@retval	Return-Value
		Material
	*/
	public Material MaterialGet(int TextureNo, Library_SpriteStudio.KindColorOperation Operation)
	{
		int MaterialNo = TextureNo * ((int)Library_SpriteStudio.KindColorOperation.TERMINATOR - 1);
		MaterialNo += (int)Operation - 1;
		return(((null != TableMaterial) && (0 <= TextureNo)) ? TableMaterial[MaterialNo] : null);
	}

	/* ******************************************************** */
	//! Copy (Dupicate) Material-Table
	/*!
	@param
		(none)
	@retval	Return-Value
		Created Copy Material-Table<br>
		null == Failure

	Generate a duplicate of "TableMaterial".<br>
	Caution: The result is another instance the "TableMaterial".<br>
	*/
	public Material[] TableCopyMaterial()
	{
		return(Library_SpriteStudio.UtilityMaterial.TableCopyMaterial(TableMaterial));
	}

	/* ******************************************************** */
	//! Get count of Textures
	/*!
	@param
		(none)
	@retval	Return-Value
		Count of Texture<br>
		0 == Failure

	Get count of textures that can be set to "TableMaterial".
	*/
	public int TextureGetCount()
	{
		return(Library_SpriteStudio.UtilityMaterial.TextureGetCount(TableMaterial));
	}

	/* ******************************************************** */
	//! Change 1-Texture in Material-Table
	/*!
	@param	Index
		Texture No.
	@param	DataTexture
		Texture
	@retval	Return-Value
		true == Success<br>
		false == Failure

	Change 1-Texture is set in "TableMaterial".
	Texture is set to "TableMaterial[Index * 4] to TableMaterial[Index * 4 + 3]".<br>
	<br>
	Appropriate range of "Material"-instance of the texture you want to change will be created in the new.<br>
	*/
	public bool TextureChange(int Index, Texture2D DataTexture)
	{
		int CountTextureBlock = (int)Library_SpriteStudio.KindColorOperation.TERMINATOR - 1;

		if(null == TableMaterial)
		{
			return(false);
		}
		int CountTableMaterial = TableMaterial.Length;
		if(0 >= CountTableMaterial)
		{
			return(false);
		}

		int IndexMaterial = Index * CountTextureBlock;
		if((0 > IndexMaterial) || (CountTableMaterial <= IndexMaterial))
		{
			return(false);
		}
		for(int i=0; i<CountTextureBlock; i++)
		{
			TableMaterial[IndexMaterial + i] = new Material(Library_SpriteStudio.Shader_SpriteStudioTriangleX[i]);
			TableMaterial[IndexMaterial + i].mainTexture = DataTexture;
		}
		return(true);
	}

	/* ******************************************************** */
	//! Registration of calling-back of "User-Data"
	/*!
	@param	PartsName
		Name of animation-part
	@param	AnimationDataParts
		control data for animation-part
	@param	FrameNoData
		Frame-No, "User-Data" is arranged
	@param	Data
		Instance "User-Data"
	@retval	Return-Value
		(None)

	Don't use this function. <br>
	(This function is for the animation-parts' scripts.)
	*/
	internal void CallBackExecUserData(string PartsName, Library_SpriteStudio.AnimationData AnimationDataParts, int FrameNoData, Library_SpriteStudio.KeyFrame.ValueUser.Data Data, bool FlagWayBack)
	{
		if(null == ListCallBackUserData)
		{
#if false
			/* MEMO: Non-Generic List-Class */
			ListCallBackUserData = new ArrayList();
			ListCallBackUserData.Clear();
#else
			ListCallBackUserData = new List<ParameterCallBackUserData>();
			ListCallBackUserData.Clear();
#endif
		}

		ParameterCallBackUserData Parameter = new ParameterCallBackUserData();
		Parameter.PartsName = string.Copy(PartsName);
		Parameter.AnimationDataParts = AnimationDataParts;
		Parameter.FrameNo = FrameNoData;
		Parameter.FlagWayBack = FlagWayBack;
		Parameter.Data = Data;
		ListCallBackUserData.Add(Parameter);
	}

	/* ******************************************************** */
	//! Draw-List Clear
	/*!
	@param	
		(None)
	@retval	Return-Value
		(None)

	Don't use this function. <br>
	(This function is for the Draw-Manager's scripts.)
	*/
	internal void DrawListClear()
	{
		arrayListMeshDraw.Clear();
	}

	/* ******************************************************** */
	//! Force-Set Offset-Time
	/*!
	@param	TimeElapsed
		Offset Time
	@retval	Return-Value
		(None)

	Don't use this function. <br>
	(This function is for the Instance-Parts' scripts.)
	*/
	internal void TimeElapsedSetForce(float TimeElapsed, bool FlagIndependent)
	{
		int FrameCount = (frameNoEnd - frameNoStart) + 1;
		float TimeRate = (true == FlagIndependent) ? rateTimePlay : 1.0f;
		float TimeRange = (float)FrameCount * TimeFramePerSecond;
		TimeAnimation = (TimeElapsed * TimeRate) % TimeRange;
	}

	/* ******************************************************** */
	//! Force-Set Offset-Time
	/*!
	@param	GameObjectControl
		Control-GameObject
	@retval	Return-Value
		(None)

	Don't use this function. <br>
	(This function is for the Link-Prefab-Parts' scripts.)
	*/
	internal void NodeSetControl(GameObject GameObjectControl)
	{
		InstanceGameObjectControl = GameObjectControl;
	}

	/* ******************************************************** */
	//! Force Boot-Up
	/*!
	@param
		(None)
	@retval	Return-Value
		(None)

	Don't use this function. <br>
	(This function is for the Importer in Editor)
	*/
	public void BootUpForce()
	{
		SpriteStudioData = new Library_SpriteStudio.AnimationData();
		RateTimeAnimation = 1.0f;
		Status = BitStatus.CLEAR;
	}

	/* ******************************************************** */
	//! Get "Overlay-Color" Parameter
	/*!
	@param
		(None)
	@retval	Return-Value
		"Overlay-Color" Setting Buffer

	Get the Parameter-Buffer of "Overlay-Color("Color-Blend" Attribute)" for instances of this class.<br>
	Set the contents of this buffer, it is possible to overwrite the state of the Overlay-Color from the script.<br>
	<br>
	The detail of how to set, please refer to the commentary of "Script_SpriteStudio_PartsRoot::ColorBlendOverwrite".
	*/
	public ColorBlendOverwrite DataGetColorBlendOverwrite()
	{
		if(null == dataColorBlendOverwrite)
		{
			dataColorBlendOverwrite = new ColorBlendOverwrite();
		}
		return(dataColorBlendOverwrite);
	}

	/* ******************************************************** */
	//! Delete "Overlay-Color" Parameter
	/*!
	@param
		(None)
	@retval	Return-Value
		(None)

	Delete the Parameter-Buffer of "Overlay-Color".
	*/
	public void DataReleaseColorBlendOverwrite()
	{
		dataColorBlendOverwrite = null;
	}
}