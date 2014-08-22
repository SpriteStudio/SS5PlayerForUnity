/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[System.Serializable]
public class Script_SpriteStudio_PartsInstance : Library_SpriteStudio.SpriteBase
{
	/* Variables & Propaties */
	public Library_SpriteStudio.AnimationData SpriteStudioData;
	public Script_SpriteStudio_PartsRoot ScriptRoot;

	int AnimationNo;

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
	}

	void Update()
	{
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
			SpriteStudioData.UpdateUserData(ScriptRoot.FrameNoNow, gameObject, ScriptRoot);

			/* Update Instance-Data */
			bool FlagValidInstanceData = SpriteStudioData.UpdateInstanceData(ScriptRoot.FrameNoNow, gameObject, ScriptRoot, this);

			/* Set Matrix for Transform (to the GameObject) */
			if((true == SpriteStudioData.UpdateGameObject(gameObject, ScriptRoot.FrameNoNow)) && (null != scriptPartsRootSub) && (true == FlagValidInstanceData))
			{	/* Show Instance */
				/* MEMO: "Instance"-Parts has no mesh */
				DataMeshInformation.DataMesh = null;
				DataMeshInformation.DataTransform = transform;
				DataMeshInformation.PartsInstance = this;
				SpriteStudioData.DrawEntryInstance(DataMeshInformation, ScriptRoot.FrameNoNow, ScriptRoot);
			}
		}
	}

	void LateUpdate()
	{
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
			scriptPartsRootSub.ArrayListMeshDraw.Clear();
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
		SpriteStudioData = new Library_SpriteStudio.AnimationData();
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
