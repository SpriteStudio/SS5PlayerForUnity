/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using UnityEditor;

public sealed class MenuItem_SpriteStudio : EditorWindow
{
	static private float CollisionThicknessZ = 1.0f;
	static int TextureSizePixelMaximum = 4096;
	static private bool FlagAttachRigidBody = false;
	static private bool FlagAttachControlGameObject = false;
	static private bool FlagConfirmOverWrite = false;
	static private bool FlagCreateProjectFolder = false;
	static private bool FlagGetAnimationReferencedPartsRoot = true;
	static private bool FlagGetMaterialPartsRoot = true;
	static private bool FlagGetTextureMaterial = true;
	static private bool FlagDataCalculateInAdvance = true;

	[MenuItem("Tools/SpriteStudio/Import SS5(sspj)")]
	static void OpenWindow()
	{
		EditorWindow.GetWindow<MenuItem_SpriteStudio>(true, "OPTPiX SpriteStudio Import-Settings");
		SettingGetImport();
	}
    void OnGUI()
	{
		GUILayout.Box("", GUILayout.Width(this.position.width), GUILayout.Height(1));
		EditorGUILayout.LabelField("- Options for basic");
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		FlagDataCalculateInAdvance = EditorGUILayout.Toggle("Calculate in advance", FlagDataCalculateInAdvance);
		EditorGUILayout.LabelField(" Deformations of \"Mesh\" and \"Collider\" are calculated at importing.");
		EditorGUILayout.LabelField(" Checked: Improving execution speed of the runtime.");
		EditorGUILayout.LabelField(" Unchecked: The data size is reduced.");
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		FlagAttachControlGameObject = EditorGUILayout.Toggle("Create Control-Prefab", FlagAttachControlGameObject);
		EditorGUILayout.LabelField(" Control-Prefab is GameObject attached the script");
		EditorGUILayout.LabelField("    for \"Auto-Developping Body-Data-Prefab (Script_LinkPrefab.cs)\".");
		EditorGUILayout.Space();
		FlagCreateProjectFolder = EditorGUILayout.Toggle("Create Project Folder", FlagCreateProjectFolder);
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		FlagConfirmOverWrite = EditorGUILayout.Toggle("Confirm Overwrite-Prefab", FlagConfirmOverWrite);
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		GUILayout.Box("", GUILayout.Width(this.position.width), GUILayout.Height(1));
		EditorGUILayout.LabelField("- Options for Collision-Detection");
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		CollisionThicknessZ = EditorGUILayout.FloatField("Collider-Thickness", CollisionThicknessZ);
		EditorGUILayout.LabelField(" (Local Z-Axis Width)");
		EditorGUILayout.Space();
		FlagAttachRigidBody = EditorGUILayout.Toggle("Attach Rigid-Body", FlagAttachRigidBody);
		EditorGUILayout.LabelField(" to Collider");
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		GUILayout.Box("", GUILayout.Width(this.position.width), GUILayout.Height(1));
		EditorGUILayout.LabelField("- Options for getting path of assets from existing Prefab data");
		EditorGUILayout.LabelField("  (This options are for do not change the assets\' GUID.)");
		EditorGUILayout.Space();
		FlagGetAnimationReferencedPartsRoot = EditorGUILayout.Toggle("Fixed-Animation Data", FlagGetAnimationReferencedPartsRoot);
		EditorGUILayout.Space();
		FlagGetMaterialPartsRoot = EditorGUILayout.Toggle("Material", FlagGetMaterialPartsRoot);
		EditorGUILayout.Space();
		FlagGetTextureMaterial = EditorGUILayout.Toggle("Texture (from Material)", FlagGetTextureMaterial);
		EditorGUILayout.Space();
		GUILayout.Box("", GUILayout.Width(this.position.width), GUILayout.Height(1));
		EditorGUILayout.Space();
		if(true == GUILayout.Button("Import"))
		{
			LibraryEditor_SpriteStudio.SettingImport SettingImport;
			SettingImport.TextureSizePixelMaximum = TextureSizePixelMaximum;
			SettingImport.CollisionThicknessZ = CollisionThicknessZ;
			SettingImport.FlagAttachRigidBody = FlagAttachRigidBody;
			SettingImport.FlagAttachControlGameObject = FlagAttachControlGameObject;
			SettingImport.FlagConfirmOverWrite = FlagConfirmOverWrite;
			SettingImport.FlagCreateProjectFolder = FlagCreateProjectFolder;
			SettingImport.FlagGetAnimationReferencedPartsRoot = FlagGetAnimationReferencedPartsRoot;
			SettingImport.FlagGetMaterialPartsRoot = FlagGetMaterialPartsRoot;
			SettingImport.FlagGetTextureMaterial = FlagGetTextureMaterial;
			SettingImport.FlagDataCalculateInAdvance = FlagDataCalculateInAdvance;
			LibraryEditor_SpriteStudio.Menu.ImportSSPJ(SettingImport);

			Close();
		}
		EditorGUILayout.Space();
		if(true == GUILayout.Button("Default Configuration"))
		{
			LibraryEditor_SpriteStudio.Menu.SettingClearImport();
			SettingGetImport();
		}
	}
	private	static void SettingGetImport()
	{
		LibraryEditor_SpriteStudio.SettingImport SettingImport;
		LibraryEditor_SpriteStudio.Menu.SettingGetImport(out SettingImport);

		TextureSizePixelMaximum = SettingImport.TextureSizePixelMaximum;
		CollisionThicknessZ = SettingImport.CollisionThicknessZ;
		FlagAttachRigidBody = SettingImport.FlagAttachRigidBody;
		FlagAttachControlGameObject = SettingImport.FlagAttachControlGameObject;
		FlagConfirmOverWrite = SettingImport.FlagConfirmOverWrite;
		FlagCreateProjectFolder = SettingImport.FlagCreateProjectFolder;
		FlagGetAnimationReferencedPartsRoot = SettingImport.FlagGetAnimationReferencedPartsRoot;
		FlagGetMaterialPartsRoot = SettingImport.FlagGetMaterialPartsRoot;
		FlagGetTextureMaterial = SettingImport.FlagGetTextureMaterial;
		FlagDataCalculateInAdvance = SettingImport.FlagDataCalculateInAdvance;
	}

	[MenuItem("Tools/SpriteStudio/About")]
	static void About()
	{
		string VersionText = "1.2.13";
		EditorUtility.DisplayDialog(	"SpriteStudio 5 Player for Unity",
										"Version: " + VersionText
										+ "\n\n"
										+ "Copyright(C) Web Technology Corp.",
										"OK"
									);
	}
}