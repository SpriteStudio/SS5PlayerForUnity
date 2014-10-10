/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Script_SpriteStudio_Triangle4))]
public class Inspector_SpriteStudio_Sprite_Triangle4 : Editor
{
	public override void OnInspectorGUI()
	{
		Script_SpriteStudio_Triangle4 Data = (Script_SpriteStudio_Triangle4)target;

		EditorGUILayout.LabelField("[SpriteStudio Parts-Sprite(Triangle4)]");
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
