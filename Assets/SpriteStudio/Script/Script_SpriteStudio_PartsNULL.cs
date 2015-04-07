/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[System.Serializable]
public class Script_SpriteStudio_PartsNULL : Library_SpriteStudio.PartsBase
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

	void Start()
	{
		/* Get Animation-Data-Referenced */
		if(null != ScriptRoot.SpriteStudioDataReferenced)
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

		/* Update User-CallBack */
		spriteStudioData.UpdateUserData(ScriptRoot.FrameNoNow, gameObject, ScriptRoot);

		/* Set Matrix for Transform (to the GameObject) */
		/* MEMO: Return-Value is ignored, 'cause NULL-Node has no Meshes. */
		spriteStudioData.UpdateGameObject(gameObject, ScriptRoot.FrameNoNow, CollisionComponent, WorkArea);
	}

	void LateUpdate()
	{
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
	}
}