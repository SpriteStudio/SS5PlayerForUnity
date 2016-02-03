/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Script_SpriteStudio_Collider))]
public class Inspector_SpriteStudio_Collider : Editor
{
	public override void OnInspectorGUI()
	{
		EditorGUILayout.LabelField("[SpriteStudio Collider]");
//		Script_SpriteStudio_Collider Data = (Script_SpriteStudio_Collider)target;
	}
}
