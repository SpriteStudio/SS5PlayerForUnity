/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
#if false
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Script_SpriteStudio_DataCell))]
public class Inspector_SpriteStudio_DataCell : Editor
{
	public override void OnInspectorGUI()
	{
		EditorGUILayout.LabelField("[SpriteStudio CellMap-Data]");
		Script_SpriteStudio_DataCell Data = (Script_SpriteStudio_DataCell)target;
	}
}
#endif
