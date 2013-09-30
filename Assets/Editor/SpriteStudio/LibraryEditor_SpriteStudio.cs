/**
	SpriteStudio5 Player for Unity

	Copyright(C) 2003-2013 Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public static partial class LibraryEditor_SpriteStudio
{
	private readonly static int ShaderOperationMax = (int)Library_SpriteStudio.KindColorOperation.TERMINATOR - 1;
	private readonly static Shader[] Shader_SpriteStudioTriangleX = new Shader[(int)Library_SpriteStudio.KindColorOperation.TERMINATOR - 1]
	{
		Shader.Find("Custom/SpriteStudio5/Mix"),
		Shader.Find("Custom/SpriteStudio5/Add"),
		Shader.Find("Custom/SpriteStudio5/Sub"),
		Shader.Find("Custom/SpriteStudio5/Mul")
	};

	private readonly static Library_SpriteStudio.KindAttributeKey[][] MaskKeyAttoribute_OPSS =
	{
		new Library_SpriteStudio.KindAttributeKey[]
		{
			Library_SpriteStudio.KindAttributeKey.TERMINATOR
		},
		new Library_SpriteStudio.KindAttributeKey[]
		{
			Library_SpriteStudio.KindAttributeKey.PRIORITY,
			Library_SpriteStudio.KindAttributeKey.VERTEX_CORRECTION,
			Library_SpriteStudio.KindAttributeKey.ORIGIN_OFFSET_X,
			Library_SpriteStudio.KindAttributeKey.ORIGIN_OFFSET_Y,
			Library_SpriteStudio.KindAttributeKey.TEXTURE_TRANSLATE_X,
			Library_SpriteStudio.KindAttributeKey.TEXTURE_TRANSLATE_Y,
			Library_SpriteStudio.KindAttributeKey.TEXTURE_ROTATE,
			Library_SpriteStudio.KindAttributeKey.TEXTURE_SCALE_X,
			Library_SpriteStudio.KindAttributeKey.TEXTURE_SCALE_Y,
			Library_SpriteStudio.KindAttributeKey.PALETTE_CHANGE,
			Library_SpriteStudio.KindAttributeKey.SOUND,

			Library_SpriteStudio.KindAttributeKey.TERMINATOR
		},
		new Library_SpriteStudio.KindAttributeKey[]
		{
			Library_SpriteStudio.KindAttributeKey.PRIORITY,
			Library_SpriteStudio.KindAttributeKey.SOUND,

			Library_SpriteStudio.KindAttributeKey.TERMINATOR
		},
		new Library_SpriteStudio.KindAttributeKey[]
		{
			Library_SpriteStudio.KindAttributeKey.TERMINATOR
		},
		new Library_SpriteStudio.KindAttributeKey[]
		{
			Library_SpriteStudio.KindAttributeKey.TERMINATOR
		}
	};

	public struct SettingImport
	{
		public int TextureSizePixelMaximum;
		public float CollisionThicknessZ;
		public bool FlagAttachRigidBody;
	}

	public static class DataIntermediate
	{
		public class InformationCell
		{
			public Rect Area;
			public Vector2 Pivot;
			public float Rotate;

			public void CleanUp()
			{
				Area.x = 0.0f;
				Area.y = 0.0f;
				Area.width = 0.0f;
				Area.height = 0.0f;
				Pivot = Vector2.zero;
				Rotate = 0.0f;
			}
		}
		public struct PartsImage
		{
			public string FileName;
			public Hashtable CellArea;

			public void CleanUp()
			{
				FileName = "";
				CellArea = null;
			}
		}

		public class AnimationDataEditor : Library_SpriteStudio.AnimationDataBasis
		{
			public float[] RateInheritance;
			public ArrayList[] DataKeyFrame;

			public void CleanUp()
			{
				Inheritance = Library_SpriteStudio.KindInheritance.PARENT;

				FlagInheritance = Library_SpriteStudio.FlagAttributeKeyInherit.CLEAR;
				RateInheritance = null;

				FlagKeyParameter = Library_SpriteStudio.FlagAttributeKeyInherit.CLEAR;
				DataKeyFrame = null;
			}
		}

		public struct PartsSprite
		{
			public struct ParamaterInheritance
			{
				public bool Use;
				public float Rate;

				public void CleanUp()
				{
					Use = false;
					Rate = 0.0f;
				}

				public override string ToString()
				{
					return("Use: " + Use + ", Rate: " + Rate);
				}
			}

			public Library_SpriteStudio.KindParts PartsKind;
			public Library_SpriteStudio.KindParts ObjectKind;
			public Library_SpriteStudio.KindCollision CollisionKind;
			public int ID;
			public int IDParent;

			public string Name;
			public Library_SpriteStudio.KindColorOperation KindBlendTarget;

			public AnimationDataEditor DataAnimation;

			public void CleanUp()
			{
				PartsKind = Library_SpriteStudio.KindParts.NORMAL;
				ObjectKind = Library_SpriteStudio.KindParts.TERMINATOR;
				CollisionKind = Library_SpriteStudio.KindCollision.NON;
				ID = -1;
				IDParent = -1;

				Name = "";
				KindBlendTarget = Library_SpriteStudio.KindColorOperation.MIX;

				DataAnimation = null;
			}

			public void BootUp()
			{
				DataAnimation = new AnimationDataEditor();
			}
		}

		public class TrunkParts
		{
			public PartsSprite[] ListParts = null;
			public PartsImage[] ListImage = null;
			public Library_SpriteStudio.AnimationInformationPlay[] ListInformationPlay = null;
			public int CountNode = -1;
			public bool FlameFlipForImageOnly = false;
		}
	}

	public static partial class Menu
	{
		public static void ImportSSPJ(SettingImport DataSettingImport)
		{
			string NameDirectory = "";
			string NameFileBody = "";
			string NameFileExtension = "";
			if(false == File.FileNameGetFileDialog(	out NameDirectory,
													out NameFileBody,
													out NameFileExtension,
													"Select Importing SSPJ-File",
													"sspj"
													)
				)
			{
				return;
			}

			int StepNow = 0;
			int StepFull = 0;
			ProgressBarUpdate("Decoding Project Files", 0, 1);

			ParseOPSS.InformationSSPJ InformationSSPJ = ParseOPSS.ImportSSPJ(NameDirectory, NameFileBody + NameFileExtension);
			StepFull = 1 + (InformationSSPJ.ListSSCE.Count + InformationSSPJ.ListSSAE.Count) * 2;

			int Count = InformationSSPJ.ListSSCE.Count;
			DataIntermediate.PartsImage[] DataListImage = new DataIntermediate.PartsImage[Count];
			for(int i=0; i<Count; i++)
			{
				StepNow++;
				ProgressBarUpdate(	"Decoding SSCE Files: " + i.ToString() + "-" + Count.ToString(),
									StepNow,
									StepFull
								);

				DataListImage[i] = new DataIntermediate.PartsImage();
				DataListImage[i].CleanUp();
				if(false == ParseOPSS.ImportSSCE(ref DataListImage[i], InformationSSPJ.NameDirectorySSCE, InformationSSPJ.NameDirectoryImage, (string)InformationSSPJ.ListSSCE[i]))
				{
					return;
				}
			}

			Count = InformationSSPJ.ListSSAE.Count;
			DataIntermediate.TrunkParts[] DataOutput = new DataIntermediate.TrunkParts[Count];
			for(int i=0; i<Count; i++)
			{
				StepNow++;
				ProgressBarUpdate(	"Decoding SSAE Files: " + i.ToString() + "-" + Count.ToString(),
									StepNow,
									StepFull
								);

				DataOutput[i] = new DataIntermediate.TrunkParts();
				DataOutput[i].ListImage = DataListImage;

				if(false == ParseOPSS.ImportSSAE(ref DataOutput[i], InformationSSPJ.NameDirectorySSAE, (string)InformationSSPJ.ListSSAE[i], InformationSSPJ))
				{
					return;
				}
			}

			string NamePathBase = AssetUtility.NamePathGetSelectNow(null);

			StepNow++;
			ProgressBarUpdate(	"Creating Materials",
								StepNow,
								StepFull
							);
			Material[] TableMaterial = Create.DataMaterial(	NameFileBody,
															NamePathBase,
															DataOutput[0],
															ref DataSettingImport
														);
			StepNow += InformationSSPJ.ListSSCE.Count;

			string FileNameBodySSAE = "";
			for(int i=0; i<Count; i++)
			{
				StepNow++;
				ProgressBarUpdate(	"Creating Animation Prefabs: " + i.ToString() + "-" + Count.ToString(),
									StepNow,
									StepFull
								);

				FileNameBodySSAE = Path.GetFileNameWithoutExtension((string)InformationSSPJ.ListSSAE[i]);

				Object DataPrefab = Create.DataPrefabSprite(	FileNameBodySSAE,
																NamePathBase,
																DataOutput[i],
																TableMaterial,
																ref DataSettingImport,
																MaskKeyAttoribute_OPSS,
																Library_SpriteStudio.SpriteData.BitStatus.PIVOTPLANE_UV | Library_SpriteStudio.SpriteData.BitStatus.TEXTURETRANSLATE_UV
															);
				if(null == DataPrefab)
				{
					Debug.LogError("SSPJ-Import: Failure Creating Prefab :" + FileNameBodySSAE);
				}
			}

			ProgressBarUpdate(	"Import End", -1, -1);
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
	}

	public static class File
	{
		private static string DirectoryPrevious = "";
		private readonly static string NamePathRootFile = Application.dataPath;

		public static bool FileNameGetFileDialog(out string NameDirectory, out string NameFileBody, out string NameFileExtension, string TitleDialog, string FilterExtension)
		{
			string FileNameFullPath = EditorUtility.OpenFilePanel(	TitleDialog,
																	DirectoryPrevious,
																	FilterExtension
																);
			if(0 == FileNameFullPath.Length)
			{
				NameDirectory = "";
				NameFileBody = "";
				NameFileExtension = "";

				return(false);
			}

			NameDirectory = Path.GetDirectoryName(FileNameFullPath);
			NameFileBody = Path.GetFileNameWithoutExtension(FileNameFullPath);
			NameFileExtension = Path.GetExtension(FileNameFullPath);

			DirectoryPrevious = NameDirectory;

			return(true);
		}

		public static string NamePathToAsset(string Name)
		{
			string NamePath = System.String.Copy(NamePathRootFile);
			if(null != Name)
			{
				NamePath += "/" + Name.Substring(AssetUtility.NamePathRootAsset.Length + 1);
			}

			return(NamePath);
		}

		public static bool FileCopyToAsset(string NameAsset, string NameOriginalFileName, bool FlagOverCopy)
		{
			System.IO.File.Copy(NameOriginalFileName, NameAsset, true);

			return(true);
		}
	}

	public static class Cursor
	{
		public static Object ObjectGetOnCursor()
		{
			return(Selection.activeObject);
		}
	}

	public static class ParseOPSS
	{
		public enum KindVersionSSPJ
		{
			ERROR = 0x00000000,
			VERSION_000100  = 0x00000100,
			VERSION_010000  = 0x00010000,

			VERSION_REQUIRED = VERSION_000100,
			VERSION_CURRENT = VERSION_010000,
		};
		public enum KindVersionSSCE
		{
			ERROR = 0x00000000,
			VERSION_000100  = 0x00000100,
			VERSION_010000  = 0x00010000,

			VERSION_REQUIRED = VERSION_000100,
			VERSION_CURRENT = VERSION_010000,
		};
		public enum KindVersionSSAE
		{
			ERROR = 0x00000000,
			VERSION_000100  = 0x00000100,
			VERSION_010000  = 0x00010000,

			VERSION_REQUIRED = VERSION_000100,
			VERSION_CURRENT = VERSION_010000,
		};

		public class InformationSSPJ
		{
			public KindVersionSSPJ VersionCode;
			public ArrayList ListSSCE;
			public ArrayList ListSSAE;
			public string NameDirectorySSCE;
			public string NameDirectorySSAE;
			public string NameDirectoryImage;

			public void CleanUp()
			{
				VersionCode = LibraryEditor_SpriteStudio.ParseOPSS.KindVersionSSPJ.ERROR;
				ListSSCE = null;
				ListSSAE = null;
				NameDirectorySSCE = "";
				NameDirectorySSAE = "";
				NameDirectoryImage = "";
			}

			public void AddSSCE(string FileName)
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

			public void AddSSAE(string FileName)
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

			public int ArraySearchFileName(ArrayList ListFileName, string FileName)
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

			public int ArraySearchFileNameBody(ArrayList ListFileName, string FileName)
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
		}
		public static class ManagerDescriptionAttribute
		{
			public class DescriptionAttribute
			{
				public Library_SpriteStudio.KindAttributeKey Attribute
				{
					set;
					get;
				}
				public Library_SpriteStudio.KindValueKey KindValue
				{
					set;
					get;
				}
				public Library_SpriteStudio.KindTypeKey KindType
				{
					set;
					get;
				}
				public bool Interpolatable
				{
					get
					{
						switch(KindValue)
						{
							case Library_SpriteStudio.KindValueKey.CHECK:
							case Library_SpriteStudio.KindValueKey.USER:
							case Library_SpriteStudio.KindValueKey.SOUND:
								return(false);
						}
						return(true);
					}
				}

				public DescriptionAttribute(Library_SpriteStudio.KindAttributeKey AttributeNew,
											Library_SpriteStudio.KindValueKey KindValueNew,
											Library_SpriteStudio.KindTypeKey KindTypeNew
										)
				{
					Attribute = AttributeNew;
					KindValue = KindValueNew;
					KindType = KindTypeNew;
				}
			}

			private readonly static Dictionary<string, DescriptionAttribute> ListDescriptionSSAE = new Dictionary<string, DescriptionAttribute>
			{
				{"POSX",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.POSITION_X,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.FLOAT
													)
				},
				{"POSY",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.POSITION_Y,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.FLOAT
													)
				},
				{"POSZ",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.POSITION_Z,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.FLOAT
													)
				},

				{"ROTX",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.ROTATE_X,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.DEGREE
													)
				},
				{"ROTY",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.ROTATE_Y,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.DEGREE
													)
				},
				{"ROTZ",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.ROTATE_Z,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.DEGREE
													)
				},

				{"SCLX",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.SCALE_X,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.FLOAT
													)
				},
				{"SCLY",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.SCALE_Y,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.FLOAT
													)
				},

				{"ALPH",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.OPACITY_RATE,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.FLOAT
													)
				},

				{"PRIO",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.PRIORITY,
													 	Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.FLOAT
													)
				},

				{"FLPH",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.FLIP_X,
														Library_SpriteStudio.KindValueKey.CHECK,
														Library_SpriteStudio.KindTypeKey.BOOL
													)
				},
				{"FLPV",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.FLIP_Y,
														Library_SpriteStudio.KindValueKey.CHECK,
														Library_SpriteStudio.KindTypeKey.BOOL
													)
				},
				{"HIDE",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.SHOW_HIDE,
														Library_SpriteStudio.KindValueKey.CHECK,
														Library_SpriteStudio.KindTypeKey.BOOL
													)
				},

				{"VCOL",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.COLOR_BLEND,
														Library_SpriteStudio.KindValueKey.COLOR,
														Library_SpriteStudio.KindTypeKey.OTHER
													)
				},
				{"VERT",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.VERTEX_CORRECTION,
														Library_SpriteStudio.KindValueKey.QUADRILATERRAL,
														Library_SpriteStudio.KindTypeKey.OTHER
													)
				},

				{"PVTX",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.ORIGIN_OFFSET_X,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.FLOAT
													)
				},
				{"PVTY",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.ORIGIN_OFFSET_Y,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.FLOAT
													)
				},

				{"ANCX",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.ANCHOR_POSITION_X,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.FLOAT
													)
				},
				{"ANCY",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.ANCHOR_POSITION_Y,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.FLOAT
													)
				},
				{"SIZX",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.ANCHOR_SIZE_X,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.FLOAT
													)
				},
				{"SIZY",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.ANCHOR_SIZE_Y,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.FLOAT
													)
				},

				{"UVTX",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.TEXTURE_TRANSLATE_X,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.FLOAT
													)
				},
				{"UVTY",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.TEXTURE_TRANSLATE_Y,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.FLOAT
													)
				},
				{"UVRZ",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.TEXTURE_ROTATE,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.DEGREE
													)
				},
				{"UVSX",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.TEXTURE_EXPAND_WIDTH,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.FLOAT
													)
				},
				{"UVSY",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.TEXTURE_EXPAND_HEIGHT,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.FLOAT
													)
				},
				{"BNDR",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.COLLISION_RADIUS,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.FLOAT
													)
				},

				{"CELL",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.CELL,
														Library_SpriteStudio.KindValueKey.CELL,
														Library_SpriteStudio.KindTypeKey.OTHER
													)
				},
				{"USER",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.USER_DATA,
														Library_SpriteStudio.KindValueKey.USER,
														Library_SpriteStudio.KindTypeKey.OTHER
													)
				},

				{"IFLH",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.TEXTURE_FLIP_X,
														Library_SpriteStudio.KindValueKey.CHECK,
														Library_SpriteStudio.KindTypeKey.BOOL
													)
				},
				{"IFLV",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.TEXTURE_FLIP_Y,
														Library_SpriteStudio.KindValueKey.CHECK,
														Library_SpriteStudio.KindTypeKey.BOOL
													)
				},

				{"IMGX",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.TEXTURE_TRANSLATE_X,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.FLOAT
													)
				},
				{"IMGY",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.TEXTURE_TRANSLATE_Y,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.FLOAT
													)
				},
				{"IMGW",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.TEXTURE_EXPAND_WIDTH,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.FLOAT
													)
				},
				{"IMGH",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.TEXTURE_EXPAND_HEIGHT,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.FLOAT
													)
				},

				{"ORFX",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.ORIGIN_OFFSET_X,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.FLOAT
													)
				},
				{"ORFY",	new DescriptionAttribute(	Library_SpriteStudio.KindAttributeKey.ORIGIN_OFFSET_Y,
														Library_SpriteStudio.KindValueKey.NUMBER,
														Library_SpriteStudio.KindTypeKey.FLOAT
													)
				},
			};

			static public DescriptionAttribute DescriptionAttributeGetTagName(string NameTag)
			{
				DescriptionAttribute Description;
				return((true == ListDescriptionSSAE.TryGetValue(NameTag, out Description)) ? Description : null);
			}

			static public DescriptionAttribute DescriptionAttributeGetKey(Library_SpriteStudio.KindAttributeKey Key)
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

		public static InformationSSPJ ImportSSPJ(string DataPathBase, string FileName)
		{
			XmlDocument DataXML = new XmlDocument();
			DataXML.Load(DataPathBase + "/" + FileName);

			InformationSSPJ InformationProject = null;
			XmlNode NodeRoot = DataXML.FirstChild;
			NodeRoot = NodeRoot.NextSibling;
			KindVersionSSPJ VersionCode = (KindVersionSSPJ)(VersionCodeGet(NodeRoot, "SpriteStudioProject", (int)KindVersionSSPJ.ERROR));
			switch(VersionCode)
			{
				case KindVersionSSPJ.VERSION_000100:
				case KindVersionSSPJ.VERSION_010000:
					break;

				case KindVersionSSPJ.ERROR:
				default:
					Debug.LogError("SSCE-Import: Aborted.");
					return(null);
			}

			InformationProject = new InformationSSPJ();
			if(null == InformationProject)
			{
				Debug.LogError("SSPJ-Import: Error!: Not Enough Memory");
				return(null);
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

			NodeList = XMLUtility.XML_SelectNodes(NodeRoot, "cellmapNames/value", ManagerNameSpace);
			if(null == NodeList)
			{
				Debug.LogError("SSPJ-Import: Error!: CellMapNameList-Node Not-Found");
				return(null);
			}
			foreach(XmlNode NodeNameCellMap in NodeList)
			{
				string NameFileName = NodeNameCellMap.InnerText;
				InformationProject.AddSSCE(NameFileName);
			}

			NodeList = XMLUtility.XML_SelectNodes(NodeRoot, "animepackNames/value", ManagerNameSpace);
			if(null == NodeList)
			{
				Debug.LogError("SSPJ-Import: Error!: AnimePackNameList-Node Not-Found");
				return(null);
			}
			foreach(XmlNode NodeNameAnimation in NodeList)
			{
				string NameFileName = NodeNameAnimation.InnerText;
				InformationProject.AddSSAE(NameFileName);
			}

			return(InformationProject);
		}

		public static bool ImportSSCE(	ref DataIntermediate.PartsImage InformationCellMap,
										string NameDirectoryCellMap,
										string NameDirectoryImage,
										string FileName
									)
		{
			XmlDocument DataXML = new XmlDocument();
			DataXML.Load(NameDirectoryCellMap + "/" + FileName);

			XmlNode NodeRoot = DataXML.FirstChild;
			NodeRoot = NodeRoot.NextSibling;
			KindVersionSSCE VersionCode = (KindVersionSSCE)(VersionCodeGet(NodeRoot, "SpriteStudioCellMap", (int)KindVersionSSCE.ERROR));
			switch(VersionCode)
			{
				case KindVersionSSCE.VERSION_000100:
				case KindVersionSSCE.VERSION_010000:
					break;

				case KindVersionSSCE.ERROR:
				default:
					Debug.LogError("SSCE-Import: Aborted.");
					return(false);
			}

			NameTable NodeNameSpace = new NameTable();
			XmlNamespaceManager ManagerNameSpace = new XmlNamespaceManager(NodeNameSpace);
			XmlNodeList NodeList = null;

			string NameTexture = XMLUtility.TextGetSelectSingleNode(NodeRoot, "imagePath", ManagerNameSpace);
			if(0 == string.Compare(NameDirectoryCellMap, string.Empty))
			{
				NameDirectoryCellMap = Path.GetDirectoryName(FileName);
			}
			string NameFileBody = Path.GetFileNameWithoutExtension(NameTexture);
			string NameFileExtension = Path.GetExtension(NameTexture);
			InformationCellMap.FileName = NameDirectoryImage + "/" + NameFileBody + NameFileExtension;

			InformationCellMap.CellArea = new Hashtable();
			if(null == InformationCellMap.CellArea)
			{
				Debug.LogError("SSCE-Import: Error!: Not Enough Memory");
				return(false);
			}
			DataIntermediate.InformationCell Cell = null;

			NodeList = XMLUtility.XML_SelectNodes(NodeRoot, "cells/cell", ManagerNameSpace);
			if(null == NodeList)
			{
				Debug.LogError("SSCE-Import: Error!: Cells-Node Not-Found");
				return(false);
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
					Debug.LogError("SSCE-Import: Error!: Not Enough Memory");
					return(false);
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
		}

		public static bool ImportSSAE(ref DataIntermediate.TrunkParts DataTrunk, string NameDirectory, string FileName, InformationSSPJ InformationProject)
		{
			XmlDocument DataXML = new XmlDocument();
			DataXML.Load(NameDirectory + "/" + FileName);

			XmlNode NodeRoot = DataXML.FirstChild;
			NodeRoot = NodeRoot.NextSibling;
			KindVersionSSAE VersionCode = (KindVersionSSAE)(VersionCodeGet(NodeRoot, "SpriteStudioAnimePack", (int)KindVersionSSAE.ERROR));
			switch(VersionCode)
			{
				case KindVersionSSAE.VERSION_000100:
				case KindVersionSSAE.VERSION_010000:
					break;

				case KindVersionSSAE.ERROR:
				default:
					Debug.LogError("SSAE-Import: Aborted.");
					return(false);
			}

			NameTable NodeNameSpace = new NameTable();
			XmlNamespaceManager ManagerNameSpace = new XmlNamespaceManager(NodeNameSpace);
			XmlNodeList NodeList = null;

			NodeList = XMLUtility.XML_SelectNodes(NodeRoot, "cellmapNames/value", ManagerNameSpace);
			if(null == NodeList)
			{
				Debug.LogError("SSAE-Import: Error!: CellMapNames-Node Not-Found");
				return(false);
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
					Debug.LogWarning("SSAE-Import: Error!: CellMap Not-Found: " + NodeCellMapName.Value);
					return(false);
				}
				CellNo++;
			}

			NodeList = XMLUtility.XML_SelectNodes(NodeRoot, "Model/partList/value", ManagerNameSpace);
			if(null == NodeList)
			{
				Debug.LogError("SSAE-Import: Error!: PartsList-Node Not-Found");
				return(false);
			}

			DataTrunk.CountNode = NodeList.Count;
			DataTrunk.ListParts = new DataIntermediate.PartsSprite[DataTrunk.CountNode];
			if(null == DataTrunk.ListParts)
			{
				Debug.LogError("SSAE-Import: Error!: Not Enough Memory");
				return(false);
			}
			for(int i=0; i<DataTrunk.CountNode; i++)
			{
				DataTrunk.ListParts[i].BootUp();
				if(null == DataTrunk.ListParts[i].DataAnimation)
				{
					Debug.LogError("SSAE-Import: Error!: Not Enough Memory");
					return(false);
				}

				DataTrunk.ListParts[i].DataAnimation.DataKeyFrame = new ArrayList[(int)Library_SpriteStudio.KindAttributeKey.TERMINATOR];
				if(null == DataTrunk.ListParts[i].DataAnimation.DataKeyFrame)
				{
					Debug.LogError("SSAE-Import: Error!: Not Enough Memory");
					return(false);
				}
				for(int j=(int)Library_SpriteStudio.KindAttributeKey.POSITION_X; j<(int)Library_SpriteStudio.KindAttributeKey.TERMINATOR; j++)
				{
					DataTrunk.ListParts[i].DataAnimation.DataKeyFrame[j] = null;
				}

				DataTrunk.ListParts[i].DataAnimation.RateInheritance = new float[(int)Library_SpriteStudio.KindAttributeKey.TERMINATOR_INHELIT];
				if(null == DataTrunk.ListParts[i].DataAnimation.RateInheritance)
				{
					Debug.LogError("SSAE-Import: Error!: Not Enough Memory");
					return(false);
				}
				for(int j=(int)Library_SpriteStudio.KindAttributeKey.POSITION_X; j<(int)Library_SpriteStudio.KindAttributeKey.TERMINATOR_INHELIT; j++)
				{
					DataTrunk.ListParts[i].DataAnimation.RateInheritance[j] = 0.0f;
				}
			}

			int PartsNo = -1;
			foreach(XmlNode NodeParts in NodeList)
			{
				PartsNo = XMLUtility.ValueGetInt(XMLUtility.TextGetSelectSingleNode(NodeParts, "arrayIndex", ManagerNameSpace));
				DataTrunk.ListParts[PartsNo].ID = PartsNo;

				if(false == ImportSSAESetParts(ref DataTrunk.ListParts[PartsNo], NodeParts, ManagerNameSpace))
				{
					Debug.LogError("SSAE-Import: Aborted.");
					return(false);
				}
			}

			for(int i=0; i<DataTrunk.CountNode; i++)
			{
				if(Library_SpriteStudio.KindInheritance.PARENT == DataTrunk.ListParts[i].DataAnimation.Inheritance)
				{
					PartsNo = DataTrunk.ListParts[i].IDParent;
					while(Library_SpriteStudio.KindInheritance.PARENT == DataTrunk.ListParts[PartsNo].DataAnimation.Inheritance)
					{
						PartsNo = DataTrunk.ListParts[PartsNo].IDParent;
					}
					DataTrunk.ListParts[i].DataAnimation.FlagInheritance = DataTrunk.ListParts[PartsNo].DataAnimation.FlagInheritance;
				}

				for(int j=(int)Library_SpriteStudio.KindAttributeKey.POSITION_X; j<(int)Library_SpriteStudio.KindAttributeKey.TERMINATOR_INHELIT; j++)
				{
					DataTrunk.ListParts[i].DataAnimation.RateInheritance[j] = (0 != (DataTrunk.ListParts[i].DataAnimation.FlagInheritance & Library_SpriteStudio.FlagParameterKeyFrameInherit[j])) ? 1.0f : 0.0f;
				}
			}

			NodeList = XMLUtility.XML_SelectNodes(NodeRoot, "animeList/anime", ManagerNameSpace);
			if(null == NodeList)
			{
				Debug.LogError("SSAE-Import: Error!: AnimationList-Node Not-Found");
				return(false);
			}
			int CountAnimation = NodeList.Count;
			DataTrunk.ListInformationPlay = new Library_SpriteStudio.AnimationInformationPlay[CountAnimation];
			for(int i=0; i<CountAnimation; i++)
			{
				DataTrunk.ListInformationPlay[i] = new Library_SpriteStudio.AnimationInformationPlay();
			}

			CountAnimation = 0;
			int FrameNoStart = 0;
			foreach(XmlNode NodeAnimation in NodeList)
			{
				DataTrunk.ListInformationPlay[CountAnimation].Name = string.Copy(XMLUtility.TextGetSelectSingleNode(NodeAnimation, "name", ManagerNameSpace));
				DataTrunk.ListInformationPlay[CountAnimation].FrameStart = FrameNoStart;
				DataTrunk.ListInformationPlay[CountAnimation].FrameEnd = XMLUtility.ValueGetInt(XMLUtility.TextGetSelectSingleNode(NodeAnimation, "settings/frameCount", ManagerNameSpace));
				DataTrunk.ListInformationPlay[CountAnimation].FrameEnd += FrameNoStart;
				DataTrunk.ListInformationPlay[CountAnimation].FrameEnd--;
				DataTrunk.ListInformationPlay[CountAnimation].FramePerSecond = XMLUtility.ValueGetInt(XMLUtility.TextGetSelectSingleNode(NodeAnimation, "settings/fps", ManagerNameSpace));

				if(false == ImportSSAESetAnimation(ref DataTrunk, CountAnimation, FrameNoStart, CellMapNo, NodeAnimation, ManagerNameSpace))
				{
					Debug.LogError("SSAE-Import: Aborted.");
					return(false);
				}

				FrameNoStart = ((DataTrunk.ListInformationPlay[CountAnimation].FrameEnd / 10) + 1) * 10;
				CountAnimation++;
			}

			return(true);
		}

		private	static int VersionCodeGet(XmlNode NodeRoot, string NameTag, int ErrorCode)
		{
			XmlAttributeCollection AttributeNodeRoot = NodeRoot.Attributes;
			if(NameTag != NodeRoot.Name)
			{
				Debug.LogError(	"SSxx-Import: Error!: Invalid Root-Tag: "
								+ NodeRoot.Name
							);
				return(ErrorCode);
			}

			XmlNode NodeVersion = AttributeNodeRoot["version"];
			string VersionText = NodeVersion.Value;
			int Version = XMLUtility.VersionGetHexCode(VersionText);
			if(-1 == Version)
			{
				Debug.LogError(	"SSxx-Import: Error!: Version-Invalid = " + VersionText);
				return(ErrorCode);
			}

			Version &= ~0x000000ff;
			return(Version);
		}

		private static bool ImportSSAESetParts(	ref DataIntermediate.PartsSprite DataParts,
												XmlNode NodeParts,
												XmlNamespaceManager ManagerNameSpace
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

				default:
					Debug.LogWarning("SSAE-Import: Warning: Parts["
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
					Debug.LogWarning("SSAE-Import: Warning: Parts["
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
					if(0 == DataParts.ID)
					{
						DataParts.DataAnimation.Inheritance = Library_SpriteStudio.KindInheritance.SELF;
						DataParts.DataAnimation.FlagInheritance = Library_SpriteStudio.FlagAttributeKeyInherit.PRESET;
					}
					else
					{
						DataParts.DataAnimation.Inheritance = Library_SpriteStudio.KindInheritance.PARENT;
						DataParts.DataAnimation.FlagInheritance = Library_SpriteStudio.FlagAttributeKeyInherit.CLEAR;
					}
					break;

				case "self":
					{
						DataParts.DataAnimation.Inheritance = Library_SpriteStudio.KindInheritance.SELF;
						DataParts.DataAnimation.FlagInheritance = Library_SpriteStudio.FlagAttributeKeyInherit.PRESET;
						DataParts.DataAnimation.FlagInheritance &= ~Library_SpriteStudio.FlagAttributeKeyInherit.OPACITY_RATE;

						XmlNode NodeAttribute = null;
						NodeAttribute = XMLUtility.XML_SelectSingleNode(NodeParts, "ineheriteRates/ALPH", ManagerNameSpace);
						if(null == NodeAttribute)
						{
							DataParts.DataAnimation.FlagInheritance |= Library_SpriteStudio.FlagAttributeKeyInherit.OPACITY_RATE;
						}

						NodeAttribute = XMLUtility.XML_SelectSingleNode(NodeParts, "ineheriteRates/FLPH", ManagerNameSpace);
						if(null == NodeAttribute)
						{
							DataParts.DataAnimation.FlagInheritance |= Library_SpriteStudio.FlagAttributeKeyInherit.FLIP_X;
						}

						NodeAttribute = XMLUtility.XML_SelectSingleNode(NodeParts, "ineheriteRates/FLPV", ManagerNameSpace);
						if(null == NodeAttribute)
						{
							DataParts.DataAnimation.FlagInheritance |= Library_SpriteStudio.FlagAttributeKeyInherit.FLIP_Y;
						}

						NodeAttribute = XMLUtility.XML_SelectSingleNode(NodeParts, "ineheriteRates/HIDE", ManagerNameSpace);
						if(null == NodeAttribute)
						{
							DataParts.DataAnimation.FlagInheritance |= Library_SpriteStudio.FlagAttributeKeyInherit.SHOW_HIDE;
						}
					}
					break;

				default:
					Debug.LogWarning("SSAE-Import: Warning: Parts["
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
					Debug.LogWarning("SSAE-Import: Warning: Parts["
										+ DataParts.ID.ToString()
										+ "] Invalid Alpha-Blend Kind.: "
										+ ValueText
									);
					goto case "mix";
			}

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
				Debug.LogError(	"SSAE-Import: Error!: Animation["
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
					Debug.LogWarning(	"SSAE-Import: Warning: Animation["
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

		private static int ImportSSAEPartsSearchNameToID(	DataIntermediate.TrunkParts DataTrunk,
															string Name
														)
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
				Debug.LogError(	"SSAE-Import: Error!: Animation["
								+ DataParts.Name
								+ "] KeyData Not-Found."
							);
				return(false);
			}

			ManagerDescriptionAttribute.DescriptionAttribute Description = null;
			XmlNodeList NodeListKey = null;
			XmlNode NodeInterpolation = null;
			bool FlagWithParameterInterpolation = false;
			bool FlagEnableInterpolation = false;
			int AttributeNo = -1;
			int KeyNo = -1;
			string ValueText = "";
			string[] ValueTextParameter = null;
			Library_SpriteStudio.KeyFrame.DataCurve DataCurve = null;
			Library_SpriteStudio.KeyFrame.InterfaceData KeyData = null;
			foreach(XmlNode NodeAttribute in ListNodeAttribute)
			{
				ValueText = NodeAttribute.Attributes["tag"].Value;

				Description = ManagerDescriptionAttribute.DescriptionAttributeGetTagName(ValueText);
				if(null == Description)
				{
					Debug.LogWarning(	"SSAE-Import: Warning: Part["
										+ DataParts.Name
										+ "] Attribute Invalid. ["
										+ ValueText
										+ "]"
									);
					continue;
				}
				AttributeNo = (int)Description.Attribute;
				if((int)Library_SpriteStudio.KindAttributeKey.TERMINATOR_INHELIT > AttributeNo)
				{
					DataParts.DataAnimation.FlagKeyParameter |= Library_SpriteStudio.FlagParameterKeyFrameInherit[AttributeNo];
				}
				if(null == DataParts.DataAnimation.DataKeyFrame[AttributeNo])
				{
					DataParts.DataAnimation.DataKeyFrame[AttributeNo] = new ArrayList();
					DataParts.DataAnimation.DataKeyFrame[AttributeNo].Clear();
				}

				NodeListKey = XMLUtility.XML_SelectNodes(NodeAttribute, "key", ManagerNameSpace);
				if(null == NodeListKey)
				{
					Debug.LogWarning(	"SSAE-Import: Warning: Part["
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
						DataCurve = new Library_SpriteStudio.KeyFrame.DataCurve();
						NodeInterpolation = NodeKey.Attributes["ipType"];
						FlagWithParameterInterpolation = false;
						if(null == NodeInterpolation)
						{
							DataCurve.Kind = Library_SpriteStudio.KindInterpolation.NON;
						}
						else
						{
							switch(NodeInterpolation.Value)
							{
								case "linear":
									DataCurve.Kind = Library_SpriteStudio.KindInterpolation.LINEAR;
									break;

								case "hermite":
									DataCurve.Kind = Library_SpriteStudio.KindInterpolation.HERMITE;
									FlagWithParameterInterpolation = true;
									break;

								case "bezier":
									DataCurve.Kind = Library_SpriteStudio.KindInterpolation.BEZIER;
									FlagWithParameterInterpolation = true;
									break;

								case "acceleration":
									DataCurve.Kind = Library_SpriteStudio.KindInterpolation.ACCELERATE;
									break;

								case "deceleration":
									DataCurve.Kind = Library_SpriteStudio.KindInterpolation.DECELERATE;
									break;

								default:
									Debug.LogWarning(	"SSAE-Import: Warning: Part["
														+ DataParts.Name
														+ "] Attribute ["
														+ Description.Attribute.ToString()
														+ "] : Key ["
														+ KeyNo.ToString()
														+ "] Invalid Interpolation Kind: "
														+ NodeInterpolation.Value
													);
									DataCurve.Kind = Library_SpriteStudio.KindInterpolation.NON;
									break;
							}

							if(true == FlagWithParameterInterpolation)
							{
								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "curve", ManagerNameSpace);
								if(null == ValueText)
								{
									Debug.LogWarning(	"SSAE-Import: Warning: Part["
														+ DataParts.Name
														+ "] Attribute ["
														+ Description.Attribute.ToString()
														+ "] : Key ["
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
						case Library_SpriteStudio.KindValueKey.NUMBER:
							{
								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value", ManagerNameSpace);

								switch(Description.KindType)
								{
									case Library_SpriteStudio.KindTypeKey.INT:
										{
											int Value = XMLUtility.ValueGetInt(ValueText);
											Library_SpriteStudio.KeyFrame.DataInt DataInt = new Library_SpriteStudio.KeyFrame.DataInt();
											DataInt.Value = Value;
											KeyData = DataInt;
										}
										break;

									case Library_SpriteStudio.KindTypeKey.HEX:
										{
											int Value = XMLUtility.TextHexToInt(ValueText);
											Library_SpriteStudio.KeyFrame.DataInt DataInt = new Library_SpriteStudio.KeyFrame.DataInt();
											DataInt.Value = Value;
											KeyData = DataInt;
										}
										break;

									case Library_SpriteStudio.KindTypeKey.FLOAT:
									case Library_SpriteStudio.KindTypeKey.DEGREE:
										{
											float Value = (float)(XMLUtility.ValueGetDouble(ValueText));
											Library_SpriteStudio.KeyFrame.DataFloat DataFloat = new Library_SpriteStudio.KeyFrame.DataFloat();
											DataFloat.Value = Value;
											KeyData = DataFloat;
										}
										break;

									case Library_SpriteStudio.KindTypeKey.BOOL:
									case Library_SpriteStudio.KindTypeKey.OTHER:
									default:
										Debug.LogWarning(	"SSAE-Import: Warning: Part["
															+ DataParts.Name
															+ "] Attribute ["
															+ Description.Attribute.ToString()
															+ "] : Key ["
															+ KeyNo.ToString()
															+ "] Not-Supported Value-Type: "
															+ Description.KindType.ToString()
														);
										break;
								}
							}
							break;

						case Library_SpriteStudio.KindValueKey.CHECK:
							{
								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value", ManagerNameSpace);

								bool Value = XMLUtility.ValueGetBool(ValueText);
								Library_SpriteStudio.KeyFrame.DataBool DataBool = new Library_SpriteStudio.KeyFrame.DataBool();
								DataBool.Value = Value;
								KeyData = DataBool;

								FlagEnableInterpolation = false;
							}
							break;

						case Library_SpriteStudio.KindValueKey.COLOR:
							{
								Library_SpriteStudio.KeyFrame.ValueColor Value = new Library_SpriteStudio.KeyFrame.ValueColor();

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
										Debug.LogWarning(	"SSAE-Import: Warning: Part["
															+ DataParts.Name
															+ "] Attribute ["
															+ Description.Attribute.ToString()
															+ "] : Key ["
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
											Debug.LogWarning(	"SSAE-Import: Warning: Part["
																+ DataParts.Name
																+ "] Attribute ["
																+ Description.Attribute.ToString()
																+ "] : Key ["
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

								Library_SpriteStudio.KeyFrame.DataColor DataColor = new Library_SpriteStudio.KeyFrame.DataColor();
								DataColor.Value = Value;
								KeyData = DataColor;
							}
							break;

						case Library_SpriteStudio.KindValueKey.QUADRILATERRAL:
							{
								Library_SpriteStudio.KeyFrame.ValueQuadrilateral Value = new Library_SpriteStudio.KeyFrame.ValueQuadrilateral();

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

								Library_SpriteStudio.KeyFrame.DataQuadrilateral DataQuadrilateral = new Library_SpriteStudio.KeyFrame.DataQuadrilateral();
								DataQuadrilateral.Value = Value;
								KeyData = DataQuadrilateral;
							}
							break;

						case Library_SpriteStudio.KindValueKey.USER:
							{
								Library_SpriteStudio.KeyFrame.ValueUser Value = new Library_SpriteStudio.KeyFrame.ValueUser();
								Value.Flag = Library_SpriteStudio.KeyFrame.ValueUser.FlagData.CLEAR;

								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value/integer", ManagerNameSpace);
								if(null != ValueText)
								{
									if(false == uint.TryParse(ValueText, out Value.Number))
									{
										int	ValueIntTemp = 0;
										if(false == int.TryParse(ValueText, out ValueIntTemp))
										{
											Debug.LogWarning(	"SSAE-Import: Warning: Part["
																+ DataParts.Name
																+ "] : Key ["
																+ KeyNo.ToString()
																+ "] Invalid Number: "
																+ ValueText
															);
										}
										Value.Number = (uint)ValueIntTemp;
									}
									Value.Flag |= Library_SpriteStudio.KeyFrame.ValueUser.FlagData.NUMBER;
								}
								else
								{
									Value.Number = 0;
									Value.Flag &= ~Library_SpriteStudio.KeyFrame.ValueUser.FlagData.NUMBER;
								}

								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value/rect", ManagerNameSpace);
								if(null != ValueText)
								{
									ValueTextParameter = ValueText.Split(' ');
									Value.Rectangle.xMin = (float)(XMLUtility.ValueGetDouble(ValueTextParameter[0]));
									Value.Rectangle.yMin = (float)(XMLUtility.ValueGetDouble(ValueTextParameter[1]));
									Value.Rectangle.xMax = (float)(XMLUtility.ValueGetDouble(ValueTextParameter[2]));
									Value.Rectangle.yMax = (float)(XMLUtility.ValueGetDouble(ValueTextParameter[3]));
									Value.Flag |= Library_SpriteStudio.KeyFrame.ValueUser.FlagData.RECTANGLE;
								}
								else
								{
									Value.Rectangle.xMin = 0.0f;
									Value.Rectangle.yMin = 0.0f;
									Value.Rectangle.xMax = 0.0f;
									Value.Rectangle.yMax = 0.0f;
									Value.Flag &= ~Library_SpriteStudio.KeyFrame.ValueUser.FlagData.RECTANGLE;
								}

								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value/point", ManagerNameSpace);
								Value.Coordinate = new Library_SpriteStudio.KeyFrame.ValuePoint();
								if(null != ValueText)
								{
									ValueTextParameter = ValueText.Split(' ');
									Value.Coordinate.X = (float)(XMLUtility.ValueGetDouble(ValueTextParameter[0]));
									Value.Coordinate.Y = (float)(XMLUtility.ValueGetDouble(ValueTextParameter[1]));
									Value.Flag |= Library_SpriteStudio.KeyFrame.ValueUser.FlagData.COORDINATE;
								}
								else
								{
									Value.Coordinate.X = 0.0f;
									Value.Coordinate.Y = 0.0f;
									Value.Flag &= ~Library_SpriteStudio.KeyFrame.ValueUser.FlagData.COORDINATE;
								}

								ValueText = XMLUtility.TextGetSelectSingleNode(NodeKey, "value/string", ManagerNameSpace);
								if(null != ValueText)
								{
									Value.Text = string.Copy(ValueText);
									Value.Flag |= Library_SpriteStudio.KeyFrame.ValueUser.FlagData.TEXT;
								}
								else
								{
									Value.Text = null;
									Value.Flag &= ~Library_SpriteStudio.KeyFrame.ValueUser.FlagData.TEXT;
								}

								Library_SpriteStudio.KeyFrame.DataUser DataUser = new Library_SpriteStudio.KeyFrame.DataUser();
								DataUser.Value = Value;
								KeyData = DataUser;

								FlagEnableInterpolation = false;
							}
							break;

						case Library_SpriteStudio.KindValueKey.CELL:
							{
								Library_SpriteStudio.KeyFrame.ValueCell Value = new Library_SpriteStudio.KeyFrame.ValueCell();

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
									Debug.LogWarning(	"SSAE-Import: Warning: Part["
														+ DataParts.Name
														+ "] : Key ["
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

								Library_SpriteStudio.KeyFrame.DataCell DataCell = new Library_SpriteStudio.KeyFrame.DataCell();
								DataCell.Value = Value;
								KeyData = DataCell;

								FlagEnableInterpolation = false;
							}
							break;

						case Library_SpriteStudio.KindValueKey.POINT:
						case Library_SpriteStudio.KindValueKey.SOUND:
						case Library_SpriteStudio.KindValueKey.PALETTE:
						default:
							Debug.LogWarning(	"SSAE-Import: Warning: Part["
												+ DataParts.Name
												+ "] Attribute ["
												+ Description.Attribute.ToString()
												+ "] : Key ["
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

		public static void ImportSSAEKeyDataGetColorBlend(	out float ColorA,
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

	public static class Create
	{
		private readonly static string NamePathSubImportTexture = "Texture";
		private readonly static string NamePathSubImportMaterial = "Material";

		public static class Parts
		{
			public static GameObject Root(DataIntermediate.PartsSprite Parts, DataIntermediate.TrunkParts Trunk, GameObject GameObjectParent)
			{
				GameObject GameObjectNow = AssetUtility.Create.GameObject(Parts.Name, GameObjectParent);

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
				}

				PartsRoot.RateTimeAnimation = 1.0f;
				PartsRoot.AnimationNo = 0;
				PartsRoot.CountLoopRemain = -1;
				PartsRoot.FrameNoInitial = 0;

				PartsRoot.KindRenderQueueBase = Script_SpriteStudio_PartsRoot.KindDrawQueue.SHADER_SETTING;
				PartsRoot.OffsetDrawQueue = 0;
				PartsRoot.RateDrawQueueEffectZ = 250.0f;

				GameObjectNow.AddComponent<MeshFilter>();
				MeshRenderer InstanceMeshRenderer = GameObjectNow.AddComponent<MeshRenderer>();
				InstanceMeshRenderer.enabled = true;
				InstanceMeshRenderer.castShadows = false;
				InstanceMeshRenderer.receiveShadows = false;

				return(GameObjectNow);
			}

			public static GameObject Node(DataIntermediate.PartsSprite DataParts, GameObject GameObjectParent)
			{
				GameObject GameObjectNow = AssetUtility.Create.GameObject(DataParts.Name, GameObjectParent);

				switch(DataParts.PartsKind)
				{
					case Library_SpriteStudio.KindParts.NORMAL:
						{
							if(null == DataParts.DataAnimation.DataKeyFrame[(int)Library_SpriteStudio.KindAttributeKey.VERTEX_CORRECTION])
							{
								GameObjectNow.AddComponent<Script_SpriteStudio_Triangle2>();
							}
							else
							{
								GameObjectNow.AddComponent<Script_SpriteStudio_Triangle4>();
							}
						}
						break;

					case Library_SpriteStudio.KindParts.ROOT:
						break;

					case Library_SpriteStudio.KindParts.NULL:
						{
							GameObjectNow.AddComponent<Script_SpriteStudio_PartsNULL>();
						}
						break;

					case Library_SpriteStudio.KindParts.BOUND:
						break;

					case Library_SpriteStudio.KindParts.SOUND:
						break;
				}

				return(GameObjectNow);
			}

			public static Library_SpriteStudio.SpriteData NodeSetAnimation(	GameObject GameObjectParts,
																			DataIntermediate.PartsSprite DataParts,
																			Script_SpriteStudio_PartsRoot ScriptRoot,
																			Library_SpriteStudio.KindAttributeKey[][] TableMaskAttribute,
																			Library_SpriteStudio.SpriteData.BitStatus StatusInitial
																		)
			{
				Library_SpriteStudio.SpriteData DataSpriteStudio = null;

				Script_SpriteStudio_Triangle2 ComponentScript_Triangle2 = GameObjectParts.GetComponent<Script_SpriteStudio_Triangle2>();
				if(null != ComponentScript_Triangle2)
				{
					ComponentScript_Triangle2.BootUpForce();
					DataSpriteStudio = ComponentScript_Triangle2.SpriteStudioData;
					DataSpriteStudio.ID = DataParts.ID;

					AnimationDataConvertRuntime(	DataSpriteStudio,
													DataParts.DataAnimation,
													TableMaskAttribute[(int)Library_SpriteStudio.KindParts.NORMAL],
													StatusInitial
												);

					ComponentScript_Triangle2.RateOpacityLU = 1.0f;
					ComponentScript_Triangle2.RateOpacityRU = 1.0f;
					ComponentScript_Triangle2.RateOpacityRD = 1.0f;
					ComponentScript_Triangle2.RateOpacityLD = 1.0f;

					ComponentScript_Triangle2.TextureScale = Vector2.one;
					ComponentScript_Triangle2.TextureRotate = 0.0f;

					ComponentScript_Triangle2.VertexColorLU = Color.white;
					ComponentScript_Triangle2.VertexColorRU = Color.white;
					ComponentScript_Triangle2.VertexColorLD = Color.white;
					ComponentScript_Triangle2.VertexColorRD = Color.white;
					ComponentScript_Triangle2.SpriteStudioData.ColorBlendKind = Library_SpriteStudio.KindColorOperation.NON;

					ComponentScript_Triangle2.SpriteStudioData.KindBlendTarget = DataParts.KindBlendTarget;
				}
				else
				{
					Script_SpriteStudio_Triangle4 ComponentScript_Triangle4 = GameObjectParts.GetComponent<Script_SpriteStudio_Triangle4>();
					if(null != ComponentScript_Triangle4)
					{
						ComponentScript_Triangle4.BootUpForce();
						DataSpriteStudio = ComponentScript_Triangle4.SpriteStudioData;
						DataSpriteStudio.ID = DataParts.ID;

						AnimationDataConvertRuntime(	DataSpriteStudio,
														DataParts.DataAnimation,
														TableMaskAttribute[(int)Library_SpriteStudio.KindParts.NORMAL],
														StatusInitial
													);

						ComponentScript_Triangle4.RateOpacityLU = 1.0f;
						ComponentScript_Triangle4.RateOpacityRU = 1.0f;
						ComponentScript_Triangle4.RateOpacityRD = 1.0f;
						ComponentScript_Triangle4.RateOpacityLD = 1.0f;

						ComponentScript_Triangle4.TextureScale = Vector2.one;
						ComponentScript_Triangle4.TextureRotate = 0.0f;

						ComponentScript_Triangle4.VertexColorLU = Color.white;
						ComponentScript_Triangle4.VertexColorRU = Color.white;
						ComponentScript_Triangle4.VertexColorLD = Color.white;
						ComponentScript_Triangle4.VertexColorRD = Color.white;
						ComponentScript_Triangle4.SpriteStudioData.ColorBlendKind = Library_SpriteStudio.KindColorOperation.NON;

						ComponentScript_Triangle4.SpriteStudioData.KindBlendTarget = DataParts.KindBlendTarget;
					}
					else
					{
						Script_SpriteStudio_PartsNULL ComponentScript_PartsNULL = GameObjectParts.GetComponent<Script_SpriteStudio_PartsNULL>();
						if(null != ComponentScript_PartsNULL)
						{
							ComponentScript_PartsNULL.BootUpForce();
							DataSpriteStudio = ComponentScript_PartsNULL.SpriteStudioData;
							DataSpriteStudio.ID = DataParts.ID;

							AnimationDataConvertRuntime(	DataSpriteStudio,
															DataParts.DataAnimation,
															TableMaskAttribute[(int)Library_SpriteStudio.KindParts.NULL],
															StatusInitial
														);
						}
						else
						{
							Script_SpriteStudio_PartsRoot ComponentScript_PartsRoot = GameObjectParts.GetComponent<Script_SpriteStudio_PartsRoot>();
							if(null != ComponentScript_PartsRoot)
							{
								ComponentScript_PartsRoot.BootUpForce();
								DataSpriteStudio = ComponentScript_PartsRoot.SpriteStudioData;
								DataSpriteStudio.ID = DataParts.ID;

								AnimationDataConvertRuntime(	DataSpriteStudio,
																DataParts.DataAnimation,
																TableMaskAttribute[(int)Library_SpriteStudio.KindParts.ROOT],
																StatusInitial
															);
							}
						}
					}
				}

				return(DataSpriteStudio);
			}
			private static Library_SpriteStudio.KeyFrame.InterfaceData KeyFrameGetTime0(Library_SpriteStudio.KeyFrame.InterfaceData KeyData)
			{
				if(null != KeyData)
				{
					if(0 == KeyData.Time)
					{
						return(KeyData);
					}
				}
				return(null);
			}

			public static void NodeSetCollider(	GameObject GameObjectParts,
												DataIntermediate.PartsSprite DataParts,
												Library_SpriteStudio.SpriteData DataPartsRuntime,
												ref SettingImport DataSettingImport
											)
			{
				switch(DataParts.CollisionKind)
				{
					case Library_SpriteStudio.KindCollision.NON:
						return;

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

							DataPartsRuntime.CollisionKind = Library_SpriteStudio.KindCollision.SQUARE;
							DataPartsRuntime.CollisionComponent = InstanceColliderBox;
						}
						break;

					case Library_SpriteStudio.KindCollision.AABB:
						return;

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

							DataPartsRuntime.CollisionKind = Library_SpriteStudio.KindCollision.CIRCLE;
							DataPartsRuntime.CollisionComponent = InstanceColliderCapsule;
						}
						break;

					case Library_SpriteStudio.KindCollision.CIRCLE_SCALEMINIMUM:
						return;

					case Library_SpriteStudio.KindCollision.CIRCLE_SCALEMAXIMUM:
						return;
				}
			}

			private static Library_SpriteStudio.SpriteData Script_SpriteDataGet(GameObject GameObjectParts)
			{
				Script_SpriteStudio_Triangle2 ComponentScript_Triangle2 = GameObjectParts.GetComponent<Script_SpriteStudio_Triangle2>();
				if(null != ComponentScript_Triangle2)
				{
					return(ComponentScript_Triangle2.SpriteStudioData);
				}
				else
				{
					Script_SpriteStudio_Triangle4 ComponentScript_Triangle4 = GameObjectParts.GetComponent<Script_SpriteStudio_Triangle4>();
					if(null != ComponentScript_Triangle4)
					{
						return(ComponentScript_Triangle4.SpriteStudioData);
					}
					else
					{
						Script_SpriteStudio_PartsNULL ComponentScript_PartsNULL = GameObjectParts.GetComponent<Script_SpriteStudio_PartsNULL>();
						if(null != ComponentScript_PartsNULL)
						{
							return(ComponentScript_PartsNULL.SpriteStudioData);
						}
						else
						{
							Script_SpriteStudio_PartsRoot ComponentScript_PartsRoot = GameObjectParts.GetComponent<Script_SpriteStudio_PartsRoot>();
							if(null != ComponentScript_PartsRoot)
							{
								return(null);
							}
						}
					}
				}
				return(null);
			}

			public static void AnimationDataConvertRuntime(	Library_SpriteStudio.AnimationDataRuntime DataRuntime,
															DataIntermediate.AnimationDataEditor DataEditor,
															Library_SpriteStudio.KindAttributeKey[] ListMaskAttribute,
															Library_SpriteStudio.SpriteData.BitStatus StatusInitial
														)
			{
				DataRuntime.Status = StatusInitial & Library_SpriteStudio.AnimationDataRuntime.BitStatus.MASK_RESET;
				DataRuntime.Inheritance = DataEditor.Inheritance;
				DataRuntime.FlagInheritance = DataEditor.FlagInheritance;

				int AttributeNo = -1;
				for(int i=0; i<ListMaskAttribute.Length; i++)
				{
					AttributeNo = (int)ListMaskAttribute[i];
					if((int)Library_SpriteStudio.KindAttributeKey.TERMINATOR == AttributeNo)
					{
						break;
					}

					if(null != DataEditor.DataKeyFrame[AttributeNo])
					{
						DataEditor.DataKeyFrame[AttributeNo].Clear();
						DataEditor.DataKeyFrame[AttributeNo] = null;
					}
					if((int)Library_SpriteStudio.KindAttributeKey.TERMINATOR_INHELIT > AttributeNo)
					{
						DataEditor.FlagKeyParameter = DataEditor.FlagKeyParameter & ~Library_SpriteStudio.FlagParameterKeyFrameInherit[AttributeNo];
					}
				}
				DataRuntime.FlagKeyParameter = DataEditor.FlagKeyParameter;

				AttributeNo = -1;
				if(null != DataEditor.DataKeyFrame)
				{
					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.POSITION_X;
					DataRuntime.AnimationDataPositionX = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataFloat>(DataEditor.DataKeyFrame[AttributeNo]);

					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.POSITION_Y;
					DataRuntime.AnimationDataPositionY = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataFloat>(DataEditor.DataKeyFrame[AttributeNo]);

					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.POSITION_Z;
					DataRuntime.AnimationDataPositionZ = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataFloat>(DataEditor.DataKeyFrame[AttributeNo]);

					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.ROTATE_X;
					DataRuntime.AnimationDataRotateX = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataFloat>(DataEditor.DataKeyFrame[AttributeNo]);

					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.ROTATE_Y;
					DataRuntime.AnimationDataRotateY = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataFloat>(DataEditor.DataKeyFrame[AttributeNo]);

					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.ROTATE_Z;
					DataRuntime.AnimationDataRotateZ = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataFloat>(DataEditor.DataKeyFrame[AttributeNo]);

					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.SCALE_X;
					DataRuntime.AnimationDataScaleX = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataFloat>(DataEditor.DataKeyFrame[AttributeNo]);

					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.SCALE_Y;
					DataRuntime.AnimationDataScaleY = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataFloat>(DataEditor.DataKeyFrame[AttributeNo]);

					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.FLIP_X;
					DataRuntime.AnimationDataFlipX = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataBool>(DataEditor.DataKeyFrame[AttributeNo]);

					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.FLIP_Y;
					DataRuntime.AnimationDataFlipY = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataBool>(DataEditor.DataKeyFrame[AttributeNo]);

					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.SHOW_HIDE;
					DataRuntime.AnimationDataHide = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataBool>(DataEditor.DataKeyFrame[AttributeNo]);


					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.OPACITY_RATE;
					DataRuntime.AnimationDataOpacityRate = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataFloat>(DataEditor.DataKeyFrame[AttributeNo]);

					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.PRIORITY;
					DataRuntime.AnimationDataPriority = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataFloat>(DataEditor.DataKeyFrame[AttributeNo]);

					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.COLOR_BLEND;
					DataRuntime.AnimationDataColorBlend = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataColor>(DataEditor.DataKeyFrame[AttributeNo]);

					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.VERTEX_CORRECTION;
					DataRuntime.AnimationDataVertexCorrection = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataQuadrilateral>(DataEditor.DataKeyFrame[AttributeNo]);

					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.ORIGIN_OFFSET_X;
					DataRuntime.AnimationDataOriginOffsetX = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataFloat>(DataEditor.DataKeyFrame[AttributeNo]);

					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.ORIGIN_OFFSET_Y;
					DataRuntime.AnimationDataOriginOffsetY = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataFloat>(DataEditor.DataKeyFrame[AttributeNo]);


					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.ANCHOR_POSITION_X;
					DataRuntime.AnimationDataAnchorPositionX = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataFloat>(DataEditor.DataKeyFrame[AttributeNo]);

					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.ANCHOR_POSITION_Y;
					DataRuntime.AnimationDataAnchorPositionY = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataFloat>(DataEditor.DataKeyFrame[AttributeNo]);

					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.ANCHOR_SIZE_X;
					DataRuntime.AnimationDataAnchorSizeX = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataFloat>(DataEditor.DataKeyFrame[AttributeNo]);

					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.ANCHOR_SIZE_Y;
					DataRuntime.AnimationDataAnchorSizeY = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataFloat>(DataEditor.DataKeyFrame[AttributeNo]);


					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.TEXTURE_FLIP_X;
					DataRuntime.AnimationDataTextureFlipX = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataBool>(DataEditor.DataKeyFrame[AttributeNo]);

					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.TEXTURE_FLIP_Y;
					DataRuntime.AnimationDataTextureFlipY = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataBool>(DataEditor.DataKeyFrame[AttributeNo]);


					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.TEXTURE_TRANSLATE_X;
					DataRuntime.AnimationDataTextureTranslateX = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataFloat>(DataEditor.DataKeyFrame[AttributeNo]);

					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.TEXTURE_TRANSLATE_Y;
					DataRuntime.AnimationDataTextureTranslateY = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataFloat>(DataEditor.DataKeyFrame[AttributeNo]);

					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.TEXTURE_ROTATE;
					DataRuntime.AnimationDataTextureRotate = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataFloat>(DataEditor.DataKeyFrame[AttributeNo]);

					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.TEXTURE_SCALE_X;
					DataRuntime.AnimationDataTextureScaleX = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataFloat>(DataEditor.DataKeyFrame[AttributeNo]);

					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.TEXTURE_SCALE_Y;
					DataRuntime.AnimationDataTextureScaleY = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataFloat>(DataEditor.DataKeyFrame[AttributeNo]);


					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.COLLISION_RADIUS;
					DataRuntime.AnimationDataCollisionRadius = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataFloat>(DataEditor.DataKeyFrame[AttributeNo]);


					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.CELL;
					DataRuntime.AnimationDataCell = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataCell>(DataEditor.DataKeyFrame[AttributeNo]);

					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.USER_DATA;
					DataRuntime.AnimationDataUser = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataUser>(DataEditor.DataKeyFrame[AttributeNo]);

					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.SOUND;
					DataRuntime.AnimationDataSound = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataSound>(DataEditor.DataKeyFrame[AttributeNo]);

					AttributeNo = (int)Library_SpriteStudio.KindAttributeKey.PALETTE_CHANGE;
					DataRuntime.AnimationDataPaletteChange = AnimationDataConvertRuntimeValue<Library_SpriteStudio.KeyFrame.DataParette>(DataEditor.DataKeyFrame[AttributeNo]);
				}
			}
			private static Library_SpriteStudio.KeyFrame.DataCurve AnimationDataDuplicateCurve(Library_SpriteStudio.KeyFrame.DataCurve CurveOriginal)
			{
				if(null == CurveOriginal)
				{
					return(null);
				}
				Library_SpriteStudio.KeyFrame.DataCurve CurveOutput = new Library_SpriteStudio.KeyFrame.DataCurve();
				CurveOutput.Kind = CurveOriginal.Kind;
				CurveOutput.TimeStart = CurveOriginal.TimeStart;
				CurveOutput.ValueStart = CurveOriginal.ValueStart;
				CurveOutput.TimeEnd = CurveOriginal.TimeEnd;
				CurveOutput.ValueEnd = CurveOriginal.ValueEnd;
				return(CurveOutput);
			}
			private static _Type[] AnimationDataConvertRuntimeValue<_Type>(ArrayList DataOriginalArray)
				where _Type : Library_SpriteStudio.KeyFrame.InterfaceData, new()
			{
				if(null == DataOriginalArray)
				{
					return(null);
				}

				int Count = DataOriginalArray.Count;
				_Type[] DataOutput = new _Type[Count];

				_Type DataInput = default(_Type);
				for(int i=0; i<Count; i++)
				{
					DataInput = (_Type)DataOriginalArray[i];
					DataOutput[i] = new _Type();
					DataOutput[i].ObjectValue = DataInput.ObjectValue;

					DataOutput[i].Kind = DataInput.Kind;
					DataOutput[i].Time = DataInput.Time;
					DataOutput[i].Curve = AnimationDataDuplicateCurve(DataInput.Curve);
				}

				return(DataOutput);
			}
		}

		public static Material[] DataMaterial(	string Name,
												string NamePath,
												DataIntermediate.TrunkParts Data,
												ref SettingImport DataSettingImport
											)
		{
			string NamePathImportAssetSub = "";

			AssetUtility.Create.Folder(NamePathSubImportTexture, NamePath);
			AssetUtility.Create.Folder(NamePathSubImportMaterial, NamePath);

			NamePathImportAssetSub = NamePath + "/" + NamePathSubImportTexture;
			Texture2D[] TebleTexture = TextureTable(	NamePathImportAssetSub,
														Data.ListImage,
														Data.ListImage.Length,
														ref DataSettingImport
													);

			NamePathImportAssetSub = NamePath + "/" + NamePathSubImportMaterial;
			Material[] TableMaterial = MaterialTable(	NamePathImportAssetSub,
														TebleTexture
													);

			AssetDatabase.SaveAssets();

			return(TableMaterial);
		}

		public static Object DataPrefabSprite(	string Name,
												string NamePath,
												DataIntermediate.TrunkParts Data,
												Material[] TableMaterial,
												ref SettingImport DataSettingImport,
												Library_SpriteStudio.KindAttributeKey[][] TableMaskAttribute,
												Library_SpriteStudio.AnimationDataRuntime.BitStatus StatusInitial
											)
		{
			Object PrefabNow = AssetUtility.Create.Prefab(Name, NamePath);
			if(null == PrefabNow)
			{
				return(null);
			}

			GameObject GameObjectControl = AssetUtility.Create.GameObject(Name + "_Control", null);

			GameObject GameObjectRoot = Parts.Root(Data.ListParts[0], Data, GameObjectControl);
			Script_SpriteStudio_PartsRoot ScriptRoot = GameObjectRoot.GetComponent<Script_SpriteStudio_PartsRoot>();

			ScriptRoot.TableMaterial = TableMaterial;

			GameObject[] GameObjectParts = new GameObject[Data.ListParts.Length];
			GameObjectParts[0] = GameObjectRoot;
			for(int i=1; i<Data.ListParts.Length; i++)
			{
				GameObjectParts[i] = Parts.Node(Data.ListParts[i], GameObjectRoot);
			}
			GameObject GameObjectParent = null;
			int ParentNo = -1;
			for(int i=1; i<Data.ListParts.Length; i++)
			{
				ParentNo = Data.ListParts[i].IDParent;
				if(0 < ParentNo)
				{
					GameObjectParent = GameObjectParts[ParentNo];
					GameObjectParts[i].transform.parent = GameObjectParent.transform;
				}
			}
			Library_SpriteStudio.SpriteData DataSpriteStudio = null;
			for(int i=0; i<Data.ListParts.Length; i++)
			{
				DataSpriteStudio = Parts.NodeSetAnimation(	GameObjectParts[i],
															Data.ListParts[i],
															ScriptRoot,
															TableMaskAttribute,
															StatusInitial
														);

				Parts.NodeSetCollider(	GameObjectParts[i],
										Data.ListParts[i],
										DataSpriteStudio,
										ref DataSettingImport
									);
			}

			AssetUtility.GameObjectSetActive(GameObjectControl, true);
			AssetUtility.GameObjectSetActive(GameObjectRoot, true);
			for(int i=0; i<GameObjectParts.Length; i++)
			{
				if(null != GameObjectParts[i])
				{
					AssetUtility.GameObjectSetActive(GameObjectParts[i], true);
				}
			}

			PrefabUtility.ReplacePrefab(GameObjectControl, PrefabNow, ReplacePrefabOptions.ConnectToPrefab);

			AssetDatabase.SaveAssets();

			Object.DestroyImmediate(GameObjectControl);

			return(PrefabNow);
		}

		public static Texture2D[] TextureTable(	string NamePath,
												DataIntermediate.PartsImage[] ListImage,
												int Count,
												ref SettingImport DataSettingImport
											)
		{
			Texture2D[] TableTexture = new Texture2D[Count];
			string NameFileBody = "";
			string NameFileExtensionTexture = "";
			string NameAsset = "";

			for(int i=0; i<Count; i++)
			{
				NameFileBody = Path.GetFileNameWithoutExtension(ListImage[i].FileName);
				NameFileExtensionTexture = Path.GetExtension(ListImage[i].FileName);

				NameAsset = NamePath + "/" + NameFileBody + NameFileExtensionTexture;

				File.FileCopyToAsset(File.NamePathToAsset(NameAsset), ListImage[i].FileName, true);

				AssetDatabase.ImportAsset(NameAsset);
				TextureImporter Importer = TextureImporter.GetAtPath(NameAsset) as TextureImporter;
				Importer.anisoLevel = 1;
				Importer.borderMipmap = false;
				Importer.convertToNormalmap = false;
				Importer.fadeout = false;
				Importer.filterMode = FilterMode.Bilinear;
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
				Importer.wrapMode = TextureWrapMode.Clamp;
				AssetDatabase.ImportAsset(NameAsset, ImportAssetOptions.ForceUpdate);
				TableTexture[i] = AssetDatabase.LoadAssetAtPath(NameAsset, typeof(Texture2D)) as Texture2D;
			}

			AssetDatabase.SaveAssets();

			return(TableTexture);
		}

		public static Material[] MaterialTable(string NamePath, Texture2D[] TableTexture)
		{
			int Count = TableTexture.Length;
			Material[] TableMaterial = new Material[Count * ShaderOperationMax];
			string NameFileBody = "";
			string NameAsset = "";
			for(int i=0; i<Count; i++)
			{
				string NameMaterialSuffix = "";
				int MaterialNo = 0;
				Library_SpriteStudio.KindColorOperation	KindOperation = Library_SpriteStudio.KindColorOperation.NON;
				NameFileBody = TableTexture[i].name;
				for(int j=0; j<ShaderOperationMax; j++)
				{
					KindOperation = (Library_SpriteStudio.KindColorOperation)(j + 1);
					NameMaterialSuffix = KindOperation.ToString();

					MaterialNo = (i * ShaderOperationMax) + j;
					NameAsset = NamePath + "/" + NameFileBody + "_" + NameMaterialSuffix + ".mat";

					TableMaterial[MaterialNo] = new Material(Shader_SpriteStudioTriangleX[j]);
					TableMaterial[MaterialNo].mainTexture = TableTexture[i];
					AssetDatabase.CreateAsset(TableMaterial[MaterialNo], NameAsset);
					TableMaterial[MaterialNo] = AssetDatabase.LoadAssetAtPath(NameAsset, typeof(Material)) as Material;
				}
			}
			AssetDatabase.SaveAssets();

			return(TableMaterial);
		}

		private readonly static int[] SizePixelNormalize = {64, 128, 256, 512, 1024, 2048, 4096, 8192, 16384, 32768, 65536, -1};
		public static bool PixelSizeCheck(int Size)
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
	}

	public static partial class AssetUtility
	{
		internal readonly static string NamePathRootAsset = "Assets";

		public static class Create
		{
			public static bool Folder(string Name, string NameParent)
			{
				string NameFolderParent = "";
				if(null != NameParent)
				{
					NameFolderParent = string.Copy(NameParent);
				}

				if(null == AssetDatabase.LoadMainAssetAtPath(NameFolderParent + "/" + Name))
				{
					AssetDatabase.CreateFolder(NameFolderParent, Name);
				}

				return(true);
			}

			public static Object Prefab(string Name, string NamePath)
			{
				string NamePathAsset = NamePath + "/" + Name + ".prefab";
				if(false == AssetUtility.ObjectCheckOverwrite(NamePathAsset))
				{
					return(null);
				}

				Object PrefabNow = PrefabUtility.CreateEmptyPrefab(NamePathAsset);

				return(PrefabNow);
			}

			public static GameObject GameObject(string Name, GameObject GameObjectParent)
			{
				GameObject GameObjectNow = new GameObject(Name);
				GameObjectNow.active = false;
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
				}
				return(GameObjectNow);
			}
		}

		public static string NamePathGet(string Name)
		{
			string NamePath = System.String.Copy(NamePathRootAsset);
			if(null != Name)
			{
				NamePath += "/" + Name;
			}

			return(NamePath);
		}

		public static string NamePathGetSelectNow(string NamePath)
		{
			string NamePathAsset = "";
			if(null == NamePath)
			{
				Object ObjectNow = Selection.activeObject;
				if(null == ObjectNow)
				{
					NamePathAsset = System.String.Copy(NamePathRootAsset);
				}
				else
				{
					NamePathAsset = AssetDatabase.GetAssetPath(ObjectNow);
				}
			}
			else
			{
				NamePathAsset = System.String.Copy(NamePath);
			}

			return(NamePathAsset);
		}

		public static bool ObjectCheckOverwrite(string NameAsset)
		{
			Object ObjectExsting = AssetDatabase.LoadAssetAtPath(NameAsset, typeof(GameObject));
			if(null != ObjectExsting)
			{
				if(false == EditorUtility.DisplayDialog(	"The asset already exists.\n" + NameAsset,
															"Do you want to overwrite?",
															"Yes",
															"No"
														)
					)
				{
					return(false);
				}
			}

			return(true);
		}

		public static void GameObjectSetActive(GameObject InstanceGameObject, bool FlagSwitch)
		{
			InstanceGameObject.active = FlagSwitch;
		}
	}

	public static partial class XMLUtility
	{
		public static bool ValueGetBool<_Type>(_Type Source)
		{
			return((0 != ValueGetInt(Source)) ? true : false);
		}

		public static byte ValueGetByte<_Type>(_Type Source)
		{
			return(System.Convert.ToByte(Source));
		}

		public static int ValueGetInt<_Type>(_Type Source)
		{
			return(System.Convert.ToInt32(Source));
		}

		public static uint ValueGetUInt<_Type>(_Type Source)
		{
			return(System.Convert.ToUInt32(Source));
		}

		public static float ValueGetFloat<_Type>(_Type Source)
		{
			return(System.Convert.ToSingle(Source));
		}

		public static double ValueGetDouble<_Type>(_Type Source)
		{
			return(System.Convert.ToDouble(Source));
		}

		public static bool TextToBool(string Text)
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

		public static int TextHexToInt(string Text)
		{
			return(System.Convert.ToInt32(Text, 16));
		}

		public static uint TextHexToUInt(string Text)
		{
			return(System.Convert.ToUInt32(Text, 16));
		}

		public static XmlNode XML_SelectSingleNode(XmlNode Node, string NamePath, XmlNamespaceManager Manager)
		{
			return(Node.SelectSingleNode(NamePath, Manager));
		}

		public static string TextGetSelectSingleNode(XmlNode Node, string NamePath, XmlNamespaceManager Manager)
		{
			XmlNode NodeNow = XML_SelectSingleNode(Node, NamePath, Manager);
			return((null != NodeNow) ? NodeNow.InnerText : null);
		}

		public static XmlNodeList XML_SelectNodes(XmlNode Node, string NamePath, XmlNamespaceManager Manager)
		{
			return(Node.SelectNodes(NamePath, Manager));
		}

		public static int VersionGetHexCode(string Text)
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

		public static string VersionGetString(int VersionCode)
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
