/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Script_SpriteStudio_PartsRoot))]
public class Inspector_SpriteStudio_PartsRoot : Editor
{
	private static bool FoldOutAnimationInformation;
	private static bool FoldOutPlayInformation;
	private static bool FoldOutMaterialTable;

	public override void OnInspectorGUI()
	{
		Script_SpriteStudio_PartsRoot Data = (Script_SpriteStudio_PartsRoot)target;

		EditorGUILayout.LabelField("[SpriteStudio Parts-Root]");
		int LevelIndent = 0;

		EditorGUILayout.Space();
		FoldOutMaterialTable = EditorGUILayout.Foldout(FoldOutMaterialTable, "Based-Material Table");
		if(true == FoldOutMaterialTable)
		{
			EditorGUI.indentLevel = LevelIndent + 1;
			if(null != Data.TableMaterial)
			{
				int CountShader = (int)(Library_SpriteStudio.KindColorOperation.TERMINATOR - 1);
				int Count = Data.TableMaterial.Length / CountShader;
				int MaterialNoTop = 0;
				Library_SpriteStudio.KindColorOperation MaterialTableNo = 0;
				string NameField = "";
				for(int i=0; i<Count; i++)
				{
					MaterialNoTop = i * (int)(Library_SpriteStudio.KindColorOperation.TERMINATOR - 1);
					EditorGUILayout.LabelField(	"Material No ["
												+ MaterialNoTop
												+ "-"
												+ ((MaterialNoTop + CountShader) - 1)
												+ "]: Texture-"
												+ i
											);

					EditorGUI.indentLevel = LevelIndent + 2;
					for(int j=0; j<CountShader; j++)
					{
						MaterialTableNo = (Library_SpriteStudio.KindColorOperation)(j+1);
						NameField = "Shader [" + MaterialTableNo.ToString() + "]";
						Data.TableMaterial[MaterialNoTop + j] = (Material)(EditorGUILayout.ObjectField(NameField, Data.TableMaterial[MaterialNoTop + j], typeof(Material), false));
					}
					EditorGUILayout.Space();
					EditorGUI.indentLevel = LevelIndent + 1;
				}
			}
			EditorGUI.indentLevel = LevelIndent;
		}
		EditorGUILayout.Space();

		FoldOutAnimationInformation = EditorGUILayout.Foldout(FoldOutAnimationInformation, "Animation Information");
		if(true == FoldOutAnimationInformation)
		{
			EditorGUI.indentLevel = LevelIndent + 1;
			if(null != Data.ListInformationPlay)
			{
				for(int i=0; i<Data.ListInformationPlay.Length; i++)
				{
					EditorGUILayout.LabelField("Animation No [" + i + "]: Name[" + Data.ListInformationPlay[i].Name + "]");

					EditorGUI.indentLevel = LevelIndent + 2;
					EditorGUILayout.LabelField("Start Frame-No [" + Data.ListInformationPlay[i].FrameStart.ToString("D5") + "]");
					EditorGUILayout.LabelField("End Frame-No [" + Data.ListInformationPlay[i].FrameEnd.ToString("D5") + "]");

					float FPS = 1.0f / (float)Data.ListInformationPlay[i].FramePerSecond;
					EditorGUILayout.LabelField("Base FPS [" + Data.ListInformationPlay[i].FramePerSecond.ToString("D3") + "]:(" + FPS.ToString() + " Sec.)");
					EditorGUILayout.Space();

					EditorGUI.indentLevel = LevelIndent + 1;
				}
			}
			EditorGUI.indentLevel = LevelIndent;
		}
		EditorGUILayout.Space();

		FoldOutPlayInformation = EditorGUILayout.Foldout(FoldOutPlayInformation, "Initial/Preview Play Setting");
		if(true == FoldOutPlayInformation)
		{
			bool FlagUpdate = false;
			EditorGUI.indentLevel = LevelIndent + 1;

			string[] NameAnimation = new string[Data.ListInformationPlay.Length];
			int[] IndexAnimation = new int[Data.ListInformationPlay.Length];
			for(int i=0; i<Data.ListInformationPlay.Length; i++)
			{
				IndexAnimation[i] = i;
				NameAnimation[i] = Data.ListInformationPlay[i].Name;
			}
			int AnimationNo = EditorGUILayout.IntPopup("Animation Name", Data.AnimationNo, NameAnimation, IndexAnimation);
			if(Data.AnimationNo != AnimationNo)
			{
				Data.AnimationNo = AnimationNo;
				FlagUpdate = true;
			}
			int CountLabel = Data.ListInformationPlay[AnimationNo].Label.Length;
			bool FlagLabelSelectable = (0 < CountLabel) ? true : false;

			int FrameNoStart = 0;
			int FrameNoEnd = Data.ListInformationPlay[AnimationNo].FrameEnd - Data.ListInformationPlay[AnimationNo].FrameStart;
			int FrameNoStartRange = FrameNoStart;
			int FrameNoEndRange = FrameNoEnd;
			EditorGUILayout.LabelField("Animation Frames: " + FrameNoStart.ToString() + " to " + FrameNoEnd.ToString());
			EditorGUILayout.Space();

			string[] NameLabel = null;
			int[] IndexLabel = null;
			int[] FrameNoLabel = null;
			int LabelStart = -1;
			int LabelEnd = -1;
			if(true == FlagLabelSelectable)
			{
				CountLabel += 2;	/* +2 ... "_start" and "_end" (Reserved-Labels) */
				NameLabel = new string[CountLabel];
				IndexLabel = new int[CountLabel];
				FrameNoLabel = new int[CountLabel];

				NameLabel[0] = string.Copy(Library_SpriteStudio.AnimationInformationPlay.LabelDefaultStart);
				IndexLabel[0] = 0;
				FrameNoLabel[0] = 0;
				for(int j=1; j<(CountLabel-1); j++)
				{
					IndexLabel[j] = j;
					NameLabel[j] = string.Copy(Data.ListInformationPlay[AnimationNo].Label[j-1].Name);
					FrameNoLabel[j] = Data.ListInformationPlay[AnimationNo].Label[j-1].FrameNo;
				}
				NameLabel[CountLabel - 1] = string.Copy(Library_SpriteStudio.AnimationInformationPlay.LabelDefaultEnd);
				IndexLabel[CountLabel - 1] = CountLabel - 1;
				FrameNoLabel[CountLabel - 1] = FrameNoEnd;

				for(int j=0; j<CountLabel; j++)
				{
					if(0 == string.Compare(NameLabel[j], Data.NameLabelStart))
					{
						LabelStart = j;
					}
					if(0 == string.Compare(NameLabel[j], Data.NameLabelEnd))
					{
						LabelEnd = j;
					}
				}
				if(-1 == LabelStart)
				{
					LabelStart = 0;
				}
				if(-1 == LabelEnd)
				{
					LabelEnd = CountLabel - 1;
				}
			}
			else
			{
				CountLabel = 2;
				
				NameLabel = new string[CountLabel];
				IndexLabel = new int[CountLabel];
				FrameNoLabel = new int[CountLabel];
				
				NameLabel[0] = string.Copy(Library_SpriteStudio.AnimationInformationPlay.LabelDefaultStart);
				IndexLabel[0] = 0;
				FrameNoLabel[0] = 0;

				NameLabel[1] = string.Copy(Library_SpriteStudio.AnimationInformationPlay.LabelDefaultEnd);
				IndexLabel[1] = CountLabel - 1;
				FrameNoLabel[1] = FrameNoEnd;
			}
		
			EditorGUILayout.Space();
			if(true == FlagLabelSelectable)
			{
				int LabelOld = LabelStart;
				LabelStart = EditorGUILayout.IntPopup("Range Start Label", LabelOld, NameLabel, IndexLabel);
				if((LabelOld != LabelStart) || (true == string.IsNullOrEmpty(Data.NameLabelStart)))
				{
					Data.NameLabelStart = string.Copy(NameLabel[IndexLabel[LabelStart]]);
					FlagUpdate = true;
				}
				FrameNoStartRange = FrameNoLabel[LabelStart];
			}

			int OffsetOld = Data.OffsetFrameStart - FrameNoStart;
			int OffsetNew = EditorGUILayout.IntField("Range Start Offset: ", OffsetOld);
			if((FrameNoStartRange + OffsetNew) >= FrameNoEnd)
			{
				OffsetNew = FrameNoEnd - FrameNoStartRange - 1;
			}
			if(OffsetOld != OffsetNew)
			{
				Data.OffsetFrameStart = OffsetNew;
				FlagUpdate = true;
			}

			EditorGUILayout.LabelField(	"Range Start: (" + FrameNoStartRange.ToString()
										+ " + " 
										+ OffsetNew.ToString()
										+ ")="
										+ (FrameNoStartRange + OffsetNew).ToString()
									);
			FrameNoStartRange += OffsetNew;
			EditorGUILayout.Space();

			if(true == FlagLabelSelectable)
			{
				int LabelOld = LabelEnd;
				LabelEnd = EditorGUILayout.IntPopup("Range End Lable", LabelOld, NameLabel, IndexLabel);
				if((LabelOld != LabelEnd) || (true == string.IsNullOrEmpty(Data.NameLabelEnd)))
				{
					Data.NameLabelEnd = string.Copy(NameLabel[IndexLabel[LabelEnd]]);
					FlagUpdate = true;
				}
				FrameNoEndRange = FrameNoLabel[LabelEnd];
			}

			OffsetOld = Data.OffsetFrameEnd;
			OffsetNew = EditorGUILayout.IntField("Range End Offset", OffsetOld);
			if((FrameNoEndRange + OffsetNew) >= FrameNoEnd)
			{
				OffsetNew = 0;
			}
			if((FrameNoEndRange + OffsetNew) <= FrameNoStartRange)
			{
				OffsetNew = (FrameNoStartRange - FrameNoEndRange) + 1;
			}
			if(OffsetOld != OffsetNew)
			{
				Data.OffsetFrameEnd = OffsetNew;
				FlagUpdate = true;
			}

			EditorGUILayout.LabelField(	"Range End: (" + FrameNoEndRange.ToString()
										+ " + " 
										+ OffsetNew.ToString()
										+ ")="
										+ (FrameNoEndRange + OffsetNew).ToString()
									);
			FrameNoEndRange += OffsetNew;
			EditorGUILayout.Space();

			int FrameNoInitialOld = Data.FrameNoInitial;
			if(0 > FrameNoInitialOld)
			{
				FrameNoInitialOld = 0;
			}
			if((FrameNoEnd - FrameNoStart) < FrameNoInitialOld)
			{
				FrameNoInitialOld = FrameNoEnd - FrameNoStart;
			}
			int FrameNoInitial = EditorGUILayout.IntField("Initial Start Offset", FrameNoInitialOld);
			EditorGUILayout.LabelField(	"Valid Value Range: 0 to " + (FrameNoEndRange - FrameNoStartRange).ToString());
			if(0 > FrameNoInitial)
			{
				FrameNoInitial = 0;
			}
			if((FrameNoEndRange - FrameNoStartRange) < FrameNoInitial)
			{
				FrameNoInitial = FrameNoEndRange - FrameNoStartRange;
			}
			if(FrameNoInitialOld != FrameNoInitial)
			{
				Data.FrameNoInitial = FrameNoInitial;
				FlagUpdate = true;
			}

			EditorGUILayout.Space();
			Data.FlagStylePingpong = EditorGUILayout.Toggle("Play-Pingpong", Data.FlagStylePingpong);

			EditorGUILayout.Space();
			Data.RateTimeAnimation = EditorGUILayout.FloatField("Rate Time-Progress", Data.RateTimeAnimation);
			EditorGUILayout.LabelField("(set Negative-Value, Play Backwards.)");

			EditorGUILayout.Space();
			Data.PlayTimes = EditorGUILayout.IntField("Number of Plays", Data.PlayTimes);
			EditorGUILayout.LabelField("(1: No Loop / 0: Infinite Loop)");

			EditorGUILayout.Space();
			if(true == GUILayout.Button("Reset (Reinitialize)"))
			{
				Data.AnimationNo = 0;
				Data.FrameNoInitial = 0;
				Data.RateTimeAnimation = 1.0f;
				Data.PlayTimes = 0;
				Data.FlagStylePingpong = false;
				Data.NameLabelStart = string.Copy(Library_SpriteStudio.AnimationInformationPlay.LabelDefaultStart);
				Data.OffsetFrameStart = 0;
				Data.NameLabelEnd = string.Copy(Library_SpriteStudio.AnimationInformationPlay.LabelDefaultEnd);
				Data.OffsetFrameEnd = 0;
				FlagUpdate = true;
			}

			EditorGUI.indentLevel = LevelIndent;

			if(true == FlagUpdate)
			{
				Data.AnimationPlay();
			}
		}
		EditorGUILayout.Space();

		Data.FlagHideForce = EditorGUILayout.Toggle("Force-Hide", Data.FlagHideForce);
		if(true == GUILayout.Button("Apply \"Force-Hide\" to Children"))
		{
			LibraryEditor_SpriteStudio.Utility.HideSetForce(Data.gameObject, Data.FlagHideForce, true, false);
		}
		EditorGUILayout.Space();
	
		EditorGUILayout.Space();
		if(true == GUILayout.Button("Reset Parent-\"View\""))
		{
			Data.ViewSet();
		}

		if(true == GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}
}

