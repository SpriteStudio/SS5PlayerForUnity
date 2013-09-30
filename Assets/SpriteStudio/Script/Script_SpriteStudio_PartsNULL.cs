/**
	SpriteStudio5 Player for Unity

	Copyright(C) 2003-2013 Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using System.Collections;

[System.Serializable]
[ExecuteInEditMode]
public class Script_SpriteStudio_PartsNULL : Library_SpriteStudio.PartsBase
{
	public Library_SpriteStudio.SpriteData SpriteStudioData;

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

	public void BootUpForce()
	{
		SpriteStudioData = new Library_SpriteStudio.SpriteData();
		SpriteStudioData.BootUp();
	}

	protected void AppendExecStart()
	{
		SpriteStudioData.BootUp();
		SpriteStudioData.PartsSetParent(Library_SpriteStudio.Utility.SpriteStudioDataGetParent(gameObject));
		SpriteStudioData.PartsSetRoot(Library_SpriteStudio.Utility.SpriteStudioDataGetRoot(gameObject));
	}

	protected void AppendExecUpdate()
	{
		if(null != SpriteStudioData)
		{
			SpriteStudioData.AnimationUpdate(gameObject);
			SpriteStudioData.AnimationFixTransform(transform);
		}
	}

	protected void AppendExecLateUpdate()
	{
	}

}