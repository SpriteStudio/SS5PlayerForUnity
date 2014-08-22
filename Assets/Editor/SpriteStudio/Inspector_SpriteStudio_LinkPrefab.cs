/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Script_SpriteStudio_LinkPrefab))]
public class Inspector_SpriteStudio_LinkPrefab : Editor
{
	public override void OnInspectorGUI()
	{
		Script_SpriteStudio_LinkPrefab Data = (Script_SpriteStudio_LinkPrefab)target;

		EditorGUILayout.LabelField("[SpriteStudio Prefab Instantiate Script]");
		EditorGUILayout.Space();
		Data.LinkPrefab = EditorGUILayout.ObjectField("Prefab ", Data.LinkPrefab, typeof(GameObject), false);
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		Data.FlagAutoDevelop = EditorGUILayout.Toggle("Auto Develop-Prefab", Data.FlagAutoDevelop); 
		EditorGUILayout.LabelField("[Check] Developing Prefab,");
		EditorGUILayout.LabelField(" when Dropped to Hierarchy and Reloaded.");
		EditorGUILayout.LabelField("[No-Check] Not-Developping Prefab,");
		EditorGUILayout.LabelField(" when Dropped to Hierarchy and Reloaded.");
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		Data.FlagDeleteScript = EditorGUILayout.Toggle("Script Self-Delete ", Data.FlagDeleteScript); 
		EditorGUILayout.LabelField("[Check] Deleted After Develop-Prefab.");
		EditorGUILayout.LabelField("               Can't Recover, after Deleted.");
		EditorGUILayout.LabelField("[No-Check] Relict After Develop-Prefab.");
		EditorGUILayout.LabelField("                    for Testing on Editor.");
		EditorGUILayout.LabelField("[Finally, This Toggle is Checking.]");
		EditorGUILayout.LabelField("(on Runtime, Deleted Force.)");
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		if(true == GUILayout.Button("Develop-Prefab"))
		{
			Data.PrefabLinkInstantiate();
		}
		EditorGUILayout.Space();
		if(true == GUILayout.Button("Erase Developped-Prefab"))
		{
			Data.PrefabLinkDestroy();
		}
		EditorGUILayout.LabelField("[CAUTION] Above This Object is stored");
		EditorGUILayout.LabelField("in Prefab, Click \"Erase Developped-Prefab\".");
		EditorGUILayout.Space();
	}
}
