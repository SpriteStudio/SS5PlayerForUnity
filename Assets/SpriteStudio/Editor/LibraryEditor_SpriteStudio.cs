/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

public static partial class LibraryEditor_SpriteStudio
{
	/* Default Shaders' Data */
	private readonly static int ShaderOperationMax = (int)Library_SpriteStudio.KindColorOperation.TERMINATOR - 1;				/* Excluded "NON" */
	private readonly static int ShaderOperationMaxEffect = (int)Library_SpriteStudio.KindColorOperationEffect.TERMINATOR - 1;	/* Excluded "NON" */

	/* Default Asset-File Folders & Extensions */
	internal readonly static string NamePathSubImportTexture = "Texture";
	internal readonly static string NamePathSubImportMaterial = "Material";
	internal readonly static string NamePathSubImportPrefab = "Prefab";
	internal readonly static string NamePathSubImportAnimation = "Animation";
	internal readonly static string NamePathSubImportCellMap = "CellMap";
	internal readonly static string NamePathSubImportEffect = "Effect";
	internal readonly static string NamePathSubImportPrefabEffect = "Prefab_Effect";
	internal readonly static string NamePathSubImportMaterialEffect = "Material_Effect";

	internal readonly static string NamePrefixPrefab = "pa_";
	internal readonly static string NamePrefixPrefabEffect = "pe_";
	internal readonly static string NamePrefixAnimation = "da_";
	internal readonly static string NamePrefixEffect = "de_";
	internal readonly static string NamePrefixCellMap = "dc_";
	internal readonly static string NamePrefixMaterial = "ma_";
	internal readonly static string NamePrefixMaterialEffect = "me_";
	internal readonly static string NamePrefixTexture = "tx_";

	internal readonly static string NameExtensionPrefab = ".prefab";
	internal readonly static string NameExtensionPrefabEffect = ".prefab";
	internal readonly static string NameExtensionAnimation = ".asset";
	internal readonly static string NameExtensionEffect = ".asset";
	internal readonly static string NameExtensionCellMap = ".asset";
	internal readonly static string NameExtensionMaterial = ".mat";
	internal readonly static string NameExtensionMaterialEffect = ".mat";

	internal readonly static string NameSuffixPrefabControl = "_Control";

	/* Interfaces between Unity's-Menu-Script and OPSS(SS5)Data-Importing-Script */
	public struct SettingImport
	{
		public bool FlagNameDataRuleOld;
		public bool FlagNameDataAttachSpecific;
		public bool FlagNameDataAttachSpecificToPrefab;
//		public bool FlagNameDataAttachSpecificToCellMap;
		public bool FlagNameDataAttachSpecificToTexture;
		public bool FlagNameDataAttachSpecificSSPJ;

		public int TextureSizePixelMaximum;
		public bool FlagAttachControlGameObject;
		public bool FlagCreateProjectFolder;
		public bool FlagDataCalculateInAdvance;
		public bool FlagDataCompress;
		public bool FlagDataTakeOverSettingPrefab;

		public float CollisionThicknessZ;
		public bool FlagAttachRigidBody;

		public bool FlagConfirmOverWrite;
		public bool FlagConfirmOverWriteRoot;
		public bool FlagConfirmOverWriteRootEffect;
		public bool FlagConfirmOverWriteDataCellMap;
		public bool FlagConfirmOverWriteDataAnimation;
		public bool FlagConfirmOverWriteDataEffect;
		public bool FlagConfirmOverWriteMaterial;
		public bool FlagConfirmOverWriteTexture;

		public bool FlagGetAnimationReferencedPartsRoot;
		public bool FlagGetCellMapReferencedPartsRoot;
		public bool FlagGetEffectReferencedPartsRoot;
		public bool FlagGetMaterialPartsRoot;
		public bool FlagGetTextureMaterial;
	}
	internal readonly static string PrefsKeyFlagNameDataRuleOld = "SS5PU_Importer_FlagNameDataRuleOld";
	internal readonly static string PrefsKeyFlagNameDataAttachSpecific = "SS5PU_Importer_FlagNameDataAttachSpecific";
	internal readonly static string PrefsKeyFlagNameDataAttachSpecificToPrefab = "SS5PU_Importer_FlagNameDataAttachSpecificToPrefab";
//	internal readonly static string PrefsKeyFlagNameDataAttachSpecificToCellMap = "SS5PU_Importer_FlagNameDataAttachSpecificToCellMap";
	internal readonly static string PrefsKeyFlagNameDataAttachSpecificToTexture = "SS5PU_Importer_FlagNameDataAttachSpecificToTexture";
	internal readonly static string PrefsKeyFlagNameDataAttachSpecificSSPJ = "SS5PU_Importer_FlagNameDataAttachSpecificSSPJ";
	internal readonly static string PrefsKeyTextureSizePixelMaximum = "SS5PU_Importer_TextureSizePixelMaximum";
	internal readonly static string PrefsKeyFlagAttachControlGameObject = "SS5PU_Importer_FlagAttachControlGameObject";
	internal readonly static string PrefsKeyFlagCreateProjectFolder = "SS5PU_Importer_FlagCreateProjectFolder";
	internal readonly static string PrefsKeyFlagDataCalculateInAdvance = "SS5PU_Importer_FlagDataCalculateInAdvance";
	internal readonly static string PrefsKeyFlagDataCompress = "SS5PU_Importer_FlagDataCompress";
	internal readonly static string PrefsKeyFlagTakeOverSettingPrefab = "SS5PU_Importer_FlagTakeOverSettingPrefab";
	internal readonly static string PrefsKeyCollisionThicknessZ = "SS5PU_Importer_CollisionThicknessZ";
	internal readonly static string PrefsKeyFlagAttachRigidBody = "SS5PU_Importer_FlagAttachRigidBody";
	internal readonly static string PrefsKeyFlagConfirmOverWrite = "SS5PU_Importer_FlagConfirmOverWrite";
	internal readonly static string PrefsKeyFlagConfirmOverWriteRoot = "SS5PU_Importer_FlagConfirmOverWriteRoot";
	internal readonly static string PrefsKeyFlagConfirmOverWriteRootEffect = "SS5PU_Importer_FlagConfirmOverWriteRootEffect";
	internal readonly static string PrefsKeyFlagConfirmOverWriteDataCellMap = "SS5PU_Importer_FlagConfirmOverWriteDataCellMap";
	internal readonly static string PrefsKeyFlagConfirmOverWriteDataAnimation = "SS5PU_Importer_FlagConfirmOverWriteDataAnimation";
	internal readonly static string PrefsKeyFlagConfirmOverWriteDataEffect = "SS5PU_Importer_FlagConfirmOverWriteDataEffect";
	internal readonly static string PrefsKeyFlagConfirmOverWriteMaterial = "SS5PU_Importer_FlagConfirmOverWriteMaterial";
	internal readonly static string PrefsKeyFlagConfirmOverWriteTexture = "SS5PU_Importer_FlagConfirmOverWriteTexture";
	internal readonly static string PrefsKeyFolderNameImpoertLast = "SS5PU_Importer_FolderNameImpoertLast";
	internal readonly static string PrefsKeyFlagGetAnimationDataPartsRoot = "SS5PU_Importer_FlagGetAnimationReferencedPartsRoot";
	internal readonly static string PrefsKeyFlagGetCellMapDataPartsRoot = "SS5PU_Importer_FlagGetCellMapReferencedPartsRoot";
	internal readonly static string PrefsKeyFlagGetEffectDataPartsRoot = "SS5PU_Importer_FlagGetEffectReferencedPartsRoot";
	internal readonly static string PrefsKeyFlagGetMaterialPartsRoot = "SS5PU_Importer_FlagGetMaterialPartsRoot";
	internal readonly static string PrefsKeyFlagGetTextureMaterial = "SS5PU_Importer_FlagGetTextureMaterial";

	internal readonly static bool DefaultFlagNameDataRuleOld = false;
	internal readonly static bool DefaultFlagNameDataAttachSpecific = false;
	internal readonly static bool DefaultFlagNameDataAttachSpecificToPrefab = false;
//	internal readonly static bool DefaultFlagNameDataAttachSpecificToCellMap = false;
	internal readonly static bool DefaultFlagNameDataAttachSpecificToTexture = false;
	internal readonly static bool DefaultFlagNameDataAttachSpecificSSPJ = true;
	internal readonly static int DefaultTextureSizePixelMaximum = 4096;
	internal readonly static bool DefaultFlagAttachControlGameObject = true;
	internal readonly static bool DefaultFlagCreateProjectFolder = true;
	internal readonly static bool DefaultFlagDataCompress = true;
	internal readonly static bool DefaultFlagDataCalculateInAdvance = false;
	internal readonly static bool DefaultFlagTakeOverSettingPrefab = false;
	internal readonly static float DefaultCollisionThicknessZ = 1.0f;
	internal readonly static bool DefaultFlagAttachRigidBody = true;
	internal readonly static bool DefaultFlagConfirmOverWrite = true;
	internal readonly static bool DefaultFlagConfirmOverWriteRoot = true;
	internal readonly static bool DefaultFlagConfirmOverWriteRootEffect = true;
	internal readonly static bool DefaultFlagConfirmOverWriteDataCellMap = true;
	internal readonly static bool DefaultFlagConfirmOverWriteDataAnimation = true;
	internal readonly static bool DefaultFlagConfirmOverWriteDataEffect = true;
	internal readonly static bool DefaultFlagConfirmOverWriteMaterial = true;
	internal readonly static bool DefaultFlagConfirmOverWriteTexture = true;
	internal readonly static bool DefaultFlagGetAnimationReferencedPartsRoot = true;
	internal readonly static bool DefaultFlagGetCellMapReferencedPartsRoot = true;
	internal readonly static bool DefaultFlagGetEffectReferencedPartsRoot = true;
	internal readonly static bool DefaultFlagGetMaterialPartsRoot = true;
	internal readonly static bool DefaultFlagGetTextureMaterial = true;
	internal readonly static string DefaultFolderNameImpoertLast = "";

	internal readonly static ReplacePrefabOptions OptionPrefabOverwrite = ReplacePrefabOptions.ReplaceNameBased;

	/* Functions for Interface with "Unity" */
	public static partial class Menu
	{
		internal static void SettingClearImport()
		{
			EditorPrefs.SetBool(PrefsKeyFlagNameDataRuleOld, DefaultFlagNameDataRuleOld);
			EditorPrefs.SetBool(PrefsKeyFlagNameDataAttachSpecific, DefaultFlagNameDataAttachSpecific);
			EditorPrefs.SetBool(PrefsKeyFlagNameDataAttachSpecificToPrefab, DefaultFlagNameDataAttachSpecificToPrefab);
//			EditorPrefs.SetBool(PrefsKeyFlagNameDataAttachSpecificToCellMap, DefaultFlagNameDataAttachSpecificToCellMap);
			EditorPrefs.SetBool(PrefsKeyFlagNameDataAttachSpecificToTexture, DefaultFlagNameDataAttachSpecificToTexture);
			EditorPrefs.SetBool(PrefsKeyFlagNameDataAttachSpecificSSPJ, DefaultFlagNameDataAttachSpecificSSPJ);
			EditorPrefs.SetInt(PrefsKeyTextureSizePixelMaximum, DefaultTextureSizePixelMaximum);
			EditorPrefs.SetFloat(PrefsKeyCollisionThicknessZ, DefaultCollisionThicknessZ);
			EditorPrefs.SetBool(PrefsKeyFlagCreateProjectFolder, DefaultFlagCreateProjectFolder);
			EditorPrefs.SetBool(PrefsKeyFlagDataCalculateInAdvance, DefaultFlagDataCalculateInAdvance);
			EditorPrefs.SetBool(PrefsKeyFlagDataCompress, DefaultFlagDataCompress);
			EditorPrefs.SetBool(PrefsKeyFlagTakeOverSettingPrefab, DefaultFlagTakeOverSettingPrefab);

			EditorPrefs.SetBool(PrefsKeyFlagAttachRigidBody, DefaultFlagAttachRigidBody);
			EditorPrefs.SetBool(PrefsKeyFlagAttachControlGameObject, DefaultFlagAttachControlGameObject);

			EditorPrefs.SetBool(PrefsKeyFlagConfirmOverWrite, DefaultFlagConfirmOverWrite);
			EditorPrefs.SetBool(PrefsKeyFlagConfirmOverWriteRoot, DefaultFlagConfirmOverWriteRoot);
			EditorPrefs.SetBool(PrefsKeyFlagConfirmOverWriteRootEffect, DefaultFlagConfirmOverWriteRootEffect);
			EditorPrefs.SetBool(PrefsKeyFlagConfirmOverWriteDataCellMap, DefaultFlagConfirmOverWriteDataCellMap);
			EditorPrefs.SetBool(PrefsKeyFlagConfirmOverWriteDataAnimation, DefaultFlagConfirmOverWriteDataAnimation);
			EditorPrefs.SetBool(PrefsKeyFlagConfirmOverWriteDataEffect, DefaultFlagConfirmOverWriteDataEffect);
			EditorPrefs.SetBool(PrefsKeyFlagConfirmOverWriteMaterial, DefaultFlagConfirmOverWriteMaterial);
			EditorPrefs.SetBool(PrefsKeyFlagConfirmOverWriteTexture, DefaultFlagConfirmOverWriteTexture);

			EditorPrefs.SetBool(PrefsKeyFlagGetAnimationDataPartsRoot, DefaultFlagGetAnimationReferencedPartsRoot);
			EditorPrefs.SetBool(PrefsKeyFlagGetCellMapDataPartsRoot, DefaultFlagGetCellMapReferencedPartsRoot);
			EditorPrefs.SetBool(PrefsKeyFlagGetEffectDataPartsRoot, DefaultFlagGetEffectReferencedPartsRoot);
			EditorPrefs.SetBool(PrefsKeyFlagGetMaterialPartsRoot, DefaultFlagGetMaterialPartsRoot);
			EditorPrefs.SetBool(PrefsKeyFlagGetTextureMaterial, DefaultFlagGetTextureMaterial);

			EditorPrefs.SetString(PrefsKeyFolderNameImpoertLast, DefaultFolderNameImpoertLast);
		}

		internal static void SettingGetImport(out SettingImport DataSettingImport)
		{
			DataSettingImport.FlagNameDataRuleOld = EditorPrefs.GetBool(PrefsKeyFlagNameDataRuleOld, DefaultFlagNameDataRuleOld);
			DataSettingImport.FlagNameDataAttachSpecific = EditorPrefs.GetBool(PrefsKeyFlagNameDataAttachSpecific, DefaultFlagNameDataAttachSpecific);
			DataSettingImport.FlagNameDataAttachSpecificToPrefab = EditorPrefs.GetBool(PrefsKeyFlagNameDataAttachSpecificToPrefab, DefaultFlagNameDataAttachSpecificToPrefab);
//			DataSettingImport.FlagNameDataAttachSpecificToCellMap = EditorPrefs.GetBool(PrefsKeyFlagNameDataAttachSpecificToCellMap, DefaultFlagNameDataAttachSpecificToCellMap);
			DataSettingImport.FlagNameDataAttachSpecificToTexture = EditorPrefs.GetBool(PrefsKeyFlagNameDataAttachSpecificToTexture, DefaultFlagNameDataAttachSpecificToTexture);
			DataSettingImport.FlagNameDataAttachSpecificSSPJ = EditorPrefs.GetBool(PrefsKeyFlagNameDataAttachSpecificSSPJ, DefaultFlagNameDataAttachSpecificSSPJ);
			DataSettingImport.TextureSizePixelMaximum = EditorPrefs.GetInt(PrefsKeyTextureSizePixelMaximum, DefaultTextureSizePixelMaximum);
			DataSettingImport.FlagAttachControlGameObject = EditorPrefs.GetBool(PrefsKeyFlagAttachControlGameObject, DefaultFlagAttachControlGameObject);
			DataSettingImport.FlagCreateProjectFolder = EditorPrefs.GetBool(PrefsKeyFlagCreateProjectFolder, DefaultFlagCreateProjectFolder);
			DataSettingImport.FlagDataCalculateInAdvance = EditorPrefs.GetBool(PrefsKeyFlagDataCalculateInAdvance, DefaultFlagDataCalculateInAdvance);
			DataSettingImport.FlagDataCompress = EditorPrefs.GetBool(PrefsKeyFlagDataCompress, DefaultFlagDataCompress);
			DataSettingImport.FlagDataTakeOverSettingPrefab = EditorPrefs.GetBool(PrefsKeyFlagTakeOverSettingPrefab, DefaultFlagTakeOverSettingPrefab);

			DataSettingImport.CollisionThicknessZ = EditorPrefs.GetFloat(PrefsKeyCollisionThicknessZ, DefaultCollisionThicknessZ);
			DataSettingImport.FlagAttachRigidBody = EditorPrefs.GetBool(PrefsKeyFlagAttachRigidBody, DefaultFlagAttachRigidBody);

			DataSettingImport.FlagConfirmOverWrite = EditorPrefs.GetBool(PrefsKeyFlagConfirmOverWrite, DefaultFlagConfirmOverWrite);
			DataSettingImport.FlagConfirmOverWriteRoot = EditorPrefs.GetBool(PrefsKeyFlagConfirmOverWriteRoot, DefaultFlagConfirmOverWriteRoot);
			DataSettingImport.FlagConfirmOverWriteRootEffect = EditorPrefs.GetBool(PrefsKeyFlagConfirmOverWriteRootEffect, DefaultFlagConfirmOverWriteRootEffect);
			DataSettingImport.FlagConfirmOverWriteDataCellMap = EditorPrefs.GetBool(PrefsKeyFlagConfirmOverWriteDataCellMap, DefaultFlagConfirmOverWriteDataCellMap);
			DataSettingImport.FlagConfirmOverWriteDataAnimation = EditorPrefs.GetBool(PrefsKeyFlagConfirmOverWriteDataAnimation, DefaultFlagConfirmOverWriteDataAnimation);
			DataSettingImport.FlagConfirmOverWriteDataEffect = EditorPrefs.GetBool(PrefsKeyFlagConfirmOverWriteDataEffect, DefaultFlagConfirmOverWriteDataEffect);
			DataSettingImport.FlagConfirmOverWriteMaterial = EditorPrefs.GetBool(PrefsKeyFlagConfirmOverWriteMaterial, DefaultFlagConfirmOverWriteMaterial);
			DataSettingImport.FlagConfirmOverWriteTexture = EditorPrefs.GetBool(PrefsKeyFlagConfirmOverWriteTexture, DefaultFlagConfirmOverWriteTexture);

			DataSettingImport.FlagGetAnimationReferencedPartsRoot = EditorPrefs.GetBool(PrefsKeyFlagGetAnimationDataPartsRoot, DefaultFlagGetAnimationReferencedPartsRoot);
			DataSettingImport.FlagGetCellMapReferencedPartsRoot = EditorPrefs.GetBool(PrefsKeyFlagGetCellMapDataPartsRoot, DefaultFlagGetCellMapReferencedPartsRoot);
			DataSettingImport.FlagGetEffectReferencedPartsRoot = EditorPrefs.GetBool(PrefsKeyFlagGetEffectDataPartsRoot, DefaultFlagGetEffectReferencedPartsRoot);
			DataSettingImport.FlagGetMaterialPartsRoot = EditorPrefs.GetBool(PrefsKeyFlagGetMaterialPartsRoot, DefaultFlagGetMaterialPartsRoot);
			DataSettingImport.FlagGetTextureMaterial = EditorPrefs.GetBool(PrefsKeyFlagGetTextureMaterial, DefaultFlagGetTextureMaterial);
		}
		internal static void SettingSetImport(ref SettingImport DataSettingImport)
		{
			EditorPrefs.SetBool(PrefsKeyFlagNameDataRuleOld, DataSettingImport.FlagNameDataRuleOld);
			EditorPrefs.SetBool(PrefsKeyFlagNameDataAttachSpecific, DataSettingImport.FlagNameDataAttachSpecific);
			EditorPrefs.SetBool(PrefsKeyFlagNameDataAttachSpecificToPrefab, DataSettingImport.FlagNameDataAttachSpecificToPrefab);
//			EditorPrefs.SetBool(PrefsKeyFlagNameDataAttachSpecificToCellMap, DataSettingImport.FlagNameDataAttachSpecificToCellMap);
			EditorPrefs.SetBool(PrefsKeyFlagNameDataAttachSpecificToTexture, DataSettingImport.FlagNameDataAttachSpecificToTexture);
			EditorPrefs.SetBool(PrefsKeyFlagNameDataAttachSpecificSSPJ, DataSettingImport.FlagNameDataAttachSpecificSSPJ);
			EditorPrefs.SetInt(PrefsKeyTextureSizePixelMaximum, DataSettingImport.TextureSizePixelMaximum);
			EditorPrefs.SetBool(PrefsKeyFlagAttachControlGameObject, DataSettingImport.FlagAttachControlGameObject);
			EditorPrefs.SetBool(PrefsKeyFlagCreateProjectFolder, DataSettingImport.FlagCreateProjectFolder);
			EditorPrefs.SetBool(PrefsKeyFlagDataCalculateInAdvance, DataSettingImport.FlagDataCalculateInAdvance);
			EditorPrefs.SetBool(PrefsKeyFlagDataCompress, DataSettingImport.FlagDataCompress);
			EditorPrefs.SetBool(PrefsKeyFlagTakeOverSettingPrefab, DataSettingImport.FlagDataTakeOverSettingPrefab);

			EditorPrefs.SetFloat(PrefsKeyCollisionThicknessZ, DataSettingImport.CollisionThicknessZ);
			EditorPrefs.SetBool(PrefsKeyFlagAttachRigidBody, DataSettingImport.FlagAttachRigidBody);

			EditorPrefs.SetBool(PrefsKeyFlagConfirmOverWrite, DataSettingImport.FlagConfirmOverWrite);
			EditorPrefs.SetBool(PrefsKeyFlagConfirmOverWriteRoot, DataSettingImport.FlagConfirmOverWriteRoot);
			EditorPrefs.SetBool(PrefsKeyFlagConfirmOverWriteRootEffect, DataSettingImport.FlagConfirmOverWriteRootEffect);
			EditorPrefs.SetBool(PrefsKeyFlagConfirmOverWriteDataCellMap, DataSettingImport.FlagConfirmOverWriteDataCellMap);
			EditorPrefs.SetBool(PrefsKeyFlagConfirmOverWriteDataAnimation, DataSettingImport.FlagConfirmOverWriteDataAnimation);
			EditorPrefs.SetBool(PrefsKeyFlagConfirmOverWriteDataEffect, DataSettingImport.FlagConfirmOverWriteDataEffect);
			EditorPrefs.SetBool(PrefsKeyFlagConfirmOverWriteMaterial, DataSettingImport.FlagConfirmOverWriteMaterial);
			EditorPrefs.SetBool(PrefsKeyFlagConfirmOverWriteTexture, DataSettingImport.FlagConfirmOverWriteTexture);

			EditorPrefs.SetBool(PrefsKeyFlagGetAnimationDataPartsRoot, DataSettingImport.FlagGetAnimationReferencedPartsRoot);
			EditorPrefs.SetBool(PrefsKeyFlagGetCellMapDataPartsRoot, DataSettingImport.FlagGetCellMapReferencedPartsRoot);
			EditorPrefs.SetBool(PrefsKeyFlagGetEffectDataPartsRoot, DataSettingImport.FlagGetEffectReferencedPartsRoot);
			EditorPrefs.SetBool(PrefsKeyFlagGetMaterialPartsRoot, DataSettingImport.FlagGetMaterialPartsRoot);
			EditorPrefs.SetBool(PrefsKeyFlagGetTextureMaterial, DataSettingImport.FlagGetTextureMaterial);
		}
		internal static void SettingGetFolderImport(out string NameFolder)
		{
			string Name64 = "";
			Name64 = EditorPrefs.GetString(PrefsKeyFolderNameImpoertLast, DefaultFolderNameImpoertLast);
			NameFolder = String.Copy(UTF8Encoding.UTF8.GetString(System.Convert.FromBase64String(Name64)));
		}
		internal static void SettingSetFolderImport(ref string NameFolder)
		{
			string Name64 = System.Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(NameFolder));
			EditorPrefs.SetString(PrefsKeyFolderNameImpoertLast, Name64);
		}

		/* Caution: "NameInputFullPathSSPJ" must be Full-Path Name. */
		/* Caution: "NameOutputAssetFolder" must be Relative-Path Name("Assets/"). */
		public static bool ImportSSPJ(	SettingImport DataSettingImport,
										string NameInputFullPathSSPJ="",
										string NameOutputAssetFolderBase="",
										bool FlagSaveSetting=true,
										bool FlagDisplayProgressBar=true,
										bool FlagDisplayErrorDialog=true
									)
		{
			bool FlagValidSSPJ = String.IsNullOrEmpty(NameInputFullPathSSPJ);

			/* Prefs Save */
			if(true == FlagSaveSetting)
			{
				SettingSetImport(ref DataSettingImport);
			}

			/* Select Project,Imported(.sspj) */
			string NameDirectory = "";
			string NameFileBody = "";
			string NameFileExtension = "";
			if(true == FlagValidSSPJ)
			{	/* Select */
				if(false == Utility.File.FileNameGetFileDialog(	out NameDirectory,
																out NameFileBody,
																out NameFileExtension,
																"Select Importing SSPJ-File",
																"sspj"
															)
					)
				{	/* Cancelled */
					return(false);
				}
			}
			else
			{	/* Force */
				if(false == System.IO.File.Exists(NameInputFullPathSSPJ))
				{	/* Not Found */
					Debug.LogError("SSPJ Importing Error: File Not Found [" + NameInputFullPathSSPJ + "]");
					return(false);
				}

				NameDirectory = Path.GetDirectoryName(NameInputFullPathSSPJ);
				NameFileBody = Path.GetFileNameWithoutExtension(NameInputFullPathSSPJ);
				NameFileExtension = Path.GetExtension(NameInputFullPathSSPJ);
			}

			/* Initialize */
			string NameFileSSPJ = NameFileBody + NameFileExtension;
			int StepNow = 0;
			int StepFull = 1;	/* Import-Step: Decoding SSPJ */
			ProgressBarUpdate(	"Decoding Project Files",
								FlagDisplayProgressBar,
								StepNow,
								StepFull
							);
			StepNow++;

			/* ".sspj" Import */
			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationSSPJ = LibraryEditor_SpriteStudio.ParseOPSS.ImportSSPJ(	ref DataSettingImport,
																																	NameDirectory,
																																	NameFileSSPJ
																																);
			if(null == InformationSSPJ)
			{
				goto Menu_ImportSSPJ_ErrorEnd;
			}

			/* Get step-count */
			StepFull += 1;																														/* Import-Step: Decoding SSPJ  */
			StepFull += 1;																														/* Import-Step: Copying Texture-Files */
			StepFull += ((InformationSSPJ.ListNameSSCE.Count + InformationSSPJ.ListNameSSAE.Count + InformationSSPJ.ListNameSSEE.Count) * 2);	/* Import-Step: (SSCE + SSAE + SSEE) * (Decoding + Create) */
			int Count = 0;

			/* ".ssce" Import */
			Count = InformationSSPJ.ListNameSSCE.Count;
			for(int i=0; i<Count; i++)
			{
				ProgressBarUpdate(	"Decoding SSCE Files: " + (i + 1).ToString() + "-" + Count.ToString(),
									FlagDisplayProgressBar,
									StepNow,
									StepFull
								);
				StepNow++;

				InformationSSPJ.ListInformationSSCE[i] = LibraryEditor_SpriteStudio.ParseOPSS.ImportSSCE(	ref DataSettingImport,
																											InformationSSPJ,
																											i
																										);
				if(null == InformationSSPJ.ListInformationSSCE[i])
				{
					goto Menu_ImportSSPJ_ErrorEnd;
				}
			}

			/* Destination Base-Folder Name Create */
			string NameBaseAssetPath = "";
			if(true == String.IsNullOrEmpty(NameOutputAssetFolderBase))
			{	/* Select */
				NameBaseAssetPath = LibraryEditor_SpriteStudio.Utility.File.AssetPathGetSelected(null);
				if(true == String.IsNullOrEmpty(NameBaseAssetPath))
				{
					Debug.LogError(	"SSPJ Importing Error: Please select the folder you want to store in before import."
									+ " File[" + NameFileSSPJ + "]"
								);
					goto Menu_ImportSSPJ_ErrorEnd;
				}
			}
			else
			{	/* Force */
				if(false == System.IO.Directory.Exists(NameOutputAssetFolderBase))
				{	/* Not Found */
					Debug.LogError(	"SSPJ Importing Error: Output-Base-Folder Not Found."
									+ " Folder-Name[" + NameOutputAssetFolderBase + "]"
									+ " File[" + NameFileSSPJ + "]"
								);
					goto Menu_ImportSSPJ_ErrorEnd;
				}
				NameBaseAssetPath = String.Copy(NameOutputAssetFolderBase);
			}
			if(true == DataSettingImport.FlagCreateProjectFolder)
			{
				NameBaseAssetPath = LibraryEditor_SpriteStudio.Utility.File.AssetFolderCreate(NameBaseAssetPath, InformationSSPJ.NameFileBody);
			}
			else
			{
				NameBaseAssetPath = LibraryEditor_SpriteStudio.Utility.File.AssetFolderCreate(NameBaseAssetPath, null);
			}

			/* ".ssee" Import */
			Count = InformationSSPJ.ListNameSSEE.Count;
			for(int i=0; i<Count; i++)
			{
				ProgressBarUpdate(	"Decoding SSEE Files: " + (i + 1).ToString() + "-" + Count.ToString(),
									FlagDisplayProgressBar,
									StepNow,
									StepFull
								);
				StepNow++;

				InformationSSPJ.ListInformationSSEE[i] = LibraryEditor_SpriteStudio.ParseOPSS.ImportSSEE(	ref DataSettingImport,
																											InformationSSPJ,
																											i
																										);
				if(null == InformationSSPJ.ListInformationSSEE[i])
				{
					goto Menu_ImportSSPJ_ErrorEnd;
				}
			}

			/* ".ssae" Import */
			Count = InformationSSPJ.ListNameSSAE.Count;
			for(int i=0; i<Count; i++)
			{
				ProgressBarUpdate(	"Decoding SSAE Files: " + (i + 1).ToString() + "-" + Count.ToString(),
									FlagDisplayProgressBar,
									StepNow,
									StepFull
								);
				StepNow++;

				InformationSSPJ.ListInformationSSAE[i] = LibraryEditor_SpriteStudio.ParseOPSS.ImportSSAE(	ref DataSettingImport,
																											InformationSSPJ,
																											i
																										);
				if(null == InformationSSPJ.ListInformationSSAE[i])
				{
					goto Menu_ImportSSPJ_ErrorEnd;
				}
			}

			/* Set Instance-Data in ".ssae" */
			if(false == IncetanceDataSetAnimationParts(	ref DataSettingImport,
														InformationSSPJ,
														NameFileSSPJ
													)
				)
			{
				goto Menu_ImportSSPJ_ErrorEnd;
			}
			/* Set Effect-Data in ".ssae" */
			if(false == EffectDataSetAnimationParts(	ref DataSettingImport,
														InformationSSPJ,
														NameFileSSPJ
													)
				)
			{
				goto Menu_ImportSSPJ_ErrorEnd;
			}

			/* Set Texture-Data in ".ssce" */
			if(false == TextureDataSetSSCE(	ref DataSettingImport,
											InformationSSPJ,
											NameFileSSPJ
										)
				)
			{
				goto Menu_ImportSSPJ_ErrorEnd;
			}

			/* Converting-Order Get */
			int[] OrderConvertSSAE = OrderGetConvertSSAE(	ref DataSettingImport,
															InformationSSPJ,
															NameFileSSPJ
														);
			if(null == OrderConvertSSAE)
			{
				goto Menu_ImportSSPJ_ErrorEnd;
			}

			/* Existing-Prefabs Get */
			PrefabGetExisting(ref DataSettingImport, InformationSSPJ, NameBaseAssetPath);

			/* Convert Textures */
			ProgressBarUpdate(	"Copying Texture-Files",
								FlagDisplayProgressBar,
								StepNow,
								StepFull
							);
			StepNow++;

			Count = InformationSSPJ.ListNameTexture.Count;
			for(int i=0; i<Count; i++)
			{
				if(false == LibraryEditor_SpriteStudio.Convert.PrefabCreateTexture(	ref DataSettingImport,
																					InformationSSPJ,
																					i,
																					NameBaseAssetPath,
																					NameFileSSPJ
																				)
					)
				{
					goto Menu_ImportSSPJ_ErrorEnd;
				}
			}

			/* Convert Cell-Maps */
			ProgressBarUpdate(	"Creating Asset (CellMaps)",
								FlagDisplayProgressBar,
								StepNow,
								StepFull
							);
			StepNow += InformationSSPJ.ListNameSSCE.Count;

			if(false == LibraryEditor_SpriteStudio.Convert.PrefabCreateSSCE(	ref DataSettingImport,
																				InformationSSPJ,
																				NameBaseAssetPath,
																				NameFileSSPJ
																			)
				)
			{
				goto Menu_ImportSSPJ_ErrorEnd;
			}
			InformationSSPJ.TableMaterialCreate();
			InformationSSPJ.TableMaterialCreateEffect();

			/* Convert Effect */
			Count = InformationSSPJ.ListNameSSEE.Count;
			for(int i=0; i<Count; i++)
			{
				ProgressBarUpdate(	"Creating Asset (Effects): " + (i + 1).ToString() + "-" + Count.ToString(),
									FlagDisplayProgressBar,
									StepNow,
									StepFull
								);
				StepNow++;

				if(false == LibraryEditor_SpriteStudio.Convert.PrefabCreateSSEE(	ref DataSettingImport,
																					InformationSSPJ,
																					i,
																					NameBaseAssetPath,
																					NameFileSSPJ
																				)
					)
				{
					goto Menu_ImportSSPJ_ErrorEnd;
				}
			}

			/* Convert Animation */
			Count = InformationSSPJ.ListNameSSAE.Count;
			for(int i=0; i<Count; i++)
			{
				ProgressBarUpdate(	"Creating Asset (Animations): " + (i + 1).ToString() + "-" + Count.ToString(),
									FlagDisplayProgressBar,
									StepNow,
									StepFull
								);
				StepNow++;

				int IndexSSAE = OrderConvertSSAE[i];
				if(false == LibraryEditor_SpriteStudio.Convert.PrefabCreateSSAE(	ref DataSettingImport,
																					InformationSSPJ,
																					IndexSSAE,
																					NameBaseAssetPath,
																					NameFileSSPJ
																				)
					)
				{
					goto Menu_ImportSSPJ_ErrorEnd;
				}
			}

			/* "Control" Node Attached Prefab Create (SSAE) */
			Count = InformationSSPJ.ListNameSSAE.Count;
			for(int i=0; i<Count; i++)
			{
				if(false == LibraryEditor_SpriteStudio.Convert.PrefabCreateControlObjectSSAE(	ref DataSettingImport,
																								InformationSSPJ,
																								i,
																								NameBaseAssetPath,
																								NameFileSSPJ
																							)
					)
				{
					goto Menu_ImportSSPJ_ErrorEnd;
				}
			}

			/* End of Importing (Success) */
			ProgressBarUpdate("Import End", FlagDisplayProgressBar, -1, -1);
			return(true);

		Menu_ImportSSPJ_ErrorEnd:;
			/* End of Importing (Failure) */
			ProgressBarUpdate("Import Stop", FlagDisplayProgressBar, -1, -1);
			if(true == FlagDisplayErrorDialog)
			{
				EditorUtility.DisplayDialog(	"SpriteStudio5 Player for Unity",
												"Import Interrupted! Check Error on Console.",
										 		"OK"
											);
			}
			return(false);
		}
		private static void ProgressBarUpdate(string NowTaskName, bool FlagSwitch, int Step, int StepFull)
		{
			if(false == FlagSwitch)
			{
				return;
			}

			if((-1 == Step) || (-1 == StepFull))
			{
				EditorUtility.ClearProgressBar();
				return;
			}

			EditorUtility.DisplayProgressBar("Importing SpriteStudio Animation", NowTaskName, ((float)Step / (float)StepFull));
		}
		private static bool IncetanceDataSetAnimationParts(	ref SettingImport DataSettingImport,
															LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
															string FileName
														)
		{
			string MessageError = "";
			int Count = InformationProject.ListNameSSAE.Count;
			int CountParts = -1;
			int IndexSSAE = -1;
			int IndexAnimation = -1;
			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAE InformationAnimationSet = null;
			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAE InformationAnimationSetInstance = null;
			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts InformationParts = null;
			for(int i=0; i<Count; i++)
			{
				InformationAnimationSet = InformationProject.ListInformationSSAE[i];
				CountParts = InformationAnimationSet.ListParts.Length;
				for(int j=0; j<CountParts; j++)
				{
					InformationParts = InformationAnimationSet.ListParts[j];
					if(Library_SpriteStudio.KindParts.INSTANCE == InformationParts.Kind)
					{
#if false
						IndexSSAE = InformationProject.IndexGetFileName(InformationProject.ListNameSSAE, InformationParts.NameSSAEInstance);
#else
						/* MEMO: Determine the target data, from the file name stored in SSPJ. */
						IndexSSAE = InformationProject.IndexGetFileNameBody(InformationProject.ListNameSSAE, InformationParts.NameSSAEInstance);
#endif
						if(-1 == IndexSSAE)
						{
							MessageError = "Instance[" + InformationParts.NameSSAEInstance + "] Not Found."
											+ " SSAE[" + InformationAnimationSet.Name + "]"
											+ " PartsName[" + InformationParts.Name + "]";
							goto Menu_IncetanceDataSetAnimationParts_ErrorEnd;
						}
						InformationParts.IndexSSAEInstance = IndexSSAE;

						InformationAnimationSetInstance = InformationProject.ListInformationSSAE[IndexSSAE];
						IndexAnimation = InformationAnimationSetInstance.IndexGetAnimation(InformationParts.NameAnimationUnderControl);
						if(-1 == IndexAnimation)
						{
							MessageError = "Animation[" + InformationParts.NameAnimationUnderControl + "] Not Found."
											+ " SSAE[" + InformationAnimationSet.Name + "]"
											+ " PartsName[" + InformationParts.Name + "]";
							goto Menu_IncetanceDataSetAnimationParts_ErrorEnd;
						}
						InformationParts.IndexAnimationInstance = IndexAnimation;
					}
				}
			}
			return(true);

		Menu_IncetanceDataSetAnimationParts_ErrorEnd:;
			Debug.LogError("Import-Processing Error (Instance-Data Set): " + MessageError
							+ " SSPJ[" + FileName + "]"
						);
			return(false);
		}
		private static bool EffectDataSetAnimationParts(	ref SettingImport DataSettingImport,
															LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
															string FileName
														)
		{
			string MessageError = "";
			int Count = InformationProject.ListNameSSAE.Count;
			int CountParts = -1;
			int IndexSSEE = -1;
			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAE InformationAnimationSet = null;
			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts InformationParts = null;
			for(int i=0; i<Count; i++)
			{
				InformationAnimationSet = InformationProject.ListInformationSSAE[i];
				CountParts = InformationAnimationSet.ListParts.Length;
				for(int j=0; j<CountParts; j++)
				{
					InformationParts = InformationAnimationSet.ListParts[j];
					if(Library_SpriteStudio.KindParts.EFFECT == InformationParts.Kind)
					{
						if(true == string.IsNullOrEmpty(InformationParts.NameSSEEEffect))
						{
							Debug.LogWarning("Import-Processing Warining (Effect-Data Set): "
												+ "Effect is empty. "
												+ " SSAE[" + InformationAnimationSet.Name + "]"
												+ " PartsName[" + InformationParts.Name + "]"
												+ " SSPJ[" + FileName + "]"
											);
							InformationParts.IndexSSEEEffect = -1;
						}
						else
						{
#if false
							IndexSSEE = InformationProject.IndexGetFileName(InformationProject.ListNameSSEE, InformationParts.NameSSEEEffect);
#else
							/* MEMO: Determine the target data, from the file name stored in SSPJ. */
							IndexSSEE = InformationProject.IndexGetFileNameBody(InformationProject.ListNameSSEE, InformationParts.NameSSEEEffect);
#endif
							if(-1 == IndexSSEE)
							{
								MessageError = "Effect[" + InformationParts.NameSSEEEffect + "] Not Found."
												+ " SSAE[" + InformationAnimationSet.Name + "]"
												+ " PartsName[" + InformationParts.Name + "]";
								goto Menu_EffectDataSetAnimationParts_ErrorEnd;
							}
							InformationParts.IndexSSEEEffect = IndexSSEE;
						}
					}
				}
			}
			return(true);

		Menu_EffectDataSetAnimationParts_ErrorEnd:;
			Debug.LogError("Import-Processing Error (Effect-Data Set): " + MessageError
							+ " SSPJ[" + FileName + "]"
						);
			return(false);
		}
		private static bool TextureDataSetSSCE(	ref SettingImport DataSettingImport,
												LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
												string FileName
											)
		{
			string MessageError = "";
			int CountTexture = InformationProject.ListNameTexture.Count;
			int CountSSCE = InformationProject.ListNameSSCE.Count;

			InformationProject.ListInformationTexture = new LibraryEditor_SpriteStudio.ParseOPSS.InformationTexture[CountTexture];
			if(null == InformationProject.ListInformationTexture)
			{
				MessageError = "Not Enough Memory.";
				goto Menu_TextureDataSetSSCE_ErrorEnd;
			}
			for(int i=0; i<CountTexture; i++)
			{
				InformationProject.ListInformationTexture[i] = null;
			}

			/* Texture Data Create */
			int IndexTexture;
			string NamePathTexture = null;
			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSCE InformationSSCE = null;
			LibraryEditor_SpriteStudio.ParseOPSS.InformationTexture InformationTexture = null;
			for(int i=0; i<CountSSCE; i++)
			{
				InformationSSCE = InformationProject.ListInformationSSCE[i];
				IndexTexture = InformationSSCE.IndexTexture;
				NamePathTexture = (string)(InformationProject.ListNameTexture[IndexTexture]);
				InformationTexture = InformationProject.ListInformationTexture[IndexTexture];
				if(null == InformationTexture)
				{
					InformationTexture = new LibraryEditor_SpriteStudio.ParseOPSS.InformationTexture();
					InformationTexture.CleanUp();
					InformationTexture.ListPrefabMaterial = new Material[ShaderOperationMax];
					InformationTexture.ListNamePrefabMaterial = new string[ShaderOperationMax];
					for(int j=0; j<ShaderOperationMax; j++)
					{
						InformationTexture.ListPrefabMaterial[j] = null;
						InformationTexture.ListNamePrefabMaterial[j] = null;
					}
					InformationTexture.ListPrefabMaterialEffect = new Material[ShaderOperationMaxEffect];
					InformationTexture.ListNamePrefabMaterialEffect = new string[ShaderOperationMaxEffect];
					for(int j=0; j<ShaderOperationMaxEffect; j++)
					{
						InformationTexture.ListPrefabMaterialEffect[j] = null;
						InformationTexture.ListNamePrefabMaterialEffect[j] = null;
					}

					InformationProject.ListInformationTexture[IndexTexture] = InformationTexture;

					InformationTexture.Name = LibraryEditor_SpriteStudio.Utility.Text.PathGetRelative(	NamePathTexture,
																										InformationProject,
																										LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.TEXTURE
																									);
					InformationTexture.Name = LibraryEditor_SpriteStudio.Utility.Text.DataNameGetFromPath(InformationTexture.Name, DataSettingImport.FlagNameDataRuleOld);
					InformationTexture.WrapTexture = InformationSSCE.WrapTexture;
					InformationTexture.FilterTexture = InformationSSCE.FilterTexture;
//					InformationTexture.SizeX =
//					InformationTexture.SizeY =
					InformationTexture.NameDirectory = Path.GetDirectoryName(NamePathTexture) + "/";
					InformationTexture.NameFileBody = Path.GetFileNameWithoutExtension(NamePathTexture);
					InformationTexture.NameFileExtension = Path.GetExtension(NamePathTexture);
				}
			}
			return(true);

		Menu_TextureDataSetSSCE_ErrorEnd:;
			Debug.LogError("Import-Processing Error (Texture-Data Set): " + MessageError
							+ " SSPJ[" + FileName + "]"
						);
			return(false);
		}
		private static int[] OrderGetConvertSSAE(	ref SettingImport DataSettingImport,
													LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
													string FileName
												)
		{
			string MessageError = "";
			int Count = InformationProject.ListNameSSAE.Count;
			int Index = 0;

			int[] Order = new int[Count];
			if(null == Order)
			{	/* Error */
				MessageError = "Not Enough Memory. (Convert-Order SSAE) ";
				goto Menu_OrderGetConvertSSAE_ErrorEnd;
			}
			for(int i=0; i<Count; i++)
			{
				Order[i] = -1;
			}

			/* Initial-SSAE (has no Instance-Pars) Set */
			for(int i=0; i<Count; i++)
			{
				if(0 >= CountGetOrderConvertSSAE(ref DataSettingImport, InformationProject, InformationProject.ListInformationSSAE[i]))
				{
					Order[Index] = i;
					Index++;
				}
			}

			/* SSAE (has Instance-Parts) Set */
			bool FlagAlreadyOrdered = false;
			bool FlagAllInstanceExist = false;
			while(Count > Index)
			{
				for(int i=0; i<Count; i++)
				{
					/* Check Already-Ordered */
					FlagAlreadyOrdered = false;
					for(int j=0; j<Index; j++)
					{
						if(i == Order[j])
						{	/* Already Set */
							FlagAlreadyOrdered = true;
							break;
						}
					}
					if(true == FlagAlreadyOrdered)
					{
						continue;
					}

					/* Check Calling-InstanceParts are ordered */
					FlagAllInstanceExist = false;
					if(false == OrderCheckInstanceParts(	ref FlagAllInstanceExist,
															ref DataSettingImport,
															InformationProject,
															InformationProject.ListInformationSSAE[i],
															Order,
															Index
														)
						)
					{	/* Error (Not Found SSAE-Name) */
						MessageError = "Insntace-Index Missing. SSAE[" + (string)InformationProject.ListNameSSAE[i] + "]";
						goto Menu_OrderGetConvertSSAE_ErrorEnd;
					}
					if(true == FlagAllInstanceExist)
					{	/* All Instance-Parts Orderd */
						Order[Index] = i;
						Index++;
						break;	/* Break for-Loop */
					}
				}
			}
			return(Order);

		Menu_OrderGetConvertSSAE_ErrorEnd:;
			Debug.LogError("Import-Processing Error (Convert-Order Determining): " + MessageError
							+ " SSPJ[" + FileName + "]"
						);
			return(null);
		}
		private static int CountGetOrderConvertSSAE(	ref SettingImport DataSettingImport,
														LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
														LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAE InformationAnimationSet
													)
		{
			int CountParts = InformationAnimationSet.ListParts.Length;
			int Count = 0;
			for(int i=0; i<CountParts; i++)
			{
				if(Library_SpriteStudio.KindParts.INSTANCE == InformationAnimationSet.ListParts[i].Kind)
				{
					Count++;
				}
			}
			return(Count);
		}
		private static bool OrderCheckInstanceParts(	ref bool FlagAllInstanceExist,
														ref SettingImport DataSettingImport,
														LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
														LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAE InformationAnimationSet,
														int[] Order,
														int CountOrder
													)
		{
			int CountParts = InformationAnimationSet.ListParts.Length;
			int IndexSSAEInstance;
			bool FlagExist;
			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts InformationParts;
			for(int i=0; i<CountParts; i++)
			{
				InformationParts = InformationAnimationSet.ListParts[i];
				if(Library_SpriteStudio.KindParts.INSTANCE == InformationParts.Kind)
				{
					IndexSSAEInstance = InformationParts.IndexSSAEInstance;
					if(-1 == IndexSSAEInstance)
					{
						FlagAllInstanceExist = false;
						return(false);
					}
					FlagExist = false;
					for(int j=0; j<CountOrder; j++)
					{
						if(Order[j] == IndexSSAEInstance)
						{
							FlagExist = true;
							break;
						}
					}
					if(false == FlagExist)
					{
						FlagAllInstanceExist = false;
						return(true);
					}
				}
			}
			FlagAllInstanceExist = true;
			return(true);
		}
		private static bool PrefabGetExisting(	ref SettingImport DataSettingImport,
												LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
												string NameBaseAssetPath
											)
		{
			int Count;
			string NamePathAsset = null;
			GameObject AssetGameObjectRoot = null;
			Script_SpriteStudio_Root ScriptRootAsset = null;
			Script_SpriteStudio_RootEffect ScriptRootEffectAsset = null;

			/* SSAE-Prefabs Get */
			Count = InformationProject.ListInformationSSAE.Length;
			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAE InformationAnimationSet = null;
			for(int i=0; i<Count; i++)
			{
				InformationAnimationSet = InformationProject.ListInformationSSAE[i];

				/* Create Default-Asset-Name (GameObject-Prefab) */
				/* MEMO: This data is specified in the Asset-Path. */
				NamePathAsset = NameBaseAssetPath + "/"
								+ NamePathSubImportPrefab + "/"
								+ Utility.File.NameAssetBodyGet(	InformationAnimationSet.Name,
																	null,	/* NamePrefixPrefab */
																	InformationProject.NameFileBody,
																	DataSettingImport.FlagNameDataAttachSpecificToPrefab,
																	ref DataSettingImport
																)
								+ NameExtensionPrefab;

				AssetGameObjectRoot = AssetDatabase.LoadAssetAtPath(NamePathAsset, typeof(GameObject)) as GameObject;
				ScriptRootAsset = (null != AssetGameObjectRoot) ? (AssetGameObjectRoot.GetComponent<Script_SpriteStudio_Root>()) : null;
				InformationAnimationSet.NamePrefabGameObject = NamePathAsset;
				InformationAnimationSet.PrefabGameObject = AssetGameObjectRoot;

				/* Create Default-Asset-Name (Static Animation-Data) */
				NamePathAsset = NameBaseAssetPath + "/"
								+ NamePathSubImportAnimation + "/"
								+ Utility.File.NameAssetBodyGet(	InformationAnimationSet.Name,
																	NamePrefixAnimation,
																	InformationProject.NameFileBody,
																	true,
																	ref DataSettingImport
																)
								+ NameExtensionAnimation;

				ScriptableObject AssetDataAnimation = null;
				if(true == DataSettingImport.FlagGetAnimationReferencedPartsRoot)
				{
					AssetDataAnimation = (null != ScriptRootAsset) ? ScriptRootAsset.DataAnimation : null;
				}
				else
				{
					AssetDataAnimation = AssetDatabase.LoadAssetAtPath(NamePathAsset, typeof(ScriptableObject)) as ScriptableObject;
				}
				InformationAnimationSet.PrefabDataAnimation = AssetDataAnimation;
				InformationAnimationSet.NamePrefabDataAnimation = (null != AssetDataAnimation) ? AssetDatabase.GetAssetPath(AssetDataAnimation) : NamePathAsset;
			}

			/* SSEE-Prefabs Get */
			Count = InformationProject.ListInformationSSEE.Length;
			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSEE InformationEffectSet = null;
			for(int i=0; i<Count; i++)
			{
				InformationEffectSet = InformationProject.ListInformationSSEE[i];

				/* Create Default-Asset-Name (GameObject-Prefab) */
				/* MEMO: This data is specified in the Asset-Path. */
				NamePathAsset = NameBaseAssetPath + "/"
								+ NamePathSubImportPrefabEffect + "/"
								+ Utility.File.NameAssetBodyGet(	InformationEffectSet.Name,
																	NamePrefixPrefabEffect,
																	InformationProject.NameFileBody,
																	DataSettingImport.FlagNameDataAttachSpecificToPrefab,
																	ref DataSettingImport
																)
								+ NameExtensionPrefabEffect;	// + InformationEffectSet.Name + NameExtensionPrefabEffect;

				AssetGameObjectRoot = AssetDatabase.LoadAssetAtPath(NamePathAsset, typeof(GameObject)) as GameObject;
				ScriptRootEffectAsset = (null != AssetGameObjectRoot) ? (AssetGameObjectRoot.GetComponent<Script_SpriteStudio_RootEffect>()) : null;
				InformationEffectSet.NamePrefabGameObject = NamePathAsset;
				InformationEffectSet.PrefabGameObject = AssetGameObjectRoot;

				/* Create Default-Asset-Name (Static Effect-Data) */
				NamePathAsset = NameBaseAssetPath + "/"
								+ NamePathSubImportEffect + "/"
								+ Utility.File.NameAssetBodyGet(	InformationEffectSet.Name,
																	NamePrefixEffect,
																	InformationProject.NameFileBody,
																	true,
																	ref DataSettingImport
																)
								+ NameExtensionEffect;
				ScriptableObject AssetDataEffect = null;
				if(true == DataSettingImport.FlagGetAnimationReferencedPartsRoot)
				{
					AssetDataEffect = (null != ScriptRootEffectAsset) ? ScriptRootEffectAsset.DataEffect : null;
				}
				else
				{
					AssetDataEffect = AssetDatabase.LoadAssetAtPath(NamePathAsset, typeof(ScriptableObject)) as ScriptableObject;
				}
				InformationEffectSet.PrefabDataEffect = AssetDataEffect;
				InformationEffectSet.NamePrefabDataEffect = (null != AssetDataEffect) ? AssetDatabase.GetAssetPath(AssetDataEffect) : NamePathAsset;
			}

			/* SSCE-Prefabs Get */
			Count = InformationProject.ListInformationSSCE.Length;
			for(int i=0; i<Count; i++)
			{
				/* Create Default-Asset-Name (Static CellMap-Data) */
				NamePathAsset = NameBaseAssetPath + "/"
								+ NamePathSubImportCellMap + "/"
								+ Utility.File.NameAssetBodyGet(	InformationProject.NameFileBody,
																	null,	/* NamePrefixCellMap, */
																	null,
																	false,
																	ref DataSettingImport
																)
								+ NameExtensionCellMap;
				InformationProject.PrefabCell = null;
				InformationProject.NamePrefabCell = NamePathAsset;
			}

			/* Texture Get */
			Count = InformationProject.ListInformationTexture.Length;
			LibraryEditor_SpriteStudio.ParseOPSS.InformationTexture InformationTexture = null;
			for(int i=0; i<Count; i++)
			{
				InformationTexture = InformationProject.ListInformationTexture[i];

				/* Create Default-Asset-Name */
				NamePathAsset = NameBaseAssetPath + "/"
								+ NamePathSubImportTexture + "/"
								+ Utility.File.NameAssetBodyGet(	InformationTexture.Name,
																	null,	/* NamePrefixTexture */
																	InformationProject.NameFileBody,
																	DataSettingImport.FlagNameDataAttachSpecificToTexture,
																	ref DataSettingImport
																)
								+ InformationTexture.NameFileExtension;

				InformationTexture.PrefabTexture = null;
				InformationTexture.NamePrefabTexture = NamePathAsset;

				/* Create Default-Asset-Name */
				Library_SpriteStudio.KindColorOperation KindColorOperation;
				for(int j=0; j<ShaderOperationMax; j++)
				{
					KindColorOperation = (Library_SpriteStudio.KindColorOperation)(j + 1);	/* +1 == ".NON" */
					NamePathAsset = NameBaseAssetPath + "/"
									+ NamePathSubImportMaterial + "/"
									+ Utility.File.NameAssetBodyGet(	InformationTexture.Name,
																		NamePrefixMaterial,
																		InformationProject.NameFileBody,
																		true,
																		ref DataSettingImport
																	) + "_" + (KindColorOperation.ToString())
									+ NameExtensionMaterial;
					InformationTexture.ListPrefabMaterial[j] = null;
					InformationTexture.ListNamePrefabMaterial[j] = NamePathAsset;
				}
				Library_SpriteStudio.KindColorOperationEffect KindColorOperationEffect;
				for(int j=0; j<ShaderOperationMaxEffect; j++)
				{
					KindColorOperationEffect = (Library_SpriteStudio.KindColorOperationEffect)(j + 1);	/* +1 == ".NON" */
					NamePathAsset = NameBaseAssetPath + "/"
									+ NamePathSubImportMaterialEffect + "/"
									+ Utility.File.NameAssetBodyGet(	InformationTexture.Name,
																		NamePrefixMaterialEffect,
																		InformationProject.NameFileBody,
																		true,
																		ref DataSettingImport
																	) + "_" + (KindColorOperationEffect.ToString())
									+ NameExtensionMaterialEffect;
					InformationTexture.ListPrefabMaterialEffect[j] = null;
					InformationTexture.ListNamePrefabMaterialEffect[j] = NamePathAsset;
				}
			}

			return(true);
		}
	}

	/* Functions for Parsing OPSS(SS5) Datas */
	internal static class ParseOPSS
	{
		/* for Parsing ".sspj" */
		internal static LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ ImportSSPJ(	ref SettingImport DataSettingImport,
																							string DataPathBase,
																							string FileName
																						)
		{
			string MessageError = "";
			string FileNameLoad = DataPathBase + "/" + FileName;

			/* ".sspj" Load */
			XmlDocument DataXML = new XmlDocument();
			DataXML.Load(FileNameLoad);

			/* Version-Check */
			XmlNode NodeRoot = DataXML.FirstChild;
			NodeRoot = NodeRoot.NextSibling;
			KindVersionSSPJ VersionCode = (KindVersionSSPJ)(LibraryEditor_SpriteStudio.Utility.XML.VersionCodeGet(NodeRoot, "SpriteStudioProject", (int)KindVersionSSPJ.ERROR, true));
			switch(VersionCode)
			{
				case KindVersionSSPJ.VERSION_000100:
				case KindVersionSSPJ.VERSION_010000:
				case KindVersionSSPJ.VERSION_010200:
				case KindVersionSSPJ.VERSION_010201:
					break;

				case KindVersionSSPJ.ERROR:
					MessageError = "Not Supported Version.";
					goto ParseOPSS_ImportSSPJ_ErrorEnd;

				default:
					if(KindVersionSSPJ.VERSION_LATEST < VersionCode)
					{	/* MEMO: Dealing as the latest supported version. */
						VersionCode = KindVersionSSPJ.VERSION_LATEST;
						goto case KindVersionSSPJ.VERSION_LATEST;
					}
					goto case KindVersionSSPJ.ERROR;
			}

			/* Create WorkArea */
			InformationSSPJ InformationProject = new LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ();
			if(null == InformationProject)
			{
				MessageError = "Not Enough Memory.";
				goto ParseOPSS_ImportSSPJ_ErrorEnd;
			}

			InformationProject.VersionCode = VersionCode;
			string FileNameNormalized = LibraryEditor_SpriteStudio.Utility.Text.PathGetAbsolute(FileNameLoad);
			InformationProject.NameDirectory = Path.GetDirectoryName(FileNameNormalized) + "/";
			InformationProject.NameFileBody = Path.GetFileNameWithoutExtension(FileNameNormalized);
			InformationProject.NameFileExtension = Path.GetExtension(FileNameNormalized);

			NameTable NodeNameSpace = new NameTable();
			XmlNamespaceManager ManagerNameSpace = new XmlNamespaceManager(NodeNameSpace);

			/* Get Project-Setting */
			InformationProject.NameDirectoryBaseSSAE = string.Copy(InformationProject.NameDirectory);
			string ValueText = "";
			ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeRoot, "settings/animeBaseDirectory", ManagerNameSpace);
			if(false == string.IsNullOrEmpty(ValueText))
			{
				InformationProject.NameDirectoryBaseSSAE = LibraryEditor_SpriteStudio.Utility.Text.PathGetAbsolute(InformationProject.NameDirectoryBaseSSAE + ValueText + "/");
			}

			InformationProject.NameDirectoryBaseSSCE = string.Copy(InformationProject.NameDirectory);
			ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeRoot, "settings/cellMapBaseDirectory", ManagerNameSpace);
			if(false == string.IsNullOrEmpty(ValueText))
			{
				InformationProject.NameDirectoryBaseSSCE = LibraryEditor_SpriteStudio.Utility.Text.PathGetAbsolute(InformationProject.NameDirectoryBaseSSCE + ValueText + "/");
			}

			InformationProject.NameDirectoryBaseSSEE = string.Copy(InformationProject.NameDirectory);
			ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeRoot, "settings/effectBaseDirectory", ManagerNameSpace);
			if(false == string.IsNullOrEmpty(ValueText))
			{
				InformationProject.NameDirectoryBaseSSEE = LibraryEditor_SpriteStudio.Utility.Text.PathGetAbsolute(InformationProject.NameDirectoryBaseSSEE + ValueText + "/");
			}

			InformationProject.NameDirectoryBaseTexture = string.Copy(InformationProject.NameDirectory);
			ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeRoot, "settings/imageBaseDirectory", ManagerNameSpace);
			if(false == string.IsNullOrEmpty(ValueText))
			{
				InformationProject.NameDirectoryBaseTexture = LibraryEditor_SpriteStudio.Utility.Text.PathGetAbsolute(InformationProject.NameDirectoryBaseTexture + ValueText + "/");
			}

			ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeRoot, "settings/wrapMode", ManagerNameSpace);
			switch(ValueText)
			{
				case "repeat":
					InformationProject.WrapTexture = Library_SpriteStudio.KindWrapTexture.REPEAT;
					break;

				case "mirror":
					Debug.LogWarning("SSPJ-Import Warning: Texture Wrap-Mode \"Mirror\" is not Suppoted. Force-Changed \"Clamp\""
										+ " SSPJ[" + FileName + "]"
								);
					goto case "clamp";

				case "clamp":
				default:
					InformationProject.WrapTexture = Library_SpriteStudio.KindWrapTexture.CLAMP;
					break;
			}

			ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeRoot, "settings/filterMode", ManagerNameSpace);
			switch(ValueText)
			{
				case "nearlest":
					InformationProject.FilterTexture = Library_SpriteStudio.KindFilterTexture.NEAREST;
					break;

				case "linear":
				default:
					InformationProject.FilterTexture = Library_SpriteStudio.KindFilterTexture.LINEAR;
					break;
			}

			XmlNodeList NodeList = null;

			/* Get Cell-Maps */
			InformationProject.ListNameSSCE = new ArrayList();
			InformationProject.ListNameSSCE.Clear();

			NodeList = LibraryEditor_SpriteStudio.Utility.XML.ListGetNode(NodeRoot, "cellmapNames/value", ManagerNameSpace);
			if(null == NodeList)
			{
				MessageError = "CellMapNameList-Node Not-Found.";
				goto ParseOPSS_ImportSSPJ_ErrorEnd;
			}
			foreach(XmlNode NodeNameCellMap in NodeList)
			{
				string NameFile = NodeNameCellMap.InnerText;
				NameFile = LibraryEditor_SpriteStudio.Utility.Text.PathGetAbsolute(	NameFile,
																					InformationProject,
																					LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.SSCE
																				);
				InformationProject.AddSSCE(NameFile);
			}
			InformationProject.ListInformationSSCE = new LibraryEditor_SpriteStudio.ParseOPSS.InformationSSCE[InformationProject.ListNameSSCE.Count];

			/* Get Animations */
			InformationProject.ListNameSSAE = new ArrayList();
			InformationProject.ListNameSSAE.Clear();

			NodeList = LibraryEditor_SpriteStudio.Utility.XML.ListGetNode(NodeRoot, "animepackNames/value", ManagerNameSpace);
			if(null == NodeList)
			{
				MessageError = "AnimePackNameList-Node Not-Found.";
				goto ParseOPSS_ImportSSPJ_ErrorEnd;
			}
			foreach(XmlNode NodeNameAnimation in NodeList)
			{
				string NameFile = NodeNameAnimation.InnerText;
				NameFile = LibraryEditor_SpriteStudio.Utility.Text.PathGetAbsolute(	NameFile,
																					InformationProject,
																					LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.SSAE
																				);
				InformationProject.AddSSAE(NameFile);
			}
			InformationProject.ListInformationSSAE = new LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAE[InformationProject.ListNameSSAE.Count];

			/* Get Effects */
			InformationProject.ListNameSSEE = new ArrayList();
			InformationProject.ListNameSSEE.Clear();

			NodeList = LibraryEditor_SpriteStudio.Utility.XML.ListGetNode(NodeRoot, "effectFileNames/value", ManagerNameSpace);
			if(null == NodeList)
			{
				MessageError = "EffectNameList-Node Not-Found.";
				goto ParseOPSS_ImportSSPJ_ErrorEnd;
			}
			foreach(XmlNode NodeEffect in NodeList)
			{
				string NameFile = NodeEffect.InnerText;
				NameFile = LibraryEditor_SpriteStudio.Utility.Text.PathGetAbsolute(	NameFile,
																					InformationProject,
																					LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.SSEE
																				);
				InformationProject.AddSSEE(NameFile);
			}
			InformationProject.ListInformationSSEE = new LibraryEditor_SpriteStudio.ParseOPSS.InformationSSEE[InformationProject.ListNameSSEE.Count];

			return(InformationProject);

		ParseOPSS_ImportSSPJ_ErrorEnd:;
			Debug.LogError("SSPJ-Import Error: " + MessageError
							+ " SSPJ[" + FileName + "]"
						);
			return(null);
		}
		internal enum KindVersionSSPJ
		{
			ERROR = 0x00000000,
			VERSION_000100 = 0x00000100,
			VERSION_010000 = 0x00010000,
			VERSION_010200 = 0x00010200,	/* sspj ver. 5.5.0 beta-3 */
			VERSION_010201 = 0x00010201,	/* sspj ver. 5.7.0 beta */

			VERSION_LATEST = VERSION_010201
		}
		internal class InformationSSPJ
		{
			internal enum KindFile
			{
				NON = -1,	/* Through */
				SSPJ = 0,
				SSCE,
				SSAE,
				SSEE,
				TEXTURE
			}

			/* Project Setting: SSPJ */
			internal KindVersionSSPJ VersionCode;

			/* Project Setting: Base-Directory */
			internal string NameDirectoryBaseSSCE;
			internal string NameDirectoryBaseSSAE;
			internal string NameDirectoryBaseSSEE;
			internal string NameDirectoryBaseTexture;

			/* Project Setting: Texture Mode */
			internal Library_SpriteStudio.KindWrapTexture WrapTexture;
			internal Library_SpriteStudio.KindFilterTexture FilterTexture;

			/* Project Data: SSCE Datas */
			internal ArrayList ListNameSSCE;
			internal InformationSSCE[] ListInformationSSCE;

			/* Project Data: SSAE Datas */
			internal ArrayList ListNameSSAE;
			internal InformationSSAE[] ListInformationSSAE;

			/* Project Data: SSEE Datas */
			internal ArrayList ListNameSSEE;
			internal InformationSSEE[] ListInformationSSEE;

			/* Project Data: Texture */
			internal ArrayList ListNameTexture;
			internal InformationTexture[] ListInformationTexture;

			/* WorkArea: (SSPJ File's) File/Path Name */
			internal string NameDirectory;
			internal string NameFileBody;
			internal string NameFileExtension;

			/* Converted Data: Runtime-Datas */
			internal ScriptableObject PrefabCell;
			internal string NamePrefabCell;
			internal Material[] TableMaterial;
			internal Material[] TableMaterialEffect;

			internal void CleanUp()
			{
				VersionCode = LibraryEditor_SpriteStudio.ParseOPSS.KindVersionSSPJ.ERROR;
				NameDirectoryBaseSSCE = "";
				NameDirectoryBaseSSAE = "";
				NameDirectoryBaseSSEE = "";
				NameDirectoryBaseTexture = "";

				WrapTexture = Library_SpriteStudio.KindWrapTexture.CLAMP;
				FilterTexture = Library_SpriteStudio.KindFilterTexture.NEAREST;

				ListNameSSCE = null;
				ListInformationSSCE = null;

				ListNameSSAE = null;
				ListInformationSSAE = null;

				ListNameSSEE = null;
				ListInformationSSEE = null;

				ListNameTexture = null;
				ListInformationTexture = null;

				NameDirectory = "";
				NameFileBody = "";
				NameFileExtension = "";

				PrefabCell = null;
				NamePrefabCell = "";
				TableMaterial = null;
				TableMaterialEffect = null;
			}

			internal int AddSSCE(string FileName)
			{
				int Index = IndexGetFileName(ListNameSSCE, FileName);
				if(0 > Index)
				{	/* New SSCE */
					string FileNameNew = string.Copy(FileName);
					ListNameSSCE.Add(FileNameNew);
					Index = ListNameSSCE.Count - 1;
				}
				return(Index);
			}
			internal int AddSSAE(string FileName)
			{
				int Index = IndexGetFileName(ListNameSSAE, FileName);
				if(0 > Index)
				{	/* New SSAE */
					string FileNameNew = string.Copy(FileName);
					ListNameSSAE.Add(FileNameNew);
					Index = ListNameSSAE.Count - 1;
				}
				return(Index);
			}
			internal int AddSSEE(string FileName)
			{
				int Index = IndexGetFileName(ListNameSSEE, FileName);
				if(0 > Index)
				{	/* New SSEE */
					string FileNameNew = string.Copy(FileName);
					ListNameSSEE.Add(FileNameNew);
					Index = ListNameSSEE.Count - 1;
				}
				return(Index);
			}
			internal int AddTexture(string FileName)
			{
				if(null == ListNameTexture)
				{
					ListNameTexture = new ArrayList();
					ListNameTexture.Clear();
				}

				int Index = IndexGetFileName(ListNameTexture, FileName);
				if(0 > Index)
				{	/* New SSAE */
					string FileNameNew = string.Copy(FileName);
					ListNameTexture.Add(FileNameNew);
					Index = ListNameTexture.Count - 1;
				}
				return(Index);
			}

			internal int IndexGetFileName(ArrayList ListFileName, string FileName)
			{
				if(null != ListFileName)
				{
					for(int i=0; i<ListFileName.Count; i++)
					{
						string FileNameNow = ListFileName[i] as string;
						if(0 == FileName.CompareTo(FileNameNow))
						{
							return(i);
						}
					}
				}
				return(-1);
			}
			internal int IndexGetFileNameBody(ArrayList ListFileName, string FileName)
			{
				if(null != ListFileName)
				{
					for(int i=0; i<ListFileName.Count; i++)
					{
						string FileNameBody = NameGetFileBody(ListFileName, i);
						if(0 == FileName.CompareTo(FileNameBody))
						{
							return(i);
						}
					}
				}
				return(-1);
			}

			internal string NameGetFileBody(ArrayList ListFileName, int Index)
			{
				string FileNameNow = ListFileName[Index] as string;
				string FileNameBody = Path.GetFileNameWithoutExtension(FileNameNow);
				return(FileNameBody);
			}
			internal string NameGetFilePathBody(ArrayList ListFileName, int Index)
			{
				string FileNameNow = ListFileName[Index] as string;
				string FileNamePath = Path.GetDirectoryName(FileNameNow);
				string FileNameBody = Path.GetFileNameWithoutExtension(FileNameNow);
				return(FileNamePath + "/" + FileNameBody);
			}

			internal bool TableMaterialCreate()
			{
				int CountSSCE = ListInformationSSCE.Length;
				int Index;
				LibraryEditor_SpriteStudio.ParseOPSS.InformationTexture InformationTexture = null;
				TableMaterial = new Material[CountSSCE * ShaderOperationMax];
				for(int i=0; i<CountSSCE; i++)
				{
					InformationTexture = ListInformationTexture[ListInformationSSCE[i].IndexTexture];
					Index = i * ShaderOperationMax;
					for(int j=0; j<ShaderOperationMax; j++)
					{
						TableMaterial[Index + j] = InformationTexture.ListPrefabMaterial[j];
					}
				}
				return(true);
			}

			internal bool TableMaterialCreateEffect()
			{
				int CountSSCE = ListInformationSSCE.Length;
				int Index;
				LibraryEditor_SpriteStudio.ParseOPSS.InformationTexture InformationTexture = null;
				TableMaterialEffect = new Material[CountSSCE * ShaderOperationMaxEffect];
				for(int i=0; i<CountSSCE; i++)
				{
					InformationTexture = ListInformationTexture[ListInformationSSCE[i].IndexTexture];
					Index = i * ShaderOperationMaxEffect;
					for(int j=0; j<ShaderOperationMaxEffect; j++)
					{
						TableMaterialEffect[Index + j] = InformationTexture.ListPrefabMaterialEffect[j];
					}
				}
				return(true);
			}
		}
		internal class InformationTexture
		{
			/* Base Data */
			internal string Name;

			/* SSCE Data: Image Datas */
			internal Library_SpriteStudio.KindWrapTexture WrapTexture;
			internal Library_SpriteStudio.KindFilterTexture FilterTexture;

			/* WorkArea: Original Texture Size (Determine when Copy into Asset) */
			internal int SizeX;
			internal int SizeY;

			/* WorkArea: File/Path Name */
			internal string NameDirectory;
			internal string NameFileBody;
			internal string NameFileExtension;

			/* Converted Data: Runtime-Datas */
			internal Texture2D PrefabTexture;
			internal string NamePrefabTexture;
			internal Material[] ListPrefabMaterial;
			internal string[] ListNamePrefabMaterial;
			internal Material[] ListPrefabMaterialEffect;
			internal string[] ListNamePrefabMaterialEffect;

			internal void CleanUp()
			{
				Name = "";

				WrapTexture = Library_SpriteStudio.KindWrapTexture.CLAMP;
				FilterTexture = Library_SpriteStudio.KindFilterTexture.NEAREST;

				SizeX = -1;
				SizeY = -1;

				NameDirectory = "";
				NameFileBody = "";
				NameFileExtension = "";

				PrefabTexture = null;
				NamePrefabTexture = "";
				ListPrefabMaterial = null;
				ListNamePrefabMaterial = null;
			}
		}

		/* for Parsing ".ssce" */
		internal static LibraryEditor_SpriteStudio.ParseOPSS.InformationSSCE ImportSSCE(	ref SettingImport DataSettingImport,
																							InformationSSPJ InformationProject,
																							int IndexSSCE
																						)
		{
			string MessageError = "";
			string NameOptions = "";

			/* Open SSCE-File */
			if((0 > IndexSSCE) || (InformationProject.ListNameSSCE.Count <= IndexSSCE))
			{
				MessageError = "SSCE Index[" + IndexSSCE.ToString() + "] is Invalid.";
				goto ParseOPSS_ImportSSCE_ErrorEnd;
			}
			string NamePathSSCE = (string)InformationProject.ListNameSSCE[IndexSSCE];
			XmlDocument DataXML = new XmlDocument();
			DataXML.Load(NamePathSSCE);

			/* Create WorkArea */
			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSCE InformationCellMap = new LibraryEditor_SpriteStudio.ParseOPSS.InformationSSCE();
			if(null == InformationCellMap)
			{
				MessageError = "Not Enough Memory (SSCE Work-Area)";
				goto ParseOPSS_ImportSSCE_ErrorEnd;
			}
			InformationCellMap.CleanUp();
			InformationCellMap.Name = LibraryEditor_SpriteStudio.Utility.Text.PathGetRelative(	NamePathSSCE,
																								InformationProject,
																								LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.SSCE
																							);
			InformationCellMap.Name = LibraryEditor_SpriteStudio.Utility.Text.DataNameGetFromPath(InformationCellMap.Name, DataSettingImport.FlagNameDataRuleOld);
			InformationCellMap.NameDirectory = Path.GetDirectoryName(NamePathSSCE) + "/";
			InformationCellMap.NameFileBody = Path.GetFileNameWithoutExtension(NamePathSSCE);
			InformationCellMap.NameFileExtension = Path.GetExtension(NamePathSSCE);

			/* Version-Check */
			XmlNode NodeRoot = DataXML.FirstChild;
			NodeRoot = NodeRoot.NextSibling;
			KindVersionSSCE VersionCode = (KindVersionSSCE)(LibraryEditor_SpriteStudio.Utility.XML.VersionCodeGet(NodeRoot, "SpriteStudioCellMap", (int)KindVersionSSCE.ERROR, true));
			switch(VersionCode)
			{
				case KindVersionSSCE.VERSION_000100:
				case KindVersionSSCE.VERSION_010000:
				case KindVersionSSCE.VERSION_010001:
					break;

				case KindVersionSSCE.ERROR:
					MessageError = "Not Supported Version.";
					goto ParseOPSS_ImportSSCE_ErrorEnd;

				default:
					if(KindVersionSSCE.VERSION_LATEST < VersionCode)
					{	/* MEMO: Dealing as the latest supported version. */
						VersionCode = KindVersionSSCE.VERSION_LATEST;
						goto case KindVersionSSCE.VERSION_LATEST;
					}
					goto case KindVersionSSCE.ERROR;
			}
			InformationCellMap.VersionCode = VersionCode;

			NameTable NodeNameSpace = new NameTable();
			XmlNamespaceManager ManagerNameSpace = new XmlNamespaceManager(NodeNameSpace);
			XmlNodeList NodeList = null;

			/* Get Texture Path-Name */
			string NameTexture = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeRoot, "imagePath", ManagerNameSpace);
			string NamePathTexture = "";
			if(true == Path.IsPathRooted(NameTexture))
			{
				NamePathTexture = string.Copy(NameTexture);
			}
			else
			{
				NamePathTexture = LibraryEditor_SpriteStudio.Utility.Text.PathGetAbsolute(	NameTexture,
																							InformationProject,
																							LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.TEXTURE
																						);
			}
			InformationCellMap.IndexTexture = InformationProject.AddTexture(NamePathTexture);

			/* Get Texture Addressing */
			InformationCellMap.WrapTexture = InformationProject.WrapTexture;
			InformationCellMap.FilterTexture = InformationProject.FilterTexture;

			string ValueTextBool = null;
			bool ValueBool = false;
			ValueTextBool = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeRoot, "overrideTexSettings", ManagerNameSpace);
			if(null != ValueTextBool)
			{
				ValueBool = LibraryEditor_SpriteStudio.Utility.Text.ValueGetBool(ValueTextBool);
				if(true == ValueBool)
				{
					/* Get Texture Wrap-Mode */
					NameOptions = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeRoot, "wrapMode", ManagerNameSpace);
					switch(NameOptions)
					{
						case "repeat":
							InformationCellMap.WrapTexture = Library_SpriteStudio.KindWrapTexture.REPEAT;
							break;

						case "mirror":
							Debug.LogWarning(	"SSCE-Import Warning: Texture Wrap-Mode \"Mirror\" is not Suppoted. Force-Changed \"Clamp\" : File["
												+ (string)InformationProject.ListNameSSCE[IndexSSCE] + "] "
												+ " SSPJ[" + InformationProject.NameFileBody + InformationProject.NameFileExtension + "]"
											);
							goto case "clamp";

						case "clamp":
							InformationCellMap.WrapTexture = Library_SpriteStudio.KindWrapTexture.CLAMP;
							break;

						default:
							break;
					}

					/* Get Texture Filter-Mode */
					NameOptions = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeRoot, "filterMode", ManagerNameSpace);
					switch(NameOptions)
					{
						case "nearlest":
							InformationCellMap.FilterTexture = Library_SpriteStudio.KindFilterTexture.NEAREST;
							break;

						case "linear":
							InformationCellMap.FilterTexture = Library_SpriteStudio.KindFilterTexture.LINEAR;
							break;

						default:
							break;
					}
				}
			}

			/* Get Cells */
			ArrayList ArrayCell = new ArrayList();
			if(null == ArrayCell)
			{
				MessageError = "Not Enough Memory. (CellMap WorkArea)";
				goto ParseOPSS_ImportSSCE_ErrorEnd;
			}
			ArrayCell.Clear();

			NodeList = LibraryEditor_SpriteStudio.Utility.XML.ListGetNode(NodeRoot, "cells/cell", ManagerNameSpace);
			if(null == NodeList)
			{
				MessageError = "Cells-Node Not-Found.";
				goto ParseOPSS_ImportSSCE_ErrorEnd;
			}
			string ItemText = null;
			string[] ItemTextSprit = null;
			double PivotNormalizeX = 0.0;
			double PivotNormalizeY = 0.0;
			LibraryEditor_SpriteStudio.ParseOPSS.InformationCell Cell = null;
			foreach(XmlNode NodeCell in NodeList)
			{
				Cell = new LibraryEditor_SpriteStudio.ParseOPSS.InformationCell();
				if(null == Cell)
				{
					MessageError = "Not Enough Memory.";
					goto ParseOPSS_ImportSSCE_ErrorEnd;
				}

				ItemText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeCell, "name", ManagerNameSpace);
				Cell.Name = string.Copy(ItemText);

				ItemText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeCell, "pos", ManagerNameSpace);
				ItemTextSprit = ItemText.Split(' ');
				Cell.Area.x = (float)(LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(ItemTextSprit[0]));
				Cell.Area.y = (float)(LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(ItemTextSprit[1]));

				ItemText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeCell, "size", ManagerNameSpace);
				ItemTextSprit = ItemText.Split(' ');
				Cell.Area.width = (float)(LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(ItemTextSprit[0]));
				Cell.Area.height = (float)(LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(ItemTextSprit[1]));

				ItemText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeCell, "pivot", ManagerNameSpace);
				ItemTextSprit = ItemText.Split(' ');
				PivotNormalizeX = LibraryEditor_SpriteStudio.Utility.Text.ValueGetDouble(ItemTextSprit[0]);
				PivotNormalizeY = LibraryEditor_SpriteStudio.Utility.Text.ValueGetDouble(ItemTextSprit[1]);
				Cell.Pivot.x = (float)((double)Cell.Area.width * (PivotNormalizeX + 0.5));
				Cell.Pivot.y = (float)((double)Cell.Area.height * (-PivotNormalizeY + 0.5));

				ItemText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeCell, "rotated", ManagerNameSpace);
				Cell.Rotate = LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(ItemText);

				ArrayCell.Add(Cell);
			}
			InformationCellMap.ListCell = (LibraryEditor_SpriteStudio.ParseOPSS.InformationCell[])ArrayCell.ToArray(typeof(LibraryEditor_SpriteStudio.ParseOPSS.InformationCell));

			return(InformationCellMap);

		ParseOPSS_ImportSSCE_ErrorEnd:;
			Debug.LogError(	"SSCE-Import Error: "
							+ MessageError + ": File[" + (string)InformationProject.ListNameSSCE[IndexSSCE] + "]"
							+ " SSPJ[" + InformationProject.NameFileBody + InformationProject.NameFileExtension + "]"
						);
			return(null);
		}
		internal enum KindVersionSSCE
		{
			ERROR = 0x00000000,
			VERSION_000100 = 0x00000100,
			VERSION_010000 = 0x00010000,
			VERSION_010001 = 0x00010001,

			VERSION_LATEST = VERSION_010001
		};
		internal class InformationSSCE
		{
			/* SSCE Data: Base Data */
			internal string Name;

			/* SSCE Data: Image Datas */
			internal KindVersionSSCE VersionCode;

			/* SSCE Data: Image Datas */
			internal Library_SpriteStudio.KindWrapTexture WrapTexture;
			internal Library_SpriteStudio.KindFilterTexture FilterTexture;
			internal int IndexTexture;

			/* SSCE Data: Cell Datas */
			internal LibraryEditor_SpriteStudio.ParseOPSS.InformationCell[] ListCell;

			/* WorkArea: File/Path Name */
			internal string NameDirectory;
			internal string NameFileBody;
			internal string NameFileExtension;

			/* Convert-Data: Runtime-CellMap */
			internal Library_SpriteStudio.Data.CellMap DataCellMap;

			internal void CleanUp()
			{
				Name = "";

				VersionCode = LibraryEditor_SpriteStudio.ParseOPSS.KindVersionSSCE.ERROR;

				WrapTexture = Library_SpriteStudio.KindWrapTexture.CLAMP;
				FilterTexture = Library_SpriteStudio.KindFilterTexture.LINEAR;
				IndexTexture = -1;

				ListCell = null;

				NameDirectory = "";
				NameFileBody = "";
				NameFileExtension = "";

				DataCellMap = null;
			}

			internal int IndexGetCell(string Name)
			{
				if(null != ListCell)
				{
					int Count = ListCell.Length;
					for(int i=0; i<Count; i++)
					{
						if(0 == string.Compare(Name, ListCell[i].Name))
						{
							return(i);
						}
					}
				}
				return(-1);
			}
		}
		internal class InformationCell
		{
			internal string Name;
			internal Rect Area;
			internal Vector2 Pivot;
			internal float Rotate;

			internal void CleanUp()
			{
				Name = "";
				Area.x = 0.0f;
				Area.y = 0.0f;
				Area.width = 0.0f;
				Area.height = 0.0f;
				Pivot = Vector2.zero;
				Rotate = 0.0f;
			}
		}

		/* for Parsing ".ssae" */
		internal static LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAE ImportSSAE(	ref SettingImport DataSettingImport,
																							InformationSSPJ InformationProject,
																							int IndexSSAE
																						)
		{
			string MessageError = "";

			/* Open SSAE-File */
			if((0 > IndexSSAE) || (InformationProject.ListNameSSAE.Count <= IndexSSAE))
			{
				MessageError = "SSCE Index[" + IndexSSAE.ToString() + "] is Invalid.";
				goto ParseOPSS_ImportSSAE_ErrorEnd;
			}
			string NamePathSSAE = (string)InformationProject.ListNameSSAE[IndexSSAE];
			XmlDocument DataXML = new XmlDocument();
			DataXML.Load(NamePathSSAE);

			/* Create WorkArea */
			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAE InformationAnimation = new InformationSSAE();
			if(null == InformationAnimation)
			{
				MessageError = "Not Enough Memory (SSAE Work-Area)";
				goto ParseOPSS_ImportSSAE_ErrorEnd;
			}
			InformationAnimation.CleanUp();
			InformationAnimation.Name = LibraryEditor_SpriteStudio.Utility.Text.PathGetRelative(	NamePathSSAE,
																									InformationProject,
																									LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.SSAE
																								);
			InformationAnimation.Name = LibraryEditor_SpriteStudio.Utility.Text.DataNameGetFromPath(InformationAnimation.Name, DataSettingImport.FlagNameDataRuleOld);
			InformationAnimation.NameDirectory = Path.GetDirectoryName(NamePathSSAE) + "/";
			InformationAnimation.NameFileBody = Path.GetFileNameWithoutExtension(NamePathSSAE);
			InformationAnimation.NameFileExtension = Path.GetExtension(NamePathSSAE);

			/* Version-Check */
			XmlNode NodeRoot = DataXML.FirstChild;
			NodeRoot = NodeRoot.NextSibling;
			KindVersionSSAE VersionCode = (KindVersionSSAE)(LibraryEditor_SpriteStudio.Utility.XML.VersionCodeGet(NodeRoot, "SpriteStudioAnimePack", (int)KindVersionSSAE.ERROR, false));
			switch(VersionCode)
			{
				case KindVersionSSAE.VERSION_000100:
				case KindVersionSSAE.VERSION_010000:
				case KindVersionSSAE.VERSION_010001:
				case KindVersionSSAE.VERSION_010002:
				case KindVersionSSAE.VERSION_010200:
				case KindVersionSSAE.VERSION_010201:
				case KindVersionSSAE.VERSION_010202:
					break;

				case KindVersionSSAE.ERROR:
					MessageError = "Not Supported Version.";
					goto ParseOPSS_ImportSSAE_ErrorEnd;

				default:
					if(KindVersionSSAE.VERSION_LATEST < VersionCode)
					{	/* MEMO: Dealing as the latest supported version. */
						VersionCode = KindVersionSSAE.VERSION_LATEST;
						goto case KindVersionSSAE.VERSION_LATEST;
					}
					goto case KindVersionSSAE.ERROR;
			}
			InformationAnimation.VersionCode = VersionCode;

			NameTable NodeNameSpace = new NameTable();
			XmlNamespaceManager ManagerNameSpace = new XmlNamespaceManager(NodeNameSpace);
			XmlNodeList NodeList = null;

			/* Parts-Data Get */
			NodeList = LibraryEditor_SpriteStudio.Utility.XML.ListGetNode(NodeRoot, "Model/partList/value", ManagerNameSpace);
			if(null == NodeList)
			{
				MessageError = "PartsList-Node Not-Found.";
				goto ParseOPSS_ImportSSAE_ErrorEnd;
			}

			InformationAnimation.ListParts = new LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts[NodeList.Count];
			if(null == InformationAnimation.ListParts)
			{
				MessageError = "Not Enough Memory. (Parts-Data WorkArea)";
				goto ParseOPSS_ImportSSAE_ErrorEnd;
			}
			foreach(XmlNode NodeParts in NodeList)
			{
				/* Part-ID Get */
				int IDParts = LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeParts, "arrayIndex", ManagerNameSpace));

				/* Part-Data Get */
				InformationAnimation.ListParts[IDParts] = ImportSSAEParts(	ref DataSettingImport,
																			InformationProject,
																			NodeParts,
																			ManagerNameSpace,
																			InformationAnimation,
																			IDParts,
																			(string)InformationProject.ListNameSSAE[IndexSSAE]
																		);
				if(null == InformationAnimation.ListParts[IDParts])
				{
					goto ParseOPSS_ImportSSAE_ErrorEnd_NoMessage;
				}
			}

			/* Use-CellMap Get */
			NodeList = LibraryEditor_SpriteStudio.Utility.XML.ListGetNode(NodeRoot, "cellmapNames/value", ManagerNameSpace);
			if(null == NodeList)
			{
				InformationAnimation.ListIndexCellMap = null;
			}
			else
			{
				int CountCellMap = NodeList.Count;
				int IndexCellMap = 0;
				string NameCellMap = "";

				InformationAnimation.ListIndexCellMap = new int[CountCellMap];
				for(int i=0; i<CountCellMap; i++)
				{
					InformationAnimation.ListIndexCellMap[i] = -1;
				}
				foreach(XmlNode NodeCellMapName in NodeList)
				{
					NameCellMap = NodeCellMapName.InnerText;
					NameCellMap = LibraryEditor_SpriteStudio.Utility.Text.PathGetAbsolute(	NameCellMap,
																							InformationProject,
																							LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.SSCE
																						);
					InformationAnimation.ListIndexCellMap[IndexCellMap] = InformationProject.IndexGetFileName(InformationProject.ListNameSSCE, NameCellMap);
					if(-1 == InformationAnimation.ListIndexCellMap[IndexCellMap])
					{
						MessageError = "CellMap Not-Found. [" + NameCellMap + "]";
						goto ParseOPSS_ImportSSAE_ErrorEnd;
					}
					IndexCellMap++;
				}
			}

			/* Animations (& Parts' Key-Frames) Get */
			NodeList = LibraryEditor_SpriteStudio.Utility.XML.ListGetNode(NodeRoot, "animeList/anime", ManagerNameSpace);
			if(null == InformationAnimation.ListParts)
			{
				MessageError = "Not Enough Memory. (Animation-Data WorkArea)";
				goto ParseOPSS_ImportSSAE_ErrorEnd;
			}

			InformationAnimation.ListAnimation = new LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEAnimation[NodeList.Count];
			int IndexAnimation = 0;
			foreach(XmlNode NodeAnimation in NodeList)
			{
				/* Animation (& Parts' Key-Frames) Get */
				InformationAnimation.ListAnimation[IndexAnimation] = ImportSSAEAnimation(	ref DataSettingImport,
																							InformationProject,
																							NodeAnimation,
																							ManagerNameSpace,
																							InformationAnimation,
																							IndexAnimation,
																							(string)InformationProject.ListNameSSAE[IndexSSAE]
																						);
				if(null == InformationAnimation.ListAnimation[IndexAnimation])
				{
					goto ParseOPSS_ImportSSAE_ErrorEnd_NoMessage;
				}
				IndexAnimation++;
			}

			return(InformationAnimation);

		ParseOPSS_ImportSSAE_ErrorEnd:;
			Debug.LogError(	"SSAE-Import Error: "
							+ MessageError + ": File[" + (string)InformationProject.ListNameSSAE[IndexSSAE] + "]"
							+ " SSPJ[" + InformationProject.NameFileBody + InformationProject.NameFileExtension + "]"
						);
		ParseOPSS_ImportSSAE_ErrorEnd_NoMessage:;
			return(null);
		}
		internal static LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts ImportSSAEParts(	ref SettingImport DataSettingImport,
																									InformationSSPJ InformationProject,
																									XmlNode NodeParts,
																									XmlNamespaceManager ManagerNameSpace,
																									LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAE InformationAnimationSet,
																									int ID,
																									string NameFileSSAE
																								)
		{
			string MessageError = "";

			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts InformationParts = new LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts();
			if(null == InformationParts)
			{
				MessageError = "Parts[" + ID.ToString() + "] Not Enough Memory. (Parts WorkArea)";
				goto ParseOPSS_InformationSSAEParts_ErrorEnd;
			}
			InformationParts.CleanUp();
			InformationParts.ListIndexPartsChild = new List<int>();
			InformationParts.ListIndexPartsChild.Clear();

			/* Base Datas Get */
			string ValueText = "";

			ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeParts, "name", ManagerNameSpace);
			InformationParts.Name = string.Copy(ValueText);

			InformationParts.ID = ID;

			ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeParts, "parentIndex", ManagerNameSpace);
			InformationParts.IDParent = LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(ValueText);
			if(0 <= InformationParts.IDParent)
			{
				LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts InformationPartsParent = InformationAnimationSet.ListParts[InformationParts.IDParent];
				InformationPartsParent.ListIndexPartsChild.Add(InformationParts.ID);
			}

			/* Parts-Kind Get */
			ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeParts, "type", ManagerNameSpace);
			switch(ValueText)
			{
				case "null":
					InformationParts.Kind = (0 == InformationParts.ID) ? Library_SpriteStudio.KindParts.ROOT : Library_SpriteStudio.KindParts.NULL;
					break;

				case "normal":
					InformationParts.Kind = Library_SpriteStudio.KindParts.NORMAL;
					break;

				case "instance":
					InformationParts.Kind = Library_SpriteStudio.KindParts.INSTANCE;
					break;

				case "effect":
					InformationParts.Kind = Library_SpriteStudio.KindParts.EFFECT;
					break;

				default:
					Debug.LogWarning("SSAE-Import Warning: Parts["
										+ InformationParts.ID.ToString()
										+ "] Invalid Parts Kind.: "
										+ ValueText
									);
					goto case "null";
			}

			/* "Collision" Datas Get */
			ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeParts, "boundsType", ManagerNameSpace);
			switch(ValueText)
			{
				case "none":
					InformationParts.KindShapeCollision = Library_SpriteStudio.KindCollision.NON;
					InformationParts.SizeCollisionZ = 0.0f;
					break;

				case "quad":
					InformationParts.KindShapeCollision = Library_SpriteStudio.KindCollision.SQUARE;
					InformationParts.SizeCollisionZ = DataSettingImport.CollisionThicknessZ;
					break;

				case "aabb":
					InformationParts.KindShapeCollision = Library_SpriteStudio.KindCollision.AABB;
					InformationParts.SizeCollisionZ = DataSettingImport.CollisionThicknessZ;
					break;

				case "circle":
					InformationParts.KindShapeCollision = Library_SpriteStudio.KindCollision.CIRCLE;
					InformationParts.SizeCollisionZ = DataSettingImport.CollisionThicknessZ;
					break;

				case "circle_smin":
					InformationParts.KindShapeCollision = Library_SpriteStudio.KindCollision.CIRCLE_SCALEMINIMUM;
					InformationParts.SizeCollisionZ = DataSettingImport.CollisionThicknessZ;
					break;

				case "circle_smax":
					InformationParts.KindShapeCollision = Library_SpriteStudio.KindCollision.CIRCLE_SCALEMAXIMUM;
					InformationParts.SizeCollisionZ = DataSettingImport.CollisionThicknessZ;
					break;

				default:
					Debug.LogWarning("SSAE-Import Warning: Parts["
										+ InformationParts.ID.ToString()
										+ "] Invalid Collision Kind.: "
										+ ValueText
									);
					goto case "none";
			}

			/* "Inheritance" Datas Get */
			ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeParts, "inheritType", ManagerNameSpace);
			switch(ValueText)
			{
				case "parent":
					{
						switch(InformationAnimationSet.VersionCode)
						{
							case KindVersionSSAE.VERSION_010000:
							case KindVersionSSAE.VERSION_010001:
							case KindVersionSSAE.VERSION_010002:
							case KindVersionSSAE.VERSION_010200:
							case KindVersionSSAE.VERSION_010201:
							case KindVersionSSAE.VERSION_010202:	/* EffectPartsCheck? */
								{
									if(0 == InformationParts.ID)
									{
										InformationParts.KindInheritance = LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.KindTypeInheritance.SELF;
										InformationParts.FlagInheritance = LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.FlagBitInheritance.PRESET;
									}
									else
									{
										InformationParts.KindInheritance = LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.KindTypeInheritance.PARENT;
										InformationParts.FlagInheritance = LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.FlagBitInheritance.CLEAR;
									}
								}
								break;
						}
					}
					break;

				case "self":
					{
						switch(InformationAnimationSet.VersionCode)
						{
							case KindVersionSSAE.VERSION_010000:
							case KindVersionSSAE.VERSION_010001:
								{
									/* MEMO: Default-Value: 0(true) */
									/*       Attributes'-Tag exists when Value is 0(false). */
									InformationParts.KindInheritance = LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.KindTypeInheritance.SELF;
									InformationParts.FlagInheritance = LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.FlagBitInheritance.CLEAR;

									XmlNode NodeAttribute = null;
									NodeAttribute = LibraryEditor_SpriteStudio.Utility.XML.NodeGet(NodeParts, "ineheritRates/ALPH", ManagerNameSpace);
									if(null == NodeAttribute)
									{
										InformationParts.FlagInheritance |= LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.FlagBitInheritance.OPACITY_RATE;
									}

									NodeAttribute = LibraryEditor_SpriteStudio.Utility.XML.NodeGet(NodeParts, "ineheritRates/FLPH", ManagerNameSpace);
									if(null == NodeAttribute)
									{
										InformationParts.FlagInheritance |= LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.FlagBitInheritance.FLIP_X;
									}

									NodeAttribute = LibraryEditor_SpriteStudio.Utility.XML.NodeGet(NodeParts, "ineheritRates/FLPV", ManagerNameSpace);
									if(null == NodeAttribute)
									{
										InformationParts.FlagInheritance |= LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.FlagBitInheritance.FLIP_Y;
									}

									NodeAttribute = LibraryEditor_SpriteStudio.Utility.XML.NodeGet(NodeParts, "ineheritRates/HIDE", ManagerNameSpace);
									if(null == NodeAttribute)
									{
										InformationParts.FlagInheritance |= LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.FlagBitInheritance.SHOW_HIDE;
									}
								}
								break;

							case KindVersionSSAE.VERSION_010002:
							case KindVersionSSAE.VERSION_010200:
							case KindVersionSSAE.VERSION_010201:
							case KindVersionSSAE.VERSION_010202:
								{
									/* MEMO: Attributes'-Tag always exists. */
									string ValueTextBool = "";
									bool ValueBool = false;

									InformationParts.KindInheritance = LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.KindTypeInheritance.SELF;
									InformationParts.FlagInheritance = LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.FlagBitInheritance.PRESET;
									InformationParts.FlagInheritance |= LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.FlagBitInheritance.FLIP_X;
									InformationParts.FlagInheritance |= LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.FlagBitInheritance.FLIP_Y;
									InformationParts.FlagInheritance |= LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.FlagBitInheritance.SHOW_HIDE;

									ValueTextBool = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeParts, "ineheritRates/ALPH", ManagerNameSpace);
									if(null != ValueTextBool)
									{
										ValueBool = LibraryEditor_SpriteStudio.Utility.Text.ValueGetBool(ValueTextBool);
										if(false == ValueBool)
										{
											InformationParts.FlagInheritance &= ~LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.FlagBitInheritance.OPACITY_RATE;
										}
									}

									ValueTextBool = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeParts, "ineheritRates/FLPH", ManagerNameSpace);
									if(null != ValueTextBool)
									{
										ValueBool = LibraryEditor_SpriteStudio.Utility.Text.ValueGetBool(ValueTextBool);
										if(false == ValueBool)
										{
											InformationParts.FlagInheritance &= ~LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.FlagBitInheritance.FLIP_X;
										}
									}

									ValueTextBool = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeParts, "ineheritRates/FLPV", ManagerNameSpace);
									if(null != ValueTextBool)
									{
										ValueBool = LibraryEditor_SpriteStudio.Utility.Text.ValueGetBool(ValueTextBool);
										if(false == ValueBool)
										{
											InformationParts.FlagInheritance &= ~LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.FlagBitInheritance.FLIP_Y;
										}
									}

									ValueTextBool = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeParts, "ineheritRates/HIDE", ManagerNameSpace);
									if(null != ValueTextBool)
									{
										ValueBool = LibraryEditor_SpriteStudio.Utility.Text.ValueGetBool(ValueTextBool);
										if(false == ValueBool)
										{
											InformationParts.FlagInheritance &= ~LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.FlagBitInheritance.SHOW_HIDE;
										}
									}
								}
								break;
						}
					}
					break;

				default:
					Debug.LogWarning("SSAE-Import Warning: Parts["
										+ InformationParts.ID.ToString()
										+ "] Invalid Inheritance Kind.: "
										+ ValueText
									);
					goto case "parent";
			}

			/* Target-Blending Get */
			ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeParts, "alphaBlendType", ManagerNameSpace);
			switch(ValueText)
			{
				case "mix":
					InformationParts.KindBlendTarget = Library_SpriteStudio.KindColorOperation.MIX;
					break;

				case "mul":
					InformationParts.KindBlendTarget = Library_SpriteStudio.KindColorOperation.MUL;
					break;

				case "add":
					InformationParts.KindBlendTarget = Library_SpriteStudio.KindColorOperation.ADD;
					break;

				case "sub":
					InformationParts.KindBlendTarget = Library_SpriteStudio.KindColorOperation.SUB;
					break;

				default:
					Debug.LogWarning("SSAE-Import Warning: Parts["
										+ InformationParts.ID.ToString()
										+ "] Invalid Alpha-Blend Kind.: "
										+ ValueText
									);
					goto case "mix";
			}

			/* Instance-Animation Datas Get */
			ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeParts, "refAnimePack", ManagerNameSpace);
			if(null == ValueText)
			{
				InformationParts.NameSSAEInstance = "";
			}
			else
			{
#if false
				InformationParts.NameSSAEInstance = InformationAnimationSet.NamePathGetAbsolute(ValueText + ".ssae");
#else
				/* MEMO: Without confirming the file-path, search at the time of reference. */
				InformationParts.NameSSAEInstance = ValueText;
#endif
			}
			ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeParts, "refAnime", ManagerNameSpace);
			InformationParts.NameAnimationUnderControl = (null != ValueText) ? String.Copy(ValueText) : "";

			/* Effect Datas Get */
			ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeParts, "refEffectName", ManagerNameSpace);
			if(null == ValueText)
			{
				InformationParts.NameSSEEEffect = "";
			}
			else
			{
				/* MEMO: Even if Tag is present, it may value is empty. */
#if false
				InformationParts.NameSSEEEffect = (false == string.IsNullOrEmpty(ValueText)) ? InformationAnimationSet.NamePathGetAbsolute(ValueText + ".ssee") : "";
#else
				/* MEMO: Without confirming the file-path, search at the time of reference. */
				InformationParts.NameSSEEEffect = (false == string.IsNullOrEmpty(ValueText)) ? ValueText : "";
#endif
			}

			/* Color-Label Get */
			ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeParts, "colorLabel", ManagerNameSpace);
			if(null == ValueText)
			{
				InformationParts.KindLabelColor = Library_SpriteStudio.KindColorLabel.NON;
			}
			else
			{
				switch(ValueText)
				{
					case "Red":
						InformationParts.KindLabelColor = Library_SpriteStudio.KindColorLabel.RED;
						break;

					case "Orange":
						InformationParts.KindLabelColor = Library_SpriteStudio.KindColorLabel.ORANGE;
						break;

					case "Yellow":
						InformationParts.KindLabelColor = Library_SpriteStudio.KindColorLabel.YELLOW;
						break;

					case "Green":
						InformationParts.KindLabelColor = Library_SpriteStudio.KindColorLabel.GREEN;
						break;

					case "Blue":
						InformationParts.KindLabelColor = Library_SpriteStudio.KindColorLabel.BLUE;
						break;

					case "Violet":
						InformationParts.KindLabelColor = Library_SpriteStudio.KindColorLabel.VIOLET;
						break;

					case "Gray":
						InformationParts.KindLabelColor = Library_SpriteStudio.KindColorLabel.GRAY;
						break;

					default:
						Debug.LogWarning("SSAE-Import Warning: Parts["
											+ InformationParts.ID.ToString()
											+ "] Invalid Color-Label Kind.: "
											+ ValueText
										);
						InformationParts.KindLabelColor = Library_SpriteStudio.KindColorLabel.NON;
						break;
				}
			}


			return(InformationParts);

		ParseOPSS_InformationSSAEParts_ErrorEnd:;
			Debug.LogError(	"SSAE-Import Error (Parts): "
							+ MessageError + ": File[" + NameFileSSAE + "]"
							+ " SSPJ[" + InformationProject.NameFileBody + InformationProject.NameFileExtension + "]"
						);
			return(null);
		}
		internal static LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEAnimation ImportSSAEAnimation(	ref SettingImport DataSettingImport,
																											InformationSSPJ InformationProject,
																											XmlNode NodeAnimation,
																											XmlNamespaceManager ManagerNameSpace,
																											LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAE InformationAnimationSet,
																											int IndexAnimation,
																											string NameFileSSAE
																										)
		{
			string MessageError = "";
			string ValueText = "";

			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEAnimation InformationAnimation = new LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEAnimation();
			if(null == InformationAnimation)
			{
				MessageError = "Animation[" + IndexAnimation.ToString() + "] Not Enough Memory. (Animation WorkArea)";
				goto ParseOPSS_InformationSSAEAnimation_ErrorEnd;
			}
			InformationAnimation.CleanUp();

			/* Base Datas Get */
			ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeAnimation, "name", ManagerNameSpace);
			InformationAnimation.Name = string.Copy(ValueText);

			ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeAnimation, "settings/fps", ManagerNameSpace);
			InformationAnimation.FramePerSecond = LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(ValueText);

			ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeAnimation, "settings/frameCount", ManagerNameSpace);
			InformationAnimation.CountFrame = LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(ValueText);

			/* Labels Get */
			ArrayList ArrayInformationLabel = new ArrayList();
			Library_SpriteStudio.Data.Label InformationLabel = null;
			if(null == ArrayInformationLabel)
			{
				MessageError = "Animation[" + IndexAnimation.ToString() + "] Not Enough Memory. (Animation-Label WorkArea)";
				goto ParseOPSS_InformationSSAEAnimation_ErrorEnd;
			}
			ArrayInformationLabel.Clear();
			XmlNodeList NodeListLabel = LibraryEditor_SpriteStudio.Utility.XML.ListGetNode(NodeAnimation, "labels/value", ManagerNameSpace);
			foreach(XmlNode NodeLabel in NodeListLabel)
			{
				ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeLabel, "name", ManagerNameSpace);
				if(0 > Library_SpriteStudio.Data.NameCheckLabelReserved(ValueText))
				{
					InformationLabel = new Library_SpriteStudio.Data.Label();
					if(null == InformationLabel)
					{
						MessageError = "Animation[" + IndexAnimation.ToString() + "] Not Enough Memory. (Animation-LabelData WorkArea)";
						goto ParseOPSS_InformationSSAEAnimation_ErrorEnd;
					}
					InformationLabel.Name = string.Copy(ValueText);

					ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeLabel, "time", ManagerNameSpace);
					InformationLabel.FrameNo = LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(ValueText);

					ArrayInformationLabel.Add(InformationLabel);
				}
				else
				{
					Debug.LogWarning(	"SSAE-Import Warning (Animation:" + IndexAnimation.ToString() + "): "
										+ "Used Deault-Label. Label-Name[" + ValueText + "]"
										+ ": File[" + NameFileSSAE + "]"
										+ " SSPJ[" + InformationProject.NameFileBody + InformationProject.NameFileExtension + "]"
									);
				}
			}
			InformationAnimation.ListLabel = (Library_SpriteStudio.Data.Label[])ArrayInformationLabel.ToArray(typeof(Library_SpriteStudio.Data.Label));
			ArrayInformationLabel.Clear();
			ArrayInformationLabel = null;

			/* Parts' Key-Frame Get */
			int CountParts = InformationAnimationSet.ListParts.Length;
			InformationAnimation.ListAnimationPartsKeyFrames = new LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEAnimationParts[CountParts];
			if(null == InformationAnimation.ListAnimationPartsKeyFrames)
			{
				MessageError = "Animation[" + IndexAnimation.ToString() + "] Not Enough Memory. (Animation-KeyFrame Data)";
				goto ParseOPSS_InformationSSAEAnimation_ErrorEnd;
			}
			for(int i=0; i<CountParts; i++)
			{
				InformationAnimation.ListAnimationPartsKeyFrames[i] = new LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEAnimationParts();
				if(null == InformationAnimation.ListAnimationPartsKeyFrames[i])
				{
					MessageError = "Animation[" + IndexAnimation.ToString() + "] Not Enough Memory. (Animation-KeyFrame Data Parts[" + i.ToString() + "])";
					goto ParseOPSS_InformationSSAEAnimation_ErrorEnd;
				}
				InformationAnimation.ListAnimationPartsKeyFrames[i].CleanUp();
				InformationAnimation.ListAnimationPartsKeyFrames[i].BootUp();
			}
			XmlNodeList NodeListAnimationParts = LibraryEditor_SpriteStudio.Utility.XML.ListGetNode(NodeAnimation, "partAnimes/partAnime", ManagerNameSpace);
			int IndexParts = -1;
			foreach(XmlNode NodeAnimationPart in NodeListAnimationParts)
			{
				ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeAnimationPart, "partName", ManagerNameSpace);
				IndexParts = InformationAnimationSet.IndexGetParts(ValueText);
				if(-1 == IndexParts)
				{
					MessageError = "Animation[" + IndexAnimation.ToString() + "] Parts-Name Not Found. Name[" + ValueText + "]";
					goto ParseOPSS_InformationSSAEAnimation_ErrorEnd;
				}

				XmlNode NodeAnimationPartsAttributes = LibraryEditor_SpriteStudio.Utility.XML.NodeGet(NodeAnimationPart, "attributes", ManagerNameSpace);
				if(false == ImportSSAEAnimationParts(	ref DataSettingImport,
														InformationProject,
														NodeAnimationPartsAttributes,
														ManagerNameSpace,
														InformationAnimationSet,
														InformationAnimation,
														IndexParts,
														NameFileSSAE
													)
					)
				{
					goto ParseOPSS_InformationSSAEAnimation_ErrorEnd_NoMessage;
				}
			}

			return(InformationAnimation);

		ParseOPSS_InformationSSAEAnimation_ErrorEnd:;
			Debug.LogError(	"SSAE-Import Error (Animation:" + IndexAnimation.ToString() + "): "
							+ MessageError + ": File[" + NameFileSSAE + "]"
							+ " SSPJ[" + InformationProject.NameFileBody + InformationProject.NameFileExtension + "]"
						);
		ParseOPSS_InformationSSAEAnimation_ErrorEnd_NoMessage:;
			return(null);
		}
		internal static bool ImportSSAEAnimationParts(	ref SettingImport DataSettingImport,
														InformationSSPJ InformationProject,
														XmlNode NodeAnimationPartsAttribute,
														XmlNamespaceManager ManagerNameSpace,
														LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAE InformationAnimationSet,
														LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEAnimation InformationAnimation,
														int Index,
														string NameFileSSAE
													)
		{
			string MessageError = "";
			string ValueText = "";

			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEAnimationParts InformationAnimationPart = InformationAnimation.ListAnimationPartsKeyFrames[Index];
			if(null == InformationAnimationPart)
			{
				MessageError = "WorkArea Not Allocated.";
				goto ParseOPSS_InformationSSAEAnimationParts_ErrorEnd;
			}

			/* KeyFrame List Get */
			LibraryEditor_SpriteStudio.KeyFrame.InformationAttribute InformationAttribute = null;
			XmlNodeList ListNodeAttribute = LibraryEditor_SpriteStudio.Utility.XML.ListGetNode(NodeAnimationPartsAttribute, "attribute", ManagerNameSpace);
			foreach(XmlNode NodeAttribute in ListNodeAttribute)
			{
				/* Attribute Get */
				ValueText = NodeAttribute.Attributes["tag"].Value;
				InformationAttribute = LibraryEditor_SpriteStudio.KeyFrame.InformationAttributeGetTagName(ValueText);
				if(null == InformationAttribute)
				{
					Debug.LogWarning(	"SSAE-Import Warning (Attribute): "
										+ "Attribute Invalid.: Attribute[" + ValueText + "]"
										+ ": Animation-Name[" + InformationAnimation.Name + "]"
										+ ": Parts-Name[" + InformationAnimationSet.ListParts[Index].Name + "]"
										+ ": File[" + NameFileSSAE + "]"
										+ " SSPJ[" + InformationProject.NameFileBody + InformationProject.NameFileExtension + "]"
									);
					continue;
				}

				XmlNodeList ListNodeKey = LibraryEditor_SpriteStudio.Utility.XML.ListGetNode(NodeAttribute, "key", ManagerNameSpace);
				if(null == ListNodeKey)
				{
					Debug.LogWarning(	"SSAE-Import Warning (Attribute): "
										+ "Attribute has No Key-Data.: Attribute[" + ValueText + "]"
										+ ": Animation-Name[" + InformationAnimation.Name + "]"
										+ ": Parts-Name[" + InformationAnimationSet.ListParts[Index].Name + "]"
										+ ": File[" + NameFileSSAE + "]"
										+ " SSPJ[" + InformationProject.NameFileBody + InformationProject.NameFileExtension + "]"
									);
					continue;
				}

				/* KeyFrame Data Get */
				XmlNode NodeInterpolation = null;
				int FrameNo = -1;
				Library_SpriteStudio.KindInterpolation KindCurve = Library_SpriteStudio.KindInterpolation.NON;
				bool FlagHasParameterCurve = false;
				float TimeCurveStart = 0.0f;
				float ValueCurveStart = 0.0f;
				float TimeCurveEnd = 0.0f;
				float ValueCurveEnd = 0.0f;
				string ValueTextParameter = "";
				string[] ValueTextParameterArgument = null;
				foreach(XmlNode NodeKey in ListNodeKey)
				{
					/* Base Data Get */
					FrameNo = LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(NodeKey.Attributes["time"].Value);
					KindCurve = Library_SpriteStudio.KindInterpolation.NON;
					FlagHasParameterCurve = false;
					TimeCurveStart = 0.0f;
					ValueCurveStart = 0.0f;
					TimeCurveEnd = 0.0f;
					ValueCurveEnd = 0.0f;

					/* Interpolation-Parameters Get */
					NodeInterpolation = NodeKey.Attributes["ipType"];
					if(null != NodeInterpolation)
					{
						ValueText = string.Copy(NodeInterpolation.Value);
						switch(ValueText)
						{
							case "linear":
								KindCurve = Library_SpriteStudio.KindInterpolation.LINEAR;
								FlagHasParameterCurve = false;
								break;

							case "hermite":
								KindCurve = Library_SpriteStudio.KindInterpolation.HERMITE;
								FlagHasParameterCurve = true;
								break;

							case "bezier":
								KindCurve = Library_SpriteStudio.KindInterpolation.BEZIER;
								FlagHasParameterCurve = true;
								break;

							case "acceleration":
								KindCurve = Library_SpriteStudio.KindInterpolation.ACCELERATE;
								FlagHasParameterCurve = false;
								break;

							case "deceleration":
								KindCurve = Library_SpriteStudio.KindInterpolation.DECELERATE;
								FlagHasParameterCurve = false;
								break;

							default:
								Debug.LogWarning(	"SSAE-Import Warning (Attribute): "
													+ "Interpolation Invalid.: Kind[" + ValueText + "]"
													+ ": FrameNo[" + FrameNo.ToString() + "]"
													+ ": Animation-Name[" + InformationAnimation.Name + "]"
													+ ": Parts-Name[" + InformationAnimationSet.ListParts[Index].Name + "]"
													+ ": File[" + NameFileSSAE + "]"
													+ " SSPJ[" + InformationProject.NameFileBody + InformationProject.NameFileExtension + "]"
												);
								KindCurve = Library_SpriteStudio.KindInterpolation.NON;
								FlagHasParameterCurve = false;
								break;
						}

						/* Additional Parameters Get */
						if(true == FlagHasParameterCurve)
						{
							ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "curve", ManagerNameSpace);
							if(null == ValueTextParameter)
							{
								Debug.LogWarning(	"SSAE-Import Warning (Attribute): "
													+ "Interpolation-Parameter Missing.: Kind[" + ValueTextParameter + "]"
													+ ": FrameNo[" + FrameNo.ToString() + "]"
													+ ": Animation-Name[" + InformationAnimation.Name + "]"
													+ ": Parts-Name[" + InformationAnimationSet.ListParts[Index].Name + "]"
													+ ": File[" + NameFileSSAE + "]"
													+ " SSPJ[" + InformationProject.NameFileBody + InformationProject.NameFileExtension + "]"
												);
								KindCurve = Library_SpriteStudio.KindInterpolation.NON;
								FlagHasParameterCurve = false;
								TimeCurveStart = 0.0f;
								ValueCurveStart = 0.0f;
								TimeCurveEnd = 0.0f;
								ValueCurveEnd = 0.0f;
							}
							else
							{
								ValueTextParameterArgument = ValueTextParameter.Split(' ');
								TimeCurveStart = (float)LibraryEditor_SpriteStudio.Utility.Text.ValueGetDouble(ValueTextParameterArgument[0]);
								ValueCurveStart = (float)LibraryEditor_SpriteStudio.Utility.Text.ValueGetDouble(ValueTextParameterArgument[1]);
								TimeCurveEnd = (float)LibraryEditor_SpriteStudio.Utility.Text.ValueGetDouble(ValueTextParameterArgument[2]);
								ValueCurveEnd = (float)LibraryEditor_SpriteStudio.Utility.Text.ValueGetDouble(ValueTextParameterArgument[3]);
							}
						}
					}

					/* Key-Data Get */
					switch(InformationAttribute.TypeValue)
					{
						case LibraryEditor_SpriteStudio.KeyFrame.KindValue.BOOL:
							{
								LibraryEditor_SpriteStudio.KeyFrame.DataBool Data = new LibraryEditor_SpriteStudio.KeyFrame.DataBool();
								Data.FrameNo = FrameNo;

								/* Curve-Data Set */
								Data.KindCurve = KindCurve;
								Data.TimeCurveStart = TimeCurveStart;
								Data.ValueCurveStart = ValueCurveStart;
								Data.TimeCurveEnd = TimeCurveEnd;
								Data.ValueCurveEnd = ValueCurveEnd;

								/* Body-Data Set */
								ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value", ManagerNameSpace);
								Data.Value = LibraryEditor_SpriteStudio.Utility.Text.ValueGetBool(ValueTextParameter);

								/* Key-Data Add */
								InformationAnimationPart.Attribute[InformationAttribute.IndexAttribute].Add(Data);
							}
							break;

						case LibraryEditor_SpriteStudio.KeyFrame.KindValue.INT:
							{
								LibraryEditor_SpriteStudio.KeyFrame.DataInt Data = new LibraryEditor_SpriteStudio.KeyFrame.DataInt();
								Data.FrameNo = FrameNo;

								/* Curve-Data Set */
								Data.KindCurve = KindCurve;
								Data.TimeCurveStart = TimeCurveStart;
								Data.ValueCurveStart = ValueCurveStart;
								Data.TimeCurveEnd = TimeCurveEnd;
								Data.ValueCurveEnd = ValueCurveEnd;

								/* Body-Data Set */
								ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value", ManagerNameSpace);
								Data.Value = LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(ValueTextParameter);

								/* Key-Data Add */
								InformationAnimationPart.Attribute[InformationAttribute.IndexAttribute].Add(Data);
							}
							break;

						case LibraryEditor_SpriteStudio.KeyFrame.KindValue.FLOAT:
							{
								LibraryEditor_SpriteStudio.KeyFrame.DataFloat Data = new LibraryEditor_SpriteStudio.KeyFrame.DataFloat();
								Data.FrameNo = FrameNo;

								/* Curve-Data Set */
								Data.KindCurve = KindCurve;
								Data.TimeCurveStart = TimeCurveStart;
								Data.ValueCurveStart = ValueCurveStart;
								Data.TimeCurveEnd = TimeCurveEnd;
								Data.ValueCurveEnd = ValueCurveEnd;

								/* Body-Data Set */
								ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value", ManagerNameSpace);
								Data.Value = LibraryEditor_SpriteStudio.Utility.Text.ValueGetFloat(ValueTextParameter);

								/* Key-Data Add */
								InformationAnimationPart.Attribute[InformationAttribute.IndexAttribute].Add(Data);
							}
							break;

						case LibraryEditor_SpriteStudio.KeyFrame.KindValue.USERDATA:
							{
								LibraryEditor_SpriteStudio.KeyFrame.DataUserData Data = new LibraryEditor_SpriteStudio.KeyFrame.DataUserData();
								Data.FrameNo = FrameNo;

								/* Curve-Data Set */
								/* MEMO: "User-Data" has no interpolation. */
								Data.KindCurve = Library_SpriteStudio.KindInterpolation.NON;
								Data.TimeCurveStart = 0.0f;
								Data.ValueCurveStart = 0.0f;
								Data.TimeCurveEnd = 0.0f;
								Data.ValueCurveEnd = 0.0f;

								/* Body-Data Set */
								Data.Value.Flags = Library_SpriteStudio.Data.AttributeUserData.FlagBit.CLEAR;
								ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value/integer", ManagerNameSpace);
								if(null != ValueTextParameter)
								{
									if(false == int.TryParse(ValueTextParameter, out Data.Value.NumberInt))
									{
										uint ValueUintTemp = 0;
										if(false == uint.TryParse(ValueTextParameter, out ValueUintTemp))
										{
											Debug.LogWarning(	"SSAE-Import Warning (Attribute): "
																+ "User-Data Invalid: Int[" + ValueTextParameter + "]"
																+ ": FrameNo[" + FrameNo.ToString() + "]"
																+ ": Animation-Name[" + InformationAnimation.Name + "]"
																+ ": Parts-Name[" + InformationAnimationSet.ListParts[Index].Name + "]"
																+ ": File[" + NameFileSSAE + "]"
																+ " SSPJ[" + InformationProject.NameFileBody + InformationProject.NameFileExtension + "]"
															);
										}
										Data.Value.NumberInt = (int)ValueUintTemp;
									}
									Data.Value.Flags |= Library_SpriteStudio.Data.AttributeUserData.FlagBit.NUMBER;
								}
								else
								{
									Data.Value.NumberInt = 0;
									Data.Value.Flags &= ~Library_SpriteStudio.Data.AttributeUserData.FlagBit.NUMBER;
								}

								ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value/rect", ManagerNameSpace);
								if(null != ValueTextParameter)
								{
									ValueTextParameterArgument = ValueTextParameter.Split(' ');
									Data.Value.Rectangle.xMin = (float)(LibraryEditor_SpriteStudio.Utility.Text.ValueGetDouble(ValueTextParameterArgument[0]));
									Data.Value.Rectangle.yMin = (float)(LibraryEditor_SpriteStudio.Utility.Text.ValueGetDouble(ValueTextParameterArgument[1]));
									Data.Value.Rectangle.xMax = (float)(LibraryEditor_SpriteStudio.Utility.Text.ValueGetDouble(ValueTextParameterArgument[2]));
									Data.Value.Rectangle.yMax = (float)(LibraryEditor_SpriteStudio.Utility.Text.ValueGetDouble(ValueTextParameterArgument[3]));
									Data.Value.Flags |= Library_SpriteStudio.Data.AttributeUserData.FlagBit.RECTANGLE;
								}
								else
								{
									Data.Value.Rectangle.xMin = 0.0f;
									Data.Value.Rectangle.yMin = 0.0f;
									Data.Value.Rectangle.xMax = 0.0f;
									Data.Value.Rectangle.yMax = 0.0f;
									Data.Value.Flags &= ~Library_SpriteStudio.Data.AttributeUserData.FlagBit.RECTANGLE;
								}

								ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value/point", ManagerNameSpace);
								if(null != ValueTextParameter)
								{
									ValueTextParameterArgument = ValueTextParameter.Split(' ');
									Data.Value.Coordinate.x = (float)(LibraryEditor_SpriteStudio.Utility.Text.ValueGetDouble(ValueTextParameterArgument[0]));
									Data.Value.Coordinate.y = (float)(LibraryEditor_SpriteStudio.Utility.Text.ValueGetDouble(ValueTextParameterArgument[1]));
									Data.Value.Flags |= Library_SpriteStudio.Data.AttributeUserData.FlagBit.COORDINATE;
								}
								else
								{
									Data.Value.Coordinate.x = 0.0f;
									Data.Value.Coordinate.y = 0.0f;
									Data.Value.Flags &= ~Library_SpriteStudio.Data.AttributeUserData.FlagBit.COORDINATE;
								}

								ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value/string", ManagerNameSpace);
								if(null != ValueTextParameter)
								{
									Data.Value.Text = string.Copy(ValueTextParameter);
									Data.Value.Flags |= Library_SpriteStudio.Data.AttributeUserData.FlagBit.TEXT;
								}
								else
								{
									Data.Value.Text = null;
									Data.Value.Flags &= ~Library_SpriteStudio.Data.AttributeUserData.FlagBit.TEXT;
								}

								/* Key-Data Add */
								InformationAnimationPart.Attribute[InformationAttribute.IndexAttribute].Add(Data);
							}
							break;

						case LibraryEditor_SpriteStudio.KeyFrame.KindValue.CELL:
							{
								LibraryEditor_SpriteStudio.KeyFrame.DataCell Data = new LibraryEditor_SpriteStudio.KeyFrame.DataCell();
								Data.FrameNo = FrameNo;

								/* Curve-Data Set */
								/* MEMO: "Cell" has no interpolation. */
								Data.KindCurve = Library_SpriteStudio.KindInterpolation.NON;
								Data.TimeCurveStart = 0.0f;
								Data.ValueCurveStart = 0.0f;
								Data.TimeCurveEnd = 0.0f;
								Data.ValueCurveEnd = 0.0f;

								/* Body-Data Set */
								bool FlagValidCell = true;
								ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value/mapId", ManagerNameSpace);
								if(null == ValueTextParameter)
								{
									Data.Value.IndexCellMap = -1;
									Data.Value.IndexCell = -1;
									FlagValidCell = false;
								}
								else
								{
									int IndexCellMap = InformationAnimationSet.ListIndexCellMap[LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(ValueTextParameter)];
									Data.Value.IndexCellMap = IndexCellMap;

									ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value/name", ManagerNameSpace);
									if(null == ValueTextParameter)
									{
										Data.Value.IndexCell = -1;
										FlagValidCell = false;
									}
									else
									{
										int IndexCell = InformationProject.ListInformationSSCE[IndexCellMap].IndexGetCell(ValueTextParameter);
										Data.Value.IndexCell = IndexCell;
										if(-1 == IndexCell)
										{
											FlagValidCell = false;
										}
									}
								}
								if(false == FlagValidCell)
								{
									Debug.LogWarning(	"SSAE-Import Warning (Attribute): "
														+ "Cell-Data Invalid."
														+ ": FrameNo[" + FrameNo.ToString() + "]"
														+ ": Animation-Name[" + InformationAnimation.Name + "]"
														+ ": Parts-Name[" + InformationAnimationSet.ListParts[Index].Name + "]"
														+ ": File[" + NameFileSSAE + "]"
														+ " SSPJ[" + InformationProject.NameFileBody + InformationProject.NameFileExtension + "]"
													);
								}

								/* Key-Data Add */
								InformationAnimationPart.Attribute[InformationAttribute.IndexAttribute].Add(Data);
							}
							break;

						case LibraryEditor_SpriteStudio.KeyFrame.KindValue.COLORBLEND:
							{
								LibraryEditor_SpriteStudio.KeyFrame.DataColorBlend Data = new LibraryEditor_SpriteStudio.KeyFrame.DataColorBlend();
								Data.FrameNo = FrameNo;

								/* Curve-Data Set */
								Data.KindCurve = KindCurve;
								Data.TimeCurveStart = TimeCurveStart;
								Data.ValueCurveStart = ValueCurveStart;
								Data.TimeCurveEnd = TimeCurveEnd;
								Data.ValueCurveEnd = ValueCurveEnd;

								/* Body-Data Set */
								ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value/blendType", ManagerNameSpace);
								switch(ValueTextParameter)
								{
									case "mix":
										Data.Value.Operation = Library_SpriteStudio.KindColorOperation.MIX;
										break;

									case "mul":
										Data.Value.Operation = Library_SpriteStudio.KindColorOperation.MUL;
										break;

									case "add":
										Data.Value.Operation = Library_SpriteStudio.KindColorOperation.ADD;
										break;

									case "sub":
										Data.Value.Operation = Library_SpriteStudio.KindColorOperation.SUB;
										break;

									default:
										Debug.LogWarning(	"SSAE-Import Warning (Attribute): "
															+ "ColorBlend-Data Invalid. Operation[" + ValueTextParameter + "]"
															+ ": FrameNo[" + FrameNo.ToString() + "]"
															+ ": Animation-Name[" + InformationAnimation.Name + "]"
															+ ": Parts-Name[" + InformationAnimationSet.ListParts[Index].Name + "]"
															+ ": File[" + NameFileSSAE + "]"
															+ " SSPJ[" + InformationProject.NameFileBody + InformationProject.NameFileExtension + "]"
														);
										Data.Value.Operation = Library_SpriteStudio.KindColorOperation.NON;
										break;
								}

								float ColorA = 0.0f;
								float ColorR = 0.0f;
								float ColorG = 0.0f;
								float ColorB = 0.0f;
								float RatePixel = 0.0f;
								ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value/target", ManagerNameSpace);
								switch(ValueTextParameter)
								{
									case "whole":
										{
											Data.Value.Bound = Library_SpriteStudio.KindColorBound.OVERALL;

											ImportSSAEKeyDataGetColorBlend(	out ColorA, out ColorR, out ColorG, out ColorB, out RatePixel,
																			NodeKey, "value/color", ManagerNameSpace
																		);
											for(int i=0; i<(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2; i++)
											{
												Data.Value.VertexColor[i].r = ColorR;
												Data.Value.VertexColor[i].g = ColorG;
												Data.Value.VertexColor[i].b = ColorB;
												Data.Value.VertexColor[i].a = ColorA;
												Data.Value.RatePixelAlpha[i] = RatePixel;
											}
										}
										break;

									case "vertex":
										{
											Data.Value.Bound = Library_SpriteStudio.KindColorBound.VERTEX;

											ImportSSAEKeyDataGetColorBlend(	out ColorA, out ColorR, out ColorG, out ColorB, out RatePixel,
																			NodeKey, "value/LT", ManagerNameSpace
																		);
											Data.Value.VertexColor[0].r = ColorR;
											Data.Value.VertexColor[0].g = ColorG;
											Data.Value.VertexColor[0].b = ColorB;
											Data.Value.VertexColor[0].a = ColorA;
											Data.Value.RatePixelAlpha[0] = RatePixel;

											ImportSSAEKeyDataGetColorBlend(	out ColorA, out ColorR, out ColorG, out ColorB, out RatePixel,
																			NodeKey, "value/RT", ManagerNameSpace
																		);
											Data.Value.VertexColor[1].r = ColorR;
											Data.Value.VertexColor[1].g = ColorG;
											Data.Value.VertexColor[1].b = ColorB;
											Data.Value.VertexColor[1].a = ColorA;
											Data.Value.RatePixelAlpha[1] = RatePixel;

											ImportSSAEKeyDataGetColorBlend(	out ColorA, out ColorR, out ColorG, out ColorB, out RatePixel,
																			NodeKey, "value/RB", ManagerNameSpace
																		);
											Data.Value.VertexColor[2].r = ColorR;
											Data.Value.VertexColor[2].g = ColorG;
											Data.Value.VertexColor[2].b = ColorB;
											Data.Value.VertexColor[2].a = ColorA;
											Data.Value.RatePixelAlpha[2] = RatePixel;

											ImportSSAEKeyDataGetColorBlend(	out ColorA, out ColorR, out ColorG, out ColorB, out RatePixel,
																			NodeKey, "value/LB", ManagerNameSpace
																		);
											Data.Value.VertexColor[3].r = ColorR;
											Data.Value.VertexColor[3].g = ColorG;
											Data.Value.VertexColor[3].b = ColorB;
											Data.Value.VertexColor[3].a = ColorA;
											Data.Value.RatePixelAlpha[3] = RatePixel;
										}
										break;

									default:
										{
											Debug.LogWarning(	"SSAE-Import Warning (Attribute): "
																+ "ColorBlend-Data Invalid. Bound[" + ValueTextParameter + "]"
																+ ": FrameNo[" + FrameNo.ToString() + "]"
																+ ": Animation-Name[" + InformationAnimation.Name + "]"
																+ ": Parts-Name[" + InformationAnimationSet.ListParts[Index].Name + "]"
																+ ": File[" + NameFileSSAE + "]"
																+ " SSPJ[" + InformationProject.NameFileBody + InformationProject.NameFileExtension + "]"
															);
											Data.Value.Bound = Library_SpriteStudio.KindColorBound.OVERALL;
											for(int i=0; i<(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2; i++)
											{
												Data.Value.VertexColor[i].r = 0.0f;
												Data.Value.VertexColor[i].g = 0.0f;
												Data.Value.VertexColor[i].b = 0.0f;
												Data.Value.VertexColor[i].a = 0.0f;
												Data.Value.RatePixelAlpha[i] = 1.0f;
											}
										}
										break;
								}

								/* Key-Data Add */
								InformationAnimationPart.Attribute[InformationAttribute.IndexAttribute].Add(Data);
							}
							break;

						case LibraryEditor_SpriteStudio.KeyFrame.KindValue.VERTEXCORRECTION:
							{
								LibraryEditor_SpriteStudio.KeyFrame.DataVertexCorrection Data = new LibraryEditor_SpriteStudio.KeyFrame.DataVertexCorrection();
								Data.FrameNo = FrameNo;

								/* Curve-Data Set */
								Data.KindCurve = KindCurve;
								Data.TimeCurveStart = TimeCurveStart;
								Data.ValueCurveStart = ValueCurveStart;
								Data.TimeCurveEnd = TimeCurveEnd;
								Data.ValueCurveEnd = ValueCurveEnd;

								/* Body-Data Set */
								ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value/LT", ManagerNameSpace);
								ValueTextParameterArgument = ValueTextParameter.Split(' ');
								Data.Value.Coordinate[(int)Library_SpriteStudio.KindVertexNo.LU].x = (float)(LibraryEditor_SpriteStudio.Utility.Text.ValueGetDouble(ValueTextParameterArgument[0]));
								Data.Value.Coordinate[(int)Library_SpriteStudio.KindVertexNo.LU].y = (float)(LibraryEditor_SpriteStudio.Utility.Text.ValueGetDouble(ValueTextParameterArgument[1]));

								ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value/RT", ManagerNameSpace);
								ValueTextParameterArgument = ValueTextParameter.Split(' ');
								Data.Value.Coordinate[(int)Library_SpriteStudio.KindVertexNo.RU].x = (float)(LibraryEditor_SpriteStudio.Utility.Text.ValueGetDouble(ValueTextParameterArgument[0]));
								Data.Value.Coordinate[(int)Library_SpriteStudio.KindVertexNo.RU].y = (float)(LibraryEditor_SpriteStudio.Utility.Text.ValueGetDouble(ValueTextParameterArgument[1]));

								ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value/RB", ManagerNameSpace);
								ValueTextParameterArgument = ValueTextParameter.Split(' ');
								Data.Value.Coordinate[(int)Library_SpriteStudio.KindVertexNo.RD].x = (float)(LibraryEditor_SpriteStudio.Utility.Text.ValueGetDouble(ValueTextParameterArgument[0]));
								Data.Value.Coordinate[(int)Library_SpriteStudio.KindVertexNo.RD].y = (float)(LibraryEditor_SpriteStudio.Utility.Text.ValueGetDouble(ValueTextParameterArgument[1]));

								ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value/LB", ManagerNameSpace);
								ValueTextParameterArgument = ValueTextParameter.Split(' ');
								Data.Value.Coordinate[(int)Library_SpriteStudio.KindVertexNo.LD].x = (float)(LibraryEditor_SpriteStudio.Utility.Text.ValueGetDouble(ValueTextParameterArgument[0]));
								Data.Value.Coordinate[(int)Library_SpriteStudio.KindVertexNo.LD].y = (float)(LibraryEditor_SpriteStudio.Utility.Text.ValueGetDouble(ValueTextParameterArgument[1]));

								/* Key-Data Add */
								InformationAnimationPart.Attribute[InformationAttribute.IndexAttribute].Add(Data);
							}
							break;

						case LibraryEditor_SpriteStudio.KeyFrame.KindValue.INSTANCE:
							{
								LibraryEditor_SpriteStudio.KeyFrame.DataInstance Data = new LibraryEditor_SpriteStudio.KeyFrame.DataInstance();
								Data.FrameNo = FrameNo;

								/* Curve-Data Set */
								Data.KindCurve = KindCurve;
								Data.TimeCurveStart = TimeCurveStart;
								Data.ValueCurveStart = ValueCurveStart;
								Data.TimeCurveEnd = TimeCurveEnd;
								Data.ValueCurveEnd = ValueCurveEnd;

								/* Body-Data Set */
								Data.Value.PlayCount = -1;
								ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value/infinity", ManagerNameSpace);
								if(null != ValueTextParameter)
								{	/* Check */
									if(true == LibraryEditor_SpriteStudio.Utility.Text.ValueGetBool(ValueTextParameter))
									{
										Data.Value.PlayCount = 0;
									}
								}
								if(-1 == Data.Value.PlayCount)
								{	/* Loop-Limited */
									ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value/loopNum", ManagerNameSpace);
									Data.Value.PlayCount = (null == ValueTextParameter) ? 1 : LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(ValueTextParameter);
								}

								float SignRateSpeed = 1.0f;
								ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value/reverse", ManagerNameSpace);
								if(null != ValueTextParameter)
								{
									SignRateSpeed = (true == LibraryEditor_SpriteStudio.Utility.Text.ValueGetBool(ValueTextParameter)) ? -1.0f : 1.0f;
								}

								ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value/pingpong", ManagerNameSpace);
								if(null == ValueTextParameter)
								{
									Data.Value.Flags &= ~Library_SpriteStudio.Data.AttributeInstance.FlagBit.PINGPONG;
								}
								else
								{
									Data.Value.Flags = (true == LibraryEditor_SpriteStudio.Utility.Text.ValueGetBool(ValueTextParameter)) ?
															(Data.Value.Flags | Library_SpriteStudio.Data.AttributeInstance.FlagBit.PINGPONG)
															: (Data.Value.Flags & ~Library_SpriteStudio.Data.AttributeInstance.FlagBit.PINGPONG);
								}

								ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value/independent", ManagerNameSpace);
								if(null == ValueTextParameter)
								{
									Data.Value.Flags &= ~Library_SpriteStudio.Data.AttributeInstance.FlagBit.INDEPENDENT;
								}
								else
								{
									Data.Value.Flags = (true == LibraryEditor_SpriteStudio.Utility.Text.ValueGetBool(ValueTextParameter)) ?
															(Data.Value.Flags | Library_SpriteStudio.Data.AttributeInstance.FlagBit.INDEPENDENT)
															: (Data.Value.Flags & ~Library_SpriteStudio.Data.AttributeInstance.FlagBit.INDEPENDENT);
								}

								ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value/startLabel", ManagerNameSpace);
								Data.Value.LabelStart = (null == ValueTextParameter) ? string.Copy(Library_SpriteStudio.ListNameLabelAnimationReserved[(int)Library_SpriteStudio.KindLabelAnimationReserved.START]) : string.Copy(ValueTextParameter);

								ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value/startOffset", ManagerNameSpace);
								Data.Value.OffsetStart = (null == ValueTextParameter) ? 0 : LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(ValueTextParameter);

								ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value/endLabel", ManagerNameSpace);
								Data.Value.LabelEnd = (null == ValueTextParameter) ? string.Copy(Library_SpriteStudio.ListNameLabelAnimationReserved[(int)Library_SpriteStudio.KindLabelAnimationReserved.END]) : string.Copy(ValueTextParameter);

								ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value/endOffset", ManagerNameSpace);
								Data.Value.OffsetEnd = (null == ValueTextParameter) ? 0 : LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(ValueTextParameter);

								ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value/speed", ManagerNameSpace);
								Data.Value.RateTime = (null == ValueTextParameter) ? 1.0f : (float)LibraryEditor_SpriteStudio.Utility.Text.ValueGetDouble(ValueTextParameter);
								Data.Value.RateTime *= SignRateSpeed;

								/* Key-Data Add */
								InformationAnimationPart.Attribute[InformationAttribute.IndexAttribute].Add(Data);
							}
							break;

						case LibraryEditor_SpriteStudio.KeyFrame.KindValue.EFFECT:
							{
								LibraryEditor_SpriteStudio.KeyFrame.DataEffect Data = new LibraryEditor_SpriteStudio.KeyFrame.DataEffect();
								Data.FrameNo = FrameNo;

								/* Body-Data Set */
								ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value/startTime", ManagerNameSpace);
								Data.Value.FrameStart = (null == ValueTextParameter) ? 0 : LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(ValueTextParameter);

								ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value/speed", ManagerNameSpace);
								Data.Value.RateTime = (null == ValueTextParameter) ? 1.0f : LibraryEditor_SpriteStudio.Utility.Text.ValueGetFloat(ValueTextParameter);

								ValueTextParameter = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, "value/independent", ManagerNameSpace);
								if(null == ValueTextParameter)
								{
									Data.Value.Flags &= ~Library_SpriteStudio.Data.AttributeEffect.FlagBit.INDEPENDENT;
								}
								else
								{
									Data.Value.Flags = (true == LibraryEditor_SpriteStudio.Utility.Text.ValueGetBool(ValueTextParameter)) ?
															(Data.Value.Flags | Library_SpriteStudio.Data.AttributeEffect.FlagBit.INDEPENDENT)
															: (Data.Value.Flags & ~Library_SpriteStudio.Data.AttributeEffect.FlagBit.INDEPENDENT);
								}

								/* Key-Data Add */
								InformationAnimationPart.Attribute[InformationAttribute.IndexAttribute].Add(Data);
							}
							break;
					}
				}
			}

			/* Erase attributes that must not be set */
			ImportSSAEAnimationPartsIgnoreAttributes(InformationAnimationSet, InformationAnimationPart, Index);

			return(true);

		ParseOPSS_InformationSSAEAnimationParts_ErrorEnd:;
			Debug.LogError(	"SSAE-Import Error (Attribute): "
							+ MessageError
							+ ": Animation-Name[" + InformationAnimation.Name + "]"
							+ ": Parts-Name[" + InformationAnimationSet.ListParts[Index].Name + "]"
							+ ": File[" + NameFileSSAE + "]"
							+ " SSPJ[" + InformationProject.NameFileBody + InformationProject.NameFileExtension + "]"
						);
			return(false);
		}
		private static void ImportSSAEKeyDataGetColorBlend(	out float ColorA,
															out float ColorR,
															out float ColorG,
															out float ColorB,
															out float RatePixel,
															XmlNode NodeKey,
															string NameTagBase,
															XmlNamespaceManager ManagerNameSpace
														)
		{
			string ValueText = "";

			ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, NameTagBase + "/rgba", ManagerNameSpace);
			uint ARGB = LibraryEditor_SpriteStudio.Utility.Text.HexToUInt(ValueText);
			RatePixel = (float)((ARGB >> 24) & 0xff) / 255.0f;
			ColorR = (float)((ARGB >> 16) & 0xff) / 255.0f;
			ColorG = (float)((ARGB >> 8) & 0xff) / 255.0f;
			ColorB = (float)(ARGB & 0xff) / 255.0f;

			ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeKey, NameTagBase + "/rate", ManagerNameSpace);
			ColorA = (float)(LibraryEditor_SpriteStudio.Utility.Text.ValueGetDouble(ValueText));
		}
		private static void ImportSSAEAnimationPartsIgnoreAttributes(	LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAE InformationAnimationSet,
																		LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEAnimationParts InformationAnimationPart,
																		int IDParts
																	)
		{
			InformationSSAEParts InformationPart = InformationAnimationSet.ListParts[IDParts];
			LibraryEditor_SpriteStudio.KeyFrame.KindPartsIgnoreAttribute KindIgnore = LibraryEditor_SpriteStudio.KeyFrame.KindPartsToIgnore[(int)InformationPart.Kind];
			LibraryEditor_SpriteStudio.KeyFrame.KindAttribute[] ListAttributeIgnore = LibraryEditor_SpriteStudio.KeyFrame.ListKindAttributeIgnore[(int)KindIgnore];
			int Count = ListAttributeIgnore.Length;
			int IndexAttribute;
			ArrayList ListAttribute = null;
			for(int i=0; i<Count; i++)
			{
				/* Key-Datas Purge & Erase */
				IndexAttribute = (int)ListAttributeIgnore[i];
				ListAttribute = InformationAnimationPart.Attribute[IndexAttribute];
#if false
				InformationAnimationPart.Attribute[IndexAttribute] = null;
				if(null != ListAttribute)
				{
					ListAttribute.Clear();
				}
#else
				ListAttribute.Clear();
#endif
			}
		}

		internal enum KindVersionSSAE
		{
			ERROR = 0x00000000,
			VERSION_000100 = 0x00000100,
			VERSION_010000 = 0x00010000,
			VERSION_010001 = 0x00010001,
			VERSION_010002 = 0x00010002,	/* ssae ver.5.3.5 */
			VERSION_010200 = 0x00010200,    /* ssae ver.5.5.0 beta-3 */
			VERSION_010201 = 0x00010201,	/* ssae ver.5.7.0 beta-1 */
			VERSION_010202 = 0x00010202,	/* ssae ver.5.7.0 beta-2 */

			VERSION_LATEST = VERSION_010202
		};
		internal class InformationSSAE
		{
			/* SSAE Data: Base Data */
			internal string Name;

			/* SSAE Data: Image Datas */
			internal KindVersionSSAE VersionCode;

			/* SSAE Data: Parts Datas */
			internal InformationSSAEParts[] ListParts;

			/* SSAE Data: Animation Datas */
			internal InformationSSAEAnimation[] ListAnimation;

			/* WorkArea: (SSAE File's) File/Path Name */
			internal string NameDirectory;
			internal string NameFileBody;
			internal string NameFileExtension;

			/* WorkArea: CellMap Index (SSAE -> SSCE) */
			internal int[] ListIndexCellMap;

			/* Converted Data: Runtime-Datas */
			internal Library_SpriteStudio.Data.Parts[] ListDataPartsRuntime;
			internal Library_SpriteStudio.Data.Animation[] ListDataAnimationRuntime;
			internal ScriptableObject PrefabDataAnimation;
			internal string NamePrefabDataAnimation;
			internal UnityEngine.Object PrefabGameObject;
			internal string NamePrefabGameObject;

			internal void CleanUp()
			{
				VersionCode = LibraryEditor_SpriteStudio.ParseOPSS.KindVersionSSAE.ERROR;

				ListParts = null;
				ListAnimation = null;

				NameDirectory = "";
				NameFileBody = "";
				NameFileExtension = "";

				ListDataPartsRuntime = null;
				ListDataAnimationRuntime = null;
				PrefabDataAnimation = null;
				NamePrefabDataAnimation = "";
				PrefabGameObject = null;
				NamePrefabGameObject = "";

				ListIndexCellMap = null;
			}

			internal int IndexGetParts(string Name)
			{
				if(null != ListParts)
				{
					for(int i=0; i<ListParts.Length; i++)
					{
						if(0 == string.Compare(Name, ListParts[i].Name))
						{
							return(i);
						}
					}
				}
				return(-1);
			}

			internal int IndexGetAnimation(string Name)
			{
				if(null != ListAnimation)
				{
					for(int i=0; i<ListAnimation.Length; i++)
					{
						if(0 == string.Compare(Name, ListAnimation[i].Name))
						{
							return(i);
						}
					}
				}
				return(-1);
			}

			internal string NamePathGetAbsolute(string NameFile)
			{
				string NameFileFullPath = NameDirectory + NameFile;
				NameFileFullPath = Path.GetFullPath(NameFileFullPath);
				NameFileFullPath = NameFileFullPath.Replace("\\", "/");	/* "\" -> "/" */

				string FileNamePath = Path.GetDirectoryName(NameFileFullPath);
				string FileNameBody = Path.GetFileNameWithoutExtension(NameFileFullPath);
				string FileNameExtension = Path.GetExtension(NameFileFullPath);
				return(FileNamePath + "/" + FileNameBody + FileNameExtension);
			}
		}
		internal class InformationSSAEParts : Library_SpriteStudio.Data.Parts
		{
			internal enum KindTypeInheritance
			{
				PARENT = 0,
				SELF
			}
			internal enum FlagBitInheritance
			{
				OPACITY_RATE = 0x000000001,
				SHOW_HIDE = 0x000000002,
				FLIP_X = 0x000000010,
				FLIP_Y = 0x000000020,

				CLEAR = 0x00000000,
				ALL = OPACITY_RATE
					| SHOW_HIDE
					| FLIP_X
					| FLIP_Y,
				PRESET = OPACITY_RATE
//						| FLIP_X
//						| FLIP_Y
//						| SHOW_HIDE
			}

			internal List<int> ListIndexPartsChild;

			internal string NameSSAEInstance;
//			internal string NameAnimationInstance;	/* Present in the base-class */

			internal string NameSSEEEffect;

			internal int IndexSSAEInstance;
			internal int IndexAnimationInstance;

			internal int IndexSSEEEffect;

			internal KindTypeInheritance KindInheritance;
			internal FlagBitInheritance FlagInheritance;

			internal new void CleanUp()
			{
				base.CleanUp();

				NameSSAEInstance = "";
//				NameAnimationInstance = "";

				NameSSEEEffect = "";

				IndexSSAEInstance = -1;
				IndexAnimationInstance = -1;

				IndexSSEEEffect = -1;

				KindInheritance = KindTypeInheritance.PARENT;
				FlagInheritance = FlagBitInheritance.CLEAR;
			}
		}
		internal class InformationSSAEAnimation : Library_SpriteStudio.Data.Animation
		{
			internal InformationSSAEAnimationParts[] ListAnimationPartsKeyFrames;

			internal new void CleanUp()
			{
				base.CleanUp();
				ListAnimationPartsKeyFrames = null;
			}
		}
		internal class InformationSSAEAnimationParts
		{
			internal ArrayList[] Attribute;

			internal void CleanUp()
			{
				Attribute = null;
			}

			internal bool BootUp()
			{
				int Count = (int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.TERMINATOR;
				Attribute = new ArrayList[Count];
				if(null == Attribute)
				{
					return(false);
				}
				for(int i=0; i<Count; i++)
				{
					Attribute[i] = new ArrayList();
					if(null == Attribute[i])
					{
						return(false);
					}
					Attribute[i].Clear();
				}
				return(true);
			}

			internal void ShutDown()
			{
				int Count = (int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.TERMINATOR;
				if(null != Attribute)
				{
					for(int i=0; i<Count; i++)
					{
						if(null != Attribute[i])
						{
							Attribute[i].Clear();
							Attribute[i] = null;
						}
					}
				}
				CleanUp();
			}
		}

		/* for Parsing ".ssee" */
		internal static LibraryEditor_SpriteStudio.ParseOPSS.InformationSSEE ImportSSEE(	ref SettingImport DataSettingImport,
																							InformationSSPJ InformationProject,
																							int IndexSSEE
																						)
		{
			string MessageError = "";

			/* Open SSAE-File */
			if((0 > IndexSSEE) || (InformationProject.ListNameSSEE.Count <= IndexSSEE))
			{
				MessageError = "SSEE Index[" + IndexSSEE.ToString() + "] is Invalid.";
				goto ParseOPSS_ImportSSEE_ErrorEnd;
			}
			string NamePathSSEE = (string)InformationProject.ListNameSSEE[IndexSSEE];
			XmlDocument DataXML = new XmlDocument();
			DataXML.Load(NamePathSSEE);

			/* Create WorkArea */
			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSEE InformationEffect = new InformationSSEE();
			if(null == InformationEffect)
			{
				MessageError = "Not Enough Memory (SSEE Work-Area)";
				goto ParseOPSS_ImportSSEE_ErrorEnd;
			}
			InformationEffect.CleanUp();
			InformationEffect.Name = LibraryEditor_SpriteStudio.Utility.Text.PathGetRelative(	NamePathSSEE,
																								InformationProject,
																								LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.SSEE
																							);
			InformationEffect.Name = LibraryEditor_SpriteStudio.Utility.Text.DataNameGetFromPath(InformationEffect.Name, DataSettingImport.FlagNameDataRuleOld);
			InformationEffect.NameDirectory = Path.GetDirectoryName(NamePathSSEE) + "/";
			InformationEffect.NameFileBody = Path.GetFileNameWithoutExtension(NamePathSSEE);
			InformationEffect.NameFileExtension = Path.GetExtension(NamePathSSEE);

			/* Version-Check */
			XmlNode NodeRoot = DataXML.FirstChild;
			NodeRoot = NodeRoot.NextSibling;
			KindVersionSSEE VersionCode = (KindVersionSSEE)(LibraryEditor_SpriteStudio.Utility.XML.VersionCodeGet(NodeRoot, "SpriteStudioEffect", (int)KindVersionSSEE.ERROR, false));
			switch(VersionCode)
			{
				case KindVersionSSEE.VERSION_010000:
				case KindVersionSSEE.VERSION_010001:
					goto case KindVersionSSEE.ERROR;

				case KindVersionSSEE.VERSION_010002:
					/* MEMO: SS5.6 Unsupported */
#if false
					goto case KindVersionSSEE.ERROR;
#else
					/* MEMO: SS5.7-Underdevelopment */
					VersionCode = KindVersionSSEE.VERSION_010100;
					goto case KindVersionSSEE.VERSION_010100;
#endif

				case KindVersionSSEE.VERSION_010100:
					break;

				case KindVersionSSEE.ERROR:
					MessageError = "Not Supported Version.";
					goto ParseOPSS_ImportSSEE_ErrorEnd;

				default:
					if(KindVersionSSEE.VERSION_LATEST < VersionCode)
					{	/* MEMO: Dealing as the latest supported version. */
						VersionCode = KindVersionSSEE.VERSION_LATEST;
						goto case KindVersionSSEE.VERSION_LATEST;
					}
					goto case KindVersionSSEE.ERROR;
			}
			InformationEffect.VersionCode = VersionCode;

			NameTable NodeNameSpace = new NameTable();
			XmlNamespaceManager ManagerNameSpace = new XmlNamespaceManager(NodeNameSpace);
			XmlNodeList NodeList = null;

			/* Base-Data Get */
			string ValueText = "";

			switch(InformationEffect.VersionCode)
			{
				case KindVersionSSEE.VERSION_010002:
					/* MEMO: SS5.6 Unsupported */
					/* MEMO: SS5.7-Underdevelopment */
					break;

				case KindVersionSSEE.VERSION_010100:
					{	/* SS5.7 */
						ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeRoot, "effectData/lockRandSeed", ManagerNameSpace);
						if(false == string.IsNullOrEmpty(ValueText))
						{
							InformationEffect.Seed = LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(ValueText);
						}

						ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeRoot, "effectData/isLockRandSeed", ManagerNameSpace);
						if(false == string.IsNullOrEmpty(ValueText))
						{
							InformationEffect.FlagLockSeed = LibraryEditor_SpriteStudio.Utility.Text.ValueGetBool(ValueText);
						}

						ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeRoot, "effectData/fps", ManagerNameSpace);
						if(false == string.IsNullOrEmpty(ValueText))
						{
							InformationEffect.FramePerSecond = LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(ValueText);
						}

						ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeRoot, "effectData/renderVersion", ManagerNameSpace);
						if(false == string.IsNullOrEmpty(ValueText))
						{
							InformationEffect.VersionRenderer = LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(ValueText);
						}

						ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeRoot, "effectData/layoutScaleX", ManagerNameSpace);
						if(false == string.IsNullOrEmpty(ValueText))
						{
							InformationEffect.ScaleLayout.x = (float)(LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(ValueText)) / 100.0f;
						}

						ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeRoot, "effectData/layoutScaleY", ManagerNameSpace);
						if(false == string.IsNullOrEmpty(ValueText))
						{
							InformationEffect.ScaleLayout.y = (float)(LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(ValueText)) / 100.0f;
						}

						/* Parts-Data Get */
						NodeList = LibraryEditor_SpriteStudio.Utility.XML.ListGetNode(NodeRoot, "effectData/nodeList/node", ManagerNameSpace);
						if(null == NodeList)
						{
							MessageError = "PartsList-Node[Effect] Not-Found.";
							goto ParseOPSS_ImportSSEE_ErrorEnd;
						}
						InformationEffect.ListParts = new LibraryEditor_SpriteStudio.ParseOPSS.InformationSSEEParts[NodeList.Count];
						if(null == InformationEffect.ListParts)
						{
							MessageError = "Not Enough Memory. (Parts-Data[Effect] WorkArea)";
							goto ParseOPSS_ImportSSEE_ErrorEnd;
						}
						foreach(XmlNode NodeParts in NodeList)
						{
							/* Part-ID Get */
							int IDParts = LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeParts, "arrayIndex", ManagerNameSpace));

							/* Part-Data Get */
							InformationEffect.ListParts[IDParts] = ImportSSEEParts(	ref DataSettingImport,
																						InformationProject,
																						NodeParts,
																						ManagerNameSpace,
																						InformationEffect,
																						IDParts,
																						(string)InformationProject.ListNameSSEE[IndexSSEE]
																					);
							if(null == InformationEffect.ListParts[IDParts])
							{
								goto ParseOPSS_ImportSSEE_ErrorEnd_NoMessage;
							}
						}
					}
					break;
			}
			return(InformationEffect);

		ParseOPSS_ImportSSEE_ErrorEnd:;
			Debug.LogError(	"SSEE-Import Error: "
							+ MessageError + ": File[" + (string)InformationProject.ListNameSSEE[IndexSSEE] + "]"
							+ " SSPJ[" + InformationProject.NameFileBody + InformationProject.NameFileExtension + "]"
						);
		ParseOPSS_ImportSSEE_ErrorEnd_NoMessage:;
			return(null);
		}
		internal static LibraryEditor_SpriteStudio.ParseOPSS.InformationSSEEParts ImportSSEEParts(	ref SettingImport DataSettingImport,
																										InformationSSPJ InformationProject,
																										XmlNode NodeParts,
																										XmlNamespaceManager ManagerNameSpace,
																										LibraryEditor_SpriteStudio.ParseOPSS.InformationSSEE InformationEffect,
																										int ID,
																										string NameFileSSEE
																									)
		{	/* MEMO: SS5.7 */
			string MessageError = "";

			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSEEParts InformationPartsEffect = new LibraryEditor_SpriteStudio.ParseOPSS.InformationSSEEParts();
			if(null == InformationPartsEffect)
			{
				MessageError = "Parts[" + ID.ToString() + "] Not Enough Memory. (Effect-Parts WorkArea)";
				goto ParseOPSS_InformationSSEEParts_ErrorEnd;
			}
			InformationPartsEffect.CleanUp();
			InformationPartsEffect.ListIndexPartsChild = new List<int>();
			InformationPartsEffect.ListIndexPartsChild.Clear();

			/* Base Datas Get */
			string ValueText = "";

			ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeParts, "name", ManagerNameSpace);
			InformationPartsEffect.Name = string.Copy(ValueText);

			InformationPartsEffect.ID = ID;

			ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeParts, "parentIndex", ManagerNameSpace);
			InformationPartsEffect.IDParent = LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(ValueText);
			if(0 <= InformationPartsEffect.IDParent)
			{
				LibraryEditor_SpriteStudio.ParseOPSS.InformationSSEEParts InformationPartsEffectParent = InformationEffect.ListParts[InformationPartsEffect.IDParent];
				InformationPartsEffectParent.ListIndexPartsChild.Add(InformationPartsEffect.ID);
			}

			Library_SpriteStudio.KindColorOperationEffect KindColorOperationTartget = Library_SpriteStudio.KindColorOperationEffect.NON;
			ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeParts, "behavior/BlendType", ManagerNameSpace);
			if(false == string.IsNullOrEmpty(ValueText))
			{
				switch(ValueText)
				{
					case "Mix":
						KindColorOperationTartget = Library_SpriteStudio.KindColorOperationEffect.MIX;
						break;

					case "Add":
						KindColorOperationTartget = Library_SpriteStudio.KindColorOperationEffect.ADD;
						break;

					default:
						Debug.LogWarning("SSEE-Import Warning: Parts["
											+ InformationPartsEffect.ID.ToString()
											+ "] Invalid Alpha-Blend Kind.: "
											+ ValueText
										);
						goto case "Mix";
				}
			}

			string NameCellMap = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeParts, "behavior/CellMapName", ManagerNameSpace);
			string NameCell = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeParts, "behavior/CellName", ManagerNameSpace);

			/* Parts-Kind Get */
			ValueText = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeParts, "type", ManagerNameSpace);
			switch(ValueText)
			{
				case "Root":
					if(true == string.IsNullOrEmpty(InformationPartsEffect.Name))
					{	/* Default-Name */
						InformationPartsEffect.Name = "Root";
					}
					InformationPartsEffect.Kind = Library_SpriteStudio.KindPartsEffect.ROOT;
					InformationPartsEffect.Emitter = null;
					break;

				case "Emmiter":	/* "Emitter" */
					if(true == string.IsNullOrEmpty(InformationPartsEffect.Name))
					{	/* Default-Name */
						InformationPartsEffect.Name = "Emitter";
					}
					InformationPartsEffect.Kind = Library_SpriteStudio.KindPartsEffect.EMITTER;
					InformationPartsEffect.Emitter = ImportSSEEPartsEmitter(	ref DataSettingImport,
																				InformationProject,
																				NodeParts,
																				ManagerNameSpace,
																				InformationEffect,
																				InformationPartsEffect,
																				KindColorOperationTartget,
																				NameCellMap,
																				NameCell,
																				ID,
																				NameFileSSEE
																			);
					if(null == InformationPartsEffect.Emitter)
					{
						goto ParseOPSS_InformationSSEEParts_ErrorEnd_NoMessage;
					}
					break;

				case "Particle":
					if(true == string.IsNullOrEmpty(InformationPartsEffect.Name))
					{	/* Default-Name */
						InformationPartsEffect.Name = "Particle";
					}
					InformationPartsEffect.Kind = Library_SpriteStudio.KindPartsEffect.PARTICLE;
					InformationPartsEffect.Emitter = null;
					break;

				default:
					/* MEMO: Error */
					if(true == string.IsNullOrEmpty(InformationPartsEffect.Name))
					{	/* Default-Name */
						InformationPartsEffect.Name = "Error";
					}
					InformationPartsEffect.Kind = Library_SpriteStudio.KindPartsEffect.NON;
					InformationPartsEffect.Emitter = null;
					break;
			}

			return(InformationPartsEffect);

		ParseOPSS_InformationSSEEParts_ErrorEnd:;
			Debug.LogError(	"SSEE-Import Error (Parts): "
							+ MessageError + ": File[" + NameFileSSEE + "]"
							+ " SSPJ[" + InformationProject.NameFileBody + InformationProject.NameFileExtension + "]"
						);
		ParseOPSS_InformationSSEEParts_ErrorEnd_NoMessage:;
			return(null);
		}
		private static LibraryEditor_SpriteStudio.ParseOPSS.InformationSSEEEmitter ImportSSEEPartsEmitter(	ref SettingImport DataSettingImport,
																												InformationSSPJ InformationProject,
																												XmlNode NodeParts,
																												XmlNamespaceManager ManagerNameSpace,
																												LibraryEditor_SpriteStudio.ParseOPSS.InformationSSEE InformationEffectSet,
																												LibraryEditor_SpriteStudio.ParseOPSS.InformationSSEEParts InformationEffectParts,
																												Library_SpriteStudio.KindColorOperationEffect KindColorOperationTartget,
																												string NameCellMap,
																												string NameCell,
																												int ID,
																												string NameFileSSEE
																											)
		{	/* MEMO: SS5.7 */
			string MessageError = "";
			string ValueText = "";

			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSEEEmitter InformationEmitter = new LibraryEditor_SpriteStudio.ParseOPSS.InformationSSEEEmitter();
			if(null == InformationEmitter)
			{
				MessageError = "Parts[" + ID.ToString() + "] Not Enough Memory. (Effect-Emitter WorkArea)";
				goto ParseOPSS_InformationSSEEEmitter_ErrorEnd;
			}
//			InformationEmitter.CleanUp();
			InformationEmitter.BootUp();

			/* Base Datas Set */
			InformationEmitter.FlagData = Library_SpriteStudio.Data.EmitterEffect.FlagBit.CLEAR;
			InformationEmitter.KindBlendTarget = KindColorOperationTartget;
			InformationEmitter.NameCellMap = NameCellMap;
			InformationEmitter.NameCell = NameCell;
			ValueText = InformationEmitter.NameCellMap;
			if(false == string.IsNullOrEmpty(ValueText))
			{
				ValueText = LibraryEditor_SpriteStudio.Utility.Text.PathGetAbsolute(	ValueText,
																						InformationProject,
																						LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.SSCE
																					);
				InformationEmitter.IndexCellMap = InformationProject.IndexGetFileName(InformationProject.ListNameSSCE, ValueText);
			}
			InformationEmitter.IndexCell = -1;
			XmlNode NodeEmitterAttributes = LibraryEditor_SpriteStudio.Utility.XML.NodeGet(NodeParts, "behavior/list", ManagerNameSpace);

			string ValueTextAttribute = "";
			XmlNodeList ListNodeAttribute = LibraryEditor_SpriteStudio.Utility.XML.ListGetNode(NodeEmitterAttributes, "value", ManagerNameSpace);
			foreach(XmlNode NodeAttribute in ListNodeAttribute)
			{
				/* Attribute-Classification Set */
				ValueText = NodeAttribute.Attributes["name"].Value;
				switch(ValueText)
				{
					case "Basic":
						{
							InformationEmitter.FlagData |= Library_SpriteStudio.Data.EmitterEffect.FlagBit.BASIC;

							ValueTextAttribute = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeAttribute, "priority", ManagerNameSpace);
							if(false == string.IsNullOrEmpty(ValueTextAttribute))
							{
								InformationEmitter.PriorityParticle = LibraryEditor_SpriteStudio.Utility.Text.ValueGetFloat(ValueTextAttribute);
							}

							ValueTextAttribute = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeAttribute, "maximumParticle", ManagerNameSpace);
							if(false == string.IsNullOrEmpty(ValueTextAttribute))
							{
								InformationEmitter.CountParticleMax = LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(ValueTextAttribute);
							}

							ValueTextAttribute = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeAttribute, "attimeCreate", ManagerNameSpace);
							if(false == string.IsNullOrEmpty(ValueTextAttribute))
							{
								InformationEmitter.CountParticleEmit = LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(ValueTextAttribute);
							}

							ValueTextAttribute = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeAttribute, "interval", ManagerNameSpace);
							if(false == string.IsNullOrEmpty(ValueTextAttribute))
							{
								InformationEmitter.Interval = (int)(LibraryEditor_SpriteStudio.Utility.Text.ValueGetFloat(ValueTextAttribute));
							}

							ValueTextAttribute = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeAttribute, "lifetime", ManagerNameSpace);
							if(false == string.IsNullOrEmpty(ValueTextAttribute))
							{
								InformationEmitter.DurationEmitter = (int)(LibraryEditor_SpriteStudio.Utility.Text.ValueGetFloat(ValueTextAttribute));
							}

							ImportSSEEPartsEmitterGetRangeFloat(ref InformationEmitter.Speed.Main, ref InformationEmitter.Speed.Sub, NodeAttribute, "speed", ManagerNameSpace, InformationProject, ID, NameFileSSEE);

							ImportSSEEPartsEmitterGetRangeFloat(ref InformationEmitter.DurationParticle.Main, ref InformationEmitter.DurationParticle.Sub, NodeAttribute, "lifespan", ManagerNameSpace, InformationProject, ID, NameFileSSEE);

							ValueTextAttribute = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeAttribute, "angle", ManagerNameSpace);
							if(false == string.IsNullOrEmpty(ValueTextAttribute))
							{
								InformationEmitter.Angle.Main = LibraryEditor_SpriteStudio.Utility.Text.ValueGetFloat(ValueTextAttribute);
							}

							ValueTextAttribute = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeAttribute, "angleVariance", ManagerNameSpace);
							if(false == string.IsNullOrEmpty(ValueTextAttribute))
							{
								InformationEmitter.Angle.Sub = LibraryEditor_SpriteStudio.Utility.Text.ValueGetFloat(ValueTextAttribute);
							}
						}
						break;

					case "OverWriteSeed":
						{
							InformationEmitter.FlagData |= Library_SpriteStudio.Data.EmitterEffect.FlagBit.SEEDRANDOM;

							ValueTextAttribute = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeAttribute, "Seed", ManagerNameSpace);
							if(false == string.IsNullOrEmpty(ValueTextAttribute))
							{
								InformationEmitter.SeedRandom = LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(ValueTextAttribute);
							}
						}
						break;

					case "Delay":
						{
							InformationEmitter.FlagData |= Library_SpriteStudio.Data.EmitterEffect.FlagBit.DELAY;

							ValueTextAttribute = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeAttribute, "DelayTime", ManagerNameSpace);
							if(false == string.IsNullOrEmpty(ValueTextAttribute))
							{
								InformationEmitter.Delay = LibraryEditor_SpriteStudio.Utility.Text.ValueGetInt(ValueTextAttribute);
							}
						}
						break;

					case "Gravity":
						{
							InformationEmitter.FlagData |= Library_SpriteStudio.Data.EmitterEffect.FlagBit.GRAVITY_DIRECTION;

							ValueTextAttribute = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeAttribute, "Gravity", ManagerNameSpace);
							if(false == string.IsNullOrEmpty(ValueTextAttribute))
							{
								string[] TextSplit = ValueTextAttribute.Split(' ');
								if(2 == TextSplit.Length)
								{
									InformationEmitter.GravityDirectional.x = LibraryEditor_SpriteStudio.Utility.Text.ValueGetFloat(TextSplit[0]);
									InformationEmitter.GravityDirectional.y = LibraryEditor_SpriteStudio.Utility.Text.ValueGetFloat(TextSplit[1]);
								}
								else
								{
									Debug.LogError(	"SSEE-Import Waring (Parts-Emitter): "
													+ "Tag Broken: Tag[" + "Gravity/Gravity" + "]"
													+ " Parts-Name[" + InformationEffectParts.Name + "]"
													+ " File[" + NameFileSSEE + "]"
													+ " SSPJ[" + InformationProject.NameFileBody + InformationProject.NameFileExtension + "]"
												);
								}
							}
						}
						break;

					case "init_position":
						{
							InformationEmitter.FlagData |= Library_SpriteStudio.Data.EmitterEffect.FlagBit.POSITION;

							ImportSSEEPartsEmitterGetRangeFloat(ref InformationEmitter.Position.Main.x, ref InformationEmitter.Position.Sub.x, NodeAttribute, "OffsetX", ManagerNameSpace, InformationProject, ID, NameFileSSEE);
							ImportSSEEPartsEmitterGetRangeFloat(ref InformationEmitter.Position.Main.y, ref InformationEmitter.Position.Sub.y, NodeAttribute, "OffsetY", ManagerNameSpace, InformationProject, ID, NameFileSSEE);
						}
						break;

					case "trans_speed":
						{
							InformationEmitter.FlagData |= Library_SpriteStudio.Data.EmitterEffect.FlagBit.SPEED_FLUCTUATION;

							ImportSSEEPartsEmitterGetRangeFloat(ref InformationEmitter.SpeedFluctuation.Main, ref InformationEmitter.SpeedFluctuation.Sub, NodeAttribute, "Speed", ManagerNameSpace, InformationProject, ID, NameFileSSEE);
						}
						break;

					case "init_rotation":
						{
							InformationEmitter.FlagData |= Library_SpriteStudio.Data.EmitterEffect.FlagBit.ROTATION;

							ImportSSEEPartsEmitterGetRangeFloat(ref InformationEmitter.Rotation.Main, ref InformationEmitter.Rotation.Sub, NodeAttribute, "Rotation", ManagerNameSpace, InformationProject, ID, NameFileSSEE);

							ImportSSEEPartsEmitterGetRangeFloat(ref InformationEmitter.RotationFluctuation.Main, ref InformationEmitter.RotationFluctuation.Sub, NodeAttribute, "RotationAdd", ManagerNameSpace, InformationProject, ID, NameFileSSEE);
						}
						break;

					case "trans_rotation":
						{
							InformationEmitter.FlagData |= Library_SpriteStudio.Data.EmitterEffect.FlagBit.ROTATION_FLUCTUATION;

							ValueTextAttribute = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeAttribute, "RotationFactor", ManagerNameSpace);
							if(false == string.IsNullOrEmpty(ValueTextAttribute))
							{
								InformationEmitter.RotationFluctuationRate = LibraryEditor_SpriteStudio.Utility.Text.ValueGetFloat(ValueTextAttribute);
							}

							ValueTextAttribute = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeAttribute, "EndLifeTimePer", ManagerNameSpace);
							if(false == string.IsNullOrEmpty(ValueTextAttribute))
							{
								/* MEMO: Percent -> Rate */
								InformationEmitter.RotationFluctuationRateTime = LibraryEditor_SpriteStudio.Utility.Text.ValueGetFloat(ValueTextAttribute) * 0.01f;
							}
						}
						break;

					case "add_tangentiala":
						{
							InformationEmitter.FlagData |= Library_SpriteStudio.Data.EmitterEffect.FlagBit.TANGENTIALACCELATION;

							ImportSSEEPartsEmitterGetRangeFloat(ref InformationEmitter.RateTangentialAcceleration.Main, ref InformationEmitter.RateTangentialAcceleration.Sub, NodeAttribute, "Acceleration", ManagerNameSpace, InformationProject, ID, NameFileSSEE);
						}
						break;

					case "add_pointgravity":
						{
							InformationEmitter.FlagData |= Library_SpriteStudio.Data.EmitterEffect.FlagBit.GRAVITY_POINT;

							ValueTextAttribute = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeAttribute, "Position", ManagerNameSpace);
							if(false == string.IsNullOrEmpty(ValueTextAttribute))
							{
								string[] TextSplit = ValueTextAttribute.Split(' ');
								if(2 == TextSplit.Length)
								{
									InformationEmitter.GravityPointPosition.x = LibraryEditor_SpriteStudio.Utility.Text.ValueGetFloat(TextSplit[0]);
									InformationEmitter.GravityPointPosition.y = LibraryEditor_SpriteStudio.Utility.Text.ValueGetFloat(TextSplit[1]);
								}
								else
								{
									Debug.LogError(	"SSEE-Import Waring (Parts-Emitter): "
													+ "Tag Broken: Tag[" + "add_pointgravity/Position" + "]"
													+ " Parts-Name[" + InformationEffectParts.Name + "]"
													+ " File[" + NameFileSSEE + "]"
													+ " SSPJ[" + InformationProject.NameFileBody + InformationProject.NameFileExtension + "]"
												);
								}
							}

							ValueTextAttribute = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeAttribute, "Power", ManagerNameSpace);
							if(false == string.IsNullOrEmpty(ValueTextAttribute))
							{
								InformationEmitter.GravityPointPower = LibraryEditor_SpriteStudio.Utility.Text.ValueGetFloat(ValueTextAttribute);
							}
						}
						break;

					case "init_vertexcolor":
						{
							InformationEmitter.FlagData |= Library_SpriteStudio.Data.EmitterEffect.FlagBit.COLORVERTEX;

							ImportSSEEPartsEmitterGetRangeColor(ref InformationEmitter.ColorVertex.Main, ref InformationEmitter.ColorVertex.Sub, NodeAttribute, "Color", ManagerNameSpace, InformationProject, ID, NameFileSSEE);
						}
						break;

					case "trans_vertexcolor":
						{
							InformationEmitter.FlagData |= Library_SpriteStudio.Data.EmitterEffect.FlagBit.COLORVERTEX_FLUCTUATION;

							ImportSSEEPartsEmitterGetRangeColor(ref InformationEmitter.ColorVertexFluctuation.Main, ref InformationEmitter.ColorVertexFluctuation.Sub, NodeAttribute, "Color", ManagerNameSpace, InformationProject, ID, NameFileSSEE);
						}
						break;

					case "init_size":
						{
							InformationEmitter.FlagData |= Library_SpriteStudio.Data.EmitterEffect.FlagBit.SCALE_START;

							ImportSSEEPartsEmitterGetRangeFloat(ref InformationEmitter.ScaleStart.Main.x, ref InformationEmitter.ScaleStart.Sub.x, NodeAttribute, "SizeX", ManagerNameSpace, InformationProject, ID, NameFileSSEE);

							ImportSSEEPartsEmitterGetRangeFloat(ref InformationEmitter.ScaleStart.Main.y, ref InformationEmitter.ScaleStart.Sub.y, NodeAttribute, "SizeY", ManagerNameSpace, InformationProject, ID, NameFileSSEE);

							ImportSSEEPartsEmitterGetRangeFloat(ref InformationEmitter.ScaleRateStart.Main, ref InformationEmitter.ScaleRateStart.Sub, NodeAttribute, "ScaleFactor", ManagerNameSpace, InformationProject, ID, NameFileSSEE);
						}
						break;

					case "trans_size":
						{
							InformationEmitter.FlagData |= Library_SpriteStudio.Data.EmitterEffect.FlagBit.SCALE_END;

							ImportSSEEPartsEmitterGetRangeFloat(ref InformationEmitter.ScaleEnd.Main.x, ref InformationEmitter.ScaleEnd.Sub.x, NodeAttribute, "SizeX", ManagerNameSpace, InformationProject, ID, NameFileSSEE);

							ImportSSEEPartsEmitterGetRangeFloat(ref InformationEmitter.ScaleEnd.Main.y, ref InformationEmitter.ScaleEnd.Sub.y, NodeAttribute, "SizeY", ManagerNameSpace, InformationProject, ID, NameFileSSEE);

							ImportSSEEPartsEmitterGetRangeFloat(ref InformationEmitter.ScaleRateEnd.Main, ref InformationEmitter.ScaleRateEnd.Sub, NodeAttribute, "ScaleFactor", ManagerNameSpace, InformationProject, ID, NameFileSSEE);
						}
						break;

					case "trans_colorfade":
						{
							InformationEmitter.FlagData |= Library_SpriteStudio.Data.EmitterEffect.FlagBit.FADEALPHA;

							Library_SpriteStudio.Data.AttributeEffectRangeFloat RangeTemp = new Library_SpriteStudio.Data.AttributeEffectRangeFloat();
							ImportSSEEPartsEmitterGetRangeFloat(ref RangeTemp.Main, ref RangeTemp.Sub, NodeAttribute, "disprange", ManagerNameSpace, InformationProject, ID, NameFileSSEE);
							InformationEmitter.AlphaFadeStart = RangeTemp.Main * 0.01f;
							InformationEmitter.AlphaFadeEnd = (RangeTemp.Main + RangeTemp.Sub) * 0.01f;
						}
						break;

					case "TurnToDirection":
						{
							InformationEmitter.FlagData |= Library_SpriteStudio.Data.EmitterEffect.FlagBit.TURNDIRECTION;

							ValueTextAttribute = LibraryEditor_SpriteStudio.Utility.XML.TextGetNode(NodeAttribute, "Rotation", ManagerNameSpace);
							if(false == string.IsNullOrEmpty(ValueTextAttribute))
							{
								InformationEmitter.TurnDirectionFluctuation = LibraryEditor_SpriteStudio.Utility.Text.ValueGetFloat(ValueTextAttribute);
							}
						}
						break;

					case "InfiniteEmit":
						{
							InformationEmitter.FlagData |= Library_SpriteStudio.Data.EmitterEffect.FlagBit.EMIT_INFINITE;
						}
						break;

					default:
						Debug.LogWarning(	"SSEE-Import Warning (Emitter): "
											+ "Attribute Invalid.: Attribute[" + ValueText + "]"
											+ ": Parts-Name[" + InformationEffectParts.Name + "]"
											+ " File[" + NameFileSSEE + "]"
											+ " SSPJ[" + InformationProject.NameFileBody + InformationProject.NameFileExtension + "]"
										);
						break;
				}
			}

			return(InformationEmitter);

		ParseOPSS_InformationSSEEEmitter_ErrorEnd:;
			Debug.LogError(	"SSEE-Import Error (Parts-Emitter): "
							+ MessageError
							+ ": Parts-Name[" + InformationEffectParts.Name + "]"
							+ " File[" + NameFileSSEE + "]"
							+ " SSPJ[" + InformationProject.NameFileBody + InformationProject.NameFileExtension + "]"
						);
			return(null);
		}

		private static bool ImportSSEEPartsEmitterGetRangeFloat(	ref float OutputMain,
																	ref float OutputSub,
																	XmlNode NodeAttribute,
																	string Name,
																	XmlNamespaceManager ManagerNameSpace,
																	InformationSSPJ InformationProject,
																	int ID,
																	string NameFileSSEE
																)
		{
			bool FlagValid = true;
			string ValueText = "";
			XmlNode NodeNow = LibraryEditor_SpriteStudio.Utility.XML.NodeGet(NodeAttribute, Name, ManagerNameSpace);
			if(null != NodeNow)
			{
				ValueText = NodeNow.Attributes["value"].Value;
				if(false == string.IsNullOrEmpty(ValueText))
				{
					OutputMain = LibraryEditor_SpriteStudio.Utility.Text.ValueGetFloat(ValueText);
				}
				else
				{
					FlagValid = false;
				}

				ValueText = NodeNow.Attributes["subvalue"].Value;
				if(false == string.IsNullOrEmpty(ValueText))
				{
					OutputSub = LibraryEditor_SpriteStudio.Utility.Text.ValueGetFloat(ValueText);
				}
				else
				{
					FlagValid = false;
				}

				ImportSSEEPartsEmitterSortRangeFloat(ref OutputMain, ref OutputSub);
				OutputSub -= OutputMain;
			}
			if(false == FlagValid)
			{
				Debug.LogError(	"SSEE-Import Waring (Parts-Emitter): "
								+ "Not-Enough Tag's-Attribute: Tag[" + Name + "]"
								+ " File[" + NameFileSSEE + "]"
								+ " SSPJ[" + InformationProject.NameFileBody + InformationProject.NameFileExtension + "]"
						);
			}
			return(FlagValid);
		}
		private static void ImportSSEEPartsEmitterSortRangeFloat(	ref float OutputMain,
																	ref float OutputSub
																)
		{
			if(OutputMain > OutputSub)
			{
				float FloatTemp = OutputSub;
				OutputSub = OutputMain;
				OutputMain = FloatTemp;
			}
		}
		private static bool ImportSSEEPartsEmitterGetRangeColor(	ref Color OutputMain,
																	ref Color OutputSub,
																	XmlNode NodeAttribute,
																	string Name,
																	XmlNamespaceManager ManagerNameSpace,
																	InformationSSPJ InformationProject,
																	int ID,
																	string NameFileSSEE
																)
		{
			bool FlagValid = true;
			string ValueText = "";
			XmlNode NodeNow = LibraryEditor_SpriteStudio.Utility.XML.NodeGet(NodeAttribute, Name, ManagerNameSpace);
			if(null != NodeNow)
			{
				uint ColorTemp;
				ValueText = NodeNow.Attributes["value"].Value;
				if(false == string.IsNullOrEmpty(ValueText))
				{
					ColorTemp = LibraryEditor_SpriteStudio.Utility.Text.HexToUInt(ValueText);
					OutputMain.a = ((float)((ColorTemp >> 24) & 0xff)) / 255.0f;
					OutputMain.r = ((float)((ColorTemp >> 16) & 0xff)) / 255.0f;
					OutputMain.g = ((float)((ColorTemp >> 8) & 0xff)) / 255.0f;
					OutputMain.b = ((float)(ColorTemp & 0xff)) / 255.0f;
				}
				else
				{
					FlagValid = false;
				}

				ValueText = NodeNow.Attributes["subvalue"].Value;
				if(false == string.IsNullOrEmpty(ValueText))
				{
					ColorTemp = LibraryEditor_SpriteStudio.Utility.Text.HexToUInt(ValueText);
					OutputSub.a = ((float)((ColorTemp >> 24) & 0xff)) / 255.0f;
					OutputSub.r = ((float)((ColorTemp >> 16) & 0xff)) / 255.0f;
					OutputSub.g = ((float)((ColorTemp >> 8) & 0xff)) / 255.0f;
					OutputSub.b = ((float)(ColorTemp & 0xff)) / 255.0f;
				}
				else
				{
					FlagValid = false;
				}

				float FloatTemp;
				if(OutputMain.a > OutputSub.a)
				{
					FloatTemp = OutputSub.a;
					OutputSub.a = OutputMain.a;
					OutputMain.a = FloatTemp;
				}
				if(OutputMain.r > OutputSub.r)
				{
					FloatTemp = OutputSub.r;
					OutputSub.r = OutputMain.r;
					OutputMain.r = FloatTemp;
				}
				if(OutputMain.g > OutputSub.g)
				{
					FloatTemp = OutputSub.g;
					OutputSub.g = OutputMain.g;
					OutputMain.g = FloatTemp;
				}
				if(OutputMain.b > OutputSub.b)
				{
					FloatTemp = OutputSub.b;
					OutputSub.b = OutputMain.b;
					OutputMain.b = FloatTemp;
				}

				OutputSub.a -= OutputMain.a;
				OutputSub.r -= OutputMain.r;
				OutputSub.g -= OutputMain.g;
				OutputSub.b -= OutputMain.b;
			}
			if(false == FlagValid)
			{
				Debug.LogError(	"SSEE-Import Waring (Parts-Emitter): "
								+ "Not-Enough Tag's-Attribute: Tag[" + Name + "]"
								+ " File[" + NameFileSSEE + "]"
								+ " SSPJ[" + InformationProject.NameFileBody + InformationProject.NameFileExtension + "]"
						);
			}
			return(FlagValid);
		}
		internal enum KindVersionSSEE
		{
			ERROR = 0x00000000,
			VERSION_010000 = 0x00010000,	/* Disuse */
			VERSION_010001 = 0x00010001,	/* Disuse */
			VERSION_010002 = 0x00010002,	/* ssee ver.5.5.1 */
			VERSION_010100 = 0x00010100,	/* ssee ver.5.7 */

			VERSION_LATEST = VERSION_010100
		};
		internal class InformationSSEE
		{
			/* SSEE Data: Base Data */
			internal string Name;

			/* SSEE Data: Image Datas */
			internal KindVersionSSEE VersionCode;

			/* SSEE Data: Parts Datas */
			internal InformationSSEEParts[] ListParts;

			/* SSEE Data: Parameters */
			internal int VersionRenderer;
			internal Vector2 ScaleLayout;
			internal int Seed;
			internal bool FlagLockSeed;
			internal int FramePerSecond;

			/* WorkArea: (SSEE File's) File/Path Name */
			internal string NameDirectory;
			internal string NameFileBody;
			internal string NameFileExtension;

			/* Converted Data: Runtime-Datas */
			internal Library_SpriteStudio.Data.PartsEffect[] ListDataPartsRuntime;
			internal Library_SpriteStudio.Data.EmitterEffect[] ListDataEmitterRuntime;
			internal ScriptableObject PrefabDataEffect;
			internal string NamePrefabDataEffect;
			internal UnityEngine.Object PrefabGameObject;
			internal string NamePrefabGameObject;

			internal void CleanUp()
			{
				VersionCode = LibraryEditor_SpriteStudio.ParseOPSS.KindVersionSSEE.ERROR;

				ListParts = null;

				VersionRenderer = 0;
				ScaleLayout = Vector2.one;
				Seed = 0;
				FlagLockSeed = false;
				FramePerSecond = 60;

				NameDirectory = "";
				NameFileBody = "";
				NameFileExtension = "";

				ListDataPartsRuntime = null;
				ListDataEmitterRuntime = null;
				PrefabDataEffect = null;
				NamePrefabDataEffect = "";
				PrefabGameObject = null;
				NamePrefabGameObject = "";
			}
		}
		internal class InformationSSEEParts : Library_SpriteStudio.Data.PartsEffect
		{	/* SS5.7 */
			internal List<int> ListIndexPartsChild;
			internal InformationSSEEEmitter Emitter;

			internal new void CleanUp()
			{
				base.CleanUp();

				ListIndexPartsChild = null;
				Emitter = null;
			}
		}
		internal class InformationSSEEEmitter : Library_SpriteStudio.Data.EmitterEffect
		{	/* SS5.7 */
			internal string NameCellMap;
			internal string NameCell;

			internal new void CleanUp()
			{
				base.CleanUp();

				NameCellMap = "";
				NameCell = "";
			}

			internal void BootUp()
			{
				CleanUp();
			}
		}
	}

	internal static class Convert
	{
		internal static bool PrefabCreateTexture(	ref SettingImport DataSettingImport,
													LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
													int Index,
													string NameBaseAssetPath,
													string FileName
												)
		{
//			string MessageError = "";
			LibraryEditor_SpriteStudio.ParseOPSS.InformationTexture InformationTexture = InformationProject.ListInformationTexture[Index];
			string NamePathAssetNative = null;

			/* Determine Destination */
			if(null == InformationTexture.PrefabTexture)
			{
				/* Create Destination-Folder */
				LibraryEditor_SpriteStudio.Utility.File.AssetFolderCreate(NameBaseAssetPath, NamePathSubImportTexture);
			}
			else
			{
				/* Confirm Overwrite. */
				if(false == LibraryEditor_SpriteStudio.Utility.File.OverwriteConfirmDialogue(	InformationTexture.NamePrefabTexture,
																								"Textures",
																								DataSettingImport.FlagConfirmOverWrite,
																								ref DataSettingImport.FlagConfirmOverWriteTexture
																							)
					)
				{
					return(true);
				}
			}

			/* Copy into Asset */
			NamePathAssetNative = LibraryEditor_SpriteStudio.Utility.File.PathGetAssetPath(InformationTexture.NamePrefabTexture);
			LibraryEditor_SpriteStudio.Utility.File.FileCopyToAsset(	NamePathAssetNative,
																		(string)InformationProject.ListNameTexture[Index],
																		true
																	);

			/* Importer Setting */
			AssetDatabase.ImportAsset(InformationTexture.NamePrefabTexture);
			TextureImporter Importer = TextureImporter.GetAtPath(InformationTexture.NamePrefabTexture) as TextureImporter;
			if(null != Importer)
			{
				Importer.anisoLevel = 1;
				Importer.borderMipmap = false;
				Importer.convertToNormalmap = false;
				Importer.fadeout = false;
				switch(InformationTexture.FilterTexture)
				{
					case Library_SpriteStudio.KindFilterTexture.NEAREST:
						Importer.filterMode = FilterMode.Point;
						break;

					case Library_SpriteStudio.KindFilterTexture.LINEAR:
					default:
						Importer.filterMode = FilterMode.Bilinear;
						break;
				}

				Importer.generateCubemap = TextureImporterGenerateCubemap.None;
				Importer.generateMipsInLinearSpace = false;
				Importer.grayscaleToAlpha = false;
				Importer.isReadable = false;
				Importer.lightmap = false;
				Importer.linearTexture = false;
				Importer.mipmapEnabled = false;
				Importer.maxTextureSize = DataSettingImport.TextureSizePixelMaximum;
				Importer.normalmap = false;
				Importer.npotScale = TextureImporterNPOTScale.None;
				Importer.textureFormat = TextureImporterFormat.AutomaticTruecolor;
				Importer.textureType = TextureImporterType.Advanced;
				switch(InformationTexture.WrapTexture)
				{
					case Library_SpriteStudio.KindWrapTexture.REPEAT:
						Importer.wrapMode = TextureWrapMode.Repeat;
						break;

					case Library_SpriteStudio.KindWrapTexture.MIRROR:
					case Library_SpriteStudio.KindWrapTexture.CLAMP:
					default:
						Importer.wrapMode = TextureWrapMode.Clamp;
						break;
				}
				AssetDatabase.ImportAsset(InformationTexture.NamePrefabTexture, ImportAssetOptions.ForceUpdate);
			}

			InformationTexture.PrefabTexture = AssetDatabase.LoadAssetAtPath(InformationTexture.NamePrefabTexture, typeof(Texture2D)) as Texture2D;
			InformationTexture.SizeX = InformationTexture.PrefabTexture.width;
			InformationTexture.SizeY = InformationTexture.PrefabTexture.height;

			/* Texture's Pixel-Size Check */
			if((true != PixelSizeCheck(InformationTexture.SizeX)) || (true != PixelSizeCheck(InformationTexture.SizeY)))
			{
				Debug.LogWarning("Convert-Warning (Texture):"
									+ " Texture Pixel-Size is not multiples of powerd by 2:"
									+ " File[" + InformationProject.ListNameTexture[Index] + "]"
									+ " SSPJ[" + FileName + "]"
								);
			}

			/* Fixing Created Assets */
			EditorUtility.SetDirty(InformationTexture.PrefabTexture);
			AssetDatabase.SaveAssets();

			/* Create Material */
			for(int i=0; i<ShaderOperationMax; i++)
			{
				InformationTexture.ListPrefabMaterial[i] = PrefabCreateTextureMaterial(	ref DataSettingImport,
																						InformationProject,
																						InformationTexture,
																						i,
																						NameBaseAssetPath,
																						FileName
																					);
				if(null == InformationTexture.ListPrefabMaterial[i])
				{
					goto Convert_PrefabCreateTexture_ErrorEnd_NoMessage;
				}
			}
			for(int i=0; i<ShaderOperationMaxEffect; i++)
			{
				InformationTexture.ListPrefabMaterialEffect[i] = PrefabCreateTextureMaterialEffect(	ref DataSettingImport,
																									InformationProject,
																									InformationTexture,
																									i,
																									NameBaseAssetPath,
																									FileName
																								);
				if(null == InformationTexture.ListPrefabMaterialEffect[i])
				{
					goto Convert_PrefabCreateTexture_ErrorEnd_NoMessage;
				}
			}

			return(true);

//		Convert_PrefabCreateTexture_ErrorEnd:;
//			Debug.LogError("Convert-Error (Texture): " + MessageError + "(in " + FileName + ")");
		Convert_PrefabCreateTexture_ErrorEnd_NoMessage:;
			return(false);
		}
		private readonly static int[] SizePixelNormalize = {64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, -1};
		private static bool PixelSizeCheck(int Size)
		{
			int SizeNow;
			for(int i=0; ; i++)
			{
				SizeNow = SizePixelNormalize[i];
				if(-1 == SizeNow)
				{
					return(false);
				}
				if(SizeNow < Size)
				{
					continue;
				}
				if(SizeNow == Size)
				{
					return(true);
				}
				return(false);
			}
		}

		internal static Material PrefabCreateTextureMaterial(	ref SettingImport DataSettingImport,
																LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
																LibraryEditor_SpriteStudio.ParseOPSS.InformationTexture InformationTexture,
																int IndexColorOperation,
																string NameBaseAssetPath,
																string FileName
															)
		{
//			string MessageError = "";
			Material DataMaterial = InformationTexture.ListPrefabMaterial[IndexColorOperation];

			/* Determine Destination */
			if(null == DataMaterial)
			{
				/* Create Destination-Folder */
				LibraryEditor_SpriteStudio.Utility.File.AssetFolderCreate(NameBaseAssetPath, NamePathSubImportMaterial);
				LibraryEditor_SpriteStudio.Utility.File.AssetFolderCreate(NameBaseAssetPath, NamePathSubImportMaterialEffect);

				/* Create new Material */
				DataMaterial = new Material(Library_SpriteStudio.Shader_SpriteStudioTriangleX[IndexColorOperation]);
				AssetDatabase.CreateAsset(DataMaterial, InformationTexture.ListNamePrefabMaterial[IndexColorOperation]);
			}
			else
			{
				/* Confirm Overwrite */
				if(false == LibraryEditor_SpriteStudio.Utility.File.OverwriteConfirmDialogue(	InformationTexture.ListNamePrefabMaterial[IndexColorOperation],
																								"Materials",
																								DataSettingImport.FlagConfirmOverWrite,
																								ref DataSettingImport.FlagConfirmOverWriteMaterial
																							)
					)
				{
					return(DataMaterial);
				}
			}

			/* Data Set */
			DataMaterial.mainTexture = InformationTexture.PrefabTexture;
			EditorUtility.SetDirty(DataMaterial);
			AssetDatabase.SaveAssets();

			return(DataMaterial);

//		Convert_PrefabCreateTextureMaterial_ErrorEnd:;
//			Debug.LogError("Convert-Error (Material): " + MessageError + "(in " + FileName + ")");
//			return(null);
		}
		internal static Material PrefabCreateTextureMaterialEffect(	ref SettingImport DataSettingImport,
																	LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
																	LibraryEditor_SpriteStudio.ParseOPSS.InformationTexture InformationTexture,
																	int IndexColorOperation,
																	string NameBaseAssetPath,
																	string FileName
																)
		{
//			string MessageError = "";
			Material DataMaterial = InformationTexture.ListPrefabMaterialEffect[IndexColorOperation];

			/* Determine Destination */
			if(null == DataMaterial)
			{
				/* Create Destination-Folder */
				LibraryEditor_SpriteStudio.Utility.File.AssetFolderCreate(NameBaseAssetPath, NamePathSubImportMaterial);

				/* Create new Material */
				DataMaterial = new Material(Library_SpriteStudio.Shader_SpriteStudioEffect[IndexColorOperation]);
				AssetDatabase.CreateAsset(DataMaterial, InformationTexture.ListNamePrefabMaterialEffect[IndexColorOperation]);
			}
			else
			{
				/* Confirm Overwrite */
				if(false == LibraryEditor_SpriteStudio.Utility.File.OverwriteConfirmDialogue(	InformationTexture.ListNamePrefabMaterialEffect[IndexColorOperation],
																								"Materials",
																								DataSettingImport.FlagConfirmOverWrite,
																								ref DataSettingImport.FlagConfirmOverWriteMaterial
																							)
					)
				{
					return(DataMaterial);
				}
			}

			/* Data Set */
			DataMaterial.mainTexture = InformationTexture.PrefabTexture;
			EditorUtility.SetDirty(DataMaterial);
			AssetDatabase.SaveAssets();

			return(DataMaterial);

//		Convert_PrefabCreateTextureMaterial_ErrorEnd:;
//			Debug.LogError("Convert-Error (Material): " + MessageError + "(in " + FileName + ")");
//			return(null);
		}

		internal static bool PrefabCreateSSCE(	ref SettingImport DataSettingImport,
												LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
												string NameBaseAssetPath,
												string FileName
											)
		{
//			string MessageError = "";
			Script_SpriteStudio_DataCell DataPrefabCell = null;

			/* Determine Destination */
			if(null == InformationProject.PrefabCell)
			{
				/* Create Destination-Folder */
				LibraryEditor_SpriteStudio.Utility.File.AssetFolderCreate(NameBaseAssetPath, NamePathSubImportCellMap);

				/* Create New Asset */
				DataPrefabCell = ScriptableObject.CreateInstance<Script_SpriteStudio_DataCell>();
				AssetDatabase.CreateAsset(DataPrefabCell, InformationProject.NamePrefabCell);
				InformationProject.PrefabCell = AssetDatabase.LoadAssetAtPath(InformationProject.NamePrefabCell, typeof(ScriptableObject)) as ScriptableObject;
			}
			else
			{
				/* Confirm Overwrite */
				if(false == LibraryEditor_SpriteStudio.Utility.File.OverwriteConfirmDialogue(	InformationProject.NamePrefabCell,
																								"Fixed-CellMap Datas",
																								DataSettingImport.FlagConfirmOverWrite,
																								ref DataSettingImport.FlagConfirmOverWriteDataCellMap
																							)
					)
				{
					return(true);
				}
			}
			DataPrefabCell = InformationProject.PrefabCell as Script_SpriteStudio_DataCell;

			int Count = InformationProject.ListNameSSCE.Count;
			DataPrefabCell.ListDataCellMap = new Library_SpriteStudio.Data.CellMap[Count];

			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSCE InformationSSCE = null;
			Library_SpriteStudio.Data.CellMap DataCellMap = null;
			LibraryEditor_SpriteStudio.ParseOPSS.InformationTexture InformationTexture = null;
			for(int i=0; i<Count; i++)
			{
				DataCellMap = new Library_SpriteStudio.Data.CellMap();
				DataCellMap.CleanUp();

				InformationSSCE = InformationProject.ListInformationSSCE[i];
				InformationSSCE.DataCellMap = DataCellMap;
				InformationTexture = InformationProject.ListInformationTexture[InformationSSCE.IndexTexture];

				DataCellMap.Name = InformationSSCE.Name;
				DataCellMap.SizeOriginal.x = (float)InformationTexture.SizeX;
				DataCellMap.SizeOriginal.y = (float)InformationTexture.SizeY;

				int CountListCell = InformationSSCE.ListCell.Length;
				DataCellMap.ListCell = new Library_SpriteStudio.Data.Cell[CountListCell];
				for(int j=0; j<CountListCell; j++)
				{
					DataCellMap.ListCell[j] = new Library_SpriteStudio.Data.Cell();
					DataCellMap.ListCell[j].Name = InformationSSCE.ListCell[j].Name;
					DataCellMap.ListCell[j].Rectangle = InformationSSCE.ListCell[j].Area;
					DataCellMap.ListCell[j].Pivot = InformationSSCE.ListCell[j].Pivot;
				}
				DataPrefabCell.ListDataCellMap[i] = InformationSSCE.DataCellMap;
			}
			EditorUtility.SetDirty(DataPrefabCell);
			AssetDatabase.SaveAssets();

			return(true);
		}

		internal static bool PrefabCreateSSAE(	ref SettingImport DataSettingImport,
												LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
												int Index,
												string NameBaseAssetPath,
												string FileName
											)
		{
//			string MessageError = "";
			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAE InformationAnimationSet = InformationProject.ListInformationSSAE[Index];
			Script_SpriteStudio_DataAnimation DataAnimationSet = null;
			int Count = -1;

			/* Determine Destination */
			bool FlagOverwrite = false;
			if(null == InformationAnimationSet.PrefabDataAnimation)
			{
				/* Create Destination-Folder */
				LibraryEditor_SpriteStudio.Utility.File.AssetFolderCreate(NameBaseAssetPath, NamePathSubImportAnimation);

				/* Create New Asset */
				DataAnimationSet = ScriptableObject.CreateInstance<Script_SpriteStudio_DataAnimation>();
				AssetDatabase.CreateAsset(DataAnimationSet, InformationAnimationSet.NamePrefabDataAnimation);
				InformationAnimationSet.PrefabDataAnimation = AssetDatabase.LoadAssetAtPath(InformationAnimationSet.NamePrefabDataAnimation, typeof(ScriptableObject)) as ScriptableObject;

				FlagOverwrite = true;
			}
			else
			{
				/* Confirm Overwrite */
				FlagOverwrite = LibraryEditor_SpriteStudio.Utility.File.OverwriteConfirmDialogue(	InformationAnimationSet.NamePrefabDataAnimation,
																									"Fixed-Animation Datas",
																									DataSettingImport.FlagConfirmOverWrite,
																									ref DataSettingImport.FlagConfirmOverWriteDataAnimation
																								);
			}
			DataAnimationSet = InformationAnimationSet.PrefabDataAnimation as Script_SpriteStudio_DataAnimation;
			if(false == FlagOverwrite)
			{
				InformationAnimationSet.ListDataPartsRuntime = DataAnimationSet.ListDataParts;
				InformationAnimationSet.ListDataAnimationRuntime = DataAnimationSet.ListDataAnimation;
				goto Convert_PrefabCreateSSAE_ConvertGameObject;
			}

			/* Create Parts-Data */
			InformationAnimationSet.ListDataPartsRuntime = new Library_SpriteStudio.Data.Parts[InformationAnimationSet.ListParts.Length];
			if(false == PrefabCreateSSAEParts(	ref DataSettingImport,
												InformationProject,
												InformationAnimationSet,
												NameBaseAssetPath,
												FileName
											)
				)
			{
				goto Convert_PrefabCreateSSAE_ErrorEnd_NoMessage;
			}

			/* Create Animation-Data (ConvertPass-1: Expand all Frame-Datas) */
			Count = InformationAnimationSet.ListAnimation.Length;
			InformationAnimationSet.ListDataAnimationRuntime = new Library_SpriteStudio.Data.Animation[Count];
			for(int i=0; i<Count; i++)
			{
				InformationAnimationSet.ListDataAnimationRuntime[i] = PrefabCreateSSAEAnimation(	ref DataSettingImport,
																									InformationProject,
																									InformationAnimationSet,
																									i,
																									NameBaseAssetPath,
																									FileName
																								);
				if(null == InformationAnimationSet.ListDataAnimationRuntime[i])
				{
					goto Convert_PrefabCreateSSAE_ErrorEnd_NoMessage;
				}
			}

			/* ConvertPass-1: Solving "Inheritance" & Parts-Kind(NORMAL -> NORMAL_TRIANGLE2/4) */
			for(int i=0; i<InformationAnimationSet.ListDataAnimationRuntime.Length; i++)
			{
				for(int j=0; j<InformationAnimationSet.ListDataPartsRuntime.Length; j++)
				{
					PrefabCreateSSAEDataSolveParts(	ref DataSettingImport,
													InformationProject,
													InformationAnimationSet,
													i,
													j,
													NameBaseAssetPath,
													FileName
												);
				}
			}

			/* ConvertPass-1: Judge no-use "Instance" and "Effect" */
			for(int i=0; i<InformationAnimationSet.ListDataAnimationRuntime.Length; i++)
			{
				for(int j=0; j<InformationAnimationSet.ListDataPartsRuntime.Length; j++)
				{
					PrefabCreateSSAEDataJudgeNoUseParts(	ref DataSettingImport,
															InformationProject,
															InformationAnimationSet,
															i,
															j,
															NameBaseAssetPath,
															FileName
														);
				}
			}

			/* ConvertPass-2: "Calculation in Advance" */
			if(true == DataSettingImport.FlagDataCalculateInAdvance)
			{
				for(int i=0; i<InformationAnimationSet.ListDataAnimationRuntime.Length; i++)
				{
					for(int j=0; j<InformationAnimationSet.ListDataPartsRuntime.Length; j++)
					{
						PrefabCreateSSAEDataCalculateInAdvance(	ref DataSettingImport,
																InformationProject,
																InformationAnimationSet,
																i,
																j,
																NameBaseAssetPath,
																FileName
															);
					}
				}
			}

			/* ConvertPass-1/2: Compress only the required attributes */
			for(int i=0; i<InformationAnimationSet.ListDataAnimationRuntime.Length; i++)
			{
				for(int j=0; j<InformationAnimationSet.ListDataPartsRuntime.Length; j++)
				{
					PrefabCreateSSAEDataCompressRequired(	ref DataSettingImport,
															InformationProject,
															InformationAnimationSet,
															i,
															j,
															NameBaseAssetPath,
															FileName
														);
				}
			}

			/* ConvertPass-3: "Compress" */
			if(true == DataSettingImport.FlagDataCompress)
			{
				Count = InformationAnimationSet.ListDataAnimationRuntime.Length;
				for(int i=0; i<Count; i++)
				{
					InformationAnimationSet.ListDataAnimationRuntime[i].Compress();
				}
			}

			/* Animation Data Fix */
			DataAnimationSet.ListDataParts = InformationAnimationSet.ListDataPartsRuntime;
			DataAnimationSet.ListDataAnimation = InformationAnimationSet.ListDataAnimationRuntime;
			EditorUtility.SetDirty(DataAnimationSet);
			AssetDatabase.SaveAssets();

			/* WorkArea(for ".ssae" importing) Purge */
			Count = InformationAnimationSet.ListParts.Length;
			for(int i=0; i<Count; i++)
			{
				InformationAnimationSet.ListParts[i].CleanUp();
				InformationAnimationSet.ListParts[i] = null;
			}
			InformationAnimationSet.ListParts = null;

		Convert_PrefabCreateSSAE_ConvertGameObject:;
			/* Construction GameObject-s */
			if(false == PrefabCreateSSAEGameObject(	ref DataSettingImport,
													InformationProject,
													InformationAnimationSet,
													DataAnimationSet,
													NameBaseAssetPath,
													FileName
												)
				)
			{
				goto Convert_PrefabCreateSSAE_ErrorEnd_NoMessage;
			}

			/* Data-Store */
			AssetDatabase.SaveAssets();

			return(true);

//		Convert_PrefabCreateSSAE_ErrorEnd:;
		Convert_PrefabCreateSSAE_ErrorEnd_NoMessage:;
			return(false);
		}
		internal static bool PrefabCreateSSAEGameObject(	ref SettingImport DataSettingImport,
															LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
															LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAE InformationAnimationSet,
															Script_SpriteStudio_DataAnimation DataAnimationSet,
															string NameBaseAssetPath,
															string FileName
														)
		{
//			string MessageError = "";

			/* Determine Destination */
			bool FlagNewCreate = false;
			if(null == InformationAnimationSet.PrefabGameObject)
			{
				/* Create Destination-Folder */
				LibraryEditor_SpriteStudio.Utility.File.AssetFolderCreate(NameBaseAssetPath, NamePathSubImportPrefab);

				/* Create Prefab */
				InformationAnimationSet.PrefabGameObject = PrefabUtility.CreateEmptyPrefab(InformationAnimationSet.NamePrefabGameObject);

				FlagNewCreate = true;
			}
			else
			{
				/* Confirm Overwrite */
				if(false == LibraryEditor_SpriteStudio.Utility.File.OverwriteConfirmDialogue(	InformationAnimationSet.NamePrefabGameObject,
																								"Root(SSAE)-Prefabs",
																								DataSettingImport.FlagConfirmOverWrite,
																								ref DataSettingImport.FlagConfirmOverWriteRoot
																							)
					)
				{
					return(true);
				}

				FlagNewCreate = false;
			}

			/* Create GameObject (Root-Parts) */
			GameObject InstanceGameObjectRoot = Library_SpriteStudio.Miscellaneousness.Asset.GameObjectCreate(InformationAnimationSet.Name, false, null);
			if(null == InstanceGameObjectRoot)
			{
				goto Convert_PrefabCreateSSAEGameObject_ErrorEnd;
			}
			Script_SpriteStudio_Root InstanceRoot = InstanceGameObjectRoot.AddComponent<Script_SpriteStudio_Root>();

			/* Datas Set */
			InstanceRoot.DataCellMap = (Script_SpriteStudio_DataCell)InformationProject.PrefabCell;
			InstanceRoot.DataAnimation = (Script_SpriteStudio_DataAnimation)InformationAnimationSet.PrefabDataAnimation;
			InstanceRoot.TableMaterial = InformationProject.TableMaterial;

			/* Construct Child-GameObjects & Controls */
			GameObject[] ListGameObject = InstanceRoot.GameObjectBuildUp(DataSettingImport.FlagAttachRigidBody, DataSettingImport.CollisionThicknessZ);
			InstanceRoot.ControlPartsBuildUp(ListGameObject);

			/* Initial-Setting Parts-Root */
			if((true == FlagNewCreate) || (false == DataSettingImport.FlagDataTakeOverSettingPrefab))
			{	/* Parameters Set */
				InstanceRoot.IndexAnimation = 0;
				InstanceRoot.RateSpeed = 1.0f;
				InstanceRoot.RateSpeedInitial = 1.0f;
				InstanceRoot.TimesPlay = 0;
				InstanceRoot.FlagPingpong = false;
				InstanceRoot.IndexLabelStart = (int)(Library_SpriteStudio.KindLabelAnimationReserved.START | Library_SpriteStudio.KindLabelAnimationReserved.INDEX_RESERVED);
				InstanceRoot.FrameOffsetStart = 0;
				InstanceRoot.IndexLabelEnd = (int)(Library_SpriteStudio.KindLabelAnimationReserved.END | Library_SpriteStudio.KindLabelAnimationReserved.INDEX_RESERVED);
				InstanceRoot.FrameOffsetEnd = 0;
				InstanceRoot.FrameOffsetInitial = 0;
				InstanceRoot.FlagAnimationStopInitial = false;
				InstanceRoot.InstanceManagerDraw = null;
			}
			else
			{	/* Parameters Inherit */
				GameObject InstanceGameObjectRootOld = (GameObject)InformationAnimationSet.PrefabGameObject;
				Script_SpriteStudio_Root InstanceRootOld = InstanceGameObjectRootOld.GetComponent<Script_SpriteStudio_Root>();

				InstanceRoot.IndexAnimation = InstanceRootOld.IndexAnimation;
				InstanceRoot.RateSpeed = InstanceRootOld.RateSpeed;
				InstanceRoot.RateSpeedInitial = InstanceRootOld.RateSpeedInitial;
				InstanceRoot.TimesPlay = InstanceRootOld.TimesPlay;
				InstanceRoot.FlagPingpong = InstanceRootOld.FlagPingpong;
				InstanceRoot.IndexLabelStart = InstanceRootOld.IndexLabelStart;
				InstanceRoot.FrameOffsetStart = InstanceRootOld.FrameOffsetStart;
				InstanceRoot.IndexLabelEnd = InstanceRootOld.IndexLabelEnd;
				InstanceRoot.FrameOffsetEnd = InstanceRootOld.FrameOffsetEnd;
				InstanceRoot.FrameOffsetInitial = InstanceRootOld.FrameOffsetInitial;
				InstanceRoot.FlagAnimationStopInitial = InstanceRootOld.FlagAnimationStopInitial;
				InstanceRoot.InstanceManagerDraw = InstanceRootOld.InstanceManagerDraw;
			}
			Library_SpriteStudio.Miscellaneousness.Asset.ActiveSetGameObject(InstanceGameObjectRoot, true);

			/* Fixing Created Assets */
			InformationAnimationSet.PrefabGameObject = PrefabUtility.ReplacePrefab(	InstanceGameObjectRoot,
																					InformationAnimationSet.PrefabGameObject,
																					OptionPrefabOverwrite
																				);
			EditorUtility.SetDirty(InformationAnimationSet.PrefabGameObject);
			AssetDatabase.SaveAssets();

			/* Store Prefab */
			UnityEngine.Object.DestroyImmediate(InstanceGameObjectRoot);
			InstanceGameObjectRoot = null;

			return(true);

		Convert_PrefabCreateSSAEGameObject_ErrorEnd:;
			return(false);
		}
		private static bool PrefabCreateSSAEParts(	ref SettingImport DataSettingImport,
													LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
													LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAE InformationAnimationSet,
													string NameBaseAssetPath,
													string FileName
												)
		{
//			string MessageError = "";
			int Count = InformationAnimationSet.ListParts.Length;

			/* Create Datas */
			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts InformationParts = null;
			Library_SpriteStudio.Data.Parts InformationPartsRuntime = null;
			int IndexUnderControl;
			for(int i=0; i<Count; i++)
			{
				InformationParts = InformationAnimationSet.ListParts[i];
				InformationPartsRuntime = new Library_SpriteStudio.Data.Parts();

				InformationPartsRuntime.Name = InformationParts.Name;
				InformationPartsRuntime.ID = InformationParts.ID;
				InformationPartsRuntime.IDParent = InformationParts.IDParent;
				InformationPartsRuntime.ListIDChild = InformationParts.ListIndexPartsChild.ToArray();

				InformationPartsRuntime.Kind = InformationParts.Kind;	/* Not firm */
				InformationPartsRuntime.KindBlendTarget = InformationParts.KindBlendTarget;
				InformationPartsRuntime.KindLabelColor = InformationParts.KindLabelColor;
				InformationPartsRuntime.KindShapeCollision = InformationParts.KindShapeCollision;
				InformationPartsRuntime.SizeCollisionZ = InformationParts.SizeCollisionZ;

				InformationPartsRuntime.PrefabUnderControl = null;
				InformationPartsRuntime.NameAnimationUnderControl = "";

				IndexUnderControl = InformationParts.IndexSSAEInstance;	/* Instance */
				if(0 <= IndexUnderControl)
				{
					InformationPartsRuntime.PrefabUnderControl = InformationProject.ListInformationSSAE[IndexUnderControl].PrefabGameObject;
					InformationPartsRuntime.NameAnimationUnderControl = (null != InformationParts.NameAnimationUnderControl) ? InformationParts.NameAnimationUnderControl : "";
				}

				IndexUnderControl = InformationParts.IndexSSEEEffect;	/* Effect */
				if(0 <= IndexUnderControl)
				{
					InformationPartsRuntime.PrefabUnderControl = InformationProject.ListInformationSSEE[IndexUnderControl].PrefabGameObject;
//					InformationPartsRuntime.NameAnimationUnderControl = "";
				}

				InformationAnimationSet.ListDataPartsRuntime[i] = InformationPartsRuntime;
			}

			return(true);

//		Convert_PrefabCreateSSAEParts_ErrorEnd:;
//			return(false);
		}
		private static Library_SpriteStudio.Data.Animation PrefabCreateSSAEAnimation(	ref SettingImport DataSettingImport,
																						LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
																						LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAE InformationAnimationSet,
																						int Index,
																						string NameBaseAssetPath,
																						string FileName
																					)
		{
//			string MessageError = "";
			int Count;
			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEAnimation InformationAnimation = InformationAnimationSet.ListAnimation[Index];
			Library_SpriteStudio.Data.Animation InformationAnimationRuntime = new Library_SpriteStudio.Data.Animation();

			/* Set Animation-Data */
			InformationAnimationRuntime.Name = InformationAnimation.Name;
			InformationAnimationRuntime.FramePerSecond = InformationAnimation.FramePerSecond;
			InformationAnimationRuntime.CountFrame = InformationAnimation.CountFrame;
			InformationAnimationRuntime.ListLabel = InformationAnimation.ListLabel;

			/* Create Parts'-Animation-Data */
			Library_SpriteStudio.Data.AnimationParts InformationAnimationPartsRuntime = null;
			Count = InformationAnimationSet.ListParts.Length;
			InformationAnimationRuntime.ListAnimationParts = new Library_SpriteStudio.Data.AnimationParts[Count];
			for(int i=0; i<Count; i++)
			{
				InformationAnimationPartsRuntime = new Library_SpriteStudio.Data.AnimationParts();
				InformationAnimationRuntime.ListAnimationParts[i] = InformationAnimationPartsRuntime;

				InformationAnimationPartsRuntime.CleanUp();
				InformationAnimationPartsRuntime.DataPlain = new Library_SpriteStudio.Data.AnimationPartsPartsPlain();
				InformationAnimationPartsRuntime.DataPlain.CleanUp();

				if(false == PrefabCreateSSAEAnimationParts(	ref DataSettingImport,
															InformationProject,
															InformationAnimationSet,
															InformationAnimation,
															InformationAnimationRuntime,
															i,
															NameBaseAssetPath,
															FileName
														)
					)
				{
					goto Convert_PrefabCreateSSAEAnimation_ErrorEnd_NoMessage;
				}
			}

			return(InformationAnimationRuntime);

//		Convert_PrefabCreateSSAEAnimation_ErrorEnd:;
		Convert_PrefabCreateSSAEAnimation_ErrorEnd_NoMessage:;
			return(null);
		}
		private static bool PrefabCreateSSAEAnimationParts(	ref SettingImport DataSettingImport,
															LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
															LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAE InformationAnimationSet,
															LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEAnimation InformationAnimation,
															Library_SpriteStudio.Data.Animation InformationAnimationRuntime,
															int Index,
															string NameBaseAssetPath,
															string FileName
														)
		{
//			string MessageError = "";
			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEAnimationParts InformationParts = InformationAnimation.ListAnimationPartsKeyFrames[Index];
			Library_SpriteStudio.Data.AnimationParts InformationPartsRuntime = InformationAnimationRuntime.ListAnimationParts[Index];

			if(null == InformationParts)
			{
				InformationPartsRuntime.Status = new Library_SpriteStudio.Data.ListAttributeStatus();
					InformationPartsRuntime.Status.CleanUp();
					InformationPartsRuntime.Status.BootUp(0);
				InformationPartsRuntime.Position = new Library_SpriteStudio.Data.ListAttributeVector3();
					InformationPartsRuntime.Position.CleanUp();
					InformationPartsRuntime.Position.BootUp(0);
				InformationPartsRuntime.Rotation = new Library_SpriteStudio.Data.ListAttributeVector3();
					InformationPartsRuntime.Rotation.CleanUp();
					InformationPartsRuntime.Rotation.BootUp(0);
				InformationPartsRuntime.Scaling = new Library_SpriteStudio.Data.ListAttributeVector2();
					InformationPartsRuntime.Scaling.CleanUp();
					InformationPartsRuntime.Scaling.BootUp(0);
				InformationPartsRuntime.RateOpacity = new Library_SpriteStudio.Data.ListAttributeFloat();
					InformationPartsRuntime.RateOpacity.CleanUp();
					InformationPartsRuntime.RateOpacity.BootUp(0);
				InformationPartsRuntime.Priority = new Library_SpriteStudio.Data.ListAttributeFloat();
					InformationPartsRuntime.Priority.CleanUp();
					InformationPartsRuntime.Priority.BootUp(0);
				InformationPartsRuntime.PositionAnchor = new Library_SpriteStudio.Data.ListAttributeVector2();
					InformationPartsRuntime.PositionAnchor.CleanUp();
					InformationPartsRuntime.PositionAnchor.BootUp(0);
				InformationPartsRuntime.SizeForce = new Library_SpriteStudio.Data.ListAttributeVector2();
					InformationPartsRuntime.SizeForce.CleanUp();
					InformationPartsRuntime.SizeForce.BootUp(0);
				InformationPartsRuntime.UserData = new Library_SpriteStudio.Data.ListAttributeUserData();
					InformationPartsRuntime.UserData.CleanUp();
					InformationPartsRuntime.UserData.BootUp(0);
				InformationPartsRuntime.Instance = new Library_SpriteStudio.Data.ListAttributeInstance();
					InformationPartsRuntime.Instance.CleanUp();
					InformationPartsRuntime.Instance.BootUp(0);
				InformationPartsRuntime.Effect = new Library_SpriteStudio.Data.ListAttributeEffect();
					InformationPartsRuntime.Instance.CleanUp();
					InformationPartsRuntime.Instance.BootUp(0);

				InformationPartsRuntime.DataPlain.Cell = new Library_SpriteStudio.Data.ListAttributeCell();
					InformationPartsRuntime.DataPlain.Cell.CleanUp();
					InformationPartsRuntime.DataPlain.Cell.BootUp(0);
				InformationPartsRuntime.DataPlain.ColorBlend = new Library_SpriteStudio.Data.ListAttributeColorBlend();
					InformationPartsRuntime.DataPlain.ColorBlend.CleanUp();
					InformationPartsRuntime.DataPlain.ColorBlend.BootUp(0);
				InformationPartsRuntime.DataPlain.VertexCorrection = new Library_SpriteStudio.Data.ListAttributeVertexCorrection();
					InformationPartsRuntime.DataPlain.VertexCorrection.CleanUp();
					InformationPartsRuntime.DataPlain.VertexCorrection.BootUp(0);
				InformationPartsRuntime.DataPlain.PositionTexture = new Library_SpriteStudio.Data.ListAttributeVector2();
					InformationPartsRuntime.DataPlain.PositionTexture.CleanUp();
					InformationPartsRuntime.DataPlain.PositionTexture.BootUp(0);
				InformationPartsRuntime.DataPlain.ScalingTexture = new Library_SpriteStudio.Data.ListAttributeVector2();
					InformationPartsRuntime.DataPlain.ScalingTexture.CleanUp();
					InformationPartsRuntime.DataPlain.ScalingTexture.BootUp(0);
				InformationPartsRuntime.DataPlain.RotationTexture = new Library_SpriteStudio.Data.ListAttributeFloat();
					InformationPartsRuntime.DataPlain.RotationTexture.CleanUp();
					InformationPartsRuntime.DataPlain.RotationTexture.BootUp(0);
				InformationPartsRuntime.DataPlain.RadiusCollision = new Library_SpriteStudio.Data.ListAttributeFloat();
					InformationPartsRuntime.DataPlain.RadiusCollision.CleanUp();
					InformationPartsRuntime.DataPlain.RadiusCollision.BootUp(0);

				return(true);
			}

			/* Expand Key-Frame */
			int CountFrameFull = InformationAnimation.CountFrame;
			InformationPartsRuntime.Status = ListStatusGetSSAEAttribute(	ref DataSettingImport,
																			InformationProject,
																			CountFrameFull,
																			InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.HIDE],
																			InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.FLIP_X],
																			InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.FLIP_Y],
																			InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.TEXTURE_FLIP_X],
																			InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.TEXTURE_FLIP_Y],
																			Library_SpriteStudio.Data.AttributeStatus.FlagBit.HIDE,
																			NameBaseAssetPath,
																			FileName
																		);
			InformationPartsRuntime.Position = ListVector3GetSSAEAttribute(	ref DataSettingImport,
																			InformationProject,
																			CountFrameFull,
																			InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.POSITION_X],
																			InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.POSITION_Y],
																			InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.POSITION_Z],
																			Vector3.zero,
																			NameBaseAssetPath,
																			FileName
																		);
			InformationPartsRuntime.Rotation = ListVector3GetSSAEAttribute(	ref DataSettingImport,
																			InformationProject,
																			CountFrameFull,
																			InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.ROTATION_X],
																			InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.ROTATION_Y],
																			InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.ROTATION_Z],
																			Vector3.zero,
																			NameBaseAssetPath,
																			FileName
																		);
			InformationPartsRuntime.Scaling = ListVector2GetSSAEAttribute(	ref DataSettingImport,
																			InformationProject,
																			CountFrameFull,
																			InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.SCALING_X],
																			InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.SCALING_Y],
																			Vector2.one,
																			NameBaseAssetPath,
																			FileName
																		);
			InformationPartsRuntime.RateOpacity = ListFloatGetSSAEAttribute(	ref DataSettingImport,
																				InformationProject,
																				CountFrameFull,
																				InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.RATE_OPACITY],
																				1.0f,
																				NameBaseAssetPath,
																				FileName
																			);
			InformationPartsRuntime.Priority = ListFloatGetSSAEAttribute(	ref DataSettingImport,
																			InformationProject,
																			CountFrameFull,
																			InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.PRIORITY],
																			0.0f,
																			NameBaseAssetPath,
																			FileName
																		);
			InformationPartsRuntime.PositionAnchor = ListVector2GetSSAEAttribute(	ref DataSettingImport,
																					InformationProject,
																					CountFrameFull,
																					InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.ANCHOR_POSITION_X],
																					InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.ANCHOR_POSITION_Y],
																					Vector2.zero,
																					NameBaseAssetPath,
																					FileName
																				);
			InformationPartsRuntime.SizeForce = ListSizeForceGetSSAEAttribute(	ref DataSettingImport,
																				InformationProject,
																				CountFrameFull,
																				InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.SIZE_FORCE_X],
																				InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.SIZE_FORCE_Y],
																				NameBaseAssetPath,
																				FileName
																			);
			InformationPartsRuntime.UserData = ListUserDataGetSSAEAttribute(	ref DataSettingImport,
																				InformationProject,
																				CountFrameFull,
																				InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.USERDATA],
																				NameBaseAssetPath,
																				FileName
																			);
			InformationPartsRuntime.Instance = ListInstanceGetSSAEAttribute(	ref DataSettingImport,
																				InformationProject,
																				CountFrameFull,
																				InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.INSTANCE],
																				InformationAnimationSet.ListParts[Index].Kind,
																				NameBaseAssetPath,
																				FileName
																			);
			InformationPartsRuntime.Effect = ListEffectGetSSAEAttribute(	ref DataSettingImport,
																			InformationProject,
																			CountFrameFull,
																			InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.EFFECT],
																			InformationAnimationSet.ListParts[Index].Kind,
																			NameBaseAssetPath,
																			FileName
																		);

			InformationPartsRuntime.DataPlain.Cell = ListCellGetSSAEAttribute(	ref DataSettingImport,
																				InformationProject,
																				CountFrameFull,
																				InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.CELL],
																				NameBaseAssetPath,
																				FileName
																			);

			InformationPartsRuntime.DataPlain.ColorBlend = ListColorBlendGetSSAEAttribute(	ref DataSettingImport,
																							InformationProject,
																							CountFrameFull,
																							InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.COLORBLEND],
																							NameBaseAssetPath,
																							FileName
																						);
			InformationPartsRuntime.DataPlain.VertexCorrection = ListVertexCorrectionGetSSAEAttribute(	ref DataSettingImport,
																										InformationProject,
																										CountFrameFull,
																										InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.VERTEXCORRECTION],
																										NameBaseAssetPath,
																										FileName
																									);
			InformationPartsRuntime.DataPlain.OffsetPivot = ListVector2GetSSAEAttribute(	ref DataSettingImport,
																							InformationProject,
																							CountFrameFull,
																							InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.PIVOT_OFFSET_X],
																							InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.PIVOT_OFFSET_Y],
																							Vector2.zero,
																							NameBaseAssetPath,
																							FileName
																						);
			InformationPartsRuntime.DataPlain.PositionTexture = ListVector2GetSSAEAttribute(	ref DataSettingImport,
																								InformationProject,
																								CountFrameFull,
																								InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.TEXTURE_POSITION_X],
																								InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.TEXTURE_POSITION_Y],
																								Vector2.zero,
																								NameBaseAssetPath,
																								FileName
																							);
			InformationPartsRuntime.DataPlain.ScalingTexture = ListVector2GetSSAEAttribute(	ref DataSettingImport,
																							InformationProject,
																							CountFrameFull,
																							InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.TEXTURE_SCALING_X],
																							InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.TEXTURE_SCALING_Y],
																							Vector2.one,
																							NameBaseAssetPath,
																							FileName
																						);
			InformationPartsRuntime.DataPlain.RotationTexture = ListFloatGetSSAEAttribute(	ref DataSettingImport,
																							InformationProject,
																							CountFrameFull,
																							InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.TEXTURE_ROTATION],
																							0.0f,
																							NameBaseAssetPath,
																							FileName
																						);
			InformationPartsRuntime.DataPlain.RadiusCollision = ListFloatGetSSAEAttribute(	ref DataSettingImport,
																							InformationProject,
																							CountFrameFull,
																							InformationParts.Attribute[(int)LibraryEditor_SpriteStudio.KeyFrame.KindAttribute.COLLISION_RADIUS],
																							0.0f,
																							NameBaseAssetPath,
																							FileName
																						);

			return(true);

//		Convert_PrefabCreateSSAEAnimationParts_ErrorEnd:;
//		Convert_PrefabCreateSSAEAnimationParts_ErrorEnd_NoMessage:;
//			return(false);
		}
		private static Library_SpriteStudio.Data.ListAttributeFloat ListFloatGetSSAEAttribute(	ref SettingImport DataSettingImport,
																								LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
																								int CountFrameFull,
																								ArrayList AttributeList,
																								float ValueInitial,
																								string NameBaseAssetPath,
																								string FileName
																							)
		{
			Library_SpriteStudio.Data.ListAttributeFloat Rv = null;
			int CountAttributeList = (null != AttributeList) ? AttributeList.Count : 0;
			if(0 >= CountAttributeList)
			{
				Rv = new Library_SpriteStudio.Data.ListAttributeFloat();
				Rv.CleanUp();
				Rv.BootUp(0);
				return(Rv);
			}

			/* Data Initialize */
			Rv = new Library_SpriteStudio.Data.ListAttributeFloat();
			Rv.CleanUp();
			Rv.BootUp(CountFrameFull);
			for(int i=0; i<CountFrameFull; i++)
			{
				Rv.ListValue[i] = ValueInitial;
			}

			/* Key-Frames Expand */
			bool FlagDataFirst = true;
			LibraryEditor_SpriteStudio.KeyFrame.DataFloat DataFloatStart = null;
			LibraryEditor_SpriteStudio.KeyFrame.DataFloat DataFloatEnd = null;
			for(int i=0; i<CountAttributeList; i++)
			{
				DataFloatStart = (LibraryEditor_SpriteStudio.KeyFrame.DataFloat)(AttributeList[i]);
				if(true == FlagDataFirst)
				{	/* Data Padding (Front) */
					FlagDataFirst = false;
					for(int FrameNow=0; FrameNow<DataFloatStart.FrameNo; FrameNow++)
					{
						Rv.ListValue[FrameNow] = DataFloatStart.Value;
					}
				}
				if((CountAttributeList - 1) > i)
				{
					DataFloatEnd = (LibraryEditor_SpriteStudio.KeyFrame.DataFloat)(AttributeList[i + 1]);
					for(int FrameNow=DataFloatStart.FrameNo; FrameNow<DataFloatEnd.FrameNo; FrameNow++)
					{
						if((CountFrameFull > FrameNow) && (0 <= FrameNow))
						{
							DataFloatStart.Interpolation(ref Rv.ListValue[FrameNow], FrameNow, DataFloatEnd);
						}
					}
				}
				else
				{
					for(int FrameNow=DataFloatStart.FrameNo; FrameNow<CountFrameFull; FrameNow++)
					{
						if((CountFrameFull > FrameNow) && (0 <= FrameNow))
						{
							Rv.ListValue[FrameNow] = DataFloatStart.Value;
						}
					}
				}
			}

			return(Rv);
		}
		private static Library_SpriteStudio.Data.ListAttributeVector2 ListVector2GetSSAEAttribute(	ref SettingImport DataSettingImport,
																									LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
																									int CountFrameFull,
																									ArrayList AttributeListX,
																									ArrayList AttributeListY,
																									Vector2 ValueInitial,
																									string NameBaseAssetPath,
																									string FileName
																								)
		{
			Library_SpriteStudio.Data.ListAttributeVector2 Rv = null;
			int CountAttributeListX = (null != AttributeListX) ? AttributeListX.Count : 0;
			int CountAttributeListY = (null != AttributeListY) ? AttributeListY.Count : 0;
			if((0 >= CountAttributeListX) && (0 >= CountAttributeListY))
			{
				Rv = new Library_SpriteStudio.Data.ListAttributeVector2();
				Rv.CleanUp();
				Rv.BootUp(0);
				return(Rv);
			}

			/* Data Initialize */
			Rv = new Library_SpriteStudio.Data.ListAttributeVector2();
			Rv.CleanUp();
			Rv.BootUp(CountFrameFull);
			for(int i=0; i<CountFrameFull; i++)
			{
				Rv.ListValue[i] = ValueInitial;
			}

			/* Key-Frames Expand */
			bool FlagDataFirst = true;
			LibraryEditor_SpriteStudio.KeyFrame.DataFloat DataFloatStart = null;
			LibraryEditor_SpriteStudio.KeyFrame.DataFloat DataFloatEnd = null;
			for(int i=0; i<CountAttributeListX; i++)
			{
				DataFloatStart = (LibraryEditor_SpriteStudio.KeyFrame.DataFloat)(AttributeListX[i]);
				if(true == FlagDataFirst)
				{	/* Data Padding (Front) */
					FlagDataFirst = false;
					for(int FrameNow=0; FrameNow<DataFloatStart.FrameNo; FrameNow++)
					{
						Rv.ListValue[FrameNow].x = DataFloatStart.Value;
					}
				}
				if((CountAttributeListX - 1) > i)
				{
					DataFloatEnd = (LibraryEditor_SpriteStudio.KeyFrame.DataFloat)(AttributeListX[i + 1]);
					for(int FrameNow=DataFloatStart.FrameNo; FrameNow<DataFloatEnd.FrameNo; FrameNow++)
					{
						if((CountFrameFull > FrameNow) && (0 <= FrameNow))
						{
							DataFloatStart.Interpolation(ref Rv.ListValue[FrameNow].x, FrameNow, DataFloatEnd);
						}
					}
				}
				else
				{
					for(int FrameNow=DataFloatStart.FrameNo; FrameNow<CountFrameFull; FrameNow++)
					{
						if((CountFrameFull > FrameNow) && (0 <= FrameNow))
						{
							Rv.ListValue[FrameNow].x = DataFloatStart.Value;
						}
					}
				}
			}
			FlagDataFirst = true;
			for(int i=0; i<CountAttributeListY; i++)
			{
				DataFloatStart = (LibraryEditor_SpriteStudio.KeyFrame.DataFloat)(AttributeListY[i]);
				if(true == FlagDataFirst)
				{	/* Data Padding (Front) */
					FlagDataFirst = false;
					for(int FrameNow=0; FrameNow<DataFloatStart.FrameNo; FrameNow++)
					{
						Rv.ListValue[FrameNow].y = DataFloatStart.Value;
					}
				}
				if((CountAttributeListY - 1) > i)
				{
					DataFloatEnd = (LibraryEditor_SpriteStudio.KeyFrame.DataFloat)(AttributeListY[i + 1]);
					for(int FrameNow=DataFloatStart.FrameNo; FrameNow<DataFloatEnd.FrameNo; FrameNow++)
					{
						if((CountFrameFull > FrameNow) && (0 <= FrameNow))
						{
							DataFloatStart.Interpolation(ref Rv.ListValue[FrameNow].y, FrameNow, DataFloatEnd);
						}
					}
				}
				else
				{
					for(int FrameNow=DataFloatStart.FrameNo; FrameNow<CountFrameFull; FrameNow++)
					{
						if((CountFrameFull > FrameNow) && (0 <= FrameNow))
						{
							Rv.ListValue[FrameNow].y = DataFloatStart.Value;
						}
					}
				}
			}

			return(Rv);
		}
		private static Library_SpriteStudio.Data.ListAttributeVector3 ListVector3GetSSAEAttribute(	ref SettingImport DataSettingImport,
																									LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
																									int CountFrameFull,
																									ArrayList AttributeListX,
																									ArrayList AttributeListY,
																									ArrayList AttributeListZ,
																									Vector3 ValueInitial,
																									string NameBaseAssetPath,
																									string FileName
																								)
		{
			Library_SpriteStudio.Data.ListAttributeVector3 Rv = null;
			int CountAttributeListX = (null != AttributeListX) ? AttributeListX.Count : 0;
			int CountAttributeListY = (null != AttributeListY) ? AttributeListY.Count : 0;
			int CountAttributeListZ = (null != AttributeListZ) ? AttributeListZ.Count : 0;
			if((0 >= CountAttributeListX) && (0 >= CountAttributeListY) && (0 >= CountAttributeListZ))
			{
				Rv = new Library_SpriteStudio.Data.ListAttributeVector3();
				Rv.CleanUp();
				Rv.BootUp(0);
				return(Rv);
			}

			/* Data Initialize */
			Rv = new Library_SpriteStudio.Data.ListAttributeVector3();
			Rv.CleanUp();
			Rv.BootUp(CountFrameFull);
			for(int i=0; i<CountFrameFull; i++)
			{
				Rv.ListValue[i] = ValueInitial;
			}

			/* Key-Frames Expand */
			bool FlagDataFirst = true;
			LibraryEditor_SpriteStudio.KeyFrame.DataFloat DataFloatStart = null;
			LibraryEditor_SpriteStudio.KeyFrame.DataFloat DataFloatEnd = null;
			for(int i=0; i<CountAttributeListX; i++)
			{
				DataFloatStart = (LibraryEditor_SpriteStudio.KeyFrame.DataFloat)(AttributeListX[i]);
				if(true == FlagDataFirst)
				{	/* Data Padding (Front) */
					FlagDataFirst = false;
					for(int FrameNow=0; FrameNow<DataFloatStart.FrameNo; FrameNow++)
					{
						Rv.ListValue[FrameNow].x = DataFloatStart.Value;
					}
				}
				if((CountAttributeListX - 1) > i)
				{
					DataFloatEnd = (LibraryEditor_SpriteStudio.KeyFrame.DataFloat)(AttributeListX[i + 1]);
					for(int FrameNow=DataFloatStart.FrameNo; FrameNow<DataFloatEnd.FrameNo; FrameNow++)
					{
						if((CountFrameFull > FrameNow) && (0 <= FrameNow))
						{
							DataFloatStart.Interpolation(ref Rv.ListValue[FrameNow].x, FrameNow, DataFloatEnd);
						}
					}
				}
				else
				{
					for(int FrameNow=DataFloatStart.FrameNo; FrameNow<CountFrameFull; FrameNow++)
					{
						if((CountFrameFull > FrameNow) && (0 <= FrameNow))
						{
							Rv.ListValue[FrameNow].x = DataFloatStart.Value;
						}
					}
				}
			}
			FlagDataFirst = true;
			for(int i=0; i<CountAttributeListY; i++)
			{
				DataFloatStart = (LibraryEditor_SpriteStudio.KeyFrame.DataFloat)(AttributeListY[i]);
				if(true == FlagDataFirst)
				{	/* Data Padding (Front) */
					FlagDataFirst = false;
					for(int FrameNow=0; FrameNow<DataFloatStart.FrameNo; FrameNow++)
					{
						Rv.ListValue[FrameNow].y = DataFloatStart.Value;
					}
				}
				if((CountAttributeListY - 1) > i)
				{
					DataFloatEnd = (LibraryEditor_SpriteStudio.KeyFrame.DataFloat)(AttributeListY[i + 1]);
					for(int FrameNow=DataFloatStart.FrameNo; FrameNow<DataFloatEnd.FrameNo; FrameNow++)
					{
						if((CountFrameFull > FrameNow) && (0 <= FrameNow))
						{
							DataFloatStart.Interpolation(ref Rv.ListValue[FrameNow].y, FrameNow, DataFloatEnd);
						}
					}
				}
				else
				{
					for(int FrameNow=DataFloatStart.FrameNo; FrameNow<CountFrameFull; FrameNow++)
					{
						if((CountFrameFull > FrameNow) && (0 <= FrameNow))
						{
							Rv.ListValue[FrameNow].y = DataFloatStart.Value;
						}
					}
				}
			}
			FlagDataFirst = true;
			for(int i=0; i<CountAttributeListZ; i++)
			{
				DataFloatStart = (LibraryEditor_SpriteStudio.KeyFrame.DataFloat)(AttributeListZ[i]);
				if(true == FlagDataFirst)
				{	/* Data Padding (Front) */
					FlagDataFirst = false;
					for(int FrameNow=0; FrameNow<DataFloatStart.FrameNo; FrameNow++)
					{
						Rv.ListValue[FrameNow].z = DataFloatStart.Value;
					}
				}
				if((CountAttributeListZ - 1) > i)
				{
					DataFloatEnd = (LibraryEditor_SpriteStudio.KeyFrame.DataFloat)(AttributeListZ[i + 1]);
					for(int FrameNow=DataFloatStart.FrameNo; FrameNow<DataFloatEnd.FrameNo; FrameNow++)
					{
						if((CountFrameFull > FrameNow) && (0 <= FrameNow))
						{
							DataFloatStart.Interpolation(ref Rv.ListValue[FrameNow].z, FrameNow, DataFloatEnd);
						}
					}
				}
				else
				{
					for(int FrameNow=DataFloatStart.FrameNo; FrameNow<CountFrameFull; FrameNow++)
					{
						if((CountFrameFull > FrameNow) && (0 <= FrameNow))
						{
							Rv.ListValue[FrameNow].z = DataFloatStart.Value;
						}
					}
				}
			}

			return(Rv);
		}
		private static Library_SpriteStudio.Data.ListAttributeVector2 ListSizeForceGetSSAEAttribute(	ref SettingImport DataSettingImport,
																										LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
																										int CountFrameFull,
																										ArrayList AttributeListX,
																										ArrayList AttributeListY,
																										string NameBaseAssetPath,
																										string FileName
																									)
		{	/* MEMO: Specialization "ListVector2GetSSAEAttribute" for SizeForce */
			Library_SpriteStudio.Data.ListAttributeVector2 Rv = null;
			int CountAttributeListX = (null != AttributeListX) ? AttributeListX.Count : 0;
			int CountAttributeListY = (null != AttributeListY) ? AttributeListY.Count : 0;
			if((0 >= CountAttributeListX) && (0 >= CountAttributeListY))
			{
				Rv = new Library_SpriteStudio.Data.ListAttributeVector2();
				Rv.CleanUp();
				Rv.BootUp(0);
				return(Rv);
			}

			/* Data Initialize */
			Rv = new Library_SpriteStudio.Data.ListAttributeVector2();
			Rv.CleanUp();
			Rv.BootUp(CountFrameFull);
			for(int i=0; i<CountFrameFull; i++)
			{
				Rv.ListValue[i].x = -1.0f;
				Rv.ListValue[i].y = -1.0f;
			}

			/* Key-Frames Expand */
			bool FlagDataFirst = true;
			LibraryEditor_SpriteStudio.KeyFrame.DataFloat DataFloatStart = null;
			LibraryEditor_SpriteStudio.KeyFrame.DataFloat DataFloatEnd = null;
			for(int i=0; i<CountAttributeListX; i++)
			{
				DataFloatStart = (LibraryEditor_SpriteStudio.KeyFrame.DataFloat)(AttributeListX[i]);
				if(true == FlagDataFirst)
				{	/* Data Padding (Front) */
					FlagDataFirst = false;
					for(int FrameNow=0; FrameNow<DataFloatStart.FrameNo; FrameNow++)
					{
						Rv.ListValue[FrameNow].x = DataFloatStart.Value;
						if(0.0f > Rv.ListValue[FrameNow].x)
						{
							Rv.ListValue[FrameNow].x = 0.0f;
						}
					}
				}
				if((CountAttributeListX - 1) > i)
				{
					DataFloatEnd = (LibraryEditor_SpriteStudio.KeyFrame.DataFloat)(AttributeListX[i + 1]);
					for(int FrameNow=DataFloatStart.FrameNo; FrameNow<DataFloatEnd.FrameNo; FrameNow++)
					{
						if((CountFrameFull > FrameNow) && (0 <= FrameNow))
						{
							DataFloatStart.Interpolation(ref Rv.ListValue[FrameNow].x, FrameNow, DataFloatEnd);
							if(0.0f > Rv.ListValue[FrameNow].x)
							{
								Rv.ListValue[FrameNow].x = 0.0f;
							}
						}
					}
				}
				else
				{
					for(int FrameNow=DataFloatStart.FrameNo; FrameNow<CountFrameFull; FrameNow++)
					{
						if((CountFrameFull > FrameNow) && (0 <= FrameNow))
						{
							Rv.ListValue[FrameNow].x = DataFloatStart.Value;
							if(0.0f > Rv.ListValue[FrameNow].x)
							{
								Rv.ListValue[FrameNow].y = 0.0f;
							}
						}
					}
				}
			}
			FlagDataFirst = true;
			for(int i=0; i<CountAttributeListY; i++)
			{
				DataFloatStart = (LibraryEditor_SpriteStudio.KeyFrame.DataFloat)(AttributeListY[i]);
				if(true == FlagDataFirst)
				{	/* Data Padding (Front) */
					FlagDataFirst = false;
					for(int FrameNow=0; FrameNow<DataFloatStart.FrameNo; FrameNow++)
					{
						Rv.ListValue[FrameNow].y = DataFloatStart.Value;
						if(0.0f > Rv.ListValue[FrameNow].y)
						{
							Rv.ListValue[FrameNow].y = 0.0f;
						}
					}
				}
				if((CountAttributeListY - 1) > i)
				{
					DataFloatEnd = (LibraryEditor_SpriteStudio.KeyFrame.DataFloat)(AttributeListY[i + 1]);
					for(int FrameNow=DataFloatStart.FrameNo; FrameNow<DataFloatEnd.FrameNo; FrameNow++)
					{
						if((CountFrameFull > FrameNow) && (0 <= FrameNow))
						{
							DataFloatStart.Interpolation(ref Rv.ListValue[FrameNow].y, FrameNow, DataFloatEnd);
							if(0.0f > Rv.ListValue[FrameNow].y)
							{
								Rv.ListValue[FrameNow].y = 0.0f;
							}
						}
					}
				}
				else
				{
					for(int FrameNow=DataFloatStart.FrameNo; FrameNow<CountFrameFull; FrameNow++)
					{
						if((CountFrameFull > FrameNow) && (0 <= FrameNow))
						{
							Rv.ListValue[FrameNow].y = DataFloatStart.Value;
							if(0.0f > Rv.ListValue[FrameNow].y)
							{
								Rv.ListValue[FrameNow].y = 0.0f;
							}
						}
					}
				}
			}

			return(Rv);
		}
		private static Library_SpriteStudio.Data.ListAttributeStatus ListStatusGetSSAEAttribute(	ref SettingImport DataSettingImport,
																									LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
																									int CountFrameFull,
																									ArrayList AttributeListHide,
																									ArrayList AttributeListFlipX,
																									ArrayList AttributeListFlipY,
																									ArrayList AttributeListFlipXTexture,
																									ArrayList AttributeListFlipYTexture,
																									Library_SpriteStudio.Data.AttributeStatus.FlagBit ValueInitial,
																									string NameBaseAssetPath,
																									string FileName
																								)
		{
			Library_SpriteStudio.Data.ListAttributeStatus Rv = null;
			int CountAttributeListHide = (null != AttributeListHide) ? AttributeListHide.Count : 0;
			int CountAttributeListFlipX = (null != AttributeListFlipX) ? AttributeListFlipX.Count : 0;
			int CountAttributeListFlipY = (null != AttributeListFlipY) ? AttributeListFlipY.Count : 0;
			int CountAttributeListFlipXTexture = (null != AttributeListFlipXTexture) ? AttributeListFlipXTexture.Count : 0;
			int CountAttributeListFlipYTexture = (null != AttributeListFlipYTexture) ? AttributeListFlipYTexture.Count : 0;
			/* MEMO: Force-Create */

			/* Data Initialize */
			Rv = new Library_SpriteStudio.Data.ListAttributeStatus();
			Rv.CleanUp();
			Rv.BootUp(CountFrameFull);
			for(int i=0; i<CountFrameFull; i++)
			{
				Rv.ListValue[i] = new Library_SpriteStudio.Data.AttributeStatus();
				Rv.ListValue[i].Flags = (Library_SpriteStudio.Data.AttributeStatus.FlagBit.VALID | ValueInitial);
			}

			/* Key-Frames Expand */
			bool FlagDataFirst = true;
			LibraryEditor_SpriteStudio.KeyFrame.DataBool DataBool = null;
			Library_SpriteStudio.Data.AttributeStatus.FlagBit Flag;
			Flag = Library_SpriteStudio.Data.AttributeStatus.FlagBit.HIDE;
			for(int i=0; i<CountAttributeListHide; i++)
			{
				DataBool = (LibraryEditor_SpriteStudio.KeyFrame.DataBool)(AttributeListHide[i]);
				if(true == FlagDataFirst)
				{	/* Data Padding (Front) : Force-Hide */
					FlagDataFirst = false;
					for(int FrameNow=0; FrameNow<DataBool.FrameNo; FrameNow++)
					{
						/* MEMO: If Key-Data is present, Default is "hide". */
						Rv.ListValue[FrameNow].Flags |= Flag;
					}
				}
				for(int FrameNow=DataBool.FrameNo; FrameNow<CountFrameFull; FrameNow++)
				{
					Rv.ListValue[FrameNow].Flags = (true == DataBool.Value) ? (Rv.ListValue[FrameNow].Flags | Flag) : (Rv.ListValue[FrameNow].Flags & ~Flag);
				}
			}

			FlagDataFirst = true;
			Flag = Library_SpriteStudio.Data.AttributeStatus.FlagBit.FLIPX;
			for(int i=0; i<CountAttributeListFlipX; i++)
			{
				DataBool = (LibraryEditor_SpriteStudio.KeyFrame.DataBool)(AttributeListFlipX[i]);
				if(true == FlagDataFirst)
				{	/* Data Padding (Front) */
					FlagDataFirst = false;
					for(int FrameNow=0; FrameNow<DataBool.FrameNo; FrameNow++)
					{
						Rv.ListValue[FrameNow].Flags = (true == DataBool.Value) ? (Rv.ListValue[FrameNow].Flags | Flag) : (Rv.ListValue[FrameNow].Flags & ~Flag);
					}
				}
				for(int FrameNow=DataBool.FrameNo; FrameNow<CountFrameFull; FrameNow++)
				{
					Rv.ListValue[FrameNow].Flags = (true == DataBool.Value) ? (Rv.ListValue[FrameNow].Flags | Flag) : (Rv.ListValue[FrameNow].Flags & ~Flag);
				}
			}

			FlagDataFirst = true;
			Flag = Library_SpriteStudio.Data.AttributeStatus.FlagBit.FLIPY;
			for(int i=0; i<CountAttributeListFlipY; i++)
			{
				DataBool = (LibraryEditor_SpriteStudio.KeyFrame.DataBool)(AttributeListFlipY[i]);
				if(true == FlagDataFirst)
				{	/* Data Padding (Front) */
					FlagDataFirst = false;
					for(int FrameNow=0; FrameNow<DataBool.FrameNo; FrameNow++)
					{
						Rv.ListValue[FrameNow].Flags = (true == DataBool.Value) ? (Rv.ListValue[FrameNow].Flags | Flag) : (Rv.ListValue[FrameNow].Flags & ~Flag);
					}
				}
				for(int FrameNow=DataBool.FrameNo; FrameNow<CountFrameFull; FrameNow++)
				{
					Rv.ListValue[FrameNow].Flags = (true == DataBool.Value) ? (Rv.ListValue[FrameNow].Flags | Flag) : (Rv.ListValue[FrameNow].Flags & ~Flag);
				}
			}

			FlagDataFirst = true;
			Flag = Library_SpriteStudio.Data.AttributeStatus.FlagBit.FLIPXTEXTURE;
			for(int i=0; i<CountAttributeListFlipXTexture; i++)
			{
				DataBool = (LibraryEditor_SpriteStudio.KeyFrame.DataBool)(AttributeListFlipXTexture[i]);
				if(true == FlagDataFirst)
				{	/* Data Padding (Front) */
					FlagDataFirst = false;
					for(int FrameNow=0; FrameNow<DataBool.FrameNo; FrameNow++)
					{
						Rv.ListValue[FrameNow].Flags = (true == DataBool.Value) ? (Rv.ListValue[FrameNow].Flags | Flag) : (Rv.ListValue[FrameNow].Flags & ~Flag);
					}
				}
				for(int FrameNow=DataBool.FrameNo; FrameNow<CountFrameFull; FrameNow++)
				{
					Rv.ListValue[FrameNow].Flags = (true == DataBool.Value) ? (Rv.ListValue[FrameNow].Flags | Flag) : (Rv.ListValue[FrameNow].Flags & ~Flag);
				}
			}

			FlagDataFirst = true;
			Flag = Library_SpriteStudio.Data.AttributeStatus.FlagBit.FLIPYTEXTURE;
			for(int i=0; i<CountAttributeListFlipYTexture; i++)
			{
				DataBool = (LibraryEditor_SpriteStudio.KeyFrame.DataBool)(AttributeListFlipYTexture[i]);
				if(true == FlagDataFirst)
				{	/* Data Padding (Front) */
					FlagDataFirst = false;
					for(int FrameNow=0; FrameNow<DataBool.FrameNo; FrameNow++)
					{
						Rv.ListValue[FrameNow].Flags = (true == DataBool.Value) ? (Rv.ListValue[FrameNow].Flags | Flag) : (Rv.ListValue[FrameNow].Flags & ~Flag);
					}
				}
				for(int FrameNow=DataBool.FrameNo; FrameNow<CountFrameFull; FrameNow++)
				{
					Rv.ListValue[FrameNow].Flags = (true == DataBool.Value) ? (Rv.ListValue[FrameNow].Flags | Flag) : (Rv.ListValue[FrameNow].Flags & ~Flag);
				}
			}

			for(int i=0; i<CountAttributeListHide; i++)
			{
				DataBool = (LibraryEditor_SpriteStudio.KeyFrame.DataBool)(AttributeListHide[i]);
				if(true == FlagDataFirst)
				{	/* Data Padding (Front) : Force-Hide */
					FlagDataFirst = false;
					for(int FrameNow=0; FrameNow<DataBool.FrameNo; FrameNow++)
					{
						/* MEMO: If Key-Data is present, Default is "hide". */
//						Rv.ListValue[FrameNow].Flags &= ~Flag;
						Rv.ListValue[FrameNow].Flags |= Flag;
					}
				}
				for(int FrameNow=DataBool.FrameNo; FrameNow<CountFrameFull; FrameNow++)
				{
					Rv.ListValue[FrameNow].Flags = (true == DataBool.Value) ? (Rv.ListValue[FrameNow].Flags | Flag) : (Rv.ListValue[FrameNow].Flags & ~Flag);
				}
			}

			return(Rv);
		}
		private static Library_SpriteStudio.Data.ListAttributeUserData ListUserDataGetSSAEAttribute(	ref SettingImport DataSettingImport,
																										LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
																										int CountFrameFull,
																										ArrayList AttributeList,
																										string NameBaseAssetPath,
																										string FileName
																									)
		{
			Library_SpriteStudio.Data.ListAttributeUserData Rv = null;
			int CountAttributeList = (null != AttributeList) ? AttributeList.Count : 0;
			if(0 >= CountAttributeList)
			{
				Rv = new Library_SpriteStudio.Data.ListAttributeUserData();
				Rv.CleanUp();
				Rv.BootUp(0);
				return(Rv);
			}

			/* CAUTION!: "UserData" is stored in Compressed-State from scratch. */
			/* Data Initialize */
			Rv = new Library_SpriteStudio.Data.ListAttributeUserData();
			Rv.CleanUp();
			Rv.BootUpCompressedForce(CountAttributeList);

			/* Key-Frames Set */
			LibraryEditor_SpriteStudio.KeyFrame.DataUserData DataUserData = null;
			for(int i=0; i<CountAttributeList; i++)
			{
				DataUserData = (LibraryEditor_SpriteStudio.KeyFrame.DataUserData)(AttributeList[i]);
				Rv.ListStatus[i] = Library_SpriteStudio.Data.ListAttributeUserData.GetFlagBit(DataUserData.FrameNo, i);
				Rv.ListValue[i] = new Library_SpriteStudio.Data.AttributeUserData();
				Rv.ListValue[i].Duplicate(DataUserData.Value);
			}

			return(Rv);
		}
		private static Library_SpriteStudio.Data.ListAttributeInstance ListInstanceGetSSAEAttribute(	ref SettingImport DataSettingImport,
																										LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
																										int CountFrameFull,
																										ArrayList AttributeList,
																										Library_SpriteStudio.KindParts KindParts,
																										string NameBaseAssetPath,
																										string FileName
																									)
		{
			Library_SpriteStudio.Data.ListAttributeInstance Rv = null;
			LibraryEditor_SpriteStudio.KeyFrame.DataInstance DataInstance = null;
			int CountAttributeList = (null != AttributeList) ? AttributeList.Count : 0;
			bool FlagDummyDataInsert = false;
			if(0 >= CountAttributeList)
			{	/* Has no datas */
				if(Library_SpriteStudio.KindParts.INSTANCE == KindParts)
				{
					FlagDummyDataInsert = true;
				}
				else
				{
					Rv = new Library_SpriteStudio.Data.ListAttributeInstance();
					Rv.CleanUp();
					Rv.BootUp(0);
					return(Rv);
				}
			}
			else
			{	/* Has datas */
				/* Check top key's frame */
				DataInstance = (LibraryEditor_SpriteStudio.KeyFrame.DataInstance)(AttributeList[0]);
				if(0 < DataInstance.FrameNo)
				{	/* Insert Default, force (After SS5.7) */
					FlagDummyDataInsert = true;
				}
			}

			/* CAUTION!: "Instance" is stored in Compressed-State from scratch. */
			/* Data Initialize */
			Rv = new Library_SpriteStudio.Data.ListAttributeInstance();
			Rv.CleanUp();
			Rv.BootUpCompressedForce(CountAttributeList + ((false == FlagDummyDataInsert) ? 0 : 1));

			/* Key-Frames Set */
			int Index = 0;
			if(true == FlagDummyDataInsert)
			{	/* Cover a missing data */
				FlagDummyDataInsert = false;
				Rv.ListStatus[Index] = Library_SpriteStudio.Data.ListAttributeInstance.GetFlagBit(0, 0);
				Rv.ListValue[Index] = new Library_SpriteStudio.Data.AttributeInstance();
				Rv.ListValue[Index].CleanUp();
				Index++;
			}
			for(int i=0; i<CountAttributeList; i++)
			{
				DataInstance = (LibraryEditor_SpriteStudio.KeyFrame.DataInstance)(AttributeList[i]);
				Rv.ListStatus[Index] = Library_SpriteStudio.Data.ListAttributeInstance.GetFlagBit(DataInstance.FrameNo, Index);
				Rv.ListValue[Index] = new Library_SpriteStudio.Data.AttributeInstance();
				Rv.ListValue[Index].Duplicate(DataInstance.Value);
				Index++;
			}

			return(Rv);
		}
		private static Library_SpriteStudio.Data.ListAttributeEffect ListEffectGetSSAEAttribute(	ref SettingImport DataSettingImport,
																									LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
																									int CountFrameFull,
																									ArrayList AttributeList,
																									Library_SpriteStudio.KindParts KindParts,
																									string NameBaseAssetPath,
																									string FileName
																								)
		{
			Library_SpriteStudio.Data.ListAttributeEffect Rv = null;
			LibraryEditor_SpriteStudio.KeyFrame.DataEffect DataEffect = null;
			int CountAttributeList = (null != AttributeList) ? AttributeList.Count : 0;
			bool FlagDummyDataInsert = false;
			if(0 >= CountAttributeList)
			{	/* Has no datas */
				if(Library_SpriteStudio.KindParts.EFFECT == KindParts)
				{
					FlagDummyDataInsert = true;
				}
				else
				{
					Rv = new Library_SpriteStudio.Data.ListAttributeEffect();
					Rv.CleanUp();
					Rv.BootUp(0);
					return(Rv);
				}
			}
			else
			{	/* Has datas */
				/* Check top key's frame */
				DataEffect = (LibraryEditor_SpriteStudio.KeyFrame.DataEffect)(AttributeList[0]);
				if(0 < DataEffect.FrameNo)
				{	/* Insert Default, force (After SS5.7) */
					FlagDummyDataInsert = true;
				}
			}

			/* CAUTION!: "Instance" is stored in Compressed-State from scratch. */
			/* Data Initialize */
			Rv = new Library_SpriteStudio.Data.ListAttributeEffect();
			Rv.CleanUp();
			Rv.BootUpCompressedForce(CountAttributeList + ((false == FlagDummyDataInsert) ? 0 : 1));

			/* Key-Frames Set */
			int Index = 0;
			if(true == FlagDummyDataInsert)
			{	/* Cover a missing data */
				FlagDummyDataInsert = false;
				Rv.ListStatus[Index] = Library_SpriteStudio.Data.ListAttributeEffect.GetFlagBit(0, 0);
				Rv.ListValue[Index] = new Library_SpriteStudio.Data.AttributeEffect();
				Rv.ListValue[Index].CleanUp();
				Rv.ListValue[Index].FrameStart = 0;
				Rv.ListValue[Index].RateTime = 1.0f;

				Index++;
			}
			for(int i=0; i<CountAttributeList; i++)
			{
				DataEffect = (LibraryEditor_SpriteStudio.KeyFrame.DataEffect)(AttributeList[i]);
				Rv.ListStatus[Index] = Library_SpriteStudio.Data.ListAttributeEffect.GetFlagBit(DataEffect.FrameNo, Index);
				Rv.ListValue[Index] = new Library_SpriteStudio.Data.AttributeEffect();
				Rv.ListValue[Index].Duplicate(DataEffect.Value);
				Index++;
			}

			return(Rv);
		}
		private static Library_SpriteStudio.Data.ListAttributeCell ListCellGetSSAEAttribute(	ref SettingImport DataSettingImport,
																								LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
																								int CountFrameFull,
																								ArrayList AttributeList,
																								string NameBaseAssetPath,
																								string FileName
																							)
		{
			Library_SpriteStudio.Data.ListAttributeCell Rv = null;
			int CountAttributeList = (null != AttributeList) ? AttributeList.Count : 0;
			if(0 >= CountAttributeList)
			{
				Rv = new Library_SpriteStudio.Data.ListAttributeCell();
				Rv.CleanUp();
				Rv.BootUp(0);
				return(Rv);
			}

			/* Data Initialize */
			Rv = new Library_SpriteStudio.Data.ListAttributeCell();
			Rv.CleanUp();
			Rv.BootUp(CountFrameFull);
			for(int i=0; i<CountFrameFull; i++)
			{
				Rv.ListValue[i] = new Library_SpriteStudio.Data.AttributeCell();
				Rv.ListValue[i].CleanUp();
			}

			/* Key-Frames Expand */
			bool FlagDataFirst = true;
			LibraryEditor_SpriteStudio.KeyFrame.DataCell DataCell = null;
			for(int i=0; i<CountAttributeList; i++)
			{
				DataCell = (LibraryEditor_SpriteStudio.KeyFrame.DataCell)(AttributeList[i]);
				if(true == FlagDataFirst)
				{	/* Data Padding (Front) */
					FlagDataFirst = false;
					for(int FrameNow=0; FrameNow<DataCell.FrameNo; FrameNow++)
					{
						Rv.ListValue[FrameNow].Duplicate(DataCell.Value);
					}
				}
				for(int FrameNow=DataCell.FrameNo; FrameNow<CountFrameFull; FrameNow++)
				{
					Rv.ListValue[FrameNow].Duplicate(DataCell.Value);
				}
			}

			return(Rv);
		}
		private static Library_SpriteStudio.Data.ListAttributeColorBlend ListColorBlendGetSSAEAttribute(	ref SettingImport DataSettingImport,
																											LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
																											int CountFrameFull,
																											ArrayList AttributeList,
																											string NameBaseAssetPath,
																											string FileName
																										)
		{
			Library_SpriteStudio.Data.ListAttributeColorBlend Rv = null;
			int CountAttributeList = (null != AttributeList) ? AttributeList.Count : 0;
			if(0 >= CountAttributeList)
			{
				Rv = new Library_SpriteStudio.Data.ListAttributeColorBlend();
				Rv.CleanUp();
				Rv.BootUp(0);
				return(Rv);
			}

			/* Data Initialize */
			Rv = new Library_SpriteStudio.Data.ListAttributeColorBlend();
			Rv.CleanUp();
			Rv.BootUp(CountFrameFull);
			for(int i=0; i<CountFrameFull; i++)
			{
				Rv.ListValue[i] = new Library_SpriteStudio.Data.AttributeColorBlend();
				Rv.ListValue[i].CleanUp();

				Rv.ListValue[i].VertexColor = new Color[(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2];
				Rv.ListValue[i].RatePixelAlpha = new float[(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2];
				for(int j=0; j<(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2; j++)
				{
					Rv.ListValue[i].VertexColor[j] = Color.white;
					Rv.ListValue[i].RatePixelAlpha[j] = 1.0f;
				}
			}

			/* Key-Frames Expand */
			bool FlagDataFirst = true;
			LibraryEditor_SpriteStudio.KeyFrame.DataColorBlend DataColorBlendStart = null;
			LibraryEditor_SpriteStudio.KeyFrame.DataColorBlend DataColorBlendEnd = null;
			for(int i=0; i<CountAttributeList; i++)
			{
				DataColorBlendStart = (LibraryEditor_SpriteStudio.KeyFrame.DataColorBlend)(AttributeList[i]);
				if(true == FlagDataFirst)
				{	/* Data Padding (Front) */
					FlagDataFirst = false;
					for(int FrameNow=0; FrameNow<DataColorBlendStart.FrameNo; FrameNow++)
					{
						Rv.ListValue[FrameNow].Duplicate(DataColorBlendStart.Value);
					}
				}
				if((CountAttributeList - 1) > i)
				{
					DataColorBlendEnd = (LibraryEditor_SpriteStudio.KeyFrame.DataColorBlend)(AttributeList[i + 1]);
					for(int FrameNow=DataColorBlendStart.FrameNo; FrameNow<DataColorBlendEnd.FrameNo; FrameNow++)
					{
						if(CountFrameFull > FrameNow)
						{
							DataColorBlendStart.Interpolation(ref Rv.ListValue[FrameNow], FrameNow, DataColorBlendEnd);
						}
					}
				}
				else
				{
					for(int FrameNow=DataColorBlendStart.FrameNo; FrameNow<CountFrameFull; FrameNow++)
					{
						Rv.ListValue[FrameNow].Duplicate(DataColorBlendStart.Value);
					}
				}
			}

			return(Rv);
		}
		private static Library_SpriteStudio.Data.ListAttributeVertexCorrection ListVertexCorrectionGetSSAEAttribute(	ref SettingImport DataSettingImport,
																														LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
																														int CountFrameFull,
																														ArrayList AttributeList,
																														string NameBaseAssetPath,
																														string FileName
																													)
		{
			Library_SpriteStudio.Data.ListAttributeVertexCorrection Rv = null;
			int CountAttributeList = (null != AttributeList) ? AttributeList.Count : 0;
			if(0 >= CountAttributeList)
			{
				Rv = new Library_SpriteStudio.Data.ListAttributeVertexCorrection();
				Rv.CleanUp();
				Rv.BootUp(0);
				return(Rv);
			}

			/* Data Initialize */
			Rv = new Library_SpriteStudio.Data.ListAttributeVertexCorrection();
			Rv.CleanUp();
			Rv.BootUp(CountFrameFull);
			for(int i=0; i<CountFrameFull; i++)
			{
				Rv.ListValue[i] = new Library_SpriteStudio.Data.AttributeVertexCorrection();
				Rv.ListValue[i].CleanUp();

				Rv.ListValue[i].Coordinate = new Vector2[(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2];
				for(int j=0; j<(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2; j++)
				{
					Rv.ListValue[i].Coordinate[j] = Vector2.zero;
				}
			}

			/* Key-Frames Expand */
			bool FlagDataFirst = true;
			LibraryEditor_SpriteStudio.KeyFrame.DataVertexCorrection DataVertexCorrectionStart = null;
			LibraryEditor_SpriteStudio.KeyFrame.DataVertexCorrection DataVertexCorrectionEnd = null;
			for(int i=0; i<CountAttributeList; i++)
			{
				DataVertexCorrectionStart = (LibraryEditor_SpriteStudio.KeyFrame.DataVertexCorrection)(AttributeList[i]);
				if(true == FlagDataFirst)
				{	/* Data Padding (Front) */
					FlagDataFirst = false;
					for(int FrameNow=0; FrameNow<DataVertexCorrectionStart.FrameNo; FrameNow++)
					{
						Rv.ListValue[FrameNow].Duplicate(DataVertexCorrectionStart.Value);
					}
				}
				if((CountAttributeList - 1) > i)
				{
					DataVertexCorrectionEnd = (LibraryEditor_SpriteStudio.KeyFrame.DataVertexCorrection)(AttributeList[i + 1]);
					for(int FrameNow=DataVertexCorrectionStart.FrameNo; FrameNow<DataVertexCorrectionEnd.FrameNo; FrameNow++)
					{
						if(CountFrameFull > FrameNow)
						{
							DataVertexCorrectionStart.Interpolation(ref Rv.ListValue[FrameNow], FrameNow, DataVertexCorrectionEnd);
						}
					}
				}
				else
				{
					for(int FrameNow=DataVertexCorrectionStart.FrameNo; FrameNow<CountFrameFull; FrameNow++)
					{
						Rv.ListValue[FrameNow].Duplicate(DataVertexCorrectionStart.Value);
					}
				}
			}

			return(Rv);
		}
		private static bool PrefabCreateSSAEDataSolveParts(	ref SettingImport DataSettingImport,
															LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
															LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAE InformationAnimationSet,
															int IndexAnimation,
															int IndexParts,
															string NameBaseAssetPath,
															string FileName
														)
		{
			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts InformationParts = InformationAnimationSet.ListParts[IndexParts];
			Library_SpriteStudio.Data.Parts DataPartsRuntime = InformationAnimationSet.ListDataPartsRuntime[IndexParts];
			Library_SpriteStudio.Data.Animation DataAnimationRuntime = InformationAnimationSet.ListDataAnimationRuntime[IndexAnimation];
			Library_SpriteStudio.Data.AnimationParts DataAnimationPartsRuntime = DataAnimationRuntime.ListAnimationParts[IndexParts];

			/* Parts-Kind Solving */
			switch(DataPartsRuntime.Kind)
			{
				case Library_SpriteStudio.KindParts.NORMAL:
					DataPartsRuntime.Kind = Library_SpriteStudio.KindParts.NORMAL_TRIANGLE2;
					goto case Library_SpriteStudio.KindParts.NORMAL_TRIANGLE2;

				case Library_SpriteStudio.KindParts.NORMAL_TRIANGLE2:
					if(true == DataAnimationPartsRuntime.DataPlain.VertexCorrection.IsValid)
					{
						DataPartsRuntime.Kind = Library_SpriteStudio.KindParts.NORMAL_TRIANGLE4;
					}
					break;

				case Library_SpriteStudio.KindParts.NORMAL_TRIANGLE4:
					break;

				default:
					break;
			}

			/* Animation-Data Solving: Inherit-Status */
			int IndexPartsParent = DataPartsRuntime.IDParent;
			switch(InformationParts.KindInheritance)
			{
				case LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.KindTypeInheritance.PARENT:
					/* MEMO: Parent-Part is already resolved. */
					{
						IndexPartsParent = InformationParts.IDParent;
						if(0 <= IndexPartsParent)
						{
							InformationParts.KindInheritance = LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.KindTypeInheritance.SELF;
							InformationParts.FlagInheritance = InformationAnimationSet.ListParts[IndexPartsParent].FlagInheritance;
						}
					}
					break;
				case LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.KindTypeInheritance.SELF:
					break;

				default:
					goto case LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.KindTypeInheritance.PARENT;
			}

			/* Animation-Data Solving: Status (Hide/FlipX/FlipY) */
			/* MEMO: "Status" is always exist */
			int CountFrameFull = DataAnimationRuntime.CountFrame;
			IndexPartsParent = DataPartsRuntime.IDParent;
			if(-1 == IndexPartsParent)
			{	/* Root-Parts */
				goto Convert_PrefabCreateSSAEDataSolveParts_End;
			}
			Library_SpriteStudio.Data.AnimationParts DataAnimationPartsRuntimeParent = DataAnimationRuntime.ListAnimationParts[IndexPartsParent];
			Library_SpriteStudio.Data.AttributeStatus.FlagBit DataStatus;
			Library_SpriteStudio.Data.AttributeStatus.FlagBit DataStatusParent;
			Library_SpriteStudio.Data.AttributeStatus.FlagBit DataMask;
			for(int i=0; i<CountFrameFull; i++)
			{
				DataStatus = DataAnimationPartsRuntime.Status.ListValue[i].Flags;
				DataStatusParent = DataAnimationPartsRuntimeParent.Status.ListValue[i].Flags;

				/* Solving FlipX */
				if(0 != (InformationParts.FlagInheritance & LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.FlagBitInheritance.FLIP_X))
				{
					/* XOR (1:1=0/1:0=1/0:1=1/0:0=0) */
					DataMask = Library_SpriteStudio.Data.AttributeStatus.FlagBit.FLIPX;
					DataStatus &= ~DataMask;
					DataStatus |= (DataStatus ^ (DataStatusParent & DataMask));
				}

				/* Solving FlipY */
				if(0 != (InformationParts.FlagInheritance & LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.FlagBitInheritance.FLIP_Y))
				{
					/* XOR (1:1=0/1:0=1/0:1=1/0:0=0) */
					DataMask = Library_SpriteStudio.Data.AttributeStatus.FlagBit.FLIPY;
					DataStatus &= ~DataMask;
					DataStatus |= (DataStatus ^ (DataStatusParent & DataMask));
				}

				/* Solving Hide */
				if(0 != (InformationParts.FlagInheritance & LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.FlagBitInheritance.SHOW_HIDE))
				{
					/* Copy Parent's Data */
					DataMask = Library_SpriteStudio.Data.AttributeStatus.FlagBit.HIDE;
					DataStatus &= ~DataMask;
					DataStatus |= (DataStatusParent & DataMask);
				}

				/* Set Data */
				DataAnimationPartsRuntime.Status.ListValue[i].Flags = DataStatus;
			}

			/* Animation-Data Solving: Status (RateOpacity) */
			if(0 != (InformationParts.FlagInheritance & LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAEParts.FlagBitInheritance.OPACITY_RATE))
			{
				/* Parent-Parts Get */
				IndexPartsParent = DataPartsRuntime.IDParent;
				if(0 <= IndexPartsParent)
				{	/* not "Root" */
					if(false == DataAnimationPartsRuntime.RateOpacity.IsValid)
					{	/* Part don't have "RateOpacity" */
						/* Create "RateOpacity" */
						DataAnimationPartsRuntime.RateOpacity.BootUp(CountFrameFull);
						for(int i=0; i<CountFrameFull; i++)
						{
							DataAnimationPartsRuntime.RateOpacity.ListValue[i] = 1.0f;	/* Default-Value */
						}
					}

					DataAnimationPartsRuntimeParent = DataAnimationRuntime.ListAnimationParts[IndexPartsParent];
					if(true == DataAnimationPartsRuntimeParent.RateOpacity.IsValid)
					{	/* Parent has "RateOpacity". */
						for(int i=0; i<CountFrameFull; i++)
						{
							DataAnimationPartsRuntime.RateOpacity.ListValue[i] *= DataAnimationPartsRuntimeParent.RateOpacity.ListValue[i];
						}
					}
					else
					{	/* Parent don't have "RateOpacity". */
						/* MEMO: Parent's "RateOpacity" is always 1.0f. */
					}
				}
			}

		Convert_PrefabCreateSSAEDataSolveParts_End:;
			return(true);
		}
		private static bool PrefabCreateSSAEDataJudgeNoUseParts(	ref SettingImport DataSettingImport,
																	LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
																	LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAE InformationAnimationSet,
																	int IndexAnimation,
																	int IndexParts,
																	string NameBaseAssetPath,
																	string FileName
																)
		{
			Library_SpriteStudio.Data.Parts DataPartsRuntime = InformationAnimationSet.ListDataPartsRuntime[IndexParts];
			Library_SpriteStudio.Data.Animation DataAnimationRuntime = InformationAnimationSet.ListDataAnimationRuntime[IndexAnimation];
			Library_SpriteStudio.Data.AnimationParts DataAnimationPartsRuntime = DataAnimationRuntime.ListAnimationParts[IndexParts];

			DataAnimationPartsRuntime.StatusParts = Library_SpriteStudio.Data.AnimationParts.FlagBitStatus.CLEAR;
			switch(DataPartsRuntime.Kind)
			{
				case Library_SpriteStudio.KindParts.INSTANCE:
				case Library_SpriteStudio.KindParts.EFFECT:
					break;

				default:
					goto PrefabCreateSSAEDataJudgeNoUseParts_NotApplicable;
			}

			/* Check Hide */
			int CountFrameFull = DataAnimationRuntime.CountFrame;
			int IndexAttribute;
			int FrameNoOrigin;
			Library_SpriteStudio.Data.AttributeStatus DataStatus;
			bool FlagHideAll = true;
			for(int i=0; i<CountFrameFull; i++)
			{
				IndexAttribute = DataAnimationPartsRuntime.Status.IndexGetValue(out FrameNoOrigin, i);
				DataStatus = (0 <= IndexAttribute) ? DataAnimationPartsRuntime.Status.ListValue[IndexAttribute] : Library_SpriteStudio.Data.DummyStatus;
				if(false == DataStatus.IsHide)
				{
					FlagHideAll = false;
					break;
				}
			}
			if(true == FlagHideAll)
			{	/* not Used */
				DataAnimationPartsRuntime.StatusParts |= Library_SpriteStudio.Data.AnimationParts.FlagBitStatus.HIDE_FULL;
			}

			return(true);

		PrefabCreateSSAEDataJudgeNoUseParts_NotApplicable:;
			DataAnimationPartsRuntime.StatusParts &= ~Library_SpriteStudio.Data.AnimationParts.FlagBitStatus.HIDE_FULL;
			return(true);
		}
		private static bool PrefabCreateSSAEDataCompressRequired(	ref SettingImport DataSettingImport,
																	LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
																	LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAE InformationAnimationSet,
																	int IndexAnimation,
																	int IndexParts,
																	string NameBaseAssetPath,
																	string FileName
																)
		{
			Library_SpriteStudio.Data.Animation DataAnimationRuntime = InformationAnimationSet.ListDataAnimationRuntime[IndexAnimation];
			Library_SpriteStudio.Data.AnimationParts DataAnimationPartsRuntime = DataAnimationRuntime.ListAnimationParts[IndexParts];

			DataAnimationPartsRuntime.Fix(DataAnimationRuntime.CountFrame);

			return(true);
		}
		private static bool PrefabCreateSSAEDataCalculateInAdvance(	ref SettingImport DataSettingImport,
																	LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
																	LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAE InformationAnimationSet,
																	int IndexAnimation,
																	int IndexParts,
																	string NameBaseAssetPath,
																	string FileName
																)
		{
			Library_SpriteStudio.Data.Parts DataPartsRuntime = InformationAnimationSet.ListDataPartsRuntime[IndexParts];
			Library_SpriteStudio.Data.Animation DataAnimationRuntime = InformationAnimationSet.ListDataAnimationRuntime[IndexAnimation];
			Library_SpriteStudio.Data.AnimationParts DataAnimationPartsRuntime = DataAnimationRuntime.ListAnimationParts[IndexParts];

			int CountFrame = DataAnimationRuntime.CountFrame;
			int CountVertexData = (Library_SpriteStudio.KindParts.NORMAL_TRIANGLE2 == DataPartsRuntime.Kind) ? (int)Library_SpriteStudio.KindVertexNo.TERMINATOR2 : (int) Library_SpriteStudio.KindVertexNo.TERMINATOR4;

			/* Create Data Area */
			bool FlagHasMesh = false;
			bool FlagHasCollider = false;
			switch(DataPartsRuntime.Kind)
			{
				case Library_SpriteStudio.KindParts.NORMAL_TRIANGLE2:
					/* Has Mesh, 2-Triangle */
					FlagHasMesh = true;
					CountVertexData = (int)Library_SpriteStudio.KindVertexNo.TERMINATOR2;
					break;

				case Library_SpriteStudio.KindParts.NORMAL_TRIANGLE4:
					/* Has Mesh, 4-Triangle */
					FlagHasMesh = true;
					CountVertexData = (int)Library_SpriteStudio.KindVertexNo.TERMINATOR4;
					break;

				case Library_SpriteStudio.KindParts.ROOT:
				case Library_SpriteStudio.KindParts.NULL:
				case Library_SpriteStudio.KindParts.INSTANCE:
				case Library_SpriteStudio.KindParts.EFFECT:
					/* Has No-Mesh */
					FlagHasMesh = false;
					CountVertexData = 0;
					break;

				case Library_SpriteStudio.KindParts.NON:
				default:
					/* Error */
					goto case Library_SpriteStudio.KindParts.NULL;
			}
			switch(DataPartsRuntime.KindShapeCollision)
			{
				case Library_SpriteStudio.KindCollision.SQUARE:
					FlagHasCollider= true;
					break;

				case Library_SpriteStudio.KindCollision.AABB:
					goto case Library_SpriteStudio.KindCollision.NON;

				case Library_SpriteStudio.KindCollision.CIRCLE:
					/* Data is Collider-Radius ... Has No datas for Box-Collider */
					FlagHasCollider= false;
					break;

				case Library_SpriteStudio.KindCollision.CIRCLE_SCALEMINIMUM:
					goto case Library_SpriteStudio.KindCollision.NON;

				case Library_SpriteStudio.KindCollision.CIRCLE_SCALEMAXIMUM:
					goto case Library_SpriteStudio.KindCollision.NON;

				case Library_SpriteStudio.KindCollision.NON:
				default:
					FlagHasCollider= false;
					break;
			}
			if(	((null == DataAnimationPartsRuntime.DataPlain.Cell) || (false == DataAnimationPartsRuntime.DataPlain.Cell.IsValid))
				&& ((null == DataAnimationPartsRuntime.SizeForce) || (false == DataAnimationPartsRuntime.SizeForce.IsValid))
				)
			{
				FlagHasMesh = false;
				FlagHasCollider = false;
			}

			Library_SpriteStudio.Data.ListAttributeIndexCellMapFix IndexCellMapFix = new Library_SpriteStudio.Data.ListAttributeIndexCellMapFix();
			Library_SpriteStudio.Data.ListAttributeCoordinateMeshFix CoordinateMesh = new Library_SpriteStudio.Data.ListAttributeCoordinateMeshFix();
			Library_SpriteStudio.Data.ListAttributeColorBlendMeshFix ColorBlendMesh = new Library_SpriteStudio.Data.ListAttributeColorBlendMeshFix();
			Library_SpriteStudio.Data.ListAttributeUVMeshFix UV0Mesh = new Library_SpriteStudio.Data.ListAttributeUVMeshFix();
			if(false == FlagHasMesh)
			{	/* No-Datas */
				IndexCellMapFix.BootUp(0);
				CoordinateMesh.BootUp(0);
				ColorBlendMesh.BootUp(0);
				UV0Mesh.BootUp(0);
			}
			else
			{
				IndexCellMapFix.BootUp(CountFrame);
				CoordinateMesh.BootUp(CountFrame);
				ColorBlendMesh.BootUp(CountFrame);
				UV0Mesh.BootUp(CountFrame);

				for(int FrameNo=0; FrameNo<CountFrame; FrameNo++)
				{
					for(int i=0; i<CountVertexData; i++)
					{
						CoordinateMesh.ListValue[FrameNo] = new Library_SpriteStudio.Data.AttributeCoordinateMeshFix();
						CoordinateMesh.ListValue[FrameNo].Coordinate = new Vector3[CountVertexData];

						ColorBlendMesh.ListValue[FrameNo] = new Library_SpriteStudio.Data.AttributeColorBlendMeshFix();
						ColorBlendMesh.ListValue[FrameNo].ColorOverlay = new Color32[CountVertexData];
						ColorBlendMesh.ListValue[FrameNo].UV = new Vector2[CountVertexData];

						UV0Mesh.ListValue[FrameNo] = new Library_SpriteStudio.Data.AttributeUVMeshFix();
						UV0Mesh.ListValue[FrameNo].UV = new Vector2[CountVertexData];
					}
				}
			}

			Library_SpriteStudio.Data.ListAttributeVector2 SizeCollision = new Library_SpriteStudio.Data.ListAttributeVector2();
			Library_SpriteStudio.Data.ListAttributeVector2 PivotCollision = new Library_SpriteStudio.Data.ListAttributeVector2();
			if(false == FlagHasCollider)
			{	/* No-Datas */
				SizeCollision.BootUp(0);
				PivotCollision.BootUp(0);
			}
			else
			{
				SizeCollision.BootUp(CountFrame);
				PivotCollision.BootUp(CountFrame);
			}

			/* Calculate Datas */
			int FrameNoOrigin;
			int IndexAttribute;
			int IndexAttributeStatus;
			Library_SpriteStudio.Data.AttributeStatus DataStatus;
			Library_SpriteStudio.Data.CellMap DataCellMap;
			Library_SpriteStudio.Data.Cell DataCell;
			int IndexCellMap;
			int IndexCell;
			int IndexVertexCollectionTable;
			Rect RectangleCell = Rect.MinMaxRect(0.0f, 0.0f, 64.0f, 64.0f);
			Vector2 SizeTextureOriginal;
			Vector2 RateScaleMesh;
			Vector2 SizePixelMesh;
			Vector2 PivotMesh;
			Vector2 RateScaleTexture;
			Vector2 RateScaleTextureData;
			Vector2 PivotTexture;
			Vector2 RateScaleBoxCollider;
			Vector2 SizeBoxCollider;
			Vector2 PivotBoxCollider;
			Matrix4x4 MatrixTexture;

			for(int FrameNo=0; FrameNo<CountFrame; FrameNo++)
			{
				/* Default Value Set */
				IndexVertexCollectionTable = 0;
				SizeTextureOriginal = Vector2.one * 64.0f;

				RectangleCell.x = 0.0f;
				RectangleCell.y = 0.0f;
				RectangleCell.width = 64.0f;
				RectangleCell.height = 64.0f;
				RateScaleMesh = Vector2.one;
				SizePixelMesh = Vector2.zero;
				PivotMesh = Vector2.zero;

				RateScaleTexture = Vector2.one;
				PivotTexture = Vector2.zero;

				IndexCellMap = -1;
				IndexCell = -1;

				/* Status Get */
				IndexAttributeStatus = DataAnimationPartsRuntime.Status.IndexGetValue(out FrameNoOrigin, FrameNo);
				DataStatus = (0 <= IndexAttributeStatus) ? DataAnimationPartsRuntime.Status.ListValue[IndexAttributeStatus] : Library_SpriteStudio.Data.DummyStatus;
				if(true == DataStatus.IsFlipX)
				{
					RateScaleMesh.x = -1.0f;
					IndexVertexCollectionTable += 1;
				}
				else
				{
					RateScaleMesh.x = 1.0f;
				}
				if(true == DataStatus.IsFlipY)
				{
					RateScaleMesh.y = -1.0f;
					IndexVertexCollectionTable += 2;
				}
				else
				{
					RateScaleMesh.y = 1.0f;
				}
				RateScaleTexture.x = (true == DataStatus.IsTextureFlipX) ? -1.0f : 1.0f;
				RateScaleTexture.y = (true == DataStatus.IsTextureFlipY) ? -1.0f : 1.0f;

				/* Cell Data Get */
				IndexAttribute = DataAnimationPartsRuntime.DataPlain.Cell.IndexGetValue(out FrameNoOrigin, FrameNo);
				if(0 <= IndexAttribute)
				{
					/* Cell Data Get */
					Library_SpriteStudio.Data.AttributeCell AttributeCell = DataAnimationPartsRuntime.DataPlain.Cell.ListValue[IndexAttribute];
					IndexCellMap = AttributeCell.IndexCellMap;
					IndexCellMapFix.ListValue[FrameNo] = IndexCellMap;
					if(0 <= IndexCellMap)
					{
						IndexCell = AttributeCell.IndexCell;
						DataCellMap = InformationProject.ListInformationSSCE[IndexCellMap].DataCellMap;
						if(null != DataCellMap)
						{
							/* Original Texture Size Get */
							SizeTextureOriginal = DataCellMap.SizeOriginal;

							/* Sprite Size Get */
							DataCell = DataCellMap.DataGetCell(IndexCell);
							if(null == DataCell)
							{	/* Invalid */
								PivotMesh = Vector2.zero;
								RectangleCell = Rect.MinMaxRect(0.0f, 0.0f, 64.0f, 64.0f);
							}
							else
							{	/* Valid */
								PivotMesh = DataCell.Pivot;
								RectangleCell = DataCell.Rectangle;
							}
							SizePixelMesh.x = RectangleCell.width;
							SizePixelMesh.y = RectangleCell.height;

							/* Texture Size Get */
							PivotTexture.x = RectangleCell.width * 0.5f;
							PivotTexture.y = RectangleCell.height * 0.5f;
							IndexAttribute = DataAnimationPartsRuntime.DataPlain.ScalingTexture.IndexGetValue(out FrameNoOrigin, FrameNo);
							if(0 <= IndexAttribute)
							{
								RateScaleTextureData = DataAnimationPartsRuntime.DataPlain.ScalingTexture.ListValue[IndexAttribute];
								RateScaleTexture.x *= RateScaleTextureData.x;
								RateScaleTexture.y *= RateScaleTextureData.y;
							}
						}
					}
				}

				/* Recalc Mesh Size & Pivot (Considering SizeForce-X/Y & OffsetPivot-X/Y) */
				MeshRecalcSizeAndPivot(ref PivotMesh, ref SizePixelMesh, ref RateScaleMesh, FrameNo, DataAnimationPartsRuntime);

				/* Calculate Collider Size & Pivot */
				RateScaleBoxCollider = RateScaleMesh;
				SizeBoxCollider = SizePixelMesh;
				PivotBoxCollider = PivotMesh;
				PivotBoxCollider.x = -(PivotBoxCollider.x - (SizeBoxCollider.x * 0.5f)) * RateScaleBoxCollider.x;
				PivotBoxCollider.y = (PivotBoxCollider.y - (SizeBoxCollider.y * 0.5f)) * RateScaleBoxCollider.y;
				if(true == FlagHasCollider)
				{
					SizeCollision.ListValue[FrameNo] = SizeBoxCollider;
					PivotCollision.ListValue[FrameNo] = PivotBoxCollider;
				}

				/* Calculate Matrix-Texture & Set Mapping-UV */
				IndexAttribute = DataAnimationPartsRuntime.DataPlain.RotationTexture.IndexGetValue(out FrameNoOrigin, FrameNo);
				float Rotate = (0 <= IndexAttribute) ? DataAnimationPartsRuntime.DataPlain.RotationTexture.ListValue[IndexAttribute] : 0.0f;

				IndexAttribute = DataAnimationPartsRuntime.DataPlain.PositionTexture.IndexGetValue(out FrameNoOrigin, FrameNo);
				Vector2 TextureOffset = (0 <= IndexAttribute) ? DataAnimationPartsRuntime.DataPlain.PositionTexture.ListValue[IndexAttribute] : Vector2.zero;

				Vector3 Translation = new Vector3(	((RectangleCell.xMin + PivotTexture.x) / SizeTextureOriginal.x) + TextureOffset.x,
													((SizeTextureOriginal.y - (RectangleCell.yMin + PivotTexture.y)) / SizeTextureOriginal.y) - TextureOffset.y,
													0.0f
												);
				Vector3 Scaling = new Vector3(	(RectangleCell.width / SizeTextureOriginal.x) * RateScaleTexture.x,
												(RectangleCell.height / SizeTextureOriginal.y) * RateScaleTexture.y,
												1.0f
											);
				Quaternion Rotation = Quaternion.Euler(0.0f, 0.0f, -Rotate);
				MatrixTexture = Matrix4x4.TRS(Translation, Rotation, Scaling);
				if(true == FlagHasMesh)
				{
					for(int i=0; i<CountVertexData; i++)
					{	/* Memo: "ArrayUVMappingUV0_Triangle4" of the data up to the "VertexNo.TERMINATOR2"-th elements are same as those of "ArrayUVMappingUV0_Triangle2". */
						UV0Mesh.ListValue[FrameNo].UV[i] = MatrixTexture.MultiplyPoint3x4(Library_SpriteStudio.ArrayUVMappingUV0_Triangle4[i]);
					}
				}

				/* ColorBlend & RateOpacity */
				if(true == FlagHasMesh)
				{
					IndexAttribute = DataAnimationPartsRuntime.RateOpacity.IndexGetValue(out FrameNoOrigin, FrameNo);
					float RateOpacity = (0 <= IndexAttribute) ? DataAnimationPartsRuntime.RateOpacity.ListValue[IndexAttribute] : 1.0f;

					Vector2 DataUV2 = new Vector2(	RateOpacity,
													(float)Library_SpriteStudio.KindColorOperation.NON + 0.01f	/* "+0.01f" for Rounding-off-Error */
												);

					IndexAttribute = DataAnimationPartsRuntime.DataPlain.ColorBlend.IndexGetValue(out FrameNoOrigin, FrameNo);
					if(0 <= IndexAttribute)
					{
						Library_SpriteStudio.Data.AttributeColorBlend DataColorBlend = DataAnimationPartsRuntime.DataPlain.ColorBlend.ListValue[IndexAttribute];
						DataUV2.y = (float)DataColorBlend.Operation + 0.01f;	/* "+0.01f" for Rounding-off-Error */
						if(Library_SpriteStudio.KindColorBound.NON != DataColorBlend.Bound)
						{
							for(int i=0; i<(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2; i++)
							{
								ColorBlendMesh.ListValue[FrameNo].UV[i] = DataUV2;
								ColorBlendMesh.ListValue[FrameNo].UV[i].x *= DataColorBlend.RatePixelAlpha[i];
								ColorBlendMesh.ListValue[FrameNo].ColorOverlay[i] = DataColorBlend.VertexColor[i];
							}
							goto UpdateMesh_ColorBlend_End;
						}
					}

					/* MEMO: Trapping "No-Operation" */
					for(int i=0; i<(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2; i++)
					{
						ColorBlendMesh.ListValue[FrameNo].UV[i] = DataUV2;
						ColorBlendMesh.ListValue[FrameNo].ColorOverlay[i] = Color.white;
					}

				UpdateMesh_ColorBlend_End:;
				}

				/* Mesh Coordinate */
				if(true == FlagHasMesh)
				{
					if((int)Library_SpriteStudio.KindVertexNo.TERMINATOR4 == CountVertexData)	/* Vertex-Coordinate */
					{	/* 4-Triangles Mesh */
						/* Get Color Blend (Center) */
						ColorBlendMesh.ListValue[FrameNo].UV[(int)Library_SpriteStudio.KindVertexNo.C] = ColorBlendMesh.ListValue[FrameNo].UV[0];
						Color DataColor = ColorBlendMesh.ListValue[FrameNo].ColorOverlay[0];
						DataColor += ColorBlendMesh.ListValue[FrameNo].ColorOverlay[1];
						DataColor += ColorBlendMesh.ListValue[FrameNo].ColorOverlay[2];
						DataColor += ColorBlendMesh.ListValue[FrameNo].ColorOverlay[3];
						DataColor *= 0.25f;
						ColorBlendMesh.ListValue[FrameNo].ColorOverlay[(int)Library_SpriteStudio.KindVertexNo.C] = DataColor;

						Vector2 DataUV2 = ColorBlendMesh.ListValue[FrameNo].UV[0];
						DataUV2 += ColorBlendMesh.ListValue[FrameNo].UV[1];
						DataUV2 += ColorBlendMesh.ListValue[FrameNo].UV[2];
						DataUV2 += ColorBlendMesh.ListValue[FrameNo].UV[3];
						DataUV2 *= 0.25f;
						ColorBlendMesh.ListValue[FrameNo].UV[(int)Library_SpriteStudio.KindVertexNo.C] = DataUV2;

						/* Get Coordinates */
						/* MEMO: No Check "AnimationDataVertexCorrection.Length", 'cause 4-Triangles-Mesh necessarily has "AnimationDataVertexCorrection" */
						float Left = (-PivotMesh.x) * RateScaleMesh.x;
						float Right = (RectangleCell.width - PivotMesh.x) * RateScaleMesh.x;
						float Top = -((-PivotMesh.y) * RateScaleMesh.y);	/* * -1.0f ... Y-Axis Inverse */
						float Bottom = -((RectangleCell.height - PivotMesh.y) * RateScaleMesh.y);	/* * -1.0f ... Y-Axis Inverse */
						IndexAttribute = DataAnimationPartsRuntime.DataPlain.VertexCorrection.IndexGetValue(out FrameNoOrigin, FrameNo);
						Library_SpriteStudio.Data.AttributeVertexCorrection VertexCorrection = (0 <= IndexAttribute) ? DataAnimationPartsRuntime.DataPlain.VertexCorrection.ListValue[IndexAttribute] : Library_SpriteStudio.Data.DummyVertexCorrection;

						CoordinateMesh.ListValue[FrameNo].Coordinate[(int)Library_SpriteStudio.KindVertexNo.LU] = new Vector3(	Left + VertexCorrection.Coordinate[Library_SpriteStudio.VertexCollectionOrderVertex[IndexVertexCollectionTable, (int)Library_SpriteStudio.KindVertexNo.LU]].x,
																																Top + VertexCorrection.Coordinate[Library_SpriteStudio.VertexCollectionOrderVertex[IndexVertexCollectionTable, (int)Library_SpriteStudio.KindVertexNo.LU]].y,
																																0.0f
																															);
						CoordinateMesh.ListValue[FrameNo].Coordinate[(int)Library_SpriteStudio.KindVertexNo.RU] = new Vector3(	Right + VertexCorrection.Coordinate[Library_SpriteStudio.VertexCollectionOrderVertex[IndexVertexCollectionTable, (int)Library_SpriteStudio.KindVertexNo.RU]].x,
																																Top + VertexCorrection.Coordinate[Library_SpriteStudio.VertexCollectionOrderVertex[IndexVertexCollectionTable, (int)Library_SpriteStudio.KindVertexNo.RU]].y,
																																0.0f
																															);
						CoordinateMesh.ListValue[FrameNo].Coordinate[(int)Library_SpriteStudio.KindVertexNo.RD] = new Vector3(	Right + VertexCorrection.Coordinate[Library_SpriteStudio.VertexCollectionOrderVertex[IndexVertexCollectionTable, (int)Library_SpriteStudio.KindVertexNo.RD]].x,
																																Bottom + VertexCorrection.Coordinate[Library_SpriteStudio.VertexCollectionOrderVertex[IndexVertexCollectionTable, (int)Library_SpriteStudio.KindVertexNo.RD]].y,
																																0.0f
																															);
						CoordinateMesh.ListValue[FrameNo].Coordinate[(int)Library_SpriteStudio.KindVertexNo.LD] = new Vector3(	Left + VertexCorrection.Coordinate[Library_SpriteStudio.VertexCollectionOrderVertex[IndexVertexCollectionTable, (int)Library_SpriteStudio.KindVertexNo.LD]].x,
																																Bottom + VertexCorrection.Coordinate[Library_SpriteStudio.VertexCollectionOrderVertex[IndexVertexCollectionTable, (int)Library_SpriteStudio.KindVertexNo.LD]].y,
																																0.0f
																															);
						Vector3 CoordinateLURU = (CoordinateMesh.ListValue[FrameNo].Coordinate[(int)Library_SpriteStudio.KindVertexNo.LU] + CoordinateMesh.ListValue[FrameNo].Coordinate[(int)Library_SpriteStudio.KindVertexNo.RU]) * 0.5f;
						Vector3 CoordinateLULD = (CoordinateMesh.ListValue[FrameNo].Coordinate[(int)Library_SpriteStudio.KindVertexNo.LU] + CoordinateMesh.ListValue[FrameNo].Coordinate[(int)Library_SpriteStudio.KindVertexNo.LD]) * 0.5f;
						Vector3 CoordinateLDRD = (CoordinateMesh.ListValue[FrameNo].Coordinate[(int)Library_SpriteStudio.KindVertexNo.LD] + CoordinateMesh.ListValue[FrameNo].Coordinate[(int)Library_SpriteStudio.KindVertexNo.RD]) * 0.5f;
						Vector3 CoordinateRURD = (CoordinateMesh.ListValue[FrameNo].Coordinate[(int)Library_SpriteStudio.KindVertexNo.RU] + CoordinateMesh.ListValue[FrameNo].Coordinate[(int)Library_SpriteStudio.KindVertexNo.RD]) * 0.5f;
						Library_SpriteStudio.Miscellaneousness.Math.CoordinateGetDiagonalIntersection(	out CoordinateMesh.ListValue[FrameNo].Coordinate[(int)Library_SpriteStudio.KindVertexNo.C],
																										ref CoordinateLURU,
																										ref CoordinateRURD,
																										ref CoordinateLULD,
																										ref CoordinateLDRD
																									);
					}
					else
					{	/* 2-Triangles Mesh */
						/* Get Coordinates */
						float Left = (-PivotMesh.x) * RateScaleMesh.x;
						float Right = (SizePixelMesh.x - PivotMesh.x) * RateScaleMesh.x;
						float Top = (-PivotMesh.y) * RateScaleMesh.y;
						float Bottom = (SizePixelMesh.y - PivotMesh.y)* RateScaleMesh.y;

						CoordinateMesh.ListValue[FrameNo].Coordinate[(int)Library_SpriteStudio.KindVertexNo.LU] = new Vector3(Left, -Top, 0.0f);
						CoordinateMesh.ListValue[FrameNo].Coordinate[(int)Library_SpriteStudio.KindVertexNo.RU] = new Vector3(Right, -Top, 0.0f);
						CoordinateMesh.ListValue[FrameNo].Coordinate[(int)Library_SpriteStudio.KindVertexNo.RD] = new Vector3(Right, -Bottom, 0.0f);
						CoordinateMesh.ListValue[FrameNo].Coordinate[(int)Library_SpriteStudio.KindVertexNo.LD] = new Vector3(Left, -Bottom, 0.0f);
					}
				}
			}

			/* Data-Exchange */
			DataAnimationPartsRuntime.DataFix = new Library_SpriteStudio.Data.AnimationPartsPartsFix();
			DataAnimationPartsRuntime.DataFix.IndexCellMapMesh = IndexCellMapFix;
			DataAnimationPartsRuntime.DataFix.CoordinateMesh = CoordinateMesh;
			DataAnimationPartsRuntime.DataFix.ColorBlendMesh = ColorBlendMesh;
			DataAnimationPartsRuntime.DataFix.UV0Mesh = UV0Mesh;
			DataAnimationPartsRuntime.DataFix.SizeCollision = SizeCollision;
			DataAnimationPartsRuntime.DataFix.PivotCollision = PivotCollision;
			DataAnimationPartsRuntime.DataFix.RadiusCollision = DataAnimationPartsRuntime.DataPlain.RadiusCollision;

			DataAnimationPartsRuntime.SizeForce = null;	/* Delete */
			DataAnimationPartsRuntime.DataPlain = null;	/* Delete */
			DataAnimationPartsRuntime.KindFormat = Library_SpriteStudio.KindFormat.FIX;	/* Format Change */

			return(true);
		}
		private static void MeshRecalcSizeAndPivot(	ref Vector2 Pivot,
													ref Vector2 Size,
													ref Vector2 RateScale,
													int FrameNo,
													Library_SpriteStudio.Data.AnimationParts DataAnimationPartsRuntime
												)
		{
			int FrameNoOrigin;
			int IndexAttribute;
			IndexAttribute = DataAnimationPartsRuntime.DataPlain.OffsetPivot.IndexGetValue(out FrameNoOrigin, FrameNo);
			Vector2 PivotOffset = (0 <= IndexAttribute) ? DataAnimationPartsRuntime.DataPlain.OffsetPivot.ListValue[IndexAttribute] : Vector2.zero;
			Pivot.x += (Size.x * PivotOffset.x) * RateScale.x;
			Pivot.y -= (Size.y * PivotOffset.y) * RateScale.y;

			/* Arbitrate Anchor-Size */
			IndexAttribute = DataAnimationPartsRuntime.SizeForce.IndexGetValue(out FrameNoOrigin, FrameNo);
			if(0 <= IndexAttribute)
			{
				float RatePivot;
				Vector2 SizeForce = DataAnimationPartsRuntime.SizeForce.ListValue[IndexAttribute];
				if(0.0f <= SizeForce.x)
				{
					RatePivot = Pivot.x / Size.x;
					Size.x = SizeForce.x;
					Pivot.x = SizeForce.x * RatePivot;
				}
				if(0.0f <= SizeForce.y)
				{
					RatePivot = Pivot.y / Size.y;
					Size.y = SizeForce.y;
					Pivot.y = SizeForce.y * RatePivot;
				}
			}
		}

		internal static bool PrefabCreateSSEE(	ref SettingImport DataSettingImport,
												LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
												int Index,
												string NameBaseAssetPath,
												string FileName
											)
		{
			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSEE InformationEffectSet = InformationProject.ListInformationSSEE[Index];
			switch(InformationEffectSet.VersionCode)
			{
				case ParseOPSS.KindVersionSSEE.VERSION_010002:
					/* MEMO: SS5.6 Unsupported */
					/* MEMO: SS5.7-Underdevelopment */
					break;

				case ParseOPSS.KindVersionSSEE.VERSION_010100:
					return(PrefabCreateSSEEMain(ref DataSettingImport, InformationProject, Index, NameBaseAssetPath, FileName));

				default:
					break;
			}
			return(false);
		}
		internal struct WorkAreaPrefabCreateSSEE
		{
			internal int IndexParts;
			internal int IndexEmitter;
			internal int IndexPartsHasIndexChild;
			internal int IndexPartsBody;
		}
		internal static bool PrefabCreateSSEEMain(	ref SettingImport DataSettingImport,
													LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
													int Index,
													string NameBaseAssetPath,
													string FileName
												)
		{	/* MEMO: SS5.7 */
//			string MessageError = "";
			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSEE InformationEffectSet = InformationProject.ListInformationSSEE[Index];
			Script_SpriteStudio_DataEffect DataEffectSet = null;
			int Count = -1;
			int IndexTemp = -1;

			/* Determine Destination */
			bool FlagOverwrite = false;
			if(null == InformationEffectSet.PrefabDataEffect)
			{
				/* Create Destination-Folder */
				LibraryEditor_SpriteStudio.Utility.File.AssetFolderCreate(NameBaseAssetPath, NamePathSubImportEffect);

				/* Create New Asset */
				DataEffectSet = ScriptableObject.CreateInstance<Script_SpriteStudio_DataEffect>();
				AssetDatabase.CreateAsset(DataEffectSet, InformationEffectSet.NamePrefabDataEffect);
				InformationEffectSet.PrefabDataEffect = AssetDatabase.LoadAssetAtPath(InformationEffectSet.NamePrefabDataEffect, typeof(ScriptableObject)) as ScriptableObject;

				FlagOverwrite = true;
			}
			else
			{
				/* Confirm Overwrite */
				FlagOverwrite = LibraryEditor_SpriteStudio.Utility.File.OverwriteConfirmDialogue(	InformationEffectSet.NamePrefabDataEffect,
																									"Fixed-Effect Datas",
																									DataSettingImport.FlagConfirmOverWrite,
																									ref DataSettingImport.FlagConfirmOverWriteDataEffect
																								);
			}
			DataEffectSet = InformationEffectSet.PrefabDataEffect as Script_SpriteStudio_DataEffect;
			if(false == FlagOverwrite)
			{
				InformationEffectSet.ListDataPartsRuntime = DataEffectSet.ListDataParts;
				InformationEffectSet.ListDataEmitterRuntime = DataEffectSet.ListDataEmitter;
				goto Convert_PrefabCreateSSEEMain_ConvertGameObject;
			}

			/* Optimize Parts */
			Count = InformationEffectSet.ListParts.Length;
			WorkAreaPrefabCreateSSEE[] InformationOptimize = new WorkAreaPrefabCreateSSEE[Count];
			int CountPartsOptimize = 0;
			int CountEmitterOptimize = 0;
			for(int i=0; i<Count; i++)
			{	/* Buffer Clear */
				InformationOptimize[i].IndexParts = -1;
				InformationOptimize[i].IndexEmitter = -1;
				InformationOptimize[i].IndexPartsHasIndexChild = -1;
				InformationOptimize[i].IndexPartsBody = -1;
			}
			for(int i=0; i<Count; i++)
			{
				switch(InformationEffectSet.ListParts[i].Kind)
				{
					case Library_SpriteStudio.KindPartsEffect.ROOT:
						InformationOptimize[i].IndexParts = CountPartsOptimize;
						InformationOptimize[i].IndexEmitter = -1;
						InformationOptimize[i].IndexPartsHasIndexChild = CountPartsOptimize;	// InformationOptimize[i].IndexParts;
						InformationOptimize[i].IndexPartsBody = i;

						CountPartsOptimize++;
						break;

					case Library_SpriteStudio.KindPartsEffect.EMITTER:
						InformationOptimize[i].IndexParts = CountPartsOptimize;
						InformationOptimize[i].IndexEmitter = CountEmitterOptimize;
//						InformationOptimize[i].IndexPartsHasIndexChild =
						InformationOptimize[i].IndexPartsBody = CountPartsOptimize;

						CountPartsOptimize++;
						CountEmitterOptimize++;
						break;

					case Library_SpriteStudio.KindPartsEffect.PARTICLE:
//						InformationOptimize[i].IndexParts = -1;
//						InformationOptimize[i].IndexEmitter = -1;
						IndexTemp = InformationEffectSet.ListParts[i].IDParent;
						if(0 <= IndexTemp)
						{
							InformationOptimize[IndexTemp].IndexPartsHasIndexChild = i;
						}
						InformationOptimize[i].IndexPartsBody = InformationOptimize[IndexTemp].IndexParts;
						break;

					default:
						continue;
				}
			}

			/* New Parts-Data Buffer */
			InformationEffectSet.ListDataPartsRuntime = new Library_SpriteStudio.Data.PartsEffect[CountPartsOptimize];
			for(int i=0; i<CountPartsOptimize; i++)
			{
				InformationEffectSet.ListDataPartsRuntime[i] = new Library_SpriteStudio.Data.PartsEffect();
				InformationEffectSet.ListDataPartsRuntime[i].CleanUp();
			}
			InformationEffectSet.ListDataEmitterRuntime = new Library_SpriteStudio.Data.EmitterEffect[CountEmitterOptimize];
			for(int i=0; i<CountEmitterOptimize; i++)
			{
				InformationEffectSet.ListDataEmitterRuntime[i] = new Library_SpriteStudio.Data.EmitterEffect();
				InformationEffectSet.ListDataEmitterRuntime[i].CleanUp();
			}

			/* "Parts" & "Emitter" Set */
			int[] ArrayListIntDummy = new int[0];
			int IndexParts = -1;
			int IndexEmitter = -1;
			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSEEParts DataPartsSource = null;
			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSEEParts DataPartsSourceList = null;
			Library_SpriteStudio.Data.PartsEffect DataPartsRuntime = null;
			Library_SpriteStudio.Data.EmitterEffect DataEmitterRuntime = null;
			for(int i=0; i<InformationEffectSet.ListParts.Length; i++)
			{
				DataPartsSource = InformationEffectSet.ListParts[i];
				IndexParts = InformationOptimize[i].IndexParts;
				if(0 <= IndexParts)
				{	/* "Root" or "Emitter" */
					DataPartsRuntime = InformationEffectSet.ListDataPartsRuntime[IndexParts];

					/* Part's ID & Name Set */
					DataPartsRuntime.Name = DataPartsSource.Name;
					DataPartsRuntime.ID = IndexParts;

					/* Parent's ID Set */
					IndexTemp = DataPartsSource.IDParent;
					if(0 <= IndexTemp)
					{	/* Has Parent */
						int IndexTempOptimize = InformationOptimize[IndexTemp].IndexPartsBody;	/* Parent Index (Optimized) */
						DataPartsRuntime.IDParent = IndexTempOptimize;
					}
					else
					{	/* Has no Parent */
						DataPartsRuntime.IDParent = -1;
					}

					/* Children's ID List Set */
					IndexTemp = InformationOptimize[i].IndexPartsHasIndexChild;
					if(0 <= IndexTemp)
					{
						DataPartsSourceList = InformationEffectSet.ListParts[IndexTemp];
						if(null != DataPartsSourceList.ListIndexPartsChild)
						{
							int CountChild = DataPartsSourceList.ListIndexPartsChild.Count;
							DataPartsRuntime.ListIDChild = new int[CountChild];
							for(int j=0; j<CountChild; j++)
							{
								IndexTemp = DataPartsSourceList.ListIndexPartsChild[j];
								DataPartsRuntime.ListIDChild[j] = InformationOptimize[IndexTemp].IndexPartsBody;
							}
						}
						else
						{
							DataPartsRuntime.ListIDChild = ArrayListIntDummy;
						}
					}

					/* Part's Kind & Emitter-Index Set */
					DataPartsRuntime.Kind = DataPartsSource.Kind;
					DataPartsRuntime.IndexEmitter = InformationOptimize[i].IndexEmitter;
				}

				IndexEmitter = InformationOptimize[i].IndexEmitter;
				if(0 <= IndexEmitter)
				{
					DataEmitterRuntime = InformationEffectSet.ListDataEmitterRuntime[IndexEmitter];

					/* Cell-Index Get */
					if(0 <= DataPartsSource.Emitter.IndexCellMap)
					{
						DataPartsSource.Emitter.IndexCell = -1;
						string NameCell = DataPartsSource.Emitter.NameCell;
						if(false == string.IsNullOrEmpty(NameCell))
						{
							DataPartsSource.Emitter.IndexCell = InformationProject.ListInformationSSCE[DataPartsSource.Emitter.IndexCellMap].IndexGetCell(NameCell);
						}
					}

					/* Emit-Pattern Create */

					/* Data Copy */
					DataEmitterRuntime.Copy(DataPartsSource.Emitter);
				}
			}

			/* Fairure-Data Filtering */
			if(false == PrefabCreateSSEEMainValidate(ref InformationEffectSet.ListDataPartsRuntime, ref InformationEffectSet.ListDataEmitterRuntime))
			{
				Debug.LogWarning(	"SSEE Warning: Number of Sub-Emitters or Nesting Depth is exceeded the limit "
									+ "(Number: " + ((int)Script_SpriteStudio_RootEffect.Constants.LIMIT_SUBEMITTER_COUNT).ToString()
									+ " / Depth : " + ((int)Script_SpriteStudio_RootEffect.Constants.LIMIT_SUBEMITTER_DEPTH).ToString()
									+ "). : File["
									+ (string)InformationProject.ListNameSSEE[Index] + "] "
									+ " SSPJ[" + InformationProject.NameFileBody + InformationProject.NameFileExtension + "]"
									);
			}

			/* Emitters'-Data Calculation in Advance */
			/* MEMO: This process is unaffected by the setting,"Calculate In Advance" */
			PrefabCreateSSEEMainCalculateInAdvance(	InformationEffectSet.ListDataEmitterRuntime,
													InformationEffectSet.FlagLockSeed,
													InformationEffectSet.Seed
												);

			/* Effect Data Fix */
			DataEffectSet.ListDataParts = InformationEffectSet.ListDataPartsRuntime;
			DataEffectSet.ListDataEmitter = InformationEffectSet.ListDataEmitterRuntime;

			DataEffectSet.FlagData = Script_SpriteStudio_DataEffect.FlagBit.CLEAR;
			DataEffectSet.FlagData |= Script_SpriteStudio_DataEffect.FlagBit.IMPORTED_BY_VER1_4_ORLATER;
			DataEffectSet.FlagData |= (true == InformationEffectSet.FlagLockSeed) ? Script_SpriteStudio_DataEffect.FlagBit.SEEDRANDOM_LOCK : Script_SpriteStudio_DataEffect.FlagBit.CLEAR;
			DataEffectSet.SeedRandom = InformationEffectSet.Seed;
			DataEffectSet.CountFramePerSecond = InformationEffectSet.FramePerSecond;
			DataEffectSet.ScaleLayout = InformationEffectSet.ScaleLayout;
			DataEffectSet.VersionRenderer = InformationEffectSet.VersionRenderer;
			DataEffectSet.CountMaxParticle = 0;

			EditorUtility.SetDirty(DataEffectSet);
			AssetDatabase.SaveAssets();

			/* WorkArea(for ".ssee" importing) Purge */

		Convert_PrefabCreateSSEEMain_ConvertGameObject:;
			/* Construction GameObject-s */
			if(false == PrefabCreateSSEEGameObject(	ref DataSettingImport,
														InformationProject,
														InformationEffectSet,
														DataEffectSet,
														NameBaseAssetPath,
														FileName
													)
				)
			{
				goto Convert_PrefabCreateSSEE_ErrorEnd_NoMessage;
			}

//		Convert_PrefabCreateSSEE_End:;
			return(true);

		Convert_PrefabCreateSSEE_ErrorEnd_NoMessage:;
			return(false);
		}
		internal static bool PrefabCreateSSEEMainValidate(	ref Library_SpriteStudio.Data.PartsEffect[] ListDataParts,
															ref Library_SpriteStudio.Data.EmitterEffect[] ListDataEmitter
															)
		{
			int CountParts = ListDataParts.Length;
//			int CountEmitters = ListDataEmitter.Length;
			bool FlagWarning = false;

			/* WorkArea Initialize */
			List<int> ListIndexParts = new List<int>();
			ListIndexParts.Clear();
			ListIndexParts.Add(0);	/* Root-Parts */
			List<int> ListIndexEmitters = new List<int>();
			ListIndexEmitters.Clear();

			/* All Parts Check */
			/* MEMO: Root-Parts ... Forced-Valid. */
			Library_SpriteStudio.Data.PartsEffect InstanceDataParts = null;
			Library_SpriteStudio.Data.PartsEffect InstanceDataPartsParent = null;
			for(int i=1; i<CountParts; i++)
			{
				InstanceDataParts = ListDataParts[i];

				if(0 <= InstanceDataParts.IDParent)
				{	/* Has Parent */
					InstanceDataPartsParent = ListDataParts[InstanceDataParts.IDParent];

					/* Check Number of Child-Emitters */
					int[] ListIDChildParent = InstanceDataPartsParent.ListIDChild;
					int CountListChildParent = ListIDChildParent.Length;
					int IndexIDListParent = (int)Script_SpriteStudio_RootEffect.Constants.LIMIT_SUBEMITTER_COUNT;	/* for Not-Found */
					for(int j=0; j<CountListChildParent; j++)
					{	/* Search my ID */
						if(i == ListIDChildParent[j])
						{
							IndexIDListParent = j;
							break;
						}
					}
					if((int)Script_SpriteStudio_RootEffect.Constants.LIMIT_SUBEMITTER_COUNT <= IndexIDListParent)
					{
						FlagWarning |= true;	/* Invalid Emitter */
						continue;
					}

					/* Check Depth */
					int CountGeneration = 0;
					while(0 <= InstanceDataPartsParent.IDParent)
					{
						CountGeneration++;
						InstanceDataPartsParent = ListDataParts[InstanceDataPartsParent.IDParent];
					}
					if((int)Script_SpriteStudio_RootEffect.Constants.LIMIT_SUBEMITTER_DEPTH <= CountGeneration)
					{
						FlagWarning |= true;	/* Invalid Emitter */
						continue;
					}
				}
				/* MEMO: Valid-Emitter */
				ListIndexParts.Add(i);
				ListIndexEmitters.Add(InstanceDataParts.IndexEmitter);
			}

			/* Extract only Valid-Data */
			if(true == FlagWarning)
			{
				int CountPartsNew = ListIndexParts.Count;
				int CountEmittersNew = ListIndexEmitters.Count;

				Library_SpriteStudio.Data.PartsEffect InstanceDataPartsNow = null;
				Library_SpriteStudio.Data.PartsEffect[] ListDataPartsNew = new Library_SpriteStudio.Data.PartsEffect[CountPartsNew];
				Library_SpriteStudio.Data.EmitterEffect[] ListDataEmitterNew = new Library_SpriteStudio.Data.EmitterEffect[CountEmittersNew];

				int IndexEmitterNew;
				for(int i=0; i<CountPartsNew; i++)
				{
					InstanceDataPartsNow = ListDataParts[ListIndexParts[i]];

					/* Renumber Emitter-Index */
					IndexEmitterNew = ListIndexEmitters.BinarySearch(InstanceDataPartsNow.IndexEmitter);
					InstanceDataPartsNow.IndexEmitter = IndexEmitterNew;

					/* Relisting Child-Parts */
					int CountChild = InstanceDataPartsNow.ListIDChild.Length;
					CountChild = ((int)Script_SpriteStudio_RootEffect.Constants.LIMIT_SUBEMITTER_COUNT < CountChild) ? (int)Script_SpriteStudio_RootEffect.Constants.LIMIT_SUBEMITTER_COUNT : CountChild;
					int[] ListIDChildNew = new int[CountChild];
					for(int j=0; j<CountChild; j++)
					{
						ListIDChildNew[j] = ListIndexParts.BinarySearch(InstanceDataPartsNow.ListIDChild[j]);
					}
					InstanceDataPartsNow.ListIDChild = ListIDChildNew;

					/* Set to New-List */
					ListDataPartsNew[i] = InstanceDataPartsNow;
				}

				for(int i=0; i<CountEmittersNew; i++)
				{
					/* Set to New-List */
					ListDataEmitterNew[i] = ListDataEmitter[ListIndexEmitters[i]];
				}

				/* Replace List */
				ListDataParts = ListDataPartsNew;
				ListDataEmitter = ListDataEmitterNew;
			}

			return(!FlagWarning);
		}
		internal static void PrefabCreateSSEEMainCalculateInAdvance(	Library_SpriteStudio.Data.EmitterEffect[] ListDataEmitter,
																		bool FlagLockSeedEffect,
																		int SeedRandomEffect
																)
		{
			int Count = ListDataEmitter.Length;
			Library_SpriteStudio.Data.EmitterEffect InstanceDataEmitter = null;
			bool FlagLockSeed = false;
			uint SeedRandom = 0;

			Library_SpriteStudio.Utility.Random.Generator InstanceRandom = Script_SpriteStudio_RootEffect.InstanceCreateRandom();
			Library_SpriteStudio.Data.EmitterEffect.PatternEmit[] DataTablePatternEmit = null;
			long[] DataTableSeedParticle = null;

			for(int i=0; i<Count; i++)
			{
				InstanceDataEmitter = ListDataEmitter[i];

				/* Fixed Random-Seed */
				FlagLockSeed = false;
				if(0 != (InstanceDataEmitter.FlagData & Library_SpriteStudio.Data.EmitterEffect.FlagBit.SEEDRANDOM))
				{	/* Seed Overwrite */
					SeedRandom = (uint)InstanceDataEmitter.SeedRandom + (uint)Library_SpriteStudio.Data.EmitterEffect.Constant.SEED_MAGIC;
					FlagLockSeed = true;
				}
				else
				{
					if(true == FlagLockSeedEffect)
					{	/* Seed Locked */
						/* MEMO: Overwritten to the Effect's Seed. */
						SeedRandom = ((uint)SeedRandomEffect + 1) * (uint)Library_SpriteStudio.Data.EmitterEffect.Constant.SEED_MAGIC;
						FlagLockSeed = true;
					}
				}

				/* Calcurate Table-Offset-Pattern */
				Library_SpriteStudio.Data.EmitterEffect.TableGetOffset( ref InstanceDataEmitter.TablePatternOffset,
																		InstanceDataEmitter
																	);

				/* Set to Data */
				if(true == FlagLockSeed)
				{	/* Fixed Data */
					/* Calcurate Table-Datas */
					Library_SpriteStudio.Data.EmitterEffect.TableGet(	ref DataTablePatternEmit,
																		ref DataTableSeedParticle,
																		InstanceDataEmitter,
																		InstanceRandom,
																		SeedRandom
																	);

					InstanceDataEmitter.TablePatternEmit = DataTablePatternEmit;
					InstanceDataEmitter.TableSeedParticle = DataTableSeedParticle;
				}
				else
				{	/* Calculate on runtime */
					InstanceDataEmitter.TablePatternEmit = null;
					InstanceDataEmitter.TableSeedParticle = null;
				}
			}
		}
		internal static bool PrefabCreateSSEEGameObject(	ref SettingImport DataSettingImport,
															LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
															LibraryEditor_SpriteStudio.ParseOPSS.InformationSSEE InformationEffectSet,
															Script_SpriteStudio_DataEffect DataEffectSet,
															string NameBaseAssetPath,
															string FileName
														)
		{
//			string MessageError = "";
			/* Determine Destination */
			bool FlagNewCreate = false;
			if(null == InformationEffectSet.PrefabGameObject)
			{
				/* Create Destination-Folder */
				LibraryEditor_SpriteStudio.Utility.File.AssetFolderCreate(NameBaseAssetPath, NamePathSubImportPrefabEffect);

				/* Create Prefab */
				InformationEffectSet.PrefabGameObject = PrefabUtility.CreateEmptyPrefab(InformationEffectSet.NamePrefabGameObject);

				FlagNewCreate = true;
			}
			else
			{
				/* Confirm Overwrite */
				if(false == LibraryEditor_SpriteStudio.Utility.File.OverwriteConfirmDialogue(	InformationEffectSet.NamePrefabGameObject,
																								"Root(Effect)-Prefabs",
																								DataSettingImport.FlagConfirmOverWrite,
																								ref DataSettingImport.FlagConfirmOverWriteRootEffect
																							)
					)
				{
					return(true);
				}

				FlagNewCreate = false;
			}

			/* Create GameObject (Root-Parts) */
			GameObject InstanceGameObjectRoot = Library_SpriteStudio.Miscellaneousness.Asset.GameObjectCreate(InformationEffectSet.Name, false, null);
			if(null == InstanceGameObjectRoot)
			{
				goto Convert_PrefabCreateSSEEGameObject_ErrorEnd;
			}
			Script_SpriteStudio_RootEffect InstanceRootEffect = InstanceGameObjectRoot.AddComponent<Script_SpriteStudio_RootEffect>();

			/* Datas Set */
			InstanceRootEffect.DataCellMap = (Script_SpriteStudio_DataCell)InformationProject.PrefabCell;
			InstanceRootEffect.DataEffect = (Script_SpriteStudio_DataEffect)InformationEffectSet.PrefabDataEffect;
			InstanceRootEffect.TableMaterial = InformationProject.TableMaterialEffect;

			/* Initial-Setting Parts-Root */
			if((true == FlagNewCreate) || (false == DataSettingImport.FlagDataTakeOverSettingPrefab))
			{	/* Parameters Set */
				InstanceRootEffect.CountLimitParticleInitial = 0;
				InstanceRootEffect.RateSpeed = 1.0f;
			}
			else
			{	/* Parameters Inherit */
				GameObject InstanceGameObjectRootOld = (GameObject)InformationEffectSet.PrefabGameObject;
				Script_SpriteStudio_RootEffect InstanceRootOld = InstanceGameObjectRootOld.GetComponent<Script_SpriteStudio_RootEffect>();

				InstanceRootEffect.CountLimitParticleInitial = InstanceRootOld.CountLimitParticleInitial;
				InstanceRootEffect.RateSpeed = InstanceRootOld.RateSpeed;
			}
			InstanceRootEffect.TableCreateBlendOffset();
			InstanceRootEffect.IndexMaterialBlendOffset[(int)Library_SpriteStudio.KindColorOperationEffect.MIX - 1] = 0;
			InstanceRootEffect.IndexMaterialBlendOffset[(int)Library_SpriteStudio.KindColorOperationEffect.ADD - 1] = (int)Library_SpriteStudio.KindColorOperationEffect.ADD2 - (int)Library_SpriteStudio.KindColorOperationEffect.ADD;

			Library_SpriteStudio.Miscellaneousness.Asset.ActiveSetGameObject(InstanceGameObjectRoot, true);

			/* Fixing Created Assets */
			InformationEffectSet.PrefabGameObject = PrefabUtility.ReplacePrefab(	InstanceGameObjectRoot,
																					InformationEffectSet.PrefabGameObject,
																					OptionPrefabOverwrite
																				);
			AssetDatabase.SaveAssets();

			/* Store Prefab */
			UnityEngine.Object.DestroyImmediate(InstanceGameObjectRoot);
			InstanceGameObjectRoot = null;

			return(true);

		Convert_PrefabCreateSSEEGameObject_ErrorEnd:;
			return(false);
		}

		internal static bool PrefabCreateControlObjectSSAE(	ref SettingImport DataSettingImport,
															LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject,
															int Index,
															string NameBaseAssetPath,
															string FileName
														)
		{
			string MessageError = "";
			LibraryEditor_SpriteStudio.ParseOPSS.InformationSSAE InformationAnimationSet = InformationProject.ListInformationSSAE[Index];

			/* Asset Path Generate */
//			string NameObjectControl = InformationAnimationSet.NameFileBody + LibraryEditor_SpriteStudio.NameSuffixPrefabControl;
			string NameObjectControl = Utility.File.NameAssetBodyGet(	InformationAnimationSet.Name,	/* InformationAnimationSet.NameFileBody, */
																		null,	/* NamePrefixPrefab */
																		InformationProject.NameFileBody,
																		DataSettingImport.FlagNameDataAttachSpecificToPrefab,
																		ref DataSettingImport
																	) + LibraryEditor_SpriteStudio.NameSuffixPrefabControl;
			string NamePathAssetControl = NameBaseAssetPath + "/"
											+ NameObjectControl + LibraryEditor_SpriteStudio.NameExtensionPrefab;

			/* Exsisting Check */
			GameObject InstanceGameObject = null;
			Script_SpriteStudio_ControlPrefab ScriptControlPrefab = null;
			UnityEngine.Object ObjectControl = AssetDatabase.LoadAssetAtPath(NamePathAssetControl, typeof(GameObject));
			if(null == ObjectControl)
			{	/* Create New */
				if(true == DataSettingImport.FlagAttachControlGameObject)
				{
					goto PrefabCreateControlObjectSSAE_ControlCreate;
				}
			}
			else
			{	/* Exist */
				/* "Control"-Object Get */
				InstanceGameObject = (GameObject)ObjectControl;
				if(null == InstanceGameObject)
				{
					MessageError = "Getting Failure Existing Control-GameObject";
					goto PrefabCreateControlObjectSSAE_ErrorEnd;
				}

				/* Get Script & Link Prefab */
				ScriptControlPrefab = InstanceGameObject.GetComponent<Script_SpriteStudio_ControlPrefab>();
				if(null == ScriptControlPrefab)
				{
					goto PrefabCreateControlObjectSSAE_ControlCreate;
				}
				ScriptControlPrefab.PrefabUnderControl = InformationAnimationSet.PrefabGameObject;

				/* Fixing Changed Assets */
				AssetDatabase.SaveAssets();
			}

		PrefabCreateControlObjectSSAE_End:;
			return(true);

		PrefabCreateControlObjectSSAE_ErrorEnd:;
			Debug.LogError(	"Control-Object Generate: " + MessageError
							+ " SSPJ[" + FileName + "]");
			return(false);

		PrefabCreateControlObjectSSAE_ControlCreate:;
			/* "Control"-Object Create */
			InstanceGameObject = Library_SpriteStudio.Miscellaneousness.Asset.GameObjectCreate(NameObjectControl, false, null);
			if(null == InstanceGameObject)
			{
				MessageError = "Creating Failure New Control-GameObject";
				goto PrefabCreateControlObjectSSAE_ErrorEnd;
			}

			/* Attach Script & Link Prefab */
			ScriptControlPrefab = InstanceGameObject.AddComponent<Script_SpriteStudio_ControlPrefab>();
			ScriptControlPrefab.PrefabUnderControl = InformationAnimationSet.PrefabGameObject;

			/* Create Prefab */
			Library_SpriteStudio.Miscellaneousness.Asset.ActiveSetGameObject(InstanceGameObject, true);

			UnityEngine.Object PrefabControl = PrefabUtility.CreateEmptyPrefab(NamePathAssetControl);
			PrefabUtility.ReplacePrefab(	InstanceGameObject,
											PrefabControl,
											OptionPrefabOverwrite
										);

			/* Fixing Created Assets */
			AssetDatabase.SaveAssets();

			/* Deleting Tempolary-Instance */
			UnityEngine.Object.DestroyImmediate(InstanceGameObject);

			goto PrefabCreateControlObjectSSAE_End;
		}
	}

	internal static class KeyFrame
	{
		internal enum KindAttribute
		{
			NON = -1,			/* Error-Code */

			POSITION_X = 0,		/* POSX : float */
			POSITION_Y,			/* POSY : float */
			POSITION_Z,			/* POSZ : float */
			ROTATION_X,			/* ROTX : float */
			ROTATION_Y,			/* ROTY : float */
			ROTATION_Z,			/* ROTZ : float */
			SCALING_X,			/* SCLX : float */
			SCALING_Y,			/* SCLY : float */

			RATE_OPACITY,		/* ALPH : float */
			PRIORITY,			/* PRIO : float */

			FLIP_X,				/* FLPH : bool */
			FLIP_Y,				/* FLPV : bool */
			HIDE,				/* HIDE : bool */

			ANCHOR_POSITION_X,	/* ANCX : float */
			ANCHOR_POSITION_Y,	/* ANCY : float */
			SIZE_FORCE_X,		/* SIZX : float */
			SIZE_FORCE_Y,		/* SIZY : float */

			TEXTURE_POSITION_X,	/* UVTX / IMGX : float */
			TEXTURE_POSITION_Y,	/* UVTY / IMGY : float */
			TEXTURE_ROTATION,	/* UVRZ : float */
			TEXTURE_SCALING_X,	/* UVSX / IMGW : float */
			TEXTURE_SCALING_Y,	/* UVSY / IMGH : float */
			TEXTURE_FLIP_X,		/* IFLX : float */
			TEXTURE_FLIP_Y,		/* IFLY : float */

			PIVOT_OFFSET_X,		/* ORFX : float */
			PIVOT_OFFSET_Y,		/* ORFY : float */

			COLLISION_RADIUS,	/* BNDR : float */

			USERDATA,			/* USER : UserData */
			CELL,				/* CELL : Cell */
			COLORBLEND,			/* VCOL : ColorBlend */
			VERTEXCORRECTION,	/* VERT : VertexCorrection */

			INSTANCE,			/* IPRM : Instance */
			EFFECT,				/* EFCT : Effect */

			TERMINATOR
		}

		internal enum KindValue
		{
			NON = -1,	/* Error-Code */

			BOOL = 0,
			INT,
			FLOAT,

			USERDATA,
			CELL,
			COLORBLEND,
			VERTEXCORRECTION,
			INSTANCE,
			EFFECT
		}

		public enum KindPartsIgnoreAttribute
		{
			NON = -1,				/* ERROR-Code */

			NULL = 0,				/* NULL-Parts */
			SPRITE,					/* Sprite-Parts */
			INSTANCE,				/* Instance-Parts */
			EFFECT,					/* Effect-Parts */

			TERMINATOR
		}
		internal readonly static KindPartsIgnoreAttribute[] KindPartsToIgnore = new KindPartsIgnoreAttribute[]
		{
			KindPartsIgnoreAttribute.NULL,		/* ROOT */
			KindPartsIgnoreAttribute.NULL,		/* NULL */
			KindPartsIgnoreAttribute.SPRITE,	/* NORMAL_TRIANGLE2 */
			KindPartsIgnoreAttribute.SPRITE,	/* NORMAL_TRIANGLE4 */
			KindPartsIgnoreAttribute.INSTANCE,	/* INSTANCE */
			KindPartsIgnoreAttribute.EFFECT,	/* EFFECT */
			KindPartsIgnoreAttribute.SPRITE		/* NORMAL */
		};
		internal readonly static KindAttribute[][] ListKindAttributeIgnore = new KindAttribute[(int)KindPartsIgnoreAttribute.TERMINATOR][]
		{
			new KindAttribute[]
			{	/* NULL */
//				KindAttribute.POSITION_X,
//				KindAttribute.POSITION_Y,
//				KindAttribute.POSITION_Z,
//				KindAttribute.ROTATION_X,
//				KindAttribute.ROTATION_Y,
//				KindAttribute.ROTATION_Z,
//				KindAttribute.SCALING_X,
//				KindAttribute.SCALING_Y,

//				KindAttribute.RATE_OPACITY,
//				KindAttribute.PRIORITY,

//				KindAttribute.FLIP_X,
//				KindAttribute.FLIP_Y,
//				KindAttribute.HIDE,

//				KindAttribute.ANCHOR_POSITION_X,
//				KindAttribute.ANCHOR_POSITION_Y,
				KindAttribute.SIZE_FORCE_X,
				KindAttribute.SIZE_FORCE_Y,

				KindAttribute.TEXTURE_POSITION_X,
				KindAttribute.TEXTURE_POSITION_Y,
				KindAttribute.TEXTURE_ROTATION,
				KindAttribute.TEXTURE_SCALING_X,
				KindAttribute.TEXTURE_SCALING_Y,
				KindAttribute.TEXTURE_FLIP_X,
				KindAttribute.TEXTURE_FLIP_Y,

				KindAttribute.PIVOT_OFFSET_X,
				KindAttribute.PIVOT_OFFSET_Y,

//				KindAttribute.COLLISION_RADIUS,

//				KindAttribute.USERDATA,
//				KindAttribute.CELL,
//				KindAttribute.COLORBLEND,
//				KindAttribute.VERTEXCORRECTION,

				KindAttribute.INSTANCE,
				KindAttribute.EFFECT,
			},
			new KindAttribute[]
			{	/* SPRITE */
//				KindAttribute.POSITION_X,
//				KindAttribute.POSITION_Y,
//				KindAttribute.POSITION_Z,
//				KindAttribute.ROTATION_X,
//				KindAttribute.ROTATION_Y,
//				KindAttribute.ROTATION_Z,
//				KindAttribute.SCALING_X,
//				KindAttribute.SCALING_Y,

//				KindAttribute.RATE_OPACITY,
//				KindAttribute.PRIORITY,

//				KindAttribute.FLIP_X,
//				KindAttribute.FLIP_Y,
//				KindAttribute.HIDE,

//				KindAttribute.ANCHOR_POSITION_X,
//				KindAttribute.ANCHOR_POSITION_Y,
//				KindAttribute.SIZE_FORCE_X,
//				KindAttribute.SIZE_FORCE_Y,

//				KindAttribute.TEXTURE_POSITION_X,
//				KindAttribute.TEXTURE_POSITION_Y,
//				KindAttribute.TEXTURE_ROTATION,
//				KindAttribute.TEXTURE_SCALING_X,
//				KindAttribute.TEXTURE_SCALING_Y,
//				KindAttribute.TEXTURE_FLIP_X,
//				KindAttribute.TEXTURE_FLIP_Y,

//				KindAttribute.PIVOT_OFFSET_X,
//				KindAttribute.PIVOT_OFFSET_Y,

//				KindAttribute.COLLISION_RADIUS,

//				KindAttribute.USERDATA,
//				KindAttribute.CELL,
//				KindAttribute.COLORBLEND,
//				KindAttribute.VERTEXCORRECTION,

				KindAttribute.INSTANCE,
				KindAttribute.EFFECT,
			},
			new KindAttribute[]
			{	/* INSTANCE */
//				KindAttribute.POSITION_X,
//				KindAttribute.POSITION_Y,
//				KindAttribute.POSITION_Z,
//				KindAttribute.ROTATION_X,
//				KindAttribute.ROTATION_Y,
//				KindAttribute.ROTATION_Z,
//				KindAttribute.SCALING_X,
//				KindAttribute.SCALING_Y,

//				KindAttribute.RATE_OPACITY,
//				KindAttribute.PRIORITY,

				KindAttribute.FLIP_X,
				KindAttribute.FLIP_Y,
//				KindAttribute.HIDE,

				KindAttribute.ANCHOR_POSITION_X,
				KindAttribute.ANCHOR_POSITION_Y,
				KindAttribute.SIZE_FORCE_X,
				KindAttribute.SIZE_FORCE_Y,

				KindAttribute.TEXTURE_POSITION_X,
				KindAttribute.TEXTURE_POSITION_Y,
				KindAttribute.TEXTURE_ROTATION,
				KindAttribute.TEXTURE_SCALING_X,
				KindAttribute.TEXTURE_SCALING_Y,
				KindAttribute.TEXTURE_FLIP_X,
				KindAttribute.TEXTURE_FLIP_Y,

				KindAttribute.PIVOT_OFFSET_X,
				KindAttribute.PIVOT_OFFSET_Y,

//				KindAttribute.COLLISION_RADIUS,

//				KindAttribute.USERDATA,
				KindAttribute.CELL,
				KindAttribute.COLORBLEND,
				KindAttribute.VERTEXCORRECTION,

//				KindAttribute.INSTANCE,
				KindAttribute.EFFECT,
			},
			new KindAttribute[]
			{	/* EFFECT */
//				KindAttribute.POSITION_X,
//				KindAttribute.POSITION_Y,
//				KindAttribute.POSITION_Z,
//				KindAttribute.ROTATION_X,
//				KindAttribute.ROTATION_Y,
//				KindAttribute.ROTATION_Z,
//				KindAttribute.SCALING_X,
//				KindAttribute.SCALING_Y,

//				KindAttribute.RATE_OPACITY,
//				KindAttribute.PRIORITY,

				KindAttribute.FLIP_X,
				KindAttribute.FLIP_Y,
//				KindAttribute.HIDE,

				KindAttribute.ANCHOR_POSITION_X,
				KindAttribute.ANCHOR_POSITION_Y,
				KindAttribute.SIZE_FORCE_X,
				KindAttribute.SIZE_FORCE_Y,

				KindAttribute.TEXTURE_POSITION_X,
				KindAttribute.TEXTURE_POSITION_Y,
				KindAttribute.TEXTURE_ROTATION,
				KindAttribute.TEXTURE_SCALING_X,
				KindAttribute.TEXTURE_SCALING_Y,
				KindAttribute.TEXTURE_FLIP_X,
				KindAttribute.TEXTURE_FLIP_Y,

				KindAttribute.PIVOT_OFFSET_X,
				KindAttribute.PIVOT_OFFSET_Y,

//				KindAttribute.COLLISION_RADIUS,

//				KindAttribute.USERDATA,
				KindAttribute.CELL,
				KindAttribute.COLORBLEND,
				KindAttribute.VERTEXCORRECTION,

				KindAttribute.INSTANCE,
//				KindAttribute.EFFECT,
			},
		};

		public class Data_Base<_Type>
		{
			/* Frame Values */
			public int FrameNo;
			public _Type Value;

			/* Interpolation-Curve Data */
			public Library_SpriteStudio.KindInterpolation KindCurve;
			public float TimeCurveStart;
			public float ValueCurveStart;
			public float TimeCurveEnd;
			public float ValueCurveEnd;

			public void CleanUp()
			{
				FrameNo = -1;
//				Value =
				KindCurve = Library_SpriteStudio.KindInterpolation.NON;
				TimeCurveStart = 0.0f;
				ValueCurveStart = 0.0f;
				TimeCurveEnd = 0.0f;
				ValueCurveEnd = 0.0f;
			}

			public override string ToString()
			{
				return( "Data(" + Value.GetType().Name + ") ["
						+ "FrameNo: " + FrameNo.ToString()
						+ ", Value: " + Value.ToString()
						+ "] / Curve["
						+ "Type: " + KindCurve.ToString()
						+ ", StartT: " + TimeCurveStart.ToString()
						+ ", StartV: " + ValueCurveStart.ToString()
						+ ", EndT: " + TimeCurveEnd.ToString()
						+ ", EndV: " + ValueCurveEnd.ToString()
						+ "]"
					);
			}
		}

		public class DataBool : Data_Base<bool>
		{
			public DataBool()
			{
				CleanUp();
			}
			public new void CleanUp()
			{
				base.CleanUp();
				Value = false;
			}

			public void Interpolation(ref bool Output, int FrameNoPoint, DataBool End)
			{
				/* MEMO: Not Interpolated */
				Output = Value;
			}
		}
		public class DataInt : Data_Base<int>
		{
			public DataInt()
			{
				CleanUp();
			}
			public new void CleanUp()
			{
				base.CleanUp();
				Value = 0;
			}

			public void Interpolation(ref int Output, int FrameNoPoint, DataInt End)
			{
				Output = (int)LibraryEditor_SpriteStudio.KeyFrame.Interpolation.ValueGetFloat(	KindCurve,
																								FrameNoPoint,
																								FrameNo,
																								(float)Value,
																								End.FrameNo,
																								(float)End.Value,
																								TimeCurveStart,
																								ValueCurveStart,
																								TimeCurveEnd,
																								ValueCurveEnd
																							);
			}
		}
		public class DataFloat : Data_Base<float>
		{
			public DataFloat()
			{
				CleanUp();
			}

			public new void CleanUp()
			{
				base.CleanUp();
				Value = 0.0f;
			}

			public void Interpolation(ref float Output, int FrameNoPoint, DataFloat End)
			{
				Output = LibraryEditor_SpriteStudio.KeyFrame.Interpolation.ValueGetFloat(	KindCurve,
																							FrameNoPoint,
																							FrameNo,
																							(float)Value,
																							End.FrameNo,
																							(float)End.Value,
																							TimeCurveStart,
																							ValueCurveStart,
																							TimeCurveEnd,
																							ValueCurveEnd
																						);
			}
		}
		public class DataVector2 : Data_Base<Vector2>
		{
			public DataVector2()
			{
				CleanUp();
			}
			public new void CleanUp()
			{
				base.CleanUp();
				Value = Vector2.zero;
			}

			public void Interpolation(ref Vector2 Output, int FrameNoPoint, DataVector2 End)
			{
				Output.x = LibraryEditor_SpriteStudio.KeyFrame.Interpolation.ValueGetFloat(	KindCurve,
																							FrameNoPoint,
																							FrameNo,
																							(float)Value.x,
																							End.FrameNo,
																							(float)End.Value.x,
																							TimeCurveStart,
																							ValueCurveStart,
																							TimeCurveEnd,
																							ValueCurveEnd
																						);
				Output.y = LibraryEditor_SpriteStudio.KeyFrame.Interpolation.ValueGetFloat(	KindCurve,
																							FrameNoPoint,
																							FrameNo,
																							(float)Value.y,
																							End.FrameNo,
																							(float)End.Value.y,
																							TimeCurveStart,
																							ValueCurveStart,
																							TimeCurveEnd,
																							ValueCurveEnd
																						);
			}
		}
		public class DataVector3 : Data_Base<Vector3>
		{
			public DataVector3()
			{
				CleanUp();
			}
			public new void CleanUp()
			{
				base.CleanUp();
				Value = Vector3.zero;
			}

			public void Interpolation(ref Vector3 Output, int FrameNoPoint, DataVector3 End)
			{
				Output.x = LibraryEditor_SpriteStudio.KeyFrame.Interpolation.ValueGetFloat(	KindCurve,
																							FrameNoPoint,
																							FrameNo,
																							(float)Value.x,
																							End.FrameNo,
																							(float)End.Value.x,
																							TimeCurveStart,
																							ValueCurveStart,
																							TimeCurveEnd,
																							ValueCurveEnd
																						);
				Output.y = LibraryEditor_SpriteStudio.KeyFrame.Interpolation.ValueGetFloat(	KindCurve,
																							FrameNoPoint,
																							FrameNo,
																							(float)Value.y,
																							End.FrameNo,
																							(float)End.Value.y,
																							TimeCurveStart,
																							ValueCurveStart,
																							TimeCurveEnd,
																							ValueCurveEnd
																						);
				Output.z = LibraryEditor_SpriteStudio.KeyFrame.Interpolation.ValueGetFloat(	KindCurve,
																							FrameNoPoint,
																							FrameNo,
																							(float)Value.z,
																							End.FrameNo,
																							(float)End.Value.z,
																							TimeCurveStart,
																							ValueCurveStart,
																							TimeCurveEnd,
																							ValueCurveEnd
																						);
			}
		}
		public class DataColorBlend : Data_Base<ValueColorBlend>
		{
			public DataColorBlend()
			{
				CleanUp();
				Value = new ValueColorBlend();
			}
			public new void CleanUp()
			{
				base.CleanUp();
				Value = null;
			}

			public void Interpolation(ref Library_SpriteStudio.Data.AttributeColorBlend Output, int FrameNoPoint, DataColorBlend End)
			{
				Output.Bound = Value.Bound;
				Output.Operation = Value.Operation;
				for(int i=0; i<(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2; i++)
				{
#if false
					/* MEMO: SpriteStudio Ver.5.0-5.2 */
					Output.VertexColor[i].r = LibraryEditor_SpriteStudio.KeyFrame.Interpolation.ValueGetFloat(	KindCurve,
																												FrameNoPoint,
																												FrameNo,
																												(float)Value.VertexColor[i].r,
																												End.FrameNo,
																												(float)End.Value.VertexColor[i].r,
																												TimeCurveStart,
																												ValueCurveStart,
																												TimeCurveEnd,
																												ValueCurveEnd
																											);
					Output.VertexColor[i].g = LibraryEditor_SpriteStudio.KeyFrame.Interpolation.ValueGetFloat(	KindCurve,
																												FrameNoPoint,
																												FrameNo,
																												(float)Value.VertexColor[i].g,
																												End.FrameNo,
																												(float)End.Value.VertexColor[i].g,
																												TimeCurveStart,
																												ValueCurveStart,
																												TimeCurveEnd,
																												ValueCurveEnd
																											);
					Output.VertexColor[i].b = LibraryEditor_SpriteStudio.KeyFrame.Interpolation.ValueGetFloat(	KindCurve,
																												FrameNoPoint,
																												FrameNo,
																												(float)Value.VertexColor[i].b,
																												End.FrameNo,
																												(float)End.Value.VertexColor[i].b,
																												TimeCurveStart,
																												ValueCurveStart,
																												TimeCurveEnd,
																												ValueCurveEnd
																											);
					Output.VertexColor[i].a = LibraryEditor_SpriteStudio.KeyFrame.Interpolation.ValueGetFloat(	KindCurve,
																												FrameNoPoint,
																												FrameNo,
																												(float)Value.VertexColor[i].a,
																												End.FrameNo,
																												(float)End.Value.VertexColor[i].a,
																												TimeCurveStart,
																												ValueCurveStart,
																												TimeCurveEnd,
																												ValueCurveEnd
																											);

					Output.RatePixelAlpha[i] = LibraryEditor_SpriteStudio.KeyFrame.Interpolation.ValueGetFloat(	KindCurve,
																												FrameNoPoint,
																												FrameNo,
																												(float)Value.RatePixelAlpha[i],
																												End.FrameNo,
																												(float)End.Value.RatePixelAlpha[i],
																												TimeCurveStart,
																												ValueCurveStart,
																												TimeCurveEnd,
																												ValueCurveEnd
																											);
#else
					/* MEMO: SpriteStudio Ver.5.2- or Ver -4.x */
					float Rate = LibraryEditor_SpriteStudio.KeyFrame.Interpolation.ValueGetFloat(	KindCurve,
																									FrameNoPoint,
																									FrameNo,
																									0.0f,
																									End.FrameNo,
																									1.0f,
																									TimeCurveStart,
																									ValueCurveStart,
																									TimeCurveEnd,
																									ValueCurveEnd
																								);
					Rate = Mathf.Clamp01(Rate);

					Output.VertexColor[i].r = LibraryEditor_SpriteStudio.KeyFrame.Interpolation.Linear(Value.VertexColor[i].r, End.Value.VertexColor[i].r, Rate);
					Output.VertexColor[i].g = LibraryEditor_SpriteStudio.KeyFrame.Interpolation.Linear(Value.VertexColor[i].g, End.Value.VertexColor[i].g, Rate);
					Output.VertexColor[i].b = LibraryEditor_SpriteStudio.KeyFrame.Interpolation.Linear(Value.VertexColor[i].b, End.Value.VertexColor[i].b, Rate);
					Output.VertexColor[i].a = LibraryEditor_SpriteStudio.KeyFrame.Interpolation.Linear(Value.VertexColor[i].a, End.Value.VertexColor[i].a, Rate);
					Output.RatePixelAlpha[i] = LibraryEditor_SpriteStudio.KeyFrame.Interpolation.Linear(Value.RatePixelAlpha[i], End.Value.RatePixelAlpha[i], Rate);
#endif
				}
			}
		}
		public class DataVertexCorrection : Data_Base<ValueVertexCorrection>
		{
			public DataVertexCorrection()
			{
				CleanUp();
				Value = new ValueVertexCorrection();
			}
			public new void CleanUp()
			{
				base.CleanUp();
				Value = null;
			}

			public void Interpolation(ref Library_SpriteStudio.Data.AttributeVertexCorrection Output, int FrameNoPoint, DataVertexCorrection End)
			{
				for(int i=0; i<(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2; i++)
				{
					Output.Coordinate[i].x = LibraryEditor_SpriteStudio.KeyFrame.Interpolation.ValueGetFloat(	KindCurve,
																												FrameNoPoint,
																												FrameNo,
																												(float)Value.Coordinate[i].x,
																												End.FrameNo,
																												(float)End.Value.Coordinate[i].x,
																												TimeCurveStart,
																												ValueCurveStart,
																												TimeCurveEnd,
																												ValueCurveEnd
																											);
					Output.Coordinate[i].y = LibraryEditor_SpriteStudio.KeyFrame.Interpolation.ValueGetFloat(	KindCurve,
																												FrameNoPoint,
																												FrameNo,
																												(float)Value.Coordinate[i].y,
																												End.FrameNo,
																												(float)End.Value.Coordinate[i].y,
																												TimeCurveStart,
																												ValueCurveStart,
																												TimeCurveEnd,
																												ValueCurveEnd
																											);
				}
			}
		}
		public class DataCell : Data_Base<ValueCell>
		{
			public DataCell()
			{
				CleanUp();
				Value = new ValueCell();
			}
			public new void CleanUp()
			{
				base.CleanUp();
				Value = null;
			}

			public void Interpolation(ref Library_SpriteStudio.Data.AttributeCell Output, int FrameNoPoint, DataCell End)
			{
				/* MEMO: Not Interpolated */
				Output.IndexCellMap = Value.IndexCellMap;
				Output.IndexCell = Value.IndexCell;
			}
		}
		public class DataUserData : Data_Base<ValueUserData>
		{
			public DataUserData()
			{
				CleanUp();
				Value = new ValueUserData();
			}
			public new void CleanUp()
			{
				base.CleanUp();
				Value = null;
			}

			public void Interpolation(ref Library_SpriteStudio.Data.AttributeUserData Output, int FrameNoPoint, DataUserData End)
			{
				/* MEMO: Not Interpolated */
				Output.Flags = Value.Flags;
				Output.NumberInt = Value.NumberInt;
				Output.Coordinate = Value.Coordinate;
				Output.Rectangle = Value.Rectangle;
				Output.Text = Value.Text;
			}
		}
		public class DataInstance : Data_Base<ValueInstance>
		{
			public DataInstance()
			{
				CleanUp();
				Value = new ValueInstance();
			}
			public new void CleanUp()
			{
				base.CleanUp();
				Value = null;
			}

			public void Interpolation(ref Library_SpriteStudio.Data.AttributeInstance Output, int FrameNoPoint, DataInstance End)
			{
				/* MEMO: Not Interpolated */
				Output.Flags = Value.Flags;
				Output.RateTime = Value.RateTime;
				Output.PlayCount = Value.PlayCount;
				Output.LabelStart = Value.LabelStart;
				Output.OffsetStart = Value.OffsetStart;
				Output.LabelEnd = Value.LabelEnd;
				Output.OffsetEnd = Value.OffsetEnd;
			}
		}
		public class DataEffect : Data_Base<ValueEffect>
		{
			public DataEffect()
			{
				CleanUp();
				Value = new ValueEffect();
			}
			public new void CleanUp()
			{
				base.CleanUp();
				Value = null;
			}

			public void Interpolation(ref Library_SpriteStudio.Data.AttributeEffect Output, int FrameNoPoint, DataEffect End)
			{
				/* MEMO: Not Interpolated */
				Output.Flags = Value.Flags;
				Output.FrameStart = Value.FrameStart;
				Output.RateTime = Value.RateTime;
			}
		}

		public class ValueColorBlend : Library_SpriteStudio.Data.AttributeColorBlend
		{
			public ValueColorBlend()
			{
				CleanUp();
				VertexColor = new Color[(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2];
				RatePixelAlpha = new float[(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2];
			}

			public new void CleanUp()
			{
				base.CleanUp();
			}

			public override string ToString()
			{
				return( "Bound: " + Bound.ToString()
						+ ", Operation: " + Operation.ToString()
						+ ", Color: (" + VertexColor[0].ToString() + " - " + VertexColor[1].ToString()
							+ " - " + VertexColor[2].ToString() + " - " + VertexColor[3].ToString() + ")"
						+ ", Alpha: (" + RatePixelAlpha[0].ToString() + " - " + RatePixelAlpha[1].ToString()
							+ " - " + RatePixelAlpha[2].ToString() + " - " + RatePixelAlpha[3].ToString() + ") "
 					);
			}
		}
		public class ValueVertexCorrection : Library_SpriteStudio.Data.AttributeVertexCorrection
		{
			public ValueVertexCorrection()
			{
				CleanUp();
				Coordinate = new Vector2[(int)Library_SpriteStudio.KindVertexNo.TERMINATOR2];
			}

			public new void CleanUp()
			{
				base.CleanUp();
			}

			public override string ToString()
			{
				return("");
			}
		}
		public class ValueCell : Library_SpriteStudio.Data.AttributeCell
		{
			public ValueCell()
			{
				CleanUp();
			}

			public new void CleanUp()
			{
				base.CleanUp();
			}

			public override string ToString()
			{
				return("");
			}
		}
		public class ValueUserData : Library_SpriteStudio.Data.AttributeUserData
		{
			public ValueUserData()
			{
				CleanUp();
			}

			public new void CleanUp()
			{
				base.CleanUp();
			}

			public override string ToString()
			{
				return("");
			}
		}
		public class ValueInstance : Library_SpriteStudio.Data.AttributeInstance
		{
			public ValueInstance()
			{
				CleanUp();
			}

			public new void CleanUp()
			{
				base.CleanUp();
			}

			public override string ToString()
			{
				return("");
			}
		}
		public class ValueEffect : Library_SpriteStudio.Data.AttributeEffect
		{
			public ValueEffect()
			{
				CleanUp();
			}

			public new void CleanUp()
			{
				base.CleanUp();
			}

			public override string ToString()
			{
				return("");
			}
		}

		internal static class Interpolation
		{
			internal static float Linear(float Start, float End, float Point)
			{
				return(((End - Start) * Point) + Start);
			}

			internal static float Hermite(float Start, float End, float Point, float SpeedStart, float SpeedEnd)
			{
				float PointPow2 = Point * Point;
				float PointPow3 = PointPow2 * Point;
				return(	(((2.0f * PointPow3) - (3.0f * PointPow2) + 1.0f) * Start)
						+ (((3.0f * PointPow2) - (2.0f * PointPow3)) * End)
						+ ((PointPow3 - (2.0f * PointPow2) + Point) * (SpeedStart - Start))
						+ ((PointPow3 - PointPow2) * (SpeedEnd - End))
					);
			}

			internal static float Bezier(ref Vector2 Start, ref Vector2 End, float Point, ref Vector2 VectorStart, ref Vector2 VectorEnd)
			{
				float PointNow = Linear(Start.x, End.x, Point);
				float PointTemp;

				float AreaNow = 0.5f;
				float RangeNow = 0.5f;

				float Base;
				float BasePow2;
				float BasePow3;
				float AreaNowPow2;
				for(int i=0; i<8; i++)
				{
					Base = 1.0f - AreaNow;
					BasePow2 = Base * Base;
					BasePow3 = BasePow2 * Base;
					AreaNowPow2 = AreaNow * AreaNow;
					PointTemp = (BasePow3 * Start.x)
								+ (3.0f * BasePow2 * AreaNow * (VectorStart.x + Start.x))
								+ (3.0f * Base * AreaNowPow2 * (VectorEnd.x + End.x))
								+ (AreaNow * AreaNowPow2 * End.x);
					RangeNow *= 0.5f;
					AreaNow += ((PointTemp > PointNow) ? (-RangeNow) : (RangeNow));
				}

				AreaNowPow2 = AreaNow * AreaNow;
				Base = 1.0f - AreaNow;
				BasePow2 = Base * Base;
				BasePow3 = BasePow2 * Base;
				return(	(BasePow3 * Start.y)
						+ (3.0f * BasePow2 * AreaNow * (VectorStart.y + Start.y))
						+ (3.0f * Base * AreaNowPow2 * (VectorEnd.y + End.y))
						+ (AreaNow * AreaNowPow2 * End.y)
					);
			}

			internal static float Accelerate(float Start, float End, float Point)
			{
				return(((End - Start) * (Point * Point)) + Start);
			}

			internal static float Decelerate(float Start, float End, float Point)
			{
				float PointInverse = 1.0f - Point;
				float Rate = 1.0f - (PointInverse * PointInverse);
				return(((End - Start) * Rate) + Start);
			}

			internal static float ValueGetFloat(	Library_SpriteStudio.KindInterpolation Kind,
													int FrameNow,
													int FrameStart,
													float ValueStart,
													int FrameEnd,
													float ValueEnd,
													float CurveTimeStart,
													float CurveValueStart,
													float CurveTimeEnd,
													float CurveValueEnd
												)
			{
				if(FrameEnd <= FrameStart)
				{
					return(ValueStart);
//					return(ValueEnd);
				}
				float FrameNormalized = ((float)(FrameNow - FrameStart)) / ((float)(FrameEnd - FrameStart));
				FrameNormalized = Mathf.Clamp01(FrameNormalized);

				switch(Kind)
				{
					case Library_SpriteStudio.KindInterpolation.NON:
						return(ValueStart);

					case Library_SpriteStudio.KindInterpolation.LINEAR:
						return(Linear(ValueStart, ValueEnd, FrameNormalized));

					case Library_SpriteStudio.KindInterpolation.HERMITE:
						return(Hermite(ValueStart, ValueEnd, FrameNormalized, CurveValueStart, CurveValueEnd));

					case Library_SpriteStudio.KindInterpolation.BEZIER:
						{
							Vector2 Start = new Vector2((float)FrameStart, ValueStart);
							Vector2 VectorStart = new Vector2(CurveTimeStart, CurveValueStart);
							Vector2 End = new Vector2((float)FrameEnd, ValueEnd);
							Vector2 VectorEnd = new Vector2(CurveTimeEnd, CurveValueEnd);
							return(Interpolation.Bezier(ref Start, ref End, FrameNormalized, ref VectorStart, ref VectorEnd));
						}
//						break;	/* Redundant */

					case Library_SpriteStudio.KindInterpolation.ACCELERATE:
						return(Accelerate(ValueStart, ValueEnd, FrameNormalized));

					case Library_SpriteStudio.KindInterpolation.DECELERATE:
						return(Decelerate(ValueStart, ValueEnd, FrameNormalized));

					default:
						break;
				}
				return(ValueStart);	/* Error */
			}
		}

		internal static InformationAttribute InformationAttributeGetTagName(string NameTag)
		{
			InformationAttribute Description;
			return((true == ListInformationAttribute.TryGetValue(NameTag, out Description)) ? Description : null);
		}
		internal class InformationAttribute
		{
			internal int IndexAttribute;
			internal KindValue TypeValue;

			internal InformationAttribute(KindAttribute KindAttributeInitial, KindValue TypeValueInitial)
			{
				IndexAttribute = (int)KindAttributeInitial;
				TypeValue = TypeValueInitial;
			}
		}
		private readonly static Dictionary<string, InformationAttribute> ListInformationAttribute = new Dictionary<string, InformationAttribute>
		{
			/* Live Attributes */
			{"CELL",	new InformationAttribute(	KindAttribute.CELL,
													KindValue.CELL
												)
			},

			{"POSX",	new InformationAttribute(	KindAttribute.POSITION_X,
													KindValue.FLOAT
												)
			},
			{"POSY",	new InformationAttribute(	KindAttribute.POSITION_Y,
													KindValue.FLOAT
												)
			},
			{"POSZ",	new InformationAttribute(	KindAttribute.POSITION_Z,
													KindValue.FLOAT
												)
			},

			{"ROTX",	new InformationAttribute(	KindAttribute.ROTATION_X,
													KindValue.FLOAT
												)
			},
			{"ROTY",	new InformationAttribute(	KindAttribute.ROTATION_Y,
													KindValue.FLOAT
												)
			},
			{"ROTZ",	new InformationAttribute(	KindAttribute.ROTATION_Z,
													KindValue.FLOAT
												)
			},

			{"SCLX",	new InformationAttribute(	KindAttribute.SCALING_X,
													KindValue.FLOAT
												)
			},
			{"SCLY",	new InformationAttribute(	KindAttribute.SCALING_Y,
													KindValue.FLOAT
												)
			},

			{"ALPH",	new InformationAttribute(	KindAttribute.RATE_OPACITY,
													KindValue.FLOAT
												)
			},
			{"PRIO",	new InformationAttribute(	KindAttribute.PRIORITY,
													KindValue.FLOAT
												)
			},
			{"HIDE",	new InformationAttribute(	KindAttribute.HIDE,
													KindValue.BOOL
												)
			},

			{"VCOL",	new InformationAttribute(	KindAttribute.COLORBLEND,
													KindValue.COLORBLEND
												)
			},
			{"VERT",	new InformationAttribute(	KindAttribute.VERTEXCORRECTION,
													KindValue.VERTEXCORRECTION
												)
			},


			{"FLPH",	new InformationAttribute(	KindAttribute.FLIP_X,
													KindValue.BOOL
												)
			},
			{"FLPV",	new InformationAttribute(	KindAttribute.FLIP_Y,
													KindValue.BOOL
												)
			},

			{"PVTX",	new InformationAttribute(	KindAttribute.PIVOT_OFFSET_X,
													KindValue.FLOAT
												)
			},
			{"PVTY",	new InformationAttribute(	KindAttribute.PIVOT_OFFSET_Y,
													KindValue.FLOAT
												)
			},

			{"ANCX",	new InformationAttribute(	KindAttribute.ANCHOR_POSITION_X,
													KindValue.FLOAT
												)
			},
			{"ANCY",	new InformationAttribute(	KindAttribute.ANCHOR_POSITION_Y,
													KindValue.FLOAT
												)
			},

			{"SIZX",	new InformationAttribute(	KindAttribute.SIZE_FORCE_X,
													KindValue.FLOAT
												)
			},
			{"SIZY",	new InformationAttribute(	KindAttribute.SIZE_FORCE_Y,
													KindValue.FLOAT
												)
			},

			{"IFLH",	new InformationAttribute(	KindAttribute.TEXTURE_FLIP_X,
													KindValue.BOOL
												)
			},
			{"IFLV",	new InformationAttribute(	KindAttribute.TEXTURE_FLIP_Y,
													KindValue.BOOL
												)
			},

			{"UVTX",	new InformationAttribute(	KindAttribute.TEXTURE_POSITION_X,
													KindValue.FLOAT
												)
			},
			{"UVTY",	new InformationAttribute(	KindAttribute.TEXTURE_POSITION_Y,
													KindValue.FLOAT
												)
			},
			{"UVRZ",	new InformationAttribute(	KindAttribute.TEXTURE_ROTATION,
													KindValue.FLOAT
												)
			},
			{"UVSX",	new InformationAttribute(	KindAttribute.TEXTURE_SCALING_X,
													KindValue.FLOAT
												)
			},
			{"UVSY",	new InformationAttribute(	KindAttribute.TEXTURE_SCALING_Y,
													KindValue.FLOAT
												)
			},

			{"BNDR",	new InformationAttribute(	KindAttribute.COLLISION_RADIUS,
													KindValue.FLOAT
												)
			},

			{"USER",	new InformationAttribute(	KindAttribute.USERDATA,
													KindValue.USERDATA
												)
			},
			{"IPRM",	new InformationAttribute(	KindAttribute.INSTANCE,
													KindValue.INSTANCE
												)
			},
			{"EFCT",	new InformationAttribute(	KindAttribute.EFFECT,
													KindValue.EFFECT
												)
			},

#if false
			/* Legacy(Disused) Attributes */
			{"IMGX",	new InformationAttribute(	KindAttribute.TEXTURE_POSITION_X,
													KindValue.FLOAT
												)
			},
			{"IMGY",	new InformationAttribute(	KindAttribute.TEXTURE_POSITION_Y,
													KindValue.FLOAT
												)
			},
			{"IMGW",	new InformationAttribute(	KindAttribute.TEXTURE_SCALING_X,
													KindValue.FLOAT
												)
			},
			{"IMGH",	new InformationAttribute(	KindAttribute.TEXTURE_SCALING_Y,
													KindValue.FLOAT
												)
			},
			{"ORFX",	new InformationAttribute(	KindAttribute.PIVOT_OFFSET_X,
													KindValue.FLOAT
												)
			},
			{"ORFY",	new InformationAttribute(	KindAttribute.PIVOT_OFFSET_Y,
													KindValue.FLOAT
												)
			},
#endif
		};
	}

	internal static class Utility
	{
		internal static class File
		{
			internal readonly static string NamePathRootFile = Application.dataPath;
			internal readonly static string NamePathRootAsset = "Assets";

			/* Choose File on File-Dialogue */
			internal static bool FileNameGetFileDialog(out string NameDirectory, out string NameFileBody, out string NameFileExtension, string TitleDialog, string FilterExtension)
			{
				/* Get Previous Folder-Name */
				string DirectoryPrevious = "";
				Menu.SettingGetFolderImport(out DirectoryPrevious);

				/* Choose Import-File */
				string FileNameFullPath = EditorUtility.OpenFilePanel(TitleDialog, DirectoryPrevious, FilterExtension);
				if(0 == FileNameFullPath.Length)
				{	/* Cancelled */
					NameDirectory = "";
					NameFileBody = "";
					NameFileExtension = "";
					return(false);
				}

				NameDirectory = Path.GetDirectoryName(FileNameFullPath);
				NameFileBody = Path.GetFileNameWithoutExtension(FileNameFullPath);
				NameFileExtension = Path.GetExtension(FileNameFullPath);

				/* Save Folder-Name */
				Menu.SettingSetFolderImport(ref NameDirectory);

				return(true);
			}

			/* Overwrite Confirm Dialogue */
			internal static bool OverwriteConfirmDialogue(string NameAsset, string NameTypeAsset, bool FlagSwitch, ref bool FlagSwitchSub)
			{
				if((false == FlagSwitch) || (false == FlagSwitchSub))
				{	/* No-Confirm */
					return(true);
				}

				bool Rv = false;
				int KindResult = EditorUtility.DisplayDialogComplex(	"Do you want to overwrite?",
																		"The asset already exists.\n" + NameAsset,
																		"Yes",
																		"All \"" + NameTypeAsset +"\"",
																		"No"
																	);
				switch(KindResult)
				{
					case 0:	/* Yes */
						Rv = true;
						break;

					case 1:	/* All */
						FlagSwitchSub = false;
						Rv = true;
						break;

					case 2:	/* No */
						Rv = false;
						break;

				}
				return(Rv);
			}

			/* Convert Path-in-Asset to Native-Path */
			internal static string PathGetAssetPath(string Name)
			{
				string NamePath = System.String.Copy(Application.dataPath);
				if(null != Name)
				{
					NamePath += "/" + Name.Substring(NamePathRootAsset.Length + 1);
				}
				return(NamePath);
			}

			/* File-Copy (Native-File System -> Asset Folder) */
			internal static bool FileCopyToAsset(string NameAsset, string NameOriginalFileName, bool FlagOverCopy)
			{
				System.IO.File.Copy(NameOriginalFileName, NameAsset, true);
				return(true);
			}

			/* Get Selected PathName (in Project-Window) */
			internal static string AssetPathGetSelected(string NamePath)
			{
				string NamePathAsset = "";
				if(null == NamePath)
				{	/* Now Selected Path in "Project" */
					UnityEngine.Object ObjectNow = Selection.activeObject;
					if(null == ObjectNow)
					{	/* No Selected *//* Error */
						NamePathAsset = "";
					}
					else
					{	/* Selected */
						NamePathAsset = AssetDatabase.GetAssetPath(ObjectNow);
					}
				}
				else
				{	/* Specified */
					NamePathAsset = System.String.Copy(NamePath);
				}

				return(NamePathAsset);
			}

			/* Create Destination Folders */
			internal static string AssetFolderCreate(string NamePath, string NamePathSubFolder)
			{
				string NamePathBase = String.Copy(NamePath);

				/* Create Project-Name Folder */
				if(null != NamePathSubFolder)
				{
					AssetFolderCreateMain(NamePathSubFolder, NamePathBase);
					NamePathBase += "/" + NamePathSubFolder;
				}

				return(NamePathBase);
			}
			private static bool AssetFolderCreateMain(string Name, string NameParent)
			{
				string NameFolderParent = "";
				if(null != NameParent)
				{	/* Sub-Folder */
					NameFolderParent = string.Copy(NameParent);
				}
				else
				{	/* Root */
					NameFolderParent = string.Copy(NamePathRootAsset);
				}

				string PathFolderNative = PathGetAssetPath(NameFolderParent + "/" + Name);
				if(false == Directory.Exists(PathFolderNative))
				{	/* Not Found */
					AssetDatabase.CreateFolder(NameFolderParent, Name);
					AssetDatabase.SaveAssets();
				}
				return(true);
			}

			internal static string NameAssetBodyGet(	string NameAssetBase,
														string NamePrefixBase,
														string NameSSPJ,
														bool FlagApply,
														ref SettingImport DataSettingImport
													)
			{
				if(true == DataSettingImport.FlagNameDataAttachSpecific)
				{
					if(null == NamePrefixBase)
					{
						if(true == DataSettingImport.FlagNameDataAttachSpecificSSPJ)
						{	/* SSPJ_... */
							if(true == FlagApply)
							{
								return(NameSSPJ + "_" + NameAssetBase);
							}
						}
						goto NameAssetBodyGet_AsItIs;	/* Option Error = .... */
					}
					else
					{	/* xx_... */
						if(true == DataSettingImport.FlagNameDataAttachSpecificSSPJ)
						{
							if(null == NameSSPJ)
							{
								if(true == FlagApply)
								{
									return(NamePrefixBase + NameAssetBase);
								}
							}
							else
							{
								if(true == FlagApply)
								{
									return(NamePrefixBase + NameSSPJ + "_" + NameAssetBase);
								}
							}
						}
						else
						{
							if(true == FlagApply)
							{
								return(NamePrefixBase + NameAssetBase);
							}
						}
					}
				}

			NameAssetBodyGet_AsItIs:;
				return(NameAssetBase);
			}
		}

		internal class Text
		{
			internal static bool ValueGetBool<_Type>(_Type Source)
			{
				return((0 != ValueGetInt(Source)) ? true : false);
			}

			internal static byte ValueGetByte<_Type>(_Type Source)
			{
				return(System.Convert.ToByte(Source));
			}

			internal static int ValueGetInt<_Type>(_Type Source)
			{
				return(System.Convert.ToInt32(Source));
			}

			internal static uint ValueGetUInt<_Type>(_Type Source)
			{
				return(System.Convert.ToUInt32(Source));
			}

			internal static float ValueGetFloat<_Type>(_Type Source)
			{
				return(System.Convert.ToSingle(Source));
			}

			internal static double ValueGetDouble<_Type>(_Type Source)
			{
				return(System.Convert.ToDouble(Source));
			}

			internal static int HexToInt(string Text)
			{
				return(System.Convert.ToInt32(Text, 16));
			}

			internal static uint HexToUInt(string Text)
			{
				return(System.Convert.ToUInt32(Text, 16));
			}

			internal static bool TextToBool(string Text)
			{
				bool Rv = false;
				try
				{
					Rv = System.Convert.ToBoolean(Text);
				}
				catch(System.FormatException)
				{
					Rv = ((0 == System.Convert.ToInt32(Text)) ? (false) : (true));
				}
				return(Rv);
			}

			internal static int TextToVersionCode(string Text)
			{	/* MEMO: Text = "Major:1"."Minor:2"."Revison:2" */
				string[] Item = Text.Split('.');
				if (3 != Item.Length)
				{
					return(-1);
				}

				int VersionMajor = LibraryEditor_SpriteStudio.Utility.Text.HexToInt(Item[0]);
				int VersionMinor = LibraryEditor_SpriteStudio.Utility.Text.HexToInt(Item[1]);
				int Revision = LibraryEditor_SpriteStudio.Utility.Text.HexToInt(Item[2]);
				return((VersionMajor << 16) | (VersionMinor << 8) | Revision);
			}

			internal static string PathGetAbsolute(	string NamePath,
													LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject=null,
													LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile KindBase=LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.NON
												)
			{
				string NameBase = "";
				if(true == Path.IsPathRooted(NamePath))
				{	/* MEMO: "NamePath" is "Absolute". */
					NameBase = NamePath;
				}
				else
				{	/* MEMO: "NamePath" is "Relative". */
					switch(KindBase)
					{
						case LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.NON:
							NameBase = NamePath;
							break;

						case LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.SSPJ:
							NameBase = InformationProject.NameDirectory + NamePath;
							break;

						case LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.SSCE:
							if(false == string.IsNullOrEmpty(InformationProject.NameDirectoryBaseSSCE))
							{
								NameBase = InformationProject.NameDirectoryBaseSSCE + NamePath;
							}
							else
							{
								goto case LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.SSPJ;
							}
							break;

						case LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.SSAE:
							if(false == string.IsNullOrEmpty(InformationProject.NameDirectoryBaseSSAE))
							{
								NameBase = InformationProject.NameDirectoryBaseSSAE + NamePath;
							}
							else
							{
								goto case LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.SSPJ;
							}
							break;

						case LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.SSEE:
							if(false == string.IsNullOrEmpty(InformationProject.NameDirectoryBaseSSEE))
							{
								NameBase = InformationProject.NameDirectoryBaseSSEE + NamePath;
							}
							else
							{
								goto case LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.SSPJ;
							}
							break;

						case LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.TEXTURE:
							if(false == string.IsNullOrEmpty(InformationProject.NameDirectoryBaseTexture))
							{
								NameBase = InformationProject.NameDirectoryBaseTexture + NamePath;
							}
							else
							{
								goto case LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.SSPJ;
							}
							break;
					}
				}

				/* Normalize Path-Name */
				string NameNewDirectory = Path.GetDirectoryName(NameBase);
				string NameNewFileBody = Path.GetFileNameWithoutExtension(NameBase);
				string NameNewFileExtension = Path.GetExtension(NameBase);
				string NamePathNew = NameNewDirectory + "/" + NameNewFileBody + NameNewFileExtension;
				NamePathNew = NamePathNew.Replace("\\", "/");	/* "\" -> "/" */

				return(NamePathNew);
			}

			internal static string PathGetRelative(	string NamePath,
													LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ InformationProject=null,
													LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile KindBase=LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.NON
												)
			{
				string NameBase = "";
				if(false == Path.IsPathRooted(NamePath))
				{	/* MEMO: "NamePath" is "Relative". */
					NameBase = NamePath;
					NameBase = NameBase.Replace("\\", "/");	/* "\" -> "/" */
					return(NameBase);
				}
				else
				{	/* MEMO: "NamePath" is "Absolute". */
					switch(KindBase)
					{
						case LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.NON:
							return(NamePath);

						case LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.SSPJ:
							NameBase = InformationProject.NameDirectory;
							break;

						case LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.SSCE:
							if(false == string.IsNullOrEmpty(InformationProject.NameDirectoryBaseSSCE))
							{
								NameBase = InformationProject.NameDirectoryBaseSSCE;
							}
							else
							{
								goto case LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.SSPJ;
							}
							break;

						case LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.SSAE:
							if(false == string.IsNullOrEmpty(InformationProject.NameDirectoryBaseSSAE))
							{
								NameBase = InformationProject.NameDirectoryBaseSSAE;
							}
							else
							{
								goto case LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.SSPJ;
							}
							break;

						case LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.SSEE:
							if(false == string.IsNullOrEmpty(InformationProject.NameDirectoryBaseSSEE))
							{
								NameBase = InformationProject.NameDirectoryBaseSSEE;
							}
							else
							{
								goto case LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.SSPJ;
							}
							break;

						case LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.TEXTURE:
							if(false == string.IsNullOrEmpty(InformationProject.NameDirectoryBaseTexture))
							{
								NameBase = InformationProject.NameDirectoryBaseTexture;
							}
							else
							{
								goto case LibraryEditor_SpriteStudio.ParseOPSS.InformationSSPJ.KindFile.SSPJ;
							}
							break;
					}
					NameBase = NameBase.Replace("\\", "/");	/* "\" -> "/" */
					string NameNew = NamePath.Replace(NameBase, "");	/* CAUTION: Modified this line! */
					return(NameNew);
				}
			}

			internal static string DataNameGetFromPath(string NamePath, bool FlagRuleOld)
			{
				string Rv = "";
				if(true == string.IsNullOrEmpty(NamePath))
				{
					return("");
				}

				string NameNewDirectory = Path.GetDirectoryName(NamePath);
				string NameNewFileBody = Path.GetFileNameWithoutExtension(NamePath);
				if((true == string.IsNullOrEmpty(NameNewDirectory)) || (true == FlagRuleOld))
				{
					Rv = NameNewFileBody;
				}
				else
				{
					Rv = NameNewDirectory + "/" + NameNewFileBody;
				}
				Rv = Rv.Replace("\\", "/");	/* "\" -> "/" */

				Rv = Rv.Replace("../", "_");	/* "../" -> "_" */
				Rv = Rv.Replace("/", "_");	/* "/" -> "_" */
				return(Rv);
			}
		}

		internal class XML
		{
			internal static XmlNodeList ListGetNode(XmlNode Node, string NamePath, XmlNamespaceManager Manager)
			{
				return(Node.SelectNodes(NamePath, Manager));
			}

			internal static XmlNode NodeGet(XmlNode Node, string NamePath, XmlNamespaceManager Manager)
			{
				return(Node.SelectSingleNode(NamePath, Manager));
			}

			internal static string TextGetNode(XmlNode Node, string NamePath, XmlNamespaceManager Manager)
			{
				XmlNode NodeNow = NodeGet(Node, NamePath, Manager);
				return((null != NodeNow) ? NodeNow.InnerText : null);
			}

			internal static int VersionCodeGet(XmlNode NodeRoot, string NameTag, int ErrorCode, bool FlagMaskRevision)
			{
				XmlAttributeCollection AttributeNodeRoot = NodeRoot.Attributes;
				if(NameTag != NodeRoot.Name)
				{
					Debug.LogError(	"SSxx-Import Error: Invalid Root-Tag: "
									+ NodeRoot.Name
								);
					return(ErrorCode);
				}

				XmlNode NodeVersion = AttributeNodeRoot["version"];
				string VersionText = NodeVersion.Value;
				int Version = LibraryEditor_SpriteStudio.Utility.Text.TextToVersionCode(VersionText);
				if(-1 == Version)
				{
					Debug.LogError(	"SSxx-Import Error: Version-Invalid = " + VersionText);
					return(ErrorCode);
				}

				if(true == FlagMaskRevision)
				{
					Version &= ~0x000000ff;
				}
				return(Version);
			}
		}

		internal class Inspector
		{
			internal static void DataDisplayDataAnimation(int LevelIndent, Script_SpriteStudio_DataAnimation DataAnimation)
			{
				if(null != DataAnimation)
				{
					int CountAnimation = DataAnimation.CountGetAnimation();
					Library_SpriteStudio.Data.Animation DataAnimationBody = null;
					for(int i=0; i<CountAnimation; i++)
					{
						DataAnimationBody = DataAnimation.DataGetAnimation(i);
						if(null != DataAnimationBody)
						{
							EditorGUI.indentLevel = LevelIndent;
							EditorGUILayout.LabelField("Animation No [" + i + "]: Name[" + DataAnimationBody.Name + "]");

							EditorGUI.indentLevel = LevelIndent + 1;
							EditorGUILayout.LabelField("Frame Length: " + DataAnimationBody.CountFrame.ToString("D5"));

							int CountFPS = DataAnimationBody.FramePerSecond;
							float FPS = 1.0f / (float)CountFPS;
							EditorGUILayout.LabelField("Base FPS: " + CountFPS.ToString("D3") + " (" + FPS.ToString() + " Sec.)");
							EditorGUILayout.Space();
						}
						else
						{	/* Error */
							EditorGUI.indentLevel = LevelIndent;
							EditorGUILayout.LabelField("Animation No [" + i + "]: Error[Data Missing! (Fatal)]");
							EditorGUILayout.Space();
						}
					}
				}
				else
				{	/* Error */
					EditorGUI.indentLevel = LevelIndent;
					EditorGUILayout.LabelField("Error[Datas Missing! (Fatal)]");
					EditorGUILayout.Space();
				}
				EditorGUI.indentLevel = LevelIndent;
			}

			internal static void DataDisplayDataCellMap(int LevelIndent, Script_SpriteStudio_DataCell DataCell)
			{
				if(null != DataCell)
				{
					int CountCellMap = DataCell.CountGetCellMap();
					int CountCell = 0;
					Library_SpriteStudio.Data.CellMap DataCellMapBody = null;
					Library_SpriteStudio.Data.Cell DataCellBody = null;
					for(int i=0; i<CountCellMap; i++)
					{
						DataCellMapBody = DataCell.DataGetCellMap(i);
						if(null != DataCellMapBody)
						{
							EditorGUI.indentLevel = LevelIndent;
							EditorGUILayout.LabelField("CellMap No [" + i + "]: Name[" + DataCellMapBody.Name + "]");

							EditorGUI.indentLevel = LevelIndent + 1;
							EditorGUILayout.LabelField("Original Texture-Size: " + ((int)(DataCellMapBody.SizeOriginal.x)).ToString() + ", "
																					+ ((int)(DataCellMapBody.SizeOriginal.y)).ToString()
													);
							CountCell = DataCellMapBody.CountGetCell();
							for(int j=0; j<CountCell; j++)
							{
								DataCellBody = DataCellMapBody.DataGetCell(j);
								if(null != DataCellBody)
								{
									EditorGUI.indentLevel = LevelIndent + 1;
									EditorGUILayout.LabelField("Cell No [" + j + "]: Name[" + DataCellBody.Name + "]");

									EditorGUI.indentLevel = LevelIndent + 2;
									EditorGUILayout.LabelField("Orientation: " + ((int)DataCellBody.Rectangle.x).ToString() + ", "
																				+ ((int)DataCellBody.Rectangle.y).ToString()
															);
									EditorGUILayout.LabelField("Size: " + ((int)DataCellBody.Rectangle.width).ToString() + ", "
																				+ ((int)DataCellBody.Rectangle.height).ToString()
															);
									EditorGUILayout.LabelField("Pivot: " + ((int)DataCellBody.Pivot.x).ToString() + ", "
																				+ ((int)DataCellBody.Pivot.y).ToString()
															);
//									EditorGUILayout.Space();
								}
								else
								{	/* Error */
									EditorGUI.indentLevel = LevelIndent + 1;
									EditorGUILayout.LabelField("Cell No [" + j + "]: Error[Data Missing! (Fatal)]");
//									EditorGUILayout.Space();
								}
							}
							EditorGUILayout.Space();
						}
						else
						{	/* Error */
							EditorGUI.indentLevel = LevelIndent;
							EditorGUILayout.LabelField("CellMap No [" + i + "]: Error[Data Missing! (Fatal)]");
							EditorGUILayout.Space();
						}
					}
				}
				else
				{	/* Error */
					EditorGUI.indentLevel = LevelIndent;
					EditorGUILayout.LabelField("Error[Datas Missing! (Fatal)]");
					EditorGUILayout.Space();
				}
				EditorGUI.indentLevel = LevelIndent;
			}
		}
	}
}
