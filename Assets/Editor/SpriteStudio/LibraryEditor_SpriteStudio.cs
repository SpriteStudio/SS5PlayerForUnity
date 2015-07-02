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
	private readonly static int ShaderOperationMax = (int)Library_SpriteStudio.KindColorOperation.TERMINATOR - 1;

	public static class Utility
	{
		public static void HideSetForce(GameObject InstanceGameObject, bool FlagSwitch, bool FlagSetChild, bool FlagSetInstance)
		{
			GameObject InstanceGameObjectNow = InstanceGameObject;
			Transform InstanceTransform = InstanceGameObjectNow.transform;
			Script_SpriteStudio_Triangle2 ScriptTriangle2 = InstanceGameObjectNow.GetComponent<Script_SpriteStudio_Triangle2>();
			if(null != ScriptTriangle2)
			{
				ScriptTriangle2.FlagHideForce = FlagSwitch;
				EditorUtility.SetDirty(ScriptTriangle2);
			}
			else
			{
				Script_SpriteStudio_Triangle4 ScriptTriangle4 = InstanceGameObjectNow.GetComponent<Script_SpriteStudio_Triangle4>();
				if(null != ScriptTriangle4)
				{
					ScriptTriangle4.FlagHideForce = FlagSwitch;
					EditorUtility.SetDirty(ScriptTriangle4);
				}
				else
				{
					Script_SpriteStudio_PartsInstance ScriptInstasnce = InstanceGameObjectNow.GetComponent<Script_SpriteStudio_PartsInstance>();
					if(null != ScriptInstasnce)
					{
						ScriptInstasnce.FlagHideForce = FlagSwitch;
						EditorUtility.SetDirty(ScriptInstasnce);
					}
					else
					{
						Script_SpriteStudio_PartsRoot ScriptRoot = InstanceGameObjectNow.GetComponent<Script_SpriteStudio_PartsRoot>();
						if((false == FlagSetInstance) && (null != ScriptRoot.PartsRootOrigin))
						{	/* "Instance"-Object */
							return;
						}
					}
				}
			}
			if(true == FlagSetChild)
			{
				for(int i=0; i<InstanceTransform.childCount; i++)
				{
					HideSetForce(InstanceTransform.GetChild(i).gameObject, FlagSwitch, FlagSetChild, FlagSetInstance);
				}
			}
		}

		internal static int IndexGetOrderedDictionary(OrderedDictionary DataList, string Key)
		{
			int Count = DataList.Count;
			if(0 < Count)
			{
				ICollection DictionaryKey = DataList.Keys;
				string[] NameKeys = new string[Count];
				DictionaryKey.CopyTo(NameKeys, 0);
				for(int i=0; i<Count; i++)
				{
					if(0 == String.Compare(Key, NameKeys[i]))
					{
						return(i);
					}
				}
			}
			return(-1);
		}
	}
	
	/* Interfaces between Unity's-Menu-Script and OPSS(SS5)Data-Importing-Script */
	public struct SettingImport
	{
		public int TextureSizePixelMaximum;
		public float CollisionThicknessZ;
		public bool FlagAttachRigidBody;
		public bool FlagAttachControlGameObject;
		public bool FlagConfirmOverWrite;
		public bool FlagCreateProjectFolder;
		public bool FlagGetAnimationReferencedPartsRoot;
		public bool FlagGetMaterialPartsRoot;
		public bool FlagGetTextureMaterial;
		public bool FlagDataCalculateInAdvance;
	}
	internal readonly static string PrefsKeyTextureSizePixelMaximum = "SS5PU_Importer_TextureSizePixelMaximum";
	internal readonly static string PrefsKeyCollisionThicknessZ = "SS5PU_Importer_CollisionThicknessZ";
	internal readonly static string PrefsKeyFlagAttachRigidBody = "SS5PU_Importer_FlagAttachRigidBody";
	internal readonly static string PrefsKeyFlagAttachControlGameObject = "SS5PU_Importer_FlagAttachControlGameObject";
	internal readonly static string PrefsKeyFlagConfirmOverWrite = "SS5PU_Importer_FlagConfirmOverWrite";
	internal readonly static string PrefsKeyFlagCreateProjectFolder = "SS5PU_Importer_FlagCreateProjectFolder";
	internal readonly static string PrefsKeyFlagGetAnimationReferencedPartsRoot = "SS5PU_Importer_FlagGetAnimationReferencedPartsRoot";
	internal readonly static string PrefsKeyFlagGetMaterialPartsRoot = "SS5PU_Importer_FlagGetMaterialPartsRoot";
	internal readonly static string PrefsKeyFlagGetTextureMaterial = "SS5PU_Importer_FlagGetTextureMaterial";
	internal readonly static string PrefsKeyFlagDataCalculateInAdvance = "SS5PU_Importer_FlagDataCalculateInAdvance";
	internal readonly static string PrefsKeyFolderNameImpoertLast = "SS5PU_Importer_FolderNameImpoertLast";

	public static partial class Menu
	{
		/* Information for Containing "Instance"-Parts */
		public class InformationInstance
		{
			public int ID;
			public string NameInstanceSSAE;
			public string NameInstanceAnimation;

			public InformationInstance()
			{
				ID = -1;
				NameInstanceSSAE = "";
				NameInstanceAnimation = "";
			}
		}

		internal static void SettingClearImport()
		{
			EditorPrefs.SetInt(PrefsKeyTextureSizePixelMaximum, 4096);
			EditorPrefs.SetFloat(PrefsKeyCollisionThicknessZ, 1.0f);
			EditorPrefs.SetBool(PrefsKeyFlagAttachRigidBody, true);
			EditorPrefs.SetBool(PrefsKeyFlagAttachControlGameObject, true);
			EditorPrefs.SetBool(PrefsKeyFlagConfirmOverWrite, true);
			EditorPrefs.SetBool(PrefsKeyFlagCreateProjectFolder, true);
			EditorPrefs.SetBool(PrefsKeyFlagGetAnimationReferencedPartsRoot, true);
			EditorPrefs.SetBool(PrefsKeyFlagGetMaterialPartsRoot, true);
			EditorPrefs.SetBool(PrefsKeyFlagGetTextureMaterial, true);
			EditorPrefs.SetBool(PrefsKeyFlagDataCalculateInAdvance, true);
			EditorPrefs.SetString(PrefsKeyFolderNameImpoertLast, "");
		}

		internal static void SettingGetImport(out SettingImport DataSettingImport)
		{
			DataSettingImport.TextureSizePixelMaximum = EditorPrefs.GetInt(PrefsKeyTextureSizePixelMaximum, 4096);
			DataSettingImport.CollisionThicknessZ = EditorPrefs.GetFloat(PrefsKeyCollisionThicknessZ, 1.0f);
			DataSettingImport.FlagAttachRigidBody = EditorPrefs.GetBool(PrefsKeyFlagAttachRigidBody, true);
			DataSettingImport.FlagAttachControlGameObject = EditorPrefs.GetBool(PrefsKeyFlagAttachControlGameObject, true);
			DataSettingImport.FlagConfirmOverWrite = EditorPrefs.GetBool(PrefsKeyFlagConfirmOverWrite, true);
			DataSettingImport.FlagCreateProjectFolder = EditorPrefs.GetBool(PrefsKeyFlagCreateProjectFolder, true);
			DataSettingImport.FlagGetAnimationReferencedPartsRoot = EditorPrefs.GetBool(PrefsKeyFlagGetAnimationReferencedPartsRoot, true);
			DataSettingImport.FlagGetMaterialPartsRoot = EditorPrefs.GetBool(PrefsKeyFlagGetMaterialPartsRoot, true);
			DataSettingImport.FlagGetTextureMaterial = EditorPrefs.GetBool(PrefsKeyFlagGetTextureMaterial, true);
			DataSettingImport.FlagDataCalculateInAdvance = EditorPrefs.GetBool(PrefsKeyFlagDataCalculateInAdvance, true);
		}
		internal static void SettingSetImport(ref SettingImport DataSettingImport)
		{
			EditorPrefs.SetInt(PrefsKeyTextureSizePixelMaximum, DataSettingImport.TextureSizePixelMaximum);
			EditorPrefs.SetFloat(PrefsKeyCollisionThicknessZ, DataSettingImport.CollisionThicknessZ);
			EditorPrefs.SetBool(PrefsKeyFlagAttachRigidBody, DataSettingImport.FlagAttachRigidBody);
			EditorPrefs.SetBool(PrefsKeyFlagAttachControlGameObject, DataSettingImport.FlagAttachControlGameObject);
			EditorPrefs.SetBool(PrefsKeyFlagConfirmOverWrite, DataSettingImport.FlagConfirmOverWrite);
			EditorPrefs.SetBool(PrefsKeyFlagCreateProjectFolder, DataSettingImport.FlagCreateProjectFolder);
			EditorPrefs.SetBool(PrefsKeyFlagGetAnimationReferencedPartsRoot, DataSettingImport.FlagGetAnimationReferencedPartsRoot);
			EditorPrefs.SetBool(PrefsKeyFlagGetMaterialPartsRoot, DataSettingImport.FlagGetMaterialPartsRoot);
			EditorPrefs.SetBool(PrefsKeyFlagGetTextureMaterial, DataSettingImport.FlagGetTextureMaterial);
			EditorPrefs.SetBool(PrefsKeyFlagDataCalculateInAdvance, DataSettingImport.FlagDataCalculateInAdvance);
		}
		internal static void SettingGetFolderImport(out string NameFolder)
		{
			string Name64 = "";
			Name64 = EditorPrefs.GetString(PrefsKeyFolderNameImpoertLast, "");
			NameFolder = String.Copy(UTF8Encoding.UTF8.GetString(System.Convert.FromBase64String(Name64)));
		}
		internal static void SettingSetFolderImport(ref string NameFolder)
		{
			string Name64 = System.Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(NameFolder));
			EditorPrefs.SetString(PrefsKeyFolderNameImpoertLast, Name64);
		}

		/* Caution: "NameInputFullPathSSPJ" must be Full-Path Name. */
		/* Caution: "NameOutputAssetFolder" must be Relative-Path Name("Assets/"). */
		public static bool ImportSSPJ(SettingImport DataSettingImport, string NameInputFullPathSSPJ="", string NameOutputAssetFolderBase="", bool FlagSaveSetting=true, bool FlagDisplayProgressBar=true, bool FlagDisplayErrorDialog=true)
		{
			bool FlagValidSSPJ = String.IsNullOrEmpty(NameInputFullPathSSPJ);
			string NameFileBodySSAE = "";

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
				if(false == File.FileNameGetFileDialog(	out NameDirectory,
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
			int StepNow = 0;
			int StepFull = 0;
			if(true == FlagDisplayProgressBar)
			{
				ProgressBarUpdate("Decoding Project Files", 0, 1);
			}

			/* ".sspj" Import */
			ParseOPSS.InformationSSPJ InformationSSPJ = ParseOPSS.ImportSSPJ(NameDirectory, NameFileBody + NameFileExtension);
			if(null == InformationSSPJ)
			{
				goto Menu_ImportSSPJ_ErrorEnd;
			}
			StepFull = 1 + (InformationSSPJ.ListSSCE.Count + InformationSSPJ.ListSSAE.Count) * 2;	/* BaseFolderGet + ((SSCE + SSAE) * (Decoding + Create) */

			/* ".ssce" Import */
			int Count = InformationSSPJ.ListSSCE.Count;
			DataIntermediate.PartsImage[] DataListImage = new DataIntermediate.PartsImage[Count];
			for(int i=0; i<Count; i++)
			{
				StepNow++;
				if(true == FlagDisplayProgressBar)
				{
					ProgressBarUpdate(	"Decoding SSCE Files: " + i.ToString() + "-" + Count.ToString(),
										StepNow,
										StepFull
									);
				}

				DataListImage[i] = new DataIntermediate.PartsImage();
				DataListImage[i].CleanUp();
				if(false == ParseOPSS.ImportSSCE(	ref DataListImage[i],
													ref InformationSSPJ,
													InformationSSPJ.NameDirectorySSCE,
													InformationSSPJ.NameDirectoryImage,
													(string)InformationSSPJ.ListSSCE[i]
												)
					)
				{
					goto Menu_ImportSSPJ_ErrorEnd;
				}
			}

			/* ".ssae" Import */
			Count = InformationSSPJ.ListSSAE.Count;
			DataIntermediate.TrunkParts[] DataOutput = new DataIntermediate.TrunkParts[Count];
			for(int i=0; i<Count; i++)
			{
				StepNow++;
				if(true == FlagDisplayProgressBar)
				{
					ProgressBarUpdate(	"Decoding SSAE Files: " + i.ToString() + "-" + Count.ToString(),
										StepNow,
										StepFull
									);
				}

				/* Create Data-Trunk */
				DataOutput[i] = new DataIntermediate.TrunkParts();
				DataOutput[i].BootUp();
				DataOutput[i].ListImage = DataListImage;

				/* Convert Data */
				if(false == ParseOPSS.ImportSSAE(ref DataOutput[i], InformationSSPJ.NameDirectorySSAE, (string)InformationSSPJ.ListSSAE[i], InformationSSPJ))
				{
					goto Menu_ImportSSPJ_ErrorEnd;
				}
			}

			/* Importing Base-Folder Get & Create Destination-Folders */
			string NamePathBase = "";
			if(true == String.IsNullOrEmpty(NameOutputAssetFolderBase))
			{	/* Select */
				NamePathBase = AssetUtility.NamePathGetSelectNow(null);
				if(true == String.IsNullOrEmpty(NamePathBase))
				{
					Debug.LogError("SSPJ Importing Error: Please select the folder you want to store in before import.");
					goto Menu_ImportSSPJ_ErrorEnd;
				}
			}
			else
			{	/* Force */
				if(false == System.IO.Directory.Exists(NameOutputAssetFolderBase))
				{	/* Not Found */
					if(true == FlagDisplayProgressBar)
					{
						ProgressBarUpdate("Import Stop", -1, -1);
					}
					Debug.LogError("SSPJ Importing Error: Output-Base-Folder Not Found [" + NameOutputAssetFolderBase + "]");
					return(false);
				}
				NamePathBase = String.Copy(NameOutputAssetFolderBase);
			}

			/* Create Destination Folder (Base) */
			NamePathBase = DataIntermediate.TrunkParts.CreateDestinationFolder(	NamePathBase,
																				(true == DataSettingImport.FlagCreateProjectFolder) ? NameFileBody : null
																			);
			StepNow++;

			/* Get Old-Prefabs */
			/* MEMO: Need not Instantiate, for Access Root-Node */
			Count = InformationSSPJ.ListSSAE.Count;
			for(int i=0; i<Count; i++)
			{
				NameFileBodySSAE = Path.GetFileNameWithoutExtension((string)InformationSSPJ.ListSSAE[i]);
				string NamePathPrefabOld = NamePathBase + "/" + DataIntermediate.NamePathSubImportPrefab + "/" + NameFileBodySSAE + "." + DataIntermediate.NameExtensionPrefab;
				DataOutput[i].PrefabDataOld = AssetDatabase.LoadAssetAtPath(NamePathPrefabOld, typeof(GameObject));
			}

			/* Materials Creating */
			if(true == FlagDisplayProgressBar)
			{
				ProgressBarUpdate(	"Creating Materials",
									StepNow,
									StepFull
								);
			}

			Count = InformationSSPJ.ListSSAE.Count;
			int[][] TableIndexMaterial = new int[Count][];
			OrderedDictionary ListMaterial = new OrderedDictionary();	/* Key=AssetPath / value=DataIntermediate.TrunkParts.InformationMaterial */
			OrderedDictionary ListTexture = new OrderedDictionary();	/* Key=AssetPath / value=DataIntermediate.TrunkParts.InformationTexture */
			ListMaterial.Clear();
			ListTexture.Clear();
			for(int i=0; i<Count; i++)
			{
				/* Get Old Materials & Textures */
				NameFileBodySSAE = Path.GetFileNameWithoutExtension((string)InformationSSPJ.ListSSAE[i]);
				DataOutput[i].GetAssetMaterial(	out TableIndexMaterial[i],
												ListMaterial,
												ListTexture,
												NamePathBase,
												NameFileBody,	/* NameFileBodySSAE, */
												ref DataSettingImport
											);
			}
			Material[][] TableMaterial = new Material[Count][];

			int CountListMaterial = ListMaterial.Count;
			int CountListTexture = ListTexture.Count;
			if(0 < CountListMaterial)
			{
				bool[] FlagCreateMaterial = new bool[CountListMaterial];
				for(int i=0; i<CountListMaterial; i++)
				{
					FlagCreateMaterial[i] = false;
				}

				CountListTexture = (0 < CountListTexture) ? CountListTexture : 1;
				bool[] FlagCreateTexture = new bool[CountListTexture];
				for(int i=0; i<CountListTexture; i++)
				{
					FlagCreateTexture[i] = false;
				}

				for(int i=0; i<Count; i++)
				{
					/* Update Materials & Textures */
					DataOutput[i].UpdateAssetMaterial(	out TableMaterial[i],
														ref FlagCreateMaterial,
														ref FlagCreateTexture,
														TableIndexMaterial[i],
														ListMaterial,
														ListTexture,
														NameFileBody,
														NamePathBase,
														ref DataSettingImport
													);
				}
			}
			ListMaterial.Clear();
			ListTexture.Clear();

			StepNow += InformationSSPJ.ListSSCE.Count;

			/* Animations Creating */
			Count = InformationSSPJ.ListSSAE.Count;
			for(int i=0; i<Count; i++)
			{
				/* Progress-Bar Update */
				StepNow++;
				if(true == FlagDisplayProgressBar)
				{
					ProgressBarUpdate(	"Creating Animation Prefabs: " + i.ToString() + "-" + Count.ToString(),
										StepNow,
										StepFull
									);
				}

				/* GameObjects Create */
				NameFileBodySSAE = Path.GetFileNameWithoutExtension((string)InformationSSPJ.ListSSAE[i]);
				if(false == DataOutput[i].CreateDataGameObjectSprite(	NameFileBodySSAE,
																		NamePathBase,
																		TableMaterial[i],
																		ref DataSettingImport
																	)
					)
				{
					if(null != DataOutput[i].GameObjectRoot)
					{
						UnityEngine.Object.DestroyImmediate(DataOutput[i].GameObjectRoot);
						DataOutput[i].GameObjectRoot = null;
					}

					Debug.LogError("SSAE-Convert-GameObject Error:" + NameFileBodySSAE);
					goto Menu_ImportSSPJ_ErrorEnd;
				}
			}

			/* Solving Prefab-Create-Order */
			int[] OrderCreatePrefab = new int[Count];
			for(int i=0; i<Count; i++)
			{
				OrderCreatePrefab[i] = -1;
			}
			{
				/* Set "Has no Instance-Pars" Animation */
				int IndexNow = 0;
				for(int i=0; i<Count; i++)
				{
					if(0 >= DataOutput[i].ListPartsInstance.Count)
					{	/* Has no Instance-Parts */
						OrderCreatePrefab[IndexNow] = i;
						IndexNow++;
					}
				}

				/* Set "Has Instance-Parts" */
				bool FlagAllInstanceExist = false;
				bool FlagAlreadyOrdered = false;
				while(Count > IndexNow)
				{
					for(int i=0; i<Count; i++)
					{
						/* Check Already-Ordered */
						FlagAlreadyOrdered = false;
						for(int j=0; j<IndexNow; j++)
						{
							if(i == OrderCreatePrefab[j])
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
						if(false == OrderCheckInstancePartsPrefabed(	ref FlagAllInstanceExist,
																		DataOutput[i].ListPartsInstance,
																		OrderCreatePrefab,
																		IndexNow,
																		InformationSSPJ
																	)
							)
						{	/* Error (Not Found SSAE-Name) */
							Debug.LogError("Instance Calling Data Name:" + NameFileBodySSAE);
							goto Menu_ImportSSPJ_ErrorEnd;
						}

						if(true == FlagAllInstanceExist)
						{	/* All Instance-Parts Orderd */
							OrderCreatePrefab[IndexNow] = i;
							IndexNow++;
							break;	/* Break for-Loop */
						}
					}
				}
			}

			/* Prefab Create & Destroy Old-PrefabData */
			int IndexOrder;
			for(int i=0; i<Count; i++)
			{
				IndexOrder = OrderCreatePrefab[i];
				NameFileBodySSAE = Path.GetFileNameWithoutExtension((string)InformationSSPJ.ListSSAE[IndexOrder]);
				if(false == DataOutput[IndexOrder].CreateDataPrefabSprite(	DataOutput,
																			NameFileBodySSAE,
																			NamePathBase,
																			InformationSSPJ,
																			ref DataSettingImport
																		)
					)
				{
					Debug.LogError("SSAE-Convert-Prefab Error:" + NameFileBodySSAE);
					goto Menu_ImportSSPJ_ErrorEnd;
				}
			}

			/* End of Importing (Success) */
			if(true == FlagDisplayProgressBar)
			{
				ProgressBarUpdate("Import End", -1, -1);
			}
			return(true);

		Menu_ImportSSPJ_ErrorEnd:;
			/* End of Importing (Failure) */
			if(true == FlagDisplayProgressBar)
			{
				ProgressBarUpdate("Import Stop", -1, -1);
			}
			if(true == FlagDisplayErrorDialog)
			{
				EditorUtility.DisplayDialog(	"SpriteStudio5 Player for Unity",
											"Import Interrupted! Check Error on Console.",
									 		"OK"
										);
			}
			return(false);
		}
		private static void ProgressBarUpdate(string NowTaskName, int Step, int StepFull)
		{
			if((-1 == Step) || (-1 == StepFull))
			{
				EditorUtility.ClearProgressBar();
			}
			else
			{
				EditorUtility.DisplayProgressBar(	"Importing SpriteStudio Animation",
													NowTaskName,
													(float)Step / (float)StepFull
												);
			}
		}
		private static bool OrderCheckInstancePartsPrefabed(	ref bool FlagExistAll,
																ArrayList ArrayInstanceInformation,
																int[] OrderIndex,
																int CountOrdered,
																ParseOPSS.InformationSSPJ InformationSSPJ
			)
		{
			LibraryEditor_SpriteStudio.Menu.InformationInstance InformationInstance = null;
			int Count = ArrayInstanceInformation.Count;
			int IndexInstance = -1;
			bool FlagExist = false;
			FlagExistAll = true;
			for(int i=0; i<Count; i++)
			{
				InformationInstance = ArrayInstanceInformation[i] as LibraryEditor_SpriteStudio.Menu.InformationInstance;
				IndexInstance = InformationSSPJ.ArraySearchFileNameBody(InformationSSPJ.ListSSAE, InformationInstance.NameInstanceSSAE);
				if(-1 == IndexInstance)
				{	/* Instance-Parts is not in SSPJ */
					Debug.LogError("Instance PartsName is not found Error:" + InformationSSPJ.ListSSAE);
					FlagExistAll = false;
					return(false);
				}

				FlagExist = false;
				for(int j=0; j<CountOrdered; j++)
				{
					if(IndexInstance == OrderIndex[j])
					{
						FlagExist = true;
						break;
					}
				}
				FlagExistAll = (false == FlagExist) ? false : FlagExistAll;
			}
			return(true);
		}
	}

	/// <summary>
	/// The parts currently processed while importing.
	/// </summary>
	static	internal string	CurrentProcessingPartsName;

	/* Functions for Parsing OPSS(SS5) Datas */
	internal static class ParseOPSS
	{
		/* for Parsing ".sspj" */
		internal static InformationSSPJ ImportSSPJ(string DataPathBase, string FileName)
		{
			string MessageError = "";

			XmlDocument DataXML = new XmlDocument();
			DataXML.Load(DataPathBase + "/" + FileName);

			/* Version-Check */
			InformationSSPJ InformationProject = null;
			XmlNode NodeRoot = DataXML.FirstChild;
			NodeRoot = NodeRoot.NextSibling;
			KindVersionSSPJ VersionCode = (KindVersionSSPJ)(VersionCodeGet(NodeRoot, "SpriteStudioProject", (int)KindVersionSSPJ.ERROR, true));
			switch(VersionCode)
			{
				case KindVersionSSPJ.VERSION_000100:
				case KindVersionSSPJ.VERSION_010000:
				case KindVersionSSPJ.VERSION_010200:
					break;

				case KindVersionSSPJ.ERROR:
				default:
					MessageError = "Not Supported Version.";
					goto ParseOPSS_ImportSSPJ_ErrorEnd;
			}

			/* Get Directories */
			InformationProject = new InformationSSPJ();
			if(null == InformationProject)
			{
				MessageError = "Not Enough Memory.";
				goto ParseOPSS_ImportSSPJ_ErrorEnd;
			}

			InformationProject.VersionCode = VersionCode;

			NameTable NodeNameSpace = new NameTable();
			XmlNamespaceManager ManagerNameSpace = new XmlNamespaceManager(NodeNameSpace);
			XmlNodeList NodeList = null;

			InformationProject.NameDirectorySSAE = string.Copy(DataPathBase);
			string ValueText = "";
			ValueText = XMLUtility.TextGetSelectSingleNode(NodeRoot, "settings/animeBaseDirectory", ManagerNameSpace);
			if(false == string.IsNullOrEmpty(ValueText))
			{
				InformationProject.NameDirectorySSAE += "/" + ValueText;
			}
			InformationProject.NameDirectorySSCE = string.Copy(DataPathBase);
			ValueText = XMLUtility.TextGetSelectSingleNode(NodeRoot, "settings/cellMapBaseDirectory", ManagerNameSpace);
			if(false == string.IsNullOrEmpty(ValueText))
			{
				InformationProject.NameDirectorySSCE += "/" + ValueText;
			}
			InformationProject.NameDirectoryImage = string.Copy(DataPathBase);
			ValueText = XMLUtility.TextGetSelectSingleNode(NodeRoot, "settings/imageBaseDirectory", ManagerNameSpace);
			if(false == string.IsNullOrEmpty(ValueText))
			{
				InformationProject.NameDirectoryImage += "/" + ValueText;
			}

			/* Get Cell-Maps */
			NodeList = XMLUtility.XML_SelectNodes(NodeRoot, "cellmapNames/value", ManagerNameSpace);
			if(null == NodeList)
			{
				MessageError = "CellMapNameList-Node Not-Found.";
				goto ParseOPSS_ImportSSPJ_ErrorEnd;
			}
			foreach(XmlNode NodeNameCellMap in NodeList)
			{
				string NameFileName = NodeNameCellMap.InnerText;
				InformationProject.AddSSCE(NameFileName);
			}

			/* Get Animations */
			NodeList = XMLUtility.XML_SelectNodes(NodeRoot, "animepackNames/value", ManagerNameSpace);
			if(null == NodeList)
			{
				MessageError = "AnimePackNameList-Node Not-Found.";
				goto ParseOPSS_ImportSSPJ_ErrorEnd;
			}
			foreach(XmlNode NodeNameAnimation in NodeList)
			{
				string NameFileName = NodeNameAnimation.InnerText;
				InformationProject.AddSSAE(NameFileName);
			}

			return(InformationProject);

		ParseOPSS_ImportSSPJ_ErrorEnd:;
			Debug.LogError("SSPJ-Import Error: " + MessageError + " (" + FileName + ")");
			return(null);
		}
		internal enum KindVersionSSPJ
		{
			ERROR = 0x00000000,
			VERSION_000100  = 0x00000100,
			VERSION_010000  = 0x00010000,
			VERSION_010200  = 0x00010200,	/* sspj ver. 5.5.0 beta-3 */

			VERSION_REQUIRED = VERSION_000100,
			VERSION_CURRENT = VERSION_010000,
		};
		internal class InformationSSPJ
		{
			internal KindVersionSSPJ VersionCode;
			internal ArrayList ListSSCE;
			internal ArrayList ListSSAE;
			internal string NameDirectorySSCE;
			internal string NameDirectorySSAE;
			internal string NameDirectoryImage;

			internal void CleanUp()
			{
				VersionCode = LibraryEditor_SpriteStudio.ParseOPSS.KindVersionSSPJ.ERROR;
				ListSSCE = null;
				ListSSAE = null;
				NameDirectorySSCE = "";
				NameDirectorySSAE = "";
				NameDirectoryImage = "";
			}

			internal void AddSSCE(string FileName)
			{
				if(null == ListSSCE)
				{
					ListSSCE = new ArrayList();
					ListSSCE.Clear();
				}

				if(0 <= ArraySearchFileName(ListSSCE, FileName))
				{
					return;
				}

				string FileNameNew = string.Copy(FileName);
				ListSSCE.Add(FileNameNew);
			}

			internal void AddSSAE(string FileName)
			{
				if(null == ListSSAE)
				{
					ListSSAE = new ArrayList();
					ListSSAE.Clear();
				}

				if(0 <= ArraySearchFileName(ListSSAE, FileName))
				{
					return;
				}

				string FileNameNew = string.Copy(FileName);
				ListSSAE.Add(FileNameNew);
			}

			internal int ArraySearchFileName(ArrayList ListFileName, string FileName)
			{
				for(int i=0; i<ListFileName.Count; i++)
				{
					string FileNameNow = ListFileName[i] as string;
					if(0 == FileName.CompareTo(FileNameNow))
					{
						return(i);
					}
				}
				return(-1);
			}

			internal int ArraySearchFileNameBody(ArrayList ListFileName, string FileName)
			{
				for(int i=0; i<ListFileName.Count; i++)
				{
					string FileNameNow = ListFileName[i] as string;
					string FileNameBody = Path.GetFileNameWithoutExtension(FileNameNow);
					if(0 == FileName.CompareTo(FileNameBody))
					{
						return(i);
					}
				}
				return(-1);
			}
		}

		/* for Parsing ".ssce" */
		internal static bool ImportSSCE(ref DataIntermediate.PartsImage InformationCellMap,
										ref InformationSSPJ InformationProject,
										string NameDirectoryCellMap,
										string NameDirectoryImage,
										string FileName
									)
		{
			string MessageError = "";
			string NameOptions = "";

			XmlDocument DataXML = new XmlDocument();
			DataXML.Load(NameDirectoryCellMap + "/" + FileName);

			XmlNode NodeRoot = DataXML.FirstChild;
			NodeRoot = NodeRoot.NextSibling;
			KindVersionSSCE VersionCode = (KindVersionSSCE)(VersionCodeGet(NodeRoot, "SpriteStudioCellMap", (int)KindVersionSSCE.ERROR, true));
			switch(VersionCode)
			{
				case KindVersionSSCE.VERSION_000100:
				case KindVersionSSCE.VERSION_010000:
					break;

				case KindVersionSSCE.ERROR:
				default:
					MessageError = "Not Supported Version.";
					goto ParseOPSS_ImportSSCE_ErrorEnd;
			}

			NameTable NodeNameSpace = new NameTable();
			XmlNamespaceManager ManagerNameSpace = new XmlNamespaceManager(NodeNameSpace);
			XmlNodeList NodeList = null;

			/* Get Texture Path-Name */
			string NameTexture = XMLUtility.TextGetSelectSingleNode(NodeRoot, "imagePath", ManagerNameSpace);
			if(0 == string.Compare(NameDirectoryCellMap, string.Empty))
			{
				NameDirectoryCellMap = Path.GetDirectoryName(FileName);
			}

			if(true == Path.IsPathRooted(NameTexture))
			{
				InformationCellMap.FileName = string.Copy(NameTexture);
			}
			else
			{
				InformationCellMap.FileName = Path.GetFullPath(NameDirectoryImage + "/" + NameTexture);
			}

			/* Get Texture Wrap-Mode */
			NameOptions = XMLUtility.TextGetSelectSingleNode(NodeRoot, "wrapMode", ManagerNameSpace);
			switch(NameOptions)
			{
				case "repeat":
					InformationCellMap.Wrap = LibraryEditor_SpriteStudio.DataIntermediate.PartsImage.KindWrap.REPEAT;
					break;

				case "mirror":
					Debug.LogWarning("SSCE-Import Warning: Texture Wrap-Mode \"Mirror\" is not Suppoted. Force-Changed \"Clamp\" (" + FileName + ")");
					goto case "clamp";

				case "clamp":
				default:
					InformationCellMap.Wrap = LibraryEditor_SpriteStudio.DataIntermediate.PartsImage.KindWrap.CLAMP;
					break;
			}

			/* Get Texture Filter-Mode */
			NameOptions = XMLUtility.TextGetSelectSingleNode(NodeRoot, "filterMode", ManagerNameSpace);
			switch(NameOptions)
			{
				case "nearlest":
					InformationCellMap.Filter = LibraryEditor_SpriteStudio.DataIntermediate.PartsImage.KindFilter.NEAREST;
					break;

				case "linear":
				default:
					InformationCellMap.Filter = LibraryEditor_SpriteStudio.DataIntermediate.PartsImage.KindFilter.LINEAR;
					break;
			}

			/* Get Cells */
			InformationCellMap.CellArea = new Hashtable();
			if(null == InformationCellMap.CellArea)
			{
				MessageError = "Not Enough Memory.";
				goto ParseOPSS_ImportSSCE_ErrorEnd;
			}
			DataIntermediate.InformationCell Cell = null;

			NodeList = XMLUtility.XML_SelectNodes(NodeRoot, "cells/cell", ManagerNameSpace);
			if(null == NodeList)
			{
				MessageError = "Cells-Node Not-Found.";
				goto ParseOPSS_ImportSSCE_ErrorEnd;
			}
			string Key = null;
			string ItemText = null;
			string[] ItemTextSprit = null;
			double PivotNormalizeX = 0.0;
			double PivotNormalizeY = 0.0;
			foreach(XmlNode NodeCell in NodeList)
			{
				Key = XMLUtility.TextGetSelectSingleNode(NodeCell, "name", ManagerNameSpace);

				Cell = new DataIntermediate.InformationCell();
				if(null == Cell)
				{
					MessageError = "Not Enough Memory.";
					goto ParseOPSS_ImportSSCE_ErrorEnd;
				}

				ItemText = XMLUtility.TextGetSelectSingleNode(NodeCell, "pos", ManagerNameSpace);
				ItemTextSprit = ItemText.Split(' ');
				Cell.Area.x = (float)(XMLUtility.ValueGetInt(ItemTextSprit[0]));
				Cell.Area.y = (float)(XMLUtility.ValueGetInt(ItemTextSprit[1]));

				ItemText = XMLUtility.TextGetSelectSingleNode(NodeCell, "size", ManagerNameSpace);
				ItemTextSprit = ItemText.Split(' ');
				Cell.Area.width = (float)(XMLUtility.ValueGetInt(ItemTextSprit[0]));
				Cell.Area.height = (float)(XMLUtility.ValueGetInt(ItemTextSprit[1]));

				ItemText = XMLUtility.TextGetSelectSingleNode(NodeCell, "pivot", ManagerNameSpace);
				ItemTextSprit = ItemText.Split(' ');
				PivotNormalizeX = XMLUtility.ValueGetDouble(ItemTextSprit[0]);
				PivotNormalizeY = XMLUtility.ValueGetDouble(ItemTextSprit[1]);
				Cell.Pivot.x = (float)((double)Cell.Area.width * (PivotNormalizeX + 0.5));
				Cell.Pivot.y = (float)((double)Cell.Area.height * (-PivotNormalizeY + 0.5));

				ItemText = XMLUtility.TextGetSelectSingleNode(NodeCell, "rotated", ManagerNameSpace);
				Cell.Rotate = XMLUtility.ValueGetInt(ItemText);

				InformationCellMap.CellArea.Add(Key, Cell);
			}
			return(true);

		ParseOPSS_ImportSSCE_ErrorEnd:;
			Debug.LogError("SSCE-Import Error: " + MessageError + " (" + FileName + ")");
			return(false);
		}
		internal enum KindVersionSSCE
		{
			ERROR = 0x00000000,
			VERSION_000100  = 0x00000100,
			VERSION_010000  = 0x00010000,

			VERSION_REQUIRED = VERSION_000100,
			VERSION_CURRENT = VERSION_010000,
		};

		/* for Parsing ".ssae" */
		internal static bool ImportSSAE(ref DataIntermediate.TrunkParts DataTrunk, string NameDirectory, string FileName, InformationSSPJ InformationProject)
		{
			string MessageError = "";

			XmlDocument DataXML = new XmlDocument();
			DataXML.Load(NameDirectory + "/" + FileName);

			XmlNode NodeRoot = DataXML.FirstChild;
			NodeRoot = NodeRoot.NextSibling;
			KindVersionSSAE VersionCode = (KindVersionSSAE)(VersionCodeGet(NodeRoot, "SpriteStudioAnimePack", (int)KindVersionSSAE.ERROR, false));
			switch(VersionCode)
			{
				case KindVersionSSAE.VERSION_000100:
				case KindVersionSSAE.VERSION_010000:
				case KindVersionSSAE.VERSION_010001:
				case KindVersionSSAE.VERSION_010002:
				case KindVersionSSAE.VERSION_010200:
					break;

				case KindVersionSSAE.ERROR:
				default:
					MessageError = "Not Supported Version.";
					goto ParseOPSS_ImportSSAE_ErrorEnd;
			}
			DataTrunk.VersionCodeSSAE = VersionCode;

			NameTable NodeNameSpace = new NameTable();
			XmlNamespaceManager ManagerNameSpace = new XmlNamespaceManager(NodeNameSpace);
			XmlNodeList NodeList = null;

			NodeList = XMLUtility.XML_SelectNodes(NodeRoot, "cellmapNames/value", ManagerNameSpace);
			if(null == NodeList)
			{
				MessageError = "CellMapNames-Node Not-Found.";
				goto ParseOPSS_ImportSSAE_ErrorEnd;
			}
			int[] CellMapNo = new int[NodeList.Count];
			for(int i=0; i<NodeList.Count; i++)
			{
				CellMapNo[i] = -1;
			}
			int CellNo = 0;
			foreach(XmlNode NodeCellMapName in NodeList)
			{
				CellMapNo[CellNo] = InformationProject.ArraySearchFileName(InformationProject.ListSSCE, NodeCellMapName.InnerText);
				if(-1 == CellMapNo[CellNo])
				{
					MessageError = "CellMap Not-Found.";
					goto ParseOPSS_ImportSSAE_ErrorEnd;
				}
				CellNo++;
			}

			NodeList = XMLUtility.XML_SelectNodes(NodeRoot, "Model/partList/value", ManagerNameSpace);
			if(null == NodeList)
			{
				MessageError = "PartsList-Node Not-Found.";
				goto ParseOPSS_ImportSSAE_ErrorEnd;
			}

			DataTrunk.CountNode = NodeList.Count;
			DataTrunk.ListParts = new DataIntermediate.PartsSprite[DataTrunk.CountNode];
			if(null == DataTrunk.ListParts)
			{
				MessageError = "Not Enough Memory.";
				goto ParseOPSS_ImportSSAE_ErrorEnd;
			}
			for(int i=0; i<DataTrunk.CountNode; i++)
			{
				DataTrunk.ListParts[i].BootUp();
				if(null == DataTrunk.ListParts[i].DataAnimation)
				{
					MessageError = "Not Enough Memory.";
					goto ParseOPSS_ImportSSAE_ErrorEnd;
				}

				DataTrunk.ListParts[i].DataAnimation.DataKeyFrame = new ArrayList[(int)DataIntermediate.KindAttributeKey.TERMINATOR];
				if(null == DataTrunk.ListParts[i].DataAnimation.DataKeyFrame)
				{
					MessageError = "Not Enough Memory.";
					goto ParseOPSS_ImportSSAE_ErrorEnd;
				}
				for(int j=(int)DataIntermediate.KindAttributeKey.POSITION_X; j<(int)DataIntermediate.KindAttributeKey.TERMINATOR; j++)
				{
					DataTrunk.ListParts[i].DataAnimation.DataKeyFrame[j] = null;
				}

				DataTrunk.ListParts[i].DataAnimation.RateInheritance = new float[(int)DataIntermediate.KindAttributeKey.TERMINATOR_INHERIT];
				if(null == DataTrunk.ListParts[i].DataAnimation.RateInheritance)
				{
					MessageError = "Not Enough Memory.";
					goto ParseOPSS_ImportSSAE_ErrorEnd;
				}
				for(int j=(int)DataIntermediate.KindAttributeKey.POSITION_X; j<(int)DataIntermediate.KindAttributeKey.TERMINATOR_INHERIT; j++)
				{
					DataTrunk.ListParts[i].DataAnimation.RateInheritance[j] = 0.0f;
				}
			}

			int PartsNo = -1;
			foreach(XmlNode NodeParts in NodeList)
			{
				PartsNo = XMLUtility.ValueGetInt(XMLUtility.TextGetSelectSingleNode(NodeParts, "arrayIndex", ManagerNameSpace));
				DataTrunk.ListParts[PartsNo].ID = PartsNo;

				if(false == ImportSSAESetParts(ref DataTrunk.ListParts[PartsNo], NodeParts, ManagerNameSpace, VersionCode))
				{
					goto ParseOPSS_ImportSSAE_ErrorEnd_NoMessage;
				}
			}

			for(int i=0; i<DataTrunk.CountNode; i++)
			{
				if(DataIntermediate.KindInheritance.PARENT == DataTrunk.ListParts[i].DataAnimation.Inheritance)
				{
					PartsNo = DataTrunk.ListParts[i].IDParent;
					while(DataIntermediate.KindInheritance.PARENT == DataTrunk.ListParts[PartsNo].DataAnimation.Inheritance)
					{
						PartsNo = DataTrunk.ListParts[PartsNo].IDParent;
					}
					DataTrunk.ListParts[i].DataAnimation.FlagInheritance = DataTrunk.ListParts[PartsNo].DataAnimation.FlagInheritance;
				}

				for(int j=(int)DataIntermediate.KindAttributeKey.POSITION_X; j<(int)DataIntermediate.KindAttributeKey.TERMINATOR_INHERIT; j++)
				{
					DataTrunk.ListParts[i].DataAnimation.RateInheritance[j] = (0 != (DataTrunk.ListParts[i].DataAnimation.FlagInheritance & DataIntermediate.FlagParameterKeyFrameInherit[j])) ? 1.0f : 0.0f;
				}
			}

			NodeList = XMLUtility.XML_SelectNodes(NodeRoot, "animeList/anime", ManagerNameSpace);
			if(null == NodeList)
			{
				MessageError = "AnimationList-Node Not-Found.";
				goto ParseOPSS_ImportSSAE_ErrorEnd;
			}
			int CountAnimation = NodeList.Count;
			DataTrunk.ListInformationPlay = new Library_SpriteStudio.AnimationInformationPlay[CountAnimation];
			for(int i=0; i<CountAnimation; i++)
			{
				DataTrunk.ListInformationPlay[i] = new Library_SpriteStudio.AnimationInformationPlay();
			}

			CountAnimation = 0;
			int FrameNoStart = 0;
			int CountLabel = 0;
			Library_SpriteStudio.AnimationInformationPlay.InformationLabel InformationLabel = null;
			ArrayList ArrayLabel = new ArrayList();
			foreach(XmlNode NodeAnimation in NodeList)
			{
				/* Collection Labels */
				ArrayLabel.Clear();
				XmlNodeList NodeListLabel = XMLUtility.XML_SelectNodes(NodeAnimation, "labels/value", ManagerNameSpace);
				string NameLabel = "";
				int FrameLabel = 0;
				foreach(XmlNode NodeLabel in NodeListLabel)
				{
					NameLabel = XMLUtility.TextGetSelectSingleNode(NodeLabel, "name", ManagerNameSpace);
					FrameLabel = XMLUtility.ValueGetInt(XMLUtility.TextGetSelectSingleNode(NodeLabel, "time", ManagerNameSpace));
					if(false == Library_SpriteStudio.AnimationInformationPlay.NameCheckDefault(NameLabel))
					{
						InformationLabel = new Library_SpriteStudio.AnimationInformationPlay.InformationLabel();
						InformationLabel.Name = string.Copy(NameLabel);
						InformationLabel.FrameNo = FrameLabel;
						ArrayLabel.Add(InformationLabel);
					}
					else
					{
						Debug.LogWarning("SSAE-Import Warning: Label-Name is reserved. Ignored. Name[" + NameLabel
											+ "]: Frame[" + FrameLabel + "]"
										);
					}
				}
				CountLabel = ArrayLabel.Count;

				/* Create Information */
				DataTrunk.ListInformationPlay[CountAnimation].Name = string.Copy(XMLUtility.TextGetSelectSingleNode(NodeAnimation, "name", ManagerNameSpace));
				DataTrunk.ListInformationPlay[CountAnimation].FrameStart = FrameNoStart;
				DataTrunk.ListInformationPlay[CountAnimation].FrameEnd = XMLUtility.ValueGetInt(XMLUtility.TextGetSelectSingleNode(NodeAnimation, "settings/frameCount", ManagerNameSpace));
				DataTrunk.ListInformationPlay[CountAnimation].FrameEnd += FrameNoStart;
				DataTrunk.ListInformationPlay[CountAnimation].FrameEnd--;
				DataTrunk.ListInformationPlay[CountAnimation].FramePerSecond = XMLUtility.ValueGetInt(XMLUtility.TextGetSelectSingleNode(NodeAnimation, "settings/fps", ManagerNameSpace));
				DataTrunk.ListInformationPlay[CountAnimation].Label = new Library_SpriteStudio.AnimationInformationPlay.InformationLabel[CountLabel];
				for(int j=0; j<CountLabel; j++)
				{
					InformationLabel = ArrayLabel[j] as Library_SpriteStudio.AnimationInformationPlay.InformationLabel;
					DataTrunk.ListInformationPlay[CountAnimation].Label[j] = new Library_SpriteStudio.AnimationInformationPlay.InformationLabel();
					DataTrunk.ListInformationPlay[CountAnimation].Label[j].Name = string.Copy(InformationLabel.Name);
					DataTrunk.ListInformationPlay[CountAnimation].Label[j].FrameNo = InformationLabel.FrameNo;
				}
				DataTrunk.CountFrameFull = DataTrunk.ListInformationPlay[CountAnimation].FrameEnd + 1;

				/* Decode Animation-Datas */
				if(false == ImportSSAESetAnimation(ref DataTrunk, CountAnimation, FrameNoStart, CellMapNo, NodeAnimation, ManagerNameSpace))
				{
					goto ParseOPSS_ImportSSAE_ErrorEnd_NoMessage;
				}

				FrameNoStart = (((DataTrunk.ListInformationPlay[CountAnimation].FrameEnd + 9) / 10) + 1) * 10;
				CountAnimation++;
			}
			ArrayLabel.Clear();

			return(true);

		ParseOPSS_ImportSSAE_ErrorEnd:;
			Debug.LogError("SSAE-Import Error: " + MessageError + " (" + FileName + ")");
		ParseOPSS_ImportSSAE_ErrorEnd_NoMessage:;
			return(false);
		}
		internal enum KindVersionSSAE
		{
			ERROR = 0x00000000,
			VERSION_000100  = 0x00000100,
			VERSION_010000  = 0x00010000,
			VERSION_010001  = 0x00010001,
			VERSION_010002  = 0x00010002,	/* ssae ver.5.3.5 */
			VERSION_010200  = 0x00010200,	/* ssae ver.5.5.0 beta-3 */

			VERSION_REQUIRED = VERSION_000100,
			VERSION_CURRENT = VERSION_010000,	/* VERSION_010002 */
		};

		/* Version-Code Shaping */
		private static int VersionCodeGet(XmlNode NodeRoot, string NameTag, int ErrorCode, bool FlagMaskRevision)
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
			int Version = XMLUtility.VersionGetHexCode(VersionText);
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

		/* SSAE-Parts Data Decoding */
		private static bool ImportSSAESetParts(	ref DataIntermediate.PartsSprite DataParts,
												XmlNode NodeParts,
												XmlNamespaceManager ManagerNameSpace,
												KindVersionSSAE VersionCode
											)
		{
			string ValueText = "";

			ValueText = XMLUtility.TextGetSelectSingleNode(NodeParts, "name", ManagerNameSpace);
			DataParts.Name = string.Copy(ValueText);

			ValueText = XMLUtility.TextGetSelectSingleNode(NodeParts, "parentIndex", ManagerNameSpace);
			DataParts.IDParent = XMLUtility.ValueGetInt(ValueText);

			ValueText = XMLUtility.TextGetSelectSingleNode(NodeParts, "type", ManagerNameSpace);
			switch(ValueText)
			{
				case "null":
					if(0 == DataParts.ID)
					{
						DataParts.PartsKind = Library_SpriteStudio.KindParts.ROOT;
					}
					else
					{
						DataParts.PartsKind = Library_SpriteStudio.KindParts.NULL;
					}
					break;

				case "normal":
					DataParts.PartsKind = Library_SpriteStudio.KindParts.NORMAL;
					break;

				case "instance":
					DataParts.PartsKind = Library_SpriteStudio.KindParts.INSTANCE;
					break;

				case "effect":
//					DataParts.PartsKind = Library_SpriteStudio.KindParts.EFFECT;
					DataParts.PartsKind = Library_SpriteStudio.KindParts.NULL;
					Debug.LogWarning("SSAE-Import Warning: Parts["
										+ DataParts.ID.ToString()
										+ "] Now UnSupported Parts Kind.: "
										+ ValueText
									);
					break;

				default:
					Debug.LogWarning("SSAE-Import Warning: Parts["
										+ DataParts.ID.ToString()
										+ "] Invalid Parts Kind.: "
										+ ValueText
									);
					goto case "null";
			}

			ValueText = XMLUtility.TextGetSelectSingleNode(NodeParts, "boundsType", ManagerNameSpace);
			switch(ValueText)
			{
				case "none":
					DataParts.CollisionKind = Library_SpriteStudio.KindCollision.NON;
					break;

				case "quad":
					DataParts.CollisionKind = Library_SpriteStudio.KindCollision.SQUARE;
					break;

				case "aabb":
					DataParts.CollisionKind = Library_SpriteStudio.KindCollision.AABB;
					break;

				case "circle":
					DataParts.CollisionKind = Library_SpriteStudio.KindCollision.CIRCLE;
					break;

				case "circle_smin":
					DataParts.CollisionKind = Library_SpriteStudio.KindCollision.CIRCLE_SCALEMINIMUM;
					break;

				case "circle_smax":
					DataParts.CollisionKind = Library_SpriteStudio.KindCollision.CIRCLE_SCALEMAXIMUM;
					break;

				default:
					Debug.LogWarning("SSAE-Import Warning: Parts["
										+ DataParts.ID.ToString()
										+ "] Invalid Collision Kind.: "
										+ ValueText
									);
					goto case "none";
			}

			ValueText = XMLUtility.TextGetSelectSingleNode(NodeParts, "inheritType", ManagerNameSpace);
			switch(ValueText)
			{
				case "parent":
					{
						switch(VersionCode)
						{
							case KindVersionSSAE.VERSION_010000:
							case KindVersionSSAE.VERSION_010001:
							case KindVersionSSAE.VERSION_010002:
							case KindVersionSSAE.VERSION_010200:
								{
									if(0 == DataParts.ID)
									{
										DataParts.DataAnimation.Inheritance = DataIntermediate.KindInheritance.SELF;
										DataParts.DataAnimation.FlagInheritance = DataIntermediate.FlagAttributeKeyInherit.PRESET;
									}
									else
									{
										DataParts.DataAnimation.Inheritance = DataIntermediate.KindInheritance.PARENT;
										DataParts.DataAnimation.FlagInheritance = DataIntermediate.FlagAttributeKeyInherit.CLEAR;
									}
								}
								break;
						}
					}
					break;

				case "self":
					{
						switch(VersionCode)
						{
							case KindVersionSSAE.VERSION_010000:
							case KindVersionSSAE.VERSION_010001:
								{
									/* MEMO: Default-Value: 0(true) */
									/*       Attributes'-Tag exists when Value is 0(false). */
									DataParts.DataAnimation.Inheritance = DataIntermediate.KindInheritance.SELF;
									DataParts.DataAnimation.FlagInheritance = DataIntermediate.FlagAttributeKeyInherit.CLEAR;

									XmlNode NodeAttribute = null;
									NodeAttribute = XMLUtility.XML_SelectSingleNode(NodeParts, "ineheritRates/ALPH", ManagerNameSpace);
									if(null == NodeAttribute)
									{
										DataParts.DataAnimation.FlagInheritance |= DataIntermediate.FlagAttributeKeyInherit.OPACITY_RATE;
									}

									NodeAttribute = XMLUtility.XML_SelectSingleNode(NodeParts, "ineheritRates/FLPH", ManagerNameSpace);
									if(null == NodeAttribute)
									{
										DataParts.DataAnimation.FlagInheritance |= DataIntermediate.FlagAttributeKeyInherit.FLIP_X;
									}

									NodeAttribute = XMLUtility.XML_SelectSingleNode(NodeParts, "ineheritRates/FLPV", ManagerNameSpace);
									if(null == NodeAttribute)
									{
										DataParts.DataAnimation.FlagInheritance |= DataIntermediate.FlagAttributeKeyInherit.FLIP_Y;
									}

									NodeAttribute = XMLUtility.XML_SelectSingleNode(NodeParts, "ineheritRates/HIDE", ManagerNameSpace);
									if(null == NodeAttribute)
									{
										DataParts.DataAnimation.FlagInheritance |= DataIntermediate.FlagAttributeKeyInherit.SHOW_HIDE;
									}
								}
								break;

							case KindVersionSSAE.VERSION_010002:
							case KindVersionSSAE.VERSION_010200:
								{
									/* MEMO: Attributes'-Tag always exists. */
									string ValueTextBool = "";
									bool ValueBool = false;
									DataParts.DataAnimation.Inheritance = DataIntermediate.KindInheritance.SELF;
									DataParts.DataAnimation.FlagInheritance = DataIntermediate.FlagAttributeKeyInherit.PRESET;
									ValueTextBool = XMLUtility.TextGetSelectSingleNode(NodeParts, "ineheritRates/ALPH", ManagerNameSpace);
									if(null != ValueTextBool)
									{
										ValueBool = XMLUtility.ValueGetBool(ValueTextBool);
										if(false == ValueBool)
										{
											DataParts.DataAnimation.FlagInheritance &= ~DataIntermediate.FlagAttributeKeyInherit.OPACITY_RATE;
										}
									}

									ValueTextBool = XMLUtility.TextGetSelectSingleNode(NodeParts, "ineheritRates/FLPH", ManagerNameSpace);
									if(null != ValueTextBool)
									{
										ValueBool = XMLUtility.ValueGetBool(ValueTextBool);
										if(false == ValueBool)
										{
											DataParts.DataAnimation.FlagInheritance &= ~DataIntermediate.FlagAttributeKeyInherit.FLIP_X;
										}
									}

									ValueTextBool = XMLUtility.TextGetSelectSingleNode(NodeParts, "ineheritRates/FLPV", ManagerNameSpace);
									if(null != ValueTextBool)
									{
										ValueBool = XMLUtility.ValueGetBool(ValueTextBool);
										if(false == ValueBool)
										{
											DataParts.DataAnimation.FlagInheritance &= ~DataIntermediate.FlagAttributeKeyInherit.FLIP_Y;
										}
									}

									ValueTextBool = XMLUtility.TextGetSelectSingleNode(NodeParts, "ineheritRates/HIDE", ManagerNameSpace);
									if(null != ValueTextBool)
									{
										ValueBool = XMLUtility.ValueGetBool(ValueTextBool);
										if(false == ValueBool)
										{
											DataParts.DataAnimation.FlagInheritance &= ~DataIntermediate.FlagAttributeKeyInherit.SHOW_HIDE;
										}
									}
								}
								break;
						}
					}
					break;

				default:
					Debug.LogWarning("SSAE-Import Warning: Parts["
										+ DataParts.ID.ToString()
										+ "] Invalid Inheritance Kind.: "
										+ ValueText
									);
					goto case "parent";
			}

			ValueText = XMLUtility.TextGetSelectSingleNode(NodeParts, "alphaBlendType", ManagerNameSpace);
			switch(ValueText)
			{
				case "mix":
					DataParts.KindBlendTarget = Library_SpriteStudio.KindColorOperation.MIX;
					break;

				case "mul":
					DataParts.KindBlendTarget = Library_SpriteStudio.KindColorOperation.MUL;
					break;

				case "add":
					DataParts.KindBlendTarget = Library_SpriteStudio.KindColorOperation.ADD;
					break;

				case "sub":
					DataParts.KindBlendTarget = Library_SpriteStudio.KindColorOperation.SUB;
					break;

				default:
					Debug.LogWarning("SSAE-Import Warning: Parts["
										+ DataParts.ID.ToString()
										+ "] Invalid Alpha-Blend Kind.: "
										+ ValueText
									);
					goto case "mix";
			}

			ValueText = XMLUtility.TextGetSelectSingleNode(NodeParts, "refAnimePack", ManagerNameSpace);
			DataParts.InstanceNameSSAE = (null != ValueText) ? String.Copy(ValueText) : "";
			ValueText = XMLUtility.TextGetSelectSingleNode(NodeParts, "refAnime", ManagerNameSpace);
			DataParts.InstanceNameAnimation = (null != ValueText) ? String.Copy(ValueText) : "";

			return(true);
		}
		private static bool ImportSSAESetAnimation(	ref DataIntermediate.TrunkParts DataTrunk,
													int No,
													int FrameNoStart,
													int[] CellMapNo,
													XmlNode NodeAnimation,
													XmlNamespaceManager ManagerNameSpace
												)
		{
			XmlNodeList NodeList = XMLUtility.XML_SelectNodes(NodeAnimation, "partAnimes/partAnime", ManagerNameSpace);
			if(null == NodeList)
			{
				Debug.LogError(	"SSAE-Import Error: Animation["
								+ No.ToString()
								+ "] Parts-KeyDatas Not-Found"
							);
				return(false);
			}
			int IndexNoParts = -1;
			string ValueText = "";
			XmlNodeList NodeAttributes = null;
			foreach(XmlNode NodeParts in NodeList)
			{
				ValueText = XMLUtility.TextGetSelectSingleNode(NodeParts, "partName", ManagerNameSpace);
				IndexNoParts = ImportSSAEPartsSearchNameToID(DataTrunk, ValueText);
				if(-1 == IndexNoParts)
				{
					Debug.LogWarning(	"SSAE-Import Warning: Animation["
										+ No.ToString()
										+ "] Parts Name Invalid: "
										+ ValueText
									);
					continue;
				}

				NodeAttributes = XMLUtility.XML_SelectNodes(NodeParts, "attributes/attribute", ManagerNameSpace);
				ImportSSAEPartsSetKeyData(ref DataTrunk.ListParts[IndexNoParts], FrameNoStart, CellMapNo, DataTrunk.ListImage, NodeAttributes, ManagerNameSpace);
			}
			return(true);
		}
		private static int ImportSSAEPartsSearchNameToID(DataIntermediate.TrunkParts DataTrunk, string Name)
		{
			int Count = DataTrunk.CountNode;
			for(int i=0; i<Count; i++)
			{
				if(0 == Name.CompareTo(DataTrunk.ListParts[i].Name))
				{
					return(i);
				}
			}
			return(-1);
		}
		private static bool ImportSSAEPartsSetKeyData(	ref DataIntermediate.PartsSprite DataParts,
														int FrameNoStart,
														int[] CellMapNo,
														DataIntermediate.PartsImage[] ListImage,
														XmlNodeList ListNodeAttribute,
														XmlNamespaceManager ManagerNameSpace
													)
		{
			if(null == ListNodeAttribute)
			{
				Debug.LogError(	"SSAE-Import Error: Animation["
								+ DataParts.Name
								+ "] KeyData Not-Found."
							);
				return(false);
			}

			DataIntermediate.ManagerDescriptionAttribute.DescriptionAttribute Description = null;
			XmlNodeList NodeListKey = null;
			XmlNode NodeInterpolation = null;
			bool FlagWithParameterInterpolation = false;
			bool FlagEnableInterpolation = false;
			int AttributeNo = -1;
			int KeyNo = -1;
			string ValueText = "";
			string[] ValueTextParameter = null;
			DataIntermediate.KeyFrame.DataCurve DataCurve = null;
			DataIntermediate.KeyFrame.InterfaceData KeyData = null;
			foreach(XmlNode NodeAttribute in ListNodeAttribute)
			{
				ValueText = NodeAttribute.Attributes["tag"].Value;

				Description = DataIntermediate.ManagerDescriptionAttribute.DescriptionAttributeGetTagName(ValueText);
				if(null == Description)
				{
					Debug.LogWarning(	"SSAE-Import Warning: Part["
										+ DataParts.Name
										+ "] Attribute Invalid. ["
										+ ValueText
										+ "]"
									);
					continue;
				}
				AttributeNo = (int)Description.Attribute;
				if((int)DataIntermediate.KindAttributeKey.TERMINATOR_INHERIT > AttributeNo)
				{
					DataParts.DataAnimation.FlagKeyParameter |= DataIntermediate.FlagParameterKeyFrameInherit[AttributeNo];
				}
				if(null == DataParts.DataAnimation.DataKeyFrame[AttributeNo])
				{
					DataParts.DataAnimation.DataKeyFrame[AttributeNo] = new ArrayList();
					DataParts.DataAnimation.DataKeyFrame[AttributeNo].Clear();
				}

				NodeListKey = XMLUtility.XML_SelectNodes(NodeAttribute, "key", ManagerNameSpace);
				if(null == NodeListKey)
				{
					Debug.LogWarning(	"SSAE-Import Warning: Part["
										+ DataParts.Name
										+ "] Attribute ["
										+ Description.Attribute.ToString()
										+ "] is No Key-Data."
									);
					continue;
				}
				foreach(XmlNode NodeKey in NodeListKey)
				{
					FlagEnableInterpolation = true;

					KeyNo = XMLUtility.ValueGetInt(NodeKey.Attributes["time"].Value);

					if(true == Description.Interpolatable)
					{
						DataCurve = new DataIntermediate.KeyFrame.DataCurve();
						NodeInterpolation = NodeKey.Attributes["ipType"];
						FlagWithParameterInterpolation = false;
						if(null == NodeInterpolation)
						{
							DataCurve.Kind = DataIntermediate.KindInterpolation.NON;
						}
						else
						{
							switch(NodeInterpolation.Value)
							{
								case "linear":
									DataCurve.Kind = DataIntermediate.KindInterpolation.LINEAR;
									break;

								case "hermite":
									DataCurve.Kind = DataIntermediate.KindInterpolation.HERMITE;
									FlagWithParameterInterpolation = true;
									break;

								case "bezier":
									DataCurve.Kind = DataIntermediate.KindInterpolation.BEZIER;
									FlagWithParameterInterpolation = true;
									break;

								case "acceleration":
									DataCurve.Kind = DataIntermediate.KindInterpolation.ACCELERATE;
									break;

								case "deceleration":
									DataCurve.Kind = DataIntermediate.KindInterpolation.DECELERATE;
									break;

								default:
									Debug.LogWarning(	"SSAE-Import Warning: Part["
														+ DataParts.Name
														+ "] Attribute ["
														+ Description.Attribute.ToString()
														+ "] - Key ["
														+ KeyNo.ToString()
														+ "] Invalid Interpolation Kind: "
														+ NodeInterpolation.Value
													);
									DataCurve.Kind = DataIntermediate.KindInterpolation.NON;
									break;
							}

							if(true == FlagWithParameterInterpolation)
							{
								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "curve", ManagerNameSpace);
								if(null == ValueText)
								{
									Debug.LogWarning(	"SSAE-Import Warning: Part["
														+ DataParts.Name
														+ "] Attribute ["
														+ Description.Attribute.ToString()
														+ "] - Key ["
														+ KeyNo.ToString()
														+ "] Has No Interpolation-Parameters: "
														+ NodeInterpolation.Value
													);
									DataCurve.TimeStart = 0.0f;
									DataCurve.ValueStart = 0.0f;
									DataCurve.TimeEnd = 0.0f;
									DataCurve.ValueEnd = 0.0f;
									FlagEnableInterpolation = false;
								}
								else
								{
									ValueTextParameter = ValueText.Split(' ');
									DataCurve.TimeStart = (float)(XMLUtility.ValueGetDouble(ValueTextParameter[0]));
									DataCurve.ValueStart = (float)(XMLUtility.ValueGetDouble(ValueTextParameter[1]));
									DataCurve.TimeEnd = (float)(XMLUtility.ValueGetDouble(ValueTextParameter[2]));
									DataCurve.ValueEnd = (float)(XMLUtility.ValueGetDouble(ValueTextParameter[3]));
								}
							}
							else
							{
								DataCurve.TimeStart = 0.0f;
								DataCurve.ValueStart = 0.0f;
								DataCurve.TimeEnd = 0.0f;
								DataCurve.ValueEnd = 0.0f;
							}
						}
					}
					else
					{
						DataCurve = null;
						FlagEnableInterpolation = false;
					}

					KeyData = null;
					switch(Description.KindValue)
					{
						case DataIntermediate.KindValueKey.NUMBER:
							{
								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value", ManagerNameSpace);

								switch(Description.KindType)
								{
									case DataIntermediate.KindTypeKey.INT:
										{
											int Value = XMLUtility.ValueGetInt(ValueText);
											DataIntermediate.KeyFrame.DataInt DataInt = new DataIntermediate.KeyFrame.DataInt();
											DataInt.Value = Value;
											KeyData = DataInt;
										}
										break;

									case DataIntermediate.KindTypeKey.HEX:
										{
											int Value = XMLUtility.TextHexToInt(ValueText);
											DataIntermediate.KeyFrame.DataInt DataInt = new DataIntermediate.KeyFrame.DataInt();
											DataInt.Value = Value;
											KeyData = DataInt;
										}
										break;

									case DataIntermediate.KindTypeKey.FLOAT:
									case DataIntermediate.KindTypeKey.DEGREE:
										{
											float Value = (float)(XMLUtility.ValueGetDouble(ValueText));
											DataIntermediate.KeyFrame.DataFloat DataFloat = new DataIntermediate.KeyFrame.DataFloat();
											DataFloat.Value = Value;
											KeyData = DataFloat;
										}
										break;

									case DataIntermediate.KindTypeKey.BOOL:
									case DataIntermediate.KindTypeKey.OTHER:
									default:
										Debug.LogWarning(	"SSAE-Import Warning: Part["
															+ DataParts.Name
															+ "] Attribute ["
															+ Description.Attribute.ToString()
															+ "] - Key ["
															+ KeyNo.ToString()
															+ "] Not-Supported Value-Type: "
															+ Description.KindType.ToString()
														);
										break;
								}
							}
							break;

						case DataIntermediate.KindValueKey.CHECK:
							{
								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value", ManagerNameSpace);

								bool Value = XMLUtility.ValueGetBool(ValueText);
								DataIntermediate.KeyFrame.DataBool DataBool = new DataIntermediate.KeyFrame.DataBool();
								DataBool.Value = Value;
								KeyData = DataBool;

								FlagEnableInterpolation = false;
							}
							break;

						case DataIntermediate.KindValueKey.COLOR:
							{
								DataIntermediate.KeyFrame.ValueColor Value = new DataIntermediate.KeyFrame.ValueColor();

								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value/blendType", ManagerNameSpace);
								switch(ValueText)
								{
									case "mix":
										Value.Operation = Library_SpriteStudio.KindColorOperation.MIX;
										break;

									case "mul":
										Value.Operation = Library_SpriteStudio.KindColorOperation.MUL;
										break;

									case "add":
										Value.Operation = Library_SpriteStudio.KindColorOperation.ADD;
										break;

									case "sub":
										Value.Operation = Library_SpriteStudio.KindColorOperation.SUB;
										break;

									default:
										Debug.LogWarning(	"SSAE-Import Warning: Part["
															+ DataParts.Name
															+ "] Attribute ["
															+ Description.Attribute.ToString()
															+ "] - Key ["
															+ KeyNo.ToString()
															+ "] Not-Supported ColorBlend-Type: "
															+ ValueText
														);
										Value.Operation = Library_SpriteStudio.KindColorOperation.NON;
										break;
								}

								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value/target", ManagerNameSpace);
								float ColorA = 0.0f;
								float ColorR = 0.0f;
								float ColorG = 0.0f;
								float ColorB = 0.0f;
								float RatePixel = 0.0f;
								switch(ValueText)
								{
									case "whole":
										{
											Value.Bound = Library_SpriteStudio.KindColorBound.OVERALL;

											ImportSSAEKeyDataGetColorBlend(	out ColorA, out ColorR, out ColorG, out ColorB, out RatePixel,
																			NodeKey, "value/color", ManagerNameSpace
																		);
											for(int i=0; i<4; i++)
											{
												Value.VertexColor[i].r = ColorR;
												Value.VertexColor[i].g = ColorG;
												Value.VertexColor[i].b = ColorB;
												Value.VertexColor[i].a = ColorA;
												Value.RatePixelAlpha[i] = RatePixel;
											}
										}
										break;

									case "vertex":
										{
											Value.Bound = Library_SpriteStudio.KindColorBound.VERTEX;

											ImportSSAEKeyDataGetColorBlend(	out ColorA, out ColorR, out ColorG, out ColorB, out RatePixel,
																			NodeKey, "value/LT", ManagerNameSpace
																		);
											Value.VertexColor[0].r = ColorR;
											Value.VertexColor[0].g = ColorG;
											Value.VertexColor[0].b = ColorB;
											Value.VertexColor[0].a = ColorA;
											Value.RatePixelAlpha[0] = RatePixel;

											ImportSSAEKeyDataGetColorBlend(	out ColorA, out ColorR, out ColorG, out ColorB, out RatePixel,
																			NodeKey, "value/RT", ManagerNameSpace
																		);
											Value.VertexColor[1].r = ColorR;
											Value.VertexColor[1].g = ColorG;
											Value.VertexColor[1].b = ColorB;
											Value.VertexColor[1].a = ColorA;
											Value.RatePixelAlpha[1] = RatePixel;

											ImportSSAEKeyDataGetColorBlend(	out ColorA, out ColorR, out ColorG, out ColorB, out RatePixel,
																			NodeKey, "value/RB", ManagerNameSpace
																		);
											Value.VertexColor[2].r = ColorR;
											Value.VertexColor[2].g = ColorG;
											Value.VertexColor[2].b = ColorB;
											Value.VertexColor[2].a = ColorA;
											Value.RatePixelAlpha[2] = RatePixel;

											ImportSSAEKeyDataGetColorBlend(	out ColorA, out ColorR, out ColorG, out ColorB, out RatePixel,
																			NodeKey, "value/LB", ManagerNameSpace
																		);
											Value.VertexColor[3].r = ColorR;
											Value.VertexColor[3].g = ColorG;
											Value.VertexColor[3].b = ColorB;
											Value.VertexColor[3].a = ColorA;
											Value.RatePixelAlpha[3] = RatePixel;
										}
										break;

									default:
										{
											Debug.LogWarning(	"SSAE-Import Warning: Part["
																+ DataParts.Name
																+ "] Attribute ["
																+ Description.Attribute.ToString()
																+ "] - Key ["
																+ KeyNo.ToString()
																+ "] Not-Supported ColorBlend-Target: "
																+ ValueText
															);
											Value.Bound = Library_SpriteStudio.KindColorBound.OVERALL;
											for(int i=0; i<4; i++)
											{
												Value.VertexColor[i].r = 0.0f;
												Value.VertexColor[i].g = 0.0f;
												Value.VertexColor[i].b = 0.0f;
												Value.VertexColor[i].a = 0.0f;
												Value.RatePixelAlpha[i] = 1.0f;
											}
										}
										break;
								}

								DataIntermediate.KeyFrame.DataColor DataColor = new DataIntermediate.KeyFrame.DataColor();
								DataColor.Value = Value;
								KeyData = DataColor;
							}
							break;

						case DataIntermediate.KindValueKey.QUADRILATERRAL:
							{
								DataIntermediate.KeyFrame.ValueQuadrilateral Value = new DataIntermediate.KeyFrame.ValueQuadrilateral();

								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value/LT", ManagerNameSpace);
								ValueTextParameter = ValueText.Split(' ');
								Value.Coordinate[0].X = (float)(XMLUtility.ValueGetDouble(ValueTextParameter[0]));
								Value.Coordinate[0].Y = (float)(XMLUtility.ValueGetDouble(ValueTextParameter[1]));

								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value/RT", ManagerNameSpace);
								ValueTextParameter = ValueText.Split(' ');
								Value.Coordinate[1].X = (float)(XMLUtility.ValueGetDouble(ValueTextParameter[0]));
								Value.Coordinate[1].Y = (float)(XMLUtility.ValueGetDouble(ValueTextParameter[1]));

								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value/RB", ManagerNameSpace);
								ValueTextParameter = ValueText.Split(' ');
								Value.Coordinate[2].X = (float)(XMLUtility.ValueGetDouble(ValueTextParameter[0]));
								Value.Coordinate[2].Y = (float)(XMLUtility.ValueGetDouble(ValueTextParameter[1]));

								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value/LB", ManagerNameSpace);
								ValueTextParameter = ValueText.Split(' ');
								Value.Coordinate[3].X = (float)(XMLUtility.ValueGetDouble(ValueTextParameter[0]));
								Value.Coordinate[3].Y = (float)(XMLUtility.ValueGetDouble(ValueTextParameter[1]));

								DataIntermediate.KeyFrame.DataQuadrilateral DataQuadrilateral = new DataIntermediate.KeyFrame.DataQuadrilateral();
								DataQuadrilateral.Value = Value;
								KeyData = DataQuadrilateral;
							}
							break;

						case DataIntermediate.KindValueKey.USER:
							{
								DataIntermediate.KeyFrame.ValueUser Value = new DataIntermediate.KeyFrame.ValueUser();
								Value.Flag = DataIntermediate.KeyFrame.ValueUser.FlagData.CLEAR;

								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value/integer", ManagerNameSpace);
								if(null != ValueText)
								{
									if(false == int.TryParse(ValueText, out Value.Number))
									{
										uint	ValueUintTemp = 0;
										if(false == uint.TryParse(ValueText, out ValueUintTemp))
										{
											Debug.LogWarning("SSAE-Import Warning: Part["
																+ DataParts.Name
																+ "] - Key ["
																+ KeyNo.ToString()
																+ "] Invalid Number: "
																+ ValueText
															);
										}
										Value.Number = (int)ValueUintTemp;
									}
									Value.Flag |= DataIntermediate.KeyFrame.ValueUser.FlagData.NUMBER;
								}
								else
								{
									Value.Number = 0;
									Value.Flag &= ~DataIntermediate.KeyFrame.ValueUser.FlagData.NUMBER;
								}

								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value/rect", ManagerNameSpace);
								if(null != ValueText)
								{
									ValueTextParameter = ValueText.Split(' ');
									Value.Rectangle.xMin = (float)(XMLUtility.ValueGetDouble(ValueTextParameter[0]));
									Value.Rectangle.yMin = (float)(XMLUtility.ValueGetDouble(ValueTextParameter[1]));
									Value.Rectangle.xMax = (float)(XMLUtility.ValueGetDouble(ValueTextParameter[2]));
									Value.Rectangle.yMax = (float)(XMLUtility.ValueGetDouble(ValueTextParameter[3]));
									Value.Flag |= DataIntermediate.KeyFrame.ValueUser.FlagData.RECTANGLE;
								}
								else
								{
									Value.Rectangle.xMin = 0.0f;
									Value.Rectangle.yMin = 0.0f;
									Value.Rectangle.xMax = 0.0f;
									Value.Rectangle.yMax = 0.0f;
									Value.Flag &= ~DataIntermediate.KeyFrame.ValueUser.FlagData.RECTANGLE;
								}

								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value/point", ManagerNameSpace);
								Value.Coordinate = new DataIntermediate.KeyFrame.ValuePoint();
								if(null != ValueText)
								{
									ValueTextParameter = ValueText.Split(' ');
									Value.Coordinate.X = (float)(XMLUtility.ValueGetDouble(ValueTextParameter[0]));
									Value.Coordinate.Y = (float)(XMLUtility.ValueGetDouble(ValueTextParameter[1]));
									Value.Flag |= DataIntermediate.KeyFrame.ValueUser.FlagData.COORDINATE;
								}
								else
								{
									Value.Coordinate.X = 0.0f;
									Value.Coordinate.Y = 0.0f;
									Value.Flag &= ~DataIntermediate.KeyFrame.ValueUser.FlagData.COORDINATE;
								}

								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value/string", ManagerNameSpace);
								if(null != ValueText)
								{
									Value.Text = string.Copy(ValueText);
									Value.Flag |= DataIntermediate.KeyFrame.ValueUser.FlagData.TEXT;
								}
								else
								{
									Value.Text = null;
									Value.Flag &= ~DataIntermediate.KeyFrame.ValueUser.FlagData.TEXT;
								}

								DataIntermediate.KeyFrame.DataUser DataUser = new DataIntermediate.KeyFrame.DataUser();
								DataUser.Value = Value;
								KeyData = DataUser;

								FlagEnableInterpolation = false;
							}
							break;

						case DataIntermediate.KindValueKey.CELL:
							{
								DataIntermediate.KeyFrame.ValueCell Value = new DataIntermediate.KeyFrame.ValueCell();

								bool	FlagValidCell = true;
								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value/mapId", ManagerNameSpace);
								if(null == ValueText)
								{
									Value.TextureNo = -1;
									FlagValidCell = false;
								}
								else
								{
									Value.TextureNo = CellMapNo[XMLUtility.ValueGetInt(ValueText)];

									ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value/name", ManagerNameSpace);
									if(null == ValueText)
									{
										FlagValidCell = false;
									}
									else
									{
										DataIntermediate.InformationCell Cell = ListImage[Value.TextureNo].CellArea[ValueText] as DataIntermediate.InformationCell;
										if(null == Cell)
										{
											FlagValidCell = false;
										}
										else
										{
											Value.Rectangle.x = Cell.Area.x;
											Value.Rectangle.y = Cell.Area.y;
											Value.Rectangle.width = Cell.Area.width;
											Value.Rectangle.height = Cell.Area.height;

											Value.Pivot.x = Cell.Pivot.x;
											Value.Pivot.y = Cell.Pivot.y;
										}
									}
								}
								if(false == FlagValidCell)
								{
									Debug.LogWarning(	"SSAE-Import Warning: Part["
														+ DataParts.Name
														+ "] - Key ["
														+ KeyNo.ToString()
														+ "] Invalid Cell-Name: "
														+ ValueText
													);
									Value.Rectangle.x = 0.0f;
									Value.Rectangle.y = 0.0f;
									Value.Rectangle.width = 0.0f;
									Value.Rectangle.height = 0.0f;

									Value.Pivot.x = 0.0f;
									Value.Pivot.y = 0.0f;
								}

								DataIntermediate.KeyFrame.DataCell DataCell = new DataIntermediate.KeyFrame.DataCell();
								DataCell.Value = Value;
								KeyData = DataCell;

								FlagEnableInterpolation = false;
							}
							break;

						case DataIntermediate.KindValueKey.INSTANCE:
							{
								DataIntermediate.KeyFrame.ValueInstance Value = new DataIntermediate.KeyFrame.ValueInstance();
								Value.Flag = LibraryEditor_SpriteStudio.DataIntermediate.KeyFrame.ValueInstance.FlagData.CLEAR;

								Value.PlayCount = -1;
								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value/infinity", ManagerNameSpace);
								if(null != ValueText)
								{	/* Check */
									if(true == XMLUtility.ValueGetBool(ValueText))
									{
										Value.PlayCount = 0;
									}
								}
								if(-1 == Value.PlayCount)
								{	/* Loop-Limited */
									ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value/loopNum", ManagerNameSpace);
									Value.PlayCount = (null == ValueText) ? 1 : XMLUtility.ValueGetInt(ValueText);
								}

								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value/reverse", ManagerNameSpace);
								if(null == ValueText)
								{	/* Play Normaly */
									Value.Flag &= ~LibraryEditor_SpriteStudio.DataIntermediate.KeyFrame.ValueInstance.FlagData.REVERSE;
								}
								else
								{	/* Check */
									Value.Flag = (true == XMLUtility.ValueGetBool(ValueText)) ?
													(Value.Flag | LibraryEditor_SpriteStudio.DataIntermediate.KeyFrame.ValueInstance.FlagData.REVERSE)
													: (Value.Flag & ~LibraryEditor_SpriteStudio.DataIntermediate.KeyFrame.ValueInstance.FlagData.REVERSE);
								}
								
								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value/pingpong", ManagerNameSpace);
								if(null == ValueText)
								{	/* Play Normaly */
									Value.Flag &= ~LibraryEditor_SpriteStudio.DataIntermediate.KeyFrame.ValueInstance.FlagData.PINGPONG;
								}
								else
								{	/* Check */
									Value.Flag = (true == XMLUtility.ValueGetBool(ValueText)) ?
													(Value.Flag | LibraryEditor_SpriteStudio.DataIntermediate.KeyFrame.ValueInstance.FlagData.PINGPONG)
													: (Value.Flag & ~LibraryEditor_SpriteStudio.DataIntermediate.KeyFrame.ValueInstance.FlagData.PINGPONG);
								}
								
								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value/independent", ManagerNameSpace);
								if(null == ValueText)
								{	/* Play Normaly */
									Value.Flag &= ~LibraryEditor_SpriteStudio.DataIntermediate.KeyFrame.ValueInstance.FlagData.INDEPENDENT;
								}
								else
								{	/* Check */
									Value.Flag = (true == XMLUtility.ValueGetBool(ValueText)) ?
													(Value.Flag | LibraryEditor_SpriteStudio.DataIntermediate.KeyFrame.ValueInstance.FlagData.INDEPENDENT)
													: (Value.Flag & ~LibraryEditor_SpriteStudio.DataIntermediate.KeyFrame.ValueInstance.FlagData.INDEPENDENT);
								}

								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value/startLabel", ManagerNameSpace);
								Value.LabelStart = (null == ValueText) ? string.Copy(Library_SpriteStudio.AnimationInformationPlay.LabelDefaultStart) : string.Copy(ValueText);
						
								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value/startOffset", ManagerNameSpace);
								Value.OffsetStart = (null == ValueText) ? 0 : XMLUtility.ValueGetInt(ValueText);

								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value/endLabel", ManagerNameSpace);
								Value.LabelEnd = (null == ValueText) ? string.Copy(Library_SpriteStudio.AnimationInformationPlay.LabelDefaultEnd) : string.Copy(ValueText);

								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value/endOffset", ManagerNameSpace);
								Value.OffsetEnd = (null == ValueText) ? 0 : XMLUtility.ValueGetInt(ValueText);

								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value/speed", ManagerNameSpace);
								Value.RateTime = (null == ValueText) ? 1.0f : (float)XMLUtility.ValueGetDouble(ValueText);

								DataIntermediate.KeyFrame.DataInstance DataInstance = new DataIntermediate.KeyFrame.DataInstance();
								DataInstance.Value = Value;
								KeyData = DataInstance;

								FlagEnableInterpolation = false;
							}
							break;

						case DataIntermediate.KindValueKey.POINT:
						case DataIntermediate.KindValueKey.SOUND:
						case DataIntermediate.KindValueKey.PALETTE:
						default:
							Debug.LogWarning(	"SSAE-Import Warning: Part["
												+ DataParts.Name
												+ "] Attribute ["
												+ Description.Attribute.ToString()
												+ "] - Key ["
												+ KeyNo.ToString()
												+ "] Not-Supported Value-Kind: "
												+ Description.KindValue.ToString()
											);
							FlagEnableInterpolation = false;
							break;
					}

					KeyData.Kind = Description.Attribute;
					KeyData.Time = KeyNo + FrameNoStart;
					KeyData.Curve = (true == FlagEnableInterpolation) ? DataCurve : null;

					DataParts.DataAnimation.DataKeyFrame[AttributeNo].Add(KeyData);
				}
			}
			return(true);
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

			ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, NameTagBase + "/rgba", ManagerNameSpace);
			uint ARGB = XMLUtility.TextHexToUInt(ValueText);
			RatePixel = (float)((ARGB >> 24) & 0xff) / 255.0f;
			ColorR = (float)((ARGB >> 16) & 0xff) / 255.0f;
			ColorG = (float)((ARGB >> 8) & 0xff) / 255.0f;
			ColorB = (float)(ARGB & 0xff) / 255.0f;

			ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, NameTagBase + "/rate", ManagerNameSpace);
			ColorA = (float)(XMLUtility.ValueGetDouble(ValueText));
		}

	}

	/* File Utilities (for Native File-System) */
	internal static class File
	{
		private readonly static string NamePathRootFile = Application.dataPath;

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

		/* Convert Path-in-Asset to Native-Path */
		internal static string AssetPathToNativePath(string Name)
		{
			string NamePath = System.String.Copy(NamePathRootFile);
			if(null != Name)
			{
				NamePath += "/" + Name.Substring(AssetUtility.NamePathRootAsset.Length + 1);
			}
			return(NamePath);
		}

		internal static bool FileCopyToAsset(string NameAsset, string NameOriginalFileName, bool FlagOverCopy)
		{
			System.IO.File.Copy(NameOriginalFileName, NameAsset, true);
			return(true);
		}
	}

	/* Asset Utilities (for Unity Asset-Files) */
	internal static partial class AssetUtility
	{
		/* Unity's Assets' Root Path-Name */
		internal readonly static string NamePathRootAsset = "Assets";

		/* Unity's Path-Name to "Assets"-Origined-Path-Name */
		internal static string NamePathGet(string Name)
		{
			string NamePath = System.String.Copy(NamePathRootAsset);
			if(null != Name)
			{
				NamePath += "/" + Name;
			}

			return(NamePath);
		}

		/* Get Selected PathName (in Project-Window) */
		internal static string NamePathGetSelectNow(string NamePath)
		{
			string NamePathAsset = "";
			if(null == NamePath)
			{	/* Now Selected Path in "Project" */
				UnityEngine.Object ObjectNow = Selection.activeObject;
				if(null == ObjectNow)
				{	/* No Selected *//* Error */
//					NamePathAsset = System.String.Copy(NamePathRootAsset);
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

		/* Confirm Overwrite */
		internal static bool ObjectCheckOverwrite(UnityEngine.Object ObjectExisting, Type TypeObject, bool FlagComfirmOverwrite)
		{
			if((null != ObjectExisting) && (true == FlagComfirmOverwrite))
			{	/* Existing & Overwrite-Cheking */
				if(false == EditorUtility.DisplayDialog(	"The asset already exists.\n" + AssetDatabase.GetAssetPath(ObjectExisting),
															"Do you want to overwrite?",
															"Yes",
															"No"
														)
					)
				{	/* "No" */
					return(false);
				}
			}
			return(true);
		}

		/* Switch GameObject's Active-Status */
		internal static void GameObjectSetActive(GameObject InstanceGameObject, bool FlagSwitch)
		{
#if UNITY_3_5
			InstanceGameObject.active = FlagSwitch;
#else
			InstanceGameObject.SetActive(FlagSwitch);
#endif
		}

		/* Creating Assets */
		internal static class Create
		{
			internal static bool Folder(string Name, string NameParent)
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

				string PathFolderNative = File.AssetPathToNativePath(NameFolderParent + "/" + Name);
				if(false == Directory.Exists(PathFolderNative))
				{	/* Not Found */
					AssetDatabase.CreateFolder(NameFolderParent, Name);
				}
				return(true);
			}

			internal static UnityEngine.Object Prefab(GameObject GameObjectInstanceTop, string Name, string NamePath)
			{
				/* Check Existing */
				string NamePathAsset = NamePath + "/" + Name + "." + DataIntermediate.NameExtensionPrefab;
				UnityEngine.Object PrefabNow = AssetDatabase.LoadAssetAtPath(NamePathAsset, typeof(GameObject));
				if(null == PrefabNow)
				{	/* Create New */
					PrefabNow = PrefabUtility.CreateEmptyPrefab(NamePathAsset);
				}
				PrefabNow = PrefabUtility.ReplacePrefab(GameObjectInstanceTop, PrefabNow, ReplacePrefabOptions.Default);
				return(PrefabNow);
			}

			public static GameObject GameObjectNode(DataIntermediate.TrunkParts Trunk, DataIntermediate.PartsSprite DataParts, GameObject GameObjectParent)
			{
				GameObject GameObjectNow = AssetUtility.Create.GameObject(DataParts.Name, GameObjectParent);
				switch(DataParts.PartsKind)
				{
					case Library_SpriteStudio.KindParts.NORMAL:	/* Sprite-Node */
						{
							if(null == DataParts.DataAnimation.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.VERTEX_CORRECTION])
							{
								GameObjectNow.AddComponent<Script_SpriteStudio_Triangle2>();
							}
							else
							{
								GameObjectNow.AddComponent<Script_SpriteStudio_Triangle4>();
							}
						}
						break;

					case Library_SpriteStudio.KindParts.ROOT:	/* Root-Node */
						{
							Script_SpriteStudio_PartsRoot PartsRoot = GameObjectNow.AddComponent<Script_SpriteStudio_PartsRoot>();
							PartsRoot.BootUpForce();
							int CountAnimation = Trunk.ListInformationPlay.Length;
							PartsRoot.ListInformationPlay = new Library_SpriteStudio.AnimationInformationPlay[CountAnimation];
							for(int i=0; i<CountAnimation; i++)
							{
								PartsRoot.ListInformationPlay[i] = new Library_SpriteStudio.AnimationInformationPlay();

								PartsRoot.ListInformationPlay[i].Name = string.Copy(Trunk.ListInformationPlay[i].Name);
								PartsRoot.ListInformationPlay[i].FrameStart = Trunk.ListInformationPlay[i].FrameStart;
								PartsRoot.ListInformationPlay[i].FrameEnd = Trunk.ListInformationPlay[i].FrameEnd;
								PartsRoot.ListInformationPlay[i].FramePerSecond = Trunk.ListInformationPlay[i].FramePerSecond;

								int CountLabel = Trunk.ListInformationPlay[i].Label.Length;
								if(0 == CountLabel)
								{
									PartsRoot.ListInformationPlay[i].Label = null;
								}
								else
								{
									PartsRoot.ListInformationPlay[i].Label = new Library_SpriteStudio.AnimationInformationPlay.InformationLabel[CountLabel];
									for(int j=0; j<CountLabel; j++)
									{
										PartsRoot.ListInformationPlay[i].Label[j] = new Library_SpriteStudio.AnimationInformationPlay.InformationLabel();
										PartsRoot.ListInformationPlay[i].Label[j].Name = string.Copy(Trunk.ListInformationPlay[i].Label[j].Name);
										PartsRoot.ListInformationPlay[i].Label[j].FrameNo = Trunk.ListInformationPlay[i].Label[j].FrameNo;
									}
								}
							}

							PartsRoot.RateTimeAnimation = 1.0f;
							PartsRoot.AnimationNo = 0;
							PartsRoot.PlayTimes = 0;
							PartsRoot.FrameNoInitial = 0;
							PartsRoot.NameLabelStart = string.Copy(Library_SpriteStudio.AnimationInformationPlay.LabelDefaultStart);
							PartsRoot.OffsetFrameStart = 0;
							PartsRoot.NameLabelEnd = string.Copy(Library_SpriteStudio.AnimationInformationPlay.LabelDefaultEnd);
							PartsRoot.OffsetFrameEnd = 0;
						}
						break;

					case Library_SpriteStudio.KindParts.NULL:	/* NULL-Node */
						{
							GameObjectNow.AddComponent<Script_SpriteStudio_PartsNULL>();
						}
						break;

					case Library_SpriteStudio.KindParts.BOUND:	/* Bound-Node (not Supported SS5) */
						break;

					case Library_SpriteStudio.KindParts.SOUND:	/* Sound-Node (not Supported SS5) */
						break;

					case Library_SpriteStudio.KindParts.INSTANCE:	/* Instance-Node */
						{
							GameObjectNow.AddComponent<Script_SpriteStudio_PartsInstance>();
							GameObjectNow.AddComponent<Script_SpriteStudio_LinkPrefab>();
						}
						break;

					case Library_SpriteStudio.KindParts.EFFECT:	/* Effect-Node (Now UnSupported) */
						break;
				}
				return(GameObjectNow);
			}

			internal static GameObject GameObject(string Name, GameObject GameObjectParent)
			{
				GameObject GameObjectNow = new GameObject(Name);
				AssetUtility.GameObjectSetActive(GameObjectNow, false);
				if(null != GameObjectNow)
				{
					if(null == GameObjectParent)
					{
						GameObjectNow.transform.parent = null;
					}
					else
					{
						GameObjectNow.transform.parent = GameObjectParent.transform;
					}
					GameObjectNow.name = System.String.Copy(Name);
					GameObjectNow.transform.localPosition = Vector3.zero;
					GameObjectNow.transform.localEulerAngles = Vector3.zero;
					GameObjectNow.transform.localScale = Vector3.one;
				}
				return(GameObjectNow);
			}
		}
	}

	internal static class DataIntermediate
	{
		internal readonly static string NamePathSubImportTexture = "Texture";
		internal readonly static string NamePathSubImportMaterial = "Material";
		internal readonly static string NamePathSubImportPrefab = "Prefab";
		internal readonly static string NamePathSubImportAnimation = "Animation";

		internal readonly static string NameExtensionPrefab = "prefab";
		internal readonly static string NameExtensionAnimation = "asset";

		/* Animation-Parts Trunk */
		internal class TrunkParts
		{
			/* Animation Data (Pass-1: Unbaked Runtime-Parameters) */
			/* MEMO: This storage format had been used for Runtime until "Ver.1.1.18". */
			/*       From " Ver.1.2.0(Alpha-3)", in order of execution speed improvement, storage format has changed. */
			internal class AnimationDataRuntime
			{
				public Library_SpriteStudio.KindSprite SpriteKind;
				public Collider CollisionComponent;

				public Library_SpriteStudio.AnimationData DataMain;
				public Library_SpriteStudio.AnimationData.Plain DataPlain;	/* Pass-1 (or Plain Data) */
				public Library_SpriteStudio.AnimationData.Fix DataFix;	/* Pass-2 (Mesh/Collider Fixed-Data) */

				public AnimationDataRuntime()
				{
					SpriteKind = Library_SpriteStudio.KindSprite.NON;
					CollisionComponent = null;
					DataMain = new Library_SpriteStudio.AnimationData();
					DataPlain = new Library_SpriteStudio.AnimationData.Plain();
					DataMain.DataPlain = DataPlain;	/* Pass-1 */
					DataMain.KindDataFormat = Library_SpriteStudio.KindAnimationData.PLAIN;
					DataFix = null;
				}

				public bool ConvertPass1to2(ref SettingImport DataSettingImport)
				{
					if(false == DataSettingImport.FlagDataCalculateInAdvance)
					{	/* Not Convert (Plain-Data) */
						return(true);
					}

					/* Switch Pass-2 Data */
					DataFix = new Library_SpriteStudio.AnimationData.Fix();
					DataMain.DataFix = DataFix;
					DataMain.KindDataFormat = Library_SpriteStudio.KindAnimationData.FIX;

					/* Data-Array Create */
					int CountFrame = DataMain.AnimationDataFlags.Length;
					int CountVertexData;
					switch(SpriteKind)
					{
						case Library_SpriteStudio.KindSprite.TRIANGLE2:
							DataFix.AnimationDataMesh = new Library_SpriteStudio.AnimationData.Fix.MeshAlies[CountFrame];
							CountVertexData = (int)Library_SpriteStudio.VertexNo.TERMINATOR2;
							for(int i=0; i<CountFrame; i++)
							{
								DataFix.AnimationDataMesh[i] = new Library_SpriteStudio.AnimationData.Fix.MeshAlies(CountVertexData);
							}
							break;

						case Library_SpriteStudio.KindSprite.TRIANGLE4:
							DataFix.AnimationDataMesh = new Library_SpriteStudio.AnimationData.Fix.MeshAlies[CountFrame];
							CountVertexData = (int)Library_SpriteStudio.VertexNo.TERMINATOR4;
							for(int i=0; i<CountFrame; i++)
							{
								DataFix.AnimationDataMesh[i] = new Library_SpriteStudio.AnimationData.Fix.MeshAlies(CountVertexData);
							}
							break;

						case Library_SpriteStudio.KindSprite.NON:
						default:
							DataFix.AnimationDataMesh = null;
							CountVertexData = 0;
							break;
					}

					switch(DataMain.CollisionKind)
					{
						case Library_SpriteStudio.KindCollision.CIRCLE:
							DataFix.AnimationDataCollisionSize = null;
							DataFix.AnimationDataCollisionPivot = null;
							DataFix.AnimationDataCollisionRadius = DataPlain.AnimationDataCollisionRadius;
							break;

						case Library_SpriteStudio.KindCollision.SQUARE:
							DataFix.AnimationDataCollisionSize = new Vector2[CountFrame];
							DataFix.AnimationDataCollisionPivot = new Vector2[CountFrame];
							DataFix.AnimationDataCollisionRadius = null;
							break;

						case Library_SpriteStudio.KindCollision.NON:
						case Library_SpriteStudio.KindCollision.CIRCLE_SCALEMAXIMUM:
						case Library_SpriteStudio.KindCollision.CIRCLE_SCALEMINIMUM:
						case Library_SpriteStudio.KindCollision.AABB:
						default:
							DataFix.AnimationDataCollisionSize = null;
							DataFix.AnimationDataCollisionPivot = null;
							DataFix.AnimationDataCollisionRadius = null;
							break;
					}

					/* Baking Data (Mesh) */
					if(null != DataFix.AnimationDataMesh)
					{
						int TextureNo = -1;
						for(int i=0; i<CountFrame; i++)
						{
							if(true == DataMain.AnimationDataFlags[i].IsValid)
							{
								TextureNo = ConvertMeshPass2(i, CountVertexData);
								if(0 <= TextureNo)
								{
									DataMain.AnimationDataFlags[i].Flag &= ~Library_SpriteStudio.KeyFrame.ValueBools.FlagData.TEXTURENO;
									DataMain.AnimationDataFlags[i].Flag |= (Library_SpriteStudio.KeyFrame.ValueBools.FlagData)TextureNo & Library_SpriteStudio.KeyFrame.ValueBools.FlagData.TEXTURENO;
								}
							}
						}
					}

					/* Baking Data (Collider) */
					switch(DataMain.CollisionKind)
					{
						case Library_SpriteStudio.KindCollision.SQUARE:
							for(int i=0; i<CountFrame; i++)
							{
								if(true == DataMain.AnimationDataFlags[i].IsValid)
								{
									DataMain.CollisionSizeZ = DataSettingImport.CollisionThicknessZ;
									ConvertCollisionPass2(i);
								}
							}
							break;

						case Library_SpriteStudio.KindCollision.NON:
						case Library_SpriteStudio.KindCollision.CIRCLE:
						case Library_SpriteStudio.KindCollision.CIRCLE_SCALEMAXIMUM:
						case Library_SpriteStudio.KindCollision.CIRCLE_SCALEMINIMUM:
						case Library_SpriteStudio.KindCollision.AABB:
						default:
							break;
					}

					return(true);
				}
				private int ConvertMeshPass2(int FrameNo, int CountVertexData)
				{
					Matrix4x4 MatrixTexture = Matrix4x4.identity;
					Vector2 SizeTexture = Vector2.one;
					Vector2 RateScaleTexture = Vector2.one;
					Vector2 PivotTexture = Vector2.zero;
					Vector2 RateScaleMesh = Vector2.one;
					Vector2 PivotMesh = Vector2.zero;
					Rect RectCell = Rect.MinMaxRect(0.0f, 0.0f, 64.0f, 64.0f);
					int	VertexCollectionIndexTableNo = 0;

					int IndexCell = -1;
					int IndexTexture = -1;
					Library_SpriteStudio.KeyFrame.ValueCell.Data DataBodyCell = Library_SpriteStudio.KeyFrame.DummyDataCell;
					Library_SpriteStudio.AnimationData.Fix.MeshAlies MeshAlies = DataFix.AnimationDataMesh[FrameNo];

					/* Cell-Data Get */
					if((null != DataPlain.AnimationDataCell) && (0 < DataPlain.AnimationDataCell.Length))
					{
						IndexCell = DataPlain.AnimationDataCell[FrameNo];
					}
					if((null != DataPlain.ArrayDataBodyCell) && (0 < DataPlain.ArrayDataBodyCell.Length) && (0 <= IndexCell))
					{
						DataBodyCell = DataPlain.ArrayDataBodyCell[IndexCell];
						SizeTexture.x = DataBodyCell.SizeOriginal.x;
						SizeTexture.y = DataBodyCell.SizeOriginal.y;
					}
					if((0 <= IndexCell) && (null != DataBodyCell))
					{
						IndexTexture = DataBodyCell.TextureNo;

						RectCell = DataBodyCell.Rectangle;
						PivotTexture = new Vector2(RectCell.width * 0.5f, RectCell.height * 0.5f);

						PivotMesh = DataBodyCell.Pivot;
					}

					/* Disolve Flipping & Texture-Scaling */
					if((null != DataMain.AnimationDataFlags) && (0 < DataMain.AnimationDataFlags.Length))
					{
						RateScaleTexture.x = (true == DataMain.AnimationDataFlags[FrameNo].IsTextureFlipX) ? -1.0f : 1.0f;
						RateScaleTexture.y = (true == DataMain.AnimationDataFlags[FrameNo].IsTextureFlipY) ? -1.0f : 1.0f;
						if(true == DataMain.AnimationDataFlags[FrameNo].IsFlipX)
						{
							RateScaleMesh.x *= -1.0f;
							VertexCollectionIndexTableNo += 1;
						}
						else
						{
							RateScaleMesh.x *= 1.0f;
						}
						if(true == DataMain.AnimationDataFlags[FrameNo].IsFlipY)
						{
							RateScaleMesh.y *= -1.0f;
							VertexCollectionIndexTableNo += 2;
						}
						else
						{
							RateScaleMesh.y *= 1.0f;
						}
					}
					if((null != DataPlain.AnimationDataTextureScale) && (0 < DataPlain.AnimationDataTextureScale.Length))
					{
						RateScaleTexture.x *= DataPlain.AnimationDataTextureScale[FrameNo].x;
						RateScaleTexture.y *= DataPlain.AnimationDataTextureScale[FrameNo].y;
					}

					/* Calculate Matrix-Texture */
					float Rotate = 0.0f;
					if((null != DataPlain.AnimationDataTextureRotate) && (0 < DataPlain.AnimationDataTextureRotate.Length))
					{
						Rotate = DataPlain.AnimationDataTextureRotate[FrameNo];
					}
					Vector2 TextureOffset = Vector2.zero;
					if((null != DataPlain.AnimationDataTextureTranslate) && (0 < DataPlain.AnimationDataTextureTranslate.Length))
					{
						TextureOffset = DataPlain.AnimationDataTextureTranslate[FrameNo];
					}

					Vector3 Translation = new Vector3(	((RectCell.xMin + PivotTexture.x) / SizeTexture.x) + TextureOffset.x,
														((SizeTexture.y - (RectCell.yMin + PivotTexture.y)) / SizeTexture.y) - TextureOffset.y,
														0.0f
													);
					Vector3 Scaling = new Vector3(	(RectCell.width / SizeTexture.x) * RateScaleTexture.x,
													(RectCell.height / SizeTexture.y) * RateScaleTexture.y,
													1.0f
												);
					Quaternion Rotation = Quaternion.Euler(0.0f, 0.0f, Rotate);
					MatrixTexture = Matrix4x4.TRS(Translation, Rotation, Scaling);

					/* Set Vertex-Datas */
					Vector2[] DataUV = new Vector2[CountVertexData];
					Vector3 Coodinate = Vector3.zero;
					for(int i=0; i<CountVertexData; i++)	/* Texture-UV */
					{	/* Memo: "ArrayUVMappingUV0_Triangle4" of the data up to the "VertexNo.TERMINATOR2"-th elements are same as those of "ArrayUVMappingUV0_Triangle2". */
						Coodinate = MatrixTexture.MultiplyPoint3x4(Library_SpriteStudio.ArrayUVMappingUV0_Triangle4[i]);
						DataUV[i] = new Vector2(Coodinate.x, Coodinate.y);
					}

					float RateOpacity = 1.0f;
					if((null != DataMain.AnimationDataOpacityRate) && (0 < DataMain.AnimationDataOpacityRate.Length))
					{
						RateOpacity = DataMain.AnimationDataOpacityRate[FrameNo];
					}

					Vector2[] DataUV2 = new Vector2[CountVertexData];
					Color32[] DataColor32 = new Color32[CountVertexData];
					float OperationDefault = (float)Library_SpriteStudio.KindColorOperation.NON + 0.01f;	/* "+0.01f" for Rounding-off-Error */
					for(int i=0; i<CountVertexData; i++)
					{
						DataUV2[i] = new Vector2(RateOpacity, OperationDefault);
						DataColor32[i] = Color.white;
					}
					if((null != DataPlain.AnimationDataColorBlend) && (0 < DataPlain.AnimationDataColorBlend.Length))	/* Blending-Color & Opacity */
					{
						if(Library_SpriteStudio.KindColorBound.NON != DataPlain.AnimationDataColorBlend[FrameNo].Bound)
						{
							for(int i=0; i<CountVertexData; i++)
							{
								DataUV2[i] = new Vector2(	DataPlain.AnimationDataColorBlend[FrameNo].RatePixelAlpha[i] * RateOpacity,
															(float)DataPlain.AnimationDataColorBlend[FrameNo].Operation + 0.01f	/* "+0.01f" for Rounding-off-Error */
														);
								DataColor32[i] = DataPlain.AnimationDataColorBlend[FrameNo].VertexColor[i];
							}
						}
					}

					Vector3[] DataCoordinate = new Vector3[CountVertexData];
					if((int)Library_SpriteStudio.VertexNo.TERMINATOR4 == CountVertexData)	/* Vertex-Coordinate */
					{	/* 4-Triangles Mesh */
						/* Get SpriteSize & Pivot */
						DataPlain.SpriteRecalcSizeAndPivot(ref PivotMesh, ref RectCell, ref RateScaleMesh, FrameNo);

						/* Get Coordinates */
						/* MEMO: No Check "AnimationDataVertexCorrection.Length", 'cause 4-Triangles-Mesh necessarily has "AnimationDataVertexCorrection" */
						float Left = (-PivotMesh.x) * RateScaleMesh.x;
						float Right = (RectCell.width - PivotMesh.x) * RateScaleMesh.x;
						float Top = (-PivotMesh.y) * RateScaleMesh.y;
						float Bottom = (RectCell.height - PivotMesh.y) * RateScaleMesh.y;

						DataCoordinate[(int)Library_SpriteStudio.VertexNo.LU] =
							new Vector3(	Left + DataPlain.AnimationDataVertexCorrection[FrameNo].Coordinate[Library_SpriteStudio.VertexCollectionOrderVertex[VertexCollectionIndexTableNo, (int)Library_SpriteStudio.VertexNo.LU]].x,
											-Top + DataPlain.AnimationDataVertexCorrection[FrameNo].Coordinate[Library_SpriteStudio.VertexCollectionOrderVertex[VertexCollectionIndexTableNo, (int)Library_SpriteStudio.VertexNo.LU]].y,
											0.0f
										);
						DataCoordinate[(int)Library_SpriteStudio.VertexNo.RU] =
							new Vector3(	Right + DataPlain.AnimationDataVertexCorrection[FrameNo].Coordinate[Library_SpriteStudio.VertexCollectionOrderVertex[VertexCollectionIndexTableNo, (int)Library_SpriteStudio.VertexNo.RU]].x,
											-Top + DataPlain.AnimationDataVertexCorrection[FrameNo].Coordinate[Library_SpriteStudio.VertexCollectionOrderVertex[VertexCollectionIndexTableNo, (int)Library_SpriteStudio.VertexNo.RU]].y,
											0.0f
										);
						DataCoordinate[(int)Library_SpriteStudio.VertexNo.RD] =
							new Vector3(	Right + DataPlain.AnimationDataVertexCorrection[FrameNo].Coordinate[Library_SpriteStudio.VertexCollectionOrderVertex[VertexCollectionIndexTableNo, (int)Library_SpriteStudio.VertexNo.RD]].x,
											-Bottom + DataPlain.AnimationDataVertexCorrection[FrameNo].Coordinate[Library_SpriteStudio.VertexCollectionOrderVertex[VertexCollectionIndexTableNo, (int)Library_SpriteStudio.VertexNo.RD]].y,
											0.0f
										);
						DataCoordinate[(int)Library_SpriteStudio.VertexNo.LD] =
							new Vector3(	Left + DataPlain.AnimationDataVertexCorrection[FrameNo].Coordinate[Library_SpriteStudio.VertexCollectionOrderVertex[VertexCollectionIndexTableNo, (int)Library_SpriteStudio.VertexNo.LD]].x,
											-Bottom + DataPlain.AnimationDataVertexCorrection[FrameNo].Coordinate[Library_SpriteStudio.VertexCollectionOrderVertex[VertexCollectionIndexTableNo, (int)Library_SpriteStudio.VertexNo.LD]].y,
											0.0f
										);
						Vector3 CoordinateLURU = (DataCoordinate[(int)Library_SpriteStudio.VertexNo.LU] + DataCoordinate[(int)Library_SpriteStudio.VertexNo.RU]) * 0.5f;
						Vector3 CoordinateLULD = (DataCoordinate[(int)Library_SpriteStudio.VertexNo.LU] + DataCoordinate[(int)Library_SpriteStudio.VertexNo.LD]) * 0.5f;
						Vector3 CoordinateLDRD = (DataCoordinate[(int)Library_SpriteStudio.VertexNo.LD] + DataCoordinate[(int)Library_SpriteStudio.VertexNo.RD]) * 0.5f;
						Vector3 CoordinateRURD = (DataCoordinate[(int)Library_SpriteStudio.VertexNo.RU] + DataCoordinate[(int)Library_SpriteStudio.VertexNo.RD]) * 0.5f;
						Library_SpriteStudio.AnimationData.Plain.CoordinateGetDiagonalIntersection(
																		out DataCoordinate[(int)Library_SpriteStudio.VertexNo.C],
																		ref CoordinateLURU,
																		ref CoordinateRURD,
																		ref CoordinateLULD,
																		ref CoordinateLDRD
																	);
					}
					else
					{	/* 2-Triangles Mesh */
						/* Get SpriteSize & Pivot */
						DataPlain.SpriteRecalcSizeAndPivot(ref PivotMesh, ref RectCell, ref RateScaleMesh, FrameNo);

						/* Get Coordinates */
						float Left = (-PivotMesh.x) * RateScaleMesh.x;
						float Right = (RectCell.width - PivotMesh.x) * RateScaleMesh.x;
						float Top = (-PivotMesh.y) * RateScaleMesh.y;
						float Bottom = (RectCell.height - PivotMesh.y) * RateScaleMesh.y;

						DataCoordinate[(int)Library_SpriteStudio.VertexNo.LU] = new Vector3(Left, -Top, 0.0f);
						DataCoordinate[(int)Library_SpriteStudio.VertexNo.RU] = new Vector3(Right, -Top, 0.0f);
						DataCoordinate[(int)Library_SpriteStudio.VertexNo.RD] = new Vector3(Right, -Bottom, 0.0f);
						DataCoordinate[(int)Library_SpriteStudio.VertexNo.LD] = new Vector3(Left, -Bottom, 0.0f);
					}

					/* Set Frame-Data */
					MeshAlies.Coordinate = DataCoordinate;
					MeshAlies.ColorOverlay = DataColor32;
					MeshAlies.UV = DataUV;
					MeshAlies.UV2 = DataUV2;

					return(IndexTexture);
				}
				private static void CoordinateGetDiagonalIntersection(out Vector3 Output, ref Vector3 LU, ref Vector3 RU, ref Vector3 LD, ref Vector3 RD)
				{
					Output = Vector3.zero;

					float c1 = (LD.y - RU.y) * (LD.x - LU.x) - (LD.x - RU.x) * (LD.y - LU.y);
					float c2 = (RD.x - LU.x) * (LD.y - LU.y) - (RD.y - LU.y) * (LD.x - LU.x);
					float c3 = (RD.x - LU.x) * (LD.y - RU.y) - (RD.y - LU.y) * (LD.x - RU.x);
					float ca = c1 / c3;
					float cb = c2 / c3;

					if(((0.0f <= ca) && (1.0f >= ca)) && ((0.0f <= cb) && (1.0f >= cb)))
					{
						Output.x = LU.x + ca * (RD.x - LU.x);
						Output.y = LU.y + ca * (RD.y - LU.y);
					}
				}
				private void ConvertCollisionPass2(int FrameNo)
				{
					/* Calculate Sprite-Parts size */
					Vector2 PivotCell = Vector2.zero;
					Vector2 RateScaleMesh = Vector2.one;
					Rect RectCell = Rect.MinMaxRect(0.0f, 0.0f, 64.0f, 64.0f);
					int IndexCell = -1;
					Library_SpriteStudio.KeyFrame.ValueCell.Data DataBodyCell = Library_SpriteStudio.KeyFrame.DummyDataCell;

					/* Cell-Data Get */
					if(null != DataPlain.AnimationDataCell)
					{
						if(0 < DataPlain.AnimationDataCell.Length)
						{
							IndexCell = DataPlain.AnimationDataCell[FrameNo];
						}
					}
					if((null != DataPlain.ArrayDataBodyCell) && (0 < DataPlain.ArrayDataBodyCell.Length) && (0 <= IndexCell))
					{
						DataBodyCell = DataPlain.ArrayDataBodyCell[IndexCell];
						RectCell.width = DataBodyCell.Rectangle.width;
						RectCell.height = DataBodyCell.Rectangle.height;

						PivotCell = DataBodyCell.Pivot;
						PivotCell.x -= (RectCell.width * 0.5f);
						PivotCell.y -= (RectCell.height * 0.5f);
					}
					if((null != DataMain.AnimationDataFlags) && (0 < DataMain.AnimationDataFlags.Length))
					{
						RateScaleMesh.x = (true == DataMain.AnimationDataFlags[FrameNo].IsFlipX) ? -1.0f : 1.0f;
						RateScaleMesh.y = (true == DataMain.AnimationDataFlags[FrameNo].IsFlipY) ? -1.0f : 1.0f;
					}

					/* Accommodate Pivot's-Offset */
					Vector2 PivotCollide = Vector2.zero;
					DataPlain.SpriteRecalcSizeAndPivot(ref PivotCollide, ref RectCell, ref RateScaleMesh, FrameNo);

					Vector3 PivotNew = Vector3.zero;
					PivotNew.x = -(PivotCollide.x + PivotCell.x) * RateScaleMesh.x;
					PivotNew.y = (PivotCollide.y + PivotCell.y) * RateScaleMesh.y;
					PivotNew.z = 0.0f;

					/* Get Collision-Size */
					Vector3 SizeNew = Vector3.zero;
					SizeNew.x = RectCell.width;
					SizeNew.y = RectCell.height;
					SizeNew.z = DataMain.CollisionSizeZ;

					/* Set Frame-Data */
					DataFix.AnimationDataCollisionPivot[FrameNo] = PivotNew;
					DataFix.AnimationDataCollisionSize[FrameNo] = SizeNew;
				}
			}

			internal PartsSprite[] ListParts = null;
			internal PartsImage[] ListImage = null;
			internal Library_SpriteStudio.AnimationInformationPlay[] ListInformationPlay = null;
			internal ArrayList ListPartsInstance = null;

			internal AnimationDataRuntime[] ListDataRuntime = null;

			internal ParseOPSS.KindVersionSSAE VersionCodeSSAE;

			internal int CountNode = -1;
			internal int CountFrameFull = -1;
			internal bool FlameFlipForImageOnly = false;

			internal GameObject GameObjectRoot = null;
			internal UnityEngine.Object PrefabData = null;
			internal UnityEngine.Object PrefabDataOld = null;

			internal Script_SpriteStudio_AnimationReferenced DataAnimationReferenced = null;
			internal ScriptableObject PrefabDataAnimationReferenced = null;

			internal void BootUp()
			{
				ListPartsInstance = new ArrayList();
				ListPartsInstance.Clear();
				ListDataRuntime = null;

				GameObjectRoot = null;
				PrefabData = null;
				PrefabDataOld = null;

				DataAnimationReferenced = null;
				PrefabDataAnimationReferenced = null;
			}

			/* Create Destination Folders */
			internal static string CreateDestinationFolder(string NamePath, string NamePathSubFolder)
			{
				string NamePathBase = String.Copy(NamePath);

				/* Create Project-Name Folder */
				if(null != NamePathSubFolder)
				{
					AssetUtility.Create.Folder(NamePathSubFolder, NamePathBase);
					NamePathBase += "/" + NamePathSubFolder;
				}

				return(NamePathBase);
			}

			/* Asset Allocation Informations */
			/* MEMO: OrderedDictionarys' "value" for List-Materials & List-Textures */
			internal class InformationTexture
			{
				internal Texture2D SubstanceAsset;
				internal int IndexImage;

				internal InformationTexture()
				{
					SubstanceAsset = null;
					IndexImage = -1;
				}
			}
			internal class InformationMaterial
			{
				internal Material SubstanceAsset;
				internal int IndexTextureInformation;

				internal InformationMaterial()
				{
					SubstanceAsset = null;
					IndexTextureInformation = -1;
				}
			}
			/* Create Assets (Materials & Textures) */
			internal void GetAssetMaterial(	out int[] TableIndexMaterial,
											OrderedDictionary ListMaterial,
											OrderedDictionary ListTexture,
											string NamePathBase,
											string NameFileBodySSAE,
											ref SettingImport DataSettingImport
										)
			{
				GameObject ObjectRootParts = PrefabDataOld as GameObject;
				int CountTexture = ListImage.Length;
				int CountMaterial = CountTexture * ShaderOperationMax;
				TableIndexMaterial = new int[CountMaterial];
				for(int i=0; i<CountMaterial; i++)
				{
					TableIndexMaterial[i] = -1;
				}

				/* Create Material-Table */
				if((null == ObjectRootParts) || (false == DataSettingImport.FlagGetMaterialPartsRoot))
				{	/* Create-New */
					goto GetAssetMaterial_CreateNew;
				}
				else
				{	/* Update */
					Script_SpriteStudio_PartsRoot ScriptRoot = ObjectRootParts.GetComponent<Script_SpriteStudio_PartsRoot>();
					if(null != ScriptRoot)
					{
						/* Copy Material & Texture */
						int CountMaterialOld = ScriptRoot.TableMaterial.Length;
						if(CountMaterial != CountMaterialOld)
						{	/* Different count: Create-New */
#if false
							int CountTextureOld = ScriptRoot.TableMaterial.Length / ShaderOperationMax;
							for(int i=0; i<CountTexture; i++)
							{
								if(i < CountTextureOld)
								{
									GetAssetMaterial_SetUpdate(	TableIndexMaterial,
																ListMaterial, ListTexture,
																NamePathBase, NameFileBodySSAE,
																i,
																ScriptRoot,
																ref DataSettingImport
															);
								}
								else
								{	/* Create Asset in Default-Folder */
									GetAssetMaterial_CreateNew(	TableIndexMaterial,
																ListMaterial, ListTexture,
																NamePathBase, NameFileBodySSAE,
																i
															);
								}
							}
#else
							/* MEMO: If the number of the material does not match        */
							/*        the assigned texture can not be tracked correctly. */
							Debug.LogWarning(	"Material-Update-Check Warning:" +
												" Materials are regenerated forcibly in default folder." +
												" Because the number of material does not match the old data." +
												" [" + NameFileBodySSAE + ".ssae]");
							goto GetAssetMaterial_CreateNew;
#endif
						}
						else
						{	/* Same count */
							for(int i=0; i<CountTexture; i++)
							{
								GetAssetMaterial_SetUpdate(	TableIndexMaterial,
															ListMaterial, ListTexture,
															NamePathBase, NameFileBodySSAE,
															i,
															ScriptRoot,
															ref DataSettingImport
														);
							}
						}
					}
					else
					{	/* Not-Found(Error) ... Create-New */
						goto GetAssetMaterial_CreateNew;
					}
				}
				return;

			GetAssetMaterial_CreateNew:;
				/* Create Material-Table */
				for(int i=0; i<CountTexture; i++)
				{
					GetAssetMaterial_CreateNew(	TableIndexMaterial,
												ListMaterial, ListTexture,
												NamePathBase, NameFileBodySSAE,
												i
											);
				}
				return;
			}
			private void GetAssetMaterial_CreateNew(	int[] TableIndexMaterial,
														OrderedDictionary ListMaterial,
														OrderedDictionary ListTexture,
														string NamePathBase,
														string NameBodySSAE,
														int IndexImage
													)
			{
				string NameFileAsset = "";
				string NameFileBody = "";
				string NameFileExtension = "";

				/* Create Texture-Asset's Name */
				NameFileBody = Path.GetFileNameWithoutExtension(ListImage[IndexImage].FileName);
				NameFileExtension = Path.GetExtension(ListImage[IndexImage].FileName);
				NameFileAsset = NamePathBase + "/" + NamePathSubImportTexture + "/" + NameFileBody + NameFileExtension;
				InformationTexture DataTextureInformation = new InformationTexture();
				if(false == ListTexture.Contains(NameFileAsset))
				{	/* Unregistered */
					DataTextureInformation.SubstanceAsset = null;
					DataTextureInformation.IndexImage = IndexImage;
					ListTexture.Add(NameFileAsset, DataTextureInformation);
				}
				int IndexTextureList = Utility.IndexGetOrderedDictionary(ListTexture, NameFileAsset);

				/* Create Material-Asset's Name */
				int IndexMaterialTop = IndexImage * ShaderOperationMax;
				NameFileAsset = NamePathBase + "/" + NamePathSubImportMaterial + "/" + NameFileBody;
				string NameFileMaterial = "";
				Library_SpriteStudio.KindColorOperation	KindOperation = Library_SpriteStudio.KindColorOperation.NON;
				InformationMaterial DataMaterialInformation = null;
				for(int i=0; i<ShaderOperationMax; i++)
				{
					DataMaterialInformation = new InformationMaterial();
					KindOperation = (Library_SpriteStudio.KindColorOperation)(i + 1);
					NameFileMaterial = NameFileAsset + "_" + (KindOperation.ToString()) + ".mat";
					if(false == ListMaterial.Contains(NameFileMaterial))
					{	/* Unregistered */
						DataMaterialInformation.SubstanceAsset = null;
						DataMaterialInformation.IndexTextureInformation = IndexTextureList;
						ListMaterial.Add(NameFileMaterial, DataMaterialInformation);
					}
					TableIndexMaterial[IndexMaterialTop + i] = Utility.IndexGetOrderedDictionary(ListMaterial, NameFileMaterial);
				}
			}
			private void GetAssetMaterial_SetUpdate(	int[] TableIndexMaterial,
														OrderedDictionary ListMaterial,
														OrderedDictionary ListTexture,
														string NamePathBase,
														string NameBodySSAE,
														int IndexImage,
														Script_SpriteStudio_PartsRoot ScriptRoot,
														ref SettingImport DataSettingImport
													)
			{
				int IndexTextureList = -1;
				Material MaterialNow = null;
				Texture2D TextureNow = null;
				string NamePathAssetMaterial = "";
				string NamePathAssetTexture = "";
				InformationTexture DataTextureInformation = null;
				InformationMaterial DataMaterialInformation = null;

				/* Meterial Get */
				int IndexMaterialTop = IndexImage * ShaderOperationMax;
				for(int i=0; i<ShaderOperationMax; i++)
				{
					MaterialNow = ScriptRoot.TableMaterial[IndexMaterialTop + i];
					if(null != MaterialNow)
					{	/* Exist */
						NamePathAssetMaterial = AssetDatabase.GetAssetPath(MaterialNow);
						if(false == ListMaterial.Contains(NamePathAssetMaterial))
						{	/* Unregistered */
							if(true == DataSettingImport.FlagGetTextureMaterial)
							{	/* Overwrite */
								/* Texture Get */
								TextureNow = MaterialNow.mainTexture as Texture2D;
								if(null != TextureNow)
								{
									NamePathAssetTexture = AssetDatabase.GetAssetPath(TextureNow);
									if(false == ListTexture.Contains(NamePathAssetTexture))
									{	/* Unregistered */
										/* Create Texture-Information */
										DataTextureInformation = new InformationTexture();
										DataTextureInformation.SubstanceAsset = TextureNow;
										DataTextureInformation.IndexImage = IndexImage;
										ListTexture.Add(NamePathAssetTexture, DataTextureInformation);
									}
									IndexTextureList = Utility.IndexGetOrderedDictionary(ListTexture, NamePathAssetTexture);
								}
								else
								{	/* Error: Texture is not created*/
									IndexTextureList = -1;
								}
							}
							else
							{	/* Create-New, Force */
								if(false == ListTexture.Contains(NamePathAssetTexture))
								{	/* Unregistered */
									string NameFileBody = "";
									string NameFileExtension = "";

									/* Create Texture-Information */
									NameFileBody = Path.GetFileNameWithoutExtension(ListImage[IndexImage].FileName);
									NameFileExtension = Path.GetExtension(ListImage[IndexImage].FileName);
									NamePathAssetTexture = NamePathBase + "/" + NamePathSubImportTexture + "/" + NameFileBody + NameFileExtension;

									DataTextureInformation = new InformationTexture();
									DataTextureInformation.SubstanceAsset = null;
									DataTextureInformation.IndexImage = IndexImage;
									ListTexture.Add(NamePathAssetTexture, DataTextureInformation);
								}
								IndexTextureList = Utility.IndexGetOrderedDictionary(ListTexture, NamePathAssetTexture);
							}

							/* Create Material-Information */
							DataMaterialInformation = new InformationMaterial();
							DataMaterialInformation.SubstanceAsset = MaterialNow;
							DataMaterialInformation.IndexTextureInformation = IndexTextureList;
							ListMaterial.Add(NamePathAssetMaterial, DataMaterialInformation);
						}
						TableIndexMaterial[IndexMaterialTop + i] = Utility.IndexGetOrderedDictionary(ListMaterial, NamePathAssetMaterial);
					}
					else
					{	/* Error: Material is not created */
						TableIndexMaterial[IndexMaterialTop + i] = -1;
					}
				}
			}
			internal void UpdateAssetMaterial(	out Material[] TableMaterial,
												ref bool[] FlagCreateMaterial,
												ref bool[] FlagCreateTexture,
												int[] TableIndexMaterial,
												OrderedDictionary ListMaterial,
												OrderedDictionary ListTexture,
												string NameFileBody,
												string NamePathBase,
												ref SettingImport DataSettingImport
											)
			{
				ICollection CollectionKey = null;
				InformationTexture TextureInformation = null;
				InformationMaterial MaterialInformation = null;

				/* Create Material Table */
				int CountMaterial = TableIndexMaterial.Length;
				int CountTexture = ListTexture.Count;
				TableMaterial = new Material[CountMaterial];
				string NameAsset = "";

				/* Update Texture */
				if(0 < CountTexture)
				{
					/* Get Asset-Names */
					CollectionKey = ListTexture.Keys;
					string[] Keys = new string[CountTexture];
					CollectionKey.CopyTo(Keys, 0);
					for(int i=0; i<CountTexture; i++)
					{
						/* Texture File Copy */
						NameAsset = File.AssetPathToNativePath(Keys[i]);
						TextureInformation = ListTexture[Keys[i]] as InformationTexture;
						if(null == TextureInformation.SubstanceAsset)
						{	/* No Existing */
							/* Create Destination-Folders */
							AssetUtility.Create.Folder(NamePathSubImportTexture, NamePathBase);
						}
						File.FileCopyToAsset(	NameAsset,
												ListImage[TextureInformation.IndexImage].FileName,
												true
											);
						NameAsset = String.Copy(Keys[i]);
						if(false == FlagCreateTexture[i])
						{
							/* Importer Setting */
							AssetDatabase.ImportAsset(NameAsset);
							TextureImporter Importer = TextureImporter.GetAtPath(NameAsset) as TextureImporter;
							if(null != Importer)
							{
								Importer.anisoLevel = 1;
								Importer.borderMipmap = false;
								Importer.convertToNormalmap = false;
								Importer.fadeout = false;
								switch(ListImage[TextureInformation.IndexImage].Filter)
								{
									case PartsImage.KindFilter.NEAREST:
										Importer.filterMode = FilterMode.Point;
										break;

									case PartsImage.KindFilter.LINEAR:
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
								Importer.textureType  = TextureImporterType.Advanced;
								switch(ListImage[TextureInformation.IndexImage].Wrap)
								{
									case PartsImage.KindWrap.REPEAT:
										Importer.wrapMode = TextureWrapMode.Repeat;
										break;

									case PartsImage.KindWrap.MIRROR:
									case PartsImage.KindWrap.CLAMP:
									default:
										Importer.wrapMode = TextureWrapMode.Clamp;
										break;
								}
								AssetDatabase.ImportAsset(NameAsset, ImportAssetOptions.ForceUpdate);
							}
							TextureInformation.SubstanceAsset = AssetDatabase.LoadAssetAtPath(NameAsset, typeof(Texture2D)) as Texture2D;
							FlagCreateTexture[i] = true;

							/* Texture's Pixel-Size Check */
							if(	(true != PixelSizeCheck(TextureInformation.SubstanceAsset.width))
								|| (true != PixelSizeCheck(TextureInformation.SubstanceAsset.height))
							)
							{
								Debug.LogWarning("SSCE-Texture Warning: CellMap Pixel-Size is not multiples of powerd by 2: (" + ListImage[i].FileName + ")");
							}

							/* Fixing Created Assets */
							AssetDatabase.SaveAssets();
						}
					}
				}

				/* Update Material */
				if(0 < CountMaterial)
				{
					/* Get Asset-Names */
					CollectionKey = ListMaterial.Keys;
					string[] Keys = new string[CountMaterial];
					CollectionKey.CopyTo(Keys, 0);
					int IndexMaterial = -1;
					int OperationNo = -1;
					Material MaterialNew = null;
					for(int i=0; i<CountMaterial; i++)
					{
						OperationNo = i % ShaderOperationMax;
						IndexMaterial = TableIndexMaterial[i];
						if(0 > IndexMaterial)
						{	/* No-Creating */
							TableMaterial[i] = null;
						}
						else
						{
							MaterialInformation = ListMaterial[IndexMaterial] as InformationMaterial;
							NameAsset = String.Copy(Keys[IndexMaterial]);
							if(false == FlagCreateMaterial[IndexMaterial])
							{	/* Create-New */
								if(null != MaterialInformation.SubstanceAsset)
								{	/* Overwrite */
									MaterialNew = MaterialInformation.SubstanceAsset;
								}
								else
								{	/* Create-New */
									/* Create Destination-Folders */
									AssetUtility.Create.Folder(NamePathSubImportMaterial, NamePathBase);

									/* Create new Material */
									MaterialNew = new Material(Library_SpriteStudio.Shader_SpriteStudioTriangleX[OperationNo]);
									AssetDatabase.CreateAsset(MaterialNew, NameAsset);
								}
								TextureInformation = ListTexture[MaterialInformation.IndexTextureInformation] as InformationTexture;
								MaterialNew.mainTexture = TextureInformation.SubstanceAsset;
								AssetDatabase.SaveAssets();

								TableMaterial[i] = AssetDatabase.LoadAssetAtPath(NameAsset, typeof(Material)) as Material;
								MaterialInformation.SubstanceAsset = TableMaterial[i];
								FlagCreateMaterial[IndexMaterial] = true;
							}
							else
							{	/* Already Created */
								MaterialInformation = ListMaterial[IndexMaterial] as InformationMaterial;
								TableMaterial[i] = MaterialInformation.SubstanceAsset;
							}
						}
					}
				}
			}

			/* Legal-Check Texture's-Pixel-Size, */
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

			/* Create GameObjects */
			public bool CreateDataGameObjectSprite(	string Name,
													string NamePath,
													Material[] TableMaterial,
													ref LibraryEditor_SpriteStudio.SettingImport DataSettingImport
												)
			{
				CurrentProcessingPartsName = "";

				/* "Root" Node(GameObject)s Create (on Scene) */
				GameObjectRoot = AssetUtility.Create.GameObjectNode(this, ListParts[0], null);
				if(null == GameObjectRoot)
				{	/* Error */
					return(false);
				}
				ListParts[0].GameObjectParts = GameObjectRoot;
				Script_SpriteStudio_PartsRoot ScriptRoot = GameObjectRoot.GetComponent<Script_SpriteStudio_PartsRoot>();
				ScriptRoot.TableMaterial = TableMaterial;

				/* Ordinary-Nodes Create (on Scene) */
				GameObject[] GameObjectParts = new GameObject[ListParts.Length];
				GameObjectParts[0] = GameObjectRoot;
				for(int i=1; i<ListParts.Length; i++)
				{
					/* Create GameObjects and Attach Script */
					GameObjectParts[i] = AssetUtility.Create.GameObjectNode(this, ListParts[i], GameObjectRoot);
					ListParts[i].GameObjectParts = GameObjectParts[i];
				}

				/* Set Parent-Child-Relation */
				GameObject GameObjectParent = null;
				int ParentNo = -1;
				for(int i=1; i<ListParts.Length; i++)
				{
					ParentNo = ListParts[i].IDParent;
					if(0 < ParentNo)
					{
						GameObjectParent = GameObjectParts[ParentNo];
						GameObjectParts[i].transform.parent = GameObjectParent.transform;
					}
				}

				/* Animation Converting "Pass-1" (Intermediate to Runtime) & Attaching Collider */
				ListDataRuntime = new AnimationDataRuntime[ListParts.Length];
				{
					for(int i=0; i<ListParts.Length; i++)
					{
						/* Create Animation-Data for Converting-Temporary */
						ListDataRuntime[i] = new AnimationDataRuntime();

						/* Create Animation-Data for Runtime */
						CurrentProcessingPartsName = ListParts[i].Name;
						NodeSetAnimation(GameObjectParts[i], i, ScriptRoot, TableMaterial);

						/* Attach Collider (to GameObject) */
						NodeSetCollider(GameObjectParts[i], i, ref DataSettingImport);
					}
					CurrentProcessingPartsName = "";

					/* Solving Inherit-Attributes */
					/* MEMO: Child-nodes only (Parent-node need not solve) */
					/* MEMO: Child-node's ID might not be smaller than Parent-Node's ID. */
					/*       However...To make sure..."Inherit" is solved after all nodes'data are decided. */
					for(int i=1; i<ListParts.Length; i++)
					{
						DataRuntimeSolvingInherit(i);
					}
				}

				/* Animation Converting "Pass-2" (Baking Mesh-Data) */
				for(int i=0; i<ListParts.Length; i++)
				{
					ListDataRuntime[i].ConvertPass1to2(ref DataSettingImport);
				}
				return(true);
			}

			/* Create Prefab (Sprite-Data) */
			private readonly static string NamePrefabControlSuffix = "_Control";
			public bool CreateDataPrefabSprite( DataIntermediate.TrunkParts[] ArrayTrankParts,
												string Name,
												string NamePath,
												ParseOPSS.InformationSSPJ InformationSSPJ,
												ref LibraryEditor_SpriteStudio.SettingImport DataSettingImport
											)
			{
				/* File Overwrite Check */
				bool FlagOverwrite = PrefabUpdateCheck(	out PrefabData,
														out PrefabDataAnimationReferenced,
														Name,
														NamePath,
														DataSettingImport.FlagConfirmOverWrite,
														DataSettingImport.FlagGetAnimationReferencedPartsRoot
													);
				if(false == FlagOverwrite)
				{	/* Not-OverWrite */
					if(null != GameObjectRoot)
					{
						UnityEngine.Object.DestroyImmediate(GameObjectRoot);
					}
					return(true);
				}

				/* Set Prefab-Ref.("Instance"-Parts) */
				{
					int Count = ListPartsInstance.Count;
					GameObject GameObjectPartsInstance = null;
					LibraryEditor_SpriteStudio.Menu.InformationInstance InformationInstance = null;
					Script_SpriteStudio_LinkPrefab ScriptLinkPrefab = null;
					for(int i=0; i<Count; i++)
					{
						/* Get "Instance"-Parts */
						InformationInstance = ListPartsInstance[i] as LibraryEditor_SpriteStudio.Menu.InformationInstance;
						GameObjectPartsInstance = GameObjectGetID(InformationInstance.ID);
						ScriptLinkPrefab = GameObjectPartsInstance.GetComponent<Script_SpriteStudio_LinkPrefab>();
						if(null != ScriptLinkPrefab)
						{
							int IndexPrefab = InformationSSPJ.ArraySearchFileNameBody(InformationSSPJ.ListSSAE, InformationInstance.NameInstanceSSAE);
							if(0 <= IndexPrefab)
							{
								ScriptLinkPrefab.LinkPrefab = ArrayTrankParts[IndexPrefab].PrefabData;
								ScriptLinkPrefab.FlagDeleteScript = false;
								ScriptLinkPrefab.FlagAutoDevelop = false;
							}

							/* Get "Instance"-Object's Animation Index */
							Script_SpriteStudio_PartsInstance ScriptInstance = GameObjectPartsInstance.GetComponent<Script_SpriteStudio_PartsInstance>();
							GameObject GameObjectInstanceObject = (GameObject)PrefabUtility.InstantiatePrefab(ArrayTrankParts[IndexPrefab].PrefabData);
							Script_SpriteStudio_PartsRoot ScriptRootInstanceObject = GameObjectInstanceObject.GetComponent<Script_SpriteStudio_PartsRoot>();
							ScriptInstance.AnimationNo = ScriptRootInstanceObject.AnimationGetIndexNo(InformationInstance.NameInstanceAnimation);
							ScriptInstance.AnimationNo = (-1 == ScriptInstance.AnimationNo) ? 0 : ScriptInstance.AnimationNo;	/* Error */
							UnityEngine.Object.DestroyImmediate(GameObjectInstanceObject);
						}
					}
				}

				/* Parts' Animation-Data Transfer to "Refernced-Animation-Data" */
				{
					int Count = 0;

					/* Animation-Information-Data Transfar */
					Count = ListInformationPlay.Length;
					DataAnimationReferenced.ListInformationAnimation = new Library_SpriteStudio.AnimationInformationPlay[Count];
					for(int i=0; i<Count; i++)
					{
						DataAnimationReferenced.ListInformationAnimation[i] = ListInformationPlay[i];
					}

					/* Nodes'-Animation-Data Transfar */
					Count = ListParts.Length;
					DataAnimationReferenced.ListNodeAnimationData = new Library_SpriteStudio.AnimationData[Count];
					for(int i=0; i<Count; i++)
					{
						DataAnimationReferenced.ListNodeAnimationData[i] = ListDataRuntime[i].DataMain;
					}

					DataAnimationReferenced.Compress();
					EditorUtility.SetDirty(DataAnimationReferenced);
				}

				/* Set ScriptableObject to Root-Parts */
				Script_SpriteStudio_PartsRoot PartsRoot = GameObjectRoot.GetComponent<Script_SpriteStudio_PartsRoot>();
				PartsRoot.SpriteStudioDataReferenced = DataAnimationReferenced;

				/* Set Nodes Active */
				{
					AssetUtility.GameObjectSetActive(GameObjectRoot, true);
					GameObject GameObjectParts = null;
					for(int i=0; i<ListParts.Length; i++)
					{
						GameObjectParts = ListParts[i].GameObjectParts;
						if(null != GameObjectParts)
						{
							AssetUtility.GameObjectSetActive(GameObjectParts, true);
						}
					}
				}

				/* Create Prefab (GameObjects) */
#if false
				/* MEMO: until Ver.1.2.9 */
				PrefabData = PrefabUtility.ReplacePrefab(GameObjectRoot, PrefabData, ReplacePrefabOptions.Default);
#else
				/* MEMO: since Ver.1.2.11 */
				PrefabData = PrefabUtility.ReplacePrefab(GameObjectRoot, PrefabData, ReplacePrefabOptions.ReplaceNameBased);
#endif
				if(null == PrefabData)
				{
					return(false);
				}

				/* Fixing Created Assets */
				AssetDatabase.SaveAssets();

				/* Deleting Tempolary-Instance */
				UnityEngine.Object.DestroyImmediate(GameObjectRoot);

				/* "Control" Node Attached Prefab Create */
				{
					/* Existing Check */
					string NameControl = Name + NamePrefabControlSuffix;
					string NamePathAssetControl = NamePath + "/" + NameControl + ".prefab";
					UnityEngine.Object ObjectExisting = AssetDatabase.LoadAssetAtPath(NamePathAssetControl, typeof(GameObject));
					if(null == ObjectExisting)
					{	/* No existing */
						if(true == DataSettingImport.FlagAttachControlGameObject)
						{
							/* "Control" Node(GameObject)s Create (on Scene) */
							GameObject GameObjectControl = AssetUtility.Create.GameObject(NameControl, null);
							AssetUtility.GameObjectSetActive(GameObjectControl, true);

							/*  Attach Link-Prefab Script */
							Script_SpriteStudio_LinkPrefab ScriptLinkPrefab = GameObjectControl.AddComponent<Script_SpriteStudio_LinkPrefab>();
							ScriptLinkPrefab.LinkPrefab = PrefabData;
							ScriptLinkPrefab.FlagDeleteScript = false;
							ScriptLinkPrefab.FlagAutoDevelop = true;

							/* Create Control Prefab */
							/* MEMO: Not Confirm Overwrite */
							UnityEngine.Object PrefabControl = AssetUtility.Create.Prefab(GameObjectControl, NameControl, NamePath);
							if(null == PrefabControl)
							{
								Debug.LogError("Miss-Creating[" + Name + "]");
								return(false);
							}

							/* Fixing Created Assets */
							AssetDatabase.SaveAssets();

							/* Deleting Tempolary-Instance */
							UnityEngine.Object.DestroyImmediate(GameObjectControl);
						}
					}
					else
					{	/* Existing */
						/* Overwrite SpritePrefab-Link */
						GameObject GameObjectControl = (GameObject)ObjectExisting;
						if(null != GameObjectControl)
						{
							Script_SpriteStudio_LinkPrefab ScriptLinkPrefab = GameObjectControl.GetComponent<Script_SpriteStudio_LinkPrefab>();
							if(null != ScriptLinkPrefab)
							{
								ScriptLinkPrefab.LinkPrefab = PrefabData;
							}

							/* Fixing Created Assets */
							AssetDatabase.SaveAssets();
						}
					}
				}

				return(true);
			}
			private GameObject GameObjectGetID(int NodeID)
			{
				for(int i=0; i<ListParts.Length; i++)
				{
					if(NodeID == ListParts[i].ID)
					{
						return(ListParts[i].GameObjectParts);
					}
				}
				return(null);
			}
			private bool PrefabUpdateCheck(	out UnityEngine.Object PrefabData,
											out ScriptableObject ObjectDataAnimationReferenced,
											string Name,
											string NamePath,
											bool FlagConfirmOverWrite,
											bool FlagGetAnimationReferencedPartsRoot
										)
			{
				/* Check Existing */
				string NamePathDataParts = NamePath + "/" + NamePathSubImportPrefab + "/" + Name + "." + NameExtensionPrefab;
				string NamePathDataReferenced = NamePath + "/" + NamePathSubImportAnimation + "/" + Name + "." + NameExtensionAnimation;
				ObjectDataAnimationReferenced = AssetDatabase.LoadAssetAtPath(NamePathDataReferenced, typeof(ScriptableObject)) as ScriptableObject;
				PrefabData = PrefabDataOld;
				if(false == AssetUtility.ObjectCheckOverwrite(	PrefabData,
																typeof(GameObject),
																FlagConfirmOverWrite
															)
					)
				{	/* Cancelled */
					Debug.Log("SSAE-Create-Prefab: Not-Overwritten Prefab[" + NamePathDataParts + "]");
					return(false);
				}

				if(null == PrefabData)
				{	/* Data Not-Found */
					/* Create Destination Folder */
					AssetUtility.Create.Folder(NamePathSubImportPrefab, NamePath);
					PrefabData = PrefabUtility.CreateEmptyPrefab(NamePathDataParts);
				}
				else
				{	/* Data Exist */
					if(true == FlagGetAnimationReferencedPartsRoot)
					{	/* Get from Prefab-Data */
						if(null != PrefabDataOld)
						{	/* Old-Data is existing */
							GameObject GameObjectRootOld = PrefabDataOld as GameObject;
							Script_SpriteStudio_PartsRoot ScriptRootOld = GameObjectRootOld.GetComponent<Script_SpriteStudio_PartsRoot>();
							if(null != ScriptRootOld)
							{
								string NamePathDataReferencedOld = AssetDatabase.GetAssetPath(ScriptRootOld.SpriteStudioDataReferenced);
								if(false == String.IsNullOrEmpty(NamePathDataReferencedOld))
								{
									NamePathDataReferenced = String.Copy(NamePathDataReferencedOld);
									ObjectDataAnimationReferenced = AssetDatabase.LoadAssetAtPath(NamePathDataReferenced, typeof(ScriptableObject)) as ScriptableObject;
								}
							}
						}
					}
				}
				if(null == ObjectDataAnimationReferenced)
				{	/* Data Not-Found (Pass-2) : Create New-Data */
					/* Create Destination Folder */
					AssetUtility.Create.Folder(NamePathSubImportAnimation, NamePath);

					/* Create New Asset */
					DataAnimationReferenced = ScriptableObject.CreateInstance<Script_SpriteStudio_AnimationReferenced>();
					AssetDatabase.CreateAsset(DataAnimationReferenced, NamePathDataReferenced);
					ObjectDataAnimationReferenced = AssetDatabase.LoadAssetAtPath(NamePathDataReferenced, typeof(ScriptableObject)) as ScriptableObject;
				}
				DataAnimationReferenced = ObjectDataAnimationReferenced as Script_SpriteStudio_AnimationReferenced;
				return(true);
			}
			private bool NodeSetAnimation(GameObject GameObjectParts, int IndexParts, Script_SpriteStudio_PartsRoot ScriptRoot, Material[] TableMaterial)
			{
				DataIntermediate.KindAttributeKey[] MaskKeyAttribute = null;
				bool FlagRoot = false;

				Script_SpriteStudio_Triangle2 ComponentScript_Triangle2 = GameObjectParts.GetComponent<Script_SpriteStudio_Triangle2>();
				if(null != ComponentScript_Triangle2)
				{	/* Normal (Triangle-2) */
					ComponentScript_Triangle2.BootUpForce();

					ListDataRuntime[IndexParts].DataMain.ID = ComponentScript_Triangle2.ID = ListParts[IndexParts].ID;
					ListDataRuntime[IndexParts].DataMain.KindBlendTarget = ListParts[IndexParts].KindBlendTarget;
					ListDataRuntime[IndexParts].SpriteKind = Library_SpriteStudio.KindSprite.TRIANGLE2;

					ComponentScript_Triangle2.SpriteStudioData.KindBlendTarget = ListParts[IndexParts].KindBlendTarget;
					ComponentScript_Triangle2.ScriptRoot = ScriptRoot;
					ComponentScript_Triangle2.FlagHideForce = false;

					MaskKeyAttribute = DataIntermediate.MaskKeyAttribute_OPSS[(int)Library_SpriteStudio.KindParts.NORMAL];
					FlagRoot = false;
				}
				else
				{
					Script_SpriteStudio_Triangle4 ComponentScript_Triangle4 = GameObjectParts.GetComponent<Script_SpriteStudio_Triangle4>();
					if(null != ComponentScript_Triangle4)
					{	/* Normal (Triangle-4) */
						ComponentScript_Triangle4.BootUpForce();

						ListDataRuntime[IndexParts].DataMain.ID = ComponentScript_Triangle4.ID = ListParts[IndexParts].ID;
						ListDataRuntime[IndexParts].DataMain.KindBlendTarget = ListParts[IndexParts].KindBlendTarget;
						ListDataRuntime[IndexParts].SpriteKind = Library_SpriteStudio.KindSprite.TRIANGLE4;

						ComponentScript_Triangle4.SpriteStudioData.KindBlendTarget = ListParts[IndexParts].KindBlendTarget;
						ComponentScript_Triangle4.ScriptRoot = ScriptRoot;
						ComponentScript_Triangle4.FlagHideForce = false;

						MaskKeyAttribute = DataIntermediate.MaskKeyAttribute_OPSS[(int)Library_SpriteStudio.KindParts.NORMAL];
						FlagRoot = false;
					}
					else
					{
						Script_SpriteStudio_PartsNULL ComponentScript_PartsNULL = GameObjectParts.GetComponent<Script_SpriteStudio_PartsNULL>();
						if(null != ComponentScript_PartsNULL)
						{	/* NULL */
							ComponentScript_PartsNULL.BootUpForce();

							ListDataRuntime[IndexParts].DataMain.ID = ComponentScript_PartsNULL.ID = ListParts[IndexParts].ID;
							ListDataRuntime[IndexParts].DataMain.KindBlendTarget = Library_SpriteStudio.KindColorOperation.NON;
							ListDataRuntime[IndexParts].SpriteKind = Library_SpriteStudio.KindSprite.NON;

							ComponentScript_PartsNULL.ScriptRoot = ScriptRoot;

							MaskKeyAttribute = DataIntermediate.MaskKeyAttribute_OPSS[(int)Library_SpriteStudio.KindParts.NULL];
							FlagRoot = false;
						}
						else
						{
							Script_SpriteStudio_PartsInstance ComponentScript_PartsInstance = GameObjectParts.GetComponent<Script_SpriteStudio_PartsInstance>();
							if(null != ComponentScript_PartsInstance)
							{	/* Instance */
								ComponentScript_PartsInstance.BootUpForce();

								ListDataRuntime[IndexParts].DataMain.ID = ComponentScript_PartsInstance.ID = ListParts[IndexParts].ID;
								ListDataRuntime[IndexParts].DataMain.KindBlendTarget = Library_SpriteStudio.KindColorOperation.NON;
								ListDataRuntime[IndexParts].SpriteKind = Library_SpriteStudio.KindSprite.NON;

								ComponentScript_PartsInstance.ScriptRoot = ScriptRoot;
								ComponentScript_PartsInstance.FlagHideForce = false;

								MaskKeyAttribute = DataIntermediate.MaskKeyAttribute_OPSS[(int)Library_SpriteStudio.KindParts.INSTANCE];
								FlagRoot = false;

								/* Add Informarion to Array */
								LibraryEditor_SpriteStudio.Menu.InformationInstance Information = new LibraryEditor_SpriteStudio.Menu.InformationInstance();
								Information.ID = ListParts[IndexParts].ID;
								Information.NameInstanceSSAE = String.Copy(ListParts[IndexParts].InstanceNameSSAE);
								Information.NameInstanceAnimation = String.Copy(ListParts[IndexParts].InstanceNameAnimation);
								ListPartsInstance.Add(Information);
							}
							else
							{
								Script_SpriteStudio_PartsRoot ComponentScript_PartsRoot = GameObjectParts.GetComponent<Script_SpriteStudio_PartsRoot>();
								if(null != ComponentScript_PartsRoot)
								{	/* Root */
									ComponentScript_PartsRoot.BootUpForce();

									ListDataRuntime[IndexParts].DataMain.ID = ComponentScript_PartsRoot.ID = ListParts[IndexParts].ID;
									ListDataRuntime[IndexParts].DataMain.KindBlendTarget = Library_SpriteStudio.KindColorOperation.NON;
									ListDataRuntime[IndexParts].SpriteKind = Library_SpriteStudio.KindSprite.NON;

									MaskKeyAttribute = DataIntermediate.MaskKeyAttribute_OPSS[(int)Library_SpriteStudio.KindParts.ROOT];
									FlagRoot = true;
								}
								else
								{	/* Node is Invalid */
									Debug.LogError("SSAE-Create-Animation-Data Error: Animation[" + ListParts[IndexParts].Name + "] Invalid Node-Kind.");
									return(false);
								}
							}
						}
					}
				}

				bool FlagInstanceParts = (Library_SpriteStudio.KindParts.INSTANCE == ListParts[IndexParts].PartsKind) ? true : false;
				if(false == AnimationDataConvertRuntime(ListDataRuntime[IndexParts], ListParts[IndexParts].DataAnimation, MaskKeyAttribute, FlagRoot, TableMaterial, FlagInstanceParts))
				{	/* No-Data */
					GameObjectParts.transform.localPosition = Vector3.zero;
					GameObjectParts.transform.localEulerAngles = Vector3.zero;
					GameObjectParts.transform.localScale = Vector3.one;
				}

				return(true);
			}
			private bool AnimationDataConvertRuntime(	AnimationDataRuntime DataRuntime,
														DataIntermediate.AnimationDataEditor DataEditor,
														DataIntermediate.KindAttributeKey[] ListMaskAttribute,
														bool FlagRoot,
														Material[] TableMaterial,
														bool FlagInstanceParts
													)
			{
				if(null == DataEditor.DataKeyFrame)
				{
					return(false);
				}

				/* Delete Invalid-Attributes */
				int AttributeNo = -1;
				for(int i=0; i<ListMaskAttribute.Length; i++)
				{
					AttributeNo = (int)ListMaskAttribute[i];
					if((int)DataIntermediate.KindAttributeKey.TERMINATOR == AttributeNo)
					{
						break;
					}

					if(null != DataEditor.DataKeyFrame[AttributeNo])
					{
						DataEditor.DataKeyFrame[AttributeNo].Clear();
						DataEditor.DataKeyFrame[AttributeNo] = null;
					}
					if((int)DataIntermediate.KindAttributeKey.TERMINATOR_INHERIT > AttributeNo)
					{
						DataEditor.FlagKeyParameter = DataEditor.FlagKeyParameter & ~DataIntermediate.FlagParameterKeyFrameInherit[AttributeNo];
					}
				}

				DataRuntime.DataMain.AnimationDataFlags = AnimationDataConvertRuntimeBools(	DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.SHOW_HIDE],
																							DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.FLIP_X],
																							DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.FLIP_Y],
																							DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.TEXTURE_FLIP_X],
																							DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.TEXTURE_FLIP_Y]
																						);

				DataRuntime.DataMain.AnimationDataPosition = AnimationDataConvertRuntimeVector3(	DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.POSITION_X],
																									DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.POSITION_Y],
																									DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.POSITION_Z],
																									Vector3.zero
																							);
				DataRuntime.DataMain.AnimationDataRotation = AnimationDataConvertRuntimeVector3(	DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.ROTATE_X],
																									DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.ROTATE_Y],
																									DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.ROTATE_Z],
																									Vector3.zero
																								);
				DataRuntime.DataMain.AnimationDataScaling = AnimationDataConvertRuntimeVector2(	DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.SCALE_X],
																								DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.SCALE_Y],
																								Vector2.one
																							);

				DataRuntime.DataMain.AnimationDataOpacityRate = AnimationDataConvertRuntimeFloat(DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.OPACITY_RATE], 1.0f);
				DataRuntime.DataMain.AnimationDataPriority = AnimationDataConvertRuntimeFloat(DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.PRIORITY], 0.0f);

				DataRuntime.DataPlain.AnimationDataColorBlend = AnimationDataConvertRuntimeColorBlend(DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.COLOR_BLEND]);
				DataRuntime.DataPlain.AnimationDataVertexCorrection = AnimationDataConvertRuntimeQuadrilateral(DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.VERTEX_CORRECTION]);

				DataRuntime.DataPlain.AnimationDataOriginOffset = AnimationDataConvertRuntimeVector2(	DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.ORIGIN_OFFSET_X],
																										DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.ORIGIN_OFFSET_Y],
																										Vector2.zero
																									);

				DataRuntime.DataPlain.AnimationDataAnchorPosition = AnimationDataConvertRuntimeVector2(	DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.ANCHOR_POSITION_X],
																										DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.ANCHOR_POSITION_Y],
																										Vector2.zero
																									);
				DataRuntime.DataPlain.AnimationDataAnchorSize =  AnimationDataConvertRuntimeVector2(	DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.ANCHOR_SIZE_X],
																										DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.ANCHOR_SIZE_Y],
																										new Vector2(-1.0f, -1.0f)
																									);

				DataRuntime.DataPlain.AnimationDataTextureTranslate = AnimationDataConvertRuntimeVector2(	DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.TEXTURE_TRANSLATE_X],
																											DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.TEXTURE_TRANSLATE_Y],
																											Vector2.zero
																										);
				DataRuntime.DataPlain.AnimationDataTextureRotate = AnimationDataConvertRuntimeFloat(	DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.TEXTURE_ROTATE],
																										0.0f
																									);
				DataRuntime.DataPlain.AnimationDataTextureScale = AnimationDataConvertRuntimeVector2(	DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.TEXTURE_SCALE_X],
																										DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.TEXTURE_SCALE_Y],
																										Vector2.one
																									);
				DataRuntime.DataPlain.AnimationDataTextureExpand = AnimationDataConvertRuntimeVector2(	DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.TEXTURE_EXPAND_WIDTH],
																										DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.TEXTURE_EXPAND_HEIGHT],
																										Vector2.zero
																									);

				DataRuntime.DataPlain.AnimationDataCollisionRadius = AnimationDataConvertRuntimeFloat(	DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.COLLISION_RADIUS],
																										0.0f
																									);
				DataRuntime.DataPlain.AnimationDataCell = AnimationDataConvertRuntimeCell(ref DataRuntime.DataPlain.ArrayDataBodyCell, DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.CELL], TableMaterial);
				DataRuntime.DataMain.AnimationDataUser = AnimationDataConvertRuntimeUserData(ref DataRuntime.DataMain.ArrayDataBodyUser, DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.USER_DATA]);
				if(true == FlagInstanceParts)
				{
					DataRuntime.DataMain.AnimationDataInstance = AnimationDataConvertRuntimeInstance(ref DataRuntime.DataMain.ArrayDataBodyInstance, DataEditor.DataKeyFrame[(int)DataIntermediate.KindAttributeKey.INSTANCE]);
				}
				else
				{
					DataRuntime.DataMain.AnimationDataInstance = null;
				}

				return(true);
			}
			private Vector3[] AnimationDataConvertRuntimeVector3(ArrayList DataOriginalArrayX, ArrayList DataOriginalArrayY, ArrayList DataOriginalArrayZ, Vector3 ValueInitial)
			{
				if((null == DataOriginalArrayX) && (null == DataOriginalArrayY) && (null == DataOriginalArrayZ))
				{	/* All Attributes don't exist */
					return(null);
				}

				/* Create & Initialize All Frames */
				Vector3[] DataOutput = new Vector3[CountFrameFull];
				for(int i=0; i<CountFrameFull; i++)
				{
					DataOutput[i] = ValueInitial;
				}

				/* Set Frames */
				DataIntermediate.KeyFrame.DataFloat DataStart;
				DataIntermediate.KeyFrame.DataFloat DataEnd;
				int CountKeyFrame;
				int IndexAnimationStart;
				int IndexAnimationEnd;
				bool FirstKeyFrameAnimation;
				
				if(null != DataOriginalArrayX)
				{
					CountKeyFrame = DataOriginalArrayX.Count;
					FirstKeyFrameAnimation = true;
					for(int i=0; i<CountKeyFrame; i++)
					{
						/* Get Key-Frames */
						DataStart = (DataIntermediate.KeyFrame.DataFloat)DataOriginalArrayX[i];
						IndexAnimationStart = IndexAnimationGetFrameNo(DataStart.Time);
				
						if((CountKeyFrame - 1) == i)
						{
							DataEnd = DataStart;	/* Dummy */
							IndexAnimationEnd = -1;
						}
						else
						{
							DataEnd = (DataIntermediate.KeyFrame.DataFloat)DataOriginalArrayX[i + 1];
							IndexAnimationEnd = IndexAnimationGetFrameNo(DataEnd.Time);
						}

						/* Calculate Frames' Value  */
						if(IndexAnimationStart != IndexAnimationEnd)
						{	/* Differnt Animation */
							if((true == FirstKeyFrameAnimation) && (ListInformationPlay[IndexAnimationStart].FrameStart < DataStart.Time))
							{	/* Padding to Start-Frames  */
								for(int j=ListInformationPlay[IndexAnimationStart].FrameStart; j<=DataStart.Time; j++)
								{
									DataOutput[j].x = DataStart.Value;
								}
							}

							if(ListInformationPlay[IndexAnimationStart].FrameEnd > DataStart.Time)
							{	/* Padding to End-Frames  */
								for(int j=DataStart.Time; j<=ListInformationPlay[IndexAnimationStart].FrameEnd; j++)
								{
									DataOutput[j].x = DataStart.Value;
								}
							}

							FirstKeyFrameAnimation = true;
						}
						else
						{	/* Same Animation */
							if (IndexAnimationStart < 0 || IndexAnimationStart >= ListInformationPlay.Length)
							{
								Debug.LogError("Vector3.X keyframe is out of range!! Parts:" + CurrentProcessingPartsName + " frame:" + DataStart.Time);
								return(null);
							}
							if((true == FirstKeyFrameAnimation) && (ListInformationPlay[IndexAnimationStart].FrameStart < DataStart.Time))
							{	/* Padding to Start-Frames  */
								for(int j=ListInformationPlay[IndexAnimationStart].FrameStart; j<=DataStart.Time; j++)
								{
									DataOutput[j].x = DataStart.Value;
								}
							}

							/* Value Interpolation */
							for(int j=DataStart.Time; j<=DataEnd.Time; j++)
							{
								DataOutput[j].x = Interpolation.Interpolate<float>(DataStart.Curve, j, DataStart.Value, DataEnd.Value, DataStart.Time, DataEnd.Time);
							}

							FirstKeyFrameAnimation = false;
						}
					}
				}

				if(null != DataOriginalArrayY)
				{
					CountKeyFrame = DataOriginalArrayY.Count;
					FirstKeyFrameAnimation = true;
					for(int i=0; i<CountKeyFrame; i++)
					{
						/* Get Key-Frames */
						DataStart = (DataIntermediate.KeyFrame.DataFloat)DataOriginalArrayY[i];
						IndexAnimationStart = IndexAnimationGetFrameNo(DataStart.Time);
				
						if((CountKeyFrame - 1) == i)
						{
							DataEnd = DataStart;	/* Dummy */
							IndexAnimationEnd = -1;
						}
						else
						{
							DataEnd = (DataIntermediate.KeyFrame.DataFloat)DataOriginalArrayY[i + 1];
							IndexAnimationEnd = IndexAnimationGetFrameNo(DataEnd.Time);
						}

						/* Calculate Frames' Value  */
						if(IndexAnimationStart != IndexAnimationEnd)
						{	/* Differnt Animation */
							if((true == FirstKeyFrameAnimation) && (ListInformationPlay[IndexAnimationStart].FrameStart < DataStart.Time))
							{	/* Padding to Start-Frames  */
								for(int j=ListInformationPlay[IndexAnimationStart].FrameStart; j<=DataStart.Time; j++)
								{
									DataOutput[j].y = DataStart.Value;
								}
							}

							if(ListInformationPlay[IndexAnimationStart].FrameEnd > DataStart.Time)
							{	/* Padding to End-Frames  */
								for(int j=DataStart.Time; j<=ListInformationPlay[IndexAnimationStart].FrameEnd; j++)
								{
									DataOutput[j].y = DataStart.Value;
								}
							}

							FirstKeyFrameAnimation = true;
						}
						else
						{	/* Same Animation */
							if (IndexAnimationStart < 0 || IndexAnimationStart >= ListInformationPlay.Length)
							{
								Debug.LogError("Vector3.Y keyframe is out of range!! Parts:" + CurrentProcessingPartsName + " frame:" + DataStart.Time);
								return(null);
							}
							if((true == FirstKeyFrameAnimation) && (ListInformationPlay[IndexAnimationStart].FrameStart < DataStart.Time))
							{	/* Padding to Start-Frames  */
								for(int j=ListInformationPlay[IndexAnimationStart].FrameStart; j<=DataStart.Time; j++)
								{
									DataOutput[j].y = DataStart.Value;
								}
							}

							/* Value Interpolation */
							for(int j=DataStart.Time; j<=DataEnd.Time; j++)
							{
								DataOutput[j].y = Interpolation.Interpolate<float>(DataStart.Curve, j, DataStart.Value, DataEnd.Value, DataStart.Time, DataEnd.Time);
							}

							FirstKeyFrameAnimation = false;
						}
					}
				}

				if(null != DataOriginalArrayZ)
				{
					CountKeyFrame = DataOriginalArrayZ.Count;
					FirstKeyFrameAnimation = true;
					for(int i=0; i<CountKeyFrame; i++)
					{
						/* Get Key-Frames */
						DataStart = (DataIntermediate.KeyFrame.DataFloat)DataOriginalArrayZ[i];
						IndexAnimationStart = IndexAnimationGetFrameNo(DataStart.Time);
				
						if((CountKeyFrame - 1) == i)
						{
							DataEnd = DataStart;	/* Dummy */
							IndexAnimationEnd = -1;
						}
						else
						{
							DataEnd = (DataIntermediate.KeyFrame.DataFloat)DataOriginalArrayZ[i + 1];
							IndexAnimationEnd = IndexAnimationGetFrameNo(DataEnd.Time);
						}

						/* Calculate Frames' Value  */
						if(IndexAnimationStart != IndexAnimationEnd)
						{	/* Differnt Animation */
							if((true == FirstKeyFrameAnimation) && (ListInformationPlay[IndexAnimationStart].FrameStart < DataStart.Time))
							{	/* Padding to Start-Frames  */
								for(int j=ListInformationPlay[IndexAnimationStart].FrameStart; j<=DataStart.Time; j++)
								{
									DataOutput[j].z = DataStart.Value;
								}
							}

							if(ListInformationPlay[IndexAnimationStart].FrameEnd > DataStart.Time)
							{	/* Padding to End-Frames  */
								for(int j=DataStart.Time; j<=ListInformationPlay[IndexAnimationStart].FrameEnd; j++)
								{
									DataOutput[j].z = DataStart.Value;
								}
							}

							FirstKeyFrameAnimation = true;
						}
						else
						{	/* Same Animation */
							if (IndexAnimationStart < 0 || IndexAnimationStart >= ListInformationPlay.Length)
							{
								Debug.LogError("Vector3.Z keyframe is out of range!! Parts:" + CurrentProcessingPartsName + " frame:" + DataStart.Time);
								return(null);
							}
							if((true == FirstKeyFrameAnimation) && (ListInformationPlay[IndexAnimationStart].FrameStart < DataStart.Time))
							{	/* Padding to Start-Frames  */
								for(int j=ListInformationPlay[IndexAnimationStart].FrameStart; j<=DataStart.Time; j++)
								{
									DataOutput[j].z = DataStart.Value;
								}
							}

							/* Value Interpolation */
							for(int j=DataStart.Time; j<=DataEnd.Time; j++)
							{
								DataOutput[j].z = Interpolation.Interpolate<float>(DataStart.Curve, j, DataStart.Value, DataEnd.Value, DataStart.Time, DataEnd.Time);
							}

							FirstKeyFrameAnimation = false;
						}
					}
				}

				return(DataOutput);
			}
			private Vector2[] AnimationDataConvertRuntimeVector2(ArrayList DataOriginalArrayX, ArrayList DataOriginalArrayY, Vector2 ValueInitial)
			{
				if((null == DataOriginalArrayX) && (null == DataOriginalArrayY))
				{	/* All Attributes don't exist */
					return(null);
				}

				/* Create & Initialize All Frames */
				Vector2[] DataOutput = new Vector2[CountFrameFull];
				for(int i=0; i<CountFrameFull; i++)
				{
					DataOutput[i] = ValueInitial;
				}

				/* Set Frames */
				DataIntermediate.KeyFrame.DataFloat DataStart;
				DataIntermediate.KeyFrame.DataFloat DataEnd;
				int CountKeyFrame;
				int IndexAnimationStart;
				int IndexAnimationEnd;
				bool FirstKeyFrameAnimation;
				
				if(null != DataOriginalArrayX)
				{
					CountKeyFrame = DataOriginalArrayX.Count;
					FirstKeyFrameAnimation = true;
					for(int i=0; i<CountKeyFrame; i++)
					{
						/* Get Key-Frames */
						DataStart = (DataIntermediate.KeyFrame.DataFloat)DataOriginalArrayX[i];
						IndexAnimationStart = IndexAnimationGetFrameNo(DataStart.Time);
				
						if((CountKeyFrame - 1) == i)
						{
							DataEnd = DataStart;	/* Dummy */
							IndexAnimationEnd = -1;
						}
						else
						{
							DataEnd = (DataIntermediate.KeyFrame.DataFloat)DataOriginalArrayX[i + 1];
							IndexAnimationEnd = IndexAnimationGetFrameNo(DataEnd.Time);
						}

						/* Calculate Frames' Value  */
						if(IndexAnimationStart != IndexAnimationEnd)
						{	/* Differnt Animation */
							if((true == FirstKeyFrameAnimation) && (ListInformationPlay[IndexAnimationStart].FrameStart < DataStart.Time))
							{	/* Padding to Start-Frames  */
								for(int j=ListInformationPlay[IndexAnimationStart].FrameStart; j<=DataStart.Time; j++)
								{
									DataOutput[j].x = DataStart.Value;
								}
							}

							if(ListInformationPlay[IndexAnimationStart].FrameEnd > DataStart.Time)
							{	/* Padding to End-Frames  */
								for(int j=DataStart.Time; j<=ListInformationPlay[IndexAnimationStart].FrameEnd; j++)
								{
									DataOutput[j].x = DataStart.Value;
								}
							}

							FirstKeyFrameAnimation = true;
						}
						else
						{	/* Same Animation */
							if (IndexAnimationStart < 0 || IndexAnimationStart >= ListInformationPlay.Length)
							{
#if false
								/* MEMO: Regular Procedure */
								Debug.LogError("Vector2.X keyframe is out of range!! Parts:" + CurrentProcessingPartsName + " frame:" + DataStart.Time);
#else
								/* MEMO: Interim-Patch for "Effect"-Parts */
#endif
								return(null);
							}
							if((true == FirstKeyFrameAnimation) && (ListInformationPlay[IndexAnimationStart].FrameStart < DataStart.Time))
							{	/* Padding to Start-Frames  */
								for(int j=ListInformationPlay[IndexAnimationStart].FrameStart; j<=DataStart.Time; j++)
								{
									DataOutput[j].x = DataStart.Value;
								}
							}

							/* Value Interpolation */
							for(int j=DataStart.Time; j<=DataEnd.Time; j++)
							{
								DataOutput[j].x = Interpolation.Interpolate<float>(DataStart.Curve, j, DataStart.Value, DataEnd.Value, DataStart.Time, DataEnd.Time);
							}

							FirstKeyFrameAnimation = false;
						}
					}
				}

				if(null != DataOriginalArrayY)
				{
					CountKeyFrame = DataOriginalArrayY.Count;
					FirstKeyFrameAnimation = true;
					for(int i=0; i<CountKeyFrame; i++)
					{
						/* Get Key-Frames */
						DataStart = (DataIntermediate.KeyFrame.DataFloat)DataOriginalArrayY[i];
						IndexAnimationStart = IndexAnimationGetFrameNo(DataStart.Time);
				
						if((CountKeyFrame - 1) == i)
						{
							DataEnd = DataStart;	/* Dummy */
							IndexAnimationEnd = -1;
						}
						else
						{
							DataEnd = (DataIntermediate.KeyFrame.DataFloat)DataOriginalArrayY[i + 1];
							IndexAnimationEnd = IndexAnimationGetFrameNo(DataEnd.Time);
						}

						/* Calculate Frames' Value  */
						if(IndexAnimationStart != IndexAnimationEnd)
						{	/* Differnt Animation */
							if((true == FirstKeyFrameAnimation) && (ListInformationPlay[IndexAnimationStart].FrameStart < DataStart.Time))
							{	/* Padding to Start-Frames  */
								for(int j=ListInformationPlay[IndexAnimationStart].FrameStart; j<=DataStart.Time; j++)
								{
									DataOutput[j].y = DataStart.Value;
								}
							}

							if(ListInformationPlay[IndexAnimationStart].FrameEnd > DataStart.Time)
							{	/* Padding to End-Frames  */
								for(int j=DataStart.Time; j<=ListInformationPlay[IndexAnimationStart].FrameEnd; j++)
								{
									DataOutput[j].y = DataStart.Value;
								}
							}

							FirstKeyFrameAnimation = true;
						}
						else
						{	/* Same Animation */
							if (IndexAnimationStart < 0 || IndexAnimationStart >= ListInformationPlay.Length)
							{
								Debug.LogError("Vector3.Y keyframe is out of range!! Parts:" + CurrentProcessingPartsName + " frame:" + DataStart.Time);
								return(null);
							}
							if((true == FirstKeyFrameAnimation) && (ListInformationPlay[IndexAnimationStart].FrameStart < DataStart.Time))
							{	/* Padding to Start-Frames  */
								for(int j=ListInformationPlay[IndexAnimationStart].FrameStart; j<=DataStart.Time; j++)
								{
									DataOutput[j].y = DataStart.Value;
								}
							}

							/* Value Interpolation */
							for(int j=DataStart.Time; j<=DataEnd.Time; j++)
							{
								DataOutput[j].y = Interpolation.Interpolate<float>(DataStart.Curve, j, DataStart.Value, DataEnd.Value, DataStart.Time, DataEnd.Time);
							}

							FirstKeyFrameAnimation = false;
						}
					}
				}

				return(DataOutput);
			}
			private float[] AnimationDataConvertRuntimeFloat(ArrayList DataOriginalArray, float ValueInitial)
			{
				if(null == DataOriginalArray)
				{	/* Attribute doesn't exist */
					return(null);
				}

				/* Create & Initialize All Frames */
				float[] DataOutput = new float[CountFrameFull];
				for(int i=0; i<CountFrameFull; i++)
				{
					DataOutput[i] = ValueInitial;
				}

				/* Set Frames */
				DataIntermediate.KeyFrame.DataFloat DataStart;
				DataIntermediate.KeyFrame.DataFloat DataEnd;
				int CountKeyFrame;
				int IndexAnimationStart;
				int IndexAnimationEnd;
				bool FirstKeyFrameAnimation;
				
				if(null != DataOriginalArray)
				{
					CountKeyFrame = DataOriginalArray.Count;
					FirstKeyFrameAnimation = true;
					for(int i=0; i<CountKeyFrame; i++)
					{
						/* Get Key-Frames */
						DataStart = (DataIntermediate.KeyFrame.DataFloat)DataOriginalArray[i];
						IndexAnimationStart = IndexAnimationGetFrameNo(DataStart.Time);
				
						if((CountKeyFrame - 1) == i)
						{
							DataEnd = DataStart;	/* Dummy */
							IndexAnimationEnd = -1;
						}
						else
						{
							DataEnd = (DataIntermediate.KeyFrame.DataFloat)DataOriginalArray[i + 1];
							IndexAnimationEnd = IndexAnimationGetFrameNo(DataEnd.Time);
						}

						/* Calculate Frames' Value  */
						if(IndexAnimationStart != IndexAnimationEnd)
						{	/* Differnt Animation */
							if((true == FirstKeyFrameAnimation) && (ListInformationPlay[IndexAnimationStart].FrameStart < DataStart.Time))
							{	/* Padding to Start-Frames  */
								for(int j=ListInformationPlay[IndexAnimationStart].FrameStart; j<=DataStart.Time; j++)
								{
									DataOutput[j] = DataStart.Value;
								}
							}

							if(ListInformationPlay[IndexAnimationStart].FrameEnd > DataStart.Time)
							{	/* Padding to End-Frames  */
								for(int j=DataStart.Time; j<=ListInformationPlay[IndexAnimationStart].FrameEnd; j++)
								{
									DataOutput[j] = DataStart.Value;
								}
							}

							FirstKeyFrameAnimation = true;
						}
						else
						{	/* Same Animation */
							if (IndexAnimationStart < 0 || IndexAnimationStart >= ListInformationPlay.Length)
							{
								Debug.LogError("Float keyframe is out of range!! Parts:" + CurrentProcessingPartsName + " frame:" + DataStart.Time);
								return(null);
							}
							if((true == FirstKeyFrameAnimation) && (ListInformationPlay[IndexAnimationStart].FrameStart < DataStart.Time))
							{	/* Padding to Start-Frames  */
								for(int j=ListInformationPlay[IndexAnimationStart].FrameStart; j<=DataStart.Time; j++)
								{
									DataOutput[j] = DataStart.Value;
								}
							}

							/* Value Interpolation */
							for(int j=DataStart.Time; j<=DataEnd.Time; j++)
							{
								DataOutput[j] = Interpolation.Interpolate<float>(DataStart.Curve, j, DataStart.Value, DataEnd.Value, DataStart.Time, DataEnd.Time);
							}

							FirstKeyFrameAnimation = false;
						}
					}
				}

				return(DataOutput);
			}
			private Library_SpriteStudio.KeyFrame.ValueColor[] AnimationDataConvertRuntimeColorBlend(ArrayList DataOriginalArray)
			{
				if(null == DataOriginalArray)
				{	/* Attribute doesn't exist */
					return(null);
				}

				/* Create & Initialize All Frames */
				Library_SpriteStudio.KeyFrame.ValueColor[] DataOutput = new Library_SpriteStudio.KeyFrame.ValueColor[CountFrameFull];
				for(int i=0; i<CountFrameFull; i++)
				{
					DataOutput[i] = new Library_SpriteStudio.KeyFrame.ValueColor();
				}

				/* Set Frames */
				DataIntermediate.KeyFrame.DataColor DataStart;
				DataIntermediate.KeyFrame.DataColor DataEnd;
				DataIntermediate.KeyFrame.ValueColor DataInterpolation;
				int CountKeyFrame;
				int IndexAnimationStart;
				int IndexAnimationEnd;
				bool FirstKeyFrameAnimation;
				
				if(null != DataOriginalArray)
				{
					CountKeyFrame = DataOriginalArray.Count;
					FirstKeyFrameAnimation = true;
					for(int i=0; i<CountKeyFrame; i++)
					{
						/* Get Key-Frames */
						DataStart = (DataIntermediate.KeyFrame.DataColor)DataOriginalArray[i];
						IndexAnimationStart = IndexAnimationGetFrameNo(DataStart.Time);
				
						if((CountKeyFrame - 1) == i)
						{
							DataEnd = DataStart;	/* Dummy */
							IndexAnimationEnd = -1;
						}
						else
						{
							DataEnd = (DataIntermediate.KeyFrame.DataColor)DataOriginalArray[i + 1];
							IndexAnimationEnd = IndexAnimationGetFrameNo(DataEnd.Time);
						}

						/* Calculate Frames' Value  */
						if(IndexAnimationStart != IndexAnimationEnd)
						{	/* Differnt Animation */
							if((true == FirstKeyFrameAnimation) && (ListInformationPlay[IndexAnimationStart].FrameStart < DataStart.Time))
							{	/* Padding to Start-Frames  */
								for(int j=ListInformationPlay[IndexAnimationStart].FrameStart; j<=DataStart.Time; j++)
								{
									AnimationDataCopyColorBlend(DataOutput[j], DataStart.Value);
								}
							}

							if(ListInformationPlay[IndexAnimationStart].FrameEnd > DataStart.Time)
							{	/* Padding to End-Frames  */
								for(int j=DataStart.Time; j<=ListInformationPlay[IndexAnimationStart].FrameEnd; j++)
								{
									AnimationDataCopyColorBlend(DataOutput[j], DataStart.Value);
								}
							}

							FirstKeyFrameAnimation = true;
						}
						else
						{	/* Same Animation */
							if (IndexAnimationStart < 0 || IndexAnimationStart >= ListInformationPlay.Length)
							{
								Debug.LogError("Color keyframe is out of range!! Parts:" + CurrentProcessingPartsName + " frame:" + DataStart.Time);
								return(null);
							}
							if((true == FirstKeyFrameAnimation) && (ListInformationPlay[IndexAnimationStart].FrameStart < DataStart.Time))
							{	/* Padding to Start-Frames  */
								for(int j=ListInformationPlay[IndexAnimationStart].FrameStart; j<=DataStart.Time; j++)
								{
									AnimationDataCopyColorBlend(DataOutput[j], DataStart.Value);
								}
							}

							/* Value Interpolation */
							for(int j=DataStart.Time; j<=DataEnd.Time; j++)
							{
								DataInterpolation = (DataIntermediate.KeyFrame.ValueColor)DataStart.Value.GetInterpolated(DataStart.Curve, j, DataStart.Value, DataEnd.Value, DataStart.Time, DataEnd.Time);
								AnimationDataCopyColorBlend(DataOutput[j], DataInterpolation);
							}

							FirstKeyFrameAnimation = false;
						}
					}
				}

				return(DataOutput);
			}
			private void AnimationDataCopyColorBlend(Library_SpriteStudio.KeyFrame.ValueColor DataOutput, DataIntermediate.KeyFrame.ValueColor Data)
			{
				DataOutput.Bound = Data.Bound;
				DataOutput.Operation = Data.Operation;

				DataOutput.VertexColor[(int)Library_SpriteStudio.VertexNo.LU] = Data.VertexColor[0];
				DataOutput.VertexColor[(int)Library_SpriteStudio.VertexNo.RU] = Data.VertexColor[1];
				DataOutput.VertexColor[(int)Library_SpriteStudio.VertexNo.RD] = Data.VertexColor[2];
				DataOutput.VertexColor[(int)Library_SpriteStudio.VertexNo.LD] = Data.VertexColor[3];
				DataOutput.VertexColor[(int)Library_SpriteStudio.VertexNo.C] = (Data.VertexColor[0] + Data.VertexColor[1] + Data.VertexColor[2] + Data.VertexColor[3]) / 4.0f;

				DataOutput.RatePixelAlpha[(int)Library_SpriteStudio.VertexNo.LU] = Data.RatePixelAlpha[0];
				DataOutput.RatePixelAlpha[(int)Library_SpriteStudio.VertexNo.RU] = Data.RatePixelAlpha[1];
				DataOutput.RatePixelAlpha[(int)Library_SpriteStudio.VertexNo.RD] = Data.RatePixelAlpha[2];
				DataOutput.RatePixelAlpha[(int)Library_SpriteStudio.VertexNo.LD] = Data.RatePixelAlpha[3];
				DataOutput.RatePixelAlpha[(int)Library_SpriteStudio.VertexNo.C] = (Data.RatePixelAlpha[0] + Data.RatePixelAlpha[1] + Data.RatePixelAlpha[2] + Data.RatePixelAlpha[3]) / 4.0f;
			}
			private Library_SpriteStudio.KeyFrame.ValueQuadrilateral[] AnimationDataConvertRuntimeQuadrilateral(ArrayList DataOriginalArray)
			{
				if(null == DataOriginalArray)
				{	/* Attribute doesn't exist */
					return(null);
				}

				/* Create & Initialize All Frames */
				Library_SpriteStudio.KeyFrame.ValueQuadrilateral[] DataOutput = new Library_SpriteStudio.KeyFrame.ValueQuadrilateral[CountFrameFull];
				for(int i=0; i<CountFrameFull; i++)
				{
					DataOutput[i] = new Library_SpriteStudio.KeyFrame.ValueQuadrilateral();
				}

				/* Set Frames */
				DataIntermediate.KeyFrame.DataQuadrilateral DataStart;
				DataIntermediate.KeyFrame.DataQuadrilateral DataEnd;
				DataIntermediate.KeyFrame.ValueQuadrilateral DataInterpolation;
				int CountKeyFrame;
				int IndexAnimationStart;
				int IndexAnimationEnd;
				bool FirstKeyFrameAnimation;
				
				if(null != DataOriginalArray)
				{
					CountKeyFrame = DataOriginalArray.Count;
					FirstKeyFrameAnimation = true;
					for(int i=0; i<CountKeyFrame; i++)
					{
						/* Get Key-Frames */
						DataStart = (DataIntermediate.KeyFrame.DataQuadrilateral)DataOriginalArray[i];
						IndexAnimationStart = IndexAnimationGetFrameNo(DataStart.Time);
				
						if((CountKeyFrame - 1) == i)
						{
							DataEnd = DataStart;	/* Dummy */
							IndexAnimationEnd = -1;
						}
						else
						{
							DataEnd = (DataIntermediate.KeyFrame.DataQuadrilateral)DataOriginalArray[i + 1];
							IndexAnimationEnd = IndexAnimationGetFrameNo(DataEnd.Time);
						}

						/* Calculate Frames' Value  */
						if(IndexAnimationStart != IndexAnimationEnd)
						{	/* Differnt Animation */
							if((true == FirstKeyFrameAnimation) && (ListInformationPlay[IndexAnimationStart].FrameStart < DataStart.Time))
							{	/* Padding to Start-Frames  */
								for(int j=ListInformationPlay[IndexAnimationStart].FrameStart; j<=DataStart.Time; j++)
								{
									AnimationDataCopyQuadrilateral(DataOutput[j], DataStart.Value);
								}
							}

							if(ListInformationPlay[IndexAnimationStart].FrameEnd > DataStart.Time)
							{	/* Padding to End-Frames  */
								for(int j=DataStart.Time; j<=ListInformationPlay[IndexAnimationStart].FrameEnd; j++)
								{
									AnimationDataCopyQuadrilateral(DataOutput[j], DataStart.Value);
								}
							}

							FirstKeyFrameAnimation = true;
						}
						else
						{	/* Same Animation */
							if (IndexAnimationStart < 0 || IndexAnimationStart >= ListInformationPlay.Length)
							{
								Debug.LogError("Quadrilateral keyframe is out of range!! Parts:" + CurrentProcessingPartsName + " frame:" + DataStart.Time);
								return(null);
							}
							if((true == FirstKeyFrameAnimation) && (ListInformationPlay[IndexAnimationStart].FrameStart < DataStart.Time))
							{	/* Padding to Start-Frames  */
								for(int j=ListInformationPlay[IndexAnimationStart].FrameStart; j<=DataStart.Time; j++)
								{
									AnimationDataCopyQuadrilateral(DataOutput[j], DataStart.Value);
								}
							}

							/* Value Interpolation */
							for(int j=DataStart.Time; j<=DataEnd.Time; j++)
							{
								DataInterpolation = (DataIntermediate.KeyFrame.ValueQuadrilateral)DataStart.Value.GetInterpolated(DataStart.Curve, j, DataStart.Value, DataEnd.Value, DataStart.Time, DataEnd.Time);
								AnimationDataCopyQuadrilateral(DataOutput[j], DataInterpolation);
							}

							FirstKeyFrameAnimation = false;
						}
					}
				}

				return(DataOutput);
			}
			private void AnimationDataCopyQuadrilateral(Library_SpriteStudio.KeyFrame.ValueQuadrilateral DataOutput, DataIntermediate.KeyFrame.ValueQuadrilateral Data)
			{
				DataOutput.Coordinate[(int)Library_SpriteStudio.VertexNo.LU] = Data.Coordinate[0].Point;
				DataOutput.Coordinate[(int)Library_SpriteStudio.VertexNo.RU] = Data.Coordinate[1].Point;
				DataOutput.Coordinate[(int)Library_SpriteStudio.VertexNo.RD] = Data.Coordinate[2].Point;
				DataOutput.Coordinate[(int)Library_SpriteStudio.VertexNo.LD] = Data.Coordinate[3].Point;
			}
			private int[] AnimationDataConvertRuntimeCell(	ref Library_SpriteStudio.KeyFrame.ValueCell.Data[] ArrayDataBody,
															ArrayList DataOriginalArray,
															Material[] TableMaterial
														)
			{
				if(null == DataOriginalArray)
				{	/* Attribute doesn't exist */
					ArrayDataBody = null;
					return(null);
				}

				/* Create Body-Data Array */
				int Count = DataOriginalArray.Count;
				ArrayDataBody = new Library_SpriteStudio.KeyFrame.ValueCell.Data[Count];
					
				DataIntermediate.KeyFrame.DataCell DataOriginal;
				for(int i=0; i<Count; i++)
				{
					DataOriginal = (DataIntermediate.KeyFrame.DataCell)DataOriginalArray[i];
					ArrayDataBody[i] = new Library_SpriteStudio.KeyFrame.ValueCell.Data();
						
					/* Copy Data-Body */ 
					ArrayDataBody[i].FrameNoBase = DataOriginal.Time;
					int TextureNo = DataOriginal.Value.TextureNo;
					ArrayDataBody[i].TextureNo = TextureNo;
					ArrayDataBody[i].Rectangle.x = DataOriginal.Value.Rectangle.x;
					ArrayDataBody[i].Rectangle.y = DataOriginal.Value.Rectangle.y;
					ArrayDataBody[i].Rectangle.width = DataOriginal.Value.Rectangle.width;
					ArrayDataBody[i].Rectangle.height = DataOriginal.Value.Rectangle.height;
					ArrayDataBody[i].Pivot = DataOriginal.Value.Pivot;
					if(0 > TextureNo)
					{	/* Error */
						ArrayDataBody[i].SizeOriginal = Vector2.zero;
					}
					else
					{
						int MaterialNo = TextureNo * ((int)Library_SpriteStudio.KindColorOperation.TERMINATOR - 1);
						ArrayDataBody[i].SizeOriginal.x = TableMaterial[MaterialNo].mainTexture.width;
						ArrayDataBody[i].SizeOriginal.y = TableMaterial[MaterialNo].mainTexture.height;
					}
				}

				/* Create & Initialize All Frames */
				bool[] FlagSetData = new bool[CountFrameFull];
				for(int i=0; i<CountFrameFull; i++)
				{
					FlagSetData[i] = false;
				}
				int[] DataOutput = new int[CountFrameFull];
				for(int i=0; i<CountFrameFull; i++)
				{
					DataOutput[i] = -1;
				}

				/* Set Frames */
				int CountKeyFrame;
				int IndexKeyBase;
				int IndexKeyNow;
				int IndexAnimation;
				int IndexAnimationNow;

				if(null != DataOriginalArray)
				{
					/* Set Key-Frames */
					CountKeyFrame = DataOriginalArray.Count;
					for(int i=0; i<CountKeyFrame; i++)
					{
						/* Create Data-Body */
						DataOriginal = (DataIntermediate.KeyFrame.DataCell)DataOriginalArray[i];
						IndexKeyBase = DataOriginal.Time;
						DataOutput[IndexKeyBase] = i;
						FlagSetData[IndexKeyBase] = true;
					}

					/* Solving All-KeyFrame */
					CountKeyFrame = DataOutput.Length;
					IndexKeyBase = -1;
					for(int i=0; i<CountKeyFrame; i++)
					{
						IndexAnimation = IndexAnimationGetFrameNo(i);
						if(-1 == IndexAnimation)
						{	/* Animation Range Over */
							IndexKeyBase = -1;	/* Clear Base-Key */
							continue;
						}
						
						if(false == FlagSetData[i])
						{	/* Not-Solved Frame */
							if(-1 == IndexKeyBase)
							{	/* Before First Valid-KeyData */
								continue;
							}
							DataOutput[i] = DataOutput[IndexKeyBase];
							FlagSetData[i] = true;
						}
						else
						{	/* Valid Key-Data */
							/* Solving Previous Frames */
							IndexKeyBase = i;
							IndexKeyNow = i - 1;
							for( ; ; )
							{
								if(IndexKeyNow < 0)
								{	/* Data Range Over */
									break;
								}
								
								if(true == FlagSetData[IndexKeyNow])
								{	/* Already Solved */
									break;
								}

								IndexAnimationNow = IndexAnimationGetFrameNo(IndexKeyNow);
								if((-1 == IndexAnimationNow) || (IndexAnimation != IndexAnimationNow))
								{	/**/
									break;
								}
								DataOutput[IndexKeyNow] = DataOutput[IndexKeyBase];
								FlagSetData[IndexKeyNow] = true;

								IndexKeyNow--;
							}
						}
					}
				}
				return(DataOutput);
			}
			private int[] AnimationDataConvertRuntimeUserData(	ref Library_SpriteStudio.KeyFrame.ValueUser.Data[] ArrayDataBody,
																ArrayList DataOriginalArray
															)
			{
				if(null == DataOriginalArray)
				{	/* Attribute doesn't exist */
					ArrayDataBody = null;
					return(null);
				}

				/* Create Body-Data Array */
				int Count = DataOriginalArray.Count;
				ArrayDataBody = new Library_SpriteStudio.KeyFrame.ValueUser.Data[Count];

				DataIntermediate.KeyFrame.DataUser DataOriginal;
				for(int i=0; i<Count; i++)
				{
					DataOriginal = (DataIntermediate.KeyFrame.DataUser)DataOriginalArray[i];
					ArrayDataBody[i] = new Library_SpriteStudio.KeyFrame.ValueUser.Data();

					/* Copy Data-Body */ 
					ArrayDataBody[i].Flag = Library_SpriteStudio.KeyFrame.ValueUser.Data.FlagData.CLEAR;
					if(true == DataOriginal.Value.IsNumber)
					{
						ArrayDataBody[i].Flag |= Library_SpriteStudio.KeyFrame.ValueUser.Data.FlagData.NUMBER;
						ArrayDataBody[i].NumberInt = DataOriginal.Value.Number;
					}
					else
					{
						ArrayDataBody[i].Flag &= ~Library_SpriteStudio.KeyFrame.ValueUser.Data.FlagData.NUMBER;
						ArrayDataBody[i].NumberInt = 0;
					}

					if(true == DataOriginal.Value.IsRectangle)
					{
						ArrayDataBody[i].Flag |= Library_SpriteStudio.KeyFrame.ValueUser.Data.FlagData.RECTANGLE;
						ArrayDataBody[i].Rectangle = DataOriginal.Value.Rectangle;
					}
					else
					{
						ArrayDataBody[i].Flag &= ~Library_SpriteStudio.KeyFrame.ValueUser.Data.FlagData.RECTANGLE;
						ArrayDataBody[i].Rectangle.xMin = 0.0f;
						ArrayDataBody[i].Rectangle.yMin = 0.0f;
						ArrayDataBody[i].Rectangle.xMax = 0.0f;
						ArrayDataBody[i].Rectangle.yMax = 0.0f;
					}

					if(true == DataOriginal.Value.IsCoordinate)
					{
						ArrayDataBody[i].Flag |= Library_SpriteStudio.KeyFrame.ValueUser.Data.FlagData.COORDINATE;
						ArrayDataBody[i].Coordinate = DataOriginal.Value.Coordinate.Point;
					}
					else
					{
						ArrayDataBody[i].Flag &= ~Library_SpriteStudio.KeyFrame.ValueUser.Data.FlagData.COORDINATE;
						ArrayDataBody[i].Coordinate = Vector2.zero;
					}

					if(true == DataOriginal.Value.IsText)
					{
						ArrayDataBody[i].Flag |= Library_SpriteStudio.KeyFrame.ValueUser.Data.FlagData.TEXT;
						ArrayDataBody[i].Text = String.Copy(DataOriginal.Value.Text);
					}
					else
					{
						ArrayDataBody[i].Flag &= ~Library_SpriteStudio.KeyFrame.ValueUser.Data.FlagData.TEXT;
						ArrayDataBody[i].Text = "";
					}
				}

				/* Create & Initialize All Frames */
				int[] DataOutput = new int[CountFrameFull];
				for(int i=0; i<CountFrameFull; i++)
				{
					DataOutput[i] = -1;
				}

				/* Set Frames */
				int IndexKeyFrame;
				int CountKeyFrame;
				if(null != DataOriginalArray)
				{
					CountKeyFrame = DataOriginalArray.Count;
					for(int i=0; i<CountKeyFrame; i++)
					{
						/* Copy Key-Frames */
						DataOriginal = (DataIntermediate.KeyFrame.DataUser)DataOriginalArray[i];
						IndexKeyFrame = DataOriginal.Time;
						DataOutput[IndexKeyFrame] = i;
					}
				}

				return(DataOutput);
			}
			private int[] AnimationDataConvertRuntimeInstance(	ref Library_SpriteStudio.KeyFrame.ValueInstance.Data[] ArrayDataBody,
																ArrayList DataOriginalArray
															)
			{
				if(null == DataOriginalArray)
				{	/* Attribute doesn't exist */
					return(null);
				}
				else
				{	/* Make up a Shortfall */
					DataIntermediate.KeyFrame.DataInstance DataDummy = null;

					int IndexAnimationMax = 0;
					int IndexAnimationTemp = 0;
					for(int i=0; i<CountFrameFull; i++)
					{
						IndexAnimationTemp = IndexAnimationGetFrameNo(i);
						if(-1 != IndexAnimationTemp)
						{
							IndexAnimationMax = IndexAnimationTemp;
						}
					}

					int[] ArrayNo = new int[IndexAnimationMax+1];
					for(int i=0; i<IndexAnimationMax; i++)
					{
						ArrayNo[i] = -1;
					}
					for(int i=0; i<DataOriginalArray.Count; i++)
					{
						DataDummy = (DataIntermediate.KeyFrame.DataInstance)DataOriginalArray[i];
						IndexAnimationTemp = IndexAnimationGetFrameNo(DataDummy.Time);
						if(-1 != IndexAnimationTemp)
						{
							ArrayNo[IndexAnimationTemp] = i;
						}
					}
					for(int i=0; i<IndexAnimationMax; i++)
					{
						if(-1 == ArrayNo[i])
						{
							for(int j=0; j<CountFrameFull; j++)
							{
								IndexAnimationTemp = IndexAnimationGetFrameNo(i);
								if(i == IndexAnimationTemp)
								{
									break;
								}
							}

							DataIntermediate.KeyFrame.ValueInstance Value = new DataIntermediate.KeyFrame.ValueInstance();
							Value.Flag = LibraryEditor_SpriteStudio.DataIntermediate.KeyFrame.ValueInstance.FlagData.CLEAR;
							Value.LabelStart = string.Copy(Library_SpriteStudio.AnimationInformationPlay.LabelDefaultStart);
							Value.OffsetStart = 0;
							Value.LabelEnd = string.Copy(Library_SpriteStudio.AnimationInformationPlay.LabelDefaultEnd);
							Value.OffsetEnd = 0;
							Value.PlayCount = 1;
							Value.RateTime = 1.0f;

							DataDummy = new DataIntermediate.KeyFrame.DataInstance();
							DataDummy.Time = IndexAnimationTemp;
							DataDummy.Kind = LibraryEditor_SpriteStudio.DataIntermediate.KindAttributeKey.INSTANCE;
							DataDummy.Curve = null;
							DataDummy.ObjectValue = Value;

							DataOriginalArray.Add(DataDummy);
						}
					}
				}

				/* Create Body-Data Array */
				int Count = DataOriginalArray.Count;
				ArrayDataBody = new Library_SpriteStudio.KeyFrame.ValueInstance.Data[Count];

				DataIntermediate.KeyFrame.DataInstance DataOriginal;
				for(int i=0; i<Count; i++)
				{
					DataOriginal = (DataIntermediate.KeyFrame.DataInstance)DataOriginalArray[i];
					ArrayDataBody[i] = new Library_SpriteStudio.KeyFrame.ValueInstance.Data();

					/* Copy Data-Body */
					ArrayDataBody[i].FrameNoBase = DataOriginal.Time;
					ArrayDataBody[i].Flag = Library_SpriteStudio.KeyFrame.ValueInstance.Data.FlagData.CLEAR;
					ArrayDataBody[i].Flag |= (true == DataOriginal.Value.IsPingPong) ? Library_SpriteStudio.KeyFrame.ValueInstance.Data.FlagData.PINGPONG : 0;
					ArrayDataBody[i].Flag |= (true == DataOriginal.Value.IsIndependent) ? Library_SpriteStudio.KeyFrame.ValueInstance.Data.FlagData.INDEPENDENT : 0;

					ArrayDataBody[i].LabelStart = string.Copy(DataOriginal.Value.LabelStart);
					ArrayDataBody[i].OffsetStart = DataOriginal.Value.OffsetStart;
					ArrayDataBody[i].LabelEnd = string.Copy(DataOriginal.Value.LabelEnd);
					ArrayDataBody[i].OffsetEnd = DataOriginal.Value.OffsetEnd;
					ArrayDataBody[i].PlayCount = DataOriginal.Value.PlayCount;
					ArrayDataBody[i].RateTime = DataOriginal.Value.RateTime;
					ArrayDataBody[i].RateTime *= (true == DataOriginal.Value.IsReverse) ? -1.0f : 1.0f;
				}

				/* Create & Initialize All Frames */
				bool[] FlagSetData = new bool[CountFrameFull];
				for(int i=0; i<CountFrameFull; i++)
				{
					FlagSetData[i] = false;
				}
				int[] DataOutput = new int[CountFrameFull];
				for(int i=0; i<CountFrameFull; i++)
				{
					DataOutput[i] = -1;
				}

				/* Set Frames */
				int CountKeyFrame;
				int IndexKeyBase;
				int IndexKeyNow;
				int IndexAnimation;
				int IndexAnimationNow;

				if(null != DataOriginalArray)
				{
					/* Set Key-Frames */
					CountKeyFrame = DataOriginalArray.Count;
					for(int i=0; i<CountKeyFrame; i++)
					{
						/* Create Data-Body */
						DataOriginal = (DataIntermediate.KeyFrame.DataInstance)DataOriginalArray[i];
						IndexKeyBase = DataOriginal.Time;
						DataOutput[IndexKeyBase] = i;
						FlagSetData[IndexKeyBase] = true;
					}

					/* Solving All-KeyFrame */
					CountKeyFrame = DataOutput.Length;
					IndexKeyBase = -1;
					for(int i=0; i<CountKeyFrame; i++)
					{
						IndexAnimation = IndexAnimationGetFrameNo(i);
						if(-1 == IndexAnimation)
						{	/* Animation Range Over */
							IndexKeyBase = -1;	/* Clear Base-Key */
							continue;
						}

						if(false == FlagSetData[i])
						{	/* Not-Solved Frame */
							if(-1 == IndexKeyBase)
							{	/* Before First Valid-KeyData */
								continue;
							}

							DataOutput[i] = DataOutput[IndexKeyBase];
							FlagSetData[i] = true;
						}
						else
						{	/* Valid Key-Data */
							/* Solving Previous Frames */
							IndexKeyBase = i;
							IndexKeyNow = i - 1;
							for( ; ; )
							{
								if(IndexKeyNow < 0)
								{	/* Data Range Over */
									break;
								}

								if(true == FlagSetData[IndexKeyNow])
								{	/* Already Solved */
									break;
								}

								IndexAnimationNow = IndexAnimationGetFrameNo(IndexKeyNow);
								if((-1 == IndexAnimationNow) || (IndexAnimation != IndexAnimationNow))
								{
									break;
								}
								DataOutput[IndexKeyNow] = DataOutput[IndexKeyBase];
								FlagSetData[IndexKeyNow] = true;

								IndexKeyNow--;
							}
						}
					}
				}

				return(DataOutput);
			}
			private Library_SpriteStudio.KeyFrame.ValueBools[] AnimationDataConvertRuntimeBools(	ArrayList DataOriginalArrayHide,
																									ArrayList DataOriginalArrayFlipX,
																									ArrayList DataOriginalArrayFlipY,
																									ArrayList DataOriginalArrayTextureFlipX,
																									ArrayList DataOriginalArrayTextureFlipY
																								)
			{
				/* Create New Datas */
				Library_SpriteStudio.KeyFrame.ValueBools[] DataOutput = new Library_SpriteStudio.KeyFrame.ValueBools[CountFrameFull];

				/* Valid Flag-Set */
				for(int i=0; i<CountFrameFull; i++)
				{
					DataOutput[i] = new Library_SpriteStudio.KeyFrame.ValueBools();
					if(0 <= IndexAnimationGetFrameNo(i))
					{
						DataOutput[i].Flag = Library_SpriteStudio.KeyFrame.ValueBools.FlagData.CLEAR;
						DataOutput[i].Flag |= Library_SpriteStudio.KeyFrame.ValueBools.FlagData.VALID;
					}
				}

				/* Check Phantom-Data */
				if((null == DataOriginalArrayHide) && (null == DataOriginalArrayFlipX) && (null == DataOriginalArrayFlipY) && (null == DataOriginalArrayTextureFlipX) && (null == DataOriginalArrayTextureFlipY))
				{	/* All Attributes don't exist */
					return(DataOutput);
				}

				/* Set Hide */
				for(int i=0; i<CountFrameFull; i++)
				{
					DataOutput[i].Flag |= Library_SpriteStudio.KeyFrame.ValueBools.FlagData.HIDE;
				}

				AnimationDataConvertRuntimeBoolsHide(DataOutput, DataOriginalArrayHide);
				AnimationDataConvertRuntimeBoolsFlipX(DataOutput, DataOriginalArrayFlipX);
				AnimationDataConvertRuntimeBoolsFlipY(DataOutput, DataOriginalArrayFlipY);
				AnimationDataConvertRuntimeBoolsTextureFlipX(DataOutput, DataOriginalArrayTextureFlipX);
				AnimationDataConvertRuntimeBoolsTextureFlipY(DataOutput, DataOriginalArrayTextureFlipY);
				
				return(DataOutput);
			}
			private void AnimationDataConvertRuntimeBoolsHide(Library_SpriteStudio.KeyFrame.ValueBools[] DataOutput, ArrayList DataOriginalArray)
			{
				if(null == DataOriginalArray)
				{	/* Attribute doesn't exist */
					return;
				}

				/* Set Frames */
				Library_SpriteStudio.KeyFrame.ValueBools.FlagData ValueFlag = Library_SpriteStudio.KeyFrame.ValueBools.FlagData.HIDE;
				DataIntermediate.KeyFrame.DataBool DataStart;
				DataIntermediate.KeyFrame.DataBool DataEnd;
				int CountKeyFrame = DataOriginalArray.Count;
				int IndexAnimationStart;
				int IndexAnimationEnd;
				
				for(int i=0; i<CountKeyFrame; i++)
				{
					/* Get Key-Frames */
					DataStart = (DataIntermediate.KeyFrame.DataBool)DataOriginalArray[i];
					IndexAnimationStart = IndexAnimationGetFrameNo(DataStart.Time);
				
					if((CountKeyFrame - 1) == i)
					{
						DataEnd = DataStart;	/* Dummy */
						IndexAnimationEnd = -1;
					}
					else
					{
						DataEnd = (DataIntermediate.KeyFrame.DataBool)DataOriginalArray[i + 1];
						IndexAnimationEnd = IndexAnimationGetFrameNo(DataEnd.Time);
					}

					/* Calculate Frames' Value  */
					if(IndexAnimationStart != IndexAnimationEnd)
					{	/* Differnt Animation */
						if(ListInformationPlay[IndexAnimationStart].FrameEnd > DataStart.Time)
						{	/* Padding to End-Frames  */
							for(int j=DataStart.Time; j<=ListInformationPlay[IndexAnimationStart].FrameEnd; j++)
							{
								DataOutput[j].Flag = (true == DataStart.Value) ? (DataOutput[j].Flag | ValueFlag) : (DataOutput[j].Flag & ~ValueFlag);
							}
						}
					}
					else
					{	/* Same Animation */
						/* Range-Fill (Not Interpolation) */
						for(int j=DataStart.Time; j<=DataEnd.Time; j++)
						{
							DataOutput[j].Flag = (true == DataStart.Value) ? (DataOutput[j].Flag | ValueFlag) : (DataOutput[j].Flag & ~ValueFlag);
						}
					}
				}
			}
			private void AnimationDataConvertRuntimeBoolsFlipX(Library_SpriteStudio.KeyFrame.ValueBools[] DataOutput, ArrayList DataOriginalArray)
			{
				if(null == DataOriginalArray)
				{	/* Attribute doesn't exist */
					return;
				}

				/* Set Frames */
				Library_SpriteStudio.KeyFrame.ValueBools.FlagData ValueFlag = Library_SpriteStudio.KeyFrame.ValueBools.FlagData.FLIPX;
				DataIntermediate.KeyFrame.DataBool DataStart;
				DataIntermediate.KeyFrame.DataBool DataEnd;
				int CountKeyFrame = DataOriginalArray.Count;
				int IndexAnimationStart;
				int IndexAnimationEnd;
				
				for(int i=0; i<CountKeyFrame; i++)
				{
					/* Get Key-Frames */
					DataStart = (DataIntermediate.KeyFrame.DataBool)DataOriginalArray[i];
					IndexAnimationStart = IndexAnimationGetFrameNo(DataStart.Time);
				
					if((CountKeyFrame - 1) == i)
					{
						DataEnd = DataStart;	/* Dummy */
						IndexAnimationEnd = -1;
					}
					else
					{
						DataEnd = (DataIntermediate.KeyFrame.DataBool)DataOriginalArray[i + 1];
						IndexAnimationEnd = IndexAnimationGetFrameNo(DataEnd.Time);
					}

					/* Calculate Frames' Value  */
					if(IndexAnimationStart != IndexAnimationEnd)
					{	/* Differnt Animation */
						if(ListInformationPlay[IndexAnimationStart].FrameEnd > DataStart.Time)
						{	/* Padding to End-Frames  */
							for(int j=DataStart.Time; j<=ListInformationPlay[IndexAnimationStart].FrameEnd; j++)
							{
								DataOutput[j].Flag = (true == DataStart.Value) ? (DataOutput[j].Flag | ValueFlag) : (DataOutput[j].Flag & ~ValueFlag);
							}
						}
					}
					else
					{	/* Same Animation */
						/* Range-Fill (Not Interpolation) */
						for(int j=DataStart.Time; j<=DataEnd.Time; j++)
						{
							DataOutput[j].Flag = (true == DataStart.Value) ? (DataOutput[j].Flag | ValueFlag) : (DataOutput[j].Flag & ~ValueFlag);
						}
					}
				}
			}
			private void AnimationDataConvertRuntimeBoolsFlipY(Library_SpriteStudio.KeyFrame.ValueBools[] DataOutput, ArrayList DataOriginalArray)
			{
				if(null == DataOriginalArray)
				{	/* Attribute doesn't exist */
					return;
				}

				/* Set Frames */
				Library_SpriteStudio.KeyFrame.ValueBools.FlagData ValueFlag = Library_SpriteStudio.KeyFrame.ValueBools.FlagData.FLIPY;
				DataIntermediate.KeyFrame.DataBool DataStart;
				DataIntermediate.KeyFrame.DataBool DataEnd;
				int CountKeyFrame = DataOriginalArray.Count;
				int IndexAnimationStart;
				int IndexAnimationEnd;
				
				for(int i=0; i<CountKeyFrame; i++)
				{
					/* Get Key-Frames */
					DataStart = (DataIntermediate.KeyFrame.DataBool)DataOriginalArray[i];
					IndexAnimationStart = IndexAnimationGetFrameNo(DataStart.Time);

					if((CountKeyFrame - 1) == i)
					{
						DataEnd = DataStart;	/* Dummy */
						IndexAnimationEnd = -1;
					}
					else
					{
						DataEnd = (DataIntermediate.KeyFrame.DataBool)DataOriginalArray[i + 1];
						IndexAnimationEnd = IndexAnimationGetFrameNo(DataEnd.Time);
					}

					/* Calculate Frames' Value  */
					if(IndexAnimationStart != IndexAnimationEnd)
					{	/* Differnt Animation */
						if(ListInformationPlay[IndexAnimationStart].FrameEnd > DataStart.Time)
						{	/* Padding to End-Frames  */
							for(int j=DataStart.Time; j<=ListInformationPlay[IndexAnimationStart].FrameEnd; j++)
							{
								DataOutput[j].Flag = (true == DataStart.Value) ? (DataOutput[j].Flag | ValueFlag) : (DataOutput[j].Flag & ~ValueFlag);
							}
						}
					}
					else
					{	/* Same Animation */
						/* Range-Fill (Not Interpolation) */
						for(int j=DataStart.Time; j<=DataEnd.Time; j++)
						{
							DataOutput[j].Flag = (true == DataStart.Value) ? (DataOutput[j].Flag | ValueFlag) : (DataOutput[j].Flag & ~ValueFlag);
						}
					}
				}
			}
			private void AnimationDataConvertRuntimeBoolsTextureFlipX(Library_SpriteStudio.KeyFrame.ValueBools[] DataOutput, ArrayList DataOriginalArray)
			{
				if(null == DataOriginalArray)
				{	/* Attribute doesn't exist */
					return;
				}

				/* Set Frames */
				Library_SpriteStudio.KeyFrame.ValueBools.FlagData ValueFlag = Library_SpriteStudio.KeyFrame.ValueBools.FlagData.FLIPXTEXTURE;
				DataIntermediate.KeyFrame.DataBool DataStart;
				DataIntermediate.KeyFrame.DataBool DataEnd;
				int CountKeyFrame = DataOriginalArray.Count;
				int IndexAnimationStart;
				int IndexAnimationEnd;
				
				for(int i=0; i<CountKeyFrame; i++)
				{
					/* Get Key-Frames */
					DataStart = (DataIntermediate.KeyFrame.DataBool)DataOriginalArray[i];
					IndexAnimationStart = IndexAnimationGetFrameNo(DataStart.Time);
				
					if((CountKeyFrame - 1) == i)
					{
						DataEnd = DataStart;	/* Dummy */
						IndexAnimationEnd = -1;
					}
					else
					{
						DataEnd = (DataIntermediate.KeyFrame.DataBool)DataOriginalArray[i + 1];
						IndexAnimationEnd = IndexAnimationGetFrameNo(DataEnd.Time);
					}

					/* Calculate Frames' Value  */
					if(IndexAnimationStart != IndexAnimationEnd)
					{	/* Differnt Animation */
						if(ListInformationPlay[IndexAnimationStart].FrameEnd > DataStart.Time)
						{	/* Padding to End-Frames  */
							for(int j=DataStart.Time; j<=ListInformationPlay[IndexAnimationStart].FrameEnd; j++)
							{
								DataOutput[j].Flag = (true == DataStart.Value) ? (DataOutput[j].Flag | ValueFlag) : (DataOutput[j].Flag & ~ValueFlag);
							}
						}
					}
					else
					{	/* Same Animation */
						/* Range-Fill (Not Interpolation) */
						for(int j=DataStart.Time; j<=DataEnd.Time; j++)
						{
							DataOutput[j].Flag = (true == DataStart.Value) ? (DataOutput[j].Flag | ValueFlag) : (DataOutput[j].Flag & ~ValueFlag);
						}
					}
				}
			}
			private void AnimationDataConvertRuntimeBoolsTextureFlipY(Library_SpriteStudio.KeyFrame.ValueBools[] DataOutput, ArrayList DataOriginalArray)
			{
				if(null == DataOriginalArray)
				{	/* Attribute doesn't exist */
					return;
				}

				/* Set Frames */
				Library_SpriteStudio.KeyFrame.ValueBools.FlagData ValueFlag = Library_SpriteStudio.KeyFrame.ValueBools.FlagData.FLIPYTEXTURE;
				DataIntermediate.KeyFrame.DataBool DataStart;
				DataIntermediate.KeyFrame.DataBool DataEnd;
				int CountKeyFrame = DataOriginalArray.Count;
				int IndexAnimationStart;
				int IndexAnimationEnd;
				
				for(int i=0; i<CountKeyFrame; i++)
				{
					/* Get Key-Frames */
					DataStart = (DataIntermediate.KeyFrame.DataBool)DataOriginalArray[i];
					IndexAnimationStart = IndexAnimationGetFrameNo(DataStart.Time);
				
					if((CountKeyFrame - 1) == i)
					{
						DataEnd = DataStart;	/* Dummy */
						IndexAnimationEnd = -1;
					}
					else
					{
						DataEnd = (DataIntermediate.KeyFrame.DataBool)DataOriginalArray[i + 1];
						IndexAnimationEnd = IndexAnimationGetFrameNo(DataEnd.Time);
					}

					/* Calculate Frames' Value  */
					if(IndexAnimationStart != IndexAnimationEnd)
					{	/* Differnt Animation */
						if(ListInformationPlay[IndexAnimationStart].FrameEnd > DataStart.Time)
						{	/* Padding to End-Frames  */
							for(int j=DataStart.Time; j<=ListInformationPlay[IndexAnimationStart].FrameEnd; j++)
							{
								DataOutput[j].Flag = (true == DataStart.Value) ? (DataOutput[j].Flag | ValueFlag) : (DataOutput[j].Flag & ~ValueFlag);
							}
						}
					}
					else
					{	/* Same Animation */
						/* Range-Fill (Not Interpolation) */
						for(int j=DataStart.Time; j<=DataEnd.Time; j++)
						{
							DataOutput[j].Flag = (true == DataStart.Value) ? (DataOutput[j].Flag | ValueFlag) : (DataOutput[j].Flag & ~ValueFlag);
						}
					}
				}
			}

			public bool NodeSetCollider(	GameObject GameObjectParts,
											int IndexParts,
											ref SettingImport DataSettingImport
										)
			{
				Collider InstanceCollider = null;
				switch(ListParts[IndexParts].CollisionKind)
				{
					case Library_SpriteStudio.KindCollision.NON:
						return(true);

					case Library_SpriteStudio.KindCollision.SQUARE:
						{
							if(true == DataSettingImport.FlagAttachRigidBody)
							{
								Rigidbody InstanceRigidbody = GameObjectParts.AddComponent<Rigidbody>();
								InstanceRigidbody.isKinematic = false;
								InstanceRigidbody.useGravity = false;
							}

							BoxCollider InstanceColliderBox = GameObjectParts.AddComponent<BoxCollider>();
							InstanceColliderBox.enabled = false;
							InstanceColliderBox.size = new Vector3(1.0f, 1.0f, DataSettingImport.CollisionThicknessZ);
							InstanceColliderBox.isTrigger = false;

							InstanceCollider = InstanceColliderBox;
							ListDataRuntime[IndexParts].DataMain.CollisionKind = Library_SpriteStudio.KindCollision.SQUARE;
							ListDataRuntime[IndexParts].CollisionComponent = InstanceCollider;
						}
						break;

					case Library_SpriteStudio.KindCollision.AABB:
						return(true);

					case Library_SpriteStudio.KindCollision.CIRCLE:
						{
							if(true == DataSettingImport.FlagAttachRigidBody)
							{
								Rigidbody InstanceRigidbody = GameObjectParts.AddComponent<Rigidbody>();
								InstanceRigidbody.isKinematic = false;
								InstanceRigidbody.useGravity = false;
							}

							CapsuleCollider  InstanceColliderCapsule = GameObjectParts.AddComponent<CapsuleCollider>();
							InstanceColliderCapsule.enabled = false;
							InstanceColliderCapsule.radius = 1.0f;
							InstanceColliderCapsule.height = DataSettingImport.CollisionThicknessZ;
							InstanceColliderCapsule.direction = 2;
							InstanceColliderCapsule.isTrigger = false;

							InstanceCollider = InstanceColliderCapsule;
							ListDataRuntime[IndexParts].DataMain.CollisionKind = Library_SpriteStudio.KindCollision.CIRCLE;
							ListDataRuntime[IndexParts].CollisionComponent = InstanceCollider;
						}
						break;

					case Library_SpriteStudio.KindCollision.CIRCLE_SCALEMINIMUM:
						return(true);

					case Library_SpriteStudio.KindCollision.CIRCLE_SCALEMAXIMUM:
						return(true);
				}

				Script_SpriteStudio_Triangle2 ComponentScript_Triangle2 = GameObjectParts.GetComponent<Script_SpriteStudio_Triangle2>();
				if(null != ComponentScript_Triangle2)
				{	/* Normal (Triangle-2) */
					ComponentScript_Triangle2.CollisionComponent = InstanceCollider;
				}
				else
				{
					Script_SpriteStudio_Triangle4 ComponentScript_Triangle4 = GameObjectParts.GetComponent<Script_SpriteStudio_Triangle4>();
					if(null != ComponentScript_Triangle4)
					{	/* Normal (Triangle-4) */
						ComponentScript_Triangle4.CollisionComponent = InstanceCollider;
					}
					else
					{
						Script_SpriteStudio_PartsNULL ComponentScript_PartsNULL = GameObjectParts.GetComponent<Script_SpriteStudio_PartsNULL>();
						if(null != ComponentScript_PartsNULL)
						{	/* NULL */
							ComponentScript_PartsNULL.CollisionComponent = InstanceCollider;
						}
						else
						{
							Script_SpriteStudio_PartsInstance ComponentScript_PartsInstance = GameObjectParts.GetComponent<Script_SpriteStudio_PartsInstance>();
							if(null != ComponentScript_PartsInstance)
							{	/* Instance */
								ComponentScript_PartsInstance.CollisionComponent = InstanceCollider;
							}
							else
							{
								Script_SpriteStudio_PartsRoot ComponentScript_PartsRoot = GameObjectParts.GetComponent<Script_SpriteStudio_PartsRoot>();
								if(null != ComponentScript_PartsRoot)
								{	/* Root */
									ComponentScript_PartsRoot.CollisionComponent = InstanceCollider;
								}
								else
								{	/* Node is Invalid */
									Debug.LogError("SSAE-Create-Animation-Data Error: Collider[" + ListParts[IndexParts].Name + "] Invalid Node-Kind.");
									return(false);
								}
							}
						}
					}
				}

				return(true);
			}

			private void DataRuntimeSolvingInherit(int NodeNo)
			{
				/* Get My Datas */
				int Count = CountFrameFull;
				Library_SpriteStudio.KeyFrame.ValueBools[] DataFlags = ListDataRuntime[NodeNo].DataMain.AnimationDataFlags;
				float[] OpacityRate = ListDataRuntime[NodeNo].DataMain.AnimationDataOpacityRate;
				FlagAttributeKeyInherit FlagInherit = DataRuntimeParentGetInherit(NodeNo);

				/* Get Parent-Datas */
				bool FlagRootFlipX = false;
				Library_SpriteStudio.KeyFrame.ValueBools[] DataFlagsParentFlipX = DataRuntimeParentGetFlags(out FlagRootFlipX, NodeNo, FlagAttributeKeyInherit.FLIP_X);

				bool FlagRootFlipY = false;
				Library_SpriteStudio.KeyFrame.ValueBools[] DataFlagsParentFlipY = DataRuntimeParentGetFlags(out FlagRootFlipY, NodeNo, FlagAttributeKeyInherit.FLIP_Y);

				bool FlagRootHide = false;
				Library_SpriteStudio.KeyFrame.ValueBools[] DataFlagsParentHide = DataRuntimeParentGetFlags(out FlagRootHide, NodeNo, FlagAttributeKeyInherit.SHOW_HIDE);

				/* Solving Flip-X */
				if(0 != (FlagInherit & FlagAttributeKeyInherit.FLIP_X) && (null != DataFlagsParentFlipX))
				{
					Library_SpriteStudio.KeyFrame.ValueBools.FlagData MaskFlag = Library_SpriteStudio.KeyFrame.ValueBools.FlagData.FLIPX;
					Library_SpriteStudio.KeyFrame.ValueBools.FlagData DataParent;
					Library_SpriteStudio.KeyFrame.ValueBools.FlagData Data;
					for(int i=0; i<Count; i++)
					{
						Data = DataFlags[i].Flag & MaskFlag;
						DataParent = DataFlagsParentFlipX[i].Flag & MaskFlag;

						/* XOR (1:1=0/1:0=1/0:1=1/0:0=0) */
						DataFlags[i].Flag &= ~MaskFlag;
						DataFlags[i].Flag |= (Data ^ DataParent);
					}
				}

				/* Solving Flip-Y */
				if(0 != (FlagInherit & FlagAttributeKeyInherit.FLIP_Y) && (null != DataFlagsParentFlipY))
				{
					Library_SpriteStudio.KeyFrame.ValueBools.FlagData MaskFlag = Library_SpriteStudio.KeyFrame.ValueBools.FlagData.FLIPY;
					Library_SpriteStudio.KeyFrame.ValueBools.FlagData DataParent;
					Library_SpriteStudio.KeyFrame.ValueBools.FlagData Data;
					for(int i=0; i<Count; i++)
					{
						Data = DataFlags[i].Flag & MaskFlag;
						DataParent = DataFlagsParentFlipY[i].Flag & MaskFlag;

						/* XOR (1:1=0/1:0=1/0:1=1/0:0=0) */
						DataFlags[i].Flag &= ~MaskFlag;
						DataFlags[i].Flag |= (Data ^ DataParent);
					}
				}

				/* Solving Hide */
				switch(VersionCodeSSAE)
				{
					case ParseOPSS.KindVersionSSAE.VERSION_010000:
					case ParseOPSS.KindVersionSSAE.VERSION_010001:
						if(0 != (FlagInherit & FlagAttributeKeyInherit.SHOW_HIDE) && (null != DataFlagsParentHide) && (null != ListParts[NodeNo].DataAnimation.DataKeyFrame[(int)KindAttributeKey.SHOW_HIDE]))
						{
							Library_SpriteStudio.KeyFrame.ValueBools.FlagData MaskFlag = Library_SpriteStudio.KeyFrame.ValueBools.FlagData.HIDE;
							Library_SpriteStudio.KeyFrame.ValueBools.FlagData DataParent;
							for(int i=0; i<Count; i++)
							{
								if(true == ValidityCheckFrameNoBool(NodeNo, i, LibraryEditor_SpriteStudio.DataIntermediate.KindAttributeKey.SHOW_HIDE))
								{
									DataParent = DataFlagsParentHide[i].Flag & MaskFlag;

									/* Copy Parent's Data */
									DataFlags[i].Flag &= ~MaskFlag;
									DataFlags[i].Flag |= DataParent;
								}
							}
						}
						break;

					case ParseOPSS.KindVersionSSAE.VERSION_010002:
					case ParseOPSS.KindVersionSSAE.VERSION_010200:
						if(0 != (FlagInherit & FlagAttributeKeyInherit.SHOW_HIDE) && (null != DataFlagsParentHide))
						{
							Library_SpriteStudio.KeyFrame.ValueBools.FlagData MaskFlag = Library_SpriteStudio.KeyFrame.ValueBools.FlagData.HIDE;
							Library_SpriteStudio.KeyFrame.ValueBools.FlagData DataParent;
							for(int i=0; i<Count; i++)
							{
								if(true == ValidityCheckFrameNoBool(NodeNo, i, LibraryEditor_SpriteStudio.DataIntermediate.KindAttributeKey.SHOW_HIDE))
								{
									DataParent = DataFlagsParentHide[i].Flag & MaskFlag;

									/* Copy Parent's Data */
									DataFlags[i].Flag &= ~MaskFlag;
									DataFlags[i].Flag |= DataParent;
								}
							}
						}
						break;
				}

				/* Solving Opacity-Rate */
				if(0 != (FlagInherit & FlagAttributeKeyInherit.OPACITY_RATE))
				{	/* Inhelit */
					if(null == OpacityRate)
					{
						/* Create New Frame-Datas */
						OpacityRate = new float[Count];
						for(int i=0; i<Count; i++)
						{
							OpacityRate[i] = 1.0f;
						}
						ListDataRuntime[NodeNo].DataMain.AnimationDataOpacityRate = OpacityRate;
					}

					float[] OpacityRateParent = DataRuntimeParentGetOpacityRate(NodeNo);
					if(null != OpacityRateParent)
					{
						for(int i=0; i<Count; i++)
						{
							OpacityRate[i] *= OpacityRateParent[i];
						}
					}
				}
				if(Library_SpriteStudio.KindParts.INSTANCE == ListParts[NodeNo].PartsKind)
				{
					if(0 == (ListParts[NodeNo].DataAnimation.FlagInheritance & DataIntermediate.FlagAttributeKeyInherit.OPACITY_RATE))
					{
						ListDataRuntime[NodeNo].DataMain.AnimationDataOpacityRate = null;
					}
				}
			}
			private FlagAttributeKeyInherit DataRuntimeParentGetInherit(int NodeNo)
			{
				int NodeNoParent = NodeNo;
				while(-1 != NodeNoParent)
				{
					if(KindInheritance.SELF == ListParts[NodeNoParent].DataAnimation.Inheritance)
					{	/* Has Original Inherit-Parameter */
						return(ListParts[NodeNoParent].DataAnimation.FlagInheritance);
					}
					NodeNoParent = ListParts[NodeNoParent].IDParent;
				}
				return(FlagAttributeKeyInherit.OPACITY_RATE);
			}
			private Library_SpriteStudio.KeyFrame.ValueBools[] DataRuntimeParentGetFlags(out bool FlagRoot, int NodeNo, FlagAttributeKeyInherit FlagData)
			{
				FlagRoot = false;
				int NodeNoParent = ListParts[NodeNo].IDParent;
				while(-1 != NodeNoParent)
				{
					if(null != ListDataRuntime[NodeNoParent].DataMain.AnimationDataFlags)
					{
						if(0 < ListDataRuntime[NodeNoParent].DataMain.AnimationDataFlags.Length)
						{	/* Valid-Node */
							return(ListDataRuntime[NodeNoParent].DataMain.AnimationDataFlags);
						}
					}
					NodeNoParent = ListParts[NodeNoParent].IDParent;
				}
				FlagRoot = true;
				return(null);
			}
			private float[] DataRuntimeParentGetOpacityRate(int NodeNo)
			{
				int NodeNoParent = ListParts[NodeNo].IDParent;
				while(-1 != NodeNoParent)
				{
					if(0 != (ListParts[NodeNoParent].DataAnimation.FlagInheritance & FlagAttributeKeyInherit.OPACITY_RATE))
					{	/* Parent-Node has datas. */
						return(ListDataRuntime[NodeNoParent].DataMain.AnimationDataOpacityRate);	/* valid-datas */
					}
					else
					{	/* Parent-Node has no-datas. */
						if((KindInheritance.SELF == ListParts[NodeNoParent].DataAnimation.Inheritance) && (0 == (ListParts[NodeNoParent].DataAnimation.FlagInheritance & FlagAttributeKeyInherit.OPACITY_RATE)))
						{	/* Parent-Node doesn't inherit datas */
							return(ListDataRuntime[NodeNoParent].DataMain.AnimationDataOpacityRate);	/* null or valid-datas */
						}
					}
					NodeNoParent = ListParts[NodeNoParent].IDParent;
				}
				return(null);
			}

			private bool ValidityCheckFrameNoBool(int NodeNo, int FrameNo, KindAttributeKey KindAttribute)
			{
				/* Frame in Animation ? */
				int AnimationNo = IndexAnimationGetFrameNo(FrameNo);
				if(-1 == AnimationNo)
				{
					return(false);
				}

				/* Existing KeyFrame in Animation ? */
				ArrayList ArrayKeyFrame = ListParts[NodeNo].DataAnimation.DataKeyFrame[(int)KindAttribute];
				DataIntermediate.KeyFrame.DataBool Data = null;
				if(null != ArrayKeyFrame)
				{
					for(int i=0; i<ArrayKeyFrame.Count; i++)
					{
						Data = (DataIntermediate.KeyFrame.DataBool)ArrayKeyFrame[i];
						if((ListInformationPlay[AnimationNo].FrameStart <= Data.Time) && (ListInformationPlay[AnimationNo].FrameEnd >= Data.Time))
						{
							return(true);
						}
					}
				}
				return(false);
			}
			private int IndexAnimationGetFrameNo(int FrameNo)
			{
				for(int i=0; i<ListInformationPlay.Length; i++)
				{
					if((ListInformationPlay[i].FrameStart <= FrameNo) && (ListInformationPlay[i].FrameEnd >= FrameNo))
					{	/* in Range */
						return(i);
					}
				}
				return(-1);	/* Out of Range (Error) */
			}
		}

		/* for Cell */
		internal class InformationCell
		{
			internal Rect Area;
			internal Vector2 Pivot;
			internal float Rotate;

			internal void CleanUp()
			{
				Area.x = 0.0f;
				Area.y = 0.0f;
				Area.width = 0.0f;
				Area.height = 0.0f;
				Pivot = Vector2.zero;
				Rotate = 0.0f;
			}
		}
		internal struct PartsImage
		{
			internal enum KindWrap
			{
				CLAMP = 0,
				REPEAT,
				MIRROR,
			}
			internal enum KindFilter
			{
				NEAREST = 0,
				LINEAR,
			}

			internal string FileName;
			internal KindWrap Wrap;
			internal KindFilter Filter;
			internal Hashtable CellArea;

			internal void CleanUp()
			{
				FileName = "";
				Wrap = KindWrap.CLAMP;
				Filter = KindFilter.LINEAR;
				CellArea = null;
			}
		}

		/* for Sprite */
		internal enum KindInheritance
		{
			PARENT,
			SELF
		};
		internal enum KindTypeKey
		{
			INT,
			FLOAT,
			BOOL,
			HEX,
			DEGREE,
			OTHER,
		};
		internal enum KindValueKey
		{
			NUMBER,
			CHECK,
			POINT,
			PALETTE,
			COLOR,
			QUADRILATERRAL,
			USER,
			SOUND,
			CELL,
			INSTANCE,
		};
		internal enum KindInterpolation
		{
			NON = 0,
			LINEAR,
			HERMITE,
			BEZIER,
			ACCELERATE,
			DECELERATE,
		};
		internal enum FlagDirection
		{
			JUSTNOW = 0x0001,
			PAST = 0x0002,
			FUTURE = 0x0004,
			ALL = JUSTNOW | PAST | FUTURE,
		};
		internal enum KindAttributeKey
		{	/* MEMO: Don't change the order of enumerate, "POSITION_X"-"SHOW_HIDE" */
			POSITION_X = 0,
			POSITION_Y,
			POSITION_Z,
			ROTATE_X,
			ROTATE_Y,
			ROTATE_Z,
			SCALE_X,
			SCALE_Y,

			OPACITY_RATE,
			FLIP_X,
			FLIP_Y,
			SHOW_HIDE,

			PRIORITY,

			COLOR_BLEND,
			VERTEX_CORRECTION,
			ORIGIN_OFFSET_X,
			ORIGIN_OFFSET_Y,

			ANCHOR_POSITION_X,
			ANCHOR_POSITION_Y,
			ANCHOR_SIZE_X,
			ANCHOR_SIZE_Y,

			TEXTURE_FLIP_X,
			TEXTURE_FLIP_Y,

			TEXTURE_TRANSLATE_X,
			TEXTURE_TRANSLATE_Y,
			TEXTURE_ROTATE,
			TEXTURE_SCALE_X,
			TEXTURE_SCALE_Y,
			TEXTURE_EXPAND_WIDTH,
			TEXTURE_EXPAND_HEIGHT,

			COLLISION_RADIUS,

			CELL,
			USER_DATA,

			INSTANCE,

			PALETTE_CHANGE,
			SOUND,

			TERMINATOR,
			TERMINATOR_INHERIT = (SHOW_HIDE + 1),
		};
		internal enum FlagAttributeKeyInherit
		{
			POSITION_X = (1 << KindAttributeKey.POSITION_X),
			POSITION_Y = (1 << KindAttributeKey.POSITION_Y),
			POSITION_Z = (1 << KindAttributeKey.POSITION_Z),
			ROTATE_X = (1 << KindAttributeKey.ROTATE_X),
			ROTATE_Y = (1 << KindAttributeKey.ROTATE_Y),
			ROTATE_Z = (1 << KindAttributeKey.ROTATE_Z),
			SCALE_X = (1 << KindAttributeKey.SCALE_X),
			SCALE_Y = (1 << KindAttributeKey.SCALE_Y),

			OPACITY_RATE = (1 << KindAttributeKey.OPACITY_RATE),
			FLIP_X = (1 << KindAttributeKey.FLIP_X),
			FLIP_Y = (1 << KindAttributeKey.FLIP_Y),
			SHOW_HIDE = (1 << KindAttributeKey.SHOW_HIDE),

			CLEAR = 0,
			ALL = ((1 << KindAttributeKey.TERMINATOR_INHERIT) - 1),
			PRESET = POSITION_X
					| POSITION_Y
					| POSITION_Z
					| ROTATE_X
					| ROTATE_Y
					| ROTATE_Z
					| SCALE_X
					| SCALE_Y
					| OPACITY_RATE
//					| FLIP_X
//					| FLIP_Y
//					| SHOW_HIDE
		};
		internal readonly static FlagAttributeKeyInherit[] FlagParameterKeyFrameInherit = new FlagAttributeKeyInherit[(int)KindAttributeKey.TERMINATOR_INHERIT]
		{
			FlagAttributeKeyInherit.POSITION_X,
			FlagAttributeKeyInherit.POSITION_Y,
			FlagAttributeKeyInherit.POSITION_Z,
			FlagAttributeKeyInherit.ROTATE_X,
			FlagAttributeKeyInherit.ROTATE_Y,
			FlagAttributeKeyInherit.ROTATE_Z,
			FlagAttributeKeyInherit.SCALE_X,
			FlagAttributeKeyInherit.SCALE_Y,

			FlagAttributeKeyInherit.OPACITY_RATE,
			FlagAttributeKeyInherit.FLIP_X,
			FlagAttributeKeyInherit.FLIP_Y,
			FlagAttributeKeyInherit.SHOW_HIDE,
		};
		internal readonly static KindAttributeKey[][] MaskKeyAttribute_OPSS =
		{
			new KindAttributeKey[]
			{	/* NORMAL: Sprite-Node */
				KindAttributeKey.TERMINATOR
			},
			new KindAttributeKey[]
			{	/* ROOT: Root-Node */
				KindAttributeKey.PRIORITY,
				KindAttributeKey.VERTEX_CORRECTION,
				KindAttributeKey.ORIGIN_OFFSET_X,
				KindAttributeKey.ORIGIN_OFFSET_Y,
				KindAttributeKey.TEXTURE_TRANSLATE_X,
				KindAttributeKey.TEXTURE_TRANSLATE_Y,
				KindAttributeKey.TEXTURE_ROTATE,
				KindAttributeKey.TEXTURE_SCALE_X,
				KindAttributeKey.TEXTURE_SCALE_Y,
				KindAttributeKey.PALETTE_CHANGE,
				KindAttributeKey.SOUND,

				KindAttributeKey.TERMINATOR
			},
			new KindAttributeKey[]
			{	/* NULL: NULL-Node */
#if false
				KindAttributeKey.PRIORITY,
				KindAttributeKey.SOUND,
#else
				KindAttributeKey.PRIORITY,
				KindAttributeKey.VERTEX_CORRECTION,
				KindAttributeKey.ORIGIN_OFFSET_X,
				KindAttributeKey.ORIGIN_OFFSET_Y,
				KindAttributeKey.TEXTURE_TRANSLATE_X,
				KindAttributeKey.TEXTURE_TRANSLATE_Y,
				KindAttributeKey.TEXTURE_ROTATE,
				KindAttributeKey.TEXTURE_SCALE_X,
				KindAttributeKey.TEXTURE_SCALE_Y,
				KindAttributeKey.PALETTE_CHANGE,
				KindAttributeKey.SOUND,
#endif

				KindAttributeKey.TERMINATOR
			},
			new KindAttributeKey[]
			{	/* BOUND: Bound-Node (not supported SS5) */
				KindAttributeKey.TERMINATOR
			},
			new KindAttributeKey[]
			{	/* BOUND: Sound-Node (not supported SS5) */
				KindAttributeKey.TERMINATOR
			},
			new KindAttributeKey[]
			{	/* INSTANCE: Instance-Node */
				KindAttributeKey.VERTEX_CORRECTION,
				KindAttributeKey.ORIGIN_OFFSET_X,
				KindAttributeKey.ORIGIN_OFFSET_Y,
				KindAttributeKey.TEXTURE_TRANSLATE_X,
				KindAttributeKey.TEXTURE_TRANSLATE_Y,
				KindAttributeKey.TEXTURE_ROTATE,
				KindAttributeKey.TEXTURE_SCALE_X,
				KindAttributeKey.TEXTURE_SCALE_Y,
				KindAttributeKey.PALETTE_CHANGE,
				KindAttributeKey.SOUND,

				KindAttributeKey.TERMINATOR
			},
		};
		internal class AnimationDataEditor
		{
			internal KindInheritance Inheritance;

			internal FlagAttributeKeyInherit FlagInheritance;
			internal float[] RateInheritance;

			internal FlagAttributeKeyInherit FlagKeyParameter;
			internal ArrayList[] DataKeyFrame;

			internal void CleanUp()
			{
				Inheritance = KindInheritance.PARENT;

				FlagInheritance = FlagAttributeKeyInherit.CLEAR;
				RateInheritance = null;

				FlagKeyParameter = FlagAttributeKeyInherit.CLEAR;
				DataKeyFrame = null;
			}
		}
		internal struct PartsSprite
		{
			internal int ID;
			internal int IDParent;

			internal Library_SpriteStudio.KindParts PartsKind;
			internal Library_SpriteStudio.KindParts ObjectKind;
			internal Library_SpriteStudio.KindCollision CollisionKind;

			internal GameObject GameObjectParts;
			internal string Name;
			internal Library_SpriteStudio.KindColorOperation KindBlendTarget;

			internal AnimationDataEditor DataAnimation;

			internal string InstanceNameSSAE;
			internal string InstanceNameAnimation;

			internal void CleanUp()
			{
				ID = -1;
				IDParent = -1;

				PartsKind = Library_SpriteStudio.KindParts.NORMAL;
				ObjectKind = Library_SpriteStudio.KindParts.TERMINATOR;
				CollisionKind = Library_SpriteStudio.KindCollision.NON;

				GameObjectParts = null;
				Name = "";
				KindBlendTarget = Library_SpriteStudio.KindColorOperation.MIX;

				DataAnimation = null;

				InstanceNameSSAE = null;
				InstanceNameAnimation = null;

			}

			internal void BootUp()
			{
				DataAnimation = new AnimationDataEditor();
			}
		}
		internal static class ManagerDescriptionAttribute
		{
			internal class DescriptionAttribute
			{
				internal DataIntermediate.KindAttributeKey Attribute
				{
					set;
					get;
				}
				internal DataIntermediate.KindValueKey KindValue
				{
					set;
					get;
				}
				internal DataIntermediate.KindTypeKey KindType
				{
					set;
					get;
				}
				internal bool Interpolatable
				{
					get
					{
						switch(KindValue)
						{
							case DataIntermediate.KindValueKey.CHECK:
							case DataIntermediate.KindValueKey.USER:
							case DataIntermediate.KindValueKey.SOUND:
								return(false);
						}
						return(true);
					}
				}

				internal DescriptionAttribute(	DataIntermediate.KindAttributeKey AttributeNew,
												DataIntermediate.KindValueKey KindValueNew,
												DataIntermediate.KindTypeKey KindTypeNew
										)
				{
					Attribute = AttributeNew;
					KindValue = KindValueNew;
					KindType = KindTypeNew;
				}
			}

			private readonly static Dictionary<string, DescriptionAttribute> ListDescriptionSSAE = new Dictionary<string, DescriptionAttribute>
			{
				{"POSX",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.POSITION_X,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.FLOAT
													)
				},
				{"POSY",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.POSITION_Y,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.FLOAT
													)
				},
				{"POSZ",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.POSITION_Z,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.FLOAT
													)
				},

				{"ROTX",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.ROTATE_X,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.DEGREE
													)
				},
				{"ROTY",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.ROTATE_Y,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.DEGREE
													)
				},
				{"ROTZ",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.ROTATE_Z,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.DEGREE
													)
				},

				{"SCLX",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.SCALE_X,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.FLOAT
													)
				},
				{"SCLY",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.SCALE_Y,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.FLOAT
													)
				},

				{"ALPH",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.OPACITY_RATE,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.FLOAT
													)
				},

				{"PRIO",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.PRIORITY,
													 	DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.FLOAT
													)
				},

				{"FLPH",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.FLIP_X,
														DataIntermediate.KindValueKey.CHECK,
														DataIntermediate.KindTypeKey.BOOL
													)
				},
				{"FLPV",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.FLIP_Y,
														DataIntermediate.KindValueKey.CHECK,
														DataIntermediate.KindTypeKey.BOOL
													)
				},
				{"HIDE",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.SHOW_HIDE,
														DataIntermediate.KindValueKey.CHECK,
														DataIntermediate.KindTypeKey.BOOL
													)
				},

				{"VCOL",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.COLOR_BLEND,
														DataIntermediate.KindValueKey.COLOR,
														DataIntermediate.KindTypeKey.OTHER
													)
				},
				{"VERT",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.VERTEX_CORRECTION,
														DataIntermediate.KindValueKey.QUADRILATERRAL,
														DataIntermediate.KindTypeKey.OTHER
													)
				},

				{"PVTX",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.ORIGIN_OFFSET_X,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.FLOAT
													)
				},
				{"PVTY",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.ORIGIN_OFFSET_Y,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.FLOAT
													)
				},

				{"ANCX",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.ANCHOR_POSITION_X,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.FLOAT
													)
				},
				{"ANCY",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.ANCHOR_POSITION_Y,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.FLOAT
													)
				},
				{"SIZX",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.ANCHOR_SIZE_X,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.FLOAT
													)
				},
				{"SIZY",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.ANCHOR_SIZE_Y,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.FLOAT
													)
				},

				{"UVTX",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.TEXTURE_TRANSLATE_X,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.FLOAT
													)
				},
				{"UVTY",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.TEXTURE_TRANSLATE_Y,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.FLOAT
													)
				},
				{"UVRZ",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.TEXTURE_ROTATE,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.DEGREE
													)
				},
				{"UVSX",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.TEXTURE_EXPAND_WIDTH,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.FLOAT
													)
				},
				{"UVSY",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.TEXTURE_EXPAND_HEIGHT,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.FLOAT
													)
				},
				{"BNDR",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.COLLISION_RADIUS,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.FLOAT
													)
				},

				{"CELL",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.CELL,
														DataIntermediate.KindValueKey.CELL,
														DataIntermediate.KindTypeKey.OTHER
													)
				},
				{"USER",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.USER_DATA,
														DataIntermediate.KindValueKey.USER,
														DataIntermediate.KindTypeKey.OTHER
													)
				},

				{"IFLH",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.TEXTURE_FLIP_X,
														DataIntermediate.KindValueKey.CHECK,
														DataIntermediate.KindTypeKey.BOOL
													)
				},
				{"IFLV",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.TEXTURE_FLIP_Y,
														DataIntermediate.KindValueKey.CHECK,
														DataIntermediate.KindTypeKey.BOOL
													)
				},

				{"IMGX",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.TEXTURE_TRANSLATE_X,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.FLOAT
													)
				},
				{"IMGY",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.TEXTURE_TRANSLATE_Y,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.FLOAT
													)
				},
				{"IMGW",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.TEXTURE_EXPAND_WIDTH,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.FLOAT
													)
				},
				{"IMGH",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.TEXTURE_EXPAND_HEIGHT,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.FLOAT
													)
				},

				{"ORFX",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.ORIGIN_OFFSET_X,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.FLOAT
													)
				},
				{"ORFY",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.ORIGIN_OFFSET_Y,
														DataIntermediate.KindValueKey.NUMBER,
														DataIntermediate.KindTypeKey.FLOAT
													)
				},

				{"IPRM",	new DescriptionAttribute(	DataIntermediate.KindAttributeKey.INSTANCE,
														DataIntermediate.KindValueKey.INSTANCE,
														DataIntermediate.KindTypeKey.OTHER
													)
				},
			};

			static internal DescriptionAttribute DescriptionAttributeGetTagName(string NameTag)
			{
				DescriptionAttribute Description;
				return((true == ListDescriptionSSAE.TryGetValue(NameTag, out Description)) ? Description : null);
			}

			static internal DescriptionAttribute DescriptionAttributeGetKey(DataIntermediate.KindAttributeKey Key)
			{
				foreach (DescriptionAttribute Description in ListDescriptionSSAE.Values)
				{
					if(Description.Attribute == Key)
					{
						return(Description);
					}
				}
				return(null);
			}
		}

		internal static class KeyFrame
		{
			public class DataCurve
			{
				internal KindInterpolation Kind;
				internal float TimeStart;
				internal float ValueStart;
				internal float TimeEnd;
				internal float ValueEnd;

				public bool CheckEqual(DataCurve Target)
				{
					if(Kind == Target.Kind)
					{
						if(TimeStart == Target.TimeStart)
						{
							if(ValueStart == Target.ValueStart)
							{
								if(TimeEnd == Target.TimeEnd)
								{
									if(ValueEnd != Target.ValueEnd)
									{
										return(true);
									}
								}
							}
						}
					}
					return(false);
				}

				public override string ToString()
				{
					return(	"Type: " + Kind
							+ ", StartT: " + TimeStart
							+ ", StartV: " + ValueStart
							+ ", EndT: " + TimeEnd
							+ ", EndV: " + ValueEnd
						);
				}
			}
			public interface InterfaceData
			{
				KindAttributeKey Kind { get; set; }
				int Time { get; set; }
				object ObjectValue { get; set; }
				DataCurve Curve { get; set; }

				bool ValueCheckEqual(InterfaceData Target);
			}
			public interface InterfaceInterpolatable
			{
				InterfaceInterpolatable GetInterpolated(	DataCurve Curve,
															int Time,
															InterfaceInterpolatable ValueStart,
															InterfaceInterpolatable ValueEnd,
															int TimeStart,
															int TimeEnd
														);

				InterfaceInterpolatable Interpolate(	DataCurve Curve,
														int Time,
														InterfaceInterpolatable ValueStart,
														InterfaceInterpolatable ValueEnd,
														int TimeStart,
														int TimeEnd
													);
			}
			public class DataBase<_Type> : InterfaceData
			{
				protected KindAttributeKey ValueKind;
				public KindAttributeKey	Kind
				{
					get { return(ValueKind); }
					set { ValueKind = value; }
				}
				protected int ValueTime;
				public int Time
				{
					get { return(ValueTime); }
					set { ValueTime = value; }
				}
				protected DataCurve ValueCurve;
				public DataCurve Curve
				{
					get { return(ValueCurve); }
					set { ValueCurve = value; }
				}
				internal _Type Value;
				public object ObjectValue
				{
					get{ return(Value); }
					set{ Value = (_Type)value; }
				}

				public bool ValueCheckEqual(InterfaceData Target)
				{
					if((null != Curve) && (null != Target.Curve))
					{
						if(false == Curve.CheckEqual(Target.Curve))
						{
							return(false);
						}
					}
					else
					{
						if(Curve != Target.Curve)
						{
							return(false);
						}
					}

					DataBase<_Type> TargetDerived = (DataBase<_Type>)Target;
					return(Value.Equals(TargetDerived.Value));
				}
				public override string ToString()
				{
					return(	"MyType: " + typeof(_Type)
							+ ", ValueType: " + Kind
							+ ", Time: " + Time
							+ ", Value {" + ObjectValue
							+ "}, Curve {" + Curve + "}\n"
						);
				}
			}

			public class ValuePoint : InterfaceInterpolatable
			{
				internal Vector2 Point;
				public float X
				{
					set	{	Point.x = value;	}
					get	{	return(Point.x);	}
				}
				public float Y
				{
					set	{	Point.y = value;	}
					get	{	return(Point.y);	}
				}

				public ValuePoint()
				{
				}
				public ValuePoint(ValuePoint Value)
				{
					Point = Value.Point;
				}
				public ValuePoint Clone()
				{
					ValuePoint Value = new ValuePoint(this);
					return(Value);
				}
				public override string ToString()
				{
					return(	"X: " + X
							+ ", Y: " + Y
						);
				}
				public static ValuePoint[] CreateArray(int Count)
				{
					var ArrayPoint = new ValuePoint[Count];
					for(int i=0; i<Count; i++)
					{
						ArrayPoint[i] = new ValuePoint();
					}
					return(ArrayPoint);
				}
				public InterfaceInterpolatable GetInterpolated(	DataCurve Curve,
																	int TimeNow,
																	InterfaceInterpolatable Start,
																	InterfaceInterpolatable End,
																	int TimeStart,
																	int TimeEnd
																)
				{
					ValuePoint Value = new ValuePoint();
					return(Value.Interpolate(Curve, TimeNow, Start, End, TimeStart, TimeEnd));
				}
				public InterfaceInterpolatable Interpolate(	DataCurve Curve,
																int TimeNow,
																InterfaceInterpolatable Start,
																InterfaceInterpolatable End,
																int TimeStart,
																int TimeEnd
															)
				{
					ValuePoint ValueStart = (ValuePoint)Start;
					ValuePoint ValueEnd = (ValuePoint)End;
					Point.x = Interpolation.Interpolate<float>(Curve, TimeNow, ValueStart.Point.x, ValueEnd.Point.x, TimeStart, TimeEnd);
					Point.y = Interpolation.Interpolate<float>(Curve, TimeNow, ValueStart.Point.y, ValueEnd.Point.y, TimeStart, TimeEnd);
					return(this);
				}
				public void Scale(float Ratio)
				{
					Point *= Ratio;
				}
			}

			public class ValuePalette : InterfaceInterpolatable
			{
				internal bool FlagUse;
				internal int Page;
				internal byte Block;
				public bool Use
				{
					set {	FlagUse = value;	}
					get {	return(FlagUse);	}
				} 

				public ValuePalette()
				{
				}
				public ValuePalette(ValuePalette Value)
				{
					Page = Value.Page;
					Block = Value.Block;
				}
				public ValuePalette Clone()
				{
					ValuePalette Value = new ValuePalette(this);
					return(Value);
				}
				public override string ToString()
				{
					return(	"FlagUse: " + FlagUse
							+ "Page: " + Page
							+ ", Block: " + Block
						);
				}
				public InterfaceInterpolatable GetInterpolated(	DataCurve Curve,
																	int TimeNow,
																	InterfaceInterpolatable Start,
																	InterfaceInterpolatable End,
																	int TimeStart,
																	int TimeEnd
																)
				{
					ValuePalette Value = new ValuePalette();
					return(Value.Interpolate(Curve, TimeNow, Start, End, TimeStart, TimeEnd));
				}
				public InterfaceInterpolatable Interpolate(	DataCurve Curve,
																int TimeNow,
																InterfaceInterpolatable Start,
																InterfaceInterpolatable End,
																int TimeStart,
																int TimeEnd
															)
				{
					ValuePalette ValueStart = (ValuePalette)Start;
					ValuePalette ValueEnd = (ValuePalette)End;
					FlagUse = (0.5f <= Interpolation.Interpolate<float>(	Curve,
																			TimeNow,
																			((true == ValueStart.FlagUse) ? 1.0f : 0.0f),
																			((true == ValueEnd.FlagUse) ? 1.0f : 0.0f),
																			TimeStart,
																			TimeEnd
																		)) ? true : false;
					Page = (int)Interpolation.Interpolate<int>(Curve, TimeNow, ValueStart.Page, ValueEnd.Page, TimeStart, TimeEnd);
					Block = (byte)(Interpolation.Interpolate<byte>(Curve, TimeNow, ValueStart.Block, ValueEnd.Block, TimeStart, TimeEnd));
					return(this);
				}
			}
			public class ValueColor : InterfaceInterpolatable
			{
				internal Library_SpriteStudio.KindColorBound Bound;
				internal Library_SpriteStudio.KindColorOperation Operation;
				internal Color[] VertexColor;
				internal float[] RatePixelAlpha;

				public ValueColor()
				{
					Bound = Library_SpriteStudio.KindColorBound.NON;
					Operation = Library_SpriteStudio.KindColorOperation.MIX;
					VertexColor = new Color[4];
					RatePixelAlpha = new float[4];
					for(int i=0; i<VertexColor.Length; i++)
					{
						VertexColor[i] = Color.white;
						RatePixelAlpha[i] = 0.0f;
					}
				}
				public ValueColor(ValueColor Value)
				{
					Bound = Value.Bound;
					Operation = Value.Operation;
					VertexColor = new Color[4];
					RatePixelAlpha = new float[4];
					for(int i=0; i<Value.VertexColor.Length; i++)
					{
						VertexColor[i] = Value.VertexColor[i];
						RatePixelAlpha[i] = Value.RatePixelAlpha[i];
					}
				}
				public ValueColor Clone()
				{
					ValueColor Value = new ValueColor(this);
					return(Value);
				}
				public override string ToString()
				{
					string Text = "Bound: " + Bound + ", Operation: " + Operation;
					if(null != VertexColor)
					{
						for(int i=0; i<VertexColor.Length; i++)
						{
							Text += ", Color("
									+ i
									+ ")["
									+ VertexColor[i].ToString()
									+ "], PixelAlphaRate ["
									+ RatePixelAlpha[i].ToString();
						}
					}
					return(Text);
				}
				public InterfaceInterpolatable GetInterpolated(	DataCurve Curve,
																	int TimeNow,
																	InterfaceInterpolatable Start,
																	InterfaceInterpolatable End,
																	int TimeStart,
																	int TimeEnd
																)
				{
					ValueColor Value = null;
					Value = new ValueColor();
					return(Value.Interpolate(Curve, TimeNow, Start, End, TimeStart, TimeEnd));
				}
				public InterfaceInterpolatable Interpolate(	DataCurve Curve,
																int TimeNow,
																InterfaceInterpolatable Start,
																InterfaceInterpolatable End,
																int TimeStart,
																int TimeEnd
															)
				{
					ValueColor ValueStart = (ValueColor)Start;
					ValueColor ValueEnd = (ValueColor)End;

					if((Library_SpriteStudio.KindColorBound.NON == ValueStart.Bound) && (Library_SpriteStudio.KindColorBound.NON == ValueEnd.Bound))
					{
						Debug.LogError("Sprite Color-Blend Error!!");
					}
					else
					{
						for(int i=0; i<VertexColor.Length; i++)
						{
#if false
							/* MEMO: SpriteStudio Ver.5.0-5.2 */
							VertexColor[i].r = Interpolation.Interpolate<float>(Curve, TimeNow, ValueStart.VertexColor[i].r , ValueEnd.VertexColor[i].r, TimeStart, TimeEnd);
							VertexColor[i].g = Interpolation.Interpolate<float>(Curve, TimeNow, ValueStart.VertexColor[i].g , ValueEnd.VertexColor[i].g, TimeStart, TimeEnd);
							VertexColor[i].b = Interpolation.Interpolate<float>(Curve, TimeNow, ValueStart.VertexColor[i].b , ValueEnd.VertexColor[i].b, TimeStart, TimeEnd);
							VertexColor[i].a = Interpolation.Interpolate<float>(Curve, TimeNow, ValueStart.VertexColor[i].a , ValueEnd.VertexColor[i].a, TimeStart, TimeEnd);

							RatePixelAlpha[i] = Interpolation.Interpolate<float>(Curve, TimeNow, ValueStart.RatePixelAlpha[i], ValueEnd.RatePixelAlpha[i], TimeStart, TimeEnd);
#else
							/* MEMO: SpriteStudio Ver.5.2- or Ver -4.x */
							float Rate = Interpolation.Interpolate<float>(Curve, TimeNow, 0.0f, 1.0f, TimeStart, TimeEnd);
							Rate = (0.0f > Rate) ? 0.0f : Rate;
							Rate = (1.0f < Rate) ? 1.0f : Rate;

							VertexColor[i].r = Interpolation.Linear(ValueStart.VertexColor[i].r, ValueEnd.VertexColor[i].r, Rate);
							VertexColor[i].g = Interpolation.Linear(ValueStart.VertexColor[i].g, ValueEnd.VertexColor[i].g, Rate);
							VertexColor[i].b = Interpolation.Linear(ValueStart.VertexColor[i].b, ValueEnd.VertexColor[i].b, Rate);
							VertexColor[i].a = Interpolation.Linear(ValueStart.VertexColor[i].a, ValueEnd.VertexColor[i].a, Rate);

							RatePixelAlpha[i] = Interpolation.Linear(ValueStart.RatePixelAlpha[i], ValueEnd.RatePixelAlpha[i], Rate);
#endif
						}

						bool FlagStartParam = (TimeEnd > TimeNow) ? true : false;
						if(Library_SpriteStudio.KindColorBound.NON == ValueStart.Bound)
						{
							for(int i=0; i<VertexColor.Length; i++)
							{
								VertexColor[i].r = ValueEnd.VertexColor[i].r;
								VertexColor[i].g = ValueEnd.VertexColor[i].g;
								VertexColor[i].b = ValueEnd.VertexColor[i].b;
								VertexColor[i].a = ValueEnd.VertexColor[i].a;

								RatePixelAlpha[i] = ValueEnd.RatePixelAlpha[i];
							}
							FlagStartParam = false;
						}
						else
						{
							if(Library_SpriteStudio.KindColorBound.NON == ValueEnd.Bound)
							{
								for(int i=0; i<VertexColor.Length; i++)
								{
									VertexColor[i].r = ValueStart.VertexColor[i].r;
									VertexColor[i].g = ValueStart.VertexColor[i].g;
									VertexColor[i].b = ValueStart.VertexColor[i].b;
									VertexColor[i].a = ValueStart.VertexColor[i].a;

									RatePixelAlpha[i] = ValueStart.RatePixelAlpha[i];
								}
							}
						}
						if (true == FlagStartParam)
						{
							Bound = ValueStart.Bound;
							Operation = ValueStart.Operation;
						}
						else
						{
							Bound = ValueEnd.Bound;
							Operation = ValueEnd.Operation;
						}
					}
					return(this);
				}
			}
			public class ValueQuadrilateral : InterfaceInterpolatable
			{
				internal ValuePoint[] Coordinate;

				internal ValueQuadrilateral()
				{
					Coordinate = ValuePoint.CreateArray(4);
				}
				internal ValueQuadrilateral(ValueQuadrilateral Value)
				{
					Coordinate = ValuePoint.CreateArray(4);
					for(int i=0; i<Coordinate.Length; i++)
					{
						Coordinate[i] = Value.Coordinate[i];
					}
				}
				internal ValueQuadrilateral Clone()
				{
					ValueQuadrilateral Value = new ValueQuadrilateral(this);
					return(Value);
				}
				public override string ToString()
				{
					string Text = "Vertices: ";
					for(int i=0; i<Coordinate.Length; i++)
					{
						Text += "[" + i + "](" + Coordinate[i].ToString() + ") ";
					}
					return(Text);
				}
				public InterfaceInterpolatable GetInterpolated(	DataCurve Curve,
																	int TimeNow,
																	InterfaceInterpolatable Start,
																	InterfaceInterpolatable End,
																	int TimeStart,
																	int TimeEnd
																)
				{
					ValueQuadrilateral Value = new ValueQuadrilateral();
					return(Value.Interpolate(Curve, TimeNow, Start, End, TimeStart, TimeEnd));
				}
				public InterfaceInterpolatable Interpolate(	DataCurve Curve,
																int TimeNow,
																InterfaceInterpolatable Start,
																InterfaceInterpolatable End,
																int TimeStart,
																int TimeEnd
															)
				{
					ValueQuadrilateral ValueStart = (ValueQuadrilateral)Start;
					ValueQuadrilateral ValueEnd = (ValueQuadrilateral)End;
#if false
					/* MEMO: SpriteStudio Ver.5.0-5.2 */
					for(int i=0; i<Coordinate.Length; i++)
					{
						Coordinate[i].Interpolate(Curve, TimeNow, ValueStart.Coordinate[i], ValueEnd.Coordinate[i], TimeStart, TimeEnd);
					}
#else
					/* MEMO: SpriteStudio Ver.5.2- or Ver -4.x */
					float Rate = Interpolation.Interpolate<float>(Curve, TimeNow, 0.0f, 1.0f, TimeStart, TimeEnd);
					/* MEMO: VertexCorrection is not Clampped (0.0 - 1.0). */

					for(int i=0; i<Coordinate.Length; i++)
					{
						Coordinate[i].X = Interpolation.Linear(ValueStart.Coordinate[i].X, ValueEnd.Coordinate[i].X, Rate);
						Coordinate[i].Y = Interpolation.Linear(ValueStart.Coordinate[i].Y, ValueEnd.Coordinate[i].Y, Rate);
					}
#endif
					return(this);
				}
			}
			public class ValueCell
			{
				internal int TextureNo;
				internal Rect Rectangle;
				internal Vector2 Pivot;

				internal ValueCell()
				{
				}
				internal ValueCell(ValueCell Value)
				{
					TextureNo = Value.TextureNo;
					Rectangle = Value.Rectangle;
					Pivot = Value.Pivot;
				}
				internal ValueCell Clone()
				{
					ValueCell Value = new ValueCell(this);
					return(Value);
				}
				public override string ToString()
				{
					return(	"TextureNo: " + TextureNo.ToString()
							+ ", Rectangle: " + Rectangle.ToString()
							+ ", Pivot: " + Pivot.ToString()
						);
				}
			}
			public class ValueUser
			{
				internal enum FlagData
				{
					CLEAR = 0x00000000,
					NUMBER = 0x00000001,
					RECTANGLE = 0x00000002,
					COORDINATE = 0x00000004,
					TEXT = 0x00000008,
				};

				internal FlagData Flag;
				internal bool IsNumber
				{
					get
					{
						return((0 != (Flag & FlagData.NUMBER)));
					}
				}
				internal bool IsRectangle
				{
					get
					{
						return((0 != (Flag & FlagData.RECTANGLE)));
					}
				}
				internal bool IsCoordinate
				{
					get
					{
						return((0 != (Flag & FlagData.COORDINATE)));
					}
				}
				internal bool IsText
				{
					get
					{
						return((0 != (Flag & FlagData.TEXT)));
					}
				}
				internal int Number;
				internal Rect Rectangle;
				internal ValuePoint Coordinate;
				internal string Text;

				internal ValueUser()
				{
				}
				internal ValueUser(ValueUser Value)
				{
					Flag = Value.Flag;
					Number = Value.Number;
					Rectangle = Value.Rectangle;
					Coordinate = Value.Coordinate.Clone();
					Text = System.String.Copy(Value.Text);
				}
				internal ValueUser Clone()
				{
					ValueUser Value = new ValueUser(this);
					return(Value);
				}
				public override string ToString()
				{
					return(	"IsNum: " + IsNumber.ToString()
							+ ", Num: " + Number
							+ ", IsRect: " + IsRectangle.ToString()
							+ ", Rect: " + Rectangle.ToString()
							+ ", IsPoint: " + IsCoordinate.ToString()
							+ ", Point: X:" + Coordinate.ToString()
							+ ", IsString: " + IsText.ToString()
							+ ", String: " + Text
						);
				}
			}
			public class ValueSound
			{
				internal enum FlagData
				{
					CLEAR = 0x00000000,
					NOTE = 0x00000001,
					VOLUME = 0x00000002,
					USERDATA = 0x00000004,
				};

				internal FlagData Flag;
				internal uint DataSound;
				internal byte SoundId
				{
					set
					{
						DataSound = (DataSound & 0xffffff00) | (((uint)value) & 0x000000ff);
					}
					get
					{
						return((byte)(DataSound & 0x000000ff));
					}
				}
				internal byte NoteOn
				{
					set
					{
						DataSound = (DataSound & 0xffff00ff) | ((((uint)value) << 8) & 0x0000ff00);
					}
					get
					{
						return((byte)((DataSound & 0x0000ff00) >> 8));
					}
				}
				internal byte Volume
				{
					set
					{
						DataSound = (DataSound & 0xff00ffff) | ((((uint)value) << 16) & 0x00ff0000);
					}
					get
					{
						return((byte)((DataSound & 0x00ff0000) >> 16));
					}
				}
				internal byte LoopNum
				{
					set
					{
						DataSound = (DataSound & 0x00ffffff) | ((((uint)value) << 24) & 0x00ffffff);
					}
					get
					{
						return((byte)((DataSound & 0xff000000) >> 24));
					}
				}
				internal uint UserData;

				internal ValueSound()
				{
				}
				internal ValueSound(ValueSound Value)
				{
					Flag = Value.Flag;
					DataSound = Value.DataSound;
					UserData = Value.UserData;
				}
				internal ValueSound Clone()
				{
					ValueSound Value = new ValueSound(this);
					return(Value);
				}
				public override string ToString()
				{
					return(	"Flags: " + Flag
							+ ", SoundId: " + SoundId.ToString()
							+ ", NoteOn: " + NoteOn.ToString()
							+ ", Volume: " + Volume.ToString()
							+ ", LoopNum: " + LoopNum.ToString()
							+ ", UserData: " + UserData.ToString()
						);
				}
			}
			public class ValueInstance
			{
				internal enum FlagData
				{
					CLEAR = 0x00000000,

					PINGPONG = 0x00000001,
					INDEPENDENT = 0x00000002,

					REVERSE = 0x00000004,
				};

				internal FlagData Flag;
				internal bool IsPingPong
				{
					get
					{
						return((0 != (Flag & FlagData.PINGPONG)));
					}
				}
				internal bool IsIndependent
				{
					get
					{
						return((0 != (Flag & FlagData.INDEPENDENT)));
					}
				}
				internal bool IsReverse
				{
					get
					{
						return((0 != (Flag & FlagData.REVERSE)));
					}
				}

				internal int PlayCount;
				internal float RateTime;
				internal int OffsetStart;
				internal int OffsetEnd;
				internal string LabelStart;
				internal string LabelEnd;

				internal ValueInstance()
				{
				}
				internal ValueInstance(ValueInstance Value)
				{
					Flag = Value.Flag;
					PlayCount = Value.PlayCount;
					RateTime = Value.RateTime;
					OffsetStart = Value.OffsetStart;
					OffsetEnd = Value.OffsetEnd;
					LabelStart = System.String.Copy(Value.LabelStart);
					LabelEnd = System.String.Copy(Value.LabelEnd);
				}
				internal ValueInstance Clone()
				{
					ValueInstance Value = new ValueInstance(this);
					return(Value);
				}
				public override string ToString()
				{
					return(	"IsPingPong: " + IsPingPong.ToString()
							+ ", IsIndependent: " + IsIndependent.ToString()
							+ ", IsReverse: " + IsReverse.ToString()
							+ ", PlayCount: " + PlayCount.ToString()
							+ ", RateTime: " + RateTime.ToString()
							+ ", LabelStart: " + LabelStart
							+ ", OffsetStart: " + OffsetStart.ToString()
							+ ", LabelEnd: " + LabelEnd
							+ ", OffsetEnd: " + OffsetEnd.ToString()
						);
				}
			}

			public class DataBool : DataBase<bool> {}
			public class DataInt : DataBase<int> {}
			public class DataFloat : DataBase<float> {}
			public class DataPoint : DataBase<ValuePoint> {}
			public class DataColor : DataBase<ValueColor> {}
			public class DataQuadrilateral : DataBase<ValueQuadrilateral> {}
			public class DataCell : DataBase<ValueCell> {}
			public class DataUser : DataBase<ValueUser> {}
			public class DataParette : DataBase<ValuePalette> {}
			public class DataSound : DataBase<ValueSound> {}
			public class DataInstance : DataBase<ValueInstance> {}

			public static _Type DataGetIndex<_Type>(_Type[] TableKeyData, int Index)
				where _Type : InterfaceData
			{
				if(null == TableKeyData)
				{
					return(default(_Type));
				}
				if((0 > Index) || (TableKeyData.Length <= Index))
				{
					return(default(_Type));
				}
				return(TableKeyData[Index]);
			}
			public static int DataIndexGetFrame<_Type>(_Type[] TableKeyData, int FrameNo, FlagDirection Direction, int IndexTop)
				where _Type : InterfaceData
			{
				_Type KeyDataNow = default(_Type);
				int IndexPast = -1;
				int IndexFuture = -1;
				if((0 < TableKeyData.Length) && (-1 < IndexTop))
				{
					int Count = TableKeyData.Length;
					for(int i=IndexTop; i<Count; i++)
					{
						KeyDataNow = TableKeyData[i];
						if(FrameNo == KeyDataNow.Time)
						{
							if(0 != (Direction & FlagDirection.JUSTNOW))
							{
								return(i);
							}
						}
						else
						{
							if(FrameNo < KeyDataNow.Time)
							{
								IndexFuture = i;
								break;
							}
							IndexPast = i;
							IndexFuture = -1;
						}
					}
					if(0 != (Direction & FlagDirection.PAST))
					{
						if(0 == (Direction & FlagDirection.FUTURE))
						{
							return(IndexPast);
						}
					}
					if(0 != (Direction & FlagDirection.FUTURE))
					{
						if(0 != (Direction & FlagDirection.PAST))
						{
							if(0 <= IndexPast)
							{
								if(0 <= IndexFuture)
								{
									KeyDataNow = TableKeyData[IndexPast];
									int TimePast = KeyDataNow.Time;
									KeyDataNow = TableKeyData[IndexFuture];
									int TimeFuture = KeyDataNow.Time;
									return(((TimePast - FrameNo) <= (TimeFuture - FrameNo)) ? IndexPast : IndexFuture);
								}
								else
								{
									return(IndexPast);
								}
							}
							else
							{
								return(IndexFuture);
							}
						}
						else
						{
							return(IndexFuture);
						}
					}
				}
				return(-1);
			}
		}

		internal static class Interpolation
		{
			internal static float Linear(float Start, float End, float Point)
			{
				return(((End - Start) * Point) + Start);
			}

			internal static float Hermite(float Start, float SpeedStart, float End, float SpeedEnd, float Point)
			{
				float PointPow2 = Point * Point;
				float PointPow3 = PointPow2 * Point;
				return(	(((2.0f * PointPow3) - (3.0f * PointPow2) + 1.0f) * Start)
						+ (((3.0f * PointPow2) - (2.0f * PointPow3)) * End)
						+ ((PointPow3 - (2.0f * PointPow2) + Point) * (SpeedStart - Start))
						+ ((PointPow3 - PointPow2) * (SpeedEnd - End))
					);
			}

			internal static float Bezier(ref Vector2 Start, ref Vector2 VectorStart, ref Vector2 End, ref Vector2 VectorEnd, float Point)
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

			internal static float Interpolate<_Type>(	KeyFrame.DataCurve Curve,
													int TimeNow,
													_Type ValueStart,
													_Type ValueEnd,
													int TimeStart,
													int TimeEnd)
			{
				if(TimeEnd <= TimeStart)
				{
					return(Convert.ToSingle(ValueEnd));
				}
				float TimeNormalize = ((float)(TimeNow - TimeStart)) / ((float)(TimeEnd - TimeStart));
				TimeNormalize = Mathf.Clamp01(TimeNormalize);

				switch(Curve.Kind)
				{
					case KindInterpolation.NON:
						return(Convert.ToSingle(ValueStart));

					case KindInterpolation.LINEAR:
						return(Interpolation.Linear(Convert.ToSingle(ValueStart), Convert.ToSingle(ValueEnd), TimeNormalize));

					case KindInterpolation.HERMITE:
						return(Interpolation.Hermite(Convert.ToSingle(ValueStart), Curve.ValueStart, Convert.ToSingle(ValueEnd), Curve.ValueEnd, TimeNormalize));

					case KindInterpolation.BEZIER:
						{
							Vector2 Start = new Vector2((float)TimeStart, Convert.ToSingle(ValueStart));
							Vector2 VectorStart = new Vector2(Curve.TimeStart, Curve.ValueStart);
							Vector2 End = new Vector2((float)TimeEnd, Convert.ToSingle(ValueEnd));
							Vector2 VectorEnd = new Vector2(Curve.TimeEnd, Curve.ValueEnd);
							return(Interpolation.Bezier(ref Start, ref VectorStart, ref End, ref VectorEnd, TimeNormalize));
						}

					case KindInterpolation.ACCELERATE:
						return(Interpolation.Accelerate(Convert.ToSingle(ValueStart), Convert.ToSingle(ValueEnd), TimeNormalize));

					case KindInterpolation.DECELERATE:
						return(Interpolation.Decelerate(Convert.ToSingle(ValueStart), Convert.ToSingle(ValueEnd), TimeNormalize));

					default:
						break;
				}
				return(Convert.ToSingle(ValueStart));
			}
		}
	}

	/* Utilities Parsing XML */
	internal static partial class XMLUtility
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

		internal static bool TextToBool(string Text)
		{
			bool ret = false;
			try {
				ret = System.Convert.ToBoolean(Text);
			}catch(System.FormatException){
				int i = System.Convert.ToInt32(Text);
				ret = ((0 == i) ? (false) : (true));
			}
			return(ret);
		}

		internal static int TextHexToInt(string Text)
		{
			return(System.Convert.ToInt32(Text, 16));
		}

		internal static uint TextHexToUInt(string Text)
		{
			return(System.Convert.ToUInt32(Text, 16));
		}

		internal static XmlNode XML_SelectSingleNode(XmlNode Node, string NamePath, XmlNamespaceManager Manager)
		{
			return(Node.SelectSingleNode(NamePath, Manager));
		}

		internal static string TextGetSelectSingleNode(XmlNode Node, string NamePath, XmlNamespaceManager Manager)
		{
			XmlNode NodeNow = XML_SelectSingleNode(Node, NamePath, Manager);
			return((null != NodeNow) ? NodeNow.InnerText : null);
		}

		internal static XmlNodeList XML_SelectNodes(XmlNode Node, string NamePath, XmlNamespaceManager Manager)
		{
			return(Node.SelectNodes(NamePath, Manager));
		}

		internal static int VersionGetHexCode(string Text)
		{
			string[] Item = Text.Split('.');
			if (3 != Item.Length)
			{
				return(-1);
			}

			int VersionMajor = TextHexToInt(Item[0]);
			int VersionMinor = TextHexToInt(Item[1]);
			int Revision = TextHexToInt(Item[2]);
			return((VersionMajor << 16) | (VersionMinor << 8) | Revision);
		}

		internal static string VersionGetString(int VersionCode)
		{
			int VersionMajor = (VersionCode >> 16) & 0xff;
			if (0 == VersionMajor)
			{
				return(null);
			}
			int VersionMinor = (VersionCode >> 8) & 0xff;
			int Revision = (VersionCode & 0xff);
			return(System.String.Format("{0:X}.{1:X2}.{2:X2}", VersionMajor, VersionMinor, Revision));
		}
	}
}

