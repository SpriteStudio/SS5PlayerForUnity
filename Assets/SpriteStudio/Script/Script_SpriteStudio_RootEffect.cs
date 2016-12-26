/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;

using RandomGenerator = Library_SpriteStudio.Utility.Random.XorShift32;

[ExecuteInEditMode]
[System.Serializable]
public class Script_SpriteStudio_RootEffect : Library_SpriteStudio.Script.Root
{
	public enum Constants : int
	{
		LIMIT_PARTICLE = 1024,

		LIMIT_SUBEMITTER_DEPTH = 2,
		LIMIT_SUBEMITTER_COUNT = 10,
	}

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

		STYLE_PINGPONG = 0x08000000,	/* Reserved */
		STYLE_REVERSE = 0x04000000,	/* Reserved */

		PLAYING_START = 0x00800000,	/* Reserved */
		PLAYING_REVERSE = 0x00400000,	/* Reserved */
		PLAYING_REVERSEPREVIOUS = 0x00200000,	/* Reserved */
		PLAYING_TURN = 0x00100000,	/* Reserved */
		PLAYING_FIRSTUPDATE = 0x00080000,	/* Reserved */
		PLAYING_INFINITY = 0x00040000,

//		DECODE_USERDATA = 0x00008000,	/* Reserved & Disuse */
//		DECODE_INSTANCE = 0x00004000,	/* Reserved & Disuse */
//		DECODE_EFFECT = 0x00002000,	/* Reserved & Disuse */

		REQUEST_DESTROY = 0x00000800,	/* Reserved */
		REQUEST_PLAYEND = 0x00000400,

		CELL_TABLECHANGED = 0x00000080,

		CLEAR = 0x00000000,
	}

	/* Base-Datas */
	public Script_SpriteStudio_DataEffect DataEffect;

	/* Material Replacement Parameters  */
	/* MEMO: "IndexMaterialBlendDefault" & "IndexMaterialBlendOffset" is for Inspector & Importer. */
	public static readonly int[] IndexMaterialBlendDefault = new int[(int)Library_SpriteStudio.KindColorOperationEffect.TERMINATOR_KIND - 1]
	{
		(int)Library_SpriteStudio.KindColorOperationEffect.MIX - 1,
		(int)Library_SpriteStudio.KindColorOperationEffect.ADD - 1,
	};
	public static readonly int[] CountVariationShader = new int[(int)Library_SpriteStudio.KindColorOperationEffect.TERMINATOR_KIND - 1]
	{
		(int)Library_SpriteStudio.KindColorOperationEffect.MIX - (int)Library_SpriteStudio.KindColorOperationEffect.MIX,
		(int)Library_SpriteStudio.KindColorOperationEffect.ADD2 - (int)Library_SpriteStudio.KindColorOperationEffect.ADD,
	};
	public int[] IndexMaterialBlendOffset;

	/* Effect Datas */
	internal float RateTimeToFrame;
	internal float FrameNow;
	internal float FrameLength;

	/* Control Datas */
	public int CountLimitParticleInitial;
	internal Library_SpriteStudio.Control.PoolPartsEffect PoolParts = null;
	internal bool FlagUnderControl = false;

	/* Playing Datas: for Runtime (WorkArea) */
	private FlagBitStatus Status = FlagBitStatus.CLEAR;	
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
	internal bool StatusIsPlayStylePingpong
	{
		get
		{
			return(0 != (Status & FlagBitStatus.STYLE_PINGPONG));
		}
	}
	internal bool StatusIsPlayStyleReverse
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
		set
		{
			Status = (true == value) ? (Status | FlagBitStatus.PLAYING_INFINITY) : (Status & ~FlagBitStatus.PLAYING_INFINITY);
		}
	}
//	internal bool StatusIsDecodeUserData
//	{
//		get
//		{
//			return(0 != (Status & FlagBitStatus.DECODE_USERDATA));
//		}
//	}
//	internal bool StatusIsDecodeInstance
//	{
//		get
//		{
//			return(0 != (Status & FlagBitStatus.DECODE_INSTANCE));
//		}
//	}
//	internal bool StatusIsDecodeEffect
//	{
//		get
//		{
//			return(0 != (Status & FlagBitStatus.DECODE_EFFECT));
//		}
//	}
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

	/* CallBack-s */
	internal Library_SpriteStudio.FunctionCallBackPlayEndEffect FunctionPlayEnd = null;

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
		StartBase((int)Constants.LIMIT_PARTICLE);

		/* Initialize */
		if(null != DataEffect)
		{
			/* WorkArea BootUp */
			RateTimeToFrame = (float)DataEffect.CountFramePerSecond;	/* Provisional */
			TimePerFrame = 1.0f / RateTimeToFrame;
			PoolBootUpParts();

			/* Status Set */
			Status |= FlagBitStatus.VALID;

			/* Play Animation Initialize */
			if(false == FlagUnderControl)
			{
				AnimationPlay();
			}
		}
	}

//	void Update()
//	{
//	}

	void LateUpdate()
	{
		float TimeDelta = Time.deltaTime * RateSpeed;

		if((null == DataEffect) || (null == DataCellMap))
		{
			return;
		}

		/* DrawManager Get */
		if((null == InstanceManagerDraw) && (null == InstanceRootParent))
		{	/* MEMO: "Instance" and "Effect" cannot have Manager-Draw. */
			InstanceManagerDraw = Library_SpriteStudio.Utility.Parts.ManagerDrawGetParent(gameObject);
		}

		/* DrawParts-Cluster Create */
		LateUpdateBase();

		/* Animation Play-Check */
		if(0 == (Status & FlagBitStatus.PLAYING)) 
		{	/* Not-Playing */
			return;
		}

		/* Particle&Emitter Control WorkArea Create */
		if(null == PoolParts)
		{
			PoolBootUpParts();
		}

		/* Update Emitter & Particle */
		ChainClusterDrawParts.ChainCleanUp();   /* DrawParts-Cluster-Chain Clear */
		TimeElapsed += ((0 != (Status & FlagBitStatus.PAUSING)) || (0 != (Status & FlagBitStatus.PLAYING_START))) ? 0.0f : TimeDelta;
		FrameNow = TimeElapsed * RateTimeToFrame;
		if(0 != (Status & FlagBitStatus.PLAYING_INFINITY))
		{	/* Independent */
//			FrameNow %= FrameLength;
		}
		else
		{	/* Dependent */
			FrameNow = Mathf.Clamp(FrameNow, 0.0f, FrameLength);
		}
		PoolParts.Update(this);

		/* Set to DrawManager */
		/* MEMO: Test */
		if(((null != InstanceManagerDraw) && (null != DrawObject)) && (false == FlagHideForce))
		{
			/* Set To Draw-Manager */
			InstanceManagerDraw.DrawSet(this);
		}

		/* Status Update */
		Status &= ~FlagBitStatus.PLAYING_START;
		Status &= ~FlagBitStatus.CELL_TABLECHANGED;
	}
	internal void TimeElapsedSetForce(float TimeElapsedForce, bool FlagReverseParent)
	{   /* MEMO: In principle, This Function is for calling from "(Control.PartsEffect.)Update". */
		TimeElapsed = TimeElapsedForce;
	}

	private bool PoolBootUpParts()
	{
		int Count = CountLimitParticleInitial;
		if(0 >= Count)
		{
			Count = (int)Constants.LIMIT_PARTICLE;
		}

		PoolParts = new Library_SpriteStudio.Control.PoolPartsEffect();
		PoolParts.BootUpWorkArea(this, Count);
		PoolParts.BootUp(this);

		FrameLength = (float)(PoolParts.EffectDurationFull);

		return(true);
	}

	public static Library_SpriteStudio.Utility.Random.Generator InstanceCreateRandom()
	{
		return(new RandomGenerator());
	}

	private readonly static System.DateTime TimeUnixEpoch = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
	private static uint RandomKeyMakeID = 123456;
	public static uint KeyCreateRandom()
	{
		RandomKeyMakeID++;

		/* MEMO: time(0) at C++ */
		System.DateTime TimeNow = System.DateTime.Now;
		TimeNow.ToUniversalTime();
		System.TimeSpan SecNow = TimeNow - TimeUnixEpoch;
		
		return(RandomKeyMakeID + (uint)SecNow.TotalSeconds);
	}

	internal void SeedOffsetSet(uint Value)
	{
		if(null != PoolParts)
		{
			PoolParts.SeedOffsetSet(Value);
		}
	}

	public void TableCreateBlendOffset()
	{
		int CountBlendKind = (int)Library_SpriteStudio.KindColorOperationEffect.TERMINATOR_KIND - 1;
		if((null == IndexMaterialBlendOffset) || (CountBlendKind != IndexMaterialBlendOffset.Length))
		{
			IndexMaterialBlendOffset = new int[CountBlendKind];
			for(int i=0; i<CountBlendKind; i++)
			{
				IndexMaterialBlendOffset[i] = 0;
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
	"Name" must be a "Emitter"-part's name on "SpriteStudio5".
	<br>
	The Index is the serial-number (0 origins) in the Runtime (not in Effect-data).
	*/
	public int IDGetParts(string Name)
	{
		if((null != PoolParts) && (null != PoolParts.PoolEmitter) && (false == string.IsNullOrEmpty(Name)))
		{
			int CountEmitter = PoolParts.PoolEmitter.Length;
			for(int i=0; i<CountEmitter; i++)
			{
//				if(0 == string.Compare(Name, PoolParts.PoolEmitter[i].InstanceDataParts.Name))
				if(Name == PoolParts.PoolEmitter[i].InstanceDataParts.Name)
				{
					return(i);
				}
			}
		}
		return(-1);
	}

	/* ******************************************************** */
	//! Start playing the animation
	/*!
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
	@retval	Return-Value
		true == Success <br>
		false == Failure (Error)

	The playing of animation begins. <br>
	<br>
	The update speed of animation quickens when you give a value that is bigger than 1.0f to "RateTime".
	*/
	public bool AnimationPlay(	bool PlayLoop = false,
								float RateSpeedTimeProgress = 1.0f
							)
	{
		/* Check Fatal-Error */
		if((null == DataCellMap) || (null == DataEffect))
		{
			return(false);
		}

		/* Pool Refresh */
		if(null == PoolParts)
		{
			PoolBootUpParts();
		}

		RateSpeed = RateSpeedTimeProgress;
		RateTimeToFrame = (null != InstanceRootParent) ? (1.0f / InstanceRootParent.TimePerFrame) : (float)DataEffect.CountFramePerSecond;
		TimeElapsed = (0.0f > RateSpeed) ? (FrameLength * TimePerFrame) : 0.0f; 

		/* Status Set */
		Status |= FlagBitStatus.PLAYING;
		Status |= FlagBitStatus.PLAYING_START;

		return(true);
	}

	/* ******************************************************** */
	//! Stop playing the animation
	/*!
	@param	FlagWarpEnd
		Specifying Display-Frame<br>
		true == End-Frame<br>
		false == Status-quo<br>
		default: false
	@retval	Return-Value
		(None)

	The playing of animation stops.
	*/
	public void AnimationStop()
	{
		/* Pool Refresh */
		if(null != PoolParts)
		{
			PoolParts.ParticleReset();
		}

		/* Status Set */
		Status &= ~FlagBitStatus.PLAYING;

		return;
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

		return(false);
	}

	/* ******************************************************** */
	//! Check the animation is playing
	/*!
	@param
		(None)
	@retval	Return-Value
		true == Playing / Pause-true(suspended) <br>
		false == Stopping

	Use this function for checking the animation's play-status.
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

	Use this function for checking the animation's pause-status.
	*/
	public bool AnimationCheckPause()
	{
		return((true == StatusIsPlaying) ? StatusIsPausing : false);
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
	public Material MaterialGet(int IndexCellMap, Library_SpriteStudio.KindColorOperationEffect KindOperation)
	{
		const int CountLength = (int)Library_SpriteStudio.KindColorOperationEffect.TERMINATOR - 1;

		if((0 > IndexCellMap) || (null == TableMaterial) || ((TableMaterial.Length / CountLength) <= IndexCellMap))
		{
			return(null);
		}
		if((Library_SpriteStudio.KindColorOperationEffect.NON >= KindOperation) || (Library_SpriteStudio.KindColorOperationEffect.TERMINATOR_KIND <= KindOperation))
		{
			return(null);
		}
		int IndexBlendKind = (int)KindOperation - 1;
		int IndexMaterial = IndexMaterialBlendDefault[IndexBlendKind];
		if((null != IndexMaterialBlendOffset) && (0 < IndexMaterialBlendOffset.Length))
		{
			IndexMaterial += IndexMaterialBlendOffset[IndexBlendKind];
		}
		return(TableMaterial[(IndexCellMap * CountLength) + IndexMaterial]);
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
	public int TextureGetCount()
	{
		return(Library_SpriteStudio.Utility.TableMaterial.CountGetTextureEffect(TableMaterial));
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
	Texture is set to "TableMaterial[Index * 3] to TableMaterial[Index * 3 + 2]".<br>
	<br>
	Appropriate range of "Material"-instance of the texture you want to change will be created in the new.<br>
	*/
	public bool TextureChange(int Index, Texture2D DataTexture)
	{
		int CountTextureBlock = (int)Library_SpriteStudio.KindColorOperationEffect.TERMINATOR - 1;

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
			TableMaterial[IndexMaterial + i] = new Material(Library_SpriteStudio.Shader_SpriteStudioEffect[i]);
			TableMaterial[IndexMaterial + i].mainTexture = DataTexture;
		}
		return(true);
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
	public bool TableMaterialChange(Material[] TableMaterialChange)
	{
		TableMaterial = TableMaterialChange;
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
	However, the search target is the only "Cell-Map"s that is initially set ("DataCellMap" member in this Class).
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
	This function must be called after "Start" and "Awake" are executed.
	*/
	public bool CellChange(int IDParts, int IndexCellMap, int IndexCell, bool FlagIgnoreAttributeCell=false)
	{
		if((null == PoolParts) || (null == PoolParts.PoolEmitter))
		{
			return(false);
		}
		if((0 > IDParts) || (PoolParts.PoolEmitter.Length <= IDParts))
		{
			return(false);
		}

		Library_SpriteStudio.Control.PartsEffectEmitter.FlagBitStatus Status = PoolParts.PoolEmitter[IDParts].Status;
		if(0 == (Status & (Library_SpriteStudio.Control.PartsEffectEmitter.FlagBitStatus.VALID | Library_SpriteStudio.Control.PartsEffectEmitter.FlagBitStatus.RUNNING)))
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

		PoolParts.PoolEmitter[IDParts].IndexCellMapOverwrite = IndexCellMap;
		PoolParts.PoolEmitter[IDParts].IndexCellOverwrite = IndexCell;
		Status = (false == FlagIgnoreAttributeCell) ? 
					(Status & ~Library_SpriteStudio.Control.PartsEffectEmitter.FlagBitStatus.OVERWRITE_CELL_IGNOREATTRIBUTE) :
					(Status | Library_SpriteStudio.Control.PartsEffectEmitter.FlagBitStatus.OVERWRITE_CELL_IGNOREATTRIBUTE);
		Status |= Library_SpriteStudio.Control.PartsEffectEmitter.FlagBitStatus.OVERWRITE_CELL_UNREFLECTED;
		PoolParts.PoolEmitter[IDParts].Status = Status;
		return(true);

	CellChange_Clear:;
		PoolParts.PoolEmitter[IDParts].IndexCellMapOverwrite = -1;
		PoolParts.PoolEmitter[IDParts].IndexCellOverwrite = -1;
		Status &= ~Library_SpriteStudio.Control.PartsEffectEmitter.FlagBitStatus.OVERWRITE_CELL_IGNOREATTRIBUTE;
		Status |= Library_SpriteStudio.Control.PartsEffectEmitter.FlagBitStatus.OVERWRITE_CELL_UNREFLECTED;
		PoolParts.PoolEmitter[IDParts].Status = Status;
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
	This function must be called after "Start" and "Awake" are executed.
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
	Utility for processing this table is defined in "Library_SpriteStudio.Utility.TableCellChange" class.
	*/
	public bool CellMapChange(Library_SpriteStudio.Control.CellChange[][] InstanceTableCellChange)
	{
		TableCellChange = InstanceTableCellChange;
		Status |= FlagBitStatus.CELL_TABLECHANGED;

		return(true);
	}
}
