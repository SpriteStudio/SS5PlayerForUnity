/**
	SpriteStudio5 Player for Unity

	Copyright(C) Web Technology Corp.
	All rights reserved.
*/
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Script_SpriteStudio_RootEffect))]
public class Inspector_SpriteStudio_RootEffect : Editor
{
	private static bool FoldOutStaticDatas;
	private static bool FoldOutMaterialTable;
	private static bool FoldOutPlayInformation;

	public override void OnInspectorGUI()
	{
		Script_SpriteStudio_RootEffect Data = (Script_SpriteStudio_RootEffect)target;

		EditorGUILayout.LabelField("[SpriteStudio Root-Effect]");
		int LevelIndent = 0;

		EditorGUILayout.Space();
		Data.InstanceManagerDraw = (Script_SpriteStudio_ManagerDraw)(EditorGUILayout.ObjectField("View (Manager-Draw)", Data.InstanceManagerDraw, typeof(Script_SpriteStudio_ManagerDraw), true));

		EditorGUILayout.Space();
		FoldOutStaticDatas = EditorGUILayout.Foldout(FoldOutStaticDatas, "Static Datas");
		if(true == FoldOutStaticDatas)
		{
			EditorGUI.indentLevel = LevelIndent + 1;
			Data.DataCellMap = (Script_SpriteStudio_DataCell)(EditorGUILayout.ObjectField("Data:CellMap", Data.DataCellMap, typeof(Script_SpriteStudio_DataCell), true));
			Data.DataEffect = (Script_SpriteStudio_DataEffect)(EditorGUILayout.ObjectField("Data:Effect", Data.DataEffect, typeof(Script_SpriteStudio_DataEffect), true));
			EditorGUI.indentLevel = LevelIndent;
		}

		EditorGUILayout.Space();
		FoldOutMaterialTable = EditorGUILayout.Foldout(FoldOutMaterialTable, "Based-Material Table");
		if(true == FoldOutMaterialTable)
		{
			EditorGUI.indentLevel = LevelIndent + 1;
			if(null != Data.TableMaterial)
			{
				int CountShader = (int)(Library_SpriteStudio.KindColorOperation.TERMINATOR_EFFECT - 1);
				int Count = Data.TableMaterial.Length / CountShader;
				int MaterialNoTop = 0;
				Library_SpriteStudio.KindColorOperation MaterialTableNo = 0;
				string NameField = "";
				for(int i=0; i<Count; i++)
				{
					MaterialNoTop = i * (int)(Library_SpriteStudio.KindColorOperation.TERMINATOR_EFFECT - 1);
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
			EditorGUI.indentLevel = LevelIndent;

			Data.FlagHideForce = EditorGUILayout.Toggle("Hide Force", Data.FlagHideForce);
			EditorGUILayout.Space();

			int CountLimitParts = EditorGUILayout.IntField("Count Limit Parts", Data.CountLimitPartsInitial);
			EditorGUILayout.LabelField("(-1: Default-Value Set)");
			if(-1 != CountLimitParts)
			{
				if(0 > CountLimitParts)
				{
					CountLimitParts = -1;
				}
			}
			if(CountLimitParts != Data.CountLimitPartsInitial)
			{
				Data.CountLimitPartsInitial = CountLimitParts;
			}

			EditorGUI.indentLevel = LevelIndent;
		}

		if(true == GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}
	}
}
