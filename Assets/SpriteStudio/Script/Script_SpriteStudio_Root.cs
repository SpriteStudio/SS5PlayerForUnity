/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;

[ExecuteInEditMode]
[System.Serializable]
public partial class Script_SpriteStudio_Root : Library_SpriteStudio.Script.Root
{
	public enum PlayStyle
	{
		NO_CHANGE = -1,
		NORMAL = 0,
		PINGPONG = 1,
	}

	[System.Flags]
	private enum FlagBitStatus
	{
		VALID = 0x40000000,
		PLAYING = 0x20000000,
		PAUSING = 0x10000000,

		STYLE_PINGPONG = 0x08000000,
		STYLE_REVERSE = 0x04000000,

		PLAYING_START = 0x00800000,
		PLAYING_REVERSE = 0x00400000,
		PLAYING_REVERSEPREVIOUS = 0x00200000,
		PLAYING_TURN = 0x00100000,
		PLAYING_FIRSTUPDATE = 0x00080000,
		PLAYING_INFINITY = 0x00040000,	/* Reserved */

		DECODE_USERDATA = 0x00008000,
		DECODE_INSTANCE = 0x00004000,
		DECODE_EFFECT = 0x00002000,

		REQUEST_DESTROY = 0x00000800,
		REQUEST_PLAYEND = 0x00000400,

		CELL_TABLECHANGED = 0x00000080,

		CLEAR = 0x00000000,
	}

	/* Base-Datas */
	public Script_SpriteStudio_DataAnimation DataAnimation;

	/* Control Parts'-Data (& Drawing Parts' Mesh) */
	public Library_SpriteStudio.Control.Parts[] ListControlParts;

	/* Playing Datas */
	public bool FlagAnimationStopInitial;	/* Initial: false */
	public bool FlagPingpong;				/* Initial: false */
	public float RateSpeedInitial;			/* Initial: 1.0f */

	public int IndexAnimation;				/* Initial: 0 */
	public int TimesPlay;					/* Initial: 0 */
	public int IndexLabelStart;				/* Initial: -1 */
	public int FrameOffsetStart;			/* Initial: 0 */
	public int IndexLabelEnd;				/* Initial: -1 */
	public int FrameOffsetEnd;				/* Initial: 0 */
	public int FrameOffsetInitial;			/* Initial: 0 */
//	public float RateSpeed;					/* Initial: 1.0f */

	/* Playing Datas: for Runtime (WorkArea) */
	private FlagBitStatus Status = FlagBitStatus.CLEAR;
	public bool StatusIsValid
	{
		get
		{
			return(0 != (Status & FlagBitStatus.VALID));
		}
	}
	public bool StatusIsPlaying
	{
		get
		{
			return(0 != (Status & FlagBitStatus.PLAYING));
		}
	}
	public bool StatusIsPausing
	{
		get
		{
			return(0 != (Status & FlagBitStatus.PAUSING));
		}
	}
	public bool StatusIsPlayStylePingpong
	{
		get
		{
			return(0 != (Status & FlagBitStatus.STYLE_PINGPONG));
		}
	}
	public bool StatusIsPlayStyleReverse
	{
		get
		{
			return(0 != (Status & FlagBitStatus.STYLE_REVERSE));
		}
	}
	internal bool StatusIsPlayingStart
	{
		get
		{
			return(0 != (Status & FlagBitStatus.PLAYING_START));
		}
	}
	internal bool StatusIsPlayingReverse
	{
		get
		{
			return(0 != (Status & FlagBitStatus.PLAYING_REVERSE));
		}
	}
	internal bool StatusIsPlayingReversePrevious
	{
		get
		{
			return(0 != (Status & FlagBitStatus.PLAYING_REVERSEPREVIOUS));
		}
	}
	internal bool StatusIsPlayingTurn
	{
		get
		{
			return(0 != (Status & FlagBitStatus.PLAYING_TURN));
		}
	}
	internal bool StatusIsPlayingFirstUpdate
	{
		get
		{
			return(0 != (Status & FlagBitStatus.PLAYING_FIRSTUPDATE));
		}
	}
	internal bool StatusIsPlayingInfinity
	{
		get
		{
			return(0 != (Status & FlagBitStatus.PLAYING_INFINITY));
		}
//		set
//		{
//			Status = (true == value) ? (Status | FlagBitStatus.PLAYING_INFINITY) : (Status & ~FlagBitStatus.PLAYING_INFINITY);
//		}
	}
	internal bool StatusIsDecodeUserData
	{
		get
		{
			return(0 != (Status & FlagBitStatus.DECODE_USERDATA));
		}
	}
	internal bool StatusIsDecodeInstance
	{
		get
		{
			return(0 != (Status & FlagBitStatus.DECODE_INSTANCE));
		}
	}
	internal bool StatusIsDecodeEffect
	{
		get
		{
			return(0 != (Status & FlagBitStatus.DECODE_EFFECT));
		}
	}
	internal bool StatusIsRequestDestroy
	{
		get
		{
			return(0 != (Status & FlagBitStatus.REQUEST_DESTROY));
		}
	}
	internal bool StatusIsRequestPlayEnd
	{
		get
		{
			return(0 != (Status & FlagBitStatus.REQUEST_PLAYEND));
		}
	}
	internal bool StatusIsCellTableChanged
	{
		get
		{
			return(0 != (Status & FlagBitStatus.CELL_TABLECHANGED));
		}
	}

	internal int FrameNoStart = 0;
	internal int FrameNoEnd = 0;
	internal int FrameNoNow = 0;
	internal int FrameNoPrevious = -1;
	internal int CountLoop = 0;
	internal int CountLoopNow = 0;
	internal int TimesPlayNow = 0;
//	internal float TimePerFrame = 1.0f;
//	internal float TimeElapsed = 0.0f;
//	internal float RateSpeed = 1.0f;
//	internal float RateOpacity = 1.0f;
//	internal float TimeDelay = 0.0f;
	internal int FrameRange = 0;
	internal float TimeRange = 0.0f;
	internal float TimePerFrameConsideredRateSpeed = 0.0f;

	/* CAUTION!: Don't use "ColorBlendOverwrite"-Property (for Player's Internal-Processing) */
	private Library_SpriteStudio.Control.ColorBlendOverwrite dataColorBlendOverwrite = null;
	internal Library_SpriteStudio.Control.ColorBlendOverwrite DataColorBlendOverwrite
	{
		get
		{
			return(dataColorBlendOverwrite);
		}
	}

	/* Relation Datas */
	internal GameObject InstanceGameObjectControl = null;

	/* CallBack-s */
	internal Library_SpriteStudio.FunctionCallBackPlayEnd FunctionPlayEnd = null;
	internal Library_SpriteStudio.FunctionCallBackUserData FunctionUserData = null;
	internal Library_SpriteStudio.FunctionCallBackCollider FunctionColliderEnter = null;
	internal Library_SpriteStudio.FunctionCallBackCollider FunctionColliderExit = null;
	internal Library_SpriteStudio.FunctionCallBackCollider FunctionColliderStay = null;
	internal Library_SpriteStudio.FunctionCallBackCollision FunctionCollisionEnter = null;
	internal Library_SpriteStudio.FunctionCallBackCollision FunctionCollisionExit = null;
	internal Library_SpriteStudio.FunctionCallBackCollision FunctionCollisionStay = null;

	void Awake()
	{
		foreach(Material MaterialNow in TableMaterial)
		{
			MaterialNow.shader = Shader.Find(MaterialNow.shader.name);
		}
	}

	void Start()
	{
		/* Base Start */
		CountPartsDraw = DataAnimation.CountGetPartsDraw();
		StartBase(CountPartsDraw);

		/* Parts-WorkArea Start */
		int CountParts = DataAnimation.CountGetParts();
		if(null == ListControlParts)
		{
			GameObject[] ListGameObject = new GameObject[CountParts];
			ListGameObject[0] = gameObject;
			for(int i=1; i<CountParts; i++)
			{
				ListGameObject[i] = (transform.Find(DataAnimation.ListDataParts[i].Name)).gameObject;
			}

			for(int i=0; i<CountParts; i++)
			{
				ListControlParts[i] = new Library_SpriteStudio.Control.Parts();
				ListControlParts[i].BootUp(this, i, ListGameObject[i]);
			}
		}
		for(int i=0; i<CountParts; i++)
		{
			ListControlParts[i].BootUpRuntime(this, i);
		}

		/* Status Set */
		Status |= FlagBitStatus.VALID;

		/* Play Animation Initialize */
		AnimationPlay();
		if(true == FlagAnimationStopInitial)
		{
			AnimationStop();
		}
	}

//	void Update()
//	{
//	}

	void LateUpdate()
	{
		int CountParts = DataAnimation.CountGetParts();
		if(0 == (Status & FlagBitStatus.VALID))
		{	/* Not Start */
			return;
		}

		/* DrawParts-Cluster Create */
		LateUpdateBase();

		/* DrawManager Get */
		if((null == InstanceManagerDraw) && (null == InstanceRootParent))
		{	/* MEMO: "Instance" and "Effect" cannot have Manager-Draw. */
			InstanceManagerDraw = Library_SpriteStudio.Utility.Parts.ManagerDrawGetParent(gameObject);
		}

		/* Playing-Frame Update (Common) */
		AnimationUpdate();

		/* Parts Update */
		ChainClusterDrawParts.ChainCleanUp();
		for(int i=0; i<CountParts; i++)
		{
			/* Transform/Collider Update */
			ListControlParts[i].UpdateGameObject(this, FrameNoNow);

			/* Draw-Parts */
			switch(ListControlParts[i].DataParts.Kind)
			{
				case Library_SpriteStudio.KindParts.NORMAL_TRIANGLE2:
				case Library_SpriteStudio.KindParts.NORMAL_TRIANGLE4:
					/* Mesh Data Update */
					if(null != ListControlParts[i].BufferParameterMesh)
					{
						ListControlParts[i].UpdateMesh(this, FrameNoNow);
					}
					break;

				case Library_SpriteStudio.KindParts.INSTANCE:
					/* Instance Update */
					ListControlParts[i].UpdateInstance(this, FrameNoNow);
					break;

				case Library_SpriteStudio.KindParts.EFFECT:
					/* Effect Update */
					ListControlParts[i].UpdateEffect(this, FrameNoNow);
					break;

				default:
					break;
			}

			/* Exec CallBack (User-Data) */
			if(0 != (Status & FlagBitStatus.PLAYING))
			{
				if((0 != (Status & FlagBitStatus.DECODE_USERDATA)) && (null != FunctionUserData))
				{
					ListControlParts[i].UpdateUserData(this, FrameNoNow);
				}
			}
		}

		Status &= ~(FlagBitStatus.DECODE_USERDATA | FlagBitStatus.DECODE_INSTANCE | FlagBitStatus.DECODE_EFFECT);
		Status &= ~FlagBitStatus.PLAYING_START;
		Status &= ~FlagBitStatus.CELL_TABLECHANGED;

		/* Exec CallBack (Play-End) */
		if(0 != (Status & FlagBitStatus.REQUEST_PLAYEND))
		{
			Status &= ~(FlagBitStatus.PAUSING | FlagBitStatus.PLAYING);
			if(null != FunctionPlayEnd)
			{
				if(false == FunctionPlayEnd(this, InstanceGameObjectControl))
				{	/* Destroy (Request) */
					/* MEMO: Destroy at end of "LateUpdate" */
					Status |= FlagBitStatus.REQUEST_DESTROY;
				}
			}
			Status &= ~FlagBitStatus.REQUEST_PLAYEND;
		}

		/* Set to DrawManager */
		if(((null != InstanceManagerDraw) && (null != DrawObject)) && (false == FlagHideForce))
		{
			/* Set To Draw-Manager */
			InstanceManagerDraw.DrawSet(this);
		}

		/* Checking Destroy-Request */
		if(0 != (Status & FlagBitStatus.REQUEST_DESTROY))
		{
			if(null != InstanceGameObjectControl)
			{
				Object.Destroy(InstanceGameObjectControl);
			}
			else
			{
				Object.Destroy(gameObject);
			}
		}
	}
	private void AnimationUpdate()
	{
		float TimeDelta = Time.deltaTime * RateSpeed;

		CountLoopNow = 0;
		Status &= ~FlagBitStatus.PLAYING_TURN;
		Status = (0 != (Status & FlagBitStatus.PLAYING_REVERSE)) ? (Status | FlagBitStatus.PLAYING_REVERSEPREVIOUS) : (Status & ~FlagBitStatus.PLAYING_REVERSEPREVIOUS);
		if(0 == (Status & FlagBitStatus.PLAYING))
		{	/* Not-Playing */
			return;
		}

		/* Determine Animation start */
		if((0 != (Status & FlagBitStatus.PAUSING)) && (0 == (Status & FlagBitStatus.PLAYING_START)))
		{	/* Play & Pausing (Through, Right-After-Starting) */
			return;
		}
		if(0.0f > TimeDelay)
		{	/* Wait Infinite */
			TimeDelay = -1.0f;
			Status &= ~FlagBitStatus.PLAYING_START;	/* Start Cancel */
			Status &= ~(FlagBitStatus.DECODE_USERDATA | FlagBitStatus.DECODE_INSTANCE | FlagBitStatus.DECODE_EFFECT);
			return;
		}
		else
		{	/* Wait Limited-Time */
			if(0.0f < TimeDelay)
			{	/* Waiting */
				TimeDelay -= TimeDelta;
				if(0.0f < TimeDelay)
				{
					Status &= ~FlagBitStatus.PLAYING_START;	/* Start Cancel */
					Status &= ~(FlagBitStatus.DECODE_USERDATA | FlagBitStatus.DECODE_INSTANCE | FlagBitStatus.DECODE_EFFECT);
					return;
				}

				/* Start */
				TimeElapsed += -TimeDelay * ((0 == (Status & FlagBitStatus.PLAYING_REVERSE)) ? 1.0f : -1.0f);
				TimeDelay = 0.0f;
				FrameNoPrevious = -1;
				Status |= FlagBitStatus.PLAYING_START;
				Status |= (FlagBitStatus.DECODE_USERDATA | FlagBitStatus.DECODE_INSTANCE | FlagBitStatus.DECODE_EFFECT);
			}
		}
		if(0 != (Status & FlagBitStatus.PLAYING_START))
		{	/* Play & Right-After-Starting */
			Status |= (FlagBitStatus.DECODE_USERDATA | FlagBitStatus.DECODE_INSTANCE | FlagBitStatus.DECODE_EFFECT);
			TimeDelta = 0.0f;
			goto AnimationUpdate_End;	/* Display the first frame, force */
		}

		/* Calculate New-Frame */
		FrameNoPrevious = FrameNoNow;
		bool FlagRangeOverPrevious = false;
		if(0 != (Status & FlagBitStatus.STYLE_PINGPONG))
		{	/* Play-Style: PingPong */
			if(0 != (Status & FlagBitStatus.PLAYING_REVERSE))
			{
				FlagRangeOverPrevious = (0.0f > TimeElapsed) ? true : false;
				TimeElapsed -= TimeDelta;
				if((0.0f > TimeElapsed) && (true == FlagRangeOverPrevious))
				{	/* Still Range-Over */
					goto AnimationUpdate_End;
				}
			}
			else
			{
				FlagRangeOverPrevious = (TimeRange <= TimeElapsed) ? true : false;
				TimeElapsed += TimeDelta;
				if((TimeRange <= TimeElapsed) && (true == FlagRangeOverPrevious))
				{	/* Still Range-Over */
					goto AnimationUpdate_End;
				}
			}

			if(0 != (Status & FlagBitStatus.STYLE_REVERSE))
			{	/* Play-Style: PingPong & Reverse */
				while((TimeRange <= TimeElapsed) || (0.0f > TimeElapsed))
				{
					if(0 != (Status & FlagBitStatus.PLAYING_REVERSE))
					{	/* Now: Reverse */
						if(TimeRange <= TimeElapsed)
						{	/* MEMO: Follow "FlagRangeOverPrevious" */
							break;
						}
						if(0.0f > TimeElapsed)
						{	/* Frame-Over: Turn */
							TimeElapsed += TimeRange;
							TimeElapsed = TimeRange - TimeElapsed;
							Status |= FlagBitStatus.PLAYING_TURN;
							Status &= ~FlagBitStatus.PLAYING_REVERSE;
						}
					}
					else
					{   /* Now: Foward */
						if(true == FlagRangeOverPrevious)
						{
							FlagRangeOverPrevious = false;
							Status |= FlagBitStatus.PLAYING_TURN;
							break;
						}
						else
						{
							CountLoop++;
							if(TimeRange <= TimeElapsed)
							{	/* Frame-Over: Loop/End */
								if(0 < TimesPlayNow)
								{	/* Limited-Count Loop */
									TimesPlayNow--;
									if(0 >= TimesPlayNow)
									{	/* End */
										goto AnimationUpdate_PlayEnd_Foward;
									}
								}

								/* Not-End */
								TimeElapsed -= TimeRange;
								TimeElapsed = TimeRange - TimeElapsed;
								Status |= FlagBitStatus.PLAYING_REVERSE;
								Status |= FlagBitStatus.PLAYING_TURN;
								CountLoopNow++;
							}
						}
					}
				}
			}
			else
			{	/* Play-Style: PingPong & Foward */
				while((TimeRange <= TimeElapsed) || (0.0f > TimeElapsed))
				{
					if(0 != (Status & FlagBitStatus.PLAYING_REVERSE))
					{	/* Now: Reverse */
						if(true == FlagRangeOverPrevious)
						{
							FlagRangeOverPrevious = false;
							Status |= FlagBitStatus.PLAYING_TURN;
							break;
						}
						else
						{
							CountLoop++;
							if(0.0f > TimeElapsed)
							{	/* Frame-Over: Loop/End */
								if(0 < TimesPlayNow)
								{	/* Limited-Count Loop */
									TimesPlayNow--;
									if(0 >= TimesPlayNow)
									{	/* End */
										goto AnimationUpdate_PlayEnd_Reverse;
									}
								}

								/* Not-End */
								TimeElapsed += TimeRange;
								TimeElapsed = TimeRange - TimeElapsed;
								Status &= ~FlagBitStatus.PLAYING_REVERSE;
								Status |= FlagBitStatus.PLAYING_TURN;
								CountLoopNow++;
							}
						}
					}
					else
					{	/* Now: Foward */
						if(0.0f > TimeElapsed)
						{	/* MEMO: Follow "FlagRangeOverPrevious" */
							break;
						}
						if(TimeRange <= TimeElapsed)
						{	/* Frame-Over: Turn */
							TimeElapsed -= TimeRange;
							TimeElapsed = TimeRange - TimeElapsed;
							Status |= FlagBitStatus.PLAYING_TURN;
							Status |= FlagBitStatus.PLAYING_REVERSE;
						}
					}
				}
			}
		}
		else
		{	/* Play-Style: OneWay */
			if(0 != (Status & FlagBitStatus.STYLE_REVERSE))
			{	/* Play-Style: OneWay & Reverse */
				FlagRangeOverPrevious = (0.0f > TimeElapsed) ? true : false;
				TimeElapsed -= TimeDelta;
				if((0.0f > TimeElapsed) && (true == FlagRangeOverPrevious))
				{	/* Still Range-Over */
					goto AnimationUpdate_End;
				}

				while(0.0f > TimeElapsed)
				{
					TimeElapsed += TimeRange;
					if(true == FlagRangeOverPrevious)
					{
						FlagRangeOverPrevious = false;
						Status |= FlagBitStatus.PLAYING_TURN;
					}
					else
					{
						CountLoop++;
						if(0 < TimesPlayNow)
						{	/* Limited-Count Loop */
							TimesPlayNow--;
							if(0 >= TimesPlayNow)
							{	/* End */
								goto AnimationUpdate_PlayEnd_Reverse;
							}
						}

						/* Not-End */
						CountLoopNow++;
						Status |= FlagBitStatus.PLAYING_TURN;
					}
				}
			}
			else
			{	/* Play-Style: OneWay & Foward */
				FlagRangeOverPrevious = (TimeRange <= TimeElapsed) ? true : false;
				TimeElapsed += TimeDelta;
				if((TimeRange <= TimeElapsed) && (true == FlagRangeOverPrevious))
				{	/* Still Range-Over */
					goto AnimationUpdate_End;
				}

				while(TimeRange <= TimeElapsed)
				{
					TimeElapsed -= TimeRange;
					if(true == FlagRangeOverPrevious)
					{
						FlagRangeOverPrevious = false;
						Status |= FlagBitStatus.PLAYING_TURN;
					}
					else
					{
						CountLoop++;
						if(0 < TimesPlayNow)
						{	/* Limited-Count Loop */
							TimesPlayNow--;
							if(0 >= TimesPlayNow)
							{	/* End */
								goto AnimationUpdate_PlayEnd_Foward;
							}
						}

						/* Not-End */
						Status |= FlagBitStatus.PLAYING_TURN;
						CountLoopNow++;
					}
				}
			}
		}

	AnimationUpdate_End:;
		/* Update Playing-Datas */
		FrameNoNow = (int)(TimeElapsed / TimePerFrame);
		FrameNoNow = Mathf.Clamp(FrameNoNow, 0, (FrameRange - 1));
		FrameNoNow += FrameNoStart;
		Status |= ((FrameNoNow != FrameNoPrevious) || (0 != (Status & FlagBitStatus.PLAYING_TURN)))
					? (FlagBitStatus.DECODE_USERDATA | FlagBitStatus.DECODE_INSTANCE | FlagBitStatus.DECODE_EFFECT)
					: FlagBitStatus.CLEAR;
		return;

	AnimationUpdate_PlayEnd_Foward:;
		TimesPlayNow = 0;	/* Clip */
		Status |= FlagBitStatus.REQUEST_PLAYEND;
		Status |= (FlagBitStatus.DECODE_USERDATA | FlagBitStatus.DECODE_INSTANCE | FlagBitStatus.DECODE_EFFECT);
		TimeElapsed = TimeRange;
		FrameNoNow = FrameNoEnd;
		return;

	AnimationUpdate_PlayEnd_Reverse:;
		TimesPlayNow = 0;	/* Clip */
		Status |= FlagBitStatus.REQUEST_PLAYEND;
		Status |= (FlagBitStatus.DECODE_USERDATA | FlagBitStatus.DECODE_INSTANCE | FlagBitStatus.DECODE_EFFECT);
		TimeElapsed = 0.0f;
		FrameNoNow = FrameNoStart;
		return;
	}
	internal void TimeElapsedSetForce(float TimeElapsedForce, bool FlagReverseParent, bool FlagRangeEnd)
	{   /* MEMO: In principle, This Function is for calling from "UpdateInstance". */
		if(0.0f > TimeElapsedForce)
		{	/* Wait Infinity */
			TimeDelay = -1.0f;
			return;
		}

		bool FlagPongPong = (0 != (Status & FlagBitStatus.STYLE_PINGPONG)) ? true : false;
		bool FlagReverseStyle = (0 != (Status & FlagBitStatus.STYLE_REVERSE)) ? true : false;
		float TimeLoop = TimeRange * ((true == FlagPongPong) ? 2.0f : 1.0f);
		float TimeCursor = TimeElapsedForce;
		int CountLoop = 0;

		/* Loop-Count Get */
		while(TimeLoop <= TimeCursor)
		{
			TimeCursor -= TimeLoop;
			CountLoop++;
		}

		/* Solving Play-Count */
		if(0 >= TimesPlayNow)
		{	/* Infinite-Loop */
			/* MEMO: "TimesPlayNow" does not change. */
			CountLoop = 0;
			TimeDelay = 0.0f;
		}
		else
		{	/* Limited-Loop */
			if(0 >= CountLoop)
			{	/* No-Wrap-Around */
				/* MEMO: "TimesPlayNow" does not change. */
				CountLoop = 0;
				TimeDelay = 0.0f;
			}
			else
			{	/* Wrap-Around */
				if(TimesPlayNow <= CountLoop)
				{	/* Over */
					if(true == FlagReverseParent)
					{	/* Reverse ... Play-Delay */
						/* MEMO: "TimesPlayNow" does not change. */
						TimeDelay = ((float)(CountLoop - TimesPlayNow) * TimeLoop) + TimeCursor;
						TimeCursor = TimeLoop;
					}
					else
					{	/* Foward ... Play-End */
						TimeDelay = 0.0f;
						TimesPlayNow = 0;

						if(true == FlagPongPong)
						{	/* Play-Style: PingPong */
							TimeCursor = (0 != (Status & FlagBitStatus.STYLE_REVERSE)) ? TimeRange : 0.0f;
						}
						else
						{	/* Play-Style: OneWay */
							TimeCursor = (0 != (Status & FlagBitStatus.STYLE_REVERSE)) ? 0.0f : TimeRange;
						}
						AnimationStop();
					}
				}
				else
				{   /* In-Range */
					TimesPlayNow -= CountLoop;
				}
			}
		}

		/* Time Adjust */
		if(true == FlagPongPong)
		{	/* Play-Style: PingPong */
			Status &= ~FlagBitStatus.PLAYING_REVERSE;
			if(true == FlagReverseStyle)
			{	/* Play-Stype: PingPong & Reverse */
				if(TimeRange <= TimeCursor)
				{	/* Start: Foward */
					TimeCursor -= TimeRange;
//					Status &= ~FlagBitStatus.PLAYING_REVERSE;
				}
				else
				{	/* Start: Reverse */
					Status |= FlagBitStatus.PLAYING_REVERSE;
				}
			}
			else
			{	/* Play-Style: PingPong & Foward */
				if(TimeRange <= TimeCursor)
				{	/* Start: Reverse */
					TimeCursor -= TimeRange;
					TimeCursor = TimeRange - TimeCursor;
					Status |= FlagBitStatus.PLAYING_REVERSE;
				}
				else
				{	/* Start: Foward */
//					Status &= ~FlagBitStatus.PLAYING_REVERSE;
				}
			}
		}
		else
		{	/* Play-Style: One-Way */
			Status &= ~FlagBitStatus.PLAYING_REVERSE;
			if(true == FlagReverseStyle)
			{	/* Play-Stype: One-Way & Reverse */
				Status |= FlagBitStatus.PLAYING_REVERSE;
			}
			else
			{	/* Play-Stype: One-Way & Foward */
//				Status &= ~FlagBitStatus.PLAYING_REVERSE;
			}
			TimeCursor = (true == FlagRangeEnd) ? (TimeRange - TimeCursor) : TimeCursor;
		}

		TimeElapsed = TimeCursor;
		FrameNoNow = (int)(TimeElapsed / TimePerFrame);
		FrameNoNow = Mathf.Clamp(FrameNoNow, 0, (FrameRange - 1));
		FrameNoNow += FrameNoStart;
	}


	/* ******************************************************** */
	//! Child-GameObjects Build-Up
	/*!
	Don't use this function. <br>
	(This function is for Importer.)
	*/
	public GameObject[] GameObjectBuildUp(bool FlagColliderRigidBody, float ColliderThicknessZ)
	{
		return(Library_SpriteStudio.Miscellaneousness.Asset.GameObjectBuildUpRoot(this, FlagColliderRigidBody, ColliderThicknessZ));
	}

	/* ******************************************************** */
	//! Control-Parts Build-Up
	/*!
	Don't use this function. <br>
	(This function is for Importer.)
	*/
	public bool ControlPartsBuildUp(GameObject[] ListGameObject)
	{
		int Count = DataAnimation.CountGetParts();
		if(null == ListControlParts)
		{
			ListControlParts = new Library_SpriteStudio.Control.Parts[Count];
		}

		Library_SpriteStudio.Control.Parts ControlParts = null;
		for(int i=0; i<Count; i++)
		{
			ControlParts = ListControlParts[i];
			if(null == ControlParts)
			{
				ControlParts = new Library_SpriteStudio.Control.Parts();
				ControlParts.CleanUp();
				ListControlParts[i] = ControlParts;
			}
			ControlParts.BootUp(this, i, ListGameObject[i]);
		}
		return(true);
	}

	/* ******************************************************** */
	//! Set Animation-Data to Control-Parts
	/*!
	Don't use this function. <br>
	(This function is for Internal-Processing)
	*/
	public bool ControlSetAnimation()
	{
		int Count = ListControlParts.Length;
		Library_SpriteStudio.Control.Parts ControlParts = null;
		for(int i=0; i<Count; i++)
		{
			ControlParts = ListControlParts[i];
			if(false == ControlParts.AnimationSet(this, IndexAnimation, i))
			{
				return(false);
			}
		}
		return(true);
	}

	/* ******************************************************** */
	//! Colliders' CallBack-Execs
	/*!
	Don't use this function. <br>
	(This function is for the "Script_SpriteStudio_Collider")
	*/
	internal void CallBackExecColliderOnTriggerEnter(int PartsID, Collider InstanceColliderPair)
	{
		Library_SpriteStudio.Control.Parts InstanceControlParts = ControlGetParts(PartsID);
		if(null != InstanceControlParts)
		{
			if(null != FunctionColliderEnter)
			{
				FunctionColliderEnter(	this,
										InstanceControlParts.DataParts.Name,
										PartsID,
										InstanceControlParts.InstanceComponentCollider,
										InstanceColliderPair
									);
			}
		}
	}
	internal void CallBackExecColliderOnTriggerExit(int PartsID, Collider InstanceColliderPair)
	{
		Library_SpriteStudio.Control.Parts InstanceControlParts = ControlGetParts(PartsID);
		if(null != InstanceControlParts)
		{
			if(null != FunctionColliderExit)
			{
				FunctionColliderExit(	this,
										InstanceControlParts.DataParts.Name,
										PartsID,
										InstanceControlParts.InstanceComponentCollider,
										InstanceColliderPair
									);
			}
		}
	}
	internal void CallBackExecColliderOnTriggerStay(int PartsID, Collider InstanceColliderPair)
	{
		Library_SpriteStudio.Control.Parts InstanceControlParts = ControlGetParts(PartsID);
		if(null != InstanceControlParts)
		{
			if(null != FunctionColliderStay)
			{
				FunctionColliderStay(	this,
										InstanceControlParts.DataParts.Name,
										PartsID,
										InstanceControlParts.InstanceComponentCollider,
										InstanceColliderPair
									);
			}
		}
	}

	/* ******************************************************** */
	//! Collisions' CallBack-Execs
	/*!
	Don't use this function. <br>
	(This function is for the "Script_SpriteStudio_Collider")
	*/
	internal void CallBackExecCollisionTriggerEnter(int PartsID, Collision InstanceCollisionContact)
	{
		Library_SpriteStudio.Control.Parts InstanceControlParts = ControlGetParts(PartsID);
		if(null != InstanceControlParts)
		{
			if(null != FunctionCollisionEnter)
			{
				FunctionCollisionEnter(	this,
										InstanceControlParts.DataParts.Name,
										PartsID,
										InstanceControlParts.InstanceComponentCollider,
										InstanceCollisionContact
									);
			}
		}
	}
	internal void CallBackExecCollisionTriggerExit(int PartsID, Collision InstanceCollisionContact)
	{
		Library_SpriteStudio.Control.Parts InstanceControlParts = ControlGetParts(PartsID);
		if(null != InstanceControlParts)
		{
			if(null != FunctionCollisionExit)
			{
				FunctionCollisionExit(	this,
										InstanceControlParts.DataParts.Name,
										PartsID,
										InstanceControlParts.InstanceComponentCollider,
										InstanceCollisionContact
									);
			}
		}
	}
	internal void CallBackExecCollisionTriggerStay(int PartsID, Collision InstanceCollisionContact)
	{
		Library_SpriteStudio.Control.Parts InstanceControlParts = ControlGetParts(PartsID);
		if(null != InstanceControlParts)
		{
			if(null != FunctionCollisionStay)
			{
				FunctionCollisionStay(	this,
										InstanceControlParts.DataParts.Name,
										PartsID,
										InstanceControlParts.InstanceComponentCollider,
										InstanceCollisionContact
									);
			}
		}
	}

	/* ******************************************************** */
	//! Get the part's-index(ID) from the part's-name
	/*!
	@param	Name
		Part's name
	@retval	Return-Value
		Index of Part
		-1 == Error / "Name" is not-found.

	Get the Part's-Index(ID) from the name.<br>
	<br>
	The Index is the serial-number (0 origins) in the Animation-data.
	*/
	public int IDGetParts(string Name)
	{
		return(((null != DataAnimation) && (false == string.IsNullOrEmpty(Name))) ? DataAnimation.IndexGetParts(Name) : -1);
	}

	/* ******************************************************** */
	//! Get the part's-control-workarea
	/*!
	@param	IDParts
		Part's ID (Index)
	@retval	Return-Value
		Part's Control WorkArea<br>
		null == Error.

	Get the Part's-Control-WorkArea.<br>
	<br>
	Part's-Control-WorkArea is used when you need to control the each parts at runtime.<br>
	*/
	public Library_SpriteStudio.Control.Parts ControlGetParts(int IDParts)
	{
		return(((null == ListControlParts) || (0 >= ListControlParts.Length)) ? null : ListControlParts[IDParts]);
	}

	/* ******************************************************** */
	//! Get the index from the animation's name
	/*!
	@param	Name
		Animation's name
	@retval	Return-Value
		Index of Animation
		-1 == Error / "Name" is not-found.

	(Especially,) Get the Index by using this function when two or more animation data is recorded in the imported "ssae" data. <br>
	<br>
	The Index is the serial-number (0 origins) in the (imported "ssae") data. <br>
	The Index is needed when you call "AnimationPlay" function.
	*/
	public int IndexGetAnimation(string Name)
	{
		return((null == DataAnimation) ? -1 : DataAnimation.IndexGetAnimation(Name));
	}

	/* ******************************************************** */
	//! Start playing the animation
	/*!
	@param	IndexAnimation
		Animation's Index<br>
		-1 == Current value is not changed.<br>
		default: -1
	@param	PlayTimes
		-1 == Current value is not changed.<br>
		0 == Infinite-looping <br>
		1 == Not looping<br>
		2 <= Number of Plays<br>
		default: -1
	@param	FrameInitial
		Offset frame-number of starting Play in animation (0 origins). <br>
		At the time of the first play-loop, Animation is started "LabelStart + FrameOffsetStart + FrameInitial".
		-1 == Current value is not changed.<br>
		default: -1
	@param	RateSpeedTimeProgress
		Coefficient of time-passage of animation.<br>
		Minus Value is given, Animation is played backwards.<br>
		0.0f is given, Current value is not changed.<br>
		default: 0.0f
	@param	KindStylePlay
		PlayStyle.NOMAL == Animation is played One-Way.<br>
		PlayStyle.PINGPONG == Animation is played Wrap-Around.<br>
		PlayStyle.NO_CHANGE == Current value is not changed.<br>
		default: PlayStyle.NO_CHANGE
	@param	LabelRangeStart
		Label-name of starting Play in animation.<br>
		"_start" == Top-frame of Animation (reserved label-name)<br>
		"" == Current value is not changed.<br>
		default: ""
	@param	FrameRangeOffsetStart
		Offset frame-number from LabelRangeStart<br>
		Animation's Top-frame is "LabelRangeStart + FrameRangeOffsetStart".<br>
		int.MinValue == Current value is not changed.<br>
		default: int.MinValue
	@param	LabelRangeEnd
		Label-name of the terminal in animation.<br>
		"_end" == Last-frame of Animation (reserved label-name)<br>
		"" == Current value is not changed.<br>
		default: ""
	@param	FrameRangeOffsetEnd
		Offset frame-number from LabelRangeEnd<br>
		Animation's Last-frame is "LabelRangeEnd + FrameRangeOffsetEnd".<br>
		int.MaxValue == Current value is not changed.<br>
		default: int.MaxValue
	@retval	Return-Value
		true == Success <br>
		false == Failure (Error)

	The playing of animation begins. <br>
	<br>
	"IndexAnimation" is the Animation's Index (Get the Index in the "IndexGetAnimation" function.). <br>
	Give "0" to "IndexAnimation" when the animation included in (imported "ssae") data is one. <br>
	When the Animation's Index not existing is given, this function returns false. <br>
	<br>
	The update speed of animation quickens when you give a value that is bigger than 1.0f to "RateSpeedTimeProgress".
	*/
	public bool AnimationPlay(	int No = -1,
								int PlayTimes = -1,
								int FrameInitial = -1,
								float RateSpeedTimeProgress = 0.0f,
								PlayStyle KindStylePlay = PlayStyle.NO_CHANGE,
								string LabelRangeStart = "",
								int FrameRangeOffsetStart = int.MinValue,
								string LabelRangeEnd = "",
								int FrameRangeOffsetEnd = int.MaxValue
							)
	{
		/* Check Fatal-Error */
		if((null == DataCellMap) || (null == DataAnimation))
		{
			return(false);
		}

		/* Get Animation-Data */
		if(0 > No)
		{
			No = IndexAnimation;
		}
		if((0 > No) || ((DataAnimation.CountGetAnimation()) <= No))
		{
			return(false);
		}
		IndexAnimation = No;
		Library_SpriteStudio.Data.Animation DataAnimationPlay = DataAnimation.DataGetAnimation(IndexAnimation);

		/* Parts-WorkArea Setting */
		if(0 != (Status & FlagBitStatus.VALID))
		{
			ControlSetAnimation();
		}

		/* Initialize Playing-Status */
		Status &= FlagBitStatus.VALID;	// Status = FlagBitStatus.CLEAR;

		switch(KindStylePlay)
		{
			case PlayStyle.NO_CHANGE:
				Status |= (true == FlagPingpong) ? FlagBitStatus.STYLE_PINGPONG : FlagBitStatus.CLEAR;
				break;
			case PlayStyle.NORMAL:
				Status &= ~FlagBitStatus.STYLE_PINGPONG;
				FlagPingpong = false;
				break;
			case PlayStyle.PINGPONG:
				Status |= FlagBitStatus.STYLE_PINGPONG;
				FlagPingpong = true;
				break;
			default:
				goto case PlayStyle.NO_CHANGE;
		}

		/* Set Playing Animation-Datas */
		if(int.MinValue != FrameRangeOffsetStart)
		{
			FrameOffsetStart = FrameRangeOffsetStart;
		}
		if(int.MaxValue != FrameRangeOffsetEnd)
		{
			FrameOffsetEnd = FrameRangeOffsetEnd;
		}

		if(true == string.IsNullOrEmpty(LabelRangeStart))
		{
			LabelRangeStart = DataAnimationPlay.NameGetLabel(IndexLabelStart);
			if(null == LabelRangeStart)
			{	/* Error */
				LabelRangeStart = Library_SpriteStudio.ListNameLabelAnimationReserved[(int)Library_SpriteStudio.KindLabelAnimationReserved.START];
			}
		}
		if(true == string.IsNullOrEmpty(LabelRangeEnd))
		{
			LabelRangeEnd = DataAnimationPlay.NameGetLabel(IndexLabelEnd);
			if(null == LabelRangeEnd)
			{	/* Error */
				LabelRangeEnd = Library_SpriteStudio.ListNameLabelAnimationReserved[(int)Library_SpriteStudio.KindLabelAnimationReserved.END];
			}
		}
		DataAnimationPlay.FrameRangeGet(	out FrameNoStart, out FrameNoEnd,
											out IndexLabelStart, out IndexLabelEnd,
											LabelRangeStart, FrameOffsetStart,
											LabelRangeEnd, FrameOffsetEnd
										);

		if(0.0f != RateSpeedTimeProgress)
		{
			RateSpeed = RateSpeedTimeProgress;
			RateSpeedInitial = RateSpeedTimeProgress;
		}
		else
		{
			RateSpeed = (0.0f == RateSpeedInitial) ? 1.0f : RateSpeedInitial;
		}
		if(0.0f > RateSpeed)
		{
			Status = (0 == (Status & FlagBitStatus.STYLE_REVERSE)) ? (Status | FlagBitStatus.STYLE_REVERSE) : (Status & ~FlagBitStatus.STYLE_REVERSE);
			RateSpeed *= -1.0f;
		}

		TimePerFrame = 1.0f / (float)DataAnimationPlay.FramePerSecond;

		FrameNoNow = (-1 == FrameInitial) ? FrameOffsetInitial : FrameInitial;
		FrameNoNow += FrameNoStart;

		if(0 != (Status & FlagBitStatus.STYLE_REVERSE))
		{	/* Play-Reverse */
			Status |= FlagBitStatus.PLAYING_REVERSE;
			FrameNoNow = (FrameNoNow <= FrameNoStart) ? (FrameNoEnd + 1) : FrameNoNow;
		}
		else
		{	/* Play-Normal */
			Status &= ~FlagBitStatus.PLAYING_REVERSE;
			FrameNoNow = (FrameNoNow >= (FrameNoEnd + 1)) ? (FrameNoStart - 1) : FrameNoNow;
		}

		TimesPlay = (0 > PlayTimes) ? TimesPlay : PlayTimes;
		TimesPlayNow = TimesPlay;

		/* Play Start */
		CountLoop = 0;
		TimeDelay = 0.0f;
		TimeElapsed = (FrameNoNow - FrameNoStart) * TimePerFrame;
		FrameNoNow = Mathf.Clamp(FrameNoNow, FrameNoStart, FrameNoEnd);
		Status &= ~FlagBitStatus.PAUSING;
		Status |= FlagBitStatus.PLAYING;
		Status |= FlagBitStatus.PLAYING_START;
		FlagAnimationStopInitial = false;

		FrameRange = (FrameNoEnd - FrameNoStart) + 1;
		TimeRange = (float)FrameRange * TimePerFrame;
		TimePerFrameConsideredRateSpeed = TimePerFrame * RateSpeed;

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
		Status &= ~(FlagBitStatus.PAUSING | FlagBitStatus.PLAYING);
		TimeDelay = 0.0f;

		/* "Instance"/"Effect"-Parts Stop */
		int CountParts = DataAnimation.CountGetParts();
		for(int i=0; i<CountParts; i++)
		{
			switch(ListControlParts[i].DataParts.Kind)
			{
				case Library_SpriteStudio.KindParts.INSTANCE:
					{
						Script_SpriteStudio_Root InstanceRootUnderControl = ListControlParts[i].InstanceRootUnderControl;
						if(null != InstanceRootUnderControl)
						{
							InstanceRootUnderControl.AnimationStop();
						}
					}
					break;

				case Library_SpriteStudio.KindParts.EFFECT:
					{
						Script_SpriteStudio_RootEffect InstanceRootUnderControlEffect = ListControlParts[i].InstanceRootUnderControlEffect;
						if(null != InstanceRootUnderControlEffect)
						{
							InstanceRootUnderControlEffect.AnimationStop();
						}
					}
					break;

				default:
					break;
			}
		}
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

	The playing of animation suspends or resumes.<br>
	This function fails if the animation is not playing.
	*/
	public bool AnimationPause(bool FlagSwitch)
	{
		if(0 == (Status & FlagBitStatus.PLAYING))
		{
			return(false);
		}

		if(true == FlagSwitch)
		{
			if(0 == (Status & FlagBitStatus.PAUSING))
			{
				Status |= FlagBitStatus.PAUSING;
			}
		}
		else
		{
			if(0 != (Status & FlagBitStatus.PAUSING))
			{
				Status &= ~FlagBitStatus.PAUSING;
			}
		}

		/* "Instance"/"Effect"-Parts Pausing */
		int CountParts = DataAnimation.CountGetParts();
		for(int i=0; i<CountParts; i++)
		{
			switch(ListControlParts[i].DataParts.Kind)
			{
				case Library_SpriteStudio.KindParts.INSTANCE:
					{
						Script_SpriteStudio_Root InstanceRootUnderControlInstance = ListControlParts[i].InstanceRootUnderControl;
						if(null != InstanceRootUnderControlInstance)
						{
							InstanceRootUnderControlInstance.AnimationPause(FlagSwitch);
						}
					}
					break;

				case Library_SpriteStudio.KindParts.EFFECT:
					{
						Script_SpriteStudio_RootEffect InstanceRootUnderControlEffect = ListControlParts[i].InstanceRootUnderControlEffect;
						if(null != InstanceRootUnderControlEffect)
						{
							InstanceRootUnderControlEffect.AnimationPause(FlagSwitch);
						}
					}
					break;

				default:
					break;
			}
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

	Use this function for checking the animation's play-status.<br>
	*/
	public bool AnimationCheckPlay()
	{
		return(StatusIsPlaying);
	}

	/* ******************************************************** */
	//! Check if the animation is being paused (suspended)
	/*!
	@param
		(None)
	@retval	Return-Value
		true == Suspended <br>
		false == Not Suspended or Stopping

	Use this function for checking the animation's pause-status.<br>
	*/
	public bool AnimationCheckPause()
	{
		return((true == StatusIsPlaying) ? StatusIsPausing : false);
	}

	/* ******************************************************** */
	//! Force-Hide Set
	/*!
	@param	IDParts
		IDParts(Part-Index)<br>
		0 >= Root (All Parts Hide)
	@param	FlagSwitch
		true == Force-Hide Set (Hide)<br>
		false == Force-Hide Reset (Show. State of animation is followed.)
	@param	FlagSetChildren
		true == Children are set same state.<br>
		false == only oneself.<br>
		default: false
	@retval	Return-Value
		true == Success <br>
		false == Failure (Error)

	The state of "Force-Hide" is set, it is not concerned with the state of animation.<br>
	To hide all parts, set less than 0 to "PartsID".<br>
	*/
	public bool HideSetForce(int IDParts, bool FlagSwitch, bool FlagSetChildren=false)
	{
		if(0 >= IDParts)
		{	/* "Root"-Parts */
			FlagHideForce = FlagSwitch;
			return(true);
		}

		if((null == ListControlParts) || (ListControlParts.Length <= IDParts))
		{
			return(false);
		}

		Library_SpriteStudio.Control.Parts InstancePartsControl = ListControlParts[IDParts];
		InstancePartsControl.StatusHideForce = FlagSwitch;

		if(true == FlagSetChildren)
		{
			Library_SpriteStudio.Data.Parts InstanceParts = DataAnimation.DataGetParts(IDParts);
			if(null != InstanceParts)
			{
				for(int i=0; i<InstanceParts.ListIDChild.Length; i++)
				{
					HideSetForce(InstanceParts.ListIDChild[i], FlagSwitch, FlagSetChildren);
				}
			}
		}

		return(true);
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
	The detail of how to set, please refer to the commentary of "Library_SpriteStudio.Control.ColorBlendOverwrite".
	*/
	public Library_SpriteStudio.Control.ColorBlendOverwrite DataGetColorBlendOverwrite()
	{
		if(null == dataColorBlendOverwrite)
		{
			dataColorBlendOverwrite = new Library_SpriteStudio.Control.ColorBlendOverwrite();
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

	/* ******************************************************** */
	//! Get Material
	/*!
	@param	IndexCellMap
		Serial-number of using Cell-Map<br>
		MEMO: using Texture-Index ,After Ver.1.4.3
	@param	Operation
		Color-Blend Operation for the target
	@retval	Return-Value
		Material
	*/
	public Material MaterialGet(int IndexCellMap, Library_SpriteStudio.KindColorOperation KindOperation)
	{
		const int CountLength = (int)Library_SpriteStudio.KindColorOperation.TERMINATOR - 1;
		return((	(0 <= IndexCellMap)
					&& ((null != TableMaterial) && ((TableMaterial.Length / CountLength) > IndexCellMap))
					&& (Library_SpriteStudio.KindColorOperation.NON < KindOperation) && (Library_SpriteStudio.KindColorOperation.TERMINATOR > KindOperation)
				)
				? TableMaterial[(IndexCellMap * CountLength) + ((int)KindOperation - 1)]
				: null
			);
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
	public Material[] TableMaterialCopy()
	{
		return(Library_SpriteStudio.Utility.TableMaterial.Copy(TableMaterial));
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
	public int CountGetTextureTableMaterial()
	{
		return(Library_SpriteStudio.Utility.TableMaterial.CountGetTexture(TableMaterial));
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
	public bool TextureChangeTableMaterial(int Index, Texture2D DataTexture)
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
	//! Change playing the Instance-Object
	/*!
	@param	IDParts
		IDParts(Part-Index)
	@param	PrefabNew
		New Prefab<br>
		null == Instance-Object not change.
	@retval	Return-Value
		true == Success <br>
		false == Failure (Error)

	The playing of Instance-Object's Prefab changes.<br>
	<br>
	In order to run this function properly, you need to meet the required minimum below.<br>
	- Part that are specified in the " IDParts" should be "Instance Parts".<br>
	- "PrefabNew"(Instance-Object) and "NameAnimation"(Instance-Object's Animation) must meet the playback specification of part that are specified in the "IDParts".<br>
	- Animation must be stopped.<br>
	*/
	public bool InstanceChange(int IDParts, GameObject PrefabNew, string NameAnimation)
	{
		bool FlagRenewInstance = false;

		if((0 > IDParts) || (null == DataAnimation))
		{
			return(false);
		}

		Library_SpriteStudio.Data.Parts DataParts = DataAnimation.DataGetParts(IDParts);
		if(Library_SpriteStudio.KindParts.INSTANCE != DataParts.Kind)
		{
			return(false);
		}

		Library_SpriteStudio.Control.Parts ControlParts = ControlGetParts(IDParts);
		if(null == ControlParts)
		{
			return(false);
		}

		/* Change Instance-Object */
		if(null != PrefabNew)
		{
			ControlParts.PrefabUnderControl = PrefabNew;
			FlagRenewInstance = true;
		}

		/* Change Instance-Object's Animation */
		ControlParts.NameAnimationUnderControl = NameAnimation;

		/* Refresh Instance */
//		ControlParts.Status |= Library_SpriteStudio.Control.Parts.FlagBitStatus.REFRESH_INSTANCEUNDERCONTROL;
		ControlParts.FrameNoPreviousUpdateUnderControl = -1;	/* ReDecode Instance-Attribute */
		return(ControlParts.RebootPrefabInstance(this, IDParts, FlagRenewInstance));
	}

	/* ******************************************************** */
	//! Change Material-Table
	/*!
	@param	TableMaterialChange
		New Material-Table<br>
		null == Disable the changed
	@param	FlagChangeInstance
		true == Set "TableMaterialChangeInstance" to "Instance" Animation-Objects.<br>
		false == Do not affect "Instance" Animation-Objects.<br>
		default: false
	@param	TableMaterialChangeInstance
		New Material-Table for "Instance" Animation-Objects.<br>
		null == Disable the changed (When "FlagChangeInstance" is true).<br>
		default: null
	@param	FlagChangeEffect
		true == Set "TableMaterialChangeEffect" to "Effect" Animation-Objects.<br>
		false == Do not affect "Effect" Animation-Objects.<br>
		default: false
	@param	TableMaterialChangeEffect
		New Material-Table for "Effect" Animation-Objects.<br>
		null == Disable the changed (When "FlagChangeEffect" is true).<br>
		default: null
	@retval	Return-Value
		true == Success <br>
		false == Failure (Error)

	Replace the Material-Table that is used in the Animation.<br>
	<br>
	Following the format of the Material-Table. (for "TableMaterialChange" / "TableMaterialChangeInstance")<br>
	Table[0]: Material for Texture 0(Cell-Map 0) / Blending "Mix"<br>
	Table[1]: Material for Texture 0(Cell-Map 0) / Blending "Add"<br>
	Table[2]: Material for Texture 0(Cell-Map 0) / Blending "Sub"<br>
	Table[3]: Material for Texture 0(Cell-Map 0) / Blending "Mul"<br>
	Table[4]: Material for Texture 1(Cell-Map 1) / Blending "Mix"<br>
	Table[5]: Material for Texture 1(Cell-Map 1) / Blending "Add"<br>
	...<br>
	<br>
	Following the format of the Material-Table. (for "TableMaterialChangeEffect")<br>
	Table[0]: Material for Texture 0(Cell-Map 0) / Blending "Mix"<br>
	Table[1]: Material for Texture 0(Cell-Map 0) / Blending "Add" (PreMultiplied Alpha)<br>
	Table[2]: Material for Texture 0(Cell-Map 0) / Blending "Add2" (Straight Alpha)<br>
	Table[3]: Material for Texture 1(Cell-Map 1) / Blending "Mul"<br>
	Table[4]: Material for Texture 1(Cell-Map 1) / Blending "Add" (PreMultiplied Alpha)<br>
	...<br>
	<br>
	Utility for processing this table is defined in "Library_SpriteStudio.Utility.TableMaterial" class.
	*/
	public bool TableMaterialChange(	Material[] TableMaterialChange,
										bool FlagChangeInstance = false,
										Material[] TableMaterialChangeInstance = null,
										bool FlagChangeEffect = false,
										Material[] TableMaterialChangeEffect = null
									)
	{
		if(null != TableMaterialChange)
		{
			TableMaterial = TableMaterialChange;
		}

		if(true == (FlagChangeInstance | FlagChangeInstance))
		{
			int Count = DataAnimation.CountGetParts();
			Library_SpriteStudio.Data.Parts DataParts;
			Library_SpriteStudio.Control.Parts InstanceControlParts;
			for(int i=0; i<Count; i++)
			{
				DataParts = DataAnimation.DataGetParts(i);
				InstanceControlParts = ControlGetParts(i);
				if(null == InstanceControlParts)
				{
					continue;
				}
				switch(DataParts.Kind)
				{
					case Library_SpriteStudio.KindParts.INSTANCE:   /* Instance-Parts */
						if((true == FlagChangeInstance) && (null != TableMaterialChangeInstance))
						{
							Script_SpriteStudio_Root InstanceRootUnderControl = InstanceControlParts.InstanceRootUnderControl;
							if(null != InstanceRootUnderControl)
							{
								InstanceRootUnderControl.TableMaterialChange(	TableMaterialChangeInstance,
																				FlagChangeInstance,
																				TableMaterialChangeInstance,
																				FlagChangeEffect,
																				TableMaterialChangeEffect
																			);
							}
						}
						break;

					case Library_SpriteStudio.KindParts.EFFECT: /* Effect-Parts */
						if((true == FlagChangeInstance) && (null != TableMaterialChangeEffect))
						{
							Script_SpriteStudio_RootEffect InstanceRootUnderControl = InstanceControlParts.InstanceRootUnderControlEffect;
							if(null != InstanceRootUnderControl)
							{
								/* Cell-Table Change */
								InstanceRootUnderControl.TableMaterialChange(TableMaterialChangeEffect);
							}
						}
						break;

					default:
						break;
				}
			}
		}
		return(true);
	}

	/* ******************************************************** */
	//! Get Cell-Map count
	/*!
	@param	FlagConsideringChanged
		true == Auto<br>
		false == Initially-Set, force<br>
		Default: false
	@retval	Return-Value
		Count of "Cell-Map"s<br>
		-1 == Failure (Error)

	Get count of "Cell-Map"s.<br>
	<br>
	When "FlagConsideringChanged" is true, this function returns the number of "Cell-Map"s of the changed (If "Cell-Map"-Table has been changed).
	*/
	public int CountGetCellMap(bool FlagConsideringChanged=false)
	{
		if((true == FlagConsideringChanged) && (null != TableCellChange))
		{
			return(Library_SpriteStudio.Utility.TableCellChange.CountGetCellMap(TableCellChange));
		}

		if(null != DataCellMap)
		{
			return(DataCellMap.CountGetCellMap());
		}

		return(-1);
	}

	/* ******************************************************** */
	//! Get "Cell-Map"'s Index
	/*!
	@param	Name
		"Cell-Map" Name
	@retval	Return-Value
		"Cell-Map"'s Index<br>
		-1 == Not-Found / Failure (Error)

	Get Cell-Map's Index.<br>
	However, the search target is the only "Cell-Map"s that is initially set ("DataCellMap" member in this Class).<br>
	*/
	public int IndexGetCellMap(string Name)
	{
		if(null == DataCellMap)
		{
			return(-1);
		}
		return(DataCellMap.IndexGetCellMap(Name));
	}

	/* ******************************************************** */
	//! Get Cell count
	/*!
	@param	IndexCellMap
		"Cell-Map"'s Index
	@param	FlagConsideringChanged
		true == Auto<br>
		false == Initially-Set, force<br>
		Default: false
	@retval	Return-Value
		Count of "Cell-Map"s<br>
		-1 == Failure (Error)

	Get count of Cells in the "Cell-Map".<br>
	<br>
	When "FlagConsideringChanged" is true, this function returns the number of cell in "Cell-Map"s of the changed (If "Cell-Map"-Table has been changed).
	*/
	public int CountGetCell(int IndexCellMap, bool FlagConsideringChanged=false)
	{
		if((true == FlagConsideringChanged) && (null != TableCellChange))
		{
			return(Library_SpriteStudio.Utility.TableCellChange.CountGetCell(TableCellChange, IndexCellMap));
		}

		if(null != DataCellMap)
		{
			Library_SpriteStudio.Data.CellMap InstanceCellMap = DataCellMap.DataGetCellMap(IndexCellMap);
			if(null == InstanceCellMap)
			{
				return(-1);
			}
			return(InstanceCellMap.CountGetCell());
		}

		return(-1);
	}

	/* ******************************************************** */
	//! Get Cell's Index
	/*!
	@param	IndexCellMap
		"Cell-Map"'s Index
	@param	Name
		Cell's Name
	@param	FlagConsideringChanged
		true == Auto<br>
		false == Initially-Set, force<br>
		Default: false
	@retval	Return-Value
		Cell's Index<br>
		-1 == Not-Found / Failure (Error)

	Get Cell's Index.<br>
	<br>
	When "FlagConsideringChanged" is true, this function returns the index of cell in "Cell-Map"s of the changed (If "Cell-Map"-Table has been changed).
	*/
	public int IndexGetCell(int IndexCellMap, string NameCell, bool FlagConsideringChanged=false)
	{
		if(true == string.IsNullOrEmpty(NameCell))
		{
			return(-1);
		}

		if((true == FlagConsideringChanged) && (null != TableCellChange))
		{
			return(Library_SpriteStudio.Utility.TableCellChange.IndexGetCell(TableCellChange, IndexCellMap, NameCell));
		}

		if(null != DataCellMap)
		{
			Library_SpriteStudio.Data.CellMap InstanceCellMap = DataCellMap.DataGetCellMap(IndexCellMap);
			if(null == InstanceCellMap)
			{
				return(-1);
			}
			return(InstanceCellMap.IndexGetCell(NameCell));
		}

		return(-1);
	}

	/* ******************************************************** */
	//! Change Part's-Cell
	/*!
	@param	IDParts
		IDParts(Part-Index)
	@param	IndexCellMap
		Cell-Map Index<br>
		-1 == Accorde to Animation-Data
	@param	IndexCell
		Cell Index in Cell-Map<br>
		-1 == Accorde to Animation-Data
	@param	FlagIgnoreAttributeCell
		true == Ignore "Reference-Cell" Attribute in the Animation-Data.<br>
		false == Will be updated, if the new "Reference-Cell"Attribute-Data has appeared after changing.<br>
		default: false
	@retval	Return-Value
		true == Success <br>
		false == Failure (Error)

	Change the cells that are displayed in the parts.<br>
	<br>
	This function must be called after "Start" and "Awake" are executed.<br>
	<br>
	Caution: This function does not effect the data that is set "Calculate In Advance" to "true" at the import.
	*/
	public bool CellChange(int IDParts, int IndexCellMap, int IndexCell, bool FlagIgnoreAttributeCell=false)
	{
		Library_SpriteStudio.Control.Parts InstanceControlParts = ControlGetParts(IDParts);
		if(null == InstanceControlParts)
		{
			return(false);
		}

		if(null == TableCellChange)
		{	/* Default */
			if((null == DataCellMap) || (null == DataCellMap.ListDataCellMap))
			{	/* Error */
				return(false);
			}

			if((0 > IndexCellMap) || (0 > IndexCell))
			{	/* Clear Changing */
				goto CellChange_Clear;
			}

			if(DataCellMap.ListDataCellMap.Length <= IndexCellMap)
			{	/* Error */
				return(false);
			}
			if(DataCellMap.ListDataCellMap[IndexCellMap].CountGetCell() <= IndexCell)
			{	/* Error */
				return(false);
			}
		}
		else
		{	/* Cell-Table Changed */
			if((0 > IndexCellMap) || (0 > IndexCell))
			{	/* Clear Changing */
				goto CellChange_Clear;
			}

			if(false == Library_SpriteStudio.Utility.TableCellChange.TableCheckValidIndex(TableCellChange, IndexCellMap, IndexCell))
			{	/* Error */
				return(false);
			}
		}

		InstanceControlParts.IndexCellMapOverwrite = IndexCellMap;
		InstanceControlParts.IndexCellOverwrite = IndexCell;
		InstanceControlParts.Status = (false == FlagIgnoreAttributeCell) ? 
										(InstanceControlParts.Status & ~Library_SpriteStudio.Control.Parts.FlagBitStatus.OVERWRITE_CELL_IGNOREATTRIBUTE) :
										(InstanceControlParts.Status | Library_SpriteStudio.Control.Parts.FlagBitStatus.OVERWRITE_CELL_IGNOREATTRIBUTE);
		InstanceControlParts.Status |= Library_SpriteStudio.Control.Parts.FlagBitStatus.OVERWRITE_CELL_UNREFLECTED;
		return(true);

	CellChange_Clear:;
		InstanceControlParts.IndexCellMapOverwrite = -1;
		InstanceControlParts.IndexCellOverwrite = -1;
		InstanceControlParts.Status &= ~Library_SpriteStudio.Control.Parts.FlagBitStatus.OVERWRITE_CELL_IGNOREATTRIBUTE;
		InstanceControlParts.Status |= Library_SpriteStudio.Control.Parts.FlagBitStatus.OVERWRITE_CELL_UNREFLECTED;
		return(true);
	}

	/* ******************************************************** */
	//! Change "Cell-Map"-Table
	/*!
	@param	InstanceTableCellChange
		New "Cell-Map"-Table<br>
		null == Disable the changed
	@param	FlagChangeInstance
		true == Set "InstanceTableCellChangeInstance" to "Instance" Animation-Objects.<br>
		false == Do not affect "Instance" Animation-Objects.<br>
		default: false
	@param	InstanceTableCellChangeInstance
		New "Cell-Map"-Table for "Instance" Animation-Objects.<br>
		null == Disable the changed (When "FlagChangeInstance" is true).<br>
		default: null
	@param	FlagChangeEffect
		true == Set "InstanceTableCellChangeEffect" to "Effect" Animation-Objects.<br>
		false == Do not affect "Effect" Animation-Objects.<br>
		default: false
	@param	InstanceTableCellChangeEffect
		New "Cell-Map"-Table for "Effect" Animation-Objects.<br>
		null == Disable the changed (When "FlagChangeEffect" is true).<br>
		default: null
	@retval	Return-Value
		true == Success <br>
		false == Failure (Error)

	Replace the Material-Table that is used in the Animation.<br>
	<br>
	This function must be called after "Start" and "Awake" are executed.<br>
	<br>
	Following the format of the "Cell-Map"-Table.<br>
	Table[0][0]: Replacing Cell Data (Cell-Map No.:0, Cell No.:0)<br>
	Table[0][1]: Replacing Cell Data (Cell-Map No.:0, Cell No.:1)<br>
	Table[0][2]: Replacing Cell Data (Cell-Map No.:0, Cell No.:2)<br>
	...<br>
	Table[0][x]: Replacing Cell Data (Cell-Map No.:0, The last of the cell owned by Cell-Map 0)<br>
	--- (End of Table[0])<br>
	Table[1][0]: Replacing Cell Data (Cell-Map No.:1, Cell No.:0)<br>
	Table[1][1]: Replacing Cell Data (Cell-Map No.:1, Cell No.:1)<br>
	...<br>
	Table[1][y]: Replacing Cell Data (Cell-Map No.:1, The last of the cell owned by Cell-Map 1)<br>
	---(End of Table[1])<br>
	Table[n][0]: Replacing Cell Data (Cell-Map No.:n, Cell No.: 0)<br>
	...<br>
	Table[n][z]: Replacing Cell Data (Cell-Map No.:n, The last of the cell owned by Cell-Map n)<br>
	---(End of Table[n])<br>
	<br>
	Following the function of each member of the " Library_SpriteStudio.Control.CellChange".
	- IndexTexture: Index of Texture (in Material-Table)<br>
	- DataCellMap: Information of Texture-Atlas that Cell is stored. (".ListCell" is dis-used.)<br>
	- DataCell: Cell's placement-information in Texture-Atlas.<br>
	<br>
	Utility for processing this table is defined in "Library_SpriteStudio.Utility.TableCellChange" class.<br>
	<br>
	Caution: This function does not effect the data that is set "Calculate In Advance" to "true" at the import.
	*/
	public bool CellMapChange(	Library_SpriteStudio.Control.CellChange[][] InstanceTableCellChange,
								bool FlagChangeInstance = false,
								Library_SpriteStudio.Control.CellChange[][] InstanceTableCellChangeInstance = null,
								bool FlagChangeEffect = false,
								Library_SpriteStudio.Control.CellChange[][] InstanceTableCellChangeEffect = null
							)
	{
		TableCellChange = InstanceTableCellChange;
		Status |= FlagBitStatus.CELL_TABLECHANGED;

		if(true == (FlagChangeInstance | FlagChangeEffect))
		{
			int Count = DataAnimation.CountGetParts();
			Library_SpriteStudio.Data.Parts DataParts;
			Library_SpriteStudio.Control.Parts InstanceControlParts;
			for(int i=0; i<Count; i++)
			{
				DataParts = DataAnimation.DataGetParts(i);
				InstanceControlParts = ControlGetParts(i);
				if(null == InstanceControlParts)
				{
					continue;
				}
				switch(DataParts.Kind)
				{
					case Library_SpriteStudio.KindParts.INSTANCE:   /* Instance-Parts */
						if(true == FlagChangeInstance)
						{
							Script_SpriteStudio_Root InstanceRootUnderControl = InstanceControlParts.InstanceRootUnderControl;
							if(null != InstanceRootUnderControl)
							{
								/* Cell-Table Change */
								InstanceRootUnderControl.CellMapChange(	InstanceTableCellChangeInstance,
																		FlagChangeInstance,
																		InstanceTableCellChangeInstance,
																		FlagChangeEffect,
																		InstanceTableCellChangeEffect
																	);
							}
						}
						break;

					case Library_SpriteStudio.KindParts.EFFECT: /* Effect-Parts */
						if(true == FlagChangeInstance)
						{
							Script_SpriteStudio_RootEffect InstanceRootUnderControl = InstanceControlParts.InstanceRootUnderControlEffect;
							if(null != InstanceRootUnderControl)
							{
								/* Cell-Table Change */
								InstanceRootUnderControl.CellMapChange(InstanceTableCellChange);
							}
						}
						break;

					default:
						break;
				}
			}
		}
		return(true);
	}
}
