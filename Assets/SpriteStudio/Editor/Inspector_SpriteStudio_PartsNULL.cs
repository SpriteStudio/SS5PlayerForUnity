/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Script_SpriteStudio_PartsNULL))]
public class Inspector_SpriteStudio_PartsNULL : Editor
{
	public override void OnInspectorGUI()
	{
		Script_SpriteStudio_PartsNULL Data = (Script_SpriteStudio_PartsNULL)target;

		EditorGUILayout.LabelField("[SpriteStudio Parts-NULL]");

		Data.FlagHideForce = EditorGUILayout.Toggle("Force-Hide", Data.FlagHideForce);
		if(true == GUILayout.Button("Apply \"Force-Hide\" to Children"))
		{
			LibraryEditor_SpriteStudio.Utility.HideSetForce(Data.gameObject, Data.FlagHideForce, true, false);
		}
		EditorGUILayout.Space();
	}
}
