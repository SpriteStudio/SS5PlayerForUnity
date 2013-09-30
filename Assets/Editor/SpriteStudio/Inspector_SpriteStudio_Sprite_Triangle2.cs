/**
	SpriteStudio5 Player for Unity

	Copyright(C) 2003-2013 Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Script_SpriteStudio_Triangle2))]
public class Inspector_SpriteStudio_Sprite_Triangle2 : Editor
{
	public override void OnInspectorGUI()
	{
		EditorGUILayout.LabelField("[SpriteStudio Parts-Sprite(Triangle2)]");
	}
}
