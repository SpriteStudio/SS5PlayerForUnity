/**
	SpriteStudio5 Player for Unity

	Copyright(C) 2003-2013 Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using System.Collections;

[System.Serializable]
[ExecuteInEditMode]
public class Script_SpriteStudio_Triangle4 : Library_SpriteStudio.SpriteBase
{
	public Library_SpriteStudio.SpriteData SpriteStudioData;

	[SerializeField]
	private Vector2 vertexCoordinateLU;
	public Vector2 VertexCoordinateLU
	{
		set
		{
			Status |= ((value.x != vertexCoordinateLU.x) || (value.y != vertexCoordinateLU.y))  ?
				Library_SpriteStudio.SpriteBase.BitStatus.UPDATE_COORDINATE : Library_SpriteStudio.SpriteBase.BitStatus.CLEAR;
			vertexCoordinateLU = value;
		}
		get
		{
			return(vertexCoordinateLU);
		}
	}
	[SerializeField]
	private Vector2 vertexCoordinateRU;
	public Vector2 VertexCoordinateRU
	{
		set
		{
			Status |= ((value.x != vertexCoordinateRU.x) || (value.y != vertexCoordinateRU.y)) ?
				Library_SpriteStudio.SpriteBase.BitStatus.UPDATE_COORDINATE : Library_SpriteStudio.SpriteBase.BitStatus.CLEAR;
			vertexCoordinateRU = value;
		}
		get
		{
			return(vertexCoordinateRU);
		}
	}
	[SerializeField]
	private Vector2 vertexCoordinateLD;
	public Vector2 VertexCoordinateLD
	{
		set
		{
			Status |= ((value.x != vertexCoordinateLD.x) || (value.y != vertexCoordinateLD.y)) ?
				Library_SpriteStudio.SpriteBase.BitStatus.UPDATE_COORDINATE : Library_SpriteStudio.SpriteBase.BitStatus.CLEAR;
			vertexCoordinateLD = value;
		}
		get
		{
			return(vertexCoordinateLD);
		}
	}
	[SerializeField]
	private Vector2 vertexCoordinateRD;
	public Vector2 VertexCoordinateRD
	{
		set
		{
			Status |= ((value.x != vertexCoordinateRD.x) || (value.y != vertexCoordinateRD.y)) ?
				Library_SpriteStudio.SpriteBase.BitStatus.UPDATE_COORDINATE : Library_SpriteStudio.SpriteBase.BitStatus.CLEAR;
			vertexCoordinateRD = value;
		}
		get
		{
			return(vertexCoordinateRD);
		}
	}

	private int[] VertexArrayIndex =
	{
		-1,
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

			Vector2 CoordinateLU;
			Vector2 CoordinateRU;
			Vector2 CoordinateLD;
			Vector2 CoordinateRD;
			SpriteStudioData.AnimationGetPlaneTriangle4(out CoordinateLU, out CoordinateRU, out CoordinateLD, out CoordinateRD, out Pivot, ref AreaTexture);
			VertexCoordinateLU = CoordinateLU;
			VertexCoordinateRU = CoordinateRU;
			VertexCoordinateLD = CoordinateLD;
			VertexCoordinateRD = CoordinateRD;
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
				Vector3[] VertexUV0 = (0 != (Status & BitStatus.UPDATE_MAPPING)) ? Library_SpriteStudio.ArrayUVMappingUV0_Triangle4 : null;
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
		for(int i=(int)Library_SpriteStudio.VertexNo.LU; i<(int)Library_SpriteStudio.VertexNo.TERMINATOR4; i++)
		{
			VertexArrayIndex[i] = i;
		}

		dataMesh = new Mesh();
		dataMesh.name = "BaseMesh4";
		MeshModify(	dataMesh,
					InstanceTexture,
					ArrayCreateCoordinateVertex(),
					Library_SpriteStudio.ArrayUVMappingUV0_Triangle4,
					ArrayCreateUV2Vertex(),
					ArrayCreateColorVertex(),
					null,
					Library_SpriteStudio.ArrayVertexIndex_Triangle4,
					true,
					false
				);
	}

	private Vector3[] ArrayCreateCoordinateVertex()
	{
		Vector2 VertexCoordinateC;
		Vector2 CoordinateLURU = (vertexCoordinateLU + vertexCoordinateRU) * 0.5f;
		Vector2 CoordinateLULD = (vertexCoordinateLU + vertexCoordinateLD) * 0.5f;
		Vector2 CoordinateLDRD = (vertexCoordinateLD + vertexCoordinateRD) * 0.5f;
		Vector2 CoordinateRURD = (vertexCoordinateRU + vertexCoordinateRD) * 0.5f;
		Library_SpriteStudio.Utility.CoordinateGetDiagonalIntersection(	out VertexCoordinateC,
																		ref CoordinateLURU,
																		ref CoordinateRURD,
																		ref CoordinateLULD,
																		ref CoordinateLDRD
																	);

		int	VertexNo = -1;
		Vector3[] PositionVertex = new Vector3[(int)Library_SpriteStudio.VertexNo.TERMINATOR4];

		VertexNo = (int)Library_SpriteStudio.VertexNo.LU;
		PositionVertex[VertexArrayIndex[VertexNo]] = new Vector3(vertexCoordinateLU.x - planePivot.x, vertexCoordinateLU.y + planePivot.y, 0.0f);

		VertexNo = (int)Library_SpriteStudio.VertexNo.RU;
		PositionVertex[VertexArrayIndex[VertexNo]] = new Vector3(vertexCoordinateRU.x - planePivot.x, vertexCoordinateRU.y + planePivot.y, 0.0f);

		VertexNo = (int)Library_SpriteStudio.VertexNo.RD;
		PositionVertex[VertexArrayIndex[VertexNo]] = new Vector3(vertexCoordinateRD.x - planePivot.x, vertexCoordinateRD.y + planePivot.y, 0.0f);

		VertexNo = (int)Library_SpriteStudio.VertexNo.LD;
		PositionVertex[VertexArrayIndex[VertexNo]] = new Vector3(vertexCoordinateLD.x - planePivot.x, vertexCoordinateLD.y + planePivot.y, 0.0f);

		VertexNo = (int)Library_SpriteStudio.VertexNo.C;
		PositionVertex[VertexArrayIndex[VertexNo]] = new Vector3(VertexCoordinateC.x - planePivot.x, VertexCoordinateC.y + planePivot.y, 0.0f);

		return(PositionVertex);
	}

	private Color32[] ArrayCreateColorVertex()
	{
		Color ColorLU = VertexColorLU;
		Color ColorRU = VertexColorRU;
		Color ColorLD = VertexColorLD;
		Color ColorRD = VertexColorRD;
		Color ColorC = (ColorLU + ColorRU + ColorLD + ColorRD) * 0.25f;

		int	VertexNo = -1;
		Color32[] ColorVertex = new Color32[(int)Library_SpriteStudio.VertexNo.TERMINATOR4];

		VertexNo = (int)Library_SpriteStudio.VertexNo.LU;
		ColorVertex[VertexArrayIndex[VertexNo]] = VertexColorLU;

		VertexNo = (int)Library_SpriteStudio.VertexNo.RU;
		ColorVertex[VertexArrayIndex[VertexNo]] = VertexColorRU;

		VertexNo = (int)Library_SpriteStudio.VertexNo.LD;
		ColorVertex[VertexArrayIndex[VertexNo]] = VertexColorLD;

		VertexNo = (int)Library_SpriteStudio.VertexNo.RD;
		ColorVertex[VertexArrayIndex[VertexNo]] = VertexColorRD;

		VertexNo = (int)Library_SpriteStudio.VertexNo.C;
		ColorVertex[VertexArrayIndex[VertexNo]] = ColorC;

		return(ColorVertex);
	}

	private Vector2[] ArrayCreateUV2Vertex()
	{
		Vector2 parameterEffectC = Vector2.zero;
		parameterEffectC = (parameterEffectLU + parameterEffectRU + parameterEffectLD + parameterEffectRD) * 0.25f;

		int	VertexNo = -1;
		Vector2[] UV2Vertex = new Vector2[(int)Library_SpriteStudio.VertexNo.TERMINATOR4];

		VertexNo = (int)Library_SpriteStudio.VertexNo.LU;
		UV2Vertex[VertexArrayIndex[VertexNo]] = parameterEffectLU;

		VertexNo = (int)Library_SpriteStudio.VertexNo.RU;
		UV2Vertex[VertexArrayIndex[VertexNo]] = parameterEffectRU;

		VertexNo = (int)Library_SpriteStudio.VertexNo.LD;
		UV2Vertex[VertexArrayIndex[VertexNo]] = parameterEffectLD;

		VertexNo = (int)Library_SpriteStudio.VertexNo.RD;
		UV2Vertex[VertexArrayIndex[VertexNo]] = parameterEffectRD;

		VertexNo = (int)Library_SpriteStudio.VertexNo.C;
		UV2Vertex[VertexArrayIndex[VertexNo]] = parameterEffectC;

		return(UV2Vertex);
	}

	private void MeshAnalyzeVertexNo(Mesh InstanceMesh)
	{
		int	VertexNo = -1;
		Vector2 Coordinate = Vector2.zero;

		VertexNo = (int)Library_SpriteStudio.VertexNo.LU;
		if(-1 == VertexArrayIndex[VertexNo])
		{
			Coordinate = Library_SpriteStudio.ArrayUVMappingUV0_Triangle4[VertexNo];
			VertexArrayIndex[VertexNo] = Library_SpriteStudio.Utility.VertexNoSearchMappingUV(InstanceMesh, ref Coordinate);
		}

		VertexNo = (int)Library_SpriteStudio.VertexNo.RU;
		if(-1 == VertexArrayIndex[VertexNo])
		{
			Coordinate = Library_SpriteStudio.ArrayUVMappingUV0_Triangle4[VertexNo];
			VertexArrayIndex[VertexNo] = Library_SpriteStudio.Utility.VertexNoSearchMappingUV(InstanceMesh, ref Coordinate);
		}

		VertexNo = (int)Library_SpriteStudio.VertexNo.LD;
		if(-1 == VertexArrayIndex[VertexNo])
		{
			Coordinate = Library_SpriteStudio.ArrayUVMappingUV0_Triangle4[VertexNo];
			VertexArrayIndex[VertexNo] = Library_SpriteStudio.Utility.VertexNoSearchMappingUV(InstanceMesh, ref Coordinate);
		}

		VertexNo = (int)Library_SpriteStudio.VertexNo.RD;
		if(-1 == VertexArrayIndex[VertexNo])
		{
			Coordinate = Library_SpriteStudio.ArrayUVMappingUV0_Triangle4[VertexNo];
			VertexArrayIndex[VertexNo] = Library_SpriteStudio.Utility.VertexNoSearchMappingUV(InstanceMesh, ref Coordinate);
		}

		VertexNo = (int)Library_SpriteStudio.VertexNo.C;
		if(-1 == VertexArrayIndex[VertexNo])
		{
			Coordinate = Library_SpriteStudio.ArrayUVMappingUV0_Triangle4[VertexNo];
			VertexArrayIndex[VertexNo] = Library_SpriteStudio.Utility.VertexNoSearchMappingUV(InstanceMesh, ref Coordinate);
		}
	}
}