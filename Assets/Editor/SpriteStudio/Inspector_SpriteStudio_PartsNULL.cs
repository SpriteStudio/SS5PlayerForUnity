/**
	SpriteStudio5 Player for Unity

	Copyright(C) 2003-2013 Web Technology Corp. 
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
		EditorGUILayout.LabelField("[SpriteStudio Parts-NULL]");
	}
}
