/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp. 
	All rights reserved.
*/
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Script_SpriteStudio_Root))]
public class Inspector_SpriteStudio_Root : Editor
{
	private static bool FoldOutStaticDatas;
	private static bool FoldOutMaterialTable;
	private static bool FoldOutPlayInformation;

	public override void OnInspectorGUI()
	{
		Script_SpriteStudio_Root Data = (Script_SpriteStudio_Root)target;

		EditorGUILayout.LabelField("[SpriteStudio (Parts-)Root]");
		int LevelIndent = 0;

		EditorGUILayout.Space();
		Data.InstanceManagerDraw = (Script_SpriteStudio_ManagerDraw)(EditorGUILayout.ObjectField("View (Manager-Draw)", Data.InstanceManagerDraw, typeof(Script_SpriteStudio_ManagerDraw), true));

		EditorGUILayout.Space();
		FoldOutStaticDatas = EditorGUILayout.Foldout(FoldOutStaticDatas, "Static Datas");
		if(true == FoldOutStaticDatas)
		{
			EditorGUI.indentLevel = LevelIndent + 1;
			Data.DataCellMap = (Script_SpriteStudio_DataCell)(EditorGUILayout.ObjectField("Data:CellMap", Data.DataCellMap, typeof(Script_SpriteStudio_DataCell), true));
			Data.DataAnimation = (Script_SpriteStudio_DataAnimation)(EditorGUILayout.ObjectField("Data:Animation", Data.DataAnimation, typeof(Script_SpriteStudio_DataAnimation), true));
			EditorGUI.indentLevel = LevelIndent;
		}

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

		FoldOutPlayInformation = EditorGUILayout.Foldout(FoldOutPlayInformation, "Initial/Preview Play Setting");
		if(true == FoldOutPlayInformation)
		{
			EditorGUI.indentLevel = LevelIndent + 1;

//			if((null != Data.DataAnimation) && (null != Data.DataCellMap))
			if(null != Data.DataAnimation)
			{
				bool FlagUpdate = false;

				Data.FlagHideForce = EditorGUILayout.Toggle("Hide Force", Data.FlagHideForce);
				EditorGUILayout.Space();

				Data.FlagAnimationStopInitial = EditorGUILayout.Toggle("Initial Stopping Animation", Data.FlagAnimationStopInitial);
				EditorGUILayout.Space();

				/* "Animation" Select */
				Library_SpriteStudio.Data.Animation DataAnimationBody = null;
				int CountAnimation = Data.DataAnimation.CountGetAnimation();
				string[] ListNameAnimation = new string[CountAnimation];
				for(int i=0; i<CountAnimation; i++)
				{
					DataAnimationBody = Data.DataAnimation.DataGetAnimation(i);
					if(null != DataAnimationBody)
					{
						ListNameAnimation[i] = DataAnimationBody.Name;
					}
					else
					{
						ListNameAnimation[i] = "(Data Missing)";
					}
				}
				int IndexAnimationCursor = Data.IndexAnimation;
				if((0 > IndexAnimationCursor) || (Data.DataAnimation.ListDataAnimation.Length <= IndexAnimationCursor))
				{
					IndexAnimationCursor = 0;
					FlagUpdate |= true;
				}
				int IndexNow = EditorGUILayout.Popup("Animation Name", IndexAnimationCursor, ListNameAnimation);
				if(IndexNow != IndexAnimationCursor)
				{
					IndexAnimationCursor = IndexNow;
					Data.IndexAnimation = IndexAnimationCursor;
					FlagUpdate |= true;
				}

				DataAnimationBody = Data.DataAnimation.DataGetAnimation(IndexAnimationCursor);
				int FrameNoLast = DataAnimationBody.CountFrame - 1;
				int CountLabel = DataAnimationBody.CountGetLabel();

				EditorGUILayout.LabelField(	"- Frame Count: [" + DataAnimationBody.CountFrame.ToString() + "] (0 - " + FrameNoLast.ToString() + ")");
				EditorGUILayout.Space();

				/* "Label" Selector Create (for Start-Label / End-Label) */
				string[] ListNameLabel = null;
				int[] ListIndexLabel = null;
				int[] ListFrameNoLabel = null;
				if(0 < CountLabel)
				{	/* Has Labels */
					CountLabel += 2;	/* +2 ... "_start" and "_end" (Reserved-Labels) */
					ListNameLabel = new string[CountLabel];
					ListIndexLabel = new int[CountLabel];
					ListFrameNoLabel = new int[CountLabel];

					Library_SpriteStudio.Data.Label DataLabelBody = null;
					for(int j=1; j<(CountLabel-1); j++)
					{
						DataLabelBody = DataAnimationBody.DataGetLabel(j - 1);

						ListNameLabel[j] = DataLabelBody.Name;
						ListIndexLabel[j] = (j - 1);
						ListFrameNoLabel[j] = DataLabelBody.FrameNo;
					}
				}
				else
				{	/* Has No-Labels */
					CountLabel = 2;

					ListNameLabel = new string[CountLabel];
					ListIndexLabel = new int[CountLabel];
					ListFrameNoLabel = new int[CountLabel];
				}
				ListNameLabel[0] = Library_SpriteStudio.ListNameLabelAnimationReserved[(int)Library_SpriteStudio.KindLabelAnimationReserved.START];
				ListIndexLabel[0] = (int)(Library_SpriteStudio.KindLabelAnimationReserved.START | Library_SpriteStudio.KindLabelAnimationReserved.INDEX_RESERVED);
				ListFrameNoLabel[0] = 0;

				ListNameLabel[CountLabel - 1] = Library_SpriteStudio.ListNameLabelAnimationReserved[(int)Library_SpriteStudio.KindLabelAnimationReserved.END];
				ListIndexLabel[CountLabel - 1] = (int)(Library_SpriteStudio.KindLabelAnimationReserved.END | Library_SpriteStudio.KindLabelAnimationReserved.INDEX_RESERVED);
				ListFrameNoLabel[CountLabel - 1] = FrameNoLast;

				int IndexListLabelStart = -1;
				int IndexListLabelEnd = -1;
				for(int i=0; i<CountLabel; i++)
				{
					if(ListIndexLabel[i] == Data.IndexLabelStart)
					{
						IndexListLabelStart = i;
					}
					if(ListIndexLabel[i] == Data.IndexLabelEnd)
					{
						IndexListLabelEnd = i;
					}
				}
				if(0 > IndexListLabelStart)
				{
					IndexListLabelStart = 0;
				}
				if(0 > IndexListLabelEnd)
				{
					IndexListLabelEnd = CountLabel - 1;
				}

				int OffsetStart = Data.FrameOffsetStart;
				int OffsetEnd = Data.FrameOffsetEnd;
				int FrameNoStart = ListFrameNoLabel[IndexListLabelStart];
				int FrameNoEnd = ListFrameNoLabel[IndexListLabelEnd];
				int IndexListLabelNow;
				int FrameNoLimit;

				/* Range "Start" */
				EditorGUILayout.LabelField(	"Range Start: (" + FrameNoStart.ToString()
											+ " + "
											+ OffsetStart.ToString()
											+ ") = "
											+ (FrameNoStart + OffsetStart).ToString()
										);

				/* Start-Label Select */
				IndexListLabelNow = EditorGUILayout.Popup("Range Start Label", IndexListLabelStart, ListNameLabel);
				if(IndexListLabelNow != IndexListLabelStart)
				{	/* Data is valid & Changed Animation */
					IndexListLabelStart = IndexListLabelNow;
					FlagUpdate |= true;
				}
				Data.IndexLabelStart = ListIndexLabel[IndexListLabelStart];
				FrameNoStart = ListFrameNoLabel[IndexListLabelStart];

				/* Start-Offset */
				FrameNoLimit = FrameNoEnd + OffsetEnd;
				OffsetStart = EditorGUILayout.IntField("Range Start Offset", Data.FrameOffsetStart);
				EditorGUILayout.LabelField(	"- Valid Value Range: Min[" + (-FrameNoStart).ToString() +
											"] to Max[" + ((FrameNoLimit - FrameNoStart) - 1).ToString() + "] "
										);

				OffsetStart = (FrameNoLimit <= (FrameNoStart + OffsetStart)) ? ((FrameNoLimit - FrameNoStart) - 1) : OffsetStart;
				OffsetStart = (0 > (FrameNoStart + OffsetStart)) ? (0 - FrameNoStart) : OffsetStart;
				OffsetStart = (FrameNoLast < (FrameNoStart + OffsetStart)) ? (FrameNoLast - FrameNoStart) : OffsetStart;
				if(Data.FrameOffsetStart != OffsetStart)
				{
					Data.FrameOffsetStart = OffsetStart;
					FlagUpdate |= true;
				}
				EditorGUILayout.Space();

				/* Range "End" */
				EditorGUILayout.LabelField(	"Range End: (" + FrameNoEnd.ToString()
											+ " + "
											+ OffsetEnd.ToString()
											+ ") = "
											+ (FrameNoEnd + OffsetEnd).ToString()
										);

				/* End-Label Select */
				IndexListLabelNow = EditorGUILayout.Popup("Range End Label", IndexListLabelEnd, ListNameLabel);
				if(IndexListLabelNow != IndexListLabelEnd)
				{	/* Data is valid & Changed Animation */
					IndexListLabelEnd = IndexListLabelNow;
					FlagUpdate |= true;
				}
				Data.IndexLabelEnd = ListIndexLabel[IndexListLabelEnd];
				FrameNoEnd = ListFrameNoLabel[IndexListLabelEnd];

				/* End-Offset */
				FrameNoLimit = FrameNoStart + OffsetStart;
				OffsetEnd = EditorGUILayout.IntField("Range End Offset", Data.FrameOffsetEnd);
				EditorGUILayout.LabelField(	"- Valid Value Range: Min[" + ((FrameNoLimit - FrameNoEnd) + 1).ToString() +
											"] to Max[" + (FrameNoLast - FrameNoEnd).ToString() + "] "
										);

				OffsetEnd = (FrameNoLimit >= (FrameNoEnd + OffsetEnd)) ? ((FrameNoLimit - FrameNoEnd) + 1) : OffsetEnd;
				OffsetEnd = (0 > (FrameNoEnd + OffsetEnd)) ? (0 - FrameNoEnd) : OffsetEnd;
				OffsetEnd = (FrameNoLast < (FrameNoEnd + OffsetEnd)) ? (FrameNoLast - FrameNoEnd) : OffsetEnd;
				if(Data.FrameOffsetEnd != OffsetEnd)
				{
					Data.FrameOffsetEnd = OffsetEnd;
					FlagUpdate |= true;
				}
				EditorGUILayout.Space();

				int FrameNoRangeStart = FrameNoStart + OffsetStart;
				int FrameNoRangeEnd = FrameNoEnd + OffsetEnd;
				int CountFrameRange = FrameNoRangeEnd - FrameNoRangeStart;
				int FrameNoOffset = EditorGUILayout.IntField("Initial Start Offset", Data.FrameOffsetInitial);
				EditorGUILayout.LabelField(	"- Valid Value Range: Min[0] to Max[" + CountFrameRange.ToString() + "]");
				FrameNoOffset = (0 > FrameNoOffset) ? 0 : FrameNoOffset;
				FrameNoOffset = (CountFrameRange <= FrameNoOffset) ? CountFrameRange : FrameNoOffset;
				if(FrameNoOffset != Data.FrameOffsetInitial)
				{
					Data.FrameOffsetInitial = FrameNoOffset;
					FlagUpdate |= true;
				}

				EditorGUILayout.Space();
				bool FlagPingpongNow = EditorGUILayout.Toggle("Play-Pingpong", Data.FlagPingpong);
				if(FlagPingpongNow != Data.FlagPingpong)
				{
					Data.FlagPingpong = FlagPingpongNow;
					FlagUpdate |= true;
				}

				EditorGUILayout.Space();
				if(0.0f == Data.RateSpeedInitial)
				{
					Data.RateSpeedInitial = 1.0f;
				}
				float RateSpeedNow = EditorGUILayout.FloatField("Rate Time-Progress", Data.RateSpeedInitial);
				EditorGUILayout.LabelField("(set Negative-Value, Play Backwards.)");
				if(RateSpeedNow != Data.RateSpeedInitial)
				{
					Data.RateSpeedInitial = RateSpeedNow;
					FlagUpdate |= true;
				}

				EditorGUILayout.Space();
				int TimesPlayNow = EditorGUILayout.IntField("Number of Plays", Data.TimesPlay);
				EditorGUILayout.LabelField("(1: No Loop / 0: Infinite Loop)");
				if(TimesPlayNow != Data.TimesPlay)
				{
					Data.TimesPlay = TimesPlayNow;
					FlagUpdate |= true;
				}

				EditorGUILayout.Space();
				if(true == GUILayout.Button("Reset (Reinitialize)"))
				{
					Data.IndexAnimation = 0;
					Data.FrameOffsetInitial = 0;
					Data.RateSpeedInitial = 1.0f;
					Data.TimesPlay = 0;
					Data.FlagPingpong = false;
					Data.IndexLabelStart = (int)(Library_SpriteStudio.KindLabelAnimationReserved.START | Library_SpriteStudio.KindLabelAnimationReserved.INDEX_RESERVED);
					Data.FrameOffsetStart = 0;
					Data.IndexLabelEnd = (int)(Library_SpriteStudio.KindLabelAnimationReserved.END | Library_SpriteStudio.KindLabelAnimationReserved.INDEX_RESERVED);
					Data.FrameOffsetEnd = 0;
					Data.FlagAnimationStopInitial = false;
					FlagUpdate = true;	/* Force */
				}

				/* Re-Play Animation */
				if(true == FlagUpdate)
				{
					Data.AnimationPlay();
					if(true == Data.FlagAnimationStopInitial)
					{
						Data.AnimationStop();
					}
				}
			}
			else
			{
				EditorGUILayout.LabelField("Error[Datas Missing! (Fatal)]");
			}
		}
		EditorGUI.indentLevel = LevelIndent;
		EditorGUILayout.Space();

//		Data.FlagHideForce = EditorGUILayout.Toggle("Force-Hide", Data.FlagHideForce);
//		if(true == GUILayout.Button("Apply \"Force-Hide\" to Children"))
//		{
//			LibraryEditor_SpriteStudio.Utility.HideSetForce(Data.gameObject, Data.FlagHideForce, true, false);
//		}
//		EditorGUILayout.Space();

//		EditorGUILayout.Space();
//		if(true == GUILayout.Button("Reset Parent-\"View\""))
//		{
//			Data.ViewSet();
//		}

		if(true == GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}
}
