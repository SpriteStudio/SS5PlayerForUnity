/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Script_SpriteStudio_PartsInstance))]
public class Inspector_SpriteStudio_PartsInstance : Editor
{
	public override void OnInspectorGUI()
	{
		Script_SpriteStudio_PartsInstance Data = (Script_SpriteStudio_PartsInstance)target;

		EditorGUILayout.LabelField("[SpriteStudio Parts-Instance]");
		EditorGUILayout.Space();		

		Data.FlagHideForce = EditorGUILayout.Toggle("Force-Hide", Data.FlagHideForce);
		if(true == GUILayout.Button("Apply \"Force-Hide\" to Children"))
		{
			LibraryEditor_SpriteStudio.Utility.HideSetForce(Data.gameObject, Data.FlagHideForce, true, false);
		}
		EditorGUILayout.Space();

		if(true == GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}
}
