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
		Library_SpriteStudio.DrawManager.ArrayListMeshDraw ArrayListMeshDraw = null;
		Library_SpriteStudio.DrawManager.ListMeshDraw ListMeshDraw = null;
		int Count = DrawEntryPartsRoot.Count;
		for(int i=0; i<Count; i++)
		{
			DrawObject = DrawEntryPartsRoot[i] as InformationDrawObject;
			ArrayListMeshDraw = DrawObject.PartsRoot.ArrayListMeshDraw;
			int CountList = ArrayListMeshDraw.TableListMesh.Count;
			for(int j=0; j<CountList; j++)
			{
				/* Add Mesh-List */
				ListMeshDraw = ArrayListMeshDraw.TableListMesh[j] as Library_SpriteStudio.DrawManager.ListMeshDraw;
				arrayListMeshDraw.TableListMesh.Add(ListMeshDraw);
			}

			/* Clear Original List */
			ArrayListMeshDraw.Clear();
		}

		/* Collect Draw-Parts "Instance"-Object's Parts */

		/* Optimize Draw-Parts List */
		Library_SpriteStudio.DrawManager.ListMeshDraw ListMeshDrawNext = null;
		ArrayList TableListMesh = arrayListMeshDraw.TableListMesh;
		for(int i=0; i<(TableListMesh.Count - 1); )
		{
			Count = i + 1;	/* "Count"  is temporary */
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

		/* Meshes Combine each Material & Set to MeshFilter/MeshRenderer */
		MeshFilter InstanceMeshFilter = GetComponent<MeshFilter>();
		MeshRenderer InstanceMeshRenderer = GetComponent<MeshRenderer>();
		arrayListMeshDraw.MeshSetCombine(InstanceMeshFilter, InstanceMeshRenderer, InstanceCameraDraw, transform);

		/* Clear Draw-Entry (RootParts) */
		DrawEntryPartsRoot.Clear();
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

		/* Add Draw-Object (Not "Instance"-Object)  * */
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
}
