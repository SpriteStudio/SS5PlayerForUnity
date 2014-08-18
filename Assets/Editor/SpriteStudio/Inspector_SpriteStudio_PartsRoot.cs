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

			int FrameNoEnd = Data.ListInformationPlay[Data.AnimationNo].FrameEnd - Data.ListInformationPlay[Data.AnimationNo].FrameStart;
			int FrameNoInitial = EditorGUILayout.IntField("Start Offset Frame-No", Data.FrameNoInitial);
//			int FrameNoInitial = EditorGUILayout.IntSlider("Start Offset Frame-No", Data.FrameNoInitial, 0, FrameNoEnd);
			EditorGUILayout.LabelField("(This-Value influences only at Initial)");
			EditorGUILayout.LabelField("(Don't set Negative-Value or OverRun-Value)");
			if(0 > FrameNoInitial)
			{
				FrameNoInitial = 0;
			}
			if(FrameNoEnd < FrameNoInitial)
			{
				FrameNoInitial = FrameNoEnd;
			}
			if(Data.FrameNoInitial != FrameNoInitial)
			{
				Data.FrameNoInitial = FrameNoInitial;
				FlagUpdate = true;
			}

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
				FlagUpdate = true;
			}

			EditorGUI.indentLevel = LevelIndent;

			if(true == FlagUpdate){
				Data.AnimationPlay(AnimationNo, Data.PlayTimes, -1, 0.0f);
			}
		}
		EditorGUILayout.Space();

		if(true == GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}
}

