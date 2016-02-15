/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Script_SpriteStudio_ManagerDraw))]
public class Inspector_SpriteStudio_ManagerDraw : Editor
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

	private static bool FoldOutCameraTarget;
	private static bool FoldOutDrawSetting;

	public override void OnInspectorGUI()
	{
		Script_SpriteStudio_ManagerDraw Data = (Script_SpriteStudio_ManagerDraw)target;

		EditorGUILayout.LabelField("[SpriteStudio Manager-Draw]");
		int LevelIndent = 0;

		EditorGUILayout.Space();
		Data.CameraTarget = EditorGUILayout.ObjectField("Camera", Data.CameraTarget, typeof(Camera), true) as Camera;
		EditorGUILayout.LabelField("- Objects to be drawn (Has \"Script_SpriteStudio_PartsRoot\")");
		EditorGUILayout.LabelField("  are Z-sort based on of this camera.");

		EditorGUILayout.Space();
		FoldOutCameraTarget = EditorGUILayout.Foldout(FoldOutCameraTarget, "\"Camera\" Setting");
		if(true == FoldOutCameraTarget)
		{
			EditorGUI.indentLevel = LevelIndent + 1;

			if(true == GUILayout.Button("Set \"Automatically Finding-Camera\""))
			{
				Data.CameraTarget = null;
			}
			EditorGUILayout.LabelField("- When you press this button, \"View (Camera)\" is set");
			EditorGUILayout.LabelField("  \"automatically scanning parent-GameObjects");
			EditorGUILayout.LabelField("  that have Camera-Component\".");
		}
		EditorGUI.indentLevel = LevelIndent;

		EditorGUILayout.Space();
		FoldOutDrawSetting = EditorGUILayout.Foldout(FoldOutDrawSetting, "Rendering Setting");
		if(true == FoldOutDrawSetting)
		{
			EditorGUI.indentLevel = LevelIndent + 1;

			EditorGUILayout.LabelField("Caution:");
			EditorGUILayout.LabelField("  This setting does not currently effect.");
			EditorGUILayout.LabelField("  Scheduled obsolete function in Ver.1.3.x.");
			EditorGUILayout.Space();

			int CountKindQueue = (int)Library_SpriteStudio.ManagerDraw.KindDrawQueue.OVERLAY + 1;
			int[] IndexDrawKind = new int[CountKindQueue];
			for(int i=0; i<CountKindQueue; i++)
			{
				IndexDrawKind[i] = i;
			}
			int KindRenderQueueBaseNo = (int)Data.KindRenderQueueBase;
			KindRenderQueueBaseNo = EditorGUILayout.IntPopup("Render-Queue Base", KindRenderQueueBaseNo, NameDrawKind, IndexDrawKind);
			Data.KindRenderQueueBase = (Library_SpriteStudio.ManagerDraw.KindDrawQueue)KindRenderQueueBaseNo;
			EditorGUI.indentLevel = LevelIndent + 2;
			EditorGUILayout.LabelField("Details [" + NameDrawKind[KindRenderQueueBaseNo] + "]");
			switch((Library_SpriteStudio.ManagerDraw.KindDrawQueue)KindRenderQueueBaseNo)
			{
				case Library_SpriteStudio.ManagerDraw.KindDrawQueue.SHADER_SETTING:
					EditorGUILayout.LabelField("- Value Base: Defined Tag\"Queue\" in Shader (Default: Transparent)");
					EditorGUILayout.LabelField("- Offset Range: Depend on Tag\"Queue\"(Default: 0-999)");
					break;

				case Library_SpriteStudio.ManagerDraw.KindDrawQueue.USER_SETTING:
					EditorGUILayout.LabelField("- Value Base: 0");
					EditorGUILayout.LabelField("- Offset Range: 1000-4999");
					break;

				default:
					EditorGUILayout.LabelField("- Value Base: " + Library_SpriteStudio.ManagerDraw.ValueKindDrawQueue[KindRenderQueueBaseNo]);
					EditorGUILayout.LabelField("- Offset Range: 0-" + (Library_SpriteStudio.ManagerDraw.ValueKindDrawQueue[KindRenderQueueBaseNo+1] - Library_SpriteStudio.ManagerDraw.ValueKindDrawQueue[KindRenderQueueBaseNo]-1));
					break;
			}
			EditorGUI.indentLevel = LevelIndent + 1;

			EditorGUILayout.Space();
			Data.OffsetDrawQueue = EditorGUILayout.IntField("Render-Queue Offset", Data.OffsetDrawQueue);

			EditorGUI.indentLevel = LevelIndent;
		}
		EditorGUILayout.Space();

		if(true == GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}
}
