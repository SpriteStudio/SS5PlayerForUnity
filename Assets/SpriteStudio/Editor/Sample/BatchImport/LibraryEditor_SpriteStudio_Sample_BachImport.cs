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

public static partial class LibraryEditor_SpriteStudio_Sample_BachImport
{
	/* First-Character(Prifix) of the Special-Line */
	private const char PrefixChangeSetting = '?';
	private const char PrefixRemarks = '*';

	/* WorkArea */
	private static string NameBaseFolderSS5Data = "";
	private static string NameBaseFolderAsset = "";

	public static partial class Menu
	{
		public static bool ImportSSPJBatch(string FileImportList)
		{
			/* Create SSPJ Import-Setting */
			/* MEMO: Default-Values are same SSPJ-Importer's Default. (except "FlagConfirmOverWriteXXX") */
			LibraryEditor_SpriteStudio.SettingImport DataSettingImport = new LibraryEditor_SpriteStudio.SettingImport();
			DataSettingImport.FlagNameDataRuleOld = true;
			DataSettingImport.FlagNameDataAttachSpecific = false;
			DataSettingImport.FlagNameDataAttachSpecificToPrefab = false;
			DataSettingImport.FlagNameDataAttachSpecificToTexture = false;
			DataSettingImport.FlagNameDataAttachSpecificSSPJ = false;
			DataSettingImport.TextureSizePixelMaximum = 4096;
			DataSettingImport.FlagAttachControlGameObject = true;
			DataSettingImport.FlagCreateProjectFolder = true;
			DataSettingImport.FlagDataCalculateInAdvance = false;
			DataSettingImport.FlagDataCompress = true;
			DataSettingImport.FlagDataTakeOverSettingPrefab = false;
			DataSettingImport.CollisionThicknessZ = 1.0f;
			DataSettingImport.FlagAttachRigidBody = true;
			DataSettingImport.FlagConfirmOverWrite = false;					/* Caution!: This item is different */
			DataSettingImport.FlagConfirmOverWriteRoot = false;				/* Caution!: This item is different */
			DataSettingImport.FlagConfirmOverWriteRootEffect = false;		/* Caution!: This item is different */
			DataSettingImport.FlagConfirmOverWriteDataCellMap = false;		/* Caution!: This item is different */
			DataSettingImport.FlagConfirmOverWriteDataAnimation = false;	/* Caution!: This item is different */
			DataSettingImport.FlagConfirmOverWriteDataEffect = false;		/* Caution!: This item is different */
			DataSettingImport.FlagConfirmOverWriteMaterial = false;			/* Caution!: This item is different */
			DataSettingImport.FlagConfirmOverWriteTexture = false;			/* Caution!: This item is different */
			DataSettingImport.FlagGetAnimationReferencedPartsRoot = true;
			DataSettingImport.FlagGetCellMapReferencedPartsRoot = true;
			DataSettingImport.FlagGetMaterialPartsRoot = true;
			DataSettingImport.FlagGetTextureMaterial = true;

			UnityEngine.Object ObjectSelected = Selection.activeObject;
			if(null == ObjectSelected)
			{	/* No Selected *//* Error */
				NameBaseFolderAsset = "Assets";
			}
			else
			{	/* Selected */
				NameBaseFolderAsset = AssetDatabase.GetAssetPath(ObjectSelected);
			}

			/* Open List-File */
			System.IO.StreamReader FileReader = new System.IO.StreamReader(FileImportList, System.Text.Encoding.Default);

			/* Decode List-File (1 Line) */
			string TextLineOriginal = "";
			string TextLineTrimmed = "";
			char PrifixLine;
			while(0 <= FileReader.Peek())
			{
				/* Read & Trim 1-Line */
				TextLineOriginal = FileReader.ReadLine();
				TextLineTrimmed = Decode.TextTrim(TextLineOriginal);

				/* Line-Top-Command Decode */
				if(false == String.IsNullOrEmpty(TextLineTrimmed))
				{
					PrifixLine = TextLineTrimmed[0];	/* Get Top-Letter */
					switch(PrifixLine)
					{
						case PrefixChangeSetting:
							{
								/* Remove Top-Letter & Decode Command */
								Decode.ChangeSetting(ref DataSettingImport, TextLineTrimmed.Remove(0, 1));
							}
							break;
						case PrefixRemarks:
							/* MEMO: Text of this line will be ignored . */
							break;
						default:
							/* File-Import */
							Decode.Import(ref DataSettingImport, TextLineTrimmed);
							break;
					}
				}
			}

			/* Close List-File */
			FileReader.Close();

			/**/
			return(true);
		}
	}

	internal static class Decode
	{
		/* Trimmed characters */
		/* MEMO: When these letters are attached to the front and back, it will be ignored. */
		private readonly static char[] TextIgnored = new char[] { ' ', '\t' };

		/* Command Seperator */
		private readonly static char[] TextSeparatorCommand = new char[] { '>' };

		/* Command-Names for Setting-Changing */
		private const string CommandNameBaseFolderSS5Data = "NameBaseFolderSS5Data";
		private const string CommandNameBaseFolderAsset = "NameBaseFolderAsset";

		private const string CommandFlagNameDataRuleOld = "FlagNameDataRuleOld";
		private const string CommandFlagNameDataAttachSpecific = "FlagNameDataAttachSpecific";
		private const string CommandFlagNameDataAttachSpecificToPrefab = "FlagNameDataAttachSpecificToPrefab";
		private const string CommandFlagNameDataAttachSpecificToTexture = "FlagNameDataAttachSpecificToTexture";
		private const string CommandFlagNameDataAttachSpecificSSPJ = "FlagNameDataAttachSpecificSSPJ";

		private const string CommandTextureSizePixelMaximum = "TextureSizePixelMaximum";
		private const string CommandFlagAttachControlGameObject = "FlagAttachControlGameObject";
		private const string CommandFlagCreateProjectFolder = "FlagCreateProjectFolder";
		private const string CommandFlagDataCalculateInAdvance = "FlagDataCalculateInAdvance";
		private const string CommandFlagDataCompress = "FlagDataCompress";
		private const string CommandFlagDataTakeOverSettingPrefab = "FlagDataTakeOverSettingPrefab";

		private const string CommandCollisionThicknessZ = "CollisionThicknessZ";
		private const string CommandFlagAttachRigidBody = "FlagAttachRigidBody";

		private const string CommandFlagConfirmOverWrite = "FlagConfirmOverWrite";
		private const string CommandFlagConfirmOverWriteRoot = "FlagConfirmOverWriteRoot";
		private const string CommandFlagConfirmOverWriteRootEffect = "FlagConfirmOverWriteRootEffect";
		private const string CommandFlagConfirmOverWriteDataCellMap = "FlagConfirmOverWriteDataCellMap";
		private const string CommandFlagConfirmOverWriteDataAnimation = "FlagConfirmOverWriteDataAnimation";
		private const string CommandFlagConfirmOverWriteDataEffect = "FlagConfirmOverWriteDataEffect";
		private const string CommandFlagConfirmOverWriteMaterial = "FlagConfirmOverWriteMaterial";
		private const string CommandFlagConfirmOverWriteTexture = "FlagConfirmOverWriteTexture";

		private const string CommandFlagGetAnimationReferencedPartsRoot = "FlagGetAnimationReferencedPartsRoot";
		private const string CommandFlagGetCellMapReferencedPartsRoot = "FlagGetCellMapReferencedPartsRoot";
		private const string CommandFlagGetMaterialPartsRoot = "FlagGetMaterialPartsRoot";
		private const string CommandFlagGetTextureMaterial = "FlagGetTextureMaterial";

		private const string TextTrue = "true";
		private const string TextFalse = "false";

		internal static string TextTrim(string Text)
		{
			return(Text.Trim(TextIgnored));
		}

		internal static bool ChangeSetting(ref LibraryEditor_SpriteStudio.SettingImport DataSettingImport, string TextLine)
		{
			/* Sprit Line */
			string TextLineTrimmed = TextTrim(TextLine);
			string[] Arguments = TextLineTrimmed.Split(TextSeparatorCommand);
			string TextCommand = TextTrim(Arguments[0]);
			string TextParameter = "";

			/* Command Decoding */
			switch(TextCommand)
			{
				case CommandNameBaseFolderAsset:	/* NameBaseFolderAsset */
					/* NameBaseFolderAsset [Base Asset-Folder Name] */
					TextParameter = String.Copy(Arguments[1]);
					NameBaseFolderAsset = TextTrim(TextParameter);
					break;

				case CommandNameBaseFolderSS5Data:	/* NameBaseFolderSS5Data */
					/* NameBaseFolderSS5Data [Base Folder Name] */
					TextParameter = String.Copy(Arguments[1]);
					NameBaseFolderSS5Data = TextTrim(TextParameter);
					break;

				case CommandFlagNameDataRuleOld:	/* FlagNameDataRuleOld */
					/* FlagNameDataRuleOld [true/false] */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagNameDataRuleOld = FlagGetChangeSetting(TextParameter);
					break;

				case CommandFlagNameDataAttachSpecific:	/* FlagNameDataAttachSpecific */
					/* FlagNameDataAttachSpecific [true/false] */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagNameDataAttachSpecific = FlagGetChangeSetting(TextParameter);
					break;

				case CommandFlagNameDataAttachSpecificToPrefab: /* FlagNameDataAttachSpecificToPrefab */
					/* FlagNameDataAttachSpecificToPrefab [true/false] */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagNameDataAttachSpecificToPrefab = FlagGetChangeSetting(TextParameter);
					break;

				case CommandFlagNameDataAttachSpecificToTexture:    /* FlagNameDataAttachSpecificToTexture */
					/* FlagNameDataAttachSpecificToTexture [true/false] */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagNameDataAttachSpecificToTexture = FlagGetChangeSetting(TextParameter);
					break;

				case CommandFlagNameDataAttachSpecificSSPJ:	/* FlagNameDataAttachSpecificSSPJ */
					/* FlagNameDataAttachSpecificSSPJ [true/false] */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagNameDataAttachSpecificSSPJ = FlagGetChangeSetting(TextParameter);
					break;

				case CommandTextureSizePixelMaximum:	/* TextureSizePixelMaximum */
					/* TextureSizePixelMaximum [value] */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.TextureSizePixelMaximum = IntGetChangeSetting(TextParameter);
					break;

				case CommandFlagAttachControlGameObject:	/* FlagAttachControlGameObject */
					/* FlagAttachControlGameObject [true/false] */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagAttachControlGameObject = FlagGetChangeSetting(TextParameter);
					break;

				case CommandFlagCreateProjectFolder:	/* FlagCreateProjectFolder */
					/* FlagCreateProjectFolder [true/false] */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagCreateProjectFolder = FlagGetChangeSetting(TextParameter);
					break;

				case CommandFlagDataCalculateInAdvance:	/* FlagDataCalculateInAdvance */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagDataCalculateInAdvance = FlagGetChangeSetting(TextParameter);
					break;

				case CommandFlagDataCompress:	/* FlagDataCompress */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagDataCompress = FlagGetChangeSetting(TextParameter);
					break;

				case CommandFlagDataTakeOverSettingPrefab:	/* FlagDataTakeOverSettingPrefab */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagDataTakeOverSettingPrefab = FlagGetChangeSetting(TextParameter);
					break;

				case CommandCollisionThicknessZ:	/* CollisionThicknessZ */
					/* CollisionThicknessZ [value] */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.CollisionThicknessZ = FloatGetChangeSetting(TextParameter);
					break;

				case CommandFlagAttachRigidBody:	/* FlagAttachRigidBody */
					/* FlagAttachRigidBody [true/false] */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagAttachRigidBody = FlagGetChangeSetting(TextParameter);
					break;

				case CommandFlagConfirmOverWrite:	/* FlagConfirmOverWrite */
					/* FlagConfirmOverWrite [true/false] */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagConfirmOverWrite = FlagGetChangeSetting(TextParameter);
					break;

				case CommandFlagConfirmOverWriteRoot:	/* FlagConfirmOverWriteRoot */
					/* FlagConfirmOverWriteRoot [true/false] */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagConfirmOverWriteRoot = FlagGetChangeSetting(TextParameter);
					break;

				case CommandFlagConfirmOverWriteRootEffect:	/* FlagConfirmOverWriteRootEffect */
					/* FlagConfirmOverWriteRootEffect [true/false] */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagConfirmOverWriteRootEffect = FlagGetChangeSetting(TextParameter);
					break;

				case CommandFlagConfirmOverWriteDataCellMap:	/* FlagConfirmOverWriteDataCellMap */
					/* FlagConfirmOverWriteDataCellMap [true/false] */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagConfirmOverWriteDataCellMap = FlagGetChangeSetting(TextParameter);
					break;

				case CommandFlagConfirmOverWriteDataAnimation:	/* FlagConfirmOverWriteDataAnimation */
					/* FlagConfirmOverWriteDataAnimation [true/false] */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagConfirmOverWriteDataAnimation = FlagGetChangeSetting(TextParameter);
					break;

				case CommandFlagConfirmOverWriteDataEffect:	/* FlagConfirmOverWriteDataEffect */
					/* FlagConfirmOverWriteDataEffect [true/false] */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagConfirmOverWriteDataEffect = FlagGetChangeSetting(TextParameter);
					break;

				case CommandFlagConfirmOverWriteMaterial:	/* FlagConfirmOverWriteMaterial */
					/* FlagConfirmOverWriteMaterial [true/false] */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagConfirmOverWriteMaterial = FlagGetChangeSetting(TextParameter);
					break;

				case CommandFlagConfirmOverWriteTexture:	/* FlagConfirmOverWriteTexture */
					/* FlagConfirmOverWriteTexture [true/false] */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagConfirmOverWriteTexture = FlagGetChangeSetting(TextParameter);
					break;

				case CommandFlagGetAnimationReferencedPartsRoot:	/* FlagGetAnimationReferencedPartsRoot */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagGetAnimationReferencedPartsRoot = FlagGetChangeSetting(TextParameter);
					break;

				case CommandFlagGetCellMapReferencedPartsRoot:	/* FlagGetCellMapReferencedPartsRoot */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagGetCellMapReferencedPartsRoot = FlagGetChangeSetting(TextParameter);
					break;

				case CommandFlagGetMaterialPartsRoot:	/* FlagGetMaterialPartsRoot */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagGetMaterialPartsRoot = FlagGetChangeSetting(TextParameter);
					break;

				case CommandFlagGetTextureMaterial:	/* FlagGetTextureMaterial */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagGetTextureMaterial = FlagGetChangeSetting(TextParameter);
					break;

				default:	/* Command Error */
					return(false);
			}
			return(true);
		}
		private static bool FlagGetChangeSetting(string Text)
		{
			string TextParameter = TextTrim(Text);
			switch(TextParameter)
			{
				case TextTrue:
					return(true);

				case TextFalse:
					return(false);
			}
			return(false);
		}
		private static int IntGetChangeSetting(string Text)
		{
			return(System.Convert.ToInt32(Text));
		}
		private static float FloatGetChangeSetting(string Text)
		{
			return(System.Convert.ToSingle(Text));
		}

		internal static bool Import(ref LibraryEditor_SpriteStudio.SettingImport DataSettingImport, string TextLine)
		{
			/* Calling Importer */
			string NameFileInput = TextTrim(TextLine);
			bool Rv = false;
			if(false == String.IsNullOrEmpty(NameFileInput))
			{
				if(false == String.IsNullOrEmpty(NameBaseFolderSS5Data))
				{
					NameFileInput = NameBaseFolderSS5Data + "/" + NameFileInput;
				}
				Rv = LibraryEditor_SpriteStudio.Menu.ImportSSPJ(	DataSettingImport,
																	NameFileInput,
																	NameBaseFolderAsset,
																	false,					/* Import-Setting Not-Saved */
																	true,					/* Display Progress-Bar */
																	false					/* Don't Stop, Error Importing-SSPJ */
					);
			}
			return(Rv);
		}
	}
}
