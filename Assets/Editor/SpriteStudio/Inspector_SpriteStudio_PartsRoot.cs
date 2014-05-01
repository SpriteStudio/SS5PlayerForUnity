/**
	SpriteStudio5 Player for Unity

	Copyright(C) 2003-2014 Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Script_SpriteStudio_PartsRoot))]
public class Inspector_SpriteStudio_PartsRoot : Editor
{
	private static readonly string[] NameDrawKind =
	{
		"Shader Setting (Initial)",
		"User Setting",
		"Back Ground",
		"Geometry",
		"Alpha Test",
		"Transparent",
		"Overlay",
//		"(TERMINATOR)"
	};

	private static bool FoldOutDrawSetting;
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
			EditorGUI.indentLevel = LevelIndent + 1;

			string[] NameAnimation = new string[Data.ListInformationPlay.Length];
			int[] IndexAnimation = new int[Data.ListInformationPlay.Length];
			for(int i=0; i<Data.ListInformationPlay.Length; i++)
			{
				IndexAnimation[i] = i;
				NameAnimation[i] = Data.ListInformationPlay[i].Name;
			}
			Data.AnimationNo = EditorGUILayout.IntPopup("Animation Name", Data.AnimationNo, NameAnimation, IndexAnimation);

			Data.FrameNoInitial = EditorGUILayout.IntField("Start Offset Frame-No", Data.FrameNoInitial);
			EditorGUILayout.LabelField("(This-Value influences only at Initial)");
			EditorGUILayout.LabelField("(Don't set Negative-Value or OverRun-Value)");
			if(0 > Data.FrameNoInitial)
			{
				Data.FrameNoInitial = 0;
			}
			int FrameNoEnd = Data.ListInformationPlay[Data.AnimationNo].FrameEnd - Data.ListInformationPlay[Data.AnimationNo].FrameStart;
			if(FrameNoEnd < Data.FrameNoInitial)
			{
				Data.FrameNoInitial = FrameNoEnd;
			}

			EditorGUILayout.Space();
			Data.RateTimeAnimation = EditorGUILayout.FloatField("Rate Time-Progress", Data.RateTimeAnimation);
			EditorGUILayout.LabelField("(set Negative-Value, Play Backwards.)");

			EditorGUILayout.Space();
			Data.CountLoopRemain = EditorGUILayout.IntField("Loop Count", Data.CountLoopRemain);
			EditorGUILayout.LabelField("(0: No Loop / -1: Infinity)");
			if(-2 > Data.CountLoopRemain)
			{
				Data.CountLoopRemain = -1;
			}

			EditorGUILayout.Space();
			if(true == GUILayout.Button("Reset (Reinitialize)"))
			{
				Data.AnimationNo = 0;
				Data.FrameNoInitial = 0;
				Data.RateTimeAnimation = 1.0f;
				Data.CountLoopRemain = -1;
			}

			EditorGUI.indentLevel = LevelIndent;
		}
		EditorGUILayout.Space();

		FoldOutDrawSetting = EditorGUILayout.Foldout(FoldOutDrawSetting, "Rendering Setting");
		if(true == FoldOutDrawSetting)
		{
			EditorGUI.indentLevel = LevelIndent + 1;

			int CountKindQueue = (int)Script_SpriteStudio_PartsRoot.KindDrawQueue.OVERLAY + 1;
			int[] IndexDrawKind = new int[CountKindQueue];
			for(int i=0; i<CountKindQueue; i++)
			{
				IndexDrawKind[i] = i;
			}
			int KindRenderQueueBaseNo = (int)Data.KindRenderQueueBase;
			KindRenderQueueBaseNo = EditorGUILayout.IntPopup("Render-Queue Base", KindRenderQueueBaseNo, NameDrawKind, IndexDrawKind);
			Data.KindRenderQueueBase = (Script_SpriteStudio_PartsRoot.KindDrawQueue)KindRenderQueueBaseNo;
			EditorGUI.indentLevel = LevelIndent + 2;
			EditorGUILayout.LabelField("Details [" + NameDrawKind[KindRenderQueueBaseNo] + "]");
			switch((Script_SpriteStudio_PartsRoot.KindDrawQueue)KindRenderQueueBaseNo)
			{
				case Script_SpriteStudio_PartsRoot.KindDrawQueue.SHADER_SETTING:
					EditorGUILayout.LabelField("- Value Base: Defined Tag\"Queue\" in Shader (Default: Transparent)");
					EditorGUILayout.LabelField("- Offset Range: Depend on Tag\"Queue\"(Default: 0-999)");
					break;

				case Script_SpriteStudio_PartsRoot.KindDrawQueue.USER_SETTING:
					EditorGUILayout.LabelField("- Value Base: 0");
					EditorGUILayout.LabelField("- Offset Range: 1000-4999");
					break;

				default:
					EditorGUILayout.LabelField("- Value Base: " + Script_SpriteStudio_PartsRoot.ValueKindDrawQueue[KindRenderQueueBaseNo]);
					EditorGUILayout.LabelField("- Offset Range: 0-" + (Script_SpriteStudio_PartsRoot.ValueKindDrawQueue[KindRenderQueueBaseNo+1] - Script_SpriteStudio_PartsRoot.ValueKindDrawQueue[KindRenderQueueBaseNo]-1));
					break;
			}
			EditorGUI.indentLevel = LevelIndent + 1;

			EditorGUILayout.Space();
			Data.OffsetDrawQueue = EditorGUILayout.IntField("Render-Queue Offset", Data.OffsetDrawQueue);

			EditorGUILayout.Space();
			Data.RateDrawQueueEffectZ = EditorGUILayout.FloatField("Rate Z Effect", Data.RateDrawQueueEffectZ);
			EditorGUILayout.LabelField("(\"This-Value x ViewPort-Z\" Added to Offset)");

			EditorGUI.indentLevel = LevelIndent;
		}
		EditorGUILayout.Space();

		if(true == GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}
}

