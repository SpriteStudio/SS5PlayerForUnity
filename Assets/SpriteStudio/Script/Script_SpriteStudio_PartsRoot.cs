/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[System.Serializable]
public class Script_SpriteStudio_PartsRoot : Library_SpriteStudio.PartsBase
{
	/* Constants */
	public enum BitStatus
	{
		PLAYING = 0x800000,
		PAUSING = 0x400000,
		
		STYLE_PINGPONG = 0x080000,
		STYLE_REVERSE = 0x040000,
		PLAYING_REVERSE = 0x020000,
		IGNORE_LOOP = 0x010000,

		REQUEST_PLAYEND = 0x008000,
		DECODE_USERDATA = 0x004000,

		CLEAR = 0x000000,
		MASKINITIAL = (PLAYING | PAUSING | STYLE_REVERSE | PLAYING_REVERSE | IGNORE_LOOP | REQUEST_PLAYEND | DECODE_USERDATA),
	};
	public enum PlayStyle
	{
		NO_CHANGE = -1,
		NORMAL = 0,
		PINGPONG = 1,
	}

	/* Classes */
	private class ParameterCallBackUserData
	{
		public string PartsName = null;
		public Library_SpriteStudio.AnimationData AnimationDataParts = null;
		public int FrameNo = -1;
		public Library_SpriteStudio.KeyFrame.ValueUser.Data Data = null;
		public bool FlagWayBack = false;
	}

	/* Variables & Propaties */
	public BitStatus Status;
	public bool StatusStylePigpong
	{
		get
		{
			return((0 != (Status & BitStatus.STYLE_PINGPONG)) ? true : false);
		}
		set
		{
			Status = (true == value) ? (Status | BitStatus.STYLE_PINGPONG) : (Status & ~BitStatus.STYLE_PINGPONG);
		}
	}
	public Library_SpriteStudio.AnimationData SpriteStudioData;
	public Library_SpriteStudio.AnimationInformationPlay[] ListInformationPlay;

	public float RateTimeAnimation;
	public string NameLabelStart;
	public string NameLabelEnd;
	public int OffsetFrameStart;
	public int OffsetFrameEnd;

	protected float TimeAnimation;
	protected float RateTimePlay;

	public bool FlagHideForce;

	/* CAUTION!: Don't set values from Code(Read-Only in principle). Use Function"AnimationPlay". */
	/*           "AnimationNo","CountLoopRemain" and "FrameNoStartOffset" are defined public for Setting on Inspector. */
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
			if((value != animationNo) && ((value < ListInformationPlay.Length) && ((0 <= ListInformationPlay.Length))))
			{
				AnimationStop();
				FrameNoInitial = 0;
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

	private ArrayList ListCallBackUserData;
	private Library_SpriteStudio.FunctionCallBackUserData functionUserData;
	internal Library_SpriteStudio.FunctionCallBackUserData FunctionUserData
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
	internal Library_SpriteStudio.FunctionCallBackPlayEnd FunctionPlayEnd
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
	{	/* Caution!: Public-Scope for Editor & Inspector */
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
	}

	void Start()
	{
		InstanceCameraDraw = Library_SpriteStudio.Utility.CameraGetParent(gameObject);
		InstanceDrawManagerView = Library_SpriteStudio.Utility.DrawManagerViewGetParent(gameObject);

		arrayListMeshDraw = new Library_SpriteStudio.DrawManager.ArrayListMeshDraw();
		arrayListMeshDraw.BootUp();

		ListCallBackUserData = new ArrayList();
		ListCallBackUserData.Clear();

		if(null == ListInformationPlay)
		{
			frameNoStart = 0;
			frameNoEnd = 0;
			framePerSecond = 0;
		}

		AnimationPlay();
	}

	void Update()
	{
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
			ListCallBackUserData = new ArrayList();
			ListCallBackUserData.Clear();
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
		if(null != SpriteStudioData)
		{
			if(null != ListInformationPlay)
			{
				if(0 != (Status & BitStatus.PLAYING))
				{
					int FrameCountNow = 0;
					int FrameCountEnd = frameNoEnd - frameNoStart;
					int FrameCountFull = FrameCountEnd + 1;
					int FrameCountLoop = 0;

					float RateTimeProgress = (0 == (Status & BitStatus.PLAYING_REVERSE)) ? 1.0f : -1.0f;
					float TimeAnimationFull = (float)FrameCountFull * TimeFramePerSecond;

					bool FlagLoop = false;
					bool FlagReLoop = true;
					bool FlagIgnoreLoop = false;

					if(0 == (Status & BitStatus.PAUSING))
					{
						/* FrameNo Update */
						if(-1 != frameNoPrevious)
						{	/* Not Update, Just Starting */
							TimeAnimation += (Time.deltaTime * RateTimePlay) * RateTimeProgress;
							Status &= ~BitStatus.DECODE_USERDATA;
						}
					}
					FrameCountNow = (int)(TimeAnimation / TimeFramePerSecond);
					countLoopThisTime = 0;
					flagTurnBackPingPong = false;
					flagReversePrevious = (0 != (Status & BitStatus.PLAYING_REVERSE)) ? true : false;

					if(0 == (Status & BitStatus.STYLE_PINGPONG))
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
									FlagIgnoreLoop = (0 != (Status & BitStatus.IGNORE_LOOP)) ? true : false;
									FrameCountLoop = (true == FlagIgnoreLoop) ? 0 : 1;

									/* Loop-Count Check */
									if(0 <= CountLoopRemain)
									{	/* Limited-Count Loop */
										CountLoopRemain -= FrameCountLoop;
										FlagLoop = (0 > CountLoopRemain) ? false : FlagLoop;
									}

									/* ReCalculate Frame */
									if(true == FlagLoop)
									{	/* Loop */
										countLoopThisTime += FrameCountLoop;

										TimeAnimation -= TimeAnimationFull;
										FrameCountNow = (int)(TimeAnimation / TimeFramePerSecond);
										FlagReLoop = (TimeAnimationFull <= TimeAnimation) ? true : false;
									}
									else
									{	/* End */
										TimeAnimation = ((float)FrameCountEnd) * TimeFramePerSecond;
										FrameCountNow = FrameCountEnd;
										Status |= BitStatus.REQUEST_PLAYEND;
										FlagReLoop = false;
									}
									Status &= ~BitStatus.IGNORE_LOOP;
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
									FlagIgnoreLoop = (0 != (Status & BitStatus.IGNORE_LOOP)) ? true : false;
									FrameCountLoop = (true == FlagIgnoreLoop) ? 0 : 1;

									/* Loop-Count Check */
									if(0 <= CountLoopRemain)
									{	/* Limited-Count Loop */
										CountLoopRemain -= FrameCountLoop;
										FlagLoop = (0 > CountLoopRemain) ? false : FlagLoop;
									}

									/* ReCalculate Frame */
									if(true == FlagLoop)
									{	/* Loop */
										countLoopThisTime += FrameCountLoop;

										TimeAnimation += TimeAnimationFull;
										FrameCountNow = (int)(TimeAnimation / TimeFramePerSecond);
										FlagReLoop = (0.0f > TimeAnimation) ? true : false;
										flagTurnBackPingPong = true;
									}
									else
									{	/* End */
										TimeAnimation = 0.0f;
										FrameCountNow = 0;
										Status |= BitStatus.REQUEST_PLAYEND;
										FlagReLoop = false;
									}
									Status &= ~BitStatus.IGNORE_LOOP;
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
								FlagIgnoreLoop = (0 != (Status & BitStatus.IGNORE_LOOP)) ? true : false;

								if(0 == (Status & BitStatus.PLAYING_REVERSE))
								{	/* Play foward */
									if(FrameCountEnd < FrameCountNow)
									{	/* Frame-Over */
										if(false == FlagIgnoreLoop)
										{	/* Turn-Back */
											/* Set Turn-Back */
											Status |= BitStatus.PLAYING_REVERSE;

											/* ReCalculate Frame */
											TimeAnimation -= TimeAnimationFull;
											TimeAnimation = TimeAnimationFull - TimeAnimation;
											FrameCountNow = (int)(TimeAnimation / TimeFramePerSecond);
											flagTurnBackPingPong = true;
										}
										else
										{	/* Wrap-Around */
											/* ReCalculate Frame */
											TimeAnimation -= TimeAnimationFull;
											FrameCountNow = (int)(TimeAnimation / TimeFramePerSecond);
											FlagReLoop = (TimeAnimationFull <= TimeAnimation) ? true : false;
											flagTurnBackPingPong = false;

											Status &= ~BitStatus.IGNORE_LOOP;
										}
										FlagReLoop = ((TimeAnimationFull <= TimeAnimation) || (0.0f > TimeAnimation)) ? true : false;
									}
								}
								else
								{	/* Play backwards */
									/* Loop-Count Check */
									if(0 > FrameCountNow)
									{	/* Frame-Over */
										FlagLoop = true;
										FrameCountLoop = (true == FlagIgnoreLoop) ? 0 : 1;

										if(0 <= CountLoopRemain)
										{	/* Limited-Count Loop */
											CountLoopRemain -= FrameCountLoop;
											FlagLoop = (0 > CountLoopRemain) ? false : FlagLoop;
										}

										if(true == FlagLoop)
										{	/* Loop */
											countLoopThisTime += FrameCountLoop;

											/* Set Turn-Back */
											Status &= ~BitStatus.PLAYING_REVERSE;

											/* ReCalculate Frame */
											TimeAnimation += TimeAnimationFull;
											TimeAnimation = TimeAnimationFull - TimeAnimation;
											FrameCountNow = (int)(TimeAnimation / TimeFramePerSecond);
											FlagReLoop = (0.0f < TimeAnimation) ? true : false;
											flagTurnBackPingPong = true;
										}
										else
										{	/* End */
											TimeAnimation = 0.0f;
											FrameCountNow = 0;
											Status |= BitStatus.REQUEST_PLAYEND;
											FlagReLoop = false;
										}
										FlagReLoop = ((TimeAnimationFull <= TimeAnimation) || (0.0f > TimeAnimation)) ? true : false;
										Status &= ~BitStatus.IGNORE_LOOP;
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
								FlagIgnoreLoop = (0 != (Status & BitStatus.IGNORE_LOOP)) ? true : false;

								if(0 != (Status & BitStatus.PLAYING_REVERSE))
								{	/* Play backwards */
									if(0 > FrameCountNow)
									{	/* Frame-Over */
										if(false == FlagIgnoreLoop)
										{	/* Turn-Back */
											/* Set Turn-Back */
											Status &= ~BitStatus.PLAYING_REVERSE;

											/* ReCalculate Frame */
											TimeAnimation += TimeAnimationFull;
											TimeAnimation = TimeAnimationFull - TimeAnimation;
											FrameCountNow = (int)(TimeAnimation / TimeFramePerSecond);
											FlagReLoop = (0.0f < TimeAnimation) ? true : false;
											flagTurnBackPingPong = true;
										}
										else
										{	/* Wrap-Around */
											/* ReCalculate Frame */
											TimeAnimation += TimeAnimationFull;
											FrameCountNow = (int)(TimeAnimation / TimeFramePerSecond);
											FlagReLoop = (0.0f > TimeAnimation) ? true : false;
											flagTurnBackPingPong = false;

											Status &= ~BitStatus.IGNORE_LOOP;
										}
									}
								}
								else
								{	/* Play foward */
									if(FrameCountEnd < FrameCountNow)
									{	/* Frame-Over */
										FlagLoop = true;
										FrameCountLoop = (true == FlagIgnoreLoop) ? 0 : 1;

										/* Loop-Count Check */
										if(0 <= CountLoopRemain)
										{	/* Limited-Count Loop */
											CountLoopRemain -= FrameCountLoop;
											FlagLoop = (0 > CountLoopRemain) ? false : FlagLoop;
										}

										if(true == FlagLoop)
										{	/* Loop */
											countLoopThisTime += FrameCountLoop;

											/* Set Turn-Back */
											Status |= BitStatus.PLAYING_REVERSE;

											/* ReCalculate Frame */
											TimeAnimation -= TimeAnimationFull;
											TimeAnimation = TimeAnimationFull - TimeAnimation;
											FrameCountNow = (int)(TimeAnimation / TimeFramePerSecond);
											FlagReLoop = (TimeAnimationFull <= TimeAnimation) ? true : false;
											flagTurnBackPingPong = true;
										}
										else
										{	/* End */
											TimeAnimation = ((float)FrameCountEnd) * TimeFramePerSecond;
											FrameCountNow = FrameCountEnd;
											Status |= BitStatus.REQUEST_PLAYEND;
											FlagReLoop = false;
										}
									}
									Status &= ~BitStatus.IGNORE_LOOP;
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
					SpriteStudioData.UpdateUserData(frameNoNow, gameObject, this);

					/* Update GameObject */
					SpriteStudioData.UpdateGameObject(gameObject, frameNoNow, false);
				}
			}
		}
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
				Parameter = ListCallBackUserData[i] as ParameterCallBackUserData;
				functionUserData(	transform.parent.gameObject,
									Parameter.PartsName,
									Parameter.AnimationDataParts,
									AnimationNo,
									frameNoNow,
									Parameter.FrameNo,
									Parameter.Data,
									Parameter.FlagWayBack
								);
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
		if((0 > animationNo) || (ListInformationPlay.Length <= animationNo))
		{
			return(false);
		}

		/* Set Playing-Datas */
		Status &= ~BitStatus.MASKINITIAL;
		Status |= BitStatus.PLAYING;
		Status |= BitStatus.DECODE_USERDATA;
		switch(KindStylePlay)
		{
			case PlayStyle.NO_CHANGE:
				break;
			case PlayStyle.NORMAL:
				Status &= ~BitStatus.STYLE_PINGPONG;
				break;
			case PlayStyle.PINGPONG:
				Status |= ~BitStatus.STYLE_PINGPONG;
				break;
			default:
				goto case PlayStyle.NO_CHANGE;
		}

		/* Set Animation Information */
		int FrameNo;

		Library_SpriteStudio.AnimationInformationPlay InformationAnimation = ListInformationPlay[animationNo];
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
		frameNoNow = FrameInitial;
		frameNoPrevious = -1;

		RateTime = (0.0f == RateTime) ? RateTimeAnimation : RateTime;
		RateTime *= (null != partsRootOrigin) ? partsRootOrigin.RateTimePlay : 1.0f;
		if(0.0f > RateTime)
		{
			Status = (0 == (Status & BitStatus.STYLE_REVERSE)) ? (Status | BitStatus.STYLE_REVERSE) : (Status & ~BitStatus.STYLE_REVERSE);
			RateTime *= -1.0f;
		}
		RateTimePlay = RateTime;

		Status |= (0 != (Status & BitStatus.STYLE_REVERSE)) ? BitStatus.PLAYING_REVERSE : 0;
		if(0 != (Status & BitStatus.PLAYING_REVERSE))
		{	/* Play-Reverse & Start Top-Frame */
			Status |= (frameNoNow <= frameNoStart) ? BitStatus.IGNORE_LOOP : 0;
		}
		else
		{	/* Play-Normal & Start End-Frame */
			Status |= (frameNoNow >= frameNoEnd) ? BitStatus.IGNORE_LOOP : 0;
		}
		TimeAnimation = FrameInitial * TimeFramePerSecond;

		if(-1 != TimesPlay)
		{
			/* MEMO: TimesPlay is Invalid, Force Play-Once */
			CountLoopRemain = (0 > TimesPlay) ? 0 : (TimesPlay - 1);
		}

		/* UserData-CallBack Buffer Create */
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
		(None)
	*/
	public Material MaterialGet(int TextureNo, Library_SpriteStudio.KindColorOperation Operation)
	{
		int MaterialNo = TextureNo * ((int)Library_SpriteStudio.KindColorOperation.TERMINATOR - 1);
		return(((null != TableMaterial) && (0 <= TextureNo)) ? TableMaterial[MaterialNo] : null);
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
			ListCallBackUserData = new ArrayList();
			ListCallBackUserData.Clear();
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
	internal void TimeElapsedSetForce(float TimeElapsed)
	{
		int FrameCount = (frameNoEnd - frameNoStart) + 1;
		float TimeRange = (float)FrameCount * TimeFramePerSecond;
		TimeAnimation = (TimeElapsed * RateTimePlay) % TimeRange;
		Status |= BitStatus.IGNORE_LOOP;
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
	}
}