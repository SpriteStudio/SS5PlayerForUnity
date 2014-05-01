/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using UnityEditor;

public sealed class MenuItem_SpriteStudio : EditorWindow
{
	public float CollisionThicknessZ = 1.0f;
	public bool FlagAttachRigidBody = true;
	public bool FlagAttachControlGameObject = true;

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
		FlagAttachControlGameObject = EditorGUILayout.Toggle("Attach GameObject", FlagAttachControlGameObject);
		EditorGUILayout.LabelField(" (GameObject is attached to \"Root-Node\"'s parent)");
		EditorGUILayout.Space();
		if(true == GUILayout.Button("Import"))
		{
			LibraryEditor_SpriteStudio.SettingImport SettingImport;
			SettingImport.TextureSizePixelMaximum = 4096;
			SettingImport.CollisionThicknessZ = CollisionThicknessZ;
			SettingImport.FlagAttachRigidBody = FlagAttachRigidBody;
			SettingImport.FlagAttachControlGameObject = FlagAttachControlGameObject;
			LibraryEditor_SpriteStudio.Menu.ImportSSPJ(SettingImport);

			Close();
		}
	}

	[MenuItem("Custom/SpriteStudio/About")]
	static void About()
	{
		string VersionText = "1.00 (Candidate)";
		EditorUtility.DisplayDialog(	"SpriteStudio5 Player for Unity",
										"Version: " + VersionText
										+ "\n\n"
										+ "Copyright(C) Web Technology Corp.",
										"OK"
									);
	}
}