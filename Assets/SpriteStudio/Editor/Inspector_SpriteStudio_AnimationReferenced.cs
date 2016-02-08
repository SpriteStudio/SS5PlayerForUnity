/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Script_SpriteStudio_AnimationReferenced))]
public class Inspector_SpriteStudio_AnimationReferenced : Editor
{
	public override void OnInspectorGUI()
	{
		Script_SpriteStudio_AnimationReferenced Data = (Script_SpriteStudio_AnimationReferenced)target;

		EditorGUILayout.LabelField("[SpriteStudio Animation-Fixed Data]");
		EditorGUILayout.Space();

		EditorGUILayout.LabelField("- Animation Count : " + Data.ListInformationAnimation.Length);
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("- Node Count : " + Data.ListNodeAnimationData.Length);
		EditorGUILayout.Space();

		if (GUILayout.Button("Decompress"))
		{
			Data.Decompress();
			EditorUtility.SetDirty(Data);
		}
		if (GUILayout.Button("Compress"))
		{
			Data.Compress();
			EditorUtility.SetDirty(Data);
		}
	}
}