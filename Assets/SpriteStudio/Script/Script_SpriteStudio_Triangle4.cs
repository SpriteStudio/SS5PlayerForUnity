/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[System.Serializable]
public class Script_SpriteStudio_Triangle4 : Library_SpriteStudio.SpriteBase
{
	/* Variables & Propaties */
	public Library_SpriteStudio.AnimationDataSprite SpriteStudioData;
	public Script_SpriteStudio_PartsRoot ScriptRoot;
	public bool FlagHideForce;

	/* Functions */
	void Start()
	{
		MeshCreate();
		DataMeshInformation = new Library_SpriteStudio.DrawManager.InformationMeshData();
	}

	void Update()
	{
		/* Boot-Check */
		if(null == dataMesh)
		{
			MeshCreate();
		}
		if(null == DataMeshInformation)
		{
			DataMeshInformation = new Library_SpriteStudio.DrawManager.InformationMeshData();
		}

		/* Update User-CallBack */
		SpriteStudioData.UpdateUserData(ScriptRoot.FrameNoNow, gameObject, ScriptRoot);

		/* Mesh-Data Update */
		SpriteStudioData.UpdateMesh(dataMesh, ScriptRoot.FrameNoNow, ScriptRoot);

		/* Set Matrix for Transform (to the GameObject) */
		if((true == SpriteStudioData.UpdateGameObject(gameObject, ScriptRoot.FrameNoNow, true)) && (false == FlagHideForce))
		{	/* Show the Sprite */
			DataMeshInformation.DataMesh = dataMesh;
			DataMeshInformation.DataTransform = transform;
			DataMeshInformation.PartsInstance = null;
			SpriteStudioData.DrawEntry(DataMeshInformation, ScriptRoot.FrameNoNow, ScriptRoot);
		}
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
	public void HideSetForce(bool FlagSwitch, bool FlagSetChild=true, bool FlagSetInstance=false)
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
	(This function is for the Importer in Editor)
	*/
	public void BootUpForce()
	{
		SpriteStudioData = new Library_SpriteStudio.AnimationDataSprite();
	}

	private void MeshCreate()
	{
		/* Create Mesh */
		dataMesh = new Mesh();
		dataMesh.Clear();

		/* Create Vertex-Datas */
		/* MEMO: Create "vertices"-Datas for Deciding "vertexCount" */
		Vector3[] CoordinateVertex = new Vector3[(int)Library_SpriteStudio.VertexNo.TERMINATOR4];

		dataMesh.vertices = CoordinateVertex;
		dataMesh.triangles = Library_SpriteStudio.ArrayVertexIndex_Triangle4;
		dataMesh.normals = Library_SpriteStudio.ArrayNormal_Triangle4;
	}
}