/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using System.Collections;

[System.Serializable]
[ExecuteInEditMode]
public class Script_SpriteStudio_PartsNULL : Library_SpriteStudio.PartsBase
{
	public Library_SpriteStudio.AnimationData SpriteStudioData;
	public Script_SpriteStudio_PartsRoot ScriptRoot;

	void Start()
	{
	}

	void Update()
	{
		/* Update User-CallBack */
		SpriteStudioData.UpdateUserData(ScriptRoot.FrameNoNow, gameObject, ScriptRoot);

		/* Set Matrix for Transform (to the GameObject) */
		/* MEMO: Return-Value is ignored, 'cause NULL-Node has no Meshes. */
		SpriteStudioData.UpdateGameObject(gameObject, ScriptRoot.FrameNoNow);
	}

	void LateUpdate()
	{
	}

	public void BootUpForce()
	{
		SpriteStudioData = new Library_SpriteStudio.AnimationData();
	}
}