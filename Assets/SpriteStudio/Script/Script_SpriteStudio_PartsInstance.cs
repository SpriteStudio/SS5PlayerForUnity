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

	private Script_SpriteStudio_PartsRoot scriptRootInstance;
	public Script_SpriteStudio_PartsRoot ScriptRootInstance
	{
		get
		{
			return(scriptRootInstance);
		}
		set
		{
			scriptRootInstance = value;
		}
	}

	/* Functions */
	void Start()
	{
		/* MEMO: "Instance"-Parts has no mesh */
		DataMesh = null;
		DataMeshInformation = new Library_SpriteStudio.DrawManager.InformationMeshData();
	}

	void Update()
	{
		/* Boot-Check */
		if(null == DataMeshInformation)
		{
			DataMeshInformation = new Library_SpriteStudio.DrawManager.InformationMeshData();
		}

		/* Update User-CallBack */
		SpriteStudioData.UpdateUserData(ScriptRoot.FrameNoNow, gameObject, ScriptRoot);

		/* Update Instance-Data */
		SpriteStudioData.UpdateInstanceData(ScriptRoot.FrameNoNow, gameObject, ScriptRoot, this);

		/* Set Matrix for Transform (to the GameObject) */
		if((true == SpriteStudioData.UpdateGameObject(gameObject, ScriptRoot.FrameNoNow)) && (null != scriptRootInstance))
		{	/* Show Instance */
			/* MEMO: "Instance"-Parts has no mesh */
			DataMeshInformation.DataMesh = null;
			DataMeshInformation.DataTransform = transform;
			DataMeshInformation.PartsInstance = this;
			SpriteStudioData.DrawEntryInstance(DataMeshInformation, ScriptRoot.FrameNoNow, ScriptRoot);
		}
	}

	void LateUpdate()
	{
	}

	public void BootUpForce()
	{
		SpriteStudioData = new Library_SpriteStudio.AnimationData();
	}
}
