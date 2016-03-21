/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using UnityEditor;

public sealed class MenuItem_SpriteStudio : EditorWindow
{
	private static bool FlagNameDataRuleOld = false;
	private static int TextureSizePixelMaximum = 4096;
	private static bool FlagAttachControlGameObject = false;
	private static bool FlagCreateProjectFolder = false;
	private static bool FlagDataCalculateInAdvance = true;
	private static bool FlagDataCompress = true;
	private static bool FlagDataTakeOverSettingPrefab = false;
	private static float CollisionThicknessZ = 1.0f;
	private static bool FlagAttachRigidBody = false;
	private static bool FlagConfirmOverWrite = false;
	private static bool FlagConfirmOverWriteRoot = false;
	private static bool FlagConfirmOverWriteRootEffect = false;
	private static bool FlagConfirmOverWriteDataCellMap = false;
	private static bool FlagConfirmOverWriteDataAnimation = false;
	private static bool FlagConfirmOverWriteDataEffect = false;
	private static bool FlagConfirmOverWriteMaterial = false;
	private static bool FlagConfirmOverWriteTexture = false;
	private static bool FlagGetAnimationReferencedPartsRoot = true;
	private static bool FlagGetCellMapReferencedPartsRoot = true;
	private static bool FlagGetEffectReferencedPartsRoot = true;
	private static bool FlagGetMaterialPartsRoot = true;
	private static bool FlagGetTextureMaterial = true;

	private static bool FlagMenuFallOutBasic = false;
	private static bool FlagMenuFallOutOverwrite = false;
	private static bool FlagMenuFallOutCollider = false;
	private static bool FlagMenuFallOutAsset = false;

	private readonly static string PrefsKeyFallOutBasic = "SS5PU_ImporterMenu_FallOutBasic";
	private readonly static string PrefsKeyFallOutOverwrite = "SS5PU_ImporterMenu_Overwrite";
	private readonly static string PrefsKeyFallOutAsset = "SS5PU_ImporterMenu_FallOutAsset";
	private readonly static string PrefsKeyFallOutCollider = "SS5PU_ImporterMenu_FallOutCollider";

	[MenuItem("Tools/SpriteStudio/Import SS5(sspj)")]
	static void OpenWindow()
	{
		EditorWindow.GetWindow<MenuItem_SpriteStudio>(true, "OPTPiX SpriteStudio Import-Settings");
		SettingGetMenu();
		SettingGetImport();
	}
	void OnGUI()
	{
		int LevelIndent = 0;

		EditorGUI.indentLevel = LevelIndent;

		GUILayout.Box("", GUILayout.Width(this.position.width), GUILayout.Height(1));
		FlagMenuFallOutBasic = EditorGUILayout.Foldout(FlagMenuFallOutBasic, "Options for basic");
		if(true == FlagMenuFallOutBasic)
		{
			EditorGUI.indentLevel = LevelIndent + 1;

			FlagNameDataRuleOld = EditorGUILayout.Toggle("Naming Old Rule", FlagNameDataRuleOld);
			EditorGUILayout.LabelField(" Difference in naming of datas.");
			EditorGUILayout.LabelField(" Checked: Conpatible with up to Ver.1.2.x.");
			EditorGUILayout.LabelField(" Unchecked: Contain SubFolder-Name in Data-Name.");
			EditorGUILayout.Space();

			FlagDataCalculateInAdvance = EditorGUILayout.Toggle("Calculate in advance", FlagDataCalculateInAdvance);
			EditorGUILayout.LabelField(" Deformations of \"Mesh\" and \"Collider\" are calculated at importing.");
			EditorGUILayout.LabelField(" Checked: Improving execution speed of the runtime.");
			EditorGUILayout.LabelField(" Unchecked: The data size is reduced.");
			EditorGUILayout.Space();

			FlagDataCompress = EditorGUILayout.Toggle("Compress", FlagDataCompress);
			EditorGUILayout.LabelField(" Compress animation data at importing.");
			EditorGUILayout.LabelField(" Checked: Compress.");
			EditorGUILayout.LabelField(" Unchecked: Uncompress. CPU-Load is reduced.");
			EditorGUILayout.Space();
			EditorGUILayout.Space();

			FlagAttachControlGameObject = EditorGUILayout.Toggle("Create Control-Prefab", FlagAttachControlGameObject);
			EditorGUILayout.LabelField(" \"Control-Prefab\" is GameObject attached the script");
			EditorGUILayout.LabelField("   for Auto-Instantiate Body-Prefab (\"Script_SpriteStudio_ControlPrefab.cs\").");
			EditorGUILayout.Space();

			FlagCreateProjectFolder = EditorGUILayout.Toggle("Create Project Folder", FlagCreateProjectFolder);
			EditorGUILayout.Space();

			FlagDataTakeOverSettingPrefab = EditorGUILayout.Toggle("Take over setting", FlagDataTakeOverSettingPrefab);
			EditorGUILayout.LabelField(" Takes over the setting of \"Script_SpriteStudio_Root\"");
			EditorGUILayout.LabelField("   and \"Script_SpriteStudio_RootEffect\"  in Prefabs.");
			EditorGUILayout.Space();

			EditorGUI.indentLevel = LevelIndent;
		}
		EditorGUILayout.Space();

		GUILayout.Box("", GUILayout.Width(this.position.width), GUILayout.Height(1));
		FlagMenuFallOutCollider = EditorGUILayout.Foldout(FlagMenuFallOutCollider, "Options for \"Collider\"");
		if(true == FlagMenuFallOutCollider)
		{
			EditorGUI.indentLevel = LevelIndent + 1;

			CollisionThicknessZ = EditorGUILayout.FloatField("Collider-Thickness", CollisionThicknessZ);
			EditorGUILayout.LabelField(" (Local Z-Axis Width)");
			EditorGUILayout.Space();

			FlagAttachRigidBody = EditorGUILayout.Toggle("Attach Rigid-Body", FlagAttachRigidBody);
			EditorGUILayout.LabelField(" to Collider");
			EditorGUILayout.Space();

			EditorGUI.indentLevel = LevelIndent;
		}
		EditorGUILayout.Space();

		GUILayout.Box("", GUILayout.Width(this.position.width), GUILayout.Height(1));
		FlagMenuFallOutOverwrite = EditorGUILayout.Foldout(FlagMenuFallOutOverwrite, "Options for Confirm-Overwrite");
		if(true == FlagMenuFallOutOverwrite)
		{
			EditorGUI.indentLevel = LevelIndent + 1;

			FlagConfirmOverWrite = EditorGUILayout.Toggle("Confirm Overwrite", FlagConfirmOverWrite);
			EditorGUILayout.Space();

			if(true == FlagConfirmOverWrite)
			{
				EditorGUI.indentLevel = LevelIndent + 2;

				EditorGUILayout.LabelField("Additional options: Overwrite-check for individual data types.");
				EditorGUILayout.LabelField(" Unchecked: No confirm.");
				EditorGUILayout.Space();

				FlagConfirmOverWriteRoot = EditorGUILayout.Toggle("Root", FlagConfirmOverWriteRoot);
				EditorGUILayout.Space();

				FlagConfirmOverWriteRootEffect = EditorGUILayout.Toggle("Root-Effect", FlagConfirmOverWriteRootEffect);
				EditorGUILayout.Space();

				FlagConfirmOverWriteDataCellMap = EditorGUILayout.Toggle("Fixed-CellMap Data", FlagConfirmOverWriteDataCellMap);
				EditorGUILayout.Space();

				FlagConfirmOverWriteDataAnimation = EditorGUILayout.Toggle("Fixed-Animation Data", FlagConfirmOverWriteDataAnimation);
				EditorGUILayout.Space();

				FlagConfirmOverWriteDataEffect = EditorGUILayout.Toggle("Fixed-Effect Data", FlagConfirmOverWriteDataEffect);
				EditorGUILayout.Space();

				FlagConfirmOverWriteMaterial = EditorGUILayout.Toggle("Material", FlagConfirmOverWriteMaterial);
				EditorGUILayout.Space();

				FlagConfirmOverWriteTexture = EditorGUILayout.Toggle("Texture", FlagConfirmOverWriteTexture);
				EditorGUILayout.Space();

				EditorGUI.indentLevel = LevelIndent + 1;
			}

			EditorGUI.indentLevel = LevelIndent;
		}
		EditorGUILayout.Space();

		GUILayout.Box("", GUILayout.Width(this.position.width), GUILayout.Height(1));
		FlagMenuFallOutAsset = EditorGUILayout.Foldout(FlagMenuFallOutAsset, "Options for Tracking-Assets");
		if(true == FlagMenuFallOutAsset)
		{
			EditorGUI.indentLevel = LevelIndent + 1;

			EditorGUILayout.LabelField("Track assets location from reference data");
			EditorGUILayout.LabelField(" Unchecked: Generate deterrmined path-name to create assets.");
			EditorGUILayout.Space();

			FlagGetCellMapReferencedPartsRoot = EditorGUILayout.Toggle("Fixed-CellMapData", FlagGetCellMapReferencedPartsRoot);
			EditorGUILayout.Space();

			FlagGetAnimationReferencedPartsRoot = EditorGUILayout.Toggle("Fixed-Animation Data", FlagGetAnimationReferencedPartsRoot);
			EditorGUILayout.Space();

			FlagGetEffectReferencedPartsRoot = EditorGUILayout.Toggle("Fixed-Effect Data", FlagGetEffectReferencedPartsRoot);
			EditorGUILayout.Space();

			FlagGetMaterialPartsRoot = EditorGUILayout.Toggle("Material", FlagGetMaterialPartsRoot);
			EditorGUILayout.Space();

			FlagGetTextureMaterial = EditorGUILayout.Toggle("Texture (from Material)", FlagGetTextureMaterial);
			EditorGUILayout.Space();

			EditorGUI.indentLevel = LevelIndent;
		}
		EditorGUILayout.Space();

		GUILayout.Box("", GUILayout.Width(this.position.width), GUILayout.Height(1));
		EditorGUILayout.Space();
		if(true == GUILayout.Button("Import"))
		{
			SettingSetMenu();

			LibraryEditor_SpriteStudio.SettingImport SettingImport;
			SettingImport.FlagNameDataRuleOld = FlagNameDataRuleOld;
			SettingImport.TextureSizePixelMaximum = TextureSizePixelMaximum;
			SettingImport.FlagAttachControlGameObject = FlagAttachControlGameObject;
			SettingImport.FlagCreateProjectFolder = FlagCreateProjectFolder;
			SettingImport.FlagDataCalculateInAdvance = FlagDataCalculateInAdvance;
			SettingImport.FlagDataCompress = FlagDataCompress;
			SettingImport.FlagDataTakeOverSettingPrefab = FlagDataTakeOverSettingPrefab;

			SettingImport.CollisionThicknessZ = CollisionThicknessZ;
			SettingImport.FlagAttachRigidBody = FlagAttachRigidBody;

			SettingImport.FlagConfirmOverWrite = FlagConfirmOverWrite;
			SettingImport.FlagConfirmOverWriteRoot = FlagConfirmOverWriteRoot;
			SettingImport.FlagConfirmOverWriteRootEffect = FlagConfirmOverWriteRootEffect;
			SettingImport.FlagConfirmOverWriteDataCellMap = FlagConfirmOverWriteDataCellMap;
			SettingImport.FlagConfirmOverWriteDataAnimation = FlagConfirmOverWriteDataAnimation;
			SettingImport.FlagConfirmOverWriteDataEffect = FlagConfirmOverWriteDataEffect;
			SettingImport.FlagConfirmOverWriteMaterial = FlagConfirmOverWriteMaterial;
			SettingImport.FlagConfirmOverWriteTexture = FlagConfirmOverWriteTexture;

			SettingImport.FlagGetAnimationReferencedPartsRoot = FlagGetAnimationReferencedPartsRoot;
			SettingImport.FlagGetCellMapReferencedPartsRoot = FlagGetCellMapReferencedPartsRoot;
			SettingImport.FlagGetEffectReferencedPartsRoot = FlagGetEffectReferencedPartsRoot;
			SettingImport.FlagGetMaterialPartsRoot = FlagGetMaterialPartsRoot;
			SettingImport.FlagGetTextureMaterial = FlagGetTextureMaterial;

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
	private static void SettingGetMenu()
	{
		FlagMenuFallOutBasic = EditorPrefs.GetBool(PrefsKeyFallOutBasic, false);
		FlagMenuFallOutOverwrite = EditorPrefs.GetBool(PrefsKeyFallOutOverwrite, false);
		FlagMenuFallOutAsset = EditorPrefs.GetBool(PrefsKeyFallOutAsset, false);
		FlagMenuFallOutCollider = EditorPrefs.GetBool(PrefsKeyFallOutCollider, false);
	}
	private static void SettingSetMenu()
	{
		EditorPrefs.SetBool(PrefsKeyFallOutBasic, FlagMenuFallOutBasic);
		EditorPrefs.SetBool(PrefsKeyFallOutOverwrite, FlagMenuFallOutOverwrite);
		EditorPrefs.SetBool(PrefsKeyFallOutAsset, FlagMenuFallOutAsset);
		EditorPrefs.SetBool(PrefsKeyFallOutCollider, FlagMenuFallOutCollider);
	}
	private	static void SettingGetImport()
	{
		LibraryEditor_SpriteStudio.SettingImport SettingImport;
		LibraryEditor_SpriteStudio.Menu.SettingGetImport(out SettingImport);

		FlagNameDataRuleOld = SettingImport.FlagNameDataRuleOld;
		TextureSizePixelMaximum = SettingImport.TextureSizePixelMaximum;
		FlagAttachControlGameObject = SettingImport.FlagAttachControlGameObject;
		FlagCreateProjectFolder = SettingImport.FlagCreateProjectFolder;
		FlagDataCalculateInAdvance = SettingImport.FlagDataCalculateInAdvance;
		FlagDataCompress = SettingImport.FlagDataCompress;
		FlagDataTakeOverSettingPrefab = SettingImport.FlagDataTakeOverSettingPrefab;

		CollisionThicknessZ = SettingImport.CollisionThicknessZ;
		FlagAttachRigidBody = SettingImport.FlagAttachRigidBody;

		FlagConfirmOverWrite = SettingImport.FlagConfirmOverWrite;
		FlagConfirmOverWriteRoot = SettingImport.FlagConfirmOverWriteRoot;
		FlagConfirmOverWriteRootEffect = SettingImport.FlagConfirmOverWriteRootEffect;
		FlagConfirmOverWriteDataCellMap = SettingImport.FlagConfirmOverWriteDataCellMap;
		FlagConfirmOverWriteDataAnimation = SettingImport.FlagConfirmOverWriteDataAnimation;
		FlagConfirmOverWriteDataEffect = SettingImport.FlagConfirmOverWriteDataEffect;
		FlagConfirmOverWriteMaterial = SettingImport.FlagConfirmOverWriteMaterial;
		FlagConfirmOverWriteTexture = SettingImport.FlagConfirmOverWriteTexture;

		FlagGetAnimationReferencedPartsRoot = SettingImport.FlagGetAnimationReferencedPartsRoot;
		FlagGetCellMapReferencedPartsRoot = SettingImport.FlagGetCellMapReferencedPartsRoot;
		FlagGetEffectReferencedPartsRoot = SettingImport.FlagGetEffectReferencedPartsRoot;
		FlagGetMaterialPartsRoot = SettingImport.FlagGetMaterialPartsRoot;
		FlagGetTextureMaterial = SettingImport.FlagGetTextureMaterial;
	}

	[MenuItem("Tools/SpriteStudio/About")]
	static void About()
	{
		string VersionText = "1.3.4";
		EditorUtility.DisplayDialog(	"SpriteStudio 5 Player for Unity",
										"Version: " + VersionText
										+ "\n\n"
										+ "Copyright(C) Web Technology Corp.",
										"OK"
									);
	}
}
