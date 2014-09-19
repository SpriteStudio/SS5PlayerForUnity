/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using UnityEditor;

public sealed class MenuItem_SpriteStudio : EditorWindow
{
	static public float CollisionThicknessZ = 1.0f;
	static public bool FlagAttachRigidBody = true;
	static public bool FlagAttachControlGameObject = true;
	static public bool FlagConfirmOverWrite = true;
	static public bool FlagCreateProjectFolder = true;

	[MenuItem("Custom/SpriteStudio/Import SS5(sspj)")]
	static void OpenWindow()
	{
		EditorWindow.GetWindow<MenuItem_SpriteStudio>(true, "OPTPiX SpriteStudio Import-Settings");
	}
    void OnGUI()
	{
		CollisionThicknessZ = EditorGUILayout.FloatField("Collider-Thickness", CollisionThicknessZ);
		EditorGUILayout.LabelField(" (Local Z-Axis Width)");
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		FlagAttachRigidBody = EditorGUILayout.Toggle("Attach Rigid-Body", FlagAttachRigidBody);
		EditorGUILayout.LabelField(" to Collider");
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		FlagAttachControlGameObject = EditorGUILayout.Toggle("Create Control-Prefab", FlagAttachControlGameObject);
		EditorGUILayout.LabelField(" Control-Prefab is GameObject attached the script");
		EditorGUILayout.LabelField("    for \"Auto-Developping Body-Data-Prefab (Script_LinkPrefab.cs)\".");
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		FlagConfirmOverWrite = EditorGUILayout.Toggle("Confirm Overwrite-Prefab", FlagConfirmOverWrite);
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		FlagCreateProjectFolder = EditorGUILayout.Toggle("Create Project Folder", FlagCreateProjectFolder);
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		if(true == GUILayout.Button("Import"))
		{
			LibraryEditor_SpriteStudio.SettingImport SettingImport;
			SettingImport.TextureSizePixelMaximum = 4096;
			SettingImport.CollisionThicknessZ = CollisionThicknessZ;
			SettingImport.FlagAttachRigidBody = FlagAttachRigidBody;
			SettingImport.FlagAttachControlGameObject = FlagAttachControlGameObject;
			SettingImport.FlagConfirmOverWrite = FlagConfirmOverWrite;
			SettingImport.FlagCreateProjectFolder = FlagCreateProjectFolder;
			LibraryEditor_SpriteStudio.Menu.ImportSSPJ(SettingImport);

			Close();
		}
	}

	[MenuItem("Custom/SpriteStudio/About")]
	static void About()
	{
		string VersionText = "1.1beta2";
		EditorUtility.DisplayDialog(	"SpriteStudio 5 Player for Unity",
										"Version: " + VersionText
										+ "\n\n"
										+ "Copyright(C) Web Technology Corp.",
										"OK"
									);
	}
}