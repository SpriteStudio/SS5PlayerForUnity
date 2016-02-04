/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp.
	All rights reserved.
*/
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Script_SpriteStudio_ControlPrefab))]
public class Inspector_SpriteStudio_ControlPrefab : Editor
{
	public override void OnInspectorGUI()
	{
		Script_SpriteStudio_ControlPrefab Data = (Script_SpriteStudio_ControlPrefab)target;

		EditorGUILayout.LabelField("[SpriteStudio Control-Prefab]");
		int LevelIndent = 0;

		EditorGUI.indentLevel = LevelIndent;

		EditorGUILayout.Space();
		Data.InstanceManagerDraw = (Script_SpriteStudio_ManagerDraw)(EditorGUILayout.ObjectField("View (DrawManager)", Data.InstanceManagerDraw, typeof(Script_SpriteStudio_ManagerDraw), true));

		EditorGUILayout.Space();
		Data.PrefabUnderControl = EditorGUILayout.ObjectField("Prefab", Data.PrefabUnderControl, typeof(GameObject), true);

		if(true == GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}
}
