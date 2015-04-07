/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[System.Serializable]
public class Script_SpriteStudio_PartsInstance : Library_SpriteStudio.SpriteBase
{
	/* Variables & Propaties */
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

	public Collider CollisionComponent;
	private Library_SpriteStudio.AnimationData.WorkAreaRuntime WorkArea = null;

	public int ID;
	public Script_SpriteStudio_PartsRoot ScriptRoot;
	public bool FlagHideForce;

	public int AnimationNo;

	private GameObject InstanceGameObjectPartsRootSub;
	private Script_SpriteStudio_PartsRoot scriptPartsRootSub;
	public Script_SpriteStudio_PartsRoot ScriptPartsRootSub
	{
		get
		{
			return(scriptPartsRootSub);
		}
	}

	private int frameNoPreviousUpdate;
	internal int FrameNoPreviousUpdate
	{
		get
		{
			return(frameNoPreviousUpdate);
		}
		set
		{
			frameNoPreviousUpdate = value;
		}
	}

	/* Functions */
	void Start()
	{
		/* MEMO: "Instance"-Parts has no mesh */
		/* MEMO: "Instance"-Parts's Instance is created with "Script_SpriteStudio_LinkPrefab.cs"  */
		DataMesh = null;
		DataMeshInformation = new Library_SpriteStudio.DrawManager.InformationMeshData();

		frameNoPreviousUpdate = -1;

		/* Get Animation-Data-Referenced */
		if((null != ScriptRoot.SpriteStudioDataReferenced) && (null == spriteStudioData))
		{
			spriteStudioData = ScriptRoot.SpriteStudioDataReferenced.DataGetNode(ID);
		}
	}

	void Update()
	{
		/* Get Animation-Data-Referenced */
		if((null != ScriptRoot.SpriteStudioDataReferenced) && (null == spriteStudioData))
		{
			spriteStudioData = ScriptRoot.SpriteStudioDataReferenced.DataGetNode(ID);
		}

		/* Boot-Check */
		if(null == WorkArea)
		{
			WorkArea = new Library_SpriteStudio.AnimationData.WorkAreaRuntime();
		}

		/* Clear Instance-Part's Draw-List */
		DrawListClearInstance();

		/* Boot-Check */
		if(null == DataMeshInformation)
		{
			DataMeshInformation = new Library_SpriteStudio.DrawManager.InformationMeshData();
		}
		if(null == InstanceGameObjectPartsRootSub)
		{	/* Get "Instance"-Parts" */
			GameObject InstanceChild = null;
			Transform InstanceTransformChild = null;
			Script_SpriteStudio_PartsRoot InstancePartsRootSub = null;
			int CountChild = transform.childCount;
			for(int i=0; i<CountChild; i++)
			{
				InstanceTransformChild = transform.GetChild(i);
				if(null != InstanceTransformChild)
				{
					InstanceChild = InstanceTransformChild.gameObject;
					InstancePartsRootSub = InstanceChild.gameObject.GetComponent<Script_SpriteStudio_PartsRoot>();
					if(null != InstancePartsRootSub)
					{
						InstanceGameObjectPartsRootSub = InstanceChild;
						scriptPartsRootSub = InstancePartsRootSub;
						scriptPartsRootSub.PartsRootOrigin = PartsRootGetOrigin(ScriptRoot);
						break;	/* Exit for-Loop */
					}
				}
			}
		}

		if(null != InstanceGameObjectPartsRootSub)
		{
			/* Update User-CallBack */
			spriteStudioData.UpdateUserData(ScriptRoot.FrameNoNow, gameObject, ScriptRoot);

			/* Update Instance-Data */
			bool FlagValidInstanceData = spriteStudioData.UpdateInstanceData(ScriptRoot.FrameNoNow, gameObject, ScriptRoot, this);

			/* Set Matrix for Transform (to the GameObject) */
			if((true == spriteStudioData.UpdateGameObject(gameObject, ScriptRoot.FrameNoNow, CollisionComponent, WorkArea)) && (null != scriptPartsRootSub) && (true == FlagValidInstanceData) && (false == FlagHideForce))
			{	/* Show Instance */
				/* MEMO: "Instance"-Parts has no mesh */
				DataMeshInformation.DataMesh = null;
				DataMeshInformation.DataTransform = transform;
				DataMeshInformation.PartsInstance = this;
				spriteStudioData.DrawEntryInstance(DataMeshInformation, ScriptRoot.FrameNoNow, ScriptRoot);
			}
		}

		if(null != scriptPartsRootSub)
		{
			Script_SpriteStudio_PartsRoot PartsOrigin = scriptPartsRootSub.PartsRootOrigin;
			if(null != PartsOrigin)
			{
				if(false == PartsOrigin.AnimationCheckPlay())
				{	/* Parent is stopped */
					scriptPartsRootSub.AnimationStop();
				}
			}
		}
	}

	void LateUpdate()
	{
	}

	/* ******************************************************** */
	//! Change playing the Instance-Object's animation
	/*!
	@param	No
		Animation's Index<br>
	@retval	Return-Value
		true == Success <br>
		false == Failure (Error)

	The playing of Instance-Object's animation changes. <br>
	It is necessary for the New Animation to have the Animation which is already set and compatibility of a data structure.<br>
	<br>
	Caution!: This Function is in experimentally inplement.
	*/
	public bool AnimationChangeInstance(int No)
	{
		AnimationNo = No;
		frameNoPreviousUpdate = -1;
		return(true);
	}

	/* ******************************************************** */
	//! Change playing the Instance-Object's animation
	/*!
	@param	InstancePrefab
		New Prefab<br>
	@retval	Return-Value
		true == Success <br>
		false == Failure (Error)

	The playing of Instance-Object's Prefab changes. <br>
	It is necessary for the New Prefab to have the Prefab which is already set and compatibility of a data structure.<br>
	<br>
	Caution!: This Function is in experimentally inplement.
	*/
	public bool PrefabChangeInstance(GameObject PrefabNew)
	{
		GameObject Instance = null;
#if UNITY_EDITOR
		DestroyImmediate(InstanceGameObjectPartsRootSub);
		Instance = (GameObject)PrefabUtility.InstantiatePrefab(PrefabNew);
#else
		Destroy(InstanceGameObjectPartsRootSub);
		Instance = (GameObject)Instantiate(PrefabNew);
#endif
		InstanceGameObjectPartsRootSub = null;
		scriptPartsRootSub = null;
		LinkSetPartsInstance(Instance);
		return(true);
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
	//! Draw-List Clear
	/*!
	@param	InstanceGameObjectSub
		Instance-Data's Root-Parts
	@retval	Return-Value
		(None)

	Don't use this function. <br>
	(This function is for the Link-Prefab's scripts in same GameObject.)
	*/
	internal void LinkSetPartsInstance(GameObject InstanceGameObjectSub)
	{
		InstanceGameObjectPartsRootSub = InstanceGameObjectSub;
		scriptPartsRootSub = InstanceGameObjectSub.GetComponent<Script_SpriteStudio_PartsRoot>();
		scriptPartsRootSub.PartsRootOrigin = PartsRootGetOrigin(ScriptRoot);
		scriptPartsRootSub.NodeSetControl(null);
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
	internal void DrawListClearInstance()
	{
		if(null != scriptPartsRootSub)
		{
			Library_SpriteStudio.DrawManager.ArrayListMeshDraw ArrayListMesh = scriptPartsRootSub.ArrayListMeshDraw;
			if(null != ArrayListMesh)
			{
				scriptPartsRootSub.ArrayListMeshDraw.Clear();
			}
		}
	}

	/* ******************************************************** */
	//! Force Boot-Up
	/*!
	@param
		(None)
	@retval	Return-Value
		(None)

	Don't use this function. <br>
	(This function is for the Importer in Editor.)
	*/
	public void BootUpForce()
	{
		spriteStudioData = new Library_SpriteStudio.AnimationData();
		InstanceGameObjectPartsRootSub = null;
	}

	private Script_SpriteStudio_PartsRoot PartsRootGetOrigin(Script_SpriteStudio_PartsRoot Start)
	{
		Script_SpriteStudio_PartsRoot PartsRootNow = Start;
		while(null != PartsRootNow.PartsRootOrigin)
		{
			PartsRootNow = PartsRootNow.PartsRootOrigin;
		}
		return(PartsRootNow);
	}
}
