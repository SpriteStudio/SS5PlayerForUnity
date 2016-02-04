/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;

[ExecuteInEditMode]
[System.Serializable]
public partial class Script_SpriteStudio_ManagerDraw : MonoBehaviour
{
	public Camera CameraTarget;
	internal Camera InstanceCamera = null;
	internal Transform InstanceTransform = null;

	public Library_SpriteStudio.ManagerDraw.KindDrawQueue KindRenderQueueBase;
	public int OffsetDrawQueue;
	internal MeshFilter InstanceMeshFilter = null;
	internal MeshRenderer InstanceMeshRenderer = null;

	internal Library_SpriteStudio.ManagerDraw.TerminalDrawObject ChainDrawObject = null;
	internal Library_SpriteStudio.ManagerDraw.TerminalClusterDrawParts ChainClusterDrawParts = null;

	private int IndexParameterMesh = -1;
	private Mesh[] InstanceMesh = null;

//	void Awake()
//	{
//	}

	void Start()
	{
		int CountMeshBuffer = (int)Library_SpriteStudio.ManagerDraw.Constant.MAX_MESHPARAMETERBUFFER;
		IndexParameterMesh = CountMeshBuffer - 1;
		IndexParameterMesh %= CountMeshBuffer;
		InstanceMesh = new Mesh[CountMeshBuffer];
		for(int i=0; i<CountMeshBuffer; i++)
		{
			InstanceMesh[i] = new Mesh();
			InstanceMesh[i].Clear();
		}

		InstanceTransform = transform;
	}

	void Update()
	{
		/* Camera Get */
		if(null == InstanceCamera)
		{
			InstanceCamera = (null == CameraTarget) ? Library_SpriteStudio.Utility.Parts.CameraGetParent(gameObject) : CameraTarget;
		}

		/* Essential-Components Get */
		if(null == InstanceMeshFilter)
		{
			InstanceMeshFilter = gameObject.GetComponent<MeshFilter>();
		}
		if(null == InstanceMeshRenderer)
		{
			InstanceMeshRenderer = gameObject.GetComponent<MeshRenderer>();
		}

		/* Draw-Object Buffer Create */
		if(null == ChainDrawObject)
		{
			ChainDrawObject = new Library_SpriteStudio.ManagerDraw.TerminalDrawObject();
		}
		ChainDrawObject.ChainCleanUp();
	}

	void LateUpdate()
	{
		/* Draw-Object Buffer Create */
		if(null != ChainDrawObject)
		{
			/* Draw-Object Sort */
			ChainDrawObject.DrawObjectSort(InstanceCamera);	/* InstanceCamera, InstanceView */

			/* Combined-Mesh Create */
			if((null != InstanceMeshFilter) && (null != InstanceMeshRenderer))
			{
				/* DrawParts-Cluster Create */
				if(null == ChainClusterDrawParts)
				{
					ChainClusterDrawParts = new Library_SpriteStudio.ManagerDraw.TerminalClusterDrawParts();
					ChainClusterDrawParts.BootUp(0);	/* ManagerDraw has No-Cluster for "Draw" */
				}
				ChainDrawObject.ClusterConstruct(ChainClusterDrawParts);

				/* Cluster-Chain Fix (Collect Sub-Clusters & Optimize) */
				Library_SpriteStudio.ManagerDraw.ClusterFix(ChainClusterDrawParts);

				/* Meshes Combine */
				Library_SpriteStudio.ManagerDraw.MeshCreate(	ChainClusterDrawParts,
																InstanceMesh[IndexParameterMesh],
																InstanceMeshRenderer,
																InstanceMeshFilter,
																InstanceTransform,
																InstanceCamera
															);
				IndexParameterMesh++;
				IndexParameterMesh %= (int)Library_SpriteStudio.ManagerDraw.Constant.MAX_MESHPARAMETERBUFFER;
				InstanceMesh[IndexParameterMesh].Clear();

				/* Draw-Object Clear */
//				ChainDrawObject.ChainCleanUp();
			}

			/* Draw-Object Clear */
			ChainDrawObject.ChainCleanUp();
		}
	}

	/* ******************************************************** */
	//! Draw-Object Set
	/*!
	@param InstanceRoot
		Draw-Object's Root
	@retval	Return-Value
		(None)

	Don't use this function. <br>
	(This function is for Internal-Processing)
	*/
	public void DrawSet(Library_SpriteStudio.Script.Root InstanceRoot)
	{
		if(null != ChainDrawObject)
		{
			/* MEMO: Sort execute after all Draw-Objects are set. */
			InstanceRoot.DrawObject.Data.InstanceRoot = InstanceRoot;
			ChainDrawObject.ChainAddForce(InstanceRoot.DrawObject);
		}
	}
}
