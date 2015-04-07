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
			/* MEMO: Default-Values are same SSPJ-Importer's Default. (except "FlagConfirmOverWrite") */
			LibraryEditor_SpriteStudio.SettingImport DataSettingImport = new LibraryEditor_SpriteStudio.SettingImport();
			DataSettingImport.TextureSizePixelMaximum = 8192;
			DataSettingImport.CollisionThicknessZ = 1.0f;
			DataSettingImport.FlagAttachRigidBody = true;
			DataSettingImport.FlagAttachControlGameObject = true;
			DataSettingImport.FlagConfirmOverWrite = false;		/* Caution!: This item is different */
			DataSettingImport.FlagCreateProjectFolder = true;
			DataSettingImport.FlagGetAnimationReferencedPartsRoot = false;
			DataSettingImport.FlagGetMaterialPartsRoot = false;
			DataSettingImport.FlagGetTextureMaterial = false;

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
		private const string CommandTextureSizePixelMaximum = "TextureSizePixelMaximum";

		private const string CommandCollisionThicknessZ = "CollisionThicknessZ";
		private const string CommandFlagAttachRigidBody = "FlagAttachRigidBody";
		private const string CommandFlagAttachControlGameObject = "FlagAttachControlGameObject";
		private const string CommandFlagConfirmOverWrite = "FlagConfirmOverWrite";
		private const string CommandFlagCreateProjectFolder = "FlagCreateProjectFolder";

		private const string CommandFlagGetAnimationReferencedPartsRoot = "FlagGetAnimationReferencedPartsRoot";
		private const string CommandFlagGetMaterialPartsRoot = "FlagGetMaterialPartsRoot";
		private const string CommandFlagGetTextureMaterial = "FlagGetTextureMaterial";

		private const string CommandNameBaseFolderSS5Data = "NameBaseFolderSS5Data";
		private const string CommandNameBaseFolderAsset = "NameBaseFolderAsset";

		private const string CommandFlagDataCalculateInAdvance = "FlagDataCalculateInAdvance";

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
				case CommandTextureSizePixelMaximum:	/* TextureSizePixelMaximum */
					/* TextureSizePixelMaximum [value] */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.TextureSizePixelMaximum = IntGetChangeSetting(TextParameter);
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

				case CommandFlagAttachControlGameObject:	/* FlagAttachControlGameObject */
					/* FlagAttachControlGameObject [true/false] */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagAttachControlGameObject = FlagGetChangeSetting(TextParameter);
					break;

				case CommandFlagConfirmOverWrite:	/* FlagConfirmOverWrite */
					/* FlagConfirmOverWrite [true/false] */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagConfirmOverWrite = FlagGetChangeSetting(TextParameter);
					break;

				case CommandFlagCreateProjectFolder:	/* FlagCreateProjectFolder */
					/* FlagCreateProjectFolder [true/false] */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagCreateProjectFolder = FlagGetChangeSetting(TextParameter);
					break;

				case CommandFlagGetAnimationReferencedPartsRoot:	/* FlagGetAnimationReferencedPartsRoot */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagGetAnimationReferencedPartsRoot = FlagGetChangeSetting(TextParameter);
					break;

				case CommandFlagGetMaterialPartsRoot:	/* FlagGetMaterialPartsRoot */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagGetMaterialPartsRoot = FlagGetChangeSetting(TextParameter);
					break;

				case CommandFlagGetTextureMaterial:	/* FlagGetTextureMaterial */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagGetTextureMaterial = FlagGetChangeSetting(TextParameter);
					break;

				case CommandFlagDataCalculateInAdvance:	/* FlagDataCalculateInAdvance */
					TextParameter = String.Copy(Arguments[1]);
					DataSettingImport.FlagDataCalculateInAdvance = FlagGetChangeSetting(TextParameter);
					break;

				case CommandNameBaseFolderSS5Data:	/* NameBaseFolderSS5Data */
					/* NameBaseFolderSS5Data [Base Folder Name] */
					TextParameter = String.Copy(Arguments[1]);
					NameBaseFolderSS5Data = TextTrim(TextParameter);
					break;

				case CommandNameBaseFolderAsset:	/* NameBaseFolderAsset */
					/* NameBaseFolderAsset [Base Asset-Folder Name] */
					TextParameter = String.Copy(Arguments[1]);
					NameBaseFolderAsset = TextTrim(TextParameter);
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
