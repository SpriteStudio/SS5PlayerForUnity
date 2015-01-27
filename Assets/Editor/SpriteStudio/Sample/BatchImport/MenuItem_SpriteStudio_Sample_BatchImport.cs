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

public sealed class MenuItem_SpriteStudio_Sample_BatchImport : EditorWindow
{
	LibraryEditor_SpriteStudio_Sample_BachImport.SettingImport DataSetting;

	[MenuItem("Tools/SpriteStudio/Sample/BatchImport(sspj)")]
	static void OpenWindow()
	{
		EditorWindow.GetWindow<MenuItem_SpriteStudio_Sample_BatchImport>(true, "OPTPiX SpriteStudio BatchImport-Sample");
	}
    void OnGUI()
	{
		if(true == GUILayout.Button("Choose List-File & Import"))
		{
			string NameListFile = EditorUtility.OpenFilePanel("Select List-File(Text)", "", "txt");
			if(false == String.IsNullOrEmpty(NameListFile))
			{
				LibraryEditor_SpriteStudio_Sample_BachImport.Menu.ImportSSPJBatch(NameListFile);
				Close();
			}
		}
	}
}
