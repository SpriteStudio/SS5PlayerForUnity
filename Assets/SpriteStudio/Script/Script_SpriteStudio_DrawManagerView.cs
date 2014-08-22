/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[System.Serializable]
public class Script_SpriteStudio_DrawManagerView : MonoBehaviour
{
	/* Classes */
	private class InformationDrawObject
	{
		public Script_SpriteStudio_PartsRoot PartsRoot = null;
		public float Priority = 0.0f;
	}

	/* Valiable & Propaties */
	private Camera InstanceCameraDraw;
	private ArrayList DrawEntryPartsRoot;
	private Library_SpriteStudio.DrawManager.ArrayListMeshDraw arrayListMeshDraw;
	public Library_SpriteStudio.DrawManager.ArrayListMeshDraw ArrayListMeshDraw
	{
		get
		{
			return(arrayListMeshDraw);
		}
	}

	/* Functions */
#if false
	void Awake()
	{
		Application.RegisterLogCallback(LogWarningHook);
	}
	void LogWarningHook(string logString, string stackTrace, LogType type)
	{
		string Text = logString;
	}
#endif
	
	void Start()
	{
		InstanceCameraDraw = Library_SpriteStudio.Utility.CameraGetParent(gameObject);

		DrawEntryPartsRoot = new ArrayList();
		DrawEntryPartsRoot.Clear();

		arrayListMeshDraw = new Library_SpriteStudio.DrawManager.ArrayListMeshDraw();
		arrayListMeshDraw.BootUp();
	}
	
	void Update()
	{
		/* Boot-Check */
		if(null == InstanceCameraDraw)
		{
			InstanceCameraDraw = Library_SpriteStudio.Utility.CameraGetParent(gameObject);
		}
		if(null == DrawEntryPartsRoot)
		{
			DrawEntryPartsRoot = new ArrayList();
		}
	}

	void LateUpdate()
	{
		/* Clear Finalize ListDrawMesh */
		if(null == arrayListMeshDraw)
		{
			arrayListMeshDraw = new Library_SpriteStudio.DrawManager.ArrayListMeshDraw();
			arrayListMeshDraw.BootUp();
		}
		arrayListMeshDraw.Clear();

		/* Collect Draw-Parts from Root-Parts */
		InformationDrawObject DrawObject = null;
		Library_SpriteStudio.DrawManager.ArrayListMeshDraw ArrayListMeshDrawObject = null;
		Library_SpriteStudio.DrawManager.ListMeshDraw ListMeshDraw = null;
		int Count = DrawEntryPartsRoot.Count;
		for(int i=0; i<Count; i++)
		{
			DrawObject = DrawEntryPartsRoot[i] as InformationDrawObject;
			ArrayListMeshDrawObject = DrawObject.PartsRoot.ArrayListMeshDraw;
			int CountList = ArrayListMeshDrawObject.TableListMesh.Count;
			for(int j=0; j<CountList; j++)
			{
				/* Add Mesh-List */
				ListMeshDraw = ArrayListMeshDrawObject.TableListMesh[j] as Library_SpriteStudio.DrawManager.ListMeshDraw;
				arrayListMeshDraw.TableListMesh.Add(ListMeshDraw);
			}

			/* Clear Original Draw-List */
			DrawObject.PartsRoot.DrawListClear();
		}
		DrawEntryPartsRoot.Clear();

		/* Collect & Add Draw-Parts "Instance"-Object's Parts */
		Library_SpriteStudio.DrawManager.InformationMeshData InformationMeshData = null;
		Script_SpriteStudio_PartsRoot ScriptPartsRootSub = null;
		for(int i=0; i<arrayListMeshDraw.TableListMesh.Count; )
		{
			ListMeshDraw = arrayListMeshDraw.TableListMesh[i] as Library_SpriteStudio.DrawManager.ListMeshDraw;
			InformationMeshData = ListMeshDraw.MeshDataTop;
			if(null != InformationMeshData.PartsInstance)
			{
				Library_SpriteStudio.DrawManager.ListMeshDraw ListMeshDrawSub = null;
				ScriptPartsRootSub = InformationMeshData.PartsInstance.ScriptPartsRootSub;
				if(null != ScriptPartsRootSub)
				{	/* Add Mesh-Table */
					/* Delete "Instance"-Data */
					arrayListMeshDraw.TableListMesh.RemoveAt(i);

					/* Insert "Instance"-Data's Draw-Mesh-List */
					ArrayListMeshDrawObject = ScriptPartsRootSub.ArrayListMeshDraw;
					for(int j=0; j<ArrayListMeshDrawObject.TableListMesh.Count; j++)
					{
						ListMeshDrawSub = ArrayListMeshDrawObject.TableListMesh[j] as Library_SpriteStudio.DrawManager.ListMeshDraw;
						arrayListMeshDraw.TableListMesh.Insert((i + j), ListMeshDrawSub);
					}

					/* Clear Original Draw-List */
					ScriptPartsRootSub.DrawListClear();
					continue;
				}
			}
			i++;
		}

		/* Optimize Draw-Parts List */
		Library_SpriteStudio.DrawManager.ListMeshDraw ListMeshDrawNext = null;
		ArrayList TableListMesh = arrayListMeshDraw.TableListMesh;
		for(int i=0; i<(TableListMesh.Count - 1); )
		{
			Count = i + 1;	/* "Count" is temporary */
			ListMeshDraw = TableListMesh[i] as Library_SpriteStudio.DrawManager.ListMeshDraw;
			ListMeshDrawNext = TableListMesh[Count] as Library_SpriteStudio.DrawManager.ListMeshDraw;
			if(ListMeshDraw.MaterialOriginal == ListMeshDrawNext.MaterialOriginal)
			{
				/* Mesh-List Merge */
				ListMeshDraw.ListMerge(ListMeshDrawNext);
				TableListMesh.RemoveAt(Count);

				/* Counter No-Incliment Continue */
				continue;
			}
			i++;
		}

		/* Counting Meshes */
		TableListMesh = arrayListMeshDraw.TableListMesh;
		Count = TableListMesh.Count;
		int CountMesh = 0;
		for(int i=0; i<Count; i++)
		{
			ListMeshDraw = TableListMesh[i] as Library_SpriteStudio.DrawManager.ListMeshDraw;
			CountMesh += ListMeshDraw.Count;
		}

		/* Meshes Combine each Material & Set to MeshFilter/MeshRenderer */
		MeshFilter InstanceMeshFilter = GetComponent<MeshFilter>();
		MeshRenderer InstanceMeshRenderer = GetComponent<MeshRenderer>();
		arrayListMeshDraw.MeshSetCombine(InstanceMeshFilter, InstanceMeshRenderer, InstanceCameraDraw, transform);
	}

	internal void DrawEntryObject(Script_SpriteStudio_PartsRoot PartsRootDrawObject)
	{
		/* Boot-Check */
		if(null == DrawEntryPartsRoot)
		{
			DrawEntryPartsRoot = new ArrayList();
			DrawEntryPartsRoot.Clear();
		}

		/* Get Priority */
		Matrix4x4 MatrixWorld = PartsRootDrawObject.transform.localToWorldMatrix;
		Vector3 OriginWorld = MatrixWorld.MultiplyPoint3x4(Vector3.zero);

		/* Add Draw-Object (Not "Instance"-Object) */
		InformationDrawObject DrawObjectNew = new InformationDrawObject();
		DrawObjectNew.PartsRoot = PartsRootDrawObject;
		DrawObjectNew.Priority = OriginWorld.z;

		/* Sort & Add Object */
		int Count = DrawEntryPartsRoot.Count;
		if(0 == Count)
		{	/* First Object */
			goto DrawEntryObject_Add;
		}

		InformationDrawObject DrawObject = null;
		int Index = 0;
		for(int i=0; i<Count; i++)
		{
			DrawObject = DrawEntryPartsRoot[i] as InformationDrawObject;
			if(DrawObject.Priority < DrawObjectNew.Priority)
			{
				goto DrawEntryObject_Insert;
			}
			Index++;
		}

	DrawEntryObject_Add:;
		DrawEntryPartsRoot.Add(DrawObjectNew);
		return;

	DrawEntryObject_Insert:;
		DrawEntryPartsRoot.Insert(Index, DrawObjectNew);
		return;
	}

	void OnDestroy()
	{	/* MEMO: to make sure ... */
		MeshFilter InstanceMeshFilter = GetComponent<MeshFilter>();
		if(null != InstanceMeshFilter)
		{
			if(null != InstanceMeshFilter.sharedMesh)
			{
				InstanceMeshFilter.sharedMesh.Clear();
				Object.DestroyImmediate(InstanceMeshFilter.sharedMesh);
			}
			InstanceMeshFilter.sharedMesh = null;
		}
	}
}
