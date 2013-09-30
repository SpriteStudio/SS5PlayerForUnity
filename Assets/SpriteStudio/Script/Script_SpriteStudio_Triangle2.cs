/**
	SpriteStudio5 Player for Unity

	Copyright(C) 2003-2013 Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using System.Collections;

[System.Serializable]
[ExecuteInEditMode]
public class Script_SpriteStudio_Triangle2 : Library_SpriteStudio.SpriteBase
{
	public Library_SpriteStudio.SpriteData SpriteStudioData;

	[SerializeField]
	protected Vector2 planeSize;
	public Vector2 PlaneSize
	{
		set
		{
			Status |= ((value.x != planeSize.x) || (value.y != planeSize.y)) ?
				Library_SpriteStudio.SpriteBase.BitStatus.UPDATE_COORDINATE : Library_SpriteStudio.SpriteBase.BitStatus.CLEAR;
			planeSize = value;
		}
		get
		{
			return(planeSize);
		}
	}

	private int[] VertexArrayIndex =
	{
		-1,
		-1,
		-1,
		-1
	};

	private Script_SpriteStudio_PartsRoot.InformationMeshData DataMeshInformation = null;

	void Start()
	{
		BaseExecStart();

		MeshCreateBase(null);
		Status |= Library_SpriteStudio.SpriteBase.BitStatus.UPDATE_MAPPING;
		Status |= Library_SpriteStudio.SpriteBase.BitStatus.UPDATE_EFFECT;

		StatusSetUseMeshRenderer(false);

		DataMaterials = new Material[1];

		SpriteStudioData.BootUp();
		SpriteStudioData.PartsSetParent(Library_SpriteStudio.Utility.SpriteStudioDataGetParent(gameObject));
		SpriteStudioData.PartsSetRoot(Library_SpriteStudio.Utility.SpriteStudioDataGetRoot(gameObject));

		SpriteStudioData.AnimationFixCell(this);
	}

	void Update()
	{
		if(false == StatusGetRunning())
		{
			return;
		}

		if(null != SpriteStudioData)
		{
			SpriteStudioData.AnimationUpdate(gameObject);
			SpriteStudioData.AnimationFixSpriteCommon(transform, this);

			Rect AreaTexture;
			Vector2 Pivot;
			Vector2 Scale;
			float Rotate;
			SpriteStudioData.AnimationGetTextureMapping(out AreaTexture, out Scale, out Rotate, out Pivot);
			TextureArea = AreaTexture;
			TexturePivot = Pivot;
			TextureScale = Scale;
			TextureRotate = Rotate;

			Vector2 SizePlane;
			SpriteStudioData.AnimationGetPlaneTriangle2(out SizePlane, out Pivot, ref AreaTexture);
			PlaneSize = SizePlane;
			PlanePivot = Pivot;

			bool FlagMaterialChange = UpdateExecMaterial();
			Texture InstanceTexture = null;
			InstanceTexture = (null != dataMaterials[0]) ? dataMaterials[0].mainTexture : null;

			if((true == FlagMaterialChange) && (null != InstanceTexture))
			{
				ParameterRenewTexture(ref textureArea, ref texturePivot, InstanceTexture);
			}

			if(null == dataMesh)
			{
				MeshCreateBase(InstanceTexture);
			}
			else
			{
				Vector3[] VertexCoordinate = (0 != (Status & BitStatus.UPDATE_COORDINATE)) ? ArrayCreateCoordinateVertex() : null;
				Color32[] VertexColor = (0 != (Status & BitStatus.UPDATE_COLOR)) ? ArrayCreateColorVertex() : null;
				Vector3[] VertexUV0 = (0 != (Status & BitStatus.UPDATE_MAPPING)) ? Library_SpriteStudio.ArrayUVMappingUV0_Triangle2 : null;
				Vector2[] VertexUV1 = (0 != (Status & BitStatus.UPDATE_EFFECT)) ? ArrayCreateUV2Vertex() : null;
				MeshModify(	dataMesh,
							InstanceTexture,
							VertexCoordinate,
							VertexUV0,
							VertexUV1,
							VertexColor,
							null,
							null,
							true,
							false
						);
			}

			BaseExecUpdate();

			if(true == StatusGetRendering())
			{
				if(null == DataMeshInformation)
				{
					DataMeshInformation = new Script_SpriteStudio_PartsRoot.InformationMeshData();
				}
				/* Caution: Don't Set ".Priority" and ".ChainNext" */
				DataMeshInformation.DataMesh = dataMesh;
				DataMeshInformation.DataTransform = transform;
				SpriteStudioData.DrawEntry(ref DataMeshInformation);
			}
		}
	}

	void LateUpdate()
	{
	}

	public void BootUpForce()
	{
		SpriteStudioData = new Library_SpriteStudio.SpriteData();
		SpriteStudioData.BootUp();
	}

	private void MeshCreateBase(Texture InstanceTexture)
	{
		for(int i=(int)Library_SpriteStudio.VertexNo.LU; i<(int)Library_SpriteStudio.VertexNo.TERMINATOR2; i++)
		{
			VertexArrayIndex[i] = i;
		}

		dataMesh = new Mesh();
		dataMesh.name = "BaseMesh2";
		MeshModify(	dataMesh,
					InstanceTexture,
					ArrayCreateCoordinateVertex(),
					Library_SpriteStudio.ArrayUVMappingUV0_Triangle2,
					ArrayCreateUV2Vertex(),
					ArrayCreateColorVertex(),
					null,
					Library_SpriteStudio.ArrayVertexIndex_Triangle2,
					true,
					true
				);
	}

	private Vector3[] ArrayCreateCoordinateVertex()
	{
		Vector2 Pivot  = PlanePivot;
		float Left = -Pivot.x;
		float Right = planeSize.x - Pivot.x;
		float Top = -Pivot.y;
		float Bottom = planeSize.y - Pivot.y;

		int	VertexNo = -1;
		Vector3[] PositionVertex = new Vector3[(int)Library_SpriteStudio.VertexNo.TERMINATOR2];

		VertexNo = (int)Library_SpriteStudio.VertexNo.LU;
		PositionVertex[VertexArrayIndex[VertexNo]] = new Vector3(Left, -Top, 0.0f);

		VertexNo = (int)Library_SpriteStudio.VertexNo.RU;
		PositionVertex[VertexArrayIndex[VertexNo]] = new Vector3(Right, -Top, 0.0f);

		VertexNo = (int)Library_SpriteStudio.VertexNo.RD;
		PositionVertex[VertexArrayIndex[VertexNo]] = new Vector3(Right, -Bottom, 0.0f);

		VertexNo = (int)Library_SpriteStudio.VertexNo.LD;
		PositionVertex[VertexArrayIndex[VertexNo]] = new Vector3(Left, -Bottom, 0.0f);

		return(PositionVertex);
	}

	private Color32[] ArrayCreateColorVertex()
	{
		int	VertexNo = -1;
		Color32[] ColorVertex = new Color32[(int)Library_SpriteStudio.VertexNo.TERMINATOR2];

		VertexNo = (int)Library_SpriteStudio.VertexNo.LU;
		ColorVertex[VertexArrayIndex[VertexNo]] = VertexColorLU;

		VertexNo = (int)Library_SpriteStudio.VertexNo.RU;
		ColorVertex[VertexArrayIndex[VertexNo]] = VertexColorRU;

		VertexNo = (int)Library_SpriteStudio.VertexNo.LD;
		ColorVertex[VertexArrayIndex[VertexNo]] = VertexColorLD;

		VertexNo = (int)Library_SpriteStudio.VertexNo.RD;
		ColorVertex[VertexArrayIndex[VertexNo]] = VertexColorRD;

		return(ColorVertex);
	}

	private Vector2[] ArrayCreateUV2Vertex()
	{
		int	VertexNo = -1;
		Vector2[] UV2Vertex = new Vector2[(int)Library_SpriteStudio.VertexNo.TERMINATOR2];

		VertexNo = (int)Library_SpriteStudio.VertexNo.LU;
		UV2Vertex[VertexArrayIndex[VertexNo]] = parameterEffectLU;

		VertexNo = (int)Library_SpriteStudio.VertexNo.RU;
		UV2Vertex[VertexArrayIndex[VertexNo]] = parameterEffectRU;

		VertexNo = (int)Library_SpriteStudio.VertexNo.LD;
		UV2Vertex[VertexArrayIndex[VertexNo]] = parameterEffectLD;

		VertexNo = (int)Library_SpriteStudio.VertexNo.RD;
		UV2Vertex[VertexArrayIndex[VertexNo]] = parameterEffectRD;

		return(UV2Vertex);
	}

	private void MeshAnalyzeVertexNo(Mesh InstanceMesh)
	{
		int	VertexNo = -1;
		Vector2 Coordinate = Vector2.zero;

		VertexNo = (int)Library_SpriteStudio.VertexNo.LU;
		if(-1 == VertexArrayIndex[VertexNo])
		{
			Coordinate = Library_SpriteStudio.ArrayUVMappingUV0_Triangle2[VertexNo];
			VertexArrayIndex[VertexNo] = Library_SpriteStudio.Utility.VertexNoSearchMappingUV(InstanceMesh, ref Coordinate);
		}

		VertexNo = (int)Library_SpriteStudio.VertexNo.RU;
		if(-1 == VertexArrayIndex[VertexNo])
		{
			Coordinate = Library_SpriteStudio.ArrayUVMappingUV0_Triangle2[VertexNo];
			VertexArrayIndex[VertexNo] = Library_SpriteStudio.Utility.VertexNoSearchMappingUV(InstanceMesh, ref Coordinate);
		}

		VertexNo = (int)Library_SpriteStudio.VertexNo.LD;
		if(-1 == VertexArrayIndex[VertexNo])
		{
			Coordinate = Library_SpriteStudio.ArrayUVMappingUV0_Triangle2[VertexNo];
			VertexArrayIndex[VertexNo] = Library_SpriteStudio.Utility.VertexNoSearchMappingUV(InstanceMesh, ref Coordinate);
		}

		VertexNo = (int)Library_SpriteStudio.VertexNo.RD;
		if(-1 == VertexArrayIndex[VertexNo])
		{
			Coordinate = Library_SpriteStudio.ArrayUVMappingUV0_Triangle2[VertexNo];
			VertexArrayIndex[VertexNo] = Library_SpriteStudio.Utility.VertexNoSearchMappingUV(InstanceMesh, ref Coordinate);
		}
	}
}