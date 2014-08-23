/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Script_SpriteStudio_DrawManagerView))]
public class Inspector_SpriteStudio_DrawManagerView : Editor
{
	private static readonly string[] NameDrawKind =
	{
		"Shader Setting (Initial)",
		"User Setting",
		"Back Ground",
		"Geometry",
		"Alpha Test",
		"Transparent",
		"Overlay",
//		"(TERMINATOR)"
	};

	private static bool FoldOutDrawSetting;

	public override void OnInspectorGUI()
	{
		Script_SpriteStudio_DrawManagerView Data = (Script_SpriteStudio_DrawManagerView)target;

		EditorGUILayout.LabelField("[SpriteStudio DrawManager]");
		int LevelIndent = 0;

		EditorGUILayout.Space();
		FoldOutDrawSetting = EditorGUILayout.Foldout(FoldOutDrawSetting, "Rendering Setting");
		if(true == FoldOutDrawSetting)
		{
			EditorGUI.indentLevel = LevelIndent + 1;

			Library_SpriteStudio.DrawManager.ArrayListMeshDraw ArrayListMeshDraw = Data.ArrayListMeshDraw;
			int CountKindQueue = (int)Library_SpriteStudio.DrawManager.KindDrawQueue.OVERLAY + 1;
			int[] IndexDrawKind = new int[CountKindQueue];
			for(int i=0; i<CountKindQueue; i++)
			{
				IndexDrawKind[i] = i;
			}
			int KindRenderQueueBaseNo = (int)ArrayListMeshDraw.KindRenderQueueBase;
			KindRenderQueueBaseNo = EditorGUILayout.IntPopup("Render-Queue Base", KindRenderQueueBaseNo, NameDrawKind, IndexDrawKind);
			ArrayListMeshDraw.KindRenderQueueBase = (Library_SpriteStudio.DrawManager.KindDrawQueue)KindRenderQueueBaseNo;
			EditorGUI.indentLevel = LevelIndent + 2;
			EditorGUILayout.LabelField("Details [" + NameDrawKind[KindRenderQueueBaseNo] + "]");
			switch((Library_SpriteStudio.DrawManager.KindDrawQueue)KindRenderQueueBaseNo)
			{
				case Library_SpriteStudio.DrawManager.KindDrawQueue.SHADER_SETTING:
					EditorGUILayout.LabelField("- Value Base: Defined Tag\"Queue\" in Shader (Default: Transparent)");
					EditorGUILayout.LabelField("- Offset Range: Depend on Tag\"Queue\"(Default: 0-999)");
					break;

				case Library_SpriteStudio.DrawManager.KindDrawQueue.USER_SETTING:
					EditorGUILayout.LabelField("- Value Base: 0");
					EditorGUILayout.LabelField("- Offset Range: 1000-4999");
					break;

				default:
					EditorGUILayout.LabelField("- Value Base: " + Library_SpriteStudio.DrawManager.ValueKindDrawQueue[KindRenderQueueBaseNo]);
					EditorGUILayout.LabelField("- Offset Range: 0-" + (Library_SpriteStudio.DrawManager.ValueKindDrawQueue[KindRenderQueueBaseNo+1] - Library_SpriteStudio.DrawManager.ValueKindDrawQueue[KindRenderQueueBaseNo]-1));
					break;
			}
			EditorGUI.indentLevel = LevelIndent + 1;

			EditorGUILayout.Space();
			ArrayListMeshDraw.OffsetDrawQueue = EditorGUILayout.IntField("Render-Queue Offset", ArrayListMeshDraw.OffsetDrawQueue);

//			EditorGUILayout.Space();
//			ArrayListMeshDraw.RateDrawQueueEffectZ = EditorGUILayout.FloatField("Rate Z Effect", ArrayListMeshDraw.RateDrawQueueEffectZ);
//			EditorGUILayout.LabelField("(\"This-Value x ViewPort-Z\" Added to Offset)");

			EditorGUI.indentLevel = LevelIndent;
		}
		EditorGUILayout.Space();

		if(true == GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}
}
