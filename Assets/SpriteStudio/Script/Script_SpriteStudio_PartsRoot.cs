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
		REQUEST_PLAYEND = 0x200000,

		DECODE_USERDATA = 0x080000,

		CLEAR = 0x000000,
	};

	/* Classes */
	private class ParameterCallBackUserData
	{
		public string PartsName = null;
		public Library_SpriteStudio.AnimationData AnimationDataParts = null;
		public int FrameNo = -1;
		public Library_SpriteStudio.KeyFrame.ValueUser Data = null;
	}

	/* Variables & Propaties */
	public BitStatus Status;
	public Library_SpriteStudio.AnimationData SpriteStudioData;
	public Library_SpriteStudio.AnimationInformationPlay[] ListInformationPlay;
	public float RateTimeAnimation;

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
			if((0 <= value) && ((frameNoEnd - frameNoStart) > value))
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

	protected float TimeAnimation;

	private ArrayList ListCallBackUserData;
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
	public Library_SpriteStudio.DrawManager.ArrayListMeshDraw ArrayListMeshDraw
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
	private Script_SpriteStudio_PartsRoot partsRootMother = null;
	public Script_SpriteStudio_PartsRoot PartsRootMother
	{
		get
		{
			return(partsRootMother);
		}
		set
		{
			partsRootMother = value;
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

		int	PlayTimes = CountLoopRemain + 1;
		AnimationPlay(animationNo, PlayTimes, frameNoInitial, RateTimeAnimation);
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
		if((null != InstanceDrawManagerView) && (null == partsRootMother))
		{	/* Not "Instance"-Parts */
			InstanceDrawManagerView.DrawEntryObject(this);
		}

		/* Animation Update */
		if(null != SpriteStudioData)
		{
			if(null != ListInformationPlay)
			{
				if(0 != (Status & BitStatus.PLAYING))
				{
					if(0 == (Status & BitStatus.PAUSING))
					{
						/* Get Frame Count */
						if(-1 != frameNoPrevious)
						{	/* Not Update, Just Starting */
							TimeAnimation += (Time.deltaTime * RateTimeAnimation);
							Status &= ~BitStatus.DECODE_USERDATA;
						}
						int FrameCountNow = (int)(TimeAnimation / TimeFramePerSecond);
						int FrameCountEnd = frameNoEnd - frameNoStart;

						if(0.0f <= RateTimeAnimation)
						{	/* Play normaly */
							if(FrameCountEnd < FrameCountNow)
							{	/* Frame-Over */
								/* Loop-Count Check */
								bool FlagLoop = true;
								if(0 <= CountLoopRemain)
								{	/* Limited-Count Loop */
									CountLoopRemain--;
									if(0 > CountLoopRemain)
									{
										FlagLoop = false;
									}
								}

								/* ReCalculate Frame */
								if(true == FlagLoop)
								{	/* Loop */
									int FrameCountFull = FrameCountEnd + 1;
									TimeAnimation -= ((float)FrameCountFull * TimeFramePerSecond);
									FrameCountNow = (int)(TimeAnimation / TimeFramePerSecond);
								}
								else
								{	/* End */
									TimeAnimation = ((float)FrameCountEnd) * TimeFramePerSecond;
									FrameCountNow = FrameCountEnd;
									Status |= BitStatus.REQUEST_PLAYEND;
								}
							}

							/* Frame-No Set */
							int FrameNoNew = frameNoStart + FrameCountNow;
							if(FrameNoNew != frameNoNow)
							{
								Status |= BitStatus.DECODE_USERDATA;
							}
							frameNoPrevious = frameNoNow;
							frameNoNow = FrameNoNew;
						}
						else
						{	/* Play backwards */
							if(0 > FrameCountNow)
							{	/* Frame-Over */
								/* Loop-Count Check */
								bool FlagLoop = true;
								if(0 <= CountLoopRemain)
								{	/* Limited-Count Loop */
									CountLoopRemain--;
									if(0 > CountLoopRemain)
									{
										FlagLoop = false;
									}
								}

								/* ReCalculate Frame */
								if(true == FlagLoop)
								{	/* Loop */
									int FrameCountFull = FrameCountEnd + 1;
									TimeAnimation += ((float)FrameCountFull * TimeFramePerSecond);
									FrameCountNow = (int)(TimeAnimation / TimeFramePerSecond);
								}
								else
								{	/* End */
									TimeAnimation = 0.0f;
									FrameCountNow = 0;
									Status |= BitStatus.REQUEST_PLAYEND;
								}
							}

							/* Frame-No Set */
							int FrameNoNew = frameNoStart + FrameCountNow;
							if(FrameNoNew != frameNoNow)
							{
								Status |= BitStatus.DECODE_USERDATA;
							}
							frameNoPrevious = frameNoNow;
							frameNoNow = FrameNoNew;
						}

						/* Update User-CallBack */
						SpriteStudioData.UpdateUserData(frameNoNow, gameObject, this);

						/* Update GameObject */
						SpriteStudioData.UpdateGameObject(gameObject, frameNoNow);
					}
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
									Parameter.Data
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
				if(false == (functionPlayEnd(transform.parent.gameObject)))
				{
					Destroy(transform.parent.gameObject);
				}
			}
		}
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
		Animation's Index<br>
		-1 == Now-Setting Index is not changed
	@param	PlayTimes
		0 == Infinite-looping <br>
		1 == Not looping <br>
		2 <= Number of Plays
	@param	StartFrameNo
		Offset frame-number of starting Play in animation (0 origins). <br>
		-1 == use "FrameNoInitial" Value<br>
		default: -1
	@param	RateTime
		Coefficient of time-passage of animation.<br>
		Minus Value is given, Animation is played backwards.<br>
		0.0f is given, the now-setting is not changed) <br>
		default: 0.0f (Setting is not changed)
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
	public bool AnimationPlay(int No, int PlayTimes, int StartFrameNo=-1, float RateTime=0.0f)
	{
		/* Error-Check */
		if(-1 != No)
		{
			animationNo = No;	/* Don't Use "AnimationNo" (occur "Stack-Overflow") */
		}
		if((0 > animationNo) || (ListInformationPlay.Length <= animationNo))
		{
			return(false);
		}

		/* Set Playing-Datas */
		Status &= ~(BitStatus.PAUSING | BitStatus.REQUEST_PLAYEND);
		Status |= BitStatus.PLAYING;
		Status |= BitStatus.DECODE_USERDATA;

		/* Set Animation Information */
		frameNoStart = ListInformationPlay[animationNo].FrameStart;
		frameNoEnd = ListInformationPlay[animationNo].FrameEnd;
		framePerSecond = ListInformationPlay[animationNo].FramePerSecond;

		int CountFrame = (frameNoEnd - frameNoStart) + 1;
		if(-1 == StartFrameNo)
		{	/* Use "FrameNoInitial" */
			StartFrameNo = ((0 <= FrameNoInitial) && (CountFrame > FrameNoInitial)) ? FrameNoInitial : 0;
		}
		else
		{	/* Direct-Frame */
			StartFrameNo = ((0 <= StartFrameNo) && (CountFrame > StartFrameNo)) ? StartFrameNo : 0;
		}
		frameNoNow = frameNoStart + StartFrameNo;	/* Set Status */
		frameNoPrevious = -1;
		RateTimeAnimation = (0.0f == RateTime) ? RateTimeAnimation : RateTime;
		TimeAnimation = StartFrameNo * TimeFramePerSecond;

		int	CountLoop = PlayTimes - 1;
		if((-1 == CountLoop) || (0 < CountLoop))
		{	/* Value-Valid (-1 == Infinite-Loop) */
			CountLoopRemain = CountLoop;
		}
		else
		{	/* Play-once or Value-Invalid */
			CountLoopRemain = 0;
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
		SpriteStudioData = new Library_SpriteStudio.AnimationData();
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
	public void CallBackExecUserData(string PartsName, Library_SpriteStudio.AnimationData AnimationDataParts, int FrameNoData, Library_SpriteStudio.KeyFrame.ValueUser Data)
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
		Parameter.Data = Data;
		ListCallBackUserData.Add(Parameter);
	}
}