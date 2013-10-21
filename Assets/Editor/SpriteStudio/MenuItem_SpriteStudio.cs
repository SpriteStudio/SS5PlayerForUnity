/**
	SpriteStudio5 Player for Unity

	Copyright(C) 2003-2013 Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using UnityEditor;

public sealed class MenuItem_SpriteStudio : EditorWindow
{
	public float CollisionThicknessZ = 1.0f;
	public bool FlagAttachRigidBody = true;

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
		FlagAttachRigidBody = EditorGUILayout.Toggle("Attach Rigid-Body", FlagAttachRigidBody);
		EditorGUILayout.LabelField(" to Collider");
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		if(true == GUILayout.Button("Import"))
		{
			LibraryEditor_SpriteStudio.SettingImport SettingImport;
			SettingImport.TextureSizePixelMaximum = 4096;
			SettingImport.CollisionThicknessZ = CollisionThicknessZ;
			SettingImport.FlagAttachRigidBody = FlagAttachRigidBody;
			LibraryEditor_SpriteStudio.Menu.ImportSSPJ(SettingImport);

			Close();
		}
	}

	[MenuItem("Custom/SpriteStudio/About")]
	static void About()
	{
		string VersionText = "0.92 (Beta)";
		EditorUtility.DisplayDialog(	"SpriteStudio5 Player for Unity",
										"Version: " + VersionText
										+ "\n\n"
										+ "Copyright(C) 2013 Web Technology Corp.",
										"OK"
									);
	}
}