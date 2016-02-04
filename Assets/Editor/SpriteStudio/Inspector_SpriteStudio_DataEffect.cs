/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Script_SpriteStudio_DataEffect))]
public class Inspector_SpriteStudio_DataEffect : Editor
{
	public override void OnInspectorGUI()
	{
		EditorGUILayout.LabelField("[SpriteStudio Effect-Data]");
#if false
		Script_SpriteStudio_DataEmitter Data = (Script_SpriteStudio_DataEffect)target;
		int LevelIndent = 0;

		LibraryEditor_SpriteStudio.Utility.Inspector.DataDisplayDataEmitter(LevelIndent + 1, Data);
		EditorGUI.indentLevel = LevelIndent;
		EditorGUILayout.Space();
#endif
	}
}
